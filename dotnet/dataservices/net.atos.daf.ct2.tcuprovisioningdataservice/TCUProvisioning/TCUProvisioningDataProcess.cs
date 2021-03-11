using Confluent.Kafka;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TCUReceive;
using TCUSend;

namespace TCUProvisioning
{
    class TCUProvisioningDataProcess
    {
        private ILog log;
        private string brokerList = ConfigurationManager.AppSetting["EH_FQDN"];
        private string connStr = ConfigurationManager.AppSetting["EH_CONNECTION_STRING"];
        private string consumergroup = ConfigurationManager.AppSetting["CONSUMER_GROUP"];
        private string topic = ConfigurationManager.AppSetting["EH_NAME"];
        private string psqlconnstring = ConfigurationManager.AppSetting["psqlconnstring"];
        private string cacertlocation = ConfigurationManager.AppSetting["CA_CERT_LOCATION"];

        public TCUProvisioningDataProcess(ILog log)
        {
            this.log = log;
        }

        public void readTCUProvisioningData()
        {
            ConsumerConfig config = getConsumer();

            using (var consumer = new ConsumerBuilder<Null, string>(config).Build())
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };
                log.Info("Subscribing Topic");
                consumer.Subscribe(topic);

                while (true)
                {
                    try
                    {
                        log.Info("Consuming Messages");
                        var msg = consumer.Consume(cts.Token);
                        String TCUDataFromTopic = msg.Message.Value;
                        TCUDataReceive TCUDataReceive = JsonConvert.DeserializeObject<TCUDataReceive>(TCUDataFromTopic);
                        String DAFData = createTCUDataInDAFFormat(TCUDataReceive);

                        Console.WriteLine(DAFData);

                    }
                    catch (ConsumeException e)
                    {
                        log.Error($"Consume error: {e.Error.Reason}");

                    }
                    catch (Exception e)
                    {
                        log.Error($"Error: {e.Message}");

                    }
                }
            }
        }


        private static String createTCUDataInDAFFormat(TCUDataReceive TCUDataReceive)
        {

            TCU tcu = new TCU(TCUDataReceive.DeviceIdentifier, "Bosch", "1.0");
            TCURegistrationEvent TCURegistrationEvent = new TCURegistrationEvent(TCUDataReceive.Vin, tcu, "Yes", TCUDataReceive.ReferenceDate);
            List<TCURegistrationEvent> TCURegistrationEvents = new List<TCURegistrationEvent>();
            TCURegistrationEvents.Add(TCURegistrationEvent);
            TCUDataSend send = new TCUDataSend(new TCURegistrationEvents(TCURegistrationEvents));

            String TCUDataSendJson = JsonConvert.SerializeObject(send);
            return TCUDataSendJson;
        }


        private ConsumerConfig getConsumer()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = brokerList,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SocketTimeoutMs = 60000,
                SessionTimeoutMs = 30000,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "$ConnectionString",
                SaslPassword = connStr,
                SslCaLocation = cacertlocation,
                GroupId = consumergroup,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                BrokerVersionFallback = "1.0.0",
                EnableAutoCommit = false
                //Debug = "security,broker,protocol"    //Uncomment for librdkafka debugging information
            };
            return config;
        }


      


    


     

       



  
       


    }
}
