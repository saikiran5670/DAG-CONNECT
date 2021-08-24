package net.atos.daf.ct2.app;

import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.service.kafka.KafkaConnectionService;
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

        Properties kafkaTopicProp = Utils.getKafkaConnectProperties(propertiesParamTool);

        logger.info("kafkaTopicProp :: {}" , kafkaTopicProp.entrySet());

         KafkaConnectionService.connectStatusObjectTopic(
                        propertiesParamTool.get(KAFKA_DAF_STATUS_MSG_TOPIC),
                        propertiesParamTool,
                        env)
                .map(statusKafkaRecord -> statusKafkaRecord.getValue())
                .returns(net.atos.daf.ct2.pojo.standard.Status.class)
                .keyBy(status -> status.getVin() !=null ? status.getVin() : status.getVid())
                 .print();

        env.execute("TripBasedTest");


    }
}
