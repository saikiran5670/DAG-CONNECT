﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.ReportScheduler
{
    public static class ReportSchedulerConstants
    {
        public const string INTERNAL_SERVER_MSG = "Error occured, Please check with DAF IT team for more info.";
        public const string REPORTSCHEDULER_ORG_ID_NOT_NULL_MSG = "Orgnization id cannot be null.";
        public const string REPORTSCHEDULER_ID_NOT_NULL_MSG = "Report Scheduler id cannot be null.";
        public const string REPORTSCHEDULER_CONTROLLER_NAME = "Report Scheduler Controller";
        public const string REPORTSCHEDULER_SERVICE_NAME = "Report Scheduler service";
        public const string REPORTSCHEDULER_EXCEPTION_LOG_MSG = "{0} method Failed. Error:{1}";
        public const string REPORTSCHEDULER_PARAMETER_NOT_FOUND_MSG = "Report Scheduler Parameter are not found.";
        public const string REPORTSCHEDULER_INTERNEL_SERVER_ISSUE = "Internal Server Error.(01)";
        public const string REPORTSCHEDULER_CREATE_FAILED_MSG = "There is an error while creating report scheduler.";
        public const string REPORTSCHEDULER_UPDATE_FAILED_MSG = "There is an error while updatating report scheduler.";
        public const string REPORTSCHEDULER_DATA_NOT_FOUND_MSG = "Report Scheduler are not found.";
        public const string VEHICLE_GROUP_NAME = "VehicleGroup_{0}_{1}";
        public const string REPORTSCHEDULER_RECIPENT_ID_NOT_FOUND = "Recipent id has to ne non zero.";
        public const string REPORTSCHEDULER_EMAIL_ID_NOT_FOUND = "Email id is empty.";
        public const string REPORTSCHEDULER_INTERNEL_SERVER_ISSUE_2 = "Internal Server Error:- {0}";
    }
}
