package net.atos.daf.etl.ct2.trip;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.apache.flink.api.java.tuple.Tuple7;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.core.fs.FileSystem;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.table.api.bridge.java.StreamTableEnvironment;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.audittrail.TripAuditTrail;
import net.atos.daf.common.ct2.exception.FailureException;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.etl.ct2.common.bo.Trip;
import net.atos.daf.etl.ct2.common.bo.TripStatusData;
import net.atos.daf.etl.ct2.common.hbase.TripStatusCompletion;
import net.atos.daf.etl.ct2.common.postgre.TripSink;
import net.atos.daf.etl.ct2.common.util.ETLConstants;
import net.atos.daf.etl.ct2.common.util.FlinkUtil;

public class TripETLJob {
	private static Logger logger = LoggerFactory.getLogger(TripETLJob.class);

	public static void main(String[] args) throws Exception {

		ParameterTool envParams = null;

		try {
			logger.info(" In Trip Details :: ");

			ParameterTool params = ParameterTool.fromArgs(args);
			if (params.get("input") != null)
				envParams = ParameterTool.fromPropertiesFile(params.get("input"));

			final StreamExecutionEnvironment env = FlinkUtil.createStreamExecutionEnvironment(envParams);
			env.getConfig().setGlobalJobParameters(envParams);

			final StreamTableEnvironment tableEnv = FlinkUtil.createStreamTableEnvironment(env);

			logger.info(" Trip Job frequency :: " + Integer.parseInt(envParams.get(ETLConstants.TRIP_ETL_FREQUENCY)));
			logger.info("  Trip Job time range to read from HBase table, startTime :: "
					+ TimeFormatter.subMilliSecFromUTCTime(
							Long.parseLong(envParams.get(ETLConstants.TRIP_JOB_START_TIME)),
							Integer.parseInt(envParams.get(ETLConstants.TRIP_ETL_FREQUENCY)))
					+ " endTime :: " + Long.parseLong(envParams.get(ETLConstants.TRIP_JOB_START_TIME)));

			List<Long> timeRangeLst = new ArrayList<>();
			timeRangeLst.add(TimeFormatter.subMilliSecFromUTCTime(
					Long.parseLong(envParams.get(ETLConstants.TRIP_JOB_START_TIME)),
					Integer.parseInt(envParams.get(ETLConstants.TRIP_ETL_FREQUENCY))));
			timeRangeLst.add(Long.parseLong(envParams.get(ETLConstants.TRIP_JOB_START_TIME)));

			Map<String, List<String>> tripStsColumns = getTripStatusColumns();
			SingleOutputStreamOperator<TripStatusData> hbaseStsData = env.addSource(new TripStatusCompletion(
					envParams.get(ETLConstants.STATUS_TABLE_NM), tripStsColumns, null, timeRangeLst));

			// TODO Only for testing
			if ("true".equals(envParams.get(ETLConstants.WRITE_OUTPUT)))
				hbaseStsData.writeAsText(envParams.get(ETLConstants.WRITE_OUTPUT) + "statusData.txt",
						FileSystem.WriteMode.OVERWRITE).name("writeStatusDataToFile");

			SingleOutputStreamOperator<Tuple7<String, String, String, Integer, Integer, String, Long>> indxData = TripAggregations
					.getTripIndexData(hbaseStsData, tableEnv, envParams);
			/*DataStream<Tuple5<String, String, String, Integer, Double>> secondLevelAggrData = TripAggregations
					.getTripIndexAggregatedData(hbaseStsData, tableEnv, indxData);
			DataStream<Trip> finalTripData = TripAggregations.getConsolidatedTripData(hbaseStsData, secondLevelAggrData,
					tableEnv);*/
			DataStream<Trip> finalTripData = TripAggregations.getConsolidatedTripData(hbaseStsData, indxData,
					tableEnv);

			// TODO read master data
			finalTripData.addSink(new TripSink());

			// Call Audit Trail
			TripAuditTrail.auditTrail(envParams, ETLConstants.AUDIT_EVENT_STATUS_START, ETLConstants.TRIP_JOB_NAME, "Trip ETL Job Started",
					ETLConstants.AUDIT_CREATE_EVENT_TYPE);
			
			/*try {
				auditing = new AuditETLJobClient(envParams.get(ETLConstants.GRPC_SERVER),
						Integer.valueOf(envParams.get(ETLConstants.GRPC_PORT)));
				auditMap = createAuditMap(ETLConstants.AUDIT_EVENT_STATUS_START, "Trip ETL Job Started");
				auditing.auditTrialGrpcCall(auditMap);
				auditing.closeChannel();
			} catch (Exception e) {
				// TODO cross check - Need not abort the job on GRPC failure
				logger.error("Issue while auditing :: " + e.getMessage());
			}*/

			env.execute("Trip ETL Job");

		} catch (Exception e) {

			/*try {
				auditMap = createAuditMap(ETLConstants.AUDIT_EVENT_STATUS_FAIL,
						"Trip ETL Job Failed, reason :: " + e.getMessage());

				auditing = new AuditETLJobClient(envParams.get(ETLConstants.GRPC_SERVER),
						Integer.valueOf(envParams.get(ETLConstants.GRPC_PORT)));
				auditing.auditTrialGrpcCall(auditMap);
				auditing.closeChannel();
			} catch (Exception ex) {
				logger.error("Issue while auditing :: " + ex.getMessage());
			}*/
			
			TripAuditTrail.auditTrail(envParams, ETLConstants.AUDIT_EVENT_STATUS_START, ETLConstants.TRIP_JOB_NAME, "Trip ETL Job Failed :: "+e.getMessage(),
					ETLConstants.AUDIT_CREATE_EVENT_TYPE);

			// TODO pass appropriate error message
			// ExceptionUtils.getRootCauseMessage(e)
			// TODO need to check with deployment team how error can be communicated
			// TODO need to check on auditing of Json with message
			
			//logger.error("Job aborted due to exception, Root cause is  :: "+ExceptionUtils.getRootCause(e));
			//logger.error("Job aborted due to exception, Root cause is  :: "+ExceptionUtils.getRootCauseMessage(e));
			//logger.error(ExceptionUtils.getStackTrace(e));
			//logger.error(ExceptionUtils.getFullStackTrace(e));
			throw new FailureException("Job aborted due to :: " + e.getMessage());
		}
	}

