package net.atos.daf.ct2.service.logistic;


import net.atos.daf.ct2.cache.service.CacheService;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.process.config.AlertConfig;
import org.apache.flink.api.common.state.BroadcastState;
import org.apache.flink.api.common.state.MapState;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.state.ReadOnlyBroadcastState;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.streaming.api.functions.co.KeyedBroadcastProcessFunction;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Map;
import java.util.Set;
import java.util.stream.Collectors;

import static net.atos.daf.ct2.process.functions.LogisticAlertFunction.excessiveDistanceDone;
import static net.atos.daf.ct2.process.functions.LogisticAlertFunction.excessiveGlobalMileage;
import static net.atos.daf.ct2.props.AlertConfigProp.*;

public class ProcessTripBasedService extends KeyedBroadcastProcessFunction<Object, Status, Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>>, Status> implements Serializable {
    private static final Logger logger = LoggerFactory.getLogger(ProcessTripBasedService.class);
    private static final long serialVersionUID = 1L;

    private Map<Object, Object> configMap = new HashMap() {{
        put("functions", Arrays.asList(
                excessiveGlobalMileage,
                excessiveDistanceDone
        ));
    }};


    @Override
    public void processElement(Status status, ReadOnlyContext readOnlyContext, Collector<Status> collector) throws Exception {

        logger.info("Alert process for status message : {}", status);
        ReadOnlyBroadcastState<String, Payload> broadcastState = readOnlyContext.getBroadcastState(vinAlertMapStateDescriptor);
        if (broadcastState.contains(status.getVin())) {
            Payload payload = broadcastState.get(status.getVin());
            Set<AlertUrgencyLevelRefSchema> vinAlertList = (Set<AlertUrgencyLevelRefSchema>) payload.getData().get();

            Set<AlertUrgencyLevelRefSchema> refSchemas = vinAlertList.stream()
                    .filter(schema -> schema.getAlertCategory().equalsIgnoreCase("L") && schema.getAlertType().equalsIgnoreCase("G"))
                    .collect(Collectors.toSet());
            Set<AlertUrgencyLevelRefSchema> distanceDoneSchemas = vinAlertList.stream()
                    .filter(schema -> schema.getAlertCategory().equalsIgnoreCase("L") && schema.getAlertType().equalsIgnoreCase("D"))
                    .collect(Collectors.toSet());

            Map<String, Object> functionThresh = new HashMap<>();
            functionThresh.put("excessiveGlobalMileage", refSchemas);
            functionThresh.put("excessiveDistanceDone", distanceDoneSchemas);

            AlertConfig
                    .buildMessage(status, configMap, functionThresh)
                    .process()
                    .getAlert()
                    .ifPresent(
                            alerts -> {
                                alerts.stream()
                                        .forEach(alert -> readOnlyContext.output(OUTPUT_TAG, alert));
                            }
                    );
            logger.info("Alert process for  {} check those alerts {}", status, refSchemas);


        } else {
            logger.info("Alert not subscribed to vehicle: {}", status);
        }
        collector.collect(status);
    }

    @Override
    public void processBroadcastElement(Payload<Tuple2<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema>> tuple2Payload, Context context, Collector<Status> collector) throws Exception {
        CacheService.updateVinAlertMappingCache(tuple2Payload,context);
    }
}
