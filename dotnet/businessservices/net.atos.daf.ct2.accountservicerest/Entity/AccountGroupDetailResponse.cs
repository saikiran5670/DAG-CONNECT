using System;

namespace net.atos.daf.ct2.accountservicerest
{
    public class AccountGroupDetailResponse
    {
        public int AccountGroupId { get; set; }
        public string Name { get; set; }
        public string VehicleCount { get; set; }
        public string AccountCount { get; set; }

    }
}