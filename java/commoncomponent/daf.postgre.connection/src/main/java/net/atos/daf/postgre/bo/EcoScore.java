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
	private Double tripCalAvgGrossWtComb;
	private Long tripCalUsedFuel;
	private Integer vPTODuration;
	private Integer vIdleDuration;
	/*
	private Double vGrossWtSum;
	private Long numberOfIndexMessage;
	private Double tripCalAvgGrossWtComb;*/
	
	private Integer vMaxThrottlePaddleDuration;
	private Integer vCruiseControlDist;
	private Integer tripCalCrsCntrlDist25To50;
	private Integer tripCalCrsCntrlDist50To75;
	private Integer tripCalCrsCntrlDistAbv75;
	private Double tachoVGrossWtCmbSum; 
	private Long vGrossWtCmbCount; 
	private Integer vTripAccelerationTime;
	private Integer vHarshBrakeDuration;
	private Integer vBrakeDuration;
	
	private Long tripProcessingTS;
	private Long etlProcessingTS;
	private Long kafkaProcessingTS;
	


}

