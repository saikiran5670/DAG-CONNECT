package net.atos.daf.ct2.service.realtime;

import net.atos.daf.ct2.cache.postgres.TableStream;
import net.atos.daf.ct2.cache.postgres.impl.JdbcFormatTableStream;
import net.atos.daf.ct2.cache.service.CacheService;
import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.process.config.AlertConfig;
import net.atos.daf.ct2.serialization.PojoKafkaSerializationSchema;
import net.atos.daf.ct2.util.Utils;
import org.apache.flink.api.common.state.ReadOnlyBroadcastState;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.KeyedStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.functions.co.KeyedBroadcastProcessFunction;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.*;

import static net.atos.daf.ct2.props.AlertConfigProp.*;
import static net.atos.daf.ct2.props.AlertConfigProp.OUTPUT_TAG;

public class StatusMessageAlertService  implements Serializable {

    private static final Logger logger = LoggerFactory.getLogger(StatusMessageAlertService.class);
    private static final long serialVersionUID = 1L;

    public static void processStatusKeyStream(KeyedStream<Status, String> statusKeyedStream,
                                             StreamExecutionEnvironment env,
                                             ParameterTool propertiesParamTool,
                                             Map<Object, Object> configMap) {

        /**
         * Alert produce topic
         */
        String dafAlertProduceTopic = propertiesParamTool.get(KAFKA_DAF_ALERT_PRODUCE_MSG_TOPIC);
        String dafAlertProduceNotificationTopic = propertiesParamTool.get(KAFKA_DAF_ALERT_PRODUCE_NOTIFICATION_MSG_TOPIC);
        Properties kafkaTopicProp = Utils.getKafkaConnectProperties(propertiesParamTool);
        FlinkKafkaProducer<Alert> alertProducerTopic = new FlinkKafkaProducer<Alert>(dafAlertProduceTopic, new PojoKafkaSerializationSchema(dafAlertProduceTopic), kafkaTopicProp, FlinkKafkaProducer.Semantic.EXACTLY_ONCE);
        FlinkKafkaProducer<Alert> alertProducerNotificationTopic = new FlinkKafkaProducer<Alert>(dafAlertProduceNotificationTopic, new PojoKafkaSerializationSchema(dafAlertProduceNotificationTopic), kafkaTopicProp, FlinkKafkaProducer.Semantic.EXACTLY_ONCE);


        KeyedStream<Tuple2<Status, Payload<Set<Long>>>, String> statusSubscribeStream = statusKeyedStream
                .connect(vehicleAlertRefSchemaBroadcastStream)
                .process(new StatusKeyBasedSubscription())
                .keyBy(tup2 -> tup2.f0.getVin());

        SingleOutputStreamOperator<Status> alertProcessStream = statusSubscribeStream.connect(alertUrgencyLevelRefSchemaBroadcastStream)
                .process(new StatusKeyBasedAlertDefService(configMap));

        /**
         * Publish alert on kafka topic
         */
        DataStream<Alert> alertFoundStream = alertProcessStream
                .getSideOutput(OUTPUT_TAG);


        alertFoundStream.addSink(alertProducerTopic);
        alertFoundStream.addSink(alertProducerNotificationTopic);

        /**
         * Store into alert db
         */
        TableStream tableStream = new JdbcFormatTableStream(env, propertiesParamTool);
        tableStream.saveAlertIntoDB(alertFoundStream);
    }
}
