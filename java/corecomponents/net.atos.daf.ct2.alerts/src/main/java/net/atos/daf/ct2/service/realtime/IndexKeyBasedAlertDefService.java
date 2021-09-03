package net.atos.daf.ct2.service.realtime;

import net.atos.daf.ct2.cache.service.CacheService;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.process.config.AlertConfig;
import org.apache.flink.api.common.state.ReadOnlyBroadcastState;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.streaming.api.functions.co.KeyedBroadcastProcessFunction;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;

import static net.atos.daf.ct2.props.AlertConfigProp.OUTPUT_TAG;
import static net.atos.daf.ct2.props.AlertConfigProp.THRESHOLD_CONFIG_DESCRIPTOR;

public class IndexKeyBasedAlertDefService extends KeyedBroadcastProcessFunction<Object, Tuple2<Index, Payload<Set<Long>>>, Payload<Object>, Index> implements Serializable {
    private static final Logger logger = LoggerFactory.getLogger(IndexKeyBasedAlertDefService.class);
    private static final long serialVersionUID = 1L;
    private Map<Object, Object> configMap;

    public IndexKeyBasedAlertDefService(Map<Object, Object> configMap){
        this.configMap=configMap;
    }

    @Override
    public void processElement(Tuple2<Index, Payload<Set<Long>>> indexTup2, KeyedBroadcastProcessFunction<Object, Tuple2<Index, Payload<Set<Long>>>, Payload<Object>, Index>.ReadOnlyContext readOnlyContext, Collector<Index> collector) throws Exception {
        ReadOnlyBroadcastState<Long, Payload> broadcastState = readOnlyContext.getBroadcastState(THRESHOLD_CONFIG_DESCRIPTOR);
        logger.info("Fetch alert definition from cache for {}", indexTup2);
        Index f0 = indexTup2.f0;
        Payload<Set<Long>> f1 = indexTup2.f1;
        Set<Long> alertIds = f1.getData().get();
        Map<String, Object> functionThresh = new HashMap<>();
        List<AlertUrgencyLevelRefSchema> hoursOfServiceAlertDef = new ArrayList<>();
        List<AlertUrgencyLevelRefSchema> excessiveAverageSpeedAlertDef = new ArrayList<>();
        List<AlertUrgencyLevelRefSchema> excessiveUnderUtilizationInHoursAlertDef = new ArrayList<>();
        List<AlertUrgencyLevelRefSchema> excessiveIdlingAlertDef = new ArrayList<>();
        for (Long alertId : alertIds) {
            if (broadcastState.contains(alertId)) {
                List<AlertUrgencyLevelRefSchema> thresholdSet = (List<AlertUrgencyLevelRefSchema>) broadcastState.get(alertId).getData().get();
                for (AlertUrgencyLevelRefSchema schema : thresholdSet) {
                    if (schema.getAlertCategory().equalsIgnoreCase("L") && schema.getAlertType().equalsIgnoreCase("S")) {
                        hoursOfServiceAlertDef.add(schema);
                    }
                    if (schema.getAlertCategory().equalsIgnoreCase("F") && schema.getAlertType().equalsIgnoreCase("A")) {
                    	excessiveAverageSpeedAlertDef.add(schema);
                    }
                    if (schema.getAlertCategory().equalsIgnoreCase("L") && schema.getAlertType().equalsIgnoreCase("H")) {
                        excessiveUnderUtilizationInHoursAlertDef.add(schema);
                    }
                    
                    if (schema.getAlertCategory().equalsIgnoreCase("F") && schema.getAlertType().equalsIgnoreCase("I")) {
                		excessiveIdlingAlertDef.add(schema);
                	}
                }
            }
        }
        logger.info("Alert definition from cache for vin :{} alertDef {}", f0.getVin(), hoursOfServiceAlertDef);
        functionThresh.put("hoursOfService", hoursOfServiceAlertDef);
        functionThresh.put("excessiveAverageSpeed", excessiveAverageSpeedAlertDef);
        functionThresh.put("excessiveUnderUtilizationInHours", excessiveUnderUtilizationInHoursAlertDef);
        functionThresh.put("excessiveIdling", excessiveIdlingAlertDef);
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
}
