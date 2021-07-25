package net.atos.daf.postgre.bo;

import java.io.Serializable;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class CurrentTrip implements Serializable{
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private String trip_id;
	private long start_time_stamp ;
	private long end_time_stamp;
	private double start_position_lattitude;
	private double start_position_longitude;
	private double start_position_heading;
	private long start_geolocation_address_id; //Integer in table
	private double last_position_lattitude;
	private double last_position_longitude;
	private double last_position_heading;
	private long last_geolocation_address_id; //Integer in table
	private long driving_time;
	private long trip_distance;
	private long fuel_consumption;
	@Override
	public String toString() {
		return "CurrentTrip [trip_id=" + trip_id + ", start_time_stamp=" + start_time_stamp + ", end_time_stamp="
				+ end_time_stamp + ", start_position_lattitude=" + start_position_lattitude
				+ ", start_position_longitude=" + start_position_longitude + ", start_position_heading="
				+ start_position_heading + ", start_geolocation_address_id=" + start_geolocation_address_id
				+ ", last_position_lattitude=" + last_position_lattitude + ", last_position_longitude="
				+ last_position_longitude + ", last_position_heading=" + last_position_heading
				+ ", last_geolocation_address_id=" + last_geolocation_address_id + ", driving_time=" + driving_time
				+ ", trip_distance=" + trip_distance + ", fuel_consumption=" + fuel_consumption + "]";
	}
	
	
	
}
