package net.atos.daf.postgre.bo;

import java.io.Serializable;

import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.SequenceGenerator;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor

public class TripCommon implements Serializable {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	
	
	/*@SequenceGenerator(name = "tripdetail.trip_statistics_id_seq")
	@GeneratedValue(strategy = GenerationType.AUTO)*/
	@SequenceGenerator(name="tripdetail.trip_statistics_id_seq", sequenceName="tripdetail.trip_statistics_id_seq", allocationSize=1)
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator="tripdetail.trip_statistics_id_seq")
	private int id;
	private String tripId;
	//not in DB
	
	private String vid;
	private String vin;
	private Long startDateTime;
	private Long endDateTime;
	private Integer gpsTripDist;
	private Long tripCalDist;
	private Integer vIdleDuration;
	private Double tripCalAvgSpeed;
	private Double vGrossWeightCombination;
	private Long gpsStartVehDist;
	private Long gpsStopVehDist;
	private Double gpsStartLatitude;
	private Double gpsStartLongitude;
	private Double gpsEndLatitude;
	private Double gpsEndLongitude;
	private String start_position;
	private String end_position;
	private Integer vUsedFuel;
	private Long tripCalUsedFuel;
	private Integer vTripMotionDuration;
	private Long tripCalDrivingTm;
	private String noOfAlerts;
	private String noOfEvents;
	private Long receivedTimestamp;
	private Long kafkaProcessingTS;
	private Long hbaseInsertionTS;
	private Long etlProcessingTS;
	private Double tripCalC02Emission;
	private Double tripCalFuelConsumption;
	private Double vTachographSpeed;
	private Double tripCalAvgGrossWtComb;
	private Double tripCalPtoDuration;
	private Double tripCalHarshBrakeDuration;
	private Double tripCalHeavyThrottleDuration;
	private Double tripCalCrsCntrlDistBelow50;
	private Double tripCalCrsCntrlDistAbv50;
	private Double tripCalCrsCntrlDistAbv75;
	private Double tripCalAvgTrafficClsfn;
	private Double tripCalCCFuelConsumption;
	private Integer vCruiseControlFuelConsumed;
	private Integer vCruiseControlDist;
	private Double tripCalfuelNonActiveCnsmpt;
	private Integer vIdleFuelConsumed;
	private Double tripCalDpaScore;
	private Integer enduranceBrake;
	private Integer tripCoasting;
	private Integer ecoRolling;
	private String driverId;
	private String driver2Id;
	private Long tripCalGpsVehTime;
	private boolean is_ongoing_trip;
	
	
}
