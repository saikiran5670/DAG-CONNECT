
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
    "M2M_in",
    "M2M_out"
})
@Generated("jsonschema2pojo")
public class Timestamps {

    @JsonProperty("M2M_in")
    private String m2MIn;
    @JsonProperty("M2M_out")
    private String m2MOut;
    @JsonIgnore
    private Map<String, Object> additionalProperties = new HashMap<String, Object>();

    @JsonProperty("M2M_in")
    public String getM2MIn() {
        return m2MIn;
    }

    @JsonProperty("M2M_in")
    public void setM2MIn(String m2MIn) {
        this.m2MIn = m2MIn;
    }

    @JsonProperty("M2M_out")
    public String getM2MOut() {
        return m2MOut;
    }

    @JsonProperty("M2M_out")
    public void setM2MOut(String m2MOut) {
        this.m2MOut = m2MOut;
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
