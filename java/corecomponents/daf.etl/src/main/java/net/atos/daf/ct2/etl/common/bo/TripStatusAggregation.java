package net.atos.daf.ct2.etl.common.bo;

import java.io.Serializable;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class TripStatusAggregation implements Serializable {

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
	private Long tripCalGpsVehTime;
	//private Long hbaseInsertionTS;
	private Long tripProcessingTS;
	private Long etlProcessingTS;
	private Long kafkaProcessingTS;
	private Long numberOfIndexMessage;
	
	//new fields
	private Long vTripDPABrakingCount;
	private Long vTripDPAAnticipationCount;
	private Long vSumTripDPABrakingScore;
	private Long vSumTripDPAAnticipationScore;
	private Long vStopFuel;
	private Long vStartFuel;
	
	private Long vHarshBrakeDuration;
	private Long vBrakeDuration; 
	private Long vTripIdlePTODuration;
	private Long vTripIdleWithoutPTODuration;
	private Long vPTODuration;
	private Long vMaxThrottlePaddleDuration;
	private Long vTripAccelerationTime;
		
	private String rpmTorque;
	private Long absRpmTorque;
	private Long ordRpmTorque;
	private Object[] nonZeroRpmTorqueMatrix;
	private Object[] numValRpmTorque;
	private Object[] clmnIdnxRpmTorque;
	
	private String rpmSpeed;
	private Long absRpmSpeed;
	private Long ordRpmSpeed;
	private Object[] nonZeroRpmSpeedMatrix;
	private Object[] numValRpmSpeed;
	private Object[] clmnIdnxRpmSpeed;
	
	private String aclnSpeed;
	private Long absAclnSpeed;
	private Long ordAclnSpeed;
	private Object[] nonZeroAclnSpeedMatrix;
	private Object[] nonZeroBrakePedalAclnSpeedMatrix;
	private Object[] numValAclnSpeed;
	private Object[] clmnIdnxAclnSpeed;
}
