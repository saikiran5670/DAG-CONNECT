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
	private Integer gpsTripDist;
	// data type changed
	private Long gpsStopVehDist;
	// data type changed
	private Long gpsStartVehDist;
	private Integer vIdleDuration;
	private Double gpsStartLatitude;
	private Double gpsStartLongitude;
	private Double gpsEndLatitude;
	private Double gpsEndLongitude;
	private Integer vUsedFuel;
	// data type changed
	private Long vStopFuel;
	// data type changed
	private Long vStartFuel;
	private Integer vTripMotionDuration;
	private Long receivedTimestamp;
	private Integer vPTODuration;
	private Integer vHarshBrakeDuration;
	private Integer vBrakeDuration;
	private Integer vMaxThrottlePaddleDuration;
	private Integer vTripAccelerationTime;
	private Integer vCruiseControlDist;
	private Integer vTripDPABrakingCount;
	private Integer vTripDPAAnticipationCount;
	private Integer vCruiseControlFuelConsumed;
	private Integer vIdleFuelConsumed;
	private Integer vSumTripDPABrakingScore;
	private Integer vSumTripDPAAnticipationScore;
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
	private Integer numberOfIndexMessage;
	private Integer vTripIdlePTODuration;
	private Integer vTripIdleWithoutPTODuration;
}
