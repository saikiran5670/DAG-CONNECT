package net.atos.daf.ct2.util;

import net.atos.daf.ct2.app.AlertProcessing;
import net.atos.daf.ct2.props.AlertConfigProp;
import org.apache.flink.api.common.restartstrategy.RestartStrategies;
import org.apache.flink.api.common.time.Time;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.runtime.state.StateBackend;
import org.apache.flink.runtime.state.filesystem.FsStateBackend;
import org.apache.flink.streaming.api.CheckpointingMode;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.util.Objects;
import java.util.concurrent.TimeUnit;

import static net.atos.daf.ct2.props.AlertConfigProp.CHECKPOINT_DIRECTORY_ALERT;
import static net.atos.daf.ct2.props.AlertConfigProp.CHECKPOINT_INTERVAL;
import static net.atos.daf.ct2.props.AlertConfigProp.CHECKPOINT_TIMEOUT;
import static net.atos.daf.ct2.props.AlertConfigProp.FIXED_RESTART_FLAG;
import static net.atos.daf.ct2.props.AlertConfigProp.MAX_CONCURRENT_CHECKPOINTS;
import static net.atos.daf.ct2.props.AlertConfigProp.MINIMUM_PAUSE_BETWEEN_CHECKPOINTS;
import static net.atos.daf.ct2.props.AlertConfigProp.PARALLELISM;
import static net.atos.daf.ct2.props.AlertConfigProp.RESTART_ATTEMPS;
import static net.atos.daf.ct2.props.AlertConfigProp.RESTART_FAILURE_DELAY;
import static net.atos.daf.ct2.props.AlertConfigProp.RESTART_FAILURE_INTERVAL;
import static net.atos.daf.ct2.props.AlertConfigProp.RESTART_FAILURE_RATE;
import static net.atos.daf.ct2.props.AlertConfigProp.RESTART_FLAG;
import static net.atos.daf.ct2.props.AlertConfigProp.RESTART_INTERVAL;


public class FlinkUtil implements Serializable {
    private static final Logger logger = LoggerFactory.getLogger(AlertProcessing.class);
    private static final long serialVersionUID = 1L;

    private ParameterTool envParams;
    private String FLINK_EXECUTION_MODE = "DEFAULT";

    /**
     * Parameterize constructor
     */
    public FlinkUtil(ParameterTool envParams){
        logger.debug("envParams:: {}", envParams);
        this.envParams=envParams;
        FLINK_EXECUTION_MODE=this.envParams.get("flink.streaming.evn", FLINK_EXECUTION_MODE);
    }

    public StreamExecutionEnvironment createStreamExecutionEnvironment(){
        logger.info("Start of FLINK UTIL...");
        StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();
        //Set PARALLELISM
        if(Objects.nonNull(envParams.get(PARALLELISM)))
            env.setParallelism(Integer.parseInt(envParams.get(PARALLELISM)));

        if(FLINK_EXECUTION_MODE.equalsIgnoreCase("CHECKPOINT_ONLY")){
           setCheckpointConfig(env);
        }
        if(FLINK_EXECUTION_MODE.equalsIgnoreCase("RESTART_ONLY")){
            setRestartConfig(env);
        }
        if(FLINK_EXECUTION_MODE.equalsIgnoreCase("CHECKPOINT_AND_RESTART")){
            setCheckpointConfig(env);
            setRestartConfig(env);
        }
        logger.info("Flink env started with {}",FLINK_EXECUTION_MODE);
        return env;
    }

    private void setCheckpointConfig(StreamExecutionEnvironment env){
        // start a checkpoint every CHECKPOINT_INTERVAL ms and mode set to EXACTLY_ONCE
        env.enableCheckpointing(Long.parseLong(envParams.get(CHECKPOINT_INTERVAL)), CheckpointingMode.EXACTLY_ONCE);

        // make sure MINIMUM_PAUSE_BETWEEN_CHECKPOINTS ms of progress happen between checkpoints
        env.getCheckpointConfig().setMinPauseBetweenCheckpoints(Long.parseLong(envParams.get(MINIMUM_PAUSE_BETWEEN_CHECKPOINTS))
        );

        // checkpoints have to complete within one minute, or are discarded
        env.getCheckpointConfig().setCheckpointTimeout(Long.parseLong(envParams.get(CHECKPOINT_TIMEOUT)));

        // allow only one checkpoint to be in progress at the same time
        env.getCheckpointConfig().setMaxConcurrentCheckpoints(
                Integer.parseInt(envParams.get(MAX_CONCURRENT_CHECKPOINTS)));
        //Checkpoint directory
        env.setStateBackend((StateBackend) new FsStateBackend(envParams.get(CHECKPOINT_DIRECTORY_ALERT), true));
    }

    private void setRestartConfig(StreamExecutionEnvironment env){
        if ("true".equals(envParams.get(FIXED_RESTART_FLAG))) {
            env.setRestartStrategy(
                    RestartStrategies.fixedDelayRestart(Integer.parseInt(envParams.get(RESTART_ATTEMPS)), //no of restart attempts
                            Long.parseLong(envParams.get(RESTART_INTERVAL))) //time in milliseconds between restarts
            );
        } else {
            env.setRestartStrategy(RestartStrategies.failureRateRestart(
                    Integer.parseInt(envParams.get(RESTART_FAILURE_RATE)), // max failures per interval
                    Time.of(Long.parseLong(envParams.get(RESTART_FAILURE_INTERVAL)), TimeUnit.MILLISECONDS), //time interval for measuring failure rate
                    Time.of(Long.parseLong(envParams.get(RESTART_FAILURE_DELAY)), TimeUnit.MILLISECONDS) // delay
            ));
        }
    }

    public static FlinkUtil getFlinkUtil(ParameterTool envParams){
        return new FlinkUtil(envParams);
    }

}
