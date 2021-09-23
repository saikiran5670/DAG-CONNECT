using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.fmsdataservice.Entity;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.fmsdataservice.Controllers
{
    [Route("vehicle")]
    [ApiController]
    public class FmsDataServiceController : ControllerBase
    {
        private readonly ILogger<FmsDataServiceController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAuditTraillib _auditTrail;
        public FmsDataServiceController(IAuditTraillib auditTrail, ILogger<FmsDataServiceController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _auditTrail = auditTrail;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("position")]
        public async Task<IActionResult> Position(Position position)
        {
            await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "FMS Data Service Postion", "FMS data service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.PARTIAL, "FMS dataservice position received object", 0, 0, JsonConvert.SerializeObject(position), 0, 0);
            return StatusCode(500, string.Empty);
        }
        [HttpGet]
        [Route("status")]
        public async Task<IActionResult> Status(Status status)
        {
            await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "FMS Data Service Status", "FMS data service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.PARTIAL, "FMS dataservice status received object", 0, 0, JsonConvert.SerializeObject(status), 0, 0);
            return StatusCode(500, string.Empty);
        }
    }
}

