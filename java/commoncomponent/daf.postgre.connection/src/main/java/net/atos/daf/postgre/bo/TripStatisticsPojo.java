package net.atos.daf.postgre.bo;



import java.io.Serializable;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class TripStatisticsPojo implements Serializable{

	
	private static final long serialVersionUID = 1L;  

	private String tripId;
	private String vid;
	private String vin;
	private  Long start_time_stamp;
	private  Long end_time_stamp;
	private String driver1ID;
	private  Double start_position_lattitude;
	private  Double start_position_longitude;
	private String start_position;
	private Double last_recieved_position_lattitude;
	private Double last_recieved_position_longitude;
	private String last_known_position;
	private  Integer vehicle_status;
	private  Integer driver1_status ;
	private  Integer vehicle_health_status;
	private  Integer last_odometer_val ;
	private  Integer distance_until_next_service ; 
	private  Long last_processed_message_timestamp;
	private String driver2ID;
	private  Integer driver2_status ;
	private  Long created_at_m2m ;
	private  Long created_at_kafka ;
	private  Long created_at_dm  ;
	private  Long modified_at  ;
	private  Long fuel_consumption ;
	//fuel consumption datatype doubt 
	
}
