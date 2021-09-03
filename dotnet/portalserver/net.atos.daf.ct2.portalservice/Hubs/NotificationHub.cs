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
        private static int _alertId = 1;
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
            try
            {
                while (true)
                {
                    TripAlert tripAlert = new TripAlert
                    {
                        Id = _alertId,
                        Tripid = Convert.ToString(Guid.NewGuid()),
                        Vin = "XLR0998HGFFT76657",
                        CategoryType = "L",
                        Type = "G",
                        Alertid = _alertId * 2,
                        Latitude = 51.12768896,
                        Longitude = 4.935644520,
                        AlertGeneratedTime = 1626965785,
                        ThresholdValue = 8766,
                        ValueAtAlertTime = 8767,
                        ThresholdValueUnitType = "M",
                        LandmarkName = this.Context.ConnectionId,
                    };
                    await Clients.All.SendAsync("NotifyAlertResponse", JsonConvert.SerializeObject(tripAlert));
                    if (_alertId == 1000)
                    {
                        _alertId = 1;
                    }
                    _alertId = _alertId + 1;
                    Thread.Sleep(10000);
                }
            }
            catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.Cancelled)
            {
                _logger.Error(null, ex);
                await Clients.Client(this.Context.ConnectionId).SendAsync("askServerResponse", ex.Message);
            }
            catch (Exception ex)
            {
                _ = ex.Message + someTextFromClient;
                _logger.Error(null, ex);
                await Clients.Client(this.Context.ConnectionId).SendAsync("askServerResponse", ex.Message);
            }
        }

        private async Task ReadKafkaMessages()
        {
            try
            {
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
                        AlertMesssageProp alertMesssageProp = new AlertMesssageProp();
                        alertMesssageProp.VIN = tripAlert.Vin;
                        alertMesssageProp.AlertId = tripAlert.Alertid;
                        await _pushNotofocationServiceClient.GetEligibleAccountForAlertAsync(alertMesssageProp);
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
