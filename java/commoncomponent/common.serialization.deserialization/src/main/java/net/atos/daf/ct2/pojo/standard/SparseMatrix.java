package net.atos.daf.ct2.pojo.standard;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Data;

import java.io.Serializable;

@Data
public class SparseMatrix  implements Serializable {

  @JsonProperty(value = "abs")
  private Integer abs;
  @JsonProperty(value = "ord")
  private Integer ord;
  @JsonProperty(value = "A")
  private Integer[] a;
  @JsonProperty(value = "IA")
  private Integer[] ia;
  @JsonProperty(value = "JA")
  private Integer[] ja;
}
