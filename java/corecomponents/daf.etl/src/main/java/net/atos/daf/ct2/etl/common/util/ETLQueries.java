package net.atos.daf.ct2.etl.common.util;

public class ETLQueries {
	
	//public static final String TRIP_INDEX_DUPLICATE_QRY = "select idxData.f0 , idxData.f1 , idxData.f2 , idxData.f3, CAST(idxData.f4 as Double) as f4  FROM indexData idxData group by idxData.f0, idxData.f1, idxData.f2, idxData.f3, idxData.f4 ";
	// order by idxData.f6
	public static final String TRIP_INDEX_DUPLICATE_QRY = " select distinct idxData.f0 as tripId , idxData.f1 , idxData.f2 , idxData.f3, CAST(idxData.f4 as Double) as f4, idxData.f6 as f6, idxData.f7 as f7, idxData.f8 as f8, idxData.f9 as grossWtRecCnt, idxData.f10 as driverId  FROM indexData idxData";
		
	//, vid, driver2Id, vTachographSpeed, vGrossWeightCombination, jobNm, evtDateTime, vDist, increment
	//proctime()
	public static final String TRIP_LAG_QRY = "select unqData.f0, unqData.f1 , unqData.f2 , unqData.f3, unqData.f4 as f4, unqData.f6 as currentVdst, LAG(unqData.f6) OVER ( partition BY unqData.f1 ORDER BY proctime()) AS pastVdst from firstLevelAggrData unqData ";
	//tripId, vid, driver2Id, vTachographSpeed, vGrossWeightCombination,vDist, previousVdist, increment, formula for avgWt
	
	public static final String TRIP_INDEX_AGGREGATION_QRY = "select idxData.f0 as tripId, idxData.f1 as vid, idxData.f2 as driver2Id, MAX(idxData.f3) as maxSpeed, AVG(CAST(idxData.f4 as Double)) as avgWt, SUM(idxData.f8) as avgGrossWtSum,  SUM(idxData.f4) as vGrossWtSum, SUM(idxData.f9) as indxCnt,  MAX(idxData.f10) as driverId, SUM(idxData.f11) as avgGrossWtCnt FROM grossWtCombData idxData group by idxData.f0, idxData.f1, idxData.f2 ";
	
