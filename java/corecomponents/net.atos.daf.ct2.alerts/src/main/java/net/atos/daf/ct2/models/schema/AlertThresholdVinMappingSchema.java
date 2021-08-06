package net.atos.daf.ct2.models.schema;

import com.fasterxml.jackson.annotation.JsonInclude;
import lombok.*;

@JsonInclude(JsonInclude.Include.NON_NULL)
@ToString
@Builder
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class AlertThresholdVinMappingSchema {

    private Integer id;
    private String vin;
    private String vid;
    private Integer alertId;
    private Double thresholdValue;
    private String unitType;
    private String urgencyLevelType;
    private String alertName;
    private String alertNameType;
    private String parentEnum;
}
