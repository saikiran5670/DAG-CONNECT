package net.atos.daf.ct2.bo;

import java.io.Serializable;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class TripMileage implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private String vin;
	private Long evtDateTime;
	private Long odoMileage;
	private Double odoDistance;
	private Double gpsDistance;
	private Double realDistance;
	private Long modifiedAt;
	
		
}
