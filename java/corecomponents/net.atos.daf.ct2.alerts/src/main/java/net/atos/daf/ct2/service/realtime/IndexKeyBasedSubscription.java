package net.atos.daf.ct2.service.realtime;

import net.atos.daf.ct2.cache.service.CacheService;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import net.atos.daf.ct2.pojo.standard.Index;
import org.apache.flink.api.common.state.ReadOnlyBroadcastState;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.streaming.api.functions.co.KeyedBroadcastProcessFunction;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.Objects;
import java.util.Set;

import static net.atos.daf.ct2.props.AlertConfigProp.INCOMING_MESSAGE_UUID;
import static net.atos.daf.ct2.props.AlertConfigProp.VIN_ALERT_MAP_STATE;

public class IndexKeyBasedSubscription extends KeyedBroadcastProcessFunction<String, Index, VehicleAlertRefSchema, Tuple2<Index, Payload<Set<Long>>>> implements Serializable {
    private static final Logger logger = LoggerFactory.getLogger(IndexKeyBasedSubscription.class);
    private static final long serialVersionUID = 1L;

    @Override
    public void processElement(Index index, KeyedBroadcastProcessFunction<String, Index, VehicleAlertRefSchema, Tuple2<Index, Payload<Set<Long>>>>.ReadOnlyContext readOnlyContext, Collector<Tuple2<Index, Payload<Set<Long>>>> collector) throws Exception {
        logger.info("Process index message for vim map check:: {} {}", index, String.format(INCOMING_MESSAGE_UUID,index.getJobName()));
        ReadOnlyBroadcastState<String, Payload> vinMapState = readOnlyContext.getBroadcastState(VIN_ALERT_MAP_STATE);
        if (vinMapState.contains(index.getVin())) {
            logger.info("Vin subscribe for alert status:: {} : {}", index, String.format(INCOMING_MESSAGE_UUID,index.getJobName()));
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
}
