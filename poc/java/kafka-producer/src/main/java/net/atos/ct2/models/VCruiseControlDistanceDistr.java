
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
    "DistrMinRangeInt",
    "DistrMaxRangeInt",
    "DistrStep",
    "DistrArrayInt"
})
@Generated("jsonschema2pojo")
public class VCruiseControlDistanceDistr {

    @JsonProperty("DistrMinRangeInt")
    private Integer distrMinRangeInt;
    @JsonProperty("DistrMaxRangeInt")
    private Integer distrMaxRangeInt;
    @JsonProperty("DistrStep")
    private Integer distrStep;
    @JsonProperty("DistrArrayInt")
    private List<Integer> distrArrayInt = null;
    @JsonIgnore
    private Map<String, Object> additionalProperties = new HashMap<String, Object>();

    @JsonProperty("DistrMinRangeInt")
    public Integer getDistrMinRangeInt() {
        return distrMinRangeInt;
    }

    @JsonProperty("DistrMinRangeInt")
    public void setDistrMinRangeInt(Integer distrMinRangeInt) {
        this.distrMinRangeInt = distrMinRangeInt;
    }

    @JsonProperty("DistrMaxRangeInt")
    public Integer getDistrMaxRangeInt() {
        return distrMaxRangeInt;
    }

    @JsonProperty("DistrMaxRangeInt")
    public void setDistrMaxRangeInt(Integer distrMaxRangeInt) {
        this.distrMaxRangeInt = distrMaxRangeInt;
    }

    @JsonProperty("DistrStep")
    public Integer getDistrStep() {
        return distrStep;
    }

    @JsonProperty("DistrStep")
    public void setDistrStep(Integer distrStep) {
        this.distrStep = distrStep;
    }

    @JsonProperty("DistrArrayInt")
    public List<Integer> getDistrArrayInt() {
        return distrArrayInt;
    }

    @JsonProperty("DistrArrayInt")
    public void setDistrArrayInt(List<Integer> distrArrayInt) {
        this.distrArrayInt = distrArrayInt;
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
