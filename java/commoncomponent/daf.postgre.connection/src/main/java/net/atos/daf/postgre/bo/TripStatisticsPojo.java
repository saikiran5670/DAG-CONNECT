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
	private String vin;
	private  Long start_time_stamp;
	private  Long end_time_stamp;
	private String driver1ID;
	private  Long trip_distance; //Integer in table
	private  Long driving_time;
	private  Long fuel_consumption ; //Integer in table
	private Character vehicle_driving_status_type; //character(1)
	private  Long odometer_val ;
	private  Long distance_until_next_service ;
	private Double last_received_position_lattitude;
	private Double last_received_position_longitude;
	private Double last_received_position_heading;
	private  Long last_geolocation_address_id; //Integer in table
	
	private Double start_position_lattitude;
	private Double start_position_longitude;
	private Double start_position_heading;
	private  Long start_geolocation_address_id; //Integer in table
	
	private  Long last_processed_message_timestamp;
	private Character vehicle_health_status_type; //character(1)
	
	private  Long latest_warning_class; //Integer in table
	private  Long latest_warning_number; //Integer in table
	private Character latest_warning_type; //character(1)
	private  Long latest_warning_timestamp; //Integer in table
	private Double latest_warning_position_latitude;
	private Double latest_warning_position_longitude;
	private  Long latest_warning_geolocation_address_id; //Integer in table
	
	private  Long created_at;
	private  Long modified_at  ;
	@Override
	public String toString() {
		return "TripStatisticsPojo [tripId=" + tripId + ", vin=" + vin + ", start_time_stamp=" + start_time_stamp
				+ ", end_time_stamp=" + end_time_stamp + ", driver1ID=" + driver1ID + ", trip_distance=" + trip_distance
				+ ", driving_time=" + driving_time + ", fuel_consumption=" + fuel_consumption
				+ ", vehicle_driving_status_type=" + vehicle_driving_status_type + ", odometer_val=" + odometer_val
				+ ", distance_until_next_service=" + distance_until_next_service + ", last_received_position_lattitude="
				+ last_received_position_lattitude + ", last_received_position_longitude="
				+ last_received_position_longitude + ", last_received_position_heading="
				+ last_received_position_heading + ", last_geolocation_address_id=" + last_geolocation_address_id
				+ ", start_position_lattitude=" + start_position_lattitude + ", start_position_longitude="
				+ start_position_longitude + ", start_position_heading=" + start_position_heading
				+ ", start_geolocation_address_id=" + start_geolocation_address_id
				+ ", last_processed_message_timestamp=" + last_processed_message_timestamp
				+ ", vehicle_health_status_type=" + vehicle_health_status_type + ", latest_warning_class="
				+ latest_warning_class + ", latest_warning_number=" + latest_warning_number + ", latest_warning_type="
				+ latest_warning_type + ", latest_warning_timestamp=" + latest_warning_timestamp
				+ ", latest_warning_position_latitude=" + latest_warning_position_latitude
				+ ", latest_warning_position_longitude=" + latest_warning_position_longitude
				+ ", latest_warning_geolocation_address_id=" + latest_warning_geolocation_address_id + ", created_at="
				+ created_at + ", modified_at=" + modified_at + "]";
	}
	
	//fuel consumption datatype doubt 
	
	
	
}

