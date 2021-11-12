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
		
		//properties.put(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, properties.get(DAFCT2Constant.AUTO_OFFSET_RESET_CONFIG));

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
			boschMessageProcessing.startExecution(properties);
			
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
		//load source properties file
		Properties sourceProperties = new Properties();
		
		sourceProperties.put(DAFCT2Constant.SOURCE_BOSCH_TOPIC_NAME,  properties.getProperty(DAFCT2Constant.SOURCE_BOSCH_TOPIC_NAME));
		sourceProperties.put(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG,properties.get(DAFCT2Constant.SOURCE_AUTO_OFFSET_RESET_CONFIG_VAL));
		sourceProperties.put(ConsumerConfig.CLIENT_ID_CONFIG, properties.get(DAFCT2Constant.SOURCE_CLIENT_ID_CONFIG_VAL));
		sourceProperties.put(ConsumerConfig.GROUP_ID_CONFIG, properties.get(DAFCT2Constant.SOURCE_GROUP_ID_CONFIG_VAL));
		sourceProperties.put(ConsumerConfig.REQUEST_TIMEOUT_MS_CONFIG, properties.get(DAFCT2Constant.SOURCE_REQUEST_TIMEOUT_MS_CONFIG_VAL));
		sourceProperties.put(ConsumerConfig.BOOTSTRAP_SERVERS_CONFIG, properties.get(DAFCT2Constant.SOURCE_BOOTSTRAP_SERVERS_CONFIG_VAL));
		sourceProperties.put(DAFCT2Constant.SOURCE_SECURITY_PROTOCOL_CONFIG, properties.get(DAFCT2Constant.SOURCE_SECURITY_PROTOCOL_CONFIG_VAL));
		sourceProperties.put(DAFCT2Constant.SOURCE_SASL_MECHANISM_CONFIG, properties.get(DAFCT2Constant.SOURCE_SASL_MECHANISM_CONFIG_VAL));
		sourceProperties.put(DAFCT2Constant.SOURCE_SASL_JAAS_CONFIG, properties.get(DAFCT2Constant.SOURCE_SASL_JAAS_CONFIG_VAL));
		
		DataStream<KafkaRecord<String>> boschInputStream = consumeSrcStream.consumeSourceInputStream(
				streamExecutionEnvironment, DAFCT2Constant.SOURCE_BOSCH_TOPIC_NAME, sourceProperties);
		
		log.info("Stage 2. Data Read from kafka topic done ==>" + sourceProperties.getProperty(DAFCT2Constant.SOURCE_BOSCH_TOPIC_NAME) );
		
		//data load in hbase
		log.info("data loading in hbase process started..");
		String isHbaseStore = (String) properties.get(DAFCT2Constant.SINK_HBASE_STORE);
		log.info(" Hbase store status "+isHbaseStore);
		
		if("TRUE".equalsIgnoreCase(isHbaseStore)) {
			MessageParseUtil.storeDataInHbase(boschInputStream,properties);
		} 
		
		
				
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
		
		log.info("Stage 6.Bosch data parsing and publishing message on kafka topic completed.");
		
		System.out.println("Stage 6.Bosch data parsing and publishing message on kafka topic completed.");

	}



	public StreamExecutionEnvironment getstreamExecutionEnvironment() {
		return this.streamExecutionEnvironment;
		
	}

	public void startExecution(Properties properties) throws DAFCT2Exception {

		try {
			this.streamExecutionEnvironment.execute(properties.getProperty(DAFCT2Constant.BOSCH_JOB_NAME));

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
