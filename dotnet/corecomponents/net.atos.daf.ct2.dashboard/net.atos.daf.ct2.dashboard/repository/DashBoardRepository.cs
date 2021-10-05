using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.dashboard.entity;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.dashboard.repository
{
    public class DashBoardRepository : IDashBoardRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartdataAccess;
        private static readonly log4net.ILog _log =
          log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DashBoardRepository(IDataAccess dataAccess, IDataMartDataAccess dataMartdataAccess)
        {
            _dataAccess = dataAccess;
            _dataMartdataAccess = dataMartdataAccess;
        }

        /// <summary>
        /// To Fetch Fleet KPI data for dashboad using visible VINs
        /// </summary>
        /// <param name="fleetKpiFilter"></param>
        /// <returns></returns>
        public async Task<FleetKpi> GetFleetKPIDetails(FleetKpiFilter fleetKpiFilter)
        {
            try
            {
                var parameterOfFilters = new DynamicParameters();
                parameterOfFilters.Add("@FromDate", fleetKpiFilter.StartDateTime);
                parameterOfFilters.Add("@ToDate", fleetKpiFilter.EndDateTime);
                parameterOfFilters.Add("@Vins", fleetKpiFilter.VINs);
                string queryFleetUtilization = @"WITH cte_filteredTrip as
                                                           (
                                                               SELECT
                                                                   vin
                                                                 , is_ongoing_trip            as isongoingtrip
                                                                 , Round(SUM(co2_emission),2) as co2emission
															     --,SUM(co2_emission)         as co2emission
                                                                 , SUM(etl_gps_distance)      as distance
                                                                 , SUM(etl_gps_driving_time)  as drivingtime
                                                                 , SUM(fuel_consumption)      as fuelconsumption 
                                                                 , SUM(etl_gps_fuel_consumed) as fuelconsumed
                                                                 , SUM(idling_consumption)    as idlingfuelconsumption
                                                                 , SUM(idle_duration)         as idlingtime
                                                               FROM
                                                                   tripdetail.trip_statistics
                                                               WHERE
                                                                   is_ongoing_trip = false 
                                                  AND (end_time_stamp >= @FromDate and end_time_stamp<= @ToDate)
                                                  AND vin=ANY(@Vins)
                                                        	GROUP BY   vin, is_ongoing_trip 
                                                           )
                                                        SELECT
                                                            isongoingtrip
                                                          , count(vin)			                        as vehiclecount
                                                          , sum(co2emission)                            as co2emission
                                                          , Round(SUM(distance),2)                      as distance
                                                          , SUM(drivingtime)                            as drivingtime
                                                          , Round(SUM(idlingfuelconsumption),2)         as idlingfuelconsumption
                                                          , Round((SUM(fuelconsumed)/SUM(distance)),7)  as fuelconsumption
                                                          , Round(SUM(fuelconsumed),7)                  as fuelconsumed
                                                          , Round(SUM(idlingtime),2)                    as idlingtime
                                                        FROM cte_filteredTrip 
                                                        GROUP BY isongoingtrip";

                List<FleetKpi> lstFleetKpiDetails = (List<FleetKpi>)await _dataMartdataAccess.QueryAsync<FleetKpi>(queryFleetUtilization, parameterOfFilters);

                return lstFleetKpiDetails.FirstOrDefault();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public async Task<List<Alert24Hours>> GetLastAlert24Hours(Alert24HoursFilter alert24HoursFilter)
        {
            try
            {
                var parameterOfFilters = new DynamicParameters();
                parameterOfFilters.Add("@Vins", alert24HoursFilter.VINs);
                //          string queryAlert24Hours = @"select                                       
                //            COUNT(CASE WHEN tra.category_type = 'L' then 1 ELSE NULL END) as Logistic,
                //               COUNT(CASE WHEN tra.category_type = 'F' then 1 ELSE NULL END) as FuelAndDriver,
                //            (select COUNT(CASE WHEN ta.category_type = 'R' then 1 ELSE NULL END) as RepairAndMaintenance	                
                //          		from 
                //          		tripdetail.tripalert ta 
                //          		where ta.vin = Any(@vins) and
                //          		to_timestamp(ta.alert_generated_time/1000)::date >= (now()::date - 1)) 
                //as RepairAndMaintenance,
                //            COUNT(CASE WHEN tra.urgency_level_type = 'A' then 1 ELSE NULL END) as Advisory,
                //            COUNT(CASE WHEN tra.urgency_level_type = 'C' then 1 ELSE NULL END) as Critical,
                //            COUNT(CASE WHEN tra.urgency_level_type = 'W' then 1 ELSE NULL END) as Warning
                //          from tripdetail.trip_statistics trs
                //          inner JOIN tripdetail.tripalert tra ON trs.trip_id = tra.trip_id
                //          where trs.vin = Any(@vins) and
                //          to_timestamp(tra.alert_generated_time/1000)::date >= (now()::date - 1)";

                string queryAlert24Hours = @"select                                       
	                 COUNT(CASE WHEN tra.category_type = 'L' then 1 ELSE NULL END) as Logistic,
                     COUNT(CASE WHEN tra.category_type = 'F' then 1 ELSE NULL END) as FuelAndDriver,
	                 COUNT(CASE WHEN tra.category_type = 'R' then 1 ELSE NULL END) as RepairAndMaintenance, 
	                 COUNT(CASE WHEN tra.urgency_level_type = 'A' then 1 ELSE NULL END) as Advisory,
	                 COUNT(CASE WHEN tra.urgency_level_type = 'C' then 1 ELSE NULL END) as Critical,
	                 COUNT(CASE WHEN tra.urgency_level_type = 'W' then 1 ELSE NULL END) as Warning
                from tripdetail.tripalert tra
                where tra.vin = Any(@vins) and
                to_timestamp(tra.alert_generated_time/1000)::date >= (now()::date - 1)";

                List<Alert24Hours> lstAlert = (List<Alert24Hours>)await _dataMartdataAccess.QueryAsync<Alert24Hours>(queryAlert24Hours, parameterOfFilters);
                return lstAlert;
            }
            catch (System.Exception)
            {

                throw;
            }

        }

        #region TodayLive Functionality
        public async Task<List<TodayLiveVehicleData>> GetTodayLiveVinData(TodayLiveVehicleRequest objTodayLiveVehicleRequest)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Vins", objTodayLiveVehicleRequest.VINs);
                parameter.Add("@todaydatetime", objTodayLiveVehicleRequest.TodayDateTime);
                parameter.Add("@yesterdaydatetime", objTodayLiveVehicleRequest.YesterdayDateTime);
                parameter.Add("@tomorrowdatetime", objTodayLiveVehicleRequest.TomorrowDateTime);
                parameter.Add("@dayBeforeYesterdaydatetime", objTodayLiveVehicleRequest.DayDeforeYesterdayDateTime);
                string queryToday = @"WITH cte_filterToday as
                        (
                        SELECT --lcts.trip_id,
					    lcts.vin,
                        (lps.last_odometer_val - (lag(lps.last_odometer_val,1) over (order by lps.vehicle_msg_trigger_type_id asc))) as last_odometer_val,
                        lcts.driving_time,
                        ta.urgency_level_type,
					    lps.vehicle_msg_trigger_type_id
                        FROM livefleet.livefleet_current_trip_statistics lcts
					    LEFT JOIN livefleet.livefleet_position_statistics lps ON lcts.trip_id = lps.trip_id
                        LEFT JOIN tripdetail.tripalert ta ON lcts.trip_id = ta.trip_id
                        WHERE lps.message_time_stamp >= @todaydatetime  --(today 00hr)
			                        AND lcts.vin = Any(@Vins)
							        AND lps.vehicle_msg_trigger_type_id in  (4,5)        
							        AND lps.Veh_Message_Type = 'I' 
                                    AND ta.urgency_level_type = 'C'
                        ), cte_filterTripEndedToday as
                        (
                        SELECT --lcts.trip_id,
                        lps.vin ,
					    (lps.last_odometer_val - (lag(lps.last_odometer_val,1) over (order by vehicle_msg_trigger_type_id asc))) as last_odometer_val, 
                        lps.driving_time,
                        ta.urgency_level_type,
						lps.vehicle_msg_trigger_type_id	
                        FROM livefleet.livefleet_position_statistics lps
                        LEFT JOIN livefleet.livefleet_current_trip_statistics lcts on lcts.trip_id = lps.trip_id
                        LEFT JOIN tripdetail.tripalert ta ON lcts.trip_id = ta.trip_id
                        WHERE lps.message_time_stamp > @yesterdaydatetime --(yesterday 00hr)
   							AND lps.message_time_stamp > @tomorrowdatetime --(Tomorrow 00hr) 
							AND lcts.vin = Any(@Vins)
							AND lps.vehicle_msg_trigger_type_id in (4,5)
                            AND lps.Veh_Message_Type = 'I' 
                            AND ta.urgency_level_type = 'C'
                        --GROUP BY TodayVin--,position.trip_id                                           	
                        ), cte_union as (
           select vin, last_odometer_val as todaydistance, driving_time as todaydrivingtime,urgency_level_type As todayalertcount
				   ,vehicle_msg_trigger_type_id from cte_filterToday 
                   WHERE vehicle_msg_trigger_type_id = 5			
				--GROUP BY vin			
			UNION 
		   select vin, last_odometer_val as todaydistance, driving_time as todaydrivingtime,urgency_level_type As todayalertcount
					,vehicle_msg_trigger_type_id from cte_filterTripEndedToday
			        WHERE vehicle_msg_trigger_type_id = 5
				--GROUP BY vin		
						)
                        SELECT
						--* from cte_union 
                        /*vin
						,todaydistance
						,todaydrivingtime,todayalertcount--trip_id,*/
						vin as TodayVin,
                        SUM(todaydistance) as TodayDistance,
                        SUM(todaydrivingtime) as TodayDrivingTime,
                        COUNT(todayalertcount) as TodayAlertCount 
                        FROM cte_union 
                        GROUP BY vin--,trip_id";
                var dataToday = await _dataMartdataAccess.QueryAsync<TodayLiveVehicleData>(queryToday, parameter);

                return dataToday.ToList();
            }
            catch (System.Exception)
            {
                throw;
            }

        }
        public async Task<List<TodayLiveVehicleData>> GetYesterdayLiveVinData(TodayLiveVehicleRequest objTodayLiveVehicleRequest)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Vins", objTodayLiveVehicleRequest.VINs);
                parameter.Add("@todaydatetime", objTodayLiveVehicleRequest.TodayDateTime);
                parameter.Add("@yesterdaydatetime", objTodayLiveVehicleRequest.YesterdayDateTime);
                parameter.Add("@tomorrowdatetime", objTodayLiveVehicleRequest.TomorrowDateTime);
                parameter.Add("@dayBeforeYesterdaydatetime", objTodayLiveVehicleRequest.DayDeforeYesterdayDateTime);
                string queryYesterday = @"WITH cte_filterYesterday as
                        (
                        SELECT --lcts.trip_id,
                        lcts.vin ,
                        (last_odometer_val - (lag(last_odometer_val,1) over (order by vehicle_msg_trigger_type_id asc))) as last_odometer_val,  
                        lcts.driving_time,
					    lps.vehicle_msg_trigger_type_id
                       FROM livefleet.livefleet_current_trip_statistics lcts
					   LEFT JOIN livefleet.livefleet_position_statistics lps ON lcts.trip_id = lps.trip_id
                        WHERE (lps.message_time_stamp >= @yesterdaydatetime   --(yesterday 00hr) 
	                        and lps.message_time_stamp <= @todaydatetime ) --(today 00hr)
	                        AND lcts.vin = Any(@Vins)
							AND lps.vehicle_msg_trigger_type_id in (4,5) -- trip started and ended
							AND lps.Veh_Message_Type = 'I'
                        ), cte_filtertripendedyesterday as (
                        SELECT --position.trip_id,
                        lps.vin ,
                        lcts.driving_time , 
                        (lps.last_odometer_val - (lag(lps.last_odometer_val,1) over (order by vehicle_msg_trigger_type_id asc))) as last_odometer_val,
						lps.vehicle_msg_trigger_type_id	
                        FROM livefleet.livefleet_position_statistics lps
                        LEFT JOIN livefleet.livefleet_current_trip_statistics lcts on lcts.trip_id = lps.trip_id
                        WHERE (lps.message_time_stamp > @dayBeforeYesterdaydatetime   --(DaybeforeYESTERDAY 00hr)
	                          and lps.message_time_stamp < @todaydatetime) --(today 00hr)
	                          AND lps.vin  = Any(@Vins)
							  AND lps.vehicle_msg_trigger_type_id in (4,5) -- trip started and ended
	                          AND lps.Veh_Message_Type = 'I'
                        ),
						cte_union as(
						select vin, driving_time, last_odometer_val,vehicle_msg_trigger_type_id from cte_filterYesterday 
							WHERE vehicle_msg_trigger_type_id = 5
							UNION
						select vin , driving_time , last_odometer_val,vehicle_msg_trigger_type_id from cte_filtertripendedyesterday 
							WHERE vehicle_msg_trigger_type_id = 5
						)
                        SELECT 
						--* from cte_union
                        vin as YesterdayVin,--trip_id,
                        SUM(last_odometer_val)as YesterdayDistance,
                        SUM(driving_time) as YesterdayDrivingTime
                        FROM cte_union 
                        GROUP BY YesterdayVin--,trip_id";
                var dataYesterday = await _dataMartdataAccess.QueryAsync<TodayLiveVehicleData>(queryYesterday, parameter);
                return dataYesterday.ToList();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        #endregion

        #region Fleet utilization

        public async Task<List<Chart_Fleetutilization>> GetUtilizationchartsData(FleetKpiFilter tripFilters)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@StartDateTime", tripFilters.StartDateTime);
                parameter.Add("@EndDateTime", tripFilters.EndDateTime);
                parameter.Add("@vins", tripFilters.VINs.ToArray());
                //string vin = string.Join("','", TripFilters.VIN.ToArray());
                //vin = "'"+ vin.Replace(",", "', '")+"'";
                //parameter.Add("@vins", vin);
                string query = @"WITH cte_workingdays AS(
                        select
                        date_trunc('day', to_timestamp(end_time_stamp/1000)) as startdate,
                        count(distinct date_trunc('day', to_timestamp(end_time_stamp/1000))) as totalworkingdays,
						Count(distinct vin) as vehiclecount,
						Count(distinct trip_id) as tripcount,
                        sum(etl_gps_distance) as totaldistance,
                        sum(etl_gps_trip_time) as totaltriptime,
                        sum(etl_gps_driving_time) as totaldrivingtime,
                        sum(idle_duration) as totalidleduration,
                        sum(etl_gps_distance) as totalAveragedistanceperday,
                        sum(average_speed) as totalaverageSpeed,
                        sum(average_weight) as totalaverageweightperprip,
                        sum(last_odometer) as totalodometer,
                        SUM(etl_gps_fuel_consumed)    as fuelconsumed,
                        (SUM(etl_gps_fuel_consumed)/SUM(etl_gps_distance))          as fuelconsumption
                        FROM tripdetail.trip_statistics
                        where is_ongoing_trip = false AND (end_time_stamp >= @StartDateTime  and end_time_stamp<= @EndDateTime) 
						and vin=ANY(@vins)
                        group by date_trunc('day', to_timestamp(end_time_stamp/1000))                     
                        )
                        select
                        '' as VIN,
                        startdate,
						extract(epoch from startdate) * 1000 as Calenderdate,
                       	totalworkingdays,
						vehiclecount,
                        tripcount,
                        CAST((totaldistance) as float) as distance,
                        CAST((totaltriptime) as float) as triptime ,
                        CAST((totaldrivingtime) as float) as drivingtime ,
                        CAST((totaldistance) as float) as distance ,
                        CAST((totalidleduration) as float) as idleduration ,
                        CAST((totaldistance) as float) as distanceperday ,
                        CAST((totalaverageSpeed) as float) as Speed ,
                        CAST((totalaverageweightperprip) as float) as weight,
                        fuelconsumed,
                        fuelconsumption
                        from cte_workingdays";


                List<Chart_Fleetutilization> data = (List<Chart_Fleetutilization>)await _dataMartdataAccess.QueryAsync<Chart_Fleetutilization>(query, parameter);
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

    }
}
