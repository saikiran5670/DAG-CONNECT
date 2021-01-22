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

    @JsonProperty(value = "ROEvtTimestamp")
    private Integer roEvtTimestamp;
    @JsonProperty(value = "ROEvtID")
    private Integer roEvtID;
    @JsonProperty(value = "ROEvtData")
    private ROEventData roEvtData;
}
