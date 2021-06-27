using System;
using System.Threading.Tasks;
using net.atos.daf.ct2.notificationengine;
using Microsoft.Extensions.Configuration;
using Confluent.Kafka;
using Newtonsoft.Json;
using net.atos.daf.ct2.notificationservice.entity;
using net.atos.daf.ct2.pushnotificationservice;
using net.atos.daf.ct2.notificationservice;
using Grpc.Core;

namespace net.atos.daf.ct2.notificationservice.services
{
    public class PushNotificationManagementService : PushNotificationService.PushNotificationServiceBase
    {
        private readonly KafkaConfiguration _kafkaConfiguration;
        private readonly IConfiguration _configuration;
        private readonly ITripAlertManager _tripAlertManager;
        public PushNotificationManagementService(ITripAlertManager tripAlertManager, IConfiguration Configuration)
        {
            this._configuration = Configuration;
            _kafkaConfiguration = new KafkaConfiguration();
            Configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfiguration);
            _tripAlertManager = tripAlertManager;
        }

        public override async Task GetAlertMessageStream(Google.Protobuf.WellKnownTypes.Empty _, IServerStreamWriter<AlertMessageData> responseStream, ServerCallContext context)
        {
            try
            {
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    //await Task.Delay(50);
                    ConsumeResult<Null, string> message = Worker.Consumer(_kafkaConfiguration.EH_FQDN, _kafkaConfiguration.EH_CONNECTION_STRING, _kafkaConfiguration.CONSUMER_GROUP, _kafkaConfiguration.EH_NAME, _kafkaConfiguration.CA_CERT_LOCATION);
                    TripAlert tripAlert = JsonConvert.DeserializeObject<TripAlert>(message.Message.Value);

                    /* Temporary disconnected from DB*/
                    //pushnotificationcorecomponent.Entity.TripAlert tripAlertDB = JsonConvert.DeserializeObject<pushnotificationcorecomponent.Entity.TripAlert>(message.Message.Value);
                    //await _tripAlertManager.CreateTripAlert(tripAlertDB);
                    /* Temporary disconnected from DB*/

                    //AlertMessageData AlertMessageData = JsonConvert.DeserializeObject<AlertMessageData>(message.Message.Value);
                    var alertMessageData = new AlertMessageData
                    {
                        Id = tripAlert.Id,
                        Tripid = tripAlert.Tripid,
                        Vin = tripAlert.Vin,
                        CategoryType = tripAlert.CategoryType,
                        Type = tripAlert.Type,
                        Name = tripAlert.Name,
                        Alertid = tripAlert.Alertid,
                        ThresholdValue = tripAlert.ThresholdValue,
                        ThresholdValueUnitType = tripAlert.ThresholdValueUnitType,
                        ValueAtAlertTime = tripAlert.ValueAtAlertTime,
                        Latitude = tripAlert.Latitude,
                        Longitude = tripAlert.Longitude,
                        AlertGeneratedTime = tripAlert.AlertGeneratedTime,
                        MessageTimestamp = tripAlert.MessageTimestamp,
                        CreatedAt = tripAlert.CreatedAt,
                        ModifiedAt = tripAlert.ModifiedAt
                    };
                    await responseStream.WriteAsync(alertMessageData).ConfigureAwait(true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
