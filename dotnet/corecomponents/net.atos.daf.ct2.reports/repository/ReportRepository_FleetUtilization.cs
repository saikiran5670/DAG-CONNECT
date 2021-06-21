using System;
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
        public async Task<List<FleetUtilizationDetails>> GetFleetUtilizationDetails(FleetUtilizationFilter FleetUtilizationFilters)
        {
            try
            {
                var parameterOfFilters = new DynamicParameters();
                parameterOfFilters.Add("@FromDate", FleetUtilizationFilters.StartDateTime);
                parameterOfFilters.Add("@ToDate", FleetUtilizationFilters.EndDateTime);
                parameterOfFilters.Add("@Vins", FleetUtilizationFilters.VIN);
                string queryFleetUtilization = @"WITH CTE_FleetDeatils as
                                            	(
                                            		SELECT
                                            			VIN
                                            		  , count(trip_id)                as numberoftrips
                                            		  , SUM(etl_gps_trip_time)        as etl_gps_trip_time
                                            		  , SUM(end_time_stamp)           as end_time_stamp
                                            		  , SUM(etl_gps_distance)         as etl_gps_distance
                                            		  , SUM(veh_message_driving_time) as veh_message_driving_time
                                            		  , SUM(idle_duration)            as idle_duration
                                            		  , SUM(veh_message_distance)     as veh_message_distance
                                            		  , SUM(average_speed)            as average_speed
                                            		  , SUM(average_weight)           as average_weight
                                            		  , SUM(start_odometer)           as start_odometer
                                            		FROM
                                            			tripdetail.trip_statistics
                                            		where
                                            			start_time_stamp   >= @FromDate
                                            			and end_time_stamp <= @ToDate
                                            		GROUP BY
                                            			VIN
                                            	)
                                              , cte_combine as
                                            	(
                                            		SELECT
                                            			vh.name as vehiclename
                                            		  , vh.vin  as VIN
                                            		  , numberoftrips
                                            		  , vh.registration_no             as RegistrationNumber
                                            		  , fd.etl_gps_trip_time           as TripTime
                                            		  , fd.end_time_stamp              as StopTime
                                            		  , round ( fd.etl_gps_distance,2) as Distance
                                            		  , fd.veh_message_driving_time    as DrivingTime
                                            		  , fd.idle_duration               as IdleTime
                                            		  , fd.veh_message_distance        as AverageDistance
                                            		  , round (fd.average_speed,2)     as AverageSpeed
                                            		  , round (fd.average_weight,2)    as AverageWeight
                                            		  , round (fd.start_odometer,2)    as Odometer
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
        /// <param name="TripFilters"></param>
        /// <returns></returns>
        public async Task<List<Calender_Fleetutilization>> GetCalenderData(FleetUtilizationFilter TripFilters)
        {
            List<Calender_Fleetutilization> List = new List<Calender_Fleetutilization>();

            var parameter = new DynamicParameters();
            parameter.Add("@StartDateTime", TripFilters.StartDateTime);
            parameter.Add("@EndDateTime", TripFilters.EndDateTime);
            parameter.Add("@vins", TripFilters.VIN.ToArray());
            //string vin = string.Join("','", TripFilters.VIN.ToArray());
            //vin = "'"+ vin.Replace(",", "', '")+"'";
            //parameter.Add("@vins", vin);           

            string query = @"WITH cte_workingdays AS(
                        select
                        start_time_stamp as CalenderDate,
                        to_timestamp(start_time_stamp) as startdate,
                        count(start_time_stamp) as totalworkingdays,
                        sum(etl_gps_distance) as totaldistance,
                        sum(etl_gps_trip_time) as totaltriptime,
                        sum(veh_message_driving_time) as totaldrivingtime,
                        sum(idle_duration) as totalidleduration,
                        sum(veh_message_distance) as totalAveragedistanceperday,
                        sum(average_speed) as totalaverageSpeed,
                        sum(average_weight) as totalaverageweightperprip,
                        sum(last_odometer) as totalodometer
                        FROM tripdetail.trip_statistics
                        where (start_time_stamp >= @StartDateTime and end_time_stamp<= @EndDateTime) and vin=ANY(@vins)
                        group by to_timestamp(start_time_stamp),
                        etl_gps_distance,etl_gps_trip_time,veh_message_driving_time
                        ,idle_duration,veh_message_distance,average_speed,average_weight,last_odometer,start_time_stamp
                        )
                        select
                        startdate,
                        CalenderDate,
                        CAST((totaldistance / totalworkingdays) as float) as Averagedistance,
                        CAST((totaltriptime / totalworkingdays) as float) as Averagetriptime ,
                        CAST((totaldrivingtime / totalworkingdays) as float) as Averagedrivingtime ,
                        CAST((totaldistance / totalworkingdays) as float) as averagedistance ,
                        CAST((totalidleduration / totalworkingdays) as float) as Averageidleduration ,
                        CAST((totalAveragedistanceperday / totalworkingdays) as float) as Averagedistanceperday ,
                        CAST((totalaverageSpeed / totalworkingdays) as float) as AverageSpeed ,
                        CAST((totalaverageweightperprip / totalworkingdays) as float) as Averageweightperprip
                        from cte_workingdays";
            List<Calender_Fleetutilization> data = (List<Calender_Fleetutilization>)await _dataMartdataAccess.QueryAsync<Calender_Fleetutilization>(query, parameter);
            return data;
        }
    }
}
