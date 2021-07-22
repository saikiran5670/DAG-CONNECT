package net.atos.daf.ct2.pojo.standard;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Data;

import java.io.Serializable;

@Data
public class SparseMatrix  implements Serializable {

  private static final long serialVersionUID = 1L;

  @JsonProperty(value = "abs")
  private Long abs;
  @JsonProperty(value = "ord")
  private Long ord;
  @JsonProperty(value = "A")
  private Long[] a;
  @JsonProperty(value = "IA")
  private Long[] ia;
  @JsonProperty(value = "JA")
  private Long[] ja;
}
