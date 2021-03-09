package net.atos.daf.etl.ct2.common.util;

public class ETLQueries {
	
	//public static final String TRIP_INDEX_DUPLICATE_QRY = "select idxData.f0 , idxData.f1 , idxData.f2 , idxData.f3, CAST(idxData.f4 as Double) as f4  FROM indexData idxData group by idxData.f0, idxData.f1, idxData.f2, idxData.f3, idxData.f4 ";
	// order by idxData.f6
	public static final String TRIP_INDEX_DUPLICATE_QRY = " select distinct idxData.f0 as tripId , idxData.f1 , idxData.f2 , idxData.f3, CAST(idxData.f4 as Double) as f4, idxData.f6 as f6, idxData.f7 as f7, idxData.f8 as f8  FROM indexData idxData";
		
	//, vid, driver2Id, vTachographSpeed, vGrossWeightCombination, jobNm, evtDateTime, vDist, increment
	//proctime()
	public static final String TRIP_LAG_QRY = "select unqData.f0, unqData.f1 , unqData.f2 , unqData.f3, unqData.f4 as f4, unqData.f6 as currentVdst, LAG(unqData.f6) OVER ( partition BY unqData.f1 ORDER BY proctime()) AS pastVdst from firstLevelAggrData unqData ";
	//tripId, vid, driver2Id, vTachographSpeed, vGrossWeightCombination,vDist, previousVdist, increment, formula for avgWt
	
	public static final String TRIP_LAG_QRY_NEW = "SELECT unqData.f0, unqData.f1 , unqData.f2 , unqData.f3, unqData.f4 , unqData.f6 as currentVdst, LAG(unqData.f6) OVER x as prevVdist, unqData.f7, cast(0 as Double) as avgWt from firstLevelAggrData unqData WINDOW x AS( partition BY unqData.f1 ORDER BY proctime())";
	
	public static final String TRIP_INDEX_AGGREGATION_QRY = "select idxData.f0 as tripId, idxData.f1 as vid, idxData.f2 as driver2Id, MAX(idxData.f3) as maxSpeed, AVG(CAST(idxData.f4 as Double)) as avgWt, SUM(idxData.f8) as avgGrossWtSum FROM grossWtCombData idxData group by idxData.f0, idxData.f1, idxData.f2 ";
	
	public static final String TRIP_INDEX_AGGREGATION_QRY_ISSUE = "select idxData.f0 as tripId, idxData.f1 as vid, idxData.f2 as driver2Id, MAX(idxData.f3) as maxSpeed, AVG(CAST(idxData.f4 as Double)) as avgWt, cast(0 as Double) as avgGrossWtSum FROM firstLevelAggrData idxData group by idxData.f0, idxData.f1, idxData.f2 ";
	
