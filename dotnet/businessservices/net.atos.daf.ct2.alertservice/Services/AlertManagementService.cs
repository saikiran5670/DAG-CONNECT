using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.alert;
using net.atos.daf.ct2.alert.entity;
using net.atos.daf.ct2.alert.ENUM;
using net.atos.daf.ct2.alertservice.Entity;
using net.atos.daf.ct2.visibility;
using Newtonsoft.Json;
using net.atos.daf.ct2.kafkacdc;
using net.atos.daf.ct2.kafkacdc.entity;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.alertservice.common;

namespace net.atos.daf.ct2.alertservice.Services
{
    public class AlertManagementService : AlertService.AlertServiceBase
    {
        private readonly ILog _logger;
        private readonly IAlertManager _alertManager;
        private readonly Mapper _mapper;
        private readonly IVisibilityManager _visibilityManager;
        private readonly IAlertMgmAlertCdcManager _alertMgmAlertCdcManager;
        private readonly AlertCdcHelper _alertCdcHelper;

        public AlertManagementService(IAlertManager alertManager, IVisibilityManager visibilityManager, IAlertMgmAlertCdcManager alertMgmAlertCdcManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _alertManager = alertManager;
            _mapper = new Mapper();
            _visibilityManager = visibilityManager;
            _alertMgmAlertCdcManager = alertMgmAlertCdcManager;
            _alertCdcHelper = new AlertCdcHelper(_alertMgmAlertCdcManager);
        }

