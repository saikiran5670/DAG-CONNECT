package net.atos.daf.ct2.bo;

import java.io.Serializable;
import java.math.BigDecimal;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class TripStatistic implements Serializable {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private String tripId;
	private String vin;
	private Long startDateTime;
	private Long endDateTime;
	private Integer gpsTripDist;
	private Long tripCalDist;
	private Integer vIdleDuration;
	private BigDecimal vGrossWeightCombination;
	private BigDecimal tripCalAvgSpeed;
	private Long gpsStartVehDist;
	private Long gpsStopVehDist;
	private Double gpsStartLatitude;
	private Double gpsStartLongitude;
	private Double gpsEndLatitude;
	private Double gpsEndLongitude;
	private Integer vUsedFuel;
	private Long tripCalUsedFuel;
	private Integer vTripMotionDuration;
	private Integer tripCalDrivingTm;
	private Long receivedTimestamp;
	private BigDecimal tripCalC02Emission;
	private BigDecimal tripCalFuelConsumption;
	private BigDecimal vTachographSpeed;
	private BigDecimal tripCalAvgGrossWtComb;
	private BigDecimal tripCalPtoDuration;
	private BigDecimal triCalHarshBrakeDuration;
	private BigDecimal tripCalHeavyThrottleDuration;
	private BigDecimal tripCalCrsCntrlDistBelow50;
	private BigDecimal tripCalCrsCntrlDistAbv50;
	private BigDecimal tripCalCrsCntrlDistAbv75;
	private String tripCalAvgTrafficClsfn;
	private BigDecimal tripCalCCFuelConsumption;
	private Integer vCruiseControlFuelConsumed;
	private Integer vCruiseControlDist;
	private Integer vIdleFuelConsumed;
	private BigDecimal tripCalfuelNonActiveCnsmpt;
	private Double tripCalDpaScore;
	private String driverId;
	private String driver2Id;
	private Integer tripCalGpsVehTime;
	private Long hbaseInsertionTS;
	private Long etlProcessingTS;
}

