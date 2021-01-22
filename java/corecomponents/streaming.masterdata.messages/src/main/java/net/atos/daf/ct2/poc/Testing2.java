package net.atos.daf.ct2.poc;

//import com.alibaba.ververica.cdc.connectors.postgres.PostgreSQLSource;
//import com.alibaba.ververica.cdc.debezium.StringDebeziumDeserializationSchema;

public class Testing2 {

  public static void main(String[] args) throws Exception {
    /*SourceFunction<String> sourceFunction =
        PostgreSQLSource.<String>builder()
            .hostname("dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com")
            .port(5432)
            .database("dafconnectmasterdatabase") // monitor all tables under inventory database
            .username("pgadmin@dafct-dev0-dta-cdp-pgsql")
            .password("W%PQ1AI}Y\\97")
            .schemaList("master")
            .tableList("vehicle")
            .decodingPluginName("pgoutput")
            .deserializer(
                new StringDebeziumDeserializationSchema())
            .build();

    StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();

    env.addSource(sourceFunction)
        .print()
        .setParallelism(1); // use parallelism 1 for sink to keep message ordering

    env.execute();*/
  }
}
