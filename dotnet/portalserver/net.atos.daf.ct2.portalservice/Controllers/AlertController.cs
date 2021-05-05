using log4net;
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

        public AlertController(AlertService.AlertServiceClient AlertServiceClient, AuditHelper auditHelper, Common.AccountPrivilegeChecker privilegeChecker,IHttpContextAccessor _httpContextAccessor)
        {
            _AlertServiceClient = AlertServiceClient;
            _auditHelper = auditHelper;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _privilegeChecker = privilegeChecker;
            _userDetails = _auditHelper.GetHeaderData(_httpContextAccessor.HttpContext.Request);
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
                 $"ActivateAlert method Failed", 1, 2, Convert.ToString(alertId),
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
                return StatusCode(500, $"Exception Occurred, Activate Alert Failed for id:- {alertId}.");
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
                 $"SuspendAlert method Failed", 1, 2, Convert.ToString(alertId),
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
                return StatusCode(500, $"Exception Occurred, Suspend Alert Failed for id:- {alertId}.");
            }
        }
        #endregion
    }
}
