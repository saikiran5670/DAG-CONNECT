using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using net.atos.daf.ct2.confluentkafka.entity;

namespace net.atos.daf.ct2.confluentkafka
{
    public class KafkaConfluentWorker
    {
        //{"schema":[],"payload":{"data":"{ \"vid\":\"M4A1117\",\"alertid\":\"yes\",\"state\":\"A\" }","op":"I","namespace":"master.vehiclealertref","ts_ms":1625681684823}}
        public static async Task Producer(KafkaConfiguration kafkaEntity)
        {
            try
            {
                var config = KafkaConfigPropertyManager.GetProducerConfig(kafkaEntity);
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    await producer.ProduceAsync(kafkaEntity.Topic, new Message<Null, string> { Value = kafkaEntity.ProducerMessage });
                    producer.Flush();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //private const string JSON_DATA = @"{'id':'0', 'tripid':'null', 'vin':'null', 'categoryType':'null', 'type':'null', 'name':'null', 'alertid':'0', 'thresholdValue':'0.0', 'thresholdValueUnitType':'null', 'valueAtAlertTime':'0.0', 'latitude':'0.0', 'longitude':'0.0', 'alertGeneratedTime':'0', 'messageTimestamp':'0', 'createdAt':'0', 'modifiedAt':'0'}";
        public static ConsumeResult<string, string> Consumer(KafkaConfiguration kafkaEntity)
        {
            ConsumerConfig consumerConfig = KafkaConfigPropertyManager.GetConsumerConfig(kafkaEntity);
            try
            {
                using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
                {
                    consumer.Subscribe(kafkaEntity.Topic);
                    //TopicPartition tp = new TopicPartition(kafkaEntity.Topic, 0);
                    //List<TopicPartition> tps = Arrays.asList(tp);
                    //consumer.assign(tps);
                    while (true)
                    {
                        try
                        {
                            var msg = consumer.Consume();
                            Console.WriteLine(msg.Message.Value);
                            consumer.Commit(msg);
                            return msg;
                        }
                        catch (ConsumeException ex)
                        {
                            consumer.Close();
                            throw ex;
                        }
                        catch (Exception ex)
                        {
                            consumer.Close();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}