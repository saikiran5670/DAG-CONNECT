package net.atos.daf.ct2.app;

import com.fasterxml.jackson.databind.module.SimpleDeserializers;
import net.atos.daf.ct2.cache.kafka.KafkaCdcStreamV2;
import net.atos.daf.ct2.cache.kafka.impl.KafkaCdcImplV2;
import net.atos.daf.ct2.cache.postgres.TableStream;
import net.atos.daf.ct2.cache.postgres.impl.JdbcFormatTableStream;
import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.props.AlertConfigProp;
import net.atos.daf.ct2.serde.KafkaMessageDeSerializeSchema;
import net.atos.daf.ct2.serialization.PojoKafkaSerializationSchema;
import net.atos.daf.ct2.service.kafka.KafkaConnectionService;
import net.atos.daf.ct2.service.kafka.KafkaService;
import net.atos.daf.ct2.service.realtime.ExcessiveUnderUtilizationProcessor;
import net.atos.daf.ct2.service.realtime.FuelDuringStopProcessor;
import net.atos.daf.ct2.service.realtime.IndexKeyBasedSubscription;
import net.atos.daf.ct2.service.realtime.IndexMessageAlertService;
import net.atos.daf.ct2.util.IndexGenerator;
import net.atos.daf.ct2.util.Utils;
import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.common.functions.ReduceFunction;
import org.apache.flink.api.common.serialization.SimpleStringSchema;
import org.apache.flink.api.common.state.MapState;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.state.ValueState;
import org.apache.flink.api.common.state.ValueStateDescriptor;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.io.jdbc.JDBCOutputFormat;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.configuration.Configuration;
import org.apache.flink.connector.jdbc.JdbcConnectionOptions;
import org.apache.flink.connector.jdbc.JdbcExecutionOptions;
import org.apache.flink.connector.jdbc.JdbcSink;
import org.apache.flink.shaded.jackson2.com.fasterxml.jackson.databind.ObjectMapper;
import org.apache.flink.streaming.api.datastream.*;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.functions.ProcessFunction;
import org.apache.flink.streaming.api.functions.timestamps.BoundedOutOfOrdernessTimestampExtractor;
import org.apache.flink.streaming.api.functions.windowing.ProcessWindowFunction;
import org.apache.flink.streaming.api.windowing.assigners.TumblingEventTimeWindows;
import org.apache.flink.streaming.api.windowing.time.Time;
import org.apache.flink.streaming.api.windowing.windows.TimeWindow;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.apache.flink.streaming.connectors.kafka.KafkaDeserializationSchema;
import org.apache.flink.types.Row;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.io.Serializable;
import java.math.BigDecimal;
import java.math.BigInteger;
import java.util.*;
import java.util.stream.Collectors;
import java.util.stream.StreamSupport;

import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.excessiveAverageSpeedFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.excessiveIdlingFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.excessiveUnderUtilizationInHoursFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.fuelIncreaseDuringStopFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.fuelDecreaseDuringStopFun;
import static net.atos.daf.ct2.props.AlertConfigProp.*;
import static net.atos.daf.ct2.util.Utils.*;

public class TripBasedTest implements Serializable {
    private static final Logger logger = LoggerFactory.getLogger(TripBasedTest.class);
    private static final long serialVersionUID = 1L;


    public static void main(String[] args) throws Exception {
        final StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();
        ParameterTool parameterTool = ParameterTool.fromArgs(args);
        logger.info("TripBasedTest started with properties :: {}", parameterTool.getProperties());
        ParameterTool propertiesParamTool = ParameterTool.fromPropertiesFile(parameterTool.get("prop"));
        logger.info("PropertiesParamTool :: {}", propertiesParamTool.getProperties());

        /**
         * Alert produce topic
         */
        String dafAlertProduceTopic = propertiesParamTool.get(KAFKA_DAF_ALERT_PRODUCE_MSG_TOPIC);
        Properties kafkaTopicProp = Utils.getKafkaConnectProperties(propertiesParamTool);
        KafkaDeserializationSchema deserializationSchema= new KafkaMessageDeSerializeSchema<String>();

        FlinkKafkaConsumer<String> alertConsumer = new FlinkKafkaConsumer<String>
                (dafAlertProduceTopic,deserializationSchema,kafkaTopicProp);

        env.addSource(alertConsumer)
                        .print();




        env.execute("TripBasedTest");


    }
}
