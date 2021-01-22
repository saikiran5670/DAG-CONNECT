package net.atos.daf.ct2.poc;

import io.debezium.config.Configuration;
import io.debezium.connector.postgresql.PostgresConnectorConfig;
import io.debezium.embedded.EmbeddedEngine;
import io.debezium.util.Clock;
import org.apache.kafka.connect.source.SourceRecord;

import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;

public class ChangeDataSender implements Runnable {

    private String STUDENT_TABLE_NAME = "public.driver";

    private final Configuration config;

    private EmbeddedEngine engine;

    public ChangeDataSender() {
    config =
        Configuration.create()
            .with("connector.class", "io.debezium.connector.postgresql.PostgresConnector")
            .with("offset.storage", "org.apache.kafka.connect.storage.FileOffsetBackingStore")
            .with(
                "offset.storage.file.filename",
                "/Users/rbg831/Documents/Sohan/Projects/POC/embedded-debezium/student-cdc-relay/student-offset.dat")
            .with("offset.flush.interval.ms", 60000)
            .with("name", "student-postgres-connector")
            .with("database.server.name", "dafconnectmasterdatabase" + "-" + "studentDBName")
            .with("database.hostname", "dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com")
            .with("database.port", "5432")
            .with("database.user", "pgadmin@dafct-dev0-dta-cdp-pgsql")
            .with("database.password", "W%PQ1AI}Y\\97")
            .with("database.dbname", "dafconnectmasterdatabase")
            .with(PostgresConnectorConfig.PLUGIN_NAME, "pgoutput")
            .with("table.whitelist", STUDENT_TABLE_NAME)
            .build();
    }

    @Override
    public void run() {
        engine = EmbeddedEngine.create()
                .using(config)
                .using(this.getClass().getClassLoader())
                .using(Clock.SYSTEM)
                .notifying(this::sendRecord)
                .build();

        ExecutorService executor = Executors.newSingleThreadExecutor();
        executor.execute(engine);

        Runtime.getRuntime().addShutdownHook(new Thread(() -> {
           // LOGGER.info("Requesting embedded engine to shut down");
            engine.stop();
        }));

        // the submitted task keeps running, only no more new ones can be added
        executor.shutdown();

        awaitTermination(executor);



        //LOGGER.info("Engine terminated");
    }

    private void awaitTermination(ExecutorService executor) {
        try {
            while (!executor.awaitTermination(10, TimeUnit.SECONDS)) {
               // LOGGER.info("Waiting another 10 seconds for the embedded engine to complete");
            }
        }
        catch (InterruptedException e) {
            Thread.currentThread().interrupt();
        }
    }

    private void sendRecord(SourceRecord record) {
        // We are interested only in data events not schema change events
       System.out.println(record);
    }

    private String streamNameMapper(String topic) {
        return topic;
    }

    public static void main(String[] args) {
        new ChangeDataSender().run();
    }
}
