package net.atos.daf.ct2.poc;

import io.debezium.config.Configuration;
import io.debezium.embedded.EmbeddedEngine;
// import io.debezium.engine.DebeziumEngine;
import io.debezium.util.Clock;
import net.atos.daf.ct2.exception.DAFCT2Exception;
import net.atos.daf.ct2.utils.Operation;
import org.apache.kafka.connect.data.Struct;
import org.apache.kafka.connect.source.SourceRecord;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.io.FileReader;
import java.io.IOException;
import java.util.Properties;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;

import static io.debezium.data.Envelope.FieldName.AFTER;
import static io.debezium.data.Envelope.FieldName.OPERATION;

public class Testing implements Runnable {

  private static final String FILE_PATH = "src/main/resources/configuration.properties";
  private static final Logger log = LogManager.getLogger(Testing.class);

  private Configuration configuration;
  private EmbeddedEngine embeddedEngine;

  private Properties configuration() throws DAFCT2Exception {
    Properties properties = new Properties();

    try {
      properties.load(new FileReader(FILE_PATH));
      log.info("Configuration Loaded for Connecting Kafka inorder to Perform Mapping.");

    } catch (IOException e) {
      log.error("Unable to Find the File " + FILE_PATH, e);
      throw new DAFCT2Exception("Unable to Find the File " + FILE_PATH, e);
    }
    return properties;
  }

  public static void main(String[] args) throws DAFCT2Exception {
    Testing testing = new Testing();
    testing.connectingPostgreSQL();
    testing.run();
  }

  public void connectingPostgreSQL() throws DAFCT2Exception {
    Properties properties = configuration();

    try {
      this.configuration =
          Configuration.create()
              // io.debezium.connector.postgresql.PostgresConnector
              .with(EmbeddedEngine.CONNECTOR_CLASS, "io.debezium.connector.mysql.MySqlConnector")
              .with(
                  EmbeddedEngine.OFFSET_STORAGE,
                  "org.apache.kafka.connect.storage.FileOffsetBackingStore")
              .with(EmbeddedEngine.OFFSET_STORAGE_FILE_FILENAME, "src/main/resources/offset.dat1")
              .with(EmbeddedEngine.OFFSET_FLUSH_INTERVAL_MS, 60000)
              .with("name", "PostgreCDC_Kafka_Streaming")
              .with("database.server.name", "postgre-connector")
              .with("database.server.id", 85744)
              .with("database.hostname", properties.getProperty("database.hostname"))
              .with("database.port", properties.getProperty("database.port"))
              .with("database.user", properties.getProperty("database.user"))
              .with("database.password", properties.getProperty("database.password"))
              .with("table.whitelist", properties.getProperty("table.whitelist"))
              .with("database.history", "io.debezium.relational.history.FileDatabaseHistory")
              .with("database.history.file.filename", "src/main/resources/dbhistory.dat1")
              .build();

      System.out.println("a->"+properties.getProperty("table.whitelist"));
      log.info("Configure PostgeSQL Database");

    } catch (Exception e) {
      log.error("Unable to Configure PostgeSQL Database", e);
      throw new DAFCT2Exception("Unable to Configure PostgeSQL Database", e);
    }
  }

  @Override
  public void run() {

    this.embeddedEngine =
        EmbeddedEngine.create()
            .using(this.configuration)
             .using(this.getClass().getClassLoader())
              .using(Clock.SYSTEM)
            .notifying(this::handleEvent)
            .build();

    ExecutorService executorService = Executors.newSingleThreadExecutor();
    executorService.execute(this.embeddedEngine);

    Runtime.getRuntime()
        .addShutdownHook(
            new Thread(
                () -> {
                  log.info("Requesting Embedded Engine to shut down");
                  this.embeddedEngine.stop();
                }));

    // the submitted task keeps running, only no more new ones can be added
    executorService.shutdown();

    awaitTermination(executorService);

    log.info("Engine terminated");
  }

  private void handleEvent(SourceRecord sourceRecord) {
    Struct sourceRecordValue = (Struct) sourceRecord.value();
    //System.out.println("Records: " + sourceRecordValue);
    if (sourceRecordValue != null) {
      try {
        Operation operation = Operation.forCode((String) sourceRecordValue.get(OPERATION));

        // Only if this is a transactional operation.
        if (operation != Operation.READ) {

          // For Update & Insert operations.
          Struct struct = (Struct) sourceRecordValue.get(AFTER);
          String vid = struct.get("vid").toString();
          String vin = struct.get("vin").toString();
          System.out.println(vid+" "+vin);
        }

      } catch (Exception e) {
        // log.warn(e.getMessage());
      }
    }
  }

  private void awaitTermination(ExecutorService executorService) {

    try {
      while (!executorService.awaitTermination(10, TimeUnit.SECONDS)) {
        log.info("Waiting another 10 seconds for the Embedded Engine to complete");
      }

    } catch (InterruptedException e) {
      Thread.currentThread().interrupt();
    }
  }
}
