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
            catch (Exception ex)
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
                                    where notref.alert_id=@alert_id";
            var parameter = new DynamicParameters();
            parameter.Add("@alert_id", tripAlert.Alertid);
            parameter.Add("@adFilterType", 'N');
            parameter.Add("@state", 'A');

            List<Notification> notificationdetailsOutput = (List<Notification>)await _dataAccess.QueryAsync<Notification>(queryStatement, parameter);
            return notificationdetailsOutput;
        }

        public async Task<List<NotificationHistory>> GetNotificationHistory(TripAlert tripAlert)
        {
            StringBuilder queryStatement = new StringBuilder();
            queryStatement.Append(@"SELECT id as Id
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
            if ((tripAlert.Type.IndexOfAny(new char[] { 'N', 'X', 'c', 'Y', 'D','G' }) >= 0))
            {
                queryStatement.Append(" and trip_id = @trip_id");
                parameter.Add("@trip_id", tripAlert.Tripid);
            }

            List<NotificationHistory> notificationHistoryOutput = (List<NotificationHistory>)await _dataAccess.QueryAsync<NotificationHistory>(queryStatement.ToString(), parameter);
            return notificationHistoryOutput;
        }

        public async Task<List<TripAlert>> GetGeneratedTripAlert(TripAlert tripAlert)
        {
            string queryStatement = @"SELECT SELECT triale.id as Id
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
    }
}