	public static final String TRIP_STATUS_AGGREGATION_QRY = " select stsData.tripId, stsData.vid, stsData.vin, stsData.startDateTime, stsData.endDateTime, stsData.gpsTripDist"
			+ ", stsData.tripCalGpsVehDistDiff as tripCalDist, stsData.vIdleDuration, stsData.vTripIdlePTODuration, stsData.vTripIdleWithoutPTODuration "
			//+ ", if(stsData.tripCalGpsVehTimeDiff <> 0, (CAST(stsData.tripCalGpsVehDistDiff as Double)/stsData.tripCalVehTimeDiffInHr), stsData.tripCalGpsVehDistDiff) as tripCalAvgSpeed"
			+ ", if(stsData.tripCalGpsVehTimeDiff <> 0, (CAST(stsData.tripCalGpsVehDistDiff as Double)/stsData.tripCalGpsVehTimeDiff), stsData.tripCalGpsVehDistDiff) as tripCalAvgSpeed"
			+ ", stsData.gpsStartVehDist, stsData.gpsStopVehDist, stsData.gpsStartLatitude, stsData.gpsStartLongitude, stsData.gpsEndLatitude"
			+ ", stsData.gpsEndLongitude, stsData.vUsedFuel, (stsData.vStopFuel - stsData.vStartFuel) as tripCalUsedFuel"
			+ ", stsData.vTripMotionDuration, ((stsData.tripCalGpsVehTimeDiff/1000) - stsData.vIdleDuration) as tripCalDrivingTm"
			+ ", stsData.receivedTimestamp, stsData.co2Emission as tripCalC02Emission "
			+ ", if(0 <> stsData.tripCalGpsVehDistDiff, (CAST(stsData.vUsedFuel as Double)/stsData.tripCalGpsVehDistDiff), stsData.vUsedFuel) as tripCalFuelConsumption"
			+ ", if(0 <> stsData.tripCalGpsVehTimeDiff, (CAST(stsData.vPTODuration as Double)/(stsData.tripCalGpsVehTimeDiff * 0.001)), stsData.vPTODuration) as tripCalPtoDuration"
			+ ", if(0 <> stsData.vBrakeDuration, (CAST(stsData.vHarshBrakeDuration as Double)/stsData.vBrakeDuration), stsData.vHarshBrakeDuration) as tripCalHarshBrakeDuration"
			+ ", if(0 <> stsData.vTripAccelerationTime, (CAST(stsData.vMaxThrottlePaddleDuration as Double)/stsData.vTripAccelerationTime), stsData.vMaxThrottlePaddleDuration) as tripCalHeavyThrottleDuration"
			+ ", stsData.tripCalCrsCntrlDist25To50, stsData.tripCalCrsCntrlDist50To75, stsData.tripCalCrsCntrlDistAbv75 "
			/*+ ", if((stsData.vCruiseControlDist > 30 AND stsData.vCruiseControlDist < 50), if(0 <> stsData.tripCalGpsVehDistDiff, CAST(stsData.vCruiseControlDist as Double)/stsData.tripCalGpsVehDistDiff, stsData.vCruiseControlDist), 0) as tripCalCrsCntrlDistBelow50"
			+ ", if((stsData.vCruiseControlDist > 50 AND stsData.vCruiseControlDist < 75), if(0 <> stsData.tripCalGpsVehDistDiff, CAST(stsData.vCruiseControlDist as Double)/stsData.tripCalGpsVehDistDiff, stsData.vCruiseControlDist), 0) as tripCalCrsCntrlDistAbv50"
			+ ", if((stsData.vCruiseControlDist > 75), if(0 <> stsData.tripCalGpsVehDistDiff, CAST(stsData.vCruiseControlDist as Double)/stsData.tripCalGpsVehDistDiff, stsData.vCruiseControlDist), 0) as tripCalCrsCntrlDistAbv75"*/
			+ ", if(0 <> stsData.tripCalGpsVehDistDiff, (CAST(stsData.vTripDPABrakingCount as Double)+ stsData.vTripDPAAnticipationCount)/stsData.tripCalGpsVehDistDiff, (stsData.vTripDPABrakingCount + stsData.vTripDPAAnticipationCount)) as tripCalAvgTrafficClsfn"
			+ ", if(0 <> stsData.vCruiseControlDist, (CAST(stsData.vCruiseControlFuelConsumed as Double)/stsData.vCruiseControlDist), stsData.vCruiseControlFuelConsumed) as tripCalCCFuelConsumption"
			+ ", stsData.vCruiseControlFuelConsumed , stsData.vCruiseControlDist, stsData.vIdleFuelConsumed, stsData.kafkaProcessingTS "
			+ ", (if( 0 <> (stsData.tripCalGpsVehDistDiff - stsData.vCruiseControlDist) ,(stsData.vUsedFuel - if(0 <> stsData.vCruiseControlDist, (CAST(stsData.vCruiseControlFuelConsumed as Double)/stsData.vCruiseControlDist), stsData.vCruiseControlDist) )/(stsData.tripCalGpsVehDistDiff - stsData.vCruiseControlDist) , 0)) as tripCalfuelNonActiveCnsmpt"
			+ ", (if(0 <> stsData.vTripDPABrakingCount, (CAST(stsData.vSumTripDPABrakingScore as Double)/stsData.vTripDPABrakingCount), 0) + if(0 <> stsData.vTripDPAAnticipationCount, (CAST(stsData.vSumTripDPAAnticipationScore as Double)/stsData.vTripDPAAnticipationCount), 0))/2 as tripCalDpaScore"
			+ ", stsData.driverId, stsData.tripCalGpsVehTimeDiff as tripCalGpsVehTime"
			+ ", stsData.tripProcessingTS, stsData.etlProcessingTS,  stsData.numberOfIndexMessage, stsData.vTripDPABrakingCount, stsData.vTripDPAAnticipationCount"
			+ ", stsData.vSumTripDPABrakingScore, stsData.vSumTripDPAAnticipationScore, stsData.vStopFuel, stsData.vStartFuel , stsData.vHarshBrakeDuration , stsData.vBrakeDuration "
			+ ", stsData.vMaxThrottlePaddleDuration, stsData.vTripAccelerationTime, stsData.vPTODuration"
			+ " FROM tripStsData stsData";
	
