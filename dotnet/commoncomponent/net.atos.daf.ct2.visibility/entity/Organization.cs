using System;

namespace net.atos.daf.ct2.visibility.entity
{
    public class Organization : TableLog
    {
        public long Organizationid { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zipcode { get; set; }
        public string Phoneno { get; set; }
        public int Languageid { get; set; }
        public string Emailid { get; set; }
        public int Timezoneid { get; set; }
        public int Currencyid { get; set; }
        public int Unitid { get; set; }
        public int VehicleDisplayid { get; set; }
        public string DateFormat { get; set; }
        public bool Optoutstatus { get; set; }
        public DateTime OptOutStatusChangeDdate { get; set; }

    }
}
