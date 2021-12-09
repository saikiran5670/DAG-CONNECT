﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
using net.atos.daf.ct2.portalservice.Hubs;
using net.atos.daf.ct2.pushnotificationservice;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


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
                consumerGroup = podConsumerGroupMap.Where(w => w.Key.ToLower() == hostName).FirstOrDefault().Value;
                if (string.IsNullOrEmpty(consumerGroup)) _logger.Error($"Error in GetCusumerGroupFmHostName - {hostName} not found in disctionory");
            }
            catch (Exception ex)
            {
                _logger.Error("Error in GetCusumerGroupFmHostName", ex);
            }
            return consumerGroup;
        }
        public override async Task OnConnectedAsync()
        {
            try
            {
                if (Context?.ConnectionId != null)
                {
                    //If exists record in hub then delete
                    if (!(_userDetails.AccountId > 0 && !_accountSignalRClientsMappingList._accountClientMapperList.Any(a => a.AccountId == _userDetails.AccountId && a.HubClientId == Context.ConnectionId)))
                    {
                        _accountSignalRClientsMappingList._accountClientMapperList.RemoveAll(client => client.HubClientId == Context.ConnectionId && client.AccountId == _userDetails.AccountId);
                    }
                    if (_userDetails.AccountId > 0 && !_accountSignalRClientsMappingList._accountClientMapperList.Any(a => a.AccountId == _userDetails.AccountId && a.HubClientId == Context.ConnectionId))
                    {
                        //Get Feature ids for alert feature
                        var featureIds = GetMappedFeatureIdByStartWithName(NotificationHubConstant.ALERT_FEATURE_STARTWITH);
                        var otaFeatureId = GetSessionFeatureIdByStartWithName(NotificationHubConstant.OTA_FEATURE_STARTWITH);
                        //_logger.Info("featureIds:" + JsonConvert.SerializeObject(featureIds));
                        //_logger.Info("_userDetails.UserFeatures:" + JsonConvert.SerializeObject(_userDetails));
                        AccountSignalRClientMapper accountSignalRClientMapper = new AccountSignalRClientMapper()
                        {
                            AccountId = _userDetails.AccountId,
                            OrganizationId = GetUserSelectedOrgId(),
                            HubClientId = Context.ConnectionId,
                            FeatureIds = featureIds,
                            OTAFeatureId = otaFeatureId,
                            ContextOrgId = GetContextOrgId()
                        };
                        _accountSignalRClientsMappingList._accountClientMapperList.Add(accountSignalRClientMapper);
                        //_logger.Info("accountClientMapper_List:" + JsonConvert.SerializeObject(accountSignalRClientMapper));

                    }
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
                if (Context?.ConnectionId != null)
                {
                    _accountSignalRClientsMappingList._accountClientMapperList.RemoveAll(client => client.HubClientId == Context.ConnectionId);
                }
                _logger.Info($"OnDisconnectedAsync client: {Context?.ConnectionId}");
                await base.OnDisconnectedAsync(ex);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.StackTrace);
                _logger.Error("Error in OnDisconnectedAsync method", err);
            }
        }
        //[AllowAnonymous]
        //public async Task NotifyAlert(string someTextFromClient)
        //{
        //    var list = someTextFromClient.Split(',');
        //    int accountId = Convert.ToInt32(list[0]);
        //    int orgId = Convert.ToInt32(list[1]);

        //    try
        //    {
        //        //Get Feature ids for alert feature
        //        var featureIds = GetMappedFeatureIdByStartWithName(NotificationHubConstant.ALERT_FEATURE_STARTWITH);
        //        if (!_accountSignalRClientsMappingList._accountClientMapperList.Any(a => a.AccountId == accountId && a.HubClientId == this.Context.ConnectionId))
        //        {
        //            AccountSignalRClientMapper accountSignalRClientMapper = new AccountSignalRClientMapper()
        //            {
        //                AccountId = accountId,
        //                OrganizationId = orgId,
        //                HubClientId = this.Context.ConnectionId,
        //                FeatureIds = featureIds
        //            };
        //            _accountSignalRClientsMappingList._accountClientMapperList.Add(accountSignalRClientMapper);
        //        }
        //        int pkId = 0;
        //        while (true)
        //        {
        //            NotificationAlertMessages notificationAlertMessages = new NotificationAlertMessages
        //            {
        //                TripAlertId = pkId,
        //                TripId = Dns.GetHostName(),
        //                Vin = "XLR0998HGFFT76657",
        //                AlertCategory = "L",
        //                AlertType = "G",
        //                AlertId = pkId * 2,
        //                AlertGeneratedTime = DateTime.Now.Millisecond,
        //                VehicleGroupId = 185,
        //                VehicleGroupName = "Fleet",
        //                VehicleName = "testKri",
        //                VehicleLicencePlate = "testKri",
        //                AlertCategoryKey = JsonConvert.SerializeObject(_kafkaConfiguration),
        //                AlertTypeKey = _userDetails.OrgId.ToString(),
        //                UrgencyTypeKey = Context.ConnectionId,
        //                UrgencyLevel = "C"
        //            };
        //            //IReadOnlyList<string> connectionIds = _accountSignalRClientsMappingList._accountClientMapperList.Where(clients => clients.AccountId == accountId).Select(clients => clients.HubClientId).ToList();
        //            await Clients.All.SendAsync("TestAlertResponse", JsonConvert.SerializeObject(JsonConvert.SerializeObject(notificationAlertMessages, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })));
        //            if (pkId > 1000)
        //            {
        //                pkId = 1;
        //            }
        //            pkId = pkId + 1;
        //            Thread.Sleep(60000);//1 minute 
        //        }
        //    }
        //    catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.Cancelled)
        //    {
        //        _logger.Error("Error in NotifyAlert.RpcException", ex);
        //        await Clients.Client(this.Context.ConnectionId).SendAsync("TestErrorResponse", ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        _ = ex.Message + someTextFromClient;
        //        _logger.Error("Error in NotifyAlert", ex);
        //        await Clients.Client(this.Context.ConnectionId).SendAsync("TestErrorResponse", ex.Message);
        //    }
        //}

        public async Task PushNotificationForAlert()
        {
            try
            {
                //if (accountId > 0 && !_accountSignalRClientsMappingList._accountClientMapperList.Any(a => a.AccountId == accountId && a.HubClientId == this.Context.ConnectionId))
                //{
                //    AccountSignalRClientMapper accountSignalRClientMapper = new AccountSignalRClientMapper()
                //    {
                //        AccountId = accountId,
                //        OrganizationId = orgId,
                //        HubClientId = this.Context.ConnectionId
                //    };
                //    _accountSignalRClientsMappingList._accountClientMapperList.Add(accountSignalRClientMapper);
                //}
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
                        //Thread.Sleep(2000);
                        ConsumeResult<string, string> response = KafkaConfluentWorker.Consumer(kafkaEntity);
                        TripAlert tripAlert = new TripAlert();
                        if (response != null)
                        {
                            //Console.WriteLine(response.Message.Value);
                            tripAlert = JsonConvert.DeserializeObject<TripAlert>(response.Message.Value);                            

                            if (tripAlert != null && (tripAlert.Alertid > 0 || (tripAlert.Alertid == 0 && tripAlert.CategoryType == "O")))
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
                                    OrganizationId = objAlertVehicleDetails.OrganizationId
                                };
                                if (tripAlert.Alertid == 0 && tripAlert.CategoryType == "O")
                                {
                                    //connectionIds = _accountSignalRClientsMappingList._accountClientMapperList.Distinct().Where(pre => objAlertVehicleDetails.OTAAccountIds.Contains(pre.AccountId)).Select(clients => clients.HubClientId).ToList();
                                    var visibleAccountByVin = _accountSignalRClientsMappingList._accountClientMapperList
                                                                .Where(pre => objAlertVehicleDetails.OTAAccountIds.Contains(pre.AccountId) && pre.OTAFeatureId > 0)
                                                                .Select(s => new AccountSignalRClientList { ContextOrgId = s.ContextOrgId, OrganizationId = s.OrganizationId, OTAFeatureId = s.OTAFeatureId })
                                                                .Distinct(new ObjectComparer())
                                                                .ToList();
                                    connectionIds = new List<string>();
                                    if (visibleAccountByVin != null && visibleAccountByVin.Count() > 0)
                                    {
                                        var accountSignalRClientMapperReq = new AccountSignalRClientMapperReq();
                                        accountSignalRClientMapperReq.Vin = tripAlert.Vin;
                                        accountSignalRClientMapperReq.AccountSignalRClientMapper.AddRange(visibleAccountByVin);
                                        var visiblityResponse = await _pushNotofocationServiceClient.GetVehicleAccountVisibilityByVINAsync(accountSignalRClientMapperReq);
                                        if (visiblityResponse.Code == ResponseCode.Success)
                                        {
                                            var visiblityList = visiblityResponse.AccountSignalRClientMapper.ToList();
                                            var connectionIdList = new List<string>();
                                            connectionIds = new List<string>();
                                            if (visiblityList != null && visiblityList.Count() > 0)
                                            {
                                                foreach (var item in _accountSignalRClientsMappingList._accountClientMapperList.Distinct().Where(pre => objAlertVehicleDetails.OTAAccountIds.Contains(pre.AccountId) && pre.OTAFeatureId > 0).ToList())
                                                {
                                                    if (!connectionIds.Any(s => s == item.HubClientId) && visiblityList.Any(s => s.OrganizationId == item.OrganizationId && s.ContextOrgId == item.ContextOrgId))
                                                    {
                                                        connectionIdList.Add(item.HubClientId);
                                                    }
                                                }
                                                connectionIds = connectionIdList;
                                            }
                                        }
                                        else
                                        {
                                            _logger.Debug($"PushNotificationForAlert: Valdation Failed - package by VIN vissibity. {_accountSignalRClientsMappingList._accountClientMapperList.Distinct().Where(pre => objAlertVehicleDetails.OTAAccountIds.Contains(pre.AccountId) && pre.OTAFeatureId > 0).ToList()}");
                                        }
                                    }
                                    else
                                    {
                                        _logger.Debug($"PushNotificationForAlert: accountClientMapper_List: {JsonConvert.SerializeObject(_accountSignalRClientsMappingList._accountClientMapperList.Distinct().ToList())}");
                                    }

                                }
                                else
                                {
                                    Grpc.Core.Metadata headers = new Grpc.Core.Metadata();
                                    var logged_in_orgId = _accountSignalRClientsMappingList._accountClientMapperList.Distinct().Where(pre => pre.HubClientId == this.Context.ConnectionId).Select(clients => clients.ContextOrgId).FirstOrDefault();
                                    var logged_FeatureId = _accountSignalRClientsMappingList._accountClientMapperList.Distinct().Where(pre => pre.HubClientId == this.Context.ConnectionId).Select(clients => clients.FeatureIds).FirstOrDefault();
                                    headers.Add("logged_in_orgId", Convert.ToString(logged_in_orgId));
                                    //_logger.Info($"\n\rPushNotificationVin - {GetUserSelectedOrgId()} - {JsonConvert.SerializeObject(logged_FeatureId)} -{GetContextOrgId()}");
                                    VisibilityVehicleRequest visibilityVehicleRequest = new VisibilityVehicleRequest();
                                    visibilityVehicleRequest.AccountId = objAlertVehicleDetails.AlertCreatedAccountId;
                                    visibilityVehicleRequest.OrganizationId = objAlertVehicleDetails.OrganizationId;
                                    visibilityVehicleRequest.FeatureIds.Add(logged_FeatureId.Select(x => x));
                                    AssociatedVehicleResponse associatedVehicleResponse = await _pushNotofocationServiceClient.GetVehicleByAccountVisibilityAsync(visibilityVehicleRequest, headers);
                                    var associatedVin = associatedVehicleResponse.AssociatedVehicle.Select(x => x.Vin).Contains(notificationAlertMessages.Vin.ToString());
                                    var featureEnum = associatedVehicleResponse.FeatureEnum.Select(x => x.FeatureEnumType).Contains(notificationAlertMessages.AlertType.ToString());
                                    if (associatedVehicleResponse.AssociatedVehicle.Any() && associatedVin && featureEnum)
                                    {
                                        connectionIds = _accountSignalRClientsMappingList._accountClientMapperList.Distinct().Where(pre => pre.AccountId == notificationAlertMessages.CreatedBy).Select(clients => clients.HubClientId).ToList();
                                    }
                                }
                                _logger.Debug($"PushNotificationForAlert - {_kafkaConfiguration.CONSUMER_GROUP} - {this.Context.ConnectionId} : {string.Join(",", connectionIds)} : {Dns.GetHostName()} : {JsonConvert.SerializeObject(notificationAlertMessages, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })}");
                                if (connectionIds.Count() > 0)
                                    await Clients.Clients(connectionIds).SendAsync("PushNotificationForAlertResponse", JsonConvert.SerializeObject(notificationAlertMessages, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
                                else
                                    _logger.Debug($"PushNotificationForAlert: No Connection ids found for the account visibility.");
                            }
                        }
                    }
                    catch (RpcException ex)
                    {
                        _logger.Error($"RpcException Error in PushNotificationForAlert - AlertID: {alertId}, Host: : {Dns.GetHostName()}, KafkaConfig: {JsonConvert.SerializeObject(kafkaEntity)}", ex);
                        await Clients.Clients(connectionIds).SendAsync("PushNotificationForAlertError", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Error in PushNotificationForAlert  - AlertID: {alertId}, Host: : {Dns.GetHostName()}, KafkaConfig: {JsonConvert.SerializeObject(kafkaEntity)}", ex);
                        await Clients.Clients(connectionIds).SendAsync("PushNotificationForAlertError", ex.Message);
                    }
                }
            }
            catch (RpcException ex)
            {
                _logger.Error("RpcException in PushNotificationForAlert method", ex);
                await Clients.Client(this.Context.ConnectionId).SendAsync("PushNotificationForAlertError", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Error in PushNotificationForAlert method", ex);
                await Clients.Client(this.Context.ConnectionId).SendAsync("PushNotificationForAlertError", ex.Message);
            }
        }

        #region Session Method
        protected SessionFeature[] GetUserSubscribeFeatures()
        {
            return _userDetails.UserFeatures;
        }

        protected IEnumerable<int> GetMappedFeatureIdByStartWithName(string featureStartWith)
        {
            return GetUserSubscribeFeatures()?.Where(x => x.Name.ToLower().StartsWith(featureStartWith.ToLower()))
                                            ?.Select(x => x.FeatureId)?.ToList();
        }
        protected int GetUserSelectedOrgId()
        {
            return _userDetails.OrgId;
        }

        protected int GetContextOrgId()
        {
            return _userDetails.ContextOrgId;
        }

        protected int GetSessionFeatureIdByStartWithName(string featureName)
        {
            return GetUserSubscribeFeatures()?.Where(x => x.Name.ToLower().Equals(featureName.ToLower()))
                                            ?.Select(x => x.FeatureId)?.FirstOrDefault() ?? default;
        }
        #endregion
    }
}
