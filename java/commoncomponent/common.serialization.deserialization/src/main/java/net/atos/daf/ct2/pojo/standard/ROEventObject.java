package net.atos.daf.ct2.pojo.standard;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.io.Serializable;
import java.util.List;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class ROEventObject  implements Serializable {

    private static final long serialVersionUID = 1L;

   @JsonProperty(value = "ROEvtList")
    private List<ROEvent> roEvtList;

}


