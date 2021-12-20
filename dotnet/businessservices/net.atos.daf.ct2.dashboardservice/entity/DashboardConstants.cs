﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.dashboardservice.entity
{
    public static class DashboardConstants
    {
        public const string INTERNAL_SERVER_MSG = "Error occured, Please check with DAF IT team for more info.";
        public const string GET_FLEETKPI_DETAILS_SUCCESS_MSG = "Fleet KPI details fetched successfully.";
        public const string GET_FLEETKPI_DETAILS_FAILURE_MSG = "Fleet KPI details fetched failed. With Error : {0}";

        public const string GET_ALERTLAST_24HOURS_SUCCESS_MSG = "Alert last 24hours details fetched successfully.";
        public const string GET_ALERTLAST_24HOURS_FAILURE_MSG = "Alert last 24hours details failed. With Error : {0}";

        public const string GET_TODAY_LIVE_VEHICLE_SUCCESS_MSG = "Today live vehicle details fetched successfully.";
        public const string GET_TODAY_LIVE_VEHICLE_SUCCESS_NODATA_MSG = "No data found for Today live vehicle details.";
        public const string GET_TODAY_LIVE_VEHICLE_FAILURE_MSG = "Today live vehicle details fetched failed. With Error : {0}";

        public const string GET_FLEETUTILIZATION_DETAILS_SUCCESS_MSG = "Fleet KPI details fetched successfully.";
        public const string GET_FLEETUTILIZATION_DETAILS_FAILURE_MSG = "Fleet KPI details fetched failed. With Error : {0}";


        public const string GET_VIN_SUCCESS_MSG = "VIN fetched successfully for given date range of 90 days";
        public const string GET_VIN_FAILURE_MSG = "VIN fetched for given date range of 90 days";
        public const string GET_VIN_VISIBILITY_FAILURE_MSG = "No vehicle found for Account Id {0} and Organization Id {1}";
        public const string GET_VIN_TRIP_NOTFOUND_MSG = "No trip for vehicle found for Account Id {0} and Organization Id {1}. for last 90 days.";
        public const string GET_VIN_TRIP_NORESULTFOUND_MSG = "No Result Found";
        public const string NORESULTFOUND_MSG = "No Result Found";

        //User Preference create
        public const string USER_PREFERENCE_CREATE_SUCCESS_MSG = "User preferences Saved successfully for account Id:- {0} and for report Id:- {1}.";
        public const string USER_PREFERENCE_CREATE_FAILURE_MSG = "Saving User Preference failed for account Id:- {0} and for report Id:- {1}.";
        public const string CHECK_SUB_REPORT_EXIST_SUCCESS_MSG = "Sub report fetched successfully.";

    }
}
