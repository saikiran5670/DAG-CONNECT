package net.atos.daf.ct2;

import com.fasterxml.jackson.databind.ObjectMapper;
import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.embeddedcluster.EmbeddedSingleNodeKafkaZookeeperCluster;
import net.atos.daf.ct2.kafka.CustomSerializer;
import net.atos.daf.ct2.main.SinkMessages;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.utils.JsonMapper;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.kafka.clients.admin.AdminClient;
import org.apache.kafka.clients.admin.AdminClientConfig;
import org.apache.kafka.clients.consumer.ConsumerConfig;
import org.apache.kafka.clients.consumer.ConsumerRecord;
import org.apache.kafka.clients.consumer.ConsumerRecords;
import org.apache.kafka.clients.consumer.KafkaConsumer;
import org.apache.kafka.clients.producer.KafkaProducer;
import org.apache.kafka.clients.producer.ProducerConfig;
import org.apache.kafka.clients.producer.ProducerRecord;
import org.apache.kafka.common.serialization.Serializer;
import org.apache.kafka.common.serialization.StringDeserializer;
import org.apache.kafka.common.serialization.StringSerializer;
import org.junit.jupiter.api.*;
import org.junit.jupiter.api.io.TempDir;

import java.io.File;
import java.io.FileReader;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.time.Duration;
import java.util.Arrays;
import java.util.Collections;
import java.util.List;
import java.util.Properties;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.TimeoutException;
import java.util.stream.Collectors;

import static org.junit.jupiter.api.Assertions.*;

public class ExternalMessageUnitTesting {

  private final String FILE_PATH = "src/test/resources/configuration.properties";
  @TempDir File temporaryFolder;
  private ObjectMapper objectMapper;
  private Properties consumerProperties;
  private Properties producerProperties;
  private String indexMessage, statusMessage, monitorMessage;
  private EmbeddedSingleNodeKafkaZookeeperCluster embeddedSingleNodeKafkaZookeeperCluster;

  @BeforeEach
  void setUp() throws Exception {

    Properties properties = new Properties();
    properties.load(new FileReader(FILE_PATH));

    this.objectMapper = JsonMapper.configuring();

    this.embeddedSingleNodeKafkaZookeeperCluster =
        new EmbeddedSingleNodeKafkaZookeeperCluster(temporaryFolder);

    this.embeddedSingleNodeKafkaZookeeperCluster.start();

    this.embeddedSingleNodeKafkaZookeeperCluster.createTopic(
        properties.getProperty(DAFCT2Constant.SINK_JSON_STRING_TOPIC_NAME), 1, 1);
    this.embeddedSingleNodeKafkaZookeeperCluster.createTopic(
        properties.getProperty(DAFCT2Constant.SINK_INDEX_TOPIC_NAME), 1, 1);
    this.embeddedSingleNodeKafkaZookeeperCluster.createTopic(
        properties.getProperty(DAFCT2Constant.SINK_STATUS_TOPIC_NAME), 1, 1);
    this.embeddedSingleNodeKafkaZookeeperCluster.createTopic(
        properties.getProperty(DAFCT2Constant.SINK_MONITOR_TOPIC_NAME), 1, 1);

    this.consumerProperties = new Properties();
    this.consumerProperties.put(
        ConsumerConfig.BOOTSTRAP_SERVERS_CONFIG,
        this.embeddedSingleNodeKafkaZookeeperCluster.bootstrapServers());

    this.consumerProperties.put(
        ConsumerConfig.KEY_DESERIALIZER_CLASS_CONFIG, StringDeserializer.class);
    this.consumerProperties.put(
        ConsumerConfig.VALUE_DESERIALIZER_CLASS_CONFIG, StringDeserializer.class);
    this.consumerProperties.put(ConsumerConfig.GROUP_ID_CONFIG, "ct2externalprocessing_group_test");
    this.consumerProperties.put(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, "earliest");

    this.producerProperties = new Properties();
    this.producerProperties.put(
        ProducerConfig.BOOTSTRAP_SERVERS_CONFIG,
        this.embeddedSingleNodeKafkaZookeeperCluster.bootstrapServers());
    this.producerProperties.put(ProducerConfig.KEY_SERIALIZER_CLASS_CONFIG, StringSerializer.class);

    this.indexMessage =
        Files.lines(Paths.get("src/test/resources/Index.json"))
            .collect(Collectors.joining(System.lineSeparator()));
    this.statusMessage =
        Files.lines(Paths.get("src/test/resources/Status.json"))
            .collect(Collectors.joining(System.lineSeparator()));
    this.monitorMessage =
        Files.lines(Paths.get("src/test/resources/Monitor.json"))
            .collect(Collectors.joining(System.lineSeparator()));

    // ObjectMapper objectMapper = JsonMapper.configuring();
    Index index = this.objectMapper.readValue(this.indexMessage, Index.class);
    Status status = this.objectMapper.readValue(this.statusMessage, Status.class);
    Monitor monitor = this.objectMapper.readValue(this.monitorMessage, Monitor.class);

    Serializer<Index> indexCustomSerializer = new CustomSerializer<Index>();
    this.producerProperties.put(
        ProducerConfig.VALUE_SERIALIZER_CLASS_CONFIG, indexCustomSerializer.getClass());
    KafkaProducer<String, Index> indexProducer = new KafkaProducer<>(this.producerProperties);
    ProducerRecord<String, Index> indexRecord =
        new ProducerRecord<String, Index>(
            properties.getProperty(DAFCT2Constant.SINK_INDEX_TOPIC_NAME), "1", index);
    indexProducer.send(indexRecord);

    Serializer<Status> statusCustomSerializer = new CustomSerializer<Status>();
    this.producerProperties.put(
        ProducerConfig.VALUE_SERIALIZER_CLASS_CONFIG, statusCustomSerializer.getClass());
    KafkaProducer<String, Status> statusProducer = new KafkaProducer<>(this.producerProperties);
    ProducerRecord<String, Status> statusRecord =
        new ProducerRecord<String, Status>(
            properties.getProperty(DAFCT2Constant.SINK_STATUS_TOPIC_NAME), "2", status);
    statusProducer.send(statusRecord);

    Serializer<Monitor> monitorCustomSerializer = new CustomSerializer<Monitor>();
    this.producerProperties.put(
        ProducerConfig.VALUE_SERIALIZER_CLASS_CONFIG, monitorCustomSerializer.getClass());
    KafkaProducer<String, Monitor> monitorProducer = new KafkaProducer<>(this.producerProperties);
    ProducerRecord<String, Monitor> monitorRecord =
        new ProducerRecord<String, Monitor>(
            properties.getProperty(DAFCT2Constant.SINK_MONITOR_TOPIC_NAME), "3", monitor);
    monitorProducer.send(monitorRecord);
  }

