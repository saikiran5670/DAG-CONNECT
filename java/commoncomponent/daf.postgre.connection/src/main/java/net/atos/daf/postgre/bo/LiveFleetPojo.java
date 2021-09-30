package net.atos.daf.postgre.bo;



import java.io.Serializable;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor

public class LiveFleetPojo implements Serializable{

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L; 
	
	private String tripId;
	private String vid;
	private String vin;
	private  Double messageTimestamp ;
	private  Double gpsAltitude;
	private  Double gpsHeading;
	private  Double gpsLatitude;
	private  Double gpsLongitude;
	private  Double co2Emission;
	private  Double fuelConsumption;
	private  Integer lastOdometerValue;
	private  Integer distUntilNextService ;
	private  Long created_at_m2m;
	private  Long created_at_kafka;
	private  Long created_at_dm ;
	
	
	private String vehMessageType;
	private Integer vehicleMsgTriggerTypeId ;
	private Long createdDatetime;
	private Long receivedDatetime; 
	
	private Double gpsSpeed;
	private Long gpsDatetime; 
	private Double wheelbasedSpeed; 
	private Double tachgraphSpeed ;
	private String driver1Id;
	private String vehicleMsgTriggerAdditionalInfo; 
	private Integer driverAuthEquipmentTypeId;
	private String cardReplacementIndex ;
	private String oem_driver_id_type ;
	private String oem_driver_id ;
	private String pto_id ;
	private Integer telltale_id;
	private String oem_telltale ;
	private Integer telltale_state_id;
	private Integer drivingTime;
	
	//status API fields
	private Long total_vehicle_distance;
	private Long total_engine_hours;
	private Long total_engine_fuel_used;
	private Long gross_combination_vehicle_weight;
	private Long engine_speed;
	private Double fuel_level1;
	private Integer catalyst_fuel_level;
	private String driver2_id;
	private Integer driver1_working_state;
	private Integer driver2_working_state;
	private Integer driver2_auth_equipment_type_id;
	private String driver2_card_replacement_index;
	private String oem_driver2_id_type;
	private String oem_driver2_id;
	private Long ambient_air_temperature;
	private Integer engine_coolant_temperature;
	private Long service_brake_air_pressure_circuit1;
	private Long service_brake_air_pressure_circuit2;
	

}