	public static final String TRIP_STATUS_AGGREGATION_QRY_BCKUP = " select stsData.tripId, stsData.vid, stsData.vin, stsData.startDateTime, stsData.endDateTime, stsData.gpsTripDist"
			+ ", stsData.tripCalGpsVehDistDiff as tripCalDist, stsData.vIdleDuration"
			//+ ", if(stsData.tripCalGpsVehTimeDiff <> 0, (CAST(stsData.tripCalGpsVehDistDiff as Double)/stsData.tripCalVehTimeDiffInHr), stsData.tripCalGpsVehDistDiff) as tripCalAvgSpeed"
			+ ", if(stsData.tripCalGpsVehTimeDiff <> 0, (CAST(stsData.tripCalGpsVehDistDiff as Double)/stsData.tripCalGpsVehTimeDiff), stsData.tripCalGpsVehDistDiff) as tripCalAvgSpeed"
			+ ", stsData.gpsStartVehDist, stsData.gpsStopVehDist, stsData.gpsStartLatitude, stsData.gpsStartLongitude, stsData.gpsEndLatitude"
			+ ", stsData.gpsEndLongitude, stsData.vUsedFuel, (stsData.vStopFuel - stsData.vStartFuel) as tripCalUsedFuel"
			+ ", stsData.vTripMotionDuration, ((stsData.tripCalGpsVehTimeDiff/1000) - stsData.vIdleDuration) as tripCalDrivingTm"
			+ ", stsData.receivedTimestamp, stsData.co2Emission as tripCalC02Emission "
			+ ", if(0 <> stsData.tripCalGpsVehDistDiff, (CAST(stsData.vUsedFuel as Double)/stsData.tripCalGpsVehDistDiff)*100, stsData.vUsedFuel) as tripCalFuelConsumption"
			+ ", if(0 <> stsData.tripCalGpsVehTimeDiff, (CAST(stsData.vPTODuration as Double)/(stsData.tripCalGpsVehTimeDiff * 0.001))*100, stsData.vPTODuration) as tripCalPtoDuration"
			+ ", if(0 <> stsData.vBrakeDuration, (CAST(stsData.vHarshBrakeDuration as Double)/stsData.vBrakeDuration)*100, stsData.vHarshBrakeDuration) as tripCalHarshBrakeDuration"
			+ ", if(0 <> stsData.vTripAccelerationTime, (CAST(stsData.vMaxThrottlePaddleDuration as Double)/stsData.vTripAccelerationTime)*100, stsData.vMaxThrottlePaddleDuration) as tripCalHeavyThrottleDuration"
			+ ", if((stsData.vCruiseControlDist > 30 AND stsData.vCruiseControlDist < 50), if(0 <> stsData.tripCalGpsVehDistDiff, CAST(stsData.vCruiseControlDist as Double)/stsData.tripCalGpsVehDistDiff, stsData.vCruiseControlDist), 0) as tripCalCrsCntrlDistBelow50"
			+ ", if((stsData.vCruiseControlDist > 50 AND stsData.vCruiseControlDist < 75), if(0 <> stsData.tripCalGpsVehDistDiff, CAST(stsData.vCruiseControlDist as Double)/stsData.tripCalGpsVehDistDiff, stsData.vCruiseControlDist), 0) as tripCalCrsCntrlDistAbv50"
			+ ", if((stsData.vCruiseControlDist > 75), if(0 <> stsData.tripCalGpsVehDistDiff, CAST(stsData.vCruiseControlDist as Double)/stsData.tripCalGpsVehDistDiff, stsData.vCruiseControlDist), 0) as tripCalCrsCntrlDistAbv75"
			+ ", if(0 <> stsData.tripCalGpsVehDistDiff, (CAST(stsData.vTripDPABrakingCount as Double)+ stsData.vTripDPAAnticipationCount)/stsData.tripCalGpsVehDistDiff, (stsData.vTripDPABrakingCount + stsData.vTripDPAAnticipationCount)) as tripCalAvgTrafficClsfn"
			+ ", if(0 <> stsData.vCruiseControlDist, (CAST(stsData.vCruiseControlFuelConsumed as Double)/stsData.vCruiseControlDist)*100, stsData.vCruiseControlFuelConsumed) as tripCalCCFuelConsumption"
			+ ", stsData.vCruiseControlFuelConsumed , stsData.vCruiseControlDist, stsData.vIdleFuelConsumed, stsData.kafkaProcessingTS "
			+ ", (if( 0 <> (stsData.tripCalGpsVehDistDiff - stsData.vCruiseControlDist) ,(stsData.vUsedFuel - if(0 <> stsData.vCruiseControlDist, (CAST(stsData.vCruiseControlFuelConsumed as Double)/stsData.vCruiseControlDist), stsData.vCruiseControlDist) )/(stsData.tripCalGpsVehDistDiff - stsData.vCruiseControlDist)* 100 , 0)) as tripCalfuelNonActiveCnsmpt"
			+ ", (CAST(stsData.vSumTripDPABrakingScore as Double) + stsData.vSumTripDPAAnticipationScore)/2 as tripCalDpaScore, stsData.driverId, stsData.tripCalGpsVehTimeDiff as tripCalGpsVehTime"
			+ ", stsData.tripProcessingTS, stsData.etlProcessingTS,  stsData.numberOfIndexMessage, stsData.vTripDPABrakingCount, stsData.vTripDPAAnticipationCount"
			+ ", stsData.vSumTripDPABrakingScore, stsData.vSumTripDPAAnticipationScore, stsData.vStopFuel, stsData.vStartFuel , stsData.vHarshBrakeDuration , stsData.vBrakeDuration "
			+ " FROM tripStsData stsData";

