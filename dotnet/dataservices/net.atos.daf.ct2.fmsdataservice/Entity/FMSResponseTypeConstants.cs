using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.fmsdataservice.entity
{

    public static class FMSResponseTypeConstants
    {
        public const string ACCPET_TYPE_VEHICLE_JSON = "application/vnd.fmsstandard.com.vehicles.v3.0+json; UTF-8";
        public const string ACCEPT_TYPE_VEHICLE_POSITION_JSON = "application/vnd.fmsstandard.com.vehiclepositions.v3.0+json; UTF-8";
        public const string ACCEPT_TYPE_VEHICLE_STATUS_JSON = "application/vnd.fmsstandard.com.vehiclestatuses.v3.0+json; UTF-8";
        public const string GET_VIN_VISIBILITY_FAILURE_MSG = "No vehicle found for Account Id {0} and Organization Id {1}";
    }
}
