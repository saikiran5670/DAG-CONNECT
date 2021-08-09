package net.atos.daf.ct2.models.kafka;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.fasterxml.jackson.annotation.JsonAnyGetter;
import com.fasterxml.jackson.annotation.JsonAnySetter;
import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.annotation.JsonPropertyOrder;
import lombok.ToString;

@JsonInclude(JsonInclude.Include.NON_NULL)
@JsonPropertyOrder({
        "alertId",
        "vinOps"
})
@JsonIgnoreProperties(ignoreUnknown = true)
@ToString
public class AlertCdc {

    @JsonProperty("alertId")
    private String alertId;
    @JsonProperty("vinOps")
    private List<VinOp> vinOps = null;
    @JsonIgnore
    private Map<String, Object> additionalProperties = new HashMap<String, Object>();

    @JsonProperty("alertId")
    public String getAlertId() {
        return alertId;
    }

    @JsonProperty("alertId")
    public void setAlertId(String alertId) {
        this.alertId = alertId;
    }

    @JsonProperty("vinOps")
    public List<VinOp> getVinOps() {
        return vinOps;
    }

    @JsonProperty("vinOps")
    public void setVinOps(List<VinOp> vinOps) {
        this.vinOps = vinOps;
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