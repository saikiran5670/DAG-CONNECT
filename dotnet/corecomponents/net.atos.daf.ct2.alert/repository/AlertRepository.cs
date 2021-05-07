using Dapper;
using net.atos.daf.ct2.alert.entity;
using net.atos.daf.ct2.alert.ENUM;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.alert.repository
{
    public class AlertRepository : IAlertRepository
    {
        private readonly IDataAccess dataAccess;
        public AlertRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;

        }
        #region Create Alert
        public async Task<Alert> CreateAlert(Alert alert)
        {
            int urgencylevelRefId = 0;
            dataAccess.connection.Open();
            var transactionScope = dataAccess.connection.BeginTransaction();
            try
            {
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
                parameterAlert.Add("@state", 'A');
                parameterAlert.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameterAlert.Add("@created_by", alert.CreatedBy);

                string queryAlert = @"INSERT INTO master.alert(organization_id, name, category, type, validity_period_type, validity_start_date, validity_end_date, vehicle_group_id, state, created_at, created_by)
	                                    VALUES (@organization_id, @name, @category, @type, @validity_period_type, @validity_start_date, @validity_end_date, @vehicle_group_id, @state, @created_at, @created_by) RETURNING id";

                var alertId = await dataAccess.ExecuteScalarAsync<int>(queryAlert, parameterAlert);
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
                    urgencylevelRefId = await CreateAlertUrgencyLevelRef(urgencylevel);
                    urgencylevel.Id = urgencylevelRefId;

                    foreach (var alertfilter in urgencylevel.AlertFilterRefs)
                    {
                        alertfilter.AlertId = alertId;
                        alertfilter.AlertUrgencyLevelId = urgencylevelRefId;
                        int alertfilterRefId = await CreateAlertFilterRef(alertfilter);
                        alertfilter.Id = alertfilterRefId;
                    }
                }
                foreach (var notification in alert.Notifications)
                {
                    notification.AlertId = alertId;
                    int notificationId = await CreateNotification(notification);
                    notification.Id = notificationId;
                    foreach (var availabilityPeriod in notification.NotificationAvailabilityPeriods)
                    {
                        availabilityPeriod.NotificationId = notificationId;
                        int alertfilterRefId = await CreateNotificationAvailabilityPeriod(availabilityPeriod);
                        availabilityPeriod.Id = alertfilterRefId;
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
            catch (Exception ex)
            {
                transactionScope.Rollback();
                throw ex;
            }
            finally
            {
                dataAccess.connection.Close();
            }
            return alert;
        }
        private async Task<int> CreateAlertLandmarkRefs(AlertLandmarkRef landmark)
        {
            try
            {
                var parameterlandmarkref = new DynamicParameters();
                parameterlandmarkref.Add("@alert_id", landmark.AlertId);
                parameterlandmarkref.Add("@landmark_type", Convert.ToChar(landmark.LandmarkType));
                parameterlandmarkref.Add("@ref_id", landmark.RefId);
                parameterlandmarkref.Add("@distance", landmark.Distance);
                parameterlandmarkref.Add("@unit_type", Convert.ToChar(landmark.UnitType));
                parameterlandmarkref.Add("@state", 'A');
                parameterlandmarkref.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                string queryLandmarkref = @"INSERT INTO master.alertlandmarkref(alert_id, landmark_type, ref_id, distance, unit_type, state, created_at)
                                                VALUES (@alert_id, @landmark_type, @ref_id, @distance, @unit_type, @state, @created_at) RETURNING id";
                var id = await dataAccess.ExecuteScalarAsync<int>(queryLandmarkref, parameterlandmarkref);
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<int> CreateAlertUrgencyLevelRef(AlertUrgencyLevelRef urgencylevel)
        {
            try
            {
                var parameterurgencylevelref = new DynamicParameters();
                parameterurgencylevelref.Add("@alert_id", urgencylevel.AlertId);
                parameterurgencylevelref.Add("@urgency_level_type", Convert.ToChar(urgencylevel.UrgencyLevelType));
                parameterurgencylevelref.Add("@threshold_value", urgencylevel.ThresholdValue);
                parameterurgencylevelref.Add("@unit_type", Convert.ToChar(urgencylevel.UnitType));
                BitArray bitArray = new BitArray(7);
                for (int i = 0; i < urgencylevel.DayType.Length; i++)
                {
                    bitArray.Set(i,urgencylevel.DayType[i]);
                }
                parameterurgencylevelref.Add("@day_type", bitArray);
                parameterurgencylevelref.Add("@period_type", Convert.ToChar(urgencylevel.PeriodType));
                parameterurgencylevelref.Add("@urgencylevel_start_date", urgencylevel.UrgencylevelStartDate);
                parameterurgencylevelref.Add("@urgencylevel_end_date", urgencylevel.UrgencylevelEndDate);
                parameterurgencylevelref.Add("@state", 'A');
                parameterurgencylevelref.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                string queryUrgencylevel = @"INSERT INTO master.alerturgencylevelref(alert_id, urgency_level_type, threshold_value, unit_type, day_type, period_type, urgencylevel_start_date, urgencylevel_end_date, state, created_at)
	                                                                            VALUES (@alert_id, @urgency_level_type, @threshold_value, @unit_type, @day_type, @period_type, @urgencylevel_start_date, @urgencylevel_end_date, @state, @created_at) RETURNING id";
                int id = await dataAccess.ExecuteScalarAsync<int>(queryUrgencylevel, parameterurgencylevelref);
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<int> CreateAlertFilterRef(AlertFilterRef alertfilter)
        {
            try
            {
                var parameteralertfilterref = new DynamicParameters();
                parameteralertfilterref.Add("@alert_id", alertfilter.AlertId);
                parameteralertfilterref.Add("@alert_urgency_level_id", alertfilter.AlertUrgencyLevelId);
                parameteralertfilterref.Add("@filter_type", Convert.ToChar(alertfilter.FilterType));
                parameteralertfilterref.Add("@threshold_value", alertfilter.ThresholdValue);
                parameteralertfilterref.Add("@unit_type", Convert.ToChar(alertfilter.UnitType));
                parameteralertfilterref.Add("@landmark_type", Convert.ToChar(alertfilter.LandmarkType));
                parameteralertfilterref.Add("@ref_id", alertfilter.RefId);
                parameteralertfilterref.Add("@position_type", Convert.ToChar(alertfilter.PositionType));
                BitArray bitArray = new BitArray(7);
                for (int i = 0; i < alertfilter.DayType.Length; i++)
                {
                    bitArray.Set(i, alertfilter.DayType[i]);
                }
                parameteralertfilterref.Add("@day_type", bitArray);
                parameteralertfilterref.Add("@period_type", Convert.ToChar(alertfilter.PeriodType));
                parameteralertfilterref.Add("@filter_start_date", alertfilter.FilterStartDate);
                parameteralertfilterref.Add("@filter_end_date", alertfilter.FilterEndDate);
                parameteralertfilterref.Add("@state", 'A');
                parameteralertfilterref.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                string queryAlertfilter = @"INSERT INTO master.alertfilterref(alert_id, alert_urgency_level_id, filter_type, threshold_value, unit_type, landmark_type, ref_id, position_type, day_type, period_type, filter_start_date, filter_end_date, state, created_at)
	                                    VALUES (@alert_id, @alert_urgency_level_id, @filter_type, @threshold_value, @unit_type, @landmark_type, @ref_id, @position_type, @day_type, @period_type, @filter_start_date, @filter_end_date, @state, @created_at) RETURNING id";
                int id = await dataAccess.ExecuteScalarAsync<int>(queryAlertfilter, parameteralertfilterref);
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<int> CreateNotification(Notification notification)
        {
            try
            {
                var parameternotification = new DynamicParameters();
                parameternotification.Add("@alert_id", notification.AlertId);
                parameternotification.Add("@alert_urgency_level_type", Convert.ToChar(notification.AlertUrgencyLevelType));
                parameternotification.Add("@frequency_type", Convert.ToChar(notification.FrequencyType));
                parameternotification.Add("@frequency_threshhold_value", notification.FrequencyThreshholdValue);
                parameternotification.Add("@validity_type", Convert.ToChar(notification.ValidityType));
                parameternotification.Add("@state", 'A');
                parameternotification.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameternotification.Add("@created_by", notification.CreatedBy);
                string queryNotification = @"INSERT INTO master.notification(alert_id, alert_urgency_level_type, frequency_type, frequency_threshhold_value, validity_type,state, created_at,created_by)
	                                    VALUES (@alert_id, @alert_urgency_level_type, @frequency_type, @frequency_threshhold_value, @validity_type,@state, @created_at,@created_by) RETURNING id";
                int id = await dataAccess.ExecuteScalarAsync<int>(queryNotification, parameternotification);
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<int> CreateNotificationAvailabilityPeriod(NotificationAvailabilityPeriod availabilityperiod)
        {
            try
            {
                var parameteravailabilityperiod = new DynamicParameters();
                parameteravailabilityperiod.Add("@notification_id", availabilityperiod.NotificationId);
                parameteravailabilityperiod.Add("@availability_period_type", Convert.ToChar(availabilityperiod.AvailabilityPeriodType));
                parameteravailabilityperiod.Add("@period_type", Convert.ToChar(availabilityperiod.PeriodType));
                parameteravailabilityperiod.Add("@start_time", availabilityperiod.StartTime);
                parameteravailabilityperiod.Add("@end_time", availabilityperiod.EndTime);
                parameteravailabilityperiod.Add("@state", 'A');
                parameteravailabilityperiod.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                string queryAvailabilityperiod = @"INSERT INTO master.notificationavailabilityperiod(notification_id, availability_period_type, period_type, start_time, end_time, state, created_at)
	                                    VALUES (@notification_id, @availability_period_type, @period_type, @start_time, @end_time, @state, @created_at) RETURNING id";
                var id = await dataAccess.ExecuteScalarAsync<int>(queryAvailabilityperiod, parameteravailabilityperiod);
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<int> CreateNotificationLimit(NotificationLimit limit)
        {
            try
            {
                var parameterlimit = new DynamicParameters();
                parameterlimit.Add("@notification_id", limit.NotificationId);
                parameterlimit.Add("@notification_mode_type", Convert.ToChar(limit.NotificationModeType));
                parameterlimit.Add("@max_limit", limit.MaxLimit);
                parameterlimit.Add("@notification_period_type", Convert.ToChar(limit.NotificationPeriodType));
                parameterlimit.Add("@period_limit", limit.PeriodLimit);
                parameterlimit.Add("@state", 'A');
                parameterlimit.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                string queryLimit = @"INSERT INTO master.notificationlimit(notification_id, notification_mode_type, max_limit, notification_period_type, period_limit, state, created_at)
	                                                            VALUES (@notification_id, @notification_mode_type, @max_limit, @notification_period_type, @period_limit, @state, @created_at) RETURNING id";
                var id = await dataAccess.ExecuteScalarAsync<int>(queryLimit, parameterlimit);
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
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
                parameterrecipient.Add("@notification_mode_type", Convert.ToChar(recipient.NotificationModeType));
                parameterrecipient.Add("@phone_no", recipient.PhoneNo);
                parameterrecipient.Add("@sms", recipient.Sms);
                parameterrecipient.Add("@email_id", recipient.EmailId);
                parameterrecipient.Add("@email_sub", recipient.EmailSub);
                parameterrecipient.Add("@email_text", recipient.EmailText);
                parameterrecipient.Add("@ws_url", recipient.WsUrl);
                parameterrecipient.Add("@ws_type", Convert.ToChar(recipient.WsType));
                parameterrecipient.Add("@ws_text", recipient.WsText);
                parameterrecipient.Add("@ws_login", recipient.WsLogin);
                parameterrecipient.Add("@ws_password", recipient.WsPassword);
                parameterrecipient.Add("@state", 'A');
                parameterrecipient.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                string queryRecipient = @"INSERT INTO master.notificationrecipient(notification_id, recipient_label, account_group_id, notification_mode_type, phone_no, sms, email_id, email_sub, email_text, ws_url, ws_type, ws_text, ws_login, ws_password, state, created_at)
                                    VALUES (@notification_id, @recipient_label, @account_group_id, @notification_mode_type, @phone_no, @sms, @email_id, @email_sub, @email_text, @ws_url, @ws_type, @ws_text, @ws_login, @ws_password, @state, @created_at) RETURNING id";
                var id = await dataAccess.ExecuteScalarAsync<int>(queryRecipient, parameterrecipient);
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Update Alert

        public async Task<Alert> UpdateAlert(Alert alert)
        {
            int urgencylevelRefId = 0;
            //Begin transaction scope for master.alert table
            dataAccess.connection.Open();
            var transactionScope = dataAccess.connection.BeginTransaction();
            try
            {

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
                int alertId = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                alert.Id = alertId;

                bool IsRefDeleted = await RemoveAlertRef(alert.ModifiedAt, alert.Id,alert.ModifiedBy);
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
                        urgencylevelRefId = await CreateAlertUrgencyLevelRef(urgencylevel);
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
                            foreach (var availabilityPeriod in notification.NotificationAvailabilityPeriods)
                            {
                                availabilityPeriod.NotificationId = notificationId;
                                int alertfilterRefId = await CreateNotificationAvailabilityPeriod(availabilityPeriod);
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
            catch (Exception ex)
            {
                transactionScope.Rollback();
                throw ex;
            }
            finally
            {
                dataAccess.connection.Close();
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
                return await dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
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
                return await dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
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
                return await dataAccess.ExecuteScalarAsync<bool>(query, parameter);

            }
            catch (Exception ex)
            {
                throw ex;
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

                IEnumerable<EnumTranslation> enumtranslationlist = await dataAccess.QueryAsync<EnumTranslation>(QueryStatement, null);

                return enumtranslationlist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Private method

        private async Task<bool> RemoveAlertRef(long modifiedAt, int alertId,int ModifiedBy)
        {
            char deleteChar = 'D';
            char activeState = 'A';
            await dataAccess.ExecuteAsync("UPDATE master.alertfilterref SET state = @state , modified_at = @modified_at WHERE alert_id = @alert_id and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState });
            await dataAccess.ExecuteAsync("UPDATE master.alertlandmarkref SET state = @state , modified_at = @modified_at WHERE alert_id = @alert_id and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState });
            await dataAccess.ExecuteAsync("UPDATE master.alerturgencylevelref SET state = @state , modified_at = @modified_at WHERE alert_id = @alert_id and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState });
            await dataAccess.ExecuteAsync("UPDATE master.notification SET state = @state , modified_at = @modified_at, modified_by=@modified_by WHERE alert_id = @alert_id and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, modified_by= ModifiedBy, alert_id = alertId, activeState = activeState });
            await dataAccess.ExecuteAsync("UPDATE master.notificationavailabilityperiod SET state = @state , modified_at = @modified_at WHERE notification_id in (select id from master.notification where alert_id = @alert_id) and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState });
            await dataAccess.ExecuteAsync("UPDATE master.notificationlimit SET state = @state , modified_at = @modified_at WHERE notification_id in (select id from master.notification where alert_id = @alert_id) and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState });
            await dataAccess.ExecuteAsync("UPDATE master.notificationrecipient SET state = @state , modified_at = @modified_at WHERE notification_id in (select id from master.notification where alert_id = @alert_id) and state=@activeState", new { state = deleteChar, modified_at = modifiedAt, alert_id = alertId, activeState = activeState });
            return true;
        }

        #endregion
    }
}
