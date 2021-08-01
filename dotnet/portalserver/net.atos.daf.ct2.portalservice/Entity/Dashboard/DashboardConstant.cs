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
    }
}
