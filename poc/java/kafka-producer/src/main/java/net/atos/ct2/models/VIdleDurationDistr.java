
package net.atos.ct2.models;

import java.util.HashMap;
import java.util.List;
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
    "DistrArrayInt",
    "DistrStep",
    "DistrMaxRangeInt",
    "DistrMinRangeInt"
})
@Generated("jsonschema2pojo")
public class VIdleDurationDistr {

    @JsonProperty("DistrArrayInt")
    private List<String> distrArrayInt = null;
    @JsonProperty("DistrStep")
    private String distrStep;
    @JsonProperty("DistrMaxRangeInt")
    private String distrMaxRangeInt;
    @JsonProperty("DistrMinRangeInt")
    private String distrMinRangeInt;
    @JsonIgnore
    private Map<String, Object> additionalProperties = new HashMap<String, Object>();

    @JsonProperty("DistrArrayInt")
    public List<String> getDistrArrayInt() {
        return distrArrayInt;
    }

    @JsonProperty("DistrArrayInt")
    public void setDistrArrayInt(List<String> distrArrayInt) {
        this.distrArrayInt = distrArrayInt;
    }

    @JsonProperty("DistrStep")
    public String getDistrStep() {
        return distrStep;
    }

    @JsonProperty("DistrStep")
    public void setDistrStep(String distrStep) {
        this.distrStep = distrStep;
    }

    @JsonProperty("DistrMaxRangeInt")
    public String getDistrMaxRangeInt() {
        return distrMaxRangeInt;
    }

    @JsonProperty("DistrMaxRangeInt")
    public void setDistrMaxRangeInt(String distrMaxRangeInt) {
        this.distrMaxRangeInt = distrMaxRangeInt;
    }

    @JsonProperty("DistrMinRangeInt")
    public String getDistrMinRangeInt() {
        return distrMinRangeInt;
    }

    @JsonProperty("DistrMinRangeInt")
    public void setDistrMinRangeInt(String distrMinRangeInt) {
        this.distrMinRangeInt = distrMinRangeInt;
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
