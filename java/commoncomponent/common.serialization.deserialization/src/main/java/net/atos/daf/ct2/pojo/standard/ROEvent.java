package net.atos.daf.ct2.pojo.standard;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.io.Serializable;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class ROEvent  implements Serializable {

    private static final long serialVersionUID = 1L;

    @JsonProperty(value = "ROEvtTimestamp")
    private Long roEvtTimestamp;
    @JsonProperty(value = "ROEvtID")
    private Integer roEvtID;
    @JsonProperty(value = "ROEvtData")
    private ROEventData roEvtData;
}
