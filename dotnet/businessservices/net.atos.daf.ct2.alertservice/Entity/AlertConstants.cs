﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.alertservice.Entity
{
    public class AlertConstants
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
        public const string ALERT_FAILURE_MSG = "Alert id not available.";
    }
}
