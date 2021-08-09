package net.atos.daf.ct2.service.kafka;

import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.serde.KafkaMessageDeSerializeSchema;
import net.atos.daf.ct2.util.Utils;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.KafkaDeserializationSchema;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.Properties;

public class KafkaConnectionService implements Serializable {

    private static final Logger logger = LoggerFactory.getLogger(KafkaConnectionService.class);
    private static final long serialVersionUID = 1L;

    public static DataStream<KafkaRecord<Status>> connectStatusObjectTopic(String topicName, ParameterTool parameterTool, StreamExecutionEnvironment env){
        KafkaService<KafkaRecord<Status>> kafkaService = new KafkaService<>();
        KafkaDeserializationSchema deserializationSchema= new KafkaMessageDeSerializeSchema<Status>();
        Properties kafkaConnectProperties = Utils.getKafkaConnectProperties(parameterTool);
        kafkaConnectProperties.put("sasl.jaas.config",parameterTool.get("status.object.sasl.jaas.config"));
        kafkaConnectProperties.put("bootstrap.servers",parameterTool.get("status.object.bootstrap.servers"));

        return  kafkaService.connectKafkaTopic(
                topicName,
                kafkaConnectProperties,
                deserializationSchema,
                env);
    }
}
