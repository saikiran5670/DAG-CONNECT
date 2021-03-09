package net.atos.daf.ct2.main;

import java.io.FileReader;
import java.io.IOException;
import java.util.Properties;

import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.kafka.clients.consumer.ConsumerConfig;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import com.fasterxml.jackson.databind.JsonNode;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.exception.DAFCT2Exception;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.Message;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.processing.BroadcastState;
import net.atos.daf.ct2.processing.ConsumeSourceStream;
import net.atos.daf.ct2.processing.EgressCorruptMessages;
import net.atos.daf.ct2.processing.KafkaAuditService;
import net.atos.daf.ct2.processing.MessageProcessing;
import net.atos.daf.ct2.processing.StoreHistoricalData;
import net.atos.daf.ct2.processing.ValidateSourceStream;
import net.atos.daf.ct2.utils.JsonMapper;

public class ContiMessageProcessing {

  private static final Logger log = LogManager.getLogger(ContiMessageProcessing.class);
  public static String FILE_PATH;
  private StreamExecutionEnvironment streamExecutionEnvironment;

  public static Properties configuration() throws DAFCT2Exception {

    Properties properties = new Properties();
    properties.put(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, "earliest");

    try {
      properties.load(new FileReader(FILE_PATH));
      log.info("Configuration Loaded for Connecting Kafka inorder to Perform Mapping.");

    } catch (IOException e) {
      log.error("Unable to Find the File " + FILE_PATH, e);
      throw new DAFCT2Exception("Unable to Find the File " + FILE_PATH, e);
    }
    return properties;
  }

  public static void main(String[] args) {
	ContiMessageProcessing contiMessageProcessing = new ContiMessageProcessing();
	Properties properties = null; 
    try {
      FILE_PATH = args[0];

     properties = configuration();
     contiMessageProcessing.auditContiJobDetails(properties, "Conti streaming job started");

      contiMessageProcessing.flinkConnection();
      contiMessageProcessing.processing(properties);
      contiMessageProcessing.startExecution();

    } catch (DAFCT2Exception e) {
      log.error("Exception: ", e);
      contiMessageProcessing.auditContiJobDetails(properties, "Conti streaming job failed :: "+e.getMessage());

      e.printStackTrace();

    } finally {
      //       auditETLJobClient.closeChannel();
    }
  }

  public void flinkConnection() {

    this.streamExecutionEnvironment = StreamExecutionEnvironment.getExecutionEnvironment();
    //this.streamExecutionEnvironment.setStreamTimeCharacteristic(TimeCharacteristic.EventTime);
    /*this.streamExecutionEnvironment.enableCheckpointing(5000);
    this.streamExecutionEnvironment.getCheckpointConfig().setMaxConcurrentCheckpoints(1);
    this.streamExecutionEnvironment
        .getCheckpointConfig()
        .enableExternalizedCheckpoints(
            CheckpointConfig.ExternalizedCheckpointCleanup.RETAIN_ON_CANCELLATION);*/
    // this.streamExecutionEnvironment.setStateBackend(new FsStateBackend("file:///checkpointDir"));

    log.info("Flink Processing Started.");
  }

