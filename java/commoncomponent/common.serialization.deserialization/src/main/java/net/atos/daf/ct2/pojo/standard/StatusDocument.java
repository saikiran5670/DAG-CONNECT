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

  @JsonProperty(value = "VEvtCause")
  private Integer vEvtCause;

  @JsonProperty(value = "GPSSpeed")
  private Integer gpsSpeed;
  @JsonProperty(value = "GPSTripDist")
  private Integer gpsTripDist;
  @JsonProperty(value = "TripHaversineDistance")
  private Integer tripHaversineDistance;

  @JsonProperty(value = "TripID")
  private String tripID;

  @JsonProperty(value = "VCruiseControlFuelConsumed")
  private Integer vCruiseControlFuelConsumed;
  @JsonProperty(value = "VIdleFuelConsumed")
  private Integer vIdleFuelConsumed;
  @JsonProperty(value = "VMaxThrottlePaddleDuration")
  private Integer vMaxThrottlePaddleDuration;
  @JsonProperty(value = "VSumTripDPABrakingScore")
  private Integer vSumTripDPABrakingScore;
  @JsonProperty(value = "VSumTripDPAAnticipationScore")
  private Integer vSumTripDPAAnticipationScore;
  @JsonProperty(value = "VTripAccelerationTime")
  private Integer vTripAccelerationTime;
  @JsonProperty(value = "VTripCoastDist")
  private Integer vTripCoastDist;
  @JsonProperty(value = "VTripCoastDuration")
  private Integer vTripCoastDuration;
  @JsonProperty(value = "VTripCoastFuelConsumed")
  private Integer vTripCoastFuelConsumed;
  @JsonProperty(value = "VTripCruiseControlDuration")
  private Integer vTripCruiseControlDuration;
  @JsonProperty(value = "VTripDPABrakingCount")
  private Integer vTripDPABrakingCount;
  @JsonProperty(value = "VTripDPAAnticipationCount")
  private Integer vTripDPAAnticipationCount;
  @JsonProperty(value = "VTripIdleFuelConsumed")
  private Integer vTripIdleFuelConsumed;
  @JsonProperty(value = "VTripIdlePTODuration")
  private Integer vTripIdlePTODuration;
  @JsonProperty(value = "VTripIdlePTOFuelConsumed")
  private Integer vTripIdlePTOFuelConsumed;
  @JsonProperty(value = "VTripIdleWithoutPTODuration")
  private Integer vTripIdleWithoutPTODuration;
  @JsonProperty(value = "VTripIdleWithoutPTOFuelConsumed")
  private Integer vTripIdleWithoutPTOFuelConsumed;
  @JsonProperty(value = "VTripMotionDuration")
  private Integer vTripMotionDuration;
  @JsonProperty(value = "VTripMotionFuelConsumed")
  private Integer vTripMotionFuelConsumed;
  @JsonProperty(value = "VTripMotionBrakeCount")
  private Integer vTripMotionBrakeCount;
  @JsonProperty(value = "VTripMotionBrakeDist")
  private Integer vTripMotionBrakeDist;
  @JsonProperty(value = "VTripMotionBrakeDuration")
  private Integer vTripMotionBrakeDuration;
  @JsonProperty(value = "VTripMotionPTODuration")
  private Integer vTripMotionPTODuration;
  @JsonProperty(value = "VTripMotionPTOFuelConsumed")
  private Integer vTripMotionPTOFuelConsumed;
  @JsonProperty(value = "VTripPTOFuelConsumed")
  private Integer vTripPTOFuelConsumed;

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
