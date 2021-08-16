package net.atos.daf.ct2.cache.kafka.impl;


import net.atos.daf.ct2.cache.kafka.KafkaCdcStream;
import net.atos.daf.ct2.cache.postgres.impl.RichPostgresMapImpl;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.kafka.AlertCdc;
import net.atos.daf.ct2.models.kafka.CdcPayloadWrapper;
import net.atos.daf.ct2.models.kafka.VinOp;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import net.atos.daf.ct2.util.Utils;
import org.apache.flink.api.common.functions.FlatMapFunction;
import org.apache.flink.api.common.serialization.SimpleStringSchema;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
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
import java.util.Optional;
import java.util.Properties;

import static net.atos.daf.ct2.props.AlertConfigProp.KAFKA_BOOTSTRAP_SERVER;
import static net.atos.daf.ct2.props.AlertConfigProp.KAFKA_DAF_ALERT_CDC_TOPIC;
import static net.atos.daf.ct2.props.AlertConfigProp.KAFKA_GRP_ID;

public class KafkaCdcImpl extends KafkaCdcStream implements Serializable {
    private static final long serialVersionUID = 1L;
    private static final Logger logger = LoggerFactory.getLogger(KafkaCdcImpl.class);

    public KafkaCdcImpl(StreamExecutionEnvironment env, ParameterTool parameterTool){
        super(env, parameterTool);
    }

    @Override
    protected KeyedStream<AlertCdc, String> init(){
        Properties kafkaTopicProp =Utils.getKafkaConnectProperties(parameterTool);

        FlinkKafkaConsumer<String> kafkaAlertCDCMessageConsumer = new FlinkKafkaConsumer<>(parameterTool.get(KAFKA_DAF_ALERT_CDC_TOPIC), new SimpleStringSchema(), kafkaTopicProp);
        KeyedStream<AlertCdc, String> alertCdcStream = env.addSource(kafkaAlertCDCMessageConsumer)
                .map(json -> (CdcPayloadWrapper) Utils.readValueAsObject(json, CdcPayloadWrapper.class))
                .returns(CdcPayloadWrapper.class)
                .map(cdc -> {
                    AlertCdc alertCdc = new AlertCdc();
                    logger.info("CDC payload received :: {}", cdc);
                            try{
                                if (cdc.getNamespace().equalsIgnoreCase("alerts"))
                                    alertCdc= (AlertCdc) Utils.readValueAsObject(cdc.getPayload(), AlertCdc.class);
                            }catch (Exception e){
                                logger.error("error while parsing alert cdc message : {}",e);
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
    protected SingleOutputStreamOperator<List<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>>> processCdcPayload(KeyedStream<AlertCdc, String> alertCdcStream){
        return alertCdcStream
                .map(new RichPostgresMapImpl(parameterTool))
                .returns(Payload.class)
                .flatMap(
                        (FlatMapFunction<Payload, List<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>>>) (payload, collector) -> {
                            logger.info("CDC record convert to tuple ");
                            Tuple2<AlertCdc, AlertUrgencyLevelRefSchema> p1 = (Tuple2<AlertCdc, AlertUrgencyLevelRefSchema>) payload.getData().get();
                            AlertUrgencyLevelRefSchema f1 = p1.f1;
                            AlertCdc alertCdc = p1.f0;
                            List<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>> lst = new ArrayList<>();
                            for (VinOp vin : alertCdc.getVinOps()) {
                                VehicleAlertRefSchema schema = new VehicleAlertRefSchema();
                                schema.setAlertId(f1.getAlertId());
                                schema.setVin(vin.getVin());
                                schema.setOp(vin.getOp());
                                logger.info("CDC record op has been set to {}", schema);
                                lst.add(Payload.<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>builder().data(Optional.of(Tuple2.of(schema, f1))).build());
                            }
                            collector.collect(lst);
                        }

                ).returns(TypeInformation.of(new TypeHint<List<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>>>() {
            @Override
            public TypeInformation<List<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>>> getTypeInfo() {
                return super.getTypeInfo();
            }
        }));

    }


    @Override
    protected SingleOutputStreamOperator<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>> flatternCdcStream(SingleOutputStreamOperator<List<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>>> listPayloadStream){
         return listPayloadStream
                 .process(
                         new ProcessFunction<List<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>>, Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>>() {
                             @Override
                             public void processElement(List<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>> payloads, Context context, Collector<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>> collector) throws Exception {
                                 for (Payload payload : payloads) {
                                     collector.collect(payload);
                                 }
                             }
                         }
                 );
    }

}
