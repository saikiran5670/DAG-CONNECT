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
using log4net;
using System.Reflection;
using net.atos.daf.ct2.confluentkafka.entity;

namespace net.atos.daf.ct2.notificationservice.services
{
    public class PushNotificationManagementService : PushNotificationService.PushNotificationServiceBase
    {
        private readonly ILog _logger;
        private readonly entity.KafkaConfiguration _kafkaConfiguration;
        private readonly IConfiguration _configuration;
        //private readonly ITripAlertManager _tripAlertManager;
        public PushNotificationManagementService(/*ITripAlertManager tripAlertManager, */IConfiguration configuration)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            this._configuration = configuration;
            _kafkaConfiguration = new entity.KafkaConfiguration();
            configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfiguration);
            //_tripAlertManager = tripAlertManager;
        }

        public override async Task GetAlertMessageStream(Google.Protobuf.WellKnownTypes.Empty _, IServerStreamWriter<AlertMessageData> responseStream, ServerCallContext context)
        {
            try
            {
                AlertMessageData alertMessageData = new AlertMessageData();
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    TripAlert tripAlert = new TripAlert();
                    //await Task.Delay(50);
                    ConsumeResult<Null, string> message = new ConsumeResult<Null, string>();//Worker.Consumer(_kafkaConfiguration.EH_FQDN, _kafkaConfiguration.EH_CONNECTION_STRING, _kafkaConfiguration.CONSUMER_GROUP, _kafkaConfiguration.EH_NAME, _kafkaConfiguration.CA_CERT_LOCATION);
                    if (message != null)
                    {
                        tripAlert = JsonConvert.DeserializeObject<TripAlert>(message.Message.Value);
                        alertMessageData = new AlertMessageData
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
                    }
                    /* Temporary disconnected from DB*/
                    //pushnotificationcorecomponent.Entity.TripAlert tripAlertDB = JsonConvert.DeserializeObject<pushnotificationcorecomponent.Entity.TripAlert>(message.Message.Value);
                    //await _tripAlertManager.CreateTripAlert(tripAlertDB);
                    /* Temporary disconnected from DB*/
                    //AlertMessageData AlertMessageData = JsonConvert.DeserializeObject<AlertMessageData>(message.Message.Value);
                    await responseStream.WriteAsync(alertMessageData).ConfigureAwait(true);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                await Task.FromResult(new AlertMessageData
                {
                    Message = "Exception :-" + ex.Message,
                    Code = ResponseCode.InternalServerError
                });
            }
        }
    }
}
