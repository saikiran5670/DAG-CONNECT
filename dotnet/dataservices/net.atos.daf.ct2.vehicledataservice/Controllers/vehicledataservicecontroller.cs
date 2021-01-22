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
using net.atos.daf.ct2.organization.entity;
using net.atos.daf.ct2.organization;
using AccountComponent = net.atos.daf.ct2.account;
using AccountEntity = net.atos.daf.ct2.account.entity;
using IdentityComponent = net.atos.daf.ct2.identity;
using IdentityEntity = net.atos.daf.ct2.identity.entity;

namespace net.atos.daf.ct2.vehicledataservice.Controllers
{
    [ApiController]
    [Route("vehicle-data")]
    public class vehicledataservicecontroller : ControllerBase
    {
        private readonly ILogger<vehicledataservicecontroller> logger;
        AccountComponent.IAccountIdentityManager accountIdentityManager;
        private readonly IVehicleManager vehicleManager;
        private readonly IOrganizationManager organizationManager;
        public vehicledataservicecontroller(AccountComponent.IAccountIdentityManager _accountIdentityManager,IVehicleManager _vehicleManager, ILogger<vehicledataservicecontroller> _logger, IOrganizationManager _organizationManager)
        {
            accountIdentityManager =_accountIdentityManager;
            organizationManager = _organizationManager;
            vehicleManager = _vehicleManager;
            logger = _logger;
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateVehicleProperties(Root vehicleData)
        {
            try
            {

                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                bool valid = await accountIdentityManager.ValidateToken(token);
                if(valid)
                {
                logger.LogInformation("UpdateVehicle function called -" + vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.VIN);
                
                if (string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.VIN))
                {
                    return StatusCode(400, "Bad Request.");
                }

                net.atos.daf.ct2.vehicle.entity.VehicleProperty vehicleProperties = new net.atos.daf.ct2.vehicle.entity.VehicleProperty();
                vehicleProperties.Classification_Type =VehicleType.None;

                vehicleProperties.VIN = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.VIN;
                vehicleProperties.License_Plate_Number = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.LicensePlate;
                if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.ManufactureDate))
                    vehicleProperties.ManufactureDate = Convert.ToDateTime(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.ManufactureDate);

                vehicleProperties.Classification_Make = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Make;
                vehicleProperties.Classification_Series = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Series;

                if(Enum.IsDefined(typeof(VehicleType), vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Type))
                vehicleProperties.Classification_Type = (VehicleType)Enum.Parse(typeof(VehicleType),vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Type.ToString());
               

                vehicleProperties.Classification_Model = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Model;
                vehicleProperties.Chasis_Id = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.ID;

                if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.FuelTanks.Tank.nr))
                    vehicleProperties.Tank_Nr = Convert.ToInt32(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.FuelTanks.Tank.nr);

                if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.FuelTanks.Tank.Volume))
                    vehicleProperties.Tank_Volume = Convert.ToInt32(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.FuelTanks.Tank.Volume);

                if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.SideSkirts))
                {
                    if(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.SideSkirts=="Yes")
                    {    
                    vehicleProperties.SideSkirts = true;
                    }
                    else
                    {
                    vehicleProperties.SideSkirts = false;
                    }
                }
                if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.SideCollars))
                {
                    if(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.SideCollars=="Yes"){    
                    vehicleProperties.SideCollars = true;
                    }
                    else{
                    vehicleProperties.SideCollars = false;
                    }
                }
                    
                if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.RearOverhang))
                    vehicleProperties.RearOverhang = Convert.ToInt32(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.RearOverhang);

                vehicleProperties.Engine_ID = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.ID;
                vehicleProperties.Engine_Type = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.Type;

                if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.Power))
                    vehicleProperties.Engine_Power = Convert.ToInt32(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.Power);
                vehicleProperties.Engine_Coolant = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.Coolant;
                vehicleProperties.Engine_EmissionLevel = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.EmissionLevel;
                vehicleProperties.GearBox_Id = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Transmission.GearBox.ID;
                vehicleProperties.GearBox_Type = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Transmission.GearBox.ID;
                vehicleProperties.DriverLine_AxleConfiguration = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.AxleConfiguration;
                vehicleProperties.DriverLine_Tire_Size = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.Wheels.Tire.Size;

                if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.FrontAxle.position))
                    vehicleProperties.DriverLine_FrontAxle_Position = Convert.ToInt32(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.FrontAxle.position);

                if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.FrontAxle.Load))
                    vehicleProperties.DriverLine_FrontAxle_Load = Convert.ToInt32(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.FrontAxle.Load);

                if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.RearAxle.position))
                    vehicleProperties.DriverLine_RearAxle_Position = Convert.ToInt32(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.RearAxle.position);

                if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.RearAxle.Load))
                    vehicleProperties.DriverLine_RearAxle_Load = Convert.ToInt32(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.RearAxle.Load);

                if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.RearAxle.Ratio))
                    vehicleProperties.DriverLine_RearAxle_Ratio = Convert.ToDecimal(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.RearAxle.Ratio);
                vehicleProperties.DriverLine_Cabin_ID = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Cabin.ID;
                vehicleProperties.DriverLine_Cabin_Color_ID = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Cabin.Color.ID;
                vehicleProperties.DriverLine_Cabin_Color_Value = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Cabin.Color.value;

                if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Length))
                    vehicleProperties.Dimensions_Size_Length = Convert.ToInt32(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Length);

                if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Width))
                    vehicleProperties.Dimensions_Size_Width = Convert.ToInt32(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Width);

                if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Height))
                vehicleProperties.Dimensions_Size_Height = Convert.ToInt32(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Height);
                
                if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Weights.Weight))
                vehicleProperties.Dimensions_Size_Weight = Convert.ToInt32(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Weights.Weight);

                if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDelivery.RegistrationDate))
                vehicleProperties.RegistrationDateTime = Convert.ToDateTime(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDelivery.RegistrationDate);

                if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDelivery.DeliveryDate))
                vehicleProperties.DeliveryDate = Convert.ToDateTime(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDelivery.DeliveryDate);
                
                List<Customer> objCustList = new List<Customer>();
                foreach (var item in vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDelivery.Party)
                {
                 if(item.ID!=null)
                 {   
                    Customer objCustomer = new Customer(){
                        CompanyUpdatedEvent =new CompanyUpdatedEvent()
                        {
                            Company=new Company(){
                                ID=item.ID,
                                Name=item.Name,
                                Address=new Address(){
                                CountryCode=item.CountryCode
                                }
                            }
                        }
                    };
                    vehicleProperties.Org_Id=item.ID;
                    objCustList.Add(objCustomer);
                  }
                }

                int count = await organizationManager.CreateVehicleParty(objCustList);
                if(count>0)
                {
                VehicleProperty vehiclePro = await vehicleManager.UpdateProperty(vehicleProperties);
                }
               

                
                logger.LogInformation("Vehicle Properties updated with VIN - " + vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.VIN);
                return Ok(200);
                }
                else
                {
                     return StatusCode(401, "Account is unauthenticated.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500,"Internal Server Error.");
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
