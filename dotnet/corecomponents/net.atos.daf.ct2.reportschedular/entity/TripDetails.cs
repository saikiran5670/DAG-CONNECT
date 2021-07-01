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
        //[DisplayName("da_report_details_vin")]
        //public string VIN { get; set; }
        [DisplayName("da_report_details_startdate")]
        public string StartDate { get; set; }
        [DisplayName("da_report_details_enddate")]
        public string EndDate { get; set; }
        [DisplayName("da_report_details_distance")]
        public int Distance { get; set; }
        [DisplayName("da_report_details_idleduration")]
        public int IdleDuration { get; set; }
        [DisplayName("da_report_details_averagespeed")]
        public int AverageSpeed { get; set; }
        [DisplayName("da_report_details_averageweight")]
        public int AverageWeight { get; set; }
        [DisplayName("da_report_details_odometer")]
        public long Odometer { get; set; }
        [DisplayName("da_report_details_startposition")]
        public string StartPosition { get; set; }
        [DisplayName("da_report_details_endposition")]
        public string EndPosition { get; set; }
        //[DisplayName("da_report_details_fuelconsumed")]
        //public double FuelConsumed { get; set; }
        [DisplayName("Fuel Consumed 100km")]
        public double FuelConsumed100km { get; set; }
        [DisplayName("da_report_details_drivingtime")]
        public int DrivingTime { get; set; }
        [DisplayName("da_report_details_alerts")]
        public int Alerts { get; set; }
        [DisplayName("Events")]
        public int Events { get; set; }
    }
}
