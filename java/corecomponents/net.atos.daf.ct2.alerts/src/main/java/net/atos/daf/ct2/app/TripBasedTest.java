package net.atos.daf.ct2.app;

import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.util.Utils;
import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.common.serialization.SimpleStringSchema;
import org.apache.flink.api.java.io.jdbc.JDBCOutputFormat;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.connector.jdbc.JdbcConnectionOptions;
import org.apache.flink.connector.jdbc.JdbcExecutionOptions;
import org.apache.flink.connector.jdbc.JdbcSink;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.ObjectMapper;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.KeyedStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.flink.types.Row;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.io.Serializable;
import java.math.BigInteger;
import java.util.Properties;

import static net.atos.daf.ct2.props.AlertConfigProp.*;

public class TripBasedTest implements Serializable {
    private static final Logger logger = LoggerFactory.getLogger(TripBasedTest.class);
    private static final long serialVersionUID = 1L;

    public static void main(String[] args) throws Exception {
        final StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();
        ParameterTool parameterTool = ParameterTool.fromArgs(args);
        logger.info("TripBasedTest started with properties :: {}", parameterTool.getProperties());
        ParameterTool propertiesParamTool = ParameterTool.fromPropertiesFile(parameterTool.get("prop"));
        env.getConfig().setGlobalJobParameters(propertiesParamTool);
        logger.info("PropertiesParamTool :: {}", parameterTool.getProperties());

//        Properties kafkaTopicProp = Utils.getKafkaConnectProperties(propertiesParamTool);
        Properties kafkaTopicProp = new Properties();
        kafkaTopicProp.put("bootstrap.servers", "localhost:9092");
        kafkaTopicProp.put("client.id", "alertsprocessing_client");
        kafkaTopicProp.put("group.id", "alertsprocessing_grp");
        kafkaTopicProp.put("group.id", "alertsprocessing_grp");
        kafkaTopicProp.put("auto.offset.reset", "earliest");

        Alert alert = Alert.builder()
                .tripid("s03bf625a-cce7-42b8-a712-7a5a161b1d003")
                .vin("XLR0998HGFFT76657")
                .categoryType("L")
                .type("G")
                .alertid("367")
                .alertGeneratedTime(String.valueOf(System.currentTimeMillis()))
                .thresholdValue("2000")
                .thresholdValueUnitType("M")
                .valueAtAlertTime("25433")
                .urgencyLevelType("A")
                .build();

        FlinkKafkaConsumer<String> kafkaContiMessageConsumer = new FlinkKafkaConsumer<>(propertiesParamTool.get(KAFKA_DAF_STATUS_MSG_TOPIC), new SimpleStringSchema(), kafkaTopicProp);

        SingleOutputStreamOperator<Alert> alertKafkaStream = env.addSource(kafkaContiMessageConsumer)
                .map(json -> alert)
                .returns(Alert.class);





        String jdbcInsertUrl = new StringBuilder("jdbc:postgresql://")
                .append("localhost")
                .append(":" + "5432" + "/")
                .append("postgres")
                .append("?user=" + "postgres")
                .append("&password=" + "root")
                .toString();

        String query = "INSERT INTO public.tripalert(trip_id, vin, category_type, type, alert_id, alert_generated_time, created_at, urgency_level_type) VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

//        env
//                .fromElements(alert)


//        JDBCOutputFormat jdbcOutput = JDBCOutputFormat.buildJDBCOutputFormat()
//                .setDrivername("org.postgresql.Driver")
//                .setDBUrl(jdbcInsertUrl)
//                .setQuery(query)
//                .finish();
//
//        DataStream<Row> rows = alertKafkaStream.map((MapFunction<Alert, Row>) a -> {
//            Row row = new Row(8);
//            /*row.setField(0, aCase.getId());
//            row.setField(1, aCase.getTraceHash());*/
//            row.setField(0, a.getTripid());
//            row.setField(1, a.getVin());
//            row.setField(2, a.getCategoryType());
//            row.setField(3, a.getType());
//            row.setField(4, Long.valueOf(a.getAlertid()));
//            row.setField(5, Long.valueOf(a.getAlertGeneratedTime()));
//            row.setField(6, Long.valueOf(a.getAlertGeneratedTime()));
//            row.setField(7, a.getUrgencyLevelType());
//            return row;
//        });

        alertKafkaStream.print();

//        rows.writeUsingOutputFormat(jdbcOutput);


        alertKafkaStream
                .addSink(JdbcSink.sink(
                        query,
                        (ps, a) -> {
                            ps.setString(1, a.getTripid());
                            ps.setString(2, a.getVin());
                            ps.setString(3, a.getCategoryType());
                            ps.setString(4, a.getType());
                            ps.setLong(5, Long.valueOf(a.getAlertid()));
                            ps.setLong(6, Long.valueOf(a.getAlertGeneratedTime()));
                            ps.setLong(7, Long.valueOf(a.getAlertGeneratedTime()));
                            ps.setString(8, a.getUrgencyLevelType());
                        },
                        JdbcExecutionOptions.builder()
                                .withMaxRetries(0)
                                .withBatchSize(1)
                                .build(),
                        new JdbcConnectionOptions.JdbcConnectionOptionsBuilder()
                                .withUrl(jdbcInsertUrl)
                                .withDriverName(propertiesParamTool.get("driver.class.name"))
                                .build())
                );


        env.execute("TripBasedTest");

    }
}
