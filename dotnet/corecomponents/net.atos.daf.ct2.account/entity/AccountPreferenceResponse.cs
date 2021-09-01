using System.Collections.Generic;
using System.Net;

namespace net.atos.daf.ct2.account.entity
{
    public class RegisterDriverResponse
    {
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }

    public class ValidateDriverResponse
    {
        public string AccountID { get; set; }
        public string AccountName { get; set; }
        public string RoleID { get; set; } = "DRIVER";
        public string Language { get; set; }
        public string TimeZone { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public string UnitDisplay { get; set; }
        public string VehicleDisplay { get; set; }
        public List<ValidateDriverOrganisation> Organisations { get; set; }
    }

    public class ValidateDriverOrganisation
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class AccountPreferenceResponse
    {
        public string Language { get; set; }
        public string TimeZone { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public string UnitDisplay { get; set; }
        public string VehicleDisplay { get; set; }
    }
}