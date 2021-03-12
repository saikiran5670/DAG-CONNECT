using Confluent.Kafka;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TCUReceive;
using TCUSend;
using System.Net.Http.Headers;
using System.Net;
using System.Text;

namespace TCUProvisioning
{
    class TCUProvisioningDataProcess
    {
        private ILog log;
        private string brokerList = ConfigurationManager.AppSetting["EH_FQDN"];
        private string connStr = ConfigurationManager.AppSetting["EH_CONNECTION_STRING"];
        private string consumergroup = ConfigurationManager.AppSetting["CONSUMER_GROUP"];
        private string topic = ConfigurationManager.AppSetting["EH_NAME"];
        private string cacertlocation = ConfigurationManager.AppSetting["CA_CERT_LOCATION"];
        private string dafurl = ConfigurationManager.AppSetting["DAFURL"];

        public TCUProvisioningDataProcess(ILog log)
        {
            this.log = log;
        }

        public async Task readTCUProvisioningDataAsync()
        {
            ConsumerConfig config = getConsumer();

            using (var consumer = new ConsumerBuilder<Null, string>(config).Build())
            {
             
                log.Info("Subscribing Topic");
                consumer.Subscribe(topic);

                while (true)
                {
                    try
                    {
                        log.Info("Consuming Messages");
                        ConsumeResult<Null, string> msg = consumer.Consume();
                        String TCUDataFromTopic = msg.Message.Value;
                        TCUDataReceive TCUDataReceive = JsonConvert.DeserializeObject<TCUDataReceive>(TCUDataFromTopic);
                        String DAFData = createTCUDataInDAFFormat(TCUDataReceive);

                        Console.WriteLine(DAFData);

                        await postTCUProvisioningMesssageToDAF(DAFData);

                        log.Info("Commiting message");
                        consumer.Commit(msg);

                    }
                    catch (ConsumeException e)
                    {
                        log.Error($"Consume error: {e.Error.Reason}");
                        consumer.Close();
                        //Environment.Exit(1);
                    }
                    catch (Exception e)
                    {
                        log.Error($"Error: {e.Message}");
                        consumer.Close();
                       // Environment.Exit(1);

                    }
                    
                }
            }
        }


        private String createTCUDataInDAFFormat(TCUDataReceive TCUDataReceive)
        {
            log.Info("Coverting message to DAF required format");

            TCU tcu = new TCU(TCUDataReceive.DeviceIdentifier, "Bosch", "1.0");
            TCURegistrationEvent TCURegistrationEvent = new TCURegistrationEvent(TCUDataReceive.Vin, tcu, TCUDataReceive.ReferenceDate);
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

        private  HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;

        }

        private async Task postTCUProvisioningMesssageToDAF(string TCUDataSendJson)
        {
            try
            {
                var client = GetHttpClient();
                var data = new StringContent(TCUDataSendJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.BadRequest;
                //log.Info(response.StatusCode);

                while (!(response.StatusCode == HttpStatusCode.OK))
                {
                    log.Info("Calling DAF rest API for sending data");
                    response = await client.PostAsync(dafurl, data);

                    log.Info("DAF Api respone is " +response.StatusCode);
                    string result = response.Content.ReadAsStringAsync().Result;
                    //log.Info(result);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                
            }
        }

    }
}
