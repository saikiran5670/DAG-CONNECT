using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.dashboardservice.entity
{
    public static class DashboardConstants
    {
        public const string GET_FLEETKPI_DETAILS_SUCCESS_MSG = "Fleet KPI details fetched successfully.";
        public const string GET_FLEETKPI_DETAILS_FAILURE_MSG = "Fleet KPI details fetched failed. With Error : {0}";

        public const string GET_ALERTLAST_24HOURS_SUCCESS_MSG = "Alert last 24hours details fetched successfully.";
        public const string GET_ALERTLAST_24HOURS_FAILURE_MSG = "Alert last 24hours details failed. With Error : {0}";

        public const string GET_TODAY_LIVE_VEHICLE_SUCCESS_MSG = "Today live vehicle details fetched successfully.";
        public const string GET_TODAY_LIVE_VEHICLE_FAILURE_MSG = "Today live vehicle details fetched failed. With Error : {0}";
    }
}
