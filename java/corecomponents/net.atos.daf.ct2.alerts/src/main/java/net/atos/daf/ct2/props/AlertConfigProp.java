package net.atos.daf.ct2.props;


import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.typeinfo.BasicArrayTypeInfo;
import org.apache.flink.api.common.typeinfo.BasicTypeInfo;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.table.planner.expressions.In;
import org.apache.flink.util.OutputTag;

public class AlertConfigProp {

    public static final String DRIVER                   = "driver.class.name";
    public static final String DATAMART_POSTGRES_HOST   = "datamart.host.name";
    public static final String DATAMART_POSTGRES_PORT   = "datamart.port";
    public static final String DATAMART_DATABASE        = "datamart.database";
    public static final String DATAMART_USERNAME        = "datamart.username";
    public static final String DATAMART_PASSWORD        = "datamart.password";
    public static final String DATAMART_POSTGRES_SSL    = "datamart.ssl";

    public static final String MASTER_POSTGRES_HOST   = "master.host.name";
    public static final String MASTER_POSTGRES_PORT   = "master.port";
    public static final String MASTER_DATABASE        = "master.database";
    public static final String MASTER_USERNAME        = "master.username";
    public static final String MASTER_PASSWORD        = "master.password";
    public static final String MASTER_POSTGRES_SSL    = "master.ssl";


    public static final String KAFKA_GRP_ID = "group.id";
    public static final String KAFKA_BOOTSTRAP_SERVER = "bootstrap.servers";
    public static final String KAFKA_AUTO_OFFSET = "auto.offset.reset";
    public static final String KAFKA_DAF_ALERT_CDC_TOPIC = "daf.alert.cdc.topic";
    public static final String KAFKA_DAF_STATUS_MSG_TOPIC = "daf.status.topic";
    public static final String KAFKA_DAF_ALERT_PRODUCE_MSG_TOPIC = "daf.alert.produce.topic";
    public static final String KAFKA_EGRESS_INDEX_MSG_TOPIC = "daf.index.topic";
    
    public static final String KAFKA_EGRESS_MONITERING_MSG_TOPIC = "daf.monitering.topic";
    public static final String KAFKA_MONITERING_BOOTSTRAP_SERVER = "monitering.object.bootstrap.servers";
    public static final String KAFKA_MONITERING_SASL_JAAS_CONFIG = "monitering.object.sasl.jaas.config";
    
    public static final String KAFKA_INDEX_BOOTSTRAP_SERVER = "index.object.bootstrap.servers";
    public static final String KAFKA_INDEX_SASL_JAAS_CONFIG = "index.object.sasl.jaas.config";

    public static final String ALERT_MAP_FETCH_QUERY="postgres.alert.map.fetch.query";
    public static final String ALERT_THRESHOLD_FETCH_QUERY="postgres.alert.threshold.fetch.query";
    public static final String ALERT_THRESHOLD_FETCH_SINGLE_QUERY="postgres.threshold.fetch.query";

    public static final String POSTGRESS_SINGLE_VEHICLE_QUERY="postgres.single.vehicle.fetch.query";
    public static final String POSTGRESS_GROUP_VEHICLE_QUERY="postgres.group.vehicle.fetch.query";
    
	//master datamart
	public static final String MASTER_POSTGRE_SERVER_NAME = "master_postgre_server_name";
	public static final String MASTER_POSTGRE_PORT = "master_postgre_port";
	public static final String MASTER_POSTGRE_DATABASE_NAME = "master_postgre_database_name";
	public static final String MASTER_POSTGRE_USER = "master_postgre_userId";
	public static final String MASTER_POSTGRE_PASSWORD = "master_postgre_password";
	
	public static final String DATAMART_POSTGRE_SERVER_NAME = "server_name";
	public static final String DATAMART_POSTGRE_PORT = "port";
	public static final String DATAMART_POSTGRE_DATABASE_NAME = "postgres_database_name";
	public static final String DATAMART_POSTGRE_USER = "userId";
	public static final String DATAMART_POSTGRE_PASSWORD = "postgresql_password";
	
	public static final String INCOMING_MESSAGE_UUID = "message uuid : %s";


    public static final TypeInformation<?>[] ALERT_THRESHOLD_SCHEMA_DEF = new TypeInformation<?>[] {
            BasicTypeInfo.INT_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.BIG_DEC_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.LONG_TYPE_INFO,
            BasicTypeInfo.LONG_TYPE_INFO,
			BasicTypeInfo.INT_TYPE_INFO,
            BasicTypeInfo.BIG_DEC_TYPE_INFO,
            BasicTypeInfo.BIG_DEC_TYPE_INFO,
            BasicTypeInfo.INT_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
    };

    public static final TypeInformation<?>[] ALERT_MAP_SCHEMA_DEF = new TypeInformation<?>[] {
            BasicTypeInfo.INT_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.INT_TYPE_INFO};

    public static final TypeInformation<?>[] ALERT_DB_VIN_THRESHOLD_SCHEMA_DEF = new TypeInformation<?>[] {
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.INT_TYPE_INFO,
            BasicTypeInfo.INT_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO
    };




    public  static final MapStateDescriptor<Long, Payload> THRESHOLD_CONFIG_DESCRIPTOR = new MapStateDescriptor("thresholdMapStateDescriptor", Long.class,Payload.class);
    public  static final MapStateDescriptor<String, Payload> vinAlertMapStateDescriptor = new MapStateDescriptor("vinAlertMapStateDescriptor", String.class,Payload.class);
    public  static final MapStateDescriptor<String, Payload> VIN_ALERT_MAP_STATE = new MapStateDescriptor("vinAlertMappingState",String.class,Payload.class);
    public  static final OutputTag<Alert> OUTPUT_TAG = new OutputTag<Alert>("side-output") {};
    public static BroadcastStream<VehicleAlertRefSchema> vehicleAlertRefSchemaBroadcastStream;
    public static BroadcastStream<Payload<Object>> alertUrgencyLevelRefSchemaBroadcastStream;
	public static final String ALERT_TIME_WINDOW_SECONDS = "alert.time.window.seconds";
	public static final String ALERT_WATERMARK_TIME_WINDOW_SECONDS = "alert.watermark.time.window.seconds";
	public static final String ALERT_MEASUREMENT_MILLISECONDS_VAL = "alert.time.measurement.milli";
		
	public static final Integer INDEX_TRIP_START = 4;
	public static final Integer INDEX_TRIP_END = 5;
	public static final String DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'";
}
