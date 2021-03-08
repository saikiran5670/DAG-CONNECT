package net.atos.daf.ct2.pojo.standard;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Data;

import java.io.Serializable;

@Data
public class SpareMatrixAcceleration extends SparseMatrix  implements Serializable {

  private static final long serialVersionUID = 1L;

  @JsonProperty(value = "A_VBrake")
  private Integer[] a_VBrake;
}
