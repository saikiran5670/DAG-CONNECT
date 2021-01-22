package net.atos.daf.ct2.poc;

import java.io.FileReader;
import java.io.IOException;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.Statement;
import java.util.Properties;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;

import net.atos.daf.ct2.exception.DAFCT2Exception;
import net.atos.daf.ct2.utils.Operation;
import org.apache.kafka.connect.data.Struct;
import org.apache.kafka.connect.source.SourceRecord;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import io.debezium.config.Configuration;
import io.debezium.embedded.EmbeddedEngine;

import static io.debezium.data.Envelope.FieldName.AFTER;
import static io.debezium.data.Envelope.FieldName.OPERATION;

public class Testing1 implements Runnable {

  private static final String FILE_PATH = "src/main/resources/configuration.properties";
  private static final Logger log = LogManager.getLogger(Testing1.class);

  private Configuration configuration;
  private EmbeddedEngine embeddedEngine;

  public static void main(String[] args) throws DAFCT2Exception {
    Testing1 testing = new Testing1();
    // testing.connectingPostgreSQL();
    // testing.run();
    testing.sqlConnection();
  }

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

  private void connectingPostgreSQL() throws DAFCT2Exception {
    Properties properties = configuration();

    try {
      this.configuration =
          Configuration.create()
              // io.debezium.connector.postgresql.PostgresConnector
              .with(EmbeddedEngine.CONNECTOR_CLASS, "io.debezium.connector.mysql.MySqlConnector")
              .with(
                  EmbeddedEngine.OFFSET_STORAGE,
                  "org.apache.kafka.connect.storage.FileOffsetBackingStore")
              .with(EmbeddedEngine.OFFSET_STORAGE_FILE_FILENAME, "src/main/resources/offset.dat")
              .with(EmbeddedEngine.OFFSET_FLUSH_INTERVAL_MS, 60000)
              .with("name", "PostgreCDC_Kafka_Streaming")
              .with("database.server.name", "postgre-connector")
              .with("database.server.id", 85749)
              .with("database.hostname", properties.getProperty("database.hostname"))
              .with("database.port", properties.getProperty("database.port"))
              .with("database.user", properties.getProperty("database.user"))
              .with("database.password", properties.getProperty("database.password"))
              .with("table.whitelist", properties.getProperty("table.whitelist"))
              .with("database.history", "io.debezium.relational.history.FileDatabaseHistory")
              .with("database.history.file.filename", "src/main/resources/dbhistory.dat")
              .build();

      log.info("Configure PostgeSQL Database");

    } catch (Exception e) {
      log.error("Unable to Configure PostgeSQL Database", e);
      throw new DAFCT2Exception("Unable to Configure PostgeSQL Database", e);
    }
  }

  @Override
  public void run() {

    this.embeddedEngine =
        EmbeddedEngine.create().using(this.configuration).notifying(this::handleEvent).build();

    ExecutorService executorService = Executors.newSingleThreadExecutor();
    executorService.execute(this.embeddedEngine);

    Runtime.getRuntime()
        .addShutdownHook(
            new Thread(
                () -> {
                  log.info("Requesting embedded engine to shut down");
                  this.embeddedEngine.stop();
                }));

    // the submitted task keeps running, only no more new ones can be added
    executorService.shutdown();

    awaitTermination(executorService);

    log.info("Engine terminated");
  }

  private void awaitTermination(ExecutorService executorService) {

    try {
      while (!executorService.awaitTermination(10, TimeUnit.SECONDS)) {
        log.info("Waiting another 10 seconds for the embedded engine to complete");
      }

    } catch (InterruptedException e) {
      Thread.currentThread().interrupt();
    }
  }

  private void handleEvent(SourceRecord sourceRecord) {

    Struct sourceRecordValue = (Struct) sourceRecord.value();

    if (sourceRecordValue != null) {
      try {
        Operation operation = Operation.forCode((String) sourceRecordValue.get(OPERATION));

        // Only if this is a transactional operation.
        if (operation != Operation.READ) {

          // For Update & Insert operations.
          Struct struct = (Struct) sourceRecordValue.get(AFTER);
          String vid = struct.get("id").toString();
          String vin = struct.get("business").toString();
          System.out.println(vid + " " + vin);
        }

      } catch (Exception e) {
        // log.warn(e.getMessage());
      }
    }
  }

  private void sqlConnection() {

    try {
      Class.forName("org.postgresql.Driver");
      Connection con =
          DriverManager.getConnection(
              "jdbc:postgresql://dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com:5432/dafconnectmasterdatabase",
              "pgadmin@dafct-dev0-dta-cdp-pgsql",
              "W%PQ1AI}Y\\97");

      Statement stmt = con.createStatement();
      ResultSet rs = stmt.executeQuery("select * from master.vehicle");

      while (rs.next())
        System.out.println(rs.getInt(1) + "  " + rs.getString(2) + "  " + rs.getString(3));

      con.close();
    } catch (Exception e) {
      System.out.println(e);
    }
  }
}
