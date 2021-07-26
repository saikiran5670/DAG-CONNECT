package net.atos.daf.ct2.main;

import net.atos.daf.ct2.exception.DAFCT2Exception;
import net.atos.daf.ct2.models.scheamas.CdcPayloadWrapper;
import net.atos.daf.ct2.models.scheamas.VehicleStatusSchema;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.Message;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.processing.BroadcastState;
import net.atos.daf.ct2.processing.ConsumeSourceStream;
import net.atos.daf.ct2.processing.EgressCorruptMessages;
import net.atos.daf.ct2.processing.KafkaAuditService;
import net.atos.daf.ct2.processing.MessageProcessing;
import net.atos.daf.ct2.processing.ValidateSourceStream;
import net.atos.daf.ct2.util.Utils;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.io.jdbc.JDBCInputFormat;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.typeutils.RowTypeInfo;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.kafka.clients.consumer.ConsumerConfig;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.FileReader;
import java.io.IOException;
import java.io.Serializable;
import java.util.Objects;
import java.util.Properties;

import static net.atos.daf.ct2.constant.DAFCT2Constant.AUTO_OFFSET_RESET_CONFIG;
import static net.atos.daf.ct2.constant.DAFCT2Constant.BROADCAST_NAME;
import static net.atos.daf.ct2.constant.DAFCT2Constant.CONTI_CORRUPT_MESSAGE_TOPIC_NAME;
import static net.atos.daf.ct2.constant.DAFCT2Constant.GRPC_PORT;
import static net.atos.daf.ct2.constant.DAFCT2Constant.GRPC_SERVER;
import static net.atos.daf.ct2.constant.DAFCT2Constant.INDEX_TRANSID;
import static net.atos.daf.ct2.constant.DAFCT2Constant.JOB_NAME;
import static net.atos.daf.ct2.constant.DAFCT2Constant.MASTER_DATA_TOPIC_NAME;
import static net.atos.daf.ct2.constant.DAFCT2Constant.MEASUREMENT_DATA;
import static net.atos.daf.ct2.constant.DAFCT2Constant.MONITOR_TRANSID;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_CDC_FETCH_DATA_QUERY;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_DATABASE_NAME;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_DRIVER;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_HOSTNAME;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_PASSWORD;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_PORT;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_USER;
import static net.atos.daf.ct2.constant.DAFCT2Constant.SINK_INDEX_TOPIC_NAME;
import static net.atos.daf.ct2.constant.DAFCT2Constant.SINK_MONITOR_TOPIC_NAME;
import static net.atos.daf.ct2.constant.DAFCT2Constant.SINK_STATUS_TOPIC_NAME;
import static net.atos.daf.ct2.constant.DAFCT2Constant.SOURCE_TOPIC_NAME;
import static net.atos.daf.ct2.constant.DAFCT2Constant.STATUS_TRANSID;
import static net.atos.daf.ct2.constant.DAFCT2Constant.VEHICLE_STATUS_SCHEMA_DEF;

public class ContiMessageProcessing implements Serializable {

    private static final Logger logger = LoggerFactory.getLogger(ContiMessageProcessing.class);
    public static String FILE_PATH;
    private StreamExecutionEnvironment streamExecutionEnvironment;
    private static final long serialVersionUID = 1L;

    public static Properties configuration() throws DAFCT2Exception {

        Properties properties = new Properties();
        try {
            properties.load(new FileReader(FILE_PATH));
            properties.put(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, properties.getProperty(AUTO_OFFSET_RESET_CONFIG));
            logger.info("Configuration Loaded for Connecting Kafka inorder to Perform Mapping.");
        } catch (IOException e) {
            logger.error("Unable to Find the File " + FILE_PATH, e);
            throw new DAFCT2Exception("Unable to Find the File " + FILE_PATH, e);
        }
        return properties;
    }

