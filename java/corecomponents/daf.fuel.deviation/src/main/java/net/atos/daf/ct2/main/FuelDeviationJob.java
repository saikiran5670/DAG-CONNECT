package net.atos.daf.ct2.main;

import java.math.BigDecimal;
import java.time.Duration;

import org.apache.flink.api.common.eventtime.SerializableTimestampAssigner;
import org.apache.flink.api.common.eventtime.WatermarkStrategy;
import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.bo.FuelDeviation;
import net.atos.daf.ct2.bo.FuelDeviationData;
import net.atos.daf.ct2.exception.FuelDeviationAuditServiceException;
import net.atos.daf.ct2.kafka.FlinkKafkaFuelDeviationMsgConsumer;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.postgre.FuelDeviationSink;
import net.atos.daf.ct2.processing.FuelDeviationProcess;
import net.atos.daf.ct2.util.FlinkUtil;
import net.atos.daf.ct2.util.FuelDeviationAuditService;
import net.atos.daf.ct2.util.FuelDeviationConstants;

public class FuelDeviationJob {
	private static Logger logger = LoggerFactory.getLogger(FuelDeviationJob.class);

	public static void main(String[] args) throws Exception {

		ParameterTool envParams = null;
		FuelDeviationJob fuelDeviationJob = null;
		try {
			logger.info(" In FuelDeviationJob :: ");
			fuelDeviationJob = new FuelDeviationJob();
			ParameterTool params = ParameterTool.fromArgs(args);
			if (params.get("input") != null)
				envParams = ParameterTool.fromPropertiesFile(params.get("input"));

			final StreamExecutionEnvironment env = FlinkUtil.createStreamExecutionEnvironment(envParams);
			env.getConfig().setGlobalJobParameters(envParams);

			fuelDeviationJob.auditFuelDevialJobDetails(envParams, "Fuel Deviation job started");

			SingleOutputStreamOperator<FuelDeviationData> fuelDeviationData = FlinkKafkaFuelDeviationMsgConsumer
					.consumeIndexMsgs(envParams, env)
					.map(new MapFunction<KafkaRecord<Index>, FuelDeviationData>() {
						/**
						 * 
						 */
						private static final long serialVersionUID = 1L;

						@Override
						public FuelDeviationData map(KafkaRecord<Index> kafkaRec) {
							return fetchFuelDeviationData(kafkaRec.getValue());
						}
					}).assignTimestampsAndWatermarks(
							WatermarkStrategy.<FuelDeviationData>forBoundedOutOfOrderness(Duration.ofSeconds(Long.parseLong(
									envParams.get(FuelDeviationConstants.FUEL_DEVIATION_WATERMARK_TIME_WINDOW_SECONDS))))
									.withTimestampAssigner(new SerializableTimestampAssigner<FuelDeviationData>() {

										private static final long serialVersionUID = 1L;

										@Override
										public long extractTimestamp(FuelDeviationData element, long recordTimestamp) {
											return element.getEvtDateTime();
										}
									}));

			FuelDeviationProcess fuelDeviation = new FuelDeviationProcess();
			SingleOutputStreamOperator<FuelDeviationData> fuelDuringStopData = fuelDeviationData
					.filter(rec -> (FuelDeviationConstants.INDEX_TRIP_START).intValue() == rec.getVEvtId().intValue()
							|| (FuelDeviationConstants.INDEX_TRIP_END).intValue() == rec.getVEvtId().intValue());

			
			
			SingleOutputStreamOperator<FuelDeviation> fuelDeviationDuringStopData = fuelDeviation
					.fuelDeviationProcessingDuringStop(fuelDuringStopData,
							Long.parseLong(envParams.get(FuelDeviationConstants.FUEL_DEVIATION_TIME_WINDOW_SECONDS)));

			fuelDeviationDuringStopData.addSink(new FuelDeviationSink());
			
			SingleOutputStreamOperator<FuelDeviationData> fuelDuringTripData = fuelDeviationData
					.filter(rec -> rec.getTripId() != null);
			
			SingleOutputStreamOperator<FuelDeviation> fuelDeviationDuringTripData = fuelDeviation
					.fuelDeviationProcessingDuringTrip(fuelDuringTripData,
							Long.parseLong(envParams.get(FuelDeviationConstants.FUEL_DEVIATION_TIME_WINDOW_SECONDS)));

			fuelDeviationDuringTripData.addSink(new FuelDeviationSink());
			
			env.execute("Fuel Deviation Streaming Job");

		} catch (Exception e) {
			fuelDeviationJob.auditFuelDevialJobDetails(envParams, "FuelDeviation streaming job failed ::" + e.getMessage());
			logger.error("FuelDeviationJob failed, reason :: " + e);
			e.printStackTrace();
		}

	}

	public static FuelDeviationData fetchFuelDeviationData(Index idxMsg) {
		FuelDeviationData fuelStopObj = new FuelDeviationData();

		try {

			fuelStopObj.setVid(idxMsg.getVid());
			fuelStopObj.setVin(idxMsg.getVin());
			if (idxMsg.getEvtDateTime() != null) {
				fuelStopObj.setEvtDateTime(TimeFormatter.getInstance().convertUTCToEpochMilli(
						idxMsg.getEvtDateTime().toString(), FuelDeviationConstants.DATE_FORMAT));
			}else
				fuelStopObj.setEvtDateTime(FuelDeviationConstants.ZERO_VAL);

			if (idxMsg.getDocument() != null) {
				fuelStopObj.setTripId(idxMsg.getDocument().getTripID());

				//cross verify
				if (idxMsg.getDocument().getVFuelLevel1() != null)
					fuelStopObj.setVFuelLevel(BigDecimal.valueOf(idxMsg.getDocument().getVFuelLevel1()));
				/*else
					fuelStopObj.setVFuelLevel(BigDecimal.ZERO);*/
			}

			if(idxMsg.getVEvtID() != null)
				fuelStopObj.setVEvtId(idxMsg.getVEvtID());
			else
				fuelStopObj.setVEvtId(0);
			
			fuelStopObj.setVDist(idxMsg.getVDist());
			fuelStopObj.setGpsLatitude(idxMsg.getGpsLatitude());
			fuelStopObj.setGpsLongitude(idxMsg.getGpsLongitude());
			fuelStopObj.setGpsHeading(idxMsg.getGpsHeading());

		} catch (Exception e) {
			logger.error("Issue while mapping deserialized Index object to fuelDeviationDuringStop object :: " + e);
			logger.error("Issue while processing fuelDeviationDuringStop record :: " + idxMsg);
		}
		return fuelStopObj;
	}

	public void auditFuelDevialJobDetails(ParameterTool properties, String message) {
		logger.info("Calling audit service for FuelDeviation Job :: ");
		try {
			new FuelDeviationAuditService().auditTrail(properties.get(FuelDeviationConstants.GRPC_SERVER),
					properties.get(FuelDeviationConstants.GRPC_PORT), FuelDeviationConstants.FUEL_DEVIATION_JOB_NAME,
					message);
		} catch (FuelDeviationAuditServiceException e) {
			logger.info("FuelDeviation Streaming Job :: ", e.getMessage());
			System.out.println("FuelDeviation Streaming Job :: " + e);
		}

	}

}
