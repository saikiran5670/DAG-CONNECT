using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{

    public static class ReportNameConstants
    {
        public const string REPORT_TRIP = "lblTripReport";
        public const string REPORT_SCHEDULE = "lblScheduleReport";
        public const string REPORT_TRIP_TRACING = "lblTripTracing";
        public const string REPORT_FLEET_UTILISATION = "lblFleetUtilisation";
    }

    public static class FormatConstants
    {
        public const string DATE_FORMAT = "dd/MM/yyyy";
        public const string TIME_FORMAT_24 = "HH:mm:ss";
        public const string TIME_FORMAT_12 = "hh:mm:ss";
        public const string TIME_FORMAT_LABLE = "dtimeformat_24Hours";
        public const string UNIT_DEFAULT_LABLE = "dunit_Metric";
        public const string UNIT_METRIC_KEY = "dunit_Metric";
        public const string UNIT_IMPERIAL_KEY = "dunit_Imperial";
    }

    public static class TimeConstants
    {
        public const string UTC = "UTC";
    }
}
