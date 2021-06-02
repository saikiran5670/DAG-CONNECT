using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicledataservice.Common;
using net.atos.daf.ct2.vehicledataservice.CustomAttributes;
using net.atos.daf.ct2.vehicledataservice.Entity;

namespace net.atos.daf.ct2.vehicledataservice.Controllers
{
    [ApiController]
    [Route("vehicle")]
    [Authorize(Policy = AccessPolicies.MAIN_MILEAGE_ACCESS_POLICY)]
    public class VehicleMileageController : ControllerBase
    {
        private readonly ILogger<VehicleMileageController> _logger;
        private readonly IVehicleManager _vehicleManager;
        private readonly IAccountManager _accountManager;
        private readonly IAuditTraillib _auditTrail;
        public VehicleMileageController(IVehicleManager vehicleManager, IAccountManager accountManager, ILogger<VehicleMileageController> logger, IAuditTraillib auditTrail)
        {
            this._vehicleManager = vehicleManager;
            this._accountManager = accountManager;
            _auditTrail = auditTrail;
            this._logger = logger;
        }

        [HttpGet]
        [Route("mileage")]
        public async Task<IActionResult> GetVehicleMileage(string since)
        {
            try
            {
                var selectedType = string.Empty;
                long currentdatetime = UTCHandling.GetUTCFromDateTime(DateTime.Now);

                this.Request.Headers.TryGetValue("Accept", out StringValues acceptHeader);

                if (!this.Request.Headers.ContainsKey("Accept") ||
                    (this.Request.Headers.ContainsKey("Accept") && acceptHeader.Count() == 0))
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, "Accept");

                await _auditTrail.AddLogs(DateTime.Now, DateTime.Now, 2, "Vehicle mileage Service", "Vehicle mileage Service", AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.PARTIAL, "Get mileage method vehicle mileage service", 1, 2, this.Request.ContentType, 0, 0);

                if (acceptHeader.Any(x => x.Trim().Equals(VehicleMileageResponseTypeConstants.CSV, StringComparison.CurrentCultureIgnoreCase)))
                    selectedType = VehicleMileageResponseTypeConstants.CSV;
                if (acceptHeader.Any(x => x.Trim().Equals(VehicleMileageResponseTypeConstants.JSON, StringComparison.CurrentCultureIgnoreCase)))
                    selectedType = VehicleMileageResponseTypeConstants.JSON;

                if (!string.IsNullOrEmpty(selectedType))
                {
                    var isValid = ValidateParameter(ref since, out bool isNumeric);
                    if (isValid)
                    {
                        var accountEmailId = User.Claims.Where(x => x.Type.Equals("email") || x.Type.Equals(ClaimTypes.Email)).FirstOrDefault();
                        var account = await _accountManager.GetAccountByEmailId(accountEmailId.Value.ToLower());

                        var orgs = await _accountManager.GetAccountOrg(account.Id);

                        VehicleMileage vehicleMileage = new VehicleMileage();
                        vehicleMileage = await _vehicleManager.GetVehicleMileage(since, isNumeric, selectedType, account.Id, orgs.First().Id);

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
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, "Accept");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing Vehicle Mileage data.");
                return StatusCode(500, string.Empty);
            }
        }

        private bool ValidateParameter(ref string since, out bool isNumeric)
        {
            isNumeric = long.TryParse(since, out _);
            if (isNumeric)
            {
                string sTimezone = "UTC";
                try
                {
                    string converteddatetime = UTCHandling.GetConvertedDateTimeFromUTC(Convert.ToInt64(since), sTimezone, null);
                    if (!DateTime.TryParse(converteddatetime, out DateTime dDate))
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

        private IActionResult GenerateErrorResponse(HttpStatusCode statusCode, string value)
        {
            return StatusCode((int)statusCode, new ErrorResponse()
            {
                ResponseCode = ((int)statusCode).ToString(),
                Message = "INVALID_PARAMETER",
                Value = value
            });
        }
    }
}
