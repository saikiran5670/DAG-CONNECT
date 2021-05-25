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
using net.atos.daf.ct2.rfms.responce;
using net.atos.daf.ct2.rfmsdataservice.Entity;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.vehicle;

namespace net.atos.daf.ct2.rfmsdataservice.Controllers
{

    [ApiController]
    [Route("rfms-vehicles")]
    [Authorize(Policy = AccessPolicies.MainAccessPolicy)]
    public class RfmsDataServiceController : ControllerBase
    {
              private readonly ILogger<RfmsDataServiceController> _logger;
              private readonly IRfmsManager _rfmsManager;
              private readonly IAuditTraillib _auditTrail;
               

              public RfmsDataServiceController(IRfmsManager rfmsManager,ILogger<RfmsDataServiceController> logger,IAuditTraillib auditTrail)
              {
                _rfmsManager = rfmsManager;
                _logger = logger;
                _auditTrail = auditTrail;
                      
              }


               [HttpGet]
               [Route("rfms/vehicles")]
               public async Task<IActionResult> GetVehicles(RfmsVehicleRequest rfmsVehicleRequest)
              {
                try
                  {
                     long currentdatetime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                     await AuditTrail.AddLogs(DateTime.Now, DateTime.Now, 0, "Rfms Vehicle Service", "Rfms Vehicle Service", AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get namelist method vehicle namelist service", 1, 2, since, 0, 0);
                     var responce = new RfmsVehicles();                  
                     responce = await rfmsManager.Get(rfmsVehicleRequest);
                     return Ok(responce);

                  }

                     
                catch (Exception ex)
                {
                     Logger.LogError(ex, "Error occurred while processing Rfms Vehicle data.");
                     return StatusCode(500, string.Empty);
                }
              }     

    }
}
