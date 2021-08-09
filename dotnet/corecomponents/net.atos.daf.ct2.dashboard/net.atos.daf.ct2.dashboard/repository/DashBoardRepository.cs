﻿using System;
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
                                                                 , is_ongoing_trip           as isongoingtrip
                                                                 , SUM(co2_emission)         as co2emission
                                                                 , SUM(etl_gps_distance)     as distance
                                                                 , SUM(etl_gps_driving_time) as drivingtime
                                                                 , SUM(fuel_consumption)     as fuelconsumption
                                                                 , SUM(etl_gps_fuel_consumed) as fuelconsumed
                                                                 , SUM(idling_consumption)   as idlingfuelconsumption
                                                                 , SUM(idle_duration)        as idlingtime
                                                               FROM
                                                                   tripdetail.trip_statistics
                                                               WHERE
                                                                   is_ongoing_trip = false AND (end_time_stamp BETWEEN @FromDate and @ToDate) AND  
                                                        		vin=ANY(@Vins)
                                                        	GROUP BY   vin, is_ongoing_trip 
                                                           )
                                                        SELECT
                                                            isongoingtrip
                                                          , count(vin)			       as vehiclecount
                                                          , Round(sum(co2emission),2)           as co2emission
                                                          , Round(sum(distance),2)              as distance
                                                          , Round(sum(drivingtime),2)           as drivingtime
                                                          , Round(sum(idlingfuelconsumption),2) as idlingfuelconsumption
                                                          , Round(sum(fuelconsumption),2)       as fuelconsumption
                                                          , Round(sum(fuelconsumed),2)          as fuelconsumed
                                                          , Round(sum(idlingtime),2)            as idlingtime
                                                        FROM cte_filteredTrip 
                                                        GROUP BY isongoingtrip";

                List<FleetKpi> lstFleetKpiDetails = (List<FleetKpi>)await _dataMartdataAccess.QueryAsync<FleetKpi>(queryFleetUtilization, parameterOfFilters);

                return lstFleetKpiDetails.FirstOrDefault();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Alert24Hours>> GetLastAlert24Hours(Alert24HoursFilter alert24HoursFilter)
        {
            try
            {
                var parameterOfFilters = new DynamicParameters();
                parameterOfFilters.Add("@Vins", alert24HoursFilter.VINs);
                string queryAlert24Hours = @"select                                       
	                 COUNT(CASE WHEN tra.category_type = 'L' then 1 ELSE NULL END) as Logistic,
                     COUNT(CASE WHEN tra.category_type = 'F' then 1 ELSE NULL END) as FuelAndDriver,
	                 COUNT(CASE WHEN tra.category_type = 'R' then 1 ELSE NULL END) as RepairAndMaintenance,
	                 COUNT(CASE WHEN tra.urgency_level_type = 'A' then 1 ELSE NULL END) as Advisory,
	                 COUNT(CASE WHEN tra.urgency_level_type = 'C' then 1 ELSE NULL END) as Critical,
	                 COUNT(CASE WHEN tra.urgency_level_type = 'W' then 1 ELSE NULL END) as Warning
                from tripdetail.trip_statistics trs
                inner JOIN tripdetail.tripalert tra ON trs.trip_id = tra.trip_id
                where trs.vin = Any(@vins) and
                to_timestamp(tra.alert_generated_time/1000)::date >= (now()::date - 1)";

                List<Alert24Hours> lstAlert = (List<Alert24Hours>)await _dataMartdataAccess.QueryAsync<Alert24Hours>(queryAlert24Hours, parameterOfFilters);
                return lstAlert;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }

        }

        public async Task<TodayLiveVehicleResponse> GetTodayLiveVinData(TodayLiveVehicleRequest objTodayLiveVehicleRequest)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Vins", objTodayLiveVehicleRequest.VINs);
                parameter.Add("@todaydatetime", objTodayLiveVehicleRequest.TodayDateTime);
                parameter.Add("@yesterdaydatetime", objTodayLiveVehicleRequest.YesterdayDateTime);
                parameter.Add("@tomorrowdatetime", objTodayLiveVehicleRequest.TomorrowDateTime);
                parameter.Add("@dayBeforeYesterdaydatetime", objTodayLiveVehicleRequest.DayDeforeYesterdayDateTime);
                string query = @"WITH CTE_Today as
				(
					SELECT 
					lcts.vin as TodayVin ,
					SUM(lcts.trip_distance) as distance, 
					SUM(lcts.driving_time) as drivingtime,
					COUNT(lcts.driver1_id) as Drivercount,
					COUNT(lcts.vin) as TodayActiveVinCount,
					SUM(lcts.driving_time) as TodayTimeBasedUtilizationRate, 
					SUM(lcts.trip_distance) as TodayDistanceBasedUtilization,
					COUNT(ta.urgency_level_type) As criticlealertcount
					FROM livefleet.livefleet_current_trip_statistics lcts
					LEFT JOIN tripdetail.tripalert ta ON lcts.trip_id = ta.trip_id
					WHERE (lcts.start_time_stamp >= @todaydatetime --(today) 
							and lcts.start_time_stamp <= @tomorrowdatetime) --(Tomorrow)
							AND (lcts.end_time_stamp >= @todaydatetime --(today)
							and lcts.end_time_stamp <= @tomorrowdatetime) -- (tomorrow)
							AND lcts.vin = Any(@Vins)
					GROUP BY TodayVin
				)
			  , cte_yesterday as
				(
					SELECT 
						vin as yesterdayVin ,
						COUNT(vin) as YesterdayActiveVinCount ,
						SUM(driving_time) as YesterDayTimeBasedUtilizationRate, 
						SUM(trip_distance) as YesterDayDistanceBasedUtilization
						FROM livefleet.livefleet_current_trip_statistics 
						WHERE (start_time_stamp >= @yesterdaydatetime --(yesterday) 
							and start_time_stamp <= @todaydatetime) --(today)
							AND (end_time_stamp >= @yesterdaydatetime --(yesterday)
							and end_time_stamp <= @todaydatetime) -- (today)
							AND vin = Any(@Vins)
						GROUP BY yesterdayVin                                            	
				)
				,cte_tripstart_yesterday_tripend_today as(
				SELECT 
					position.vin as TodayVin ,
					SUM(position.last_odometer_val) as distance, 
					SUM(position.driving_time) as drivingtime,
					COUNT(position.driver1_id) as Drivercount,
					COUNT(position.vin) as TodayActiveVinCount,
					SUM(position.driving_time) as TodayTimeBasedUtilizationRate, 
					SUM(position.last_odometer_val) as TodayDistanceBasedUtilization,
					COUNT(ta.urgency_level_type) As criticlealertcount
					FROM livefleet.livefleet_current_trip_statistics lcts
					RIGHT JOIN livefleet.livefleet_position_statistics position on lcts.trip_id = position.trip_id
					LEFT JOIN tripdetail.tripalert ta ON lcts.trip_id = ta.trip_id
					WHERE (lcts.start_time_stamp > @yesterdaydatetime --(YESTERDAY)
						      and lcts.start_time_stamp < @todaydatetime --(Today)
						      ) AND lcts.end_time_stamp > @todaydatetime--(Today) 
					        AND position.Veh_Message_Type = 'I'
							AND lcts.vin = Any(@Vins)
					GROUP BY TodayVin
				)
				, cte_tripstart_daybeforeyesterday_tripend_yesterday as(
					SELECT 
						position.vin as yesterdayVin ,
						COUNT(position.vin) as YesterdayActiveVinCount ,
						SUM(position.driving_time) as YesterDayTimeBasedUtilizationRate, 
						SUM(position.last_odometer_val) as YesterDayDistanceBasedUtilization
						FROM livefleet.livefleet_current_trip_statistics lcts
					    LEFT JOIN livefleet.livefleet_position_statistics position on lcts.trip_id = position.trip_id
						WHERE (lcts.start_time_stamp > @dayBeforeYesterdaydatetime --(DaybeforeYESTERDAY 00hr)
						      and lcts.start_time_stamp < @yesterdaydatetime) --(yesterday 00hr)
						      AND lcts.end_time_stamp > @yesterdaydatetime    --(yesterday 00hr) 
							  AND lcts.vin = Any(@Vins)
					          AND position.Veh_Message_Type = 'I'
						GROUP BY yesterdayVin     
				)
			SELECT --t.TodayVin,
			SUM(t.distance)+SUM(tytt.distance) as distance, 
			SUM(t.drivingtime)+SUM(tytt.drivingtime) as drivingtime,
			SUM(t.Drivercount)+SUM(tytt.Drivercount) as Drivercount,
			SUM(t.TodayActiveVinCount)+SUM(tytt.TodayActiveVinCount) as TodayActiveVinCount,
			SUM(t.TodayTimeBasedUtilizationRate)+SUM(tytt.TodayTimeBasedUtilizationRate) as TodayTimeBasedUtilizationRate,
			SUM(t.TodayDistanceBasedUtilization)+SUM(tytt.TodayDistanceBasedUtilization) as TodayDistanceBasedUtilization,
			SUM(t.criticlealertcount)+SUM(tytt.criticlealertcount) As criticlealertcount,
			--y.yesterdayVin,
			SUM(y.YesterdayActiveVinCount)+SUM(tdty.YesterdayActiveVinCount) as YesterdayActiveVinCount,
			SUM(y.YesterDayTimeBasedUtilizationRate)+SUM(tdty.YesterDayTimeBasedUtilizationRate) as YesterDayTimeBasedUtilizationRate,
			SUM(y.YesterDayDistanceBasedUtilization)+SUM(tdty.YesterDayDistanceBasedUtilization)  as YesterDayDistanceBasedUtilization
			FROM
				CTE_Today t
			INNER JOIN	
				cte_yesterday y on t.TodayVin = y.yesterdayVin
			INNER JOIN
			   cte_tripstart_yesterday_tripend_today tytt on t.TodayVin = tytt.TodayVin
			INNER JOIN
			   cte_tripstart_daybeforeyesterday_tripend_yesterday tdty on t.TodayVin = tdty.yesterdayVin";
                var data = await _dataMartdataAccess.QueryAsync<TodayLiveVehicleResponse>(query, parameter);
                return data.FirstOrDefault();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }

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
                        date_trunc('day', to_timestamp(start_time_stamp/1000)) as startdate,
                        count(distinct date_trunc('day', to_timestamp(start_time_stamp/1000))) as totalworkingdays,
						Count(distinct vin) as vehiclecount,
						Count(distinct trip_id) as tripcount,
                        sum(etl_gps_distance) as totaldistance,
                        sum(etl_gps_trip_time) as totaltriptime,
                        sum(etl_gps_driving_time) as totaldrivingtime,
                        sum(idle_duration) as totalidleduration,
                        sum(veh_message_distance) as totalAveragedistanceperday,
                        sum(average_speed) as totalaverageSpeed,
                        sum(average_weight) as totalaverageweightperprip,
                        sum(last_odometer) as totalodometer,
                        SUM(etl_gps_fuel_consumed)    as fuelconsumed,
                        SUM(fuel_consumption)          as fuelconsumption
                        FROM tripdetail.trip_statistics
                        where is_ongoing_trip = false AND (end_time_stamp >= @StartDateTime  and end_time_stamp<= @EndDateTime) 
						and vin=ANY(@vins)
                        group by date_trunc('day', to_timestamp(start_time_stamp/1000))                     
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
