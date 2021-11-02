using System;
using System.Threading.Tasks;
using Notificationengine = net.atos.daf.ct2.notificationengine;
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
using net.atos.daf.ct2.notificationservice.Entity;
using net.atos.daf.ct2.visibility;
using System.Collections.Generic;
using System.Linq;

namespace net.atos.daf.ct2.notificationservice.services
{
    public class PushNotificationManagementService : PushNotificationService.PushNotificationServiceBase
    {
        private readonly ILog _logger;
        private readonly entity.KafkaConfiguration _kafkaConfiguration;
        private readonly IConfiguration _configuration;
        //private readonly ITripAlertManager _tripAlertManager;
        private readonly Notificationengine.INotificationIdentifierManager _notificationIdentifierManager;
        private readonly Mapper _mapper;
        private readonly IVisibilityManager _visibilityManager;
        public PushNotificationManagementService(/*ITripAlertManager tripAlertManager, */IConfiguration configuration, Notificationengine.INotificationIdentifierManager notificationIdentifierManager, IVisibilityManager visibilityManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            this._configuration = configuration;
            _kafkaConfiguration = new entity.KafkaConfiguration();
            configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfiguration);
            //_tripAlertManager = tripAlertManager;
            _notificationIdentifierManager = notificationIdentifierManager;
            _mapper = new Mapper();
            _visibilityManager = visibilityManager;
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

        public override async Task<AlertVehicleDetails> GetEligibleAccountForAlert(AlertMesssageProp request, ServerCallContext context)
        {
            try
            {
                Notificationengine.entity.AlertMessageEntity alertMessageEntity = new Notificationengine.entity.AlertMessageEntity();
                alertMessageEntity.AlertId = request.AlertId;
                alertMessageEntity.Vin = request.VIN;
                alertMessageEntity.AlertCategory = request.AlertCategory;
                alertMessageEntity.AlertType = request.AlertType;
                alertMessageEntity.AlertUrgency = request.AlertUrgency;
                Notificationengine.entity.AlertVehicleEntity alertVehicleEntity = await _notificationIdentifierManager.GetEligibleAccountForAlert(alertMessageEntity);
                if (alertMessageEntity.AlertId == 0 && alertMessageEntity.AlertCategory == "O")
                {
                    alertVehicleEntity.OtaAccountIds = await _visibilityManager.GetAccountsForOTA(request.VIN);
                }
                return await Task.FromResult(_mapper.GetAlertVehicleEntity(alertVehicleEntity));
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AlertVehicleDetails
                {
                    Code = ResponseCode.Failed,
                    Message = "Get alert vehicle fail : " + ex.Message
                });
            }
        }

        public override async Task<AssociatedVehicleResponse> GetVehicleByAccountVisibility(VisibilityVehicleRequest request, ServerCallContext context)
        {
            try
            {
                var response = new AssociatedVehicleResponse();
                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Get("logged_in_orgid").Value);
                List<visibility.entity.VehicleDetailsAccountVisibilityForAlert> vehicleDetailsAccountVisibilty = new List<visibility.entity.VehicleDetailsAccountVisibilityForAlert>();
                if (request.FeatureIds != null)
                {
                    //foreach (int featureId in request.FeatureIds)
                    //{
                    IEnumerable<visibility.entity.VehicleDetailsAccountVisibilityForAlert> vehicleAccountVisibiltyList
                     = await _visibilityManager.GetVehicleByAccountVisibilityForAlert(request.AccountId, loggedInOrgId, request.OrganizationId, request.FeatureIds.ToArray());
                    //append visibile vins
                    vehicleDetailsAccountVisibilty.AddRange(vehicleAccountVisibiltyList);
                    //remove duplicate vins by key as vin
                    vehicleDetailsAccountVisibilty = vehicleDetailsAccountVisibilty.GroupBy(c => c.Vin, (key, c) => c.FirstOrDefault()).ToList();
                    //}
                }
                if (vehicleDetailsAccountVisibilty.Any())
                {
                    var res = JsonConvert.SerializeObject(vehicleDetailsAccountVisibilty);
                    response.AssociatedVehicle.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<AssociatedVehicle>>(res)
                        );
                    response.Code = ResponseCode.Success;
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AssociatedVehicleResponse
                {
                    Code = ResponseCode.Failed
                });
            }
        }
    }
}
