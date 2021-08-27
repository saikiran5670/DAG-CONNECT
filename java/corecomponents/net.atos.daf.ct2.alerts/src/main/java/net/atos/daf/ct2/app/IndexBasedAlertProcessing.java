package net.atos.daf.ct2.app;

import net.atos.daf.ct2.cache.kafka.KafkaCdcStreamV2;
import net.atos.daf.ct2.cache.kafka.impl.KafkaCdcImplV2;
import net.atos.daf.ct2.cache.postgres.TableStream;
import net.atos.daf.ct2.cache.postgres.impl.JdbcFormatTableStream;
import net.atos.daf.ct2.cache.service.CacheService;
import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.process.config.AlertConfig;
import net.atos.daf.ct2.serialization.PojoKafkaSerializationSchema;
import net.atos.daf.ct2.service.kafka.KafkaConnectionService;
import net.atos.daf.ct2.util.IndexGenerator;
import net.atos.daf.ct2.util.Utils;
import org.apache.flink.api.common.functions.ReduceFunction;
import org.apache.flink.api.common.state.ReadOnlyBroadcastState;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.KeyedStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.functions.co.KeyedBroadcastProcessFunction;
import org.apache.flink.streaming.api.functions.timestamps.BoundedOutOfOrdernessTimestampExtractor;
import org.apache.flink.streaming.api.functions.windowing.ProcessWindowFunction;
import org.apache.flink.streaming.api.windowing.assigners.TumblingEventTimeWindows;
import org.apache.flink.streaming.api.windowing.time.Time;
import org.apache.flink.streaming.api.windowing.windows.TimeWindow;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import java.io.Serializable;
import java.util.*;

import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.hoursOfServiceFun;
import static net.atos.daf.ct2.process.functions.LogisticAlertFunction.*;
import static net.atos.daf.ct2.props.AlertConfigProp.*;
import static net.atos.daf.ct2.util.Utils.convertDateToMillis;

public class IndexBasedAlertProcessing implements Serializable {
    private static final Logger logger = LoggerFactory.getLogger(IndexBasedAlertProcessing.class);
    private static final long serialVersionUID = 1L;

