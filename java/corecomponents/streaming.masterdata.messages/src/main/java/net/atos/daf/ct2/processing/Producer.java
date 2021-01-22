package net.atos.daf.ct2.processing;

import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.exception.DAFCT2Exception;
import org.apache.kafka.clients.producer.Callback;
import org.apache.kafka.clients.producer.KafkaProducer;
import org.apache.kafka.clients.producer.ProducerRecord;
import org.apache.kafka.clients.producer.RecordMetadata;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.util.Properties;

public class Producer {

  private static final Logger log = LogManager.getLogger(Producer.class);

  private KafkaProducer<String, String> kafkaProducer;

  public KafkaProducer<String, String> createConnection(Properties properties)
      throws DAFCT2Exception {

    try {
      this.kafkaProducer = new KafkaProducer<String, String>(properties);
      log.info("Kafka Producer process started");

    } catch (Exception e) {
      log.error("Unable to instantiated Kafka Producer");
      throw new DAFCT2Exception("Unable to instantiated Kafka Producer", e);
    }
    return this.kafkaProducer;
  }

  public void publishMessage(String key, String value, Properties properties) {

    ProducerRecord<String, String> producerRecord =
        new ProducerRecord<String, String>(properties.getProperty(DAFCT2Constant.MASTER_DATA_TOPIC_NAME), key, value);

    this.kafkaProducer.send(
        producerRecord,
        new Callback() {

          @Override
          public void onCompletion(RecordMetadata metadata, Exception exception) {
            if (exception != null) {
              log.error("Exception: " + exception.getMessage());
            }
          }
        });

    this.kafkaProducer.flush();
  }

  public void closePublisher() {
    this.kafkaProducer.close();
    log.info("Disconnected Kafka Producer connection");
  }
}
