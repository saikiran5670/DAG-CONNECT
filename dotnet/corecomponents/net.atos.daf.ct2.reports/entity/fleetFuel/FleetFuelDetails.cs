using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity.fleetFuel
{
    public class FleetFuelDetails
    {
        public string VehicleName { get; set; }
        public string VIN { get; set; }
        public string VehicleRegistrationNo { get; set; }
        public string Distance { get; set; }
        public string AverageDistancePerDay { get; set; }
        public string AverageSpeed { get; set; }
        public string MaxSpeed { get; set; }
        public string NumberOfTrips { get; set; }
        public string AverageGrossWeightComb { get; set; }
        public string FuelConsumed { get; set; }
        public string FuelConsumption { get; set; }
        public string CO2Emission { get; set; }
        public string IdleDuration { get; set; }
        public string PTODuration { get; set; }
        public string HarshBrakeDuration { get; set; }
        public string HeavyThrottleDuration { get; set; }
        public string CruiseControlDistance30_50 { get; set; }
        public string CruiseControlDistance50_75 { get; set; }
        public string CruiseControlDistance75 { get; set; }
        public string AverageTrafficClassification { get; set; }
        public string CCFuelConsumption { get; set; }
        public string FuelconsumptionCCnonactive { get; set; }
        public string IdlingConsumption { get; set; }
        public string DPAScore { get; set; }
        public string DPAAnticipationScore { get; set; }
        public string DPABrakingScore { get; set; }
        public string IdlingPTOScore { get; set; }
        public string IdlingPTO { get; set; }
        public string IdlingWithoutPTO { get; set; }
        public string IdlingWithoutPTOpercent { get; set; }
        public string FootBrake { get; set; }
        public string CO2Emmision { get; set; }

    }
}
