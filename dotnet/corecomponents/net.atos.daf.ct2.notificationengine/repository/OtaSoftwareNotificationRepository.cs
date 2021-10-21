using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.notificationengine.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.notificationengine.repository
{
    public class OtaSoftwareNotificationRepository : IOtaSoftwareNotificationRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartdataAccess;
        public OtaSoftwareNotificationRepository(IDataAccess dataAccess, IDataMartDataAccess dataMartdataAccess)
        {
            _dataMartdataAccess = dataMartdataAccess;
            _dataAccess = dataAccess;

        }
        public async Task<int> InsertTripAlert(TripAlert tripAlert)
        {
            try
            {
                string queryStatement = @"INSERT INTO tripdetail.tripalert(	                                                    
	                                                     trip_id 
	                                                     ,vin 
	                                                     ,category_type
	                                                     ,type
	                                                     ,name
	                                                     ,alert_id
	                                                     ,latitude
	                                                     ,longitude
	                                                     ,alert_generated_time
	                                                     ,processed_message_time_stamp
	                                                     ,created_at
	                                                     ,modified_at
	                                                     ,urgency_level_type
	                                                    )
	                                                    VALUES ( @trip_id
			                                                    , @vin
			                                                    , @category_type
			                                                    , @type			                                                    
			                                                    , @name
			                                                    , @alert_id
			                                                    , @latitude
			                                                    , @longitude                                                               
			                                                    , @alert_generated_time
			                                                    , @processed_message_time_stamp
                                                                , @created_at
                                                                , @modified_at
                                                                , @urgency_level_type)RETURNING id;";
                var parameter = new DynamicParameters();
                parameter.Add("@trip_id", tripAlert.Tripid);
                parameter.Add("@vin", tripAlert.Vin);
                parameter.Add("@category_type", tripAlert.CategoryType);
                parameter.Add("@type", tripAlert.Type);
                parameter.Add("@urgencyleveltype", tripAlert.UrgencyLevelType);
                parameter.Add("@name", tripAlert.Name);
                parameter.Add("@alert_id", tripAlert.Alertid);
                parameter.Add("@alert_generated_time", tripAlert.AlertGeneratedTime);
                parameter.Add("@latitude", tripAlert.Latitude);
                parameter.Add("@longitude", tripAlert.Longitude);
                parameter.Add("@processed_message_time_stamp", tripAlert.MessageTimestamp);
                parameter.Add("@created_at", tripAlert.CreatedAt);
                parameter.Add("@modified_at", tripAlert.ModifiedAt);
                parameter.Add("@urgency_level_type", tripAlert.UrgencyLevelType);
                int tripAlertId = await _dataMartdataAccess.ExecuteScalarAsync<int>(queryStatement, parameter);
                tripAlert.Id = tripAlertId;
                return tripAlert.Id;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<TripAlert> InsertTripAlertOtaConfigParam(TripAlertOtaConfigParam tripAlertOtaConfigParam)
        {
            _dataAccess.Connection.Open();
            var transactionScope = _dataAccess.Connection.BeginTransaction();
            try
            {
                //Nedd to change here for tripid 
                TripAlert tripAlert = new TripAlert();
                tripAlert.Tripid = string.Empty;
                tripAlert.Vin = tripAlertOtaConfigParam.Vin;
                tripAlert.CategoryType = "O";
                tripAlert.Type = "W";
                tripAlert.Name = string.Empty;
                tripAlert.Alertid = 0;
                tripAlert.ThresholdValue = 0;
                tripAlert.ThresholdValueUnitType = string.Empty;
                tripAlert.ValueAtAlertTime = 0;
                tripAlert.Latitude = 0;
                tripAlert.Longitude = 0;
                tripAlert.AlertGeneratedTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                tripAlert.MessageTimestamp = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                tripAlert.CreatedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                tripAlert.UrgencyLevelType = string.Empty;
                tripAlert.Id = await InsertTripAlert(tripAlert);

                if (tripAlert.Id > 0)
                {
                    string queryStatement = @"INSERT INTO tripdetail.tripalertotaconfigparam(	                                                     
	                                                     trip_alert_id
	                                                    , vin
	                                                    , campaign
	                                                    , baseline
	                                                    , status_code
	                                                    , status
	                                                    , campaign_id
	                                                    , subject
	                                                    , time_stamp	                                                    
	                                                    )
	                                                    VALUES ( @trip_alert_id
			                                                    , @vin
			                                                    , @campaign
			                                                    , @baseline
			                                                    , @status_code
			                                                    , @status
			                                                    , @campaign_id
			                                                    , @subject
			                                                    , @time_stamp)RETURNING id;";
                    var parameter = new DynamicParameters();
                    parameter.Add("@trip_alert_id", tripAlert.Id);
                    parameter.Add("@vin", tripAlertOtaConfigParam.Vin);
                    parameter.Add("@campaign", Guid.Parse(tripAlertOtaConfigParam.Campaign));
                    parameter.Add("@baseline", Guid.Parse(tripAlertOtaConfigParam.Baseline));
                    parameter.Add("@status_code", tripAlertOtaConfigParam.StatusCode);
                    parameter.Add("@status", tripAlertOtaConfigParam.Status);
                    parameter.Add("@campaign_id", tripAlertOtaConfigParam.CampaignId);
                    parameter.Add("@subject", tripAlertOtaConfigParam.Subject);
                    parameter.Add("@time_stamp", tripAlertOtaConfigParam.TimeStamp);
                    int tripAlertOtaConfigId = await _dataMartdataAccess.ExecuteScalarAsync<int>(queryStatement, parameter);
                    tripAlertOtaConfigParam.Id = tripAlertOtaConfigId;
                }
                transactionScope.Commit();
                return tripAlert;
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
        }

    }
}
