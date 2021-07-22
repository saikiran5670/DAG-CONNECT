using Confluent.Kafka;
using net.atos.daf.ct2.confluentkafka.entity;

namespace net.atos.daf.ct2.confluentkafka
{
    internal class KafkaConfigManager
    {
        private const string SASLUSERNAME = "$ConnectionString";
        private const string BROKERVERSION = "1.0.0";

        public static ConsumerConfig GetConsumerConfig(KafkaEntity kafkaEntity)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = kafkaEntity.BrokerList,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SocketTimeoutMs = 60000,
                SessionTimeoutMs = 30000,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = SASLUSERNAME,
                SaslPassword = kafkaEntity.ConnString,
                SslCaLocation = kafkaEntity.Cacertlocation,
                GroupId = kafkaEntity.Consumergroup,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                BrokerVersionFallback = BROKERVERSION,
                EnableAutoCommit = false
                //Debug = "security,broker,protocol"    //Uncomment for librdkafka debugging information
            };
            return config;
        }

        public static ProducerConfig GetProducerConfig(KafkaEntity kafkaEntity)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = kafkaEntity.BrokerList,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = SASLUSERNAME,
                SaslPassword = kafkaEntity.ConnString,
                SslCaLocation = kafkaEntity.Cacertlocation,
                //Debug = "security,broker,protocol"        //Uncomment for librdkafka debugging information
            };
            return config;
        }
    }
}