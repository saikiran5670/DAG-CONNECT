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
using AccountComponent = net.atos.daf.ct2.account;
using AccountEntity = net.atos.daf.ct2.account.entity;
using IdentityComponent = net.atos.daf.ct2.identity;
using IdentityEntity = net.atos.daf.ct2.identity.entity;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Transactions;
using net.atos.daf.ct2.vehicledataservice.Common;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.vehicledataservice.CustomAttributes;
namespace net.atos.daf.ct2.vehicledataservice.Controllers
{
    [ApiController]
    [Route("vehicle")]
    [Authorize(Policy = AccessPolicies.MainMileageAccessPolicy)]
    public class VehicleMileageController:ControllerBase
    {
        private readonly ILogger<VehicleMileageController> logger;        
        private readonly IVehicleManager vehicleManager;                
        public VehicleMileageController(IVehicleManager _vehicleManager, ILogger<VehicleMileageController> _logger)
        {                   
            vehicleManager = _vehicleManager;
            logger = _logger;
        }
        [HttpGet]
        [Route("mileage")]
        public async Task<IActionResult> GetVehicleMileage(string since)
        {
            try
            {
                long currentdatetime=UTCHandling.GetUTCFromDateTime(DateTime.Now);
                var contentType = this.Request.ContentType;
                if (contentType== "text/csv" || contentType == "application/json")
                {
                   
                    bool isNumeric = long.TryParse(since, out long n);
                    if(isNumeric)
                    {
                        string sTimezone = "UTC";
                        string targetdateformat = "MM/DD/YYYY";
                        DateTime dDate;
                        string converteddatetime = UTCHandling.GetConvertedDateTimeFromUTC(Convert.ToInt64(since), sTimezone, targetdateformat);
                        if (!DateTime.TryParse(converteddatetime, out dDate))
                        {
                            return StatusCode(400, string.Empty);
                        }
                        else
                        {
                            since = converteddatetime;
                        }
                    }
                    else if(!(string.IsNullOrEmpty(since) || since.Equals("yesterday") || since.Equals("today")))
                    {
                        return StatusCode(400, string.Empty); 
                    }

                    VehicleMileage vehiclemileage = new VehicleMileage();
                    vehiclemileage= await vehicleManager.GetVehicleMileage(since,isNumeric, contentType);

                 
                    if (vehiclemileage==null)
                    {
                        return StatusCode(404, string.Empty);
                    } 

                    if (contentType=="text/csv")
                    {                        
                     return new VehicleMileageCSVResult(vehiclemileage.VehiclesCSV); //, "mileagedata.csv"
                    }
                    else
                    {
                          VehicleMileageResponse vehicleMileageResponse = new VehicleMileageResponse();
                          vehicleMileageResponse.Vehicles = new List<Entity.Vehicles>();
                          foreach (var item in vehiclemileage.Vehicles)
                          {
                              Entity.Vehicles vehiclesobj = new Entity.Vehicles();                           
                              vehiclesobj.EvtDateTime = item.EvtDateTime.ToString();
                              vehiclesobj.VIN = item.VIN;
                              vehiclesobj.TachoMileage = item.TachoMileage;
                              vehiclesobj.GPSMileage = item.GPSMileage;
                              vehiclesobj.RealMileageAlgorithmVersion = item.RealMileageAlgorithmVersion;
                              vehicleMileageResponse.Vehicles.Add(vehiclesobj);
                          }
                        vehiclemileage.RequestTimestamp=currentdatetime;
                        return Ok(vehicleMileageResponse);
                    }
                }
                else
                {
                    return StatusCode(400, string.Empty);
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, string.Empty);
            }
        }

    }
}
