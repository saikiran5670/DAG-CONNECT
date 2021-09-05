package net.atos.daf.ct2.models;

import java.io.Serializable;
import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.List;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;


/*@Getter
@Setter
@ToString*/
@Data
@AllArgsConstructor
@NoArgsConstructor
public class Index extends net.atos.daf.ct2.pojo.standard.Index  implements Serializable {

    private String vid;
    private String vin;
    private BigDecimal vFuelStopPrevVal;
    private Integer EvtId;
    private List<net.atos.daf.ct2.pojo.standard.Index> indexList = new ArrayList<>();
    
}
