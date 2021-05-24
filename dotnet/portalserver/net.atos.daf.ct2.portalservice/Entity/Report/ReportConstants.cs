using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public static class ReportConstants
    {
        public const string USER_PREFERENCE_SUCCESS_MSG = "User preferences fetched successfully for account Id:- {0} and for report Id:- {1}.";
        public const string USER_PREFERENCE_FAILURE_MSG = "User preferences fetched failed for account Id:- {0} and for report Id:- {1}. Error: {2}";
        public const string USER_PREFERENCE_FAILURE_MSG2 = "No records found for reprot data columns.";

        public const string GET_VIN_SUCCESS_MSG = "VIN fetched successfully for given date range";
        public const string GET_VIN_FAILURE_MSG = "VIN fetched failed for given date range";

        public const string REPORT_REQUIRED_MSG = "Report id is required.";
        public const string ACCOUNT_REQUIRED_MSG = "Account id is required.";
        public const string ORGANIZATION_REQUIRED_MSG = "Organization id is required.";
    }
}