	//indxData.f4 vGrossWeightCombination
	public static final String CONSOLIDATED_TRIP_QRY = " select stsData.tripId, stsData.vid, if(stsData.vin IS NOT NULL, stsData.vin, if(stsData.vid IS NOT NULL, stsData.vid, 'UNKNOWN')) as vin, stsData.startDateTime, stsData.endDateTime, stsData.gpsTripDist"
			+ ", stsData.tripCalDist, stsData.vIdleDuration, if(0 <> indxData.f7, indxData.f6/indxData.f7, 0) vGrossWeightCombination"
			+ ", stsData.tripCalAvgSpeed, stsData.gpsStartVehDist, stsData.gpsStopVehDist, stsData.gpsStartLatitude, stsData.gpsStartLongitude, stsData.gpsEndLatitude"
			+ ", stsData.gpsEndLongitude, stsData.vUsedFuel, stsData.tripCalUsedFuel, stsData.vTripMotionDuration"
			+ ", stsData.tripCalDrivingTm, stsData.receivedTimestamp, stsData.tripCalC02Emission, stsData.tripCalFuelConsumption"
			+ ", indxData.f3 as vTachographSpeed, if(0 <> stsData.tripCalDist, indxData.f5/stsData.tripCalDist, 0) as tripCalAvgGrossWtComb"
			+ ", stsData.tripCalPtoDuration, stsData.tripCalHarshBrakeDuration, stsData.tripCalHeavyThrottleDuration"
			+ ", stsData.tripCalCrsCntrlDist25To50, stsData.tripCalCrsCntrlDist50To75, stsData.tripCalCrsCntrlDistAbv75"
			+ ", stsData.tripCalAvgTrafficClsfn, stsData.tripCalCCFuelConsumption, stsData.vCruiseControlFuelConsumed "
			+ ", stsData.vCruiseControlDist, stsData.vIdleFuelConsumed, stsData.tripCalfuelNonActiveCnsmpt"
			+ ", stsData.tripCalDpaScore, if(stsData.driverId IS NOT NULL, stsData.driverId, indxData.f8) as driverId, indxData.f2 as driver2Id, stsData.tripCalGpsVehTime"
			+ ", stsData.tripProcessingTS, stsData.etlProcessingTS, stsData.kafkaProcessingTS, indxData.f6 as vGrossWtSum, indxData.f7 as numberOfIndexMessage "
			+ ", stsData.vTripDPABrakingCount, stsData.vTripDPAAnticipationCount, stsData.vSumTripDPABrakingScore, stsData.vSumTripDPAAnticipationScore"
			+ ", stsData.vStopFuel, stsData.vStartFuel , stsData.vHarshBrakeDuration , stsData.vBrakeDuration, stsData.vTripIdlePTODuration, stsData.vTripIdleWithoutPTODuration "
			+ ", stsData.vPTODuration, stsData.vMaxThrottlePaddleDuration, stsData.vTripAccelerationTime, indxData.f9 as vGrossWtCmbCount  "
			+ " FROM stsAggregatedData stsData LEFT JOIN secondLevelAggrData indxData ON stsData.tripId = indxData.f0 ";
	
