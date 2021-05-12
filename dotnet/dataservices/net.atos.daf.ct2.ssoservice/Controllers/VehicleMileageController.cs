using System;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.vehiclerepository;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;
using net.atos.daf.ct2.singlesignonservice.Entity;
using AccountComponent = net.atos.daf.ct2.account;
using AccountEntity = net.atos.daf.ct2.account.entity;
using IdentityComponent = net.atos.daf.ct2.identity;
using IdentityEntity = net.atos.daf.ct2.identity.entity;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Transactions;
using net.atos.daf.ct2.singlesignonservice.Common;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.singlesignonservice.CustomAttributes;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using Newtonsoft.Json;
using System.Linq;

namespace net.atos.daf.ct2.singlesignonservice.Controllers
{
    [ApiController]
    [Route("vehicle")]
    [Authorize(Policy = AccessPolicies.MainMileageAccessPolicy)]
    public class VehicleMileageController:ControllerBase
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
                /*
                //TODO:: Delete after new implmentation
                var selectedType = string.Empty;
                long currentdatetime=UTCHandling.GetUTCFromDateTime(DateTime.Now);
                if(string.IsNullOrEmpty(this.Request.ContentType))
                    return StatusCode(400, string.Empty);

                await AuditTrail.AddLogs(DateTime.Now, DateTime.Now, 2, "Vehicle mileage Service", "Vehicle mileage Service", AuditTrailEnum.Event_type.GET, AuditTrailEnum.Event_status.SUCCESS, "Get mileage method vehicle mileage service", 1, 2, this.Request.ContentType, 0, 0);
                
                var contentTypes = this.Request.ContentType.Split(";");
                if (contentTypes.Any(x => x.Trim().Equals("text/csv", StringComparison.CurrentCultureIgnoreCase)))
                    selectedType = "text/csv";
                if (contentTypes.Any(x => x.Trim().Equals("application/json", StringComparison.CurrentCultureIgnoreCase)))
                    selectedType = "application/json";                
                
                if (!string.IsNullOrEmpty(selectedType))
                {                   
                    bool isNumeric = long.TryParse(since, out long n);
                    if(isNumeric)
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
                    else if(!(string.IsNullOrEmpty(since) || since.Equals("yesterday") || since.Equals("today")))
                    {
                        return StatusCode(400, string.Empty); 
                    }

                    VehicleMileage vehiclemileage = new VehicleMileage();
                    vehiclemileage= await vehicleManager.GetVehicleMileage(since,isNumeric, selectedType);


                    if (vehiclemileage.Vehicles.Count == 0 && vehiclemileage.VehiclesCSV.Count == 0)
                    {
                        return StatusCode(404, string.Empty);
                    } 

                    if (selectedType.Equals("text/csv"))
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
                        vehicleMileageResponse.RequestTimestamp=currentdatetime;
                        return Ok(vehicleMileageResponse);
                    }
                }
                else
                {
                    return StatusCode(400, string.Empty);
                }
                */
                return StatusCode(400, string.Empty);
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, string.Empty);
            }
        }

    }
}
