using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Common
{
    public static class PortalConstants
    {
        public static class AccountValidation
        {
            public const string CreateRequired = "The EmailId address, first name, last name, organization id and type is required.";
            public const string InvalidData = "The EmailId address, first name, last name and organization id should be valid.";
            public const string InvalidAccountType = "The account type is not valid";
            public const string ErrorMessage = "There is an error creating account.";
            public const string EmailSendingFailedMessage = "There is an error while sending account confirmation email to the account user.";
        }
        public static class ExceptionKeyWord
        {
            public const string FK_Constraint = "violates foreign key constraint";
            public const string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        }
        public static class ResponseError
        {
            public const string InternalServerError = "Internal Server Error.{0}";
            
        }

    }
}
