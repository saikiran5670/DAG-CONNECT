using System.Net;

namespace net.atos.daf.ct2.account.entity
{
    public class SSOTokenResponse
    {
        public string OrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public int RoleID { get; set; }
        public string TimeZone { get; set; }
        public string DateFormat { get; set; }
        public string UnitDisplay { get; set; }
        public string VehicleDisplay { get; set; }
    }

    public class SSOResponse 
    {
        public SSOTokenResponse Details { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public string Value { get; set; }
    }
}
