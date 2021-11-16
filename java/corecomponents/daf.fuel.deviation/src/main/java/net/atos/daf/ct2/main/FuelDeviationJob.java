package net.atos.daf.ct2.main;

import java.math.BigDecimal;
import java.time.Duration;
import java.util.Objects;

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
	private static final Logger logger = LoggerFactory.getLogger(FuelDeviationJob.class);

	public static void main(String[] args) throws Exception {

		ParameterTool envParams = null;
		FuelDeviationJob fuelDeviationJob = null;
		try {
			logger.debug(" In FuelDeviationJob :: ");
			fuelDeviationJob = new FuelDeviationJob();
			ParameterTool params = ParameterTool.fromArgs(args);
			if (params.get("input") != null)
				envParams = ParameterTool.fromPropertiesFile(params.get("input"));

			final StreamExecutionEnvironment env = envParams.get("flink.streaming.evn").equalsIgnoreCase("default") ?
					StreamExecutionEnvironment.getExecutionEnvironment() : FlinkUtil.createStreamExecutionEnvironment(envParams);

			env.getConfig().setGlobalJobParameters(envParams);

			fuelDeviationJob.auditFuelDevialJobDetails(envParams, "Fuel Deviation job started");

			SingleOutputStreamOperator<FuelDeviationData> fuelDeviationData = FlinkKafkaFuelDeviationMsgConsumer
					.consumeIndexMsgs(envParams, env)
					.assignTimestampsAndWatermarks(WatermarkStrategy
							.<KafkaRecord<Index>>forBoundedOutOfOrderness(Duration.ofSeconds(Long.parseLong(envParams
									.get(FuelDeviationConstants.FUEL_DEVIATION_WATERMARK_TIME_WINDOW_SECONDS))))
							.withTimestampAssigner(new SerializableTimestampAssigner<KafkaRecord<Index>>() {

								private static final long serialVersionUID = 1L;

								@Override
								public long extractTimestamp(KafkaRecord<Index> element, long recordTimestamp) {
									long eventTm = TimeFormatter.getInstance().getCurrentUTCTime();
									try {
										eventTm = TimeFormatter.getInstance().convertUTCToEpochMilli(
												element.getValue().getEvtDateTime().toString(),
												FuelDeviationConstants.DATE_FORMAT);
									} catch (Exception e) {
										logger.error("Issue mandatory field is null, msg :{}", element.getValue());
									}

									return eventTm;
								}
							}))
					.keyBy(rec ->rec.getValue().getVin()!=null ? rec.getValue().getVin() : rec.getValue().getVid())
					.map(new MapFunction<KafkaRecord<Index>, FuelDeviationData>() {
						/**
						 * 
						 */
						private static final long serialVersionUID = 1L;

						@Override
						public FuelDeviationData map(KafkaRecord<Index> kafkaRec) {
							return fetchFuelDeviationData(kafkaRec.getValue());
						}
					});

			FuelDeviationProcess fuelDeviation = new FuelDeviationProcess();
			SingleOutputStreamOperator<FuelDeviationData> fuelDuringStopData = fuelDeviationData
					.filter(rec -> Objects.nonNull(rec.getVEvtId()) && ((FuelDeviationConstants.INDEX_TRIP_START).intValue() == rec.getVEvtId().intValue()
							|| (FuelDeviationConstants.INDEX_TRIP_END).intValue() == rec.getVEvtId().intValue()));

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

			env.execute(envParams.get(FuelDeviationConstants.FUEL_DEVIATION_STREAMING_JOB_NAME));

		} catch (Exception e) {
			fuelDeviationJob.auditFuelDevialJobDetails(envParams,
					"FuelDeviation streaming job failed ::" + e.getMessage());
			logger.error("Issue FuelDeviationJob failed, reason ::{} ", e);
			e.printStackTrace();
		}

	}

	public static FuelDeviationData fetchFuelDeviationData(Index idxMsg) {
		FuelDeviationData fuelStopObj = new FuelDeviationData();

		try {

			fuelStopObj.setVid(idxMsg.getVid());

			if (Objects.nonNull(idxMsg.getVin()))
				fuelStopObj.setVin(idxMsg.getVin());
			else
				fuelStopObj.setVin(idxMsg.getVid());

			if (idxMsg.getEvtDateTime() != null) {
				fuelStopObj.setEvtDateTime(TimeFormatter.getInstance().convertUTCToEpochMilli(
						idxMsg.getEvtDateTime().toString(), FuelDeviationConstants.DATE_FORMAT));
			} else
				fuelStopObj.setEvtDateTime(FuelDeviationConstants.ZERO_VAL);

			if (idxMsg.getDocument() != null) {
				if (Objects.nonNull(idxMsg.getDocument().getTripID()))
					fuelStopObj.setTripId(idxMsg.getDocument().getTripID());
				else
					fuelStopObj.setTripId("UNKNOWN");
				// cross verify
				if (idxMsg.getDocument().getVFuelLevel1() != null)
					fuelStopObj.setVFuelLevel(BigDecimal.valueOf(idxMsg.getDocument().getVFuelLevel1()));
				/*
				 * else fuelStopObj.setVFuelLevel(BigDecimal.ZERO);
				 */
			}

			if (idxMsg.getVEvtID() != null)
				fuelStopObj.setVEvtId(idxMsg.getVEvtID());
			else
				fuelStopObj.setVEvtId(0);

			if (idxMsg.getVEvtID() != null)
				fuelStopObj.setVEvtId(idxMsg.getVEvtID());
			else
				fuelStopObj.setVEvtId(FuelDeviationConstants.ZERO);

			if (Objects.nonNull(idxMsg.getVDist()))
				fuelStopObj.setVDist(idxMsg.getVDist());
			else
				fuelStopObj.setVDist(FuelDeviationConstants.ZERO_VAL);

			if (Objects.nonNull(idxMsg.getGpsLatitude()))
				fuelStopObj.setGpsLatitude(idxMsg.getGpsLatitude());
			else
				fuelStopObj.setGpsLatitude(FuelDeviationConstants.ZERO_DOUBLE_VAL);

			if (Objects.nonNull(idxMsg.getGpsLongitude()))
				fuelStopObj.setGpsLongitude(idxMsg.getGpsLongitude());
			else
				fuelStopObj.setGpsLatitude(FuelDeviationConstants.ZERO_DOUBLE_VAL);

			if (Objects.nonNull(idxMsg.getGpsHeading()))
				fuelStopObj.setGpsHeading(idxMsg.getGpsHeading());
			else
				fuelStopObj.setGpsHeading(FuelDeviationConstants.ZERO_DOUBLE_VAL);

			logger.debug("fuelStopObj object :: {}", fuelStopObj);
		} catch (Exception e) {
			logger.error("Issue while mapping deserialized Index object to fuelDeviationDuringStop object ::{} ", e.getMessage());
			logger.error("Issue while processing fuelDeviationDuringStop record :: {}", idxMsg);
			e.printStackTrace();
		}
		return fuelStopObj;
	}

	public void auditFuelDevialJobDetails(ParameterTool properties, String message) {
		logger.debug("Calling audit service for FuelDeviation Job :: ");
		try {
			new FuelDeviationAuditService().auditTrail(properties.get(FuelDeviationConstants.GRPC_SERVER),
					properties.get(FuelDeviationConstants.GRPC_PORT), FuelDeviationConstants.FUEL_DEVIATION_JOB_NAME,
					message);
		} catch (FuelDeviationAuditServiceException e) {
			logger.error("FuelDeviation Streaming Job :: ", e.getMessage());
		}

	}

}
