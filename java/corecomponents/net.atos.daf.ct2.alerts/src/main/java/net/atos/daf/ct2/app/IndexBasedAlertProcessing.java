package net.atos.daf.ct2.app;

import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.excessiveAverageSpeedFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.excessiveIdlingFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.excessiveUnderUtilizationInHoursFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.fuelDecreaseDuringStopFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.fuelIncreaseDuringStopFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.hoursOfServiceFun;
//import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.excessiveIdlingFun;
import static net.atos.daf.ct2.props.AlertConfigProp.KAFKA_EGRESS_INDEX_MSG_TOPIC;
import static net.atos.daf.ct2.util.Utils.convertDateToMillis;

import java.io.Serializable;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Objects;
import java.util.stream.Collectors;
import java.util.stream.StreamSupport;

import net.atos.daf.ct2.service.realtime.ExcessiveAverageSpeedService;
import org.apache.flink.api.common.functions.ReduceFunction;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.KeyedStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.datastream.WindowedStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.functions.timestamps.BoundedOutOfOrdernessTimestampExtractor;
import org.apache.flink.streaming.api.functions.windowing.ProcessWindowFunction;
import org.apache.flink.streaming.api.windowing.assigners.TumblingEventTimeWindows;
import org.apache.flink.streaming.api.windowing.time.Time;
import org.apache.flink.streaming.api.windowing.windows.TimeWindow;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.cache.kafka.KafkaCdcStreamV2;
import net.atos.daf.ct2.cache.kafka.impl.KafkaCdcImplV2;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.props.AlertConfigProp;
import net.atos.daf.ct2.service.kafka.KafkaConnectionService;
import net.atos.daf.ct2.service.realtime.ExcessiveUnderUtilizationProcessor;
import net.atos.daf.ct2.service.realtime.FuelDuringStopProcessor;
import net.atos.daf.ct2.service.realtime.IndexMessageAlertService;
import net.atos.daf.ct2.util.Utils;

public class IndexBasedAlertProcessing implements Serializable {
    private static final Logger logger = LoggerFactory.getLogger(IndexBasedAlertProcessing.class);
    private static final long serialVersionUID = 1L;

