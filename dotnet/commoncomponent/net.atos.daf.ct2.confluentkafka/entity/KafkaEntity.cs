using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.confluentkafka.entity
{
    public class KafkaEntity
    {
        public string BrokerList { get; set; }
        public string ConnString { get; set; }
        public string Topic { get; set; }
        public string Cacertlocation { get; set; }
        public string Consumergroup { get; set; }
        public string ProducerMessage { get; set; }
    }
}
