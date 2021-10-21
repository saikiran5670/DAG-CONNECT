package net.atos.daf.ct2.starter;

import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.Status;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;

public  class AlertProcessStarter implements Serializable {
    private static final Logger logger = LoggerFactory.getLogger(AlertProcessStarter.class);
    private static final long serialVersionUID = 1L;

    private IndexStarter indexStarter;
    private MonitoringStarter monitoringStarter;
    private StatusStarter statusStarter;
    private ParameterTool propertiesParamTool;
    private StreamExecutionEnvironment env;

    public AlertProcessStarter(ParameterTool propertiesParamTool,final StreamExecutionEnvironment env){
        indexStarter = new IndexStarter();
        monitoringStarter = new MonitoringStarter();
        statusStarter = new StatusStarter();
        this.propertiesParamTool=propertiesParamTool;
        this.env=env;
    }

    public AlertProcessStarter start(SingleOutputStreamOperator<?> inputStream,Class clazz){
        logger.info("Booting  {} alert processing service",clazz.getSimpleName());
        if(clazz.getSimpleName().equals(Index.class.getSimpleName())){
            indexStarter.bootIndexAlert(propertiesParamTool,
                    (SingleOutputStreamOperator<Index>) inputStream,env);
        }
        if(clazz.getSimpleName().equals(Monitor.class.getSimpleName())){
            monitoringStarter.bootMonitoringAlert(propertiesParamTool,
                    (SingleOutputStreamOperator<Monitor>) inputStream,env);
        }
        if(clazz.getSimpleName().equals(Status.class.getSimpleName())){
            statusStarter.bootStatusAlert(propertiesParamTool,
                    (SingleOutputStreamOperator<Status>) inputStream,env);
        }

        return this;
    }
}
