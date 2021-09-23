package net.atos.daf.ct2.service.realtime;

import static net.atos.daf.ct2.props.AlertConfigProp.VIN_ALERT_MAP_STATE;

import java.io.Serializable;
import java.util.Set;

import org.apache.flink.api.common.state.ReadOnlyBroadcastState;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.streaming.api.functions.co.KeyedBroadcastProcessFunction;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.cache.service.CacheService;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;

import net.atos.daf.ct2.pojo.standard.Monitor;

public class MonitorKeyBasedSubscription extends KeyedBroadcastProcessFunction<String, Monitor, VehicleAlertRefSchema, Tuple2<Monitor, Payload<Set<Long>>>> implements Serializable {
    private static final Logger logger = LoggerFactory.getLogger(MonitorKeyBasedSubscription.class);
    private static final long serialVersionUID = 1L;

    @Override
    public void processElement(Monitor monitor, KeyedBroadcastProcessFunction<String, Monitor, VehicleAlertRefSchema, Tuple2<Monitor, Payload<Set<Long>>>>.ReadOnlyContext readOnlyContext, Collector<Tuple2<Monitor, Payload<Set<Long>>>> collector) throws Exception {
        logger.info("Process monitor message for vim map check:: {}", monitor);
        ReadOnlyBroadcastState<String, Payload> vinMapState = readOnlyContext.getBroadcastState(VIN_ALERT_MAP_STATE);
        if (vinMapState.contains(monitor.getVin())) {
            logger.info("Vin subscribe for alert status:: {}", monitor);
            collector.collect(Tuple2.of(monitor, vinMapState.get(monitor.getVin())));
        }
    }

    @Override
    public void processBroadcastElement(VehicleAlertRefSchema vehicleAlertRefSchema, KeyedBroadcastProcessFunction<String, Monitor, VehicleAlertRefSchema, Tuple2<Monitor, Payload<Set<Long>>>>.Context context, Collector<Tuple2<Monitor, Payload<Set<Long>>>> collector) throws Exception {
        /**
         * Update vin mapping cache
         */
        CacheService.updateVinMappingCache(vehicleAlertRefSchema, context);
    }
}

