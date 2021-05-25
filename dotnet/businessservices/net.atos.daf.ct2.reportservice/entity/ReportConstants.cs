﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.reportservice.entity
{
    public static class ReportConstants
    {
        public const string USER_PREFERENCE_SUCCESS_MSG = "User preferences fetched successfully for account Id:- {0} and for report Id:- {1}.";
        public const string USER_PREFERENCE_FAILURE_MSG = "User preferences fetched failed for account Id:- {0} and for report Id:- {1}. Error: {2}";
        public const string USER_PREFERENCE_FAILURE_MSG2 = "No records found for reprot data columns.";

        public const string USER_PREFERENCE_CREATE_SUCCESS_MSG = "User preferences Saved successfully for account Id:- {0} and for report Id:- {1}.";
        public const string USER_PREFERENCE_CREATE_FAILURE_MSG = "Saving User Preference failed for account Id:- {0} and for report Id:- {1}.";

        public const string GET_VIN_SUCCESS_MSG = "VIN fetched successfully for given date range";
        public const string GET_VIN_FAILURE_MSG = "VIN fetched for given date range";
        public const string GET_VIN_VISIBILITY_FAILURE_MSG = "No vehicle found for Account Id {0} and Organization Id {1}";
        public const string GET_VIN_TRIP_NOTFOUND_MSG = "No trip for vehicle found for Account Id {0} and Organization Id {1}. for last 90 days.";
    }
}
