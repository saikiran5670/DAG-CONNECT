package net.atos.daf.ct2.service.realtime;

import net.atos.daf.ct2.cache.service.CacheService;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.process.config.AlertConfig;
import org.apache.flink.api.common.state.ReadOnlyBroadcastState;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.streaming.api.functions.co.KeyedBroadcastProcessFunction;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.*;

import static net.atos.daf.ct2.props.AlertConfigProp.OUTPUT_TAG;
import static net.atos.daf.ct2.props.AlertConfigProp.THRESHOLD_CONFIG_DESCRIPTOR;

public class StatusKeyBasedAlertDefService
        extends KeyedBroadcastProcessFunction<Object, Tuple2<Status, Payload<Set<Long>>>, Payload<Object>, Status>
        implements Serializable {

    private static final Logger logger = LoggerFactory.getLogger(StatusKeyBasedAlertDefService.class);
    private static final long serialVersionUID = 1L;
    private Map<Object, Object> configMap;

    public StatusKeyBasedAlertDefService(Map<Object, Object> configMap) {
        this.configMap = configMap;
    }

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
}

