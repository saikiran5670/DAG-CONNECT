package net.atos.daf.ct2.embeddedcluster;

import kafka.server.KafkaConfig;
import kafka.server.KafkaServer;
import kafka.utils.TestUtils;
import org.apache.kafka.clients.admin.AdminClient;
import org.apache.kafka.clients.admin.AdminClientConfig;
import org.apache.kafka.clients.admin.NewTopic;
import org.apache.kafka.common.network.ListenerName;
import org.apache.kafka.common.security.auth.SecurityProtocol;
import org.apache.kafka.common.utils.Time;

import java.io.File;
import java.io.IOException;
import java.util.Collections;
import java.util.Properties;

public class KafkaEmbedded {

  private static final Integer NO_PARTITION = 1;
  private static final String KAFKA_PORT = "9092";
  private static final String ZOOKEEPER_PORT = "2181";
  private static final String HOST_NAME = "127.0.0.1";
  private static final String ZOOKEEPER_CONNECT = HOST_NAME + ":" + ZOOKEEPER_PORT;

  private final File logDirectory;
  private final KafkaServer kafkaServer;
  private final Properties brokerConfiguration;

  public KafkaEmbedded(Properties properties, File temporaryFolder) throws IOException {

    this.logDirectory = temporaryFolder;
    this.brokerConfiguration = configuration(properties);
    boolean loggingEnabled = true;

    KafkaConfig kafkaConfig = new KafkaConfig(this.brokerConfiguration, loggingEnabled);
    this.kafkaServer = TestUtils.createServer(kafkaConfig, Time.SYSTEM);
  }

  private Properties configuration(Properties properties) throws IOException {

    Properties clusterConfiguration = new Properties();
    clusterConfiguration.put(KafkaConfig.BrokerIdProp(), 0);
    clusterConfiguration.put(KafkaConfig.HostNameProp(), HOST_NAME);
    clusterConfiguration.put(KafkaConfig.PortProp(), KAFKA_PORT);
    clusterConfiguration.put(KafkaConfig.NumPartitionsProp(), NO_PARTITION);
    clusterConfiguration.put(KafkaConfig.MessageMaxBytesProp(), 1000000);
    clusterConfiguration.put(KafkaConfig.ControlledShutdownEnableProp(), true);
    clusterConfiguration.put(
        AdminClientConfig.BOOTSTRAP_SERVERS_CONFIG, HOST_NAME + ":" + KAFKA_PORT);

    clusterConfiguration.putAll(properties);
    clusterConfiguration.setProperty(KafkaConfig.LogDirProp(), this.logDirectory.getAbsolutePath());

    return clusterConfiguration;
  }

  public String brokerList() {

    return String.join(
        ":",
        this.kafkaServer.config().hostName(),
        Integer.toString(
            this.kafkaServer.boundPort(
                ListenerName.forSecurityProtocol(SecurityProtocol.PLAINTEXT))));
  }

  public String zookeeperConnect() {
    return this.brokerConfiguration.getProperty("zookeeper.connect", ZOOKEEPER_CONNECT);
  }

  public void createTopic(String topic, int partitions, int replication, Properties topicConfig) {

    this.brokerConfiguration.putAll(topicConfig);
    AdminClient adminClient = AdminClient.create(this.brokerConfiguration);
    NewTopic newTopic = new NewTopic(topic, partitions, (short) replication);
    adminClient.createTopics(Collections.singleton(newTopic));
    adminClient.close();
  }

  public void stop() {

    this.kafkaServer.shutdown();
    this.kafkaServer.awaitShutdown();
  }
}
