package net.atos.daf.ct2.pojo.standard;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.io.Serializable;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class Warning  implements Serializable {

    private static final long serialVersionUID = 1L;

    @JsonProperty(value = "WarningTimestamp")
    private Long warningTimestamp;
    @JsonProperty(value = "WarningClass")
    private Integer warningClass;
    // private JSON WarningNumber;

}
