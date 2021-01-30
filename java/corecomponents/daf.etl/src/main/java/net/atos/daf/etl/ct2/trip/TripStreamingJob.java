package net.atos.daf.etl.ct2.trip;

import java.text.SimpleDateFormat;

import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.java.tuple.Tuple7;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.table.api.bridge.java.StreamTableEnvironment;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.audittrail.TripAuditTrail;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.etl.ct2.common.bo.Trip;
import net.atos.daf.etl.ct2.common.bo.TripStatusData;
import net.atos.daf.etl.ct2.common.kafka.FlinkKafkaStatusMsgConsumer;
import net.atos.daf.etl.ct2.common.postgre.TripSink;
import net.atos.daf.etl.ct2.common.util.ETLConstants;
import net.atos.daf.etl.ct2.common.util.FlinkUtil;

public class TripStreamingJob {
	private static Logger logger = LoggerFactory.getLogger(TripStreamingJob.class);

	public static void main(String[] args) throws Exception {

		ParameterTool envParams = null;
		try {
			logger.info(" In TripStreamingJob :: ");
			ParameterTool params = ParameterTool.fromArgs(args);
			if (params.get("input") != null)
				envParams = ParameterTool.fromPropertiesFile(params.get("input"));

			final StreamExecutionEnvironment env = FlinkUtil.createStreamExecutionEnvironment(envParams);
			env.getConfig().setGlobalJobParameters(envParams);
			final StreamTableEnvironment tableEnv = FlinkUtil.createStreamTableEnvironment(env);

			// Map to status data
			SingleOutputStreamOperator<TripStatusData> statusDataStream = FlinkKafkaStatusMsgConsumer
					.consumeStatusMsgs(envParams, env).map(new MapFunction<KafkaRecord<Status>, TripStatusData>() {

						/**
						 * 
						 */
						private static final long serialVersionUID = 1L;

						@Override
						public TripStatusData map(KafkaRecord<Status> kafkaRec) {

							try {
								TripStatusData tripStsData = new TripStatusData();
								Status stsMsg = kafkaRec.getValue();

								tripStsData.setDriverId(stsMsg.getDriverID());
								tripStsData.setTripId(stsMsg.getDocument().getTripID());
								tripStsData.setVid(stsMsg.getVid());
								tripStsData.setVin(stsMsg.getVin());
								// tripStsData.setIncrement(stsMsg.getIncrement());

								//Temporary fix due to source data type mismatch
								SimpleDateFormat newDateStrFmt = new SimpleDateFormat(ETLConstants.DATE_FORMAT);

								if (stsMsg.getEventDateTimeFirstIndex() != null) {
									tripStsData.setStartDateTime(TimeFormatter.convertUTCToEpochMilli(
											newDateStrFmt.format(stsMsg.getEventDateTimeFirstIndex()),
											ETLConstants.DATE_FORMAT));
								} else {
									if (stsMsg.getGpsStartDateTime() != null)
										tripStsData.setStartDateTime(TimeFormatter.convertUTCToEpochMilli(
												newDateStrFmt.format(stsMsg.getGpsStartDateTime()),
												ETLConstants.DATE_FORMAT));
								}

								if (stsMsg.getEvtDateTime() != null) {
									tripStsData.setEndDateTime(TimeFormatter.convertUTCToEpochMilli(
											newDateStrFmt.format(stsMsg.getEvtDateTime()), ETLConstants.DATE_FORMAT));
								} else {
									if (stsMsg.getGpsEndDateTime() != null)
										tripStsData.setEndDateTime(TimeFormatter.convertUTCToEpochMilli(
												newDateStrFmt.format(stsMsg.getGpsEndDateTime()),
												ETLConstants.DATE_FORMAT));
								}

								tripStsData.setGpsTripDist(stsMsg.getDocument().getGpsTripDist());
								if (stsMsg.getGpsStopVehDist() != null)
									tripStsData.setGpsStopVehDist(Long.valueOf(stsMsg.getGpsStopVehDist()));

								if (stsMsg.getGpsStartVehDist() != null)
									tripStsData.setGpsStartVehDist(Long.valueOf(stsMsg.getGpsStartVehDist()));

								tripStsData.setGpsStartLatitude(stsMsg.getGpsStartLatitude());
								tripStsData.setGpsStartLongitude(stsMsg.getGpsStartLongitude());
								tripStsData.setGpsEndLatitude(stsMsg.getGpsEndLatitude());
								tripStsData.setGpsEndLongitude(stsMsg.getGpsEndLongitude());
								tripStsData.setVUsedFuel(stsMsg.getVUsedFuel());

								if (stsMsg.getVStopFuel() != null)
									tripStsData.setVStopFuel(Long.valueOf(stsMsg.getVStopFuel()));

								if (stsMsg.getVStartFuel() != null)
									tripStsData.setVStartFuel(Long.valueOf(stsMsg.getVStartFuel()));

								tripStsData.setVTripMotionDuration(stsMsg.getDocument().getVTripMotionDuration());
								tripStsData.setReceivedTimestamp(stsMsg.getReceivedTimestamp());
								tripStsData.setVIdleDuration(stsMsg.getVIdleDuration());
								tripStsData.setVPTODuration(stsMsg.getVptoDuration());
								tripStsData.setVHarshBrakeDuration(stsMsg.getVHarshBrakeDuration());
								tripStsData.setVBrakeDuration(stsMsg.getVBrakeDuration());
								tripStsData.setVMaxThrottlePaddleDuration(
										stsMsg.getDocument().getVMaxThrottlePaddleDuration());
								tripStsData.setVTripAccelerationTime(stsMsg.getDocument().getVTripAccelerationTime());
								tripStsData.setVCruiseControlDist(stsMsg.getVCruiseControlDist());
								tripStsData.setVTripDPABrakingCount(stsMsg.getDocument().getVTripDPABrakingCount());
								tripStsData.setVTripDPAAnticipationCount(
										stsMsg.getDocument().getVTripDPAAnticipationCount());
								tripStsData.setVCruiseControlFuelConsumed(
										stsMsg.getDocument().getVCruiseControlFuelConsumed());
								tripStsData.setVIdleFuelConsumed(stsMsg.getDocument().getVIdleFuelConsumed());
								tripStsData
										.setVSumTripDPABrakingScore(stsMsg.getDocument().getVSumTripDPABrakingScore());

								tripStsData.setVSumTripDPAAnticipationScore(
										stsMsg.getDocument().getVSumTripDPAAnticipationScore());

								if (tripStsData.getStartDateTime() != null && tripStsData.getEndDateTime() != null)
									tripStsData.setTripCalGpsVehTimeDiff(TimeFormatter.subPastUtcTmFrmCurrentUtcTm(
											tripStsData.getStartDateTime(), tripStsData.getEndDateTime()));

								if (tripStsData.getGpsStopVehDist() != null && tripStsData.getGpsStartVehDist() != null)
									tripStsData.setTripCalGpsVehDistDiff(
											tripStsData.getGpsStopVehDist() - tripStsData.getGpsStartVehDist());
								
								if(tripStsData.getTripCalGpsVehTimeDiff() != null){
									double timeDiffInHr = (tripStsData.getTripCalGpsVehTimeDiff()).doubleValue() /3600000;
									//double timeDiffInsec = (tripStsData.getTripCalGpsVehTimeDiff()).doubleValue() /1000;
									tripStsData.setTripCalVehTimeDiffInHr(timeDiffInHr);
								}

								// TODO Insert Kafka processing record time
								// tripStsData.set(hbaseInsertionTS);
								tripStsData.setEtlProcessingTS(TimeFormatter.getCurrentUTCTime());
								
								logger.info("tripStsData.getTripCalVehTimeDiffInHr : "+tripStsData.getTripCalVehTimeDiffInHr());
								logger.info("driving Time : "+tripStsData.getTripCalGpsVehTimeDiff() +" idle: "+tripStsData.getVIdleDuration());
								
								return tripStsData;
							} catch (Exception e) {
								logger.error(
										"Issue while mapping deserialized status object to trip status object :: " + e);
								return null;
							}

						}
					}).filter(rec -> null != rec);

			SingleOutputStreamOperator<Tuple7<String, String, String, Integer, Integer, String, Long>> indxData = TripAggregations
					.getTripIndexData(statusDataStream, tableEnv, envParams);

			DataStream<Trip> finalTripData = TripAggregations.getConsolidatedTripData(statusDataStream, indxData,
					tableEnv);

			// Call Audit Trail
			TripAuditTrail.auditTrail(envParams, ETLConstants.AUDIT_EVENT_STATUS_START, ETLConstants.TRIP_STREAMING_JOB_NAME,
					"Trip ETL Job Started", ETLConstants.AUDIT_CREATE_EVENT_TYPE);

			// TODO read master data
			finalTripData.addSink(new TripSink());

			env.execute("Trip Streaming Job");

		} catch (Exception e) {

			// Call Audit Trail
			TripAuditTrail.auditTrail(envParams, ETLConstants.AUDIT_EVENT_STATUS_START, ETLConstants.TRIP_STREAMING_JOB_NAME,
					"Trip ETL Job Started", ETLConstants.AUDIT_CREATE_EVENT_TYPE);

			logger.error(" TripStreamingJob failed, reason :: " + e);
			//e.printStackTrace();
		}

	}
}
