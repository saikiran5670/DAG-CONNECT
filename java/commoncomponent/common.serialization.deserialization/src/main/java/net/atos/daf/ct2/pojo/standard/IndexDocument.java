package net.atos.daf.ct2.pojo.standard;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.io.Serializable;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class IndexDocument implements Serializable {

  private static final long serialVersionUID = 1L;

  @JsonProperty(value = "TripID")
  private String tripID;

  @JsonProperty(value = "GPSHdop")
  private Integer gpsHdop;
  @JsonProperty(value = "GPSSpeed")
  private Long gpsSpeed;

  @JsonProperty(value = "Driver1CardInserted")
  private Boolean driver1CardInserted;
  @JsonProperty(value = "Driver1RemainingDrivingTime")
  private Long driver1RemainingDrivingTime;
  @JsonProperty(value = "Driver1RemainingRestTime")
  private Long driver1RemainingRestTime;
  @JsonProperty(value = "Driver1WorkingState")
  private Integer driver1WorkingState;
  @JsonProperty(value = "Driver2ID")
  private String driver2ID;
  @JsonProperty(value = "Driver2CardInserted")
  private Boolean driver2CardInserted;
  @JsonProperty(value = "Driver2WorkingState")
  private Integer driver2WorkingState;

  @JsonProperty(value = "VAmbiantAirTemperature")
  private Long vAmbiantAirTemperature;
  // @JsonProperty(value = "")
  // private Integer VAmbientAirTemperature;
  @JsonProperty(value = "VAcceleration")
  private Double vAcceleration;
  @JsonProperty(value = "VDEFTankLevel")
  private Integer vDEFTankLevel;
  @JsonProperty(value = "VEngineCoolantTemperature")
  private Integer vEngineCoolantTemperature;
  @JsonProperty(value = "VEngineLoad")
  private Integer vEngineLoad;
  @JsonProperty(value = "VEngineSpeed")
  private Long vEngineSpeed;
  @JsonProperty(value = "VEngineTotalHours")
  private Long vEngineTotalHours;
  @JsonProperty(value = "VEngineTotalHoursIdle")
  private Long vEngineTotalHoursIdle;
  @JsonProperty(value = "VFuelCumulated")
  private Long vFuelCumulated;
  @JsonProperty(value = "VFuelCumulatedIdle")
  private Long vFuelCumulatedIdle;
  @JsonProperty(value = "VFuelCumulatedLR")
  private Long vFuelCumulatedLR;
  @JsonProperty(value = "VFuelLevel1")
  private Double vFuelLevel1;
  @JsonProperty(value = "VGearCurrent")
  private Integer vGearCurrent;
  @JsonProperty(value = "VGrossWeightCombination")
  private Long vGrossWeightCombination;
  @JsonProperty(value = "VPedalAcceleratorPosition1")
  private Double vPedalAcceleratorPosition1;
  @JsonProperty(value = "VPowerBatteryChargeLevel")
  private Double vPowerBatteryChargeLevel;
  @JsonProperty(value = "VPowerBatteryVoltage")
  private Double vPowerBatteryVoltage;
  @JsonProperty(value = "VRetarderTorqueActual")
  private Double vRetarderTorqueActual;
  @JsonProperty(value = "VRetarderTorqueMode")
  private Integer vRetarderTorqueMode;
  @JsonProperty(value = "VServiceBrakeAirPressure1")
  private Long vServiceBrakeAirPressure1;
  @JsonProperty(value = "VServiceBrakeAirPressure2")
  private Long vServiceBrakeAirPressure2;
  @JsonProperty(value = "VTachographSpeed")
  private Integer vTachographSpeed;
  @JsonProperty(value = "VWheelBasedSpeed")
  private Long vWheelBasedSpeed;
  @JsonProperty(value = "TT_Norm")
  private String tt_Norm;
  @JsonProperty(value = "TT_ListValue")
  private Integer[] tt_ListValue;

  @JsonProperty(value = "GPSSegmentDist")
  private Long gpsSegmentDist;
  @JsonProperty(value = "SegmentHaversineDistance")
  private Long segmentHaversineDistance;
  @JsonProperty(value = "VSegmentFuelLevel1")
  private Double vSegmentFuelLevel1;
  @JsonProperty(value = "VTankDiff")
  private Double vTankDiff;

  @JsonProperty(value = "Period")
  private Long period;
  @JsonProperty(value = "StartEltsTime")
  private Long startEltsTime;
  @JsonProperty(value = "AdBlueLevel")
  private Double[] adBlueLevel;
  @JsonProperty(value = "AirPressure")
  private Integer[] airPressure;
  @JsonProperty(value = "AmbientPressure")
  private Double[] ambientPressure;
  @JsonProperty(value = "EngineCoolantLevel")
  private Double[] engineCoolantLevel;
  @JsonProperty(value = "EngineCoolantTemperature")
  private Integer[] engineCoolantTemperature;
  @JsonProperty(value = "EngineOilLevel")
  private Double[] engineOilLevel;
  @JsonProperty(value = "EngineOilTemperature")
  private Integer[] engineOilTemperature;
  @JsonProperty(value = "EngineOilPressure")
  private Integer[] engineOilPressure;
  @JsonProperty(value = "EngineLoad")
  private Integer[] engineLoad;
  @JsonProperty(value = "EngineSpeed")
  private Integer[] engineSpeed;
  @JsonProperty(value = "FuelLevel")
  private Double[] fuelLevel;
  @JsonProperty(value = "FuelTemperature")
  private Double[] fuelTemperature;
  @JsonProperty(value = "InletAirPressureInInletManifold")
  private Integer[] inletAirPressureInInletManifold;
  @JsonProperty(value = "TachoVehicleSpeed")
  private Integer[] tachoVehicleSpeed;
  @JsonProperty(value = "TotalTachoMileage")
  private Integer[] totalTachoMileage;
}
