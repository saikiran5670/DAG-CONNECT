package net.atos.daf.ct2.app;

import net.atos.daf.ct2.cache.kafka.KafkaCdcStream;
import net.atos.daf.ct2.cache.kafka.KafkaCdcStreamV2;
import net.atos.daf.ct2.cache.kafka.impl.KafkaCdcImpl;
import net.atos.daf.ct2.cache.kafka.impl.KafkaCdcImplV2;
import net.atos.daf.ct2.cache.postgres.TableStream;
import net.atos.daf.ct2.cache.postgres.impl.JdbcFormatTableStream;
import net.atos.daf.ct2.cache.postgres.impl.RichPostgresMapImpl;
import net.atos.daf.ct2.cache.service.CacheService;
import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.kafka.AlertCdc;
import net.atos.daf.ct2.models.kafka.CdcPayloadWrapper;
import net.atos.daf.ct2.models.kafka.VinOp;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.process.config.AlertConfig;
import net.atos.daf.ct2.serialization.PojoKafkaSerializationSchema;
import net.atos.daf.ct2.service.kafka.KafkaConnectionService;
import net.atos.daf.ct2.service.logistic.ProcessTripBasedService;
import net.atos.daf.ct2.util.Utils;
import org.apache.flink.api.common.functions.FlatMapFunction;
import org.apache.flink.api.common.serialization.SimpleStringSchema;
import org.apache.flink.api.common.state.BroadcastState;
import org.apache.flink.api.common.state.ReadOnlyBroadcastState;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.connector.jdbc.JdbcConnectionOptions;
import org.apache.flink.connector.jdbc.JdbcExecutionOptions;
import org.apache.flink.connector.jdbc.JdbcSink;
import org.apache.flink.streaming.api.datastream.*;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.functions.ProcessFunction;
import org.apache.flink.streaming.api.functions.co.KeyedBroadcastProcessFunction;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.apache.flink.types.Row;
import org.apache.flink.util.Collector;
import org.hibernate.Cache;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.*;
import java.util.stream.Collectors;

import static net.atos.daf.ct2.process.functions.LogisticAlertFunction.*;
import static net.atos.daf.ct2.props.AlertConfigProp.*;
import static net.atos.daf.ct2.props.AlertConfigProp.ALERT_MAP_SCHEMA_DEF;

