package net.atos.daf.ct2.serialization;


import lombok.SneakyThrows;
import net.atos.daf.ct2.util.Utils;
import org.apache.flink.streaming.connectors.kafka.KafkaSerializationSchema;
import org.apache.kafka.clients.producer.ProducerRecord;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import javax.annotation.Nullable;
import java.nio.charset.StandardCharsets;

public class PojoKafkaSerializationSchema<T> implements KafkaSerializationSchema<T> {

    private static final Logger logger = LoggerFactory.getLogger(PojoKafkaSerializationSchema.class);
    private String topic;

    public PojoKafkaSerializationSchema(String topic) {
        this.topic = topic;
    }

    @Override
    @SneakyThrows
    public ProducerRecord<byte[], byte[]> serialize(T o, @Nullable Long aLong) {
        byte[] data = Utils.writeValueAsString(o).getBytes(StandardCharsets.UTF_8);
        logger.info("Kafka data send : {}" , new String(data, StandardCharsets.UTF_8));
        return new ProducerRecord<byte[], byte[]>(topic,"alerts".getBytes(StandardCharsets.UTF_8), data);
    }
}
