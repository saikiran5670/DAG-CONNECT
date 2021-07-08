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
	private Integer gpsTripDist;
	private Long tripCalDist;
	private Integer vIdleDuration;
	private Double vGrossWeightCombination;
	private Double tripCalAvgSpeed;
	private Long gpsStartVehDist;
	private Long gpsStopVehDist;
	private Double gpsStartLatitude;
	private Double gpsStartLongitude;
	private Double gpsEndLatitude;
	private Double gpsEndLongitude;
	private Integer vUsedFuel;
	private Long tripCalUsedFuel;
	private Integer vTripMotionDuration;
	private Long tripCalDrivingTm;
	private Long receivedTimestamp;
	private Double tripCalC02Emission;
	private Double tripCalFuelConsumption;
	private Double vTachographSpeed;
	private Double tripCalAvgGrossWtComb;
	private Double tripCalPtoDuration;
	private Double tripCalHarshBrakeDuration;
	private Double tripCalHeavyThrottleDuration;
	private Integer tripCalCrsCntrlDist25To50;
	private Integer tripCalCrsCntrlDist50To75;
	private Integer tripCalCrsCntrlDistAbv75;
	private Double tripCalAvgTrafficClsfn;
	private Double tripCalCCFuelConsumption;
	private Integer vCruiseControlFuelConsumed;
	private Integer vCruiseControlDist;
	private Integer vIdleFuelConsumed;
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
	
	private Integer vTripDPABrakingCount;
	private Integer vTripDPAAnticipationCount;
	private Integer vSumTripDPABrakingScore;
	private Integer vSumTripDPAAnticipationScore;
	
	private Integer vHarshBrakeDuration;
	private Integer vBrakeDuration; 
	private Integer vTripIdlePTODuration;
	private Integer vTripIdleWithoutPTODuration;
	private Integer vPTODuration;
	private Integer vMaxThrottlePaddleDuration;
	private Integer vTripAccelerationTime;
}
