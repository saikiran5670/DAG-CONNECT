﻿using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.alertservice;
using net.atos.daf.ct2.portalservice.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using PortalAlertEntity = net.atos.daf.ct2.portalservice.Entity.Alert;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
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

        public AlertController(AlertService.AlertServiceClient AlertServiceClient, AuditHelper auditHelper, Common.AccountPrivilegeChecker privilegeChecker, IHttpContextAccessor _httpContextAccessor)
        {
            _AlertServiceClient = AlertServiceClient;
            _auditHelper = auditHelper;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _privilegeChecker = privilegeChecker;
            _userDetails = _auditHelper.GetHeaderData(_httpContextAccessor.HttpContext.Request);
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
        public async Task<IActionResult> GetAlertCategory(int accountId)
        {
            try
            {
                if (accountId == 0) return BadRequest("Account id cannot be null.");
                AlertCategoryResponse response = new AlertCategoryResponse();
                response = await _AlertServiceClient.GetAlertCategoryAsync(new AccountIdRequest { AccountId = accountId });
                if (response.EnumTranslation != null && response.VehicleGroup != null)
                {
                    response.Code = ResponseCode.Success;
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
    }
}
