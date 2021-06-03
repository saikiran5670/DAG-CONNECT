using System.Collections.Generic;

namespace net.atos.daf.ct2.portalservice.Entity.Driver
{
    public class Driver
    {
        public string DriverID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
    public class DriverValidate
    {
        public string DriverID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }

    }
    public class DriverRequest
    {
        public List<Driver> Drivers { get; set; }
        public int OrganizationId { get; set; }
    }
}
