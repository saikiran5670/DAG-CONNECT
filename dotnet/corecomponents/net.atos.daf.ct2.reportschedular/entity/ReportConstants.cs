using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{

    public static class ReportNameConstants
    {
        public const string REPORT_TRIP = "lblTripReport";
        public const string REPORT_SCHEDULE = "lblScheduleReport";
        public const string REPORT_FLEET_UTILISATION = "lblFleetUtilisation";
        public const string REPORT_FUEL_DEVIATION = "lblFuelDeviationReport";
        public const string REPORT_FLEET_FUEL = "lblFleetFuelReport";
    }

    public static class FormatConstants
    {
        public const string DATE_FORMAT = "dd/MM/yyyy";
        public const string TIME_FORMAT_24 = "HH:mm:ss";
        public const string TIME_FORMAT_12 = "hh:mm:ss tt";
        public const string TIME_FORMAT_LABLE = "dtimeformat_24Hours";
        public const string UNIT_DEFAULT_LABLE = "dunit_Metric";
        public const string UNIT_METRIC_KEY = "dunit_Metric";
        public const string UNIT_IMPERIAL_KEY = "dunit_Imperial";
    }

    public static class TimeConstants
    {
        public const string UTC = "UTC";
    }

    public static class IdlingConsumptionConstants
    {
        public const string GOOD = "Good";
        public const string VERY_GOOD = "Very Good";
        public const string MODERATE = "Moderate";
    }

    public static class DPAScoreConstants
    {
        public const string LIGHT = "Light";
        public const string MEDIUM = "Medium";
        public const string HIGH = "High";
    }

    public static class CreationConstants
    {
        public const string LOG_SQL_TIMEOUT = "ReportCreationSqlTimeout";
        public const string LOG_MSG = "ReportCreationScheduler";
        public const string LOG_UNSUBSCRIBED = "ReportCreationUnSubscribed";
    }
}
