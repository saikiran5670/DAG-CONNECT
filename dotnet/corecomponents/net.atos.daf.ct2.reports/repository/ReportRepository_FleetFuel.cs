﻿using System;
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
                                                    		Select
                                                    			VIN
                                                    		  , count(trip_id)                                                         as numberof  trips
                                                    		  , count(distinct date_trunc('day', to_timestamp(start_time_stamp/1000))) as totalwor  kingdays
                                                    		  , SUM(etl_gps_distance)                                                  as etl_gps_  distance
                                                    		  , SUM(veh_message_distance)                                              as veh_  message_        distance
                                                    		  , SUM(average_speed)                                                     as average_  speed
                                                    		  , MAX(max_speed)                                                         as max_speed
                                                    		  , SUM(average_gross_weight_comb)                                         as       aver    age_    gr  oss_weight_comb
                                                    		  , SUM(fuel_consumption)                                                  as fuel_con  sumed
                                                    		  , SUM(fuel_consumption)                                                  as fuel_con  sumption
                                                    		  , SUM(co2_emission)                                                      as co2_emission  
                                                    		  , SUM(idle_duration)                                                     as idle_dur  ation
                                                    		  , SUM(pto_duration)                                                      as pto_duration  
                                                    		  , SUM(harsh_brake_duration)                                              as hars  h_brake_        duration
                                                    		  , SUM(heavy_throttle_duration)                                           as   heav    y_thrott      le_duration
                                                    		  , SUM(cruise_control_distance_30_50)                                     as           crui          se_control_distance_30_50
                                                    		  , SUM(cruise_control_distance_50_75)                                     as           crui          se_control_distance_50_75
                                                    		  , SUM(cruise_control_distance_more_than_75)                              as               crui        se_control_distance_more_than_75
                                                    		  , MAX(average_traffic_classification)                                    as           aver          age_traffic_classification
                                                    		  , SUM(cc_fuel_consumption)                                               as cc_f  uel_cons        umption
                                                    		  , SUM(fuel_consumption_cc_non_active)                                    as           fuel          _consumption_cc_non_active
                                                    		  , SUM(idling_consumption)                                                as idli  ng_consu        mption
                                                    		  , SUM(dpa_score)                                                         as dpa_score
                                                    		From
                                                    			tripdetail.trip_statistics
                                                    		GROUP BY
                                                    			VIN
                                                    	)
                                                      , cte_combine as
                                                    	(
                                                    		SELECT
                                                    			vh.name                                              as VehicleName
                                                    		  , fd.vin                                               as VIN
                                                    		  , vh.registration_no                                   as VehicleRegistrationNo
                                                    		  , round ( fd.etl_gps_distance,2)                       as Distance
                                                    		  , round ((fd.veh_message_distance/totalworkingdays),2) as AverageDistancePerDay
                                                    		  , round (fd.average_speed,2)                           as AverageSpeed
                                                    		  , max_speed                                            as MaxSpeed
                                                    		  , numberoftrips                                        as NumberOfTrips
                                                    		  , round (fd.average_gross_weight_comb,2)               as AverageGrossWeightComb
                                                    		  , round((fd.fuel_consumption/numberoftrips),2)         As FuelConsumed
                                                    		  , round((fd.fuel_consumption/numberoftrips),2)         As FuelConsumption
                                                    		  , round((fd.co2_emission    /numberoftrips),2)         As CO2Emission
                                                    		  , fd.idle_duration                                     as IdleDuration
                                                    		  , round(fd.pto_duration,2)                             as PTODuration
                                                    		  , round((fd.harsh_brake_duration   /numberoftrips),2)    As HarshBrakeDuration
                                                    		  , round((fd.heavy_throttle_duration/numberoftrips),2)    As HeavyThrottleDuration
                                                    		  , round(fd.cruise_control_distance_30_50)                as CruiseContro  lDistance30_50
                                                    		  , round(fd.cruise_control_distance_50_75)                as CruiseContro  lDistance50_75
                                                    		  , round(fd.cruise_control_distance_more_than_75)         as CruiseControlDistance75
                                                    		  , round(fd.average_traffic_classification)               as AverageTraff  icClassification
                                                    		  , round(fd.cc_fuel_consumption)                          as CCFuelConsumption 
                                                    		  , round(fd.fuel_consumption_cc_non_active)               as Fuelconsumpt  ionCCnonactive
                                                    		  , idling_consumption                                     as IdlingConsumption
                                                    		  , dpa_score                                              as DPAScore
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
            catch (System.Exception ex)
            {
                throw ex;
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
                                                		Select
                                                			VIN
                                                		  , driver1_id                                                             as DriverId
                                                		  , count(trip_id)                                                         as numberoftrips
                                                		  , count(distinct date_trunc('day', to_timestamp(start_time_stamp/1000))) as totalworkingdays
                                                		  , SUM(etl_gps_distance)                                                  as etl_gps_distance
                                                		  , SUM(veh_message_distance)                                              as veh_message_distance
                                                		  , SUM(average_speed)                                                     as average_speed
                                                		  , MAX(max_speed)                                                         as max_speed
                                                		  , SUM(average_gross_weight_comb)                                         as average_gross_weight_comb
                                                		  , SUM(fuel_consumption)                                                  as fuel_consumed
                                                		  , SUM(fuel_consumption)                                                  as fuel_consumption
                                                		  , SUM(co2_emission)                                                      as co2_emission
                                                		  , SUM(idle_duration)                                                     as idle_duration
                                                		  , SUM(pto_duration)                                                      as pto_duration
                                                		  , SUM(harsh_brake_duration)                                              as harsh_brake_duration
                                                		  , SUM(heavy_throttle_duration)                                           as heavy_throttle_duration
                                                		  , SUM(cruise_control_distance_30_50)                                     as cruise_control_distance_30_50
                                                		  , SUM(cruise_control_distance_50_75)                                     as cruise_control_distance_50_75
                                                		  , SUM(cruise_control_distance_more_than_75)                              as cruise_control_distance_more_than_75
                                                		  , MAX(average_traffic_classification)                                    as average_traffic_classification
                                                		  , SUM(cc_fuel_consumption)                                               as cc_fuel_consumption
                                                		  , SUM(fuel_consumption_cc_non_active)                                    as fuel_consumption_cc_non_active
                                                		  , SUM(idling_consumption)                                                as idling_consumption
                                                		  , SUM(dpa_score)                                                         as dpa_score
                                                		From
                                                			tripdetail.trip_statistics
                                                		GROUP BY
                                                			driver1_id
                                                		  , VIN
                                                	)
                                                  , cte_combine as
                                                	(
                                                		SELECT
                                                			vh.name            as VehicleName
                                                		  , fd.vin             as VIN
                                                		  , vh.registration_no as VehicleRegistrationNo
                                                		  , fd.DriverId
                                                		  , round ( fd.etl_gps_distance,2)                       as Distance
                                                		  , round ((fd.veh_message_distance/totalworkingdays),2) as AverageDistancePerDay
                                                		  , round (fd.average_speed,2)                           as AverageSpeed
                                                		  , max_speed                                            as MaxSpeed
                                                		  , numberoftrips                                        as NumberOfTrips
                                                		  , round (fd.average_gross_weight_comb,2)               as AverageGrossWeightComb
                                                		  , round((fd.fuel_consumption/numberoftrips),2)         As FuelConsumed
                                                		  , round((fd.fuel_consumption/numberoftrips),2)         As FuelConsumption
                                                		  , round((fd.co2_emission    /numberoftrips),2)         As CO2Emission
                                                		  , fd.idle_duration                                     as IdleDuration
                                                		  , round(fd.pto_duration,2)                             as PTODuration
                                                		  , round((fd.harsh_brake_duration   /numberoftrips),2)    As HarshBrakeDuration
                                                		  , round((fd.heavy_throttle_duration/numberoftrips),2)    As HeavyThrottleDuration
                                                		  , round(fd.cruise_control_distance_30_50)                as CruiseControlDistance30_50
                                                		  , round(fd.cruise_control_distance_50_75)                as CruiseControlDistance50_75
                                                		  , round(fd.cruise_control_distance_more_than_75)         as CruiseControlDistance75
                                                		  , round(fd.average_traffic_classification)               as AverageTrafficClassification
                                                		  , round(fd.cc_fuel_consumption)                          as CCFuelConsumption 
                                                		  , round(fd.fuel_consumption_cc_non_active)               as FuelconsumptionCCnonactive
                                                		  , idling_consumption                                     as IdlingConsumption
                                                		  , dpa_score                                              as DPAScore
                                                		FROM
                                                			CTE_FleetDeatils fd
                                                			join
                                                				master.vehicle vh
                                                				on
                                                					fd.VIN =vh.VIN
                                                	)
                                                SELECT
                                                	dr.first_name || ' ' ||dr.last_name as DriverName
                                                  , cmb.*
                                                FROM
                                                	cte_combine cmb
                                                	join
                                                		master.driver dr
                                                		on
                                                			dr.driver_id = cmb.driverId";

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

        public async Task<List<FleetFuelDetails>> GetFleetFuelTripDetailsByVehicle(FleetFuelFilter fleetFuelFilters)
        {
            try
            {
                var parameterOfFilters = new DynamicParameters();
                parameterOfFilters.Add("@FromDate", fleetFuelFilters.StartDateTime);
                parameterOfFilters.Add("@ToDate", fleetFuelFilters.EndDateTime);
                parameterOfFilters.Add("@Vins", fleetFuelFilters.VINs);
                string queryFleetUtilization = @"SELECT 
                                                TS.vin
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
                                                FROM 
                                                tripdetail.trip_statistics TS
                                                left join master.vehicle VH on TS.vin=VH.vin       
                                                where TS.vin =ANY(@Vins) and (start_time_stamp >= @FromDate and end_time_stamp <= @ToDate)
                                                GROUP by TS.VIN,date_trunc('day', to_timestamp(TS.start_time_stamp/1000)),VH.name ,VH.registration_no";



                List<FleetFuelDetails> lstFleetDetails = (List<FleetFuelDetails>)await _dataMartdataAccess.QueryAsync<FleetFuelDetails>(queryFleetUtilization, parameterOfFilters);
                return lstFleetDetails?.Count > 0 ? lstFleetDetails : new List<FleetFuelDetails>();

            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

    }
}
