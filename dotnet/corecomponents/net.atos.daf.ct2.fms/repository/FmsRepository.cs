
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.fms.entity;

namespace net.atos.daf.ct2.fms.repository
{
    public class FmsRepository : IFmsRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartDataAccess;
        private static readonly log4net.ILog _log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public FmsRepository(IDataAccess dataAccess, IDataMartDataAccess dataMartAccess)
        {
            _dataMartDataAccess = dataMartAccess;
            _dataAccess = dataAccess;
        }

        public async Task<List<VehiclePositionResponse>> GetVehiclePosition(string vin, string since)
        {
            try
            {
                string queryStatement = string.Empty;
                queryStatement = @"SELECT DISTINCT trip_id
                                        ,vin
                                        ,gps_altitude as altitude
                                        ,gps_heading as heading
                                        ,gps_latitude as latitude
                                        ,gps_longitude as longitude
                                        ,gps_datetime as positiondatetime
                                        ,gps_speed as speed
                               from livefleet.livefleet_position_statistics
                               WHERE 1=1";
                var parameter = new DynamicParameters();
                if (!string.IsNullOrEmpty(vin))
                {
                    parameter.Add("@vin", vin);
                    queryStatement = string.Format("{0} {1}", queryStatement, "and vin = @vin");
                }
                if (!string.IsNullOrEmpty(since))
                {
                    switch (since.ToLower().Trim())
                    {
                        case "yesterday"://yesterday
                            queryStatement = string.Format("{0} {1}", queryStatement, "and message_time_stamp = @yesterday");
                            break;
                        case "today"://today
                            queryStatement = string.Format("{0} {1}", queryStatement, "and message_time_stamp = @today");
                            break;
                        default:
                            queryStatement = string.Format("{0} {1}", queryStatement, "and last_odometer_val = @millies");
                            break;
                    }
                }
                var result = await _dataMartDataAccess.QueryAsync<VehiclePositionResponse>(queryStatement, parameter);
                return result.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<VehiclePositionResponse>> GetVehicleStatus(string vin, string since)
        {
            try
            {
                string queryStatement = string.Empty;
                queryStatement = @"SELECT DISTINCT trip_id
                                        ,vin
                                        ,gps_altitude as altitude
                                        ,gps_heading as heading
                                        ,gps_latitude as latitude
                                        ,gps_longitude as longitude
                                        ,gps_datetime as positiondatetime
                                        ,gps_speed as speed
										,catalyst_fuel_level
										,driver1_id
										,driver1_working_state
										,total_engine_fuel_used
										--,event time speed
										,fuel_level1
										,gross_combination_vehicle_weight
										,total_vehicle_distance
										,tachgraph_speed
										,total_engine_hours
										,wheelbased_speed
                               from livefleet.livefleet_position_statistics
                               WHERE 1=1";
                var parameter = new DynamicParameters();
                if (!string.IsNullOrEmpty(vin))
                {
                    parameter.Add("@vin", vin);
                    queryStatement = string.Format("{0} {1}", queryStatement, "and vin = @vin");
                }
                if (!string.IsNullOrEmpty(since))
                {
                    switch (since.ToLower().Trim())
                    {
                        case "yesterday"://yesterday
                            queryStatement = string.Format("{0} {1}", queryStatement, "and message_time_stamp = @yesterday");
                            break;
                        case "today"://today
                            queryStatement = string.Format("{0} {1}", queryStatement, "and message_time_stamp = @today");
                            break;
                        default:
                            queryStatement = string.Format("{0} {1}", queryStatement, "and last_odometer_val = @millies");
                            break;
                    }
                }
                var result = await _dataMartDataAccess.QueryAsync<VehiclePositionResponse>(queryStatement, parameter);
                return result.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}