  public void processing(Properties properties) {

    ConsumeSourceStream consumeSrcStream = new ConsumeSourceStream();
	ValidateSourceStream validateSourceStream = new ValidateSourceStream();
	
	 MapStateDescriptor<Message<String>, KafkaRecord<String>> mapStateDescriptor =
		        new BroadcastState<String>()
		            .stateInitialization(properties.getProperty(DAFCT2Constant.BROADCAST_NAME));
		    
    DataStream<KafkaRecord<String>> masterDataInputStream = consumeSrcStream.consumeSourceInputStream(
			streamExecutionEnvironment, DAFCT2Constant.MASTER_DATA_TOPIC_NAME, properties);
    
    masterDataInputStream.print();

    BroadcastStream<KafkaRecord<String>> broadcastStream =
        masterDataInputStream.broadcast(mapStateDescriptor);
	 
    DataStream<KafkaRecord<String>> contiInputStream = consumeSrcStream.consumeSourceInputStream(
			streamExecutionEnvironment, DAFCT2Constant.SOURCE_TOPIC_NAME, properties);

    contiInputStream.map(new MapFunction<KafkaRecord<String>,KafkaRecord<String>>(){

		/**
		 * 
		 */
		private static final long serialVersionUID = 1L;
        String rowKey = null;
		@Override
		public KafkaRecord<String> map(KafkaRecord<String> value) throws Exception {
			try{
				JsonNode jsonNodeRec = JsonMapper.configuring().readTree(value.getValue());
				System.out.println("for history :: "+jsonNodeRec);
				rowKey = jsonNodeRec.get("TransID").asText() + "_" + jsonNodeRec.get("VID").asText() + "_" + TimeFormatter.getInstance().getCurrentUTCTime();
				
			}catch(Exception e){
				rowKey = "CorruptMessage" + "_" + TimeFormatter.getInstance().getCurrentUTCTime();
			}
			
			value.setKey(rowKey);
			return value;
		}
    	}).addSink(new StoreHistoricalData(properties.getProperty(DAFCT2Constant.HBASE_ZOOKEEPER_QUORUM),
  			properties.getProperty(DAFCT2Constant.HBASE_ZOOKEEPER_PROPERTY_CLIENTPORT),
			properties.getProperty(DAFCT2Constant.ZOOKEEPER_ZNODE_PARENT),
			properties.getProperty(DAFCT2Constant.HBASE_REGIONSERVER),
			properties.getProperty(DAFCT2Constant.HBASE_MASTER),
			properties.getProperty(DAFCT2Constant.HBASE_REGIONSERVER_PORT),
			properties.getProperty(DAFCT2Constant.HBASE_CONTI_HISTORICAL_TABLE_NAME)));
	
    DataStream<Tuple2<Integer, KafkaRecord<String>>> contiStreamValiditySts = validateSourceStream
			.isValidJSON(contiInputStream);
	DataStream<KafkaRecord<String>> contiValidInputStream = validateSourceStream
			.getValidContiMessages(contiStreamValiditySts);

	new EgressCorruptMessages().egressCorruptMessages(contiStreamValiditySts, properties,
			properties.getProperty(DAFCT2Constant.CONTI_CORRUPT_MESSAGE_TOPIC_NAME));

	contiValidInputStream.print();

    new MessageProcessing<String, Index>()
        .consumeContiMessage(
        	contiValidInputStream,
            properties.getProperty(DAFCT2Constant.INDEX_TRANSID),
            "Index",
            properties.getProperty(DAFCT2Constant.SINK_INDEX_TOPIC_NAME),
            properties,
            Index.class,
            broadcastStream);

    new MessageProcessing<String, Status>()
        .consumeContiMessage(
        	contiValidInputStream,
            properties.getProperty(DAFCT2Constant.STATUS_TRANSID),
            "Status",
            properties.getProperty(DAFCT2Constant.SINK_STATUS_TOPIC_NAME),
            properties,
            Status.class,
            broadcastStream);

    new MessageProcessing<String, Monitor>()
        .consumeContiMessage(
        	contiValidInputStream,
            properties.getProperty(DAFCT2Constant.MONITOR_TRANSID),
            "Monitor",
            properties.getProperty(DAFCT2Constant.SINK_MONITOR_TOPIC_NAME),
            properties,
            Monitor.class,
            broadcastStream);
  }

  public StreamExecutionEnvironment getstreamExecutionEnvironment() {
    return this.streamExecutionEnvironment;
  }

  public void startExecution() throws DAFCT2Exception {

    try {
      this.streamExecutionEnvironment.execute("Realtime Records");

    } catch (Exception e) {
      log.error("Unable to process Message using Flink ", e);
      e.printStackTrace();
      throw new DAFCT2Exception("Unable to process Message using Flink ", e);
    }
  }
  
  public void auditContiJobDetails(Properties properties, String message){
	  try{
		  new KafkaAuditService().auditTrail(
					properties.getProperty(DAFCT2Constant.GRPC_SERVER),
					properties.getProperty(DAFCT2Constant.GRPC_PORT),
					DAFCT2Constant.JOB_NAME, 
					message);
	  }catch(Exception e){
		  System.out.println("Issue while auditing streaming conti job ");
	  } 
  }
}
