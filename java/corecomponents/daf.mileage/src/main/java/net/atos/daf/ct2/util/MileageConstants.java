package net.atos.daf.ct2.util;

public class MileageConstants {
	
	//Streaming environment 
	public static final String PARALLELISM = "parallelism";
	public static final String CHECKPOINT_INTERVAL = "checkpoint.interval";
	public static final String MINIMUM_PAUSE_BETWEEN_CHECKPOINTS = "min.checkpoint.pause";
	public static final String CHECKPOINT_TIMEOUT = "checkpoint.timeout";
	public static final String MAX_CONCURRENT_CHECKPOINTS = "max.concurrent.checkpoints";
	public static final String CHECKPOINT_DIRECTORY = "checkpoint.directory";
	public static final String RESTART_ATTEMPS = "restart.attempts";
	public static final String RESTART_INTERVAL = "restart.interval";
	public static final String RESTART_FLAG = "restart.flag";
	public static final String FIXED_RESTART_FLAG = "fixed.restart.flag";
	public static final String RESTART_FAILURE_RATE = "restart.failure.rate";
	public static final String RESTART_FAILURE_INTERVAL = "restart.failure.interval";
	public static final String RESTART_FAILURE_DELAY = "restart.failure.delay";
	
	//Datamart Constants
	public static final String DATAMART_POSTGRE_SERVER_NAME = "datamart.postgres.server.name";
	public static final String DATAMART_POSTGRE_PORT = "datamart.postgres.port";
	public static final String DATAMART_POSTGRE_USER = "datamart.postgres.userId";
	public static final String DATAMART_POSTGRE_DATABASE_NAME ="datamart.postgres.database.name";
	public static final String DATAMART_POSTGRE_PASSWORD = "datamart.postgres.password";
	public static final String POSTGRE_SQL_DRIVER = "postgresql.driver";

	//Date Format
	public static final String DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'";
	public static final String DATE_FORMAT_UTC = "EEE MMM dd HH:mm:ss zzz yyyy";
	
	//Audit Constants
	public static final String JOB_EXEC_TIME = "jobExecTime";
	public static final String AUDIT_PERFORMED_BY = "performedBy";
	public static final String AUDIT_COMPONENT_NAME = "componentName";
	public static final String AUDIT_SERVICE_NAME = "serviceName";
	public static final String AUDIT_EVENT_TYPE = "eventType";
	public static final String AUDIT_EVENT_TIME = "eventtime";
	public static final String AUDIT_EVENT_STATUS = "eventstatus";
	public static final String AUDIT_MESSAGE = "message";
	public static final String AUDIT_SOURCE_OBJECT_ID = "sourceObjectId";
	public static final String AUDIT_TARGET_OBJECT_ID = "targetObjectId";
	public static final String AUDIT_UPDATED_DATA = "updateddata";
	public static final String AUDIT_SERVICE = "FLINK";
	public static final String AUDIT_CREATE_EVENT_TYPE = "1";
	public static final String AUDIT_EVENT_STATUS_START = "2";
	public static final String AUDIT_EVENT_STATUS_FAIL = "1";
	public static final String GRPC_SERVER = "grpc.server";
	public static final String GRPC_PORT = "grpc.port";
	public static final String MILEAGE_JOB_NAME = "mileageStreamingJob";
	public static final String DEFAULT_OBJECT_ID = "00";
	
	//Mileage parameters
	public static final String MILEAGE_TIME_WINDOW_SECONDS = "mileage.time.window.seconds";
	public static final String MILEAGE_BUSINESS_TIME_WINDOW_SECONDS = "mileage.business.time.window.seconds";
	public static final String MILEAGE_ERROR_MARGIN = "mileage.error.margin";
	public static final String MILEAGE_WATERMARK_TIME_WINDOW_SECONDS = "mileage.watermark.time.window.seconds";

	//Kafka parameter
	
	public static final String BOOTSTRAP_SERVERS="bootstrap.servers";
	public static final String STATUS_TOPIC = "status.message.topic";
	public static final String SASL_JAAS_CONFIG = "sasl.jaas.config";
	public static final String SASL_MECHANISM = "sasl.mechanism";
	public static final String SECURITY_PROTOCOL = "security.protocol";
	public static final String EVENT_HUB_BOOTSTRAP = "event.hub.bootstrap";
	public static final String GROUP_ID="group.id";
	public static final String AUTO_OFFSET_RESET_CONFIG = "auto.offset.reset.config";
	public static final String CLIENT_ID = "client.id";
	public static final String REQUEST_TIMEOUT_MILLISEC = "request.timeout.ms";
		
	public static final String UNKNOWN = "UNKNOWN";
	public static final Integer ZERO = 0;
	public static final Long ZERO_VAL = 0L;
		
}
