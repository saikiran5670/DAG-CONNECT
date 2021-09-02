package net.atos.daf.ct2.models;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.*;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.List;


@Getter
@Setter
@ToString
public class Index extends net.atos.daf.ct2.pojo.standard.Index  implements Serializable {

    private String vid;
    private String vin;
    private List<net.atos.daf.ct2.pojo.standard.Index> indexList = new ArrayList<>();
}
