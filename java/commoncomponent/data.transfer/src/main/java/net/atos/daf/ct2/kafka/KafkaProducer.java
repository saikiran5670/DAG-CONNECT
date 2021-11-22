package net.atos.daf.ct2.kafka;

import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.serde.KafkaMessageSerializeSchema;
import net.atos.daf.ct2.util.Utils;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.Objects;
import java.util.Properties;

import static net.atos.daf.ct2.props.DataTransferProp.*;

public class KafkaProducer implements Serializable {
    private static final Logger logger = LoggerFactory.getLogger(KafkaConnectionService.class);
    private static final long serialVersionUID = 1L;

    public static void transferMonitorMsg(DataStream<KafkaRecord<Monitor>> monitorStream,
                                          ParameterTool propertiesParamTool,
                                          final StreamExecutionEnvironment env){
        String destinationTopic = propertiesParamTool.get(KAFKA_MONITOR_TOPIC_DESTINATION);
        Properties kafkaTopicProp = Utils.getKafkaConnectProperties(propertiesParamTool);
        kafkaTopicProp.put("bootstrap.servers",propertiesParamTool.get(KAFKA_MONITOR_BOOTSTRAP_SERVER_DESTINATION));
        kafkaTopicProp.put("sasl.jaas.config",propertiesParamTool.get(KAFKA_MONITOR_JAAS_CONFIG_DESTINATION));
        monitorStream
                .map(record -> {
                    logger.info("Monitor message received for transfer {}",record.getValue());
                    String key =Objects.nonNull(record.getValue().getVin()) ?
                            record.getValue().getVin() : record.getValue().getVid();
                    record.setKey(key);
                    return record;
                }).returns(TypeInformation.of(new TypeHint<KafkaRecord<Monitor>>() {
                    @Override
                    public TypeInformation<KafkaRecord<Monitor>> getTypeInfo() {
                        return super.getTypeInfo();
                    }
                }))
                .keyBy(monitorKafkaRecord -> Objects.nonNull(monitorKafkaRecord.getValue().getVin()) ?
                        monitorKafkaRecord.getValue().getVin() : monitorKafkaRecord.getValue().getVid())
                .addSink(
                        new FlinkKafkaProducer<KafkaRecord<Monitor>>(
                                destinationTopic,
                                new KafkaMessageSerializeSchema<Monitor>(destinationTopic),
                                kafkaTopicProp,
                                FlinkKafkaProducer.Semantic.EXACTLY_ONCE));
    }

    public static void transferStatusMsg(DataStream<KafkaRecord<Status>> statusStream,
                                          ParameterTool propertiesParamTool,
                                          final StreamExecutionEnvironment env){
        String destinationTopic = propertiesParamTool.get(KAFKA_STATUS_TOPIC_DESTINATION);
        Properties kafkaTopicProp = Utils.getKafkaConnectProperties(propertiesParamTool);
        kafkaTopicProp.put("bootstrap.servers",propertiesParamTool.get(KAFKA_STATUS_BOOTSTRAP_SERVER_DESTINATION));
        kafkaTopicProp.put("sasl.jaas.config",propertiesParamTool.get(KAFKA_STATUS_JAAS_CONFIG_DESTINATION));
        statusStream
                .map(record -> {
                    logger.info("Status message received for transfer {}",record.getValue());
                    String key =Objects.nonNull(record.getValue().getVin()) ?
                            record.getValue().getVin() : record.getValue().getVid();
                    record.setKey(key);
                    return record;
                }).returns(TypeInformation.of(new TypeHint<KafkaRecord<Status>>() {
                    @Override
                    public TypeInformation<KafkaRecord<Status>> getTypeInfo() {
                        return super.getTypeInfo();
                    }
                }))
                .keyBy(record -> Objects.nonNull(record.getValue().getVin()) ?
                        record.getValue().getVin() : record.getValue().getVid())
                .addSink(
                        new FlinkKafkaProducer<KafkaRecord<Status>>(
                                destinationTopic,
                                new KafkaMessageSerializeSchema<Status>(destinationTopic),
                                kafkaTopicProp,
                                FlinkKafkaProducer.Semantic.EXACTLY_ONCE));
    }

    public static void transferIndexMsg(DataStream<KafkaRecord<Index>> indexStream,
                                         ParameterTool propertiesParamTool,
                                         final StreamExecutionEnvironment env){
        String destinationTopic = propertiesParamTool.get(KAFKA_INDEX_TOPIC_DESTINATION);
        Properties kafkaTopicProp = Utils.getKafkaConnectProperties(propertiesParamTool);
        kafkaTopicProp.put("bootstrap.servers",propertiesParamTool.get(KAFKA_INDEX_BOOTSTRAP_SERVER_DESTINATION));
        kafkaTopicProp.put("sasl.jaas.config",propertiesParamTool.get(KAFKA_INDEX_JAAS_CONFIG_DESTINATION));
        indexStream
                .map(record -> {
                    logger.info("Index message received for transfer {}",record.getValue());
                    String key =Objects.nonNull(record.getValue().getVin()) ?
                            record.getValue().getVin() : record.getValue().getVid();
                    record.setKey(key);
                    return record;
                }).returns(TypeInformation.of(new TypeHint<KafkaRecord<Index>>() {
                    @Override
                    public TypeInformation<KafkaRecord<Index>> getTypeInfo() {
                        return super.getTypeInfo();
                    }
                }))
                .keyBy(record -> Objects.nonNull(record.getValue().getVin()) ?
                        record.getValue().getVin() : record.getValue().getVid())
                .addSink(
                        new FlinkKafkaProducer<KafkaRecord<Index>>(
                                destinationTopic,
                                new KafkaMessageSerializeSchema<Index>(destinationTopic),
                                kafkaTopicProp,
                                FlinkKafkaProducer.Semantic.EXACTLY_ONCE));
    }
}
