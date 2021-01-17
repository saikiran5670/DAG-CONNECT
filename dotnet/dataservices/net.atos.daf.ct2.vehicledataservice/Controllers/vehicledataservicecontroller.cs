using System;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.vehiclerepository;
using System.Collections.Generic;
using net.atos.daf.ct2.vehicle.entity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.repository;
using net.atos.daf.ct2.vehicledataservice.Entity;
using net.atos.daf.ct2.organization.entity;
namespace net.atos.daf.ct2.vehicledataservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class vehicledataservicecontroller : ControllerBase
    {
        private readonly ILogger<vehicledataservicecontroller> logger;

        private readonly IVehicleManager vehicleManager;
        public vehicledataservicecontroller(IVehicleManager _vehicleManager, ILogger<vehicledataservicecontroller> _logger)
        {
            vehicleManager = _vehicleManager;
            logger = _logger;
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateVehicleProperties(VehicleUpdatedEvent vehicleUpdatedEvent)
        {
            try
            {
                logger.LogInformation("UpdateVehicle function called -" + vehicleUpdatedEvent.Vehicle.VehicleID.VIN);
                if (string.IsNullOrEmpty(vehicleUpdatedEvent.Vehicle.VehicleID.VIN))
                {
                    return StatusCode(501, "The VIN  can not blank.");
                }

                VehicleProperty vehicleProperties = new VehicleProperty();
                vehicleProperties.VIN = vehicleUpdatedEvent.Vehicle.VehicleID.VIN;
                vehicleProperties.License_Plate_Number = vehicleUpdatedEvent.Vehicle.VehicleID.LicensePlate;
                if(!string.IsNullOrEmpty(vehicleUpdatedEvent.Vehicle.VehicleID.ManufactureDate))
                vehicleProperties.ManufactureDate = Convert.ToDateTime(vehicleUpdatedEvent.Vehicle.VehicleID.ManufactureDate);

                vehicleProperties.Classification_Make = vehicleUpdatedEvent.Vehicle.VehicleClassification.Make;
                vehicleProperties.Classification_Series = vehicleUpdatedEvent.Vehicle.VehicleClassification.Series;
                vehicleProperties.Classification_Type = vehicleUpdatedEvent.Vehicle.VehicleClassification.Type;
                vehicleProperties.Classification_Model = vehicleUpdatedEvent.Vehicle.VehicleClassification.Model;
                vehicleProperties.Chasis_Id = vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.ID;
                
                if(!string.IsNullOrEmpty(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.FuelTanks.Tank.nr))
                vehicleProperties.Tank_Nr = Convert.ToInt32(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.FuelTanks.Tank.nr);
                
                if(!string.IsNullOrEmpty(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.FuelTanks.Tank.Volume))
                vehicleProperties.Tank_Volume = Convert.ToInt32(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.FuelTanks.Tank.Volume);
                
                if(!string.IsNullOrEmpty(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.SideSkirts))
                vehicleProperties.SideSkirts = Convert.ToBoolean(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.SideSkirts);

                if(!string.IsNullOrEmpty(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.SideCollars))
                vehicleProperties.SideCollars = Convert.ToBoolean(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.SideCollars);
                
                if(!string.IsNullOrEmpty(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.RearOverhang))
                vehicleProperties.RearOverhang = Convert.ToInt32(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.RearOverhang);

                vehicleProperties.Engine_ID = vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.ID;
                vehicleProperties.Engine_Type = vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.Type;

                if(!string.IsNullOrEmpty(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.Power))
                vehicleProperties.Engine_Power = Convert.ToInt32(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.Power);
                vehicleProperties.Engine_Coolant = vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.Coolant;
                vehicleProperties.Engine_EmissionLevel = vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.EmissionLevel;
                vehicleProperties.GearBox_Id = vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Transmission.GearBox.ID;
                vehicleProperties.GearBox_Type = vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Transmission.GearBox.ID;
                vehicleProperties.DriverLine_AxleConfiguration = vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.AxleConfiguration;
                vehicleProperties.DriverLine_Tire_Size = vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.Wheels.Tire.Size;
                
                if(!string.IsNullOrEmpty(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.FrontAxle.position))
                vehicleProperties.DriverLine_FrontAxle_Position = Convert.ToInt32(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.FrontAxle.position);

                if(!string.IsNullOrEmpty(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.FrontAxle.Load))
                vehicleProperties.DriverLine_FrontAxle_Load = Convert.ToInt32(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.FrontAxle.Load);

                if(!string.IsNullOrEmpty(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.RearAxle.position))
                vehicleProperties.DriverLine_RearAxle_Position = Convert.ToInt32(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.RearAxle.position);

                if(!string.IsNullOrEmpty(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.RearAxle.Load))
                vehicleProperties.DriverLine_RearAxle_Load = Convert.ToInt32(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.RearAxle.Load);

                if(!string.IsNullOrEmpty(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.RearAxle.Ratio))
                vehicleProperties.DriverLine_RearAxle_Ratio = Convert.ToDecimal(vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.RearAxle.Ratio);
                vehicleProperties.DriverLine_Cabin_ID = vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Cabin.ID;
                vehicleProperties.DriverLine_Cabin_Color_ID = vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Cabin.Color.ID;
                vehicleProperties.DriverLine_Cabin_Color_Value = vehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Cabin.Color.value;

                if(!string.IsNullOrEmpty(vehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Length))
                vehicleProperties.Dimensions_Size_Length = Convert.ToInt32(vehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Length);
                
                if(!string.IsNullOrEmpty(vehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Width))
                vehicleProperties.Dimensions_Size_Width = Convert.ToInt32(vehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Width);
                vehicleProperties.Dimensions_Size_Height = Convert.ToInt32(vehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Height);
                vehicleProperties.Dimensions_Size_Weight = Convert.ToInt32(vehicleUpdatedEvent.Vehicle.VehicleDimensions.Weights);
                vehicleProperties.RegistrationDateTime = Convert.ToDateTime(vehicleUpdatedEvent.Vehicle.VehicleDelivery.RegistrationDate);
                vehicleProperties.DeliveryDate = Convert.ToDateTime(vehicleUpdatedEvent.Vehicle.VehicleDelivery.DeliveryDate);

                 List<Organization> objOrgList=new List<Organization> ();   
                foreach (var item in vehicleUpdatedEvent.Vehicle.VehicleDelivery.Party)
                {
                    Organization objOrg=new Organization ();
                    objOrg.OrganizationId=item.ID;
                    objOrg.Name=item.Name;
                    objOrg.CountryCode=item.CountryCode;
                    objOrgList.Add(objOrg);
                }

                 VehicleProperty vehiclePro =  await vehicleManager.UpdateProperty(vehicleProperties);
                logger.LogInformation("Vehicle Properties updated with VIN - " + vehicleUpdatedEvent.Vehicle.VehicleID.VIN);
                return Ok(vehicleProperties);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                var p = ex.Message;
                return StatusCode(500, "Internal Server Error.");
            }
        }


        //     [HttpPost]
        //     [Route("AddVehicle")]
        //      public async Task<IActionResult> AddVehicle(Vehicle vehicle)
        //     {
        //         try
        //         {
        //             logger.LogInformation("Add Vehicle function called -" + vehicle.CreatedBy);
        //             if (string.IsNullOrEmpty(vehicle.VIN))
        //             {
        //                 return StatusCode(501, "The VIN  can not blank.");
        //             }
        //             if (string.IsNullOrEmpty(vehicle.RegistrationNo))
        //             {
        //                 return StatusCode(502, "The Registration No can not blank");
        //             }
        //              if (string.IsNullOrEmpty(vehicle.ChassisNo))
        //             {
        //                 return StatusCode(503, "The Chassis No can not blank");
        //             }

        //             int vehicleId = await vehicleManagement.AddVehicle(vehicle);
        //             logger.LogInformation("Vehicle added with Vehicle Id - " + vehicleId);
        //             return Ok(vehicleId);
        //         }
        //         catch (Exception ex)
        //         {
        //             logger.LogError(ex.Message);
        //             var p = ex.Message;
        //             return StatusCode(500, "Internal Server Error.");
        //         }
        //     }

        //     [HttpPut]
        //     [Route("UpdateVehicle")]
        //     public async Task<IActionResult> UpdateVehicle(Vehicle vehicle)
        //     {
        //         try
        //         {
        //             logger.LogInformation("UpdateVehicle function called -" + vehicle.UpdatedBy);
        //              if (string.IsNullOrEmpty(vehicle.VIN))
        //             {
        //                 return StatusCode(501, "The VIN  can not blank.");
        //             }
        //             if (string.IsNullOrEmpty(vehicle.RegistrationNo))
        //             {
        //                 return StatusCode(502, "The Registration No can not blank");
        //             }
        //              if (string.IsNullOrEmpty(vehicle.ChassisNo))
        //             {
        //                 return StatusCode(503, "The Chassis No can not blank");
        //             }

        //             int vehicleId = await vehicleManagement.UpdateVehicle(vehicle);
        //             logger.LogInformation("Vehicle updated with Vehicle Id - " + vehicleId);
        //             return Ok(vehicleId);

        //         }
        //         catch (Exception ex)
        //         {
        //             logger.LogError(ex.Message);
        //             var p = ex.Message;
        //             return StatusCode(500, "Internal Server Error.");
        //         }
        //     }

        //     [HttpPut]
        //     [Route("DeleteVehicle")]
        //     public async Task<IActionResult> DeleteVehicle(int vehicleID, int userId)
        //     {
        //         try
        //         {
        //             logger.LogInformation("DeleteVehicle function called " + vehicleID + "userId -" + userId);
        //             if (string.IsNullOrEmpty(vehicleID.ToString()))
        //             {
        //                 return StatusCode(501, "The vehicle ID is Empty.");
        //             }

        //             if (string.IsNullOrEmpty(userId.ToString()))
        //             {
        //                 return StatusCode(501, "The userId is Empty.");
        //             }

        //             int DeletedVehicleID = await vehicleManagement.DeleteVehicle(vehicleID, userId);
        //             logger.LogInformation("Vehicle deleted with Vehicle Id - " + vehicleID);

        //             return Ok(DeletedVehicleID);
        //         }
        //         catch (Exception ex)
        //         {
        //             logger.LogError(ex.Message);
        //             var p = ex.Message;
        //             return StatusCode(500, "Internal Server Error.");
        //         }
        //     }

        //     [HttpGet]
        //     [Route("GetVehicleByID")]
        //      public async Task<IActionResult> GetVehicleByID(int vehicleID,int orgid)
        //     {
        //         try
        //         {
        //             logger.LogInformation("GetVehicle function called " + vehicleID);

        //             if (string.IsNullOrEmpty(vehicleID.ToString()))
        //             {
        //                 return StatusCode(501, "The vehicleID is Empty.");
        //             }

        //             return Ok(await vehicleManagement.GetVehicleByID(vehicleID,orgid));
        //         }
        //         catch (Exception ex)
        //         {
        //             logger.LogError(ex.Message);
        //             var p = ex.Message;
        //             return StatusCode(500, "Internal Server Error.");
        //         }
        //     }

        //      [HttpPost]
        //     [Route("AddVehicleGroup")]
        //     public async Task<IActionResult> AddVehicleGroup(VehicleGroup vehicleGroup)
        //     {
        //         try
        //         {
        //             logger.LogInformation("AddVehicleGroup function called -" + vehicleGroup.CreatedBy);
        //             if (string.IsNullOrEmpty(vehicleGroup.Name))
        //             {
        //                 return StatusCode(501, "Vehicle group name can not blank.");
        //             }               

        //             int vehicleGroupId =await vehicleManagement.AddVehicleGroup(vehicleGroup);
        //             logger.LogInformation("Vehicle group added with VehicleGroup Id - " + vehicleGroupId);
        //             return Ok(vehicleGroupId);
        //         }
        //         catch (Exception ex)
        //         {
        //             logger.LogError(ex.Message);
        //             var p = ex.Message;
        //           //  return StatusCode(500, "Internal Server Error.");
        //             return StatusCode(500, p);
        //         }
        //     }

        //     [HttpPut]
        //     [Route("UpdateVehicleGroup")]
        //    public async Task<IActionResult> UpdateVehicleGroup(VehicleGroup vehicleGroup)
        //     {
        //         try
        //         {
        //             logger.LogInformation("UpdateVehicleGroup function called -" + vehicleGroup.UpdatedBy);
        //              if (string.IsNullOrEmpty(vehicleGroup.Name))
        //             {
        //                return StatusCode(501, "Vehicle group name can not blank.");
        //             }             

        //             int vehicleGroupId =await vehicleManagement.UpdateVehicleGroup(vehicleGroup);
        //             logger.LogInformation("Vehicle Group updated with Vehicle group Id - " + vehicleGroupId);
        //             return Ok(vehicleGroupId);

        //         }
        //         catch (Exception ex)
        //         {
        //             logger.LogError(ex.Message);
        //             var p = ex.Message;
        //             return StatusCode(500, "Internal Server Error.");
        //         }
        //     }

        //     [HttpPut]
        //     [Route("DeleteVehicleGroup")]
        //     public async Task<IActionResult> DeleteVehicleGroup(int vehicleGroupID, int userId)
        //     {
        //         try
        //         {
        //             logger.LogInformation("DeleteVehicleGroup function called " + vehicleGroupID + "userId -" + userId);
        //             if (string.IsNullOrEmpty(vehicleGroupID.ToString()))
        //             {
        //                 return StatusCode(501, "The vehicle group ID is Empty.");
        //             }

        //             if (string.IsNullOrEmpty(userId.ToString()))
        //             {
        //                 return StatusCode(501, "The userId is Empty.");
        //             }

        //             int DeletedVehicleGroupID =await vehicleManagement.DeleteVehicleGroup(vehicleGroupID, userId);
        //             logger.LogInformation("Vehicle group deleted with Vehicle group Id - " + vehicleGroupID);

        //             return Ok(DeletedVehicleGroupID);
        //         }
        //         catch (Exception ex)
        //         {
        //             logger.LogError(ex.Message);
        //             var p = ex.Message;
        //             return StatusCode(500, "Internal Server Error.");
        //         }
        //     }

        //     [HttpGet]
        //     [Route("GetVehicleGroupByID")]
        //      public async Task<IActionResult> GetVehicleGroupByID(int vehicleGroupID,int orgid)
        //     {
        //         try
        //         {
        //             logger.LogInformation("GetVehicleGroup function called " + vehicleGroupID);

        //             if (string.IsNullOrEmpty(vehicleGroupID.ToString()))
        //             {
        //                 return StatusCode(501, "The vehicleGroupID is Empty.");
        //             }

        //             return Ok(await vehicleManagement.GetVehicleGroupByID(vehicleGroupID,orgid));
        //         }
        //         catch (Exception ex)
        //         {
        //             logger.LogError(ex.Message);
        //             var p = ex.Message;
        //            // return StatusCode(500, "Internal Server Error.");
        //             return StatusCode(500, p);
        //         }
        //     }

        //     [HttpGet]
        //     [Route("GetVehicleGroupByOrgID")]
        //     public async Task<IActionResult> GetVehicleGroupByOrgID(int vehOrgID)     
        //     {
        //         try
        //         {
        //             logger.LogInformation("GetVehicleGroupByOrgID function called " + vehOrgID);

        //             if (string.IsNullOrEmpty(vehOrgID.ToString()))
        //             {
        //                 return StatusCode(501, "The vehicleorg id is Empty.");
        //             }

        //             return Ok(await vehicleManagement.GetVehicleGroupByOrgID(vehOrgID));
        //         }
        //         catch (Exception ex)
        //         {
        //             logger.LogError(ex.Message);
        //             var p = ex.Message;
        //            // return StatusCode(500, "Internal Server Error.");
        //             return StatusCode(500, ex.Message);
        //         }
        //     }

        //     [HttpGet]
        //     [Route("GetVehiclesByOrgID")]
        //     public async Task<IActionResult> GetVehiclesByOrgID(int vehOrgID)
        //     {
        //         try
        //         {
        //             logger.LogInformation("GetVehiclesByOrgID function called " + vehOrgID);

        //             if (string.IsNullOrEmpty(vehOrgID.ToString()))
        //             {
        //                 return StatusCode(501, "The VehicleOrgID is Empty.");
        //             }

        //             return Ok(await vehicleManagement.GetVehiclesByOrgID(vehOrgID));
        //         }
        //         catch (Exception ex)
        //         {
        //             logger.LogError(ex.Message);
        //             var p = ex.Message;
        //             return StatusCode(500, "Internal Server Error.");
        //         }
        //     }

        //      [HttpGet]
        //     [Route("GetServiceSubscribersByOrgID")]
        //      public async Task<IActionResult> GetServiceSubscribersByOrgID(int orgid)
        //     {
        //         try
        //         {
        //             logger.LogInformation("GetServiceSubscribersByOrgID function called " + orgid);

        //             if (string.IsNullOrEmpty(orgid.ToString()))
        //             {
        //                 return StatusCode(501, "The orgid is Empty.");
        //             }

        //             return Ok(await vehicleManagement.GetServiceSubscribersByOrgID(orgid));
        //         }
        //         catch (Exception ex)
        //         {
        //             logger.LogError(ex.Message);
        //             var p = ex.Message;
        //            // return StatusCode(500, "Internal Server Error.");
        //             return StatusCode(500, p);
        //         }
        //     }

        // [HttpGet]
        // [Route("GetUsersDetailsByGroupID")]
        // public async Task<IActionResult> GetUsersDetailsByGroupID(int orgid,int usergroupid)
        // {
        //     try
        //     {
        //         logger.LogInformation("GetUsersDetailsByGroupID function called " + orgid);

        //         if (string.IsNullOrEmpty(orgid.ToString()))
        //         {
        //             return StatusCode(501, "The orgid is Empty.");
        //         }
        //         if (string.IsNullOrEmpty(usergroupid.ToString()))
        //         {
        //             return StatusCode(501, "The usergroupid is Empty.");
        //         }

        //         return Ok(await vehicleManagement.GetUsersDetailsByGroupID(orgid,usergroupid));
        //     }
        //     catch (Exception ex)
        //     {
        //         logger.LogError(ex.Message);
        //         var p = ex.Message;
        //        // return StatusCode(500, "Internal Server Error.");
        //         return StatusCode(500, p);
        //     }
        // }

    }
}
