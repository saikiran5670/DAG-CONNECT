package net.atos.daf.ct2.main;

import java.io.FileReader;
import java.io.IOException;
import java.util.HashMap;
import java.util.Map;
import java.util.Properties;

import org.apache.kafka.clients.producer.ProducerConfig;
import org.apache.kafka.common.serialization.StringSerializer;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import io.debezium.config.Configuration;
import io.debezium.connector.postgresql.PostgresConnectorConfig;
import io.debezium.embedded.EmbeddedEngine;
import net.atos.daf.common.AuditETLJobClient;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.exception.DAFCT2Exception;
import net.atos.daf.ct2.processing.EgressCDCLayer;
import net.atos.daf.ct2.processing.MessageProcessing;

public class PostgresCdc {


	  private static final Logger log = LogManager.getLogger(PostgresCdc.class);
	  private static String FILE_PATH;
	  private static AuditETLJobClient auditETLJobClient;
	  //private static Properties properties;

	  public static void main(String[] args) throws InterruptedException {
		  Properties properties = null;  
	    try {
	       FILE_PATH= args[0];

	      PostgresCdc postgresCdc = new PostgresCdc();
	      properties = postgresCdc.configuration(FILE_PATH);
	      auditTrail(properties, "Streaming master data job started");

	      Configuration configuration = postgresCdc.connectingPostgreSQL(properties);

	      //Integer.parseInt(properties.getProperty(DAFCT2Constant.POSTGRE_CDC_VID_VIN)) == 1)
	      if ("vehicleMaster".equals(properties.getProperty(DAFCT2Constant.POSTGRE_CDC_NAME))) {
	    	MessageProcessing messageProcessing = new MessageProcessing(configuration, properties);
	        messageProcessing.run();
	      } else {
	        EgressCDCLayer egressCDCLayer = new EgressCDCLayer(configuration, properties);
	        egressCDCLayer.run();
	      }

	    } catch (DAFCT2Exception e) {
	      log.error("Exception: ", e);
	      auditTrail(properties, "Streaming master data job failed :: "+e.getMessage());
	    } 
	  }

	  public static void auditTrail(Properties properties, String msg) {
		  try{
			  auditETLJobClient =
				        new AuditETLJobClient(
				            properties.getProperty(DAFCT2Constant.GRPC_SERVER),
				            Integer.valueOf(properties.getProperty(DAFCT2Constant.GRPC_PORT)));

				    Map<String, String> auditMap = new HashMap<String, String>();

				    auditMap.put(DAFCT2Constant.JOB_EXEC_TIME, String.valueOf(TimeFormatter.getInstance().getCurrentUTCTime()));
				    auditMap.put(DAFCT2Constant.AUDIT_PERFORMED_BY, DAFCT2Constant.JOB_NAME);
				    auditMap.put(DAFCT2Constant.AUDIT_COMPONENT_NAME, DAFCT2Constant.JOB_NAME);
				    auditMap.put(DAFCT2Constant.AUDIT_SERVICE_NAME, DAFCT2Constant.AUDIT_SERVICE);
				    auditMap.put(DAFCT2Constant.AUDIT_EVENT_TYPE, DAFCT2Constant.AUDIT_CREATE_EVENT_TYPE);
				    auditMap.put(
				        DAFCT2Constant.AUDIT_EVENT_TIME, String.valueOf(TimeFormatter.getInstance().getCurrentUTCTime()));
				    auditMap.put(DAFCT2Constant.AUDIT_EVENT_STATUS, DAFCT2Constant.AUDIT_EVENT_STATUS_START);
				    auditMap.put(DAFCT2Constant.AUDIT_MESSAGE, msg);
				    auditMap.put(DAFCT2Constant.AUDIT_SOURCE_OBJECT_ID, DAFCT2Constant.DEFAULT_OBJECT_ID);
				    auditMap.put(DAFCT2Constant.AUDIT_TARGET_OBJECT_ID, DAFCT2Constant.DEFAULT_OBJECT_ID);

				    auditETLJobClient.auditTrialGrpcCall(auditMap);

				    log.info("Audit Trial Started");
		  }catch(Exception e){
			  log.error("Issue while audit Streaming master data job :: " + e.getMessage());
		  }finally{
		      auditETLJobClient.closeChannel();
		  }
	    
	  }

