using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class TripReportDetails
    {
        [DisplayName("VIN")]
        public string VIN { get; set; }

        [DisplayName("Start Date")]
        public long StartTimeStamp { get; set; }
        [DisplayName("End Date")]
        public long EndTimeStamp { get; set; }

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
