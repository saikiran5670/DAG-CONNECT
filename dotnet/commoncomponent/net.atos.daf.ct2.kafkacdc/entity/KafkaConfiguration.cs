using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.kafkacdc.entity
{
    public class KafkaConfiguration
    {
        public string EH_FQDN { get; set; }
        public string EH_CONNECTION_STRING { get; set; }
        public string EH_NAME { get; set; }
        public string CONSUMER_GROUP { get; set; }
        public string CA_CERT_LOCATION { get; set; }
        public string IsVehicleCDCEnable { get; set; }
    }
}
