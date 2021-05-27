package net.atos.daf.ct2.etl.common.bo;

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
	private Double triCalHarshBrakeDuration;
	private Double tripCalHeavyThrottleDuration;
	private Double tripCalCrsCntrlDistBelow50;
	private Double tripCalCrsCntrlDistAbv50;
	private Double tripCalCrsCntrlDistAbv75;
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
	private Long hbaseInsertionTS;
	private Long etlProcessingTS;
	private Long kafkaProcessingTS;
	
	
	/*@Id
	@Column(name = "trip_id")
	private String tripId;
	//not in DB
	@Transient
	private String vid;
	
	@Column(name = "vin")
	private String vin;
	
	@Column(name = "start_time_stamp")
	private Long startDateTime;
	
	@Column(name = "end_time_stamp")
	private Long endDateTime;
	
	@Column(name = "veh_message_distance")
	private Integer gpsTripDist;
	
	@Column(name = "etl_gps_distance")
	private Long tripCalDist;
	
	@Column(name = "idle_duration")
	private Integer vIdleDuration;
	
	@Column(name = "average_weight")
	private Double vGrossWeightCombination;
	
	@Column(name = "average_speed")
	private Double tripCalAvgSpeed;
	
	@Column(name = "start_odometer")
	private Long gpsStartVehDist;
	
	@Column(name = "last_odometer")
	private Long gpsStopVehDist;
	
	@Column(name = "start_position_lattitude")
	private Double gpsStartLatitude;
	
	@Column(name = "start_position_longitude")
	private Double gpsStartLongitude;
	
	@Column(name = "end_position_lattitude")
	private Double gpsEndLatitude;
	
	@Column(name = "end_position_longitude")
	private Double gpsEndLongitude;
	
	@Column(name = "veh_message_fuel_consumed")
	private Integer vUsedFuel;
	
	@Column(name = "etl_gps_fuel_consumed")
	private Long tripCalUsedFuel;
	
	@Column(name = "veh_message_driving_time")
	private Integer vTripMotionDuration;
	
	@Column(name = "etl_gps_driving_time")
	private Long tripCalDrivingTm;
	
	@Column(name = "message_received_timestamp")
	private Long receivedTimestamp;
	
	@Column(name = "co2_emission")
	private Double tripCalC02Emission;
	
	@Column(name = "fuel_consumption")
	private Double tripCalFuelConsumption;
	
	@Column(name = "max_speed")
	private Double vTachographSpeed;
	
	@Column(name = "average_gross_weight_comb")
	private Double tripCalAvgGrossWtComb;
	
	@Column(name = "pto_duration")
	private Double tripCalPtoDuration;
	
	@Column(name = "harsh_brake_duration")
	private Double triCalHarshBrakeDuration;
	
	@Column(name = "heavy_throttle_duration")
	private Double tripCalHeavyThrottleDuration;
	
	@Column(name = "cruise_control_distance_30_50")
	private Double tripCalCrsCntrlDistBelow50;
	
	@Column(name = "cruise_control_distance_50_75")
	private Double tripCalCrsCntrlDistAbv50;
	
	@Column(name = "cruise_control_distance_more_than_75")
	private Double tripCalCrsCntrlDistAbv75;
	
	@Column(name = "average_traffic_classification")
	private Double tripCalAvgTrafficClsfn;
	
	@Column(name = "cc_fuel_consumption")
	private Double tripCalCCFuelConsumption;
	
	@Column(name = "v_cruise_control_fuel_consumed_for_cc_fuel_consumption")
	private Integer vCruiseControlFuelConsumed;
	
	@Column(name = "v_cruise_control_dist_for_cc_fuel_consumption")
	private Integer vCruiseControlDist;
	
	@Column(name = "idling_consumption")
	private Integer vIdleFuelConsumed;
	
	@Column(name = "fuel_consumption_cc_non_active")
	private Double tripCalfuelNonActiveCnsmpt;
	
	@Column(name = "dpa_score")
	private Double tripCalDpaScore;
	
	@Column(name = "driver1_id")
	private String driverId;
	
	@Column(name = "driver2_id")
	private String driver2Id;
	
	@Column(name = "etl_gps_trip_time")
	private Long tripCalGpsVehTime;
	
	@Column(name = "message_inserted_into_hbase_timestamp")
	private Long hbaseInsertionTS;
	
	@Column(name = "etl_gps_trip_time")
	private Long etlProcessingTS;
	
	@Column(name = "message_inserted_into_kafka_timestamp")
	private Long kafkaProcessingTS;*/
}
