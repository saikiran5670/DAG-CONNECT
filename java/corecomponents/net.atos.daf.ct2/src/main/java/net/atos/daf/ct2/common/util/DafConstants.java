package net.atos.daf.ct2.common.util;

public class DafConstants {
	public static final String HBASE_ZOOKEEPER_QUORUM = "hbase.zookeeper.quorum";
	public static final String HBASE_ZOOKEEPER_PROPERTY_CLIENTPORT = "hbase.zookeeper.property.clientPort";
	public static final String ZOOKEEPER_ZNODE_PARENT = "zookeeper.znode.parent";
	
	public static final String HBASE_REGIONSERVER_PORT="hbase.regionserver.port";
	public static final String HBASE_REGIONSERVER = "hbase.regionserver";
	public static final String HBASE_MASTER="hbase.master";
	
	public static final String HBASE_TABLE_NAME="hbase.table.name";
	public static final String HBASE_STATUS_TABLE_NAME="VehicleStatusMSG";
	public static final String HBASE_MONITORING_TABLE_NAME = "VehicleMonitoringMSG";
	
	public static final String BOOTSTRAP_SERVERS="bootstrap.servers";
	
	public static final String WN0_KAFKA_INTERNAL_9092 = "wn0-kafka.c0kbh01dsx0uhhbdv5oig221gb.ax.internal.cloudapp.net:9092";
	public static final String ZOOKEEPER_CONNECT= "zookeeper.connect";
	public static final String WN0_KAFKA_INTERNAL_2181 = "wn0-kafka.c0kbh01dsx0uhhbdv5oig221gb.ax.internal.cloudapp.net:2181";
	public static final String GROUP_ID="group.id";
	
	public static final String INDEX_TOPIC="Index_Topic";
	public static final String MONITORING_TOPIC="Monitoring_Topic";
	public static final String STATUS_TOPIC = "Status_Topic";
	
	public static final String EVENT_HUB_CONFIG="event.hub.config";
	public static final String EVENT_HUB_BOOTSTRAP="event.hub.bootstrap";
	
	//event.hub.config
	
	//Postgre Sql
			public static final String POSTGRE_SQL_PASSWORD = "postgresql_password";
			public static final String POSTGRE_SQL_DRIVER = "postgresql_driver";
			public static final String POSTGRE_SQL_URL = "postgresql_url";
			public static final String POSTGRE_SQL_SSL_MODE = "&sslmode=require";
			

			public static final String GRPC_SERVER ="grpc_server";   //"52.236.153.224";
			public static final String GRPC_PORT ="grpc_port" ; //"80"; 
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
			
			public static final String TRIP_JOB_NAME ="RealtimeMSGProcess";
			public static final String DEFAULT_OBJECT_ID ="00"; 			 

	
}
