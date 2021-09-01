package net.atos.daf.ct2.app;

import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.hoursOfServiceFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.excessiveUnderUtilInHrsFun;
import static net.atos.daf.ct2.props.AlertConfigProp.DATAMART_DATABASE;
import static net.atos.daf.ct2.props.AlertConfigProp.DATAMART_PASSWORD;
import static net.atos.daf.ct2.props.AlertConfigProp.DATAMART_POSTGRES_HOST;
import static net.atos.daf.ct2.props.AlertConfigProp.DATAMART_POSTGRES_PORT;
import static net.atos.daf.ct2.props.AlertConfigProp.DATAMART_POSTGRES_SSL;
import static net.atos.daf.ct2.props.AlertConfigProp.DATAMART_USERNAME;
import static net.atos.daf.ct2.props.AlertConfigProp.KAFKA_DAF_ALERT_PRODUCE_MSG_TOPIC;
import static net.atos.daf.ct2.props.AlertConfigProp.KAFKA_DAF_STATUS_MSG_TOPIC;
import static net.atos.daf.ct2.props.AlertConfigProp.KAFKA_EGRESS_INDEX_MSG_TOPIC;
import static net.atos.daf.ct2.props.AlertConfigProp.OUTPUT_TAG;
import static net.atos.daf.ct2.props.AlertConfigProp.vinAlertMapStateDescriptor;
import static net.atos.daf.ct2.util.Utils.convertDateToMillis;

import java.io.Serializable;
import java.text.SimpleDateFormat;
import java.time.Duration;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.ListIterator;
import java.util.Map;
import java.util.Properties;
import java.util.Set;
import java.util.concurrent.TimeUnit;
import java.util.stream.Collectors;

import org.apache.flink.api.common.eventtime.WatermarkStrategy;
import org.apache.flink.api.common.functions.AggregateFunction;
import org.apache.flink.api.common.functions.ReduceFunction;
import org.apache.flink.api.common.serialization.SimpleStringSchema;
import org.apache.flink.api.common.state.ReadOnlyBroadcastState;
import org.apache.flink.streaming.api.windowing.time.Time;
import org.apache.flink.streaming.api.windowing.windows.TimeWindow;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.connector.jdbc.JdbcConnectionOptions;
import org.apache.flink.connector.jdbc.JdbcExecutionOptions;
import org.apache.flink.connector.jdbc.JdbcSink;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.KeyedStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.datastream.WindowedStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.functions.co.KeyedBroadcastProcessFunction;
import org.apache.flink.streaming.api.functions.timestamps.BoundedOutOfOrdernessTimestampExtractor;
import org.apache.flink.streaming.api.functions.windowing.ProcessWindowFunction;
import org.apache.flink.streaming.api.windowing.assigners.TumblingEventTimeWindows;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.cache.kafka.KafkaCdcStreamV2;
import net.atos.daf.ct2.cache.kafka.impl.KafkaCdcImplV2;
import net.atos.daf.ct2.cache.postgres.TableStream;
import net.atos.daf.ct2.cache.postgres.impl.JdbcFormatTableStream;
import net.atos.daf.ct2.cache.service.CacheService;
import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.process.config.AlertConfig;
import net.atos.daf.ct2.serialization.PojoKafkaSerializationSchema;
import net.atos.daf.ct2.service.kafka.KafkaConnectionService;
import net.atos.daf.ct2.service.realtime.IndexKeyBasedAlertDefService;
import net.atos.daf.ct2.service.realtime.IndexKeyBasedSubscription;
import net.atos.daf.ct2.util.IndexGenerator;
import net.atos.daf.ct2.util.Utils;
import one.util.streamex.StreamEx;
import java.util.stream.Stream;

public class PeriodBasedAlertProcessing implements Serializable {

	private static final Logger logger = LoggerFactory.getLogger(PeriodBasedAlertProcessing.class);
	private static final long serialVersionUID = 1L;

