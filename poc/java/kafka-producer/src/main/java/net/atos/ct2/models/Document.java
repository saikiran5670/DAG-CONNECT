
package net.atos.ct2.models;

import java.util.HashMap;
import java.util.Map;
import javax.annotation.Generated;
import com.fasterxml.jackson.annotation.JsonAnyGetter;
import com.fasterxml.jackson.annotation.JsonAnySetter;
import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.annotation.JsonPropertyOrder;

@JsonInclude(JsonInclude.Include.NON_NULL)
@JsonPropertyOrder({
    "VIdleFuelConsumed",
    "TripHaversineDistance",
    "VTripAccelerationTime",
    "VMaxThrottlePaddleDuration",
    "VCruiseControlDistanceDistr",
    "VSumTripDPABrakingScore",
    "VSumTripDPAAnticipationScore",
    "VTripDPABrakingCount",
    "VTripDPAAnticipationCount",
    "VCruiseControlFuelConsumed",
    "VRpmTorque",
    "VSpeedRpm",
    "VAccelerationSpeed",
    "AxeVariantId",
    "VTripIdleFuelConsumed",
    "GPSTripDist",
    "TripID",
    "VEvtCause"
})
@Generated("jsonschema2pojo")
public class Document {

    @JsonProperty("VIdleFuelConsumed")
    private Integer vIdleFuelConsumed;
    @JsonProperty("TripHaversineDistance")
    private Integer tripHaversineDistance;
    @JsonProperty("VTripAccelerationTime")
    private Integer vTripAccelerationTime;
    @JsonProperty("VMaxThrottlePaddleDuration")
    private Integer vMaxThrottlePaddleDuration;
    @JsonProperty("VCruiseControlDistanceDistr")
    private VCruiseControlDistanceDistr vCruiseControlDistanceDistr;
    @JsonProperty("VSumTripDPABrakingScore")
    private Integer vSumTripDPABrakingScore;
    @JsonProperty("VSumTripDPAAnticipationScore")
    private Integer vSumTripDPAAnticipationScore;
    @JsonProperty("VTripDPABrakingCount")
    private Integer vTripDPABrakingCount;
    @JsonProperty("VTripDPAAnticipationCount")
    private Integer vTripDPAAnticipationCount;
    @JsonProperty("VCruiseControlFuelConsumed")
    private Integer vCruiseControlFuelConsumed;
    @JsonProperty("VRpmTorque")
    private VRpmTorque vRpmTorque;
    @JsonProperty("VSpeedRpm")
    private VSpeedRpm vSpeedRpm;
    @JsonProperty("VAccelerationSpeed")
    private VAccelerationSpeed vAccelerationSpeed;
    @JsonProperty("AxeVariantId")
    private String axeVariantId;
    @JsonProperty("VTripIdleFuelConsumed")
    private Integer vTripIdleFuelConsumed;
    @JsonProperty("GPSTripDist")
    private Integer gPSTripDist;
    @JsonProperty("TripID")
    private String tripID;
    @JsonProperty("VEvtCause")
    private Integer vEvtCause;
    @JsonIgnore
    private Map<String, Object> additionalProperties = new HashMap<String, Object>();

    @JsonProperty("VIdleFuelConsumed")
    public Integer getVIdleFuelConsumed() {
        return vIdleFuelConsumed;
    }

    @JsonProperty("VIdleFuelConsumed")
    public void setVIdleFuelConsumed(Integer vIdleFuelConsumed) {
        this.vIdleFuelConsumed = vIdleFuelConsumed;
    }

    @JsonProperty("TripHaversineDistance")
    public Integer getTripHaversineDistance() {
        return tripHaversineDistance;
    }

    @JsonProperty("TripHaversineDistance")
    public void setTripHaversineDistance(Integer tripHaversineDistance) {
        this.tripHaversineDistance = tripHaversineDistance;
    }

    @JsonProperty("VTripAccelerationTime")
    public Integer getVTripAccelerationTime() {
        return vTripAccelerationTime;
    }

    @JsonProperty("VTripAccelerationTime")
    public void setVTripAccelerationTime(Integer vTripAccelerationTime) {
        this.vTripAccelerationTime = vTripAccelerationTime;
    }

    @JsonProperty("VMaxThrottlePaddleDuration")
    public Integer getVMaxThrottlePaddleDuration() {
        return vMaxThrottlePaddleDuration;
    }

    @JsonProperty("VMaxThrottlePaddleDuration")
    public void setVMaxThrottlePaddleDuration(Integer vMaxThrottlePaddleDuration) {
        this.vMaxThrottlePaddleDuration = vMaxThrottlePaddleDuration;
    }

    @JsonProperty("VCruiseControlDistanceDistr")
    public VCruiseControlDistanceDistr getVCruiseControlDistanceDistr() {
        return vCruiseControlDistanceDistr;
    }

