package net.atos.daf.ct2.bo;

import java.io.Serializable;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class VehicleMileage implements Serializable{
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private Long evtDateTime;
	private String tripId;
	private Long odoMileage;
	private Long odoDistance;
	private String vin;
	private String vid;
	//cross check it shld be long
	private Long gpsDistance;
	
}
