﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.reports.entity;

namespace net.atos.daf.ct2.reports.repository
{
    public partial class ReportRepository : IReportRepository
    {
        /// <summary>
        /// To Fetch utilization details from expected filters
        /// </summary>
        /// <param name="FleetUtilizationFilters"></param>
        /// <returns></returns>
        public async Task<List<FleetUtilizationDetails>> GetFleetUtilizationDetails(FleetUtilizationFilter fleetUtilizationFilters)
        {
            try
            {
                var parameterOfFilters = new DynamicParameters();
                parameterOfFilters.Add("@FromDate", fleetUtilizationFilters.StartDateTime);
                parameterOfFilters.Add("@ToDate", fleetUtilizationFilters.EndDateTime);
                parameterOfFilters.Add("@Vins", fleetUtilizationFilters.VIN);
                string queryFleetUtilization = @"WITH CTE_FleetDeatils as
                                            	(
                                            		  select  ts.VIN
                                                      , count(distinct date_trunc('day', to_timestamp(ts.end_time_stamp/1000))) as totalworkingdays
                                            		  , count(ts.trip_id)                as numberoftrips
                                            		  , SUM(ts.etl_gps_trip_time)        as etl_gps_trip_time
                                            		  , SUM(ts.end_time_stamp)           as end_time_stamp
                                            		  , SUM(ts.etl_gps_distance)         as etl_gps_distance
                                            		  , SUM(ts.etl_gps_driving_time)     as etl_gps_driving_time
                                            		  , SUM(ts.idle_duration)            as idle_duration
                                            		  , SUM(ts.etl_gps_distance)         as veh_message_distance
                                            		  , SUM(ts.average_speed)            as average_speed
                                            		  , SUM(ts.average_gross_weight_comb) as average_weight
                                            		  , Max(ts.last_odometer)           as last_odometer
                                                      , SUM(case when ts.average_gross_weight_comb >0 then 1 else 0 end)  as numoftripswithavgweight
                                            		FROM
                                            			tripdetail.trip_statistics ts
														join master.vehicle VH on TS.vin=VH.vin
                                            		where
                                            			ts.end_time_stamp >= @FromDate
                                            			and ts.end_time_stamp <= @ToDate
                                                        and ts.VIN = ANY(@Vins)
														and ts.end_time_stamp >= VH.reference_date
                                            		GROUP BY
                                            			ts.VIN
                                            	)
                                              , cte_combine as
                                            	(
                                            		SELECT
                                            			vh.name as VehicleName
                                            		  , vh.vin  as VIN
                                            		  , numberoftrips as NumberOfTrips
                                            		  , vh.registration_no             as RegistrationNumber
                                            		  , fd.etl_gps_trip_time           as TripTime
                                            		  , fd.end_time_stamp              as StopTime
                                                      , fd.totalworkingdays            as VehicleActiveDays
                                            		  , round (fd.etl_gps_distance,2)  as Distance
                                            		  , fd.etl_gps_driving_time        as DrivingTime
                                            		  , round(fd.idle_duration,2)      as IdleDuration
                                            		  , round ((fd.veh_message_distance/totalworkingdays),2)   as AverageDistancePerDay
                                            		  , case when fd.etl_gps_trip_time > 0 then round ((fd.etl_gps_distance::numeric)/(fd.etl_gps_trip_time),7) Else 0 END   as AverageSpeed
                                            		  ,case when numoftripswithavgweight>0 then round (fd.average_weight/numoftripswithavgweight, 3) 
													  else round (fd.average_weight,3) end as AverageWeightPerTrip
                                            		  , round (fd.last_odometer,2)    as Odometer
                                            		FROM
                                            			CTE_FleetDeatils fd
                                            			join
                                            				master.vehicle vh
                                            				on
                                            					fd.VIN =vh.VIN
                                            	)
                                            SELECT *
                                            FROM
                                            	cte_combine";

                List<FleetUtilizationDetails> lstFleetDetails = (List<FleetUtilizationDetails>)await _dataMartdataAccess.QueryAsync<FleetUtilizationDetails>(queryFleetUtilization, parameterOfFilters);
                return lstFleetDetails?.Count > 0 ? lstFleetDetails : new List<FleetUtilizationDetails>();

            }
            catch (System.Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Get Calender data for fleet utilization page
        /// </summary>
        /// <param name="tripFilters"></param>
        /// <returns></returns>
        public async Task<List<Calender_Fleetutilization>> GetCalenderData(FleetUtilizationFilter tripFilters)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@StartDateTime", tripFilters.StartDateTime);
                parameter.Add("@EndDateTime", tripFilters.EndDateTime);
                parameter.Add("@vins", tripFilters.VIN.ToArray());
                //string vin = string.Join("','", TripFilters.VIN.ToArray());
                //vin = "'"+ vin.Replace(",", "', '")+"'";
                //parameter.Add("@vins", vin);
                // changing range to end time stamp as per discussion with shailesh smita
                string query;
                if (tripFilters.VIN.Count > 0)
                {
                    query = @"WITH cte_workingdays AS( select
                        date_trunc('day', to_timestamp(ts.end_time_stamp/1000)) as startdate,
                        count(distinct date_trunc('day', to_timestamp(ts.end_time_stamp/1000))) as totalworkingdays,
						Count(distinct ts.vin) as vehiclecount,
						Count(distinct ts.trip_id) as tripcount,
                        sum(ts.etl_gps_distance) as totaldistance,
                        sum(ts.etl_gps_trip_time) as totaltriptime,
                        sum(ts.etl_gps_driving_time) as totaldrivingtime,
                        sum(ts.idle_duration) as totalidleduration,
                        sum(ts.veh_message_distance) as totalAveragedistanceperday,
                        SUM(ts.average_speed) as totalaverageSpeed,
                        sum(ts.average_gross_weight_comb) as totalaverageweightperprip,
                        sum(ts.last_odometer) as totalodometer
                        FROM tripdetail.trip_statistics ts
						join master.vehicle VH on TS.vin=VH.vin
                        where (ts.end_time_stamp >= @StartDateTime  and ts.end_time_stamp<= @EndDateTime) 
						and ts.vin=ANY(@vins)
						and ts.end_time_stamp >= VH.reference_date
                        group by date_trunc('day', to_timestamp(ts.end_time_stamp/1000))                     
                        )
                        select
                        '' as VIN,
                        startdate,
						extract(epoch from startdate) * 1000 as Calenderdate,
                       	totalworkingdays,
						vehiclecount,
                        tripcount,
                        CAST((totaldistance / totalworkingdays) as float) as Averagedistance,
                        CAST((totaltriptime / totalworkingdays) as float) as Averagetriptime ,
                        CAST((totaldrivingtime / totalworkingdays) as float) as Averagedrivingtime ,
                        CAST((totaldistance / totalworkingdays) as float) as averagedistance ,
                        CAST((totalidleduration / totalworkingdays) as float) as Averageidleduration ,
                        CAST((totalAveragedistanceperday / totalworkingdays) as float) as Averagedistanceperday ,
                        CAST((totalaverageSpeed / totalworkingdays) as float) as AverageSpeed ,
                        CAST((totalaverageweightperprip / totalworkingdays) as float) as Averageweight
                        from cte_workingdays";
                }
                else
                {
                    query = query = @"WITH cte_workingdays AS(
                        select
                        date_trunc('day', to_timestamp(ts.end_time_stamp/1000)) as startdate,
                        count(distinct date_trunc('day', to_timestamp(ts.end_time_stamp/1000))) as totalworkingdays,
						Count(distinct ts.vin) as vehiclecount,
						Count(distinct ts.trip_id) as tripcount,
                        sum(ts.etl_gps_distance) as totaldistance,
                        sum(ts.etl_gps_trip_time) as totaltriptime,
                        sum(ts.etl_gps_driving_time) as totaldrivingtime,
                        sum(ts.idle_duration) as totalidleduration,
                        sum(ts.veh_message_distance) as totalAveragedistanceperday,
                        SUM(ts.average_speed) as totalaverageSpeed,
                        sum(ts.average_gross_weight_comb) as totalaverageweightperprip,
                        sum(ts.last_odometer) as totalodometer
                        FROM tripdetail.trip_statistics ts
                        join master.vehicle VH on TS.vin=VH.vin
                        where (ts.end_time_stamp >= @StartDateTime  and ts.end_time_stamp<= @EndDateTime)
                        and ts.end_time_stamp >= VH.reference_date
                        group by date_trunc('day', to_timestamp(ts.end_time_stamp/1000))                     
                        )
                        select
                        '' as VIN,
                        startdate,
						extract(epoch from startdate) * 1000 as Calenderdate,
                       	totalworkingdays,
						vehiclecount,
                        tripcount,
                        CAST((totaldistance / totalworkingdays) as float) as Averagedistance,
                        CAST((totaltriptime / totalworkingdays) as float) as Averagetriptime ,
                        CAST((totaldrivingtime / totalworkingdays) as float) as Averagedrivingtime ,
                        CAST((totaldistance / totalworkingdays) as float) as averagedistance ,
                        CAST((totalidleduration / totalworkingdays) as float) as Averageidleduration ,
                        CAST((totalAveragedistanceperday / totalworkingdays) as float) as Averagedistanceperday ,
                        CAST((totalaverageSpeed / totalworkingdays) as float) as AverageSpeed ,
                        CAST((totalaverageweightperprip / totalworkingdays) as float) as Averageweight
                        from cte_workingdays";
                }

                List<Calender_Fleetutilization> data = (List<Calender_Fleetutilization>)await _dataMartdataAccess.QueryAsync<Calender_Fleetutilization>(query, parameter);
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
