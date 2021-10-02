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
	@Override
	public String toString() {
		return "LiveFleetPojo [tripId=" + tripId + ", vid=" + vid + ", vin=" + vin + ", messageTimestamp="
				+ messageTimestamp + ", gpsAltitude=" + gpsAltitude + ", gpsHeading=" + gpsHeading + ", gpsLatitude="
				+ gpsLatitude + ", gpsLongitude=" + gpsLongitude + ", co2Emission=" + co2Emission + ", fuelConsumption="
				+ fuelConsumption + ", lastOdometerValue=" + lastOdometerValue + ", distUntilNextService="
				+ distUntilNextService + ", created_at_m2m=" + created_at_m2m + ", created_at_kafka=" + created_at_kafka
				+ ", created_at_dm=" + created_at_dm + ", vehMessageType=" + vehMessageType
				+ ", vehicleMsgTriggerTypeId=" + vehicleMsgTriggerTypeId + ", createdDatetime=" + createdDatetime
				+ ", receivedDatetime=" + receivedDatetime + ", gpsSpeed=" + gpsSpeed + ", gpsDatetime=" + gpsDatetime
				+ ", wheelbasedSpeed=" + wheelbasedSpeed + ", tachgraphSpeed=" + tachgraphSpeed + ", driver1Id="
				+ driver1Id + ", vehicleMsgTriggerAdditionalInfo=" + vehicleMsgTriggerAdditionalInfo
				+ ", driverAuthEquipmentTypeId=" + driverAuthEquipmentTypeId + ", cardReplacementIndex="
				+ cardReplacementIndex + ", oem_driver_id_type=" + oem_driver_id_type + ", oem_driver_id="
				+ oem_driver_id + ", pto_id=" + pto_id + ", telltale_id=" + telltale_id + ", oem_telltale="
				+ oem_telltale + ", telltale_state_id=" + telltale_state_id + ", drivingTime=" + drivingTime
				+ ", total_vehicle_distance=" + total_vehicle_distance + ", total_engine_hours=" + total_engine_hours
				+ ", total_engine_fuel_used=" + total_engine_fuel_used + ", gross_combination_vehicle_weight="
				+ gross_combination_vehicle_weight + ", engine_speed=" + engine_speed + ", fuel_level1=" + fuel_level1
				+ ", catalyst_fuel_level=" + catalyst_fuel_level + ", driver2_id=" + driver2_id
				+ ", driver1_working_state=" + driver1_working_state + ", driver2_working_state="
				+ driver2_working_state + ", driver2_auth_equipment_type_id=" + driver2_auth_equipment_type_id
				+ ", driver2_card_replacement_index=" + driver2_card_replacement_index + ", oem_driver2_id_type="
				+ oem_driver2_id_type + ", oem_driver2_id=" + oem_driver2_id + ", ambient_air_temperature="
				+ ambient_air_temperature + ", engine_coolant_temperature=" + engine_coolant_temperature
				+ ", service_brake_air_pressure_circuit1=" + service_brake_air_pressure_circuit1
				+ ", service_brake_air_pressure_circuit2=" + service_brake_air_pressure_circuit2 + "]";
	}
	

}
