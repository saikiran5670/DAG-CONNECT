using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.OTASoftwareUpdate
{
    public class OTASoftwareUpdateConstants
    {
        public const string INTERNAL_SERVER_ERROR_MSG = "Internal Server Error.{0}";
        public const string VEHICLE_SOFTWARE_STATUS_FAILURE_MSG = "Get Vehicale Sotwate Status failed ith error {0}";
        public const string SOCKET_EXCEPTION_MSG = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        public const string OTA_CONTROLLER_NAME = "Alert Controller";
        public const string OTA_SERVICE_NAME = "Alert service";
        public const string OTA_EXCEPTION_LOG_MSG = "{0} method Failed. Error:{1}";
    }
}
