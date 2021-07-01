using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reports.entity.fleetFuel;

namespace net.atos.daf.ct2.reports.repository
{
    public partial class ReportRepository : IReportRepository
    {
        #region Master Data operations
        /// <summary>
        /// Get CO2 Co-Efficient data from master
        /// </summary>
        /// <returns>Data in Fule_type and Coefficient fields </returns>
        public async Task<List<CO2CoEfficient>> GetCO2CoEfficientData()
        {
            try
            {
                string queryMasterData = @"select id, Description, Fuel_type, Coefficient from master.co2coefficient";

                List<CO2CoEfficient> lstCO2CoEfficient = (List<CO2CoEfficient>)await _dataAccess.QueryAsync<CO2CoEfficient>(queryMasterData);
                return lstCO2CoEfficient?.Count > 0 ? lstCO2CoEfficient : new List<CO2CoEfficient>();
            }
            catch (System.Exception) { throw; }
        }
        /// <summary>
        /// Get Idling Consumption data from master
        /// </summary>
        /// <returns>Data in MinValue, MaxValue and Key fields </returns>
        public async Task<List<IdlingConsumption>> GetIdlingConsumptionData()
        {
            try
            {
                string queryMasterData = @"SELECT id, min_val, max_val, Key FROM master.idlingconsumption";

                List<IdlingConsumption> lstIdlingConsumption = (List<IdlingConsumption>)await _dataAccess.QueryAsync<IdlingConsumption>(queryMasterData);
                return lstIdlingConsumption?.Count > 0 ? lstIdlingConsumption : new List<IdlingConsumption>();
            }
            catch (System.Exception) { throw; }
        }
        /// <summary>
        /// Get Average Traffic Classification data from master
        /// </summary>
        /// <returns>Data in MinValue, MaxValue and Key fields </returns>
        public async Task<List<AverageTrafficClassification>> GetAverageTrafficClassificationData()
        {
            try
            {
                string queryMasterData = @"SELECT id, min_val, max_val, Key FROM master.averagetrafficclassification";

                List<AverageTrafficClassification> lstAverageTrafficClassification = (List<AverageTrafficClassification>)await _dataAccess.QueryAsync<AverageTrafficClassification>(queryMasterData);
                return lstAverageTrafficClassification?.Count > 0 ? lstAverageTrafficClassification : new List<AverageTrafficClassification>();
            }
            catch (System.Exception) { throw; }
        }
        #endregion

