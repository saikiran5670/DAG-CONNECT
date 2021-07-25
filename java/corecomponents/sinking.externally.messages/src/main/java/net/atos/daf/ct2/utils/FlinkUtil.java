package net.atos.daf.ct2.utils;

import java.util.Properties;

import org.apache.flink.api.common.restartstrategy.RestartStrategies;
import org.apache.flink.api.java.ExecutionEnvironment;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.runtime.state.StateBackend;
import org.apache.flink.runtime.state.filesystem.FsStateBackend;
import org.apache.flink.streaming.api.CheckpointingMode;
//import org.apache.flink.streaming.api.environment.CheckpointConfig.ExternalizedCheckpointCleanup;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.ct2.constant.DAFCT2Constant;

public class FlinkUtil {
	private static final Logger logger = LoggerFactory.getLogger(FlinkUtil.class);
	public static ExecutionEnvironment createExecutionEnvironment(ParameterTool envParams) throws TechnicalException {

		return ExecutionEnvironment.getExecutionEnvironment();
	}

	public static StreamExecutionEnvironment createStreamExecutionEnvironment(Properties properties)
			throws TechnicalException {

		StreamExecutionEnvironment env;

		env = StreamExecutionEnvironment.getExecutionEnvironment();
		// env.setStreamTimeCharacteristic(TimeCharacteristic.ProcessingTime);
		env.setParallelism(Integer.parseInt(properties.getProperty(DAFCT2Constant.PARALLELISM)));
		
		// start a checkpoint every 1000 ms and mode set to EXACTLY_ONCE
		env.enableCheckpointing(Long.parseLong(properties.getProperty(DAFCT2Constant.CHECKPOINT_INTERVAL)),
				CheckpointingMode.EXACTLY_ONCE);

		// make sure 500 ms of progress happen between checkpoints
		env.getCheckpointConfig().setMinPauseBetweenCheckpoints(
				Long.parseLong(properties.getProperty(DAFCT2Constant.MINIMUM_PAUSE_BETWEEN_CHECKPOINTS)));

		// checkpoints have to complete within one minute, or are discarded
		env.getCheckpointConfig()
				.setCheckpointTimeout(Long.parseLong(properties.getProperty(DAFCT2Constant.CHECKPOINT_TIMEOUT)));

		// allow only one checkpoint to be in progress at the same time
		env.getCheckpointConfig().setMaxConcurrentCheckpoints(
				Integer.parseInt(properties.getProperty(DAFCT2Constant.MAX_CONCURRENT_CHECKPOINTS)));
		
		env.setStateBackend(
				(StateBackend) new FsStateBackend(properties.getProperty(DAFCT2Constant.CHECKPOINT_DIRECTORY), true));
		
		// enable externalized checkpoints which are retained after job cancellation
		//env.getCheckpointConfig().enableExternalizedCheckpoints(ExternalizedCheckpointCleanup.RETAIN_ON_CANCELLATION);
		
		// sets the checkpoint storage where checkpoint snapshots will be written
		//env.getCheckpointConfig().setsetCheckpointStorage("hdfs:///my/checkpoint/dir");

		// TODO  enable only in QA and Prod
		logger.info("RESTART_FLAG :: "+properties.getProperty(DAFCT2Constant.RESTART_FLAG));
		if("true".equals(properties.getProperty(DAFCT2Constant.RESTART_FLAG))){
			env.setRestartStrategy(
					RestartStrategies.fixedDelayRestart(Integer.parseInt(properties.getProperty(DAFCT2Constant.RESTART_ATTEMPS)), //no of restart attempts
							Long.parseLong(properties.getProperty(DAFCT2Constant.RESTART_INTERVAL))) //time in milliseconds between restarts
						);			
		}else{
			env.setRestartStrategy(RestartStrategies.noRestart());
		}
		return env;
	}


}
