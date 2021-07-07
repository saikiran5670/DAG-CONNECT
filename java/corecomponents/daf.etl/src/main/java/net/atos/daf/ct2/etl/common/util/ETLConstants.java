package net.atos.daf.ct2.etl.common.util;

public class ETLConstants {
	
	//Streaming environment 
	public static final String PARALLELISM = "parallelism";
	public static final String CHECKPOINT_INTERVAL = "checkpoint_interval";
	public static final String MINIMUM_PAUSE_BETWEEN_CHECKPOINTS = "min_checkpoint_pause";
	public static final String CHECKPOINT_TIMEOUT = "checkpoint_timeout";
	public static final String MAX_CONCURRENT_CHECKPOINTS = "max_concurrent_checkpoints";
	public static final String CHECKPOINT_DIRECTORY = "checkpoint_directory";
	public static final String RESTART_ATTEMPS = "restart_attempts";
	public static final String RESTART_INTERVAL = "restart_interval";
	public static final String RESTART_FLAG = "restart_flag";
		
	//HBase
	public static final String HBASE_ZOOKEEPER_QUORUM = "hbase.zookeeper.quorum";
	public static final String HBASE_ZOOKEEPER_PROPERTY_CLIENTPORT = "hbase.zookeeper.property.clientPort";
	public static final String HADOOP_SECURITY_AUTHENTICATION = "hadoop.security.authentication";
	public static final String HBASE_SECURITY_AUTHENTICATION ="hbase.security.authentication";
	public static final String HBASE_MASTER_KERBEROS_PRINCIPAL = "hbase.master.kerberos.principal";
	public static final String HBASE_REGIONSERVER_KERBEROS_PRINCIPAL = "hbase.regionserver.kerberos.principal";
	public static final String HBASE_KERBEROS_USER = "hbaseKerberosUser";
	public static final String HBASE_KEYTAB_PATH = "hbaseKeytabPath";
	public static final String STATUS_SUCCESS = "SUCCESS";
	public static final String STATUS_FAILURE = "FAILURE";
	public static final String HBASE_CLIENT_RETRIES = "hbaseClientRetriesNumber";
	public static final String HBASE_CLIENT_PAUSE = "hbaseClientPause";
	public static final String HBASE_ZOOKEEPER_RETRIES = "zookeeperRecoveryRetry";
	public static final String ZOOKEEPER_ZNODE_PARENT = "zookeeper.znode.parent";
	public static final String HBASE_REGIONSERVER = "hbase.regionserver";
	public static final String HBASE_MASTER = "hbase.master";
	public static final String HBASE_REGIONSERVER_PORT = "hbase.regionserver.port";
	public static final String HBASE_ROOTDIR = "hbase.rootdir";
	
	public static final String HBASE_CONNECTION_ERROR_CODE = "";
	
	//Postgre Sql
	public static final String POSTGRE_SQL_PASSWORD = "postgresql_password";
	public static final String POSTGRE_SQL_DRIVER = "postgresql_driver";
	public static final String POSTGRE_SQL_URL = "postgresql_url";
	public static final String POSTGRE_SQL_SSL_MODE = "&sslmode=require";
	public static final String POSTGRE_SQL_SERVER = "postgresql_server";
	public static final String POSTGRE_SQL_TRIPDATAMART = "postgresql_tripdatamart";
	public static final String POSTGRE_SQL_MASTERDATAMART = "postgresql_masterdatamart";

	//Datamart Constants
	public static final String DATAMART_POSTGRE_SERVER_NAME = "datamart_postgre_server_name";
	public static final String DATAMART_POSTGRE_PORT = "datamart_postgre_port";
	public static final String DATAMART_POSTGRE_USER = "datamart_postgre_userId";
	public static final String DATAMART_POSTGRE_DATABASE_NAME ="datamart_postgre_database_name";
	public static final String DATAMART_POSTGRE_PASSWORD = "datamart_postgre_password";
	
	//master datamart
	public static final String MASTER_POSTGRE_SERVER_NAME = "master_postgre_server_name";
	public static final String MASTER_POSTGRE_PORT = "master_postgre_port";
	public static final String MASTER_POSTGRE_DATABASE_NAME = "master_postgre_database_name";
	public static final String MASTER_POSTGRE_USER = "master_postgre_userId";
	public static final String MASTER_POSTGRE_PASSWORD = "master_postgre_password";
	
	//HBase Status Column Family
	public static final String STS_MSG_COLUMNFAMILY_T = "t";
	
