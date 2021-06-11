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

        public const string GET_REPORT_DETAILS_SUCCESS_MSG = "Report details fetched successfully.";
        public const string GET_REPORT_DETAILS_FAILURE_MSG = "Report details fetched failed. With Error : {0}";

        #region Trip Details Report
        public const string GET_TRIP_VALIDATION_STARTDATE_MSG = "Invalid start date.";
        public const string GET_TRIP_VALIDATION_ENDDATE_MSG = "Invalid end date.";
        public const string GET_TRIP_VALIDATION_VINREQUIRED_MSG = "Invalid VIN details.";
        public const string GET_TRIP_VALIDATION_DATEMISMATCH_MSG = "Start Date should be less than End Date.";

        public const string GET_TRIP_SUCCESS_MSG = "Trip fetched successfully for requested Filters";
        public const string GET_TRIP_FAILURE_MSG = "No Result Found";
        #endregion

        #region Driver Time Management
        public const string GET_DRIVER_TIME_VALIDATION_STARTDATE_MSG = "Invalid start date.";
        public const string GET_DRIVER_TIME_VALIDATION_ENDDATE_MSG = "Invalid end date.";
        public const string GET_DRIVER_TIME_VALIDATION_VINREQUIRED_MSG = "Invalid VIN details.";
        public const string GET_DRIVER_TIME_VALIDATION_IDREQUIRED_MSG = "Invalid driver id/ids details.";
        public const string GET_DRIVER_TIME_VALIDATION_DATEMISMATCH_MSG = "Start Date should be less than End Date.";

        public const string GET_DRIVER_TIME_SUCCESS_MSG = "Driver details fetched successfully for requested Filters";
        public const string GET_DRIVER_TIME_FAILURE_MSG = "No Result Found";
        #endregion

        public const string GET_VIN_VISIBILITY_FAILURE_MSG = "No vehicle found for Account Id {0} and Organization Id {1}";
        public const string GET_VIN_TRIP_NOTFOUND_MSG = "No trip for vehicle found for Account Id {0} and Organization Id {1}. for last 90 days.";
        public const string GET_VIN_VISIBILITY_FAILURE_MSG2 = "Error fetching VIN and Vehicla Details for Account Id {0} and Organization Id {1}. With error: {2}.";

        #region Eco-Score Report
        public const string GET_ECOSCORE_PROFILE_SUCCESS_MSG = "Eco-Score profiles fetched successfully.";
        public const string GET_ECOSCORE_PROFILE_KPI_SUCCESS_MSG = "Eco-Score profile KPI details fetched successfully for given profile.";
        public const string DELETE_ECOSCORE_PROFILE_KPI_SUCCESS_MSG = "Eco-Score profile Deleted successfully.";
        #endregion



    }
}