	public static final String CONSOLIDATED_TRIP_QRY_BCKUP = " select stsData.tripId, stsData.vid, stsData.vin, stsData.startDateTime, stsData.endDateTime, stsData.gpsTripDist"
			+ ", stsData.tripCalGpsVehDistDiff as tripCalDist, stsData.vIdleDuration, indxData.f4 vGrossWeightCombination"
			+ ", if(stsData.tripCalGpsVehTimeDiff <> 0, (CAST(stsData.tripCalGpsVehDistDiff as Double)/stsData.tripCalVehTimeDiffInHr), stsData.tripCalGpsVehTimeDiff) as tripCalAvgSpeed"
			+ ", stsData.gpsStartVehDist, stsData.gpsStopVehDist, stsData.gpsStartLatitude, stsData.gpsStartLongitude, stsData.gpsEndLatitude"
			+ ", stsData.gpsEndLongitude, stsData.vUsedFuel, (stsData.vStopFuel - stsData.vStartFuel) as tripCalUsedFuel"
			+ ", stsData.vTripMotionDuration, ((stsData.tripCalGpsVehTimeDiff/1000) - stsData.vIdleDuration) as tripCalDrivingTm"
			+ ", stsData.receivedTimestamp, (((stsData.vUsedFuel * 0.00082) * 42.7 * 74.3)/1000) as tripCalC02Emission"
			+ ", if(0 <> stsData.tripCalGpsVehDistDiff, (CAST(stsData.vUsedFuel as Double)/stsData.tripCalGpsVehDistDiff)*100, stsData.tripCalGpsVehDistDiff) as tripCalFuelConsumption"
			+ ", indxData.f3 as vTachographSpeed, if(0 <> stsData.tripCalGpsVehDistDiff, indxData.f5/stsData.tripCalGpsVehDistDiff, 0) as tripCalAvgGrossWtComb"
			+ ", if(0 <> stsData.tripCalGpsVehTimeDiff, (CAST(stsData.vPTODuration as Double)/(stsData.tripCalGpsVehTimeDiff * 0.001))*100, stsData.tripCalGpsVehTimeDiff) as tripCalPtoDuration"
			+ ", if(0 <> stsData.vBrakeDuration, (CAST(stsData.vHarshBrakeDuration as Double)/stsData.vBrakeDuration)*100, stsData.vBrakeDuration) as triCalHarshBrakeDuration"
			+ ", if(0 <> stsData.vTripAccelerationTime, (CAST(stsData.vMaxThrottlePaddleDuration as Double)/stsData.vTripAccelerationTime)*100, stsData.vTripAccelerationTime) as tripCalHeavyThrottleDuration"
			+ ", if((stsData.vCruiseControlDist > 30 AND stsData.vCruiseControlDist < 50), if(0 <> stsData.tripCalGpsVehDistDiff, CAST(stsData.vCruiseControlDist as Double)/stsData.tripCalGpsVehDistDiff, stsData.tripCalGpsVehDistDiff), 0) as tripCalCrsCntrlDistBelow50"
			+ ", if((stsData.vCruiseControlDist > 50 AND stsData.vCruiseControlDist < 75), if(0 <> stsData.tripCalGpsVehDistDiff, CAST(stsData.vCruiseControlDist as Double)/stsData.tripCalGpsVehDistDiff, stsData.tripCalGpsVehDistDiff), 0) as tripCalCrsCntrlDistAbv50"
			+ ", if((stsData.vCruiseControlDist > 75), if(0 <> stsData.tripCalGpsVehDistDiff, CAST(stsData.vCruiseControlDist as Double)/stsData.tripCalGpsVehDistDiff, stsData.tripCalGpsVehDistDiff), 0) as tripCalCrsCntrlDistAbv75"
			+ ", if(0 <> stsData.tripCalGpsVehDistDiff, (CAST(stsData.vTripDPABrakingCount as Double)+ stsData.vTripDPAAnticipationCount)/stsData.tripCalGpsVehDistDiff, stsData.tripCalGpsVehDistDiff) as tripCalAvgTrafficClsfn"
			+ ", if(0 <> stsData.vCruiseControlDist, (CAST(stsData.vCruiseControlFuelConsumed as Double)/stsData.vCruiseControlDist)*100, stsData.vCruiseControlDist) as tripCalCCFuelConsumption"
			+ ", stsData.vCruiseControlFuelConsumed , stsData.vCruiseControlDist, stsData.vIdleFuelConsumed"
			+ ", (if( 0 <> (stsData.tripCalGpsVehDistDiff - stsData.vCruiseControlDist) ,(stsData.vUsedFuel - if(0 <> stsData.vCruiseControlDist, (CAST(stsData.vCruiseControlFuelConsumed as Double)/stsData.vCruiseControlDist), stsData.vCruiseControlDist) )/(stsData.tripCalGpsVehDistDiff - stsData.vCruiseControlDist)* 100 , 0)) as tripCalfuelNonActiveCnsmpt"
			+ ", (CAST(stsData.vSumTripDPABrakingScore as Double) + stsData.vSumTripDPAAnticipationScore)/2 as tripCalDpaScore, stsData.driverId, indxData.f2 as driver2Id, stsData.tripCalGpsVehTimeDiff as tripCalGpsVehTime"
			+ ", stsData.hbaseInsertionTS, stsData.etlProcessingTS"
			+ " FROM stsAggregatedData stsData LEFT JOIN aggrIndxData indxData ON stsData.tripId = indxData.f0 ";
	
