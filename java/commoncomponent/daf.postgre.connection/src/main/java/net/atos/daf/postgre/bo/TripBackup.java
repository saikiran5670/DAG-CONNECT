package net.atos.daf.postgre.bo;

import java.io.Serializable;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor


@Entity
@Table(name = "tripdetail.trip_statistics")
//@Table(name = "tripdetail.dummy_table")
public class TripBackup implements Serializable {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	
	@Id
	@Column(name = "id")
	/*@SequenceGenerator(name = "tripdetail.trip_statistics_id_seq")
	@GeneratedValue(strategy = GenerationType.AUTO)*/
	//@SequenceGenerator(name="tripdetail.trip_statistics_id_seq", sequenceName="tripdetail.trip_statistics_id_seq", allocationSize=1)
	//////@GeneratedValue(strategy = GenerationType.SEQUENCE, generator="tripdetail.trip_statistics_id_seq")
	private int id;
	
	
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
	
	@Column(name = "average_speed")
	private Double tripCalAvgSpeed;
	
	
	@Column(name = "average_weight")
	private Double vGrossWeightCombination;
	
	/*@Column(name = "average_speed")
	private Double tripCalAvgSpeed;*/
	
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

	/*start_position
	end_position*/
	
	@Column(name = "veh_message_fuel_consumed")
	private Integer vUsedFuel;
	
	@Column(name = "etl_gps_fuel_consumed")
	private Long tripCalUsedFuel;
	
	@Column(name = "veh_message_driving_time")
	private Integer vTripMotionDuration;
	
	@Column(name = "etl_gps_driving_time")
	private Long tripCalDrivingTm;
	
	/*no_of_alerts
	no_of_events*/
	
	@Column(name = "message_received_timestamp")
	private Long receivedTimestamp;
	
	@Column(name = "message_inserted_into_kafka_timestamp")
	private Long kafkaProcessingTS;
	
	@Column(name = "message_inserted_into_hbase_timestamp")
	private Long hbaseInsertionTS;
	
	@Column(name = "message_processed_by_etl_process_timestamp")
	private Long etlProcessingTS;

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
	private Double tripCalHarshBrakeDuration;
	
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
	
	@Column(name = "fuel_consumption_cc_non_active")
	private Double tripCalfuelNonActiveCnsmpt;
	
	@Column(name = "idling_consumption")
	private Integer vIdleFuelConsumed;
	
	@Column(name = "dpa_score")
	private Double tripCalDpaScore;
	
	/*endurance_brake
	coasting
	eco_rolling*/
	
	@Column(name = "driver1_id")
	private String driverId;
	
	@Column(name = "driver2_id")
	private String driver2Id;
	
	@Column(name = "etl_gps_trip_time")
	private Long tripCalGpsVehTime;
	
	//is_ongoing_trip
	
	
}
