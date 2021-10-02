package net.atos.daf.ct2.app;

import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.hoursOfServiceFun;
import static net.atos.daf.ct2.process.functions.MonitorBasedAlertFunction.repairMaintenanceFun;
import static net.atos.daf.ct2.props.AlertConfigProp.KAFKA_EGRESS_MONITERING_MSG_TOPIC;

import java.util.Arrays;
import java.util.HashMap;
import java.util.Map;
import java.util.UUID;

import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.KeyedStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.cache.kafka.KafkaCdcStreamV2;
import net.atos.daf.ct2.cache.kafka.impl.KafkaCdcImplV2;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.props.AlertConfigProp;
import net.atos.daf.ct2.service.kafka.KafkaConnectionService;
import net.atos.daf.ct2.service.realtime.MonitorMessageAlertService;
import net.atos.daf.ct2.service.realtime.RepairMaintenance;
@Deprecated
public class MoniteringBasedAlertProcessing {
	private static final Logger logger = LoggerFactory.getLogger(IndexBasedAlertProcessing.class);
	private static final long serialVersionUID = 1L;

	public static void main(String[] args) throws Exception {
		final StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();
		ParameterTool parameterTool = ParameterTool.fromArgs(args);
		logger.info("AlertProcessing started with properties :: {}", parameterTool.getProperties());
		/**
		 * Creating param tool from given property
		 */
		ParameterTool propertiesParamTool = ParameterTool.fromPropertiesFile(parameterTool.get("prop"));

		logger.info("PropertiesParamTool :: {}", parameterTool.getProperties());

		/**
		 * RealTime functions defined
		 */
		
		Map<Object, Object> repairMaintenanceFunConfigMap = new HashMap() {
			{
				put("functions", Arrays.asList(repairMaintenanceFun));
			}
		};
		
		/**
		 * Booting cache
		 */
		
		  KafkaCdcStreamV2 kafkaCdcStreamV2 = new KafkaCdcImplV2(env,
		  propertiesParamTool); Tuple2<BroadcastStream<VehicleAlertRefSchema>,
		  BroadcastStream<Payload<Object>>> bootCache = kafkaCdcStreamV2 .bootCache();
		  
		  AlertConfigProp.vehicleAlertRefSchemaBroadcastStream = bootCache.f0;
		  AlertConfigProp.alertUrgencyLevelRefSchemaBroadcastStream = bootCache.f1;
		 
		SingleOutputStreamOperator<Monitor> MonitorStringStream = 
				
						KafkaConnectionService
				.connectMoniteringObjectTopic(propertiesParamTool.get(KAFKA_EGRESS_MONITERING_MSG_TOPIC),
						propertiesParamTool, env)
				.map(moniterKafkaRecord -> moniterKafkaRecord.getValue()).returns(
						Monitor.class)
				.filter(moniter -> moniter.getVid() != null && moniter.getMessageType() == 10
						&& (46 == moniter.getVEvtID() || 44 == moniter.getVEvtID() || 45 == moniter.getVEvtID()
								|| 63 == moniter.getVEvtID()))
				.returns(Monitor.class)
				.map(m -> {
					m.setJobName(UUID.randomUUID().toString());
					logger.info("Monitor msg recived :: {}, msg UUD ::{}",m,m.getJobName());
					return m;
					})
				.returns(Monitor.class);

	
		  KeyedStream<Monitor, String> monitorStringKeyedStream =
		  MonitorStringStream.process(new RepairMaintenance(propertiesParamTool))
		  .keyBy(moniter -> moniter.getVin() != null ? moniter.getVin() : moniter.getVid());
		  
		  MonitorMessageAlertService.processMonitorKeyStream(monitorStringKeyedStream,
		  env, propertiesParamTool, repairMaintenanceFunConfigMap);
		env.execute(MoniteringBasedAlertProcessing.class.getSimpleName());

	}

}