	public static final String CONSOLIDATED_TRIP_QRY_BACKUP = " select stsData.tripId, stsData.vid "
			+ ", if(stsData.vin IS NOT NULL, stsData.vin, if(stsData.vid IS NOT NULL, stsData.vid, 'UNKNOWN')) as vin"
			+ ", if(stsData.startDateTime IS NOT NULL, stsData.startDateTime, 0) as startDateTime, if(stsData.endDateTime IS NOT NULL, stsData.endDateTime, 0) as endDateTime "
			+ ", if(stsData.gpsTripDist IS NOT NULL, stsData.gpsTripDist, 0) as gpsTripDist, if(stsData.tripCalDist IS NOT NULL, stsData.tripCalDist, 0) as tripCalDist"
			+ ", if(stsData.vIdleDuration IS NOT NULL, stsData.vIdleDuration, 0) as vIdleDuration, if(indxData.f4 IS NOT NULL, indxData.f4, 0) as vGrossWeightCombination"
			+ ", if(stsData.tripCalAvgSpeed IS NOT NULL, stsData.tripCalAvgSpeed, 0) as tripCalAvgSpeed, if(stsData.gpsStartVehDist IS NOT NULL, stsData.gpsStartVehDist, 0) as gpsStartVehDist"
			+ ", if(stsData.gpsStopVehDist IS NOT NULL, stsData.gpsStopVehDist, 0) as gpsStopVehDist, if(stsData.gpsStartLatitude IS NOT NULL, stsData.gpsStartLatitude, 0) as gpsStartLatitude"
			+ ", if(stsData.gpsStartLongitude IS NOT NULL, stsData.gpsStartLongitude, 0) as gpsStartLongitude, if(stsData.gpsEndLatitude IS NOT NULL, stsData.gpsEndLatitude, 0) as gpsEndLatitude"
			+ ", if(stsData.gpsEndLongitude IS NOT NULL, stsData.gpsEndLongitude, 0) as gpsEndLongitude, if(stsData.vUsedFuel IS NOT NULL, stsData.vUsedFuel, 0) as vUsedFuel"
			+ ", if(stsData.tripCalUsedFuel IS NOT NULL, stsData.tripCalUsedFuel, 0) as tripCalUsedFuel, if(stsData.asvTripMotionDuration IS NOT NULL, stsData.asvTripMotionDuration, 0) asvTripMotionDuration"
			+ ", if(stsData.tripCalDrivingTm IS NOT NULL, stsData.tripCalDrivingTm, 0) as tripCalDrivingTm, if(stsData.receivedTimestamp IS NOT NULL, stsData.receivedTimestamp, 0) as receivedTimestamp"
			+ ", if(stsData.tripCalC02Emission IS NOT NULL, stsData.tripCalC02Emission, 0) as tripCalC02Emission, if(stsData.tripCalFuelConsumption IS NOT NULL, stsData.tripCalFuelConsumption, 0) as tripCalFuelConsumption"
			+ ", if(indxData.f3 IS NOT NULL, indxData.f3, 0) as vTachographSpeed, if(0 <> stsData.tripCalDist, indxData.f5/stsData.tripCalDist, 0) as tripCalAvgGrossWtComb"
			+ ", if(stsData.tripCalPtoDuration IS NOT NULL, stsData.tripCalPtoDuration, 0) as tripCalPtoDuration, if(stsData.tripCalHarshBrakeDuration IS NOT NULL, stsData.tripCalHarshBrakeDuration, 0) as tripCalHarshBrakeDuration"
			+ ", if(stsData.tripCalHeavyThrottleDuration IS NOT NULL, stsData.tripCalHeavyThrottleDuration, 0) as tripCalHeavyThrottleDuration"
			+ ", if(stsData.tripCalCrsCntrlDistBelow50 IS NOT NULL, stsData.tripCalCrsCntrlDistBelow50, 0) as tripCalCrsCntrlDistBelow50"
			+ ", if(stsData.tripCalCrsCntrlDistAbv50 IS NOT NULL, stsData.tripCalCrsCntrlDistAbv50, 0) as tripCalCrsCntrlDistAbv50"
			+ ", if(stsData.tripCalCrsCntrlDistAbv75 IS NOT NULL, stsData.tripCalCrsCntrlDistAbv75, 0) as tripCalCrsCntrlDistAbv75"
			+ ", if(stsData.tripCalAvgTrafficClsfn IS NOT NULL, stsData.tripCalAvgTrafficClsfn, 0) as tripCalAvgTrafficClsfn"
			+ ", if(stsData.tripCalCCFuelConsumption IS NOT NULL, stsData.tripCalCCFuelConsumption, 0) as tripCalCCFuelConsumption"
			+ ", if(stsData.vCruiseControlFuelConsumed IS NOT NULL, stsData.vCruiseControlFuelConsumed, 0) as vCruiseControlFuelConsumed "
			+ ", if(stsData.vCruiseControlDist IS NOT NULL, stsData.vCruiseControlDist, 0) as vCruiseControlDist"
			+ ", if(stsData.vIdleFuelConsumed IS NOT NULL, stsData.vIdleFuelConsumed, 0) as vIdleFuelConsumed"
			+ ", if(stsData.tripCalfuelNonActiveCnsmpt IS NOT NULL, stsData.tripCalfuelNonActiveCnsmpt, 0) as tripCalfuelNonActiveCnsmpt"
			+ ", if(stsData.tripCalDpaScore IS NOT NULL, stsData.tripCalDpaScore, 0) as tripCalDpaScore, stsData.driverId"
			+ ", indxData.f2 as driver2Id, if(stsData.tripCalGpsVehTime IS NOT NULL, stsData.tripCalGpsVehTime, 0) as tripCalGpsVehTime"
			+ ", if(stsData.tripProcessingTS IS NOT NULL, stsData.tripProcessingTS, 0) as tripProcessingTS, if(stsData.etlProcessingTS IS NOT NULL, stsData.etlProcessingTS, 0) as etlProcessingTS"
			+ ", if(stsData.kafkaProcessingTS IS NOT NULL, stsData.kafkaProcessingTS, 0) as kafkaProcessingTS"
			+ " FROM stsAggregatedData stsData LEFT JOIN aggrIndxData indxData ON stsData.tripId = indxData.f0 ";

