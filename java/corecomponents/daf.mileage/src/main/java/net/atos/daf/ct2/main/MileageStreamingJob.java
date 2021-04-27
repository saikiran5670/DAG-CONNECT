package net.atos.daf.ct2.main;

import java.time.Duration;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.apache.flink.api.common.eventtime.SerializableTimestampAssigner;
import org.apache.flink.api.common.eventtime.WatermarkStrategy;
import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.common.state.MapState;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.functions.windowing.ProcessWindowFunction;
import org.apache.flink.streaming.api.windowing.assigners.TumblingProcessingTimeWindows;
import org.apache.flink.streaming.api.windowing.time.Time;
import org.apache.flink.streaming.api.windowing.windows.TimeWindow;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.bo.TripMileage;
import net.atos.daf.ct2.bo.VehicleMileage;
import net.atos.daf.ct2.exception.MileageAuditServiceException;
import net.atos.daf.ct2.kafka.FlinkKafkaMileageMsgConsumer;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.postgre.MileageSink;
import net.atos.daf.ct2.util.FlinkUtil;
import net.atos.daf.ct2.util.MileageAuditService;
import net.atos.daf.ct2.util.MileageConstants;

public class MileageStreamingJob {
	private static Logger logger = LoggerFactory.getLogger(MileageStreamingJob.class);

	public static void main(String[] args) throws Exception {

		ParameterTool envParams = null;
		MileageStreamingJob mileageStreamingJob = null;
		try {
			logger.info(" In MileageStreamingJob :: ");
			mileageStreamingJob = new MileageStreamingJob();
			ParameterTool params = ParameterTool.fromArgs(args);
			if (params.get("input") != null)
				envParams = ParameterTool.fromPropertiesFile(params.get("input"));

			final StreamExecutionEnvironment env = FlinkUtil.createStreamExecutionEnvironment(envParams);
			env.getConfig().setGlobalJobParameters(envParams);
			
			mileageStreamingJob.auditMileageJobDetails(envParams, "Mileage streaming job started");

			SingleOutputStreamOperator<VehicleMileage> statusDataStream = FlinkKafkaMileageMsgConsumer
					.consumeStatusMsgs(envParams, env).map(new MapFunction<KafkaRecord<Status>, VehicleMileage>() {
						/**
						 * 
						 */
						private static final long serialVersionUID = 1L;

						@Override
						public VehicleMileage map(KafkaRecord<Status> kafkaRec) {
							return fetchStatusData(kafkaRec.getValue());
						}
					});

			SingleOutputStreamOperator<TripMileage> tripMileageData = statusDataStream.assignTimestampsAndWatermarks(
					WatermarkStrategy.<VehicleMileage>forBoundedOutOfOrderness(Duration.ofMillis(0))
							.withTimestampAssigner(new SerializableTimestampAssigner<VehicleMileage>() {

								private static final long serialVersionUID = 1L;

								@Override
								public long extractTimestamp(VehicleMileage element, long recordTimestamp) {
									return element.getEvtDateTime();
								}
							}))
					.keyBy(value -> value.getVid()).window(TumblingProcessingTimeWindows.of(Time.seconds(Long.parseLong(envParams.get(MileageConstants.MILEAGE_TIME_WINDOW_SECONDS)))))
					.process(new ProcessWindowFunction<VehicleMileage, TripMileage, String, TimeWindow>() {
						private static final long serialVersionUID = 1L;
						ParameterTool envParam = null;
						private MapState<String, Map<Long, VehicleMileage>> modelState;
						private MapState<String, List<Long>> vehEvtTimeListState;

						@Override
						public void process(String key, Context ctx, Iterable<VehicleMileage> values,
								Collector<TripMileage> out) throws Exception {

							TripMileage tripMileage = new TripMileage();
							double odoDistance = 0;
							double gpsDistance = 0;
							long lastBusinessTS = 0;
							List<Long> vehDeleteTripTs = new ArrayList<>();
							Map<Long, VehicleMileage> vMileageMap = modelState.get(key);
							List<Long> vTimestampList = vehEvtTimeListState.get(key);
							double errorMargin = Double.parseDouble(envParam.get(MileageConstants.MILEAGE_ERROR_MARGIN));
							
							if (vTimestampList == null)
								vTimestampList = new ArrayList<>();

							for (VehicleMileage vMileageObj : values) {

								if (vMileageObj.getEvtDateTime() != null)
									vTimestampList.add(vMileageObj.getEvtDateTime());

								if (vMileageMap == null) {
									vMileageMap = new HashMap<>();
									if (vMileageObj.getEvtDateTime() != null)
										vMileageMap.put(vMileageObj.getEvtDateTime(), vMileageObj);
								} else {
									if (vMileageObj.getEvtDateTime() != null)
										vMileageMap.put(vMileageObj.getEvtDateTime(), vMileageObj);
								}

								modelState.put(key, vMileageMap);

							}
							
							Collections.sort(vTimestampList, new Comparator<Long>() {
								@Override
								public int compare(Long timeStmp1, Long timeStmp2) {
									return Long.compare(timeStmp2, timeStmp1);
								}
							});

							lastBusinessTS = TimeFormatter.getInstance().subSecondsFromUTCTime(vTimestampList.get(0),
									Long.parseLong(envParam.get(MileageConstants.MILEAGE_BUSINESS_TIME_WINDOW_SECONDS)));
							VehicleMileage vMileage = vMileageMap.get(vTimestampList.get(0));
							if (vMileage.getVin() != null)
								tripMileage.setVin(vMileage.getVin());
							else
								tripMileage.setVin(vMileage.getVid());

							tripMileage.setOdoMileage(vMileage.getOdoMileage());
							tripMileage.setEvtDateTime(vMileage.getEvtDateTime());

							for (Map.Entry<Long, VehicleMileage> entry : vMileageMap.entrySet()) {
								if (entry.getKey().longValue() > lastBusinessTS) {
									if (entry.getValue().getOdoDistance() != null) {
										odoDistance = odoDistance + entry.getValue().getOdoDistance();
									}

									if (entry.getValue().getGpsDistance() != null)
										gpsDistance = gpsDistance + entry.getValue().getGpsDistance();
								}else {
									vehDeleteTripTs.add(entry.getKey());
								}
							}
							odoDistance = odoDistance / 1000;
							gpsDistance = gpsDistance / 1000;
							tripMileage.setGpsDistance(gpsDistance);
							tripMileage.setOdoDistance(odoDistance);
							tripMileage.setModifiedAt(TimeFormatter.getInstance().getCurrentUTCTime());

							double realMileage = (odoDistance * errorMargin > gpsDistance) ? odoDistance : gpsDistance;
							tripMileage.setRealDistance(realMileage);

							for (Long vTimestamp : vehDeleteTripTs) {
								System.out.println("deleting trip that does not fall under business critera :"
										+ vMileageMap.get(vTimestamp));
								vMileageMap.remove(vTimestamp);
							}
							modelState.put(key, vMileageMap);
							
							out.collect(tripMileage);
						}

						@Override
						public void open(org.apache.flink.configuration.Configuration config) {
							envParam = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();
							TypeInformation<Map<Long, VehicleMileage>> typeInformation = TypeInformation
									.of(new TypeHint<Map<Long, VehicleMileage>>() {
									});

							TypeInformation<List<Long>> evtTimeTypeInfo = TypeInformation
									.of(new TypeHint<List<Long>>() {
									});

							MapStateDescriptor<String, Map<Long, VehicleMileage>> descriptor = new MapStateDescriptor<String, Map<Long, VehicleMileage>>(
									"modelState", TypeInformation.of(String.class), typeInformation);
							modelState = getRuntimeContext().getMapState(descriptor);

							MapStateDescriptor<String, List<Long>> vehTimeDescriptor = new MapStateDescriptor<String, List<Long>>(
									"vehEvtTimeListState", TypeInformation.of(String.class), evtTimeTypeInfo);
							vehEvtTimeListState = getRuntimeContext().getMapState(vehTimeDescriptor);
						}

					});

			tripMileageData.addSink(new MileageSink());
			env.execute("Mileage Streaming Job");

		} catch (Exception e) {
			mileageStreamingJob.auditMileageJobDetails(envParams, "Mileage streaming job failed ::"+e.getMessage());
			logger.error(" MileageStreamingJob failed, reason :: " + e);
			e.printStackTrace();
		}

	}

