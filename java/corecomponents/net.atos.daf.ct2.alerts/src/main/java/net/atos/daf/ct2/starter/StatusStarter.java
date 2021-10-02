package net.atos.daf.ct2.starter;

import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.service.realtime.StatusMessageAlertService;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.KeyedStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Map;

import static net.atos.daf.ct2.process.functions.LogisticAlertFunction.excessiveDistanceDone;
import static net.atos.daf.ct2.process.functions.LogisticAlertFunction.excessiveDriveTime;
import static net.atos.daf.ct2.process.functions.LogisticAlertFunction.excessiveGlobalMileage;

public class StatusStarter implements Serializable {
    private static final Logger logger = LoggerFactory.getLogger(IndexStarter.class);
    private static final long serialVersionUID = 1L;

    /**
     * Logistics functions defined
     */
    Map<Object, Object> configMap = new HashMap() {{
        put("functions", Arrays.asList(
                excessiveDriveTime,
                excessiveGlobalMileage,
                excessiveDistanceDone
        ));
    }};


    public void bootStatusAlert(ParameterTool propertiesParamTool,
                               SingleOutputStreamOperator<Status> statusStream,
                               final StreamExecutionEnvironment env) {
        KeyedStream<Status, String> statusKeyedStream = statusStream
                .keyBy(status -> status.getVin() != null ? status.getVin() : status.getVid());

        StatusMessageAlertService.processStatusKeyStream(statusKeyedStream,
                env, propertiesParamTool, configMap
        );
    }
}
