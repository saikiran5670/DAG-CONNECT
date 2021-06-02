namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public static class AlertConstants
    {
        public const string ACTIVATED_ALERT_SUCCESS_MSG = "Alert was Activated successfully for Id:- {0}.";
        public const string ACTIVATED_ALERT_FAILURE_MSG = "Activate Alert failed for Id:- {0}. Error: {1}";
        public const string SUSPEND_ALERT_SUCCESS_MSG = "Alert was Suspended successfully for Id:- {0}.";
        public const string SUSPEND_ALERT_FAILURE_MSG = "Suspend Alert failed for Id:- {0}. Error: {1}";
        public const string DELETE_ALERT_SUCCESS_MSG = "Alert was deleted successfully for Id:- {0}.";
        public const string DELETE_ALERT_FAILURE_MSG = "Alert deletion failed for Id:- {0}. Error: {1}";
        public const string DELETE_ALERT_NO_NOTIFICATION_MSG = "You cannot delete alert. As notification is associated with this alert Id:- {0}";
        public const string DUPLICATE_ALERT_SUCCESS_MSG = "Duplicate Alert Type fetched successfull. for Id:- {0}.";
        public const string DUPLICATE_ALERT_FAILURE_MSG = "Duplicate Alert Type fetch got failed. for Id:- {0}. Error: {1}";
        public const string ALERT_FAILURE_MSG = "Either alert id is not available or not exist in vaild state.";
        public const string SOCKET_EXCEPTION_MSG= "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        public const string ALERT_ID_NON_ZERO_MSG = "Alert id cannot be zero.";
        public const string ALERT_ACC_OR_ORG_ID_NOT_NULL_MSG = "Account id or Orgnization id cannot be null.";
        public const string ALERT_CATEGORY_NOT_FOUND_MSG = "Alert Category are not found.";
        public const string ALERT_DUPLICATE_NOTIFICATION_RECIPIENT_MSG = "Duplicate notification recipient label added in list.";
        public const string VEHICLE_GROUP_NAME="VehicleGroup_{0}_{1}";
        public const string VEHICLE_GROUP_DESCRIPTION= "Single vehicle group for alert:- {0}  org:- {1}";
        public const string INTERNAL_SERVER_ERROR_MSG = "Internal Server Error.{0}";
        public const string ALERT_EXCEPTION_LOG_MSG = "{0} method Failed. Error:{1}";
        public const string ALERT_CONTROLLER_NAME = "Alert Controller";
        public const string ALERT_SERVICE_NAME = "Alert service";

    }
}