	public static final String TRIP_STATUS_AGGREGATION_QRY = " select stsData.tripId, stsData.vid, stsData.vin, stsData.startDateTime, stsData.endDateTime, stsData.gpsTripDist"
			+ ", stsData.tripCalGpsVehDistDiff as tripCalDist, stsData.vIdleDuration"
			+ ", if(stsData.tripCalGpsVehTimeDiff <> 0, (CAST(stsData.tripCalGpsVehDistDiff as Double)/stsData.tripCalVehTimeDiffInHr), stsData.tripCalGpsVehTimeDiff) as tripCalAvgSpeed"
			+ ", stsData.gpsStartVehDist, stsData.gpsStopVehDist, stsData.gpsStartLatitude, stsData.gpsStartLongitude, stsData.gpsEndLatitude"
			+ ", stsData.gpsEndLongitude, stsData.vUsedFuel, (stsData.vStopFuel - stsData.vStartFuel) as tripCalUsedFuel"
			+ ", stsData.vTripMotionDuration, ((stsData.tripCalGpsVehTimeDiff/1000) - stsData.vIdleDuration) as tripCalDrivingTm"
			+ ", stsData.receivedTimestamp, (((stsData.vUsedFuel * vDeisel.f0) * vDeisel.f1 * vDeisel.f2)/1000) as tripCalC02Emission"
			+ ", if(0 <> stsData.tripCalGpsVehDistDiff, (CAST(stsData.vUsedFuel as Double)/stsData.tripCalGpsVehDistDiff)*100, stsData.tripCalGpsVehDistDiff) as tripCalFuelConsumption"
			+ ", if(0 <> stsData.tripCalGpsVehTimeDiff, (CAST(stsData.vPTODuration as Double)/(stsData.tripCalGpsVehTimeDiff * 0.001))*100, stsData.tripCalGpsVehTimeDiff) as tripCalPtoDuration"
			+ ", if(0 <> stsData.vBrakeDuration, (CAST(stsData.vHarshBrakeDuration as Double)/stsData.vBrakeDuration)*100, stsData.vBrakeDuration) as triCalHarshBrakeDuration"
			+ ", if(0 <> stsData.vTripAccelerationTime, (CAST(stsData.vMaxThrottlePaddleDuration as Double)/stsData.vTripAccelerationTime)*100, stsData.vTripAccelerationTime) as tripCalHeavyThrottleDuration"
			+ ", if((stsData.vCruiseControlDist > 30 AND stsData.vCruiseControlDist < 50), if(0 <> stsData.tripCalGpsVehDistDiff, CAST(stsData.vCruiseControlDist as Double)/stsData.tripCalGpsVehDistDiff, stsData.tripCalGpsVehDistDiff), 0) as tripCalCrsCntrlDistBelow50"
			+ ", if((stsData.vCruiseControlDist > 50 AND stsData.vCruiseControlDist < 75), if(0 <> stsData.tripCalGpsVehDistDiff, CAST(stsData.vCruiseControlDist as Double)/stsData.tripCalGpsVehDistDiff, stsData.tripCalGpsVehDistDiff), 0) as tripCalCrsCntrlDistAbv50"
			+ ", if((stsData.vCruiseControlDist > 75), if(0 <> stsData.tripCalGpsVehDistDiff, CAST(stsData.vCruiseControlDist as Double)/stsData.tripCalGpsVehDistDiff, stsData.tripCalGpsVehDistDiff), 0) as tripCalCrsCntrlDistAbv75"
			+ ", if(0 <> stsData.tripCalGpsVehDistDiff, (CAST(stsData.vTripDPABrakingCount as Double)+ stsData.vTripDPAAnticipationCount)/stsData.tripCalGpsVehDistDiff, stsData.tripCalGpsVehDistDiff) as tripCalAvgTrafficClsfn"
			+ ", if(0 <> stsData.vCruiseControlDist, (CAST(stsData.vCruiseControlFuelConsumed as Double)/stsData.vCruiseControlDist)*100, stsData.vCruiseControlDist) as tripCalCCFuelConsumption"
			+ ", stsData.vCruiseControlFuelConsumed , stsData.vCruiseControlDist, stsData.vIdleFuelConsumed, stsData.kafkaProcessingTS "
			+ ", (if( 0 <> (stsData.tripCalGpsVehDistDiff - stsData.vCruiseControlDist) ,(stsData.vUsedFuel - if(0 <> stsData.vCruiseControlDist, (CAST(stsData.vCruiseControlFuelConsumed as Double)/stsData.vCruiseControlDist), stsData.vCruiseControlDist) )/(stsData.tripCalGpsVehDistDiff - stsData.vCruiseControlDist)* 100 , 0)) as tripCalfuelNonActiveCnsmpt"
			+ ", (CAST(stsData.vSumTripDPABrakingScore as Double) + stsData.vSumTripDPAAnticipationScore)/2 as tripCalDpaScore, stsData.driverId, stsData.tripCalGpsVehTimeDiff as tripCalGpsVehTime"
			+ ", stsData.hbaseInsertionTS, stsData.etlProcessingTS"
			+ " FROM hbaseStsData stsData CROSS JOIN vehDieselEmissionFactors vDeisel";

