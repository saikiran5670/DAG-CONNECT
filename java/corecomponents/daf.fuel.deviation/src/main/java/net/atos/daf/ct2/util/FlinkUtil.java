package net.atos.daf.ct2.util;

import org.apache.flink.api.common.restartstrategy.RestartStrategies;
import org.apache.flink.api.java.ExecutionEnvironment;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.runtime.state.StateBackend;
import org.apache.flink.runtime.state.filesystem.FsStateBackend;
import org.apache.flink.streaming.api.CheckpointingMode;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;

import net.atos.daf.common.ct2.exception.TechnicalException;

public class FlinkUtil {

	public static ExecutionEnvironment createExecutionEnvironment(ParameterTool envParams) throws TechnicalException {

		return ExecutionEnvironment.getExecutionEnvironment();
	}

	public static StreamExecutionEnvironment createStreamExecutionEnvironment(ParameterTool envParams)
			throws TechnicalException {

		StreamExecutionEnvironment env;

		env = StreamExecutionEnvironment.getExecutionEnvironment();
		// env.setStreamTimeCharacteristic(TimeCharacteristic.ProcessingTime);
		env.setParallelism(Integer.parseInt(envParams.get(FuelDeviationConstants.PARALLELISM)));
		
		// start a checkpoint every 1000 ms and mode set to EXACTLY_ONCE
		env.enableCheckpointing(Long.parseLong(envParams.get(FuelDeviationConstants.CHECKPOINT_INTERVAL)),
				CheckpointingMode.EXACTLY_ONCE);

		// make sure 500 ms of progress happen between checkpoints
		env.getCheckpointConfig().setMinPauseBetweenCheckpoints(
				Long.parseLong(envParams.get(FuelDeviationConstants.MINIMUM_PAUSE_BETWEEN_CHECKPOINTS)));

		// checkpoints have to complete within one minute, or are discarded
		env.getCheckpointConfig()
				.setCheckpointTimeout(Long.parseLong(envParams.get(FuelDeviationConstants.CHECKPOINT_TIMEOUT)));

		// allow only one checkpoint to be in progress at the same time
		env.getCheckpointConfig().setMaxConcurrentCheckpoints(
				Integer.parseInt(envParams.get(FuelDeviationConstants.MAX_CONCURRENT_CHECKPOINTS)));

		env.setStateBackend(
				(StateBackend) new FsStateBackend(envParams.get(FuelDeviationConstants.CHECKPOINT_DIRECTORY), true));

		if("true".equals(envParams.get(FuelDeviationConstants.RESTART_FLAG))){
			env.setRestartStrategy(
					RestartStrategies.fixedDelayRestart(Integer.parseInt(envParams.get(FuelDeviationConstants.RESTART_ATTEMPS)), //no of restart attempts
							Long.parseLong(envParams.get(FuelDeviationConstants.RESTART_INTERVAL))) //time in milliseconds between restarts
						);			
		}else{
			env.setRestartStrategy(RestartStrategies.noRestart());
		}
		return env;
	}

	
}
