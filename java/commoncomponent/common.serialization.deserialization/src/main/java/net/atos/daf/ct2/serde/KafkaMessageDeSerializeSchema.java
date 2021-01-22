package net.atos.daf.ct2.serde;

import net.atos.daf.ct2.exception.DAFCT2Exception;
import net.atos.daf.ct2.pojo.KafkaRecord;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.streaming.connectors.kafka.KafkaDeserializationSchema;
import org.apache.kafka.clients.consumer.ConsumerRecord;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.nio.charset.StandardCharsets;

public class KafkaMessageDeSerializeSchema<T>
    implements KafkaDeserializationSchema<KafkaRecord<T>> {

  private static final Logger log = LogManager.getLogger(KafkaDeserializationSchema.class);

  @Override
  public boolean isEndOfStream(KafkaRecord kafkaRecord) {
    return false;
  }

  @Override
  public KafkaRecord deserialize(ConsumerRecord<byte[], byte[]> consumerRecord) throws Exception {

    KafkaRecord kafkaRecord = new KafkaRecord();
    kafkaRecord.setKey(new String(consumerRecord.key()));
    kafkaRecord.setValue(getObject(consumerRecord.value()));
    kafkaRecord.setTimeStamp(consumerRecord.timestamp());

    return kafkaRecord;
  }

  @Override
  public TypeInformation<KafkaRecord<T>> getProducedType() {
    return TypeInformation.of(
        new TypeHint<KafkaRecord<T>>() {

          @Override
          public TypeInformation<KafkaRecord<T>> getTypeInfo() {
            return super.getTypeInfo();
          }
        });
  }

  public T getObject(byte[] bytes) throws DAFCT2Exception {
    T object = null;
    ObjectInputStream objectInputStream = null;

    try {
      ByteArrayInputStream byteArrayInputStream = new ByteArrayInputStream(bytes);
      objectInputStream = new ObjectInputStream(byteArrayInputStream);
      object = (T) objectInputStream.readObject();
      objectInputStream.close();

    } catch (IOException e) {
      object = (T) new String(bytes, StandardCharsets.UTF_8);

    } catch (ClassNotFoundException e) {
      log.error("Unable to Parse Byte Array to " + object.getClass(), e);
      throw new DAFCT2Exception("Unable to Parse Byte Array to " + object.getClass(), e);
    }

    return object;
  }
}
