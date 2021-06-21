using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace pushnotificationcorecomponent
{
    public class Worker
    {
        private const string jsonData = @"{'id':'0', 'tripid':'null', 'vin':'null', 'categoryType':'null', 'type':'null', 'name':'null', 'alertid':'0', 'thresholdValue':'0.0', 'thresholdValueUnitType':'null', 'valueAtAlertTime':'0.0', 'latitude':'0.0', 'longitude':'0.0', 'alertGeneratedTime':'0', 'messageTimestamp':'0', 'createdAt':'0', 'modifiedAt':'0'}";

        public static ConsumeResult<Null, string> Consumer(string brokerList, string connStr, string consumergroup, string topic, string cacertlocation)
        {
            KafkaConfig kafkaConfig = new KafkaConfig();
            ConsumerConfig consumerConfig = kafkaConfig.GetConsumerConfig(brokerList, connStr, cacertlocation, consumergroup);
            try
            {
                using (var consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build())
                {
                    consumer.Subscribe(topic);

                    while (true)
                    {
                        try
                        {
                            var msg = consumer.Consume();
                            Console.WriteLine(msg.Message.Value);
                            consumer.Commit(msg);
                            return msg;
                        }
                        catch (ConsumeException)
                        {
                            consumer.Close();
                        }
                        catch (Exception)
                        {
                            consumer.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;                
            }
        }
        //public static async Task Producer(string brokerList, string connStr, string topic, string cacertlocation)
        //{
        //    try
        //    {
        //        var config = new ProducerConfig
        //        {
        //            BootstrapServers = brokerList,
        //            SecurityProtocol = SecurityProtocol.SaslSsl,
        //            SaslMechanism = SaslMechanism.Plain,
        //            SaslUsername = "$ConnectionString",
        //            SaslPassword = connStr,
        //            SslCaLocation = cacertlocation,
        //            //Debug = "security,broker,protocol"        //Uncomment for librdkafka debugging information
        //        };

        //        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        //        {
        //            await producer.ProduceAsync(topic, new Message<Null, string> { Value = jsonData });
        //            producer.Flush();

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(string.Format("Exception Occurred - {0}", e.Message));
        //    }

        //}


    }
}
