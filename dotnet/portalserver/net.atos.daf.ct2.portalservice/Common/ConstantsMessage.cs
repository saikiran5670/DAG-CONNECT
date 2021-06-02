namespace net.atos.daf.ct2.portalservice.Common
{
    public static class PortalConstants
    {
        public static class AccountValidation
        {
            public const string CREATE_REQUIRED = "The EmailId address, first name, last name, organization id and type is required.";
            public const string INVALID_DATA = "The EmailId address, first name, last name and organization id should be valid.";
            public const string INVALID_ACCOUNT_TYPE = "The account type is not valid";
            public const string ERROR_MESSAGE = "There is an error creating account.";
            public const string EMAIL_SENDING_FAILED_MESSAGE = "There is an error while sending account confirmation email to the account user.";
            public const string EMAIL_UPDATE_NOT_ALLOWED = "EmailId is not allowed to update.";
        }

        public static class ExceptionKeyWord
        {
            public const string FK_CONSTRAINT = "violates foreign key constraint";
            public const string SOCKET_EXCEPTION = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        }

        public static class ResponseError
        {
            public const string INTERNAL_SERVER_ERROR = "Internal Server Error.{0}";
            public const string KEY_CONSTRAINT_ERROR = "The key constraint error.";
            public const string BAD_REQUEST = "The Bad Request.";
        }

        public static class PackageValidation
        {
            public const string CREATE_REQUIRED = "The packageId , package code and featureset id  are required.";
            public const string INVALID_PACKAGE_TYPE = "The pakage type is not valid. It should be of single character";
            public const string ERROR_MESSAGE = "There is an error creating package.";
            public const string PACKAGE_STATUS_REQUIRED = "The packageId and status are required.";
            public const string INVALID_PACKAGE_STATUS = "The pakage status is not valid. It should be of single character";
        }

        public static class VehicleValidation
        {
            public const string CREATE_REQUIRED = "The group name is required.";
            public const string INVALID_DATA = "The vehicle group name, vehicle group description should be valid.";
            public const string INVALID_GROUP_TYPE = "The vehicle group type is not valid";
            public const string ERROR_MESSAGE = "There is an error creating vehicle group.";
            public const string GROUP_ID_REQUIRED = "The group Id is required.";
            public const string ORGANIZATION_ID_REQUIRED = "The organization Id is required.";
            public const string INVALID_FUNCTION_ENUM_TYPE = "The function enum type is not valid";
            public const string FUNCTION_TYPE_REQUIRED = "The function type is required.";
        }
    }
}
