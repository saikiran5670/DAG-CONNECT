﻿using System;
using System.Collections.Generic;
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
using net.atos.daf.ct2.portalservice.Entity.Vehicle;
using Newtonsoft.Json;
using VehicleBusinessService = net.atos.daf.ct2.vehicleservice;
namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("vehicle")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class VehicleController : BaseController
    {
        private readonly VehicleBusinessService.VehicleService.VehicleServiceClient _vehicleClient;
        private readonly Mapper _mapper;

        private readonly ILog _logger;
        private readonly string _fk_Constraint = "violates foreign key constraint";
        private readonly string _socketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        private readonly AuditHelper _auditHelper;

        public VehicleController(VehicleBusinessService.VehicleService.VehicleServiceClient vehicleClient, AuditHelper auditHelper, IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper) : base(httpContextAccessor, sessionHelper)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _vehicleClient = vehicleClient;
            _mapper = new Mapper();
            _auditHelper = auditHelper;
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update(VehicleRequest request)
        {
            try
            {
                _logger.Info("Update method in vehicle API called.");
                // Validation 
                if (request.ID <= 0)
                {
                    return StatusCode(400, "The VehicleId is required.");
                }

                if (request.Organization_Id <= 0)
                {
                    return StatusCode(400, "The organization id is required.");
                }
                //Assign context orgId
                request.Organization_Id = GetContextOrgId();
                var vehicleRequest = new VehicleBusinessService.VehicleRequest();
                vehicleRequest = _mapper.ToVehicle(request);
                vehicleRequest.UserOrgId = GetUserSelectedOrgId();
                var featureIds = GetMappedFeatureIdByStartWithName(net.atos.daf.ct2.portalservice.Entity.Alert.AlertConstants.ALERT_FEATURE_STARTWITH);
                Metadata headers = new Metadata();
                headers.Add("logged_in_accid", Convert.ToString(_userDetails.AccountId));
                headers.Add("report_feature_ids", JsonConvert.SerializeObject(featureIds));
                VehicleBusinessService.VehicleResponce vehicleResponse = await _vehicleClient.UpdateAsync(vehicleRequest, headers);

                if (vehicleResponse.Code == VehicleBusinessService.Responcecode.Failed
                     && vehicleResponse.Message == "There is an error updating vehicle.")
                {
                    return StatusCode(500, "There is an error creating account.");
                }
                else if (vehicleResponse.Code == VehicleBusinessService.Responcecode.Conflict)
                {
                    return StatusCode(409, vehicleResponse.Message);
                }
                else if (vehicleResponse.Code == VehicleBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Vehicle Component",
                  "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                  "Update  method in Vehicle controller", request.ID, request.ID, JsonConvert.SerializeObject(request),
                   _userDetails);

                    var response = _mapper.ToVehicle(vehicleResponse.Vehicle);
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, "vehicleResponse is null");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Vehicle Component",
                 "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Update  method in Vehicle controller", request.ID, request.ID, JsonConvert.SerializeObject(request),
                  _userDetails);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> Get([FromQuery] VehicleFilterRequest vehicleFilter)
        {
            try
            {
                _logger.Info("Get method in vehicle API called.");
                //Assign context orgId               
                vehicleFilter.OrganizationId = GetContextOrgId();
                var vehicleFilterRequest = _mapper.ToVehicleFilter(vehicleFilter);
                Metadata headers = new Metadata();
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                headers.Add("accountId", Convert.ToString(_userDetails.AccountId));

                VehicleBusinessService.VehicleListResponce vehicleListResponse = await _vehicleClient.GetAsync(vehicleFilterRequest, headers);
                List<VehicleResponse> response = new List<VehicleResponse>();
                response = _mapper.ToVehicles(vehicleListResponse);

                if (vehicleListResponse != null && vehicleListResponse.Code == VehicleBusinessService.Responcecode.Success)
                {
                    if (vehicleListResponse.Vehicles != null && vehicleListResponse.Vehicles.Count > 0)
                    {
                        return Ok(vehicleListResponse.Vehicles);
                    }
                    else
                    {
                        return StatusCode(404, "vehicle details are not found.");
                    }
                }
                else
                {
                    return StatusCode(500, vehicleListResponse.Message);
                }

            }

            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPost]
        [Route("group/create")]
        public async Task<IActionResult> CreateGroup(VehicleGroupRequest group)
        {
            try
            {
                _logger.Info("Create Group method in vehicle API called.");


                if (string.IsNullOrEmpty(group.Name) || group.Name == "string")
                {
                    return StatusCode(401, PortalConstants.VehicleValidation.CREATE_REQUIRED);
                }

                // Length validation
                if ((group.Name.Length > 50) || (group.Description.Length > 100))
                {
                    return StatusCode(400, PortalConstants.VehicleValidation.INVALID_DATA);
                }

                char groupType = Convert.ToChar(group.GroupType);
                if (!EnumValidator.ValidateGroupType(groupType))
                {
                    return StatusCode(400, PortalConstants.VehicleValidation.INVALID_GROUP_TYPE);
                }

                //Assign context orgId
                group.OrganizationId = GetContextOrgId();

                VehicleBusinessService.VehicleGroupRequest accountGroupRequest = new VehicleBusinessService.VehicleGroupRequest();


                accountGroupRequest = _mapper.ToVehicleGroup(group);
                VehicleBusinessService.VehicleGroupResponce response = await _vehicleClient.CreateGroupAsync(accountGroupRequest);
                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Vehicle Component",
                   "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                   "CreateGroup  method in Vehicle controller", 0, response.VehicleGroup.Id, JsonConvert.SerializeObject(group),
                    _userDetails);
                    return Ok(_mapper.ToVehicleGroup(response));
                }
                else if (response != null && response.Code == VehicleBusinessService.Responcecode.Conflict)
                {
                    return StatusCode(409, "Duplicate Vehicle Group.");
                }
                else
                {
                    return StatusCode(500, "VehicleGroupResponce is empty " + response.Message);
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Vehicle Component",
                      "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                      "CreateGroup  method in Vehicle controller", 0, 0, JsonConvert.SerializeObject(group), _userDetails);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPut]
        [Route("group/update")]
        public async Task<IActionResult> UpdateGroup(VehicleGroupRequest group)
        {
            try
            {
                // Fetch Feature Ids of the alert for visibility
                var featureIds = GetMappedFeatureIdByStartWithName(VehcileConstants.VEHICLE_FEATURE_STARTWITH);
                Metadata headers = new Metadata();
                headers.Add("report_feature_ids", JsonConvert.SerializeObject(featureIds));
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                headers.Add("logged_in_accId", Convert.ToString(_userDetails.AccountId));

                _logger.Info("Update Group method in vehicle API called.");

                if (group.Id == 0)
                {
                    return StatusCode(401, "invalid Vehicle Group Id: The Vehicle group id is Empty.");
                }

                if (string.IsNullOrEmpty(group.Name) || group.Name == "string")
                {
                    return StatusCode(401, PortalConstants.VehicleValidation.CREATE_REQUIRED);
                }

                // Length validation
                if ((group.Name.Length > 50) || (group.Description.Length > 100))
                {
                    return StatusCode(400, PortalConstants.VehicleValidation.INVALID_DATA);
                }

                //Assign context orgId
                group.OrganizationId = GetContextOrgId();

                char groupType = Convert.ToChar(group.GroupType);
                if (!EnumValidator.ValidateGroupType(groupType))
                {
                    return StatusCode(400, PortalConstants.VehicleValidation.INVALID_GROUP_TYPE);
                }


                VehicleBusinessService.VehicleGroupRequest vehicleGroupRequest = new VehicleBusinessService.VehicleGroupRequest();
                vehicleGroupRequest = _mapper.ToVehicleGroup(group);
                VehicleBusinessService.VehicleGroupResponce response = await _vehicleClient.UpdateGroupAsync(vehicleGroupRequest, headers);
                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Vehicle Component",
                     "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                     "UpdateGroup  method in Vehicle controller", group.Id, group.Id, JsonConvert.SerializeObject(group), _userDetails);
                    return Ok(_mapper.ToVehicleGroup(response));
                }
                else if (response != null && response.Code == VehicleBusinessService.Responcecode.Conflict)
                {
                    return StatusCode(409, "Duplicate Vehicle Group.");
                }
                else
                {
                    return StatusCode(500, "VehicleGroupResponce is null " + response.Message);
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Vehicle Component",
                    "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                    "UpdateGroup  method in Vehicle controller", group.Id, group.Id, JsonConvert.SerializeObject(group), _userDetails);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpGet]
        [Route("group/candelete")]
        public async Task<IActionResult> CanDelete(long groupId)
        {
            VehicleBusinessService.VehicleGroupIdRequest request = new VehicleBusinessService.VehicleGroupIdRequest();
            try
            {
                // Fetch Feature Ids of the alert for visibility
                var featureIds = GetMappedFeatureIdByStartWithName(VehcileConstants.VEHICLE_FEATURE_STARTWITH);
                Metadata headers = new Metadata();
                headers.Add("report_feature_ids", JsonConvert.SerializeObject(featureIds));
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                headers.Add("logged_in_accId", Convert.ToString(_userDetails.AccountId));

                _logger.Info("Can delete Group method in vehicle API called.");

                if ((Convert.ToInt32(groupId) <= 0))
                {
                    return StatusCode(400, "The vehicle group id is required.");
                }
                //Add the organizationId for Vehicle group CDC ogranization filter
                request.OrganizationId = GetContextOrgId();
                request.GroupId = Convert.ToInt32(groupId);
                VehicleBusinessService.VehicleGroupDeleteResponce response = await _vehicleClient.CanDeleteGroupAsync(request, headers);
                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Vehicle Component",
                  "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                  "CanDelete method in Vehicle controller", Convert.ToInt32(groupId), Convert.ToInt32(groupId), JsonConvert.SerializeObject(request), _userDetails);
                    return Ok(response.Result);
                }
                else
                {
                    return StatusCode(500, response.Message);
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Vehicle Component",
                "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                "CanDelete method in Vehicle controller", Convert.ToInt32(groupId), Convert.ToInt32(groupId), JsonConvert.SerializeObject(request), _userDetails);
                _logger.Error(null, ex);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpDelete]
        [Route("group/delete")]
        public async Task<IActionResult> DeleteGroup(long groupId)
        {
            VehicleBusinessService.VehicleGroupIdRequest request = new VehicleBusinessService.VehicleGroupIdRequest();
            try
            {
                // Fetch Feature Ids of the alert for visibility
                var featureIds = GetMappedFeatureIdByStartWithName(VehcileConstants.VEHICLE_FEATURE_STARTWITH);
                Metadata headers = new Metadata();
                headers.Add("report_feature_ids", JsonConvert.SerializeObject(featureIds));
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                headers.Add("logged_in_accId", Convert.ToString(_userDetails.AccountId));

                _logger.Info("Delete Group method in vehicle API called.");

                if ((Convert.ToInt32(groupId) <= 0))
                {
                    return StatusCode(400, "The vehicle group id is required.");
                }
                //Add the organizationId for Vehicle group CDC ogranization filter
                request.OrganizationId = GetContextOrgId();
                request.GroupId = Convert.ToInt32(groupId);
                VehicleBusinessService.VehicleGroupDeleteModifiedResponce response = await _vehicleClient.DeleteGroupAsync(request, headers);
                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Vehicle Component",
                  "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                  "DeleteGroup method in Vehicle controller", Convert.ToInt32(groupId), Convert.ToInt32(groupId), JsonConvert.SerializeObject(request), _userDetails);
                    return Ok(new { isDeleted = response.IsDeleted, CanDelete = response.CanDelete });
                }
                else
                {
                    return StatusCode(500, "VehicleGroupResponce is null " + response.Message);
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Vehicle Component",
                "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                "DeleteGroup method in Vehicle controller", Convert.ToInt32(groupId), Convert.ToInt32(groupId), JsonConvert.SerializeObject(request), _userDetails);
                _logger.Error(null, ex);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("group/getgroupdetails")]
        public async Task<IActionResult> GetGroupDetails(VehicleGroupFilterRequest groupFilter)
        {
            try
            {
                _logger.Info("Get Group detais method in vehicle API called.");
                //Assign context orgId
                groupFilter.OrganizationId = GetContextOrgId();
                VehicleBusinessService.GroupFilterRequest VehicleGroupRequest = new VehicleBusinessService.GroupFilterRequest();
                VehicleGroupRequest = _mapper.ToVehicleGroupFilter(groupFilter);
                VehicleBusinessService.VehicleGroupRefResponce response = await _vehicleClient.GetGroupDetailsAsync(VehicleGroupRequest);

                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    if (response.GroupRefDetails != null && response.GroupRefDetails.Count > 0)
                    {
                        return Ok(response.GroupRefDetails);
                    }
                    else
                    {
                        return StatusCode(404, "vehicle details are found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


        [HttpGet]
        [Route("group/getvehiclelist")]
        public async Task<IActionResult> GetVehiclesByVehicleGroup([FromQuery] int groupId)
        {
            try
            {
                _logger.Info("Get vehicle list by group id method in vehicle API called.");

                if (Convert.ToInt32(groupId) <= 0)
                {
                    return StatusCode(401, "invalid Vehicle Group Id: The Vehicle group id is Empty.");
                }


                VehicleBusinessService.VehicleGroupIdRequest vehicleGroupIdRequest = new VehicleBusinessService.VehicleGroupIdRequest();
                vehicleGroupIdRequest.GroupId = groupId;

                VehicleBusinessService.VehicleGroupRefResponce response = await _vehicleClient.GetVehiclesByVehicleGroupAsync(vehicleGroupIdRequest);


                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    if (response.GroupRefDetails != null && response.GroupRefDetails.Count > 0)
                    {
                        return Ok(response.GroupRefDetails);
                    }
                    else
                    {
                        return StatusCode(404, "Vehicle details are not found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Internal Server Error.");
            }
        }


        [HttpGet]
        [Route("organization/get")]
        public async Task<IActionResult> GetOrganizationVehicleGroupdetails([FromQuery] long OrganizationId)
        {
            try
            {
                _logger.Info("Get vehicle list by group id method in vehicle API called.");

                if (Convert.ToInt32(OrganizationId) <= 0)
                {
                    return StatusCode(401, "invalid organization ID: The organization Id is Empty.");
                }
                //Assign context orgId
                OrganizationId = GetContextOrgId();

                VehicleBusinessService.OrganizationIdRequest OrganizationIdRequest = new VehicleBusinessService.OrganizationIdRequest();
                OrganizationIdRequest.OrganizationId = Convert.ToInt32(OrganizationId);
                VehicleBusinessService.OrgVehicleGroupListResponse response = await _vehicleClient.GetOrganizationVehicleGroupdetailsAsync(OrganizationIdRequest);


                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    if (response.OrgVehicleGroupList != null && response.OrgVehicleGroupList.Count > 0)
                    {
                        return Ok(response.OrgVehicleGroupList);
                    }
                    else
                    {
                        return StatusCode(404, "vehicle details are found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


        [HttpGet]
        [Route("getGroup")]
        public async Task<IActionResult> GetVehicleGroup([FromQuery] int OrganizationId, int VehicleId)
        {
            try
            {
                _logger.Info("Get vehicle group list by orgnization & vehicle id method in vehicle API called.");

                if (Convert.ToInt32(OrganizationId) <= 0)
                {
                    return StatusCode(401, "invalid organization ID: The organization Id is Empty.");
                }
                if (Convert.ToInt32(VehicleId) <= 0)
                {
                    return StatusCode(401, "invalid vehicle ID: The vehicle Id is Empty.");
                }

                //Assign context orgId
                OrganizationId = GetContextOrgId();

                VehicleBusinessService.OrgvehicleIdRequest orgvehicleIdRequest = new VehicleBusinessService.OrgvehicleIdRequest();
                orgvehicleIdRequest.OrganizationId = Convert.ToInt32(OrganizationId);
                orgvehicleIdRequest.VehicleId = Convert.ToInt32(VehicleId);
                VehicleBusinessService.VehicleGroupDetailsResponse response = await _vehicleClient.GetVehicleGroupAsync(orgvehicleIdRequest);


                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    if (response.VehicleGroups != null && response.VehicleGroups.Count > 0)
                    {
                        return Ok(response.VehicleGroups);
                    }
                    else
                    {
                        return StatusCode(404, "vehicle group details are found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("group/getvehicles")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "It has to be fixed while clean up of Organization Id related code")]
        public async Task<IActionResult> GetVehiclesByAccountGroup([FromQuery] int AccountGroupId, int Organization_Id)
        {
            try
            {
                _logger.Info("Get vehicle list by group id method in vehicle API called.");

                if (AccountGroupId == 0)
                {
                    return StatusCode(401, "invalid Account Group Id: The Account group id is Empty.");
                }
                //Assign context orgId
                Organization_Id = GetContextOrgId();

                VehicleBusinessService.OrgAccountIdRequest orgAccountIdRequest = new VehicleBusinessService.OrgAccountIdRequest();
                orgAccountIdRequest.OrganizationId = Convert.ToInt32(Organization_Id);
                orgAccountIdRequest.AccountGroupId = Convert.ToInt32(AccountGroupId);
                VehicleBusinessService.VehicleGroupRefResponce response = await _vehicleClient.GetVehiclesByAccountGroupAsync(orgAccountIdRequest);


                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    if (response.GroupRefDetails != null && response.GroupRefDetails.Count > 0)
                    {
                        return Ok(response.GroupRefDetails);
                    }
                    else
                    {
                        return StatusCode(404, "vehicle details are not found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPut]
        [Route("setoptinstatus")]
        public async Task<IActionResult> SetOptInStatus(VehicleBusinessService.VehicleOptInRequest request)
        {
            try
            {
                _logger.Info("Create method in vehicle API called.");

                // Validation 
                if (request.VehicleId <= 0)
                {
                    return StatusCode(400, "The VehicleId is required.");
                }
                request.OrganizationId = GetUserSelectedOrgId();
                request.OrgContextId = GetContextOrgId();
                VehicleBusinessService.VehicleGroupDeleteResponce vehicleResponse = await _vehicleClient.SetOptInStatusAsync(request);

                if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Failed
                     && vehicleResponse.Message == "There is an error updating vehicle opt in status.")
                {
                    return StatusCode(500, "There is an error updating vehicle opt in status.");
                }
                else if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Vehicle Component",
                "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                "SetOptInStatus  method in Vehicle controller", request.VehicleId, request.VehicleId, JsonConvert.SerializeObject(request), _userDetails);

                    return Ok(vehicleResponse.Result);
                }
                else
                {
                    return StatusCode(500, "vehicleResponse is null");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Vehicle Component",
              "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
              "SetOptInStatus  method in Vehicle controller", request.VehicleId, request.VehicleId, JsonConvert.SerializeObject(request), _userDetails);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPut]
        [Route("setotastatus")]
        public async Task<IActionResult> SetOTAStatus(VehicleBusinessService.VehicleOtaRequest request)
        {
            try
            {
                _logger.Info("Create method in vehicle API called.");

                // Validation 
                if (request.VehicleId <= 0)
                {
                    return StatusCode(400, "The VehicleId is required.");
                }
                request.OrganizationId = GetUserSelectedOrgId();
                request.OrgContextId = GetContextOrgId();
                VehicleBusinessService.VehicleGroupDeleteResponce vehicleResponse = await _vehicleClient.SetOTAStatusAsync(request);

                if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Failed
                     && vehicleResponse.Message == "There is an error updating vehicle ota status.")
                {
                    return StatusCode(500, "There is an error updating vehicle ota status.");
                }
                else if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Vehicle Component",
            "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
            "SetOTAStatus  method in Vehicle controller", request.VehicleId, request.VehicleId, JsonConvert.SerializeObject(request), _userDetails);
                    return Ok(vehicleResponse.Result);
                }
                else
                {
                    return StatusCode(500, "vehicleResponse is null");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Vehicle Component",
          "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
          "SetOTAStatus  method in Vehicle controller", request.VehicleId, request.VehicleId, JsonConvert.SerializeObject(request), _userDetails);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPut]
        [Route("terminate")]
        public async Task<IActionResult> Terminate(VehicleBusinessService.VehicleTerminateRequest request)
        {
            try
            {
                _logger.Info("Create method in vehicle API called.");

                // Validation 
                if (request.VehicleId <= 0)
                {
                    return StatusCode(400, "The VehicleId is required.");
                }
                request.OrganizationId = GetUserSelectedOrgId();
                request.OrgContextId = GetContextOrgId();
                VehicleBusinessService.VehicleGroupDeleteResponce vehicleResponse = await _vehicleClient.TerminateAsync(request);

                if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Failed
                     && vehicleResponse.Message == "There is an error updating vehicle terminate status.")
                {
                    return StatusCode(500, "There is an error updating vehicle terminate status.");
                }
                else if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Success)
                {

                    await _auditHelper.AddLogs(DateTime.Now, "Vehicle Component",
       "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
       "Terminate  method in Vehicle controller", request.VehicleId, request.VehicleId, JsonConvert.SerializeObject(request), _userDetails);
                    return Ok(vehicleResponse.Result);
                }
                else
                {
                    return StatusCode(500, "vehicleResponse is null");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Vehicle Component",
   "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
   "Terminate  method in Vehicle controller", request.VehicleId, request.VehicleId, JsonConvert.SerializeObject(request), _userDetails);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("getvehicle")]
        public async Task<IActionResult> GetVehicle([FromQuery] int vehicleId)
        {
            try
            {
                _logger.Info("Get method in vehicle API called.");

                VehicleBusinessService.VehicleIdRequest Vid = new VehicleBusinessService.VehicleIdRequest();
                Vid.VehicleId = vehicleId;
                VehicleBusinessService.VehicleDetailsResponce vehicleListResponse = await _vehicleClient.GetVehicleAsync(Vid);

                if (vehicleListResponse != null && vehicleListResponse.Code == VehicleBusinessService.Responcecode.Success)
                {
                    if (vehicleListResponse.Vehicle != null)
                    {
                        return Ok(vehicleListResponse.Vehicle);
                    }
                    else
                    {
                        return StatusCode(404, "vehicle details are found.");
                    }
                }
                else
                {
                    return StatusCode(500, vehicleListResponse.Message);
                }

            }

            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("group/getvehiclegrouplist")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "It has to be fixed while clean up of Organization Id related code")]
        public async Task<IActionResult> GetGroupDetailsWithVehicleCount([FromQuery] int organizationId)
        {
            try
            {
                _logger.Info("Get Group detais method in vehicle API called.");

                //Assign context orgId
                organizationId = GetContextOrgId();

                VehicleBusinessService.VehicleGroupLandingRequest vehicleGroupRequest = new VehicleBusinessService.VehicleGroupLandingRequest();
                vehicleGroupRequest = _mapper.ToVehicleGroupLandingFilter(organizationId);
                VehicleBusinessService.VehicleGroupLandingResponse response = await _vehicleClient.GetVehicleGroupWithVehCountAsync(vehicleGroupRequest);

                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    if (response.VehicleGroupLandingDetails != null && response.VehicleGroupLandingDetails.Count > 0)
                    {
                        return Ok(response.VehicleGroupLandingDetails);
                    }
                    else
                    {
                        return StatusCode(404, "vehicle details are found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


        [HttpGet]
        [Route("group/getvehiclesDetails")]
        public async Task<IActionResult> GetVehiclesByGroup([FromQuery] DynamicVehicleGroupRequest dynamicVehicleGroupRequest)
        {
            try
            {
                _logger.Info("Get vehicle list by group id method in vehicle API called.");

                //Assign context orgId
                dynamicVehicleGroupRequest.OrganizationId = GetContextOrgId();

                if (dynamicVehicleGroupRequest.GroupType != null ? EnumValidator.ValidateGroupType(Convert.ToChar(dynamicVehicleGroupRequest.GroupType)) : false)
                {

                    if (Convert.ToInt32(dynamicVehicleGroupRequest.GroupId) <= 0 && Convert.ToChar(dynamicVehicleGroupRequest.GroupType.ToLower().Trim()) == 'g')
                    {
                        return StatusCode(401, PortalConstants.VehicleValidation.GROUP_ID_REQUIRED);
                    }
                    else
                    {
                        if (Convert.ToChar(dynamicVehicleGroupRequest.GroupType.ToLower().Trim()) == 'g')
                        {

                            VehicleBusinessService.VehicleGroupIdRequest vehicleGroupIdRequest = new VehicleBusinessService.VehicleGroupIdRequest();
                            vehicleGroupIdRequest.GroupId = dynamicVehicleGroupRequest.GroupId;
                            VehicleBusinessService.VehicleGroupRefResponce response = await _vehicleClient.GetVehiclesByVehicleGroupAsync(vehicleGroupIdRequest);

                            if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                            {
                                if (response.GroupRefDetails != null && response.GroupRefDetails.Count > 0)
                                {
                                    return Ok(response.GroupRefDetails);
                                }
                                else
                                {
                                    return StatusCode(404, "vehicle details are not found.");
                                }
                            }
                            else
                            {
                                return StatusCode(500, response.Message);
                            }
                        }
                    }

                    if (Convert.ToInt32(dynamicVehicleGroupRequest.OrganizationId) <= 0 && Convert.ToChar(dynamicVehicleGroupRequest.GroupType.ToLower().Trim()) == 'd')
                    {
                        return StatusCode(401, PortalConstants.VehicleValidation.ORGANIZATION_ID_REQUIRED);
                    }
                    else
                    {
                        char functionenumType;
                        if (!string.IsNullOrEmpty(dynamicVehicleGroupRequest.FunctionEnum))
                        {
                            functionenumType = Convert.ToChar(dynamicVehicleGroupRequest.FunctionEnum);
                        }
                        else
                        {
                            return StatusCode(401, PortalConstants.VehicleValidation.FUNCTION_TYPE_REQUIRED);
                        }

                        if (EnumValidator.ValidateFunctionEnumType(functionenumType))
                        {
                            VehicleBusinessService.VehicleListResponce response = null;

                            if (Convert.ToChar(dynamicVehicleGroupRequest.GroupType.ToLower().Trim()) == 'd' && Convert.ToChar(dynamicVehicleGroupRequest.FunctionEnum.ToLower().Trim()) == 'v')
                            {
                                VehicleBusinessService.DynamicGroupFilterRequest dynamicVehicleGroupIdRequest = new VehicleBusinessService.DynamicGroupFilterRequest();
                                dynamicVehicleGroupIdRequest.OrganizationId = dynamicVehicleGroupRequest.OrganizationId;
                                dynamicVehicleGroupIdRequest.VehicleGroupId = dynamicVehicleGroupRequest.GroupId;
                                dynamicVehicleGroupIdRequest.RelationShipId = dynamicVehicleGroupRequest.RelationShipId;

                                response = await _vehicleClient.GetDynamicVisibleVehicleAsync(dynamicVehicleGroupIdRequest);
                            }
                            else if (Convert.ToChar(dynamicVehicleGroupRequest.GroupType.ToLower().Trim()) == 'd' && Convert.ToChar(dynamicVehicleGroupRequest.FunctionEnum.ToLower().Trim()) == 'o')
                            {
                                VehicleBusinessService.DynamicGroupFilterRequest dynamicVehicleGroupIdRequest = new VehicleBusinessService.DynamicGroupFilterRequest();
                                dynamicVehicleGroupIdRequest.OrganizationId = dynamicVehicleGroupRequest.OrganizationId;
                                dynamicVehicleGroupIdRequest.VehicleGroupId = dynamicVehicleGroupRequest.GroupId;
                                dynamicVehicleGroupIdRequest.RelationShipId = dynamicVehicleGroupRequest.RelationShipId;

                                response = await _vehicleClient.GetDynamicOwnedVehicleAsync(dynamicVehicleGroupIdRequest);
                            }
                            else
                            {
                                VehicleBusinessService.DynamicGroupFilterRequest dynamicVehicleGroupIdRequest = new VehicleBusinessService.DynamicGroupFilterRequest();
                                dynamicVehicleGroupIdRequest.OrganizationId = dynamicVehicleGroupRequest.OrganizationId;
                                dynamicVehicleGroupIdRequest.VehicleGroupId = dynamicVehicleGroupRequest.GroupId;
                                dynamicVehicleGroupIdRequest.RelationShipId = dynamicVehicleGroupRequest.RelationShipId;

                                response = await _vehicleClient.GetDynamicAllVehicleAsync(dynamicVehicleGroupIdRequest);
                            }

                            if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                            {
                                if (response.Vehicles != null && response.Vehicles.Count > 0)
                                {
                                    return Ok(response.Vehicles);
                                }
                                else
                                {
                                    return StatusCode(404, "vehicle details are not found.");
                                }
                            }
                            else
                            {
                                return StatusCode(500, response.Message);
                            }
                        }
                        else
                        {
                            return StatusCode(401, PortalConstants.VehicleValidation.INVALID_FUNCTION_ENUM_TYPE);
                        }
                    }
                }
                else
                {
                    return StatusCode(401, PortalConstants.VehicleValidation.INVALID_GROUP_TYPE);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("GetRelationshipVehicles")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "It has to be fixed while clean up of Organization Id related code")]
        public async Task<IActionResult> GetRelationshipVehicles([FromQuery] int organizationId)
        {
            try
            {
                _logger.Info("GetRelationshipVehicles method in vehicle API called.");

                //Assign context orgId
                organizationId = GetContextOrgId();

                VehicleBusinessService.OrgvehicleIdRequest orgvehicleIdRequest = new VehicleBusinessService.OrgvehicleIdRequest();
                orgvehicleIdRequest.OrganizationId = organizationId;
                orgvehicleIdRequest.AccountId = _userDetails.AccountId;

                var adminRightsFeatureId = GetMappedFeatureIdByStartWithName("Admin#Admin")?.FirstOrDefault() ?? 0;

                Metadata headers = new Metadata();
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                headers.Add("admin_rights_featureId", Convert.ToString(adminRightsFeatureId));

                VehicleBusinessService.VehiclesResponse vehiclesResponse = await _vehicleClient.GetRelationshipVehiclesAsync(orgvehicleIdRequest, headers);
                List<VehicleManagementResponse> vehicles = _mapper.ToVehicles(vehiclesResponse);

                if (vehiclesResponse != null && vehiclesResponse.Code == VehicleBusinessService.Responcecode.Success)
                {
                    if (vehiclesResponse.Vehicles != null && vehiclesResponse.Vehicles.Count > 0)
                    {
                        return Ok(vehicles);
                    }
                    else
                    {
                        return StatusCode(404, "vehicle details are not found.");
                    }
                }
                else
                {
                    return StatusCode(500, vehiclesResponse.Message);
                }

            }

            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("GetVehicleAssociatedGroups")]
        public async Task<IActionResult> GetVehicleAssociatedGroups([FromQuery] int vehicleId)
        {
            try
            {
                _logger.Info("GetVehicleAssociatedGroups method in vehicle API called.");

                VehicleBusinessService.GetVehicleAssociatedGroupResponse response
                    = await _vehicleClient.GetVehicleAssociatedGroupsAsync(new VehicleBusinessService.GetVehicleAssociatedGroupRequest { VehicleId = vehicleId });

                if (response != null && response.Code == VehicleBusinessService.Responcecode.Success)
                {
                    return Ok(response.Groups);
                }
                else
                {
                    return StatusCode(500, response.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpGet]
        [Route("getvehiclebysubscriptionid")]
        public async Task<IActionResult> GetVehicleBySubscriptionId([FromQuery] int subscriptionId, string state)
        {
            try
            {
                _logger.Info("GetVehicleBySubscriptionId method in vehicle API called.");

                if (subscriptionId <= 0 ||
                    string.IsNullOrEmpty(state) ||
                    (!string.IsNullOrEmpty(state) && state.Length != 1) ||
                    (!string.IsNullOrEmpty(state) && !new string[] { "A", "I" }.Contains(state)))
                {
                    return StatusCode(400, string.Empty);
                }

                VehicleBusinessService.SubscriptionIdRequest vid = new VehicleBusinessService.SubscriptionIdRequest();
                vid.SubscriptionId = subscriptionId;
                vid.State = state;
                var response = await _vehicleClient.GetVehicleBySubscriptionIdAsync(vid);

                if (response != null)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, string.Empty);
                }

            }

            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpPut]
        [Route("updatevehicleconnection")]
        public async Task<IActionResult> UpdateVehicleConnection(List<VehicleConnectionSettings> request)

        {

            try
            {
                _logger.Info("Connect_All method in vehicle API called.");

                // Validation
                if (request.Count <= 0)
                {
                    return StatusCode(400, "The vehicle settings data is required.");
                }
                var vehicleSettings = _mapper.ToUpdateVehicleConnection(request);
                vehicleSettings.OrganizationId = GetUserSelectedOrgId();
                vehicleSettings.OrgContextId = GetContextOrgId();
                var featureIds = GetMappedFeatureIdByStartWithName(net.atos.daf.ct2.portalservice.Entity.Alert.AlertConstants.ALERT_FEATURE_STARTWITH);
                Metadata headers = new Metadata();
                headers.Add("logged_in_accid", Convert.ToString(_userDetails.AccountId));
                headers.Add("report_feature_ids", JsonConvert.SerializeObject(featureIds));
                VehicleBusinessService.VehicleConnectResponse vehicleConnectResponse = _vehicleClient.UpdateVehicleConnection(vehicleSettings, headers);
                if (vehicleConnectResponse != null && vehicleConnectResponse.Code == VehicleBusinessService.Responcecode.Failed
                     && vehicleConnectResponse.Message == "There is an error updating vehicle opt in status.")
                {
                    return StatusCode(500, "There is an error updating vehicle opt in status.");
                }
                else if (vehicleConnectResponse != null && vehicleConnectResponse.Code == VehicleBusinessService.Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Vehicle Component",
                "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                "Connect_All method in Vehicle controller", 0, 0, JsonConvert.SerializeObject(request), _userDetails);

                    return Ok(vehicleConnectResponse);
                }
                else
                {
                    return StatusCode(500, "vehicleResponse is null");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Vehicle Component",
              "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
              "Connect_All  method in Vehicle controller", 0, 0, JsonConvert.SerializeObject(request), _userDetails);
                _logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(_socketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
    }

}
