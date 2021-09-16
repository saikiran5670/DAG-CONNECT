﻿using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class FleetFuelDetails
    {
        public int Id { get; set; }
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
        public double CruiseControlDistance3050 { get; set; }
        public double CruiseControlDistance5075 { get; set; }
        public double CruiseControlDistance75 { get; set; }
        public double AverageTrafficClassification { get; set; }
        public string AverageTrafficClassificationValue { get; set; } = string.Empty;
        public double CCFuelConsumption { get; set; }
        public double FuelconsumptionCCnonactive { get; set; }
        public int IdlingConsumption { get; set; }
        public string IdlingConsumptionValue { get; set; } = string.Empty;
        public double DPAScore { get; set; }
        public List<LiveFleetPosition> LiveFleetPosition { get; set; }

        public long StartDate { get; set; }
        public long EndDate { get; set; }
        public double Startpositionlattitude { get; set; }
        public double Startpositionlongitude { get; set; }
        public double Endpositionlattitude { get; set; }
        public double Endpositionlongitude { get; set; }
        public int CCFuelDistance { get; set; }
        public double CCFuelConsumed { get; set; }
        public int CCFuelDistanceNotActive { get; set; }
        public double CCFuelConsumedNotActive { get; set; }
        public string StartPosition { get; set; }
        public string EndPosition { get; set; }
        public double IdleDurationPercentage { get; set; }
        //public string DPAAnticipationScore { get; set; } = string.Empty;
        //public string DPABrakingScore{ get; set; } = string.Empty;
        //public string IdlingPTOScore { get; set; } = string.Empty;
        //public string IdlingPTO { get; set; } = string.Empty;
        //public string IdlingWithoutPTO { get; set; } = string.Empty;
        //public string IdlingWithoutPTOpercent { get; set; } = string.Empty;
        //public string FootBrake { get; set; } = string.Empty;
        //public string CO2Emmision { get; set; } = string.Empty;

    }

    public class FleetFuelDetailsByDriver : FleetFuelDetails
    {
        public string DriverID { get; set; }
        public string DriverName { get; set; } = string.Empty;
    }
}
