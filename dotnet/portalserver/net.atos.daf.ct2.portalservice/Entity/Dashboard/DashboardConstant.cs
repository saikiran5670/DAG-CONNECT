using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Dashboard
{
    public static class DashboardConstant
    {
        public const string GET_DASBHOARD_VALIDATION_STARTDATE_MSG = "Invalid start date.";
        public const string GET_DASBHOARD_VALIDATION_ENDDATE_MSG = "Invalid end date.";
        public const string GET_DASBHOARD_VALIDATION_VINREQUIRED_MSG = "Invalid VIN details.";
        public const string GET_DASBHOARD_VALIDATION_DATEMISMATCH_MSG = "Start Date should be less than End Date.";
        public const string GET_DASBHOARD_SUCCESS_MSG = "Dashboard details fetched successfully";
        public const string GET_DASBHOARD_FAILURE_MSG = "No Result Found";

        public const string GET_ALERTLAST24HOURS_VALIDATION_VINREQUIRED_MSG = "Invalid VIN details.";
        public const string GET_ALERTLAST24HOURS_SUCCESS_MSG = "Alert last 24hours details fetched successfully";
        public const string GET_ALERTLAST24HOURS_FAILURE_MSG = "No Result Found";

        public const string GET_TODAY_LIVE_VEHICLE_SUCCESS_MSG = "Dashboard details fetched successfully";
        public const string GET_TODAY_LIVE_VEHICLE_SUCCESS_MSG_VALIDATION_VINREQUIRED_MSG = "Invalid VIN details.";

        public const string GET_VIN_SUCCESS_MSG = "VIN fetched successfully for given date range";
        public const string GET_VIN_FAILURE_MSG = "VIN fetched failed for given date range";

        public const string REPORT_REQUIRED_MSG = "Report id is required.";
        public const string ACCOUNT_REQUIRED_MSG = "Account id is required.";
        public const string ORGANIZATION_REQUIRED_MSG = "Organization id is required.";
        public const string GET_VIN_VISIBILITY_FAILURE_MSG2 = "Error fetching VIN and Vehicla Details for Account Id {0} and Organization Id {1}. With error: {2}.";
    }
}
