
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.dashboard.common;
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

        public async Task<VehiclePositionResponse> GetVehiclePosition(string vin, string since)
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
                            parameter.Add("@timestamp", GetDate(1));
                            queryStatement = string.Format("{0} {1}", queryStatement, "and message_time_stamp = @timestamp");
                            break;
                        case "today"://today
                            parameter.Add("@timestamp", GetDate(0));
                            queryStatement = string.Format("{0} {1}", queryStatement, "and message_time_stamp = @timestamp");
                            break;
                        default:
                            parameter.Add("@millies", since);
                            queryStatement = string.Format("{0} {1}", queryStatement, "and last_odometer_val = @millies");
                            break;
                    }
                }
                var data = await _dataMartDataAccess.QueryAsync<dynamic>(queryStatement, parameter);
                var result = ConvertPositionDynamicToModel(data);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        VehiclePositionResponse ConvertPositionDynamicToModel(IEnumerable<dynamic> data)
        {
            VehiclePositionResponse objVehiclePositionResponse = new VehiclePositionResponse();
            objVehiclePositionResponse.VehiclePosition = new List<VehiclePosition>();
            objVehiclePositionResponse.RequestTimestamp = DateTime.Now.ToUnixMiliSecTime();
            foreach (var item in data)
            {
                VehiclePosition objVehiclePosition = new VehiclePosition();
                objVehiclePosition.VIN = item.vin ?? string.Empty;
                objVehiclePosition.Altitude = item.altitude ?? 0;
                objVehiclePosition.Heading = item.heading ?? 0;
                objVehiclePosition.Latitude = item.latitude ?? 0;
                objVehiclePosition.Longitude = item.longitude ?? 0;
                objVehiclePosition.GPSTimestamp = item.positiondatetime ?? 0;
                objVehiclePosition.Speed = item.speed ?? 0;
                objVehiclePositionResponse.VehiclePosition.Add(objVehiclePosition);
            }
            return objVehiclePositionResponse;
        }

        public async Task<VehicleStatusResponse> GetVehicleStatus(string vin, string since)
        {
            try
            {
                string queryStatement = string.Empty;
                queryStatement = @"SELECT DISTINCT trip_id
                                        ,vin as VIN
                                        ,gps_altitude as altitude
                                        ,gps_heading as heading
                                        ,gps_latitude as latitude
                                        ,gps_longitude as longitude
                                        ,gps_datetime as positiondatetime
                                        ,gps_speed as speed
										,catalyst_fuel_level as CatalystFuelLevel
										,driver1_id as Driver1Id
										,driver1_working_state as Driver1WorkingState
										,total_engine_fuel_used as EngineTotalFuelUsed
										--,event time speed as EventTimestamp
										,fuel_level1 as FuelLevel1
										,gross_combination_vehicle_weight as GrossCombinationVehicleWeight
										,total_vehicle_distance as HRTotalVehicleDistance
										,tachgraph_speed as TachographSpeed
										,total_engine_hours as TotalEngineHours
										,wheelbased_speed as WheelBasedSpeed
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
                            parameter.Add("@timestamp", GetDate(1));
                            queryStatement = string.Format("{0} {1}", queryStatement, "and message_time_stamp = @timestamp");
                            break;
                        case "today"://today
                            parameter.Add("@timestamp", GetDate(0));
                            queryStatement = string.Format("{0} {1}", queryStatement, "and message_time_stamp = @timestamp");
                            break;
                        default:
                            parameter.Add("@millies", since);
                            queryStatement = string.Format("{0} {1}", queryStatement, "and last_odometer_val = @millies");
                            break;
                    }
                }
                var result = ConvertDynamicToStatusModel(await _dataMartDataAccess.QueryAsync<dynamic>(queryStatement, parameter));
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private VehicleStatusResponse ConvertDynamicToStatusModel(IEnumerable<dynamic> data)
        {
            VehicleStatusResponse objVehicleStatusResponse = new VehicleStatusResponse();
            objVehicleStatusResponse.VehicleStatus = new List<VehicleStatus>();
            objVehicleStatusResponse.RequestTimestamp = DateTime.Now.ToUnixMiliSecTime();
            foreach (var item in data)
            {
                VehicleStatus objVehicleStatus = new VehicleStatus();
                objVehicleStatus.VIN = item.vin;
                objVehicleStatus.Driver1Id = item.driver1id;
            }
            return objVehicleStatusResponse;
        }

        public long GetDate(int value)
        {
            DateTime filter = DateTime.Now.AddDays(-value);
            DateTime earlyHr = DateTime.Now.AddHours(-filter.Hour).AddMinutes(-filter.Minute)
                               .AddSeconds(-filter.Second).AddMilliseconds(-filter.Millisecond);
            return earlyHr.ToUnixMiliSecTime();
        }

    }
}