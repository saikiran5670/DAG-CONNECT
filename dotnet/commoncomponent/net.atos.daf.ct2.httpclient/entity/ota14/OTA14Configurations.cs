using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.httpclientfactory.entity.ota14
{
    public class OTA14Configurations
    {
        public string API_BASE_URL { get; set; }
        public string AUTH_URL { get; set; }
        public string GRANT_TYPE { get; set; }
        public string CLIENT_SCOPE { get; set; }
        public string CLIENT_ID { get; set; }
        public string CLIENT_SECRET { get; set; }
        public int RETRY_COUNT { get; set; }
    }
}
