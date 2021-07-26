package net.atos.daf.ct2.pojo.standard;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.io.Serializable;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class StatusDocument implements Serializable {

  private static final long serialVersionUID = 1L;

  @JsonProperty(value = "VEvtCause")
  private Integer vEvtCause;

  @JsonProperty(value = "GPSSpeed")
  private Long gpsSpeed;
  @JsonProperty(value = "GPSTripDist")
  private Long gpsTripDist;
  @JsonProperty(value = "TripHaversineDistance")
  private Long tripHaversineDistance;

  @JsonProperty(value = "TripID")
  private String tripID;

  @JsonProperty(value = "VCruiseControlFuelConsumed")
  private Long vCruiseControlFuelConsumed;
  @JsonProperty(value = "VIdleFuelConsumed")
  private Long vIdleFuelConsumed;
  @JsonProperty(value = "VMaxThrottlePaddleDuration")
  private Long vMaxThrottlePaddleDuration;
  @JsonProperty(value = "VSumTripDPABrakingScore")
  private Long vSumTripDPABrakingScore;
  @JsonProperty(value = "VSumTripDPAAnticipationScore")
  private Long vSumTripDPAAnticipationScore;
  @JsonProperty(value = "VTripAccelerationTime")
  private Long vTripAccelerationTime;
  @JsonProperty(value = "VTripCoastDist")
  private Long vTripCoastDist;
  @JsonProperty(value = "VTripCoastDuration")
  private Long vTripCoastDuration;
  @JsonProperty(value = "VTripCoastFuelConsumed")
  private Long vTripCoastFuelConsumed;
  @JsonProperty(value = "VTripCruiseControlDuration")
  private Long vTripCruiseControlDuration;
  @JsonProperty(value = "VTripDPABrakingCount")
  private Long vTripDPABrakingCount;
  @JsonProperty(value = "VTripDPAAnticipationCount")
  private Long vTripDPAAnticipationCount;
  @JsonProperty(value = "VTripIdleFuelConsumed")
  private Long vTripIdleFuelConsumed;
  @JsonProperty(value = "VTripIdlePTODuration")
  private Long vTripIdlePTODuration;
  @JsonProperty(value = "VTripIdlePTOFuelConsumed")
  private Long vTripIdlePTOFuelConsumed;
  @JsonProperty(value = "VTripIdleWithoutPTODuration")
  private Long vTripIdleWithoutPTODuration;
  @JsonProperty(value = "VTripIdleWithoutPTOFuelConsumed")
  private Long vTripIdleWithoutPTOFuelConsumed;
  @JsonProperty(value = "VTripMotionDuration")
  private Long vTripMotionDuration;
  @JsonProperty(value = "VTripMotionFuelConsumed")
  private Long vTripMotionFuelConsumed;
  @JsonProperty(value = "VTripMotionBrakeCount")
  private Long vTripMotionBrakeCount;
  @JsonProperty(value = "VTripMotionBrakeDist")
  private Long vTripMotionBrakeDist;
  @JsonProperty(value = "VTripMotionBrakeDuration")
  private Long vTripMotionBrakeDuration;
  @JsonProperty(value = "VTripMotionPTODuration")
  private Long vTripMotionPTODuration;
  @JsonProperty(value = "VTripMotionPTOFuelConsumed")
  private Long vTripMotionPTOFuelConsumed;
  @JsonProperty(value = "VTripPTOFuelConsumed")
  private Long vTripPTOFuelConsumed;

  @JsonProperty(value = "VCruiseControlDistanceDistr")
  private Distribution vCruiseControlDistanceDistr;
  @JsonProperty(value = "VAccelerationPedalDistr")
  private Distribution vAccelerationPedalDistr;
  @JsonProperty(value = "VEngineLoadAtEngineSpeedDistr")
  private Distribution vEngineLoadAtEngineSpeedDistr;
  @JsonProperty(value = "VRetarderTorqueActualDistr")
  private Distribution vRetarderTorqueActualDistr;

  @JsonProperty(value = "AxeVariantId")
  private String axeVariantId;
  @JsonProperty(value = "VAccelerationSpeed")
  private SpareMatrixAcceleration vAccelerationSpeed;
  @JsonProperty(value = "VRpmTorque")
  private SparseMatrix vRpmTorque;
  @JsonProperty(value = "VSpeedRpm")
  private SparseMatrix vSpeedRpm;
}
