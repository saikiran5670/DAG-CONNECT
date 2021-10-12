using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.OTASoftwareUpdate
{
    public class OTASoftwareUpdateConstants
    {
        public const string INTERNAL_SERVER_ERROR_MSG = "Internal Server Error.{0}";
        public const string VEHICLE_SOFTWARE_STATUS_FAILURE_MSG = "Get Vehicle Software Status failed with error {0}";
        public const string SOCKET_EXCEPTION_MSG = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        public const string OTA_CONTROLLER_NAME = "OTA Controller";
        public const string OTA_SERVICE_NAME = "OTA service";
        public const string OTA_EXCEPTION_LOG_MSG = "{0} method Failed. Error:{1}";
        public const string LANGUAGE_REQUIRED_MSG = "Language is reuired with 2 characters.";

        public const string GET_OTASOFTWAREUPDATE_VALIDATION_STARTDATE_MSG = "Invalid Schedule date.";
        public const string GET_OTASOFTWAREUPDATE_VINREQUIRED_MSG = "Invalid VIN details.";
        public const string GET_OTASOFTWAREUPDATE_SUCCESS_MSG = "Otasoftwareupdate details fetched successfully";
        public const string OTA14_SUCCESS_MSG = "OTA approval is successful.";
        public const string GET_OTASOFTWAREUPDATE_MSG = "No Result Found";
    }
}
