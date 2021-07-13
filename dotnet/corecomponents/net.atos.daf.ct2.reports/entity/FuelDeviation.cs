﻿using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class FuelDeviation
    {
        #region trip statistic and Vehicle fields  
        public int Id { get; set; }
        public string TripId { get; set; }
        public string VIN { get; set; }
        public string RegistrationNo { get; set; }
        public string VehicleName { get; set; }

        public long StartTimeStamp { get; set; }
        public long EndTimeStamp { get; set; }

        public int Distance { get; set; }
        public int IdleDuration { get; set; }

        public int AverageSpeed { get; set; }
        public int AverageWeight { get; set; }

        public string StartPosition { get; set; }
        public string EndPosition { get; set; }

        public double FuelConsumed { get; set; }
        public int DrivingTime { get; set; }

        public int Alerts { get; set; }

        #endregion

        #region livefleet_trip_fuel_deviation    
        public char FuelEventType { get; set; }
        public char VehicleActivityType { get; set; }
        public long EventTime { get; set; }
        public double FuelDiffernce { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long Odometer { get; set; }
        public int GeoLocationAddressId { get; set; }
        public string GeoLocationAddress { get; set; }
        #endregion
    }

    public class FuelDeviationFilter
    {
        public long StartDateTime { get; set; }
        public long EndDateTime { get; set; }
        public IEnumerable<string> VINs { get; set; }
    }

}
