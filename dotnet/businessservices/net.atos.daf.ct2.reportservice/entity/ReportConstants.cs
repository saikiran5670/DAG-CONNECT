namespace net.atos.daf.ct2.reportservice.entity
{
    public static class ReportConstants
    {
        public const string USER_PREFERENCE_SUCCESS_MSG = "User preferences fetched successfully for account Id:- {0} and for report Id:- {1}.";
        public const string USER_PREFERENCE_FAILURE_MSG = "User preferences fetched failed for account Id:- {0} and for report Id:- {1}. Error: {2}";
        public const string USER_PREFERENCE_FAILURE_MSG2 = "No records found for reprot data columns.";

        public const string USER_PREFERENCE_CREATE_SUCCESS_MSG = "User preferences Saved successfully for account Id:- {0} and for report Id:- {1}.";
        public const string USER_PREFERENCE_CREATE_FAILURE_MSG = "Saving User Preference failed for account Id:- {0} and for report Id:- {1}.";

        public const string GET_VIN_SUCCESS_MSG = "VIN fetched successfully for given date range of 90 days";
        public const string GET_VIN_FAILURE_MSG = "VIN fetched for given date range of 90 days";
        public const string GET_VIN_VISIBILITY_FAILURE_MSG = "No vehicle found for Account Id {0} and Organization Id {1}";
        public const string GET_VIN_TRIP_NOTFOUND_MSG = "No trip for vehicle found for Account Id {0} and Organization Id {1}. for last 90 days.";
        public const string GET_VIN_TRIP_NORESULTFOUND_MSG = "No Result Found";

        public const string GET_REPORT_DETAILS_SUCCESS_MSG = "Report details fetched successfully.";
        public const string GET_REPORT_DETAILS_FAILURE_MSG = "Report details fetched failed. With Error : {0}";

        #region Eco-Score Report
        public const string GET_ECOSCORE_PROFILE_SUCCESS_MSG = "Eco-Score profiles fetched successfully.";
        public const string GET_ECOSCORE_PROFILE_KPI_SUCCESS_MSG = "Eco-Score profile KPI details fetched successfully for given profile.";

        public const string GET_ECOSCORE_REPORT_SUCCESS_MSG = "Eco-Score Report details fetched successfully.";
        public const string GET_ECOSCORE_REPORT_FAILURE_MSG = "Eco-Score Report details fetched failed. With Error : {0}";
        public const string GET_ECOSCORE_REPORT_NOTFOUND_MSG = "No records found for Eco-Score reprot.";
        public const string UPDATE_ECOSCORE_PROFILE_SUCCESS_MSG = "Update successfully.";
        public const string ECOSCORE_PROFILE_ALREADY_EXIST_MSG = "Profile {0} already exist with the same name";
        public const string UPDATE_ECOSCORE_PROFILE_FAIL_MSG = "Update Eco Score Profile Fail.";
        public const string UPDATE_ECOSCORE_PROFILE_DEFAULT_PROFILE_MSG = "Is a default profile, Can't update.";
        public const string ECOSCORE_PROFILE_NOT_AUTH_MSG = "User is Not Authorize to update the profile.";

        public const string DELETE_ECOSCORE_PROFILE_SUCCESS_MSG = "Delete successfully.";
        public const string DELETE_ECOSCORE_PROFILE_NOT_EXIST_MSG = "does not exist to delete.";
        public const string DELETE_ECOSCORE_PROFILE_FAIL_MSG = "Delete Eco Score Profile Fail.";
        public const string DELETE_ECOSCORE_PROFILE_DEFAULT_PROFILE_MSG = "Is a default profile, Can't delete.";
        public const string DELETE_ECOSCORE_PROFILE_GLOBAL_PROFILE_MSG = "Is a global profile, Can't delete.";
        #endregion

        #region FleetOverview
        public const string FLEETOVERVIEW_FEATURE_NAME = "FleetOverview";
        public const string FLEETOVERVIEW_FILTER_SUCCESS_MSG = "Filter details fetched successfully.";
        #endregion
    }
}
