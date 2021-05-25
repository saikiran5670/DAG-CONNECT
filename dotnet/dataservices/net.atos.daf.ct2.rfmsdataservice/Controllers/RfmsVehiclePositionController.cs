using System;
using System.Threading.Tasks;
using log4net.Repository.Hierarchy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.rfms;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.response;
using net.atos.daf.ct2.rfmsdataservice.Entity;
using net.atos.daf.ct2.utilities;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.rfmsdataservice.Controllers
{
    public class RfmsVehiclePositionController
    {
         private readonly ILogger<RfmsDataServiceController> _logger;
        private readonly IRfmsManager _rfmsManager;
        private readonly IAuditTraillib _auditTrail;

        
        public RfmsVehiclePositionController(IRfmsManager rfmsManager, ILogger<RfmsDataServiceController> logger, IAuditTraillib auditTrail)
        {
            _rfmsManager = rfmsManager;
            _logger = logger;
            _auditTrail = auditTrail;

        }

         [HttpGet]
         [Route("rfms/vehicle")]
        public async Task<IActionResult> GetVehiclePosition(RfmsVehiclePositionRequest rfmsVehiclePositionRequest)
        {
            try
            {
                long currentdatetime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                await _auditTrail.AddLogs(DateTime.Now, DateTime.Now, 0, "Rfms Vehicle Position Service", "Rfms Vehicle Position Service", AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get vehicle position method vehicle position service", 1, 2, JsonConvert.SerializeObject(rfmsVehiclePositionRequest), 0, 0);
                var responce = new RfmsVehiclePosition();
                responce = await _rfmsManager.GetVehiclePosition(rfmsVehiclePositionRequest);
                return Ok(responce);

            }


            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing Rfms Vehicle data.");
                return StatusCode(500, string.Empty);
            }
        }
    }
}
