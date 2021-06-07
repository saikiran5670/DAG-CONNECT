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

namespace net.atos.daf.ct2.alertservice.Services
{
    public class AlertManagementService : AlertService.AlertServiceBase
    {
        private ILog _logger;
        private readonly IAlertManager _alertManager;
        private readonly Mapper _mapper;
        private readonly IVisibilityManager _visibilityManager;

        public AlertManagementService(IAlertManager alertManager, IVisibilityManager visibilityManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _alertManager = alertManager;
            _mapper = new Mapper();
            _visibilityManager = visibilityManager;
        }

        #region ActivateAlert,SuspendAlert and  DeleteAlert
        public override async Task<AlertResponse> ActivateAlert(IdRequest request, ServerCallContext context)
        {
            try
            {
                var id = await _alertManager.ActivateAlert(request.AlertId, ((char)AlertState.Active), ((char)AlertState.Suspend));
                return await Task.FromResult(new AlertResponse
                {
                    Message = id > 0 ? String.Format(AlertConstants.ACTIVATED_ALERT_SUCCESS_MSG, id) : String.Format(AlertConstants.ACTIVATED_ALERT_FAILURE_MSG, request.AlertId, AlertConstants.ALERT_FAILURE_MSG),
                    Code = id > 0 ? ResponseCode.Success : ResponseCode.Failed
                });

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AlertResponse
                {
                    Message = ex.Message,
                    Code = ResponseCode.InternalServerError
                });
            }
        }

        public override async Task<AlertResponse> SuspendAlert(IdRequest request, ServerCallContext context)
        {
            try
            {
                var id = await _alertManager.SuspendAlert(request.AlertId, ((char)AlertState.Suspend), ((char)AlertState.Active));
                return await Task.FromResult(new AlertResponse
                {
                    Message = id > 0 ? String.Format(AlertConstants.SUSPEND_ALERT_SUCCESS_MSG, id) : String.Format(AlertConstants.SUSPEND_ALERT_FAILURE_MSG, request.AlertId, AlertConstants.ALERT_FAILURE_MSG),
                    Code = id > 0 ? ResponseCode.Success : ResponseCode.Failed
                });

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AlertResponse
                {
                    Message = ex.Message,
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
                return await Task.FromResult(new AlertResponse
                {
                    Message = id > 0 ? String.Format(AlertConstants.DELETE_ALERT_SUCCESS_MSG, id) : String.Format(AlertConstants.DELETE_ALERT_FAILURE_MSG, request.AlertId, AlertConstants.ALERT_FAILURE_MSG),
                    Code = id > 0 ? ResponseCode.Success : ResponseCode.Failed
                });

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AlertResponse
                {
                    Message = String.Format(AlertConstants.DELETE_ALERT_FAILURE_MSG, request.AlertId, ex.Message),
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
                _logger.Info("Get method in alert service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AlertCategoryResponse
                {
                    Code = ResponseCode.Failed,
                    Message = "Get alert category fail : " + ex.Message
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
                IEnumerable<Alert> alertList = await _alertManager.GetAlertList(request.AccountId, request.OrganizationId);

                AlertListResponse response = new AlertListResponse();
                foreach (var item in alertList)
                {
                    response.AlertRequest.Add(_mapper.MapAlertEntity(item));
                }

                response.Message = "Alert data retrieved";
                response.Code = ResponseCode.Success;
                _logger.Info("Get method in alert service called.");
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
                var IsLandmarkIdActive = await _alertManager.IsLandmarkActiveInAlert(landmarkIds);
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
                _logger.Info("GetNotificationTemplate method in alert service called.");
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

                IEnumerable<NotificationRecipient> NotificationRecipientResponseList = await _alertManager.GetRecipientLabelList(request.OrganizationId);
                NotificationRecipientResponse response = new NotificationRecipientResponse();
                foreach (var item in NotificationRecipientResponseList)
                {
                    response.NotificationRecipient.Add(_mapper.MapNotificationRecipientEntity(item));
                }
                response.Message = "Notification Recipient data retrieved";
                response.Code = ResponseCode.Success;
                _logger.Info("Get notification recipient method in alert service called.");
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
                
                foreach (var item in enumTranslationList)
                {
                    response.EnumTranslation.Add(_mapper.MapEnumTranslation(item));
                }

                var vehicleByVisibilityAndFeature
                                            = await _visibilityManager
                                                .GetVehicleByVisibilityAndFeature(request.AccountId, request.OrganizationId,
                                                                                   request.RoleId, AlertConstants.ALERT_FEATURE_NAME);

                var res = JsonConvert.SerializeObject(vehicleByVisibilityAndFeature);
                response.AlertCategoryFilterRequest.AddRange(
                    JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<AlertCategoryFilterRequest>>(res)
                    );

                response.Message = AlertConstants.ALERT_FILTER_SUCCESS_MSG;
                response.Code = ResponseCode.Success;
                _logger.Info("Get method in alert service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new AlertCategoryFilterResponse
                {
                    Code = ResponseCode.InternalServerError,
                    Message =  ex.Message
                });
            }
        }
        #endregion
    }
}

