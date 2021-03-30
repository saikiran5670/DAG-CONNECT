using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.vehicledataservice.Entity
{
    
    public class VehicleID
    {
        public string VIN { get; set; }
        public string LicensePlate { get; set; }
        public string ManufactureDate { get; set; }
    }

    public class VehicleClassification
    {
        public string Make { get; set; }
        public Series Series { get; set; }
        public Model Model { get; set; }
        public string ModelYear{ get; set;}
        public Type Type { get; set; }
    }

    public class Series
    {
        public string ID { get; set; } 
        public string vehicleRange{get; set;}
    }

    public class Model
    {
        public string ID { get; set; } 
    }
    public class Type
    {
        public string ID { get; set; } 
    }

    public class VehicleNamedStructure
    {
        public Chassis Chassis { get; set; }
        public Engine Engine { get; set; }
        public Transmission Transmission { get; set; }
        public DriveLine DriveLine { get; set; }
        public Cabin Cabin { get; set; }
        public ElectronicControlUnits ElectronicControlUnits {get; set;}
    }
    public class Chassis
    {
        public string ID { get; set; }
        public FuelTanks FuelTanks { get; set; }
        public string SideSkirts { get; set; }
        public string SideCollars { get; set; }
        public string RearOverhang { get; set; }
    }
    public class Tank
    {
        public string nr { get; set; }
        public string Volume { get; set; }
    }

    public class FuelTanks
    {
        public Tank[] Tank { get; set; }
    }    

    public class Engine
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string Power { get; set; }
        public string Coolant { get; set; }
        public string EmissionLevel { get; set; }
        public string Fuel { get; set; }
    }
    public class Transmission
    {
        public GearBox GearBox { get; set; }
    }

    public class GearBox
    {
        public string ID { get; set; }
        public string Type { get; set; }
    }

   
    public class Tire
    {
        public string Size { get; set; }
    }

    public class Wheels
    {
        public Tire Tire { get; set; }
    }

    public class FrontAxle
    {
        public string position { get; set; }
        public string Type { get; set; }
        public string Springs {get; set;}        
        public AxleSpecificWheels AxleSpecificWheels {get; set;}
    }

    public class RearAxle
    {
        public string position { get; set; }
        public string Load { get; set; }
        public string Ratio { get; set; }
        public string Springs {get; set;}
        public AxleSpecificWheels AxleSpecificWheels {get; set;}
    }

    public class AxleSpecificWheels
    {
        public Tire Tire { get; set; }
    }

    public class DriveLine
    {
        public string AxleConfiguration { get; set; }
        public string WheelBase { get; set; }
        public Wheels Wheels { get; set; }
        public FrontAxle[] FrontAxle { get; set; }
        public RearAxle[] RearAxle { get; set; }
    }

   

    public class Cabin
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string RoofSpoiler { get; set; }
    }

    public class ElectronicControlUnits
    {
        public ElectronicControlUnit ElectronicControlUnit {get; set;}
    }
    public class ElectronicControlUnit
    {
        public string type {get; set;}
        public string Name {get; set;}
    }

    public class Size
    {
        public string Length { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
    }

    public class Weights
    {
        public Weight Weight { get; set; }
    }
    public class Weight
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class VehicleDimensions
    {
        public Size Size { get; set; }
        public Weights Weights { get; set; }
    }

    public class VehicleDelivery
    {
        public string DeliveryDate { get; set; }
       
    }

    public class Vehicle
    {
        public VehicleID VehicleID { get; set; }
        public VehicleClassification VehicleClassification { get; set; }
        public VehicleNamedStructure VehicleNamedStructure { get; set; }
        public VehicleDimensions VehicleDimensions { get; set; }
        public VehicleDelivery VehicleDelivery { get; set; }
    }
    public class VehicleUpdatedEvent    {
        public Vehicle Vehicle { get; set; } 
    }

    public class Root  {
        public VehicleUpdatedEvent VehicleUpdatedEvent { get; set; } 
    }


}