    @JsonProperty("VCruiseControlDistanceDistr")
    public void setVCruiseControlDistanceDistr(VCruiseControlDistanceDistr vCruiseControlDistanceDistr) {
        this.vCruiseControlDistanceDistr = vCruiseControlDistanceDistr;
    }

    @JsonProperty("VSumTripDPABrakingScore")
    public Integer getVSumTripDPABrakingScore() {
        return vSumTripDPABrakingScore;
    }

    @JsonProperty("VSumTripDPABrakingScore")
    public void setVSumTripDPABrakingScore(Integer vSumTripDPABrakingScore) {
        this.vSumTripDPABrakingScore = vSumTripDPABrakingScore;
    }

    @JsonProperty("VSumTripDPAAnticipationScore")
    public Integer getVSumTripDPAAnticipationScore() {
        return vSumTripDPAAnticipationScore;
    }

    @JsonProperty("VSumTripDPAAnticipationScore")
    public void setVSumTripDPAAnticipationScore(Integer vSumTripDPAAnticipationScore) {
        this.vSumTripDPAAnticipationScore = vSumTripDPAAnticipationScore;
    }

    @JsonProperty("VTripDPABrakingCount")
    public Integer getVTripDPABrakingCount() {
        return vTripDPABrakingCount;
    }

    @JsonProperty("VTripDPABrakingCount")
    public void setVTripDPABrakingCount(Integer vTripDPABrakingCount) {
        this.vTripDPABrakingCount = vTripDPABrakingCount;
    }

    @JsonProperty("VTripDPAAnticipationCount")
    public Integer getVTripDPAAnticipationCount() {
        return vTripDPAAnticipationCount;
    }

    @JsonProperty("VTripDPAAnticipationCount")
    public void setVTripDPAAnticipationCount(Integer vTripDPAAnticipationCount) {
        this.vTripDPAAnticipationCount = vTripDPAAnticipationCount;
    }

    @JsonProperty("VCruiseControlFuelConsumed")
    public Integer getVCruiseControlFuelConsumed() {
        return vCruiseControlFuelConsumed;
    }

    @JsonProperty("VCruiseControlFuelConsumed")
    public void setVCruiseControlFuelConsumed(Integer vCruiseControlFuelConsumed) {
        this.vCruiseControlFuelConsumed = vCruiseControlFuelConsumed;
    }

    @JsonProperty("VRpmTorque")
    public VRpmTorque getVRpmTorque() {
        return vRpmTorque;
    }

    @JsonProperty("VRpmTorque")
    public void setVRpmTorque(VRpmTorque vRpmTorque) {
        this.vRpmTorque = vRpmTorque;
    }

    @JsonProperty("VSpeedRpm")
    public VSpeedRpm getVSpeedRpm() {
        return vSpeedRpm;
    }

    @JsonProperty("VSpeedRpm")
    public void setVSpeedRpm(VSpeedRpm vSpeedRpm) {
        this.vSpeedRpm = vSpeedRpm;
    }

    @JsonProperty("VAccelerationSpeed")
    public VAccelerationSpeed getVAccelerationSpeed() {
        return vAccelerationSpeed;
    }

    @JsonProperty("VAccelerationSpeed")
    public void setVAccelerationSpeed(VAccelerationSpeed vAccelerationSpeed) {
        this.vAccelerationSpeed = vAccelerationSpeed;
    }

    @JsonProperty("AxeVariantId")
    public String getAxeVariantId() {
        return axeVariantId;
    }

    @JsonProperty("AxeVariantId")
    public void setAxeVariantId(String axeVariantId) {
        this.axeVariantId = axeVariantId;
    }

    @JsonProperty("VTripIdleFuelConsumed")
    public Integer getVTripIdleFuelConsumed() {
        return vTripIdleFuelConsumed;
    }

    @JsonProperty("VTripIdleFuelConsumed")
    public void setVTripIdleFuelConsumed(Integer vTripIdleFuelConsumed) {
        this.vTripIdleFuelConsumed = vTripIdleFuelConsumed;
    }

    @JsonProperty("GPSTripDist")
    public Integer getGPSTripDist() {
        return gPSTripDist;
    }

    @JsonProperty("GPSTripDist")
    public void setGPSTripDist(Integer gPSTripDist) {
        this.gPSTripDist = gPSTripDist;
    }

    @JsonProperty("TripID")
    public String getTripID() {
        return tripID;
    }

    @JsonProperty("TripID")
    public void setTripID(String tripID) {
        this.tripID = tripID;
    }

    @JsonProperty("VEvtCause")
    public Integer getVEvtCause() {
        return vEvtCause;
    }

    @JsonProperty("VEvtCause")
    public void setVEvtCause(Integer vEvtCause) {
        this.vEvtCause = vEvtCause;
    }

    @JsonAnyGetter
    public Map<String, Object> getAdditionalProperties() {
        return this.additionalProperties;
    }

    @JsonAnySetter
    public void setAdditionalProperty(String name, Object value) {
        this.additionalProperties.put(name, value);
    }

}
