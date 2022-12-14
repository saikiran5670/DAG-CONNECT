using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.notificationengine.entity;

namespace net.atos.daf.ct2.notificationengine.repository
{
    public class NotificationIdentifierRepository : INotificationIdentifierRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartdataAccess;
        public NotificationIdentifierRepository(IDataAccess dataAccess, IDataMartDataAccess dataMartdataAccess)
        {
            _dataMartdataAccess = dataMartdataAccess;
            _dataAccess = dataAccess;

        }

        public async Task<TripAlert> GetVehicleIdForTrip(TripAlert tripAlert)
        {
            try
            {
                int vehicleId = await _dataAccess.QuerySingleAsync<int>("select coalesce((SELECT id FROM master.vehicle where vin=@vin), 0)", new { vin = tripAlert.Vin });
                tripAlert.VehicleId = vehicleId;
                return tripAlert;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Notification>> GetNotificationDetails(TripAlert tripAlert)
        {

            string queryStatement = @"select distinct notref.id as Notref_id
	                                    ,notref.notification_id as Noti_id
	                                    ,notref.alert_id as Noti_alert_id
	                                    ,notrec.id as Notrec_id
	                                    ,notrec.recipient_label as Notrec_recipient_label
	                                    ,notrec.notification_mode_type as Notrec_notification_mode_type
	                                    ,notrec.sms as Notrec_sms
	                                    ,notrec.phone_no as Notrec_phone_no
	                                    ,notrec.email_id as Notrec_email_id
	                                    ,notrec.email_sub as Notrec_email_sub
	                                    ,notrec.email_text as Notrec_email_text
	                                    ,notrec.ws_url as Notrec_ws_url
	                                    ,notrec.ws_type as Notrec_ws_type
	                                    ,notrec.ws_text as Notrec_ws_text
	                                    ,notrec.ws_login as Notrec_ws_login
	                                    ,notrec.ws_password as Notrec_ws_password
	                                    ,notrec.state as Notrec_state
	                                    ,noti.id as Noti_id
	                                    ,noti.frequency_type as Noti_frequency_type
	                                    ,noti.frequency_threshhold_value as Noti_frequency_threshhold_value
	                                    ,noti.validity_type as Noti_validity_type
	                                    ,noti.state as Noti_state
	                                    ,notlim.notification_mode_type as Notlim_notification_mode_type
	                                    ,notlim.max_limit as Notlim_max_limit
	                                    ,notlim.notification_period_type as Notlim_notification_period_type
	                                    ,notlim.period_limit as Notlim_period_limit
	                                    ,notlim.state as Notlim_state
	                                    ,nottim.id as Aletimenoti_id
	                                    ,nottim.ref_id as Aletimenoti_ref_id
	                                    ,nottim.type as Aletimenoti_type
	                                    ,nottim.day_type as Aletimenoti_day_type
	                                    ,nottim.period_type as Aletimenoti_period_type
	                                    ,nottim.start_date as Aletimenoti_start_date
	                                    ,nottim.end_date as Aletimenoti_end_date
	                                    ,nottim.state as Aletimenoti_state
                                        ,ale.organization_id as Ale_organization_id
                                        ,ale.name as Ale_name
	                                    ,vgrp.name as Vehiclegroupname
										,vgrp.group_type as Vehiclegroup
										, case when vgrp.group_type ='S' then 
											(select name from master.vehicle where id=vgrp.ref_id)
											else vgrp.name
											end Vehicle_group_vehicle_name
                                    from master.notificationrecipientref notref
                                    inner join master.notificationrecipient notrec
                                    on notref.recipient_id=notrec.id and notrec.state=@state and notref.state=@state
                                    inner join master.alert ale
                                    on notref.alert_id=ale.id
                                    inner join master.notification noti
                                    on notref.notification_id=noti.id  and noti.state=@state
                                    inner join master.notificationlimit notlim
                                    on notrec.id=notlim.recipient_id and notlim.state=@state
                                    left join master.alerttimingdetail nottim
                                    on noti.id=nottim.ref_id and nottim.type=@adFilterType and nottim.state=@state
                                    inner join master.group vgrp
									on vgrp.id=ale.vehicle_group_id
                                    where notref.alert_id=@alert_id";
            var parameter = new DynamicParameters();
            parameter.Add("@alert_id", tripAlert.Alertid);
            parameter.Add("@adFilterType", 'N');
            parameter.Add("@state", 'A');

            List<Notification> notificationdetailsOutput = (List<Notification>)await _dataAccess.QueryAsync<Notification>(queryStatement, parameter);

            foreach (var item in notificationdetailsOutput)
            {
                item.Vehiclename = await _dataAccess.QuerySingleOrDefaultAsync<string>("select name from master.vehicle where vin =@vin", new { vin = tripAlert.Vin });
            }
            return notificationdetailsOutput;
        }

        public async Task<List<NotificationHistory>> GetNotificationHistory(TripAlert tripAlert)
        {
            StringBuilder queryStatement = new StringBuilder();
            queryStatement.Append(@" SELECT id as Id
                                            , organization_id as OrganizationId
                                            , trip_id as TripId
                                            , vehicle_id as VehicleId
                                            , alert_id as AlertId
                                            , notification_id as NotificationId
                                            , recipient_id as RecipientId
                                            , notification_mode_type as NotificationModeType
                                            , phone_no as PhoneNo
                                            , email_id as EmailId
                                            , ws_url as WsUrl
                                            , notification_sent_date as NotificationSendDate
                                            , status as Status
	                                            FROM master.notificationhistory 	                                           
	                                            where alert_id=@alert_id
                                                    and vehicle_id=@vehicle_id                                                    
                                                    and status<>@status");

            var parameter = new DynamicParameters();
            parameter.Add("@alert_id", tripAlert.Alertid);
            parameter.Add("@vehicle_id", tripAlert.VehicleId);
            parameter.Add("@status", ((char)NotificationSendType.Failed).ToString());
            if (tripAlert.Type != null)
            {
                //await _dataAccess.QuerySingleOrDefaultAsync<string>("select key from translation.enumtranslation where type=@type and enum=@categoryEnum", new { type = 'C', categoryEnum = item.CategoryType });
                if (tripAlert.Type.IndexOfAny(new char[] { 'N', 'X', 'C', 'Y', 'D', 'G', 'S', 'U', 'A', 'H', 'I', 'O', 'T', 'L', 'P', 'F' }) >= 0)
                {
                    queryStatement.Append(" and trip_id = @trip_id");
                    parameter.Add("@trip_id", tripAlert.Tripid);
                }
            }

            List<NotificationHistory> notificationHistoryOutput = (List<NotificationHistory>)await _dataAccess.QueryAsync<NotificationHistory>(queryStatement.ToString(), parameter);
            return notificationHistoryOutput;
        }

        public async Task<List<TripAlert>> GetGeneratedTripAlert(TripAlert tripAlert)
        {
            string queryStatement = @" SELECT triale.id as Id
                                            , triale.trip_id as Tripid
                                            , triale.vin as Vin
                                            , triale.category_type as CategoryType
                                            , triale.type as Type
                                            , triale.name as Name
                                            , triale.alert_id as Alertid
                                            , triale.latitude as Latitude 
                                            , triale.longitude as Longitude
                                            , triale.alert_generated_time as AlertGeneratedTime
                                            , triale.processed_message_time_stamp as MessageTimestamp
                                            , triale.created_at as CreatedAt
                                            , triale.modified_at as ModifiedAt
                                            , triale.urgency_level_type as UrgencyLevelType
                                            , triday.from_date_threshold_value as FromDateThresholdValue
                                            , triday.to_date_threshold_value as ToDateThresholdValue
                                            , triday.timeperiod_type as TimePeriodType
                                            , triday.day_threshold_value as DayThresholdValue
                                            , triday.from_daytime_threshold_value as FromDayTimeThresholdValue
                                            , triday.to_daytime_threshold_value as ToDayTimeThresholdValue
                                            , triday.date_breached_value as DateBreachedValue
                                            , triday.day_breached_value as DayBreachedValue
                                            , triday.daytime_breached_value as DayTimeBreachedValue
                                            , trigen.param_type as ParamType
                                            , trigen.threshold_value as TrigenThresholdValue
                                            , trigen.threshold_value_unit_type as TrigenThresholdValueUnitType
                                            , trigen.breached_value as BreachedValue
                                            , trilan.landmark_type as LandmarkType
                                            , trilan.landmark_id as LandmarkId
                                            , trilan.landmark_name as LandmarkName
                                            , trilan.landmark_position_type as LandmarkPositionType
                                            , trilan.landmark_threshold_value as LandmarkThresholdValue
                                            , trilan.landmark_threshold_value_unit_type as LandmarkThresholdValueUnitType
                                            , trilan.landmark_breached_value as LandmarkBreachedValue
	                                            FROM tripdetail.tripalert triale
	                                            left join tripdetail.tripalertdaytimeconfigparam triday
	                                            on triale.alert_id=triday.trip_alert_id
	                                            left join tripdetail.tripalertgenconfigparam trigen
	                                            on triale.alert_id=trigen.trip_alert_id
	                                            left join tripdetail.tripalertlandmarkconfigparam trilan
	                                            on triale.alert_id=trilan.trip_alert_id
	                                            where triale.alert_id = @alert_id
	                                            and triale.vin= @vin";
            var parameter = new DynamicParameters();
            parameter.Add("@alert_id", tripAlert.Alertid);
            parameter.Add("@vin", tripAlert.Vin);
            List<TripAlert> generatedAlertOutput = (List<TripAlert>)await _dataMartdataAccess.QueryAsync<TripAlert>(queryStatement, parameter);
            foreach (var item in generatedAlertOutput)
            {
                item.AlertCategoryKey = await _dataAccess.QuerySingleOrDefaultAsync<string>("select key from translation.enumtranslation where type=@type and enum=@categoryEnum", new { type = 'C', categoryEnum = item.CategoryType });
                item.AlertTypeKey = await _dataAccess.QuerySingleOrDefaultAsync<string>("select key from translation.enumtranslation where parent_enum=@parentEnum and enum=@typeEnum", new { parentEnum = item.CategoryType, typeEnum = item.Type });
                item.UrgencyTypeKey = await _dataAccess.QuerySingleOrDefaultAsync<string>("select key from translation.enumtranslation where type =@type and enum=@urgencyEnum", new { type = 'U', urgencyEnum = item.UrgencyLevelType });
            }
            return generatedAlertOutput;
        }

        public async Task<NotificationHistory> InsertNotificationSentHistory(NotificationHistory notificationHistory)
        {
            string queryStatement = @"INSERT INTO master.notificationhistory(
	                                                      organization_id
	                                                    , trip_id
	                                                    , vehicle_id
	                                                    , alert_id
	                                                    , notification_id
	                                                    , recipient_id
	                                                    , notification_mode_type
	                                                    , phone_no
	                                                    , email_id
	                                                    , ws_url
	                                                    , notification_sent_date
	                                                    , status)
	                                                    VALUES (@organization_id
			                                                    , @trip_id
			                                                    , @vehicle_id
			                                                    , @alert_id
			                                                    , @notification_id
			                                                    , @recipient_id
			                                                    , @notification_mode_type
			                                                    , @phone_no
			                                                    , @email_id
			                                                    , @ws_url
			                                                    , @notification_sent_date
			                                                    , @status) RETURNING id;";
            var parameter = new DynamicParameters();
            parameter.Add("@organization_id", notificationHistory.OrganizationId);
            parameter.Add("@trip_id", notificationHistory.TripId);
            parameter.Add("@vehicle_id", notificationHistory.VehicleId);
            parameter.Add("@alert_id", notificationHistory.AlertId);
            parameter.Add("@notification_id", notificationHistory.NotificationId);
            parameter.Add("@recipient_id", notificationHistory.RecipientId);
            parameter.Add("@notification_mode_type", notificationHistory.NotificationModeType);
            parameter.Add("@phone_no", notificationHistory.PhoneNo);
            parameter.Add("@email_id", notificationHistory.EmailId);
            parameter.Add("@ws_url", notificationHistory.WsUrl);
            parameter.Add("@notification_sent_date", notificationHistory.NotificationSendDate);
            parameter.Add("@status", notificationHistory.Status);
            int notificationSentId = await _dataAccess.ExecuteScalarAsync<int>(queryStatement, parameter);
            notificationHistory.Id = notificationSentId;
            return notificationHistory;

        }

        public async Task<string> GetTranslateValue(string languageCode, string key)
        {
            try
            {
                string translateValue = await _dataAccess.QuerySingleAsync<string>("Select coalesce((select t.value from translation.translation as t where t.name =@key and t.code=@laguageCode), (select t.value from translation.translation as t where t.name =@key and t.code='EN-GB')) as Key", new { key = key, laguageCode = languageCode });

                return translateValue;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> GetLanguageCodePreference(string emailId)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@emailId", emailId.ToLower());

                string accountQuery =
                    @"SELECT preference_id from master.account where lower(email) = @emailId";

                var accountPreferenceId = await _dataAccess.QueryFirstOrDefaultAsync<int?>(accountQuery, parameter);

                if (!accountPreferenceId.HasValue)
                {
                    string orgQuery = string.Empty;
                    int? orgPreferenceId = null;

                    orgQuery =
                        @"SELECT o.preference_id from master.account acc
                            INNER JOIN master.accountOrg ao ON acc.id=ao.account_id
                            INNER JOIN master.organization o ON ao.organization_id=o.id
                            where lower(acc.email) = @emailId";

                    orgPreferenceId = await _dataAccess.QueryFirstOrDefaultAsync<int?>(orgQuery, parameter);

                    if (!orgPreferenceId.HasValue)
                        return "EN-GB";
                    else
                        return await GetCodeByPreferenceId(orgPreferenceId.Value);
                }
                return await GetCodeByPreferenceId(accountPreferenceId.Value);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> GetCodeByPreferenceId(int preferenceId)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@preferenceId", preferenceId);

                string query =
                    @"SELECT l.code from master.accountpreference ap
                    INNER JOIN translation.language l ON ap.id = @preferenceId AND ap.language_id=l.id";

                var languageCode = await _dataAccess.QueryFirstAsync<string>(query, parameter);

                return languageCode;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> GetUnitType(int alertId, string urgencyLevelType)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@alert_id", alertId);
                parameter.Add("@urgency_level_type", urgencyLevelType);
                parameter.Add("@state", 'A');

                string query =
                    @"select unit_type from master.alerturgencylevelref 
                          where urgency_level_type=@urgency_level_type and alert_id=@alert_id and state=@state";

                string unitType = await _dataAccess.QueryFirstOrDefaultAsync<string>(query, parameter);

                return unitType;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AlertVehicleEntity> GetEligibleAccountForAlert(AlertMessageEntity alertMessageEntity)
        {
            try
            {
                var parameter = new DynamicParameters();
                AlertVehicleEntity alertVehicledetails = new AlertVehicleEntity();
                if (alertMessageEntity.AlertId > 0)
                {
                    parameter.Add("@alert_id", alertMessageEntity.AlertId);
                    string query =
                        @"select case when grp.group_type = 'S' then 0 else grp.id end  VehicleGroupId,
	                         case when grp.group_type = 'S' then '' else grp.name end VehicleGroupName,
                             ale.created_by as AlertCreatedAccountId,
                             ale.organization_id as OrganizationId
                                from master.alert ale
                                inner join master.group grp
                                on ale.vehicle_group_id=grp.id where ale.id=@alert_id";
                    alertVehicledetails = await _dataAccess.QueryFirstOrDefaultAsync<AlertVehicleEntity>(query, parameter);
                }

                string alertVehicleQuery = @"select name as VehicleName,license_plate_number as VehicleRegNo from master.vehicle where vin =@vin";
                parameter.Add("@vin", alertMessageEntity.Vin);
                AlertVehicleEntity alertVeh = await _dataAccess.QueryFirstOrDefaultAsync<AlertVehicleEntity>(alertVehicleQuery, parameter);
                alertVehicledetails.VehicleName = alertVeh?.VehicleName ?? string.Empty;
                alertVehicledetails.VehicleRegNo = alertVeh?.VehicleRegNo ?? string.Empty;
                alertVehicledetails.AlertCategoryKey = await _dataAccess.QuerySingleOrDefaultAsync<string>("select key from translation.enumtranslation where type=@type and enum=@categoryEnum", new { type = 'C', categoryEnum = alertMessageEntity.AlertCategory });
                alertVehicledetails.AlertTypeKey = await _dataAccess.QuerySingleOrDefaultAsync<string>("select key from translation.enumtranslation where parent_enum=@parentEnum and enum=@typeEnum", new { parentEnum = alertMessageEntity.AlertCategory, typeEnum = alertMessageEntity.AlertType });
                alertVehicledetails.UrgencyTypeKey = await _dataAccess.QuerySingleOrDefaultAsync<string>("select key from translation.enumtranslation where type =@type and enum=@urgencyEnum", new { type = 'U', urgencyEnum = alertMessageEntity.AlertUrgency });

                return alertVehicledetails;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<string>> GetFeatureEnumForAlert(List<int> featureIds)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@featureIds", featureIds);
                var queryStatementFeature = @"select enum from translation.enumtranslation where feature_id = ANY(@featureIds)";
                List<string> resultFeaturEnum = (List<string>)await _dataAccess.QueryAsync<string>(queryStatementFeature, parameter);
                return resultFeaturEnum;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
