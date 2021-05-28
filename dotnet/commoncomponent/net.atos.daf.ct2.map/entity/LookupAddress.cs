

using System.Collections.Generic;

namespace net.atos.daf.ct2.map.entity
{
    public class LookupAddress
    {
        public long Latitude { get; set; }
        public long Longitude { get; set; }
        public string Address { get; set; }
        public List<LookupAddress> LookupAddresses { get; set; }

    }
}
