using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.audit.Enum;

namespace net.atos.daf.ct2.auditservicerest.Controllers
{
    [ApiController]
    [Route("audit")]
    public class AuditController : ControllerBase
    {
        private readonly ILogger _logger;

        private readonly IAuditTraillib _AuditTrail;
        public AuditController(ILogger<AuditController> logger, IAuditTraillib AuditTrail)
        {
            _logger = logger;
            _AuditTrail = AuditTrail;
        }
        [HttpPost]
        [Route("addlogs")]
        public async Task<IActionResult> Addlogs(AuditTrail request)
        {
            try
            {
                // AuditTrail logs= new AuditTrail();
                // logs.Created_at= DateTime.Now;
                // logs.Performed_at = request.PerformedAt.ToDateTime();
                // logs.Performed_by=request.PerformedBy;
                // logs.Component_name=request.ComponentName;
                // logs.Service_name = request.ServiceName;                
                request.Event_type = (AuditTrailEnum.Event_type)Enum.Parse(typeof(AuditTrailEnum.Event_type), request.Event_type.ToString().ToUpper());
                request.Event_status = (AuditTrailEnum.Event_status)Enum.Parse(typeof(AuditTrailEnum.Event_status), request.Event_status.ToString().ToUpper());
                // // logs.Event_type=  AuditTrailEnum.Event_type.CREATE;
                // // logs.Event_status =  AuditTrailEnum.Event_status.SUCCESS; 
                // logs.Message = request.Message;  
                // logs.Sourceobject_id = request.SourceobjectId;  
                // logs.Targetobject_id = request.TargetobjectId;  
                // logs.Updated_data = request.UpdatedData;     
                request.Updated_data = request.Updated_data;
                var result = await _AuditTrail.AddLogs(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Add log Failed " + ex.ToString());
                return StatusCode(500, "Internal server error.");

            }




        }

        [HttpGet]
        [Route("getlogs")]
        public async Task<IActionResult> GetAuditLogs(int PerformedBy, string component_name)
        {
            try
            {
                _logger.LogInformation("All langauges method get");
                // var translations =  translationmanager.GetTranslationsByMenu(request.ID,(translationenum.MenuType)Enum.Parse(typeof(translationenum.MenuType), request.Type.ToString().ToUpper())).Result;
                var translations = await _AuditTrail.GetAuditLogs(PerformedBy, component_name);
                return Ok(translations);

            }
            catch (Exception ex)
            {
                _logger.LogError("All AuditLog method get failed " + ex.ToString());
                return StatusCode(500, "Internal server error.");
            }

        }

    }
}
