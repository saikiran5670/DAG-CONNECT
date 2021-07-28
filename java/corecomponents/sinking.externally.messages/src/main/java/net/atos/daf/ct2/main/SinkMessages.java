package net.atos.daf.ct2.main;


import java.io.FileReader;
import java.io.IOException;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Properties;

import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.kafka.clients.consumer.ConsumerConfig;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.common.AuditETLJobClient;
import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.exception.DAFCT2Exception;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.processing.MessageProcessing;
import net.atos.daf.ct2.serde.KafkaMessageDeSerializeSchema;

public class SinkMessages<T> {

  private static final Logger log = LogManager.getLogger(SinkMessages.class);
  public static String FILE_PATH;
  private static AuditETLJobClient auditETLJobClient;
  private StreamExecutionEnvironment streamExecutionEnvironment;

  public static Properties configuration() throws DAFCT2Exception {
    Properties properties = new Properties();
    
    try {
      properties.load(new FileReader(FILE_PATH));
          
      properties.put(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, properties.getProperty(DAFCT2Constant.AUTO_OFFSET_RESET_CONFIG));
      log.info("Configuration Loaded for Connecting Kafka inorder to Perform Mapping.");

    } catch (IOException e) {
      log.error("Unable to Find the File " + FILE_PATH, e);
      throw new DAFCT2Exception("Unable to Find the File " + FILE_PATH, e);
    }

    return properties;
  }

  public static void main(String[] args) {
	Properties properties = null;
    try {
      FILE_PATH = args[0];

      SinkMessages sinkMessages = new SinkMessages();
      properties = configuration();
      auditTrail(properties, "Sink external message job started");
      
    // final StreamExecutionEnvironment env = sinkMessages.flinkConnection(properties);
      sinkMessages.flinkConnection();
      
      List<String> listTopics = sinkMessages.topicList(properties);
      log.info(" listTopics :: "+listTopics);
      System.out.println(" listTopics :: "+listTopics);
      if(listTopics != null)
    	  sinkMessages.processing(properties, listTopics);
      
      sinkMessages.startExecution();

    } catch (Exception e) {
      log.error("Exception: ", e);
      auditTrail(properties, "Sink external message job failed :: "+e.getMessage());
      e.printStackTrace();

    }
  }

  public void flinkConnection() throws TechnicalException {

    this.streamExecutionEnvironment = StreamExecutionEnvironment.getExecutionEnvironment();
	  
	//  return FlinkUtil.createStreamExecutionEnvironment(properties);
   // this.streamExecutionEnvironment.setStreamTimeCharacteristic(TimeCharacteristic.EventTime);
    /*this.streamExecutionEnvironment.enableCheckpointing(5000);
    this.streamExecutionEnvironment.getCheckpointConfig().setMaxConcurrentCheckpoints(1);
    this.streamExecutionEnvironment
        .getCheckpointConfig()
        .enableExternalizedCheckpoints(
            CheckpointConfig.ExternalizedCheckpointCleanup.RETAIN_ON_CANCELLATION);*/
    // this.streamExecutionEnvironment.setStateBackend(new FsStateBackend(""));

  }

