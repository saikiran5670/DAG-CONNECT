package net.atos.daf.postgre.bo;

import java.io.Serializable;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class IndexTripData implements Serializable {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private String tripId;
	private String vid;
	private String vin;
	private String driverId;
	private String driver2Id;
	private Integer vTachographSpeed;
	private String jobName;
	private Long vGrossWeightCombination;
	private Long increment;
	private Long vDist;
	private Long evtDateTime;
	private Integer vEvtId;
	
	
}