  @Test
  @Order(value = 1)
  @Disabled
  @DisplayName(value = "Testing Embedded Kafka-Zookeeper Cluster")
  public void embededClusterStatus() {
    assertTrue(this.embeddedSingleNodeKafkaZookeeperCluster.isRunning());
  }

  @Test
  @Disabled
  @Order(value = 2)
  @DisplayName(value = "Topics Available")
  public void topicExistOrNot() throws ExecutionException, InterruptedException {

    Properties properties = new Properties();
    properties.setProperty(
        AdminClientConfig.BOOTSTRAP_SERVERS_CONFIG,
        this.embeddedSingleNodeKafkaZookeeperCluster.bootstrapServers());

    AdminClient adminClient = AdminClient.create(properties);
    List<String> topics =
        adminClient.listTopics().names().get().stream().collect(Collectors.toList());

    assertAll(
        "Validating list of topics created",
        () ->
            assertTrue(
                topics.stream()
                    .anyMatch(
                        topicName ->
                            topicName.equalsIgnoreCase("egress.conti.externalmessagedata.string")),
                "Topic my-topic is created"),
        () ->
            assertTrue(
                topics.stream()
                    .anyMatch(
                        topicName -> topicName.equalsIgnoreCase("egress.conti.indexdata.object")),
                "Topic Index is created"),
        () ->
            assertTrue(
                topics.stream()
                    .anyMatch(
                        topicName -> topicName.equalsIgnoreCase("egress.conti.statusdata.object")),
                "Topic Status is created"),
        () ->
            assertTrue(
                topics.stream()
                    .anyMatch(
                        topicName -> topicName.equalsIgnoreCase("egress.conti.monitordata.object")),
                "Topic Monitor is created"));
  }

  @Test
  @Order(value = 3)
  // @Disabled(value = "Not implemented yet")
  @DisplayName(value = "External Message")
  public void valiadateMessage() throws Exception {

    SinkMessages sinkMessages = new SinkMessages();
    SinkMessages.FILE_PATH = FILE_PATH;
    Properties properties1 = SinkMessages.configuration();

    final StreamExecutionEnvironment env = sinkMessages.flinkConnection(properties1);

    List<String> listTopics =
        Arrays.asList(
            properties1.getProperty(DAFCT2Constant.SINK_INDEX_TOPIC_NAME),
            properties1.getProperty(DAFCT2Constant.SINK_STATUS_TOPIC_NAME),
            properties1.getProperty(DAFCT2Constant.SINK_MONITOR_TOPIC_NAME));

    properties1.put(
        ProducerConfig.BOOTSTRAP_SERVERS_CONFIG,
        this.embeddedSingleNodeKafkaZookeeperCluster.bootstrapServers());

    sinkMessages.processing(env, properties1, listTopics);

    /*StreamExecutionEnvironment streamExecutionEnvironment =
        sinkMessages.getstreamExecutionEnvironment();
     */
    CompletableFuture<Void> handle =
        CompletableFuture.runAsync(
            () -> {
              try {
            	  env.execute("Testing");

              } catch (Exception e) {
                e.printStackTrace();
              }
            });
    try {
      handle.get(30L, TimeUnit.SECONDS);

    } catch (TimeoutException | InterruptedException | ExecutionException e) {
      handle.cancel(true);
    }

    this.consumerProperties.put(
        ConsumerConfig.KEY_DESERIALIZER_CLASS_CONFIG, StringDeserializer.class);
    this.consumerProperties.put(
        ConsumerConfig.VALUE_DESERIALIZER_CLASS_CONFIG, StringDeserializer.class);
    KafkaConsumer<String, String> kafkaConsumer = new KafkaConsumer<>(this.consumerProperties);
    kafkaConsumer.subscribe(
        Collections.singletonList(
            properties1.getProperty(DAFCT2Constant.SINK_JSON_STRING_TOPIC_NAME)));
    ConsumerRecords<String, String> message = kafkaConsumer.poll(Duration.ofMillis(5000));

    int count = 0;
    List<String> list = Arrays.asList("03030", "03010", "03000");

    for (ConsumerRecord<String, String> record : message) {
      assertEquals(
          this.objectMapper.readTree(record.value()).get("TransID").asText(), list.get(count++));
    }
  }

  @AfterEach
  void tearDown() {
    this.embeddedSingleNodeKafkaZookeeperCluster.stop();
  }
}
