package net.atos.daf.ct2.common.processing;

import java.io.Serializable;
import java.util.UUID;

import net.atos.daf.ct2.common.util.Utils;
import net.atos.daf.ct2.pojo.standard.Index;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.functions.timestamps.BoundedOutOfOrdernessTimestampExtractor;
import org.apache.flink.streaming.api.functions.windowing.ProcessWindowFunction;
import org.apache.flink.streaming.api.windowing.assigners.TumblingEventTimeWindows;
import org.apache.flink.streaming.api.windowing.assigners.TumblingProcessingTimeWindows;
import org.apache.flink.streaming.api.windowing.time.Time;
import net.atos.daf.ct2.pojo.standard.Monitor;
import org.apache.flink.streaming.api.windowing.windows.GlobalWindow;
import org.apache.flink.util.Collector;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;


import static net.atos.daf.ct2.common.util.DafConstants.INCOMING_MESSAGE_UUID;
import static net.atos.daf.ct2.common.util.Utils.convertDateToMillis;
import static net.atos.daf.ct2.common.util.Utils.getCurrentTimeInUTC;


public class DriverProcessing implements Serializable {

    /**
     *
     */
    private static final long serialVersionUID = 1L;
    private static final Logger logger = LogManager.getLogger(DriverProcessing.class);

    public SingleOutputStreamOperator<Monitor> driverManagementProcessing(
            SingleOutputStreamOperator<Monitor> monitorStream, long driverManagementCountWindow) {
        return monitorStream
                .map(monitor -> {
                    monitor.setJobName(UUID.randomUUID().toString());
                    logger.info("monitor message received for processing :: {}  {}", monitor, String.format(INCOMING_MESSAGE_UUID, monitor.getJobName()));
                    return monitor;
                })
                .keyBy(value -> value.getDocument().getDriverID())
                .countWindow(driverManagementCountWindow)
                .process(new DriverCalculationCountWindow());
    }

}
