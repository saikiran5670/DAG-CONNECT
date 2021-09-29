package net.atos.daf.ct2.etl.common.bo;

import java.io.Serializable;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class FuelCoEfficient implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	
    private double fuelCoefficient;
    private String fuelType;
}
