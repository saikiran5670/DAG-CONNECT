using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.httpclientfactory.Entity.ota22
{
    public class OTA22Configurations
    {
        public string API_BASE_URL { get; set; }
        public string AUTH_URL { get; set; }
        public string GRANT_TYPE { get; set; }
        public string CLIENT_SCOPE { get; set; }
        public string CLIENT_ID { get; set; }
        public string CLIENT_SECRET { get; set; }
    }
}