        #region ActivateAlert,SuspendAlert and  DeleteAlert
        public override async Task<AlertResponse> ActivateAlert(IdRequest request, ServerCallContext context)
        {
            try
            {
                var id = await _alertManager.ActivateAlert(request.AlertId, ((char)AlertState.Active), ((char)AlertState.Suspend));
                if (id > 0)
                {
                    /*For alert CDC - visibility */
                    List<int> featureIds = JsonConvert.DeserializeObject<List<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? "0");
                    var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_orgid")).FirstOrDefault()?.Value ?? "0");
                    var contextOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("context_orgid")).FirstOrDefault()?.Value ?? "0");
                    var accountId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("account_id")).FirstOrDefault()?.Value ?? "0");
                    /*For alert CDC - visibility */
                    //Triggering alert cdc 
                    await _alertCdcHelper.TriggerAlertCdc(request.AlertId, "A", accountId, loggedInOrgId, contextOrgId, featureIds);
                }
                return await Task.FromResult(new AlertResponse
                {
                    Message = id > 0 ? String.Format(AlertConstants.ACTIVATED_ALERT_SUCCESS_MSG, id) : String.Format(AlertConstants.ACTIVATED_ALERT_FAILURE_MSG, request.AlertId, AlertConstants.ALERT_FAILURE_MSG),
                    Code = id > 0 ? ResponseCode.Success : ResponseCode.Failed
                });

            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(ActivateAlert)}: With Error:-", ex);
                return await Task.FromResult(new AlertResponse
                {
                    Message = $"{nameof(ActivateAlert)}: {AlertConstants.INTERNAL_SERVER_MSG}.",
                    Code = ResponseCode.InternalServerError
                });
            }
        }

        public override async Task<AlertResponse> SuspendAlert(IdRequest request, ServerCallContext context)
        {
            try
            {
                var id = await _alertManager.SuspendAlert(request.AlertId, ((char)AlertState.Suspend), ((char)AlertState.Active));
                if (id > 0)
                {
                    /*For alert CDC - visibility */
                    List<int> featureIds = JsonConvert.DeserializeObject<List<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? "0");
                    var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_orgid")).FirstOrDefault()?.Value ?? "0");
                    var contextOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("context_orgid")).FirstOrDefault()?.Value ?? "0");
                    var accountId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("account_id")).FirstOrDefault()?.Value ?? "0");
                    /*For alert CDC - visibility */
                    //Triggering alert cdc 
                    await _alertCdcHelper.TriggerAlertCdc(request.AlertId, "I", accountId, loggedInOrgId, contextOrgId, featureIds);
                }
                return await Task.FromResult(new AlertResponse
                {
                    Message = id > 0 ? String.Format(AlertConstants.SUSPEND_ALERT_SUCCESS_MSG, id) : String.Format(AlertConstants.SUSPEND_ALERT_FAILURE_MSG, request.AlertId, AlertConstants.ALERT_FAILURE_MSG),
                    Code = id > 0 ? ResponseCode.Success : ResponseCode.Failed
                });

            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(SuspendAlert)}: With Error:-", ex);
                return await Task.FromResult(new AlertResponse
                {
                    Message = $"{nameof(SuspendAlert)}: {AlertConstants.INTERNAL_SERVER_MSG}.",
                    Code = ResponseCode.InternalServerError
                });
            }
        }

        public override async Task<AlertResponse> DeleteAlert(IdRequest request, ServerCallContext context)
        {
            try
            {

                if (await _alertManager.CheckIsNotificationExitForAlert(request.AlertId))
                    return await Task.FromResult(new AlertResponse
                    {
                        Message = String.Format(AlertConstants.DELETE_ALERT_NO_NOTIFICATION_MSG, request.AlertId),
                        Code = ResponseCode.Conflict
                    });


                var id = await _alertManager.DeleteAlert(request.AlertId, ((char)AlertState.Delete));
                if (id > 0)
                {
                    /*For alert CDC - visibility */
                    List<int> featureIds = JsonConvert.DeserializeObject<List<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? "0");
                    var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_orgid")).FirstOrDefault()?.Value ?? "0");
                    var contextOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("context_orgid")).FirstOrDefault()?.Value ?? "0");
                    var accountId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("account_id")).FirstOrDefault()?.Value ?? "0");
                    /*For alert CDC - visibility */
                    //Triggering alert cdc 
                    await _alertCdcHelper.TriggerAlertCdc(request.AlertId, "D", accountId, loggedInOrgId, contextOrgId, featureIds);
                }
                return await Task.FromResult(new AlertResponse
                {
                    Message = id > 0 ? String.Format(AlertConstants.DELETE_ALERT_SUCCESS_MSG, id) : String.Format(AlertConstants.DELETE_ALERT_FAILURE_MSG, request.AlertId, AlertConstants.ALERT_FAILURE_MSG),
                    Code = id > 0 ? ResponseCode.Success : ResponseCode.Failed
                });

            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(DeleteAlert)}: With Error:-", ex);
                return await Task.FromResult(new AlertResponse
                {
                    Message = String.Format(AlertConstants.DELETE_ALERT_FAILURE_MSG, request.AlertId, $"{nameof(DeleteAlert)}: {AlertConstants.INTERNAL_SERVER_MSG}."),
                    Code = ResponseCode.Failed
                });
            }
        }

        #endregion

        #region Alert Category
        public override async Task<AlertCategoryResponse> GetAlertCategory(AccountIdRequest request, ServerCallContext context)
        {
            try
            {
                IEnumerable<net.atos.daf.ct2.alert.entity.EnumTranslation> enumTranslationList = await _alertManager.GetAlertCategory();

                AlertCategoryResponse response = new AlertCategoryResponse();
                foreach (var item in enumTranslationList)
                {
                    response.EnumTranslation.Add(_mapper.MapEnumTranslation(item));
                }

                response.Message = "Alert Category data retrieved";
                response.Code = ResponseCode.Success;
                //_logger.Info("Get method in alert service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(GetAlertCategory)}: With Error:-", ex);
                return await Task.FromResult(new AlertCategoryResponse
                {
                    Code = ResponseCode.Failed,
                    Message = $"{nameof(GetAlertCategory)}: {AlertConstants.INTERNAL_SERVER_MSG}."
                });
            }
        }
        #endregion


        #region Update Alert

        public override async Task<AlertResponse> UpdateAlert(AlertRequest request, ServerCallContext context)
        {
            try
            {
                AlertResponse response = new AlertResponse();
                response.AlertRequest = new AlertRequest();
                Alert alert = new Alert();
                alert = _mapper.ToAlertEntity(request);
                alert = await _alertManager.UpdateAlert(alert);
                // check for alert name exists
                response.AlertRequest.Exists = false;
                if (alert.Exists)
                {
                    response.AlertRequest.Exists = true;
                    response.Message = "Duplicate alert name";
                    response.Code = ResponseCode.Conflict;
                    return response;
                }
                // check for notification recipient label exists
                var duplicateNotificationRecipients = alert.Notifications.SelectMany(a => a.NotificationRecipients).Where(y => y.Exists == true).ToList();
                if (duplicateNotificationRecipients.Count() > 0)
                {
                    response.AlertRequest.Exists = true;
                    response.Message = "Duplicate notification recipient label";
                    response.Code = ResponseCode.Conflict;
                    return response;
                }
                if (alert.Id > 0)
                {
                    /*For alert CDC - visibility */
                    List<int> featureIds = JsonConvert.DeserializeObject<List<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? "0");
                    var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_orgid")).FirstOrDefault()?.Value ?? "0");
                    var contextOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("context_orgid")).FirstOrDefault()?.Value ?? "0");
                    var accountId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("account_id")).FirstOrDefault()?.Value ?? "0");
                    /*For alert CDC - visibility */
                    //Triggering alert cdc 
                    await _alertCdcHelper.TriggerAlertCdc(alert.Id, "U", accountId, loggedInOrgId, contextOrgId, featureIds);
                }
                return await Task.FromResult(new AlertResponse
                {
                    Message = alert.Id > 0 ? $"Alert is updated successful for id:- {alert.Id}." : $"Activate Alert Failed for id:- {request.Id}.",
                    Code = alert.Id > 0 ? ResponseCode.Success : ResponseCode.Failed,
                });

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AlertResponse
                {
                    Message = "Exception :-" + ex.Message,
                    Code = ResponseCode.Failed,
                    AlertRequest = null
                });
            }
        }

        #endregion

        #region Create Alert
        public override async Task<AlertResponse> CreateAlert(AlertRequest request, ServerCallContext context)
        {
            try
            {
                AlertResponse response = new AlertResponse();
                response.AlertRequest = new AlertRequest();
                Alert alert = new Alert();
                alert = _mapper.ToAlertEntity(request);
                alert = await _alertManager.CreateAlert(alert);
                response.AlertRequest.Exists = false;
                // check for alert name exists
                if (alert.Exists)
                {
                    response.AlertRequest.Exists = true;
                    response.Message = "Duplicate alert name";
                    response.Code = ResponseCode.Conflict;
                    return response;
                }
                // check for notification recipient label exists
                var duplicateNotificationRecipients = alert.Notifications.SelectMany(a => a.NotificationRecipients).Where(y => y.Exists == true).ToList();
                if (duplicateNotificationRecipients.Count() > 0)
                {
                    response.AlertRequest.Exists = true;
                    response.Message = "Duplicate notification recipient label";
                    response.Code = ResponseCode.Conflict;
                    return response;
                }
                if (alert.Id > 0)
                {
                    /*For alert CDC - visibility */
                    List<int> featureIds = JsonConvert.DeserializeObject<List<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? "0");
                    var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_orgid")).FirstOrDefault()?.Value ?? "0");
                    var contextOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("context_orgid")).FirstOrDefault()?.Value ?? "0");
                    var accountId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("account_id")).FirstOrDefault()?.Value ?? "0");
                    /*For alert CDC - visibility */
                    //Triggering alert cdc 
                    await _alertCdcHelper.TriggerAlertCdc(alert.Id, "A", accountId, loggedInOrgId, contextOrgId, featureIds);
                }
                return await Task.FromResult(new AlertResponse
                {
                    Message = alert.Id > 0 ? $"Alert is created successful for id:- {alert.Id}." : $"Alert creation is failed for {alert.Name}",
                    Code = alert.Id > 0 ? ResponseCode.Success : ResponseCode.Failed
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AlertResponse
                {
                    Message = "Exception :-" + ex.Message,
                    Code = ResponseCode.Failed,
                    AlertRequest = null
                });
            }
        }
        #endregion

        #region Get Alert List
        public override async Task<AlertListResponse> GetAlertList(AlertListRequest request, ServerCallContext context)
        {
            try
            {
                //Alert objalert = new Alert();
                //objalert.OrganizationId = request.OrganizationId;
                //objalert.CreatedBy = request.AccountId;
                List<int> featureIds = JsonConvert.DeserializeObject<List<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? "0");
                //Feature Id is passed as 0 because feature wise filtering is applied seperately below.
                // var vehicleDetailsAccountVisibilty
                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_orgid")).FirstOrDefault()?.Value ?? "0");
                List<visibility.entity.VehicleDetailsAccountVisibilityForAlert> vehicleDetailsAccountVisibilty = new List<visibility.entity.VehicleDetailsAccountVisibilityForAlert>();
                List<int> vehicleIds = new List<int>();
                if (featureIds != null && featureIds.Count() > 0)
                {
                    //foreach (int featureId in featureIds)
                    //{
                    //    IEnumerable<visibility.entity.VehicleDetailsAccountVisibility> vehicleAccountVisibiltyList
                    //     = await _visibilityManager.GetVehicleByAccountVisibilityTemp(request.AccountId, loggedInOrgId, request.OrganizationId, featureId);
                    //    //append visibile vins
                    //    vehicleDetailsAccountVisibilty.AddRange(vehicleAccountVisibiltyList);
                    //    //remove duplicate vins by key as vin
                    //    vehicleDetailsAccountVisibilty = vehicleDetailsAccountVisibilty.GroupBy(c => c.Vin, (key, c) => c.FirstOrDefault()).ToList();
                    //}
                    //foreach (var item in vehicleDetailsAccountVisibilty)
                    //{
                    //    vehicleIds.Add(item.VehicleId);
                    //}
                    IEnumerable<visibility.entity.VehicleDetailsAccountVisibilityForAlert> vehicleAccountVisibiltyList
                    = await _visibilityManager.GetVehicleByAccountVisibilityForAlert(request.AccountId, loggedInOrgId, request.OrganizationId, featureIds.ToArray());
                    //append visibile vins
                    vehicleDetailsAccountVisibilty.AddRange(vehicleAccountVisibiltyList);
                    //remove duplicate vins by key as vin
                    vehicleDetailsAccountVisibilty = vehicleDetailsAccountVisibilty.GroupBy(c => c.Vin, (key, c) => c.FirstOrDefault()).ToList();
                    vehicleIds = vehicleDetailsAccountVisibilty.Select(x => x.VehicleId).Distinct().ToList();
                }
                IEnumerable<Alert> alertList = await _alertManager.GetAlertList(request.AccountId, request.OrganizationId, featureIds, vehicleIds);

                AlertListResponse response = new AlertListResponse();
                foreach (var item in alertList)
                {
                    response.AlertRequest.Add(_mapper.MapAlertEntity(item));
                }

                response.Message = "Alert data retrieved";
                response.Code = ResponseCode.Success;
                //_logger.Info("Get method in alert service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AlertListResponse
                {
                    Code = ResponseCode.Failed,
                    Message = "Get alert list fail : " + ex.Message
                });
            }
        }
        #endregion

        #region DuplicateAlertType
        public override async Task<DuplicateAlertResponse> DuplicateAlertType(IdRequest request, ServerCallContext context)
        {
            var alertResponse = new DuplicateAlertResponse();
            try
            {
                var duplicateAlert = await _alertManager.DuplicateAlertType(request.AlertId);
                alertResponse.DuplicateAlert = duplicateAlert != null ? _mapper.ToDupliacteAlert(duplicateAlert) : null;
                alertResponse.Code = duplicateAlert != null ? ResponseCode.Success : ResponseCode.Failed;
                alertResponse.Message = duplicateAlert != null ? String.Format(AlertConstants.DUPLICATE_ALERT_SUCCESS_MSG, request.AlertId) : String.Format(AlertConstants.DUPLICATE_ALERT_FAILURE_MSG, request.AlertId, AlertConstants.ALERT_FAILURE_MSG);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                alertResponse.Code = ResponseCode.InternalServerError;
                alertResponse.Message = String.Format(AlertConstants.DUPLICATE_ALERT_FAILURE_MSG, request.AlertId, ex.Message);
            }
            return alertResponse;
        }
        #endregion

        #region Landmark Delete Validation

        public override async Task<LandmarkIdExistResponse> IsLandmarkActiveInAlert(LandmarkIdRequest request, ServerCallContext context)
        {
            var landmarkResponse = new LandmarkIdExistResponse();
            try
            {
                List<int> landmarkIds = new List<int>();
                foreach (int item in request.LandmarkId)
                {
                    landmarkIds.Add(item);
                }
                var IsLandmarkIdActive = await _alertManager.IsLandmarkActiveInAlert(landmarkIds, request.LandmarkType);
                landmarkResponse.IsLandmarkActive = IsLandmarkIdActive != false ? true : false;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                landmarkResponse.Code = ResponseCode.InternalServerError;
                landmarkResponse.Message = String.Format("IsLandmarkActiveInAlert Method in alert service", request.LandmarkId, ex.Message);
            }
            return landmarkResponse;
        }

        #endregion

        #region Alert Notification Template
        public override async Task<NotificationTemplateResponse> GetNotificationTemplate(AccountIdRequest request, ServerCallContext context)
        {
            try
            {
                IEnumerable<net.atos.daf.ct2.alert.entity.NotificationTemplate> notificationTemplateList = await _alertManager.GetAlertNotificationTemplate();

                NotificationTemplateResponse response = new NotificationTemplateResponse();
                foreach (var item in notificationTemplateList)
                {
                    response.NotificationTemplatelist.Add(new NotificationTemplate
                    {
                        Id = item.Id,
                        AlertCategoryType = item.AlertCategoryType,
                        AlertType = item.AlertType,
                        Text = item.Text,
                        Subject = item.Subject,
                        CreatedAt = item.CreatedAt,
                        ModifiedAt = item.ModifiedAt
                    });
                }
                response.Message = "Alert notification template data is retrieved";
                response.Code = ResponseCode.Success;
                //_logger.Info("GetNotificationTemplate method in alert service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new NotificationTemplateResponse
                {
                    Code = ResponseCode.Failed,
                    Message = "Get Notification Template fail : " + ex.Message
                });
            }
        }
        #endregion

        #region Get Recipient Label

        public override async Task<NotificationRecipientResponse> GetRecipientLabelList(OrgIdRequest request, ServerCallContext context)
        {
            try
            {

                IEnumerable<NotificationRecipient> notificationRecipientResponseList = await _alertManager.GetRecipientLabelList(request.OrganizationId);
                NotificationRecipientResponse response = new NotificationRecipientResponse();
                foreach (var item in notificationRecipientResponseList)
                {
                    response.NotificationRecipient.Add(_mapper.MapNotificationRecipientEntity(item));
                }
                response.Message = "Notification Recipient data retrieved";
                response.Code = ResponseCode.Success;
                //_logger.Info("Get notification recipient method in alert service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new NotificationRecipientResponse
                {
                    Code = ResponseCode.Failed,
                    Message = "Get notification recipient list fail : " + ex.Message
                });
            }
        }

        #endregion

        #region Alert Category Filter
        public override async Task<AlertCategoryFilterResponse> GetAlertCategoryFilter(AlertCategoryFilterIdRequest request, ServerCallContext context)
        {
            try
            {
                var response = new AlertCategoryFilterResponse();
                var enumTranslationList = await _alertManager.GetAlertCategory();
                var notificationTemplate = await GetNotificationTemplate(new AccountIdRequest { AccountId = request.AccountId }, context);
                foreach (var item in enumTranslationList)
                {
                    response.EnumTranslation.Add(_mapper.MapEnumTranslation(item));
                }

                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_orgid")).FirstOrDefault()?.Value ?? "0");
                IEnumerable<int> featureIds = JsonConvert.DeserializeObject<IEnumerable<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? null);
                List<visibility.entity.VehicleDetailsAccountVisibilityForAlert> vehicleDetailsAccountVisibilty = new List<visibility.entity.VehicleDetailsAccountVisibilityForAlert>();
                ////Feature Id is passed as 0 because feature wise filtering is applied seperately below.
                //var vehicleDetailsAccountVisibilty
                //                              = await _visibilityManager
                //                                 .GetVehicleByAccountVisibilityTemp(request.AccountId, loggedInOrgId, request.OrganizationId, 0);

                IEnumerable<visibility.entity.VehicleDetailsAccountVisibilityForAlert> vehicleAccountVisibiltyList
                   = await _visibilityManager.GetVehicleByAccountVisibilityForAlert(request.AccountId, loggedInOrgId, request.OrganizationId, featureIds.ToArray());
                //append visibile vins
                vehicleDetailsAccountVisibilty.AddRange(vehicleAccountVisibiltyList);
                //remove duplicate vins by key as vin
                vehicleDetailsAccountVisibilty = vehicleDetailsAccountVisibilty.GroupBy(c => c.Vin, (key, c) => c.FirstOrDefault()).ToList();

                if (vehicleDetailsAccountVisibilty.Any())
                {
                    var res = JsonConvert.SerializeObject(vehicleDetailsAccountVisibilty);
                    response.AssociatedVehicleRequest.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<AssociatedVehicleRequest>>(res)
                        );
                    //var vehicleByVisibilityAndFeature
                    //                            = await _visibilityManager
                    //                                .GetVehicleByVisibilityAndFeatureTemp(request.AccountId, loggedInOrgId, request.OrganizationId,
                    //                                                                   request.RoleId, AlertConstants.ALERT_FEATURE_NAME);
                    var vehicleByVisibilityAndFeature
                                                = await _visibilityManager.GetSubscribedVehicleByAlertFeature(featureIds.ToList(), request.OrganizationId);

                    res = JsonConvert.SerializeObject(vehicleByVisibilityAndFeature);
                    response.AlertCategoryFilterRequest.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<AlertCategoryFilterRequest>>(res)
                        );

                }
                if (notificationTemplate.NotificationTemplatelist != null)
                {
                    foreach (var item in notificationTemplate.NotificationTemplatelist)
                    {
                        response.NotificationTemplate.Add(_mapper.MapNotificationTemplate(item));
                    }
                }
                response.Message = AlertConstants.ALERT_FILTER_SUCCESS_MSG;
                response.Code = ResponseCode.Success;
                //_logger.Info("Get method in alert service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AlertCategoryFilterResponse
                {
                    Code = ResponseCode.InternalServerError,
                    Message = ex.Message
                });
            }
        }
        #endregion


        #region Portal Notification 
        public override async Task<NotificationViewResponse> InsertViewNotification(NotificationViewRequest request, ServerCallContext context)
        {
            try
            {
                var loggedInAccountId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_accountid")).FirstOrDefault()?.Value ?? "0");
                List<NotificationViewHistory> notificationViewHistories = new List<NotificationViewHistory>();
                notificationViewHistories = _mapper.GetNotificationViewHistoryEntity(request);
                int id = await _alertManager.InsertViewNotification(notificationViewHistories, loggedInAccountId);
                return await Task.FromResult(new NotificationViewResponse
                {
                    Message = id > 0 ? $"Data saved successful" : $"Data saved  failed",
                    Code = id > 0 ? ResponseCode.Success : ResponseCode.Failed
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new NotificationViewResponse
                {
                    Message = "Exception :-" + ex.Message,
                    Code = ResponseCode.Failed
                });
            }
        }

        public override async Task<OfflineNotificationResponse> GetOfflinePushNotification(OfflinePushNotiRequest request, ServerCallContext context)
        {
            try
            {
                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_orgid")).FirstOrDefault()?.Value ?? "0");
                IEnumerable<int> featureIds = JsonConvert.DeserializeObject<IEnumerable<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? "0");
                List<string> visiblityVehicle = await GetVehicleByAccountVisibility(loggedInOrgId, request.AccountId, request.OrganizationId, featureIds);
                OfflinePushNotificationFilter offlinePushNotificationFilter = new OfflinePushNotificationFilter();
                offlinePushNotificationFilter.AccountId = request.AccountId;
                offlinePushNotificationFilter.OrganizationId = request.OrganizationId;
                offlinePushNotificationFilter.Vins = visiblityVehicle;
                offlinePushNotificationFilter.FeatureIds = featureIds.ToList();
                OfflinePushNotification offlinePushNotification = new OfflinePushNotification();
                offlinePushNotification = await _alertManager.GetOfflinePushNotification(offlinePushNotificationFilter);
                //Intersect Vin by visibility
                //var intersectNotVin = (from notiVin in offlinePushNotification.NotificationDisplayProp
                //                       join visiVin in visiblityVehicle on notiVin.Vin equals visiVin
                //                       select notiVin).ToList();

                //offlinePushNotification.NotificationDisplayProp = null;
                //offlinePushNotification.NotificationDisplayProp = intersectNotVin;
                OfflineNotificationResponse offlineNotificationResponse = new OfflineNotificationResponse();
                offlineNotificationResponse = _mapper.ToOfflineNotificationResponse(offlinePushNotification);
                offlineNotificationResponse.Message = offlinePushNotification.NotificationDisplayProp != null ? $" Offline notification data fetched successful" : $" Offline notification data not found";
                offlineNotificationResponse.Code = offlinePushNotification.NotificationDisplayProp != null ? ResponseCode.Success : ResponseCode.NotFound;
                return await Task.FromResult(offlineNotificationResponse);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new OfflineNotificationResponse
                {
                    Message = "Exception :-" + ex.Message,
                    Code = ResponseCode.Failed
                });
            }
        }

        private async Task<List<string>> GetVehicleByAccountVisibility(int loggedInOrgId, int accountId, int organizationId, IEnumerable<int> featureIds)
        {
            try
            {
                List<visibility.entity.VehicleDetailsAccountVisibilityForAlert> vehicleDetailsAccountVisibilty = new List<visibility.entity.VehicleDetailsAccountVisibilityForAlert>();
                List<string> visibleVin = new List<string>();
                if (featureIds != null)
                {
                    //foreach (int featureId in featureIds)
                    //{
                    IEnumerable<visibility.entity.VehicleDetailsAccountVisibilityForAlert> vehicleAccountVisibiltyList
                     = await _visibilityManager.GetVehicleByAccountVisibilityForAlert(accountId, loggedInOrgId, organizationId, featureIds.ToArray());
                    //append visibile vins
                    vehicleDetailsAccountVisibilty.AddRange(vehicleAccountVisibiltyList);
                    //remove duplicate vins by key as vin
                    vehicleDetailsAccountVisibilty = vehicleDetailsAccountVisibilty.GroupBy(c => c.Vin, (key, c) => c.FirstOrDefault()).ToList();
                    // }
                }
                if (vehicleDetailsAccountVisibilty.Any())
                {
                    visibleVin = vehicleDetailsAccountVisibilty.Select(x => x.Vin).ToList();
                }

                return visibleVin;

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

    }
}

