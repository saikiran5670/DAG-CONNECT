package net.atos.daf.ct2.bo;

import java.io.Serializable;
import java.math.BigDecimal;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class FuelDeviationData implements Serializable{
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private Long evtDateTime;
	private String vin;
	private String tripId;
	private String vid;
	private BigDecimal vFuelLevel;
	private Integer vEvtId;
	private Long vDist;
	private Double gpsHeading;
	private Double gpsLatitude;
	private Double gpsLongitude;
	
}
