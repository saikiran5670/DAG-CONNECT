using System.Collections.Generic;
namespace net.atos.daf.ct2.portalservice.Account
{
    public class AccountDetailsResponse
    {
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Type { get; set; }
        public int OrganizationId { get; set; }
        public string DriverId { get; set; }
        public int PreferenceId { get; set; }
        public int BlobId { get; set; }
        public long CreatedAt { get; set; }
        public List<KeyValue> Roles { get; set; }
        public List<KeyValue> AccountGroups { get; set; }
    }
    public class KeyValue
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
