using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Grpc.Core;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.confluentkafka;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Alert;
using net.atos.daf.ct2.pushnotificationservice;
using Newtonsoft.Json;
namespace net.atos.daf.ct2.portalservice.hubs
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class NotificationHub : Hub
    {
        private readonly ILog _logger;
        private readonly PushNotificationService.PushNotificationServiceClient _pushNotofocationServiceClient;
        private readonly Entity.KafkaConfiguration _kafkaConfiguration;
        private readonly IConfiguration _configuration;
        private readonly Entity.Alert.Mapper _mapper;
        private readonly AccountSignalRClientsMappingList _accountSignalRClientsMappingList;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SessionHelper _sessionHelper;
        private readonly HeaderObj _userDetails;
        private static int _pkId = 1;

        public NotificationHub(PushNotificationService.PushNotificationServiceClient pushNotofocationServiceClient, IConfiguration configuration, AccountSignalRClientsMappingList accountSignalRClientsMappingList, IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper)
        {
            _pushNotofocationServiceClient = pushNotofocationServiceClient;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            this._configuration = configuration;
            _kafkaConfiguration = new Entity.KafkaConfiguration();
            configuration.GetSection("PushAlertKafkaConfiguration").Bind(_kafkaConfiguration);
            _mapper = new Entity.Alert.Mapper();
            _accountSignalRClientsMappingList = accountSignalRClientsMappingList;
            _httpContextAccessor = httpContextAccessor;
            _sessionHelper = sessionHelper;
            _userDetails = _sessionHelper.GetSessionInfo(httpContextAccessor.HttpContext.Session);
        }
        public async Task NotifyAlert(string someTextFromClient)
        {
            try
            {
                while (true)
                {
                    NotificationAlertMessages notificationAlertMessages = new NotificationAlertMessages
                    {
                        TripAlertId = _pkId,
                        TripId = Convert.ToString(Guid.NewGuid()),
                        Vin = "XLR0998HGFFT76657",
                        AlertCategory = "L",
                        AlertType = "G",
                        AlertId = _pkId * 2,
                        AlertGeneratedTime = DateTime.Now.Millisecond,
                        VehicleGroupId = 185,
                        VehicleGroupName = "Fleet",
                        VehicleName = "testKri",
                        VehicleLicencePlate = "testKri",
                        AlertCategoryKey = _userDetails.AccountId.ToString(),
                        AlertTypeKey = _userDetails.OrgId.ToString(),
                        UrgencyTypeKey = Context.ConnectionId,
                        UrgencyLevel = "C"
                    };
                    //       IReadOnlyList<string> connectionIds = _accountSignalRClientsMappingList._accountClientMapperList.Distinct().Where(pre => pre.HubClientId == Context?.ConnectionId).Select(clients => clients.HubClientId).ToList();
                    await Clients.All.SendAsync("NotifyAlertResponse", JsonConvert.SerializeObject(notificationAlertMessages));
                    //IReadOnlyList<string> connectionIds = _accountSignalRClientsMappingList._accountClientMapperList.Distinct().Select(clients => clients.HubClientId).ToList();
                    //await Clients.Clients(connectionIds).SendAt5sync("NotifyAlertResponse", JsonConvert.SerializeObject(tripAlert));
                    if (_pkId > 1000)
                    {
                        _pkId = 1;
                    }
                    _pkId = _pkId + 1;
                    Thread.Sleep(1000);
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
        public override async Task OnConnectedAsync()
        {
            try
            {
                if (Context?.ConnectionId != null)
                {
                    AccountSignalRClientMapper accountSignalRClientMapper = new AccountSignalRClientMapper()
                    {
                        AccountId = 187,// _userDetails.AccountId,
                        OrganizationId = 36,//_userDetails.OrgId,
                        HubClientId = Context.ConnectionId
                    };
                    _accountSignalRClientsMappingList._accountClientMapperList.Add(accountSignalRClientMapper);
                    //ConnectedUser.Ids.Add(Context.ConnectionId);
                }
                await base.OnConnectedAsync();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.StackTrace);
            }
        }
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            try
            {
                //AccountSignalRClientMapper accountSignalRClientMapper = new AccountSignalRClientMapper()
                //{
                //    AccountId = 1,
                //    OrganizationId = 30,
                //    HubClientId = ""
                //};
                if (Context?.ConnectionId != null)
                {
                    _accountSignalRClientsMappingList._accountClientMapperList.RemoveAll(client => client.HubClientId == Context.ConnectionId);
                }
                //ConnectedUser.Ids.Remove(Context.ConnectionId);
                await base.OnDisconnectedAsync(ex);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.StackTrace);
            }
        }
        public async Task ReadKafkaMessages(string test)
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
                while (true)
                {
                    //Pushing message to kafka topic
                    Thread.Sleep(2000);
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
                            alertMesssageProp.AlertCategory = tripAlert.CategoryType;
                            alertMesssageProp.AlertType = tripAlert.Type;
                            alertMesssageProp.AlertUrgency = tripAlert.UrgencyLevelType ?? "";

                            AlertVehicleDetails objAlertVehicleDetails = await _pushNotofocationServiceClient.GetEligibleAccountForAlertAsync(alertMesssageProp);
                            NotificationAlertMessages notificationAlertMessages = new NotificationAlertMessages
                            {
                                TripAlertId = tripAlert.Id,
                                TripId = tripAlert.Tripid,
                                Vin = tripAlert.Vin,
                                AlertCategory = tripAlert.CategoryType,
                                AlertType = tripAlert.Type,
                                AlertId = tripAlert.Alertid,
                                UrgencyLevel = tripAlert.UrgencyLevelType,
                                AlertGeneratedTime = tripAlert.AlertGeneratedTime,
                                VehicleGroupId = objAlertVehicleDetails.VehicleGroupId,
                                VehicleGroupName = objAlertVehicleDetails.VehicleGroupName,
                                VehicleName = objAlertVehicleDetails.VehicleName,
                                VehicleLicencePlate = objAlertVehicleDetails.VehicleRegNo,
                                AlertCategoryKey = objAlertVehicleDetails.AlertCategoryKey,
                                AlertTypeKey = objAlertVehicleDetails.AlertTypeKey,
                                UrgencyTypeKey = objAlertVehicleDetails.UrgencyTypeKey,
                                CreatedBy = objAlertVehicleDetails.AlertCreatedAccountId,
                            };
                            // match session values with clientID & created by 
                            //IReadOnlyList<string> connectionIds = _accountSignalRClientsMappingList._accountClientMapperList.Distinct().Where(pre => pre.HubClientId == Context?.ConnectionId && pre.AccountId == notificationAlertMessages.CreatedBy && pre.AccountId == 187/*_userDetails.AccountId*/).Select(clients => clients.HubClientId).ToList();
                            IReadOnlyList<string> connectionIds = _accountSignalRClientsMappingList._accountClientMapperList.Distinct().Where(pre => pre.HubClientId == Context?.ConnectionId && pre.AccountId == notificationAlertMessages.CreatedBy && pre.AccountId == _userDetails.AccountId).Select(clients => clients.HubClientId).ToList();
                            await Clients.Clients(connectionIds).SendAsync("NotifyAlertResponse", JsonConvert.SerializeObject(notificationAlertMessages));

                        }
                    }
                }
            }
            catch (RpcException ex)
            {
                _logger.Error(null, ex);
                await Clients.Client(this.Context.ConnectionId).SendAsync("askServerResponse", ex.Message);
            }
            catch (Exception ex)
            {
                _ = ex.Message + test;
                _logger.Error(null, ex);
                await Clients.Client(this.Context.ConnectionId).SendAsync("askServerResponse", ex.Message);
            }
        }
        public async Task GetMessages(string someTextFromClient)
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(2000);

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
                        tripAlert.LandmarkName = this.Context.ConnectionId;
                        tripAlert.ModifiedAt = _userDetails.AccountId;
                        tripAlert.LandmarkThresholdValueUnitType = _userDetails.OrgId.ToString();

                        if (tripAlert != null && tripAlert.Alertid > 0)
                        {
                            AlertMesssageProp alertMesssageProp = new AlertMesssageProp();
                            alertMesssageProp.VIN = tripAlert.Vin;
                            alertMesssageProp.AlertId = tripAlert.Alertid;
                            AlertVehicleDetails objAlertVehicleDetails = await _pushNotofocationServiceClient.GetEligibleAccountForAlertAsync(alertMesssageProp);

                            NotificationAlertMessages notificationAlertMessages = new NotificationAlertMessages
                            {
                                TripAlertId = tripAlert.Id,
                                TripId = tripAlert.Tripid,
                                Vin = tripAlert.Vin,
                                AlertCategory = tripAlert.CategoryType,
                                AlertType = tripAlert.Type,
                                AlertId = tripAlert.Alertid,
                                AlertGeneratedTime = tripAlert.AlertGeneratedTime,
                                VehicleGroupId = objAlertVehicleDetails.VehicleGroupId,
                                VehicleGroupName = objAlertVehicleDetails.VehicleGroupName,
                                VehicleName = objAlertVehicleDetails.VehicleName,
                                VehicleLicencePlate = objAlertVehicleDetails.VehicleRegNo,
                                AlertCategoryKey = tripAlert.AlertCategoryKey,
                                AlertTypeKey = tripAlert.AlertTypeKey,
                                UrgencyTypeKey = tripAlert.UrgencyTypeKey,
                                UrgencyLevel = tripAlert.UrgencyLevelType
                            };
                            // match session values with clientID & created by 
                            IReadOnlyList<string> connectionIds = _accountSignalRClientsMappingList._accountClientMapperList.Distinct().Where(pre => pre.HubClientId == Context?.ConnectionId && pre.AccountId == tripAlert.CreatedAt && pre.AccountId == _userDetails.AccountId).Select(clients => clients.HubClientId).ToList();
                            await Clients.Clients(connectionIds).SendAsync("NotifyAlertResponse", JsonConvert.SerializeObject(tripAlert));
                        }
                    }
                    if (_pkId > 1000)
                    {
                        _pkId = 1;
                    }
                    _pkId++;

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
    }
}
