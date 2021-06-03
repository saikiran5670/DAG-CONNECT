using System;

namespace net.atos.daf.ct2.driver.entity
{
    public class DriverMaster : DriverOrg
    {
        public int Id { get; set; }
        public string DriverId { get; set; }
        public int UserOrgId { get; set; }
        public string LanguageCode { get; set; }
        public int LanguageCodeId { get; set; }
        public string TimeZone { get; set; }
        public int TimeZoneId { get; set; }
        public string Currency { get; set; }
        public int CurrencyId { get; set; }
        public string Unit { get; set; }
        public int UnitId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }













    }
}
