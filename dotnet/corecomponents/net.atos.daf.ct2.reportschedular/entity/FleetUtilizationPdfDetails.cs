using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    class FleetUtilizationPdfDetails
    {
        public string VehicleName { get; set; }
        public string VIN { get; set; }
        public string RegistrationNumber { get; set; }
        public double Distance { get; set; }
        public int NumberOfTrips { get; set; }
        public string TripTime { get; set; }
        public string DrivingTime { get; set; }
        public string IdleDuration { get; set; }
        public string StopTime { get; set; }        
        public double AverageSpeed { get; set; }
        public double AverageWeightPerTrip { get; set; }
        public double AverageDistancePerDay { get; set; }
        public long Odometer { get; set; }
    }
}
