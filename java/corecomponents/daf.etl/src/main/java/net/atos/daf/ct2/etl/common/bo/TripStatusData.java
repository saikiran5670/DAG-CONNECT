package net.atos.daf.ct2.etl.common.bo;

import java.io.Serializable;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class TripStatusData implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private String tripId;
	private String vid;
	private String increment;
	// data type changed
	private Long startDateTime;
	// data type changed
	private Long endDateTime;
	private Long gpsTripDist;
	// data type changed
	private Long gpsStopVehDist;
	// data type changed
	private Long gpsStartVehDist;
	private Long vIdleDuration;
	private Double gpsStartLatitude;
	private Double gpsStartLongitude;
	private Double gpsEndLatitude;
	private Double gpsEndLongitude;
	private Long vUsedFuel;
	// data type changed
	private Long vStopFuel;
	// data type changed
	private Long vStartFuel;
	private Long vTripMotionDuration;
	private Long receivedTimestamp;
	private Long vPTODuration;
	private Integer vHarshBrakeDuration;
	private Long vBrakeDuration;
	private Long vMaxThrottlePaddleDuration;
	private Long vTripAccelerationTime;
	private Long vCruiseControlDist;
	private Long vTripDPABrakingCount;
	private Long vTripDPAAnticipationCount;
	private Long vCruiseControlFuelConsumed;
	private Long vIdleFuelConsumed;
	private Long vSumTripDPABrakingScore;
	private Long vSumTripDPAAnticipationScore;
	private String driverId;
	// data type changed
	// private String eventDateTimeFirstIndex;
	// data type changed
	// private String evtDateTime;

	private Long tripCalGpsVehDistDiff;
	private Long tripCalGpsVehTimeDiff;
	//private Long hbaseInsertionTS;
	private Long tripProcessingTS;
	private Long etlProcessingTS;
	private Long kafkaProcessingTS;
	private Double tripCalVehTimeDiffInHr;
	private String vin;
	
	private String gpsStartDateTime;
	private String gpsEndDateTime;
	private String evtDateTime;
	private String evtDateTimeFirstIndex;
	private Double co2Emission;
	private Long numberOfIndexMessage;
	private Long vTripIdlePTODuration;
	private Long vTripIdleWithoutPTODuration;
	private Integer tripCalCrsCntrlDist25To50;
	private Integer tripCalCrsCntrlDist50To75;
	private Integer tripCalCrsCntrlDistAbv75;
}
