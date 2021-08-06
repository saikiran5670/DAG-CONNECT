package net.atos.daf.ct2.cache.service;

import net.atos.daf.ct2.cache.kafka.KafkaCdcStream;
import net.atos.daf.ct2.cache.kafka.impl.KafkaCdcImpl;
import net.atos.daf.ct2.cache.postgres.TableStream;
import net.atos.daf.ct2.cache.postgres.impl.JdbcFormatTableStream;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import org.apache.flink.api.common.state.BroadcastState;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.DataStreamSource;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.functions.co.KeyedBroadcastProcessFunction;
import org.apache.flink.types.Row;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.HashSet;
import java.util.Optional;
import java.util.Set;

import static net.atos.daf.ct2.props.AlertConfigProp.*;

public class CacheService implements Serializable {
    private static final Logger logger = LoggerFactory.getLogger(CacheService.class);
    private static final long serialVersionUID = 1L;

    public static void updateVinAlertMappingCache(Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>> tuple2Payload,KeyedBroadcastProcessFunction.Context context) throws Exception {
        Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema> tuple2 = tuple2Payload.getData().get();
        BroadcastState<String, Payload> broadcastState = context.getBroadcastState(vinAlertMapStateDescriptor);
        VehicleAlertRefSchema f0 = tuple2.f0;
        AlertUrgencyLevelRefSchema f1 = tuple2.f1;
        Set<AlertUrgencyLevelRefSchema> vinAlertList = new HashSet<>();
        logger.info("Cache updating for vehicle ::{}, threshold : {}", f0, f1);
        if (f0.getOp().equalsIgnoreCase("I")) {
            if (broadcastState.contains(f0.getVin())) {
                Payload listPayload = broadcastState.get(f0.getVin());
                vinAlertList = (Set<AlertUrgencyLevelRefSchema>) listPayload.getData().get();
                vinAlertList.add(f1);
                broadcastState.put(f0.getVin(), Payload.builder().data(Optional.of(vinAlertList)).build());
                logger.info("New alert added for vin : {}, alert definition: {}", f0, f1);
            } else {
                logger.info("New vin added: {}, alert definition: {}", f0, f1);
                vinAlertList.add(f1);
                broadcastState.put(f0.getVin(), Payload.builder().data(Optional.of(vinAlertList)).build());
            }
        }
        if (f0.getOp().equalsIgnoreCase("D")) {
            if (broadcastState.contains(f0.getVin())) {
                Payload listPayload = broadcastState.get(f0.getVin());
                vinAlertList = (Set<AlertUrgencyLevelRefSchema>) listPayload.getData().get();
                vinAlertList.remove(f1);
                broadcastState.put(f0.getVin(), Payload.builder().data(Optional.of(vinAlertList)).build());
                logger.info("Alert removed for vin : {}, alert definition: {}", f0, f1);
            }
        }
    }


    private static DataStream<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>> bootCache(StreamExecutionEnvironment env, ParameterTool propertiesParamTool){
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

        return tableStream.joinTable(thresholdStream, vinMappingStream);

    }


    public static BroadcastStream<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>> broadcastCache(StreamExecutionEnvironment env, ParameterTool propertiesParamTool){
        KafkaCdcStream kafkaCdcStream = new KafkaCdcImpl(env, propertiesParamTool);
        SingleOutputStreamOperator<Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>> processCDCFlatten = kafkaCdcStream.cdcStream();
        return processCDCFlatten.union(bootCache(env, propertiesParamTool))
                .broadcast(vinAlertMapStateDescriptor);
    }
}
