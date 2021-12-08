package net.atos.daf.ct2.etl.common.bo;

import java.io.Serializable;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class TripGrossWeight implements Serializable {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	
	private String tripId;
	//private Long tripCalDist;
	//normal sum/count
	//private Double vGrossWeightCombination;
	private Double tripCalAvgGrossWtComb;
	private Double vGrossWtSum;
	private Long vGrossWtCmbCount; 
	private Long tachographSpeed;
}
