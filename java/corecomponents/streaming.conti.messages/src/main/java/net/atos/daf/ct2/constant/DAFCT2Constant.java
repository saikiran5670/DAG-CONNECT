package net.atos.daf.ct2.constant;

import org.apache.flink.api.common.typeinfo.BasicTypeInfo;
import org.apache.flink.api.common.typeinfo.TypeInformation;

public class DAFCT2Constant {

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
    public static final String JOB_NAME = "ContiStreamingJob";
    public static final String DEFAULT_OBJECT_ID = "00";

    public static final String GRPC_SERVER = "grpc.server";
    public static final String GRPC_PORT = "grpc.port";

    public static final String SOURCE_TOPIC_NAME = "source.topic.name";
    public static final String SINK_INDEX_TOPIC_NAME = "sink.index.topic.name";
    public static final String SINK_STATUS_TOPIC_NAME = "sink.status.topic.name";
    public static final String SINK_MONITOR_TOPIC_NAME = "sink.monitor.topic.name";
    public static final String MASTER_DATA_TOPIC_NAME = "master.data.topic.name";
    public static final String CONTI_CORRUPT_MESSAGE_TOPIC_NAME = "conti.corrupt.message.topic.name";
    public static final Integer MEASUREMENT_DATA = 1;
    

    public static final String INDEX_TRANSID = "index.transid";
    public static final String STATUS_TRANSID = "status.transid";
    public static final String MONITOR_TRANSID = "monitor.transid";

    public static final String BROADCAST_NAME = "broadcast.name";
    public static final String UNKNOWN = "UNKNOWN";

    public static final String SINK_JSON_STRING_TOPIC_NAME = "sink.external.topic.name";

    public static final String POSTGRE_CONNECTOR_CLASS = "postgre.connector.class";
    public static final String POSTGRE_OFFSET_STORAGE_FILE_FILENAME =
            "postgre.offset.storage.filename";
    public static final String POSTGRE_OFFSET_FLUSH_INTERVAL_MS = "postgre.offset.flush.interval";
    public static final String POSTGRE_SERVER_NAME = "postgre.server.name";
    public static final String POSTGRE_SERVER_ID = "postgre.server.id";
    public static final String POSTGRE_HOSTNAME = "postgre.host.name";
    public static final String POSTGRE_PORT = "postgre.port";
    public static final String POSTGRE_DRIVER = "postgre.driver";
    public static final String POSTGRE_USER = "postgre.user.name";
    public static final String POSTGRE_PASSWORD = "postgre.password";
    public static final String POSTGRE_SSL = "postgre.ssl";
    public static final String POSTGRE_DATABASE_NAME = "postgre.database.name";
    public static final String POSTGRE_TABLE_WHITELIST = "postgre.table.name";
    public static final String POSTGRE_PLUGIN_NAME = "postgre.plugin.name";
    public static final String POSTGRE_CDC_FETCH_DATA_QUERY = "postgres.cdc.fetch.data.query";

    public static final String HBASE_CONTI_HISTORICAL_TABLE_NAME = "conti.historical.table.name";
    public static final String HBASE_CONTI_HISTORICAL_TABLE_CF = "conti.historical.table.colfm";
        
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
	
	//Vehicle Health
    public static final String CONNECTED_OTA_OFF="C";
    public static final String CONNECTED_OTA_ON="N";

    //KAFKA CONFIG
    public static final String AUTO_OFFSET_RESET_CONFIG = "auto.offset.reset";

    public static final TypeInformation<?>[] VEHICLE_STATUS_SCHEMA_DEF = new TypeInformation<?>[] {
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO
    };
}
