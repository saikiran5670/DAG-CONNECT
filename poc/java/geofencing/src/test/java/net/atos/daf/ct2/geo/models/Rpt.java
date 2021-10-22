
package net.atos.daf.ct2.geo.models;

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
    "-lat",
    "-lon",
    "-self-closing"
})
public class Rpt {

    @JsonProperty("-lat")
    private String lat;
    @JsonProperty("-lon")
    private String lon;
    @JsonProperty("-self-closing")
    private String selfClosing;
    @JsonIgnore
    private Map<String, Object> additionalProperties = new HashMap<String, Object>();

    @JsonProperty("-lat")
    public String getLat() {
        return lat;
    }

    @JsonProperty("-lat")
    public void setLat(String lat) {
        this.lat = lat;
    }

    @JsonProperty("-lon")
    public String getLon() {
        return lon;
    }

    @JsonProperty("-lon")
    public void setLon(String lon) {
        this.lon = lon;
    }

    @JsonProperty("-self-closing")
    public String getSelfClosing() {
        return selfClosing;
    }

    @JsonProperty("-self-closing")
    public void setSelfClosing(String selfClosing) {
        this.selfClosing = selfClosing;
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