	private static Map<String, List<String>> getTripStatusColumns() {
		Map<String, List<String>> tripStsClmns = new HashMap<>();
		List<String> stsClmns = new ArrayList<>();

		stsClmns.add(ETLConstants.TRIP_ID);
		stsClmns.add(ETLConstants.INCREMENT);
		stsClmns.add(ETLConstants.VID);
		stsClmns.add(ETLConstants.GPS_START_DATETIME);
		stsClmns.add(ETLConstants.GPS_END_DATETIME);
		stsClmns.add(ETLConstants.GPS_TRIP_DIST);
		stsClmns.add(ETLConstants.GPS_STOP_VEH_DIST);
		stsClmns.add(ETLConstants.GPS_START_VEH_DIST);
		stsClmns.add(ETLConstants.VIDLE_DURATION);
		stsClmns.add(ETLConstants.GPS_START_LATITUDE);
		stsClmns.add(ETLConstants.GPS_START_LONGITUDE);
		stsClmns.add(ETLConstants.GPS_END_LATITUDE);
		stsClmns.add(ETLConstants.GPS_END_LONGITUDE);
		stsClmns.add(ETLConstants.VUSED_FUEL);
		stsClmns.add(ETLConstants.VSTOP_FUEL);
		stsClmns.add(ETLConstants.VSTART_FUEL);
		stsClmns.add(ETLConstants.VTRIP_MOTION_DURATION);
		stsClmns.add(ETLConstants.RECEIVED_TIMESTAMP);
		stsClmns.add(ETLConstants.VPTO_DURATION);
		stsClmns.add(ETLConstants.VHARSH_BRAKE_DURATION);
		stsClmns.add(ETLConstants.VBRAKE_DURATION);
		stsClmns.add(ETLConstants.VMAX_THROTTLE_PADDLE_DURATION);
		stsClmns.add(ETLConstants.VTRIP_ACCELERATION_TIME);
		stsClmns.add(ETLConstants.VCRUISE_CONTROL_DIST);
		stsClmns.add(ETLConstants.VTRIP_DPA_BRAKINGCOUNT);
		stsClmns.add(ETLConstants.VTRIP_DPA_ANTICIPATION_COUNT);
		stsClmns.add(ETLConstants.VCRUISE_CONTROL_FUEL_CONSUMED);
		stsClmns.add(ETLConstants.VIDLE_FUEL_CONSUMED);
		stsClmns.add(ETLConstants.VSUM_TRIP_DPA_BRAKING_SCORE);
		stsClmns.add(ETLConstants.VSUM_TRIP_DPA_ANTICIPATION_SCORE);
		stsClmns.add(ETLConstants.DRIVER_ID);
		stsClmns.add(ETLConstants.EVENT_DATETIME_FIRST_INDEX);
		stsClmns.add(ETLConstants.EVT_DATETIME);
		tripStsClmns.put(ETLConstants.STS_MSG_COLUMNFAMILY_T, stsClmns);

		return tripStsClmns;
	}

	private static Map<String, String> createAuditMap(String jobStatus, String message) {
		Map<String, String> auditMap = new HashMap<>();

		auditMap.put(ETLConstants.JOB_EXEC_TIME, String.valueOf(TimeFormatter.getCurrentUTCTime()));
		auditMap.put(ETLConstants.AUDIT_PERFORMED_BY, ETLConstants.TRIP_JOB_NAME);
		auditMap.put(ETLConstants.AUDIT_COMPONENT_NAME, ETLConstants.TRIP_JOB_NAME);
		auditMap.put(ETLConstants.AUDIT_SERVICE_NAME, ETLConstants.AUDIT_SERVICE);
		auditMap.put(ETLConstants.AUDIT_EVENT_TYPE, ETLConstants.AUDIT_CREATE_EVENT_TYPE);// check
		auditMap.put(ETLConstants.AUDIT_EVENT_TIME, String.valueOf(TimeFormatter.getCurrentUTCTime()));
		auditMap.put(ETLConstants.AUDIT_EVENT_STATUS, jobStatus);
		auditMap.put(ETLConstants.AUDIT_MESSAGE, message);
		auditMap.put(ETLConstants.AUDIT_SOURCE_OBJECT_ID, ETLConstants.DEFAULT_OBJECT_ID);
		auditMap.put(ETLConstants.AUDIT_TARGET_OBJECT_ID, ETLConstants.DEFAULT_OBJECT_ID);
		auditMap.put(ETLConstants.AUDIT_UPDATED_DATA, null);

		return auditMap;
	}

}
