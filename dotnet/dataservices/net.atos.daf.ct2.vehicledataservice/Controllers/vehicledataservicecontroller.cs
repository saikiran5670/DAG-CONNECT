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
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Transactions;
using net.atos.daf.ct2.vehicledataservice.Common;
using System.Net;

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
        public IConfiguration Configuration { get; }
        public vehicledataservicecontroller(AccountComponent.IAccountIdentityManager _accountIdentityManager, IVehicleManager _vehicleManager, ILogger<vehicledataservicecontroller> _logger, IOrganizationManager _organizationManager, IConfiguration configuration)
        {
            accountIdentityManager = _accountIdentityManager;
            organizationManager = _organizationManager;
            vehicleManager = _vehicleManager;
            logger = _logger;
            Configuration = configuration;
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> UpdateVehicleProperties(Root vehicleData)
        {
            try
            {
                bool valid = false;
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                valid = await accountIdentityManager.ValidateToken(token);
                //bool valid = true;
                if (valid)
                {
                    logger.LogInformation("UpdateVehicle function called -" + vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.VIN);

                    // Length Validation

                    if (!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.VIN) ||
                        !Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.LicensePlate) ||
                        !Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Make) ||
                        !Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Series.ID) ||
                        !Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Series.vehicleRange) ||
                        !Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.ModelYear) ||
                        !Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Type.ID) ||
                        !Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.ID) ||
                        !Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.SideSkirts) ||
                        !Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.SideCollars) ||
                        !Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.RearOverhang) ||
                        !Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.ID))
                    {
                        return StatusCode(400, string.Empty);
                    }

                    if ((!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.Type)) ||
                     (!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.Power)) ||
                     (!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.Coolant)) ||
                     (!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.EmissionLevel)) ||
                     (!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Transmission.GearBox.ID)) ||
                     (!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Transmission.GearBox.Type)) ||
                     (!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.AxleConfiguration)) ||
                     (!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.Wheels.Tire.Size)) ||
                     (!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.WheelBase)) ||
                     (!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Cabin.ID)))
                    {
                        return StatusCode(400, string.Empty);
                    }

                    if ((!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Cabin.RoofSpoiler)) ||
                     (!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Cabin.Type)))
                    {
                        return StatusCode(400, string.Empty);
                    }

                    if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.ElectronicControlUnits.ElectronicControlUnit != null)
                    {
                        if ((!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.ElectronicControlUnits.ElectronicControlUnit.type)) ||
                            (!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.ElectronicControlUnits.ElectronicControlUnit.Name)))
                        {
                            return StatusCode(400, string.Empty);
                        }
                    }

                    if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Size != null)
                    {
                        if ((!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Length)) ||
                         (!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Width)) ||
                         (!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Height)))
                        {
                            return StatusCode(400, string.Empty);
                        }
                    }

                    if ((!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Weights.Weight.type)) ||
                     (!Common.Common.ValidateFieldLength(50, vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Weights.Weight.value)))
                    {
                        return StatusCode(400, string.Empty);
                    }



                    //Mandatory Validation

                    if (string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.VIN))
                    {
                         return StatusCode(400,string.Empty);
                        //return BadRequest();                        
                    }

                 

                    net.atos.daf.ct2.vehicle.entity.VehicleProperty vehicleProperties = new net.atos.daf.ct2.vehicle.entity.VehicleProperty();

                    //Vehicle ID
                    vehicleProperties.VIN = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.VIN.Trim();
                    vehicleProperties.License_Plate_Number = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.LicensePlate;
                    if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.ManufactureDate))
                    {
                        if (Common.Common.IsValidDate(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.ManufactureDate)==true)
                        {
                            vehicleProperties.ManufactureDate = Convert.ToDateTime(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.ManufactureDate);
                        }
                        else
                        {
                            return StatusCode(400, string.Empty);
                        }
                    }


                    //Vehicle Classification
                    vehicleProperties.Classification_Make = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Make.Trim();
                    vehicleProperties.Classification_Series_Id = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Series.ID.Trim();
                    if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Series.vehicleRange != "LF" && vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Series.vehicleRange != "XF" && vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Series.vehicleRange != "CF")
                    {
                        //vehicleProperties.Classification_Series_VehicleRange = null;
                        return StatusCode(400, string.Empty);
                    }
                    else
                    {
                        vehicleProperties.Classification_Series_VehicleRange = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Series.vehicleRange;
                    }

                    if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Model != null)
                    {
                        vehicleProperties.Classification_Model_Id = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Model.ID;
                    }
                    vehicleProperties.Classification_ModelYear = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.ModelYear;
                    vehicleProperties.Classification_Type_Id = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Type.ID;

                    //Vehicle Named Structure
                    //Chassis
                    vehicleProperties.Chassis_Id = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.ID.Trim();

                    //Fuel Tank
                    vehicleProperties.VehicleFuelTankProperties = new List<VehicleFuelTankProperties>();
                    if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.FuelTanks.Tank != null)
                    {
                        foreach (var tank in vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.FuelTanks.Tank)
                        {
                            if (tank != null)
                            {
                                VehicleFuelTankProperties tankProperties = new VehicleFuelTankProperties();
                                tankProperties.Chassis_Tank_Nr = tank.nr;
                                tankProperties.Chassis_Tank_Volume = tank.Volume;
                                vehicleProperties.VehicleFuelTankProperties.Add(tankProperties);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.SideSkirts))
                    {
                        if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.SideSkirts.Trim().ToLower() == "yes")
                        {
                            vehicleProperties.Chassis_SideSkirts = "Yes";
                        }
                        else
                        {
                            vehicleProperties.Chassis_SideSkirts = "No";
                        }
                    }
                    if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.SideCollars))
                    {
                        if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.SideCollars.Trim().ToLower() == "yes")
                        {
                            vehicleProperties.Chassis_SideCollars = "Yes";
                        }
                        else
                        {
                            vehicleProperties.Chassis_SideCollars = "No";
                        }
                    }

                    if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.RearOverhang != null)
                        vehicleProperties.Chassis_RearOverhang = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.RearOverhang.Trim();

                    //Engine
                    vehicleProperties.Engine_ID = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.ID;
                    vehicleProperties.Engine_Type = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.Type;
                    if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.Power != null)
                        vehicleProperties.Engine_Power = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.Power;
                    vehicleProperties.Engine_Coolant = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.Coolant;
                    vehicleProperties.Engine_EmissionLevel = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine.EmissionLevel;

                    //Transmission
                    //GearBox
                    vehicleProperties.GearBox_Id = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Transmission.GearBox.ID;
                    vehicleProperties.GearBox_Type = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Transmission.GearBox.Type;

                    //DriveLine
                    vehicleProperties.DriverLine_AxleConfiguration = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.AxleConfiguration;

                    vehicleProperties.DriverLine_Tire_Size = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.Wheels.Tire.Size;

                    vehicleProperties.DriverLine_Wheelbase = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.WheelBase;

                    //Front Axel
                    vehicleProperties.VehicleAxelInformation = new List<VehicleAxelInformation>();
                    if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.FrontAxle != null)
                    {
                        foreach (var frontAxel in vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.FrontAxle)
                        {
                            VehicleAxelInformation vehiclefrontaxelInfo = new VehicleAxelInformation();
                            vehiclefrontaxelInfo.AxelType = vehicle.AxelType.FrontAxle;
                            vehiclefrontaxelInfo.Type = frontAxel.Type;
                            vehiclefrontaxelInfo.Position = frontAxel.position;
                            vehiclefrontaxelInfo.Springs = frontAxel.Springs;
                            if (frontAxel.AxleSpecificWheels != null)
                            {
                                vehiclefrontaxelInfo.Size = frontAxel.AxleSpecificWheels.Tire.Size;
                                vehiclefrontaxelInfo.Is_Wheel_Tire_Size_Replaced = true;
                            }
                            vehicleProperties.VehicleAxelInformation.Add(vehiclefrontaxelInfo);
                        }
                    }

                    //Rear Axel
                    if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.RearAxle != null)
                    {
                        foreach (var rearAxel in vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.RearAxle)
                        {
                            VehicleAxelInformation vehiclefrontaxelInfo = new VehicleAxelInformation();
                            vehiclefrontaxelInfo.AxelType = vehicle.AxelType.RearAxle;
                            vehiclefrontaxelInfo.Ratio = rearAxel.Ratio;
                            vehiclefrontaxelInfo.Load = rearAxel.Load;
                            vehiclefrontaxelInfo.Position = rearAxel.position;
                            vehiclefrontaxelInfo.Springs = rearAxel.Springs;
                            if (rearAxel.AxleSpecificWheels != null)
                            {
                                vehiclefrontaxelInfo.Size = rearAxel.AxleSpecificWheels.Tire.Size;
                                vehiclefrontaxelInfo.Is_Wheel_Tire_Size_Replaced = true;
                            }
                            vehicleProperties.VehicleAxelInformation.Add(vehiclefrontaxelInfo);
                        }
                    }

                    //Cabin
                    vehicleProperties.DriverLine_Cabin_ID = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Cabin.ID;
                    vehicleProperties.DriverLine_Cabin_RoofSpoiler = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Cabin.RoofSpoiler;
                    vehicleProperties.DriverLine_Cabin_Type = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Cabin.Type;

                    //ElectronicControlUnits
                    if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.ElectronicControlUnits.ElectronicControlUnit != null)
                    {
                        vehicleProperties.DriverLine_ElectronicControlUnit_Type = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.ElectronicControlUnits.ElectronicControlUnit.type.Trim();
                        vehicleProperties.DriverLine_ElectronicControlUnit_Name = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.ElectronicControlUnits.ElectronicControlUnit.Name.Trim();
                    }
                    //VehicleDimensions
                    //if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Length))
                    if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Size != null)
                    {
                        vehicleProperties.Dimensions_Size_Length = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Length;

                        //if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Width))
                        vehicleProperties.Dimensions_Size_Width = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Width;

                        //if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Height))
                        vehicleProperties.Dimensions_Size_Height = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Size.Height;
                    }
                    if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Weights.Weight.type))
                        vehicleProperties.Dimensions_Size_Weight_Type = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Weights.Weight.type;

                    //if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Weights.Weight.value))
                    vehicleProperties.Dimensions_Size_Weight_Value = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Weights.Weight.value;

                    //VehicleDelivery
                    if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDelivery != null)
                    {
                        if (Common.Common.IsValidDate(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDelivery.DeliveryDate) == true)
                        {
                            vehicleProperties.DeliveryDate = Convert.ToDateTime(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDelivery.DeliveryDate);
                        }
                    }

                    using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        VehicleProperty vehiclePro = await vehicleManager.UpdateProperty(vehicleProperties);

                        // Create owner realtionship

                        int OwnerRelationship = Convert.ToInt32(Configuration.GetSection("DefaultSettings").GetSection("OwnerRelationship").Value);
                        int DAFPACCAR = Convert.ToInt32(Configuration.GetSection("DefaultSettings").GetSection("DAFPACCAR").Value);

                        RelationshipMapping relationshipMapping = new RelationshipMapping();
                        relationshipMapping.relationship_id = OwnerRelationship;
                        relationshipMapping.vehicle_id = vehicleProperties.VehicleId;
                        relationshipMapping.vehicle_group_id = 0;
                        relationshipMapping.owner_org_id = DAFPACCAR;
                        relationshipMapping.created_org_id = DAFPACCAR;
                        relationshipMapping.target_org_id = DAFPACCAR;
                        relationshipMapping.isFirstRelation = true;
                        relationshipMapping.allow_chain = true;
                        await organizationManager.CreateOwnerRelationship(relationshipMapping);
                        transactionScope.Complete();
                    }

                    logger.LogInformation("Vehicle Properties updated with VIN - " + vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.VIN);
                    return Ok();
                }
                else
                {
                    //return StatusCode(401, "Account is unauthenticated.");
                    //return Unauthorized();
                    return StatusCode(401, string.Empty);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, string.Empty);
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