	public static final String CO2_COEFFICIENT_QRY = " select coefficient from master.co2coefficient c join master.vehicle v on c.fuel_type = v.fuel_type and vin = ? ";
	
	public static final String TRIP_QRY = " select tripId ,vid ,vin ,startDateTime ,endDateTime ,gpsTripDist ,tripCalDist ,vIdleDuration, tripCalAvgSpeed "
			+ ", vGrossWeightCombination ,gpsStartVehDist ,gpsStopVehDist ,gpsStartLatitude ,gpsStartLongitude ,gpsEndLatitude ,gpsEndLongitude ,vUsedFuel "
			+ ", tripCalUsedFuel ,vTripMotionDuration ,tripCalDrivingTm ,receivedTimestamp ,tripCalC02Emission ,tripCalFuelConsumption ,vTachographSpeed "
			+ ", tripCalAvgGrossWtComb ,tripCalPtoDuration ,tripCalHarshBrakeDuration ,tripCalHeavyThrottleDuration, tripCalCrsCntrlDist25To50 "
			+ ", tripCalCrsCntrlDist50To75, tripCalCrsCntrlDistAbv75 ,tripCalAvgTrafficClsfn ,tripCalCCFuelConsumption ,vCruiseControlFuelConsumed ,vCruiseControlDist "
			+ ", vIdleFuelConsumed ,tripCalfuelNonActiveCnsmpt ,tripCalDpaScore, driverId, driver2Id, tripCalGpsVehTime ,tripProcessingTS ,etlProcessingTS "
			+ ", kafkaProcessingTS ,vGrossWtSum ,numberOfIndexMessage, vTripDPABrakingCount, vTripDPAAnticipationCount, vSumTripDPABrakingScore, vSumTripDPAAnticipationScore "
			+ ", vHarshBrakeDuration, vBrakeDuration, vTripIdlePTODuration, vTripIdleWithoutPTODuration, vPTODuration, vMaxThrottlePaddleDuration, vTripAccelerationTime "
			+ " from tripAggrData ";
	
