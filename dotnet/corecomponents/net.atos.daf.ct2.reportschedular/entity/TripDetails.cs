using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class TripReportDetails
    {

        public string VIN { get; set; }

        public long StartTimeStamp { get; set; }

        public long EndTimeStamp { get; set; }

        public int Distance { get; set; }

        public int IdleDuration { get; set; }

        public int AverageSpeed { get; set; }

        public int AverageWeight { get; set; }

        public long Odometer { get; set; }

        public string StartPosition { get; set; }

        public string EndPosition { get; set; }

        public double FuelConsumed { get; set; }

        public int DrivingTime { get; set; }

        public int Alert { get; set; }

        public int Events { get; set; }

        public double FuelConsumed100km { get; set; }
    }

    public class TripReportPdfDetails
    {
        [DisplayName("VIN")]
        public string VIN { get; set; }
        [DisplayName("Start Date")]
        public string StartDate { get; set; }
        [DisplayName("End Date")]
        public string EndDate { get; set; }
        [DisplayName("Distance")]
        public int Distance { get; set; }
        [DisplayName("Idle Duration")]
        public int IdleDuration { get; set; }
        [DisplayName("Average Speed")]
        public int AverageSpeed { get; set; }
        [DisplayName("Average Weight")]
        public int AverageWeight { get; set; }
        [DisplayName("Odometer")]
        public long Odometer { get; set; }
        [DisplayName("Start Position")]
        public string StartPosition { get; set; }
        [DisplayName("End Position")]
        public string EndPosition { get; set; }
        [DisplayName("Fuel Consumed")]
        public double FuelConsumed { get; set; }
        [DisplayName("Driving Time")]
        public int DrivingTime { get; set; }
        [DisplayName("Alerts")]
        public int Alert { get; set; }
        [DisplayName("Distance")]
        public int Events { get; set; }
        [DisplayName("Fuel Consumed 100km")]
        public double FuelConsumed100km { get; set; }
    }
}
