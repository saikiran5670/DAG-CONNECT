package net.atos.daf.ct2.embeddedcluster;

import kafka.server.KafkaConfig;

import java.io.File;
import java.io.IOException;
import java.util.Properties;

public class EmbeddedSingleNodeKafkaZookeeperCluster {

  private boolean running;
  private File temporaryFolder;
  private KafkaEmbedded kafkaEmbedded;
  private final Properties brokerConfiguration;
  private ZooKeeperEmbedded zooKeeperEmbedded;

  public EmbeddedSingleNodeKafkaZookeeperCluster(File temporaryFolder) {
    this(new Properties(), temporaryFolder);
  }

  public EmbeddedSingleNodeKafkaZookeeperCluster(Properties properties, File temporaryFolder) {
    this.brokerConfiguration = new Properties();
    this.brokerConfiguration.putAll(properties);

    this.temporaryFolder = temporaryFolder;
  }

  public void start() throws Exception {

    this.zooKeeperEmbedded = new ZooKeeperEmbedded();
    Properties properties = configuration(this.brokerConfiguration, this.zooKeeperEmbedded);
    this.kafkaEmbedded = new KafkaEmbedded(properties, temporaryFolder);

    this.running = true;
  }

  private Properties configuration(Properties properties, ZooKeeperEmbedded zookeeper) {

    Properties clusterConfiguration = new Properties();

    clusterConfiguration.putAll(properties);

    clusterConfiguration.put(KafkaConfig.ZkConnectProp(), zookeeper.connectString());
    clusterConfiguration.put(KafkaConfig.ZkSessionTimeoutMsProp(), 30 * 1000);
    clusterConfiguration.put(KafkaConfig.ZkConnectionTimeoutMsProp(), 60 * 1000);
    clusterConfiguration.put(KafkaConfig.DeleteTopicEnableProp(), true);
    clusterConfiguration.put(KafkaConfig.LogCleanerDedupeBufferSizeProp(), 2 * 1024 * 1024L);
    clusterConfiguration.put(KafkaConfig.GroupMinSessionTimeoutMsProp(), 0);
    clusterConfiguration.put(KafkaConfig.OffsetsTopicReplicationFactorProp(), (short) 1);
    clusterConfiguration.put(KafkaConfig.OffsetsTopicPartitionsProp(), 1);
    clusterConfiguration.put(KafkaConfig.AutoCreateTopicsEnableProp(), true);

    return clusterConfiguration;
  }

  public String bootstrapServers() {
    return this.kafkaEmbedded.brokerList();
  }

  public String zookeeperConnect() {
    return this.zooKeeperEmbedded.connectString();
  }

  public void createTopic(String topic) {
    createTopic(topic, 1, 1, new Properties());
  }

  public void createTopic(String topic, int partitions, int replication) {
    createTopic(topic, partitions, replication, new Properties());
  }

  public void createTopic(String topic, int partitions, int replication, Properties topicConfig) {
    this.kafkaEmbedded.createTopic(topic, partitions, replication, topicConfig);
  }

  public boolean isRunning() {
    return running;
  }

  public void stop() {

    try {
      if (this.kafkaEmbedded != null) {
        this.kafkaEmbedded.stop();
      }

      try {
        if (this.zooKeeperEmbedded != null) {
          this.zooKeeperEmbedded.stop();
        }

      } catch (IOException e) {
        throw new RuntimeException(e);
      }

    } finally {
      this.running = false;
    }
  }
}