	//tripCalPtoDuration, tripCalHeavyThrottleDuration
	public static final String ECOSCORE_QRY = " select tripId, vin, startDateTime, endDateTime, driverId, tripCalDist "
			+ ", vTripDPABrakingCount, vTripDPAAnticipationCount, vSumTripDPABrakingScore, vSumTripDPAAnticipationScore , tripCalAvgGrossWtComb "
			+ ", tripCalUsedFuel, vPTODuration, vIdleDuration, vMaxThrottlePaddleDuration, vCruiseControlDist,  tripCalCrsCntrlDist25To50 "
			+ ",  tripCalCrsCntrlDist50To75, tripCalCrsCntrlDistAbv75, vGrossWtSum as tachoVGrossWtCmbSum , vHarshBrakeDuration, vBrakeDuration"
			+ ", tripProcessingTS ,etlProcessingTS, kafkaProcessingTS, vTripAccelerationTime, vGrossWtCmbCount from tripAggrDataForEcoScore ";
	
	public static final String TRIP_INSERT_STATEMENT = "INSERT INTO tripdetail.trip_statistics( trip_id, vin, start_time_stamp, end_time_stamp, veh_message_distance, etl_gps_distance, idle_duration"
			+ ", average_speed, average_weight, start_odometer, last_odometer, start_position_lattitude, start_position_longitude, end_position_lattitude"
			+ ", end_position_longitude, veh_message_fuel_consumed, etl_gps_fuel_consumed, veh_message_driving_time"
			+ ", etl_gps_driving_time, message_received_timestamp, message_inserted_into_kafka_timestamp, etl_trip_record_insertion_time, message_processed_by_etl_process_timestamp"
			+ ", co2_emission, fuel_consumption, max_speed, average_gross_weight_comb, pto_duration, harsh_brake_duration, heavy_throttle_duration"
			+ ", cruise_control_distance_30_50, cruise_control_distance_50_75, cruise_control_distance_more_than_75"
			+ ", average_traffic_classification, cc_fuel_consumption, v_cruise_control_fuel_consumed_for_cc_fuel_consumption, v_cruise_control_dist_for_cc_fuel_consumption"
			+ ", fuel_consumption_cc_non_active, idling_consumption, dpa_score, driver1_id, driver2_id, etl_gps_trip_time, is_ongoing_trip, msg_gross_weight_combinition"
			+ ", no_of_total_index_message, veh_message_pto_duration, veh_message_harsh_brake_duration, veh_message_brake_duration, veh_message_max_throttle_paddle_duration"
			+ ", veh_message_accelerationt_time, veh_message_dpabraking_count, veh_message_dpaanticipation_count, veh_message_dpabraking_score, veh_message_dpaanticipation_score"
			+ ", veh_message_idle_without_ptoduration, veh_message_idle_ptoduration) "
			+ "  VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"
			+ "  ON CONFLICT (trip_id) "
			+ "  DO UPDATE SET  vin = ?, start_time_stamp = ?, end_time_stamp = ?, veh_message_distance = ?, etl_gps_distance = ?, idle_duration = ?, average_speed = ?"
			+ ", average_weight = ?, start_odometer = ?, last_odometer = ?, start_position_lattitude = ?, start_position_longitude = ?, end_position_lattitude = ?"
			+ ", end_position_longitude = ?, veh_message_fuel_consumed = ?, etl_gps_fuel_consumed = ?"
			+ ", veh_message_driving_time = ?, etl_gps_driving_time = ?,message_received_timestamp = ?, message_inserted_into_kafka_timestamp =?, etl_trip_record_insertion_time = ?"
			+ ", message_processed_by_etl_process_timestamp = ?, co2_emission = ?, fuel_consumption = ?, max_speed = ?, average_gross_weight_comb = ?"
			+ ", pto_duration = ?, harsh_brake_duration = ?, heavy_throttle_duration = ?, cruise_control_distance_30_50 = ?"
			+ ", cruise_control_distance_50_75 = ?, cruise_control_distance_more_than_75 = ?, average_traffic_classification = ?"
			+ ", cc_fuel_consumption = ?, v_cruise_control_fuel_consumed_for_cc_fuel_consumption = ?, v_cruise_control_dist_for_cc_fuel_consumption = ?"
			+ ", fuel_consumption_cc_non_active = ?, idling_consumption = ?, dpa_score = ?, driver1_id = ?, driver2_id = ?, etl_gps_trip_time = ?, is_ongoing_trip = ?"
			+ ", msg_gross_weight_combinition = ?, no_of_total_index_message =?, veh_message_pto_duration = ?, veh_message_harsh_brake_duration = ?, veh_message_brake_duration = ?"
			+ ", veh_message_max_throttle_paddle_duration = ?, veh_message_accelerationt_time = ?, veh_message_dpabraking_count = ?, veh_message_dpaanticipation_count = ?"
			+ ", veh_message_dpabraking_score = ?, veh_message_dpaanticipation_score = ?, veh_message_idle_without_ptoduration = ?, veh_message_idle_ptoduration = ? ";
/*
	public static final String ECOSCORE_INSERT_STATEMENT = "INSERT INTO tripdetail.ecoscoredata( trip_id, vin, start_time, end_time, driver1_id, driver2_id"
			+ ", etl_trip_distance, dpa_braking_score, dpa_braking_count, dpa_anticipation_score, dpa_anticipation_count, gross_weight_combination, used_fuel"
			+ ", pto_duration, idle_duration, heavy_throttle_pedal_duration, start_fuel, stop_fuel ) "
			+ "  VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"
			+ "  ON CONFLICT (trip_id) "
			+ "  DO UPDATE SET  vin = ?, start_time = ?, end_time = ?, driver1_id = ?, driver2_id = ?, etl_trip_distance = ?, dpa_braking_score = ?"
			+ ", dpa_braking_count = ?, dpa_anticipation_score = ?, dpa_anticipation_count = ?, gross_weight_combination = ?, used_fuel = ?, pto_duration = ?"
			+ ", idle_duration = ?, heavy_throttle_pedal_duration = ?, start_fuel = ?"
			+ ", stop_fuel = ? ";*/
	
