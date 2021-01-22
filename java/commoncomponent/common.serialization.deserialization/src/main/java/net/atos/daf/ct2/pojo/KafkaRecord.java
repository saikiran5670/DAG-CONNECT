package net.atos.daf.ct2.pojo;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.io.Serializable;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class KafkaRecord<T> implements Serializable {

  private String key;
  private T value;
  private Long timeStamp;

  @Override
  public String toString() {
    return "KafkaRecord{" +
            "key='" + key + '\'' +
            ", value=" + value +
            ", timeStamp=" + timeStamp +
            '}';
  }
}
