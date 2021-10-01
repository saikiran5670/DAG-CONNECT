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

        public async Task<TripAlertOtaConfigParam> InsertTripAlertOtaConfigParam(TripAlertOtaConfigParam tripAlertOtaConfigParam)
        {
            string queryStatement = @"INSERT INTO tripdetail.tripalertotaconfigparam(
	                                                      id
	                                                    , trip_alert_id
	                                                    , vin
	                                                    , compaign
	                                                    , baseline
	                                                    , status_code
	                                                    , status
	                                                    , compaign_id
	                                                    , subject
	                                                    , time_stamp	                                                    
	                                                    )
	                                                    VALUES (@id
			                                                    , @trip_alert_id
			                                                    , @vin
			                                                    , @compaign
			                                                    , @baseline
			                                                    , @status_code
			                                                    , @status
			                                                    , @compaign_id
			                                                    , @subject
			                                                    , @time_stamp;";
            var parameter = new DynamicParameters();
            parameter.Add("@id", tripAlertOtaConfigParam.Id);
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
            return tripAlertOtaConfigParam;

        }
    }
}
