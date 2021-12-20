﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.ReportScheduler;
using net.atos.daf.ct2.reportschedulerservice;
using net.atos.daf.ct2.vehicleservice;
using Newtonsoft.Json;
using PortalAlertEntity = net.atos.daf.ct2.portalservice.Entity.ReportScheduler;
namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("reportscheduler")]
    public class ReportSchedulerController : BaseController
    {
        private readonly ILog _logger;
        private readonly ReportSchedulerService.ReportSchedulerServiceClient _reportschedulerClient;
        private readonly AuditHelper _auditHelper;
        private readonly Entity.ReportScheduler.Mapper _mapper;
        private readonly VehicleService.VehicleServiceClient _vehicleClient;
        public ReportSchedulerController(ReportSchedulerService.ReportSchedulerServiceClient reportschedulerClient, VehicleService.VehicleServiceClient vehicleClient, AuditHelper auditHelper, IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper) : base(httpContextAccessor, sessionHelper)
        {
            _reportschedulerClient = reportschedulerClient;
            _auditHelper = auditHelper;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _mapper = new Entity.ReportScheduler.Mapper();
            _vehicleClient = vehicleClient;
        }

        #region Get Report Scheduler Paramenter
        [HttpGet]
        [Route("getreportschedulerparameter")]
        public async Task<IActionResult> GetReportSchedulerParameter(int accountId, int orgnizationid)
        {
            try
            {
                // Fetch Feature Id of the report for visibility
                var featureId = GetMappedFeatureId(HttpContext.Request.Path.Value.ToLower());

                int contextorgid = GetContextOrgId();
                int roleid = _userDetails.RoleId;
                accountId = _userDetails.AccountId;
                orgnizationid = GetUserSelectedOrgId();
                await _auditHelper.AddLogs(DateTime.Now, ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME,
                 ReportSchedulerConstants.REPORTSCHEDULER_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                "GetReportSchedulerParameter", 1, 2, Convert.ToString(accountId),
                  _userDetails);

                Metadata headers = new Metadata();
                headers.Add("report_feature_id", Convert.ToString(featureId));

                ReportParameterResponse response = await _reportschedulerClient.GetReportParameterAsync(new ReportParameterRequest { AccountId = accountId, OrganizationId = orgnizationid, RoleId = roleid, ContextOrgId = contextorgid }, headers);

                if (response == null)
                    return StatusCode(500, ReportSchedulerConstants.REPORTSCHEDULER_INTERNEL_SERVER_ISSUE);
                if (response.Code == ResponseCode.Success)
                    return Ok(response);
                if (response.Code == ResponseCode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(ReportSchedulerConstants.REPORTSCHEDULER_PARAMETER_NOT_FOUND_MSG, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME,
                 ReportSchedulerConstants.REPORTSCHEDULER_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                string.Format(ReportSchedulerConstants.REPORTSCHEDULER_EXCEPTION_LOG_MSG, "GetReportSchedulerParameter", ex.Message), 1, 2, Convert.ToString(accountId),
                  _userDetails);
                _logger.Error($"{nameof(GetReportSchedulerParameter)}: With Error:-", ex);
                return StatusCode(500, ReportSchedulerConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion


        #region Create Schedular Report
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateReportScheduler(PortalAlertEntity.ReportScheduler request)
        {
            try
            {
                request.OrganizationId = GetContextOrgId();
                if (request.ScheduledReportVehicleRef.Count > 0)
                {
                    //Condition if vehicle select All and group select All
                    if (request.ScheduledReportVehicleRef[0].VehicleGroupId == 0 && request.ScheduledReportVehicleRef[0].VehicleId == 0)
                    {
                        var scheduledReportVehicleRef = request.ScheduledReportVehicleRef;
                        request.ScheduledReportVehicleRef = new List<ScheduledReportVehicleRef>();

                        Metadata headers = new Metadata();
                        headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));

                        VehicleandVehicleGroupIdResponse vehicleandVehicleGroupId = await _reportschedulerClient.GetVehicleandVehicleGroupIdAsync(new ReportParameterRequest { AccountId = request.CreatedBy, OrganizationId = request.OrganizationId, ReportId = request.ReportId }, headers);
                        if (vehicleandVehicleGroupId.VehicleIdList.Count > 0)
                        {
                            foreach (var item in vehicleandVehicleGroupId.VehicleIdList)
                            {
                                ScheduledReportVehicleRef objScheduledReportVehicleRef = new ScheduledReportVehicleRef();
                                var vehicleGroupRequest = new vehicleservice.VehicleGroupRequest();
                                vehicleGroupRequest.Name = string.Format(ReportSchedulerConstants.VEHICLE_GROUP_NAME, request.OrganizationId.ToString(), request.Id.ToString());
                                if (vehicleGroupRequest.Name.Length > 50) vehicleGroupRequest.Name = vehicleGroupRequest.Name.Substring(0, 49);
                                vehicleGroupRequest.GroupType = "S";
                                vehicleGroupRequest.RefId = item.VehicleId;
                                vehicleGroupRequest.FunctionEnum = "N";
                                vehicleGroupRequest.OrganizationId = request.OrganizationId;
                                vehicleGroupRequest.Description = string.Format(ReportSchedulerConstants.VEHICLE_GROUP_NAME, request.Id, request.OrganizationId);
                                vehicleservice.VehicleGroupResponce response = await _vehicleClient.CreateGroupAsync(vehicleGroupRequest);
                                objScheduledReportVehicleRef.VehicleGroupId = response.VehicleGroup.Id;
                                objScheduledReportVehicleRef.CreatedAt = scheduledReportVehicleRef[0].CreatedAt;
                                objScheduledReportVehicleRef.CreatedBy = scheduledReportVehicleRef[0].CreatedBy;
                                objScheduledReportVehicleRef.ScheduleReportId = scheduledReportVehicleRef[0].ScheduleReportId;
                                objScheduledReportVehicleRef.State = scheduledReportVehicleRef[0].State;
                                request.ScheduledReportVehicleRef.Add(objScheduledReportVehicleRef);
                            }
                        }
                        if (vehicleandVehicleGroupId.VehicleGroupIdList.Count > 0)
                        {
                            foreach (var item in vehicleandVehicleGroupId.VehicleGroupIdList)
                            {
                                ScheduledReportVehicleRef objScheduledReportVehicleRef = new ScheduledReportVehicleRef();
                                objScheduledReportVehicleRef.VehicleGroupId = item.VehicleGroupId;
                                objScheduledReportVehicleRef.CreatedAt = scheduledReportVehicleRef[0].CreatedAt;
                                objScheduledReportVehicleRef.CreatedBy = scheduledReportVehicleRef[0].CreatedBy;
                                objScheduledReportVehicleRef.ScheduleReportId = scheduledReportVehicleRef[0].ScheduleReportId;
                                objScheduledReportVehicleRef.State = scheduledReportVehicleRef[0].State;
                                request.ScheduledReportVehicleRef.Add(objScheduledReportVehicleRef);
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in request.ScheduledReportVehicleRef)
                        {
                            if ((item.VehicleGroupId == 0 && item.VehicleId > 0) || (item.VehicleGroupId >= 0 && item.VehicleId > 0))
                            {
                                var vehicleGroupRequest = new vehicleservice.VehicleGroupRequest();
                                vehicleGroupRequest.Name = string.Format(ReportSchedulerConstants.VEHICLE_GROUP_NAME, request.OrganizationId.ToString(), request.Id.ToString());
                                if (vehicleGroupRequest.Name.Length > 50) vehicleGroupRequest.Name = vehicleGroupRequest.Name.Substring(0, 49);
                                vehicleGroupRequest.GroupType = "S";
                                vehicleGroupRequest.RefId = item.VehicleId;
                                vehicleGroupRequest.FunctionEnum = "N";
                                vehicleGroupRequest.OrganizationId = request.OrganizationId;
                                vehicleGroupRequest.Description = string.Format(ReportSchedulerConstants.VEHICLE_GROUP_NAME, request.Id, request.OrganizationId);
                                vehicleservice.VehicleGroupResponce response = await _vehicleClient.CreateGroupAsync(vehicleGroupRequest);
                                item.VehicleGroupId = response.VehicleGroup.Id;
                            }
                        }
                    }
                }
                ReportSchedulerRequest reportSchedulerRequest = _mapper.ToReportSchedulerEntity(request);
                ReportSchedulerResponse reportSchedulerResponse = new ReportSchedulerResponse();
                reportSchedulerResponse = await _reportschedulerClient.CreateReportSchedulerAsync(reportSchedulerRequest);

                if (reportSchedulerResponse != null && reportSchedulerResponse.Code == ResponseCode.Failed)
                {
                    return StatusCode(500, ReportSchedulerConstants.REPORTSCHEDULER_CREATE_FAILED_MSG);
                }
                else if (reportSchedulerResponse != null && reportSchedulerResponse.Code == ResponseCode.Conflict)
                {
                    return StatusCode(409, reportSchedulerResponse.Message);
                }
                else if (reportSchedulerResponse != null && reportSchedulerResponse.Code == ResponseCode.Success)
                {
                    return Ok(reportSchedulerResponse);
                }
                else
                {
                    return StatusCode(500, ReportSchedulerConstants.REPORTSCHEDULER_CREATE_FAILED_MSG);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME,
                  ReportSchedulerConstants.REPORTSCHEDULER_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 string.Format(ReportSchedulerConstants.REPORTSCHEDULER_EXCEPTION_LOG_MSG, "CreateReportScheduler", ex.Message), 1, 2, "1",
                   _userDetails);
                _logger.Error($"{nameof(CreateReportScheduler)}: With Error:-", ex);
                return StatusCode(500, ReportSchedulerConstants.INTERNAL_SERVER_MSG);
            }
        }

        #endregion

        #region Update Schedular Report
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateReportScheduler(PortalAlertEntity.ReportScheduler request)
        {
            try
            {
                request.OrganizationId = GetContextOrgId();
                if (request.ScheduledReportVehicleRef.Count > 0)
                {
                    //Condition if vehicle select All and group select All
                    if (request.ScheduledReportVehicleRef[0].VehicleGroupId == 0 && request.ScheduledReportVehicleRef[0].VehicleId == 0)
                    {
                        var scheduledReportVehicleRef = request.ScheduledReportVehicleRef;
                        request.ScheduledReportVehicleRef = new List<ScheduledReportVehicleRef>();
                        VehicleandVehicleGroupIdResponse vehicleandVehicleGroupId = await _reportschedulerClient.GetVehicleandVehicleGroupIdAsync(new ReportParameterRequest { AccountId = request.CreatedBy, OrganizationId = request.OrganizationId });
                        if (vehicleandVehicleGroupId.VehicleIdList.Count > 0)
                        {
                            foreach (var item in vehicleandVehicleGroupId.VehicleIdList)
                            {
                                ScheduledReportVehicleRef objScheduledReportVehicleRef = new ScheduledReportVehicleRef();
                                var vehicleGroupRequest = new vehicleservice.VehicleGroupRequest();
                                vehicleGroupRequest.Name = string.Format(ReportSchedulerConstants.VEHICLE_GROUP_NAME, request.OrganizationId.ToString(), request.Id.ToString());
                                if (vehicleGroupRequest.Name.Length > 50) vehicleGroupRequest.Name = vehicleGroupRequest.Name.Substring(0, 49);
                                vehicleGroupRequest.GroupType = "S";
                                vehicleGroupRequest.RefId = scheduledReportVehicleRef[0].VehicleGroupId;
                                vehicleGroupRequest.FunctionEnum = "N";
                                vehicleGroupRequest.OrganizationId = GetContextOrgId();
                                vehicleGroupRequest.Description = string.Format(ReportSchedulerConstants.VEHICLE_GROUP_NAME, request.Id, request.OrganizationId);
                                vehicleservice.VehicleGroupResponce response = await _vehicleClient.CreateGroupAsync(vehicleGroupRequest);
                                objScheduledReportVehicleRef.VehicleGroupId = response.VehicleGroup.Id;
                                objScheduledReportVehicleRef.CreatedAt = scheduledReportVehicleRef[0].CreatedAt;
                                objScheduledReportVehicleRef.CreatedBy = scheduledReportVehicleRef[0].CreatedBy;
                                objScheduledReportVehicleRef.ScheduleReportId = scheduledReportVehicleRef[0].ScheduleReportId;
                                objScheduledReportVehicleRef.State = scheduledReportVehicleRef[0].State;
                                request.ScheduledReportVehicleRef.Add(objScheduledReportVehicleRef);
                            }
                        }
                        if (vehicleandVehicleGroupId.VehicleGroupIdList.Count > 0)
                        {
                            foreach (var item in vehicleandVehicleGroupId.VehicleGroupIdList)
                            {
                                ScheduledReportVehicleRef objScheduledReportVehicleRef = new ScheduledReportVehicleRef();
                                objScheduledReportVehicleRef.VehicleGroupId = item.VehicleGroupId;
                                objScheduledReportVehicleRef.CreatedAt = scheduledReportVehicleRef[0].CreatedAt;
                                objScheduledReportVehicleRef.CreatedBy = scheduledReportVehicleRef[0].CreatedBy;
                                objScheduledReportVehicleRef.ScheduleReportId = scheduledReportVehicleRef[0].ScheduleReportId;
                                objScheduledReportVehicleRef.State = scheduledReportVehicleRef[0].State;
                                request.ScheduledReportVehicleRef.Add(objScheduledReportVehicleRef);
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in request.ScheduledReportVehicleRef)
                        {
                            if ((item.VehicleGroupId == 0 && item.VehicleId > 0) || (item.VehicleGroupId >= 0 && item.VehicleId > 0))
                            {
                                var vehicleGroupRequest = new vehicleservice.VehicleGroupRequest();
                                vehicleGroupRequest.Name = string.Format(ReportSchedulerConstants.VEHICLE_GROUP_NAME, request.OrganizationId.ToString(), request.Id.ToString());
                                if (vehicleGroupRequest.Name.Length > 50) vehicleGroupRequest.Name = vehicleGroupRequest.Name.Substring(0, 49);
                                vehicleGroupRequest.GroupType = "S";
                                vehicleGroupRequest.RefId = item.VehicleGroupId;
                                vehicleGroupRequest.FunctionEnum = "N";
                                vehicleGroupRequest.OrganizationId = GetContextOrgId();
                                vehicleGroupRequest.Description = string.Format(ReportSchedulerConstants.VEHICLE_GROUP_NAME, request.Id, request.OrganizationId);
                                vehicleservice.VehicleGroupResponce response = await _vehicleClient.CreateGroupAsync(vehicleGroupRequest);
                                item.VehicleGroupId = response.VehicleGroup.Id;
                            }
                        }
                    }

                }
                ReportSchedulerRequest reportSchedulerRequest = _mapper.ToReportSchedulerEntity(request);
                ReportSchedulerResponse reportSchedulerResponse = new ReportSchedulerResponse();
                reportSchedulerResponse = await _reportschedulerClient.UpdateReportSchedulerAsync(reportSchedulerRequest);

                if (reportSchedulerResponse != null && reportSchedulerResponse.Code == ResponseCode.Failed)
                {
                    return StatusCode(500, ReportSchedulerConstants.REPORTSCHEDULER_UPDATE_FAILED_MSG);
                }
                else if (reportSchedulerResponse != null && reportSchedulerResponse.Code == ResponseCode.Conflict)
                {
                    return StatusCode(409, reportSchedulerResponse.Message);
                }
                else if (reportSchedulerResponse != null && reportSchedulerResponse.Code == ResponseCode.Success)
                {
                    return Ok(reportSchedulerResponse);
                }
                else
                {
                    return StatusCode(500, ReportSchedulerConstants.REPORTSCHEDULER_UPDATE_FAILED_MSG);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME,
                  ReportSchedulerConstants.REPORTSCHEDULER_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 string.Format(ReportSchedulerConstants.REPORTSCHEDULER_EXCEPTION_LOG_MSG, "UpdateReportScheduler", ex.Message), 1, 2, "1",
                   _userDetails);
                _logger.Error($"{nameof(UpdateReportScheduler)}: With Error:-", ex);
                return StatusCode(500, ReportSchedulerConstants.INTERNAL_SERVER_MSG);
            }
        }

        #endregion

        #region Get Report Scheduler
        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetReportScheduler(int accountId, int orgnizationid)
        {
            try
            {
                if (orgnizationid == 0) return BadRequest(ReportSchedulerConstants.REPORTSCHEDULER_ORG_ID_NOT_NULL_MSG);

                // Fetch Feature Id of the report for visibility
                var featureId = GetMappedFeatureId(HttpContext.Request.Path.Value.ToLower());
                int roleid = _userDetails.RoleId;
                int contextorgid = GetContextOrgId();
                accountId = _userDetails.AccountId;
                orgnizationid = GetUserSelectedOrgId();
                Metadata headers = new Metadata();
                headers.Add("report_feature_id", Convert.ToString(featureId));
                ReportSchedulerListResponse response = await _reportschedulerClient.GetReportSchedulerListAsync(new ReportParameterRequest { AccountId = accountId, OrganizationId = orgnizationid, RoleId = roleid, ContextOrgId = contextorgid }, headers);
                //if (response.ReportSchedulerRequest.Any())
                //{
                //    foreach (var item in response.ReportSchedulerRequest)
                //    {
                //        if (item.ScheduledReportVehicleRef.Any())
                //        {
                //            foreach (var vehicle in item.ScheduledReportVehicleRef)
                //            {
                //                if (vehicle.VehicleGroupId > 0 && vehicle.VehicleGroupType != "S")
                //                {
                //                    VehicleCountFilterRequest vehicleRequest = new VehicleCountFilterRequest();
                //                    vehicleRequest.VehicleGroupId = vehicle.VehicleGroupId;
                //                    vehicleRequest.GroupType = vehicle.VehicleGroupType;
                //                    vehicleRequest.FunctionEnum = vehicle.FunctionEnum;
                //                    vehicleRequest.OrgnizationId = orgnizationid;
                //                    VehicleCountFilterResponse vehicleResponse = await _vehicleClient.GetVehicleAssociatedGroupCountAsync(vehicleRequest);
                //                    vehicle.VehicleCount = vehicleResponse.VehicleCount;
                //                }
                //            }
                //        }
                //    }
                //}

                if (response == null)
                    return StatusCode(500, ReportSchedulerConstants.REPORTSCHEDULER_INTERNEL_SERVER_ISSUE);
                if (response.Code == ResponseCode.Success)
                    return Ok(response);
                if (response.Code == ResponseCode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(ReportSchedulerConstants.REPORTSCHEDULER_DATA_NOT_FOUND_MSG, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME,
                 ReportSchedulerConstants.REPORTSCHEDULER_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                string.Format(ReportSchedulerConstants.REPORTSCHEDULER_EXCEPTION_LOG_MSG, "GetReportScheduler", ex.Message), 1, 2, Convert.ToString(accountId),
                  _userDetails);
                _logger.Error($"{nameof(GetReportScheduler)}: With Error:-", ex);
                return StatusCode(500, ReportSchedulerConstants.INTERNAL_SERVER_MSG);
            }
        }

        [HttpGet]
        [Route("getscheduledreport")]
        public async Task<IActionResult> GetScheduledReport(int reportSchedulerId)
        {
            try
            {
                if (reportSchedulerId == 0) return BadRequest(ReportSchedulerConstants.REPORTSCHEDULER_ID_NOT_NULL_MSG);

                ScheduledReportResponse response = await _reportschedulerClient.GetScheduledReportAsync(new ScheduledResponseIdRequest { ReportSchedulerId = reportSchedulerId });

                if (response == null)
                    return StatusCode(500, ReportSchedulerConstants.REPORTSCHEDULER_INTERNEL_SERVER_ISSUE);
                if (response.Code == ResponseCode.Success)
                    return Ok(response);
                if (response.Code == ResponseCode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(ReportSchedulerConstants.REPORTSCHEDULER_DATA_NOT_FOUND_MSG, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME,
                 ReportSchedulerConstants.REPORTSCHEDULER_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                string.Format(ReportSchedulerConstants.REPORTSCHEDULER_EXCEPTION_LOG_MSG, "GetScheduledReport", ex.Message), 1, 2, Convert.ToString(_userDetails.AccountId),
                  _userDetails);
                _logger.Error($"{nameof(GetScheduledReport)}: With Error:-", ex);
                return StatusCode(500, ReportSchedulerConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion


        #region DeleteReportSchedule
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteReportSchedule([FromQuery] Entity.ReportScheduler.ReportStatusByIdModel request)
        {
            try
            {
                net.atos.daf.ct2.reportschedulerservice.ReportStatusUpdateDeleteRequest obj = new net.atos.daf.ct2.reportschedulerservice.ReportStatusUpdateDeleteRequest();
                obj.ReportId = request.ReportId;
                obj.OrganizationId = GetContextOrgId();
                if (obj.OrganizationId <= 0)
                {
                    return StatusCode(400, ReportSchedulerConstants.REPORTSCHEDULER_ORG_ID_NOT_NULL_MSG);
                }
                var data = await _reportschedulerClient.DeleteReportScheduleAsync(obj);
                if (data == null)
                {
                    return StatusCode(404);
                }

                switch (data.Code)
                {
                    case ResponseCode.Success:
                        await _auditHelper.AddLogs(DateTime.Now, ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME,
                                        ReportSchedulerConstants.REPORTSCHEDULER_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                        string.Format("DeleteReportSchedule method in {0}", ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME), 0, 0, JsonConvert.SerializeObject(request),
                                         _userDetails);
                        return Ok(data);
                    case ResponseCode.Failed:
                        return StatusCode(400, data.Message);
                    default:
                        return StatusCode(500, data.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME,
                                         ReportSchedulerConstants.REPORTSCHEDULER_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         string.Format("DeleteReportSchedule method in {0}", ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME), 0, 0, JsonConvert.SerializeObject(request),
                                          _userDetails);
                _logger.Error($"{nameof(DeleteReportSchedule)}: With Error:-", ex);
                return StatusCode(500, ReportSchedulerConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion

        #region EnableDisableReportSchedule
        [HttpPost]
        [Route("EnableDisable")]
        public async Task<IActionResult> EnableDisableReportSchedule(Entity.ReportScheduler.ReportStatusEnableDisableModel request)
        {
            try
            {
                net.atos.daf.ct2.reportschedulerservice.ReportStatusUpdateDeleteRequest obj = new net.atos.daf.ct2.reportschedulerservice.ReportStatusUpdateDeleteRequest();
                obj.OrganizationId = GetContextOrgId();
                if (obj.OrganizationId <= 0)
                {
                    return BadRequest(ReportSchedulerConstants.REPORTSCHEDULER_ORG_ID_NOT_NULL_MSG);
                }
                obj.ReportId = request.ReportId;
                obj.Status = request.Status;
                var data = await _reportschedulerClient.EnableDisableReportScheduleAsync(obj);
                if (data == null)
                {
                    return StatusCode(404);
                }

                switch (data.Code)
                {
                    case ResponseCode.Success:
                        await _auditHelper.AddLogs(DateTime.Now, ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME,
                                        ReportSchedulerConstants.REPORTSCHEDULER_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                        string.Format("EnableDisableReportSchedule method in {0}", ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME), 0, 0, JsonConvert.SerializeObject(request),
                                         _userDetails);
                        return Ok(data);
                    case ResponseCode.Failed:
                        return StatusCode(400, data.Message);
                    default:
                        return StatusCode(500, data.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME,
                                         ReportSchedulerConstants.REPORTSCHEDULER_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         string.Format("EnableDisableReportSchedule method in {0}", ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME), 0, 0, JsonConvert.SerializeObject(request),
                                          _userDetails);
                _logger.Error($"{nameof(EnableDisableReportSchedule)}: With Error:-", ex);
                return StatusCode(500, ReportSchedulerConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion

        #region GetPDFBinaryFormatByToken
        [AllowAnonymous]
        [HttpGet]
        [Route("download")]
        public async Task<IActionResult> GetPDFBinaryFormatByToken([FromQuery] Entity.ReportScheduler.ReportStatusByTokenModel request)
        {
            try
            {
                ReportPDFByTokenRequest objReportPDFByTokenRequest = new ReportPDFByTokenRequest();
                objReportPDFByTokenRequest.Token = request.Token;
                var data = await _reportschedulerClient.GetPDFBinaryFormatByTokenAsync(objReportPDFByTokenRequest);
                if (data == null)
                    return StatusCode(500, ReportSchedulerConstants.REPORTSCHEDULER_INTERNEL_SERVER_ISSUE);
                if (data.Code == ResponseCode.Success)
                {
                    return Ok(data);
                    //if (data.Id > 0)
                    //{
                    //    var pdfStreamResult = new MemoryStream();
                    //    pdfStreamResult.Write(data.Report.ToByteArray(), 0, data.Report.Length);
                    //    pdfStreamResult.Position = 0;
                    //    string filename = data.FileName + ".pdf";
                    //    return File(pdfStreamResult, "application/pdf", filename);
                    //}
                    //else
                    //{
                    //    return StatusCode((int)data.Code, data.Message);
                    //}
                }
                if (data.Code == ResponseCode.InternalServerError)
                    return StatusCode((int)data.Code, String.Format(ReportSchedulerConstants.REPORTSCHEDULER_PARAMETER_NOT_FOUND_MSG, data.Message));
                return StatusCode((int)data.Code, data.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME,
                 ReportSchedulerConstants.REPORTSCHEDULER_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                string.Format(ReportSchedulerConstants.REPORTSCHEDULER_EXCEPTION_LOG_MSG, "GetPDFBinaryFormatByToken", ex.Message), 1, 2, JsonConvert.SerializeObject(request),
                  _userDetails);
                _logger.Error($"{nameof(GetPDFBinaryFormatByToken)}: With Error:-", ex);
                return StatusCode(500, ReportSchedulerConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion

        #region UnSubscribeById
        [AllowAnonymous]
        [HttpGet]
        [Route("UnSubscribe")]
        public async Task<IActionResult> UnSubscribe([FromQuery] UnSubscribeRequest request)
        {
            try
            {
                if (request.RecipentId == 0) return StatusCode(404, ReportSchedulerConstants.REPORTSCHEDULER_RECIPENT_ID_NOT_FOUND);
                if (string.IsNullOrEmpty(request.EmailId)) return StatusCode(404, ReportSchedulerConstants.REPORTSCHEDULER_EMAIL_ID_NOT_FOUND);
                var data = await _reportschedulerClient.UnSubscribeByIdAsync(request);
                if (data == null)
                    return StatusCode(500, ReportSchedulerConstants.REPORTSCHEDULER_INTERNEL_SERVER_ISSUE);
                if (data.Code == ResponseCode.Success)
                {
                    return Ok(data);
                }
                if (data.Code == ResponseCode.InternalServerError)
                    return StatusCode((int)data.Code, String.Format(ReportSchedulerConstants.REPORTSCHEDULER_INTERNEL_SERVER_ISSUE_2, data.Message));
                return StatusCode((int)data.Code, data.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME,
                 ReportSchedulerConstants.REPORTSCHEDULER_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                string.Format(ReportSchedulerConstants.REPORTSCHEDULER_EXCEPTION_LOG_MSG, "UnSubscribe", ex.Message), 1, 2, JsonConvert.SerializeObject(request),
                  _userDetails);
                _logger.Error($"{nameof(UnSubscribe)}: With Error:-", ex);
                return StatusCode(500, ReportSchedulerConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion

        #region UnSubscribeById
        [AllowAnonymous]
        [HttpGet]
        [Route("Unsubscribeall")]
        public async Task<IActionResult> UnSubscribeAll([FromQuery] UnSubscribeAllRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.EmailId)) return StatusCode(404, ReportSchedulerConstants.REPORTSCHEDULER_EMAIL_ID_NOT_FOUND);
                var data = await _reportschedulerClient.UnSubscribeAllByEmailIdAsync(request);
                if (data == null)
                    return StatusCode(500, ReportSchedulerConstants.REPORTSCHEDULER_INTERNEL_SERVER_ISSUE);
                if (data.Code == ResponseCode.Success)
                {
                    return Ok(data);
                }
                if (data.Code == ResponseCode.InternalServerError)
                    return StatusCode((int)data.Code, String.Format(ReportSchedulerConstants.REPORTSCHEDULER_INTERNEL_SERVER_ISSUE_2, data.Message));
                return StatusCode((int)data.Code, data.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME,
                 ReportSchedulerConstants.REPORTSCHEDULER_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                string.Format(ReportSchedulerConstants.REPORTSCHEDULER_EXCEPTION_LOG_MSG, "UnSubscribe", ex.Message), 1, 2, JsonConvert.SerializeObject(request),
                  _userDetails);
                _logger.Error($"{nameof(UnSubscribeAll)}: With Error:-", ex);
                return StatusCode(500, ReportSchedulerConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion

        #region GetPDFBinaryFormatById
        [HttpGet]
        [Route("getpdf")]
        public async Task<IActionResult> GetPDFBinaryFormatById([FromQuery] Entity.ReportScheduler.ReportStatusByIdModel request)
        {
            try
            {
                ReportPDFByIdRequest objReportPDFByIdRequest = new ReportPDFByIdRequest();
                objReportPDFByIdRequest.ReportId = request.ReportId;
                var data = await _reportschedulerClient.GetPDFBinaryFormatByIdAsync(objReportPDFByIdRequest);
                if (data == null)
                    return StatusCode(500, ReportSchedulerConstants.REPORTSCHEDULER_INTERNEL_SERVER_ISSUE);
                if (data.Code == ResponseCode.Success)
                    return Ok(data);
                if (data.Code == ResponseCode.InternalServerError)
                    return StatusCode((int)data.Code, String.Format(ReportSchedulerConstants.REPORTSCHEDULER_PARAMETER_NOT_FOUND_MSG, data.Message));
                return StatusCode((int)data.Code, data.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, ReportSchedulerConstants.REPORTSCHEDULER_CONTROLLER_NAME,
                 ReportSchedulerConstants.REPORTSCHEDULER_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                string.Format(ReportSchedulerConstants.REPORTSCHEDULER_EXCEPTION_LOG_MSG, "GetPDFBinaryFormatById", ex.Message), 1, 2, JsonConvert.SerializeObject(request),
                  _userDetails);
                _logger.Error($"{nameof(GetPDFBinaryFormatById)}: With Error:-", ex);
                return StatusCode(500, ReportSchedulerConstants.INTERNAL_SERVER_MSG);
            }
        }
        #endregion
    }
}
