using System;
using System.Collections.Generic;
using System.Linq;
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
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Get Idling Consumption data from master
        /// </summary>
        /// <returns>Data in MinValue, MaxValue and Key fields </returns>
        public async Task<List<IdlingConsumption>> GetIdlingConsumptionData(string languageCode)
        {
            try
            {
                var parameterOfFilters = new DynamicParameters();
                parameterOfFilters.Add("@LanguageCode", languageCode);

                string queryMasterData = @"SELECT ic.id, ic.min_val as MinValue, ic.max_val as MaxValue, ic.Key, tr.value FROM master.idlingconsumption ic JOIN translation.translation tr ON tr.name=ic.key where tr.code = @LanguageCode";

                //var lstIdlingConsumption1 = await _dataAccess.QueryAsync(queryMasterData, parameterOfFilters);
                List<IdlingConsumption> lstIdlingConsumption = (List<IdlingConsumption>)await _dataAccess.QueryAsync<IdlingConsumption>(queryMasterData, parameterOfFilters);
                return lstIdlingConsumption?.Count > 0 ? lstIdlingConsumption : new List<IdlingConsumption>();
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get Average Traffic Classification data from master
        /// </summary>
        /// <returns>Data in MinValue, MaxValue and Key fields </returns>
        public async Task<List<AverageTrafficClassification>> GetAverageTrafficClassificationData(string languageCode)
        {
            try
            {
                var parameterOfFilters = new DynamicParameters();
                parameterOfFilters.Add("@LanguageCode", languageCode);

                string queryMasterData = @"SELECT ac.id, ac.min_val as MinValue, ac.max_val as MaxValue, ac.Key, tr.value FROM master.averagetrafficclassification ac JOIN translation.translation tr ON tr.name=ac.key where tr.code = @LanguageCode";

                List<AverageTrafficClassification> lstAverageTrafficClassification = (List<AverageTrafficClassification>)await _dataAccess.QueryAsync<AverageTrafficClassification>(queryMasterData, parameterOfFilters);
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
                                                         			v.VIN
                                                                  , sum(trip_idle_pto_fuel_consumed) as tripidleptofuelconsumed
                                                                  , sum(idling_consumption_with_pto) as idlingconsumptionwithpto
                                                         		  , count(trip_id)                                                         as numberoftrips
                                                         		  , count(distinct date_trunc('day', to_timestamp(end_time_stamp/1000))) as totalworkingdays
                                                         		  , SUM(etl_gps_distance)                                                  as etl_gps_distance
                                                         		  , SUM(etl_gps_distance)                                                  as veh_message_distance
                                                         		  , SUM(etl_gps_distance)/case when SUM(etl_gps_trip_time) >0 then SUM(etl_gps_trip_time) else 1 end as average_speed
                                                         		  , MAX(max_speed)                                                         as max_speed
                                                         		  , SUM(average_gross_weight_comb)                                         as average_gross_weight_comb
                                                         		  , SUM(etl_gps_fuel_consumed)                                             as fuel_consumed
                                                         		  , (SUM(etl_gps_fuel_consumed)/case when SUM(etl_gps_distance) >0 then SUM(etl_gps_distance) else 1 end) as fuel_consumption
                                                         		  , SUM(co2_emission)                                                      as co2_emission
                                                                  , SUM(idle_duration) as idle_duration
                                                         		  , case when SUM(etl_gps_trip_time)>0 then ((SUM(idle_duration)/(SUM(etl_gps_trip_time)/1000))*100) else 0 end               as idle_duration_percentage
                                                      		      , case when SUM(etl_gps_trip_time)>0 then ((SUM(pto_duration)/(SUM(etl_gps_trip_time)/1000))*100)   else 0 end              as pto_duration
                                                      		      , SUM(harsh_brake_duration)                                              as harsh_brake_duration
                                                      		      --, case when SUM(etl_gps_trip_time)>0 then ((SUM(heavy_throttle_duration)/(SUM(etl_gps_trip_time)/1000))*100)  else 0 end    as heavy_throttle_duration
                                                                  , SUM(heavy_throttle_duration)  as heavy_throttle_duration
                                                      		      , case when SUM(etl_gps_distance)>0 then ((SUM(cruise_control_distance_30_50)/SUM(etl_gps_distance)) * 100)  else 0 end     as cruise_control_distance_30_50
                                                      		      , case when SUM(etl_gps_distance)>0 then ((SUM(cruise_control_distance_50_75)/SUM(etl_gps_distance)) * 100)    else 0 end   as cruise_control_distance_50_75
                                                      		      , case when SUM(etl_gps_distance)>0 then ((SUM(cruise_control_distance_more_than_75)/SUM(etl_gps_distance)) * 100)  else 0 end as     cruise_control_distance_more_than_75
                                                         		  , MAX(average_traffic_classification)                                    as average_traffic_classification
                                                         		  , SUM(cc_fuel_consumption)                                               as cc_fuel_consumption
                                                         		  , SUM(fuel_consumption_cc_non_active)                                    as fuel_consumption_cc_non_active
                                                         		  , SUM(idling_consumption)                                                as idling_consumption
                                                         		  , SUM(dpa_score)                                                         as dpa_score
                                                                  , SUM(veh_message_dpaanticipation_score/
	                                                              (case when veh_message_dpaanticipation_count > 0 then veh_message_dpaanticipation_count
	                                                              else 1 END)) 	   as DPAAnticipationScore       
                                                                  , SUM(v_cruise_control_dist_for_cc_fuel_consumption) 					   as CCFuelDistance
                                                                  , SUM(v_cruise_control_fuel_consumed_for_cc_fuel_consumption) 		   as CCFuelConsumed
                                                    			  --, SUM(veh_message_dpabraking_score) 									   as DPABrakingScore                         
                                                                  , SUM(veh_message_dpabraking_score/
	                                                              (case when veh_message_dpabraking_count > 0 then veh_message_dpabraking_count
	                                                              else 1 END)) 	   as DPABrakingScore 
                                                    			  --, SUM(veh_message_dpaanticipation_score) 								   as DPAAnticipationScore                     
                                                    			  , SUM(veh_message_idle_without_ptoduration) 							   as IdlingWithoutPTO                
                                                    			  , SUM(veh_message_idle_ptoduration) 									   as IdlingPTO 
                                                    			  , SUM(veh_message_brake_duration) 									   as FootBrake
                                                                  , SUM(case when average_weight>0 then 1 else 0 end)  as numoftripswithavgweight
																  , SUM(case when harsh_brake_duration>0 then 1 else 0 end)  as numoftripswithharshbreak
																  , SUM(case when dpa_score>0 then 1 else 0 end)  as numoftripswithdpascore
                                                                  , SUM(case when veh_message_dpaanticipation_score>0 then 1 else 0 end)  as numoftripswithdpaanticipationscore
                                                                  , SUM(case when veh_message_dpabraking_score > 0 then 1 else 0 end)  as numoftripswithdpabrakingscore
                                                         		From
                                                         			tripdetail.trip_statistics ts
                                                                    Join master.vehicle v on ts.vin=v.vin                                                                    
                                                               WHERE ts.end_time_stamp >= v.reference_date and (end_time_stamp >= @FromDate and end_time_stamp<= @ToDate) 
                                                                        and v.VIN=ANY(@Vins)
                                                         		GROUP BY
                                                         			v.VIN
                                                         	)
                                                           , cte_combine as
                                                         	(
                                                         		SELECT
                                                         			vh.name                                              					as VehicleName
                                                                 , '' as tripid
                                                         		  , fd.vin                                               					as VIN
                                                         		  , vh.registration_no                                   					as VehicleRegistrationNo
                                                         		  , round ( fd.etl_gps_distance,2)                       					as Distance
                                                         		  , case when totalworkingdays>0 then round ((fd.veh_message_distance/totalworkingdays),2) else 0 end as AverageDistancePerDay
                                                         		  , round (fd.average_speed,7)                           					as AverageSpeed
                                                         		  , max_speed                                            					as MaxSpeed
                                                         		  , numberoftrips                                        					as NumberOfTrips
                                                         		  ,case when numoftripswithavgweight>0 then round (fd.average_gross_weight_comb/numoftripswithavgweight, 3) 
													                 else round (fd.average_gross_weight_comb,3) end  as AverageGrossWeightComb
                                                         		  , round(fd.fuel_consumed,2)                            					as FuelConsumed
                                                         		  , round(fd.fuel_consumption,5)                         					as FuelConsumption
                                                         		  , round(fd.co2_emission,2)                             					as CO2Emission
                                                                  , round(((fd.etl_gps_distance * 2640)/100)/1000,4) as CO2Emmision
                                                         		  , round(fd.idle_duration,2)                            					as IdleDuration
                                                                  , round(fd.idle_duration_percentage,2)                 					as IdleDurationPercentage
                                                         		  , round(fd.pto_duration,2)                             					as PTODuration
                                                         		  ,case when numoftripswithharshbreak>0 then round (fd.harsh_brake_duration/numoftripswithharshbreak, 2) 
													  			      else round (fd.harsh_brake_duration,2) end  						    as HarshBrakeDuration
                                                         		  , round(fd.heavy_throttle_duration,2)                  					as HeavyThrottleDuration
                                                         		  , round(fd.cruise_control_distance_30_50,2)            					as CruiseControlDistance3050
                                                         		  , round(fd.cruise_control_distance_50_75,2)            					as CruiseControlDistance5075
                                                         		  , round(fd.cruise_control_distance_more_than_75,2)     					as CruiseControlDistance75
                                                         		  , round(fd.average_traffic_classification,4) * 1000            					as AverageTrafficClassification
                                                         		  , case when fd.CCFuelDistance::decimal >0 then round((fd.CCFuelConsumed::decimal/fd.CCFuelDistance::decimal),5) else fd.CCFuelConsumed end 	as CCFuelConsumption
                                                                  ,case when (fd.etl_gps_distance::decimal - fd.CCFuelDistance::decimal)>0 then round(((fd.fuel_consumed - fd.CCFuelConsumed)::decimal/(fd.etl_gps_distance - fd.CCFuelDistance)::decimal),4) else round(fd.fuel_consumption,5) end as FuelconsumptionCCnonactive
                                                         		  , idling_consumption                                   					as IdlingConsumption
                                                         		  ,case when numoftripswithdpascore>0 then  round((dpa_score/numoftripswithdpascore),2)
																      else dpa_score  end  								                    as DPAScore
                                                                  ,case when numoftripswithdpaanticipationscore>0 then  round((fd.DPAAnticipationScore/numoftripswithdpaanticipationscore),2)
                                                                     else fd.DPAAnticipationScore  end  		  as DPAAnticipationScore
                                                                  , round(fd.CCFuelDistance,2) 							 					as CCFuelDistance
                                                                  , round(fd.CCFuelConsumed,2) 							 					as CCFuelConsumed
                                                                  , round(fd.etl_gps_distance - fd.CCFuelDistance,2) 	 					as CCFuelDistanceNotActive
                                                                  , round(fd.fuel_consumed - fd.CCFuelConsumed,2) 		 					as CCFuelConsumedNotActive
                                                                  , '' AS StartPosition					
                                                                  , '' AS EndPosition					
                                                    			  ,case when numoftripswithdpabrakingscore>0 then  round((fd.DPABrakingScore/numoftripswithdpabrakingscore),2)
                                                                     else fd.DPABrakingScore  end  		  as DPABrakingScore                         
                                                    			  , round(fd.IdlingWithoutPTO,4) 							 				as IdlingWithoutPTO                
                                                    			  , round(fd.IdlingPTO,4) 								 					as IdlingPTO 
                                                    			  , round(fd.FootBrake,2) 								 					as FootBrake
                                                                ,round((case when (fd.tripidleptofuelconsumed is not null and fd.idlingconsumptionwithpto is not null and  fd.idlingconsumptionwithpto >0 )
                                                                then (fd.tripidleptofuelconsumed  / fd.idlingconsumptionwithpto ) * 100
																      else 0  end),4) as IdlingConsumptionWithPto 
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

                List<FleetFuelDetails> lstFleetDetails = (List<FleetFuelDetails>)await _dataMartdataAccess.QueryAsync<FleetFuelDetails>(queryFleetUtilization, parameterOfFilters);

                //List<CO2Coefficient> co2CoEfficientData = await GetCO2CoEfficientData();
                //List<IdlingConsumption> idlingConsumption = await GetIdlingConsumptionData("EN-GB");
                //List<AverageTrafficClassification> averageTrafficClassification = await GetAverageTrafficClassificationData("EN-GB");
                if (lstFleetDetails?.Count > 0)
                {

                    // new way To pull respective trip fleet position (One DB call for batch of 1000 trips)
                    string[] tripIds = lstFleetDetails.Select(item => item.Tripid).ToArray();
                    List<LiveFleetPosition> lstLiveFleetPosition = await GetLiveFleetPosition(tripIds);
                    if (lstLiveFleetPosition.Count > 0)
                        foreach (FleetFuelDetails trip in lstFleetDetails)
                        {
                            trip.LiveFleetPosition = lstLiveFleetPosition.Where(fleet => fleet.TripId == trip.Tripid).ToList();
                        }

                    /** Old way To pull respective trip fleet position
                    foreach (var item in data)
                    {
                        await GetLiveFleetPosition(item);
                    }
                    */

                    List<TripAlert> lstTripAlert = await GetTripAlert(fleetFuelFilters.StartDateTime, fleetFuelFilters.EndDateTime, fleetFuelFilters.VINs);
                    if (lstTripAlert.Count() > 0)
                    {
                        foreach (FleetFuelDetails trip in lstFleetDetails)
                        {
                            trip.TripAlert = lstTripAlert.Where(fleet => fleet.TripId == trip.Tripid).ToList();
                        }
                    }
                }
                return lstFleetDetails?.Count > 0 ? lstFleetDetails : new List<FleetFuelDetails>();


            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To Fetch Fuel details by Driver
        /// </summary>
        /// <param name="fleetFuelFilters"></param>
        /// <returns></returns>
        public async Task<List<FleetFuelDetailsByDriver>> GetFleetFuelDetailsByDriver(FleetFuelFilter fleetFuelFilters)
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
                                                  			v.VIN
                                                          , sum(trip_idle_pto_fuel_consumed) as tripidleptofuelconsumed
                                                          , sum(idling_consumption_with_pto) as idlingconsumptionwithpto
                                                  		  , driver1_id                                                             as tripDriverId
                                                  		  , count(trip_id)                                                         as numberoftrips
                                                  		  , count(distinct date_trunc('day', to_timestamp(end_time_stamp/1000))) as totalworkingdays
                                                  		  , SUM(etl_gps_distance)                                                  as etl_gps_distance
                                                  		  , SUM(etl_gps_distance)                                                  as veh_message_distance
                                                  		  , SUM(etl_gps_distance)/case when SUM(etl_gps_trip_time) >0 then SUM(etl_gps_trip_time) else 1 end                          as average_speed
                                                  		  , MAX(max_speed)                                                         as max_speed
                                                  		  , SUM(average_gross_weight_comb)                                         as average_gross_weight_comb
                                                  		  , SUM(etl_gps_fuel_consumed)                                             as fuel_consumed
                                                  		  , (SUM(etl_gps_fuel_consumed)/case when SUM(etl_gps_distance) >0 then SUM(etl_gps_distance) else 1 end)                     as fuel_consumption
                                                  		  , SUM(co2_emission)                                                      as co2_emission
                                                          , SUM(idle_duration) as idle_duration
                                                  		  , case when SUM(etl_gps_trip_time)>0 then ((SUM(idle_duration)/(SUM(etl_gps_trip_time)/1000))*100) else 0 end                   as idle_duration_percentage
                                                  		  , case when SUM(etl_gps_trip_time)>0 then ((SUM(pto_duration)/(SUM(etl_gps_trip_time)/1000))*100)   else 0 end                  as pto_duration
                                                  		  , SUM(harsh_brake_duration)                                              as harsh_brake_duration
                                                  		  --, case when SUM(etl_gps_trip_time)>0 then ((SUM(heavy_throttle_duration)/(SUM(etl_gps_trip_time)/1000))*100)  else 0 end        as heavy_throttle_duration
                                                  		  , SUM(heavy_throttle_duration)  as heavy_throttle_duration
                                                          , case when SUM(etl_gps_distance)>0 then ((SUM(cruise_control_distance_30_50)/SUM(etl_gps_distance)) * 100)  else 0 end        as cruise_control_distance_30_50
                                                  		  , case when SUM(etl_gps_distance)>0 then ((SUM(cruise_control_distance_50_75)/SUM(etl_gps_distance)) * 100)    else 0 end      as cruise_control_distance_50_75
                                                  		  , case when SUM(etl_gps_distance)>0 then ((SUM(cruise_control_distance_more_than_75)/SUM(etl_gps_distance)) * 100)  else 0 end  as cruise_control_distance_more_than_75
                                                  		  , MAX(average_traffic_classification)                                    as average_traffic_classification
                                                  		  , SUM(cc_fuel_consumption)                                               as cc_fuel_consumption
                                                  		  , SUM(fuel_consumption_cc_non_active)                                    as fuel_consumption_cc_non_active
                                                  		  , SUM(idling_consumption)                                                as idling_consumption
                                                  		  , SUM(dpa_score)                                                         as dpa_score
                                                          , SUM(v_cruise_control_dist_for_cc_fuel_consumption) 					   as CCFuelDistance
                                                          , SUM(v_cruise_control_fuel_consumed_for_cc_fuel_consumption) 		   as CCFuelConsumed
                                                          , SUM(veh_message_dpabraking_score/
	                                                              (case when veh_message_dpabraking_count > 0 then veh_message_dpabraking_count
	                                                              else 1 END)) 	   as DPABrakingScore  
                                                           , SUM(veh_message_dpaanticipation_score/
	                                                              (case when veh_message_dpaanticipation_count > 0 then veh_message_dpaanticipation_count
	                                                              else 1 END)) 	   as DPAAnticipationScore       
                                                          , SUM(veh_message_idle_without_ptoduration) 							   as IdlingWithoutPTO                
                                                          , SUM(veh_message_idle_ptoduration) 									   as IdlingPTO 
                                                          , SUM(veh_message_brake_duration) 									   as FootBrake
                                                          , SUM(case when average_weight>0 then 1 else 0 end)  as numoftripswithavgweight
														  , SUM(case when harsh_brake_duration>0 then 1 else 0 end)  as numoftripswithharshbreak
														  , SUM(case when dpa_score>0 then 1 else 0 end)  as numoftripswithdpascore
                                                          , SUM(case when veh_message_dpaanticipation_score>0 then 1 else 0 end)  as numoftripswithdpaanticipationscore
                                                          , SUM(case when veh_message_dpabraking_score > 0 then 1 else 0 end)  as numoftripswithdpabrakingscore
                                                  		From
                                                  			tripdetail.trip_statistics ts
                                                       Join master.vehicle v on ts.vin=v.vin                                                                    
                                                               WHERE ts.end_time_stamp >= v.reference_date and (end_time_stamp >= @FromDate and end_time_stamp<= @ToDate) 
                                                                        and v.VIN=ANY(@Vins)
                                                  		GROUP BY
                                                  			driver1_id
                                                  		  , v.VIN
                                                  	)
                                                    , cte_combine as
                                                  	(
                                                  		SELECT distinct
                                                  			vh.name            									   				    as VehicleName
                                                         , '' as tripid				    
                                                  		  , fd.vin             									   				    as VIN
                                                  		  , vh.registration_no 									   				    as VehicleRegistrationNo
                                                  		  , fd.tripDriverId				    
                                                  		  , round ( fd.etl_gps_distance,2)                         				    as Distance
                                                  		  , case when totalworkingdays>0 then round((fd.veh_message_distance/totalworkingdays),2) else 0 end  as AverageDistancePerDay
                                                  		  , round (fd.average_speed,7)                             					as AverageSpeed
                                                  		  , max_speed                                              					as MaxSpeed
                                                  		  , numberoftrips                                          					as NumberOfTrips
                                                  		 ,case when numoftripswithavgweight>0 then round (fd.average_gross_weight_comb/numoftripswithavgweight, 3) 
													                 else round (fd.average_gross_weight_comb,3) end  as AverageGrossWeightComb
                                                          , round(fd.fuel_consumed,2)                              					as FuelConsumed
                                                  		  , round((fd.fuel_consumed/fd.etl_gps_distance),5)                         as FuelConsumption
                                                  		  , round(fd.co2_emission,2)                               					as CO2Emission
                                                          , round(((fd.etl_gps_distance * 2640)/100)/1000,4) as CO2Emmision
                                                  		  , round(fd.idle_duration_percentage,2)                   					as IdleDurationPercentage
                                                         , round(fd.idle_duration,2)                               					as IdleDuration
                                                  		  , round(fd.pto_duration,2)                               					as PTODuration
                                                  		  ,case when numoftripswithharshbreak>0 then round (fd.harsh_brake_duration/numoftripswithharshbreak, 2) 
													  			      else round (fd.harsh_brake_duration,2) end  				    as HarshBrakeDuration
                                                  		  , round(fd.heavy_throttle_duration,2)                    					as HeavyThrottleDuration
                                                  		  , round(fd.cruise_control_distance_30_50,2)              					as CruiseControlDistance3050
                                                  		  , round(fd.cruise_control_distance_50_75,2)              					as CruiseControlDistance5075
                                                  		  , round(fd.cruise_control_distance_more_than_75,2)       					as CruiseControlDistance75
                                                  		  , round(fd.average_traffic_classification,4) * 1000              					as AverageTrafficClassification
                                                  		  , case when fd.CCFuelDistance::decimal >0 then round((fd.CCFuelConsumed::decimal/fd.CCFuelDistance::decimal),5) else fd.CCFuelConsumed end 	as CCFuelConsumption
                                                  		  ,case when (fd.etl_gps_distance::decimal - fd.CCFuelDistance::decimal)>0 then round(((fd.fuel_consumed - fd.CCFuelConsumed)::decimal/(fd.etl_gps_distance - fd.CCFuelDistance)::decimal),4) else round(fd.fuel_consumption,5) end as FuelconsumptionCCnonactive
                                                  		  , idling_consumption                                     					as IdlingConsumption
                                                  		  ,case when numoftripswithdpascore>0 then  round((dpa_score/numoftripswithdpascore),2)
																      else dpa_score  end  								                    as DPAScore
                                                         , round(fd.CCFuelDistance,2) 							   					as CCFuelDistance
                                                         , round(fd.CCFuelConsumed,2) 							   					as CCFuelConsumed
                                                         , round(fd.etl_gps_distance - fd.CCFuelDistance,2) 	   					as CCFuelDistanceNotActive
                                                         , round(fd.fuel_consumed - fd.CCFuelConsumed,2) 		   					as CCFuelConsumedNotActive
                                                         , '' AS StartPosition
                                                         , '' AS EndPosition
                                                         ,case when numoftripswithdpabrakingscore>0 then  round((fd.DPABrakingScore/numoftripswithdpabrakingscore),2)
                                                                else fd.DPABrakingScore  end  		  as DPABrakingScore
                                                          ,case when numoftripswithdpaanticipationscore>0 then  round((fd.DPAAnticipationScore/numoftripswithdpaanticipationscore),2)
                                                                else fd.DPAAnticipationScore  end  		  as DPAAnticipationScore
                                                         , round(fd.IdlingWithoutPTO,4) 							   				as IdlingWithoutPTO                
                                                         , round(fd.IdlingPTO,4) 								 	   				as IdlingPTO 
                                                         , round(fd.FootBrake,2)								 	   				as FootBrake
                                                         , round((case when (fd.tripidleptofuelconsumed is not null and fd.idlingconsumptionwithpto is not null and  fd.idlingconsumptionwithpto >0 )
                                                           then (fd.tripidleptofuelconsumed  / fd.idlingconsumptionwithpto ) * 100
													       else 0  end),4) as IdlingConsumptionWithPto 
                                                  		FROM
                                                  			CTE_FleetDeatils fd
                                                  		    left join
                                                  				master.vehicle vh
                                                  				on
                                                  					fd.VIN =vh.VIN                                                  	
                                                  	)
                                                  SELECT
                                                  	COALESCE(dr.first_name,'') || ' ' || COALESCE(dr.last_name,'') as DriverName
                                                    ,cmb.tripDriverId || case when COALESCE(dr.driver_id,'~*') != '~*'
                                                        then '' Else '~*' END   as driverid
                                                    , cmb.*
                                                  FROM
                                                  	cte_combine cmb
                                                  	left join
                                                  		master.driver dr
                                                  		on
                                                  dr.driver_id = cmb.tripDriverId";

                List<FleetFuelDetailsByDriver> lstFleetDetails = (List<FleetFuelDetailsByDriver>)await _dataMartdataAccess.QueryAsync<FleetFuelDetailsByDriver>(queryFleetUtilization, parameterOfFilters);
                List<TripAlert> lstTripAlert = await GetTripAlert(fleetFuelFilters.StartDateTime, fleetFuelFilters.EndDateTime, fleetFuelFilters.VINs);
                List<string> lstDriverIds = lstFleetDetails.Select(a => a.DriverID).ToList();
                //checking driverId of tripdetail.trip_statistics are opt-out or not in master.driver table
                //Mostly tripdetail.trip_statistics driverid are not availeble in master,driver table
                List<string> lstOfOptOutDriver = await GetOptOutDriver(lstDriverIds);
                for (int i = 0; i < lstOfOptOutDriver.Count; i++)
                {
                    var data = lstFleetDetails.SingleOrDefault(a => a.DriverID == lstOfOptOutDriver[0]);//"PH B313715714715196");
                    if (data != null)
                    {
                        lstFleetDetails.Remove(data);
                    }
                }
                if (lstTripAlert.Count() > 0)
                {
                    foreach (FleetFuelDetailsByDriver trip in lstFleetDetails)
                    {
                        trip.TripAlert = lstTripAlert.Where(fleet => fleet.TripId == trip.Tripid).ToList();
                    }
                }
                return lstFleetDetails?.Count > 0 ? lstFleetDetails : new List<FleetFuelDetailsByDriver>();

            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private async Task<List<string>> GetOptOutDriver(List<string> lstDriverIds)
        {
            try
            {
                var parameterOfFilters = new DynamicParameters();
                parameterOfFilters.Add("driverId", lstDriverIds);
                string query = @"select driver_id_ext as DriverId  from master.driver 
                where driver_id_ext = ANY(@driverId) and opt_in = 'U' and state='A'";
                var data = await _dataAccess.QueryAsync<string>(query, parameterOfFilters);
                return data.ToList();
            }
            catch (Exception)
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
                        date_trunc('day', to_timestamp(end_time_stamp/1000)) as startdate,
                        count(distinct date_trunc('day', to_timestamp(end_time_stamp/1000))) as totalworkingdays,
						Count(distinct v.vin)                               as vehiclecount,
						Count(distinct trip_id)                             as tripcount,
                        sum(etl_gps_distance)                               as totaldistance,
                        sum(idle_duration)                                  as totalidleduration,
						(SUM(etl_gps_fuel_consumed)/SUM(CASE WHEN etl_gps_distance >0 THEN etl_gps_distance ELSE 1 END)) AS fuelconsumption,                        
                        sum(etl_gps_fuel_consumed)                          as fuelconsumed,
						sum(co2_emission)                                   as co2emission
                        FROM tripdetail.trip_statistics CT
						Join master.vehicle v
						on CT.vin = v.vin and CT.end_time_stamp >= v.reference_date
                        where (end_time_stamp >= @FromDate 
							   and end_time_stamp<= @ToDate) 
						and CT.vin=ANY(@vins)
                        group by date_trunc('day', to_timestamp(end_time_stamp/1000))
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
                        CAST((co2emission / totalworkingdays) as float) as Co2Emission,
                        CAST((fuelconsumed / totalworkingdays) as float) as FuelConsumed    
                        --CAST((totalaverageweightperprip / totalworkingdays) as float) as Averageweight
                        from cte_workingdays";
                List<FleetFuel_VehicleGraph> lstFleetDetails = (List<FleetFuel_VehicleGraph>)await _dataMartdataAccess.QueryAsync<FleetFuel_VehicleGraph>(query, parameterOfFilters);
                return lstFleetDetails?.Count > 0 ? lstFleetDetails : new List<FleetFuel_VehicleGraph>();
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                throw;
            }
        }

        public async Task<List<FleetFuel_VehicleGraph>> GetFleetFuelDetailsForDriverGraphs(FleetFuelFilter fleetFuelFilters)
        {
            try
            {
                var parameterOfFilters = new DynamicParameters();
                parameterOfFilters.Add("@FromDate", fleetFuelFilters.StartDateTime);
                parameterOfFilters.Add("@ToDate", fleetFuelFilters.EndDateTime);
                parameterOfFilters.Add("@Vins", fleetFuelFilters.VINs);

                string query = @"WITH cte_workingdays AS(
                        select
                        date_trunc('day', to_timestamp(end_time_stamp/1000)) as startdate,
                        count(distinct date_trunc('day', to_timestamp(end_time_stamp/1000))) as totalworkingdays,
						Count(distinct v.vin)                                   as vehiclecount,
						Count(distinct trip_id)                                 as tripcount,
                        sum(etl_gps_distance)                                   as totaldistance,
                        sum(idle_duration)                                      as totalidleduration,
						(SUM(etl_gps_fuel_consumed)/SUM(etl_gps_distance))     as fuelconsumption,
                        sum(etl_gps_fuel_consumed)                              as fuelconsumed,
						sum(co2_emission)                                       as co2emission	
                        FROM tripdetail.trip_statistics CT
						Join master.vehicle v
						on CT.vin = v.vin and CT.end_time_stamp >= v.reference_date
                        Left join master.driver dr on
						dr.driver_id = CT.driver1_id
                        where (end_time_stamp >= @FromDate 
							   and end_time_stamp<= @ToDate) 
						and CT.vin=ANY(@vins)
                        group by date_trunc('day', to_timestamp(end_time_stamp/1000))  
                        )
                        select
                        '' as VIN,
                        startdate,
						extract(epoch from startdate) * 1000 as Date,
                       	totalworkingdays,
						vehiclecount,
                        tripcount as NumberofTrips,
                        CAST((totaldistance / totalworkingdays) as float)                   as Distance,
                        CAST((totalidleduration / totalworkingdays) as float)               as IdleDuration ,
                        CAST((fuelconsumption / totalworkingdays) as float)                 as FuelConsumtion ,
                        CAST((co2emission / totalworkingdays) as float)                     as Co2Emission,
                        CAST((fuelconsumed / totalworkingdays) as float)                    as FuelConsumed  
                        --CAST((totalaverageweightperprip / totalworkingdays) as float)     as Averageweight
                        from cte_workingdays";
                List<FleetFuel_VehicleGraph> lstFleetDetails = (List<FleetFuel_VehicleGraph>)await _dataMartdataAccess.QueryAsync<FleetFuel_VehicleGraph>(query, parameterOfFilters);
                return lstFleetDetails?.Count > 0 ? lstFleetDetails : new List<FleetFuel_VehicleGraph>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<FleetFuelDetails>> GetFleetFuelTripDetailsByVehicle(FleetFuelFilter fleetFuelFilters, bool isLiveFleetRequired = true)
        {
            try
            {
                var parameterOfFilters = new DynamicParameters();
                parameterOfFilters.Add("@FromDate", fleetFuelFilters.StartDateTime);
                parameterOfFilters.Add("@ToDate", fleetFuelFilters.EndDateTime);
                parameterOfFilters.Add("@Vin", fleetFuelFilters.VINs.FirstOrDefault());

                string queryFleetUtilization = @"WITH CTE_FleetDeatils as
			(
				Select distinct
					ts.VIN				  
                  , ts.id as Id
				  , trip_id  as tripid
				  , 1 as numberoftrips 
				  , 1 as totalworkingdays
				  , (etl_gps_distance)                                                  as etl_gps_distance
				  , (etl_gps_distance)                                              as veh_message_distance
				  , (etl_gps_distance::numeric)/case when (etl_gps_trip_time::numeric) >0 then (etl_gps_trip_time) else 1 end as average_speed
				  , (max_speed)                                                         as max_speed
				  , (average_gross_weight_comb)                                      as average_gross_weight_comb
				  , (etl_gps_fuel_consumed)                                             as fuel_consumed
				  , ((etl_gps_fuel_consumed::numeric)/case when (etl_gps_distance::numeric) >0 then (etl_gps_distance) else 1 end) as fuel_consumption
				  , (co2_emission)                                                      as co2_emission
                  , idle_duration as idle_duration
				  , case when (((end_time_stamp - start_time_stamp)/1000)::numeric)>0 then ((idle_duration/((end_time_stamp - start_time_stamp)/1000)::numeric) *100) else 0 end as idle_duration_percentage
				  , case when (((end_time_stamp - start_time_stamp)/1000)::numeric)>0 then ((pto_duration/((end_time_stamp - start_time_stamp)/1000)::numeric) *100)  else 0 end   as pto_duration
				  , harsh_brake_duration as harsh_brake_duration
				  --, case when (((end_time_stamp - start_time_stamp)/1000)::numeric)>0 then ((heavy_throttle_duration/((end_time_stamp - start_time_stamp)/1000)::numeric) *100)  else 0 end as heavy_throttle_duration
                  , (heavy_throttle_duration)  as heavy_throttle_duration
				  , case when etl_gps_distance>0 then ((cruise_control_distance_30_50 / etl_gps_distance) * 100) else 0 end         as cruise_control_distance_30_50
				  , case when etl_gps_distance>0 then ((cruise_control_distance_50_75  / etl_gps_distance) * 100)  else 0 end         as cruise_control_distance_50_75
				  , case when etl_gps_distance>0 then ((cruise_control_distance_more_than_75  / etl_gps_distance) * 100)  else 0 end  as cruise_control_distance_more_than_75
				  , (average_traffic_classification)                                    as average_traffic_classification
				  , (cc_fuel_consumption)                                               as cc_fuel_consumption
				  , (fuel_consumption_cc_non_active)                                    as fuel_consumption_cc_non_active
				  , (idling_consumption)                                                as idling_consumption
				  , (dpa_score)                                                         as dpa_score
                  , start_time_stamp                                                       as StartDate
                  , end_time_stamp                                                         as EndDate
				  , start_position_lattitude as startpositionlattitude
				  , start_position_longitude as startpositionlongitude
				  , end_position_lattitude as endpositionlattitude
				  , end_position_longitude as endpositionlongitude
                  , v_cruise_control_dist_for_cc_fuel_consumption as CCFuelDistance
                  , v_cruise_control_fuel_consumed_for_cc_fuel_consumption as CCFuelConsumed                  
                  , (veh_message_brake_duration) 									   as FootBrake
                  , (veh_message_dpabraking_score/
                   (case when veh_message_dpabraking_count > 0 then veh_message_dpabraking_count
                     else 1 END)) 	   as DPABrakingScore 
                  , (veh_message_dpaanticipation_score/
                   (case when veh_message_dpaanticipation_count > 0 then veh_message_dpaanticipation_count
                     else 1 END)) 	   as DPAAnticipationScore 
                  , (case when veh_message_dpabraking_score > 0 then 1 else 0 end)  as numoftripswithdpabrakingscore
                  , (case when veh_message_dpaanticipation_score>0 then 1 else 0 end)  as numoftripswithdpaanticipationscore
                From
					tripdetail.trip_statistics      ts             
                Join master.vehicle v on ts.vin=v.vin                                                                    
                 WHERE ts.end_time_stamp >= v.reference_date and (end_time_stamp >= @FromDate
                               and end_time_stamp <= @ToDate) and ts.VIN = @Vin
				
			)
		  , cte_combine as
            (
                SELECT distinct
                    vh.name as VehicleName
                  , fd.Id
				  , tripid
				  , fd.vin as VIN
				  , vh.registration_no as VehicleRegistrationNo
				  , round(fd.etl_gps_distance, 2) as Distance
				  , round((fd.veh_message_distance), 2) as AverageDistancePerDay
				  , round(fd.average_speed, 7) as AverageSpeed
				  , max_speed as MaxSpeed
				  , numberoftrips as NumberOfTrips
				  , round(fd.average_gross_weight_comb, 2) as AverageGrossWeightComb
				  , round((fd.fuel_consumed), 2)              As FuelConsumed
                   , round((fd.fuel_consumption), 2)           As FuelConsumption
                    , round((fd.co2_emission), 2)           As CO2Emission
                     , round(fd.idle_duration, 2)                                      as IdleDuration
                    , round(fd.idle_duration_percentage, 2)                                      as IdleDurationPercentage
				  , round(fd.pto_duration, 2) as PTODuration
				  , round((fd.harsh_brake_duration), 2)    As HarshBrakeDuration
                   , round((fd.heavy_throttle_duration), 2)    As HeavyThrottleDuration
                    , round(fd.cruise_control_distance_30_50, 2)                as CruiseControlDistance3050
				  , round(fd.cruise_control_distance_50_75, 2) as CruiseControlDistance5075
				  , round(fd.cruise_control_distance_more_than_75, 2) as CruiseControlDistance75
				  , round(fd.average_traffic_classification,4) * 1000 as AverageTrafficClassification
				  , round(fd.cc_fuel_consumption,5) as CCFuelConsumption
				  , case when (fd.etl_gps_distance::decimal - fd.CCFuelDistance::decimal)>0 then round(((fd.fuel_consumed - fd.CCFuelConsumed)::decimal/(fd.etl_gps_distance - fd.CCFuelDistance)::decimal),4) else round(fd.fuel_consumption,5) end  as FuelconsumptionCCnonactive
				  , idling_consumption as IdlingConsumption
				  , dpa_score as DPAScore
                  ,StartDate
                  ,EndDate
                  ,  startpositionlattitude
				  ,  startpositionlongitude
				  , endpositionlattitude
				  , endpositionlongitude
                , round(fd.CCFuelDistance, 2) as CCFuelDistance
                , round(fd.CCFuelConsumed, 2) as CCFuelConsumed
                , round(fd.etl_gps_distance - fd.CCFuelDistance, 2) as CCFuelDistanceNotActive
                , round(fd.fuel_consumed - fd.CCFuelConsumed, 2) as CCFuelConsumedNotActive                
                , coalesce(startgeoaddr.address,'') AS StartPosition
                , coalesce(endgeoaddr.address,'') AS EndPosition
                , round(fd.FootBrake,2) 	as FootBrake
                ,case when numoftripswithdpabrakingscore>0 then  round((fd.DPABrakingScore/numoftripswithdpabrakingscore),2)
                 else fd.DPABrakingScore  end  		  as DPABrakingScore 
                ,case when numoftripswithdpaanticipationscore>0 then  round((fd.DPAAnticipationScore/numoftripswithdpaanticipationscore),2)
                 else fd.DPAAnticipationScore  end  		  as DPAAnticipationScore
                FROM
                    CTE_FleetDeatils fd
                    left join master.vehicle vh
                        on fd.VIN = vh.VIN 
                    left JOIN master.geolocationaddress as startgeoaddr
                            on TRUNC(CAST(startgeoaddr.latitude as numeric),4)= TRUNC(CAST(fd.startpositionlattitude as numeric),4) 
                               and TRUNC(CAST(startgeoaddr.longitude as numeric),4) = TRUNC(CAST(fd.startpositionlongitude as numeric),4)
                     left JOIN master.geolocationaddress as endgeoaddr
                            on TRUNC(CAST(endgeoaddr.latitude as numeric),4)= TRUNC(CAST(fd.endpositionlattitude as numeric),4) 
                               and TRUNC(CAST(endgeoaddr.longitude as numeric),4) = TRUNC(CAST(fd.endpositionlongitude as numeric),4)
			)
		SELECT

             cmb.*
        FROM

            cte_combine cmb";

                List<FleetFuelDetails> lstFleetDetails = (List<FleetFuelDetails>)await _dataMartdataAccess.QueryAsync<FleetFuelDetails>(queryFleetUtilization, parameterOfFilters);

                if (isLiveFleetRequired && lstFleetDetails?.Count > 0)
                {

                    // new way To pull respective trip fleet position (One DB call for batch of 1000 trips)
                    string[] tripIds = lstFleetDetails.Select(item => item.Tripid).ToArray();
                    List<LiveFleetPosition> lstLiveFleetPosition = await GetLiveFleetPosition(tripIds);
                    if (lstLiveFleetPosition.Count > 0)
                        foreach (FleetFuelDetails trip in lstFleetDetails)
                        {
                            trip.LiveFleetPosition = lstLiveFleetPosition.Where(fleet => fleet.TripId == trip.Tripid).ToList();
                        }

                    /** Old way To pull respective trip fleet position
                    foreach (var item in data)
                    {
                        await GetLiveFleetPosition(item);
                    }
                    */
                }
                List<TripAlert> lstTripAlert = await GetTripAlert(fleetFuelFilters.StartDateTime, fleetFuelFilters.EndDateTime, fleetFuelFilters.VINs);
                if (lstTripAlert.Count() > 0)
                {
                    foreach (FleetFuelDetails trip in lstFleetDetails)
                    {
                        trip.TripAlert = lstTripAlert.Where(fleet => fleet.TripId == trip.Tripid).ToList();
                    }
                }
                return lstFleetDetails?.Count > 0 ? lstFleetDetails : new List<FleetFuelDetails>();
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        public async Task<List<FleetFuelDetails>> GetFleetFuelTripDetailsByDriver(FleetFuelFilterDriver fleetFuelFiltersDriver)
        {
            try
            {
                var parameterOfFilters = new DynamicParameters();
                parameterOfFilters.Add("@FromDate", fleetFuelFiltersDriver.StartDateTime);
                parameterOfFilters.Add("@ToDate", fleetFuelFiltersDriver.EndDateTime);
                parameterOfFilters.Add("@Vin", fleetFuelFiltersDriver.VIN);
                parameterOfFilters.Add("@DriverId", fleetFuelFiltersDriver.DriverId);

                string queryFleetUtilization = @"WITH CTE_FleetDeatils as
			(
				Select distinct
					v.VIN
                  , ts.id as Id
                  ,trip_id                                                                 as tripid
				  , driver1_id                                                             as tripDriverId
				  , 1                                                         as numberoftrips
				  , 1 as totalworkingdays
				  , (etl_gps_distance)                                                  as etl_gps_distance
				  , (etl_gps_distance)                                              as veh_message_distance
				  , (etl_gps_distance::numeric)/case when (etl_gps_trip_time::numeric) >0 then (etl_gps_trip_time) else 1 end as average_speed
				  , (max_speed)                                                         as max_speed
				  , (average_gross_weight_comb)                                      as average_gross_weight_comb
				  , (etl_gps_fuel_consumed)                                                  as fuel_consumed
				  , (etl_gps_fuel_consumed ::decimal/case when (etl_gps_distance::decimal) >0 then (etl_gps_distance::decimal) else 1 end) as fuel_consumption
				  , (co2_emission)                                                      as co2_emission
                  , idle_duration as idle_duration
				  , case when (((end_time_stamp - start_time_stamp)/1000)::numeric)>0 then ((idle_duration/((end_time_stamp - start_time_stamp)/1000)::numeric) *100) else 0 end as idle_duration_percentage
				  , case when (((end_time_stamp - start_time_stamp)/1000)::numeric)>0 then ((pto_duration/((end_time_stamp - start_time_stamp)/1000)::numeric) *100)  else 0 end   as pto_duration
				  , case when (((end_time_stamp - start_time_stamp)/1000)::numeric)>0 then ((harsh_brake_duration/((end_time_stamp - start_time_stamp)/1000)::numeric) *100)  else 0 end as harsh_brake_duration
				  --, case when (((end_time_stamp - start_time_stamp)/1000)::numeric)>0 then ((heavy_throttle_duration/((end_time_stamp - start_time_stamp)/1000)::numeric) *100)  else 0 end as heavy_throttle_duration
                  , (heavy_throttle_duration)  as heavy_throttle_duration
				  , case when etl_gps_distance>0 then ((cruise_control_distance_30_50 / etl_gps_distance) * 100) else 0 end         as cruise_control_distance_30_50
				  , case when etl_gps_distance>0 then ((cruise_control_distance_50_75  / etl_gps_distance) * 100)  else 0 end         as cruise_control_distance_50_75
				  , case when etl_gps_distance>0 then ((cruise_control_distance_more_than_75  / etl_gps_distance) * 100)  else 0 end  as cruise_control_distance_more_than_75
				  , (average_traffic_classification)                                    as average_traffic_classification
				  , (cc_fuel_consumption)                                               as cc_fuel_consumption
				  , (fuel_consumption_cc_non_active)                                    as fuel_consumption_cc_non_active
				  , (idling_consumption)                                                as idling_consumption
				  , (dpa_score)                                                         as dpa_score
                  , start_time_stamp                                                       as StartDate
                  , end_time_stamp                                                         as EndDate
                     , start_position_lattitude as startpositionlattitude
				  , start_position_longitude as startpositionlongitude
				  , end_position_lattitude as endpositionlattitude
				  , end_position_longitude as endpositionlongitude
                  , v_cruise_control_dist_for_cc_fuel_consumption as CCFuelDistance
                  , v_cruise_control_fuel_consumed_for_cc_fuel_consumption as CCFuelConsumed
                  , (veh_message_brake_duration) 									   as FootBrake
                  , (veh_message_dpabraking_score/
                   (case when veh_message_dpabraking_count > 0 then veh_message_dpabraking_count
                     else 1 END)) 	   as DPABrakingScore 
                  , (veh_message_dpaanticipation_score/
                   (case when veh_message_dpaanticipation_count > 0 then veh_message_dpaanticipation_count
                     else 1 END)) 	   as DPAAnticipationScore 
                  , (case when veh_message_dpabraking_score > 0 then 1 else 0 end)  as numoftripswithdpabrakingscore
                  , (case when veh_message_dpaanticipation_score>0 then 1 else 0 end)  as numoftripswithdpaanticipationscore
				From
					tripdetail.trip_statistics ts
				Join master.vehicle v on ts.vin=v.vin                                                                    
                WHERE ts.end_time_stamp >= v.reference_date and (end_time_stamp >= @FromDate 
							   and end_time_stamp<= @ToDate) and v.VIN =@Vin and driver1_id =@DriverId
				
			)
		  , cte_combine as
			(
				SELECT distinct
					vh.name            as VehicleName
                   ,  fd.Id
                   ,tripid
				  , fd.vin             as VIN
				  , vh.registration_no as VehicleRegistrationNo
				  , fd.tripDriverId
				  , round ( fd.etl_gps_distance,2)                       as Distance
				  , round ((fd.veh_message_distance),2) as AverageDistancePerDay
				  , round (fd.average_speed,7)                           as AverageSpeed
				  , max_speed                                            as MaxSpeed
				  , numberoftrips                                        as NumberOfTrips
				  , round (fd.average_gross_weight_comb,2)               as AverageGrossWeightComb
				  , round((fd.fuel_consumed),2)         As FuelConsumed
				  , round((fd.fuel_consumption),4)         As FuelConsumption
				  , round((fd.co2_emission),2)         As CO2Emission
				  , round(fd.idle_duration,2)                            as IdleDuration
                  , round(fd.idle_duration_percentage,2)                            as IdleDurationPercentage
				  , round(fd.pto_duration,2)                             as PTODuration
				  , round((fd.harsh_brake_duration),2)    As HarshBrakeDuration
				  , round((fd.heavy_throttle_duration),2)    As HeavyThrottleDuration
				  , round(fd.cruise_control_distance_30_50,2)                as CruiseControlDistance3050
				  , round(fd.cruise_control_distance_50_75,2)                as CruiseControlDistance5075
				  , round(fd.cruise_control_distance_more_than_75,2)         as CruiseControlDistance75
				  , round(fd.average_traffic_classification,4)  * 1000             as AverageTrafficClassification
				  , round(fd.cc_fuel_consumption/100,5)                          as CCFuelConsumption
				  , case when (fd.etl_gps_distance::decimal - fd.CCFuelDistance::decimal)>0 then round(((fd.fuel_consumed - fd.CCFuelConsumed)::decimal/(fd.etl_gps_distance - fd.CCFuelDistance)::decimal),4) else round(fd.fuel_consumption,5) end as FuelconsumptionCCnonactive
				  , idling_consumption                                     as IdlingConsumption
				  , dpa_score                                              as DPAScore
                  , StartDate
                  , EndDate
                  ,  startpositionlattitude
				  ,  startpositionlongitude
				  , endpositionlattitude
				  , endpositionlongitude
                  , round(fd.CCFuelDistance,2) as CCFuelDistance
                  , round(fd.CCFuelConsumed,2) as CCFuelConsumed
                  , round(fd.etl_gps_distance - fd.CCFuelDistance,2) as CCFuelDistanceNotActive
                  , round(fd.fuel_consumed - fd.CCFuelConsumed,2) as CCFuelConsumedNotActive
                 , coalesce(startgeoaddr.address,'') AS StartPosition
                 , coalesce(endgeoaddr.address,'') AS EndPosition
                , round(fd.FootBrake,2) 	as FootBrake
                ,case when numoftripswithdpabrakingscore>0 then  round((fd.DPABrakingScore/numoftripswithdpabrakingscore),2)
                 else fd.DPABrakingScore  end  		  as DPABrakingScore 
                ,case when numoftripswithdpaanticipationscore>0 then  round((fd.DPAAnticipationScore/numoftripswithdpaanticipationscore),2)
                 else fd.DPAAnticipationScore  end  		  as DPAAnticipationScore
				FROM
					CTE_FleetDeatils fd
				    left join
						master.vehicle vh
						on
							fd.VIN =vh.VIN
                     left JOIN master.geolocationaddress as startgeoaddr
                            on TRUNC(CAST(startgeoaddr.latitude as numeric),4)= TRUNC(CAST(fd.startpositionlattitude as numeric),4) 
                               and TRUNC(CAST(startgeoaddr.longitude as numeric),4) = TRUNC(CAST(fd.startpositionlongitude as numeric),4)
                     left JOIN master.geolocationaddress as endgeoaddr
                            on TRUNC(CAST(endgeoaddr.latitude as numeric),4)= TRUNC(CAST(fd.endpositionlattitude as numeric),4) 
                               and TRUNC(CAST(endgeoaddr.longitude as numeric),4) = TRUNC(CAST(fd.endpositionlongitude as numeric),4) 

			)
		SELECT
            COALESCE(dr.first_name,'') || ' ' || COALESCE(dr.last_name,'') as DriverName
			,cmb.tripDriverId || case when COALESCE(dr.driver_id,'~*') != '~*'
                                                        then '' Else '~*' END   as driverid
                                                    , cmb.*
                                                  FROM
                                                  	cte_combine cmb
                                                  	left join
                                                  		master.driver dr
                                                  		on
                                                  dr.driver_id = cmb.tripDriverId";

                List<FleetFuelDetails> lstFleetDetails = (List<FleetFuelDetails>)await _dataMartdataAccess.QueryAsync<FleetFuelDetails>(queryFleetUtilization, parameterOfFilters);
                if (lstFleetDetails?.Count > 0)
                {

                    // new way To pull respective trip fleet position (One DB call for batch of 1000 trips)
                    string[] tripIds = lstFleetDetails.Select(item => item.Tripid).ToArray();
                    List<LiveFleetPosition> lstLiveFleetPosition = await GetLiveFleetPosition(tripIds);
                    if (lstLiveFleetPosition.Count > 0)
                        foreach (FleetFuelDetails trip in lstFleetDetails)
                        {
                            trip.LiveFleetPosition = lstLiveFleetPosition.Where(fleet => fleet.TripId == trip.Tripid).ToList();
                        }

                    /** Old way To pull respective trip fleet position
                    foreach (var item in data)
                    {
                        await GetLiveFleetPosition(item);
                    }
                    */
                }

                List<string> vins = new List<string>();
                vins.Add(fleetFuelFiltersDriver.VIN);
                List<TripAlert> lstTripAlert = await GetTripAlert(fleetFuelFiltersDriver.StartDateTime, fleetFuelFiltersDriver.EndDateTime, vins);
                if (lstTripAlert.Count() > 0)
                {
                    foreach (FleetFuelDetails trip in lstFleetDetails)
                    {
                        trip.TripAlert = lstTripAlert.Where(fleet => fleet.TripId == trip.Tripid).ToList();
                    }
                }
                return lstFleetDetails?.Count > 0 ? lstFleetDetails : new List<FleetFuelDetails>();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

    }
}
