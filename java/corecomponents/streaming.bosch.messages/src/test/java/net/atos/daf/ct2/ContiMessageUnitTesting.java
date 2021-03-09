package net.atos.daf.ct2;

import static org.junit.jupiter.api.Assertions.assertAll;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

import java.io.File;
import java.io.FileReader;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.time.Duration;
import java.util.Collections;
import java.util.List;
import java.util.Properties;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.TimeoutException;
import java.util.stream.Collectors;

import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.kafka.clients.admin.AdminClient;
import org.apache.kafka.clients.admin.AdminClientConfig;
import org.apache.kafka.clients.consumer.ConsumerConfig;
import org.apache.kafka.clients.consumer.ConsumerRecords;
import org.apache.kafka.clients.consumer.KafkaConsumer;
import org.apache.kafka.clients.producer.KafkaProducer;
import org.apache.kafka.clients.producer.ProducerConfig;
import org.apache.kafka.clients.producer.ProducerRecord;
import org.apache.kafka.common.serialization.Deserializer;
import org.apache.kafka.common.serialization.StringDeserializer;
import org.apache.kafka.common.serialization.StringSerializer;
import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Order;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.embeddedcluster.EmbeddedSingleNodeKafkaZookeeperCluster;
import net.atos.daf.ct2.kafka.CustomDeserializer;
import net.atos.daf.ct2.main.BoschMessageProcessing;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.Status;

public class ContiMessageUnitTesting {

  private final String FILE_PATH = "src/test/resources/configuration.properties";
  @TempDir File temporaryFolder;
  private Properties consumerProperties;
  private Properties producerProperties;
  private String indexMessage, statusMessage, monitorMessage;
  private EmbeddedSingleNodeKafkaZookeeperCluster embeddedSingleNodeKafkaZookeeperCluster;

