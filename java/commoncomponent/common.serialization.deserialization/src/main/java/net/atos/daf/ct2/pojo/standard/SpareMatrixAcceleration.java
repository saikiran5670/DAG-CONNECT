package net.atos.daf.ct2.pojo.standard;

import java.io.Serializable;

import com.fasterxml.jackson.annotation.JsonProperty;

import lombok.Data;

@Data
public class SpareMatrixAcceleration extends SparseMatrix  implements Serializable {

  private static final long serialVersionUID = 1L;

  @JsonProperty(value = "A_VBrake")
  private Integer[] a_VBrake;
}