  public static void auditTrail(Properties properties, String msg) {
	  try{
		  auditETLJobClient =
		            new AuditETLJobClient(
		                    properties.getProperty(DAFCT2Constant.GRPC_SERVER),
		                    Integer.valueOf(properties.getProperty(DAFCT2Constant.GRPC_PORT)));
		    Map<String, String> auditMap = new HashMap<String, String>();

		    auditMap.put(DAFCT2Constant.JOB_EXEC_TIME, String.valueOf(TimeFormatter.getInstance().getCurrentUTCTime()));
		    auditMap.put(DAFCT2Constant.AUDIT_PERFORMED_BY, DAFCT2Constant.TRIP_JOB_NAME);
		    auditMap.put(DAFCT2Constant.AUDIT_COMPONENT_NAME, DAFCT2Constant.TRIP_JOB_NAME);
		    auditMap.put(DAFCT2Constant.AUDIT_SERVICE_NAME, DAFCT2Constant.AUDIT_SERVICE);
		    auditMap.put(DAFCT2Constant.AUDIT_EVENT_TYPE, DAFCT2Constant.AUDIT_CREATE_EVENT_TYPE);
		    auditMap.put(
		        DAFCT2Constant.AUDIT_EVENT_TIME, String.valueOf(TimeFormatter.getInstance().getCurrentUTCTime()));
		    auditMap.put(DAFCT2Constant.AUDIT_EVENT_STATUS, DAFCT2Constant.AUDIT_EVENT_STATUS_START);
		    auditMap.put(DAFCT2Constant.AUDIT_MESSAGE, msg);
		    auditMap.put(DAFCT2Constant.AUDIT_SOURCE_OBJECT_ID, DAFCT2Constant.DEFAULT_OBJECT_ID);
		    auditMap.put(DAFCT2Constant.AUDIT_TARGET_OBJECT_ID, DAFCT2Constant.DEFAULT_OBJECT_ID);

		    auditETLJobClient.auditTrialGrpcCall(auditMap);
		    auditETLJobClient.closeChannel();

		    log.info("Audit Trial Started");
	  }catch(Exception e){
		  log.error("Issue while auditing sinking external message Job :: ");
	  }/*finally{
	      auditETLJobClient.closeChannel();
	  }*/
 }

  private List<String> topicList(Properties properties)
  {
	 List<String> listTopics = null;
	  String sourceSystem = properties.getProperty(DAFCT2Constant.EGRESS_DATA_FOR_SOURCE_SYSTEM);
	  log.info(" sourceSystem :: "+sourceSystem);
	 System.out.println(" sourceSystem :: "+sourceSystem);
		 
	  if(properties.getProperty(DAFCT2Constant.CONTI_SOURCE_SYSTEM).equals(sourceSystem)){
		  listTopics =
		          Arrays.asList(
		              properties.getProperty(DAFCT2Constant.SOURCE_TOPIC_NAME));
		  
		  return listTopics;
		  
	  }else if(properties.getProperty(DAFCT2Constant.BOSCH_SOURCE_SYSTEM).equals(sourceSystem)){
		  listTopics =
		          Arrays.asList(
		              properties.getProperty(DAFCT2Constant.SOURCE_BOSCH_TOPIC_NAME));
		  return listTopics;
		  
	  }else if(properties.getProperty(DAFCT2Constant.ALL_SOURCE_SYSTEM).equals(sourceSystem)){
		  listTopics =
		          Arrays.asList(
		        		  properties.getProperty(DAFCT2Constant.SOURCE_TOPIC_NAME),
		        		  properties.getProperty(DAFCT2Constant.SOURCE_BOSCH_TOPIC_NAME));
		  return listTopics;
	  }else if(properties.getProperty(DAFCT2Constant.DAF_STANDARD_SYSTEM).equals(sourceSystem)){
		  listTopics =
		          Arrays.asList(
		              properties.getProperty(DAFCT2Constant.SINK_INDEX_TOPIC_NAME),
		              properties.getProperty(DAFCT2Constant.SINK_STATUS_TOPIC_NAME),
		              properties.getProperty(DAFCT2Constant.SINK_MONITOR_TOPIC_NAME));
		  return listTopics;
	  }
	  
	 return null;
  }
  
  public void processing( Properties properties, List<String> listTopics) {
 
    DataStream<KafkaRecord<Object>> sourceInputStream =
    		this.streamExecutionEnvironment.addSource(
            new FlinkKafkaConsumer<KafkaRecord<Object>>(
                listTopics, new KafkaMessageDeSerializeSchema<Object>(), properties));
    sourceInputStream.print();

    new MessageProcessing<Object>().consumeMessages(sourceInputStream, "Records", properties);
       
  }
 
  public StreamExecutionEnvironment getstreamExecutionEnvironment() {
    return this.streamExecutionEnvironment;
  }

  public void startExecution() throws DAFCT2Exception {

    try {
      //env.execute("External Records");
      this.streamExecutionEnvironment.execute("External Records");


    } catch (Exception e) {
      log.error("Unable to process Sink Message using Flink ", e);
      throw new DAFCT2Exception("Unable to process Sink Message using Flink ", e);
    }
  }
}
