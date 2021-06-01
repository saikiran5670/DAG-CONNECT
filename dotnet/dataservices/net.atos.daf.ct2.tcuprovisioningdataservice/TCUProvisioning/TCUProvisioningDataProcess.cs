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
        private ILog _log;
        private string _brokerList;
        private string _connStr;
        private string _consumergroup;
        private string _topic;
        private string _cacertlocation;
        private string _dafurl;
        private IAuditTraillib _auditlog;
        private IConfiguration _config;

        private string _access_url;
        private string _grant_type;
        private string _client_id;
        private string _client_secret;

        public TCUProvisioningDataProcess(ILog log, IAuditTraillib auditlog, IConfiguration config)
        {
            this._log = log;
            this._auditlog = auditlog;
            this._config = config;
            _brokerList = config.GetSection("EH_FQDN").Value;
            _connStr = config.GetSection("EH_CONNECTION_STRING").Value;
            _consumergroup = config.GetSection("CONSUMER_GROUP").Value;
            _topic = config.GetSection("EH_NAME").Value;
            _cacertlocation = config.GetSection("CA_CERT_LOCATION").Value;
            _dafurl = config.GetSection("DAFURL").Value;
            _access_url = config.GetSection("ACCESS_TOKEN_URL").Value;
            _grant_type = config.GetSection("GRANT_TYPE").Value;
            _client_id = config.GetSection("CLIENT_ID").Value;
            _client_secret = config.GetSection("CLIENT_SECRET").Value;
        }


        public async Task ReadTCUProvisioningDataAsync()
        {
            ConsumerConfig config = GetConsumer();

            using (var consumer = new ConsumerBuilder<Null, string>(config).Build())
            {
                _log.Info("Subscribing Topic");
                consumer.Subscribe(_topic);

                while (true)
                {
                    try
                    {
                        _log.Info("Consuming Messages");
                        ConsumeResult<Null, string> msg = consumer.Consume();
                        String TCUDataFromTopic = msg.Message.Value;
                        TCUDataReceive TCUDataReceive = JsonConvert.DeserializeObject<TCUDataReceive>(TCUDataFromTopic);
                        var DAFData = CreateTCUDataInDAFFormat(TCUDataReceive);

                        await PostTCUProvisioningMesssageToDAF(DAFData);

                        _log.Info("Commiting message");
                        consumer.Commit(msg);

                    }
                    catch (ConsumeException e)
                    {
                        _log.Error($"Consume error: {e.Error.Reason}");
                        consumer.Close();
                        //Environment.Exit(1);
                    }
                    catch (Exception e)
                    {
                        _log.Error($"Error: {e.Message}");
                        consumer.Close();
                        // Environment.Exit(1);

                    }

                }
            }
        }


        private TCUDataSend CreateTCUDataInDAFFormat(TCUDataReceive TCUDataReceive)
        {
            _log.Info("Coverting message to DAF required format");

            TCU tcu = new TCU(TCUDataReceive.DeviceIdentifier, "Bosch", "1.0");
            TCURegistrationEvent TCURegistrationEvent = new TCURegistrationEvent(TCUDataReceive.Vin, tcu, TCUDataReceive.ReferenceDate);
            List<TCURegistrationEvent> TCURegistrationEvents = new List<TCURegistrationEvent>();
            TCURegistrationEvents.Add(TCURegistrationEvent);
            TCUDataSend send = new TCUDataSend(new TCURegistrationEvents(TCURegistrationEvents));

            return send;
        }

        private ConsumerConfig GetConsumer()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _brokerList,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SocketTimeoutMs = 60000,
                SessionTimeoutMs = 30000,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "$ConnectionString",
                SaslPassword = _connStr,
                SslCaLocation = _cacertlocation,
                GroupId = _consumergroup,
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
                    {"grant_type", _grant_type},
                    {"client_id", _client_id},
                    {"client_secret", _client_secret},
                };

            HttpResponseMessage tokenResponse = await client.PostAsync(_access_url, new FormUrlEncodedContent(form));
            var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
            Token tok = JsonConvert.DeserializeObject<Token>(jsonContent);
            return tok;
        }

        private async Task PostTCUProvisioningMesssageToDAF(TCUDataSend TCUDataSend)
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
                    _log.Info("Calling DAF rest API for sending data");
                    response = await client.PostAsync(_dafurl, data);

                    _log.Info("DAF Api respone is " + response.StatusCode);
                    result = response.Content.ReadAsStringAsync().Result;

                    i++;
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _log.Info(result);
                    await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 0, "TCU data Service Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "postTCUProvisioningMesssageToDAF method in TCU Vehicle Component", 0, 0, JsonConvert.SerializeObject(TCUDataSend));
                }
                else
                {

                    _log.Error(result);
                    await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 0, "TCU data service Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "postTCUProvisioningMesssageToDAF method in TCU Vehicle Component", 0, 0, JsonConvert.SerializeObject(TCUDataSend));
                }

            }
            catch (Exception ex)
            {
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 0, "TCU data service Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "postTCUProvisioningMesssageToDAF method in TCU Vehicle Component", 0, 0, JsonConvert.SerializeObject(TCUDataSend));
                _log.Error(ex.Message);

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
