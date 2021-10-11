package net.atos.daf.ct2.main;

import java.io.FileReader;
import java.io.IOException;
import java.util.Properties;

import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.kafka.clients.consumer.ConsumerConfig;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.exception.DAFCT2Exception;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.processing.ConsumeSourceStream;
import net.atos.daf.ct2.processing.MessageProcessing;
import net.atos.daf.ct2.processing.ValidateSourceStream;
import net.atos.daf.ct2.util.MessageParseUtil;


public class BoschMessageProcessing {

	private static final Logger log = LogManager.getLogger(BoschMessageProcessing.class);
	public static String FILE_PATH;
	private StreamExecutionEnvironment streamExecutionEnvironment;

	public static Properties configuration() throws DAFCT2Exception {

		Properties properties = new Properties();
		properties.put(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, "earliest");

		try {
			properties.load(new FileReader(FILE_PATH));
			log.info("Configuration Loaded for Connecting Kafka inorder to Perform Mapping.");
			System.out.println("Configuration Loaded for Connecting Kafka inorder to Perform Mapping.");

		} catch (IOException e) {
			log.error("Unable to Find the File :: " + FILE_PATH, e);
			throw new DAFCT2Exception("Unable to Find the File :: " + FILE_PATH, e);
		}
		return properties;
	}

	public static void main(String[] args) {

		BoschMessageProcessing boschMessageProcessing = new BoschMessageProcessing();
		Properties properties = null;
		try {
			FILE_PATH = args[0];

			properties = configuration();
			System.out.println("properties object ===>" + properties);
			//boschMessageProcessing.auditBoschJobDetails(properties, "Bosch streaming job started");

			boschMessageProcessing.flinkConnection();
			boschMessageProcessing.processing(properties);
			boschMessageProcessing.startExecution();
			
		} catch (DAFCT2Exception e) {
			log.error("Exception: ", e);
			System.out.println(e.getMessage());
			//boschMessageProcessing.auditBoschJobDetails(properties, "Bosch streaming job failed :: "+e.getMessage());
			e.printStackTrace();
		} 
	}

	public void flinkConnection() {

		this.streamExecutionEnvironment = StreamExecutionEnvironment.getExecutionEnvironment();
		//this.streamExecutionEnvironment.setStreamTimeCharacteristic(TimeCharacteristic.EventTime);

		log.info("Flink Processing Started.");
	}

