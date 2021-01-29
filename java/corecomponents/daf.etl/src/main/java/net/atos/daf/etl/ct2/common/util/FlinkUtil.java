package net.atos.daf.etl.ct2.common.util;

import org.apache.flink.api.common.restartstrategy.RestartStrategies;
import org.apache.flink.api.java.ExecutionEnvironment;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.runtime.state.StateBackend;
import org.apache.flink.runtime.state.filesystem.FsStateBackend;
import org.apache.flink.streaming.api.CheckpointingMode;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.table.api.bridge.java.StreamTableEnvironment;

import net.atos.daf.common.ct2.exception.FailureException;
import net.atos.daf.common.ct2.exception.TechnicalException;

public class FlinkUtil {

	public static ExecutionEnvironment createExecutionEnvironment(ParameterTool envParams) throws TechnicalException {

		return ExecutionEnvironment.getExecutionEnvironment();
	}

	public static StreamExecutionEnvironment createStreamExecutionEnvironment(ParameterTool envParams)
			throws TechnicalException {

		StreamExecutionEnvironment env;

		env = StreamExecutionEnvironment.getExecutionEnvironment();
		// Deprecated in Flink 1.12.0
		// env.setStreamTimeCharacteristic(TimeCharacteristic.ProcessingTime);

		// TODO change to dynamic parameter
		env.setParallelism(Integer.parseInt(envParams.get(ETLConstants.PARALLELISM)));
		
		// start a checkpoint every 1000 ms and mode set to EXACTLY_ONCE
		env.enableCheckpointing(Long.parseLong(envParams.get(ETLConstants.CHECKPOINT_INTERVAL)),
				CheckpointingMode.EXACTLY_ONCE);

		// make sure 500 ms of progress happen between checkpoints
		env.getCheckpointConfig().setMinPauseBetweenCheckpoints(
				Long.parseLong(envParams.get(ETLConstants.MINIMUM_PAUSE_BETWEEN_CHECKPOINTS)));

		// checkpoints have to complete within one minute, or are discarded
		env.getCheckpointConfig()
				.setCheckpointTimeout(Long.parseLong(envParams.get(ETLConstants.CHECKPOINT_TIMEOUT)));

		// allow only one checkpoint to be in progress at the same time
		env.getCheckpointConfig().setMaxConcurrentCheckpoints(
				Integer.parseInt(envParams.get(ETLConstants.MAX_CONCURRENT_CHECKPOINTS)));

		env.setStateBackend(
				(StateBackend) new FsStateBackend(envParams.get(ETLConstants.CHECKPOINT_DIRECTORY), true));

		// TODO  enable only in QA and Prod
		if("true".equals(envParams.get(ETLConstants.RESTART_FLAG))){
			env.setRestartStrategy(
					RestartStrategies.fixedDelayRestart(Integer.parseInt(envParams.get(ETLConstants.RESTART_ATTEMPS)), //no of restart attempts
							Long.parseLong(envParams.get(ETLConstants.RESTART_INTERVAL))) //time in milliseconds between restarts
						);			
		}
		
		
		return env;
	}

	public static StreamTableEnvironment createStreamTableEnvironment(StreamExecutionEnvironment env)
			throws FailureException {
		return StreamTableEnvironment.create(env);
	}
}
