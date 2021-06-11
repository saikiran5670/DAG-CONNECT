﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.auditservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class AuditController : ControllerBase
    {
        // private readonly ILogger<AuditController> _logger;

        private ILog _logger;
        private readonly AuditService.AuditServiceClient _auditService;

        //Constructor
        public AuditController(AuditService.AuditServiceClient auditService)
        {
            _auditService = auditService;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        [HttpPost]
        [Route("addlogs")]
        public async Task<IActionResult> Addlogs(AuditRecord request)
        {
            try
            {
                AuditResponce auditresponse = await _auditService.AddlogsAsync(request);

                if (auditresponse != null
                  && auditresponse.Message == "There is an error In GetTranslation.")
                {
                    return StatusCode(500, "There is an error In GetTranslation.");
                }
                else if (auditresponse != null && auditresponse.Code == Responcecode.Success)
                {
                    return Ok(auditresponse);
                }
                else
                {
                    return StatusCode(500, "GetTranslations Response is null");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("getlogs")]
        public async Task<IActionResult> GetAuditLogs([FromQuery] AuditLogRequest request)
        {
            try
            {
                AuditLogResponse allauditLogs = await _auditService.GetAuditLogsAsync(request);
                if (allauditLogs != null
                 && allauditLogs.Message == "There is an error In GetTranslation.")
                {
                    return StatusCode(500, "There is an error In GetTranslation.");
                }
                else if (allauditLogs != null && allauditLogs.Code == Responcecode.Success)
                {
                    return Ok(allauditLogs);
                }
                else
                {
                    return StatusCode(500, "GetTranslations Response is null");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
