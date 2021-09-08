namespace net.atos.daf.ct2.account.entity
{
    public class RegisterDriverDataServiceRequest
    {
        public int OrganisationId { get; set; }
        public string DriverId { get; set; }
        public string AccountEmail { get; set; }
        public string Password { get; set; }
        public bool IsLoginSuccessful { get; set; }
    }
}
