using System;

namespace net.atos.daf.ct2.accountservicerest
{
    public class AccountGroupDetailRequest
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public int VehicleCount { get; set; }
        public int AccountCount{ get; set; }
        
    }
}
