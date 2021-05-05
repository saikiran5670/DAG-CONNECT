using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicledataservice.Entity;
using net.atos.daf.ct2.vehicledataservice.Common;
using System.Net;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using System.Linq;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Authorization;
using net.atos.daf.ct2.vehicledataservice.CustomAttributes;

namespace net.atos.daf.ct2.vehicledataservice.Controllers
{
    [ApiController]
    [Route("vehicle")]
    [Authorize(Policy = AccessPolicies.MainMileageAccessPolicy)]
    public class VehicleMileageController : ControllerBase
    {
        private readonly ILogger<VehicleMileageController> logger;
        private readonly IVehicleManager vehicleManager;
        private readonly IAuditTraillib AuditTrail;
        public VehicleMileageController(IVehicleManager _vehicleManager, ILogger<VehicleMileageController> _logger, IAuditTraillib _AuditTrail)
        {
            vehicleManager = _vehicleManager;
            AuditTrail = _AuditTrail;
            logger = _logger;
        }

        [HttpGet]
        [Route("mileage")]
        public async Task<IActionResult> GetVehicleMileage(string since)
        {
            try
            {
                var selectedType = string.Empty;
                long currentdatetime = UTCHandling.GetUTCFromDateTime(DateTime.Now);

                StringValues acceptHeader;
                this.Request.Headers.TryGetValue("Accept", out acceptHeader);

                if (!this.Request.Headers.ContainsKey("Accept") ||
                    (this.Request.Headers.ContainsKey("Accept") && acceptHeader.Count() == 0))
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, "Accept header");

                await AuditTrail.AddLogs(DateTime.Now, DateTime.Now, 2, "Vehicle mileage Service", "Vehicle mileage Service", AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.SUCCESS, "Get mileage method vehicle mileage service", 1, 2, this.Request.ContentType, 0, 0);

                if (acceptHeader.Any(x => x.Trim().Equals(VehicleMileageResponseTypeConstants.CSV, StringComparison.CurrentCultureIgnoreCase)))
                    selectedType = VehicleMileageResponseTypeConstants.CSV;
                if (acceptHeader.Any(x => x.Trim().Equals(VehicleMileageResponseTypeConstants.JSON, StringComparison.CurrentCultureIgnoreCase)))
                    selectedType = VehicleMileageResponseTypeConstants.JSON;

                if (!string.IsNullOrEmpty(selectedType))
                {
                    var isValid = ValidateParameter(ref since, out bool isNumeric);
                    if (isValid)
                    {
                        VehicleMileage vehicleMileage = new VehicleMileage();
                        vehicleMileage = await vehicleManager.GetVehicleMileage(since, isNumeric, selectedType);

                        if (selectedType.Equals(VehicleMileageResponseTypeConstants.CSV))
                        {
                            return new VehicleMileageCSVResult(vehicleMileage.VehiclesCSV); //, "mileagedata.csv"
                        }
                        else
                        {
                            VehicleMileageResponse vehicleMileageResponse = new VehicleMileageResponse();
                            vehicleMileageResponse.Vehicles = new List<Entity.Vehicles>();
                            foreach (var item in vehicleMileage.Vehicles)
                            {
                                Entity.Vehicles vehiclesobj = new Entity.Vehicles();
                                vehiclesobj.EvtDateTime = item.EvtDateTime.ToString();
                                vehiclesobj.VIN = item.VIN;
                                vehiclesobj.TachoMileage = item.TachoMileage;
                                vehiclesobj.GPSMileage = item.GPSMileage;
                                vehiclesobj.RealMileageAlgorithmVersion = item.RealMileageAlgorithmVersion;
                                vehicleMileageResponse.Vehicles.Add(vehiclesobj);
                            }
                            vehicleMileageResponse.RequestTimestamp = currentdatetime;
                            return Ok(vehicleMileageResponse);
                        }
                    }

                    return GenerateErrorResponse(HttpStatusCode.BadRequest, nameof(since));
                }
                else
                {
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, "Accept header");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, string.Empty);
            }
        }

        private bool ValidateParameter(ref string since, out bool isNumeric)
        {
            isNumeric = long.TryParse(since, out _);
            if (isNumeric)
            {
                string sTimezone = "UTC";
                DateTime dDate;
                try
                {
                    string converteddatetime = UTCHandling.GetConvertedDateTimeFromUTC(Convert.ToInt64(since), sTimezone, null);
                    if (!DateTime.TryParse(converteddatetime, out dDate))
                        return false;
                    else
                        since = converteddatetime;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else if (!(string.IsNullOrEmpty(since) || since.Equals("yesterday") || since.Equals("today")))
            {
                return false;
            }
            return true;
        }

        private IActionResult GenerateErrorResponse(HttpStatusCode statusCode, string parameter)
        {
            return StatusCode((int)statusCode, new ErrorResponse()
            {
                ResponseCode = ((int)statusCode).ToString(),
                Message = "INVALID_PARAMETER",
                Value = parameter + " parameter has an invalid value."
            });
        }
    }
}