public class TripBasedAlertprocessingV2 implements Serializable {
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
                    excessiveDriveTime,
                    excessiveGlobalMileage,
                    excessiveDistanceDone
            ));
        }};

        /**
         * Scan and join the table
         */
        TableStream tableStream = new JdbcFormatTableStream(env, propertiesParamTool);

        /**
         *  Booting cache
         */
        KafkaCdcStreamV2 kafkaCdcStreamV2 = new KafkaCdcImplV2(env,propertiesParamTool);
        Tuple2<BroadcastStream<VehicleAlertRefSchema>, BroadcastStream<Payload<Object>>> bootCache = kafkaCdcStreamV2.bootCache();

        BroadcastStream<VehicleAlertRefSchema> vehicleAlertRefSchemaBroadcastStream = bootCache.f0;
        BroadcastStream<Payload<Object>> alertUrgencyLevelRefSchemaBroadcastStream = bootCache.f1;


        String dafAlertProduceTopic = propertiesParamTool.get(KAFKA_DAF_ALERT_PRODUCE_MSG_TOPIC);
        String dafAlertProduceNotificationTopic = propertiesParamTool.get(KAFKA_DAF_ALERT_PRODUCE_NOTIFICATION_MSG_TOPIC);
        Properties kafkaTopicProp = Utils.getKafkaConnectProperties(propertiesParamTool);
        FlinkKafkaProducer<Alert> alertProducerTopic = new FlinkKafkaProducer<Alert>(dafAlertProduceTopic, new PojoKafkaSerializationSchema(dafAlertProduceTopic), kafkaTopicProp, FlinkKafkaProducer.Semantic.EXACTLY_ONCE);
        FlinkKafkaProducer<Alert> alertProducerNotificationTopic = new FlinkKafkaProducer<Alert>(dafAlertProduceNotificationTopic, new PojoKafkaSerializationSchema(dafAlertProduceNotificationTopic), kafkaTopicProp, FlinkKafkaProducer.Semantic.EXACTLY_ONCE);


        KeyedStream<net.atos.daf.ct2.pojo.standard.Status, String> statusKeyedStream = KafkaConnectionService.connectStatusObjectTopic(
                        propertiesParamTool.get(KAFKA_DAF_STATUS_MSG_TOPIC),
                        propertiesParamTool,
                        env)
                .map(statusKafkaRecord -> statusKafkaRecord.getValue())
                .returns(net.atos.daf.ct2.pojo.standard.Status.class)
                .keyBy(status ->  status.getVin() !=null ? status.getVin() : status.getVid());


        KeyedStream<Tuple2<Status, Payload<Set<Long>>>, String> statusSubscribeStream = statusKeyedStream
                .connect(vehicleAlertRefSchemaBroadcastStream)
                .process(new KeyedBroadcastProcessFunction<Object, Status, VehicleAlertRefSchema, Tuple2<Status, Payload<Set<Long>>>>() {
                    @Override
                    public void processElement(Status status, ReadOnlyContext readOnlyContext, Collector<Tuple2<Status, Payload<Set<Long>>>> collector) throws Exception {
                        logger.info("Process status message for vim map check:: {}", status);
                        ReadOnlyBroadcastState<String, Payload> vinMapState = readOnlyContext.getBroadcastState(VIN_ALERT_MAP_STATE);
                        if (vinMapState.contains(status.getVin())) {
                            logger.info("Vin subscribe for alert status:: {}", status);
                            collector.collect(Tuple2.of(status, vinMapState.get(status.getVin())));
                        }
                    }

                    @Override
                    public void processBroadcastElement(VehicleAlertRefSchema vehicleAlertRefSchema, Context context, Collector<Tuple2<Status, Payload<Set<Long>>>> collector) throws Exception {
                        /**
                         * Update vin mapping cache
                         */
                        CacheService.updateVinMappingCache(vehicleAlertRefSchema, context);
                    }
                })
                .keyBy(tup2 -> tup2.f0.getVin());


        SingleOutputStreamOperator<Status> alertProcessStream = statusSubscribeStream.connect(alertUrgencyLevelRefSchemaBroadcastStream)
                .process(new KeyedBroadcastProcessFunction<Object, Tuple2<Status, Payload<Set<Long>>>, Payload<Object>, Status>() {
                    @Override
                    public void processElement(Tuple2<Status, Payload<Set<Long>>> statustup2, ReadOnlyContext readOnlyContext, Collector<Status> collector) throws Exception {
                        ReadOnlyBroadcastState<Long, Payload> broadcastState = readOnlyContext.getBroadcastState(THRESHOLD_CONFIG_DESCRIPTOR);
                        logger.info("Fetch alert definition from cache for {}", statustup2);
                        Status f0 = statustup2.f0;
                        Payload<Set<Long>> f1 = statustup2.f1;
                        Set<Long> alertIds = f1.getData().get();
                        Map<String, Object> functionThresh = new HashMap<>();
                        List<AlertUrgencyLevelRefSchema> excessiveGlobalMileageAlertDef = new ArrayList<>();
                        List<AlertUrgencyLevelRefSchema> excessiveDistanceDoneAlertDef = new ArrayList<>();
                        List<AlertUrgencyLevelRefSchema> excessiveDriveTimeAlertDef = new ArrayList<>();
                        for (Long alertId : alertIds) {
                            if (broadcastState.contains(alertId)) {
                                List<AlertUrgencyLevelRefSchema> thresholdSet = (List<AlertUrgencyLevelRefSchema>) broadcastState.get(alertId).getData().get();
                                for(AlertUrgencyLevelRefSchema schema : thresholdSet){
                                    if (schema.getAlertCategory().equalsIgnoreCase("L") && schema.getAlertType().equalsIgnoreCase("G")) {
                                        excessiveGlobalMileageAlertDef.add(schema);
                                    }
                                    if (schema.getAlertCategory().equalsIgnoreCase("L") && schema.getAlertType().equalsIgnoreCase("D")) {
                                        excessiveDistanceDoneAlertDef.add(schema);
                                    }
                                    if (schema.getAlertCategory().equalsIgnoreCase("L") && schema.getAlertType().equalsIgnoreCase("U")) {
                                        excessiveDriveTimeAlertDef.add(schema);
                                    }
                                }
                            }
                        }

                        functionThresh.put("excessiveGlobalMileage", excessiveGlobalMileageAlertDef);
                        functionThresh.put("excessiveDistanceDone", excessiveDistanceDoneAlertDef);
                        functionThresh.put("excessiveDriveTime", excessiveDriveTimeAlertDef);
                        AlertConfig
                                .buildMessage(f0, configMap, functionThresh)
                                .process()
                                .getAlert()
                                .ifPresent(
                                        alerts -> {
                                            alerts.stream()
                                                    .forEach(alert -> readOnlyContext.output(OUTPUT_TAG, alert));
                                        }
                                );
                        logger.info("Alert process for  {} check those alerts {}", f0, functionThresh);
                        collector.collect(f0);
                    }

                    @Override
                    public void processBroadcastElement(Payload<Object> payload, Context context, Collector<Status> collector) throws Exception {
                        CacheService.updateAlertDefinationCache(payload,context);
                    }
                });

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
        tableStream.saveAlertIntoDB(alertFoundStream);


        env.execute(TripBasedAlertprocessingV2.class.getSimpleName());

    }
}