	public void processing(Properties properties) {
		log.info("Stage 1. Message processing method " );
		ConsumeSourceStream consumeSrcStream = new ConsumeSourceStream();
		ValidateSourceStream validateSourceStream = new ValidateSourceStream();
		
		DataStream<KafkaRecord<String>> boschInputStream = consumeSrcStream.consumeSourceInputStream(
				streamExecutionEnvironment, DAFCT2Constant.SOURCE_BOSCH_TOPIC_NAME, properties);
		log.info("Stage 2. Data Read from kafka topic done ==>" + properties.getProperty(DAFCT2Constant.SOURCE_BOSCH_TOPIC_NAME) );
		
		//data load in hbase
		log.info("data loading in hbase process started..");
		MessageParseUtil.storeDataInHbase(boschInputStream,properties);
		/*DataStream<Tuple2<Integer, KafkaRecord<String>>> boschSourceStreamSts = 
				new MessageProcessing<String, Tuple2<Integer, KafkaRecord<String>>>()
						.boschSourceStreamStatus(boschInputStream);*/

		/*boschInputStream.map(new MapFunction<KafkaRecord<String>,KafkaRecord<String>>(){

		private static final long serialVersionUID = 1L;
        String rowKey = null;
		@Override
		public KafkaRecord<String> map(KafkaRecord<String> value) throws Exception {
			try{
				JsonNode jsonNodeRec = JsonMapper.configuring().readTree(value.getValue());
				log.info("Bosch rec for history :: "+jsonNodeRec);
				String vin = DAFCT2Constant.UNKNOWN;
				String transId = DAFCT2Constant.UNKNOWN;
				
				if(jsonNodeRec != null && jsonNodeRec.get("metaData") != null && jsonNodeRec.get("metaData").get("vehicle") != null){
					if(jsonNodeRec.get("metaData").get("vehicle").get("vin") != null)
						vin = jsonNodeRec.get("metaData").get("vehicle").get("vin").asText();
					
					if(jsonNodeRec.get("metaData").get("vehicle").get("TransID") != null)
						transId = jsonNodeRec.get("metaData").get("vehicle").get("TransID").asText();
				}
				
				rowKey = transId + "_" + vin + "_" + TimeFormatter.getInstance().getCurrentUTCTime();
				
			}catch(Exception e){
				rowKey = "UnknownMessage" + "_" + TimeFormatter.getInstance().getCurrentUTCTime();
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
			properties.getProperty(DAFCT2Constant.HBASE_BOSCH_HISTORICAL_TABLE_NAME),
			properties.getProperty(DAFCT2Constant.HBASE_BOSCH_HISTORICAL_TABLE_CF)));*/

		/*DataStream<Tuple2<Integer, KafkaRecord<String>>> boschStreamValidSts = validateSourceStream
				.isValidJSON(boschInputStream);
		DataStream<KafkaRecord<String>> boschValidInputStream = validateSourceStream
				.getValidSourceMessages(boschStreamValidSts);
		//TODO 

	/*	new EgressCorruptMessages().egressCorruptMessages(boschSourceStreamSts, properties,
				properties.getProperty(DAFCT2Constant.BOSCH_CORRUPT_MESSAGE_TOPIC_NAME)); */

		/*DataStream<KafkaRecord<String>> boschMeasurementInputStream = validateSourceStream
				.getValidSourceMessages(boschSourceStreamSts, DAFCT2Constant.MEASUREMENT_DATA);*/
		
		//DataStream<KafkaRecord<String>> boschTCUInputStream = 
	/*	validateSourceStream
				.getValidSourceMessages(boschSourceStreamSts, DAFCT2Constant.TCU_DATA)
				.addSink(new FlinkKafkaProducer<>(properties.getProperty(DAFCT2Constant.SINK_TCU_TOPIC_NAME), 
						new ProducerStringSerializationSchema(properties.getProperty(DAFCT2Constant.SINK_TCU_TOPIC_NAME)), 
						properties, FlinkKafkaProducer.Semantic.EXACTLY_ONCE));*/
				
		log.info("Stage 3. data filtering as per message type start ");
		
		DataStream<KafkaRecord<String>> boschIndexTCUInputStream = MessageParseUtil.filterDataInputStream(boschInputStream,properties.getProperty(DAFCT2Constant.FILTER_INDEX_TRANSID));
		DataStream<KafkaRecord<String>> boschMoniterTCUInputStream = MessageParseUtil.filterDataInputStream(boschInputStream,properties.getProperty(DAFCT2Constant.FILTER_MONITOR_TRANSID));
		DataStream<KafkaRecord<String>> boschStatusTCUInputStream = MessageParseUtil.filterDataInputStream(boschInputStream,properties.getProperty(DAFCT2Constant.FILTER_STATUS_TRANSID));
		
		log.info("Stage 4. data filtering as per message type end "); 
		log.info("Stage 5. data parsing and publishing message on kafka topic start");
		
		new MessageProcessing<String, Index>().consumeBoschMessage(boschIndexTCUInputStream,
				properties.getProperty(DAFCT2Constant.INDEX_TRANSID), "Index",
				properties.getProperty(DAFCT2Constant.SINK_INDEX_TOPIC_NAME), properties, Index.class);
		
		new MessageProcessing<String, Monitor>().consumeBoschMessage(boschMoniterTCUInputStream,
				properties.getProperty(DAFCT2Constant.MONITOR_TRANSID), "Monitor",
				properties.getProperty(DAFCT2Constant.SINK_MONITOR_TOPIC_NAME), properties, Monitor.class);
		
		new MessageProcessing<String, Status>().consumeBoschMessage(boschStatusTCUInputStream,
				properties.getProperty(DAFCT2Constant.STATUS_TRANSID), "Status",
				properties.getProperty(DAFCT2Constant.SINK_STATUS_TOPIC_NAME), properties, Status.class);
		
		log.info("Stage 6. data parsing and publishing message on kafka topic end");
		
	

	}



	public StreamExecutionEnvironment getstreamExecutionEnvironment() {
		return this.streamExecutionEnvironment;
		/*this.streamExecutionEnvironment.enableCheckpointing(5000);
	    this.streamExecutionEnvironment.getCheckpointConfig().setMaxConcurrentCheckpoints(1);
	    this.streamExecutionEnvironment
	        .getCheckpointConfig()
	        .enableExternalizedCheckpoints(
	            CheckpointConfig.ExternalizedCheckpointCleanup.RETAIN_ON_CANCELLATION);*/
	    // this.streamExecutionEnvironment.setStateBackend(new FsStateBackend("file:///checkpointDir"));
	}

	public void startExecution() throws DAFCT2Exception {

		try {
			this.streamExecutionEnvironment.execute("Bosch Streaming");

		} catch (Exception e) {
			log.error("Unable to process Message using Flink ", e);
			e.printStackTrace();
			throw new DAFCT2Exception("Unable to process Message using Flink ", e);
		}
	}
	
	public void auditBoschJobDetails(Properties properties, String message){
		System.out.println("Calling audit service for Bosch :: ");
		/*new KafkaAuditService().auditTrail(
				properties.getProperty(DAFCT2Constant.GRPC_SERVER),
				properties.getProperty(DAFCT2Constant.GRPC_PORT),
				DAFCT2Constant.JOB_NAME, 
				message);*/
	}
}
