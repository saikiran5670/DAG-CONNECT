package net.atos.daf.ct2.starter;

import net.atos.daf.ct2.app.AlertProcessing;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.props.AlertConfigProp;
import net.atos.daf.ct2.service.realtime.*;
import org.apache.flink.api.common.eventtime.SerializableTimestampAssigner;
import org.apache.flink.api.common.eventtime.WatermarkStrategy;
import org.apache.flink.api.common.functions.ReduceFunction;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.KeyedStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.datastream.WindowedStream;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.functions.timestamps.BoundedOutOfOrdernessTimestampExtractor;
import org.apache.flink.streaming.api.windowing.assigners.TumblingEventTimeWindows;
import org.apache.flink.streaming.api.windowing.time.Time;
import org.apache.flink.streaming.api.windowing.windows.TimeWindow;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.time.Duration;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Map;
import java.util.Objects;

import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.enteringZoneFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.excessiveAverageSpeedFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.excessiveIdlingFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.excessiveUnderUtilizationInHoursFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.exitZoneFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.fuelDecreaseDuringStopFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.fuelDuringTripFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.fuelIncreaseDuringStopFun;
import static net.atos.daf.ct2.process.functions.IndexBasedAlertFunctions.hoursOfServiceFun;
import static net.atos.daf.ct2.props.AlertConfigProp.INCOMING_MESSAGE_UUID;
import static net.atos.daf.ct2.util.Utils.convertDateToMillis;

public class IndexStarter implements Serializable {
    private static final Logger logger = LoggerFactory.getLogger(IndexStarter.class);
    private static final long serialVersionUID = 1L;

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
     * RealTime functions defined
     */
    Map<Object, Object> fuelDuringTripFunConfigMap = new HashMap() {{
        put("functions", Arrays.asList(
                fuelDuringTripFun
        ));
    }};
    /**
     * RealTime functions defined
     * for geofence
     */
    Map<Object, Object> geofenceFunConfigMap = new HashMap() {{
        put("functions", Arrays.asList(
                enteringZoneFun,
                exitZoneFun
        ));
    }};

    public void bootIndexAlert(ParameterTool propertiesParamTool,
                               SingleOutputStreamOperator<Index> indexStringStream,
                               final StreamExecutionEnvironment env
    ) {
        /**
         * Window time in milliseconds
         */
        long WindowTime = Long.valueOf(propertiesParamTool.get("index.hours.of.service.window.millis", "300000"));

        /**
         * Index Window Stream
         */
        WindowedStream<Index, String, TimeWindow> windowedIndexStream = indexStringStream
                .assignTimestampsAndWatermarks(
                        new BoundedOutOfOrdernessTimestampExtractor<Index>(Time.milliseconds(0)) {
                            @Override
                            public long extractTimestamp(Index index) {
                                try {
                                    return convertDateToMillis(index.getEvtDateTime());
                                } catch (Exception ex) {
                                    logger.error("Error while converting event time stamp {}", index, String.format(INCOMING_MESSAGE_UUID, index.getJobName()));
                                }
                                return System.currentTimeMillis();
                            }
                        }
                )
                .filter(index -> index.getDocument().getTripID() != null)
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
                env, propertiesParamTool, hoursOfServiceFunConfigMap);

        /**
         * Excessive Average Speed Fun keyed stream
         */
        KeyedStream<Index, String> indexExcessiveAvgSpeedKeyedStream = windowedIndexStream
                .process(new ExcessiveAverageSpeedService())
                .filter(index -> index.getDocument().getTripID() != null)
                .returns(Index.class)
                .keyBy(index -> index.getDocument().getTripID() != null ? index.getDocument().getTripID() : "");


        /**
         * Process indexWindowKeyedStream for Hours of service
         */
        IndexMessageAlertService.processIndexKeyStream(indexExcessiveAvgSpeedKeyedStream,
                env, propertiesParamTool, excessiveAverageSpeedFunConfigMap);

        /**
         * Excessive Under Utilization In Hours
         */
        long WindowTimeExcessiveUnderUtilization = Long.valueOf(propertiesParamTool.get("index.excessive.under.utilization.window.seconds", "1800"));
        WindowedStream<Index, String, TimeWindow> windowedExcessiveUnderUtilizationStream = indexStringStream
                .assignTimestampsAndWatermarks(
                        new BoundedOutOfOrdernessTimestampExtractor<Index>(Time.seconds(0)) {
                            @Override
                            public long extractTimestamp(Index index) {
                                try {
                                    return convertDateToMillis(index.getEvtDateTime());
                                } catch (Exception ex) {
                                    logger.error("Error while converting event time stamp {}", index, String.format(INCOMING_MESSAGE_UUID, index.getJobName()));
                                }
                                return System.currentTimeMillis();
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
                env, propertiesParamTool, excessiveUnderUtilizationFunConfigMap);


        /**
         * Excessive Fuel during stop
         */
        KeyedStream<Index, String> indexStringKeyedStream = indexStringStream
                .filter(index -> Objects.nonNull(index.getVEvtID()) && (4 == index.getVEvtID() || 5 == index.getVEvtID()))
                .returns(Index.class)
                .keyBy(index -> index.getVin() != null ? index.getVin() : index.getVid())
                .process(new FuelDuringStopProcessor()).keyBy(index -> index.getVin() != null ? index.getVin() : index.getVid());

        /**
         * Process indexWindowKeyedStream for Excessive Under Utilization
         */
        IndexMessageAlertService.processIndexKeyStream(indexStringKeyedStream,
                env, propertiesParamTool, fuelDuringStopFunConfigMap);

        /**
         * Excessive Fuel during trip
         */
        KeyedStream<Index, String> fuelDuringTripStream = indexStringStream
                .assignTimestampsAndWatermarks(
                        WatermarkStrategy.<Index>forBoundedOutOfOrderness(Duration.ofSeconds(Long.parseLong(
                                        propertiesParamTool.get(AlertConfigProp.ALERT_WATERMARK_TIME_WINDOW_SECONDS))))
                                .withTimestampAssigner(new SerializableTimestampAssigner<Index>() {

                                    private static final long serialVersionUID = 1L;

                                    @Override
                                    public long extractTimestamp(Index element, long recordTimestamp) {
                                        try {
                                            return convertDateToMillis(element.getEvtDateTime());
                                        } catch (Exception ex) {
                                            logger.error("Error while converting event time stamp {}", element, String.format(INCOMING_MESSAGE_UUID, element.getJobName()));
                                        }
                                        return System.currentTimeMillis();
                                    }
                                }))
                .filter(index -> Objects.nonNull(index.getDocument().getTripID())).returns(Index.class)
                .keyBy(index -> index.getDocument().getTripID())
                .window(TumblingEventTimeWindows.of(Time
                        .seconds(Long.parseLong(propertiesParamTool.get(AlertConfigProp.ALERT_TIME_WINDOW_SECONDS)))))
                .process(new FuelDuringTripProcessor())
                .keyBy(index -> index.getVin() != null ? index.getVin() : index.getVid());

        /*
         * Process fuelDuringTripStream for fuel deviation during trip
         */
        IndexMessageAlertService.processIndexKeyStream(fuelDuringTripStream,
                env, propertiesParamTool, fuelDuringTripFunConfigMap);


        /**
         * Entering and exiting zone
         */
        KeyedStream<Index, String> geofenceEnteringZoneStream = indexStringStream.keyBy(index -> index.getVin() != null ? index.getVin() : index.getVid());

        IndexMessageAlertService.processIndexKeyStream(geofenceEnteringZoneStream,
                env, propertiesParamTool, geofenceFunConfigMap);

    }
}
