using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;

namespace net.atos.daf.ct2.notificationengine
{
    public class Worker
    {
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
    }
}
