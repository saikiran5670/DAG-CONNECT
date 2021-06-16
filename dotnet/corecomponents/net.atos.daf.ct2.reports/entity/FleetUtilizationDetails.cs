using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class FleetUtilizationDetails
    {
        public string VehicleName { get; set; }
        public string VIN { get; set; }
        public string RegistrationNumber { get; set; }
        public double Distance { get; set; }
        public int NumberOfTrips { get; set; }
        public long TripTime { get; set; }
        public long DrivingTime { get; set; }
        public long IdleDuration { get; set; }
        public long StopTime { get; set; }
        public double AverageDistancePerDay { get; set; }
        public double AverageSpeed { get; set; }
        public double AverageWeightPerTrip { get; set; }
        public long Odometer { get; set; }


    }
}
