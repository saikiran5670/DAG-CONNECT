package net.atos.daf.ct2.app;

import net.atos.daf.ct2.cache.service.CacheService;
import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.process.config.AlertConfig;
import net.atos.daf.ct2.serialization.PojoKafkaSerializationSchema;
import net.atos.daf.ct2.service.kafka.KafkaConnectionService;
import net.atos.daf.ct2.service.logistic.ProcessTripBasedService;
import net.atos.daf.ct2.util.Utils;
import org.apache.flink.api.common.serialization.SimpleStringSchema;
import org.apache.flink.api.common.state.ReadOnlyBroadcastState;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.connector.jdbc.JdbcConnectionOptions;
import org.apache.flink.connector.jdbc.JdbcExecutionOptions;
import org.apache.flink.connector.jdbc.JdbcSink;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.KeyedStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.functions.co.KeyedBroadcastProcessFunction;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.*;
import java.util.stream.Collectors;

import static net.atos.daf.ct2.process.functions.LogisticAlertFunction.excessiveDistanceDone;
import static net.atos.daf.ct2.process.functions.LogisticAlertFunction.excessiveGlobalMileage;
import static net.atos.daf.ct2.props.AlertConfigProp.*;
@Deprecated
public class TripBasedAlertProcessing implements Serializable {

    private static final Logger logger = LoggerFactory.getLogger(TripBasedAlertProcessing.class);
    private static final long serialVersionUID = 1L;

    public static void main(String[] args) throws Exception {
        final StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();
        ParameterTool parameterTool = ParameterTool.fromArgs(args);
        logger.info("AlertProcessing started with properties :: {}", parameterTool.getProperties());
        /**
         * Creating param tool from given property
         */
        ParameterTool propertiesParamTool = ParameterTool.fromPropertiesFile(parameterTool.get("prop"));
        env.getConfig().setGlobalJobParameters(propertiesParamTool);

        logger.info("PropertiesParamTool :: {}", parameterTool.getProperties());

        /**
         * Logistics functions defined
         */
        Map<Object, Object> configMap = new HashMap() {{
            put("functions", Arrays.asList(
                    excessiveGlobalMileage,
                    excessiveDistanceDone
            ));
        }};

        /**
         * Broadcast alert mapping cache
         */
        BroadcastStream<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>> cdcBroadcastStream = CacheService.broadcastCache(env, propertiesParamTool);

        /**
         * Kafka topic connector properties
         */
        Properties kafkaTopicProp = Utils.getKafkaConnectProperties(propertiesParamTool);
        FlinkKafkaConsumer<String> kafkaContiMessageConsumer = new FlinkKafkaConsumer<>(propertiesParamTool.get(KAFKA_DAF_STATUS_MSG_TOPIC), new SimpleStringSchema(), kafkaTopicProp);

        String dafAlertProduceTopic = propertiesParamTool.get(KAFKA_DAF_ALERT_PRODUCE_MSG_TOPIC);
        FlinkKafkaProducer<Alert> alertProducerTopic = new FlinkKafkaProducer<Alert>(dafAlertProduceTopic, new PojoKafkaSerializationSchema(dafAlertProduceTopic), kafkaTopicProp, FlinkKafkaProducer.Semantic.EXACTLY_ONCE);

        /**
         * Consume status message
         */

        KeyedStream<Status, String> statusKeyedStream = env.addSource(kafkaContiMessageConsumer)
                .map(json -> (Status) Utils.readValueAsObject(json, Status.class))
                .returns(Status.class)
                .keyBy(status -> status.getVin());

        /*KeyedStream<net.atos.daf.ct2.pojo.standard.Status, String> statusKeyedStream = KafkaConnectionService.connectStatusObjectTopic(
                propertiesParamTool.get(KAFKA_DAF_STATUS_MSG_TOPIC),
                propertiesParamTool,
                env)
                .map(statusKafkaRecord -> statusKafkaRecord.getValue())
                .returns(net.atos.daf.ct2.pojo.standard.Status.class)
                .keyBy(status -> status.getVin());*/

        /**
         * Process Conti status message for alert generation
         */

        SingleOutputStreamOperator<Status> alertProcessStream = statusKeyedStream
                .connect(cdcBroadcastStream)
                .process(new ProcessTripBasedService());

        /**
         * Publish alert on kafka topic
         */
        DataStream<Alert> alertFoundStream = alertProcessStream
                .getSideOutput(OUTPUT_TAG);


        alertFoundStream.addSink(alertProducerTopic);

        /**
         * Store into alert db
         */
        String jdbcInsertUrl = new StringBuilder("jdbc:postgresql://")
                .append(propertiesParamTool.get(DATAMART_POSTGRES_HOST))
                .append(":" + propertiesParamTool.get(DATAMART_POSTGRES_PORT) + "/")
                .append(propertiesParamTool.get(DATAMART_DATABASE))
                .append("?user=" + propertiesParamTool.get(DATAMART_USERNAME))
                .append("&password=" + propertiesParamTool.get(DATAMART_PASSWORD))
                .append("&sslmode="+propertiesParamTool.get(DATAMART_POSTGRES_SSL))
                .toString();

        alertFoundStream
                .addSink(JdbcSink.sink(
                        propertiesParamTool.get("postgres.insert.into.alerts"),
                        (ps, a) -> {
                            ps.setString(1, a.getTripid());
                            ps.setString(2, a.getVin());
                            ps.setString(3, a.getCategoryType());
                            ps.setString(4, a.getType());
                            ps.setLong(5, Long.valueOf(a.getAlertid()));
                            ps.setLong(6, Long.valueOf(a.getAlertGeneratedTime()));
                            ps.setLong(7, Long.valueOf(a.getAlertGeneratedTime()));
                            ps.setString(8, a.getUrgencyLevelType());
                        },
                        JdbcExecutionOptions.builder()
                                .withMaxRetries(0)
                                .withBatchSize(1)
                                .build(),
                        new JdbcConnectionOptions.JdbcConnectionOptionsBuilder()
                                .withUrl(jdbcInsertUrl)
                                .withDriverName(propertiesParamTool.get("driver.class.name"))
                                .build()));

        env.execute("TripBasedAlertProcessing");

    }
}
