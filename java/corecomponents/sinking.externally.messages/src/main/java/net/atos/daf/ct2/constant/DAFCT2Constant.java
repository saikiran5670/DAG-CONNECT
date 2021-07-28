package net.atos.daf.ct2.constant;

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
    public static final String TRIP_JOB_NAME = "sinkingExternalMessages";
    public static final String DEFAULT_OBJECT_ID = "00";

    public static final String GRPC_SERVER = "grpc.server";
    public static final String GRPC_PORT = "grpc.port";

    public static final String SOURCE_TOPIC_NAME = "source.topic.name";
    public static final String SINK_INDEX_TOPIC_NAME = "sink.index.topic.name";
    public static final String SINK_STATUS_TOPIC_NAME = "sink.status.topic.name";
    public static final String SINK_MONITOR_TOPIC_NAME = "sink.monitor.topic.name";
    public static final String MASTER_DATA_TOPIC_NAME = "master.data.topic.name";

    public static final String INDEX_TRANSID = "index.transid";
    public static final String STATUS_TRANSID = "status.transid";
    public static final String MONITOR_TRANSID = "monitor.transid";

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
    
    //CTS-016 - Egress Source Raw Data
    public static final String EGRESS_CONTI_DATA = "egress.conti.data";
    public static final String EGRESS_BOSCH_DATA = "egress.bosch.data";
    public static final String EGRESS_ALL_SOURCE_DATA = "egress.all.source.data";
    public static final String EGRESS_DAF_STANDAR_DATA = "egress.daf.standard.data";
    public static final String EGRESS_DATA_FOR_SOURCE_SYSTEM = "egress.data.for.source.system";
    public static final String CONTI_SOURCE_SYSTEM="conti.source.system";
    public static final String BOSCH_SOURCE_SYSTEM="bosch.source.system";
    public static final String ALL_SOURCE_SYSTEM="all.source.system";
    public static final String DAF_STANDARD_SYSTEM="daf.standard.system";
    
    //Bosch
    public static final String SOURCE_BOSCH_TOPIC_NAME = "source.bosch.topic.name";
    
    //KAFKA CONFIG
    public static final String AUTO_OFFSET_RESET_CONFIG = "auto.offset.reset";
    
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
  		

}
