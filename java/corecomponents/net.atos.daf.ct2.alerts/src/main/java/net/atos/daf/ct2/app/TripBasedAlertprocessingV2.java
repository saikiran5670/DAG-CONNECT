package net.atos.daf.ct2.app;

import net.atos.daf.ct2.cache.kafka.KafkaCdcStream;
import net.atos.daf.ct2.cache.kafka.impl.KafkaCdcImpl;
import net.atos.daf.ct2.cache.postgres.TableStream;
import net.atos.daf.ct2.cache.postgres.impl.JdbcFormatTableStream;
import net.atos.daf.ct2.cache.postgres.impl.RichPostgresMapImpl;
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

        String jdbcUrlVinMap = new StringBuilder("jdbc:postgresql://")
                .append(propertiesParamTool.get(DATAMART_POSTGRES_HOST))
                .append(":" + propertiesParamTool.get(DATAMART_POSTGRES_PORT) + "/")
                .append(propertiesParamTool.get(DATAMART_DATABASE))
                .append("?user=" + propertiesParamTool.get(DATAMART_USERNAME))
                .append("&password=" + propertiesParamTool.get(DATAMART_PASSWORD))
                .append("&sslmode="+propertiesParamTool.get(DATAMART_POSTGRES_SSL))
                .toString();

        String thresholdDefUrlVinMap = new StringBuilder("jdbc:postgresql://")
                .append(propertiesParamTool.get(MASTER_POSTGRES_HOST))
                .append(":" + propertiesParamTool.get(MASTER_POSTGRES_PORT) + "/")
                .append(propertiesParamTool.get(MASTER_DATABASE))
                .append("?user=" + propertiesParamTool.get(MASTER_USERNAME))
                .append("&password=" + propertiesParamTool.get(MASTER_PASSWORD))
                .append("&sslmode="+propertiesParamTool.get(MASTER_POSTGRES_SSL))
                .toString();

        DataStreamSource<Row> thresholdStream = tableStream.scanTable(propertiesParamTool.get(ALERT_THRESHOLD_FETCH_QUERY), ALERT_THRESHOLD_SCHEMA_DEF,thresholdDefUrlVinMap);
        DataStreamSource<Row> vinMappingStream = tableStream.scanTable(propertiesParamTool.get(ALERT_MAP_FETCH_QUERY), ALERT_MAP_SCHEMA_DEF,jdbcUrlVinMap);


        SingleOutputStreamOperator<Payload<Object>> thresholdStreamObjectStream = thresholdStream.map(row -> AlertUrgencyLevelRefSchema.builder()
                .alertId(Long.valueOf(String.valueOf(row.getField(0))))
                .alertCategory(String.valueOf(row.getField(1)))
                .alertType(String.valueOf(row.getField(2)))
                .alertState(String.valueOf(row.getField(3)))
                .urgencyLevelType(String.valueOf(row.getField(4)))
                .thresholdValue(row.getField(5) == null ? -1L : Double.valueOf(String.valueOf(row.getField(5))).longValue())
                .unitType(String.valueOf(row.getField(6)))
                .timestamp(System.currentTimeMillis())
                .build()
        ).returns(AlertUrgencyLevelRefSchema.class)
                .map(schema -> Payload.builder().data(Optional.of(Tuple2.of(new AlertCdc(), schema))).build())
                .returns(TypeInformation.of(new TypeHint<Payload<Object>>() {
                    @Override
                    public TypeInformation<Payload<Object>> getTypeInfo() {
                        return super.getTypeInfo();
                    }
                }));


        SingleOutputStreamOperator<VehicleAlertRefSchema> vinMappingStreamObject = vinMappingStream.map(row -> new VehicleAlertRefSchema()
                .withAlertId(Long.valueOf(String.valueOf(row.getField(2))))
                .withVin(String.valueOf(row.getField(1)))
                .withState("A")
        ).returns(VehicleAlertRefSchema.class);

        Properties kafkaTopicProp = Utils.getKafkaConnectProperties(propertiesParamTool);
        FlinkKafkaConsumer<String> kafkaAlertCDCMessageConsumer = new FlinkKafkaConsumer<>(propertiesParamTool.get(KAFKA_DAF_ALERT_CDC_TOPIC), new SimpleStringSchema(), kafkaTopicProp);


        String dafAlertProduceTopic = propertiesParamTool.get(KAFKA_DAF_ALERT_PRODUCE_MSG_TOPIC);
        FlinkKafkaProducer<Alert> alertProducerTopic = new FlinkKafkaProducer<Alert>(dafAlertProduceTopic, new PojoKafkaSerializationSchema(dafAlertProduceTopic), kafkaTopicProp, FlinkKafkaProducer.Semantic.EXACTLY_ONCE);



        KeyedStream<AlertCdc, String> alertCdcStream = env.addSource(kafkaAlertCDCMessageConsumer)
                .map(json ->{
                    logger.info("CDC payload received :: {}", json);
                    CdcPayloadWrapper cdcPayloadWrapper = new CdcPayloadWrapper();
                    try{
                        cdcPayloadWrapper= (CdcPayloadWrapper) Utils.readValueAsObject(json, CdcPayloadWrapper.class);
                    }catch (Exception e){
                        logger.error("Error while parsing alert cdc message : {}",e);
                    }
                    return cdcPayloadWrapper;

                })
                .returns(CdcPayloadWrapper.class)
                .map(cdc -> {
                            AlertCdc alertCdc = new AlertCdc();
                            try{
                                if (cdc.getNamespace().equalsIgnoreCase("alerts")){
                                    alertCdc= (AlertCdc) Utils.readValueAsObject(cdc.getPayload(), AlertCdc.class);
                                    alertCdc.setOperation(cdc.getOperation());
                                }
                            }catch (Exception e){
                                logger.error("Error while parsing alert cdc message : {}",e);
                            }
                            return alertCdc;
                        }
                )
                .returns(AlertCdc.class)
                .filter(alertCdc -> Objects.nonNull(alertCdc.getAlertId()))
                .returns(AlertCdc.class)
                .keyBy(alert -> alert.getAlertId());


        BroadcastStream<VehicleAlertRefSchema> vehicleAlertRefSchemaBroadcastStream = alertCdcStream
                .flatMap(
                        new FlatMapFunction<AlertCdc, List<VehicleAlertRefSchema>>() {
                            @Override
                            public void flatMap(AlertCdc alertCdc, Collector<List<VehicleAlertRefSchema>> collector) throws Exception {
                                List<VehicleAlertRefSchema> lst = new ArrayList<>();
                                for (VinOp vin : alertCdc.getVinOps()) {
                                    VehicleAlertRefSchema schema = new VehicleAlertRefSchema();
                                    schema.setAlertId(Long.valueOf(alertCdc.getAlertId()));
                                    schema.setVin(vin.getVin());
                                    schema.setOp(vin.getOp());
                                    logger.info("CDC record for vin mapping {}", schema);
                                    lst.add(schema);
                                }
                                collector.collect(lst);
                            }
                        }
                )
                .process(new ProcessFunction<List<VehicleAlertRefSchema>, VehicleAlertRefSchema>() {
                    @Override
                    public void processElement(List<VehicleAlertRefSchema> vehicleAlertRefSchemas, Context context, Collector<VehicleAlertRefSchema> collector) throws Exception {
                        for (VehicleAlertRefSchema payload : vehicleAlertRefSchemas) {
                            collector.collect(payload);
                        }
                    }
                })
                .union(vinMappingStreamObject)
                .broadcast(VIN_ALERT_MAP_STATE);


        BroadcastStream<Payload<Object>> alertUrgencyLevelRefSchemaBroadcastStream = alertCdcStream
                .map(alertCdc -> {
                    logger.info("CDC payload after flatten :: {}",alertCdc);
                    return alertCdc;
                })
                .returns(AlertCdc.class)
                .map(new RichPostgresMapImpl(propertiesParamTool) )
                .union(thresholdStreamObjectStream)
                .broadcast(THRESHOLD_CONFIG_DESCRIPTOR);


        FlinkKafkaConsumer<String> kafkaContiMessageConsumer = new FlinkKafkaConsumer<>(propertiesParamTool.get(KAFKA_DAF_STATUS_MSG_TOPIC), new SimpleStringSchema(), kafkaTopicProp);

       /* KeyedStream<Status, String> statusKeyedStream = env.addSource(kafkaContiMessageConsumer)
                .map(json -> (Status) Utils.readValueAsObject(json, Status.class))
                .returns(Status.class)
                .keyBy(status -> status.getVin());*/
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
                        BroadcastState<String, Payload> vinMappingState = context.getBroadcastState(VIN_ALERT_MAP_STATE);
                        Set<Long> vinAlertList = new HashSet<>();
                        if (vehicleAlertRefSchema.getOp().equalsIgnoreCase("I")) {
                            if (vinMappingState.contains(vehicleAlertRefSchema.getVin())) {
                                Payload listPayload = vinMappingState.get(vehicleAlertRefSchema.getVin());
                                vinAlertList = (Set<Long>) listPayload.getData().get();
                                vinAlertList.add(vehicleAlertRefSchema.getAlertId());
                                vinMappingState.put(vehicleAlertRefSchema.getVin(), Payload.builder().data(Optional.of(vinAlertList)).build());
                                logger.info("New alert added for vin : {}, alert id: {}", vehicleAlertRefSchema.getVin(), vehicleAlertRefSchema.getAlertId());
                            } else {
                                logger.info("New alert added for vin : {}, alert id: {}", vehicleAlertRefSchema.getVin(), vehicleAlertRefSchema.getAlertId());
                                vinAlertList.add(vehicleAlertRefSchema.getAlertId());
                                vinMappingState.put(vehicleAlertRefSchema.getVin(), Payload.builder().data(Optional.of(vinAlertList)).build());
                            }
                        }
                        if (vehicleAlertRefSchema.getOp().equalsIgnoreCase("D")) {
                            if (vinMappingState.contains(vehicleAlertRefSchema.getVin())) {
                                vinMappingState.remove(vehicleAlertRefSchema.getVin());
                                logger.info("Alert removed for vin : {}, alert id: {}", vehicleAlertRefSchema, vehicleAlertRefSchema.getAlertId());
                            }
                        }
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
                        Tuple2<AlertCdc, AlertUrgencyLevelRefSchema> thresholdTuple = (Tuple2<AlertCdc, AlertUrgencyLevelRefSchema>) payload.getData().get();
                        logger.info("Alert threshold broadcast payload:: {}", thresholdTuple);
                        BroadcastState<Long, Payload> broadcastState = context.getBroadcastState(THRESHOLD_CONFIG_DESCRIPTOR);
                        AlertCdc f0 = thresholdTuple.f0;
                        AlertUrgencyLevelRefSchema f1 = thresholdTuple.f1;
                        if (Objects.nonNull(f0.getOperation()) && (f0.getOperation().equalsIgnoreCase("D") || f0.getOperation().equalsIgnoreCase("I"))) {
                            logger.info("Removing alert from threshold cache:: {}", f0);
                            try{
                                broadcastState.remove(Long.valueOf(f0.getAlertId()));
                            }catch (Exception e){
                                logger.error("Error while removing threshold from cache:: {}",f0);
                            }
                        } else {
                            logger.info("Adding alert to threshold cache:: {}", f0);
                            List<AlertUrgencyLevelRefSchema> alertDef = new ArrayList<>();
                            if(Objects.nonNull(f1) && broadcastState.contains(Long.valueOf(f1.getAlertId())) ){
                                Payload broadcastPayload = broadcastState.get(Long.valueOf(f1.getAlertId()));
                                alertDef= (List<AlertUrgencyLevelRefSchema>) broadcastPayload.getData().get();
                                alertDef.remove(f1);
                                alertDef.add(f1);
                                broadcastState.put(f1.getAlertId(), Payload.builder().data(Optional.of(alertDef)).build());
                            }else{
                                alertDef.add(f1);
                                broadcastState.put(f1.getAlertId(), Payload.builder().data(Optional.of(alertDef)).build());
                            }

                        }
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


        env.execute(TripBasedAlertprocessingV2.class.getSimpleName());

    }
}
