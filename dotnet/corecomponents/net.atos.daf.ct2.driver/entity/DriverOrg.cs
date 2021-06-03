using System;

namespace net.atos.daf.ct2.driver.entity
{
    public class DriverOrg : DateTimeStamp
    {
        public int DriverOrgId { get; set; }
        public int DriverMasterId { get; set; }
        public int OrganizationId { get; set; }
        public string Civility { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool OptOutStatus { get; set; }
        public DateTime OptOutStatusChangedDate { get; set; }
        public int OptOutLevelId { get; set; }
        public bool IsConsentGiven { get; set; }
        public DateTime ConsentChangedDate { get; set; }
    }
}