  @BeforeEach
  void setUp() throws Exception {

    Properties properties = new Properties();
    properties.load(new FileReader(FILE_PATH));

    this.embeddedSingleNodeKafkaZookeeperCluster =
        new EmbeddedSingleNodeKafkaZookeeperCluster(temporaryFolder);

    this.embeddedSingleNodeKafkaZookeeperCluster.start();

    this.embeddedSingleNodeKafkaZookeeperCluster.createTopic(
        properties.getProperty(DAFCT2Constant.SOURCE_TOPIC_NAME), 1, 1);
    this.embeddedSingleNodeKafkaZookeeperCluster.createTopic(
        properties.getProperty(DAFCT2Constant.SINK_INDEX_TOPIC_NAME), 1, 1);
    this.embeddedSingleNodeKafkaZookeeperCluster.createTopic(
        properties.getProperty(DAFCT2Constant.SINK_STATUS_TOPIC_NAME), 1, 1);
    this.embeddedSingleNodeKafkaZookeeperCluster.createTopic(
        properties.getProperty(DAFCT2Constant.SINK_MONITOR_TOPIC_NAME), 1, 1);
    this.embeddedSingleNodeKafkaZookeeperCluster.createTopic(
        properties.getProperty(DAFCT2Constant.MASTER_DATA_TOPIC_NAME), 1, 1);

    this.consumerProperties = new Properties();
    this.consumerProperties.put(
        ConsumerConfig.BOOTSTRAP_SERVERS_CONFIG,
        this.embeddedSingleNodeKafkaZookeeperCluster.bootstrapServers());
    this.consumerProperties.put(ConsumerConfig.GROUP_ID_CONFIG, "ct2contiprocessing_group_test");
    this.consumerProperties.put(
        ConsumerConfig.KEY_DESERIALIZER_CLASS_CONFIG, StringDeserializer.class);
    this.consumerProperties.put(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, "earliest");

    this.producerProperties = new Properties();
    this.producerProperties.put(
        ProducerConfig.BOOTSTRAP_SERVERS_CONFIG,
        this.embeddedSingleNodeKafkaZookeeperCluster.bootstrapServers());

    this.producerProperties.put(ProducerConfig.KEY_SERIALIZER_CLASS_CONFIG, StringSerializer.class);
    this.producerProperties.put(
        ProducerConfig.VALUE_SERIALIZER_CLASS_CONFIG, StringSerializer.class);

    this.indexMessage =
        Files.lines(Paths.get("src/test/resources/Index.json"))
            .collect(Collectors.joining(System.lineSeparator()));
    this.statusMessage =
        Files.lines(Paths.get("src/test/resources/Status.json"))
            .collect(Collectors.joining(System.lineSeparator()));
    this.monitorMessage =
        Files.lines(Paths.get("src/test/resources/Monitor.json"))
            .collect(Collectors.joining(System.lineSeparator()));

    KafkaProducer<String, String> producer = new KafkaProducer<>(this.producerProperties);
    ProducerRecord<String, String> record1 =
        new ProducerRecord<String, String>(
            properties.getProperty(DAFCT2Constant.SOURCE_TOPIC_NAME), "1", this.indexMessage);
    ProducerRecord<String, String> record2 =
        new ProducerRecord<String, String>(
            properties.getProperty(DAFCT2Constant.SOURCE_TOPIC_NAME), "2", this.statusMessage);
    ProducerRecord<String, String> record3 =
        new ProducerRecord<String, String>(
            properties.getProperty(DAFCT2Constant.SOURCE_TOPIC_NAME), "3", this.monitorMessage);

    ProducerRecord<String, String> record5 =
        new ProducerRecord<String, String>(
            properties.getProperty(DAFCT2Constant.MASTER_DATA_TOPIC_NAME),
            "M4A1114",
            "M4A1114_VIN");
    ProducerRecord<String, String> record6 =
        new ProducerRecord<String, String>(
            properties.getProperty(DAFCT2Constant.MASTER_DATA_TOPIC_NAME),
            "M4A1115",
            "M4A1115_VIN");
    ProducerRecord<String, String> record7 =
        new ProducerRecord<String, String>(
            properties.getProperty(DAFCT2Constant.MASTER_DATA_TOPIC_NAME),
            "M4A1116",
            "M4A1116_VIN");

    producer.send(record1);
    producer.send(record2);
    producer.send(record3);

    producer.send(record5);
    producer.send(record6);
    producer.send(record7);
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
                            topicName.equalsIgnoreCase("ingress.conti.vehicledata.string")),
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
                "Topic Monitor is created"),
        () ->
            assertTrue(
                topics.stream()
                    .anyMatch(
                        topicName ->
                            topicName.equalsIgnoreCase("egress.conti.mastervehicledata.object")),
                "Topic Master Data is created"));
  }

  @Test
  @Order(value = 3)
  // @Disabled(value = "Not implemented yet")
  @DisplayName(value = "Segregation Message")
  public void valiadateMessage() throws Exception {

    BoschMessageProcessing boschMessageProcessing = new BoschMessageProcessing();
    BoschMessageProcessing.FILE_PATH = FILE_PATH;

    Properties properties1 = BoschMessageProcessing.configuration();
    boschMessageProcessing.flinkConnection();

    properties1.put(
        ProducerConfig.BOOTSTRAP_SERVERS_CONFIG,
        this.embeddedSingleNodeKafkaZookeeperCluster.bootstrapServers());

    boschMessageProcessing.processing(properties1);

    StreamExecutionEnvironment streamExecutionEnvironment =
    		boschMessageProcessing.getstreamExecutionEnvironment();

    CompletableFuture<Void> handle =
        CompletableFuture.runAsync(
            () -> {
              try {
                streamExecutionEnvironment.execute("Testing");

              } catch (Exception e) {
                e.printStackTrace();
              }
            });
    try {
      handle.get(30L, TimeUnit.SECONDS);

    } catch (TimeoutException | InterruptedException | ExecutionException e) {
      handle.cancel(true);
    }

    Deserializer<Index> indexDeserializer = new CustomDeserializer<Index>();
    this.consumerProperties.put(
        ConsumerConfig.VALUE_DESERIALIZER_CLASS_CONFIG, indexDeserializer.getClass());

    KafkaConsumer<String, Index> indexConsumer = new KafkaConsumer<>(this.consumerProperties);
    indexConsumer.subscribe(
        Collections.singletonList(properties1.getProperty(DAFCT2Constant.SINK_INDEX_TOPIC_NAME)));
    ConsumerRecords<String, Index> indexMessage = indexConsumer.poll(Duration.ofMillis(5000));

    indexMessage.forEach(
        r -> {
          String value = r.value().toString();
          System.out.println(value);
          assertEquals(r.value().getTransID(), "03000");
        });

    Deserializer<Status> statusDeserializer = new CustomDeserializer<Status>();
    this.consumerProperties.put(
        ConsumerConfig.VALUE_DESERIALIZER_CLASS_CONFIG, statusDeserializer.getClass());

    KafkaConsumer<String, Status> statusConsumer = new KafkaConsumer<>(this.consumerProperties);
    statusConsumer.subscribe(
        Collections.singletonList(properties1.getProperty(DAFCT2Constant.SINK_STATUS_TOPIC_NAME)));
    ConsumerRecords<String, Status> statusMessage = statusConsumer.poll(Duration.ofMillis(5000));

    statusMessage.forEach(
        r -> {
          String value = r.value().toString();
          System.out.println(value);
          assertEquals(r.value().getTransID(), "03010");
        });

    Deserializer<Monitor> monitorDeserializer = new CustomDeserializer<Monitor>();
    this.consumerProperties.put(
        ConsumerConfig.VALUE_DESERIALIZER_CLASS_CONFIG, monitorDeserializer.getClass());

    KafkaConsumer<String, Monitor> monitorConsumer = new KafkaConsumer<>(this.consumerProperties);
    monitorConsumer.subscribe(
        Collections.singletonList(properties1.getProperty(DAFCT2Constant.SINK_MONITOR_TOPIC_NAME)));
    ConsumerRecords<String, Monitor> monitorMessage = monitorConsumer.poll(Duration.ofMillis(5000));

    monitorMessage.forEach(
        r -> {
          String value = r.value().toString();
          System.out.println(value);
          assertEquals(r.value().getTransID(), "03030");
        });
  }

  @AfterEach
  void tearDown() {
    this.embeddedSingleNodeKafkaZookeeperCluster.stop();
  }
}
