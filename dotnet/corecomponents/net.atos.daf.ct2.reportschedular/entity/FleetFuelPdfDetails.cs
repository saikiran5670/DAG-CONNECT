using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class FleetFuelPdfDetails
    {
        public string VehicleName { get; set; }
        public string VIN { get; set; }
        public string VehicleRegistrationNo { get; set; }
        public double Distance { get; set; }
        public double AverageDistancePerDay { get; set; }
        public double AverageSpeed { get; set; }
        public double MaxSpeed { get; set; }
        public int NumberOfTrips { get; set; }
        public double AverageGrossWeightComb { get; set; }
        public double FuelConsumed { get; set; }
        public double FuelConsumption { get; set; }
        public double CO2Emission { get; set; }
        public double IdleDuration { get; set; }
        public double PTODuration { get; set; }
        public double HarshBrakeDuration { get; set; }
        public double HeavyThrottleDuration { get; set; }
        public double CruiseControlDistance30_50 { get; set; }
        public double CruiseControlDistance50_75 { get; set; }
        public double CruiseControlDistance75 { get; set; }
        public double AverageTrafficClassification { get; set; }
        public double CCFuelConsumption { get; set; }
        public double FuelconsumptionCCnonactive { get; set; }
        public string IdlingConsumption { get; set; }
        public string DPAScore { get; set; }
        //public string DPAAnticipationScore { get; set; } = string.Empty;
        //public string DPABrakingScore{ get; set; } = string.Empty;
        //public string IdlingPTOScore { get; set; } = string.Empty;
        //public string IdlingPTO { get; set; } = string.Empty;
        //public string IdlingWithoutPTO { get; set; } = string.Empty;
        //public string IdlingWithoutPTOpercent { get; set; } = string.Empty;
        //public string FootBrake { get; set; } = string.Empty;
        //public string CO2Emmision { get; set; } = string.Empty;
    }
}
