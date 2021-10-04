using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.notificationengine.entity;

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
        public async Task<TripAlert> InsertTripAlert(TripAlert tripAlert)
        {
            string queryStatement = @"INSERT INTO tripdetail.tripalert(	                                                    
	                                                     trip_id
	                                                    , vin
	                                                    , categorytype
	                                                    , type
	                                                    , urgencyleveltype
	                                                    , name
	                                                    , alertid
	                                                    , alertgeneratedtime
	                                                    , lattitude
                                                        , longitude
                                                        , processmessagetime
                                                        , createdat
                                                        , modifiedat
	                                                    )
	                                                    VALUES ( @trip_id
			                                                    , @vin
			                                                    , @categorytype
			                                                    , @type
			                                                    , @urgencyleveltype
			                                                    , @name
			                                                    , @alertid
			                                                    , @alertgeneratedtime
			                                                    , @lattitude
                                                                , @longitude
			                                                    , @processmessagetime
			                                                    , @createdat
                                                                , @modifiedat;";
            var parameter = new DynamicParameters();
            parameter.Add("@trip_id", tripAlert.Tripid);
            parameter.Add("@vin", tripAlert.Vin);
            parameter.Add("@categorytype", 'L');
            parameter.Add("@type", 'W');
            parameter.Add("@urgencyleveltype", tripAlert.UrgencyLevelType);
            parameter.Add("@name", tripAlert.Name);
            parameter.Add("@alertid", tripAlert.Alertid);
            parameter.Add("@alertgeneratedtime", tripAlert.AlertGeneratedTime);
            parameter.Add("@lattitude", tripAlert.Latitude);
            parameter.Add("@longitude", tripAlert.Longitude);
            parameter.Add("@processmessagetime", tripAlert.MessageTimestamp);
            parameter.Add("@createdat", tripAlert.CreatedAt);
            parameter.Add("@modifiedat", tripAlert.ModifiedAt);
            int tripAlertSentId = await _dataMartdataAccess.ExecuteScalarAsync<int>(queryStatement, parameter);
            tripAlert.Id = tripAlertSentId;
            return tripAlert;

        }
        public async Task<TripAlertOtaConfigParam> InsertTripAlertOtaConfigParam(TripAlertOtaConfigParam tripAlertOtaConfigParam)
        {
            //Nedd to change here for tripid 
            TripAlert tripAlert = new TripAlert();
            await InsertTripAlert(tripAlert);

            if (tripAlert.Tripid != null)
            {
                string queryStatement = @"INSERT INTO tripdetail.tripalertotaconfigparam(	                                                     
	                                                     trip_alert_id
	                                                    , vin
	                                                    , compaign
	                                                    , baseline
	                                                    , status_code
	                                                    , status
	                                                    , compaign_id
	                                                    , subject
	                                                    , time_stamp	                                                    
	                                                    )
	                                                    VALUES ( @trip_alert_id
			                                                    , @vin
			                                                    , @compaign
			                                                    , @baseline
			                                                    , @status_code
			                                                    , @status
			                                                    , @compaign_id
			                                                    , @subject
			                                                    , @time_stamp;";
                var parameter = new DynamicParameters();
                parameter.Add("@trip_alert_id", tripAlertOtaConfigParam.TripAlertId);
                parameter.Add("@vin", tripAlertOtaConfigParam.Vin);
                parameter.Add("@compaign", tripAlertOtaConfigParam.Compaign);
                parameter.Add("@baseline", tripAlertOtaConfigParam.Baseline);
                parameter.Add("@status_code", tripAlertOtaConfigParam.StatusCode);
                parameter.Add("@status", tripAlertOtaConfigParam.Status);
                parameter.Add("@compaign_id", tripAlertOtaConfigParam.ComapignId);
                parameter.Add("@subject", tripAlertOtaConfigParam.Subject);
                parameter.Add("@time_stamp", tripAlertOtaConfigParam.TimeStamp);
                int tripAlertOtaConfigSentId = await _dataMartdataAccess.ExecuteScalarAsync<int>(queryStatement, parameter);
                tripAlertOtaConfigParam.Id = tripAlertOtaConfigSentId;
            }
            return tripAlertOtaConfigParam;
        }

    }
}
