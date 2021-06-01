using System;
using net.atos.daf.ct2.account.ENUM;
namespace net.atos.daf.ct2.account.entity
{
    public class Account
    {
        //public Account()
        //{
        //    AccountType = AccountType.PortalAccount;
        //}
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public AccountType AccountType { get; set; }
        public int? Organization_Id { get; set; }
        public string DriverId { get; set; }
        public bool IsDuplicateInOrg { get; set; }
        public long? StartDate { get; set; }
        public long? EndDate { get; set; }
        public bool IsDuplicate { get; set; }
        public bool IsError { get; set; }
        public bool IsErrorInEmail { get; set; }
        public int? PreferenceId { get; set; }
        public int? BlobId { get; set; }
        public long? CreatedAt { get; set; }
        public Guid? ProcessToken { get; set; }
        public string OrgName { get; set; }
        public string FullName
        {
            get
            {
                return $"{Salutation} {FirstName} {LastName}";
            }
        }
    }
}
