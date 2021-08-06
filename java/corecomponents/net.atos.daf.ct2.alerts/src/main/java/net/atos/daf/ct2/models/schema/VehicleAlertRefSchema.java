package net.atos.daf.ct2.models.schema;

import com.fasterxml.jackson.annotation.*;
import lombok.ToString;

import java.io.Serializable;
import java.util.HashMap;
import java.util.Map;

@JsonInclude(JsonInclude.Include.NON_NULL)
@ToString
public class VehicleAlertRefSchema implements Serializable{

    @JsonProperty("id")
    private Long id;
    @JsonProperty("alertId")
    private Long alertId;
    @JsonProperty("state")
    private String state;
    @JsonProperty("vin")
    private String vin;
    @JsonProperty("op")
    private String op="I";
    @JsonIgnore
    private Map<String, String> additionalProperties = new HashMap<String, String>();
    private Long timestamp = System.currentTimeMillis();
    private final static long serialVersionUID = -983704599562554769L;

    @JsonProperty("id")
    public Long getId() {
        return id;
    }

    @JsonProperty("id")
    public void setId(Long id) {
        this.id = id;
    }

    public VehicleAlertRefSchema withId(Long id) {
        this.id = id;
        return this;
    }

    @JsonProperty("alertId")
    public Long getAlertId() {
        return alertId;
    }

    @JsonProperty("alertId")
    public void setAlertId(Long alertId) {
        this.alertId = alertId;
    }

    public VehicleAlertRefSchema withAlertId(Long alertId) {
        this.alertId = alertId;
        return this;
    }

    @JsonProperty("state")
    public String getState() {
        return state;
    }

    @JsonProperty("state")
    public void setState(String state) {
        this.state = state;
    }

    public VehicleAlertRefSchema withState(String state) {
        this.state = state;
        return this;
    }

    public Long getTimestamp() {
        return timestamp;
    }

    public void setTimestamp(Long timestamp) {
        this.timestamp = timestamp;
    }

    public String getVin() {
        return vin;
    }

    public void setVin(String vin) {
        this.vin = vin;
    }

    public VehicleAlertRefSchema withVin(String vin) {
        this.vin = vin;
        return this;
    }

    @JsonProperty("op")
    public String getOp() {
        return op;
    }

    @JsonProperty("op")
    public void setOp(String op) {
        this.op = op;
    }

    @JsonAnyGetter
    public Map<String, String> getAdditionalProperties() {
        return this.additionalProperties;
    }

    @JsonAnySetter
    public void setAdditionalProperty(String name, String value) {
        this.additionalProperties.put(name, value);
    }

    public VehicleAlertRefSchema withAdditionalProperty(String name, String value) {
        this.additionalProperties.put(name, value);
        return this;
    }
}
