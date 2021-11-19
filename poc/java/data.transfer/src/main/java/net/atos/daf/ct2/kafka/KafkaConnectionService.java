package net.atos.daf.ct2.kafka;

import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
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

import static net.atos.daf.ct2.props.DataTransferProp.*;

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

    public static DataStream<KafkaRecord<Index>> connectIndexObjectTopic(String topicName, ParameterTool parameterTool, StreamExecutionEnvironment env){
        KafkaService<KafkaRecord<Index>> kafkaService = new KafkaService<>();
        KafkaDeserializationSchema deserializationSchema= new KafkaMessageDeSerializeSchema<Index>();
        Properties kafkaConnectProperties = Utils.getKafkaConnectProperties(parameterTool);
        kafkaConnectProperties.put("sasl.jaas.config",parameterTool.get(KAFKA_INDEX_SASL_JAAS_CONFIG));
        kafkaConnectProperties.put("bootstrap.servers",parameterTool.get(KAFKA_INDEX_BOOTSTRAP_SERVER));
        return  kafkaService.connectKafkaTopic(
                topicName,
                kafkaConnectProperties,
                deserializationSchema,
                env);
    }
    
    public static DataStream<KafkaRecord<Monitor>> connectMonitoringObjectTopic(String topicName, ParameterTool parameterTool, StreamExecutionEnvironment env){
        KafkaService<KafkaRecord<Monitor>> kafkaService = new KafkaService<>();
        KafkaDeserializationSchema deserializationSchema= new KafkaMessageDeSerializeSchema<Monitor>();
        Properties kafkaConnectProperties = Utils.getKafkaConnectProperties(parameterTool);
        kafkaConnectProperties.put("sasl.jaas.config",parameterTool.get(KAFKA_MONITOR_JAAS_CONFIG_SOURCE));
        kafkaConnectProperties.put("bootstrap.servers",parameterTool.get(KAFKA_MONITOR_BOOTSTRAP_SERVER_SOURCE));
        return  kafkaService.connectKafkaTopic(
                topicName,
                kafkaConnectProperties,
                deserializationSchema,
                env);
    }
}