	public static final String CONSOLIDATED_TRIP_QRY = " select stsData.tripId, stsData.vid, if(stsData.vin IS NOT NULL, stsData.vin, if(stsData.vid IS NOT NULL, stsData.vid, 'UNKNOWN')) as vin, stsData.startDateTime, stsData.endDateTime, stsData.gpsTripDist"
			+ ", stsData.tripCalDist, stsData.vIdleDuration, indxData.f4 vGrossWeightCombination"
			+ ", stsData.tripCalAvgSpeed, stsData.gpsStartVehDist, stsData.gpsStopVehDist, stsData.gpsStartLatitude, stsData.gpsStartLongitude, stsData.gpsEndLatitude"
			+ ", stsData.gpsEndLongitude, stsData.vUsedFuel, stsData.tripCalUsedFuel, stsData.vTripMotionDuration"
			+ ", stsData.tripCalDrivingTm, stsData.receivedTimestamp, stsData.tripCalC02Emission, stsData.tripCalFuelConsumption"
			+ ", indxData.f3 as vTachographSpeed, if(0 <> stsData.tripCalDist, indxData.f5/stsData.tripCalDist, 0) as tripCalAvgGrossWtComb"
			+ ", stsData.tripCalPtoDuration, stsData.triCalHarshBrakeDuration, stsData.tripCalHeavyThrottleDuration"
			+ ", stsData.tripCalCrsCntrlDistBelow50, stsData.tripCalCrsCntrlDistAbv50, stsData.tripCalCrsCntrlDistAbv75"
			+ ", stsData.tripCalAvgTrafficClsfn, stsData.tripCalCCFuelConsumption, stsData.vCruiseControlFuelConsumed "
			+ ", stsData.vCruiseControlDist, stsData.vIdleFuelConsumed, stsData.tripCalfuelNonActiveCnsmpt"
			+ ", stsData.tripCalDpaScore, stsData.driverId, indxData.f2 as driver2Id, stsData.tripCalGpsVehTime"
			+ ", stsData.hbaseInsertionTS, stsData.etlProcessingTS, stsData.kafkaProcessingTS"
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
			+ ", if(stsData.tripCalPtoDuration IS NOT NULL, stsData.tripCalPtoDuration, 0) as tripCalPtoDuration, if(stsData.triCalHarshBrakeDuration IS NOT NULL, stsData.triCalHarshBrakeDuration, 0) as triCalHarshBrakeDuration"
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
			+ ", if(stsData.hbaseInsertionTS IS NOT NULL, stsData.hbaseInsertionTS, 0) as hbaseInsertionTS, if(stsData.etlProcessingTS IS NOT NULL, stsData.etlProcessingTS, 0) as etlProcessingTS"
			+ ", if(stsData.kafkaProcessingTS IS NOT NULL, stsData.kafkaProcessingTS, 0) as kafkaProcessingTS"
			+ " FROM stsAggregatedData stsData LEFT JOIN aggrIndxData indxData ON stsData.tripId = indxData.f0 ";


