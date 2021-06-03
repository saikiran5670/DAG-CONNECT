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
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.tcucore;
using Newtonsoft.Json;


namespace net.atos.daf.ct2.tcudataservice
{
    public class TCUProvisioningDataProcess : ITcuProvisioningDataReceive, ITcuProvisioningDataPost
    {
        private readonly string _brokerList;
        private readonly string _connStr;
        private readonly string _consumergroup;
        private readonly string _topic;
        private readonly string _psqlconnstring;
        private readonly string _cacertlocation;
        private readonly string _dafurl;
        private readonly string _boschTcuBrand;
        private readonly string _boschTcuVesrion;
        private readonly IDataAccess _dataacess = null;
        private readonly ILog _log = null;
        private readonly IConfiguration _config = null;
        private readonly IAuditTraillib _auditlog = null;
        private readonly IAuditLogRepository _auditrepo = null;

        private readonly string _accessUrl;
        private readonly string _grantType;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public TCUProvisioningDataProcess(ILog log, IConfiguration config)
        {
            this._log = log;
            this._config = config;
            _brokerList = this._config.GetSection("EH_FQDN").Value;
            _connStr = this._config.GetSection("EH_CONNECTION_STRING").Value;
            _consumergroup = this._config.GetSection("CONSUMER_GROUP").Value;
            _topic = this._config.GetSection("EH_NAME").Value;
            _psqlconnstring = this._config.GetSection("PSQL_CONNSTRING").Value;
            _cacertlocation = this._config.GetSection("CA_CERT_LOCATION").Value;
            _boschTcuBrand = this._config.GetSection("BOSCH_TCU_BRAND").Value;
            _boschTcuVesrion = this._config.GetSection("BOSCH_TCU_VERSION").Value;
            _dafurl = this._config.GetSection("DAFURL").Value;
            _accessUrl = this._config.GetSection("ACCESS_TOKEN_URL").Value;
            _grantType = this._config.GetSection("GRANT_TYPE").Value;
            _clientId = this._config.GetSection("CLIENT_ID").Value;
            _clientSecret = this._config.GetSection("CLIENT_SECRET").Value;

            _dataacess = new PgSQLDataAccess(_psqlconnstring);
            _auditrepo = new AuditLogRepository(_dataacess);
            _auditlog = new AuditTraillib(_auditrepo);
        }

        public async Task ReadTcuProvisioningData()
        {
            KafkaConfig kafkaConfig = new KafkaConfig();
            ConsumerConfig consumerConfig = kafkaConfig.GetConsumerConfig(_brokerList, _connStr, _cacertlocation, _consumergroup);

            using (var consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build())
            {
                _log.Info("Subscribing Topic");
                consumer.Subscribe(_topic);

                while (true)
                {
                    try
                    {
                        _log.Info("Consuming Messages");
                        var msg = consumer.Consume();
                        TCUDataReceive tcuDataReceive = JsonConvert.DeserializeObject<TCUDataReceive>(msg.Message.Value);

                        await PostTcuProvisioningMesssageToDAF(CreateTCUDataInDAFFormat(tcuDataReceive));

                        _log.Info("Commiting message");
                        consumer.Commit(msg);

                    }
                    catch (ConsumeException e)
                    {
                        _log.Error($"Consume error: {e.Error.Reason}");
                        consumer.Close();
                    }
                    catch (Exception e)
                    {
                        _log.Error($"Error: {e.Message}");
                        consumer.Close();
                    }
                }
            }
        }

        public async Task PostTcuProvisioningMesssageToDAF(TCUDataSend tcuDataSend)
        {
            int i = 0;
            string result = null;
            string tcuDataSendJson = JsonConvert.SerializeObject(tcuDataSend);
            try
            {
                var client = await GetHttpClient();
                var data = new StringContent(tcuDataSendJson, Encoding.UTF8, "application/json");
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
                    await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 0, "TCU data Service Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "postTCUProvisioningMesssageToDAF method in TCU Vehicle Component_Sucess", 0, 0, JsonConvert.SerializeObject(tcuDataSend));
                }
                else
                {
                    _log.Error(result);
                    await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 0, "TCU data service Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "postTCUProvisioningMesssageToDAF method in TCU Vehicle Component_Failed", 0, 0, JsonConvert.SerializeObject(tcuDataSend));
                }

            }
            catch (Exception ex)
            {
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 0, "TCU data service Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "postTCUProvisioningMesssageToDAF method in TCU Vehicle Component_Failed", 0, 0, JsonConvert.SerializeObject(tcuDataSend));
                _log.Error(ex.Message);

            }
        }

        private TCUDataSend CreateTCUDataInDAFFormat(TCUDataReceive tcuDataReceive)
        {
            _log.Info("Coverting message to DAF required format");

            TCU tcu = new TCU(tcuDataReceive.DeviceIdentifier, _boschTcuBrand, _boschTcuVesrion);
            TCURegistrationEvent tcuRegistrationEvent = new TCURegistrationEvent(tcuDataReceive.Vin, tcu, tcuDataReceive.ReferenceDate);
            List<TCURegistrationEvent> tcuRegistrationEvents = new List<TCURegistrationEvent>();
            tcuRegistrationEvents.Add(tcuRegistrationEvent);
            TCUDataSend tcuSend = new TCUDataSend(new TCURegistrationEvents(tcuRegistrationEvents));

            return tcuSend;
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

        private async Task<TCUDataToken> GetElibilityToken(HttpClient client)
        {
            var form = new Dictionary<string, string>
                {
                    {"grant_type", _grantType},
                    {"client_id", _clientId},
                    {"client_secret", _clientSecret},
                };

            HttpResponseMessage tokenResponse = await client.PostAsync(_accessUrl, new FormUrlEncodedContent(form));
            var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
            TCUDataToken token = JsonConvert.DeserializeObject<TCUDataToken>(jsonContent);
            return token;
        }

    }


}
