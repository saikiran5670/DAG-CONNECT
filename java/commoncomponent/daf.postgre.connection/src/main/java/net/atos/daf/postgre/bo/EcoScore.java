package net.atos.daf.postgre.bo;

import java.io.Serializable;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class EcoScore implements Serializable {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private String tripId;
	private String vin;
	private Long startDateTime;
	private Long endDateTime;
	private String driverId;
	private String driver2Id;
	private Long tripCalDist;
	private Integer vTripDPABrakingCount;
	private Integer vTripDPAAnticipationCount;
	private Integer vSumTripDPABrakingScore;
	private Integer vSumTripDPAAnticipationScore;
	private Double vGrossWeightCombination;
	private Long tripCalUsedFuel;
	private Double tripCalPtoDuration;
	private Integer vIdleDuration;
	/*
	private Double vGrossWtSum;
	private Long numberOfIndexMessage;
	private Double tripCalAvgGrossWtComb;*/
	
	private Double tripCalHeavyThrottleDuration;
	private Long vStopFuel;
	private Long vStartFuel;
	
	private Long tripProcessingTS;
	private Long etlProcessingTS;
	private Long kafkaProcessingTS;
	


}

