using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.confluentkafka;
using net.atos.daf.ct2.confluentkafka.entity;

namespace net.atos.daf.ct2.confluentkafka.test
{
    [TestClass]
    public class KafkaTest
    {
        [TestMethod]
        public async Task ProducerTest()
        {
            KafkaEntity kafkaEntity = new KafkaEntity()
            {
                BrokerList = "daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net:9093",
                ConnString = "Endpoint=sb://daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=gicUoPvdd/u2bKPFXIhaDbBVgvBDsXrz9kcSWJm8gpw=",
                Topic = "ingress.atos.alert.poc.json",
                Cacertlocation = "./cacert.pem",
                ProducerMessage = @"{'id':'0', 'tripid':'null', 'vin':'null', 'categoryType':'null', 'type':'null', 'name':'null', 'alertid':'0', 'thresholdValue':'0.0', 'thresholdValueUnitType':'null', 'valueAtAlertTime':'0.0', 'latitude':'0.0', 'longitude':'0.0', 'alertGeneratedTime':'0', 'messageTimestamp':'0', 'createdAt':'0', 'modifiedAt':'0'}"
            };
             await KafkaConfluentWorker.Producer(kafkaEntity);
        }
        [TestMethod]
        public void ConsumerTest()
        {
            KafkaEntity kafkaEntity = new KafkaEntity()
            {
                BrokerList = "daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net:9093",
                ConnString = "Endpoint=sb://daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=gicUoPvdd/u2bKPFXIhaDbBVgvBDsXrz9kcSWJm8gpw=",
                Topic = "ingress.atos.alert.poc.json",
                Cacertlocation = "./cacert.pem",
                Consumergroup= "alertpoccosumer"
            };
            ConsumeResult<Null, string> result = KafkaConfluentWorker.Consumer(kafkaEntity);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Message);
            Assert.IsNotNull(result.Message.Value);
        }
    }
}