    public static void main(String[] args) {
        ContiMessageProcessing contiMessageProcessing = new ContiMessageProcessing();
        Properties properties = null;
        try {
            FILE_PATH = args[0];

            properties = configuration();
            contiMessageProcessing.auditContiJobDetails(properties, "Conti streaming job started");

            contiMessageProcessing.flinkConnection();
            contiMessageProcessing.processing(properties);
            contiMessageProcessing.startExecution();

        } catch (DAFCT2Exception e) {
            logger.error("Exception: {}", e);
            contiMessageProcessing.auditContiJobDetails(properties, "Conti streaming job failed :: " + e.getMessage());
        } finally {
            //       auditETLJobClient.closeChannel();
        }
    }

    public void flinkConnection() {

        this.streamExecutionEnvironment = StreamExecutionEnvironment.getExecutionEnvironment();
        logger.info("Flink Processing Started.");
    }

    public void processing(Properties properties) {

        ConsumeSourceStream consumeSrcStream = new ConsumeSourceStream();
        ValidateSourceStream validateSourceStream = new ValidateSourceStream();

        MapStateDescriptor<Message<String>, KafkaRecord<VehicleStatusSchema>> mapStateDescriptor =
                new BroadcastState<String, VehicleStatusSchema>()
                        .stateInitialization(properties.getProperty(BROADCAST_NAME));

        SingleOutputStreamOperator<VehicleStatusSchema> kafkaCDCMessage = consumeSrcStream.consumeSourceInputStream(
                streamExecutionEnvironment, MASTER_DATA_TOPIC_NAME, properties)
                .map(json -> {
                    try{
                        CdcPayloadWrapper wrapper  = (CdcPayloadWrapper)Utils.readValueAsObject(json.getValue(), CdcPayloadWrapper.class);
                        if(Objects.nonNull(wrapper.getNamespace()) && wrapper.getNamespace().equalsIgnoreCase("vehicleManagement")){
                            VehicleStatusSchema schema =  (VehicleStatusSchema) Utils.readValueAsObject(wrapper.getPayload(), VehicleStatusSchema.class);
                            schema.setOperationType(wrapper.getOperation());
                            schema.setNamespace(wrapper.getNamespace());
                            logger.info("Message from kafka cdc topic:: {}",json);
                            return schema;
                        }
                    }catch (Exception e){
                        logger.error("Un-parsable cdc event :: {} exception :: {}",json,e);
                    }
                    return VehicleStatusSchema.builder().build();

                })
                .returns(VehicleStatusSchema.class)
                .filter(schema -> Objects.nonNull(schema.getVid()))
                .returns(VehicleStatusSchema.class);


        /**
         * New code added for fetching status data from databases
         * Using jdbcInput format
         */
        RowTypeInfo rowTypeInfo = new RowTypeInfo(VEHICLE_STATUS_SCHEMA_DEF);

        String jdbcUrl = new StringBuilder("jdbc:postgresql://")
                .append(properties.getProperty(POSTGRE_HOSTNAME))
                .append(":" + properties.getProperty(POSTGRE_PORT) + "/")
                .append(properties.getProperty(POSTGRE_DATABASE_NAME))
                .append("?user=" + properties.getProperty(POSTGRE_USER))
                .append("&password=" + properties.getProperty(POSTGRE_PASSWORD))
                .append("&sslmode=require")
                .toString();

        JDBCInputFormat jdbcInputFormat = JDBCInputFormat
                .buildJDBCInputFormat()
                .setDrivername(properties.getProperty(POSTGRE_DRIVER))
                .setDBUrl(jdbcUrl)
                .setQuery(properties.getProperty(POSTGRE_CDC_FETCH_DATA_QUERY))
                .setRowTypeInfo(rowTypeInfo)
                .finish();

        logger.info("Connection done and data fetched using query:: {}",properties.getProperty(POSTGRE_CDC_FETCH_DATA_QUERY));

        SingleOutputStreamOperator<VehicleStatusSchema> dbVehicleStatusStream = streamExecutionEnvironment.createInput(jdbcInputFormat)
                .map(row -> VehicleStatusSchema.builder()
                        .vin(String.valueOf(row.getField(0)))
                        .vid(String.valueOf(row.getField(1)))
                        .status(String.valueOf(row.getField(2)))
                        .fuelType(String.valueOf(row.getField(3)))
                        .build())
                .returns(VehicleStatusSchema.class);


        DataStream<VehicleStatusSchema> statusSchemaDataStream = kafkaCDCMessage.union(dbVehicleStatusStream);

        BroadcastStream<KafkaRecord<VehicleStatusSchema>> broadcastStream = statusSchemaDataStream
                .<KafkaRecord<VehicleStatusSchema>>map(vehicleStatusSchema -> {
                    KafkaRecord<VehicleStatusSchema> kafkaRecord = new KafkaRecord<>();
                    kafkaRecord.setKey(vehicleStatusSchema.getVid());
                    kafkaRecord.setValue(vehicleStatusSchema);
                    return kafkaRecord;
                })
                .returns(TypeInformation.of(new TypeHint<KafkaRecord<VehicleStatusSchema>>() {
                    @Override
                    public TypeInformation<KafkaRecord<VehicleStatusSchema>> getTypeInfo() {
                        return super.getTypeInfo();
                    }
                }))
                .broadcast(mapStateDescriptor);


        DataStream<KafkaRecord<String>> contiInputStream = consumeSrcStream.consumeSourceInputStream(
                streamExecutionEnvironment, SOURCE_TOPIC_NAME, properties);


        new MessageProcessing<String, VehicleStatusSchema, String>()
                .contiMessageForHistorical(
                        contiInputStream,
                        properties,
                        broadcastStream);

        DataStream<Tuple2<Integer, KafkaRecord<String>>> contiStreamValiditySts = validateSourceStream
                .isValidJSON(contiInputStream);
        DataStream<KafkaRecord<String>> contiValidInputStream = validateSourceStream
                .getValidSourceMessages(contiStreamValiditySts, MEASUREMENT_DATA);

        new EgressCorruptMessages().egressCorruptMessages(contiStreamValiditySts, properties,
                properties.getProperty(CONTI_CORRUPT_MESSAGE_TOPIC_NAME));

        contiValidInputStream.print();

        new MessageProcessing<String, VehicleStatusSchema, Index>()
                .consumeContiMessage(
                        contiValidInputStream,
                        properties.getProperty(INDEX_TRANSID),
                        "Index",
                        properties.getProperty(SINK_INDEX_TOPIC_NAME),
                        properties,
                        Index.class,
                        broadcastStream);

        new MessageProcessing<String, VehicleStatusSchema, Status>()
                .consumeContiMessage(
                        contiValidInputStream,
                        properties.getProperty(STATUS_TRANSID),
                        "Status",
                        properties.getProperty(SINK_STATUS_TOPIC_NAME),
                        properties,
                        Status.class,
                        broadcastStream);

        new MessageProcessing<String, VehicleStatusSchema, Monitor>()
                .consumeContiMessage(
                        contiValidInputStream,
                        properties.getProperty(MONITOR_TRANSID),
                        "Monitor",
                        properties.getProperty(SINK_MONITOR_TOPIC_NAME),
                        properties,
                        Monitor.class,
                        broadcastStream);
    }

    public StreamExecutionEnvironment getstreamExecutionEnvironment() {
        return this.streamExecutionEnvironment;
    }

    public void startExecution() throws DAFCT2Exception {

        try {
            this.streamExecutionEnvironment.execute("Realtime Records");

        } catch (Exception e) {
            logger.error("Unable to process Message using Flink {}", e);
            throw new DAFCT2Exception("Unable to process Message using Flink ", e);
        }
    }

    public void auditContiJobDetails(Properties properties, String message) {
        try {
            new KafkaAuditService().auditTrail(
                    properties.getProperty(GRPC_SERVER),
                    properties.getProperty(GRPC_PORT),
                    JOB_NAME,
                    message);
        } catch (Exception e) {
            logger.error("Issue while auditing streaming conti job ");
        }
    }
}
