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
            public const string EmailUpdateNotAllowed = "EmailId is not allowed to update.";
        }
        public static class ExceptionKeyWord
        {
            public const string FK_Constraint = "violates foreign key constraint";
            public const string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        }
        public static class ResponseError
        {
            public const string InternalServerError = "Internal Server Error.{0}";
            public const string KeyConstraintError = "The key constraint error.";
            public const string BadRequest = "The Bad Request.";



        }

        public static class PackageValidation
        {
            public const string CreateRequired = "The packageId , package code and featureset id  are required.";
            public const string InvalidPackageType = "The pakage type is not valid. It should be of single character";
            public const string ErrorMessage = "There is an error creating package.";
            public const string PackageStatusRequired = "The packageId and status are required.";
            public const string InvalidPackageStatus = "The pakage status is not valid. It should be of single character";


        }
        public static class OrgRelationshipValidation
        {

            public const string ErrorMessage = "There is an error creating Org Relationship.";
        }

        public static class VehicleValidation
        {
            public const string CreateRequired = "The group name is required.";
            public const string InvalidData = "The vehicle group name, vehicle group description should be valid.";
            public const string InvalidGroupType = "The vehicle group type is not valid";
            public const string ErrorMessage = "There is an error creating vehicle group.";
            public const string GroupIdRequired = "The group Id is required.";
            public const string OrganizationIdRequired = "The organization Id is required.";
            public const string InvalidFunctionEnumType = "The function enum type is not valid";
            public const string FunctionTypeRequired = "The function type is required.";
        }

    }
}
