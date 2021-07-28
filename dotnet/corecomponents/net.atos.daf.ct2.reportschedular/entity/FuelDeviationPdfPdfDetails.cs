using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class FuelDeviationPdfPdfDetails
    {
        public string Type { get; set; }
        public string VehicleName { get; set; }
        public string VIN { get; set; }
        public string RegistrationNo { get; set; }
        public double FuelDiffernce { get; set; }
        public string EventTime { get; set; }
        public double Odometer { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public double Distance { get; set; }
        public string IdleDuration { get; set; }

        public double AverageSpeed { get; set; }
        public double AverageWeight { get; set; }

        public string StartPosition { get; set; }
        public string EndPosition { get; set; }
        public double FuelConsumed { get; set; }
        public string DrivingTime { get; set; }
        public int Alerts { get; set; }
    }
}
