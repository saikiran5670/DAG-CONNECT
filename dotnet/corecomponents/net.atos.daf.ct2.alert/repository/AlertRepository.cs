using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.alert.entity;
using net.atos.daf.ct2.alert.ENUM;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.alert.repository
{
    public class AlertRepository : IAlertRepository
    {
        private readonly IDataAccess _dataAccess;
        public AlertRepository(IDataAccess dataAccess)
        {
            this._dataAccess = dataAccess;

        }
        #region Create Alert
        public async Task<Alert> CreateAlert(Alert alert)
        {
            _dataAccess.Connection.Open();
            var transactionScope = _dataAccess.Connection.BeginTransaction();
            try
            {
                alert = await Exists(alert);
                // duplicate alert name
                if (alert.Exists)
                {
                    return alert;
                }

                // duplicate recipients label
                if (alert.Notifications.Count() > 0)
                {
                    int cntDupliReclabel = 0;
                    foreach (var notification in alert.Notifications)
                    {
                        foreach (var notificationRecipient in notification.NotificationRecipients)
                        {
                            if (RecipientLabelExists(notificationRecipient, alert.OrganizationId).Result.Exists)
                            {
                                cntDupliReclabel = +1;
                            }
                        }
                    }
                    if (cntDupliReclabel > 0)
                    {
                        return alert;
                    }
                }

                var parameterAlert = new DynamicParameters();
                parameterAlert.Add("@organization_id", alert.OrganizationId);
                parameterAlert.Add("@name", alert.Name);
                parameterAlert.Add("@category", alert.Category);
                parameterAlert.Add("@type", Convert.ToChar(alert.Type));
                parameterAlert.Add("@validity_period_type", Convert.ToChar(alert.ValidityPeriodType));
                if (alert.ValidityPeriodType.ToUpper().ToString() == ((char)ValidityPeriodType.Custom).ToString())
                {
                    parameterAlert.Add("@validity_start_date", alert.ValidityStartDate);
                    parameterAlert.Add("@validity_end_date", alert.ValidityEndDate);
                }
                else
                {
                    parameterAlert.Add("@validity_start_date", null);
                    parameterAlert.Add("@validity_end_date", null);
                }
                parameterAlert.Add("@vehicle_group_id", alert.VehicleGroupId);
                parameterAlert.Add("@state", alert.State);
                parameterAlert.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameterAlert.Add("@created_by", alert.CreatedBy);

                string queryAlert = @"INSERT INTO master.alert(organization_id, name, category, type, validity_period_type, validity_start_date, validity_end_date, vehicle_group_id, state, created_at, created_by)
	                                    VALUES (@organization_id, @name, @category, @type, @validity_period_type, @validity_start_date, @validity_end_date, @vehicle_group_id, @state, @created_at, @created_by) RETURNING id";

                var alertId = await _dataAccess.ExecuteScalarAsync<int>(queryAlert, parameterAlert);
                alert.Id = alertId;

                foreach (var landmark in alert.AlertLandmarkRefs)
                {
                    landmark.AlertId = alertId;
                    int landmarkRefId = await CreateAlertLandmarkRefs(landmark);
                    landmark.Id = landmarkRefId;
                }
                foreach (var urgencylevel in alert.AlertUrgencyLevelRefs)
                {
                    urgencylevel.AlertId = alertId;
                    int urgencylevelRefId = await CreateAlertUrgencyLevelRef(urgencylevel);
                    urgencylevel.Id = urgencylevelRefId;

                    foreach (var alertfilter in urgencylevel.AlertFilterRefs)
                    {
                        alertfilter.AlertId = alertId;
                        alertfilter.AlertUrgencyLevelId = urgencylevelRefId;
                        int alertfilterRefId = await CreateAlertFilterRef(alertfilter);
                        alertfilter.Id = alertfilterRefId;
                        foreach (var alertTimingDetail in alertfilter.AlertTimingDetails)
                        {
                            alertTimingDetail.RefId = alertfilterRefId;
                            alertTimingDetail.Type = AlertTimingDetailType.AlertAdvanceFilter.ToString();
                            int alertTimingDetailId = await CreateAlertTimingDetail(alertTimingDetail);
                            alertTimingDetail.Id = alertTimingDetailId;
                        }
                    }
                }
                foreach (var notification in alert.Notifications)
                {
                    notification.AlertId = alertId;
                    int notificationId = await CreateNotification(notification);
                    notification.Id = notificationId;
                    foreach (var alertTimingDetail in notification.AlertTimingDetails)
                    {
                        alertTimingDetail.RefId = notificationId;
                        alertTimingDetail.Type = AlertTimingDetailType.NotificationAdvanceFilter.ToString();
                        int alertTimingDetailId = await CreateAlertTimingDetail(alertTimingDetail);
                        alertTimingDetail.Id = alertTimingDetailId;
                    }
                    foreach (var limit in notification.NotificationLimits)
                    {
                        limit.NotificationId = notificationId;
                        int alertfilterRefId = await CreateNotificationLimit(limit);
                        limit.Id = alertfilterRefId;
                    }
                    foreach (var notificationRecipient in notification.NotificationRecipients)
                    {
                        notificationRecipient.NotificationId = notificationId;
                        int alertfilterRefId = await CreateNotificationrecipient(notificationRecipient);
                        notificationRecipient.Id = alertfilterRefId;
                    }
                }

                transactionScope.Commit();
            }
            catch (Exception)
            {
                transactionScope.Rollback();
                throw;
            }
            finally
            {
                _dataAccess.Connection.Close();
            }
            return alert;
        }
        private async Task<int> CreateAlertLandmarkRefs(AlertLandmarkRef landmark)
        {
            try
            {
                var parameterlandmarkref = new DynamicParameters();
                parameterlandmarkref.Add("@alert_id", landmark.AlertId);
                if (landmark.LandmarkType != null && landmark.LandmarkType.Length > 0)
                    parameterlandmarkref.Add("@landmark_type", Convert.ToChar(landmark.LandmarkType));
                else
                    parameterlandmarkref.Add("@landmark_type", null);
                parameterlandmarkref.Add("@ref_id", landmark.RefId);
                parameterlandmarkref.Add("@distance", landmark.Distance);
                if (landmark.UnitType != null && landmark.UnitType.Length > 0)
                    parameterlandmarkref.Add("@unit_type", Convert.ToChar(landmark.UnitType));
                else
                    parameterlandmarkref.Add("@unit_type", null);
                parameterlandmarkref.Add("@state", 'A');
                parameterlandmarkref.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                string queryLandmarkref = @"INSERT INTO master.alertlandmarkref(alert_id, landmark_type, ref_id, distance, unit_type, state, created_at)
                                                VALUES (@alert_id, @landmark_type, @ref_id, @distance, @unit_type, @state, @created_at) RETURNING id";
                var id = await _dataAccess.ExecuteScalarAsync<int>(queryLandmarkref, parameterlandmarkref);
                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<int> CreateAlertUrgencyLevelRef(AlertUrgencyLevelRef urgencylevel)
        {
            try
            {
                var parameterurgencylevelref = new DynamicParameters();
                parameterurgencylevelref.Add("@alert_id", urgencylevel.AlertId);
                if (urgencylevel.UrgencyLevelType != null && urgencylevel.UrgencyLevelType.Length > 0)
                    parameterurgencylevelref.Add("@urgency_level_type", Convert.ToChar(urgencylevel.UrgencyLevelType));
                else
                    parameterurgencylevelref.Add("@urgency_level_type", null);
                parameterurgencylevelref.Add("@threshold_value", urgencylevel.ThresholdValue);
                if (urgencylevel.UnitType != null && urgencylevel.UnitType.Length > 0)
                    parameterurgencylevelref.Add("@unit_type", Convert.ToChar(urgencylevel.UnitType));
                else
                    parameterurgencylevelref.Add("@unit_type", null);
                BitArray bitArray = new BitArray(7);
                for (int i = 0; i < urgencylevel.DayType.Length; i++)
                {
                    bitArray.Set(i, urgencylevel.DayType[i]);
                }
                parameterurgencylevelref.Add("@day_type", bitArray);
                if (urgencylevel.PeriodType != null && urgencylevel.PeriodType.Length > 0)
                    parameterurgencylevelref.Add("@period_type", Convert.ToChar(urgencylevel.PeriodType));
                else
                    parameterurgencylevelref.Add("@period_type", null);
                parameterurgencylevelref.Add("@urgencylevel_start_date", urgencylevel.UrgencylevelStartDate);
                parameterurgencylevelref.Add("@urgencylevel_end_date", urgencylevel.UrgencylevelEndDate);
                parameterurgencylevelref.Add("@state", 'A');
                parameterurgencylevelref.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                string queryUrgencylevel = @"INSERT INTO master.alerturgencylevelref(alert_id, urgency_level_type, threshold_value, unit_type, day_type, period_type, urgencylevel_start_date, urgencylevel_end_date, state, created_at)
	                                                                            VALUES (@alert_id, @urgency_level_type, @threshold_value, @unit_type, @day_type, @period_type, @urgencylevel_start_date, @urgencylevel_end_date, @state, @created_at) RETURNING id";
                int id = await _dataAccess.ExecuteScalarAsync<int>(queryUrgencylevel, parameterurgencylevelref);
                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<int> CreateAlertFilterRef(AlertFilterRef alertfilter)
        {
            try
            {
                var parameteralertfilterref = new DynamicParameters();
                parameteralertfilterref.Add("@alert_id", alertfilter.AlertId);
                parameteralertfilterref.Add("@alert_urgency_level_id", alertfilter.AlertUrgencyLevelId);
                if (alertfilter.FilterType != null && alertfilter.FilterType.Length > 0)
                    parameteralertfilterref.Add("@filter_type", Convert.ToChar(alertfilter.FilterType));
                else
                    parameteralertfilterref.Add("@filter_type", null);
                parameteralertfilterref.Add("@threshold_value", alertfilter.ThresholdValue);
                if (alertfilter.UnitType != null && alertfilter.UnitType.Length > 0)
                    parameteralertfilterref.Add("@unit_type", Convert.ToChar(alertfilter.UnitType));
                else
                    parameteralertfilterref.Add("@unit_type", null);
                if (alertfilter.LandmarkType != null && alertfilter.LandmarkType.Length > 0)
                    parameteralertfilterref.Add("@landmark_type", Convert.ToChar(alertfilter.LandmarkType));
                else
                    parameteralertfilterref.Add("@landmark_type", null);
                parameteralertfilterref.Add("@ref_id", alertfilter.RefId);
                if (alertfilter.PositionType != null && alertfilter.PositionType.Length > 0)
                    parameteralertfilterref.Add("@position_type", Convert.ToChar(alertfilter.PositionType));
                else
                    parameteralertfilterref.Add("@position_type", null);
                //BitArray bitArray = new BitArray(7);
                //for (int i = 0; i < alertfilter.DayType.Length; i++)
                //{
                //    bitArray.Set(i, alertfilter.DayType[i]);
                //}
                //parameteralertfilterref.Add("@day_type", bitArray);
                //if (alertfilter.PeriodType != null && alertfilter.PeriodType.Length > 0)
                //    parameteralertfilterref.Add("@period_type", Convert.ToChar(alertfilter.PeriodType));
                //else
                //    parameteralertfilterref.Add("@period_type", null);
                //parameteralertfilterref.Add("@filter_start_date", alertfilter.FilterStartDate);
                //parameteralertfilterref.Add("@filter_end_date", alertfilter.FilterEndDate);
                parameteralertfilterref.Add("@state", 'A');
                parameteralertfilterref.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                //string queryAlertfilter = @"INSERT INTO master.alertfilterref(alert_id, alert_urgency_level_id, filter_type, threshold_value, unit_type, landmark_type, ref_id, position_type, day_type, period_type, filter_start_date, filter_end_date, state, created_at)
                //                     VALUES (@alert_id, @alert_urgency_level_id, @filter_type, @threshold_value, @unit_type, @landmark_type, @ref_id, @position_type, @day_type, @period_type, @filter_start_date, @filter_end_date, @state, @created_at) RETURNING id";
                string queryAlertfilter = @"INSERT INTO master.alertfilterref(alert_id, alert_urgency_level_id, filter_type, threshold_value, unit_type, landmark_type, ref_id, position_type, state, created_at)
	                                    VALUES (@alert_id, @alert_urgency_level_id, @filter_type, @threshold_value, @unit_type, @landmark_type, @ref_id, @position_type, @state, @created_at) RETURNING id";
                int id = await _dataAccess.ExecuteScalarAsync<int>(queryAlertfilter, parameteralertfilterref);
                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<int> CreateNotification(Notification notification)
        {
            try
            {
                var parameternotification = new DynamicParameters();
                parameternotification.Add("@alert_id", notification.AlertId);
                if (notification.AlertUrgencyLevelType != null && notification.AlertUrgencyLevelType.Length > 0)
                    parameternotification.Add("@alert_urgency_level_type", Convert.ToChar(notification.AlertUrgencyLevelType.ToUpper()));
                else
                    parameternotification.Add("@alert_urgency_level_type", null);
                if (notification.FrequencyType != null && notification.FrequencyType.Length > 0)
                    parameternotification.Add("@frequency_type", Convert.ToChar(notification.FrequencyType.ToUpper()));
                else
                    parameternotification.Add("@frequency_type", null);
                parameternotification.Add("@frequency_threshhold_value", notification.FrequencyThreshholdValue);
                if (notification.ValidityType != null && notification.ValidityType.Length > 0)
                    parameternotification.Add("@validity_type", Convert.ToChar(notification.ValidityType.ToUpper()));
                else
                    parameternotification.Add("@validity_type", null);
                parameternotification.Add("@state", 'A');
                parameternotification.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameternotification.Add("@created_by", notification.CreatedBy);
                string queryNotification = @"INSERT INTO master.notification(alert_id, alert_urgency_level_type, frequency_type, frequency_threshhold_value, validity_type,state, created_at,created_by)
	                                    VALUES (@alert_id, @alert_urgency_level_type, @frequency_type, @frequency_threshhold_value, @validity_type,@state, @created_at,@created_by) RETURNING id";
                int id = await _dataAccess.ExecuteScalarAsync<int>(queryNotification, parameternotification);
                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<int> CreateNotificationAvailabilityPeriod(NotificationAvailabilityPeriod availabilityperiod)
        {
            try
            {
                var parameteravailabilityperiod = new DynamicParameters();
                parameteravailabilityperiod.Add("@notification_id", availabilityperiod.NotificationId);
                if (availabilityperiod.AvailabilityPeriodType != null && availabilityperiod.AvailabilityPeriodType.Length > 0)
                    parameteravailabilityperiod.Add("@availability_period_type", Convert.ToChar(availabilityperiod.AvailabilityPeriodType));
                else
                    parameteravailabilityperiod.Add("@availability_period_type", null);
                if (availabilityperiod.PeriodType != null && availabilityperiod.PeriodType.Length > 0)
                    parameteravailabilityperiod.Add("@period_type", Convert.ToChar(availabilityperiod.PeriodType));
                else
                    parameteravailabilityperiod.Add("@period_type", null);
                parameteravailabilityperiod.Add("@start_time", availabilityperiod.StartTime);
                parameteravailabilityperiod.Add("@end_time", availabilityperiod.EndTime);
                parameteravailabilityperiod.Add("@state", 'A');
                parameteravailabilityperiod.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                string queryAvailabilityperiod = @"INSERT INTO master.notificationavailabilityperiod(notification_id, availability_period_type, period_type, start_time, end_time, state, created_at)
	                                    VALUES (@notification_id, @availability_period_type, @period_type, @start_time, @end_time, @state, @created_at) RETURNING id";
                var id = await _dataAccess.ExecuteScalarAsync<int>(queryAvailabilityperiod, parameteravailabilityperiod);
                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<int> CreateNotificationLimit(NotificationLimit limit)
        {
            try
            {
                var parameterlimit = new DynamicParameters();
                parameterlimit.Add("@notification_id", limit.NotificationId);
                if (limit.NotificationModeType != null && limit.NotificationModeType.Length > 0)
                    parameterlimit.Add("@notification_mode_type", Convert.ToChar(limit.NotificationModeType));
                else
                    parameterlimit.Add("@notification_mode_type", null);
                parameterlimit.Add("@max_limit", limit.MaxLimit);
                if (limit.NotificationPeriodType != null && limit.NotificationPeriodType.Length > 0)
                    parameterlimit.Add("@notification_period_type", Convert.ToChar(limit.NotificationPeriodType));
                else
                    parameterlimit.Add("@notification_period_type", null);
                parameterlimit.Add("@period_limit", limit.PeriodLimit);
                parameterlimit.Add("@state", 'A');
                parameterlimit.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                string queryLimit = @"INSERT INTO master.notificationlimit(notification_id, notification_mode_type, max_limit, notification_period_type, period_limit, state, created_at)
	                                                            VALUES (@notification_id, @notification_mode_type, @max_limit, @notification_period_type, @period_limit, @state, @created_at) RETURNING id";
                var id = await _dataAccess.ExecuteScalarAsync<int>(queryLimit, parameterlimit);
                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<int> CreateNotificationrecipient(NotificationRecipient recipient)
        {
            try
            {
                var parameterrecipient = new DynamicParameters();
                parameterrecipient.Add("@notification_id", recipient.NotificationId);
                parameterrecipient.Add("@recipient_label", recipient.RecipientLabel);
                parameterrecipient.Add("@account_group_id", recipient.AccountGroupId);
                if (recipient.NotificationModeType != null && recipient.NotificationModeType.Length > 0)
                    parameterrecipient.Add("@notification_mode_type", Convert.ToChar(recipient.NotificationModeType));
                else
                    parameterrecipient.Add("@notification_mode_type", null);
                parameterrecipient.Add("@phone_no", recipient.PhoneNo);
                parameterrecipient.Add("@sms", recipient.Sms);
                parameterrecipient.Add("@email_id", recipient.EmailId);
                parameterrecipient.Add("@email_sub", recipient.EmailSub);
                parameterrecipient.Add("@email_text", recipient.EmailText);
                parameterrecipient.Add("@ws_url", recipient.WsUrl);
                if (recipient.WsType != null && recipient.WsType.Length > 0)
                    parameterrecipient.Add("@ws_type", Convert.ToChar(recipient.WsType));
                else
                    parameterrecipient.Add("@ws_type", null);
                parameterrecipient.Add("@ws_text", recipient.WsText);
                parameterrecipient.Add("@ws_login", recipient.WsLogin);
                parameterrecipient.Add("@ws_password", recipient.WsPassword);
                parameterrecipient.Add("@state", 'A');
                parameterrecipient.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                string queryRecipient = @"INSERT INTO master.notificationrecipient(notification_id, recipient_label, account_group_id, notification_mode_type, phone_no, sms, email_id, email_sub, email_text, ws_url, ws_type, ws_text, ws_login, ws_password, state, created_at)
                                    VALUES (@notification_id, @recipient_label, @account_group_id, @notification_mode_type, @phone_no, @sms, @email_id, @email_sub, @email_text, @ws_url, @ws_type, @ws_text, @ws_login, @ws_password, @state, @created_at) RETURNING id";
                var id = await _dataAccess.ExecuteScalarAsync<int>(queryRecipient, parameterrecipient);
                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Update Alert

        public async Task<Alert> UpdateAlert(Alert alert)
        {
            //Begin transaction scope for master.alert table
            _dataAccess.Connection.Open();
            var transactionScope = _dataAccess.Connection.BeginTransaction();
            try
            {
                alert = await Exists(alert);
                // duplicate alert name
                if (alert.Exists)
                {
                    return alert;
                }

                // duplicate recipients label
                if (alert.Notifications.Count() > 0)
                {
                    int cntDupliReclabel = 0;
                    foreach (var notification in alert.Notifications)
                    {
                        foreach (var notificationRecipient in notification.NotificationRecipients)
                        {
                            if (RecipientLabelExists(notificationRecipient, alert.OrganizationId).Result.Exists)
                            {
                                cntDupliReclabel = +1;
                            }
                        }
                    }
                    if (cntDupliReclabel > 0)
                    {
                        return alert;
                    }
                }

                var QueryStatement = @" UPDATE master.alert
                                        SET 
                                         name=@name                                        
        	                            ,validity_period_type=@validity_period_type
                                        ,validity_start_date=@validity_start_date
                                        ,validity_end_date=@validity_end_date
                                        ,vehicle_group_id=@vehicle_group_id
                                        ,modified_at=@modified_at
                                        ,modified_by=@modified_by
                                         WHERE id = @id
                                         RETURNING id;";

                var parameter = new DynamicParameters();
                parameter.Add("@id", alert.Id);
                parameter.Add("@name", alert.Name);
                parameter.Add("@validity_period_type", alert.ValidityPeriodType);
                if (alert.ValidityPeriodType.ToUpper().ToString() == ((char)ValidityPeriodType.Custom).ToString())
                {
                    parameter.Add("@validity_start_date", alert.ValidityStartDate);
                    parameter.Add("@validity_end_date", alert.ValidityEndDate);
                }
                else
                {
                    parameter.Add("@validity_start_date", null);
                    parameter.Add("@validity_end_date", null);
                }
                parameter.Add("@vehicle_group_id", alert.VehicleGroupId);
                parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameter.Add("@modified_by", alert.ModifiedBy);
                int alertId = await _dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                alert.Id = alertId;

                bool IsRefDeleted = await RemoveAlertRef(alert.ModifiedAt, alert.Id, alert.ModifiedBy);
                if (IsRefDeleted)
                {
                    foreach (var landmark in alert.AlertLandmarkRefs)
                    {
                        landmark.AlertId = alertId;
                        int landmarkRefId = await CreateAlertLandmarkRefs(landmark);
                    }
                    foreach (var urgencylevel in alert.AlertUrgencyLevelRefs)
                    {
                        urgencylevel.AlertId = alertId;
                        int urgencylevelRefId = await CreateAlertUrgencyLevelRef(urgencylevel);
                        foreach (var alertfilter in urgencylevel.AlertFilterRefs)
                        {
                            alertfilter.AlertId = alertId;
                            alertfilter.AlertUrgencyLevelId = urgencylevelRefId;
                            int alertfilterRefId = await CreateAlertFilterRef(alertfilter);
                        }
                    }
                    if (alert.Notifications.Count() > 0)
                    {
                        foreach (var notification in alert.Notifications)
                        {
                            notification.AlertId = alertId;
                            int notificationId = await CreateNotification(notification);
                            foreach (var alertTimingDetail in notification.AlertTimingDetails)
                            {
                                alertTimingDetail.RefId = notificationId;
                                alertTimingDetail.Type = AlertTimingDetailType.NotificationAdvanceFilter.ToString();
                                int alertTimingDetailId = await CreateAlertTimingDetail(alertTimingDetail);
                                alertTimingDetail.Id = alertTimingDetailId;
                            }
                            foreach (var limit in notification.NotificationLimits)
                            {
                                limit.NotificationId = notificationId;
                                int alertfilterRefId = await CreateNotificationLimit(limit);
                            }
                            foreach (var notificationRecipient in notification.NotificationRecipients)
                            {
                                notificationRecipient.NotificationId = notificationId;
                                int alertfilterRefId = await CreateNotificationrecipient(notificationRecipient);
                            }
                        }
                    }
                }
                transactionScope.Commit();
            }
            catch (Exception)
            {
                transactionScope.Rollback();
                throw;
            }
            finally
            {
                _dataAccess.Connection.Close();
            }

            return alert;
        }

        #endregion

        #region Update Alert State
        public async Task<int> UpdateAlertState(int alertId, char state, char checkState)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", alertId);
                parameter.Add("@state", state);
                parameter.Add("@checkstate", checkState);
                var query = $"update master.Alert set state = @state where id=@id and state=@checkstate RETURNING id";
                return await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> AlertStateToDelete(int alertId, char state)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", alertId);
                parameter.Add("@state", state);
                var query = $"update master.Alert set state = @state where id=@id  RETURNING id";
                return await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CheckIsNotificationExitForAlert(int alertId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@alert_id", alertId);
                parameter.Add("@state", ((char)AlertState.Active));

                var query = @"SELECT EXISTS (
		                                      SELECT 1
		                                      FROM master.notification
		                                      where alert_id = @alert_id and state = @state
				                            );";
                return await _dataAccess.ExecuteScalarAsync<bool>(query, parameter);

            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion

        #region Alert Category
        public async Task<IEnumerable<EnumTranslation>> GetAlertCategory()
        {
            try
            {
                var QueryStatement = @"SELECT                                     
                                    id as Id, 
                                    type as Type, 
                                    enum as Enum, 
                                    parent_enum as ParentEnum, 
                                    key as Key
                                    FROM translation.enumtranslation;";

                IEnumerable<EnumTranslation> enumtranslationlist = await _dataAccess.QueryAsync<EnumTranslation>(QueryStatement, null);

                return enumtranslationlist;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Get Alert List
        public async Task<IEnumerable<Alert>> GetAlertList(int accountid, int organizationid)
        {
            MapperRepo repositoryMapper = new MapperRepo();
            try
            {
                var parameterAlert = new DynamicParameters();

                //parameterAlert.Add("@name", alert.Name);
                //parameterAlert.Add("@category", alert.Category);
                //parameterAlert.Add("@type", Convert.ToChar(alert.Type));
                //parameterAlert.Add("@validity_period_type", Convert.ToChar(alert.ValidityPeriodType));
                //parameterAlert.Add("@validity_start_date", alert.ValidityStartDate);
                //parameterAlert.Add("@validity_end_date", alert.ValidityEndDate);
                //parameterAlert.Add("@vehicle_group_id", alert.VehicleGroupId);
                //parameterAlert.Add("@state", 'A');
                //parameterAlert.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                //parameterAlert.Add("@created_by", alert.CreatedBy);

                string queryAlert = @"SELECT distinct
                    ale.id as ale_id,
                    ale.organization_id as ale_organization_id,
                    ale.name as ale_name,
                    ale.category as ale_category,
                    ale.type as ale_type,
                    ale.validity_period_type as ale_validity_period_type,
                    ale.validity_start_date as ale_validity_start_date,
                    ale.validity_end_date as ale_validity_end_date,
                    ale.vehicle_group_id as ale_vehicle_group_id,
                    ale.state as ale_state,
                    ale.created_at as ale_created_at,
                    ale.created_by as ale_created_by,
                    ale.modified_at as ale_modified_at,
                    ale.modified_by as ale_modified_by,
                    aleurg.id as aleurg_id,
                    aleurg.alert_id as aleurg_alert_id,
                    aleurg.urgency_level_type as aleurg_urgency_level_type,
                    aleurg.threshold_value as aleurg_threshold_value,
                    aleurg.unit_type as aleurg_unit_type,
                    aleurg.day_type as aleurg_day_type,
                    aleurg.period_type as aleurg_period_type,
                    aleurg.urgencylevel_start_date as aleurg_urgencylevel_start_date,
                    aleurg.urgencylevel_end_date as aleurg_urgencylevel_end_date,
                    aleurg.state as aleurg_state,
                    aleurg.created_at as aleurg_created_at,
                    aleurg.modified_at as aleurg_modified_at,
                    alefil.id as alefil_id,
                    alefil.alert_id as alefil_alert_id,
                    alefil.alert_urgency_level_id as alefil_alert_urgency_level_id,
                    alefil.filter_type as alefil_filter_type,
                    alefil.threshold_value as alefil_threshold_value,
                    alefil.unit_type as alefil_unit_type,
                    alefil.landmark_type as alefil_landmark_type,
                    alefil.ref_id as alefil_ref_id,
                    alefil.position_type as alefil_position_type,
                    alefil.state as alefil_state,
                    alefil.created_at as alefil_created_at,
                    alefil.modified_at as alefil_modified_at,
                    alelan.id as alelan_id,
                    alelan.alert_id as alelan_alert_id,
                    alelan.landmark_type as alelan_landmark_type,
                    alelan.ref_id as alelan_ref_id,
                    alelan.distance as alelan_distance,
                    alelan.unit_type as alelan_unit_type,
                    alelan.state as alelan_state,
                    alelan.created_at as alelan_created_at,
                    alelan.modified_at as alelan_modified_at,
                    noti.id as noti_id,
                    noti.alert_id as noti_alert_id,
                    noti.alert_urgency_level_type as noti_alert_urgency_level_type,
                    noti.frequency_type as noti_frequency_type,
                    noti.frequency_threshhold_value as noti_frequency_threshhold_value,
                    noti.validity_type as noti_validity_type,
                    noti.state as noti_state,
                    noti.created_at as noti_created_at,
                    noti.created_by as noti_created_by,
                    noti.modified_at as noti_modified_at,
                    noti.modified_by as noti_modified_by,
                    notrec.id as notrec_id,
                    notrec.notification_id as notrec_notification_id,
                    notrec.recipient_label as notrec_recipient_label,
                    notrec.account_group_id as notrec_account_group_id,
                    notrec.notification_mode_type as notrec_notification_mode_type,
                    notrec.phone_no as notrec_phone_no,
                    notrec.sms as notrec_sms,
                    notrec.email_id as notrec_email_id,
                    notrec.email_sub as notrec_email_sub,
                    notrec.email_text as notrec_email_text,
                    notrec.ws_url as notrec_ws_url,
                    notrec.ws_type as notrec_ws_type,
                    notrec.ws_text as notrec_ws_text,
                    notrec.ws_login as notrec_ws_login,
                    notrec.ws_password as notrec_ws_password,
                    notrec.state as notrec_state,
                    notrec.created_at as notrec_created_at,
                    notrec.modified_at as notrec_modified_at,
                    notlim.id as notlim_id,
                    notlim.notification_id as notlim_notification_id,
                    notlim.notification_mode_type as notlim_notification_mode_type,
                    notlim.max_limit as notlim_max_limit,
                    notlim.notification_period_type as notlim_notification_period_type,
                    notlim.period_limit as notlim_period_limit,
                    notlim.state as notlim_state,
                    notlim.created_at as notlim_created_at,
                    notlim.modified_at as notlim_modified_at,
                    (CASE WHEN grp.group_type='S' THEN vehs.vin END) as vin,
                    (CASE WHEN grp.group_type='S' THEN vehs.license_plate_number END) as regno,
					(CASE WHEN grp.group_type='S' THEN vehs.name END) as vehiclename,
					(CASE WHEN grp.group_type<>'S' THEN grp.name END) as vehiclegroupname,
                    (CASE WHEN grp.group_type='S' THEN 'V' ELSE 'G' END) as ale_applyon
                    FROM master.alert ale
                    left join master.alerturgencylevelref aleurg
                    on ale.id= aleurg.alert_id and ale.state in ('A','I') and aleurg.state in ('A','I')

                    left join master.alertfilterref alefil
                    on aleurg.id=alefil.alert_urgency_level_id and alefil.state in ('A','I')

                    left join master.alertlandmarkref alelan
                    on ale.id=alelan.alert_id and alelan.state in ('A','I')
                    left join master.notification noti
                    on ale.id=noti.alert_id and ale.state in ('A','I') and noti.state in ('A','I')
                   
                    left join master.notificationrecipient notrec
                    on noti.id=notrec.notification_id and notrec.state in ('A','I')
                    left join master.notificationlimit notlim
                    on noti.id= notlim.notification_id and notlim.state in ('A','I')                    
					left join master.group grp 
					on ale.vehicle_group_id=grp.id
					left join master.groupref vgrpref
					on  grp.id=vgrpref.group_id and grp.object_type='V'	
					left join master.vehicle veh
					on vgrpref.ref_id=veh.id 
                    left join master.vehicle vehs
					on grp.ref_id=vehs.id and grp.group_type='S'
                     ";

                //if (accountid > 0 && organizationid > 0)
                //{
                //    queryAlert = queryAlert + " where ale.created_by = @created_by AND ale.organization_id = @organization_id";
                //    parameterAlert.Add("@organization_id", organizationid);
                //    parameterAlert.Add("@created_by", accountid);
                //}
                //else if (accountid == 0 && organizationid > 0)
                //{
                queryAlert = queryAlert + " where ale.organization_id = @organization_id and ale.state<>'D'";
                parameterAlert.Add("@organization_id", organizationid);
                //}               

                IEnumerable<AlertResult> alertResult = await _dataAccess.QueryAsync<AlertResult>(queryAlert, parameterAlert);
                return repositoryMapper.GetAlertList(alertResult);

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Duplicate Alert Type
        public Task<DuplicateAlertType> DuplicateAlertType(int alertId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", alertId);
                parameter.Add("@state", ((char)AlertState.Delete));
                string query =
                    @"SELECT id as Id, organization_id as OrganizationId, name as Name,category as Category, type as Type, validity_period_type as ValidityPeriodType, validity_start_date as ValidityStartDate, validity_end_date as ValidityEndDate, vehicle_group_id as VehicleGroupId, state as State	FROM master.alert WHERE id=@id and state<>@state";

                return _dataAccess.QueryFirstOrDefaultAsync<DuplicateAlertType>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Private method

        private async Task<bool> RemoveAlertRef(long modifiedAt, int alertId, int ModifiedBy)
        {
            char deleteChar = 'D';
            char activeState = 'A';
            await _dataAccess.ExecuteAsync("UPDATE master.alertfilterref SET state = @state , modified_at = @modified_at WHERE alert_id = @alert_id and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState });
            await _dataAccess.ExecuteAsync("UPDATE master.alertlandmarkref SET state = @state , modified_at = @modified_at WHERE alert_id = @alert_id and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState });
            await _dataAccess.ExecuteAsync("UPDATE master.alerturgencylevelref SET state = @state , modified_at = @modified_at WHERE alert_id = @alert_id and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState });
            await _dataAccess.ExecuteAsync("UPDATE master.notification SET state = @state , modified_at = @modified_at, modified_by=@modified_by WHERE alert_id = @alert_id and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, modified_by = ModifiedBy, alert_id = alertId, activeState = activeState });
            //await dataAccess.ExecuteAsync("UPDATE master.notificationavailabilityperiod SET state = @state , modified_at = @modified_at WHERE notification_id in (select id from master.notification where alert_id = @alert_id) and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState });
            await _dataAccess.ExecuteAsync("UPDATE master.notificationlimit SET state = @state , modified_at = @modified_at WHERE notification_id in (select id from master.notification where alert_id = @alert_id) and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState });
            await _dataAccess.ExecuteAsync("UPDATE master.notificationrecipient SET state = @state , modified_at = @modified_at WHERE notification_id in (select id from master.notification where alert_id = @alert_id) and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState });
            return true;
        }

        private async Task<Alert> Exists(Alert alert)
        {
            try
            {
                var parameter = new DynamicParameters();
                var query = @"select id from master.alert where 1=1 ";
                if (alert != null)
                {

                    // id
                    if (Convert.ToInt32(alert.Id) > 0)
                    {
                        parameter.Add("@id", alert.Id);
                        query = query + " and id!=@id";
                    }
                    // name
                    if (!string.IsNullOrEmpty(alert.Name))
                    {
                        parameter.Add("@name", alert.Name);
                        query = query + " and name=@name";
                    }
                    // organization id filter
                    if (alert.OrganizationId > 0)
                    {
                        parameter.Add("@organization_id", alert.OrganizationId);
                        query = query + " and organization_id=@organization_id ";
                    }
                }
                var alertId = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                if (alertId > 0)
                {
                    alert.Exists = true;
                    alert.Id = alertId;
                }
                return alert;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<NotificationRecipient> RecipientLabelExists(NotificationRecipient notificationRecipient, int organizationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                var query = @"select notirec.id from master.notificationrecipient notirec 
                                        inner join master.notification noti 
                                        on notirec.notification_id= noti.id
                                        inner join master.alert alert
                                        on alert.id=noti.alert_id
                                        where noti.state=@state
                                        and alert.state=@state
                                        and notirec.state=@state";
                parameter.Add("@state", Convert.ToChar(AlertState.Active));
                if (notificationRecipient != null)
                {

                    // id
                    if (Convert.ToInt32(notificationRecipient.Id) > 0)
                    {
                        parameter.Add("@id", notificationRecipient.Id);
                        query = query + " and notirec.id!=@id";
                    }
                    // name
                    if (!string.IsNullOrEmpty(notificationRecipient.RecipientLabel))
                    {
                        parameter.Add("@recipient_label", notificationRecipient.RecipientLabel);
                        query = query + " and notirec.recipient_label=@recipient_label";
                    }
                    // organization id filter
                    if (organizationId > 0)
                    {
                        parameter.Add("@organization_id", organizationId);
                        query = query + " and alert.organization_id=@organization_id ";
                    }
                }
                var notificationRecipientId = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                if (notificationRecipientId > 0)
                {
                    notificationRecipient.Exists = true;
                    notificationRecipient.Id = notificationRecipientId;
                }
                return notificationRecipient;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<int> CreateAlertTimingDetail(AlertTimingDetail alertTimingDetail)
        {
            try
            {
                var parameteralertTimingDetail = new DynamicParameters();
                parameteralertTimingDetail.Add("@type", Convert.ToChar(alertTimingDetail.Type));
                parameteralertTimingDetail.Add("@ref_id", alertTimingDetail.RefId);
                BitArray bitArray = new BitArray(7);
                for (int i = 0; i < alertTimingDetail.DayType.Length; i++)
                {
                    bitArray.Set(i, alertTimingDetail.DayType[i]);
                }
                parameteralertTimingDetail.Add("@day_type", bitArray);
                if (alertTimingDetail.PeriodType != null && alertTimingDetail.PeriodType.Length > 0)
                    parameteralertTimingDetail.Add("@period_type", Convert.ToChar(alertTimingDetail.PeriodType));
                else
                    parameteralertTimingDetail.Add("@period_type", null);
                parameteralertTimingDetail.Add("@start_date", alertTimingDetail.StartDate);
                parameteralertTimingDetail.Add("@end_date", alertTimingDetail.EndDate);
                parameteralertTimingDetail.Add("@state", AlertState.Active);
                parameteralertTimingDetail.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                string queryAvailabilityperiod = @"INSERT INTO master.alerttimingdetail(type, ref_id, day_type, period_type, start_date, end_date, state, created_at)
	                                    VALUES (@type, @ref_id, @day_type, @period_type, @start_date, @end_date, @state, @created_at) RETURNING id";
                var id = await _dataAccess.ExecuteScalarAsync<int>(queryAvailabilityperiod, parameteralertTimingDetail);
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Landmark Delete Validation
        public async Task<bool> IsLandmarkActiveInAlert(List<int> landmarkId)
        {
            try
            {
                var parameter = new DynamicParameters();
                var query = string.Empty;
                dynamic responseId;

                query = @"select id from master.alertfilterref where ref_id = any(@ref_id) and state=@state";
                parameter.Add("@ref_id", landmarkId);
                parameter.Add("@state", Convert.ToChar(AlertState.Active));
                responseId = await _dataAccess.QueryAsync<dynamic>(query, parameter);
                if (((System.Collections.Generic.List<object>)responseId).Count == 0)
                {
                    query = @"select id from master.alertlandmarkref where ref_id = any(@ref_id) and state=@state";
                    parameter.Add("@ref_id", landmarkId);
                    parameter.Add("@state", Convert.ToChar(AlertState.Active));
                    responseId = await _dataAccess.QueryAsync<dynamic>(query, parameter);
                    if (((System.Collections.Generic.List<object>)responseId).Count == 0)
                        return false;
                    else
                        return true;
                }
                else
                    return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Alert Category Notification Template
        public async Task<IEnumerable<NotificationTemplate>> GetAlertNotificationTemplate()
        {
            try
            {
                var queryStatement = @"SELECT 
                                        id as Id, 
                                        alert_category_type as AlertCategoryType, 
                                        alert_type as AlertType, 
                                        text as Text, 
                                        created_at as CreatedAt, 
                                        modified_at as ModifiedAt, 
                                        subject as Subject
                                        FROM 
                                        master.notificationtemplate; ";

                IEnumerable<NotificationTemplate> notificationTemplatelist = await _dataAccess.QueryAsync<NotificationTemplate>(queryStatement, null);

                return notificationTemplatelist;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        public async Task<IEnumerable<NotificationRecipient>> GetRecipientLabelList(int organizationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                string queryRecipientLabel = @"SELECT notirec.id as Id
		                                                    ,notification_id as NotificationId
		                                                    , recipient_label as RecipientLabel
		                                                    , account_group_id as AccountGroupId
		                                                    , notification_mode_type as NotificationModeType
		                                                    , phone_no as PhoneNo
		                                                    , sms as Sms
		                                                    , email_id as EmailId
		                                                    , email_sub as EmailSub
		                                                    , email_text as EmailText
		                                                    , ws_url as WsUrl
		                                                    , ws_type as WsType
		                                                    , ws_text as WsText
		                                                    , ws_login as WsLogin
		                                                    , ws_password as WsPassword
		                                                    , notirec.state as State
		                                                    , notirec.created_at as CreatedAt
	                                                    FROM master.notificationrecipient notirec
	                                                    inner join master.notification noti
	                                                    on notirec.notification_id=noti.id
	                                                    inner join master.alert alert
	                                                    on noti.alert_id=alert.id
	                                                    where notirec.state='A'
	                                                    and noti.state='A'
	                                                    and alert.state='A'
                                                        and alert.organization_id=@organization_id";
                parameter.Add("@organization_id", organizationId);
                IEnumerable<NotificationRecipient> notificationRecipientResult = await _dataAccess.QueryAsync<NotificationRecipient>(queryRecipientLabel, parameter);
                return notificationRecipientResult;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
