package net.atos.daf.ct2.constant;

public class DAFCT2Constant {

    public static final String GRPC_SERVER = "grpc.server";
    public static final String GRPC_PORT = "grpc.port";
    public static final String JOB_NAME = "BoschstreamingJob";

    public static final String SOURCE_TOPIC_NAME = "source.topic.name";
    public static final String SOURCE_BOSCH_TOPIC_NAME = "source.bosch.topic.name";
    public static final String SINK_INDEX_TOPIC_NAME = "sink.index.topic.name";
    public static final String SINK_STATUS_TOPIC_NAME = "sink.status.topic.name";
    public static final String SINK_MONITOR_TOPIC_NAME = "sink.monitor.topic.name";
    public static final String MASTER_DATA_TOPIC_NAME = "master.data.topic.name";
    public static final String CONTI_CORRUPT_MESSAGE_TOPIC_NAME = "conti.corrupt.message.topic.name";
    public static final String BOSCH_CORRUPT_MESSAGE_TOPIC_NAME = "bosch.corrupt.message.topic.name";
    public static final String SINK_TCU_TOPIC_NAME = "egress.bosch.tcu.topic.name";
    

    public static final String INDEX_TRANSID = "index.transid";
    public static final String STATUS_TRANSID = "status.transid";
    public static final String MONITOR_TRANSID = "monitor.transid";
    
    public static final String CONTI_INDEX_TRANSID = "conti.index.transid";
	public static final String CONTI_MONITOR_TRANSID = "conti.monitor.transid";
	public static final String CONTI_STATUS_TRANSID = "conti.status.transid";


    public static final String BROADCAST_NAME = "broadcast.name";

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
    
    public static final String HBASE_BOSCH_HISTORICAL_TABLE_NAME = "bosch.historical.table.name";
    public static final String HBASE_BOSCH_HISTORICAL_TABLE_CF = "bosch.historical.table.colfm";
    public static final String UNKNOWN = "UNKNOWN";
    public static final Integer MEASUREMENT_DATA = 1;
    public static final Integer TCU_DATA = 2;
    public static final Integer UNKNOWN_DATA = 0;
    
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
	
	public static final String FILTER_INDEX_TRANSID = "filter.index.transid";
	public static final String FILTER_MONITOR_TRANSID = "filter.monitor.transid";
	public static final String FILTER_STATUS_TRANSID = "filter.status.transid";
	public static final String INDEXKEY = "INDEXKEY";
	public static final String MONITORKEY = "MONITORKEY";
	public static final String STATUSKEY = "STATUSKEY";
	public static final String DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'";
}
