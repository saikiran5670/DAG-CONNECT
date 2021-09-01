using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Grpc.Core;
using log4net;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.confluentkafka;
using net.atos.daf.ct2.portalservice.Entity.Alert;
using net.atos.daf.ct2.pushnotificationservice;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.portalservice.hubs
{
    public class NotificationHub : Hub
    {
        private readonly ILog _logger;
        private readonly PushNotificationService.PushNotificationServiceClient _pushNotofocationServiceClient;
        private readonly Entity.KafkaConfiguration _kafkaConfiguration;
        private readonly IConfiguration _configuration;
        private readonly Entity.Alert.Mapper _mapper;
        public NotificationHub(PushNotificationService.PushNotificationServiceClient pushNotofocationServiceClient, IConfiguration configuration)
        {
            _pushNotofocationServiceClient = pushNotofocationServiceClient;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            this._configuration = configuration;
            _kafkaConfiguration = new Entity.KafkaConfiguration();
            configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfiguration);
            _mapper = new Entity.Alert.Mapper();
        }
        public async Task NotifyAlert(string someTextFromClient)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(50));
            using var streamingCall = _pushNotofocationServiceClient.GetAlertMessageStream(new Google.Protobuf.WellKnownTypes.Empty(), cancellationToken: cts.Token);
            string str = string.Empty;
            try
            {
                await foreach (var alertMessageData in streamingCall.ResponseStream.ReadAllAsync(cancellationToken: cts.Token))
                {
                    await Clients.All.SendAsync("NotifyAlertResponse", this.Context.ConnectionId + " " + JsonConvert.SerializeObject(alertMessageData) + " " + someTextFromClient);
                }
            }
            catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.Cancelled)
            {
                _logger.Error(null, ex);
                await Clients.Client(this.Context.ConnectionId).SendAsync("askServerResponse", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                await Clients.Client(this.Context.ConnectionId).SendAsync("askServerResponse", ex.Message);
            }
        }


        private async Task ReadKafkaMessages()
        {
            try
            {
                List<AccountClient> accountClients = new List<AccountClient>();
                confluentkafka.entity.KafkaConfiguration kafkaEntity = new confluentkafka.entity.KafkaConfiguration()
                {
                    BrokerList = _kafkaConfiguration.EH_FQDN,
                    ConnString = _kafkaConfiguration.EH_CONNECTION_STRING,
                    Topic = _kafkaConfiguration.EH_NAME,
                    Cacertlocation = _kafkaConfiguration.CA_CERT_LOCATION,
                    Consumergroup = _kafkaConfiguration.CONSUMER_GROUP
                };
                //Pushing message to kafka topic
                ConsumeResult<string, string> response = KafkaConfluentWorker.Consumer(kafkaEntity);
                TripAlert tripAlert = new TripAlert();
                if (response != null)
                {
                    Console.WriteLine(response.Message.Value);
                    tripAlert = JsonConvert.DeserializeObject<TripAlert>(response.Message.Value);
                    if (tripAlert != null && tripAlert.Alertid > 0)
                    {
                        AccountClientMapping accountClientMapping = new AccountClientMapping();
                        accountClientMapping.AlertMessageData = _mapper.GetAlertMessageEntity(tripAlert);
                        foreach (var item in accountClients)
                        {
                            AccountClient accountClient = new AccountClient();
                            accountClient.AccountId = item.AccountId;
                            accountClient.OrgId = item.OrgId;
                            accountClient.HubClientId = item.HubClientId;
                            accountClientMapping.AccountClient.Add(accountClient);
                        }
                        await _pushNotofocationServiceClient.GetEligibleAccountForAlertAsync(accountClientMapping);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
