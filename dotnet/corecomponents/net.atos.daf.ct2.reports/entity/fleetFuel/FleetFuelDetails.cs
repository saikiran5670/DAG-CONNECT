using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class FleetFuelDetails
    {
        public string VehicleName { get; set; }
        public string Tripid { get; set; }
        public string VIN { get; set; }
        public string VehicleRegistrationNo { get; set; }
        public long Distance { get; set; }
        public long AverageDistancePerDay { get; set; }
        public double AverageSpeed { get; set; }
        public int MaxSpeed { get; set; }
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
        public string AverageTrafficClassificationValue { get; set; } = string.Empty;
        public double CCFuelConsumption { get; set; }
        public double FuelconsumptionCCnonactive { get; set; }
        public int IdlingConsumption { get; set; }
        public string IdlingConsumptionValue { get; set; } = string.Empty;
        public double DPAScore { get; set; }

        public string DriverID { get; set; }
        public string DriverName { get; set; }

        public List<LiveFleetPosition> LiveFleetPosition { get; set; }
        //public string DPAAnticipationScore { get => DPAAnticipationScore = string.Empty; set => DPAAnticipationScore = string.Empty; }
        //public string DPABrakingScore { get => DPAAnticipationScore = string.Empty; set => DPAAnticipationScore = string.Empty; }
        //public string IdlingPTOScore { get => DPAAnticipationScore = string.Empty; set => DPAAnticipationScore = string.Empty; }
        //public string IdlingPTO { get => DPAAnticipationScore = string.Empty; set => DPAAnticipationScore = string.Empty; }
        //public string IdlingWithoutPTO { get => DPAAnticipationScore = string.Empty; set => DPAAnticipationScore = string.Empty; }
        //public string IdlingWithoutPTOpercent { get => DPAAnticipationScore = string.Empty; set => DPAAnticipationScore = string.Empty; }
        //public string FootBrake { get => DPAAnticipationScore = string.Empty; set => DPAAnticipationScore = string.Empty; }
        //public string CO2Emmision { get => DPAAnticipationScore = string.Empty; set => DPAAnticipationScore = string.Empty; }

    }
}