	public static void main(String[] args) throws Exception {
        final StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();
        ParameterTool parameterTool = ParameterTool.fromArgs(args);
        logger.info("Period-based AlertProcessing started with properties :: {}", parameterTool.getProperties());
        /**
         * Creating param tool from given property
         */
        ParameterTool propertiesParamTool = ParameterTool.fromPropertiesFile(parameterTool.get("prop"));
        env.getConfig().setGlobalJobParameters(propertiesParamTool);

        logger.info("Period-based PropertiesParamTool :: {}", parameterTool.getProperties());

        /**
         * Logistics functions defined
         */
		Map<Object, Object> configMap = new HashMap() {{
            put("functions", Arrays.asList(
            		excessiveUnderUtilInHrsFun
            ));
        }};

        /**
         * Kafka topic connector properties
         */
        Properties kafkaTopicProp = Utils.getKafkaConnectProperties(propertiesParamTool);
        FlinkKafkaConsumer<String> kafkaContiMessageConsumer = new FlinkKafkaConsumer<>(
        		propertiesParamTool.get(KAFKA_EGRESS_INDEX_MSG_TOPIC), 
        		new SimpleStringSchema(), 
        		kafkaTopicProp);
        

        String dafAlertProduceTopic = propertiesParamTool.get(KAFKA_DAF_ALERT_PRODUCE_MSG_TOPIC);
        FlinkKafkaProducer<Alert> alertProducerTopic = new FlinkKafkaProducer<Alert>(dafAlertProduceTopic, new PojoKafkaSerializationSchema(dafAlertProduceTopic), kafkaTopicProp, FlinkKafkaProducer.Semantic.EXACTLY_ONCE);

        /*
         * Booting cache
         */
        KafkaCdcStreamV2 kafkaCdcStreamV2 = new KafkaCdcImplV2(env,propertiesParamTool);
        Tuple2<BroadcastStream<VehicleAlertRefSchema>, BroadcastStream<Payload<Object>>> bootCache = kafkaCdcStreamV2.bootCache();

        BroadcastStream<VehicleAlertRefSchema> vehicleAlertRefSchemaBroadcastStream = bootCache.f0;
        BroadcastStream<Payload<Object>> alertUrgencyLevelRefSchemaBroadcastStream = bootCache.f1;
        
		/*
		 * SingleOutputStreamOperator<Index>
		 * indexStringStream=KafkaConnectionService.connectIndexObjectTopic(
		 * propertiesParamTool.get(KAFKA_EGRESS_INDEX_MSG_TOPIC), propertiesParamTool,
		 * env) .map(indexKafkaRecord -> indexKafkaRecord.getValue())
		 * .returns(Index.class);
		 */
        SingleOutputStreamOperator<Index> indexStringStream = env.addSource(new IndexGenerator());
        
        /**
         * Consume index message in event time windows of 30 mins
         */
        long WindowTime = Long.valueOf(propertiesParamTool.get("index.excessive.under.utilization.window.secs","1800"));
        
        KeyedStream<Tuple2<Index, Payload<Set<Long>>>, String> subscribeVehicleStream = indexStringStream
        		.assignTimestampsAndWatermarks(
                		WatermarkStrategy
                		.<Index>forBoundedOutOfOrderness(Duration.ofSeconds(0))
                		.withTimestampAssigner((indexEvt,timestamp) -> convertDateToMillis(indexEvt.getEvtDateTime())
                ))
        		.keyBy(index -> index.getVin() !=null ? index.getVin() : index.getVid())
                .window(TumblingEventTimeWindows.of(Time.seconds(6))) // assign WindowTime during deployment
                .process(new ProcessWindowFunction<Index, Index, String, TimeWindow>() {
					private static final long serialVersionUID = 1L;
					@Override
					public void process(String key,
							ProcessWindowFunction<Index, Index, String, TimeWindow>.Context context,
							Iterable<Index> input, Collector<Index> out) throws Exception {
						net.atos.daf.ct2.models.Index result = new net.atos.daf.ct2.models.Index();
						for(Index ind : input) {
							result.getList().add(ind);
							result.setVin(ind.getVin());
						}
						out.collect(result);
					}
                })
                .keyBy(index -> index.getVin() !=null ? index.getVin() : index.getVid())
                .connect(vehicleAlertRefSchemaBroadcastStream)
                .process(new IndexKeyBasedSubscription())
                .keyBy(tup2 -> tup2.f0.getVin());

        /**
         * Check for alert threshold definition
         */
        SingleOutputStreamOperator<Index> alertProcessStream = subscribeVehicleStream
                .connect(alertUrgencyLevelRefSchemaBroadcastStream)
                .process(new IndexKeyBasedAlertDefService(configMap));
        
		
        /**
         * Publish alert on kafka topic
         */
        DataStream<Alert> alertFoundStream = alertProcessStream
                .getSideOutput(OUTPUT_TAG);

        env.execute("TripBasedAlertProcessing");

    }

}
