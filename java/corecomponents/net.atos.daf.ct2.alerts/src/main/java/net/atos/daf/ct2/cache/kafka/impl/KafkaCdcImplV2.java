package net.atos.daf.ct2.cache.kafka.impl;

import net.atos.daf.ct2.cache.kafka.KafkaCdcStreamV2;
import net.atos.daf.ct2.cache.postgres.impl.RichPostgresMapImpl;
import net.atos.daf.ct2.cache.service.CacheService;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.kafka.AlertCdc;
import net.atos.daf.ct2.models.kafka.CdcPayloadWrapper;
import net.atos.daf.ct2.models.kafka.VinOp;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import net.atos.daf.ct2.util.Utils;
import org.apache.flink.api.common.functions.FlatMapFunction;
import org.apache.flink.api.common.serialization.SimpleStringSchema;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.KeyedStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.functions.ProcessFunction;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.List;
import java.util.Objects;
import java.util.Properties;

import static net.atos.daf.ct2.props.AlertConfigProp.*;

public class KafkaCdcImplV2 extends KafkaCdcStreamV2 implements Serializable {

    private static final long serialVersionUID = 1L;
    private static final Logger logger = LoggerFactory.getLogger(KafkaCdcImplV2.class);

    public KafkaCdcImplV2(StreamExecutionEnvironment env, ParameterTool propertiesParamTool) {
        super(env,propertiesParamTool);
    }

    @Override
    protected KeyedStream<AlertCdc, String> init() {
        Properties kafkaTopicProp = Utils.getKafkaConnectProperties(parameterTool);
        FlinkKafkaConsumer<String> kafkaAlertCDCMessageConsumer = new FlinkKafkaConsumer<>(parameterTool.get(KAFKA_DAF_ALERT_CDC_TOPIC), new SimpleStringSchema(), kafkaTopicProp);
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

               return alertCdcStream;
    }

    @Override
    protected Tuple2<BroadcastStream<VehicleAlertRefSchema>, BroadcastStream<Payload<Object>>> processCdcPayload(KeyedStream<AlertCdc, String> alertCdcStream) {
        Tuple2<SingleOutputStreamOperator<VehicleAlertRefSchema>, SingleOutputStreamOperator<Payload<Object>>> tuple2 = CacheService.loadBootCacheFromDB(env, parameterTool);
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
                .union(tuple2.f0)
                .broadcast(VIN_ALERT_MAP_STATE);


        BroadcastStream<Payload<Object>> alertUrgencyLevelRefSchemaBroadcastStream = alertCdcStream
                .map(alertCdc -> {
                    logger.info("CDC payload after flatten :: {}",alertCdc);
                    return alertCdc;
                })
                .returns(AlertCdc.class)
                .map(new RichPostgresMapImpl(parameterTool) )
                .union(tuple2.f1)
                .broadcast(THRESHOLD_CONFIG_DESCRIPTOR);

        return Tuple2.of(vehicleAlertRefSchemaBroadcastStream, alertUrgencyLevelRefSchemaBroadcastStream);
    }


}
