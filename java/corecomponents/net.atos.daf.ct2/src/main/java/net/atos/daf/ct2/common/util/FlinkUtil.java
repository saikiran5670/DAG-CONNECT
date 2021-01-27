package net.atos.daf.ct2.common.util;

import org.apache.commons.collections.bag.SynchronizedSortedBag;
import org.apache.flink.api.common.restartstrategy.RestartStrategies;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.runtime.state.StateBackend;
import org.apache.flink.runtime.state.filesystem.FsStateBackend;
import org.apache.flink.streaming.api.CheckpointingMode;
import org.apache.flink.streaming.api.TimeCharacteristic;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;

public class FlinkUtil {
	
public static StreamExecutionEnvironment createStreamExecutionEnvironment(ParameterTool envParams, String jobName) {
		
		StreamExecutionEnvironment env;
		System.out.println("Start of FLINK UTIL...");
			env = StreamExecutionEnvironment.getExecutionEnvironment();
			/*
			 * env.setStreamTimeCharacteristic(TimeCharacteristic.ProcessingTime); // TODO
			 * change to dynamic parameter env.setParallelism(1);
			 * 
			 * // start a checkpoint every 1000 ms and mode set to EXACTLY_ONCE
			 * env.enableCheckpointing(1000, CheckpointingMode.EXACTLY_ONCE);
			 * 
			 * // make sure 500 ms of progress happen between checkpoints
			 * env.getCheckpointConfig().setMinPauseBetweenCheckpoints(500);
			 * 
			 * // checkpoints have to complete within one minute, or are discarded
			 * env.getCheckpointConfig().setCheckpointTimeout(60000);
			 * 
			 * // allow only one checkpoint to be in progress at the same time
			 * env.getCheckpointConfig().setMaxConcurrentCheckpoints(1);
			 * 
			 * 
			 * env.setStateBackend((StateBackend) new
			 * FsStateBackend("file:///daf/flink/checkpoints", true));
			 * 
			 * env.setRestartStrategy(RestartStrategies.fixedDelayRestart( 2, //no of
			 * restart attempts 20000 //time in milliseconds between restarts ));
			 */
			
			System.out.println("envParams:: "+envParams);
			env.setParallelism(Integer.parseInt(envParams.get(DafConstants.PARALLELISM)));
			
			/*// start a checkpoint every 1000 ms and mode set to EXACTLY_ONCE
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

//			env.setStateBackend(
//					(StateBackend) new FsStateBackend(envParams.get(DafConstants.CHECKPOINT_DIRECTORY), true));
			System.out.println("outside " + jobName);
				if("IndexJob".equals(jobName)){
					env.setStateBackend((StateBackend) new FsStateBackend(envParams.get(DafConstants.CHECKPOINT_DIRECTORY_INDEX), true));
					System.out.println("Index job" + jobName);
				}
				else if("StatusJob".equals(jobName)){
					env.setStateBackend((StateBackend) new FsStateBackend(envParams.get(DafConstants.CHECKPOINT_DIRECTORY_STATUS), true));
					System.out.println("StatusJob" + jobName);
				}
				else if("MonitorJob".equals(jobName)){
					env.setStateBackend((StateBackend) new FsStateBackend(envParams.get(DafConstants.CHECKPOINT_DIRECTORY_MONITORING), true));
				System.out.println("MonitorJob" + jobName);
				}
*/
			
			// TODO Need to check if restartStrategy enabled and the set
			env.setRestartStrategy(
					RestartStrategies.fixedDelayRestart(Integer.parseInt(envParams.get(DafConstants.RESTART_ATTEMPS)), //no of restart attempts
							Long.parseLong(envParams.get(DafConstants.RESTART_INTERVAL))) //time in milliseconds between restarts
						);
			
			
			System.out.println("End of Flink UTIL.");
			return env;
	}

}
