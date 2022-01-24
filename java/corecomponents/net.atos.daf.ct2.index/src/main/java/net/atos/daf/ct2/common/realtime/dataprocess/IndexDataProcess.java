package net.atos.daf.ct2.common.realtime.dataprocess;

import java.util.HashMap;
import java.util.Map;
import java.util.Properties;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.KeyedStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.AuditETLJobClient;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.common.ct2.util.DAFConstants;
import net.atos.daf.ct2.common.processing.CurrentTripStatisticsProcessing;
import net.atos.daf.ct2.common.processing.LivefleetPositionProcessing;
import net.atos.daf.ct2.common.realtime.hbase.IndexDataHbaseSink;
import net.atos.daf.ct2.common.realtime.postgresql.LiveFleetCurrentTripPostgreSink;
import net.atos.daf.ct2.common.realtime.postgresql.LiveFleetTripTracingPostgreSink;
//import net.atos.daf.common.AuditETLJobClient;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.common.util.FlinkKafkaIndexDataConsumer;
import net.atos.daf.ct2.common.util.FlinkUtil;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.postgre.bo.LiveFleetPojo;

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

			final StreamExecutionEnvironment env = envParams.get("flink.streaming.evn").equalsIgnoreCase("default") ?
					StreamExecutionEnvironment.getExecutionEnvironment() :
					FlinkUtil.createStreamExecutionEnvironment(envParams,envParams.get(DafConstants.INDEX_JOB));

			log.debug("env :: " + env);

			FlinkKafkaIndexDataConsumer flinkKafkaConsumer = new FlinkKafkaIndexDataConsumer();

			env.getConfig().setGlobalJobParameters(envParams);
			

			DataStream<KafkaRecord<Index>> consumerStream = flinkKafkaConsumer.connectToKafkaTopic(envParams, env);
			 
			if("true".equals(envParams.get(DafConstants.STORE_HISTORICAL_DATA))){
			
			 consumerStream.addSink(new IndexDataHbaseSink()); // Writing into HBase Table
			}						

			SingleOutputStreamOperator<Index> SingleConsumerStream = consumerStream.map(rec -> rec.getValue()).returns(Index.class);
			
			
			CurrentTripStatisticsProcessing currentTripProcessing= new CurrentTripStatisticsProcessing();
			SingleOutputStreamOperator<Index> currentTripDataProcessingStream = currentTripProcessing.currentTripDataProcessing(SingleConsumerStream,Long.parseLong(envParams.get(DafConstants.INDEX_COUNT_WINDOW)));
			
			
			 
			LivefleetPositionProcessing positionProcessing= new LivefleetPositionProcessing();
			
			SingleOutputStreamOperator<Index> liveFleetPositionStream = positionProcessing.liveFleetPosition(SingleConsumerStream, Long.parseLong(envParams.get(DafConstants.INDEX_COUNT_WINDOW)));
			
			//KeyedStream<KafkaRecord<Index>, String> consumerKeyedStream = consumerStream.keyBy(kafkaRecord -> kafkaRecord.getValue().getVin()!=null ? kafkaRecord.getValue().getVin() : kafkaRecord.getValue().getVid());
			//consumerKeyedStream.addSink(new LiveFleetCurrentTripPostgreSink()); 
			//consumerStream.addSink(new LiveFleetTripTracingPostgreSink());
			
			liveFleetPositionStream.addSink(new LiveFleetTripTracingPostgreSink());
			// Writing into current trip table
			currentTripDataProcessingStream.addSink(new LiveFleetCurrentTripPostgreSink());

			log.debug("after addsink");

			try {
				
				  auditing = new AuditETLJobClient(envParams.get(DafConstants.GRPC_SERVER),
				  Integer.valueOf(envParams.get(DafConstants.GRPC_PORT))); auditMap =
				  createAuditMap(DafConstants.AUDIT_EVENT_STATUS_START,
				  "Realtime Data Monitoring processing Job Started"); //
				  
				  auditing.auditTrialGrpcCall(auditMap); auditing.closeChannel();
				 
				 
			} catch (Exception e) {

				log.error("Issue while auditing :: " + e.getMessage());
			}

			//env.execute(" Realtime_IndexDataProcess");
			env.execute(envParams.get(DafConstants.INDEX_PROCESS));

		} catch (Exception e) {
			log.error("Error in Index Data Process {} {}" , e.getMessage(),e);

			try {
				
				  auditMap = createAuditMap(DafConstants.AUDIT_EVENT_STATUS_FAIL,
				  "Realtime index data processing Job Failed, reason :: " + e.getMessage());
				  auditing = new AuditETLJobClient(envParams.get(DafConstants.GRPC_SERVER),
				  Integer.valueOf(envParams.get(DafConstants.GRPC_PORT)));
				  auditing.auditTrialGrpcCall(auditMap); auditing.closeChannel();
				 
			} catch (Exception ex) {
				log.error("Issue while auditing :: " + ex.getMessage());
			}

		}
	}

	private static Map<String, String> createAuditMap(String jobStatus, String message) {
		Map<String, String> auditMap = new HashMap<>();
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