        /// <summary>
        /// To Fetch Fuel details by Vechicle
        /// </summary>
        /// <param name="fleetFuelFilters"></param>
        /// <returns></returns>
        public async Task<List<FleetFuelDetails>> GetFleetFuelDetailsByVehicle(FleetFuelFilter fleetFuelFilters)
        {
            try
            {
                var parameterOfFilters = new DynamicParameters();
                parameterOfFilters.Add("@FromDate", fleetFuelFilters.StartDateTime);
                parameterOfFilters.Add("@ToDate", fleetFuelFilters.EndDateTime);
                parameterOfFilters.Add("@Vins", fleetFuelFilters.VIN);
                string queryFleetUtilization = @"WITH CTE_FleetDeatils as
                                                (
                                                    SELECT
                                                        VIN
                                                      , count(distinct date_trunc('day', to_timestamp(start_time_stamp/1000))) as totalworkingdays
                                                      , count(trip_id)                as numberoftrips
                                                      , SUM(etl_gps_trip_time)        as etl_gps_trip_time
                                                      , SUM(end_time_stamp)           as end_time_stamp
                                                      , SUM(etl_gps_distance)         as etl_gps_distance
                                                      , SUM(veh_message_driving_time) as veh_message_driving_time
                                                      , SUM(idle_duration)            as idle_duration
                                                      , SUM(veh_message_distance)     as veh_message_distance
                                                      , SUM(average_speed)            as average_speed
                                                      , SUM(average_weight)           as average_weight
                                                      , SUM(start_odometer)           as start_odometer
                                                      , Max(max_speed) as maxspeed
                                                      , sum(fuel_consumption) as FuelConsumption
                                                      , sum(co2_emission) As Co2Emission
                                                      , sum(harsh_brake_duration) As HarshBreakDuration
                                                      , sum(heavy_throttle_duration) As HeavyThrottleDuration
                                                      , sum(cruise_control_distance_30_50) As CruiseControlDistance30_50
                                                      , sum(cruise_control_distance_50_75) As CruiseControlDistance50_75
                                                      , sum(cruise_control_distance_more_than_75) As CruiseControlDistanceMoreThan_75
                                                      , max(average_traffic_classification) As AverageTrafficClassification
                                                      , sum (cc_fuel_consumption) As CcFuelConsumption
                                                      , sum (fuel_consumption_cc_non_active) As CcFuelConsumptionCCNonActive
                                                      , sum (idling_consumption) As IdlingConsumption
                                                      , sum (dpa_score) As DPAScore
                                                    FROM 
                                                        tripdetail.trip_statistics
                                                    where
                                                        start_time_stamp   >= @FromDate
                                                        and end_time_stamp <= @ToDate
                                                        --and VIN = ANY(@Vins)
                                                    GROUP BY
                                                        VIN
                                                )
                                              , cte_combine as
                                                (
                                                    SELECT
                                                        vh.name as vehiclename
                                                     , fd.vin  as VIN
                                                      , numberoftrips
                                                      , vh.registration_no             as RegistrationNumber
                                                      , fd.etl_gps_trip_time           as TripTime
                                                      , fd.end_time_stamp              as StopTime
                                                      , round ( fd.etl_gps_distance,2) as Distance
                                                      , fd.veh_message_driving_time    as DrivingTime
                                                      , fd.idle_duration               as IdleTime
                                                      , round ((fd.veh_message_distance/totalworkingdays),2)   as AverageDistance
                                                      , round (fd.average_speed,2)     as AverageSpeed
                                                      , round (fd.average_weight,2)    as AverageWeight
                                                      , round (fd.start_odometer,2)    as Odometer
                                                      , maxspeed
                                                      , round((FuelConsumption/numberoftrips),2) As FuelConsumption
                                                      , round((Co2Emission/numberoftrips),2) As Co2Emission
                                                      , round((HarshBreakDuration/numberoftrips),2) As HarshBreakDuration
                                                      , round((HeavyThrottleDuration/numberoftrips),2) As HeavyThrottleDuration
                                                      , CruiseControlDistance30_50
                                                      , CruiseControlDistance50_75
                                                      , CruiseControlDistanceMoreThan_75
                                                      , AverageTrafficClassification
                                                      , CcFuelConsumption
                                                      , CcFuelConsumptionCCNonActive
                                                      , IdlingConsumption
                                                      , DPAScore
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

                List<FleetFuelDetails> lstFleetDetails = (List<FleetFuelDetails>)await _dataMartdataAccess.QueryAsync<FleetUtilizationDetails>(queryFleetUtilization, parameterOfFilters);
                return lstFleetDetails?.Count > 0 ? lstFleetDetails : new List<FleetFuelDetails>();

            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To Fetch Fuel details by Vechicle
        /// </summary>
        /// <param name="fleetFuelFilters"></param>
        /// <returns></returns>
        public async Task<List<FleetFuelDetails>> GetFleetFuelDetailsByDriver(FleetFuelFilter fleetFuelFilters)
        {
            try
            {
                var parameterOfFilters = new DynamicParameters();
                parameterOfFilters.Add("@FromDate", fleetFuelFilters.StartDateTime);
                parameterOfFilters.Add("@ToDate", fleetFuelFilters.EndDateTime);
                parameterOfFilters.Add("@Vins", fleetFuelFilters.VIN);
                string queryFleetUtilization = @"WITH CTE_FleetDeatils as
                                                (
                                                    SELECT
                                                        VIN
                                                      , count(distinct date_trunc('day', to_timestamp(start_time_stamp/1000))) as totalworkingdays
                                                      , count(trip_id)                as numberoftrips
                                                      , SUM(etl_gps_trip_time)        as etl_gps_trip_time
                                                      , SUM(end_time_stamp)           as end_time_stamp
                                                      , SUM(etl_gps_distance)         as etl_gps_distance
                                                      , SUM(veh_message_driving_time) as veh_message_driving_time
                                                      , SUM(idle_duration)            as idle_duration
                                                      , SUM(veh_message_distance)     as veh_message_distance
                                                      , SUM(average_speed)            as average_speed
                                                      , SUM(average_weight)           as average_weight
                                                      , SUM(start_odometer)           as start_odometer
                                                      , Max(max_speed) as maxspeed
                                                      , sum(fuel_consumption) as FuelConsumption
                                                      , sum(co2_emission) As Co2Emission
                                                      , sum(harsh_brake_duration) As HarshBreakDuration
                                                      , sum(heavy_throttle_duration) As HeavyThrottleDuration
                                                      , sum(cruise_control_distance_30_50) As CruiseControlDistance30_50
                                                      , sum(cruise_control_distance_50_75) As CruiseControlDistance50_75
                                                      , sum(cruise_control_distance_more_than_75) As CruiseControlDistanceMoreThan_75
                                                      , max(average_traffic_classification) As AverageTrafficClassification
                                                      , sum (cc_fuel_consumption) As CcFuelConsumption
                                                      , sum (fuel_consumption_cc_non_active) As CcFuelConsumptionCCNonActive
                                                      , sum (idling_consumption) As IdlingConsumption
                                                      , sum (dpa_score) As DPAScore
                                                    FROM 
                                                        tripdetail.trip_statistics
                                                    where
                                                        start_time_stamp   >= @FromDate
                                                        and end_time_stamp <= @ToDate
                                                        --and VIN = ANY(@Vins)
                                                    GROUP BY
                                                        VIN
                                                )
                                              , cte_combine as
                                                (
                                                    SELECT
                                                        vh.name as vehiclename
                                                     , fd.vin  as VIN
                                                      , numberoftrips
                                                      , vh.registration_no             as RegistrationNumber
                                                      , fd.etl_gps_trip_time           as TripTime
                                                      , fd.end_time_stamp              as StopTime
                                                      , round ( fd.etl_gps_distance,2) as Distance
                                                      , fd.veh_message_driving_time    as DrivingTime
                                                      , fd.idle_duration               as IdleTime
                                                      , round ((fd.veh_message_distance/totalworkingdays),2)   as AverageDistance
                                                      , round (fd.average_speed,2)     as AverageSpeed
                                                      , round (fd.average_weight,2)    as AverageWeight
                                                      , round (fd.start_odometer,2)    as Odometer
                                                      , maxspeed
                                                      , round((FuelConsumption/numberoftrips),2) As FuelConsumption
                                                      , round((Co2Emission/numberoftrips),2) As Co2Emission
                                                      , round((HarshBreakDuration/numberoftrips),2) As HarshBreakDuration
                                                      , round((HeavyThrottleDuration/numberoftrips),2) As HeavyThrottleDuration
                                                      , CruiseControlDistance30_50
                                                      , CruiseControlDistance50_75
                                                      , CruiseControlDistanceMoreThan_75
                                                      , AverageTrafficClassification
                                                      , CcFuelConsumption
                                                      , CcFuelConsumptionCCNonActive
                                                      , IdlingConsumption
                                                      , DPAScore
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

                List<FleetFuelDetails> lstFleetDetails = (List<FleetFuelDetails>)await _dataMartdataAccess.QueryAsync<FleetUtilizationDetails>(queryFleetUtilization, parameterOfFilters);
                return lstFleetDetails?.Count > 0 ? lstFleetDetails : new List<FleetFuelDetails>();

            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
