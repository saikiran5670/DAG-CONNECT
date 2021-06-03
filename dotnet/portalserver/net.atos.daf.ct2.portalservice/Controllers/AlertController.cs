using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
        private ILog _logger;
        private readonly AlertService.AlertServiceClient _alertServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly Entity.Alert.Mapper _mapper;
        private readonly VehicleService.VehicleServiceClient _vehicleClient;

        public AlertController(AlertService.AlertServiceClient alertServiceClient,
                               AuditHelper auditHelper,
                               Common.AccountPrivilegeChecker privilegeChecker,
                               VehicleService.VehicleServiceClient vehicleClient,
                               IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper) : base(httpContextAccessor, sessionHelper, privilegeChecker)
        {
            _alertServiceClient = alertServiceClient;
            _auditHelper = auditHelper;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _userDetails = _auditHelper.GetHeaderData(httpContextAccessor.HttpContext.Request);
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
                    return StatusCode(500, String.Format(AlertConstants.INTERNAL_SERVER_ERROR_MSG,1));
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
                  Request);
                // check for fk violation
                if (ex.Message.Contains(AlertConstants.SOCKET_EXCEPTION_MSG))
                {
                    return StatusCode(500, String.Format(AlertConstants.INTERNAL_SERVER_ERROR_MSG,2));
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
                  Request);
                //_logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(AlertConstants.SOCKET_EXCEPTION_MSG))
                {
                    return StatusCode(500, string.Format(AlertConstants.ACTIVATED_ALERT_SUCCESS_MSG,2));
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
                    return StatusCode(500, string.Format(AlertConstants.INTERNAL_SERVER_ERROR_MSG,1));
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
                  string.Format(AlertConstants.ALERT_EXCEPTION_LOG_MSG,"DeleteAlert",ex.Message), 1, 2, Convert.ToString(alertId),
                  Request);
                //_logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(AlertConstants.SOCKET_EXCEPTION_MSG))
                {
                    return StatusCode(500,string.Format(AlertConstants.INTERNAL_SERVER_ERROR_MSG,2));
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
                var alertcategory = await _alertServiceClient.GetAlertCategoryAsync(new AccountIdRequest { AccountId = accountId });
                VehicleGroupResponse vehicleGroup = await _vehicleClient.GetVehicleGroupbyAccountIdAsync(new VehicleGroupListRequest { AccountId = accountId, OrganizationId = GetUserSelectedOrgId() });
                if (alertcategory.EnumTranslation != null)
                {
                    foreach (var item in alertcategory.EnumTranslation)
                    {
                        response.EnumTranslation.Add(_mapper.MapEnumTranslation(item));
                    }
                    foreach (var item in vehicleGroup.VehicleGroupList)
                    {
                        response.VehicleGroup.Add(_mapper.MapVehicleGroup(item));
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
                  Request);
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
                var result = request.Notifications.SelectMany(a => a.NotificationRecipients).GroupBy(y => y.RecipientLabel).Where(g => g.Count() > 1).ToList();
                if (result.Count() > 0)
                {
                    return StatusCode(409, AlertConstants.ALERT_DUPLICATE_NOTIFICATION_RECIPIENT_MSG);
                }

                var alertRequest = new AlertRequest();
                alertRequest = _mapper.ToAlertRequest(request);
                if (request.ApplyOn.ToLower() == "s")
                {
                    var VehicleGroupRequest = new vehicleservice.VehicleGroupRequest();
                    VehicleGroupRequest.Name = string.Format(AlertConstants.VEHICLE_GROUP_NAME, request.OrganizationId.ToString(), request.Id.ToString());
                    if (VehicleGroupRequest.Name.Length > 50) VehicleGroupRequest.Name = VehicleGroupRequest.Name.Substring(0, 49);
                    VehicleGroupRequest.GroupType = "S";
                    VehicleGroupRequest.RefId = alertRequest.VehicleGroupId;
                    VehicleGroupRequest.FunctionEnum = "N";
                    VehicleGroupRequest.OrganizationId = GetContextOrgId();
                    VehicleGroupRequest.Description = string.Format(AlertConstants.VEHICLE_GROUP_NAME,alertRequest.Name ,alertRequest.OrganizationId);
                    vehicleservice.VehicleGroupResponce response = await _vehicleClient.CreateGroupAsync(VehicleGroupRequest);
                    alertRequest.VehicleGroupId = response.VehicleGroup.Id;
                }

                if (request.IsDuplicate)
                {
                    var isAuthWebService = request.Notifications.SelectMany(a => a.NotificationRecipients).Where(g => g.NotificationModeType.ToLower() == "w" && g.WsType.ToLower() == "a").ToList();
                    if (isAuthWebService.Count() > 0)
                    {
                        return StatusCode(400, "Duplicate alert can't be create for authentication type web service.");
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
                                return StatusCode(400, "Alert type should be same while duplicating the alert");
                            }
                        }
                        else if (duplicateAlertResponse.Code == ResponseCode.Failed || duplicateAlertResponse.Code == ResponseCode.InternalServerError)
                        {
                            return StatusCode((int)duplicateAlertResponse.Code, duplicateAlertResponse.Message);
                        }
                        else
                        {
                            return StatusCode(500, "Internal Server Error.(01)");
                        }
                    }
                }

                alertservice.AlertResponse alertResponse = await _alertServiceClient.CreateAlertAsync(alertRequest);

                if (alertResponse != null && alertResponse.Code == ResponseCode.Failed)
                {
                    return StatusCode(500, "There is an error while creating alert.");
                }
                else if (alertResponse != null && alertResponse.Code == ResponseCode.Conflict)
                {
                    return StatusCode(409, alertResponse.Message);
                }
                else if (alertResponse != null && alertResponse.Code == ResponseCode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Alert Component",
                    "Alert service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                    "Create method in Alert controller", alertRequest.Id, alertRequest.Id, JsonConvert.SerializeObject(request),
                    Request);
                    return Ok(alertResponse.Message);
                }
                else
                {
                    return StatusCode(404, "Alert Response is null");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Alert Component",
                 "Alert service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Create  method in Alert controller", 0, 0, JsonConvert.SerializeObject(request),
                  Request);
                // check for fk violation
                if (ex.Message.Contains(AlertConstants.SOCKET_EXCEPTION_MSG))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
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
                var result = request.Notifications.SelectMany(a => a.NotificationRecipients).GroupBy(y => y.RecipientLabel).Where(g => g.Count() > 1).ToList();
                if (result.Count() > 0)
                {
                    return StatusCode(409, "Duplicate notification recipient label added in list.");
                }

                var alertRequest = new AlertRequest();
                alertRequest = _mapper.ToAlertEditRequest(request);
                // create single vehicle group with selected vehicle  
                if (request.ApplyOn.ToLower() == "s")
                {
                    var VehicleGroupRequest = new vehicleservice.VehicleGroupRequest();
                    VehicleGroupRequest.Name = string.Format("VehicleGroup_{0}_{1}", request.OrganizationId.ToString(), request.Id.ToString());
                    if (VehicleGroupRequest.Name.Length > 50) VehicleGroupRequest.Name = VehicleGroupRequest.Name.Substring(0, 49);
                    VehicleGroupRequest.GroupType = "S";
                    VehicleGroupRequest.RefId = alertRequest.VehicleGroupId;
                    VehicleGroupRequest.FunctionEnum = "N";
                    VehicleGroupRequest.OrganizationId = GetContextOrgId();
                    VehicleGroupRequest.Description = "Single vehicle group for alert:-  " + alertRequest.Name + "  org:- " + alertRequest.OrganizationId;
                    vehicleservice.VehicleGroupResponce response = await _vehicleClient.CreateGroupAsync(VehicleGroupRequest);
                    alertRequest.VehicleGroupId = response.VehicleGroup.Id;
                }
                alertservice.AlertResponse alertResponse = await _alertServiceClient.UpdateAlertAsync(alertRequest);

                if (alertResponse != null && alertResponse.Code == ResponseCode.Failed)
                {
                    return StatusCode(500, "There is an error while updating alert.");
                }
                else if (alertResponse != null && alertResponse.Code == ResponseCode.Conflict)
                {
                    return StatusCode(409, alertResponse.Message);
                }
                else if (alertResponse != null && alertResponse.Code == ResponseCode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Alert Component",
                    "Alert service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                    "Update method in Alert controller", alertRequest.Id, alertRequest.Id, JsonConvert.SerializeObject(request),
                    Request);
                    return Ok(alertResponse.Message);
                }
                else
                {
                    return StatusCode(404, "Alert Response is null");
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Alert Component",
                 "Alert service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Update  method in Alert controller", 0, 0, JsonConvert.SerializeObject(request),
                  Request);
                // check for fk violation
                if (ex.Message.Contains(AlertConstants.SOCKET_EXCEPTION_MSG))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
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
                if (orgnizationid == 0) return BadRequest("Orgnization id cannot be null.");
                orgnizationid = GetContextOrgId();
                AlertListResponse response = await _alertServiceClient.GetAlertListAsync(new AlertListRequest { AccountId = accountId, OrganizationId = orgnizationid });

                if (response.AlertRequest != null && response.AlertRequest.Count > 0)
                {
                    response.Code = ResponseCode.Success;
                    return Ok(response.AlertRequest);
                }
                else
                {
                    return StatusCode(404, "Alerts are not found.");
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Alert Controller",
                 "Alert service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 $"Get alerts method Failed", 1, 2, Convert.ToString(accountId),
                  Request);
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
                if (orgnizationId == 0) return BadRequest("Orgnization id cannot be null.");
                orgnizationId = GetContextOrgId();
                NotificationRecipientResponse response = await _alertServiceClient.GetRecipientLabelListAsync(new OrgIdRequest { OrganizationId = orgnizationId });

                if (response.NotificationRecipient != null && response.NotificationRecipient.Count > 0)
                {
                    response.Code = ResponseCode.Success;
                    return Ok(response.NotificationRecipient);
                }
                else
                {
                    return StatusCode(200, "Recipient Label are not found.");
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Alert Controller",
                 "Alert service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 $"Get recipient label method Failed", 1, 2, Convert.ToString(orgnizationId),
                  Request);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }


        #endregion

    }
}
