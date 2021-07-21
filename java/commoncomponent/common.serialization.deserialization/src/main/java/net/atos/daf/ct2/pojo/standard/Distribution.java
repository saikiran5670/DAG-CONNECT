package net.atos.daf.ct2.pojo.standard;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.io.Serializable;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class Distribution implements Serializable {

  private static final long serialVersionUID = 1L;

  @JsonProperty(value = "DistrMinRangeInt")
  private Long distrMinRangeInt;
  @JsonProperty(value = "DistrMaxRangeInt")
  private Long distrMaxRangeInt;
  @JsonProperty(value = "DistrStep")
  private Long distrStep;
  @JsonProperty(value = "DistrArrayInt")
  private Integer[] distrArrayInt;
}
