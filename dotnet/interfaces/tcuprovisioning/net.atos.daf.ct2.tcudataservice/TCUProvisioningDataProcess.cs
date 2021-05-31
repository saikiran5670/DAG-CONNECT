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
        private string brokerList;
        private string connStr;
        private string consumergroup;
        private string topic;
        private string psqlconnstring;
        private string cacertlocation;
        private string dafurl;
        private string boschTcuBrand;
        private string boschTcuVesrion;
        IDataAccess dataacess = null;
        private ILog log = null;
        IConfiguration config = null;
        IAuditTraillib auditlog = null;
        IAuditLogRepository auditrepo = null;

        private string accessUrl;
        private string grantType;
        private string clientId;
        private string clientSecret;

        public TCUProvisioningDataProcess(ILog _log, IConfiguration _config)
        {
            this.log = _log;
            this.config = _config;
            brokerList = config.GetSection("EH_FQDN").Value;
            connStr = config.GetSection("EH_CONNECTION_STRING").Value;
            consumergroup = config.GetSection("CONSUMER_GROUP").Value;
            topic = config.GetSection("EH_NAME").Value;
            psqlconnstring = config.GetSection("PSQL_CONNSTRING").Value;
            cacertlocation = config.GetSection("CA_CERT_LOCATION").Value;
            boschTcuBrand = config.GetSection("BOSCH_TCU_BRAND").Value;
            boschTcuVesrion = config.GetSection("BOSCH_TCU_VERSION").Value;
            dafurl = config.GetSection("DAFURL").Value;
            accessUrl = config.GetSection("ACCESS_TOKEN_URL").Value;
            grantType = config.GetSection("GRANT_TYPE").Value;
            clientId = config.GetSection("CLIENT_ID").Value;
            clientSecret = config.GetSection("CLIENT_SECRET").Value;

            dataacess = new PgSQLDataAccess(psqlconnstring);
            auditrepo = new AuditLogRepository(dataacess);
            auditlog = new AuditTraillib(auditrepo);
        }

        public async Task ReadTcuProvisioningData()
        {
            KafkaConfig kafkaConfig = new KafkaConfig();
            ConsumerConfig consumerConfig = kafkaConfig.GetConsumerConfig(brokerList, connStr, cacertlocation, consumergroup);

            using (var consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build())
            {
                log.Info("Subscribing Topic");
                consumer.Subscribe(topic);

                while (true)
                {
                    try
                    {
                        log.Info("Consuming Messages");
                        var msg = consumer.Consume();
                        TCUDataReceive tcuDataReceive = JsonConvert.DeserializeObject<TCUDataReceive>(msg.Message.Value);

                        await PostTcuProvisioningMesssageToDAF(CreateTCUDataInDAFFormat(tcuDataReceive));

                        log.Info("Commiting message");
                        consumer.Commit(msg);

                    }
                    catch (ConsumeException e)
                    {
                        log.Error($"Consume error: {e.Error.Reason}");
                        consumer.Close();
                    }
                    catch (Exception e)
                    {
                        log.Error($"Error: {e.Message}");
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
                    log.Info("Calling DAF rest API for sending data");
                    response = await client.PostAsync(dafurl, data);

                    log.Info("DAF Api respone is " + response.StatusCode);
                    result = response.Content.ReadAsStringAsync().Result;

                    i++;
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    log.Info(result);
                    await auditlog.AddLogs(DateTime.Now, DateTime.Now, 0, "TCU data Service Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "postTCUProvisioningMesssageToDAF method in TCU Vehicle Component_Sucess", 0, 0, JsonConvert.SerializeObject(tcuDataSend));
                }
                else
                {
                    log.Error(result);
                    await auditlog.AddLogs(DateTime.Now, DateTime.Now, 0, "TCU data service Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "postTCUProvisioningMesssageToDAF method in TCU Vehicle Component_Failed", 0, 0, JsonConvert.SerializeObject(tcuDataSend));
                }

            }
            catch (Exception ex)
            {
                await auditlog.AddLogs(DateTime.Now, DateTime.Now, 0, "TCU data service Component", "TCU Component", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.FAILED, "postTCUProvisioningMesssageToDAF method in TCU Vehicle Component_Failed", 0, 0, JsonConvert.SerializeObject(tcuDataSend));
                log.Error(ex.Message);

            }
        }

        private TCUDataSend CreateTCUDataInDAFFormat(TCUDataReceive tcuDataReceive)
        {
            log.Info("Coverting message to DAF required format");

            TCU tcu = new TCU(tcuDataReceive.DeviceIdentifier, boschTcuBrand, boschTcuVesrion);
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
                    {"grant_type", grantType},
                    {"client_id", clientId},
                    {"client_secret", clientSecret},
                };

            HttpResponseMessage tokenResponse = await client.PostAsync(accessUrl, new FormUrlEncodedContent(form));
            var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
            TCUDataToken token = JsonConvert.DeserializeObject<TCUDataToken>(jsonContent);
            return token;
        }

    }


}
