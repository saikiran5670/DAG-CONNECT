package net.atos.daf.ct2.util;

public class FuelDeviationConstants {
	
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
	
	//Datamart Constants
	public static final String DATAMART_POSTGRE_SERVER_NAME = "datamart.postgres.server.name";
	public static final String DATAMART_POSTGRE_PORT = "datamart.postgres.port";
	public static final String DATAMART_POSTGRE_USER = "datamart.postgres.userId";
	public static final String DATAMART_POSTGRE_DATABASE_NAME ="datamart.postgres.database.name";
	public static final String DATAMART_POSTGRE_PASSWORD = "datamart.postgres.password";

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
	public static final String FUEL_DEVIATION_JOB_NAME = "FuelDeviationStreamingJob";
	public static final String DEFAULT_OBJECT_ID = "00";
	
	//Fuel Deviation parameters
	public static final String FUEL_DEVIATION_TIME_WINDOW_SECONDS = "fuel.deviation.time.window.seconds";
	public static final String FUEL_DEVIATION_WATERMARK_TIME_WINDOW_SECONDS = "fuel.deviation.watermark.time.window.seconds";
	public static final String FUEL_DEVIATION_DURING_STOP_INCREASE_THRESHOLD_VAL = "fuel.deviation.during.stop.increase.threshold.val";
	public static final String FUEL_DEVIATION_DURING_STOP_DECREASE_THRESHOLD_VAL = "fuel.deviation.during.stop.decrease.threshold.val";
	public static final String FUEL_DEVIATION_DURING_TRIP_INCREASE_THRESHOLD_VAL = "fuel.deviation.during.trip.increase.threshold.val";
	public static final String FUEL_DEVIATION_DURING_TRIP_DECREASE_THRESHOLD_VAL = "fuel.deviation.during.trip.decrease.threshold.val";
	public static final String FUEL_DEVIATION_INCREASE_EVENT ="I";
	public static final String FUEL_DEVIATION_DECREASE_EVENT ="D";
	public static final String FUEL_DEVIATION_STOP_ACTIVITY_TYPE ="S";
	public static final String FUEL_DEVIATION_RUNNING_ACTIVITY_TYPE ="R";
	public static final String FUEL_DEVIATION_MEASUREMENT_SECONDS_VAL = "fuel.deviation.time.measurement.seconds";   //300000L;
	
	
	public static final Integer INDEX_TRIP_START = 4;
	public static final Integer INDEX_TRIP_END = 5;
	

	//Kafka parameter
	
	public static final String BOOTSTRAP_SERVERS="bootstrap.servers";
	public static final String INDEX_TOPIC = "index.message.topic";
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
