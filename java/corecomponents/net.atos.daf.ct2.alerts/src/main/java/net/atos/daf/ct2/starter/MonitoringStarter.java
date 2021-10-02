package net.atos.daf.ct2.starter;

import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.service.realtime.MonitorMessageAlertService;
import net.atos.daf.ct2.service.realtime.RepairMaintenance;
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

import static net.atos.daf.ct2.process.functions.MonitorBasedAlertFunction.repairMaintenanceFun;

public class MonitoringStarter implements Serializable {
    private static final Logger logger = LoggerFactory.getLogger(MonitoringStarter.class);
    private static final long serialVersionUID = 1L;

    /**
     * RealTime functions defined
     */

    Map<Object, Object> repairMaintenanceFunConfigMap = new HashMap() {
        {
            put("functions", Arrays.asList(repairMaintenanceFun));
        }
    };

    public void bootMonitoringAlert(ParameterTool propertiesParamTool,
                                    SingleOutputStreamOperator<Monitor> monitorStringStream,
                                    final StreamExecutionEnvironment env
    ) {
        KeyedStream<Monitor, String> monitorStringKeyedStream =
                monitorStringStream.process(new RepairMaintenance(propertiesParamTool))
                        .keyBy(moniter -> moniter.getVin() != null ? moniter.getVin() : moniter.getVid());
        MonitorMessageAlertService.processMonitorKeyStream(monitorStringKeyedStream,
                env, propertiesParamTool, repairMaintenanceFunConfigMap);
    }
}