	public static final String CONSOLIDATED_TRIP_QRYNEW = " select stsData.tripId "
			+ ", if(null <> stsData.vin, stsData.vin, if(null <> stsData.vid, stsData.vid, 'UNKNOWN')) as vin"
			+ ", if(null <> stsData.startDateTime, stsData.startDateTime, 0) as startDateTime, if(null <> stsData.endDateTime, stsData.endDateTime, 0) as endDateTime "
			+ ", if(null <> stsData.gpsTripDist, stsData.gpsTripDist, 0) as gpsTripDist, if(null <> stsData.tripCalDist, stsData.tripCalDist, 0) as tripCalDist"
			+ ", if(null <> stsData.vIdleDuration, stsData.vIdleDuration, 0) as vIdleDuration, if(null <> indxData.f4, indxData.f4, 0) as vGrossWeightCombination"
			+ ", if(null <> stsData.tripCalAvgSpeed, stsData.tripCalAvgSpeed, 0) as tripCalAvgSpeed, if(null <> stsData.gpsStartVehDist, stsData.gpsStartVehDist, 0) as gpsStartVehDist"
			+ ", if(null <> stsData.gpsStopVehDist, stsData.gpsStopVehDist, 0) as gpsStopVehDist, if(null <> stsData.gpsStartLatitude, stsData.gpsStartLatitude, 0) as gpsStartLatitude"
			+ ", if(null <> stsData.gpsStartLongitude, stsData.gpsStartLongitude, 0) as gpsStartLongitude, if(null <> stsData.gpsEndLatitude, stsData.gpsEndLatitude, 0) as gpsEndLatitude"
			+ ", if(null <> stsData.gpsEndLongitude, stsData.gpsEndLongitude, 0) as gpsEndLongitude, if(null <> stsData.vUsedFuel, stsData.vUsedFuel, 0) as vUsedFuel"
			+ ", if(null <> stsData.tripCalUsedFuel, stsData.tripCalUsedFuel, 0) as tripCalUsedFuel, if(null <> stsData.as vTripMotionDuration, stsData.as vTripMotionDuration, 0) asvTripMotionDuration"
			+ ", if(null <> stsData.tripCalDrivingTm, stsData.tripCalDrivingTm, 0) as tripCalDrivingTm, if(null <> stsData.receivedTimestamp, stsData.receivedTimestamp, 0) as receivedTimestamp"
			+ ", if(null <> stsData.tripCalC02Emission, stsData.tripCalC02Emission, 0) as tripCalC02Emission, if(null <> stsData.tripCalFuelConsumption, stsData.tripCalFuelConsumption, 0) as tripCalFuelConsumption"
			+ ", if(null <> indxData.f3, indxData.f3, 0) as vTachographSpeed, if(0 <> stsData.tripCalDist, indxData.f5/stsData.tripCalDist, 0) as tripCalAvgGrossWtComb"
			+ ", if(null <> stsData.tripCalPtoDuration, stsData.tripCalPtoDuration, 0) as tripCalPtoDuration, if(null <> stsData.triCalHarshBrakeDuration, stsData.triCalHarshBrakeDuration, 0) as triCalHarshBrakeDuration"
			+ ", if(null <> stsData.tripCalHeavyThrottleDuration, stsData.tripCalHeavyThrottleDuration, 0) as tripCalHeavyThrottleDuration"
			+ ", if(null <> stsData.tripCalCrsCntrlDistBelow50, stsData.tripCalCrsCntrlDistBelow50, 0) as tripCalCrsCntrlDistBelow50"
			+ ", if(null <> stsData.tripCalCrsCntrlDistAbv50, stsData.tripCalCrsCntrlDistAbv50, 0) as tripCalCrsCntrlDistAbv50"
			+ ", if(null <> stsData.tripCalCrsCntrlDistAbv75, stsData.tripCalCrsCntrlDistAbv75, 0) as tripCalCrsCntrlDistAbv75"
			+ ", if(null <> stsData.tripCalAvgTrafficClsfn, stsData.tripCalAvgTrafficClsfn, 0) as tripCalAvgTrafficClsfn"
			+ ", if(null <> stsData.tripCalCCFuelConsumption, stsData.tripCalCCFuelConsumption, 0) as tripCalCCFuelConsumption"
			+ ", if(null <> stsData.vCruiseControlFuelConsumed, stsData.vCruiseControlFuelConsumed, 0) as vCruiseControlFuelConsumed "
			+ ", if(null <> stsData.vCruiseControlDist, stsData.vCruiseControlDist, 0) as vCruiseControlDist"
			+ ", if(null <> stsData.vIdleFuelConsumed, stsData.vIdleFuelConsumed, 0) as vIdleFuelConsumed"
			+ ", if(null <> stsData.tripCalfuelNonActiveCnsmpt, stsData.tripCalfuelNonActiveCnsmpt, 0) as tripCalfuelNonActiveCnsmpt"
			+ ", if(null <> stsData.tripCalDpaScore, stsData.tripCalDpaScore, 0) as tripCalDpaScore, stsData.driverId"
			+ ", indxData.f2 as driver2Id, if(null <> stsData.tripCalGpsVehTime, stsData.tripCalGpsVehTime, 0) as tripCalGpsVehTime"
			+ ", if(null <> stsData.hbaseInsertionTS, stsData.hbaseInsertionTS, 0) as hbaseInsertionTS, if(null <> stsData.etlProcessingTS, stsData.etlProcessingTS, 0) as etlProcessingTS"
			+ ", if(null <> stsData.kafkaProcessingTS, stsData.kafkaProcessingTS, 0) as kafkaProcessingTS"
			+ " FROM stsAggregatedData stsData LEFT JOIN aggrIndxData indxData ON stsData.tripId = indxData.f0 ";
}
