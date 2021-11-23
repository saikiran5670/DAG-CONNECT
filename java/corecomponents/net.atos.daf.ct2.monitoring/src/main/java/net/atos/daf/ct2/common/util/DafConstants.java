package net.atos.daf.ct2.common.util;

public class DafConstants {
	// Streaming environment
	public static final String PARALLELISM = "parallelism";
	public static final String CHECKPOINT_INTERVAL = "checkpoint_interval";
	public static final String MINIMUM_PAUSE_BETWEEN_CHECKPOINTS = "min_checkpoint_pause";
	public static final String CHECKPOINT_TIMEOUT = "checkpoint_timeout";
	public static final String MAX_CONCURRENT_CHECKPOINTS = "max_concurrent_checkpoints";
	public static final String RESTART_FLAG = "restart.flag";
	

	public static final String CHECKPOINT_DIRECTORY_INDEX = "checkpoint_directory_index";
	public static final String CHECKPOINT_DIRECTORY_STATUS = "checkpoint_directory_status";
	public static final String CHECKPOINT_DIRECTORY_MONITORING = "checkpoint_directory_monitor";

	public static final String RESTART_ATTEMPS = "restart_attempts";
	public static final String RESTART_INTERVAL = "restart_interval";
	
	public static final String FIXED_RESTART_FLAG = "fixed.restart.flag";
	public static final String RESTART_FAILURE_RATE = "restart.failure.rate";
	public static final String RESTART_FAILURE_INTERVAL = "restart.failure.interval";
	public static final String RESTART_FAILURE_DELAY = "restart.failure.delay";
	

	public static final String HBASE_ZOOKEEPER_QUORUM = "hbase.zookeeper.quorum";
	public static final String HBASE_ZOOKEEPER_PROPERTY_CLIENTPORT = "hbase.zookeeper.property.clientPort";
	public static final String ZOOKEEPER_ZNODE_PARENT = "zookeeper.znode.parent";

	public static final String HBASE_REGIONSERVER_PORT = "hbase.regionserver.port";
	public static final String HBASE_REGIONSERVER = "hbase.regionserver";
	public static final String HBASE_MASTER = "hbase.master";

	public static final String HBASE_TABLE_NAME = "hbase.table.name";

	public static final String BOOTSTRAP_SERVERS = "bootstrap.servers";
	public static final String ZOOKEEPER_CONNECT = "zookeeper.connect";
	public static final String GROUP_ID = "group.id";

	public static final String INDEX_TOPIC = "Index_Topic";
	public static final String MONITORING_TOPIC = "Monitoring_Topic";
	public static final String STATUS_TOPIC = "Status_Topic";

	public static final String EVENT_HUB_CONFIG = "event.hub.config";
	public static final String EVENT_HUB_BOOTSTRAP = "event.hub.bootstrap";
	public static final String EVENT_HUB_GROUPID = "event.hub.group.id";
	public static final String EVENT_HUB_CLIENTID = "event.hub.client.id";

	// event.hub.config

	// Postgre Sql
	public static final String POSTGRE_SQL_PASSWORD = "postgresql_password";
	public static final String POSTGRE_SQL_DRIVER = "postgresql_driver";
	public static final String POSTGRE_SQL_URL = "postgresql_url";
	public static final String POSTGRE_SQL_SSL_MODE = "&sslmode=require";

	public static final String DATAMART_POSTGRE_SERVER_NAME = "server_name";
	public static final String DATAMART_POSTGRE_PORT = "port";
	public static final String DATAMART_POSTGRE_DATABASE_NAME = "postgres_database_name";
	public static final String DATAMART_POSTGRE_USER = "userId";
	public static final String DATAMART_POSTGRE_PASSWORD = "postgresql_password";

	public static final String GRPC_SERVER = "grpc_server"; // "52.236.153.224";
	public static final String GRPC_PORT = "grpc_port"; // "80";
	// Audit Constants
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

	public static final String TRIP_JOB_NAME = "RealtimeMSGProcess";
	public static final String DEFAULT_OBJECT_ID = "00";
	
	//job constants 
	public static final String INDEX_JOB = "IndexJob";
	public static final String MONITOR_JOB = "MonitorJob";
	public static final String STATUS_JOB = "StatusJob";
	
	public static final String Monitor="M";
	public static final String MONITOR_TOPIC_NAME = "egress.conti.monitordata.object";
	//queries
		public static final String QUERY_DRIVER_ACTIVITY = "driver.activity.query";
		public static final String QUERY_DRIVER_ACTIVITY_READ = "driver.activity.read.query";
		public static final String QUERY_LIVEFLEET_POSITION = "livefleet.position.query";
	
