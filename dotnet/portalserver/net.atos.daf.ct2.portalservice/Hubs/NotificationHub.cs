using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using net.atos.daf.ct2.portalservice.Entity.Hub;
using net.atos.daf.ct2.pushnotificationservice;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace net.atos.daf.ct2.portalservice.hubs
{
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
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
        private readonly AuditHelper _auditHelper;
        private readonly PodConsumerGroupMapSettings _podComsumerGroupMapSettings = new PodConsumerGroupMapSettings();
        public NotificationHub(PushNotificationService.PushNotificationServiceClient pushNotofocationServiceClient, IConfiguration configuration, AccountSignalRClientsMappingList accountSignalRClientsMappingList, IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper, AuditHelper auditHelper)
        {
            _auditHelper = auditHelper;
            _pushNotofocationServiceClient = pushNotofocationServiceClient;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            this._configuration = configuration;
            _kafkaConfiguration = new Entity.KafkaConfiguration();
            //configuration.Bind(_podComsumerGroupMapSettings);
            _podComsumerGroupMapSettings.PodConsumerGroupMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(configuration["PodConsumerGroupMap"]);
            configuration.GetSection("PushAlertKafkaConfiguration").Bind(_kafkaConfiguration);
            _kafkaConfiguration.CONSUMER_GROUP = GetConsumerGroupFmHostName(_podComsumerGroupMapSettings.PodConsumerGroupMap, Dns.GetHostName().ToLower());
            _mapper = new Entity.Alert.Mapper();
            _accountSignalRClientsMappingList = accountSignalRClientsMappingList;
            _httpContextAccessor = httpContextAccessor;
            _sessionHelper = sessionHelper;
            _userDetails = _sessionHelper.GetSessionInfo(httpContextAccessor.HttpContext.Session);
        }

        private string GetConsumerGroupFmHostName(Dictionary<string, string> podConsumerGroupMap, string hostName)
        {
            string consumerGroup = string.Empty;
            try
            {
                _ = _auditHelper.AddLogs(DateTime.Now, AlertConstants.NOTIFICATION_HUB_MSG,
                AlertConstants.NOTIFICATION_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                string.Format(AlertConstants.ALERT_EXCEPTION_LOG_MSG, "ReadKafkaMessages", "ex.message"), 0, 0, JsonConvert.SerializeObject(podConsumerGroupMap),
                 new HeaderObj { AccountId = 1, OrgId = 2, AccountEmailId = "test", RoleId = 3, ContextOrgId = 4 }).Result;
                consumerGroup = podConsumerGroupMap.Where(w => w.Key.ToLower() == hostName).FirstOrDefault().Value;
                if (string.IsNullOrEmpty(consumerGroup)) _logger.Error($"Error in GetCusumerGroupFmHostName - {hostName} not found in disctionory");
            }
            catch (Exception ex)
            {
                _logger.Error("Error in GetCusumerGroupFmHostName", ex);
            }
            return consumerGroup;
        }

        public async Task NotifyAlert(string someTextFromClient)
        {
            var list = someTextFromClient.Split(',');
            int accountId = Convert.ToInt32(list[0]);
            int orgId = Convert.ToInt32(list[1]);

            try
            {
                if (!_accountSignalRClientsMappingList._accountClientMapperList.Any(a => a.AccountId == accountId && a.HubClientId == this.Context.ConnectionId))
                {
                    AccountSignalRClientMapper accountSignalRClientMapper = new AccountSignalRClientMapper()
                    {
                        AccountId = accountId,
                        OrganizationId = orgId,
                        HubClientId = this.Context.ConnectionId
                    };
                    _accountSignalRClientsMappingList._accountClientMapperList.Add(accountSignalRClientMapper);
                }
                while (true)
                {
                    NotificationAlertMessages notificationAlertMessages = new NotificationAlertMessages
                    {
                        TripAlertId = _pkId,
                        TripId = Dns.GetHostName(),
                        Vin = "XLR0998HGFFT76657",
                        AlertCategory = "L",
                        AlertType = "G",
                        AlertId = _pkId * 2,
                        AlertGeneratedTime = DateTime.Now.Millisecond,
                        VehicleGroupId = 185,
                        VehicleGroupName = "Fleet",
                        VehicleName = "testKri",
                        VehicleLicencePlate = "testKri",
                        AlertCategoryKey = _userDetails.AccountId.ToString() + "---" + _kafkaConfiguration.CONSUMER_GROUP,
                        AlertTypeKey = _userDetails.OrgId.ToString(),
                        UrgencyTypeKey = Context.ConnectionId,
                        UrgencyLevel = "C"
                    };
                    //       IReadOnlyList<string> connectionIds = _accountSignalRClientsMappingList._accountClientMapperList.Distinct().Where(pre => pre.HubClientId == Context?.ConnectionId).Select(clients => clients.HubClientId).ToList();
                    //await Clients.All.SendAsync("NotifyAlertResponse", JsonConvert.SerializeObject(notificationAlertMessages));
                    IReadOnlyList<string> connectionIds = _accountSignalRClientsMappingList._accountClientMapperList.Where(clients => clients.AccountId == accountId).Select(clients => clients.HubClientId).ToList();
                    //_logger.Info($"\n\rNotifyAlertKafka2019 - {_kafkaConfiguration.CONSUMER_GROUP} - {this.Context.ConnectionId} : {string.Join(",", connectionIds)} : {Dns.GetHostName()} : {JsonConvert.SerializeObject(notificationAlertMessages, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })}");
                    await Clients.Clients(connectionIds).SendAsync("TestAlertResponse", JsonConvert.SerializeObject(JsonConvert.SerializeObject(notificationAlertMessages, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })));
                    if (_pkId > 1000)
                    {
                        _pkId = 1;
                    }
                    _pkId = _pkId + 1;
                    Thread.Sleep(60000);//1 minute 
                }
            }
            catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.Cancelled)
            {
                _logger.Error("Error in NotifyAlert.RpcException", ex);
                await Clients.Client(this.Context.ConnectionId).SendAsync("TestErrorResponse", ex.Message);
            }
            catch (Exception ex)
            {
                _ = ex.Message + someTextFromClient;
                _logger.Error("Error in NotifyAlert", ex);
                await Clients.Client(this.Context.ConnectionId).SendAsync("TestErrorResponse", ex.Message);
            }
        }
        public override async Task OnConnectedAsync()
        {
            try
            {
                if (Context?.ConnectionId != null)
                {
                    if (_userDetails.AccountId > 0 && !_accountSignalRClientsMappingList._accountClientMapperList.Any(a => a.AccountId == _userDetails.AccountId && a.HubClientId == Context.ConnectionId))
                    {
                        AccountSignalRClientMapper accountSignalRClientMapper = new AccountSignalRClientMapper()
                        {
                            AccountId = _userDetails.AccountId,
                            OrganizationId = _userDetails.OrgId,
                            HubClientId = Context.ConnectionId
                        };
                        _accountSignalRClientsMappingList._accountClientMapperList.Add(accountSignalRClientMapper);
                    }
                    //ConnectedUser.Ids.Add(Context.ConnectionId);
                }
                await base.OnConnectedAsync();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.StackTrace);
                _logger.Error("Error in OnConnectedAsync method", err);
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
                _logger.Error("Error in OnDisconnectedAsync method", err);
            }
        }
        public async Task ReadKafkaMessages(int accountId, int orgId)
        {
            try
            {
                if (accountId > 0 && !_accountSignalRClientsMappingList._accountClientMapperList.Any(a => a.AccountId == accountId && a.HubClientId == this.Context.ConnectionId))
                {
                    AccountSignalRClientMapper accountSignalRClientMapper = new AccountSignalRClientMapper()
                    {
                        AccountId = accountId,
                        OrganizationId = orgId,
                        HubClientId = this.Context.ConnectionId
                    };
                    _logger.Info("accountClientMapper_List:" + JsonConvert.SerializeObject(accountSignalRClientMapper));
                    //await File.AppendAllTextAsync("_accountClientMapperList.txt", JsonConvert.SerializeObject(accountSignalRClientMapper));
                    _accountSignalRClientsMappingList._accountClientMapperList.Add(accountSignalRClientMapper);
                }
                confluentkafka.entity.KafkaConfiguration kafkaEntity = new confluentkafka.entity.KafkaConfiguration()
                {
                    BrokerList = _kafkaConfiguration.EH_FQDN,
                    ConnString = _kafkaConfiguration.EH_CONNECTION_STRING,
                    Topic = _kafkaConfiguration.EH_NAME,
                    Cacertlocation = _kafkaConfiguration.CA_CERT_LOCATION,
                    Consumergroup = _kafkaConfiguration.CONSUMER_GROUP
                };
                int alertId = 0;
                while (true)
                {
                    IReadOnlyList<string> connectionIds = new List<string> { this.Context.ConnectionId };
                    try
                    {
                        //Pushing message to kafka topic
                        Thread.Sleep(2000);
                        ConsumeResult<string, string> response = KafkaConfluentWorker.Consumer(kafkaEntity);
                        TripAlert tripAlert = new TripAlert();
                        if (response != null)
                        {
                            //Console.WriteLine(response.Message.Value);
                            tripAlert = JsonConvert.DeserializeObject<TripAlert>(response.Message.Value);
                            if (tripAlert != null && tripAlert.Alertid > 0)
                            {
                                alertId = tripAlert.Alertid;
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
                                await _auditHelper.AddLogs(DateTime.Now, AlertConstants.NOTIFICATION_HUB_MSG,
                           AlertConstants.NOTIFICATION_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.GET, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                           string.Format(AlertConstants.ALERT_AUDIT_LOG_MSG, "ReadKafkaMessages", AlertConstants.NOTIFICATION_HUB_MSG), notificationAlertMessages.AlertId, notificationAlertMessages.AlertId, JsonConvert.SerializeObject(tripAlert),
                           _userDetails);
                                // match session values with clientID & created by 
                                //IReadOnlyList<string> connectionIds = _accountSignalRClientsMappingList._accountClientMapperList.Distinct().Where(pre => pre.HubClientId == Context?.ConnectionId && pre.AccountId == notificationAlertMessages.CreatedBy && pre.AccountId == 187/*_userDetails.AccountId*/).Select(clients => clients.HubClientId).ToList();
                                connectionIds = _accountSignalRClientsMappingList._accountClientMapperList.Distinct().Where(pre => pre.AccountId == notificationAlertMessages.CreatedBy).Select(clients => clients.HubClientId).ToList();
                                _logger.Info($"\n\rReadKafka2019 - {_kafkaConfiguration.CONSUMER_GROUP} - {this.Context.ConnectionId} : {string.Join(",", connectionIds)} : {Dns.GetHostName()} : {JsonConvert.SerializeObject(notificationAlertMessages, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })}");
                                //await File.AppendAllTextAsync("NotifyAlertResponse.txt", $"\n\rReadKafka2019 - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}--- {this.Context.ConnectionId} - {_kafkaConfiguration.CONSUMER_GROUP} : {string.Join(",", connectionIds)} : {Dns.GetHostName()} : {JsonConvert.SerializeObject(notificationAlertMessages, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })}");
                                await Clients.Clients(connectionIds).SendAsync("NotifyAlertResponse", JsonConvert.SerializeObject(notificationAlertMessages, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
                            }
                        }
                    }
                    catch (RpcException ex)
                    {
                        _logger.Error($"Error in ReadKafkaMessages - AlertID: {alertId}", ex);
                        await Clients.Clients(connectionIds).SendAsync("askServerResponse", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        await _auditHelper.AddLogs(DateTime.Now, AlertConstants.NOTIFICATION_HUB_MSG,
                         AlertConstants.NOTIFICATION_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                         string.Format(AlertConstants.ALERT_EXCEPTION_LOG_MSG, "ReadKafkaMessages", ex.Message), 0, 0, "",
                          _userDetails);
                        _ = ex.Message;
                        _logger.Error($"Error in ReadKafkaMessages  - AlertID: {alertId}", ex);
                        await Clients.Clients(connectionIds).SendAsync("askServerResponse", ex.Message);
                    }
                }
            }
            catch (RpcException ex)
            {
                _logger.Error("Error in ReadKafkaMessages method", ex);
                await Clients.Client(this.Context.ConnectionId).SendAsync("askServerResponse", ex.Message);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, AlertConstants.NOTIFICATION_HUB_MSG,
                 AlertConstants.NOTIFICATION_SERVICE_NAME, Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                 string.Format(AlertConstants.ALERT_EXCEPTION_LOG_MSG, "ReadKafkaMessages", ex.Message), 0, 0, "",
                  _userDetails);
                _ = ex.Message;
                _logger.Error("Error in ReadKafkaMessages method", ex);
                await Clients.Client(this.Context.ConnectionId).SendAsync("askServerResponse", ex.Message);
            }
        }
    }
}
