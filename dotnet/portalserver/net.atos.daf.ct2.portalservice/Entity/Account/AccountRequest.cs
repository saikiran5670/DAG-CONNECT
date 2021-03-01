using System;

namespace net.atos.daf.ct2.portalservice.Account
{
    public class AccountRequest
    {
        public AccountRequest()
        {
            Type = "P";
        }
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string Type { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public int OrganizationId { get; set; }
        public string DriverId { get; set; }
        public string CreatedAt { get; set; }

    }
    public class AccountResponse
    {
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string Type { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public int OrganizationId { get; set; }
        public string DriverId { get; set; }
        public int PreferenceId { get; set; }
        public int BlobId { get; set; }

    }
    public class AccountDetailRequest
    {
        public int AccountId { get; set; }
        public int OrganizationId { get; set; }
        public int AccountGroupId { get; set; }
        public int VehicleGroupId { get; set; }
        public int RoleId { get; set; }
        public string Name { get; set; }
    }
    public class AccessRelationshipRequest
    {
        public int Id { get; set; }
        public string AccessRelationType { get; set; }
        public int AccountGroupId { get; set; }
        public int VehicleGroupId { get; set; }        
    }
    public class AccessRelationshipFilter
    {
        public int AccountId { get; set; }
        public int AccountGroupId { get; set; }
    }
    public class ChangePasswordRequest
    {
        public string EmailId { get; set; }
        public string Password { get; set; }
    }

}
