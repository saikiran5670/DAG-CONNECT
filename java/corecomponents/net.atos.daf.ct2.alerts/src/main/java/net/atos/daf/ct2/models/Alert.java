package net.atos.daf.ct2.models;

import com.fasterxml.jackson.annotation.*;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.NoArgsConstructor;
import lombok.ToString;

import java.io.Serializable;
import java.util.HashMap;
import java.util.Map;

@JsonInclude(JsonInclude.Include.NON_NULL)
@JsonPropertyOrder({
        "id",
        "tripid",
        "vin",
        "categoryType",
        "type",
        "name",
        "alertid",
        "thresholdValue",
        "thresholdValueUnitType",
        "valueAtAlertTime",
        "latitude",
        "longitude",
        "alertGeneratedTime",
        "messageTimestamp",
        "createdAt",
        "modifiedAt"
})
@ToString
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class Alert implements Serializable {
    private static final long serialVersionUID = 1L;

    @JsonProperty("id")
    private String id;
    @JsonProperty("tripid")
    private String tripid;
    @JsonProperty("vin")
    private String vin;
    @JsonProperty("categoryType")
    private String categoryType;
    @JsonProperty("type")
    private String type;
    @JsonProperty("name")
    private String name;
    @JsonProperty("alertid")
    private String alertid;
    @JsonProperty("thresholdValue")
    private String thresholdValue;
    @JsonProperty("thresholdValueUnitType")
    private String thresholdValueUnitType;
    @JsonProperty("valueAtAlertTime")
    private String valueAtAlertTime;
    @JsonProperty("latitude")
    private String latitude;
    @JsonProperty("longitude")
    private String longitude;
    @JsonProperty("alertGeneratedTime")
    private String alertGeneratedTime;
    @JsonProperty("messageTimestamp")
    private String messageTimestamp;
    @JsonProperty("createdAt")
    private String createdAt;
    @JsonProperty("modifiedAt")
    private String modifiedAt;
    @JsonProperty("urgencyLevelType")
    private String urgencyLevelType;
    @JsonIgnore
    private Map<String, Object> additionalProperties = new HashMap<String, Object>();

    @JsonProperty("id")
    public String getId() {
        return id;
    }

    @JsonProperty("id")
    public void setId(String id) {
        this.id = id;
    }

    @JsonProperty("tripid")
    public String getTripid() {
        return tripid;
    }

    @JsonProperty("tripid")
    public void setTripid(String tripid) {
        this.tripid = tripid;
    }

    @JsonProperty("vin")
    public String getVin() {
        return vin;
    }

    @JsonProperty("vin")
    public void setVin(String vin) {
        this.vin = vin;
    }

    @JsonProperty("categoryType")
    public String getCategoryType() {
        return categoryType;
    }

    @JsonProperty("categoryType")
    public void setCategoryType(String categoryType) {
        this.categoryType = categoryType;
    }

    @JsonProperty("type")
    public String getType() {
        return type;
    }

    @JsonProperty("type")
    public void setType(String type) {
        this.type = type;
    }

    @JsonProperty("name")
    public String getName() {
        return name;
    }

    @JsonProperty("name")
    public void setName(String name) {
        this.name = name;
    }

    @JsonProperty("alertid")
    public String getAlertid() {
        return alertid;
    }

    @JsonProperty("alertid")
    public void setAlertid(String alertid) {
        this.alertid = alertid;
    }

    @JsonProperty("thresholdValue")
    public String getThresholdValue() {
        return thresholdValue;
    }

    @JsonProperty("thresholdValue")
    public void setThresholdValue(String thresholdValue) {
        this.thresholdValue = thresholdValue;
    }

    @JsonProperty("thresholdValueUnitType")
    public String getThresholdValueUnitType() {
        return thresholdValueUnitType;
    }

    @JsonProperty("thresholdValueUnitType")
    public void setThresholdValueUnitType(String thresholdValueUnitType) {
        this.thresholdValueUnitType = thresholdValueUnitType;
    }

    @JsonProperty("valueAtAlertTime")
    public String getValueAtAlertTime() {
        return valueAtAlertTime;
    }

    @JsonProperty("valueAtAlertTime")
    public void setValueAtAlertTime(String valueAtAlertTime) {
        this.valueAtAlertTime = valueAtAlertTime;
    }

    @JsonProperty("latitude")
    public String getLatitude() {
        return latitude;
    }

    @JsonProperty("latitude")
    public void setLatitude(String latitude) {
        this.latitude = latitude;
    }

    @JsonProperty("longitude")
    public String getLongitude() {
        return longitude;
    }

    @JsonProperty("longitude")
    public void setLongitude(String longitude) {
        this.longitude = longitude;
    }

    @JsonProperty("alertGeneratedTime")
    public String getAlertGeneratedTime() {
        return alertGeneratedTime;
    }

    @JsonProperty("alertGeneratedTime")
    public void setAlertGeneratedTime(String alertGeneratedTime) {
        this.alertGeneratedTime = alertGeneratedTime;
    }

    @JsonProperty("messageTimestamp")
    public String getMessageTimestamp() {
        return messageTimestamp;
    }

    @JsonProperty("messageTimestamp")
    public void setMessageTimestamp(String messageTimestamp) {
        this.messageTimestamp = messageTimestamp;
    }

    @JsonProperty("createdAt")
    public String getCreatedAt() {
        return createdAt;
    }

    @JsonProperty("createdAt")
    public void setCreatedAt(String createdAt) {
        this.createdAt = createdAt;
    }

    @JsonProperty("modifiedAt")
    public String getModifiedAt() {
        return modifiedAt;
    }

    @JsonProperty("modifiedAt")
    public void setModifiedAt(String modifiedAt) {
        this.modifiedAt = modifiedAt;
    }

    public String getUrgencyLevelType() {
        return urgencyLevelType;
    }

    public void setUrgencyLevelType(String urgencyLevelType) {
        this.urgencyLevelType = urgencyLevelType;
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
