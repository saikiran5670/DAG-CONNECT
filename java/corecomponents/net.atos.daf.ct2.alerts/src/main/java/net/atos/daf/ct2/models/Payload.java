package net.atos.daf.ct2.models;

import lombok.*;

import java.io.Serializable;
import java.util.HashMap;
import java.util.Map;
import java.util.Optional;

@Builder
@Getter
@Setter
@ToString
@NoArgsConstructor
@AllArgsConstructor
public class Payload<T> implements Serializable {
    private static final long serialVersionUID = -4683496582524846984L;

    private Optional<T> data;
    private Map<String, Object> additionalProperties = new HashMap<String, Object>();
    private final Long createdTimestamp = System.currentTimeMillis();

    public Optional<T> getData() {
        return data;
    }

    public void setData(Optional<T> data) {
        this.data = data;
    }

    public Map<String, Object> getAdditionalProperties() {
        return additionalProperties;
    }

    public void setAdditionalProperties(Map<String, Object> additionalProperties) {
        this.additionalProperties = additionalProperties;
    }

    public Long getCreatedTimestamp() {
        return createdTimestamp;
    }
}
