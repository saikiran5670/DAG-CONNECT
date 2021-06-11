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
import net.atos.daf.ct2.common.realtime.hbase.IndexDataHbaseSink;
import net.atos.daf.ct2.common.realtime.postgresql.LiveFleetCurrentTripPostgreSink;
import net.atos.daf.ct2.common.realtime.postgresql.LiveFleetTripTracingPostgreSink;
//import net.atos.daf.common.AuditETLJobClient;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.common.util.FlinkKafkaIndexDataConsumer;
import net.atos.daf.ct2.common.util.FlinkUtil;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;

public class IndexDataProcess {
	public static void main(String[] args) throws Exception {

		/*
		 * This Job will read the index message data from Kafka topic and store
		 * it in HBase and Postgres
		 */

		Logger log = LoggerFactory.getLogger(IndexDataProcess.class);

		Map<String, String> auditMap = null;
		AuditETLJobClient auditing = null;

		ParameterTool envParams = null;

		try {

			log.info("============== Start of IndexDataProcess =============");

			ParameterTool params = ParameterTool.fromArgs(args);

			if (params.get("input") != null)
				envParams = ParameterTool.fromPropertiesFile(params.get("input"));

			final StreamExecutionEnvironment env = FlinkUtil.createStreamExecutionEnvironment(envParams,
					envParams.get(DafConstants.INDEX_JOB));

			log.info("env :: " + env);

			FlinkKafkaIndexDataConsumer flinkKafkaConsumer = new FlinkKafkaIndexDataConsumer();

			env.getConfig().setGlobalJobParameters(envParams);

			DataStream<KafkaRecord<Index>> consumerStream = flinkKafkaConsumer.connectToKafkaTopic(envParams, env);
			consumerStream.print();

			/*
			 * BucketingSink<KafkaRecord<Index>> hdfsSink = new
			 * BucketingSink<>("file:///home/flink-vm0-user1/checkpoint");
			 * 
			 * hdfsSink.setBucketer(new
			 * DateTimeBucketer<>("yyyy-MM-dd--HH-mm"));
			 * 
			 * consumerStream.print();
			 * 
			 * consumerStream.addSink(hdfsSink);
			 */
			consumerStream.addSink(new IndexDataHbaseSink()); // Writing into
																// HBase Table

			// consumerStream.addSink(new LiveFleetDriverActivityPostgreSink());
			// // Writing into Driver Activity PostgreSQL Table

			//consumerStream.addSink(new LiveFleetCurrentTripPostgreSink()); // Writing
																			// into
																			// Current
																			// Trip
																			// PostgreSQL
																			// Table

			consumerStream.addSink(new LiveFleetTripTracingPostgreSink());

			log.info("after addsink");

			try {
				// System.out.println("Inside try of audit");
				auditing = new AuditETLJobClient(envParams.get(DafConstants.GRPC_SERVER),
						Integer.valueOf(envParams.get(DafConstants.GRPC_PORT)));
				auditMap = createAuditMap(DafConstants.AUDIT_EVENT_STATUS_START,
						"Realtime Data Monitoring processing Job Started");
				// System.out.println("before calling auditTrail in catch");
				auditing.auditTrialGrpcCall(auditMap);
				auditing.closeChannel();
			} catch (Exception e) {

				log.error("Issue while auditing :: " + e.getMessage());
			}

			env.execute(" Realtime_IndexDataProcess");

		} catch (Exception e) {

			log.error("Error in Index Data Process" + e.getMessage());
			e.printStackTrace();

			try {
				auditMap = createAuditMap(DafConstants.AUDIT_EVENT_STATUS_FAIL,
						"Realtime index data processing Job Failed, reason :: " + e.getMessage());
				System.out.println("before calling auditTrail in catch");
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
		// System.out.println("inside createAudit Map" + message);
		auditMap.put(DafConstants.JOB_EXEC_TIME, String.valueOf(TimeFormatter.getInstance().getCurrentUTCTimeInSec()));
		auditMap.put(DafConstants.AUDIT_PERFORMED_BY, DafConstants.TRIP_JOB_NAME);
		auditMap.put(DafConstants.AUDIT_COMPONENT_NAME, DafConstants.TRIP_JOB_NAME);
		auditMap.put(DafConstants.AUDIT_SERVICE_NAME, DafConstants.AUDIT_SERVICE);
		auditMap.put(DafConstants.AUDIT_EVENT_TYPE, DafConstants.AUDIT_CREATE_EVENT_TYPE);// check
		auditMap.put(DafConstants.AUDIT_EVENT_TIME,
				String.valueOf(TimeFormatter.getInstance().getCurrentUTCTimeInSec()));
		auditMap.put(DafConstants.AUDIT_EVENT_STATUS, jobStatus);
		auditMap.put(DafConstants.AUDIT_MESSAGE, message);
		auditMap.put(DafConstants.AUDIT_SOURCE_OBJECT_ID, DafConstants.DEFAULT_OBJECT_ID);
		auditMap.put(DafConstants.AUDIT_TARGET_OBJECT_ID, DafConstants.DEFAULT_OBJECT_ID);
		auditMap.put(DafConstants.AUDIT_UPDATED_DATA, null);

		return auditMap;
	}

}
