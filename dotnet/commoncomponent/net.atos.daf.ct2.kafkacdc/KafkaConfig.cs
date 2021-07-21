using Confluent.Kafka;

namespace net.atos.daf.ct2.kafkacdc
{
    public class KafkaConfig
    {
        private const string SASLUSERNAME = "$ConnectionString";
        private const string BROKERVERSION = "1.0.0";

        public ConsumerConfig GetConsumerConfig(string brokerList
                , string connStr
                , string cacertLocation
                , string consumerGroup)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = brokerList,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SocketTimeoutMs = 60000,
                SessionTimeoutMs = 30000,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = SASLUSERNAME,
                SaslPassword = connStr,
                SslCaLocation = cacertLocation,
                GroupId = consumerGroup,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                BrokerVersionFallback = BROKERVERSION,
                EnableAutoCommit = false
                //Debug = "security,broker,protocol"    //Uncomment for librdkafka debugging information
            };
            return config;
        }

        public ProducerConfig GetProducerConfig(string brokerList
                , string connStr
                , string cacertLocation)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = brokerList,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = SASLUSERNAME,
                SaslPassword = connStr,
                SslCaLocation = cacertLocation,
                //Debug = "security,broker,protocol"        //Uncomment for librdkafka debugging information
            };
            return config;
        }
    }
}
