package net.atos.daf.ct2.bo;

import java.io.Serializable;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class FuelDeviation implements Serializable{
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private String tripId;
	private String vin;
	private String fuelEvtType;
	private String vehActivityType;
	private Long evtDateTime;
	private double fuelDiff;
	private Long vDist;
	private String vid;
	private Double gpsHeading;
	private Double gpsLatitude;
	private Double gpsLongitude;
	
}
