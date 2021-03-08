package net.atos.daf.ct2.embeddedcluster;

import org.apache.curator.test.TestingServer;

import java.io.IOException;

public class ZooKeeperEmbedded {

  private final TestingServer testingServer;

  public ZooKeeperEmbedded() throws Exception {
    this.testingServer = new TestingServer();
  }

  public void stop() throws IOException {
    this.testingServer.close();
  }

  public String connectString() {
    return this.testingServer.getConnectString();
  }

  public String hostname() {
    return connectString().substring(0, connectString().lastIndexOf(':'));
  }
}
