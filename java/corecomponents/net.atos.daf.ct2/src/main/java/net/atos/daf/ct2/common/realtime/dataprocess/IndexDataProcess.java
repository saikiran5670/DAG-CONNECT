package net.atos.daf.ct2.common.realtime.dataprocess;

import java.util.HashMap;
import java.util.Map;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.flink.streaming.connectors.kafka.KafkaDeserializationSchema;


import net.atos.daf.ct2.common.realtime.postgresql.LiveFleetDriverActivityPostgreSink;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.common.util.FlinkKafkaIndexDataConsumer;
import net.atos.daf.common.AuditETLJobClient;
import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.common.realtime.hbase.IndexDataHbaseSink;

import net.atos.daf.ct2.common.util.FlinkUtil;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;


@SuppressWarnings("unused")
public class IndexDataProcess {
	public static void main(String[] args) throws Exception {
		
		//private static Logger log = LoggerFactory.getLogger(IndexDataProcess.class); 
		Logger log = LoggerFactory.getLogger(IndexDataProcess.class); 
		
		  Map<String, String> auditMap = null; 
		  AuditETLJobClient auditing = null;
		 
		ParameterTool envParams = null;
		
		try {
			

			ParameterTool params = ParameterTool.fromArgs(args);
			
			if (params.get("input") != null)
				envParams = ParameterTool.fromPropertiesFile(params.get("input"));
			
//			System.out.println(" In Index Main EVENT_HUB_BOOTSTRAP -- "+envParams.get(DafConstants.EVENT_HUB_BOOTSTRAP));
//			System.out.println("  In Index Main  EVENT_HUB_CONFIG -- "+envParams.get(DafConstants.EVENT_HUB_CONFIG));
//			System.out.println(" In Index Main  Topic -- "+envParams.get(DafConstants.INDEX_TOPIC));
//			

			String outputFolder = params.get("output");

			final StreamExecutionEnvironment env = FlinkUtil.createStreamExecutionEnvironment(envParams,"IndexJob");
			System.out.println("env :: "+env);
			FlinkKafkaIndexDataConsumer flinkKafkaConsumer = new FlinkKafkaIndexDataConsumer();

			env.getConfig().setGlobalJobParameters(envParams);
			//final StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();
			DataStream<KafkaRecord<Index>> consumerStream = flinkKafkaConsumer.connectToKafkaTopic(envParams, env);
			//consumerStream.print();
			//DataStream<String>d1s=consumerStream.map(a ->{System.out.println("Record With OLD POJOS ---  "+a.getValue());return a.getValue().toString();});
			//d1s.print();
			/*
			 * DataStream<String> VIDstr = consumerStream3.map(new MapFunction<IndexMessage,
			 * String>() {
			 *//**
				* 
				*//*
					 * private static final long serialVersionUID = -6583095550409443793L;
					 * 
					 * @Override public String map(IndexMessage value) throws Exception {
					 * System.out.println("VID in return  " +value.getVID()); return value.getVID();
					 * } });
					 * 
					 * VIDstr.writeAsText("/home/flink-vm0-user1/Kalyan/testflink.txt",org.apache.
					 * flink.core.fs.FileSystem.WriteMode.OVERWRITE);
					 * 
					 */

			consumerStream.addSink(new IndexDataHbaseSink());	//Writing into HBase Table
			
			consumerStream.addSink(new LiveFleetDriverActivityPostgreSink());	//Writing into PostgreSQL Table
			
			System.out.println("after addsink");
			
			
						
			try { 
				auditing = new AuditETLJobClient(envParams.get(DafConstants.GRPC_SERVER),Integer.valueOf(envParams.get(DafConstants.GRPC_PORT))); 
				//auditing = new AuditETLJobClient("52.236.153.224",80);
				//System.out.println("In Audit 1");
			auditMap = createAuditMap(DafConstants.AUDIT_EVENT_STATUS_START,
					  "Realtime Data Monitoring processing Job Started");
					  auditing.auditTrialGrpcCall(auditMap); auditing.closeChannel(); } catch
					  (Exception e) { // TODO cross check - Need not abort the job on GRPC failure
						  //System.out.println("In Audit CATCH");
					  log.error("Issue while auditing :: " + e.getMessage()); }
					 
			

			env.execute(" Realtime_IndexDataProcess");
			
		} catch (Exception e) {
			
			//System.out.println("In MAIN CATCH");
			
			log.error("Error in Index Data Process" + e.getMessage() );
			
			try { auditMap = createAuditMap(DafConstants.AUDIT_EVENT_STATUS_FAIL,
					  "Realtime Data Monitoring processing Job Failed, reason :: " +
					  e.getMessage());
					  
					  auditing = new AuditETLJobClient(envParams.get(DafConstants.GRPC_SERVER),
					  Integer.valueOf(envParams.get(DafConstants.GRPC_PORT)));
					  auditing.auditTrialGrpcCall(auditMap); auditing.closeChannel(); } catch
					  (Exception ex) { 
						  //System.out.println("In GRPC CATCH");
						  log.error("Issue while auditing :: " + ex.getMessage()); }	
			
			e.printStackTrace();

		}
	}
	
	 private static Map<String, String> createAuditMap(String jobStatus, String
			  message) { Map<String, String> auditMap = new HashMap<>();
			  
			  auditMap.put(DafConstants.JOB_EXEC_TIME,
			  String.valueOf(TimeFormatter.getInstance().getCurrentUTCTimeInSec()));
			  auditMap.put(DafConstants.AUDIT_PERFORMED_BY, DafConstants.TRIP_JOB_NAME);
			  auditMap.put(DafConstants.AUDIT_COMPONENT_NAME, DafConstants.TRIP_JOB_NAME);
			  auditMap.put(DafConstants.AUDIT_SERVICE_NAME, DafConstants.AUDIT_SERVICE);
			  auditMap.put(DafConstants.AUDIT_EVENT_TYPE,
			  DafConstants.AUDIT_CREATE_EVENT_TYPE);// check
			  auditMap.put(DafConstants.AUDIT_EVENT_TIME,
			  String.valueOf(TimeFormatter.getInstance().getCurrentUTCTimeInSec()));
			  auditMap.put(DafConstants.AUDIT_EVENT_STATUS, jobStatus);
			  auditMap.put(DafConstants.AUDIT_MESSAGE, message);
			  auditMap.put(DafConstants.AUDIT_SOURCE_OBJECT_ID,
			  DafConstants.DEFAULT_OBJECT_ID);
			  auditMap.put(DafConstants.AUDIT_TARGET_OBJECT_ID,
			  DafConstants.DEFAULT_OBJECT_ID);
			  auditMap.put(DafConstants.AUDIT_UPDATED_DATA, null);
			  
			  return auditMap; }
	
}
