package net.atos.daf.ct2.kafka;

import org.apache.kafka.common.serialization.Serializer;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.ObjectOutputStream;

public class CustomSerializer<T> implements Serializer<T> {

  @Override
  public byte[] serialize(String topic, T data) {

    byte[] bytes = null;
    ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();

    try {
      ObjectOutputStream objectOutputStream = new ObjectOutputStream(byteArrayOutputStream);
      objectOutputStream.writeObject(data);
      objectOutputStream.flush();
      bytes = byteArrayOutputStream.toByteArray();
      byteArrayOutputStream.close();

    } catch (IOException e) {
      e.printStackTrace();
    }

    return bytes;
  }
}
