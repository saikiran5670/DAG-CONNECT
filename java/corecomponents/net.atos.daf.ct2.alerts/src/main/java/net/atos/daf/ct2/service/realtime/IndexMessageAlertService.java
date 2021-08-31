package net.atos.daf.ct2.service.realtime;

import net.atos.daf.ct2.cache.postgres.TableStream;
import net.atos.daf.ct2.cache.postgres.impl.JdbcFormatTableStream;
import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.props.AlertConfigProp;
import net.atos.daf.ct2.serialization.PojoKafkaSerializationSchema;
import net.atos.daf.ct2.util.Utils;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.KeyedStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.Map;
import java.util.Properties;
import java.util.Set;

import static net.atos.daf.ct2.props.AlertConfigProp.KAFKA_DAF_ALERT_PRODUCE_MSG_TOPIC;
import static net.atos.daf.ct2.props.AlertConfigProp.OUTPUT_TAG;

public class IndexMessageAlertService implements Serializable {

    private static final Logger logger = LoggerFactory.getLogger(IndexMessageAlertService.class);
    private static final long serialVersionUID = 1L;


    public static void processIndexKeyStream(KeyedStream<Index, String> indexWindowKeyedStream,
                                             StreamExecutionEnvironment env,
                                             ParameterTool propertiesParamTool,
                                             Map<Object, Object> configMap) {
        /**
         * Alert produce topic
         */
        String dafAlertProduceTopic = propertiesParamTool.get(KAFKA_DAF_ALERT_PRODUCE_MSG_TOPIC);
        Properties kafkaTopicProp = Utils.getKafkaConnectProperties(propertiesParamTool);
        FlinkKafkaProducer<Alert> alertProducerTopic = new FlinkKafkaProducer<Alert>(dafAlertProduceTopic, new PojoKafkaSerializationSchema(dafAlertProduceTopic), kafkaTopicProp, FlinkKafkaProducer.Semantic.EXACTLY_ONCE);

        /**
         * Process stream
         */
        KeyedStream<Tuple2<Index, Payload<Set<Long>>>, String> subscribeVehicleStream =
                indexWindowKeyedStream
                        .connect(AlertConfigProp.vehicleAlertRefSchemaBroadcastStream)
                        .process(new IndexKeyBasedSubscription())
                        .keyBy(tup2 -> tup2.f0.getVin());

        /**
         * Check for alert threshold definition
         */
        SingleOutputStreamOperator<Index> alertProcessStream = subscribeVehicleStream
                .connect(AlertConfigProp.alertUrgencyLevelRefSchemaBroadcastStream)
                .process(new IndexKeyBasedAlertDefService(configMap));

        /**
         * Publish alert on kafka topic
         */
        DataStream<Alert> alertFoundStream = alertProcessStream
                .getSideOutput(OUTPUT_TAG);


        alertFoundStream.addSink(alertProducerTopic);

        /**
         * Store into alert db
         */
        TableStream tableStream = new JdbcFormatTableStream(env, propertiesParamTool);
        tableStream.saveAlertIntoDB(alertFoundStream);
    }


}
