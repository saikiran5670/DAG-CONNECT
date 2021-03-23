package net.atos.daf.ct2.common.realtime.dataprocess;

import java.util.HashMap;
import java.util.Map;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.AuditETLJobClient;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.common.realtime.hbase.MonitorDataHbaseSink;
import net.atos.daf.ct2.common.realtime.postgresql.LiveFleetPositionPostgreSink;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.common.util.FlinkKafkaMonitorDataConsumer;
import net.atos.daf.ct2.common.util.FlinkUtil;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Monitor;

public class MonitorDataProcess {
	public static void main(String[] args) throws Exception {

		Logger log = LoggerFactory.getLogger(MonitorDataProcess.class);

		Map<String, String> auditMap = null;
		AuditETLJobClient auditing = null;

		ParameterTool envParams = null;

		try {

			ParameterTool params = ParameterTool.fromArgs(args);

			if (params.get("input") != null)
				envParams = ParameterTool.fromPropertiesFile(params.get("input"));

			
			final StreamExecutionEnvironment env = FlinkUtil.createStreamExecutionEnvironment(envParams,
					envParams.get(DafConstants.MONITOR_JOB));

			log.info("env :: " + env);
			FlinkKafkaMonitorDataConsumer flinkKafkaConsumer = new FlinkKafkaMonitorDataConsumer();
			env.getConfig().setGlobalJobParameters(envParams);

			DataStream<KafkaRecord<Monitor>> consumerStream = flinkKafkaConsumer.connectToKafkaTopic(envParams, env);
			consumerStream.print();

			consumerStream.addSink(new MonitorDataHbaseSink()); // Writing into HBase Table

			consumerStream.addSink(new LiveFleetPositionPostgreSink()); // Writing into PostgreSQL Table

			log.info("after addsink");
			try {
				//System.out.println("inside try of monitoring auditing");
				auditing = new AuditETLJobClient(envParams.get(DafConstants.GRPC_SERVER),
						Integer.valueOf(envParams.get(DafConstants.GRPC_PORT)));
				//System.out.println("before calling audit map");
				auditMap = createAuditMap(DafConstants.AUDIT_EVENT_STATUS_START,
						"Realtime Data Monitoring processing Job Started");
				//System.out.println("before calling auditTRail");
				auditing.auditTrialGrpcCall(auditMap);
				auditing.closeChannel();
			} catch (Exception e) {
				log.error("Issue while auditing :: " + e.getMessage());
			}

			env.execute(" Realtime_MonitorDataProcess");

		} catch (Exception e) {

			log.error("Error in Message Data Processing - " + e.getMessage());

			try {
				auditMap = createAuditMap(DafConstants.AUDIT_EVENT_STATUS_FAIL,
						"Realtime Data Monitoring processing Job Failed, reason :: " + e.getMessage());

				auditing = new AuditETLJobClient(envParams.get(DafConstants.GRPC_SERVER),
						Integer.valueOf(envParams.get(DafConstants.GRPC_PORT)));
				auditing.auditTrialGrpcCall(auditMap);
				auditing.closeChannel();
			} catch (Exception ex) {
				log.error("Issue while auditing :: " + ex.getMessage());
			}

		}
	}

	private static Map<String, String> createAuditMap(String jobStatus, String message) {
		Map<String, String> auditMap = new HashMap<>();
		//System.out.println("before calling audit mapinside createAudit map meesage="+ message);
		auditMap.put(DafConstants.JOB_EXEC_TIME, String.valueOf(TimeFormatter.getInstance().getCurrentUTCTimeInSec()));
		auditMap.put(DafConstants.AUDIT_PERFORMED_BY, DafConstants.TRIP_JOB_NAME);
		auditMap.put(DafConstants.AUDIT_COMPONENT_NAME, DafConstants.TRIP_JOB_NAME);
		auditMap.put(DafConstants.AUDIT_SERVICE_NAME, DafConstants.AUDIT_SERVICE);
		auditMap.put(DafConstants.AUDIT_EVENT_TYPE, DafConstants.AUDIT_CREATE_EVENT_TYPE);// check
		auditMap.put(DafConstants.AUDIT_EVENT_TIME, String.valueOf(TimeFormatter.getInstance().getCurrentUTCTimeInSec()));
		auditMap.put(DafConstants.AUDIT_EVENT_STATUS, jobStatus);
		auditMap.put(DafConstants.AUDIT_MESSAGE, message);
		auditMap.put(DafConstants.AUDIT_SOURCE_OBJECT_ID, DafConstants.DEFAULT_OBJECT_ID);
		auditMap.put(DafConstants.AUDIT_TARGET_OBJECT_ID, DafConstants.DEFAULT_OBJECT_ID);
		auditMap.put(DafConstants.AUDIT_UPDATED_DATA, null);

		return auditMap;
	}

}
