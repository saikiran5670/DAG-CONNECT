package net.atos.daf.ct2.models;

import java.io.Serializable;
import java.math.BigDecimal;

import lombok.Getter;
import lombok.Setter;
import lombok.ToString;

@Getter
@Setter
@ToString
public class AlertFuelMeasurement implements Serializable{
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	
	private Long evtDateTime;
	private BigDecimal vFuelLevel;
}
