using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.organization;
using net.atos.daf.ct2.organization.entity;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicledataservice.CustomAttributes;
using net.atos.daf.ct2.vehicledataservice.Entity;

namespace net.atos.daf.ct2.vehicledataservice.Controllers
{
    [ApiController]
    [Route("vehicle-data")]
    [Authorize(Policy = AccessPolicies.MainAccessPolicy)]
    public class VehicleDataserviceController : ControllerBase
    {
        private readonly ILogger<VehicleDataserviceController> logger;
        private readonly IVehicleManager vehicleManager;
        private readonly IOrganizationManager organizationManager;
        public IConfiguration Configuration { get; }
        public VehicleDataserviceController(IVehicleManager _vehicleManager, ILogger<VehicleDataserviceController> _logger, IOrganizationManager _organizationManager, IConfiguration configuration)
        {
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
                logger.LogInformation("UpdateVehicle function called - " + vehicleData.VehicleUpdatedEvent?.Vehicle?.VehicleID?.VIN);

                VehicleProperty vehicleProperties = new VehicleProperty();
                if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID != null)
                {
                    //Vehicle ID
                    vehicleProperties.VIN = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.VIN.Trim();
                    vehicleProperties.License_Plate_Number = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.LicensePlate?.Trim();
                    if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.ManufactureDate))
                    {
                        if (Common.Common.IsValidDate(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.ManufactureDate))
                        {
                            vehicleProperties.ManufactureDate = Convert.ToDateTime(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.ManufactureDate);
                        }
                        else
                        {
                            return StatusCode(400, string.Empty);
                        }
                    }
                }
                else
                {
                    return StatusCode(400, string.Empty);
                }

                if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification != null)
                {
                    //Vehicle Classification
                    vehicleProperties.Classification_Make = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Make?.Trim();
                    if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Series != null)
                    {
                        vehicleProperties.Classification_Series_Id = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Series.ID?.Trim();
                        if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Series.vehicleRange != "LF" && vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Series.vehicleRange != "XF" && vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Series.vehicleRange != "CF" && vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Series.vehicleRange != null)
                        {
                            return StatusCode(400, string.Empty);
                        }
                        else
                        {
                            vehicleProperties.Classification_Series_VehicleRange = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Series.vehicleRange?.Trim();
                        }
                    }

                    vehicleProperties.Classification_Model_Id = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Model?.ID?.Trim();
                    vehicleProperties.Classification_ModelYear = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.ModelYear?.Trim();
                    vehicleProperties.Classification_Type_Id = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleClassification.Type?.ID?.Trim();
                }

                if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure != null)
                {
                    //Vehicle Named Structure
                    //Chassis
                    if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis != null)
                    {
                        vehicleProperties.Chassis_Id = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.ID?.Trim();

                        //Fuel Tank
                        vehicleProperties.VehicleFuelTankProperties = new List<VehicleFuelTankProperties>();
                        if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.FuelTanks != null)
                        {
                            if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.FuelTanks.Tank != null)
                            {
                                foreach (var tank in vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.FuelTanks.Tank)
                                {
                                    vehicleProperties.VehicleFuelTankProperties.Add(new VehicleFuelTankProperties()
                                    {
                                        Chassis_Tank_Nr = tank?.nr?.Trim(),
                                        Chassis_Tank_Volume = tank?.Volume?.Trim()
                                    });
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.SideSkirts))
                        {
                            vehicleProperties.Chassis_SideSkirts = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.SideSkirts.Trim().ToLower() == "yes"
                                                                    ? "Yes" : "No";
                        }
                        if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.SideCollars))
                        {
                            vehicleProperties.Chassis_SideCollars = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.SideCollars.Trim().ToLower() == "yes"
                                                                    ? "Yes" : "No";
                        }
                        vehicleProperties.Chassis_RearOverhang = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Chassis.RearOverhang?.Trim();
                    }

                    //Engine
                    vehicleProperties.Engine_ID = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine?.ID?.Trim();
                    vehicleProperties.Engine_Type = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine?.Type?.Trim();
                    vehicleProperties.Engine_Power = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine?.Power?.Trim();
                    vehicleProperties.Engine_Coolant = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine?.Coolant?.Trim();
                    vehicleProperties.Engine_EmissionLevel = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine?.EmissionLevel?.Trim();
                    vehicleProperties.Fuel = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Engine?.Fuel?.Trim();

                    //Transmission
                    //GearBox
                    vehicleProperties.GearBox_Id = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Transmission?.GearBox?.ID?.Trim();
                    vehicleProperties.GearBox_Type = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Transmission?.GearBox?.Type?.Trim();

                    if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine != null)
                    {
                        //DriveLine
                        vehicleProperties.DriverLine_AxleConfiguration = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.AxleConfiguration?.Trim();
                        vehicleProperties.DriverLine_Wheelbase = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.WheelBase?.Trim();
                        vehicleProperties.DriverLine_Tire_Size = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.Wheels?.Tire?.Size?.Trim();

                        //Front Axel
                        vehicleProperties.VehicleAxelInformation = new List<VehicleAxelInformation>();
                        if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.FrontAxle != null)
                        {
                            foreach (var frontAxel in vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.FrontAxle)
                            {
                                VehicleAxelInformation vehicleFrontAxelInfo = new VehicleAxelInformation();
                                vehicleFrontAxelInfo.AxelType = vehicle.AxelType.FrontAxle;
                                vehicleFrontAxelInfo.Type = frontAxel.Type?.Trim();
                                vehicleFrontAxelInfo.Position = frontAxel.position?.Trim();
                                vehicleFrontAxelInfo.Springs = frontAxel.Springs?.Trim();
                                vehicleFrontAxelInfo.Size = frontAxel.AxleSpecificWheels?.Tire?.Size?.Trim();
                                vehicleFrontAxelInfo.Is_Wheel_Tire_Size_Replaced = true;

                                vehicleProperties.VehicleAxelInformation.Add(vehicleFrontAxelInfo);
                            }
                        }

                        //Rear Axel
                        if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.RearAxle != null)
                        {
                            foreach (var rearAxel in vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.DriveLine.RearAxle)
                            {
                                VehicleAxelInformation vehicleRearAxelInfo = new VehicleAxelInformation();
                                vehicleRearAxelInfo.AxelType = vehicle.AxelType.RearAxle;
                                vehicleRearAxelInfo.Ratio = rearAxel.Ratio?.Trim();
                                vehicleRearAxelInfo.Load = rearAxel.Load?.Trim();
                                vehicleRearAxelInfo.Position = rearAxel.position?.Trim();
                                vehicleRearAxelInfo.Springs = rearAxel.Springs?.Trim();
                                vehicleRearAxelInfo.Size = rearAxel.AxleSpecificWheels?.Tire?.Size?.Trim();
                                vehicleRearAxelInfo.Is_Wheel_Tire_Size_Replaced = true;

                                vehicleProperties.VehicleAxelInformation.Add(vehicleRearAxelInfo);
                            }
                        }
                    }

                    //Cabin
                    vehicleProperties.DriverLine_Cabin_ID = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Cabin?.ID?.Trim();
                    vehicleProperties.DriverLine_Cabin_RoofSpoiler = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Cabin?.RoofSpoiler?.Trim();
                    vehicleProperties.DriverLine_Cabin_Type = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.Cabin?.Type?.Trim();

                    //ElectronicControlUnits
                    vehicleProperties.DriverLine_ElectronicControlUnit_Type = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.ElectronicControlUnits?.ElectronicControlUnit?.type?.Trim();
                    vehicleProperties.DriverLine_ElectronicControlUnit_Name = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleNamedStructure.ElectronicControlUnits?.ElectronicControlUnit?.Name?.Trim();
                }

                if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions != null)
                {
                    //VehicleDimensions
                    vehicleProperties.Dimensions_Size_Length = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions?.Size?.Length?.Trim();
                    vehicleProperties.Dimensions_Size_Width = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions?.Size?.Width?.Trim();
                    vehicleProperties.Dimensions_Size_Height = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions?.Size?.Height?.Trim();
                    vehicleProperties.Dimensions_Size_Weight_Type = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Weights?.Weight?.type?.Trim();
                    vehicleProperties.Dimensions_Size_Weight_Value = vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDimensions.Weights?.Weight?.value?.Trim();
                }
                //VehicleDelivery
                if (vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDelivery != null)
                {
                    if (!string.IsNullOrEmpty(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDelivery.DeliveryDate))
                    {
                        if (Common.Common.IsValidDate(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDelivery.DeliveryDate))
                        {
                            vehicleProperties.DeliveryDate = Convert.ToDateTime(vehicleData.VehicleUpdatedEvent.Vehicle.VehicleDelivery.DeliveryDate);
                        }
                        else
                        {
                            return StatusCode(400, string.Empty);
                        }
                    }
                }

                VehicleProperty vehiclePro = await vehicleManager.UpdateProperty(vehicleProperties);

                // Create owner realtionship
                int IsVehicleIdExist = await organizationManager.IsOwnerRelationshipExist(vehicleProperties.VehicleId);
                if (IsVehicleIdExist <= 0)
                {
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
                }

                logger.LogInformation("Vehicle Properties updated with VIN - " + vehicleData.VehicleUpdatedEvent.Vehicle.VehicleID.VIN);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while processing Vehicle data.");
                return StatusCode(500, string.Empty);
            }
        }
    }
}
