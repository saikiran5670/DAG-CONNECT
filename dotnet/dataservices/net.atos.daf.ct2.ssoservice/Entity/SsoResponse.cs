namespace net.atos.daf.ct2.singlesignonservice.Entity
{
    public class SsoResponse
    {
        public System.Net.HttpStatusCode StatusCode { get; set; }
        public UserDetails Result { get; set; }


    }
    public class UserDetails
    {
        public string OrganizationID { get; set; }
        public string OraganizationName { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public int RoleID { get; set; }
        public string TimeZone { get; set; }
        public string DateFormat { get; set; }
        public string UnitDisplay { get; set; }
        public string VehicleDisplay { get; set; }

    }

}