	public static final String ECOSCORE_INSERT_STATEMENT = "INSERT INTO tripdetail.ecoscoredata( trip_id, vin, start_time, end_time, driver1_id "
			+ ", trip_distance, dpa_braking_score, dpa_braking_count, dpa_anticipation_score, dpa_anticipation_count, gross_weight_combination_total, used_fuel"
			+ ", pto_duration, idle_duration, heavy_throttle_pedal_duration, cruise_control_usage, cruise_control_usage_30_50, cruise_control_usage_50_75 "
			+ ", cruise_control_usage_75, tacho_gross_weight_combination, harsh_brake_duration, brake_duration, created_at, granular_level_type"
			+ ", lastest_processed_message_time_stamp,  gross_weight_combination_count, trip_acceleration_time, is_ongoing_trip ) "
			+ "  VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ? )"
			+ "  ON CONFLICT (trip_id) "
			+ "  DO UPDATE SET  vin = ?, start_time = ?, end_time = ?, driver1_id = ?, trip_distance = ?, dpa_braking_score = ?"
			+ ", dpa_braking_count = ?, dpa_anticipation_score = ?, dpa_anticipation_count = ?, gross_weight_combination_total = ?, used_fuel = ?"
			+ ", pto_duration =?, idle_duration =?, heavy_throttle_pedal_duration =?, cruise_control_usage =?, cruise_control_usage_30_50 =?"
			+ ", cruise_control_usage_50_75 =?, cruise_control_usage_75 = ?, tacho_gross_weight_combination =?, harsh_brake_duration =?, brake_duration =?"
			+ ", modified_at=?, lastest_processed_message_time_stamp =?,  gross_weight_combination_count=?, trip_acceleration_time =? ";
}
