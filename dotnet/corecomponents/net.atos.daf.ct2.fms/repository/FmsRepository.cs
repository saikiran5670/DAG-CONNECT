
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
                                        ,gps_datetime as gpstimestamp
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
                            parameter.Add("@yesterdaytimestamp", GetDate(1));
                            parameter.Add("@timestamp", GetDate(0));
                            queryStatement = string.Format("{0} {1}", queryStatement, "and (received_datetime >= @yesterdaytimestamp and received_datetime < @timestamp)");
                            break;
                        case "today"://today
                            parameter.Add("@timestamp", GetDate(0));
                            queryStatement = string.Format("{0} {1}", queryStatement, "and received_datetime >= @timestamp");
                            break;
                        default:
                            parameter.Add("@millies", Convert.ToInt64(since));
                            queryStatement = string.Format("{0} {1}", queryStatement, "and received_datetime >= @millies");
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

        public async Task<VehiclePositionResponse> GetVehiclePosition(List<string> vin, string since)
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
                                        ,gps_datetime as gpstimestamp
                                        ,gps_speed as speed
                               from livefleet.livefleet_position_statistics
                               WHERE 1=1";
                var parameter = new DynamicParameters();
                if (vin != null && vin.Count > 0)
                {
                    parameter.Add("@vin", vin);
                    queryStatement = string.Format("{0} {1}", queryStatement, "and vin = ANY(@vin)");
                }
                if (!string.IsNullOrEmpty(since))
                {
                    switch (since.ToLower().Trim())
                    {
                        case "yesterday"://yesterday
                            parameter.Add("@yesterdaytimestamp", GetDate(1));
                            parameter.Add("@timestamp", GetDate(0));
                            queryStatement = string.Format("{0} {1}", queryStatement, "and (received_datetime >= @yesterdaytimestamp and received_datetime < @timestamp)");
                            break;
                        case "today"://today
                            parameter.Add("@timestamp", GetDate(0));
                            queryStatement = string.Format("{0} {1}", queryStatement, "and received_datetime >= @timestamp");
                            break;
                        default:
                            parameter.Add("@millies", Convert.ToInt64(since));
                            queryStatement = string.Format("{0} {1}", queryStatement, "and received_datetime >= @millies");
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
                objVehiclePosition.GPSTimestamp = item.gpstimestamp ?? 0;
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
                                        ,gps_datetime as gpstimestamp
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
                            parameter.Add("@yesterdaytimestamp", GetDate(1));
                            parameter.Add("@timestamp", GetDate(0));
                            queryStatement = string.Format("{0} {1}", queryStatement, "and (received_datetime >= @yesterdaytimestamp and received_datetime < @timestamp)");
                            break;
                        case "today"://today
                            parameter.Add("@timestamp", GetDate(0));
                            queryStatement = string.Format("{0} {1}", queryStatement, "and received_datetime >= @timestamp");
                            break;
                        default:
                            parameter.Add("@millies", Convert.ToInt64(since));
                            queryStatement = string.Format("{0} {1}", queryStatement, "and received_datetime >= @millies");
                            break;
                    }
                }
                var data = await _dataMartDataAccess.QueryAsync<dynamic>(queryStatement, parameter);
                var result = ConvertDynamicToStatusModel(data);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<VehicleStatusResponse> GetVehicleStatus(List<string> vin, string since)
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
                                        ,gps_datetime as gpstimestamp
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
                if (vin != null && vin.Count > 0)
                {
                    parameter.Add("@vin", vin);
                    queryStatement = string.Format("{0} {1}", queryStatement, "and vin = ANY(@vin)");
                }
                if (!string.IsNullOrEmpty(since))
                {
                    switch (since.ToLower().Trim())
                    {
                        case "yesterday"://yesterday
                            parameter.Add("@yesterdaytimestamp", GetDate(1));
                            parameter.Add("@timestamp", GetDate(0));
                            queryStatement = string.Format("{0} {1}", queryStatement, "and (received_datetime >= @yesterdaytimestamp and received_datetime < @timestamp)");
                            break;
                        case "today"://today
                            parameter.Add("@timestamp", GetDate(0));
                            queryStatement = string.Format("{0} {1}", queryStatement, "and received_datetime >= @timestamp");
                            break;
                        default:
                            parameter.Add("@millies", Convert.ToInt64(since));
                            queryStatement = string.Format("{0} {1}", queryStatement, "and received_datetime >= @millies");
                            break;
                    }
                }
                var data = await _dataMartDataAccess.QueryAsync<dynamic>(queryStatement, parameter);
                var result = ConvertDynamicToStatusModel(data);
                return result;
            }
            catch (Exception ex)
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
                objVehicleStatus.VIN = item.vin ?? string.Empty;
                objVehicleStatus.Driver1Id = item.driver1id ?? string.Empty;
                objVehicleStatus.CatalystFuelLevel = item.catalystfuellevel ?? 0;
                objVehicleStatus.Driver1WorkingState = item.driver1workingstate != null ? DriverWorkingState.driverWorkingState[item.driver1workingstate] : string.Empty;
                objVehicleStatus.EngineTotalFuelUsed = item.enginetotalfuelused ?? 0;
                objVehicleStatus.EventTimestamp = item.eventtimestamp ?? 0;
                objVehicleStatus.FuelLevel1 = item.fuellevel1 ?? 0;
                objVehicleStatus.TachographSpeed = item.tachographspeed ?? 0; ;
                objVehicleStatus.TotalEngineHours = item.totalenginehours ?? 0;
                objVehicleStatus.HRTotalVehicleDistance = item.hrtotalvehicledistance ?? 0;
                objVehicleStatus.GrossCombinationVehicleWeight = item.grosscombinationvehicleweight ?? 0;
                objVehicleStatus.WheelBasedSpeed = item.wheelbasedspeed ?? 0;
                objVehicleStatus.VehiclePosition = new VehiclePositionForStatus();
                objVehicleStatus.VehiclePosition.Altitude = item.altitude ?? 0;
                objVehicleStatus.VehiclePosition.Heading = item.heading ?? 0;
                objVehicleStatus.VehiclePosition.Latitude = item.latitude ?? 0;
                objVehicleStatus.VehiclePosition.Longitude = item.longitude ?? 0;
                objVehicleStatus.VehiclePosition.GPSTimestamp = item.gpstimestamp ?? 0;
                objVehicleStatus.VehiclePosition.Speed = item.speed ?? 0;
                objVehicleStatusResponse.VehicleStatus.Add(objVehicleStatus);
            }
            return objVehicleStatusResponse;
        }

        public long GetDate(int value)
        {
            DateTime filter = DateTime.Now.AddDays(-value);
            DateTime earlyHr = DateTime.Now.AddDays(-value).AddHours(-filter.Hour).AddMinutes(-filter.Minute)
                               .AddSeconds(-filter.Second).AddMilliseconds(-filter.Millisecond);
            return earlyHr.ToUnixMiliSecTime();
        }

    }
}