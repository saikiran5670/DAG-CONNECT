using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using net.atos.daf.ct2.tcucore;

namespace net.atos.daf.ct2.tcuprovisioningtest
{
    public class Worker
    {
        //private const string jsonData = @"{'vin' : 'KLRAE75PC0E200122','deviceIdentifier' : 'BYXRS845213','deviceSerialNumber' : '9072435876203496','correlations' : {'deviceId' : 'DLDAE75PC0E348695','vehicleId' : '8756435876203'},'referenceDate':'2021-03-05T18:40:51'}";
        private static bool result = false;

        public static async Task<bool> Producer(string brokerList, string connStr, string topic, string cacertlocation, string jsonData)
        {
            KafkaConfig kafkaConfig = new KafkaConfig();
            ProducerConfig producerConfig = kafkaConfig.GetProducerConfig(brokerList, connStr, cacertlocation);

            try
            {
                using (var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
                {

                    await producer.ProduceAsync(topic, new Message<Null, string> { Value = jsonData });
                    producer.Flush();
                    result = true;
                }
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }

            return result;
        }
    }
}
