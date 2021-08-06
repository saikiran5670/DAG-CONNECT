package net.atos.daf.ct2.service.kafka;

import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.flink.streaming.connectors.kafka.KafkaDeserializationSchema;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import java.io.Serializable;
import java.util.Properties;

public class KafkaService<T> implements Serializable {

    private static final Logger logger = LoggerFactory.getLogger(KafkaService.class);
    private static final long serialVersionUID = 1L;

    public DataStream<T> connectKafkaTopic(String topicName, Properties properties, KafkaDeserializationSchema<T> deserializationSchema, StreamExecutionEnvironment env){
        return env.addSource(
                new FlinkKafkaConsumer<T>(
                        topicName,
                        deserializationSchema,
                        properties
                )

        );
    }


}
