using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using pushnotificationcorecomponent.Entity;

namespace pushnotificationcorecomponent.Repository
{
    public class TripAlertRepository:ITripAlertRepository
    {
        private readonly IDataAccess _dataAccess;
        public TripAlertRepository(IDataAccess dataAccess)
        {
            this._dataAccess = dataAccess;
        }
        public async Task<TripAlert> CreateTripAlert(TripAlert tripAlert)
        {
            try
            {
                if (tripAlert.Alertid > 0) //need to revisit, it only for testing 
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@tripid", tripAlert.Tripid);
                    parameter.Add("@vin", tripAlert.Vin);
                    parameter.Add("@name", tripAlert.Name);
                    parameter.Add("@type", tripAlert.Type);
                    parameter.Add("@categorytype", tripAlert.CategoryType);
                    parameter.Add("@alertid", tripAlert.Alertid);
                    parameter.Add("@thresholdvalue", tripAlert.ThresholdValue);
                    parameter.Add("@thresholdvalueunittype", tripAlert.ThresholdValueUnitType);
                    parameter.Add("@valueatalerttime", tripAlert.ValueAtAlertTime);
                    parameter.Add("@latitude", tripAlert.Latitude);
                    parameter.Add("@longitude", tripAlert.Longitude);
                    parameter.Add("@alertgeneratedtime", tripAlert.AlertGeneratedTime);
                    parameter.Add("@messagetimestamp", tripAlert.MessageTimestamp);
                    parameter.Add("@createdat", tripAlert.CreatedAt);
                    parameter.Add("@modifiedat", tripAlert.ModifiedAt);

                    string query = @"INSERT INTO tripdetail.tripalert(trip_id, vin, category_type, type, name, alert_id, param_filter_distance_threshold_value, param_filter_distance_threshold_value_unit_type, param_filter_distance_value_at_alert_time, latitude, longitude, alert_generated_time, message_timestamp, created_at, modified_at)
                                 VALUES (@tripid,@vin,@categorytype,@type,@name,@alertid,@thresholdvalue,@thresholdvalueunittype,@valueatalerttime,@latitude,@longitude,@alertgeneratedtime,@messagetimestamp,@createdat,@modifiedat) RETURNING id";

                    var id = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                    tripAlert.Id = id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return tripAlert;
        }
    }
}