	//HBase Status Columns 
	/*public static final String TRIP_ID = "tripId";
	public static final String VID = "vid";
	public static final String GPS_START_DATETIME = "gpsStartDateTime";
	public static final String GPS_END_DATETIME = "gpsEndDateTime";
	public static final String GPS_TRIP_DIST = "gpsTripDist";
	public static final String GPS_STOP_VEH_DIST = "gpsStopVehDist";
	public static final String GPS_START_VEH_DIST = "gpsStartVehDist";
	public static final String VIDLE_DURATION = "vIdleDuration";
	public static final String GPS_START_LATITUDE = "gpsStartLatitude";
	public static final String GPS_START_LONGITUDE = "gpsStartLongitude";
	public static final String GPS_END_LATITUDE = "gpsEndLatitude";
	public static final String GPS_END_LONGITUDE = "gpsEndLongitude";
	public static final String VUSED_FUEL = "vUsedFuel";
	public static final String VSTOP_FUEL = "vStopFuel";
	public static final String VSTART_FUEL = "vStartFuel";
	public static final String VTRIP_MOTION_DURATION = "vTripMotionDuration";
	public static final String RECEIVED_TIMESTAMP = "receivedTimestamp";
	public static final String VPTO_DURATION = "vPTODuration";
	public static final String VHARSH_BRAKE_DURATION = "vHarshBrakeDuration";
	public static final String VBRAKE_DURATION = "vBrakeDuration";
	public static final String VMAX_THROTTLE_PADDLE_DURATION = "vMaxThrottlePaddleDuration";
	public static final String VTRIP_ACCELERATION_TIME = "vTripAccelerationTime";
	public static final String VCRUISE_CONTROL_DIST = "vCruiseControlDist";
	public static final String VTRIP_DPA_BRAKINGCOUNT = "vTripDPABrakingCount";
	public static final String VTRIP_DPA_ANTICIPATION_COUNT = "vTripDPAAnticipationCount";
	public static final String VCRUISE_CONTROL_FUEL_CONSUMED = "vCruiseControlFuelConsumed";
	public static final String VIDLE_FUEL_CONSUMED = "vIdleFuelConsumed";
	public static final String VSUM_TRIP_DPA_BRAKING_SCORE = "vSumTripDPABrakingScore";
	public static final String VSUM_TRIP_DPA_ANTICIPATION_SCORE = "vSumTripDPAAnticipationScore";
	public static final String DRIVER_ID = "driverId";
	public static final String EVENT_DATETIME_FIRST_INDEX = "eventDateTimeFirstIndex";
	public static final String EVT_DATETIME = "evtDateTime";
	public static final String INCREMENT = "increment";*/
	
	public static final String TRIP_ID = "TripID";
	public static final String VID = "VID";
	public static final String VIN = "VIN";
	public static final String GPS_START_DATETIME = "GPSStartDateTime";
	public static final String GPS_END_DATETIME = "GPSEndDateTime";
	public static final String GPS_TRIP_DIST = "GPSTripDist";
	public static final String GPS_STOP_VEH_DIST = "GPSStopVehDist";
	public static final String GPS_START_VEH_DIST = "GPSStartVehDist";
	public static final String VIDLE_DURATION = "VIdleDuration";
	public static final String GPS_START_LATITUDE = "GPSStartLatitude";
	public static final String GPS_START_LONGITUDE = "GPSStartLongitude";
	public static final String GPS_END_LATITUDE = "GPSEndLatitude";
	public static final String GPS_END_LONGITUDE = "GPSEndLongitude";
	public static final String VUSED_FUEL = "VUsedFuel";
	public static final String VSTOP_FUEL = "VStopFuel";
	public static final String VSTART_FUEL = "VStartFuel";
	public static final String VTRIP_MOTION_DURATION = "VTripMotionDuration";
	public static final String RECEIVED_TIMESTAMP = "receivedTimestamp";
	public static final String VPTO_DURATION = "VPTODuration";
	public static final String VHARSH_BRAKE_DURATION = "VHarshBrakeDuration";
	public static final String VBRAKE_DURATION = "VBrakeDuration";
	public static final String VMAX_THROTTLE_PADDLE_DURATION = "VMaxThrottlePaddleDuration";
	public static final String VTRIP_ACCELERATION_TIME = "VTripAccelerationTime";
	public static final String VCRUISE_CONTROL_DIST = "VCruiseControlDist";
	public static final String VTRIP_DPA_BRAKINGCOUNT = "VTripDPABrakingCount";
	public static final String VTRIP_DPA_ANTICIPATION_COUNT = "VTripDPAAnticipationCount";
	public static final String VCRUISE_CONTROL_FUEL_CONSUMED = "VCruiseControlFuelConsumed";
	public static final String VIDLE_FUEL_CONSUMED = "VIdleFuelConsumed";
	public static final String VSUM_TRIP_DPA_BRAKING_SCORE = "VSumTripDPABrakingScore";
	public static final String VSUM_TRIP_DPA_ANTICIPATION_SCORE = "VSumTripDPAAnticipationScore";
	public static final String DRIVER_ID = "DriverID";
	public static final String EVENT_DATETIME_FIRST_INDEX = "EventDateTimeFirstIndex";
	public static final String EVT_DATETIME = "EvtDateTime";
	public static final String INCREMENT = "Increment";

