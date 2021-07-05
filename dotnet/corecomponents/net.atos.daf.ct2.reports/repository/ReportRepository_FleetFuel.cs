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
        public async Task<List<CO2Coefficient>> GetCO2CoEfficientData()
        {
            try
            {
                string queryMasterData = @"select id, Description, Fuel_type, Coefficient from master.co2coefficient";

                List<CO2Coefficient> lstCO2CoEfficient = (List<CO2Coefficient>)await _dataAccess.QueryAsync<CO2Coefficient>(queryMasterData);
                return lstCO2CoEfficient?.Count > 0 ? lstCO2CoEfficient : new List<CO2Coefficient>();
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
                parameterOfFilters.Add("@Vins", fleetFuelFilters.VINs);
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
                parameterOfFilters.Add("@Vins", fleetFuelFilters.VINs);
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

        public async Task<List<FleetFuel_VehicleGraph>> GetFleetFuelDetailsForVehicleGraphs(FleetFuelFilter fleetFuelFilters)
        {
            try
            {
                var parameterOfFilters = new DynamicParameters();
                parameterOfFilters.Add("@FromDate", fleetFuelFilters.StartDateTime);
                parameterOfFilters.Add("@ToDate", fleetFuelFilters.EndDateTime);
                parameterOfFilters.Add("@Vins", fleetFuelFilters.VINs);

                string query = @"WITH cte_workingdays AS(
                        select
                        date_trunc('day', to_timestamp(start_time_stamp/1000)) as startdate,
                        count(distinct date_trunc('day', to_timestamp(start_time_stamp/1000))) as totalworkingdays,
						Count(distinct v.vin) as vehiclecount,
						Count(distinct trip_id) as tripcount,
                        sum(etl_gps_distance) as totaldistance,
                        sum(idle_duration) as totalidleduration,
						sum(fuel_consumption) as fuelconsumption,
						sum(co2_emission) as co2emission						
                        FROM tripdetail.trip_statistics CT
						Join master.vehicle v
						on CT.vin = v.vin
                        where (start_time_stamp >= @FromDate 
							   and end_time_stamp<= @ToDate) 
						and CT.vin=ANY(@vins)
                        group by date_trunc('day', to_timestamp(start_time_stamp/1000))                     
                        )
                        select
                        '' as VIN,
                        startdate,
						extract(epoch from startdate) * 1000 as Date,
                       	totalworkingdays,
						vehiclecount,
                        tripcount as NumberofTrips,
                        CAST((totaldistance / totalworkingdays) as float) as Distance,
                        CAST((totalidleduration / totalworkingdays) as float) as IdleDuration ,
                        CAST((fuelconsumption / totalworkingdays) as float) as FuelConsumtion ,
                        CAST((co2emission / totalworkingdays) as float) as Co2Emission 
                        --CAST((totalaverageweightperprip / totalworkingdays) as float) as Averageweight
                        from cte_workingdays";
                List<FleetFuel_VehicleGraph> lstFleetDetails = (List<FleetFuel_VehicleGraph>)await _dataMartdataAccess.QueryAsync<FleetFuel_VehicleGraph>(query, parameterOfFilters);
                return lstFleetDetails?.Count > 0 ? lstFleetDetails : new List<FleetFuel_VehicleGraph>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<FleetFuelTripDetails>> GetFleetFuelTripDetailsByVehicle(FleetFuelFilter fleetFuelFilters)
        {
            try
            {
                var parameterOfFilters = new DynamicParameters();
                parameterOfFilters.Add("@FromDate", fleetFuelFilters.StartDateTime);
                parameterOfFilters.Add("@ToDate", fleetFuelFilters.EndDateTime);
                parameterOfFilters.Add("@Vins", fleetFuelFilters.VINs);
                string queryFleetUtilization = @"SELECT 
                                                ,TS.vin
                                                ,VH.name AS Name
                                                ,VH.registration_no AS RegistrationNo
                                                ,count(TS.trip_id) as numberoftrips
                                                ,SUM(TS.etl_gps_trip_time) as etl_gps_trip_time
                                                ,sum(TS.end_time_stamp) 
                                                ,SUM(TS.etl_gps_distance) as etl_gps_distance
                                                ,SUM(TS.veh_message_driving_time) as veh_message_driving_time
                                                ,SUM(TS.idle_duration) as idle_duration
                                                ,SUM(TS.veh_message_distance) as veh_message_distance
                                                ,SUM(TS.average_speed) as average_speed
                                                ,SUM(TS.average_weight) as average_weight
                                                ,Max(TS.max_speed) as maxspeed
                                                ,sum(TS.fuel_consumption) as FuelConsumption
                                                ,sum(TS.co2_emission) As Co2Emission
                                                ,sum(TS.harsh_brake_duration) As HarshBreakDuration
                                                ,sum(TS.heavy_throttle_duration) As HeavyThrottleDuration
                                                ,sum(TS.cruise_control_distance_30_50) As CruiseControlDistance30_50
                                                ,sum(TS.cruise_control_distance_50_75) As CruiseControlDistance50_75
                                                ,sum(TS.cruise_control_distance_more_than_75) As CruiseControlDistanceMoreThan_75
                                                ,max(TS.average_traffic_classification) As AverageTrafficClassification
                                                ,sum (TS.cc_fuel_consumption) As CcFuelConsumption
                                                ,sum (TS.fuel_consumption_cc_non_active) As CcFuelConsumptionCCNonActive
                                                ,sum (TS.idling_consumption) As IdlingConsumption
                                                ,sum (TS.dpa_score) As DPAScore
                                                ,CASE WHEN TS.start_position IS NULL THEN '' ELSE TS.start_position END AS StartPosition
                                                ,CASE WHEN TS.end_position IS NULL THEN '' ELSE TS.end_position END AS EndPosition
                                                ,TS.start_position_lattitude AS StartPositionLattitude
                                                ,TS.start_position_longitude AS StartPositionLongitude
                                                ,TS.end_position_lattitude AS EndPositionLattitude
                                                ,TS.end_position_longitude AS EndPositionLongitude
                                                FROM 
                                                tripdetail.trip_statistics TS
                                                left join master.vehicle VH on TS.vin=VH.vin
                                                where TS.vin=@Vins and(start_time_stamp >= @FromDate and end_time_stamp<= @ToDate)
                                                GROUP by TS.VIN,date_trunc('day', to_timestamp(TS.start_time_stamp/1000)),VH.name ,VH.registration_no";

                List<FleetFuelTripDetails> lstFleetDetails = (List<FleetFuelTripDetails>)await _dataMartdataAccess.QueryAsync<FleetFuelTripDetails>(queryFleetUtilization, parameterOfFilters);
                return lstFleetDetails?.Count > 0 ? lstFleetDetails : new List<FleetFuelTripDetails>();

            }
            catch (System.Exception)
            {
                throw;
            }
        }

    }
}
