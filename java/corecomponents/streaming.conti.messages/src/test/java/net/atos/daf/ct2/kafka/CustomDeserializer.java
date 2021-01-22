package net.atos.daf.ct2.kafka;

import org.apache.kafka.common.serialization.Deserializer;

import java.io.ByteArrayInputStream;
import java.io.ObjectInputStream;

public class CustomDeserializer<T> implements Deserializer {

  @Override
  public T deserialize(String s, byte[] bytes) {

    T object = null;
    ObjectInputStream objectInputStream = null;

    try {
      ByteArrayInputStream byteArrayInputStream = new ByteArrayInputStream(bytes);
      objectInputStream = new ObjectInputStream(byteArrayInputStream);
      object = (T) objectInputStream.readObject();
      objectInputStream.close();

    } catch (Exception e) {
      e.printStackTrace();
    }

    return object;
  }
}
