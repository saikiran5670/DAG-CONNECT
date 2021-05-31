using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using log4net;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using Newtonsoft.Json;
using TCUReceive;
using TCUSend;

namespace TCUProvisioning
{
    class TCUProvisioningDataProcess
    {
        private ILog log;
        private string brokerList;
        private string connStr;
        private string consumergroup;
        private string topic;
        private string cacertlocation;
        private string dafurl;
        private IAuditTraillib _auditlog;
        private IConfiguration config = null;

        private string access_url;
        private string grant_type;
        private string client_id;
        private string client_secret;

        public TCUProvisioningDataProcess(ILog log, IAuditTraillib auditlog, IConfiguration config)
        {
            this.log = log;
            this._auditlog = auditlog;
            this.config = config;
            brokerList = config.GetSection("EH_FQDN").Value;
            connStr = config.GetSection("EH_CONNECTION_STRING").Value;
            consumergroup = config.GetSection("CONSUMER_GROUP").Value;
            topic = config.GetSection("EH_NAME").Value;
            cacertlocation = config.GetSection("CA_CERT_LOCATION").Value;
            dafurl = config.GetSection("DAFURL").Value;
            access_url = config.GetSection("ACCESS_TOKEN_URL").Value;
            grant_type = config.GetSection("GRANT_TYPE").Value;
            client_id = config.GetSection("CLIENT_ID").Value;
            client_secret = config.GetSection("CLIENT_SECRET").Value;
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
                        var DAFData = createTCUDataInDAFFormat(TCUDataReceive);

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


        private TCUDataSend createTCUDataInDAFFormat(TCUDataReceive TCUDataReceive)
        {
            log.Info("Coverting message to DAF required format");

            TCU tcu = new TCU(TCUDataReceive.DeviceIdentifier, "Bosch", "1.0");
            TCURegistrationEvent TCURegistrationEvent = new TCURegistrationEvent(TCUDataReceive.Vin, tcu, TCUDataReceive.ReferenceDate);
            List<TCURegistrationEvent> TCURegistrationEvents = new List<TCURegistrationEvent>();
            TCURegistrationEvents.Add(TCURegistrationEvent);
            TCUDataSend send = new TCUDataSend(new TCURegistrationEvents(TCURegistrationEvents));

            return send;
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

        private async Task<HttpClient> GetHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var token = await GetElibilityToken(client);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            return client;

        }

        private async Task<Token> GetElibilityToken(HttpClient client)
        {
            var form = new Dictionary<string, string>
                {
                    {"grant_type", grant_type},
                    {"client_id", client_id},
                    {"client_secret", client_secret},
                };

            HttpResponseMessage tokenResponse = await client.PostAsync(access_url, new FormUrlEncodedContent(form));
            var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
            Token tok = JsonConvert.DeserializeObject<Token>(jsonContent);
            return tok;
        }

        private async Task postTCUProvisioningMesssageToDAF(TCUDataSend TCUDataSend)
        {
            int i = 0;
            string result = null;
            string TCUDataSendJson = JsonConvert.SerializeObject(TCUDataSend);
            try
            {
                var client = await GetHttpClient();
                var data = new StringContent(TCUDataSendJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.BadRequest;

                while (!(response.StatusCode == HttpStatusCode.OK) && i < 5)
                {
                    log.Info("Calling DAF rest API for sending data");
                    response = await client.PostAsync(dafurl, data);

                    log.Info("DAF Api respone is " + response.StatusCode);
                    result = response.Content.ReadAsStringAsync().Result;

                    i++;
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    log.Info(result);
                    await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 0, "TCU data Service Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "postTCUProvisioningMesssageToDAF method in TCU Vehicle Component", 0, 0, JsonConvert.SerializeObject(TCUDataSend));
                }
                else
                {

                    log.Error(result);
                    await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 0, "TCU data service Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "postTCUProvisioningMesssageToDAF method in TCU Vehicle Component", 0, 0, JsonConvert.SerializeObject(TCUDataSend));
                }

            }
            catch (Exception ex)
            {
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 0, "TCU data service Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "postTCUProvisioningMesssageToDAF method in TCU Vehicle Component", 0, 0, JsonConvert.SerializeObject(TCUDataSend));
                log.Error(ex.Message);

            }
        }

    }

    internal class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
