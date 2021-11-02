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
using net.atos.daf.ct2.alertservice;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Alert;
using net.atos.daf.ct2.vehicleservice;
using Newtonsoft.Json;
using PortalAlertEntity = net.atos.daf.ct2.portalservice.Entity.Alert;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("alert")]
    public class AlertController : BaseController
    {
        private readonly ILog _logger;
        private readonly AlertService.AlertServiceClient _alertServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly Entity.Alert.Mapper _mapper;
        private readonly VehicleService.VehicleServiceClient _vehicleClient;

        public AlertController(AlertService.AlertServiceClient alertServiceClient,
                               AuditHelper auditHelper,
                               VehicleService.VehicleServiceClient vehicleClient,
                               IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper) : base(httpContextAccessor, sessionHelper)
        {
            _alertServiceClient = alertServiceClient;
            _auditHelper = auditHelper;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _vehicleClient = vehicleClient;
            _mapper = new Entity.Alert.Mapper();
        }

        #region ActivateAlert,SuspendAlert and  DeleteAlert

        [HttpPut]
        [Route("activatealert")]
        public async Task<IActionResult> ActivateAlert(int alertId)
        {
            try
            {
                if (alertId == 0) return BadRequest(AlertConstants.ALERT_ID_NON_ZERO_MSG);
                var response = await _alertServiceClient.ActivateAlertAsync(new IdRequest { AlertId = alertId });
                if (response == null)
                    return StatusCode(500, String.Format(AlertConstants.INTERNAL_SERVER_ERROR_MSG, 1));
                if (response.Code == ResponseCode.Success)
                    return Ok(String.Format(AlertConstants.ACTIVATED_ALERT_SUCCESS_MSG, alertId));
                if (response.Code == ResponseCode.Failed)
                    return StatusCode((int)response.Code, String.Format(AlertConstants.ACTIVATED_ALERT_FAILURE_MSG, alertId, AlertConstants.ALERT_FAILURE_MSG));
                if (response.Code == ResponseCode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(AlertConstants.ACTIVATED_ALERT_FAILURE_MSG, alertId, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, AlertConstants.ALERT_CONTROLLER_NAME,
                 AlertConstants.ALERT_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 string.Format(AlertConstants.ALERT_EXCEPTION_LOG_MSG, "ActivateAlert", ex.Message), 1, 2, Convert.ToString(alertId),
                  _userDetails);
                // check for fk violation
                if (ex.Message.Contains(AlertConstants.SOCKET_EXCEPTION_MSG))
                {
                    return StatusCode(500, String.Format(AlertConstants.INTERNAL_SERVER_ERROR_MSG, 2));
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPut]
        [Route("suspendalert")]
        public async Task<IActionResult> SuspendAlert(int alertId)
        {
            try
            {
                if (alertId == 0) return BadRequest(AlertConstants.ALERT_ID_NON_ZERO_MSG);
                var response = await _alertServiceClient.SuspendAlertAsync(new IdRequest { AlertId = alertId });
                if (response == null)
                    return StatusCode(500, String.Format(AlertConstants.INTERNAL_SERVER_ERROR_MSG, 1));
                if (response.Code == ResponseCode.Success)
                    return Ok(String.Format(AlertConstants.SUSPEND_ALERT_SUCCESS_MSG, alertId));
                if (response.Code == ResponseCode.Failed)
                    return StatusCode((int)response.Code, String.Format(AlertConstants.SUSPEND_ALERT_FAILURE_MSG, alertId, AlertConstants.ALERT_FAILURE_MSG));
                if (response.Code == ResponseCode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(AlertConstants.SUSPEND_ALERT_FAILURE_MSG, alertId, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, AlertConstants.ALERT_CONTROLLER_NAME,
                 AlertConstants.ALERT_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 string.Format(AlertConstants.ALERT_EXCEPTION_LOG_MSG, "SuspendAlert", ex.Message), 1, 2, Convert.ToString(alertId),
                  _userDetails);
                //_logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(AlertConstants.SOCKET_EXCEPTION_MSG))
                {
                    return StatusCode(500, string.Format(AlertConstants.ACTIVATED_ALERT_SUCCESS_MSG, 2));
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpDelete]
        [Route("deletealert")]
        public async Task<IActionResult> DeleteAlert(int alertId)
        {
            try
            {
                if (alertId == 0) return BadRequest(AlertConstants.ALERT_ID_NON_ZERO_MSG);
                var response = await _alertServiceClient.DeleteAlertAsync(new IdRequest { AlertId = alertId });
                if (response == null)
                    return StatusCode(500, string.Format(AlertConstants.INTERNAL_SERVER_ERROR_MSG, 1));
                if (response.Code == ResponseCode.Success)
                    return Ok(String.Format(AlertConstants.DELETE_ALERT_SUCCESS_MSG, alertId));
                if (response.Code == ResponseCode.Conflict)
                    return StatusCode((int)response.Code, String.Format(AlertConstants.DELETE_ALERT_NO_NOTIFICATION_MSG, alertId));
                if (response.Code == ResponseCode.Failed)
                    return StatusCode((int)response.Code, String.Format(AlertConstants.DELETE_ALERT_FAILURE_MSG, alertId, AlertConstants.ALERT_FAILURE_MSG));
                if (response.Code == ResponseCode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(AlertConstants.DELETE_ALERT_FAILURE_MSG, alertId, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, AlertConstants.ALERT_CONTROLLER_NAME,
                 "Alert service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                  string.Format(AlertConstants.ALERT_EXCEPTION_LOG_MSG, "DeleteAlert", ex.Message), 1, 2, Convert.ToString(alertId),
                  _userDetails);
                //_logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(AlertConstants.SOCKET_EXCEPTION_MSG))
                {
                    return StatusCode(500, string.Format(AlertConstants.INTERNAL_SERVER_ERROR_MSG, 2));
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        #endregion

        #region Alert Category
        [HttpGet]
        [Route("getalertcategory")]
        public async Task<IActionResult> GetAlertCategory(int accountId, int orgnizationid)
        {
            try
            {
                if (accountId == 0 || orgnizationid == 0) return BadRequest(AlertConstants.ALERT_ACC_OR_ORG_ID_NOT_NULL_MSG);
                net.atos.daf.ct2.portalservice.Entity.Alert.AlertCategoryResponse response = new net.atos.daf.ct2.portalservice.Entity.Alert.AlertCategoryResponse();
                //
                var alertcategory = await _alertServiceClient.GetAlertCategoryAsync(new AccountIdRequest { AccountId = accountId });
                //Vehicle and Vehicle group
                VehicleGroupResponse vehicleGroup = await _vehicleClient.GetVehicleGroupbyAccountIdAsync(new VehicleGroupListRequest { AccountId = accountId, OrganizationId = GetUserSelectedOrgId() });
                //Notification Template
                NotificationTemplateResponse notificationTemplate = await _alertServiceClient.GetNotificationTemplateAsync(new AccountIdRequest { AccountId = accountId });
                if (alertcategory.EnumTranslation != null || vehicleGroup.VehicleGroupList != null || notificationTemplate.NotificationTemplatelist != null)
                {
                    foreach (var item in alertcategory.EnumTranslation)
                    {
                        response.EnumTranslation.Add(_mapper.MapEnumTranslation(item));
                    }

                    foreach (var item in vehicleGroup.VehicleGroupList)
                    {
                        response.VehicleGroup.Add(_mapper.MapVehicleGroup(item));
                    }

                    foreach (var item in notificationTemplate.NotificationTemplatelist)
                    {
                        response.NotificationTemplate.Add(_mapper.MapNotificationTemplate(item));
                    }

                    return Ok(response);
                }

                else
                {
                    return StatusCode(404, AlertConstants.ALERT_CATEGORY_NOT_FOUND_MSG);
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, AlertConstants.ALERT_CONTROLLER_NAME,
                 AlertConstants.ALERT_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 string.Format(AlertConstants.ALERT_EXCEPTION_LOG_MSG, "GetAlertCategory", ex.Message), 1, 2, Convert.ToString(accountId),
                  _userDetails);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion

        #region Create Alert
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateAlert(PortalAlertEntity.Alert request)
        {
            try
            {
                //check duplicate recipient label in UI list

                if (request.Notifications.Count() > 0)
                {
                    foreach (var item in request.Notifications)
                    {
                        var result = item.NotificationRecipients.GroupBy(y => y.RecipientLabel).Where(g => g.Count() > 1).ToList();
                        if (result.Count() > 0)
                        {
                            return StatusCode(409, AlertConstants.ALERT_DUPLICATE_NOTIFICATION_RECIPIENT_MSG);
                        }
                    }
                }

                var alertRequest = new AlertRequest();
                alertRequest = _mapper.ToAlertRequest(request);
                if (request.ApplyOn.ToLower() == "s")
                {
                    var vehicleGroupRequest = new vehicleservice.VehicleGroupRequest();
                    vehicleGroupRequest.Name = string.Format(AlertConstants.VEHICLE_GROUP_NAME, request.OrganizationId.ToString(), request.Id.ToString());
                    if (vehicleGroupRequest.Name.Length > 50) vehicleGroupRequest.Name = vehicleGroupRequest.Name.Substring(0, 49);
                    vehicleGroupRequest.GroupType = "S";
                    vehicleGroupRequest.RefId = alertRequest.VehicleGroupId;
                    vehicleGroupRequest.FunctionEnum = "N";
                    vehicleGroupRequest.OrganizationId = GetContextOrgId();
                    vehicleGroupRequest.Description = string.Format(AlertConstants.VEHICLE_GROUP_NAME, alertRequest.Name, alertRequest.OrganizationId);
                    vehicleservice.VehicleGroupResponce response = await _vehicleClient.CreateGroupAsync(vehicleGroupRequest);
                    alertRequest.VehicleGroupId = response.VehicleGroup.Id;
                }

                if (request.IsDuplicate)
                {
                    var isAuthWebService = request.Notifications.SelectMany(a => a.NotificationRecipients).Where(g => g.NotificationModeType.ToLower() == "w" && g.WsType.ToLower() == "a").ToList();
                    if (isAuthWebService.Count() > 0)
                    {
                        return StatusCode(400, AlertConstants.DUPLICATE_ALERT_CHECK_AUTH_WS_MSG);
                    }
                    else
                    {
                        alertservice.IdRequest idRequest = new IdRequest();
                        idRequest.AlertId = request.Id;
                        alertservice.DuplicateAlertResponse duplicateAlertResponse = await _alertServiceClient.DuplicateAlertTypeAsync(idRequest);
                        if (duplicateAlertResponse != null && duplicateAlertResponse.Code == ResponseCode.Success)
                        {
                            if (duplicateAlertResponse.DuplicateAlert != null && duplicateAlertResponse.DuplicateAlert.Type.ToLower() != request.Type.ToLower())
                            {
                                return StatusCode(400, AlertConstants.DUPLICATE_ALERT_CHECK_TYPE_MSG);
                            }
                        }
                        else if (duplicateAlertResponse.Code == ResponseCode.Failed || duplicateAlertResponse.Code == ResponseCode.InternalServerError)
                        {
                            return StatusCode((int)duplicateAlertResponse.Code, duplicateAlertResponse.Message);
                        }
                        else
                        {
                            return StatusCode(500, string.Format(AlertConstants.INTERNAL_SERVER_ERROR_MSG, "1"));
                        }
                    }
                }

                alertservice.AlertResponse alertResponse = await _alertServiceClient.CreateAlertAsync(alertRequest);

                if (alertResponse != null && alertResponse.Code == ResponseCode.Failed)
                {
                    return StatusCode(500, AlertConstants.ALERT_CREATE_FAILED_MSG);
                }
                else if (alertResponse != null && alertResponse.Code == ResponseCode.Conflict)
                {
                    return StatusCode(409, alertResponse.Message);
                }
                else if (alertResponse != null && alertResponse.Code == ResponseCode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, AlertConstants.ALERT_CONTROLLER_NAME,
                    AlertConstants.ALERT_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                    string.Format(AlertConstants.ALERT_AUDIT_LOG_MSG, "CreateAlert", AlertConstants.ALERT_CONTROLLER_NAME), alertRequest.Id, alertRequest.Id, JsonConvert.SerializeObject(request),
                    _userDetails);
                    return Ok(alertResponse.Message);
                }
                else
                {
                    return StatusCode(500, AlertConstants.ALERT_CREATE_FAILED_MSG);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, AlertConstants.ALERT_CONTROLLER_NAME,
                 AlertConstants.ALERT_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 string.Format(AlertConstants.ALERT_EXCEPTION_LOG_MSG, "CreateAlert", ex.Message), 0, 0, JsonConvert.SerializeObject(request),
                  _userDetails);
                // check for fk violation
                if (ex.Message.Contains(AlertConstants.SOCKET_EXCEPTION_MSG))
                {
                    return StatusCode(500, string.Format(AlertConstants.INTERNAL_SERVER_ERROR_MSG, "2"));
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        #endregion

        #region Update Alert
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateAlert([FromBody] PortalAlertEntity.AlertEdit request)
        {
            try
            {
                //check duplicate recipient label in UI list
                if (request.Notifications.Count() > 0)
                {
                    foreach (var item in request.Notifications)
                    {
                        var result = item.NotificationRecipients.GroupBy(y => y.RecipientLabel).Where(g => g.Count() > 1).ToList();
                        if (result.Count() > 0)
                        {
                            return StatusCode(409, AlertConstants.ALERT_DUPLICATE_NOTIFICATION_RECIPIENT_MSG);
                        }
                    }
                }

                var alertRequest = new AlertRequest();
                alertRequest = _mapper.ToAlertEditRequest(request);
                // create single vehicle group with selected vehicle  
                if (request.ApplyOn.ToLower() == "s")
                {
                    var vehicleGroupRequest = new vehicleservice.VehicleGroupRequest();
                    vehicleGroupRequest.Name = string.Format(AlertConstants.VEHICLE_GROUP_NAME, request.OrganizationId.ToString(), request.Id.ToString());
                    if (vehicleGroupRequest.Name.Length > 50) vehicleGroupRequest.Name = vehicleGroupRequest.Name.Substring(0, 49);
                    vehicleGroupRequest.GroupType = "S";
                    vehicleGroupRequest.RefId = alertRequest.VehicleGroupId;
                    vehicleGroupRequest.FunctionEnum = "N";
                    vehicleGroupRequest.OrganizationId = GetContextOrgId();
                    vehicleGroupRequest.Description = string.Format(AlertConstants.VEHICLE_GROUP_NAME, alertRequest.Name, alertRequest.OrganizationId);
                    vehicleservice.VehicleGroupResponce response = await _vehicleClient.CreateGroupAsync(vehicleGroupRequest);
                    alertRequest.VehicleGroupId = response.VehicleGroup.Id;
                }
                alertservice.AlertResponse alertResponse = await _alertServiceClient.UpdateAlertAsync(alertRequest);

                if (alertResponse != null && alertResponse.Code == ResponseCode.Failed)
                {
                    return StatusCode(500, AlertConstants.ALERT_UPDATE_FAILED_MSG);
                }
                else if (alertResponse != null && alertResponse.Code == ResponseCode.Conflict)
                {
                    return StatusCode(409, alertResponse.Message);
                }
                else if (alertResponse != null && alertResponse.Code == ResponseCode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, AlertConstants.ALERT_CONTROLLER_NAME,
                    AlertConstants.ALERT_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                    string.Format(AlertConstants.ALERT_AUDIT_LOG_MSG, "UpdateAlert", AlertConstants.ALERT_CONTROLLER_NAME), alertRequest.Id, alertRequest.Id, JsonConvert.SerializeObject(request),
                    _userDetails);
                    return Ok(alertResponse.Message);
                }
                else
                {
                    return StatusCode(500, AlertConstants.ALERT_UPDATE_FAILED_MSG);
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, AlertConstants.ALERT_CONTROLLER_NAME,
                 AlertConstants.ALERT_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 string.Format(AlertConstants.ALERT_EXCEPTION_LOG_MSG, "UpdateAlert", ex.Message), 0, 0, JsonConvert.SerializeObject(request),
                  _userDetails);
                // check for fk violation
                if (ex.Message.Contains(AlertConstants.SOCKET_EXCEPTION_MSG))
                {
                    return StatusCode(500, string.Format(AlertConstants.INTERNAL_SERVER_ERROR_MSG, "2"));
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        #endregion

        #region Get Alert List
        [HttpGet]
        [Route("getalerts")]
        public async Task<IActionResult> GetAlerts(int accountId, int orgnizationid)
        {
            try
            {
                // Fetch Feature Ids of the alert for visibility
                var featureIds = GetMappedFeatureIdByStartWithName(AlertConstants.ALERT_FEATURE_STARTWITH);
                Metadata headers = new Metadata();
                headers.Add("report_feature_ids", JsonConvert.SerializeObject(featureIds));
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                if (orgnizationid == 0) return BadRequest(AlertConstants.ALERT_ORG_ID_NOT_NULL_MSG);
                orgnizationid = GetContextOrgId();
                AlertListResponse response = await _alertServiceClient.GetAlertListAsync(new AlertListRequest { AccountId = accountId, OrganizationId = orgnizationid }, headers);

                if (response.AlertRequest != null && response.AlertRequest.Count > 0)
                {
                    response.Code = ResponseCode.Success;
                    return Ok(response.AlertRequest);
                }
                else if (response.Code == ResponseCode.Failed)
                {
                    return StatusCode(500, AlertConstants.ALERT_GET_FAILED_MSG);
                }
                else
                {
                    return StatusCode(404, AlertConstants.ALERT_NOT_FOUND_MSG);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, AlertConstants.ALERT_CONTROLLER_NAME,
                 AlertConstants.ALERT_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                string.Format(AlertConstants.ALERT_EXCEPTION_LOG_MSG, "GetAlert", ex.Message), 1, 2, Convert.ToString(accountId),
                  _userDetails);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion

        #region Get Recipient Label

        [HttpGet]
        [Route("getnotificationrecipients")]
        public async Task<IActionResult> GetRecipientLabel(int orgnizationId)
        {
            try
            {
                if (orgnizationId == 0) return BadRequest(AlertConstants.ALERT_ORG_ID_NOT_NULL_MSG);
                orgnizationId = GetContextOrgId();

                NotificationRecipientResponse response = await _alertServiceClient.GetRecipientLabelListAsync(new OrgIdRequest { OrganizationId = orgnizationId });

                if (response.NotificationRecipient != null && response.NotificationRecipient.Count > 0)
                {
                    response.Code = ResponseCode.Success;
                    return Ok(response.NotificationRecipient);
                }
                else
                {
                    return StatusCode(200, AlertConstants.ALERT_RECIPIENT_LABEL_NOT_FOUND_MSG);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, AlertConstants.ALERT_CONTROLLER_NAME,
                 AlertConstants.ALERT_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 string.Format(AlertConstants.ALERT_EXCEPTION_LOG_MSG, "GetAlert", ex.Message), 1, 2, Convert.ToString(orgnizationId),
                  _userDetails);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


        #endregion

        #region Alert Category Filter
        [HttpGet]
        [Route("getalertcategoryfilter")]
        public async Task<IActionResult> GetAlertCategoryFilter(int accountId, int roleid)
        {
            try
            {
                // Fetch Feature Ids of the alert for visibility
                var featureIds = GetMappedFeatureIdByStartWithName(AlertConstants.ALERT_FEATURE_STARTWITH);
                int orgnizationid = GetContextOrgId();
                if (accountId == 0 || orgnizationid == 0 || roleid == 0) return BadRequest(AlertConstants.ALERT_ACC_OR_ORG_ID_NOT_NULL_MSG);

                Metadata headers = new Metadata();
                headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                headers.Add("report_feature_ids", JsonConvert.SerializeObject(featureIds));

                var response = await _alertServiceClient.GetAlertCategoryFilterAsync(new AlertCategoryFilterIdRequest { AccountId = accountId, OrganizationId = orgnizationid, RoleId = roleid }, headers);
                if (response == null)
                    return StatusCode(500, "Internal Server Error.(01)");
                if (response.Code == ResponseCode.Success)
                    return Ok(response);
                if (response.Code == ResponseCode.InternalServerError)
                    return StatusCode((int)response.Code, String.Format(AlertConstants.ALERT_FILTER_FAILURE_MSG, response.Message));
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, AlertConstants.ALERT_CONTROLLER_NAME,
                 AlertConstants.ALERT_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 string.Format(AlertConstants.ALERT_EXCEPTION_LOG_MSG, "GetAlertCategory", ex.Message), 1, 2, Convert.ToString(accountId),
                  _userDetails);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion

        #region
        [HttpPost]
        [Route("insertviewnotification")]
        public async Task<IActionResult> InsertViewedNotifications(List<PortalAlertEntity.NotificationViewHistory> request)
        {
            try
            {
                var notificationRequest = new NotificationViewRequest();
                notificationRequest = _mapper.ToNotificationViewRequest(request);
                Metadata headers = new Metadata();
                headers.Add("logged_in_accountid", Convert.ToString(_userDetails.AccountId));
                alertservice.NotificationViewResponse notiResponse = await _alertServiceClient.InsertViewNotificationAsync(notificationRequest, headers);

                if (notiResponse != null && notiResponse.Code == ResponseCode.Failed)
                {
                    return StatusCode(500, AlertConstants.VIEWED_NOTIFICATION_INSERT_FAILED_MSG);
                }
                else if (notiResponse != null && notiResponse.Code == ResponseCode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, AlertConstants.ALERT_CONTROLLER_NAME,
                    AlertConstants.ALERT_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                    string.Format(AlertConstants.ALERT_AUDIT_LOG_MSG, "InsertViewedNotifications", AlertConstants.ALERT_CONTROLLER_NAME), notiResponse.InsertedId, notiResponse.InsertedId, JsonConvert.SerializeObject(request),
                    _userDetails);
                    return Ok(notiResponse.Message);
                }
                else
                {
                    return StatusCode(500, AlertConstants.VIEWED_NOTIFICATION_INSERT_FAILED_MSG);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, AlertConstants.ALERT_CONTROLLER_NAME,
                 AlertConstants.ALERT_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 string.Format(AlertConstants.ALERT_EXCEPTION_LOG_MSG, "InsertViewedNotifications", ex.Message), 0, 0, JsonConvert.SerializeObject(request),
                  _userDetails);
                // check for fk violation
                if (ex.Message.Contains(AlertConstants.SOCKET_EXCEPTION_MSG))
                {
                    return StatusCode(500, string.Format(AlertConstants.INTERNAL_SERVER_ERROR_MSG, "2"));
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


        [HttpGet]
        [Route("getofflinenotification")]
        public async Task<IActionResult> GetOfflinePushNotification()
        {
            try
            {
                OfflinePushNotiRequest offlinePushNotiRequest = new OfflinePushNotiRequest();
                offlinePushNotiRequest.AccountId = _userDetails.AccountId;
                offlinePushNotiRequest.OrganizationId = GetContextOrgId();
                // Fetch Feature Ids of the alert for visibility
                //var featureIds = GetMappedFeatureIdByStartWithName(AlertConstants.ALERT_FEATURE_STARTWITH);
                //Metadata headers = new Metadata();
                //headers.Add("logged_in_orgId", Convert.ToString(GetUserSelectedOrgId()));
                //headers.Add("report_feature_ids", JsonConvert.SerializeObject(featureIds));
                //_logger.Info($"\n\rGetOfflinePushNotificationVin - {GetUserSelectedOrgId()} - {JsonConvert.SerializeObject(featureIds)}");
                //OfflineNotificationResponse response = await _alertServiceClient.GetOfflinePushNotificationAsync(offlinePushNotiRequest, headers);

                OfflineNotificationResponse response = await _alertServiceClient.GetOfflinePushNotificationAsync(offlinePushNotiRequest);

                if (response.NotificationResponse != null && response.NotificationResponse.Count > 0)
                {
                    response.Code = ResponseCode.Success;
                    return Ok(response);
                }
                else if (response.Code == ResponseCode.Failed)
                {
                    return StatusCode(500, AlertConstants.OFFLINE_NOTI_GET_FAILED_MSG);
                }
                else
                {
                    return StatusCode(404, AlertConstants.OFFLINE_NOTI_NOT_FOUND_MSG);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, AlertConstants.ALERT_CONTROLLER_NAME,
                 AlertConstants.ALERT_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                string.Format(AlertConstants.ALERT_EXCEPTION_LOG_MSG, "getofflinenotification", ex.Message), 1, 2, Convert.ToString(_userDetails.AccountId),
                  _userDetails);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        #endregion
    }
}
