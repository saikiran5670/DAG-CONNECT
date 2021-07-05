using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public static class TripReportConstants
    {
        public const string ALL_PARAM_MSG = "Trip Report all Parameters are not set.";
        public const string NO_ASSOCIATION_MSG = "Cannot process, as no association vehicle are available.";
        public const string NO_VEHICLE_MSG = "Cannot process, as no vehicle are available in scheduler.";
        public const string NO_VEHICLE_ASSOCIATION_MSG = "Cannot process, as vehicle {0} is not association with the account.";
    }

}
