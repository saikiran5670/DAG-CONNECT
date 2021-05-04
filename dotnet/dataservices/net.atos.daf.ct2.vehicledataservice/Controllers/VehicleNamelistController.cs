using System;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.vehiclerepository;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;
using net.atos.daf.ct2.vehicledataservice.Entity;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Transactions;
using net.atos.daf.ct2.vehicledataservice.Common;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.vehicledataservice.CustomAttributes;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using Newtonsoft.Json;
using System.Linq;
using net.atos.daf.ct2.vehicle.response;

namespace net.atos.daf.ct2.vehicledataservice.Controllers
{
    [ApiController]
    [Route("vehicle")]
    [Authorize(Policy = AccessPolicies.MainNamelistAccessPolicy)]
    public class VehicleNamelistController : ControllerBase
    {
        private readonly ILogger<VehicleNamelistController> logger;
        private readonly IVehicleManager vehicleManager;
        private readonly IAuditTraillib AuditTrail;
        public VehicleNamelistController(IVehicleManager _vehicleManager, ILogger<VehicleNamelistController> _logger, IAuditTraillib _AuditTrail)
        {
            vehicleManager = _vehicleManager;
            AuditTrail = _AuditTrail;
            logger = _logger;
        }
        [HttpGet]
        [Route("namelist")]
        public async Task<IActionResult> GetVehicleNamelist(string since)
        {
            try
            {

                long currentdatetime = UTCHandling.GetUTCFromDateTime(DateTime.Now);


                bool isNumeric = long.TryParse(since, out long n);
                if (isNumeric)
                {
                    string sTimezone = "UTC";
                    DateTime dDate;
                    string converteddatetime = UTCHandling.GetConvertedDateTimeFromUTC(Convert.ToInt64(since), sTimezone, null);
                    if (!DateTime.TryParse(converteddatetime, out dDate))
                    {
                        return StatusCode(400, string.Empty);
                    }
                    else
                    {
                        since = converteddatetime;
                    }
                }
                else if (!(string.IsNullOrEmpty(since) || since.Equals("yesterday") || since.Equals("today")))
                {
                    return StatusCode(400, string.Empty);
                }
                VehicleNamelistResponse vehiclenamelist = new VehicleNamelistResponse();
                vehiclenamelist = await vehicleManager.GetVehicleNamelist(since, isNumeric);
                if (vehiclenamelist.Vehicles.Count == 0)
                {
                    return StatusCode(404, string.Empty);
                }
                vehiclenamelist.RequestTimestamp = currentdatetime;
                return Ok(vehiclenamelist);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, string.Empty);
            }
        }
    }
}