    public static void main(String[] args) throws Exception {
        final StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();
        ParameterTool parameterTool = ParameterTool.fromArgs(args);
        logger.info("AlertProcessing started with properties :: {}", parameterTool.getProperties());
        /**
         * Creating param tool from given property
         */
        ParameterTool propertiesParamTool = ParameterTool.fromPropertiesFile(parameterTool.get("prop"));

        logger.info("PropertiesParamTool :: {}", parameterTool.getProperties());

        /**
         * RealTime functions defined
         */
        Map<Object, Object> configMap = new HashMap() {{
            put("functions", Arrays.asList(
                    hoursOfServiceFun
            ));
        }};

        /**
         * Alert produce topic
         */
        String dafAlertProduceTopic = propertiesParamTool.get(KAFKA_DAF_ALERT_PRODUCE_MSG_TOPIC);
        Properties kafkaTopicProp = Utils.getKafkaConnectProperties(propertiesParamTool);
        FlinkKafkaProducer<Alert> alertProducerTopic = new FlinkKafkaProducer<Alert>(dafAlertProduceTopic, new PojoKafkaSerializationSchema(dafAlertProduceTopic), kafkaTopicProp, FlinkKafkaProducer.Semantic.EXACTLY_ONCE);


        /**
         *  Booting cache
         */
        KafkaCdcStreamV2 kafkaCdcStreamV2 = new KafkaCdcImplV2(env,propertiesParamTool);
        Tuple2<BroadcastStream<VehicleAlertRefSchema>, BroadcastStream<Payload<Object>>> bootCache = kafkaCdcStreamV2.bootCache();

        BroadcastStream<VehicleAlertRefSchema> vehicleAlertRefSchemaBroadcastStream = bootCache.f0;
        BroadcastStream<Payload<Object>> alertUrgencyLevelRefSchemaBroadcastStream = bootCache.f1;

        SingleOutputStreamOperator<Index> indexStringStream=KafkaConnectionService.connectIndexObjectTopic(
                        propertiesParamTool.get(KAFKA_EGRESS_INDEX_MSG_TOPIC),
                        propertiesParamTool, env)
                .map(indexKafkaRecord -> indexKafkaRecord.getValue())
                .returns(Index.class);

        /*SingleOutputStreamOperator<Index> indexStringStream = env.addSource(new IndexGenerator())
                .returns(Index.class);*/

        KeyedStream<Tuple2<Index, Payload<Set<Long>>>, String> subscribeVehicleStream = indexStringStream
                .assignTimestampsAndWatermarks(
                        new BoundedOutOfOrdernessTimestampExtractor<Index>(Time.minutes(0)) {
                            @Override
                            public long extractTimestamp(Index index) {
                                return convertDateToMillis(index.getEvtDateTime());
                            }
                        }
                )
                .keyBy(index -> index.getDocument() !=null ? index.getDocument().getTripID() : "null")
                .window(TumblingEventTimeWindows.of(Time.minutes(5)))
                .reduce(new ReduceFunction<Index>() {
                    @Override
                    public Index reduce(Index index, Index t1) throws Exception {
                        return t1;
                    }
                })
                .keyBy(index -> index.getVin() !=null ? index.getVin() : index.getVid())
                .connect(vehicleAlertRefSchemaBroadcastStream)
                .process(new KeyedBroadcastProcessFunction<String, Index, VehicleAlertRefSchema, Tuple2<Index, Payload<Set<Long>>>>() {
                    @Override
                    public void processElement(Index index, KeyedBroadcastProcessFunction<String, Index, VehicleAlertRefSchema, Tuple2<Index, Payload<Set<Long>>>>.ReadOnlyContext readOnlyContext, Collector<Tuple2<Index, Payload<Set<Long>>>> collector) throws Exception {
                        logger.info("Process index message for vim map check:: {}", index);
                        ReadOnlyBroadcastState<String, Payload> vinMapState = readOnlyContext.getBroadcastState(VIN_ALERT_MAP_STATE);
                        if (vinMapState.contains(index.getVin())) {
                            logger.info("Vin subscribe for alert status:: {}", index);
                            collector.collect(Tuple2.of(index, vinMapState.get(index.getVin())));
                        }
                    }

                    @Override
                    public void processBroadcastElement(VehicleAlertRefSchema vehicleAlertRefSchema, KeyedBroadcastProcessFunction<String, Index, VehicleAlertRefSchema, Tuple2<Index, Payload<Set<Long>>>>.Context context, Collector<Tuple2<Index, Payload<Set<Long>>>> collector) throws Exception {
                        /**
                         * Update vin mapping cache
                         */
                        CacheService.updateVinMappingCache(vehicleAlertRefSchema, context);
                    }
                })
                .keyBy(tup2 -> tup2.f0.getVin());



        /**
         * Check for alert threshold definition
         */
        SingleOutputStreamOperator<Index> alertProcessStream = subscribeVehicleStream
                .connect(alertUrgencyLevelRefSchemaBroadcastStream)
                .process(new KeyedBroadcastProcessFunction<Object, Tuple2<Index, Payload<Set<Long>>>, Payload<Object>, Index>() {
                    @Override
                    public void processElement(Tuple2<Index, Payload<Set<Long>>> indexTup2, KeyedBroadcastProcessFunction<Object, Tuple2<Index, Payload<Set<Long>>>, Payload<Object>, Index>.ReadOnlyContext readOnlyContext, Collector<Index> collector) throws Exception {
                        ReadOnlyBroadcastState<Long, Payload> broadcastState = readOnlyContext.getBroadcastState(THRESHOLD_CONFIG_DESCRIPTOR);
                        logger.info("Fetch alert definition from cache for {}", indexTup2);
                        Index f0 = indexTup2.f0;
                        Payload<Set<Long>> f1 = indexTup2.f1;
                        Set<Long> alertIds = f1.getData().get();
                        Map<String, Object> functionThresh = new HashMap<>();
                        List<AlertUrgencyLevelRefSchema> hoursOfServiceAlertDef = new ArrayList<>();
                        for (Long alertId : alertIds) {
                            if (broadcastState.contains(alertId)) {
                                List<AlertUrgencyLevelRefSchema> thresholdSet = (List<AlertUrgencyLevelRefSchema>) broadcastState.get(alertId).getData().get();
                                for (AlertUrgencyLevelRefSchema schema : thresholdSet) {
                                    if (schema.getAlertCategory().equalsIgnoreCase("L") && schema.getAlertType().equalsIgnoreCase("S")) {
                                        hoursOfServiceAlertDef.add(schema);
                                    }
                                }
                            }
                        }
                        logger.info("Alert definition from cache for vin :{} alertDef {}", f0.getVin(), hoursOfServiceAlertDef);
                        functionThresh.put("hoursOfService", hoursOfServiceAlertDef);
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
                    public void processBroadcastElement(Payload<Object> payload, KeyedBroadcastProcessFunction<Object, Tuple2<Index, Payload<Set<Long>>>, Payload<Object>, Index>.Context context, Collector<Index> collector) throws Exception {
                        /**
                         * Update threshold alert definition
                         */
                        CacheService.updateAlertDefinationCache(payload, context);
                    }
                });

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

        env.execute(IndexBasedAlertProcessing.class.getSimpleName());


    }
}
