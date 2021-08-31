package net.atos.daf.ct2.app;

import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.excessiveAverageSpeedFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.hoursOfServiceFun;
import static net.atos.daf.ct2.props.AlertConfigProp.KAFKA_DAF_ALERT_PRODUCE_MSG_TOPIC;
import static net.atos.daf.ct2.props.AlertConfigProp.KAFKA_EGRESS_INDEX_MSG_TOPIC;
import static net.atos.daf.ct2.props.AlertConfigProp.OUTPUT_TAG;
import static net.atos.daf.ct2.util.Utils.convertDateToMillis;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Properties;
import java.util.Set;
import java.util.stream.Collectors;
import java.util.stream.StreamSupport;

import net.atos.daf.ct2.util.IndexGenerator;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.KeyedStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.functions.timestamps.BoundedOutOfOrdernessTimestampExtractor;
import org.apache.flink.streaming.api.functions.windowing.ProcessWindowFunction;
import org.apache.flink.streaming.api.windowing.assigners.TumblingEventTimeWindows;
import org.apache.flink.streaming.api.windowing.time.Time;
import org.apache.flink.streaming.api.windowing.windows.TimeWindow;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.cache.kafka.KafkaCdcStreamV2;
import net.atos.daf.ct2.cache.kafka.impl.KafkaCdcImplV2;
import net.atos.daf.ct2.cache.postgres.TableStream;
import net.atos.daf.ct2.cache.postgres.impl.JdbcFormatTableStream;
import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.serialization.PojoKafkaSerializationSchema;
import net.atos.daf.ct2.service.kafka.KafkaConnectionService;
import net.atos.daf.ct2.service.realtime.IndexKeyBasedAlertDefService;
import net.atos.daf.ct2.service.realtime.IndexKeyBasedSubscription;
import net.atos.daf.ct2.util.Utils;

public class IndexBasedAlertProcessing2 {
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
        Map<Object, Object> configMap = new HashMap() {{
            put("functions", Arrays.asList(
                    excessiveAverageSpeedFun
            ));
        }};

        /**
         * Alert produce topic
         */
        String dafAlertProduceTopic = propertiesParamTool.get(KAFKA_DAF_ALERT_PRODUCE_MSG_TOPIC);
        Properties kafkaTopicProp = Utils.getKafkaConnectProperties(propertiesParamTool);
        FlinkKafkaProducer<Alert> alertProducerTopic = new FlinkKafkaProducer<Alert>(dafAlertProduceTopic, new PojoKafkaSerializationSchema(dafAlertProduceTopic), kafkaTopicProp, FlinkKafkaProducer.Semantic.EXACTLY_ONCE);


        /**
         *  Booting cache
         */
        KafkaCdcStreamV2 kafkaCdcStreamV2 = new KafkaCdcImplV2(env, propertiesParamTool);
        Tuple2<BroadcastStream<VehicleAlertRefSchema>, BroadcastStream<Payload<Object>>> bootCache = kafkaCdcStreamV2.bootCache();

        BroadcastStream<VehicleAlertRefSchema> vehicleAlertRefSchemaBroadcastStream = bootCache.f0;
        BroadcastStream<Payload<Object>> alertUrgencyLevelRefSchemaBroadcastStream = bootCache.f1;


        /*SingleOutputStreamOperator<Index> indexKeyedStream = KafkaConnectionService.connectIndexObjectTopic(
                        propertiesParamTool.get(KAFKA_EGRESS_INDEX_MSG_TOPIC), propertiesParamTool, env)
                .map(indexKafkaRecord -> indexKafkaRecord.getValue())
                .returns(net.atos.daf.ct2.pojo.standard.Index.class);*/

        SingleOutputStreamOperator<Index> indexKeyedStream = env.addSource(new IndexGenerator())
                .returns(Index.class);


        /**
         * Window time in milliseconds
         */
        long WindowTime = Long.valueOf(propertiesParamTool.get("index.hours.of.service.window.millis", "300000"));


        KeyedStream<Tuple2<Index, Payload<Set<Long>>>, String> subscribeVehicleStream = indexKeyedStream
                .assignTimestampsAndWatermarks(
                        new BoundedOutOfOrdernessTimestampExtractor<Index>(Time.milliseconds(0)) {
                            @Override
                            public long extractTimestamp(Index index) {
                                return convertDateToMillis(index.getEvtDateTime());
                            }
                        }
                )
                .keyBy(index -> index.getDocument() != null ? index.getDocument().getTripID() : "null")
                .window(TumblingEventTimeWindows.of(Time.milliseconds(WindowTime)))
                .process(new ProcessWindowFunction<Index, Index, String, TimeWindow>() {

                    @Override
                    public void process(String arg0, ProcessWindowFunction<Index, Index, String, TimeWindow>.Context arg1,
                                        Iterable<Index> indexMsg, Collector<Index> arg3) throws Exception {
                        List<Index> indexList = StreamSupport.stream(indexMsg.spliterator(), false)
                                .collect(Collectors.toList());

                        logger.info("index list size  :" + indexList.size());
                        if (!indexList.isEmpty()) {
                            Index startIndex = indexList.get(0);
                            Index endIndex = indexList.get(indexList.size() - 1);
                            Long average = Utils.calculateAverage(startIndex, endIndex);
                            startIndex.setVDist(average);
                            arg3.collect(startIndex);
                        }
                    }
                })


                /*
                 * .reduce(new ReduceFunction<Index>() {
                 *
                 * @Override public Index reduce(Index index1, Index index2) throws Exception {
                 * long average= calculateAverage(index1,index2); index2.setVDist(average) ;
                 * return index2; } })
                 */

                .keyBy(index -> index.getVin() != null ? index.getVin() : index.getVid())
                .connect(vehicleAlertRefSchemaBroadcastStream).process(new IndexKeyBasedSubscription())
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


        alertFoundStream.addSink(alertProducerTopic);

        /**
         * Store into alert db
         */
        TableStream tableStream = new JdbcFormatTableStream(env, propertiesParamTool);
        tableStream.saveAlertIntoDB(alertFoundStream);

        env.execute(IndexBasedAlertProcessing.class.getSimpleName());


    }

}
