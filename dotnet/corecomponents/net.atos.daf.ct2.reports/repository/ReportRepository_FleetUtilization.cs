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
    }
}