	public static final int FUEL_CONSUMPTION_INDICATOR = 2;		
    //KAFKA CONFIG
    public static final String AUTO_OFFSET_RESET_CONFIG = "auto.offset.reset";
    public static final String BROADCAST_NAME = "broadcast.name";
	
	//master datamart
		public static final String MASTER_POSTGRE_SERVER_NAME = "master_postgre_server_name";
		public static final String MASTER_POSTGRE_PORT = "master_postgre_port";
		public static final String MASTER_POSTGRE_DATABASE_NAME = "master_postgre_database_name";
		public static final String MASTER_POSTGRE_USER = "master_postgre_userId";
		public static final String MASTER_POSTGRE_PASSWORD = "master_postgre_password";
		
		
		public static final String DTM_TS_FORMAT = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'";	
		
		public static final String AUTO_OFFSET_RESET ="auto.offset.reset";
	public static final String INCOMING_MESSAGE_UUID = "message uuid : %s";
	public static final String DRIVER_MANAGEMENT_COUNT_WINDOW= "driver.management.count.window";
	
	public static final String STORE_HISTORICAL_DATA="store.historical.data";
	public static final String MONITOR_PROCESS="monitor.process";
	public static final String MONITOR_HBASE_PROCESS="monitor.hbase.process";
	
	
	public static final String LIVEFLEET_WARNING_INSERT = "INSERT INTO livefleet.livefleet_warning_statistics(trip_id , vin   , warning_time_stamp,	warning_class,	warning_number,	latitude,	longitude,	heading,	vehicle_health_status_type,	vehicle_driving_status_type,	driver1_id,	warning_type,	distance_until_next_service,	odometer_val,	lastest_processed_message_time_stamp,	created_at, modified_at,	message_type) VALUES ( ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
	public static final String LIVEFLEET_CURRENT_TRIP_STATISTICS_UPDATE_TEN = "UPDATE livefleet.livefleet_current_trip_statistics  SET latest_received_position_lattitude = ? , latest_received_position_longitude = ? , latest_received_position_heading = ? , latest_processed_message_time_stamp = ? , vehicle_health_status_type = ? , latest_warning_class = ? ,latest_warning_number = ? , latest_warning_type = ? , latest_warning_timestamp = ? , latest_warning_position_latitude = ? , latest_warning_position_longitude = ? WHERE trip_id = ( SELECT trip_id FROM livefleet.livefleet_current_trip_statistics WHERE vin = ? ORDER BY id DESC LIMIT 1 )";

	public static final String REPAITM_MAINTENANCE_WARNING_READ = "select warning_type from livefleet.livefleet_warning_statistics where message_type=? and vin = ? and warning_class = ? and warning_number= ? AND vin IS NOT NULL order by warning_time_stamp DESC limit 1";

	//public static final String LIVEFLEET_WARNING_READLIST = "select id, warning_class,	warning_number, vin from livefleet.livefleet_warning_statistics where vin = ? AND  message_type=10 and warning_type='A'  order by id DESC";
	public static final String LIVEFLEET_WARNING_READLIST = "select id, warning_class, warning_number, vin from livefleet.livefleet_warning_statistics ws1 where vin = ? AND message_type=10 and warning_type='A' and not exists (select 1 from livefleet.livefleet_warning_statistics ws2 where ws2.vin=ws1.vin and ws2.message_type=ws1.message_type and ws2.warning_type='D' and ws2.warning_class=ws1.warning_class and ws2.warning_number=ws1.warning_number and ws2.warning_time_stamp>=ws1.warning_time_stamp) order by id DESC";
	public static final String LIVEFLEET_WARNING_UPDATELIST = "UPDATE livefleet.livefleet_warning_statistics set warning_type='D' where id = ANY (?)";
	public static final String LIVEFLEET_DRIVER_INSERT = "INSERT INTO livefleet.livefleet_trip_driver_activity  (trip_id    , trip_start_time_stamp , trip_end_time_stamp   , activity_date,  vin   , driver_id     , code  , start_time    , end_time      , duration      , created_at_m2m        , created_at_kafka      , created_at_dm , modified_at   , last_processed_message_time_stamp ,is_driver1, logical_code    ) VALUES ( ?, ?, ?, ?   , ?,?, ?, ?, ?, ?       , ?     , ?     , ?     , ? ,?    ,?, ?)";
	
	///update query
	public static final String LIVEFLEET_WARNING_DEACTIVATE="UPDATE livefleet.livefleet_warning_statistics set warning_type='D' where vin = ? and warning_class=? and warning_number=? and warning_type='A'";
	
		
		
	

}