	//HBase Index Column Family
	public static final String INDEX_MSG_COLUMNFAMILY_T = "t";
	public static final String INDEX_MSG_COLUMNFAMILY_F = "f";
	//HBase Index Columns
	/*public static final String INDEX_MSG_TRIP_ID = "tripId";
	public static final String INDEX_MSG_VID = "vid";
	public static final String INDEX_MSG_V_TACHOGRAPH_SPEED = "vTachographSpeed";
	public static final String INDEX_MSG_V_GROSSWEIGHT_COMBINATION = "vGrossWeightCombination";
	public static final String INDEX_MSG_DRIVER2_ID = "driver2Id";
	public static final String INDEX_MSG_JOBNAME = "jobName";
	public static final String INDEX_MSG_INCREMENT = "increment";*/
	public static final String INDEX_MSG_TRIP_ID = "TripID";
	public static final String INDEX_MSG_VID = "VID";
	public static final String INDEX_MSG_V_TACHOGRAPH_SPEED = "VTachographSpeed";
	public static final String INDEX_MSG_V_GROSSWEIGHT_COMBINATION = "VGrossWeightCombination";
	public static final String INDEX_MSG_DRIVER2_ID = "Driver2ID";
	public static final String INDEX_MSG_JOBNAME = "Jobname";
	public static final String INDEX_MSG_INCREMENT = "Increment";
	public static final String INDEX_MSG_VDIST = "VDist";
	public static final String INDEX_MSG_EVT_DATETIME = "EvtDateTime";
	
	
	//HBase table details
	public static final String INDEX_TABLE_NM = "index_table_name";
	public static final String STATUS_TABLE_NM = "status_table_name";
	
	//Conti Message Constants
	public static final String INDEX_MSG_TRANSID = "03000";
	public static final String STATUS_MSG_TRANSID = "03010";
	public static final String MONITORING_MSG_TRANSID = "03030";
	
	
	//Mini Batch Parameters
	public static final String TRIP_JOB_START_TIME ="trip_job_start_time";
	public static final String TRIP_ETL_FREQUENCY = "trip_etl_frequency";
	public static final String TRIP_ETL_MAX_TIME = "trip_etl_max_time";
	public static final String TRIP_ETL_MIN_TIME = "trip_etl_min_time";
	public static final String IS_TRIP_MINI_ETL_STREAMING = "is_streaming";
	
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
	public static final String GRPC_SERVER = "grpc_server";
	public static final String GRPC_PORT = "grpc_port";
	public static final String TRIP_JOB_NAME ="tripEtlJob";
	public static final String TRIP_STREAMING_JOB_NAME ="tripStreamingJob";
	public static final String DEFAULT_OBJECT_ID ="00";
	
	//Trip related parameters
	public static final String TRIP_TIME_WINDOW_MILLISEC ="trip.time.window.millisec";

	//Kafka parameter
	
	public static final String BOOTSTRAP_SERVERS="bootstrap.servers";
	public static final String WN0_KAFKA_INTERNAL_9092 = "wn0-kafka.c0kbh01dsx0uhhbdv5oig221gb.ax.internal.cloudapp.net:9092";
	public static final String ZOOKEEPER_CONNECT= "zookeeper.connect";
	public static final String WN0_KAFKA_INTERNAL_2181 = "wn0-kafka.c0kbh01dsx0uhhbdv5oig221gb.ax.internal.cloudapp.net:2181";
	public static final String GROUP_ID="group.id";
	public static final String INDEX_TOPIC="Index_Topic";
	public static final String MONITORING_TOPIC="Monitoring_Topic";
	public static final String STATUS_TOPIC = "Status_Topic";
	public static final String EVENT_HUB_CONFIG = "event.hub.config";
	public static final String EVENT_HUB_BOOTSTRAP = "event.hub.bootstrap";
	public static final String SECURITY_PROTOCOL = "security.protocol";
	public static final String SASL_MECHANISM = "sasl.mechanism";
	public static final String SASL_JAAS_CONFIG = "sasl.jaas.config";
	public static final String AUTO_OFFSET_RESET_CONFIG ="auto.offset.reset.config";
	public static final String CLIENT_ID = "client.id";
	public static final String EGRESS_TRIP_AGGR_TOPIC_NAME = "egress.trip.aggr.data.topic";
	public static final String EGRESS_TRIP_AGGR_DATA = "egress.trip.aggr.data";
	public static final String REQUEST_TIMEOUT_MILLISEC = "request.timeout.ms";
		

	//Testing parameter
	public static final String WRITE_OUTPUT = "write_output";
	public static final String WRITE_PATH = "write_path";
	public static final String VEHICLE_DATA_PATH = "vehicleDataPath";
	public static final String UNKNOWN = "UNKNOWN";
	
	//Diesel parameters
	public static final String DIESEL_HEATING_VALUE = "diesel.heating.value";
	public static final String DIESEL_CO2_EMISSION_FACTOR = "diesel.co2.emission.factor";
	public static final String DIESEL_WEIGHT_KG = "diesel.weight.kg";
	
	//Gross Weight Threshold Parameters
	public static final String VEHICLE_GROSS_WEIGHT_THRESHOLD = "vehicle.gross.weight.threshold";
	public static final Integer ZERO = 0;
	public static final Long ZERO_VAL = 0L;
	public static final Integer ONE = 1;
}
