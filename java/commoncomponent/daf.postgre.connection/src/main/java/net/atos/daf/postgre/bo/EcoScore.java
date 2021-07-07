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
	//private String driver2Id;
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
	private Double tripCalCrsCntrlUsage;
	private Double tripCalCrsCntrlUsage30To50;
	private Double tripCalCrsCntrlUsage50To75;
	private Double tripCalCrsCntrlUsageAbv75;
	private Double tachoVGrossWtCmbSum; 
	private Integer vHarshBrakeDuration;
	private Integer vBrakeDuration;
	
	private Long tripProcessingTS;
	private Long etlProcessingTS;
	private Long kafkaProcessingTS;
	


}

