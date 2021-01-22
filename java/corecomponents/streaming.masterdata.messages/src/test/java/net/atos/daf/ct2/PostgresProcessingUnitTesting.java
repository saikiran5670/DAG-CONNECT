package net.atos.daf.ct2;

import io.debezium.config.Configuration;
import net.atos.daf.ct2.constant.DAFCT2Constant;
import net.atos.daf.ct2.embeddedcluster.EmbeddedSingleNodeKafkaZookeeperCluster;
import net.atos.daf.ct2.main.PostgresCdc;
import net.atos.daf.ct2.kafka.SqlConnection;
import net.atos.daf.ct2.processing.MessageProcessing;
import org.apache.kafka.clients.admin.AdminClient;
import org.apache.kafka.clients.admin.AdminClientConfig;
import org.apache.kafka.clients.consumer.ConsumerConfig;
import org.apache.kafka.clients.consumer.ConsumerRecords;
import org.apache.kafka.clients.consumer.KafkaConsumer;
import org.apache.kafka.clients.producer.*;
import org.apache.kafka.common.serialization.StringDeserializer;
import org.apache.kafka.common.serialization.StringSerializer;
import org.junit.jupiter.api.*;
import org.junit.jupiter.api.io.TempDir;

import java.io.File;
import java.io.FileReader;
import java.time.Duration;
import java.util.Collections;
import java.util.List;
import java.util.Properties;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.TimeoutException;
import java.util.stream.Collectors;

import static org.junit.jupiter.api.Assertions.*;

public class PostgresProcessingUnitTesting {

  private final String FILE_PATH = "src/test/resources/configuration.properties";

  @TempDir File temporaryFolder;

  private SqlConnection sqlConnection;
  private Properties properties;
  private Properties consumerProperties;
  private EmbeddedSingleNodeKafkaZookeeperCluster embeddedSingleNodeKafkaZookeeperCluster;

  @BeforeEach
  void setUp() throws Exception {

    Properties properties = new Properties();
    properties.load(new FileReader(FILE_PATH));

    this.embeddedSingleNodeKafkaZookeeperCluster =
        new EmbeddedSingleNodeKafkaZookeeperCluster(temporaryFolder);

    this.embeddedSingleNodeKafkaZookeeperCluster.start();

    this.embeddedSingleNodeKafkaZookeeperCluster.createTopic(properties.getProperty(DAFCT2Constant.MASTER_DATA_TOPIC_NAME), 1, 1);

    this.consumerProperties = new Properties();
    this.consumerProperties.put(
        ConsumerConfig.BOOTSTRAP_SERVERS_CONFIG,
        this.embeddedSingleNodeKafkaZookeeperCluster.bootstrapServers());

    this.consumerProperties.put(ConsumerConfig.KEY_DESERIALIZER_CLASS_CONFIG, StringDeserializer.class);
    this.consumerProperties.put(
        ConsumerConfig.VALUE_DESERIALIZER_CLASS_CONFIG, StringDeserializer.class);
    this.consumerProperties.put(ConsumerConfig.GROUP_ID_CONFIG, "ct2masterdataprocessing_group_test");
    this.consumerProperties.put(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, "earliest");

    this.properties = new Properties();
    this.properties.load(new FileReader(this.FILE_PATH));
    this.properties.put(
        ProducerConfig.BOOTSTRAP_SERVERS_CONFIG,
        this.embeddedSingleNodeKafkaZookeeperCluster.bootstrapServers());
    this.properties.put(ProducerConfig.KEY_SERIALIZER_CLASS_CONFIG, StringSerializer.class);
    this.properties.put(ProducerConfig.VALUE_SERIALIZER_CLASS_CONFIG, StringSerializer.class);

    this.sqlConnection = new SqlConnection(this.properties);
    this.sqlConnection.creation(
        "CREATE TABLE public.vehicle(vid VARCHAR PRIMARY KEY, vin VARCHAR);");

    this.sqlConnection.creation(
        "CREATE PUBLICATION dbz_publication FOR TABLE vehicle WITH (publish = 'insert, update');");

    this.sqlConnection.insertion(
        "INSERT INTO vehicle(vid, vin) VALUES( ?, ?);", "M4A1114", "M4A1114");
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
                topics.stream().anyMatch(topicName -> topicName.equalsIgnoreCase("ingress.conti.mastervehicledata.string")),
                "Topic masterdata is created"));
  }

  @Test
  @Order(value = 3)
  // @Disabled(value = "Not implemented yet")
  @DisplayName(value = "Ingestion Message")
  public void valiadateMessage() throws Exception {

    PostgresCdc postgresCdc = new PostgresCdc();
    Properties properties = postgresCdc.configuration(this.FILE_PATH);

    Configuration configuration = postgresCdc.connectingPostgreSQL(properties);

    properties.put(
            ProducerConfig.BOOTSTRAP_SERVERS_CONFIG,
            this.embeddedSingleNodeKafkaZookeeperCluster.bootstrapServers());

    MessageProcessing messageProcessing = new MessageProcessing(configuration, properties);

    CompletableFuture<Void> handle =
        CompletableFuture.runAsync(
            () -> {
              try {
                messageProcessing.run();

              } catch (Exception e) {
                e.printStackTrace();
              }
            });
    try {
      handle.get(20L, TimeUnit.SECONDS);

    } catch (TimeoutException | InterruptedException | ExecutionException e) {
      handle.cancel(true);
    }

    KafkaConsumer<String, String> consumer =
        new KafkaConsumer<String, String>(this.consumerProperties);
    consumer.subscribe(Collections.singletonList(properties.getProperty(DAFCT2Constant.MASTER_DATA_TOPIC_NAME)));
    ConsumerRecords<String, String> message = consumer.poll(Duration.ofMillis(5000));

    message.forEach(
        r -> {
          String value = r.value();
          System.out.println(value);
          assertEquals(value, "M4A1114");
        });
  }

  @AfterEach
  void tearDown() {

    this.sqlConnection.deletion("DROP PUBLICATION dbz_publication;");
    this.sqlConnection.deletion("DROP TABLE vehicle;");

    this.embeddedSingleNodeKafkaZookeeperCluster.stop();
  }
}
