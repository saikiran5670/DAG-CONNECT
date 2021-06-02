using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.vehicledataservice.Entity
{
    public class VehicleID
    {
        [Required]
        [StringLength(17, MinimumLength = 17)]
        public string VIN { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string LicensePlate { get; set; }
        public string ManufactureDate { get; set; }
    }

    public class VehicleClassification
    {
        [StringLength(50, MinimumLength = 0)]
        public string Make { get; set; }
        public Series Series { get; set; }
        public Model Model { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string ModelYear { get; set; }
        public Type Type { get; set; }
    }

    public class Series
    {
        [StringLength(50, MinimumLength = 0)]
        public string ID { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string VehicleRange { get; set; }
    }

    public class Model
    {
        [StringLength(50, MinimumLength = 0)]
        public string ID { get; set; }
    }
    public class Type
    {
        [StringLength(50, MinimumLength = 0)]
        public string ID { get; set; }
    }

    public class VehicleNamedStructure
    {
        public Chassis Chassis { get; set; }
        public Engine Engine { get; set; }
        public Transmission Transmission { get; set; }
        public DriveLine DriveLine { get; set; }
        public Cabin Cabin { get; set; }
        public ElectronicControlUnits ElectronicControlUnits { get; set; }
    }
    public class Chassis
    {
        [StringLength(50, MinimumLength = 0)]
        public string ID { get; set; }
        public FuelTanks FuelTanks { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string SideSkirts { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string SideCollars { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string RearOverhang { get; set; }
    }

    public class Tank
    {
        [StringLength(50, MinimumLength = 0)]
        public string Nr { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string Volume { get; set; }
    }

    public class FuelTanks
    {
        public Tank[] Tank { get; set; }
    }

    public class Engine
    {
        [StringLength(50, MinimumLength = 0)]
        public string ID { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string Type { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string Power { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string Coolant { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string EmissionLevel { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string Fuel { get; set; }
    }

    public class Transmission
    {
        public GearBox GearBox { get; set; }
    }

    public class GearBox
    {
        [StringLength(50, MinimumLength = 0)]
        public string ID { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string Type { get; set; }
    }

    public class Tire
    {
        [StringLength(50, MinimumLength = 0)]
        public string Size { get; set; }
    }

    public class Wheels
    {
        public Tire Tire { get; set; }
    }

    public class FrontAxle
    {
        [StringLength(50, MinimumLength = 0)]
        public string Position { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string Type { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string Springs { get; set; }
        public AxleSpecificWheels AxleSpecificWheels { get; set; }
    }

    public class RearAxle
    {
        [StringLength(50, MinimumLength = 0)]
        public string Position { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string Load { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string Ratio { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string Springs { get; set; }
        public AxleSpecificWheels AxleSpecificWheels { get; set; }
    }

    public class AxleSpecificWheels
    {
        public Tire Tire { get; set; }
    }

    public class DriveLine
    {
        [StringLength(50, MinimumLength = 0)]
        public string AxleConfiguration { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string WheelBase { get; set; }
        public Wheels Wheels { get; set; }
        public FrontAxle[] FrontAxle { get; set; }
        public RearAxle[] RearAxle { get; set; }
    }

    public class Cabin
    {
        [StringLength(50, MinimumLength = 0)]
        public string ID { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string Type { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string RoofSpoiler { get; set; }
    }

    public class ElectronicControlUnits
    {
        public ElectronicControlUnit ElectronicControlUnit { get; set; }
    }
    public class ElectronicControlUnit
    {
        [StringLength(50, MinimumLength = 0)]
        public string Type { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string Name { get; set; }
    }

    public class Size
    {
        [StringLength(50, MinimumLength = 0)]
        public string Length { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string Width { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string Height { get; set; }
    }

    public class Weights
    {
        public Weight Weight { get; set; }
    }

    public class Weight
    {
        [StringLength(50, MinimumLength = 0)]
        public string Type { get; set; }
        [StringLength(50, MinimumLength = 0)]
        public string Value { get; set; }
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
    public class VehicleUpdatedEvent
    {
        [Required]
        public Vehicle Vehicle { get; set; }
    }

    public class Root
    {
        [Required]
        public VehicleUpdatedEvent VehicleUpdatedEvent { get; set; }
    }
}
