package net.atos.daf.ct2.models.schema;

import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.*;

@JsonInclude(JsonInclude.Include.NON_NULL)
@ToString
@Builder
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class AlertMappingSchema {
    @JsonProperty("Id")
    private Integer id;
    @JsonProperty("vin")
    private String vin;
    @JsonProperty("alertId")
    private Long alertId;

    private Long timestamp = System.currentTimeMillis();
}
