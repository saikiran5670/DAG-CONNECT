package net.atos.daf.ct2.pojo.standard;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.io.Serializable;
import java.util.List;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class MonitorDocument implements Serializable {

    private static final long serialVersionUID = 1L;

    @JsonProperty(value = "VEvtCause")
    private Integer vEvtCause;
    @JsonProperty(value = "VEvtExtraInfo")
    private String vEvtExtraInfo;

    @JsonProperty(value = "TripID")
    private String tripID;

    @JsonProperty(value = "ROEventObject")
    private ROEventObject roEventObject;

    @JsonProperty(value = "GPSHdop")
    private Double gpsHdop;
    @JsonProperty(value = "GPSSpeed")
    private Long gpsSpeed;

    @JsonProperty(value = "DriverID")
    private String driverID;
    @JsonProperty(value = "Driver1CardInserted")
    private Boolean driver1CardInserted;
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
    // private Long VAmbientAirTemperature;
    @JsonProperty(value = "VCruiseControl")
    private Integer vCruiseControl;
    @JsonProperty(value = "VCumulatedFuel")
    private Long vCumulatedFuel;
    @JsonProperty(value = "VDEFTankLevel")
    private Integer vDEFTankLevel;
    @JsonProperty(value = "VDist")
    private Long vDist;
    @JsonProperty(value = "VDistanceUntilService")
    private Long vDistanceUntilService;
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
    private Integer vFuelCumulatedIdle;
    @JsonProperty(value = "VFuelCumulatedLR")
    private Long vFuelCumulatedLR;
    @JsonProperty(value = "VFuelLevel1")
    private Double vFuelLevel1;
    @JsonProperty(value = "VGearCurrent")
    private Integer vGearCurrent;
    @JsonProperty(value = "VGearSelected")
    private Integer vGearSelected;
    @JsonProperty(value = "VGrossWeightCombination")
    private Long vGrossWeightCombination;
    @JsonProperty(value = "VIgnitionState")
    private Integer vIgnitionState;
    @JsonProperty(value = "VPedalBreakPosition1")
    private Double vPedalBreakPosition1;
    @JsonProperty(value = "VPowerBatteryChargeLevel")
    private Double vPowerBatteryChargeLevel;
    @JsonProperty(value = "VPowerBatteryVoltage")
    private Double vPowerBatteryVoltage;
    @JsonProperty(value = "VPTOEngaged")
    private Integer vPTOEngaged;
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

    @JsonProperty(value = "DM1_active")
    private Boolean dm1Active;

    @JsonProperty(value = "DM1_data")
    private List<Integer> dm1Data;
    @JsonProperty(value = "DM1_SPN")
    private Integer dm1SPN;
    @JsonProperty(value = "DM1_FMI")
    private Integer dm1FMI;
    @JsonProperty(value = "DM1_SA")
    private Integer dm1SA;
    @JsonProperty(value = "DM1_AWL")
    private Integer dm1AWL;
    @JsonProperty(value = "DM1_MIL")
    private Integer dm1MIL;
    @JsonProperty(value = "DM1_RSL")
    private Integer dm1RSL;
    @JsonProperty(value = "DM1_PLS")
    private Integer dm1PLS;
    @JsonProperty(value = "DM1_FAWL")
    private Integer dm1FAWL;
    @JsonProperty(value = "DM1_FMIL")
    private Integer dm1FMIL;
    @JsonProperty(value = "DM1_FRSL")
    private Integer dm1FRSL;
    @JsonProperty(value = "DM1_FPLS")
    private Integer dm1FPLS;
    @JsonProperty(value = "DM1_OC")
    private Integer dm1OC;
    @JsonProperty(value = "TT_Norm")
    private String ttNorm;
    @JsonProperty(value = "TT_Id")
    private Integer ttId;
    @JsonProperty(value = "TT_Value")
    private Integer ttValue;
    @JsonProperty(value = "TT_ListValue")
    private Integer[] ttListValue;
    @JsonProperty(value = "VWarningClass")
    private Integer vWarningClass;
    @JsonProperty(value = "VWarningNumber")
    private Integer vWarningNumber;
    @JsonProperty(value = "VWarningState")
    private Integer vWarningState;

    @JsonProperty(value = "WarningObject")
    private WarningObject warningObject;

}
