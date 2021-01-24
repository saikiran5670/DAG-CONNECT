using System;

namespace net.atos.daf.ct2.accountservicerest
{
    public class AccountRequest
    {
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public int Organization_Id { get; set; }
    }
    public class AccountDetailRequest
    {
        public int AccountId { get; set; }
        public int OrganizationId { get; set; }
        public int GroupId { get; set; }
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
        public char AccountGroupId { get; set; }
    }

}
