
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
    "abs",
    "ord",
    "A",
    "A_VBrake",
    "IA",
    "JA"
})
@Generated("jsonschema2pojo")
public class VAccelerationSpeed {

    @JsonProperty("abs")
    private Integer abs;
    @JsonProperty("ord")
    private Integer ord;
    @JsonProperty("A")
    private List<Integer> a = null;
    @JsonProperty("A_VBrake")
    private List<Integer> aVBrake = null;
    @JsonProperty("IA")
    private List<Integer> ia = null;
    @JsonProperty("JA")
    private List<Integer> ja = null;
    @JsonIgnore
    private Map<String, Object> additionalProperties = new HashMap<String, Object>();

    @JsonProperty("abs")
    public Integer getAbs() {
        return abs;
    }

    @JsonProperty("abs")
    public void setAbs(Integer abs) {
        this.abs = abs;
    }

    @JsonProperty("ord")
    public Integer getOrd() {
        return ord;
    }

    @JsonProperty("ord")
    public void setOrd(Integer ord) {
        this.ord = ord;
    }

    @JsonProperty("A")
    public List<Integer> getA() {
        return a;
    }

    @JsonProperty("A")
    public void setA(List<Integer> a) {
        this.a = a;
    }

    @JsonProperty("A_VBrake")
    public List<Integer> getAVBrake() {
        return aVBrake;
    }

    @JsonProperty("A_VBrake")
    public void setAVBrake(List<Integer> aVBrake) {
        this.aVBrake = aVBrake;
    }

    @JsonProperty("IA")
    public List<Integer> getIa() {
        return ia;
    }

    @JsonProperty("IA")
    public void setIa(List<Integer> ia) {
        this.ia = ia;
    }

    @JsonProperty("JA")
    public List<Integer> getJa() {
        return ja;
    }

    @JsonProperty("JA")
    public void setJa(List<Integer> ja) {
        this.ja = ja;
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
