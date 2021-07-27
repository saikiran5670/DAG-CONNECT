using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.confluentkafka;
using net.atos.daf.ct2.confluentkafka.entity;

namespace net.atos.daf.ct2.confluentkafka.test
{
    [TestClass]
    public class ProducerTest
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            KafkaEntity kafkaEntity = new KafkaEntity()
            {
                BrokerList = "daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net:9093",
                ConnString = "Endpoint=sb://daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=gicUoPvdd/u2bKPFXIhaDbBVgvBDsXrz9kcSWJm8gpw=",
                Topic = "ingress.atos.alert.poc.json",
                Cacertlocation = "./cacert.pem"
            };
             await KafkaConfluentWorker.Producer(kafkaEntity);
        }
    }
}
