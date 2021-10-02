package net.atos.daf.ct2.service.realtime;

import net.atos.daf.ct2.cache.service.CacheService;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import net.atos.daf.ct2.pojo.standard.Status;
import org.apache.flink.api.common.state.ReadOnlyBroadcastState;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.streaming.api.functions.co.KeyedBroadcastProcessFunction;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.util.Set;

import static net.atos.daf.ct2.props.AlertConfigProp.VIN_ALERT_MAP_STATE;

public class StatusKeyBasedSubscription extends KeyedBroadcastProcessFunction<Object, Status, VehicleAlertRefSchema, Tuple2<Status, Payload<Set<Long>>>> {

    private static final Logger logger = LoggerFactory.getLogger(StatusKeyBasedSubscription.class);
    private static final long serialVersionUID = 1L;

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

}
