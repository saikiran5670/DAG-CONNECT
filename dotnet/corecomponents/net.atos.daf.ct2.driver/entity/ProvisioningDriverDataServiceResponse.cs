using System.Collections.Generic;

namespace net.atos.daf.ct2.driver.entity
{
    public class ProvisioningDriverDataServiceResponse
    {
        public List<ProvisioningDriver> Drivers { get; set; }
    }

    public class ProvisioningDriver
    {
        public string Account { get; set; }
        public string DriverId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
