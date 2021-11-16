package net.atos.daf.ct2.common.util;

import org.apache.flink.api.common.restartstrategy.RestartStrategies;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.runtime.state.StateBackend;
import org.apache.flink.runtime.state.filesystem.FsStateBackend;
import org.apache.flink.streaming.api.CheckpointingMode;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import java.util.concurrent.TimeUnit;
import org.apache.flink.api.common.time.Time;

import net.atos.daf.ct2.common.realtime.dataprocess.MonitorDataProcess;


public class FlinkUtil {

	public static StreamExecutionEnvironment createStreamExecutionEnvironment(ParameterTool envParams, String jobName) {

		Logger logger = LoggerFactory.getLogger(MonitorDataProcess.class);
		StreamExecutionEnvironment env;

		logger.debug("Start of FLINK UTIL...");
		env = StreamExecutionEnvironment.getExecutionEnvironment();

		env.setParallelism(Integer.parseInt(envParams.get(DafConstants.PARALLELISM)));

		// start a checkpoint every 1000 ms and mode set to EXACTLY_ONCE
				env.enableCheckpointing(Long.parseLong(envParams.get(DafConstants.CHECKPOINT_INTERVAL)),
						CheckpointingMode.EXACTLY_ONCE);

				// make sure 500 ms of progress happen between checkpoints
				env.getCheckpointConfig().setMinPauseBetweenCheckpoints(
						Long.parseLong(envParams.get(DafConstants.MINIMUM_PAUSE_BETWEEN_CHECKPOINTS)));

				// checkpoints have to complete within one minute, or are discarded
				env.getCheckpointConfig()
						.setCheckpointTimeout(Long.parseLong(envParams.get(DafConstants.CHECKPOINT_TIMEOUT)));

				// allow only one checkpoint to be in progress at the same time
				env.getCheckpointConfig().setMaxConcurrentCheckpoints(
						Integer.parseInt(envParams.get(DafConstants.MAX_CONCURRENT_CHECKPOINTS)));

				env.setStateBackend(
						(StateBackend) new FsStateBackend(envParams.get(DafConstants.CHECKPOINT_DIRECTORY_MONITORING), true));


				// TODO  enable only in QA and Prod
				logger.info("RESTART_FLAG :: "+envParams.get(DafConstants.RESTART_FLAG));
				if("true".equals(envParams.get(DafConstants.RESTART_FLAG))){
					if("true".equals(envParams.get(DafConstants.FIXED_RESTART_FLAG))){

					 env.setRestartStrategy(
		                        RestartStrategies.fixedDelayRestart(Integer.parseInt(envParams.get(DafConstants.RESTART_ATTEMPS)), //no of restart attempts
		                                Long.parseLong(envParams.get(DafConstants.RESTART_INTERVAL))) //time in milliseconds between restarts
		                            );   
		            }else{
		                env.setRestartStrategy(RestartStrategies.failureRateRestart(
		                          Integer.parseInt(envParams.get(DafConstants.RESTART_FAILURE_RATE)), // max failures per interval
		                          Time.of(Long.parseLong(envParams.get(DafConstants.RESTART_FAILURE_INTERVAL)), TimeUnit.MILLISECONDS), //time interval for measuring failure rate
		                          Time.of(Long.parseLong(envParams.get(DafConstants.RESTART_FAILURE_DELAY)), TimeUnit.MILLISECONDS) // delay
		                        ));
				} 
				}
				return env;
			}

}
