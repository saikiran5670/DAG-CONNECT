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
public class ThresholdSchema {
    private Integer id;
    private String thresh_col;
    private Integer thresh_val;
    private String thresh_val_datatype;
    private Integer alertId;
    private String created_on;
    private Long timestamp = System.currentTimeMillis();
}
