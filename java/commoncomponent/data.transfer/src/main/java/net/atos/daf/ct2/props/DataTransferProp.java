package net.atos.daf.ct2.props;

public class DataTransferProp {

    public static final String KAFKA_MONITOR_BOOTSTRAP_SERVER_SOURCE = "source.monitor.object.bootstrap.servers";
    public static final String KAFKA_MONITOR_JAAS_CONFIG_SOURCE = "source.monitor.object.sasl.jaas.config";
    public static final String KAFKA_MONITOR_TOPIC_SOURCE = "source.monitor.topic";

    public static final String KAFKA_MONITOR_BOOTSTRAP_SERVER_DESTINATION = "destination.monitor.object.bootstrap.servers";
    public static final String KAFKA_MONITOR_JAAS_CONFIG_DESTINATION = "destination.monitor.object.sasl.jaas.config";
    public static final String KAFKA_MONITOR_TOPIC_DESTINATION = "destination.monitor.topic";

    /**
     * Status topic prop
     */
    public static final String KAFKA_STATUS_BOOTSTRAP_SERVER_SOURCE = "source.status.object.bootstrap.servers";
    public static final String KAFKA_STATUS_JAAS_CONFIG_SOURCE = "source.status.object.sasl.jaas.config";
    public static final String KAFKA_STATUS_TOPIC_SOURCE = "source.status.topic";
    public static final String KAFKA_STATUS_BOOTSTRAP_SERVER_DESTINATION = "destination.status.object.bootstrap.servers";
    public static final String KAFKA_STATUS_JAAS_CONFIG_DESTINATION = "destination.status.object.sasl.jaas.config";
    public static final String KAFKA_STATUS_TOPIC_DESTINATION = "destination.status.topic";

    /**
     * INDEX topic prop
     */
    public static final String KAFKA_INDEX_BOOTSTRAP_SERVER_SOURCE = "source.index.object.bootstrap.servers";
    public static final String KAFKA_INDEX_JAAS_CONFIG_SOURCE = "source.index.object.sasl.jaas.config";
    public static final String KAFKA_INDEX_TOPIC_SOURCE = "source.index.topic";
    public static final String KAFKA_INDEX_BOOTSTRAP_SERVER_DESTINATION = "destination.index.object.bootstrap.servers";
    public static final String KAFKA_INDEX_JAAS_CONFIG_DESTINATION = "destination.index.object.sasl.jaas.config";
    public static final String KAFKA_INDEX_TOPIC_DESTINATION = "destination.index.topic";


    public static final String MONITOR_DATA_TRANSFER_TRIGGER = "monitor.transfer.data";
    public static final String STATUS_DATA_TRANSFER_TRIGGER = "status.transfer.data";
    public static final String INDEX_DATA_TRANSFER_TRIGGER = "index.transfer.data";
}

