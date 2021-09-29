using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.fmsdataservice.Entity;
using Newtonsoft.Json;
using net.atos.daf.ct2.fms;
using net.atos.daf.ct2.fms.entity;

namespace net.atos.daf.ct2.fmsdataservice.Controllers
{
    [Route("vehicle")]
    [ApiController]
    public class FmsDataServiceController : ControllerBase
    {
        private readonly ILogger<FmsDataServiceController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAuditTraillib _auditTrail;
        private readonly IFmsManager _fmsManager;

        public FmsDataServiceController(IAuditTraillib auditTrail, ILogger<FmsDataServiceController> logger, IConfiguration configuration, IFmsManager fmsManager)
        {
            _logger = logger;
            _auditTrail = auditTrail;
            _configuration = configuration;
            _fmsManager = fmsManager;
        }

        [HttpGet]
        [Route("position")]
        public async Task<IActionResult> Position(VehiclePositionRequest vehiclePositionRequest)
        {
            try
            {
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "FMS Data Service Postion", "FMS data service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.PARTIAL, "FMS dataservice position received object", 0, 0, JsonConvert.SerializeObject(vehiclePositionRequest), 0, 0);
                _logger.LogInformation("Fms vehicle position function called - " + vehiclePositionRequest.VIN, vehiclePositionRequest.Since);
                net.atos.daf.ct2.fms.entity.VehiclePositionResponse vehiclePositionResponse = await _fmsManager.GetVehiclePosition(vehiclePositionRequest.VIN, vehiclePositionRequest.Since);
                // VehiclePositionResponse vehiclePositionResponse = await _fmsManager.GetVehiclePosition(vehiclePositionRequest.VIN, vehiclePositionRequest.Since);
                return Ok(vehiclePositionResponse);
                // return StatusCode(500, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing fms vehicle position data.");
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Fms Data Service", "Fms data service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.FAILED, "fms data service position object", 0, 0, JsonConvert.SerializeObject(vehiclePositionRequest), 0, 0);
                return StatusCode(500, string.Empty);
            }
        }
        [HttpGet]
        [Route("status")]
        public async Task<IActionResult> Status(VehicleStatusRequest vehicleStatusRequest)
        {
            try
            {
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "FMS Data Service Status", "FMS data service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.PARTIAL, "FMS dataservice status received object", 0, 0, JsonConvert.SerializeObject(vehicleStatusRequest), 0, 0);
                _logger.LogInformation("Fms vehicle status function called - " + vehicleStatusRequest.VIN, vehicleStatusRequest.Since);

                // VehicleStatusResponse vehicleStatusResponse = await _fmsManager.GetVehicleStatus(vehiclePositionRequest);
                return Ok(vehicleStatusRequest);
                // return StatusCode(500, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing fms vehicle status data.");
                await _auditTrail.AddLogs(DateTime.UtcNow, DateTime.UtcNow, 0, "Fms Data Service", "Fms data service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.FAILED, "fms data service status object", 0, 0, JsonConvert.SerializeObject(vehicleStatusRequest), 0, 0);
                return StatusCode(500, string.Empty);
            }
        }
    }
}