	  public Properties configuration(String filePath) throws DAFCT2Exception {

	    Properties properties = new Properties();
	    properties.put(ProducerConfig.KEY_SERIALIZER_CLASS_CONFIG, StringSerializer.class);
	    properties.put(ProducerConfig.VALUE_SERIALIZER_CLASS_CONFIG, StringSerializer.class);

	    try {
	      properties.load(new FileReader(filePath));
	      log.info("Configuration Loaded for Connecting Kafka.");

	    } catch (IOException e) {
	      log.error("Unable to Find the File " + filePath, e);
	      throw new DAFCT2Exception("Unable to Find the File " + filePath, e);
	    }
	    return properties;
	  }

	  public Configuration connectingPostgreSQL(Properties properties) throws DAFCT2Exception {

	    Configuration configuration = null;

	    try {
	      configuration =
	          Configuration.create()
	              .with(
	                  EmbeddedEngine.CONNECTOR_CLASS,
	                  properties.getProperty(DAFCT2Constant.POSTGRE_CONNECTOR_CLASS))
	              .with(
	                  EmbeddedEngine.OFFSET_STORAGE,
	                  "org.apache.kafka.connect.storage.FileOffsetBackingStore")
	              .with(
	                  EmbeddedEngine.OFFSET_STORAGE_FILE_FILENAME,
	                  properties.getProperty(DAFCT2Constant.POSTGRE_OFFSET_STORAGE_FILE_FILENAME))
	              .with(
	                  EmbeddedEngine.OFFSET_FLUSH_INTERVAL_MS,
	                  properties.getProperty(DAFCT2Constant.POSTGRE_OFFSET_FLUSH_INTERVAL_MS))
	              .with("name", "PostgreCDC_Kafka_Streaming")
	              .with("database.server.id", properties.getProperty(DAFCT2Constant.POSTGRE_SERVER_ID))
	              .with(
	                  PostgresConnectorConfig.SERVER_NAME,
	                  properties.getProperty(DAFCT2Constant.POSTGRE_SERVER_NAME))
	              .with(
	                  PostgresConnectorConfig.HOSTNAME,
	                  properties.getProperty(DAFCT2Constant.POSTGRE_HOSTNAME))
	              .with(
	                  PostgresConnectorConfig.PORT, properties.getProperty(DAFCT2Constant.POSTGRE_PORT))
	              .with(
	                  PostgresConnectorConfig.USER, properties.getProperty(DAFCT2Constant.POSTGRE_USER))
	              .with(
	                  PostgresConnectorConfig.PASSWORD,
	                  properties.getProperty(DAFCT2Constant.POSTGRE_PASSWORD))
	              .with(
	                  PostgresConnectorConfig.SSL_MODE,
	                  properties.getProperty(DAFCT2Constant.POSTGRE_SSL))
	              .with(
	                  PostgresConnectorConfig.DATABASE_NAME,
	                  properties.getProperty(DAFCT2Constant.POSTGRE_DATABASE_NAME))
	              .with(
	                  PostgresConnectorConfig.TABLE_WHITELIST,
	                  properties.getProperty(DAFCT2Constant.POSTGRE_TABLE_WHITELIST))
	              .with(
	                  PostgresConnectorConfig.PLUGIN_NAME,
	                  properties.getProperty(DAFCT2Constant.POSTGRE_PLUGIN_NAME))
	              // .with("database.history", "io.debezium.relational.history.FileDatabaseHistory")
	              // .with("database.history.file.filename", "src/main/resources/dbhistory.dat")
	              .build();

	      log.info("Configure PostgeSQL Database");

	    } catch (Exception e) {
	      log.error("Unable to Configure PostgeSQL Database", e);
	      throw new DAFCT2Exception("Unable to Configure PostgeSQL Database", e);
	    }

	    return configuration;
	  }
	
}
