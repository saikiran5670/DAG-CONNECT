using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.vehicleservice.Entity
{
    public class KafkaConfiguration
    {
        public string EH_FQDN { get; set; }
        public string EH_CONNECTION_STRING { get; set; }
        public string EH_NAME { get; set; }
        public string CONSUMER_GROUP { get; set; }
        public string CA_CERT_LOCATION { get; set; }
    }
}
