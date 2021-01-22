package net.atos.daf.ct2.main;

import net.atos.daf.common.AuditETLJobClient;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.exception.DAFCT2Exception;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.processing.MessageProcessing;
import net.atos.daf.ct2.serde.KafkaMessageDeSerializeSchema;
import org.apache.flink.streaming.api.TimeCharacteristic;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.kafka.clients.consumer.ConsumerConfig;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.io.FileReader;
import java.io.IOException;
import java.util.*;

public class SinkMessages<T> {

  private static final Logger log = LogManager.getLogger(SinkMessages.class);
  public static String FILE_PATH; // "src/main/resources/configuration.properties";
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
      FILE_PATH = args[0];

      SinkMessages sinkMessages = new SinkMessages();
      Properties properties = configuration();
      sinkMessages.auditTrail(properties);

      sinkMessages.flinkConnection();

      List<String> listTopics =
          Arrays.asList(
              properties.getProperty(DAFCT2Constant.SINK_INDEX_TOPIC_NAME),
              properties.getProperty(DAFCT2Constant.SINK_STATUS_TOPIC_NAME),
              properties.getProperty(DAFCT2Constant.SINK_MONITOR_TOPIC_NAME));
      sinkMessages.processing(properties, listTopics);
      sinkMessages.startExecution();

    } catch (Exception e) {
      log.error("Exception: ", e);

    }finally{
      //auditETLJobClient.closeChannel();
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
    // this.streamExecutionEnvironment.setStateBackend(new FsStateBackend(""));

    log.info("Flink Processing Started.");
  }

  public static void auditTrail(Properties properties) {

    auditETLJobClient =
            new AuditETLJobClient(
                    properties.getProperty(DAFCT2Constant.GRPC_SERVER),
                    Integer.valueOf(properties.getProperty(DAFCT2Constant.GRPC_PORT)));
    Map<String, String> auditMap = new HashMap<String, String>();

    auditMap.put(DAFCT2Constant.JOB_EXEC_TIME, String.valueOf(TimeFormatter.getCurrentUTCTime()));
    auditMap.put(DAFCT2Constant.AUDIT_PERFORMED_BY, DAFCT2Constant.TRIP_JOB_NAME);
    auditMap.put(DAFCT2Constant.AUDIT_COMPONENT_NAME, DAFCT2Constant.TRIP_JOB_NAME);
    auditMap.put(DAFCT2Constant.AUDIT_SERVICE_NAME, DAFCT2Constant.AUDIT_SERVICE);
    auditMap.put(DAFCT2Constant.AUDIT_EVENT_TYPE, DAFCT2Constant.AUDIT_CREATE_EVENT_TYPE);
    auditMap.put(
        DAFCT2Constant.AUDIT_EVENT_TIME, String.valueOf(TimeFormatter.getCurrentUTCTime()));
    auditMap.put(DAFCT2Constant.AUDIT_EVENT_STATUS, DAFCT2Constant.AUDIT_EVENT_STATUS_START);
    auditMap.put(DAFCT2Constant.AUDIT_MESSAGE, "Storing Messages in External Topic");
    auditMap.put(DAFCT2Constant.AUDIT_SOURCE_OBJECT_ID, DAFCT2Constant.DEFAULT_OBJECT_ID);
    auditMap.put(DAFCT2Constant.AUDIT_TARGET_OBJECT_ID, DAFCT2Constant.DEFAULT_OBJECT_ID);

    auditETLJobClient.auditTrialGrpcCall(auditMap);

    log.info("Audit Trial Started");
  }

  public void processing(Properties properties, List<String> listTopics) {

    DataStream<KafkaRecord<Object>> sourceInputStream =
        this.streamExecutionEnvironment.addSource(
            new FlinkKafkaConsumer<KafkaRecord<Object>>(
                listTopics, new KafkaMessageDeSerializeSchema<Object>(), properties));
    sourceInputStream.print();

    new MessageProcessing<Object>().consumeMessages(sourceInputStream, "Records", properties);
  }

  public StreamExecutionEnvironment getstreamExecutionEnvironment() {
    return this.streamExecutionEnvironment;
  }

  public void startExecution() throws DAFCT2Exception {

    try {
      this.streamExecutionEnvironment.execute("External Records");

    } catch (Exception e) {
      log.error("Unable to process Message using Flink ", e);
      throw new DAFCT2Exception("Unable to process Message using Flink ", e);
    }
  }
}
