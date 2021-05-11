package net.atos.daf.ct2.processing;

import static io.debezium.data.Envelope.FieldName.AFTER;
import static io.debezium.data.Envelope.FieldName.OPERATION;

import java.util.Properties;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;

import org.apache.kafka.connect.data.Struct;
import org.apache.kafka.connect.source.SourceRecord;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import io.debezium.config.Configuration;
import io.debezium.embedded.EmbeddedEngine;
import net.atos.daf.ct2.exception.DAFCT2Exception;
import net.atos.daf.ct2.utils.Operation;

public class MessageProcessing implements Runnable {

  private Properties properties;
  private final Configuration configuration;
  private final Producer producer;
  private static final Logger log = LogManager.getLogger(MessageProcessing.class);

  private EmbeddedEngine embeddedEngine;

  public MessageProcessing(Configuration configuration, Properties properties)
      throws DAFCT2Exception {

    this.configuration = configuration;
    this.properties = properties;

    this.producer = new Producer();
    this.producer.createConnection(properties);
  }

  @Override
  public void run() {
    try {
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
                    // this.producer.closePublisher();
                  }));

      // the submitted task keeps running, only no more new ones can be added
      executorService.shutdown();

      awaitTermination(executorService);

      log.info("Engine terminated");

    } catch (Exception e) {
      log.error("Unable to process Postgre CDC ", e.getMessage());
    }
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
        // if (operation != Operation.READ) {
        Struct struct = (Struct) sourceRecordValue.get(AFTER);

        if (struct != null) {
          String vid = struct.get("vid").toString();
          String vin = struct.get("vin").toString();
        
          String status=struct.get("status").toString();
          String vinStatus= vin +"-" + status;
         

          if (vid != null && vin != null) {         
            this.producer.publishMessage(vid, vinStatus, this.properties);
            System.out.println("VINSTATUS: "+ vinStatus);
          }
        }

      } catch (Exception e) {
        // log.warn(e.getMessage());
      }
    }
  }
}
