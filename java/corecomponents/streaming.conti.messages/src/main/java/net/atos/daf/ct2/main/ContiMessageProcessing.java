package net.atos.daf.ct2.main;

import net.atos.daf.common.AuditETLJobClient;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.exception.DAFCT2Exception;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.Message;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.processing.BroadcastState;
import net.atos.daf.ct2.processing.MessageProcessing;
import net.atos.daf.ct2.serde.KafkaMessageDeSerializeSchema;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.streaming.api.TimeCharacteristic;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.kafka.clients.consumer.ConsumerConfig;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.io.FileReader;
import java.io.IOException;
import java.util.HashMap;
import java.util.Map;
import java.util.Properties;

public class ContiMessageProcessing {

  private static final Logger log = LogManager.getLogger(ContiMessageProcessing.class);
  public static String FILE_PATH = "src/main/resources/configuration.properties";
  private static AuditETLJobClient auditETLJobClient;
  private StreamExecutionEnvironment streamExecutionEnvironment;

  public static Properties configuration() throws DAFCT2Exception {

    Properties properties = new Properties();
    properties.put(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, "earliest");

    try {
      properties.load(new FileReader(FILE_PATH));
      log.info("Configuration Loaded for Connecting Kafka inorder to Perform Mapping.");

    } catch (IOException e) {
      log.error("Unable to Find the File " + FILE_PATH, e);
      throw new DAFCT2Exception("Unable to Find the File " + FILE_PATH, e);
    }
    return properties;
  }

  public static void main(String[] args) {

    try {
      // FILE_PATH = args[0];

      ContiMessageProcessing contiMessageProcessing = new ContiMessageProcessing();
      Properties properties = configuration();
      auditTrail(properties);

      contiMessageProcessing.flinkConnection();

      contiMessageProcessing.processing(properties);
      contiMessageProcessing.startExecution();

    } catch (DAFCT2Exception e) {
      log.error("Exception: ", e);
      e.printStackTrace();

    } finally {
      //       auditETLJobClient.closeChannel();
    }
  }

  public static void auditTrail(Properties properties) {

    try {
      auditETLJobClient =
          new AuditETLJobClient(
              properties.getProperty(DAFCT2Constant.GRPC_SERVER),
              Integer.valueOf(properties.getProperty(DAFCT2Constant.GRPC_PORT)));
      Map<String, String> auditMap = new HashMap<String, String>();

      auditMap.put(DAFCT2Constant.JOB_EXEC_TIME, String.valueOf(TimeFormatter.getCurrentUTCTimeInSec()));
      auditMap.put(DAFCT2Constant.AUDIT_PERFORMED_BY, DAFCT2Constant.TRIP_JOB_NAME);
      auditMap.put(DAFCT2Constant.AUDIT_COMPONENT_NAME, DAFCT2Constant.TRIP_JOB_NAME);
      auditMap.put(DAFCT2Constant.AUDIT_SERVICE_NAME, DAFCT2Constant.AUDIT_SERVICE);
      auditMap.put(DAFCT2Constant.AUDIT_EVENT_TYPE, DAFCT2Constant.AUDIT_CREATE_EVENT_TYPE);
      auditMap.put(
          DAFCT2Constant.AUDIT_EVENT_TIME, String.valueOf(TimeFormatter.getCurrentUTCTime()));
      auditMap.put(DAFCT2Constant.AUDIT_EVENT_STATUS, DAFCT2Constant.AUDIT_EVENT_STATUS_START);
      auditMap.put(DAFCT2Constant.AUDIT_MESSAGE, "Conti Message Streaming");
      auditMap.put(DAFCT2Constant.AUDIT_SOURCE_OBJECT_ID, DAFCT2Constant.DEFAULT_OBJECT_ID);
      auditMap.put(DAFCT2Constant.AUDIT_TARGET_OBJECT_ID, DAFCT2Constant.DEFAULT_OBJECT_ID);

      auditETLJobClient.auditTrialGrpcCall(auditMap);

      log.info("Audit Trial Started");

    } catch (Exception e) {
      log.error("Unable to initialize Audit Trials", e);
    }
  }

  public void flinkConnection() {

    this.streamExecutionEnvironment = StreamExecutionEnvironment.getExecutionEnvironment();
    this.streamExecutionEnvironment.setStreamTimeCharacteristic(TimeCharacteristic.EventTime);
    /*this.streamExecutionEnvironment.enableCheckpointing(5000);
    this.streamExecutionEnvironment.getCheckpointConfig().setMaxConcurrentCheckpoints(1);
    this.streamExecutionEnvironment
        .getCheckpointConfig()
        .enableExternalizedCheckpoints(
            CheckpointConfig.ExternalizedCheckpointCleanup.RETAIN_ON_CANCELLATION);*/
    // this.streamExecutionEnvironment.setStateBackend(new FsStateBackend("file:///checkpointDir"));

    log.info("Flink Processing Started.");
  }

  public void processing(Properties properties) {

    MapStateDescriptor<Message<String>, KafkaRecord<String>> mapStateDescriptor =
        new BroadcastState<String>()
            .stateInitialization(properties.getProperty(DAFCT2Constant.BROADCAST_NAME));

    DataStream<KafkaRecord<String>> masterDataInputStream =
        streamExecutionEnvironment.addSource(
            new FlinkKafkaConsumer<KafkaRecord<String>>(
                properties.getProperty(DAFCT2Constant.MASTER_DATA_TOPIC_NAME),
                new KafkaMessageDeSerializeSchema<String>(),
                properties));

    masterDataInputStream.print();

    BroadcastStream<KafkaRecord<String>> broadcastStream =
        masterDataInputStream.broadcast(mapStateDescriptor);

    DataStream<KafkaRecord<String>> sourceInputStream =
        streamExecutionEnvironment.addSource(
            new FlinkKafkaConsumer<KafkaRecord<String>>(
                properties.getProperty(DAFCT2Constant.SOURCE_TOPIC_NAME),
                new KafkaMessageDeSerializeSchema<String>(),
                properties));

    sourceInputStream.print();

    new MessageProcessing<String, Index>()
        .consumeContiMessage(
            sourceInputStream,
            properties.getProperty(DAFCT2Constant.INDEX_TRANSID),
            "Index",
            properties.getProperty(DAFCT2Constant.SINK_INDEX_TOPIC_NAME),
            properties,
            Index.class,
            broadcastStream);

    new MessageProcessing<String, Status>()
        .consumeContiMessage(
            sourceInputStream,
            properties.getProperty(DAFCT2Constant.STATUS_TRANSID),
            "Status",
            properties.getProperty(DAFCT2Constant.SINK_STATUS_TOPIC_NAME),
            properties,
            Status.class,
            broadcastStream);

    new MessageProcessing<String, Monitor>()
        .consumeContiMessage(
            sourceInputStream,
            properties.getProperty(DAFCT2Constant.MONITOR_TRANSID),
            "Monitor",
            properties.getProperty(DAFCT2Constant.SINK_MONITOR_TOPIC_NAME),
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
      log.error("Unable to process Message using Flink ", e);
      throw new DAFCT2Exception("Unable to process Message using Flink ", e);
    }
  }
}