	public static VehicleMileage fetchStatusData(Status stsMsg) {
		VehicleMileage vMileageObj = new VehicleMileage();

		try {

			vMileageObj.setVid(stsMsg.getVid());
			vMileageObj.setVin(stsMsg.getVin());

			if (stsMsg.getGpsStopVehDist() != null){
				vMileageObj.setOdoMileage(Long.valueOf(stsMsg.getGpsStopVehDist()));
			}else
				vMileageObj.setOdoMileage(0L);

			if (stsMsg.getGpsStopVehDist() != null && stsMsg.getGpsStartVehDist() != null){
				vMileageObj.setOdoDistance(Long.valueOf(stsMsg.getGpsStopVehDist() - stsMsg.getGpsStartVehDist()));
			} else
				vMileageObj.setOdoDistance(0L);

			if (stsMsg.getDocument() != null){
				vMileageObj.setGpsDistance(stsMsg.getDocument().getGpsTripDist());
			}else
				vMileageObj.setGpsDistance(0);
			
			if (stsMsg.getEvtDateTime() != null) {
				vMileageObj.setEvtDateTime(TimeFormatter.getInstance()
						.convertUTCToEpochMilli(stsMsg.getEvtDateTime().toString(), MileageConstants.DATE_FORMAT));
			} else {
				if (stsMsg.getGpsEndDateTime() != null){
					vMileageObj.setEvtDateTime(TimeFormatter.getInstance()
							.convertUTCToEpochMilli(stsMsg.getGpsEndDateTime().toString(), MileageConstants.DATE_FORMAT));
				}else{
					vMileageObj.setEvtDateTime(0L);
				}
			}

		} catch (Exception e) {
			logger.error("Issue while mapping deserialized status object to trip mileage object :: " + e);
			logger.error("Issue while processing mileage record :: " + stsMsg);
		}
		return vMileageObj;
	}
	
	public void auditMileageJobDetails(ParameterTool properties, String message){
		logger.info("Calling audit service for Mileage Job :: ");
		try{
			new MileageAuditService().auditTrail(
					properties.get(MileageConstants.GRPC_SERVER),
					properties.get(MileageConstants.GRPC_PORT),
					MileageConstants.MILEAGE_JOB_NAME, 
					message);
		}catch(MileageAuditServiceException e)
		{
			logger.info("Mileage Streaming Job :: ", e.getMessage());
			System.out.println("Mileage STreaming Job :: "+e);
		}
		
	}

}
