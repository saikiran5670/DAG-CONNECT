namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public static class ReportConstants
    {
        public const string USER_PREFERENCE_FAILURE_MSG = "Error: {0}";

        public const string GET_VIN_SUCCESS_MSG = "VIN fetched successfully for given date range";
        public const string GET_VIN_FAILURE_MSG = "VIN fetched failed for given date range";

        public const string REPORT_REQUIRED_MSG = "Report id is required.";
        public const string ACCOUNT_REQUIRED_MSG = "Account id is required.";
        public const string ORGANIZATION_REQUIRED_MSG = "Organization id is required.";

        public const string GET_REPORT_DETAILS_SUCCESS_MSG = "Report details fetched successfully.";
        public const string GET_REPORT_DETAILS_FAILURE_MSG = "Report details fetched failed. With Error : {0}";

        public const string VALIDATION_STARTDATE_MSG = "Invalid start date.";
        public const string VALIDATION_ENDDATE_MSG = "Invalid end date.";
        public const string VALIDATION_VINREQUIRED_MSG = "Invalid or empty VIN details.";
        public const string VALIDATION_DATEMISMATCH_MSG = "Start Date should be less than End Date.";

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
        public const string GET_ECOSCORE_REPORT_VALIDATION_STARTDATE_MSG = "Invalid start date.";
        public const string GET_ECOSCORE_REPORT_VALIDATION_ENDDATE_MSG = "Invalid end date.";
        public const string GET_ECOSCORE_REPORT_VALIDATION_VINREQUIRED_MSG = "Invalid VIN details.";
        public const string GET_ECOSCORE_REPORT_VALIDATION_DATEMISMATCH_MSG = "Start Date should be less than End Date.";
        public const string GET_ECOSCORE_REPORT_SUCCESS_MSG = "Eco-Score Report details fetched successfully.";
        public const string GET_ECOSCORE_REPORT_FAILURE_MSG = "Eco-Score Report details fetched failed. With Error : {0}";
        public const string GET_ECOSCORE_REPORT_NOTFOUND_MSG = "No records found for Eco-Score reprot.";
        public const string GET_ECOSCORE_REPORT_VALIDATION_COMPAREDRIVER_MSG = "Please select minimum 2 or maximum 4 drivers for comparison.";
        #endregion

        #region Fleet utilization Report
        public const string GET_FLEET_UTILIZATION_VALIDATION_STARTDATE_MSG = "Invalid start date.";
        public const string GET_FLEET_UTILIZATION_VALIDATION_ENDDATE_MSG = "Invalid end date.";
        public const string GET_FLEET_UTILIZATION_VALIDATION_VINREQUIRED_MSG = "Invalid VIN details.";
        public const string GET_FLEET_UTILIZATION_VALIDATION_IDREQUIRED_MSG = "Invalid driver id/ids details.";
        public const string GET_FLEET_UTILIZATION_VALIDATION_DATEMISMATCH_MSG = "Start Date should be less than End Date.";
        public const string GET_FLEET_UTILIZATION_SUCCESS_MSG = "Fleet Utilizaiton details fetched successfully";
        public const string GET_FLEET_UTILIZATION_FAILURE_MSG = "No Result Found";
        #endregion

        #region FleetOverview
        public const string FLEETOVERVIEW_SERVICE_NAME = "Report Service";
        public const string FLEETOVERVIEW_FILTER_FAILURE_MSG = "Error fetching fleet over view filter details.";
        #endregion

        #region Feet Fuel Report
        public const string GET_FLEET_FUEL_VALIDATION_STARTDATE_MSG = "Invalid start date.";
        public const string GET_FLEET_FUEL_VALIDATION_ENDDATE_MSG = "Invalid end date.";
        public const string GET_FLEET_FUEL_VALIDATION_VINREQUIRED_MSG = "Invalid VIN details.";
        public const string GET_FLEET_FUEL_VALIDATION_DATEMISMATCH_MSG = "Start Date should be less than End Date.";
        public const string GET_FLEET_FUEL_SUCCESS_MSG = "Fleet Fuel details fetched successfully";
        public const string GET_FLEET_FUEL_FAILURE_MSG = "No Result Found";
        public const string GET_FLEET_FUEL_VALIDATION_DRIVERID_MSG = "Invalid Driver ID";

        #endregion

        #region Vehicle Health Summary
        public const string VALIDATION_MSG_FROMDATE = "From Date should be less than To Date.";
        public const string FAILURE_MSG = "No Result Found.";
        public const string SUCCESS_MSG = "Vehicle summary details fetched successfully.";
        #endregion

        #region Fuel Deviation Report Table Details
        public const string GET_FUEL_DEVIATION_SUCCESS_MSG = "Fuel Deviation fetched successfully for requested Filters";
        public const string GET_FUEL_DEVIATION_FAIL_MSG = "Fuel Deviation fetched failed for requested Filters";
        #endregion
    }
}
