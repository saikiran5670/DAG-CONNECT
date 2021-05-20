using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.rfms.responce
{

      public class ProductionDate

    {

        public int Day { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

    }

 

    public class Vehicle

    {

        public string Vin { get; set; }

        public string CustomerVehicleName { get; set; }

        public string Brand { get; set; }

        public ProductionDate ProductionDate { get; set; }

        public string Type { get; set; }

        public string Model { get; set; }

        public List<string> PossibleFuelType { get; set; }

        public string EmissionLevel { get; set; }

        public string TellTaleCode { get; set; }

        public string ChassisType { get; set; }

        public int NoOfAxles { get; set; }

        public int TotalFuelTankVolume { get; set; }

        public string TachographType { get; set; }

        public string GearboxType { get; set; }

        public string BodyType { get; set; }

        public List<int> DoorConfiguration { get; set; }

        public bool HasRampOrLift { get; set; }

        public List<string> AuthorizedPaths { get; set; }

    }

 

    public class VehicleResponse

    {

        public List<Vehicle> Vehicles { get; set; }

    }
    public class RfmsVehicles
    {
        public VehicleResponse VehicleResponse { get; set; }

        public bool MoreDataAvailable { get; set; }

        public string MoreDataAvailableLink { get; set; }
    }
}
