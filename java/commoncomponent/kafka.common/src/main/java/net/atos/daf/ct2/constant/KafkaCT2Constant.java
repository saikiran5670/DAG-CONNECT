package net.atos.daf.ct2.constant;

public class KafkaCT2Constant {

	//HBASE
	public static final String HBASE_HISTORICAL_TABLE_CF = "rawcf";
	public static final String HBASE_HISTORICAL_TABLE_COLNM = "msg";
	
	//GRPC
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
   public static final String DEFAULT_OBJECT_ID = "00";
	
}
