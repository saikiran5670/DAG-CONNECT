using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.alertservice;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.vehicleservice;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using PortalAlertEntity = net.atos.daf.ct2.portalservice.Entity.Alert;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("alert")]
    public class AlertController : ControllerBase
    {
        private ILog _logger;
        private readonly AlertService.AlertServiceClient _AlertServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly Common.AccountPrivilegeChecker _privilegeChecker;
        private string FK_Constraint = "violates foreign key constraint";
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        private readonly HeaderObj _userDetails;
        private readonly Entity.Alert.Mapper _mapper;
        private readonly VehicleService.VehicleServiceClient _vehicleClient;

        public AlertController(AlertService.AlertServiceClient AlertServiceClient, AuditHelper auditHelper, Common.AccountPrivilegeChecker privilegeChecker, VehicleService.VehicleServiceClient vehicleClient, IHttpContextAccessor _httpContextAccessor)
        {
            _AlertServiceClient = AlertServiceClient;
            _auditHelper = auditHelper;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _privilegeChecker = privilegeChecker;
            _userDetails = _auditHelper.GetHeaderData(_httpContextAccessor.HttpContext.Request);
            _vehicleClient = vehicleClient;
            _mapper = new Entity.Alert.Mapper();
        }

        #region ActivateAlert,SuspendAlert and  DeleteAlert

        [HttpPut]
        [Route("ActivateAlert")]
        public async Task<IActionResult> ActivateAlert(int alertId)
        {
            try
            {
                if (alertId == 0) return BadRequest("Alert id cannot be zero.");
                var response = await _AlertServiceClient.ActivateAlertAsync(new IdRequest { AlertId = alertId });
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Alert Controller",
                 "Alert service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 $"ActivateAlert method Failed. Error:{ex.Message}", 1, 2, Convert.ToString(alertId),
                  Request);
                //_logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(SocketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, $"Exception Occurred, Activate Alert Failed for Id:- {alertId}.");
            }
        }

        [HttpPut]
        [Route("SuspendAlert")]
        public async Task<IActionResult> SuspendAlert(int alertId)
        {
            try
            {
                if (alertId == 0) return BadRequest("Alert id cannot be zero.");
                var response = await _AlertServiceClient.SuspendAlertAsync(new IdRequest { AlertId = alertId });
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Alert Controller",
                 "Alert service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 $"SuspendAlert method Failed. Error:{ex.Message}", 1, 2, Convert.ToString(alertId),
                  Request);
                //_logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(SocketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, $"Exception Occurred, Suspend Alert Failed for Id:- {alertId}.");
            }
        }

        [HttpDelete]
        [Route("DeleteAlert")]
        public async Task<IActionResult> DeleteAlert(int alertId)
        {
            try
            {
                if (alertId == 0) return BadRequest("Alert id cannot be zero.");
                var response = await _AlertServiceClient.DeleteAlertAsync(new IdRequest { AlertId = alertId });
                return StatusCode((int)response.Code, response.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Alert Controller",
                 "Alert service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 $"ActivateAlert method Failed. Error:{ex.Message}", 1, 2, Convert.ToString(alertId),
                  Request);
                //_logger.Error(null, ex);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(SocketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, $"Exception Occurred, Delete Alert Failed for Id:- {alertId}.");
            }
        }

        #endregion

        #region Alert Category
        [HttpGet]
        [Route("GetAlertCategory")]
        public async Task<IActionResult> GetAlertCategory(int accountId, int orgnizationid)
        {
            try
            {
                if (accountId == 0) return BadRequest("Account id cannot be null.");
                net.atos.daf.ct2.portalservice.Entity.Alert.AlertCategoryResponse response = new net.atos.daf.ct2.portalservice.Entity.Alert.AlertCategoryResponse();
                AlertCategoryResponse alertcategory = await _AlertServiceClient.GetAlertCategoryAsync(new AccountIdRequest { AccountId = accountId });
                VehicleGroupResponse vehicleGroup = await _vehicleClient.GetVehicleGroupbyAccountIdAsync(new VehicleGroupListRequest { AccountId = accountId, OrganizationId = orgnizationid });
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
                    return StatusCode(404, "Alert Category are not found.");
                }


            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Alert Controller",
                 "Alert service", Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 $"Get alert category method Failed", 1, 2, Convert.ToString(accountId),
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
                var alertRequest = new AlertRequest();
                alertRequest = _mapper.ToAlertRequest(request);
                alertservice.AlertResponse alertResponse = await _AlertServiceClient.CreateAlertAsync(alertRequest);

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
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Alert Component",
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
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Alert Component",
                 "Alert service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Create  method in Alert controller", 0, 0, JsonConvert.SerializeObject(request),
                  Request);
                // check for fk violation
                if (ex.Message.Contains(SocketException))
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
        public async Task<IActionResult> UpdateAlert([FromBody]PortalAlertEntity.AlertEdit request)
        {
            try
            {
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
                    VehicleGroupRequest.OrganizationId = alertRequest.OrganizationId;
                    VehicleGroupRequest.Description = "Single vehicle group for alert:-  " + alertRequest.Name + "  org:- " + alertRequest.OrganizationId;
                    vehicleservice.VehicleGroupResponce response = await _vehicleClient.CreateGroupAsync(VehicleGroupRequest);
                    alertRequest.VehicleGroupId = response.VehicleGroup.Id;
                }
                alertservice.AlertResponse alertResponse = await _AlertServiceClient.UpdateAlertAsync(alertRequest);

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
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Alert Component",
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
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Alert Component",
                 "Alert service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 "Update  method in Alert controller", 0, 0, JsonConvert.SerializeObject(request),
                  Request);
                // check for fk violation
                if (ex.Message.Contains(SocketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        
        #endregion
    }
}
