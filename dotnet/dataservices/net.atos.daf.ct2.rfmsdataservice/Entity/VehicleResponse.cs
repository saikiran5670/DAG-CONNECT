using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.rfmsdataservice.Entity
{
    public class ResponseObject
    {
        public long RequestTimestamp { get; set; }

        [DataMember(Name = "vehicleResponse")]
        public VehicleResponse VehicleResponse{ get; set; }

        [DataMember(Name = "moreDataAvailable")]
        public bool? MoreDataAvailable { get; set; }

        [DataMember(Name = "moreDataAvailableLink")]
        public string MoreDataAvailableLink { get; set; }
    }
    public class VehicleResponse
    {
        [DataMember(Name = "vehicles")]
        public List<Vehicle> Vehicles { get; set; }
    }

    public class Vehicle
    {
        [Required]
        [DataMember(Name = "vin")]
        public string Vin { get; set; }

        [DataMember(Name = "customerVehicleName")]
        public string CustomerVehicleName { get; set; }

        [DataMember(Name = "brand")]
        public string Brand { get; set; }

        [DataMember(Name = "productionDate")]
        public ProductionDate ProductionDate { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "model")]
        public string Model { get; set; }

        [DataMember(Name = "possibleFuelType")]
        public List<string> PossibleFuelType { get; set; }

        [DataMember(Name = "emissionLevel")]
        public string EmissionLevel { get; set; }

        [DataMember(Name = "tellTaleCode")]
        public string TellTaleCode { get; set; }

        [DataMember(Name = "chassisType")]
        public string ChassisType { get; set; }

        [DataMember(Name = "noOfAxles")]
        public int? NoOfAxles { get; set; }

        [DataMember(Name = "totalFuelTankVolume")]
        public int? TotalFuelTankVolume { get; set; }

        [DataMember(Name = "tachographType")]
        public string TachographType { get; set; }

        [DataMember(Name = "gearboxType")]
        public string GearboxType { get; set; }

        [DataMember(Name = "bodyType")]
        public string BodyType { get; set; }

        [DataMember(Name = "doorConfiguration")]
        public List<int> DoorConfiguration { get; set; }

        [DataMember(Name = "hasRampOrLift")]
        public bool? HasRampOrLift { get; set; }

        [DataMember(Name = "authorizedPaths")]
        public List<string> AuthorizedPaths { get; set; }
    }

    public class ProductionDate
    {
        [DataMember(Name = "day")]
        public int Day { get; set; }

        [DataMember(Name = "month")]
        public int Month { get; set; }

        [DataMember(Name = "year")]
        public int Year { get; set; }
    }
}
