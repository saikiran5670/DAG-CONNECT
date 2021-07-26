package net.atos.daf.postgre.bo;

import java.io.Serializable;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor



public class Trip implements Serializable {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	
	private String tripId;
	private String vid;
	private String vin;
	private Long startDateTime;
	private Long endDateTime;
	private Long gpsTripDist;
	private Long tripCalDist;
	private Long vIdleDuration;
	private Double vGrossWeightCombination;
	private Double tripCalAvgSpeed;
	private Long gpsStartVehDist;
	private Long gpsStopVehDist;
	private Double gpsStartLatitude;
	private Double gpsStartLongitude;
	private Double gpsEndLatitude;
	private Double gpsEndLongitude;
	private Long vUsedFuel;
	private Long tripCalUsedFuel;
	private Long vTripMotionDuration;
	private Long tripCalDrivingTm;
	private Long receivedTimestamp;
	private Double tripCalC02Emission;
	private Double tripCalFuelConsumption;
	private Double vTachographSpeed;
	private Double tripCalAvgGrossWtComb;
	private Double tripCalPtoDuration;
	private Double tripCalHarshBrakeDuration;
	private Double tripCalHeavyThrottleDuration;
	private Long tripCalCrsCntrlDist25To50;
	private Long tripCalCrsCntrlDist50To75;
	private Long tripCalCrsCntrlDistAbv75;
	private Double tripCalAvgTrafficClsfn;
	private Double tripCalCCFuelConsumption;
	private Long vCruiseControlFuelConsumed;
	private Long vCruiseControlDist;
	private Long vIdleFuelConsumed;
	private Double tripCalfuelNonActiveCnsmpt;
	private Double tripCalDpaScore;
	private String driverId;
	private String driver2Id;
	private Long tripCalGpsVehTime;
	//private Long hbaseInsertionTS;
	private Long tripProcessingTS;
	private Long etlProcessingTS;
	private Long kafkaProcessingTS;
	private Double vGrossWtSum; 
	private Long numberOfIndexMessage;
	
	private Long vTripDPABrakingCount;
	private Long vTripDPAAnticipationCount;
	private Long vSumTripDPABrakingScore;
	private Long vSumTripDPAAnticipationScore;
	
	private Long vHarshBrakeDuration;
	private Long vBrakeDuration; 
	private Long vTripIdlePTODuration;
	private Long vTripIdleWithoutPTODuration;
	private Long vPTODuration;
	private Long vMaxThrottlePaddleDuration;
	private Long vTripAccelerationTime;
}
