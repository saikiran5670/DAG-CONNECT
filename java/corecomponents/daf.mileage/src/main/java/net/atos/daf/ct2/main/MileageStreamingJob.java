package net.atos.daf.ct2.main;

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
import net.atos.daf.ct2.bo.TripMileage;
import net.atos.daf.ct2.bo.VehicleMileage;
import net.atos.daf.ct2.exception.MileageAuditServiceException;
import net.atos.daf.ct2.kafka.FlinkKafkaMileageMsgConsumer;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.postgre.MileageJdbcSink;
import net.atos.daf.ct2.postgre.MileageSink;
import net.atos.daf.ct2.processing.MileageProcessing;
import net.atos.daf.ct2.util.FlinkUtil;
import net.atos.daf.ct2.util.MileageAuditService;
import net.atos.daf.ct2.util.MileageConstants;
import net.atos.daf.postgre.util.DafConstants;

public class MileageStreamingJob {
	private static final Logger logger = LoggerFactory.getLogger(MileageStreamingJob.class);

	public static void main(String[] args) throws Exception {

		ParameterTool envParams = null;
		MileageStreamingJob mileageStreamingJob = null;
		try {
			logger.debug(" In MileageStreamingJob :: ");
			mileageStreamingJob = new MileageStreamingJob();
			ParameterTool params = ParameterTool.fromArgs(args);
			if (params.get("input") != null)
				envParams = ParameterTool.fromPropertiesFile(params.get("input"));

			final StreamExecutionEnvironment env = envParams.get("flink.streaming.evn").equalsIgnoreCase("default") ?
					StreamExecutionEnvironment.getExecutionEnvironment() : FlinkUtil.createStreamExecutionEnvironment(envParams);

			env.getConfig().setGlobalJobParameters(envParams);

			mileageStreamingJob.auditMileageJobDetails(envParams, "Mileage streaming job started");

			SingleOutputStreamOperator<VehicleMileage> statusDataStream = FlinkKafkaMileageMsgConsumer
					.consumeStatusMsgs(envParams, env)
					.assignTimestampsAndWatermarks(WatermarkStrategy
							.<KafkaRecord<Status>>forBoundedOutOfOrderness(Duration.ofSeconds(Long.parseLong(envParams.get(MileageConstants.MILEAGE_WATERMARK_TIME_WINDOW_SECONDS))))
							.withTimestampAssigner(new SerializableTimestampAssigner<KafkaRecord<Status>>() {

								private static final long serialVersionUID = 1L;

								@Override
								public long extractTimestamp(KafkaRecord<Status> element, long recordTimestamp) {
									long eventTm = TimeFormatter.getInstance().getCurrentUTCTime();
									try {
										eventTm = TimeFormatter.getInstance().convertUTCToEpochMilli(
												element.getValue().getEvtDateTime(),
												 MileageConstants.DATE_FORMAT);
									} catch (Exception e) {
										logger.error("Issue mandatory field is null, msg :{}", element.getValue());
									}

									return eventTm;
								}
							}))
					.keyBy(rec ->rec.getValue().getVin()!=null ? rec.getValue().getVin() : rec.getValue().getVid())
					.map(new MapFunction<KafkaRecord<Status>, VehicleMileage>() {
						/**
						 * 
						 */
						private static final long serialVersionUID = 1L;

						@Override
						public VehicleMileage map(KafkaRecord<Status> kafkaRec) {
							return fetchMileageData(kafkaRec.getValue());
						}
					});

			MileageProcessing mileageProcessing = new MileageProcessing();
			SingleOutputStreamOperator<TripMileage> tripMileageData = mileageProcessing.mileageDataProcessing(
					statusDataStream,
					Long.parseLong(envParams.get(MileageConstants.MILEAGE_TIME_WINDOW_SECONDS)));
		
			
			MileageJdbcSink mileageSinkObj = new MileageJdbcSink();
			
			if("true".equals(envParams.get(DafConstants.JDBC_SINK_ENABLED)))
				mileageSinkObj.saveMileageData(tripMileageData, envParams);
			else
				tripMileageData.addSink(new MileageSink());
			
			env.execute(envParams.get(MileageConstants.MILEAGE_STREAMING_JOB_NAME));

		} catch (Exception e) {
			mileageStreamingJob.auditMileageJobDetails(envParams, "Mileage streaming job failed ::" + e.getMessage());
			logger.error(" MileageStreamingJob failed, reason ::{} ", e);
			e.printStackTrace();
		}

	}

	public static VehicleMileage fetchMileageData(Status stsMsg) {
		VehicleMileage vMileageObj = new VehicleMileage();

		try {

			vMileageObj.setVid(stsMsg.getVid());
			if(Objects.nonNull(stsMsg.getVin()))
				vMileageObj.setVin(stsMsg.getVin());
			else
				vMileageObj.setVin(stsMsg.getVid());
			
			if (Objects.nonNull(stsMsg.getGpsStopVehDist())) {
				vMileageObj.setOdoMileage(Long.valueOf(stsMsg.getGpsStopVehDist()));
			} else
				vMileageObj.setOdoMileage(MileageConstants.ZERO_VAL);

			if (Objects.nonNull(stsMsg.getGpsStopVehDist()) && Objects.nonNull(stsMsg.getGpsStartVehDist())) {
				vMileageObj.setOdoDistance(Long.valueOf(stsMsg.getGpsStopVehDist() - stsMsg.getGpsStartVehDist()));
			} else if (Objects.nonNull(stsMsg.getGpsStopVehDist())) {
				vMileageObj.setOdoDistance(Long.valueOf(stsMsg.getGpsStopVehDist()));
			} else
				vMileageObj.setOdoDistance(MileageConstants.ZERO_VAL);

			if (Objects.nonNull(stsMsg.getDocument())) {
				vMileageObj.setGpsDistance(stsMsg.getDocument().getGpsTripDist());
			} else
				vMileageObj.setGpsDistance(MileageConstants.ZERO_VAL);

			if (Objects.nonNull(stsMsg.getEvtDateTime())) {
				vMileageObj.setEvtDateTime(TimeFormatter.getInstance()
						.convertUTCToEpochMilli(stsMsg.getEvtDateTime().toString(), MileageConstants.DATE_FORMAT));
			} else {
				if (Objects.nonNull(stsMsg.getGpsEndDateTime())) {
					vMileageObj.setEvtDateTime(TimeFormatter.getInstance().convertUTCToEpochMilli(
							stsMsg.getGpsEndDateTime().toString(), MileageConstants.DATE_FORMAT));
				} else {
					vMileageObj.setEvtDateTime(MileageConstants.ZERO_VAL);
				}
			}

			logger.info("vMileageObj ::{} ",vMileageObj);
		} catch (Exception e) {
			logger.error("Issue while mapping deserialized status object to trip mileage object :: {}" , e);
			logger.error("Issue while processing mileage record ::{} ", stsMsg);
		}
		return vMileageObj;
	}

	public void auditMileageJobDetails(ParameterTool properties, String message) {
		logger.debug("Calling audit service for Mileage Job :: ");
		try {
			new MileageAuditService().auditTrail(properties.get(MileageConstants.GRPC_SERVER),
					properties.get(MileageConstants.GRPC_PORT), MileageConstants.MILEAGE_JOB_NAME, message);
		} catch (MileageAuditServiceException e) {
			logger.error("Issue in Mileage Streaming Job :: ", e.getMessage());
		}

	}

}
