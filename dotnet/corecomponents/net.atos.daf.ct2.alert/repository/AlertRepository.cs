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
                    foreach (var alertTimingDetail in urgencylevel.AlertTimingDetails)
                    {
                        alertTimingDetail.RefId = urgencylevel.Id;
                        alertTimingDetail.Type = Convert.ToChar(AlertTimingDetailType.UrgencyLevelBasicFilter).ToString();
                        int alertTimingDetailId = await CreateAlertTimingDetail(alertTimingDetail);
                        alertTimingDetail.Id = alertTimingDetailId;
                    }

                    foreach (var alertfilter in urgencylevel.AlertFilterRefs)
                    {
                        alertfilter.AlertId = alertId;
                        alertfilter.AlertUrgencyLevelId = urgencylevelRefId;
                        int alertfilterRefId = await CreateAlertFilterRef(alertfilter);
                        alertfilter.Id = alertfilterRefId;
                        foreach (var alertTimingDetail in alertfilter.AlertTimingDetails)
                        {
                            alertTimingDetail.RefId = alertfilterRefId;
                            alertTimingDetail.Type = Convert.ToChar(AlertTimingDetailType.FilterRefAdvanceFilter).ToString();
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
                        alertTimingDetail.Type = Convert.ToChar(AlertTimingDetailType.NotificationAdvanceFilter).ToString();
                        int alertTimingDetailId = await CreateAlertTimingDetail(alertTimingDetail);
                        alertTimingDetail.Id = alertTimingDetailId;
                    }
                    foreach (var notificationRecipient in notification.NotificationRecipients)
                    {
                        notificationRecipient.NotificationId = notificationId;
                        NotificationRecipientRef notificationRecipientRef = new NotificationRecipientRef();
                        notificationRecipientRef.NotificationId = notificationId;
                        notificationRecipientRef.AlertId = alertId;
                        int alertNotificationRecipientId = 0;
                        if (notificationRecipient.Id > 0)
                        {
                            NotificationRecipient recipients = await CheckRecipientdetailsExists(notificationRecipient, alert.OrganizationId);
                            notificationRecipientRef.RecipientId = recipients.Id;
                            alertNotificationRecipientId = recipients.Id;
                            await CreateNotificationRecipientRef(notificationRecipientRef);
                        }
                        else
                        {
                            alertNotificationRecipientId = await CreateNotificationrecipient(notificationRecipient);
                            notificationRecipientRef.RecipientId = alertNotificationRecipientId;
                            await CreateNotificationRecipientRef(notificationRecipientRef);
                        }
                        notificationRecipient.Id = notificationRecipientRef.RecipientId;
                        foreach (var limit in notificationRecipient.NotificationLimits)
                        {
                            limit.NotificationId = notificationId;
                            limit.RecipientId = alertNotificationRecipientId;
                            int alertNotificationLimitId = 0;
                            if (limit.Id > 0)
                            {
                                NotificationLimit notificationLimit = await CheckNotificationLimitExists(limit);
                                //alertNotificationLimitId = await CreateNotificationLimit(limit);
                            }
                            else
                            {
                                alertNotificationLimitId = await CreateNotificationLimit(limit);
                            }
                            limit.Id = alertNotificationLimitId;
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
                parameterlandmarkref.Add("@state", Convert.ToChar(AlertState.Active));
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
                parameterurgencylevelref.Add("@state", Convert.ToChar(AlertState.Active));
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
                parameteralertfilterref.Add("@state", Convert.ToChar(AlertState.Active));
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
                parameternotification.Add("@state", Convert.ToChar(AlertState.Active));
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
                parameteravailabilityperiod.Add("@state", Convert.ToChar(AlertState.Active));
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
                parameterlimit.Add("@state", Convert.ToChar(AlertState.Active));
                parameterlimit.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameterlimit.Add("@recipient_id", limit.RecipientId);
                string queryLimit = @"INSERT INTO master.notificationlimit(notification_id, notification_mode_type, max_limit, notification_period_type, period_limit, state, created_at,recipient_id)
	                                                            VALUES (@notification_id, @notification_mode_type, @max_limit, @notification_period_type, @period_limit, @state, @created_at,@recipient_id) RETURNING id";
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
                // parameterrecipient.Add("@notification_id", recipient.NotificationId);
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
                parameterrecipient.Add("@state", Convert.ToChar(AlertState.Active));
                parameterrecipient.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                string queryRecipient = @"INSERT INTO master.notificationrecipient(recipient_label, account_group_id, notification_mode_type, phone_no, sms, email_id, email_sub, email_text, ws_url, ws_type, ws_text, ws_login, ws_password, state, created_at)
                                    VALUES (@recipient_label, @account_group_id, @notification_mode_type, @phone_no, @sms, @email_id, @email_sub, @email_text, @ws_url, @ws_type, @ws_text, @ws_login, @ws_password, @state, @created_at) RETURNING id";
                var id = await _dataAccess.ExecuteScalarAsync<int>(queryRecipient, parameterrecipient);
                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<int> CreateNotificationRecipientRef(NotificationRecipientRef notificationRecipientRef)
        {
            try
            {
                var parameterrecipientref = new DynamicParameters();
                parameterrecipientref.Add("@alert_id", notificationRecipientRef.AlertId);
                parameterrecipientref.Add("@notification_id", notificationRecipientRef.NotificationId);
                parameterrecipientref.Add("@recipient_id", notificationRecipientRef.RecipientId);
                parameterrecipientref.Add("@state", Convert.ToChar(AlertState.Active));
                parameterrecipientref.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                string queryRecipient = @"INSERT INTO master.notificationrecipientref(
	                                        alert_id, notification_id, recipient_id, state, created_at)
	                                        VALUES (@alert_id, @notification_id, @recipient_id, @state, @created_at) RETURNING id";
                var id = await _dataAccess.ExecuteScalarAsync<int>(queryRecipient, parameterrecipientref);
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
                alert.ModifiedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
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
                        foreach (var alertTimingDetail in urgencylevel.AlertTimingDetails)
                        {
                            alertTimingDetail.RefId = urgencylevelRefId;
                            alertTimingDetail.Type = Convert.ToChar(AlertTimingDetailType.UrgencyLevelBasicFilter).ToString();
                            int alertTimingDetailId = await CreateAlertTimingDetail(alertTimingDetail);
                            alertTimingDetail.Id = alertTimingDetailId;
                        }
                        foreach (var alertfilter in urgencylevel.AlertFilterRefs)
                        {
                            alertfilter.AlertId = alertId;
                            alertfilter.AlertUrgencyLevelId = urgencylevelRefId;
                            int alertfilterRefId = await CreateAlertFilterRef(alertfilter);
                            foreach (var alertTimingDetail in alertfilter.AlertTimingDetails)
                            {
                                alertTimingDetail.RefId = alertfilterRefId;
                                alertTimingDetail.Type = Convert.ToChar(AlertTimingDetailType.FilterRefAdvanceFilter).ToString();
                                int alertTimingDetailId = await CreateAlertTimingDetail(alertTimingDetail);
                                alertTimingDetail.Id = alertTimingDetailId;
                            }
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
                                alertTimingDetail.Type = Convert.ToChar(AlertTimingDetailType.NotificationAdvanceFilter).ToString();
                                int alertTimingDetailId = await CreateAlertTimingDetail(alertTimingDetail);
                                alertTimingDetail.Id = alertTimingDetailId;
                            }
                            foreach (var notificationRecipient in notification.NotificationRecipients)
                            {
                                //notificationRecipient.NotificationId = notificationId;
                                //int alertfilterRefId = await CreateNotificationrecipient(notificationRecipient);
                                //foreach (var limit in notificationRecipient.NotificationLimits)
                                //{
                                //    limit.NotificationId = notificationId;
                                //    int alertNotificationLimitId = await CreateNotificationLimit(limit);
                                //}
                                notificationRecipient.NotificationId = notificationId;
                                NotificationRecipientRef notificationRecipientRef = new NotificationRecipientRef();
                                notificationRecipientRef.NotificationId = notificationId;
                                notificationRecipientRef.AlertId = alertId;
                                int alertNotificationRecipientId = 0;
                                if (notificationRecipient.Id > 0)
                                {
                                    NotificationRecipient recipients = await CheckRecipientdetailsExists(notificationRecipient, alert.OrganizationId);
                                    notificationRecipientRef.RecipientId = recipients.Id;
                                    alertNotificationRecipientId = recipients.Id;
                                    await CreateNotificationRecipientRef(notificationRecipientRef);
                                }
                                else
                                {
                                    alertNotificationRecipientId = await CreateNotificationrecipient(notificationRecipient);
                                    notificationRecipientRef.RecipientId = alertNotificationRecipientId;
                                    await CreateNotificationRecipientRef(notificationRecipientRef);
                                }
                                notificationRecipient.Id = notificationRecipientRef.RecipientId;
                                foreach (var limit in notificationRecipient.NotificationLimits)
                                {
                                    limit.NotificationId = notificationId;
                                    limit.RecipientId = alertNotificationRecipientId;
                                    int alertNotificationLimitId = 0;
                                    if (limit.Id > 0)
                                    {
                                        NotificationLimit notificationLimit = await CheckNotificationLimitExists(limit);
                                        //alertNotificationLimitId = await CreateNotificationLimit(limit);
                                    }
                                    else
                                    {
                                        alertNotificationLimitId = await CreateNotificationLimit(limit);
                                    }
                                    limit.Id = alertNotificationLimitId;
                                }
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
                    (CASE WHEN grp.group_type='S' THEN vehs.id ELSE ale.vehicle_group_id END) as ale_vehicle_group_id,
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
					aletimeurg.id as aletimeurg_id, 
					aletimeurg.type as aletimeurg_type, 
					aletimeurg.ref_id as aletimeurg_ref_id, 
					aletimeurg.day_type as aletimeurg_day_type, 
					aletimeurg.period_type as aletimeurg_period_type, 
					aletimeurg.start_date as aletimeurg_start_date, 
					aletimeurg.end_date as aletimeurg_end_date, 
					aletimeurg.state as aletimeurg_state, 
					aletimeurg.created_at as aletimeurg_created_at, 
					aletimeurg.modified_at as aletimeurg_modified_at,
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
					aletimefil.id as aletimefil_id, 
					aletimefil.type as aletimefil_type, 
					aletimefil.ref_id as aletimefil_ref_id, 
					aletimefil.day_type as aletimefil_day_type, 
					aletimefil.period_type as aletimefil_period_type, 
					aletimefil.start_date as aletimefil_start_date, 
					aletimefil.end_date as aletimefil_end_date, 
					aletimefil.state as aletimefil_state, 
					aletimefil.created_at as aletimefil_created_at, 
					aletimefil.modified_at as aletimefil_modified_at,
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
					aletimenoti.id as aletimenoti_id, 
					aletimenoti.type as aletimenoti_type, 
					aletimenoti.ref_id as aletimenoti_ref_id, 
					aletimenoti.day_type as aletimenoti_day_type, 
					aletimenoti.period_type as aletimenoti_period_type, 
					aletimenoti.start_date as aletimenoti_start_date, 
					aletimenoti.end_date as aletimenoti_end_date, 
					aletimenoti.state as aletimenoti_state, 
					aletimenoti.created_at as aletimenoti_created_at, 
					aletimenoti.modified_at as aletimenoti_modified_at,
                    notrec.id as notrec_id,
                    notref.notification_id as notrec_notification_id,
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
					notlim.recipient_id as notlim_recipient_id,
                    (CASE WHEN grp.group_type='S' THEN vehs.vin END) as vin,
                    (CASE WHEN grp.group_type='S' THEN vehs.license_plate_number END) as regno,
					(CASE WHEN grp.group_type='S' THEN vehs.name END) as vehiclename,
					(CASE WHEN grp.group_type<>'S' THEN grp.name END) as vehiclegroupname,
                    (CASE WHEN grp.group_type='S' THEN 'S' ELSE 'G' END) as ale_applyon,
					notref.id as notref_id,
					notref.alert_id as notref_alert_id,
					notref.notification_id as notref_notification_id,
					notref.recipient_id as notref_recipient_id,
					notref.state as notref_state,
					notref.created_at as notref_created_at,
					notref.modified_at as notref_modified_at
                    FROM master.alert ale
                    left join master.alerturgencylevelref aleurg
                    on ale.id= aleurg.alert_id and ale.state in ('A','I') and aleurg.state in ('A','I')
					left join master.alerttimingdetail aletimeurg
					on aletimeurg.ref_id= aleurg.id and aletimeurg.type='U' and aletimeurg.state in ('A','I') and aleurg.state in ('A','I')
                    left join master.alertfilterref alefil
                    on aleurg.id=alefil.alert_urgency_level_id and alefil.state in ('A','I')
					left join master.alerttimingdetail aletimefil
					on aletimefil.ref_id= alefil.id and aletimefil.type='F' and aletimefil.state in ('A','I') and alefil.state in ('A','I')
                    left join master.alertlandmarkref alelan
                    on ale.id=alelan.alert_id and alelan.state in ('A','I')
                    left join master.notification noti
                    on ale.id=noti.alert_id and ale.state in ('A','I') and noti.state in ('A','I')
                   	left join master.alerttimingdetail aletimenoti
					on aletimenoti.ref_id= noti.id and aletimenoti.type='N' and aletimenoti.state in ('A','I') and noti.state in ('A','I')
					left join master.notificationrecipientref notref
					on noti.id=notref.notification_id and ale.state in ('A','I') and notref.state in ('A','I')
                    left join master.notificationrecipient notrec
                    on notrec.id=notref.recipient_id and notrec.state in ('A','I')
                    left join master.notificationlimit notlim
                    on notref.recipient_id= notlim.recipient_id and notlim.state in ('A','I')                    
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
            try
            {
                char deleteChar = Convert.ToChar(AlertState.Delete);
                char activeState = Convert.ToChar(AlertState.Active);
                await _dataAccess.ExecuteAsync("UPDATE master.alerttimingdetail SET state = @state , modified_at = @modified_at WHERE type=@type and ref_id in (select id from master.alertfilterref where alert_id = @alert_id and state=@activeState) and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState, type = Convert.ToChar(AlertTimingDetailType.FilterRefAdvanceFilter).ToString() });
                await _dataAccess.ExecuteAsync("UPDATE master.alertfilterref SET state = @state , modified_at = @modified_at WHERE alert_id = @alert_id and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState });
                await _dataAccess.ExecuteAsync("UPDATE master.alertlandmarkref SET state = @state , modified_at = @modified_at WHERE alert_id = @alert_id and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState });
                await _dataAccess.ExecuteAsync("UPDATE master.alerttimingdetail SET state = @state , modified_at = @modified_at WHERE type=@type and ref_id in (select id from master.alerturgencylevelref where alert_id = @alert_id and state=@activeState) and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState, type = Convert.ToChar(AlertTimingDetailType.UrgencyLevelBasicFilter).ToString() });
                await _dataAccess.ExecuteAsync("UPDATE master.alerturgencylevelref SET state = @state , modified_at = @modified_at WHERE alert_id = @alert_id and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState });
                await _dataAccess.ExecuteAsync("UPDATE master.alerttimingdetail SET state = @state , modified_at = @modified_at WHERE type=@type and ref_id in (select id from master.notification where alert_id = @alert_id and state=@activeState) and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState, type = Convert.ToChar(AlertTimingDetailType.NotificationAdvanceFilter).ToString() });
                await _dataAccess.ExecuteAsync("UPDATE master.notification SET state = @state , modified_at = @modified_at, modified_by=@modified_by WHERE alert_id = @alert_id and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, modified_by = ModifiedBy, alert_id = alertId, activeState = activeState });
                await _dataAccess.ExecuteAsync("UPDATE master.notificationrecipientref SET state = @state , modified_at = @modified_at WHERE alert_id = @alert_id and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState });
                //await _dataAccess.ExecuteAsync("UPDATE master.notificationlimit SET state = @state , modified_at = @modified_at WHERE notification_id in (select id from master.notification where alert_id = @alert_id) and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState });
                //await _dataAccess.ExecuteAsync("UPDATE master.notificationrecipient SET state = @state , modified_at = @modified_at WHERE notification_id in (select id from master.notification where alert_id = @alert_id) and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState });
                return true;
            }
            catch (Exception)
            {

                throw;
            }
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
                var query = @"select notirec.id from master.notificationrecipientref notiref
                                        --inner join master.notification noti 
                                        --on notiref.notification_id= noti.id
                                        inner join master.notificationrecipient notirec
                                        on notirec.id= notiref.recipient_id
                                        inner join master.alert alert
                                        on alert.id=notiref.alert_id
                                        where alert.state='A'
                                        and notirec.state='A'
                                        and notiref.state='A'";
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
                parameteralertTimingDetail.Add("@type", alertTimingDetail.Type.ToString());
                parameteralertTimingDetail.Add("@ref_id", alertTimingDetail.RefId);
                BitArray bitArray = new BitArray(7);
                for (int i = 0; i < alertTimingDetail.DayType.Length; i++)
                {
                    bitArray.Set(i, alertTimingDetail.DayType[i]);
                }
                parameteralertTimingDetail.Add("@day_type", bitArray);
                if (alertTimingDetail.PeriodType != null && alertTimingDetail.PeriodType.Length > 0)
                    parameteralertTimingDetail.Add("@period_type", Convert.ToChar(alertTimingDetail.PeriodType.ToString()));
                else
                    parameteralertTimingDetail.Add("@period_type", null);
                parameteralertTimingDetail.Add("@start_date", alertTimingDetail.StartDate);
                parameteralertTimingDetail.Add("@end_date", alertTimingDetail.EndDate);
                parameteralertTimingDetail.Add("@state", Convert.ToChar(AlertState.Active));
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

        private async Task<NotificationRecipient> CheckRecipientdetailsExists(NotificationRecipient notificationRecipient, int organizationId)
        {
            try
            {

                var parameter = new DynamicParameters();
                var query = @"select distinct notirec.id as Id
                                    ,notirec.recipient_label as RecipientLabel
                                    ,notirec.notification_mode_type as NotificationModeType
                                    ,notirec.phone_no as PhoneNo
                                    ,notirec.sms as Sms
                                    ,notirec.email_id as EmailId
                                    ,notirec.email_sub as EmailSub
                                    ,notirec.email_text as EmailText
                                    ,notirec.ws_url as WsUrl
                                    ,notirec.ws_type as WsType
                                    ,notirec.ws_text as WsText
                                    ,notirec.ws_login as WsLogin
                                    ,notirec.ws_password as WsPassword
                                        from master.notificationrecipient notirec 
                                        inner join master.notificationrecipientref notiref 
                                        on notirec.id= notiref.recipient_id
                                        inner join master.alert alert
                                        on alert.id=notiref.alert_id
                                        where notiref.state=@state
                                        and alert.state=@state
                                        and notirec.state=@state
                                        and notiref.recipient_id=@id
                                        and alert.organization_id=@organization_id ";
                parameter.Add("@state", Convert.ToChar(AlertState.Active));
                parameter.Add("@id", notificationRecipient.Id);
                parameter.Add("@organization_id", organizationId);

                NotificationRecipient notificationRecipientDb = await _dataAccess.QuerySingleOrDefaultAsync<NotificationRecipient>(query, parameter);
                if (notificationRecipientDb != null)
                {
                    if (notificationRecipient.Id == notificationRecipientDb.Id
                        && (notificationRecipient.RecipientLabel != notificationRecipientDb.RecipientLabel
                        || notificationRecipient.NotificationModeType != notificationRecipientDb.NotificationModeType
                        || notificationRecipient.PhoneNo != notificationRecipientDb.PhoneNo
                        || notificationRecipient.Sms != notificationRecipientDb.Sms
                        || notificationRecipient.EmailId != notificationRecipientDb.EmailId
                        || notificationRecipient.EmailSub != notificationRecipientDb.EmailSub
                        || notificationRecipient.EmailText != notificationRecipientDb.EmailText
                        || notificationRecipient.WsUrl != notificationRecipientDb.WsUrl
                        || notificationRecipient.WsType != notificationRecipientDb.WsType
                        || notificationRecipient.WsText != notificationRecipientDb.WsText
                        || notificationRecipient.WsLogin != notificationRecipientDb.WsLogin
                        || notificationRecipient.WsPassword != notificationRecipientDb.WsPassword)
                        )
                    {
                        await UpdateRecipientLabeldetails(notificationRecipient);
                    }
                }
                return notificationRecipient;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<bool> UpdateRecipientLabeldetails(NotificationRecipient notificationRecipient)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = @"UPDATE master.notificationrecipient
	                            SET recipient_label=@recipient_label
	                            , account_group_id=@account_group_id
	                            , notification_mode_type=@notification_mode_type
	                            , phone_no=@phone_no
	                            , sms=@sms
	                            , email_id=@email_id
	                            , email_sub=@email_sub
	                            , email_text=@email_text
	                            , ws_url=@ws_url
	                            , ws_type=@ws_type
	                            , ws_text=@ws_text
	                            , ws_login=@ws_login
	                            , ws_password=@ws_password
	                            , modified_at=@modified_at
	                            WHERE id=@id ";
                parameter.Add("@id", notificationRecipient.Id);
                parameter.Add("@recipient_label", notificationRecipient.RecipientLabel);
                parameter.Add("@account_group_id", notificationRecipient.AccountGroupId);
                parameter.Add("@notification_mode_type", notificationRecipient.NotificationModeType);
                parameter.Add("@phone_no", notificationRecipient.PhoneNo);
                parameter.Add("@sms", notificationRecipient.Sms);
                parameter.Add("@email_id", notificationRecipient.EmailId);
                parameter.Add("@email_sub", notificationRecipient.EmailSub);
                parameter.Add("@email_text", notificationRecipient.EmailText);
                parameter.Add("@ws_url", notificationRecipient.WsUrl);
                parameter.Add("@ws_type", notificationRecipient.WsType);
                parameter.Add("@ws_text", notificationRecipient.WsText);
                parameter.Add("@ws_login", notificationRecipient.WsLogin);
                parameter.Add("@ws_password", notificationRecipient.WsPassword);
                parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                int id = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<NotificationLimit> CheckNotificationLimitExists(NotificationLimit notificationLimit)
        {
            try
            {
                var parameter = new DynamicParameters();
                var query = @"SELECT distinct notification_mode_type as NotificationModeType
                                , max_limit as MaxLimit
                                , notification_period_type as NotificationPeriodType
                                , period_limit as PeriodLimit
                                , recipient_id as RecipientId
	                                FROM master.notificationlimit
                                    where state=@state
                                    and recipient_id=@recipient_id";
                parameter.Add("@state", Convert.ToChar(AlertState.Active));
                parameter.Add("@recipient_id", notificationLimit.RecipientId);

                NotificationLimit notificationLimitDb = await _dataAccess.QuerySingleAsync<NotificationLimit>(query, parameter);
                if (notificationLimitDb != null)
                {
                    if (notificationLimit.RecipientId == notificationLimitDb.RecipientId
                        && (notificationLimit.NotificationModeType != notificationLimitDb.NotificationModeType
                        || notificationLimit.MaxLimit != notificationLimitDb.MaxLimit
                        || notificationLimit.NotificationPeriodType != notificationLimitDb.NotificationPeriodType
                        || notificationLimit.PeriodLimit != notificationLimitDb.PeriodLimit)
                        )
                    {
                        await UpdateNotificationLimitdetails(notificationLimit);
                    }
                }
                return notificationLimit;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<bool> UpdateNotificationLimitdetails(NotificationLimit notificationLimit)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = @"UPDATE master.notificationlimit
	                                    SET  
	                                      notification_mode_type=@notification_mode_type
	                                    , max_limit=@max_limit
	                                    , notification_period_type=@notification_period_type
	                                    , period_limit=@period_limit
	                                    , modified_at=@modified_at
	                                    WHERE recipient_id=@recipient_id ";
                //parameter.Add("@notification_id", notificationLimit.NotificationId);
                parameter.Add("@recipient_id", notificationLimit.RecipientId);
                parameter.Add("@notification_mode_type", notificationLimit.NotificationModeType);
                parameter.Add("@max_limit", notificationLimit.MaxLimit);
                parameter.Add("@notification_period_type", notificationLimit.NotificationPeriodType);
                parameter.Add("@period_limit", notificationLimit.PeriodLimit);
                parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                int id = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Landmark Delete Validation
        //landmark type is added only for grouo table as landmark group refers to other table.
        public async Task<bool> IsLandmarkActiveInAlert(List<int> landmarkId, string Landmarktype)
        {
            try
            {
                var parameter = new DynamicParameters();
                var query = string.Empty;
                dynamic responseId;

                if (Landmarktype != null && Landmarktype != string.Empty)
                {
                    query = @"select id from master.alertfilterref where ref_id = any(@ref_id) and state=@state and landmark_type=@type";
                    parameter.Add("@ref_id", landmarkId);
                    parameter.Add("@state", Convert.ToChar(AlertState.Active));
                    parameter.Add("@type", Landmarktype);
                    responseId = await _dataAccess.QueryAsync<dynamic>(query, parameter);
                    if (((System.Collections.Generic.List<object>)responseId).Count == 0)
                    {
                        query = @"select id from master.alertlandmarkref where ref_id = any(@ref_id) and state=@state and landmark_type=@type";
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
                else
                {
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
	                                                    FROM master.notificationrecipientref notiref														
	                                                    inner join master.notification noti
	                                                    on notiref.notification_id=noti.id
	                                                    inner join master.alert alert
	                                                    on notiref.alert_id=alert.id
														inner join master.notificationrecipient notirec
														on notiref.recipient_id=notirec.id
	                                                    where notirec.state=@state
	                                                    and noti.state=@state
	                                                    and alert.state=@state
                                                        and notiref.state=@state
                                                        and alert.organization_id=@organization_id";
                parameter.Add("@organization_id", organizationId);
                parameter.Add("@state", Convert.ToChar(AlertState.Active));
                IEnumerable<NotificationRecipient> notificationRecipientResult = await _dataAccess.QueryAsync<NotificationRecipient>(queryRecipientLabel, parameter);

                foreach (var item in notificationRecipientResult)
                {
                    string queryLimit = @"SELECT id as Id
		                                                    , notification_id as NotificationId
		                                                    , notification_mode_type as NotificationModeType
		                                                    , max_limit as MaxLimit
		                                                    , notification_period_type as NotificationPeriodType
		                                                    , period_limit as PeriodLimit		                                                    
		                                                    , state as State
		                                                    , created_at as CreatedAt
                                                            , recipient_id as RecipientId
	                                                    FROM master.notificationlimit 
	                                                    where state=@state                                                        
                                                        and recipient_id=@recipient_id";
                    parameter.Add("@recipient_id", item.Id);
                    parameter.Add("@state", Convert.ToChar(AlertState.Active));
                    IEnumerable<NotificationLimit> notificationLimitResult = await _dataAccess.QueryAsync<NotificationLimit>(queryLimit, parameter);
                    foreach (var limit in notificationLimitResult)
                    {
                        item.NotificationLimits = new List<NotificationLimit>();
                        item.NotificationLimits.Add(limit);
                    }
                }
                return notificationRecipientResult;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
