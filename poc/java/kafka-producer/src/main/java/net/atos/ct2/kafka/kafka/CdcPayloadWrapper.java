package net.atos.ct2.kafka.kafka;

import com.fasterxml.jackson.annotation.*;
import lombok.ToString;

import java.io.Serializable;
import java.util.HashMap;
import java.util.Map;

@JsonInclude(JsonInclude.Include.NON_NULL)
@JsonPropertyOrder({
        "schema",
        "payload",
        "opration",
        "namespace",
        "timeStamp"
})
@JsonIgnoreProperties(ignoreUnknown = true)
@ToString
public class CdcPayloadWrapper implements Serializable {
    private static final long serialVersionUID = 1L;

    @JsonProperty("schema")
    private String schema;
    @JsonProperty("payload")
    private String payload;
    @JsonProperty("opration")
    private String operation;
    @JsonProperty("namespace")
    private String namespace;
    @JsonProperty("timeStamp")
    private Long timeStamp;
    @JsonIgnore
    private Map<String, Object> additionalProperties = new HashMap<String, Object>();

    @JsonProperty("schema")
    public String getSchema() {
        return schema;
    }

    @JsonProperty("schema")
    public void setSchema(String schema) {
        this.schema = schema;
    }

    @JsonProperty("payload")
    public String getPayload() {
        return payload;
    }

    @JsonProperty("payload")
    public void setPayload(String payload) {
        this.payload = payload;
    }

    @JsonProperty("operation")
    public String getOperation() {
        return operation;
    }

    @JsonProperty("operation")
    public void setOperation(String operation) {
        this.operation = operation;
    }

    @JsonProperty("namespace")
    public String getNamespace() {
        return namespace;
    }

    @JsonProperty("namespace")
    public void setNamespace(String namespace) {
        this.namespace = namespace;
    }

    @JsonProperty("timeStamp")
    public Long getTimeStamp() {
        return timeStamp;
    }

    @JsonProperty("timeStamp")
    public void setTimeStamp(Long timeStamp) {
        this.timeStamp = timeStamp;
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
