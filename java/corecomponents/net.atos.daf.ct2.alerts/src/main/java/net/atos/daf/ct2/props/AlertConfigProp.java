package net.atos.daf.ct2.props;


import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.Payload;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.typeinfo.BasicTypeInfo;
import org.apache.flink.api.common.typeinfo.TypeInformation;
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

    public static final String ALERT_MAP_FETCH_QUERY="postgres.alert.map.fetch.query";
    public static final String ALERT_THRESHOLD_FETCH_QUERY="postgres.alert.threshold.fetch.query";
    public static final String ALERT_THRESHOLD_FETCH_SINGLE_QUERY="postgres.threshold.fetch.query";

    public static final String POSTGRESS_SINGLE_VEHICLE_QUERY="postgres.single.vehicle.fetch.query";
    public static final String POSTGRESS_GROUP_VEHICLE_QUERY="postgres.group.vehicle.fetch.query";


    public static final TypeInformation<?>[] ALERT_THRESHOLD_SCHEMA_DEF = new TypeInformation<?>[] {
            BasicTypeInfo.INT_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO,
            BasicTypeInfo.BIG_DEC_TYPE_INFO,
            BasicTypeInfo.STRING_TYPE_INFO
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

    public  static final MapStateDescriptor<Long, Payload> thresholdMapStateDescriptor = new MapStateDescriptor("thresholdMapStateDescriptor", Long.class,Payload.class);
    public  static final MapStateDescriptor<String, Payload> vinAlertMapStateDescriptor = new MapStateDescriptor("vinAlertMapStateDescriptor", String.class,Payload.class);
    public static final OutputTag<Alert> OUTPUT_TAG = new OutputTag<Alert>("side-output") {};
}
