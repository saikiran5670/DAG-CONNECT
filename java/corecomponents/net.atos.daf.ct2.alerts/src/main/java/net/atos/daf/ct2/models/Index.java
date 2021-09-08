package net.atos.daf.ct2.models;

import java.io.Serializable;
import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.List;

import lombok.Getter;
import lombok.Setter;
import lombok.ToString;


@Getter
@Setter
@ToString
public class Index extends net.atos.daf.ct2.pojo.standard.Index  implements Serializable {

    /**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private String vid;
    private String vin;
    private BigDecimal vFuelStopPrevVal;
    private Integer EvtId;
    private Double averageSpeed;
    private Long idleDuration;
    private List<net.atos.daf.ct2.pojo.standard.Index> indexList = new ArrayList<>();
    
}
