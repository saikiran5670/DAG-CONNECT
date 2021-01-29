package net.atos.daf.etl.ct2.common.util;

public class ETLQueries {
	
	public static final String TRIP_INDEX_DUPLICATE_QRY = "select idxData.f0 , idxData.f1 , idxData.f2 , idxData.f3, CAST(idxData.f4 as Double) as f4  FROM indexData idxData group by idxData.f0, idxData.f1, idxData.f2, idxData.f3, idxData.f4 ";
	
	public static final String TRIP_INDEX_AGGREGATION_QRY = "select idxData.f0 as tripId, idxData.f1 as vid, idxData.f2 as driver2Id, MAX(idxData.f3) as maxSpeed, AVG(CAST(idxData.f4 as Double)) as avgGrossWt FROM firstLevelAggrData idxData group by idxData.f0, idxData.f1, idxData.f2 ";
	
	public static final String CONSOLIDATED_TRIP_QRY = " select stsData.tripId, stsData.vid, stsData.vin, stsData.startDateTime, stsData.endDateTime, stsData.gpsTripDist"
			+ ", stsData.tripCalGpsVehDistDiff as tripCalDist, stsData.vIdleDuration, indxData.f4 vGrossWeightCombination"
			+ ", if(stsData.tripCalGpsVehTimeDiff <> 0, (CAST(stsData.tripCalGpsVehDistDiff as Double)/stsData.tripCalVehTimeDiffInHr), stsData.tripCalGpsVehTimeDiff) as tripCalAvgSpeed"
			+ ", stsData.gpsStartVehDist, stsData.gpsStopVehDist, stsData.gpsStartLatitude, stsData.gpsStartLongitude, stsData.gpsEndLatitude"
			+ ", stsData.gpsEndLongitude, stsData.vUsedFuel, (stsData.vStopFuel - stsData.vStartFuel) as tripCalUsedFuel"
			+ ", stsData.vTripMotionDuration, ((stsData.tripCalGpsVehTimeDiff/1000) - stsData.vIdleDuration) as tripCalDrivingTm"
			+ ", stsData.receivedTimestamp, ((((stsData.vUsedFuel * 0.00086) * 74.3)/1000) * 0.00000085) as tripCalC02Emission"
			+ ", if(0 <> stsData.tripCalGpsVehDistDiff, (CAST(stsData.vUsedFuel as Double)/stsData.tripCalGpsVehDistDiff)*100, stsData.tripCalGpsVehDistDiff) as tripCalFuelConsumption"
			+ ", indxData.f3 as vTachographSpeed, 0 as tripCalAvgGrossWtComb"
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
			+ " FROM hbaseStsData stsData LEFT JOIN aggrIndxData indxData ON stsData.tripId = indxData.f0 ";

}