    public static void main(String[] args) throws Exception {
        final StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();
        ParameterTool parameterTool = ParameterTool.fromArgs(args);
        logger.info("AlertProcessing started with properties :: {}", parameterTool.getProperties());
        /**
         * Creating param tool from given property
         */
        ParameterTool propertiesParamTool = ParameterTool.fromPropertiesFile(parameterTool.get("prop"));

        logger.info("PropertiesParamTool :: {}", parameterTool.getProperties());

        /**
         * RealTime functions defined
         */
        Map<Object, Object> hoursOfServiceFunConfigMap = new HashMap() {{
            put("functions", Arrays.asList(
                    hoursOfServiceFun
            ));
        }};
        /**
         * RealTime functions defined
         */
        Map<Object, Object> excessiveAverageSpeedFunConfigMap = new HashMap() {{
            put("functions", Arrays.asList(
                    excessiveAverageSpeedFun,
                    excessiveIdlingFun
            ));
        }};

        /**
         * RealTime functions defined
         */
        Map<Object, Object> excessiveUnderUtilizationFunConfigMap = new HashMap() {{
            put("functions", Arrays.asList(
                    excessiveUnderUtilizationInHoursFun
            ));
        }};
        
        /**
         * RealTime functions defined
         */
        Map<Object, Object> fuelDuringStopFunConfigMap = new HashMap() {{
            put("functions", Arrays.asList(
            		fuelIncreaseDuringStopFun,
            		fuelDecreaseDuringStopFun
            ));
        }};

        /**
         *  Booting cache
         */
        KafkaCdcStreamV2 kafkaCdcStreamV2 = new KafkaCdcImplV2(env,propertiesParamTool);
        Tuple2<BroadcastStream<VehicleAlertRefSchema>, BroadcastStream<Payload<Object>>> bootCache = kafkaCdcStreamV2.bootCache();

        AlertConfigProp.vehicleAlertRefSchemaBroadcastStream = bootCache.f0;
        AlertConfigProp.alertUrgencyLevelRefSchemaBroadcastStream = bootCache.f1;

        SingleOutputStreamOperator<Index> indexStringStream = KafkaConnectionService.connectIndexObjectTopic(
                        propertiesParamTool.get(KAFKA_EGRESS_INDEX_MSG_TOPIC),
                        propertiesParamTool, env)
                .map(indexKafkaRecord -> indexKafkaRecord.getValue())
                .returns(Index.class)
                .filter(index -> index.getVid() != null)
                .returns(Index.class);

        /*SingleOutputStreamOperator<Index> indexStringStream = env.addSource(new IndexGenerator())
                .returns(Index.class);*/

        /**
         * Window time in milliseconds
         */
        long WindowTime = Long.valueOf(propertiesParamTool.get("index.hours.of.service.window.millis","300000"));

        /**
         * Index Window Stream
         */
        WindowedStream<Index, String, TimeWindow> windowedIndexStream = indexStringStream
                .assignTimestampsAndWatermarks(
                        new BoundedOutOfOrdernessTimestampExtractor<Index>(Time.milliseconds(0)) {
                            @Override
                            public long extractTimestamp(Index index) {
                                return convertDateToMillis(index.getEvtDateTime());
                            }
                        }
                )
                .filter(index -> index.getDocument().getTripID() !=null)
                .returns(Index.class)
                .keyBy(index -> index.getDocument().getTripID() != null ? index.getDocument().getTripID() : "")
                .window(TumblingEventTimeWindows.of(Time.milliseconds(WindowTime)));

        /**
         * Hours of service keyed stream
         */
        KeyedStream<Index, String> indexWindowKeyedStream =
                windowedIndexStream
                .reduce(new ReduceFunction<Index>() {
                    @Override
                    public Index reduce(Index index, Index t1) throws Exception {
                        return t1;
                    }
                })
                .keyBy(index -> index.getVin() != null ? index.getVin() : index.getVid());


        /**
         * Process indexWindowKeyedStream for Hours of service
         */
        IndexMessageAlertService.processIndexKeyStream(indexWindowKeyedStream,
                env,propertiesParamTool,hoursOfServiceFunConfigMap);


        /**
         * Excessive Average Speed Fun keyed stream
         */
        KeyedStream<Index, String> indexExcessiveAvgSpeedKeyedStream = windowedIndexStream
                .process(new ExcessiveAverageSpeedService())
                .filter(index -> index.getDocument().getTripID() !=null)
                .returns(Index.class)
                .keyBy(index -> index.getDocument().getTripID() != null ? index.getDocument().getTripID() : "");


        /**
         * Process indexWindowKeyedStream for Hours of service
         */
        IndexMessageAlertService.processIndexKeyStream(indexExcessiveAvgSpeedKeyedStream,
                env,propertiesParamTool,excessiveAverageSpeedFunConfigMap);


        /**
         * Excessive Under Utilization In Hours
         */
        long WindowTimeExcessiveUnderUtilization = Long.valueOf(propertiesParamTool.get("index.excessive.under.utilization.window.seconds","1800"));
        WindowedStream<Index, String, TimeWindow> windowedExcessiveUnderUtilizationStream = indexStringStream
                .assignTimestampsAndWatermarks(
                        new BoundedOutOfOrdernessTimestampExtractor<Index>(Time.seconds(0)) {
                            @Override
                            public long extractTimestamp(Index index) {
                                return convertDateToMillis(index.getEvtDateTime());
                            }
                        }
                )
                .keyBy(index -> index.getVin() != null ? index.getVin() : index.getVid())
                .window(TumblingEventTimeWindows.of(Time.seconds(WindowTimeExcessiveUnderUtilization)));

        KeyedStream<Index, String> excessiveUnderUtilizationProcessStream = windowedExcessiveUnderUtilizationStream
               .process(new ExcessiveUnderUtilizationProcessor())
                .keyBy(index -> index.getVin() != null ? index.getVin() : index.getVid());

        /**
         * Process indexWindowKeyedStream for Excessive Under Utilization
         */
        IndexMessageAlertService.processIndexKeyStream(excessiveUnderUtilizationProcessStream,
                env,propertiesParamTool,excessiveUnderUtilizationFunConfigMap);
        
        
        /**
         * Excessive Fuel during stop
         */
        KeyedStream<Index, String> indexStringKeyedStream = indexStringStream
                .filter( index -> Objects.nonNull(index.getVEvtID()) && (4 == index.getVEvtID() || 5 == index.getVEvtID()) )
                .returns(Index.class)
                .keyBy(index -> index.getVin() != null ? index.getVin() : index.getVid())
                .process(new FuelDuringStopProcessor()).keyBy(index -> index.getVin() != null ? index.getVin() : index.getVid());

        /**
         * Process indexWindowKeyedStream for Excessive Under Utilization
         */
        IndexMessageAlertService.processIndexKeyStream(indexStringKeyedStream,
                env,propertiesParamTool,fuelDuringStopFunConfigMap);



        env.execute(IndexBasedAlertProcessing.class.getSimpleName());


    }
}
