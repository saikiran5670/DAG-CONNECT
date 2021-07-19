package net.atos.daf.ct2.serde;

import net.atos.daf.ct2.pojo.KafkaRecord;
import org.apache.commons.io.Charsets;
import org.apache.flink.streaming.connectors.kafka.KafkaSerializationSchema;
import org.apache.kafka.clients.producer.ProducerRecord;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.ObjectOutputStream;

public class KafkaMessageSerializeSchema<T> implements KafkaSerializationSchema<KafkaRecord<T>> {

  private static final Logger log = LogManager.getLogger(KafkaSerializationSchema.class);
  private final String topicName;

  public KafkaMessageSerializeSchema(String topicName) {
    this.topicName = topicName;
  }

  @Override
  public ProducerRecord<byte[], byte[]> serialize(KafkaRecord<T> kafkaRecord, Long timeStamp) {
	  ProducerRecord<byte[], byte[]> producerRecord =
        new ProducerRecord<byte[], byte[]>(
            topicName, kafkaRecord.getKey().getBytes(), getBytes(kafkaRecord.getValue()));
    return producerRecord;
  }

  public byte[] getBytes(T t) {

    byte[] bytes = null;
    ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();

    if (t.getClass() == String.class) {
      return ((String) t).getBytes(Charsets.UTF_8);
    }

    try {
      ObjectOutputStream objectOutputStream = new ObjectOutputStream(byteArrayOutputStream);
      objectOutputStream.writeObject(t);
      objectOutputStream.flush();
      bytes = byteArrayOutputStream.toByteArray();
      byteArrayOutputStream.close();

    } catch (IOException e) {
      log.error("Unable to Read Object ", e);
    }

    return bytes;
  }
}
