package net.atos.daf.ct2.service.realtime;

import static net.atos.daf.ct2.props.AlertConfigProp.OUTPUT_TAG;
import static net.atos.daf.ct2.props.AlertConfigProp.THRESHOLD_CONFIG_DESCRIPTOR;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;

import org.apache.flink.api.common.state.ReadOnlyBroadcastState;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.streaming.api.functions.co.KeyedBroadcastProcessFunction;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.cache.service.CacheService;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;

import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.process.config.AlertConfig;

public class MonitorKeyBasedAlertDefService
		extends KeyedBroadcastProcessFunction<Object, Tuple2<Monitor, Payload<Set<Long>>>, Payload<Object>, Monitor>
		implements Serializable {
	private static final Logger logger = LoggerFactory.getLogger(MonitorKeyBasedAlertDefService.class);
	private static final long serialVersionUID = 1L;
	private Map<Object, Object> configMap;

	public MonitorKeyBasedAlertDefService(Map<Object, Object> configMap) {
		this.configMap = configMap;
	}

	@Override
	public void processElement(Tuple2<Monitor, Payload<Set<Long>>> monitorTup2,
			KeyedBroadcastProcessFunction<Object, Tuple2<Monitor, Payload<Set<Long>>>, Payload<Object>, Monitor>.ReadOnlyContext readOnlyContext,
			Collector<Monitor> collector) throws Exception {
		ReadOnlyBroadcastState<Long, Payload> broadcastState = readOnlyContext
				.getBroadcastState(THRESHOLD_CONFIG_DESCRIPTOR);
		logger.info("Fetch alert definition from cache for {}", monitorTup2);
		Monitor f0 = monitorTup2.f0;
		Payload<Set<Long>> f1 = monitorTup2.f1;
		Set<Long> alertIds = f1.getData().get();
		Map<String, Object> functionThresh = new HashMap<>();
		List<AlertUrgencyLevelRefSchema> repairMaintenanceFunAlertDef = new ArrayList<>();

		for (Long alertId : alertIds) {
			if (broadcastState.contains(alertId)) {
				List<AlertUrgencyLevelRefSchema> thresholdSet = (List<AlertUrgencyLevelRefSchema>) broadcastState
						.get(alertId).getData().get();
				for (AlertUrgencyLevelRefSchema schema : thresholdSet) {

					if (schema.getAlertCategory().equalsIgnoreCase("R")
							&& schema.getAlertType().equalsIgnoreCase("O")) {
						repairMaintenanceFunAlertDef.add(schema);
					}

					if (schema.getAlertCategory().equalsIgnoreCase("R")
							&& schema.getAlertType().equalsIgnoreCase("E")) {
						repairMaintenanceFunAlertDef.add(schema);
					}

				}
			}
		}

		functionThresh.put("repairMaintenanceFun", repairMaintenanceFunAlertDef);
		//
		AlertConfig.buildMessage(f0, configMap, functionThresh).process().getAlert().ifPresent(alerts -> {
			alerts.stream().forEach(alert -> readOnlyContext.output(OUTPUT_TAG, alert));
		});
		logger.info("Alert process for  {} check those alerts {}", f0, functionThresh);
		collector.collect(f0);
	}

	@Override
	public void processBroadcastElement(Payload<Object> payload,
			KeyedBroadcastProcessFunction<Object, Tuple2<Monitor, Payload<Set<Long>>>, Payload<Object>, Monitor>.Context context,
			Collector<Monitor> collector) throws Exception {
		/**
		 * Update threshold alert definition
		 */
		CacheService.updateAlertDefinationCache(payload, context);
	}
}
