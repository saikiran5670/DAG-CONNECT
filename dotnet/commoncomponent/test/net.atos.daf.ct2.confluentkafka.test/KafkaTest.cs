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
            //KafkaConfiguration kafkaEntity = new KafkaConfiguration()
            //{
            //    BrokerList = "daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net:9093",
            //    ConnString = "Endpoint=sb://daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=gicUoPvdd/u2bKPFXIhaDbBVgvBDsXrz9kcSWJm8gpw=",
            //    Topic = "ingress.atos.alert.poc.json",
            //    Cacertlocation = "./cacert.pem",
            //    ProducerMessage = @"{ 'id': 1, 'tripid': 'a801403e-ae4c-42cf-bf2d-ae39009c69oi', 'vin': 'XLR0998HGFFT70000', 'categoryType': 'F', 'type': 'I', 'name': 'Test01', 'alertid': 596, 'thresholdValue': 1000.0, 'thresholdValueUnitType': 'M', 'valueAtAlertTime': 10000.0, 'latitude': 51.12768896, 'longitude': 4.935644520, 'alertGeneratedTime': 1626965785, 'messageTimestamp': 1626965785, 'createdAt': 1626965785, 'modifiedAt': 1626965785,'UrgencyLevelType':'C'}"
            //};
            //await KafkaConfluentWorker.Producer(kafkaEntity);
            for (int i = 0; i < 5; i++)
            {
                //Thread.Sleep(500);
                KafkaConfiguration kafkaEntity = new KafkaConfiguration()
                {
                    BrokerList = "daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net:9093",
                    ConnString = "Endpoint=sb://daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=gicUoPvdd/u2bKPFXIhaDbBVgvBDsXrz9kcSWJm8gpw=",
                    Topic = "egress.portal.push.notification.string",
                    Cacertlocation = "./cacert.pem",
                    ProducerMessage = @"{ 'id': " + i + ",'tripid': '33f90302-6b78-4bff-830b-a2604a7a821c','vin': 'XLR0998HGFFT70000','categoryType': 'L','type': 'I','name': 'Test001','alertid': 702,'thresholdValue': 1000.0,'thresholdValueUnitType': 'M','valueAtAlertTime': 10000.0,'latitude': 51.12768896,'longitude': 4.935644520,'alertGeneratedTime': 1632202819045,'messageTimestamp': 1632202819045,'createdAt': 1632202819045,'modifiedAt': 1632202819045,'UrgencyLevelType': 'C','AlertCreatedAccountId': 143}"
                };
                await KafkaConfluentWorker.Producer(kafkaEntity);
            }

        }
        [TestMethod]
        public void ConsumerTest()
        {
            KafkaConfiguration kafkaEntity = new KafkaConfiguration()
            {
                BrokerList = "daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net:9093",
                ConnString = "Endpoint=sb://daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=gicUoPvdd/u2bKPFXIhaDbBVgvBDsXrz9kcSWJm8gpw=",
                Topic = "egress.portal.push.notification.string",
                Cacertlocation = "./cacert.pem",
                Consumergroup = "cg2"
            };
            for (int i = 0; i < 20; i++)
            {
                ConsumeResult<string, string> result = KafkaConfluentWorker.Consumer(kafkaEntity);
                Assert.IsNotNull(result);
                Assert.IsNotNull(result.Message);
                Assert.IsNotNull(result.Message.Value);
            }
        }






        [TestMethod]
        public async Task ProducerVehTest()
        {
            KafkaConfiguration kafkaEntity = new KafkaConfiguration()
            {
                BrokerList = "daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net:9093",
                ConnString = "Endpoint=sb://daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=gicUoPvdd/u2bKPFXIhaDbBVgvBDsXrz9kcSWJm8gpw=",
                Topic = "ingress.atos.vehicle.cdc.json",
                Cacertlocation = "./cacert.pem",
                ProducerMessage = @"{
	'Schema': 'master.Vehicle',
	'Payload': {
		'Data': {
			'Vin': 'XLRAE75PC0E345556',
			'Vid': 'M4A1113',
			'Status': 'C',
			'FuelType': 'B',
			'FuelTypeCoefficient': 0.0
		},
		'Operation': 'I',
		'Namespace': 'vehicleManagement',
		'Timestamp': 1627403364
	}
}"
            };
            await KafkaConfluentWorker.Producer(kafkaEntity);
        }
        [TestMethod]
        public void ConsumerVehTest()
        {
            KafkaConfiguration kafkaEntity = new KafkaConfiguration()
            {
                BrokerList = "daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net:9093",
                ConnString = "Endpoint=sb://daf-lan1-d-euwe-cdp-evh-int.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=gicUoPvdd/u2bKPFXIhaDbBVgvBDsXrz9kcSWJm8gpw=",
                Topic = "ingress.atos.vehicle.cdc.json",
                Cacertlocation = "./cacert.pem",
                Consumergroup = "cdcvehicleconsumer"
            };
            ConsumeResult<string, string> result = KafkaConfluentWorker.Consumer(kafkaEntity);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Message);
            Assert.IsNotNull(result.Message.Value);
        }

    }
}
