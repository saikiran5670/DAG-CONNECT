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

	private long trip_Start_time;
	private long start_time_stamp ;
	private long end_time_stamp;
	private double start_position_lattitude;
	private double start_position_longitude;
	private long fuel_consumption;
	
	
}
