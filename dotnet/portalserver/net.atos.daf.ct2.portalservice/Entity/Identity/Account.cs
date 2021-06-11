namespace net.atos.daf.ct2.portalservice.Identity
{
    public class Account
    {
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Organization_Id { get; set; }
        public string Salutation { get; set; }
        public int? PreferenceId { get; set; }
        public int? BlobId { get; set; }
    }
}
