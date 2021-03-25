package net.atos.daf.etl.ct2.trip;

import java.util.Map;
import java.util.Properties;
import java.util.concurrent.ConcurrentHashMap;

import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.java.tuple.Tuple3;
import org.apache.flink.api.java.tuple.Tuple9;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.functions.windowing.ProcessWindowFunction;
import org.apache.flink.streaming.api.windowing.assigners.TumblingProcessingTimeWindows;
import org.apache.flink.streaming.api.windowing.time.Time;
import org.apache.flink.streaming.api.windowing.windows.TimeWindow;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.apache.flink.table.api.bridge.java.StreamTableEnvironment;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.audittrail.TripAuditTrail;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.serde.KafkaMessageSerializeSchema;
//import net.atos.daf.etl.ct2.common.bo.Trip;
import net.atos.daf.etl.ct2.common.bo.TripStatusData;
import net.atos.daf.etl.ct2.common.kafka.FlinkKafkaStatusMsgConsumer;
import net.atos.daf.etl.ct2.common.postgre.TripSink;
import net.atos.daf.etl.ct2.common.util.ETLConstants;
import net.atos.daf.etl.ct2.common.util.FlinkUtil;
import net.atos.daf.postgre.bo.Trip;

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
			TripStreamingJob tripStreamingJob = new TripStreamingJob();
			TripAggregations tripAggregation = new TripAggregations();

			// Map to status data
			SingleOutputStreamOperator<TripStatusData> statusDataStream = FlinkKafkaStatusMsgConsumer
					.consumeStatusMsgs(envParams, env).map(new MapFunction<KafkaRecord<Status>, TripStatusData>() {
						/**
						 * 
						 */
						private static final long serialVersionUID = 1L;
						@Override
						public TripStatusData map(KafkaRecord<Status> kafkaRec) {
							return fetchStatusData(kafkaRec.getValue());
						}
					});//.filter(rec -> null != rec);
			
			logger.info(" completed reading the streaming data !!!!!!!!!!!!!! ");

			SingleOutputStreamOperator<Tuple9<String, String, String, Integer, Integer, String, Long, Long, Long>> indxData = tripAggregation
					.getTripIndexData(statusDataStream, tableEnv, envParams);
			
			DataStream<Tuple3<Double, Double, Double>> vehDieselEmissionFactors = tripAggregation.getVehDieselEmissionFactors(envParams, env);
			
			DataStream<Trip> finalTripData = tripAggregation.getConsolidatedTripData(statusDataStream, indxData,
					vehDieselEmissionFactors, Long.valueOf(envParams.get(ETLConstants.TRIP_TIME_WINDOW_MILLISEC)), tableEnv);
			
			// Call Audit Trail
			//System.out.println("call AuditTrial");
			TripAuditTrail.auditTrail(envParams, ETLConstants.AUDIT_EVENT_STATUS_START, ETLConstants.TRIP_STREAMING_JOB_NAME,
					"Trip Streaming Job Started", ETLConstants.AUDIT_CREATE_EVENT_TYPE);

			// TODO read master data
			finalTripData.addSink(new TripSink());
			
			if ("true".equals(envParams.get(ETLConstants.EGRESS_TRIP_AGGR_DATA))){
				System.out.println("Inside egress trip aggregated data :: ");
				tripStreamingJob.egressTripData(getSinkProperties(envParams), finalTripData,
						envParams.get(ETLConstants.EGRESS_TRIP_AGGR_TOPIC_NAME),
						Long.valueOf(envParams.get(ETLConstants.TRIP_TIME_WINDOW_MILLISEC)));
			}
			/*.writeAsText("/home/flink-vm0-user1/winfunTest.txt", FileSystem.WriteMode.OVERWRITE)
			.name("writeWinfunTestToFile");*/

	
			env.execute("Trip Streaming Job");

		} catch (Exception e) {

			// Call Audit Trail
			//System.out.println("call audit Trail from catch");
			TripAuditTrail.auditTrail(envParams, ETLConstants.AUDIT_EVENT_STATUS_FAIL, ETLConstants.TRIP_STREAMING_JOB_NAME,
					"Trip Streaming Job Failed" + e.getMessage(), ETLConstants.AUDIT_CREATE_EVENT_TYPE);

			logger.error(" TripStreamingJob failed, reason :: " + e);
			e.printStackTrace();
		}

	}
	
	public void egressTripData(Properties properties, DataStream<Trip> finalTripData, String sinkTopicNm,
			long timeInMilli) {
		try {
			finalTripData.keyBy(rec -> rec.getTripId())
					.window(TumblingProcessingTimeWindows.of(Time.milliseconds(timeInMilli)))
					.process(new ProcessWindowFunction<Trip, KafkaRecord<String>, String, TimeWindow>() {
						private static final long serialVersionUID = 1L;

						@Override
						public void process(String key, Context ctx, Iterable<Trip> values,
								Collector<KafkaRecord<String>> out) throws Exception {
							Map<String, Trip> tripMap = new ConcurrentHashMap<>();
							for (Trip in : values) {
								in.setHbaseInsertionTS(TimeFormatter.getInstance().getCurrentUTCTime());
								tripMap.put(in.getTripId(), in);								
							}

							for (Map.Entry<String, Trip> entry : tripMap.entrySet()) {
								System.out.println(" final trip record  for egress :: " + entry.getValue());
								KafkaRecord<String> kafkaRec = new KafkaRecord<>();
								kafkaRec.setKey(entry.getValue().getTripId());
								kafkaRec.setValue(entry.getValue().toString());
								out.collect(kafkaRec);
							}
						}
					})
					.addSink(new FlinkKafkaProducer<KafkaRecord<String>>(sinkTopicNm,
							new KafkaMessageSerializeSchema<String>(sinkTopicNm), properties,
							FlinkKafkaProducer.Semantic.AT_LEAST_ONCE));

		} catch (Exception e) {
			logger.error("Issue while egress trip aggregated data :: " + e);
		}
	}
	
	public static Properties getSinkProperties(ParameterTool envParams){
		Properties properties = new Properties();
		//properties.setProperty(ETLConstants.CLIENT_ID, envParams.get(ETLConstants.CLIENT_ID));
		//properties.setProperty(ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, envParams.get(ETLConstants.AUTO_OFFSET_RESET_CONFIG));
		//properties.setProperty(ETLConstants.GROUP_ID, envParams.get(ETLConstants.GROUP_ID));
		properties.setProperty(ETLConstants.BOOTSTRAP_SERVERS, envParams.get(ETLConstants.EVENT_HUB_BOOTSTRAP));
		properties.setProperty(ETLConstants.SECURITY_PROTOCOL, envParams.get(ETLConstants.SECURITY_PROTOCOL));
		properties.setProperty(ETLConstants.SASL_MECHANISM, envParams.get(ETLConstants.SASL_MECHANISM));
		properties.setProperty(ETLConstants.SASL_JAAS_CONFIG, envParams.get(ETLConstants.SASL_JAAS_CONFIG));
		//properties.setProperty(ETLConstants.REQUEST_TIMEOUT_MILLISEC, envParams.get(ETLConstants.REQUEST_TIMEOUT_MILLISEC));
		
		return properties;
	}
	
	public static TripStatusData fetchStatusData(Status stsMsg)
	{
		TripStatusData tripStsData = null;
		try {
			tripStsData = new TripStatusData();
			tripStsData.setDriverId(stsMsg.getDriverID());
			tripStsData.setVid(stsMsg.getVid());
			tripStsData.setVin(stsMsg.getVin());
			// tripStsData.setIncrement(stsMsg.getIncrement());

			if (stsMsg.getEventDateTimeFirstIndex() != null) {
				tripStsData.setStartDateTime(TimeFormatter.getInstance().convertUTCToEpochMilli(
						stsMsg.getEventDateTimeFirstIndex().toString(),
						ETLConstants.DATE_FORMAT));
			} else {
				if (stsMsg.getGpsStartDateTime() != null)
					tripStsData.setStartDateTime(TimeFormatter.getInstance().convertUTCToEpochMilli(
							stsMsg.getGpsStartDateTime().toString(),
							ETLConstants.DATE_FORMAT));
			}

			if (stsMsg.getEvtDateTime() != null) {
				tripStsData.setEndDateTime(TimeFormatter.getInstance().convertUTCToEpochMilli(
						stsMsg.getEvtDateTime().toString(),
						ETLConstants.DATE_FORMAT));
			} else {
				if (stsMsg.getGpsEndDateTime() != null)
					tripStsData.setEndDateTime(TimeFormatter.getInstance().convertUTCToEpochMilli(
							stsMsg.getGpsEndDateTime().toString(),
							ETLConstants.DATE_FORMAT));
			}
			
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

			tripStsData.setReceivedTimestamp(stsMsg.getReceivedTimestamp());
			tripStsData.setVIdleDuration(stsMsg.getVIdleDuration());
			tripStsData.setVPTODuration(stsMsg.getVptoDuration());
			tripStsData.setVHarshBrakeDuration(stsMsg.getVHarshBrakeDuration());
			tripStsData.setVBrakeDuration(stsMsg.getVBrakeDuration());
			tripStsData.setVCruiseControlDist(stsMsg.getVCruiseControlDist());
			

			if (tripStsData.getStartDateTime() != null && tripStsData.getEndDateTime() != null)
				tripStsData.setTripCalGpsVehTimeDiff(TimeFormatter.getInstance().subPastUtcTmFrmCurrentUtcTm(
						tripStsData.getStartDateTime(), tripStsData.getEndDateTime()));

			if (tripStsData.getGpsStopVehDist() != null && tripStsData.getGpsStartVehDist() != null)
				tripStsData.setTripCalGpsVehDistDiff(
						tripStsData.getGpsStopVehDist() - tripStsData.getGpsStartVehDist());
			
			if(tripStsData.getTripCalGpsVehTimeDiff() != null){
				double timeDiffInHr = (tripStsData.getTripCalGpsVehTimeDiff()).doubleValue() /3600000;
				//double timeDiffInsec = (tripStsData.getTripCalGpsVehTimeDiff()).doubleValue() /1000;
				tripStsData.setTripCalVehTimeDiffInHr(timeDiffInHr);
			}

			// tripStsData.set(hbaseInsertionTS);
			tripStsData.setEtlProcessingTS(TimeFormatter.getInstance().getCurrentUTCTime());
			if(stsMsg.getKafkaProcessingTS() != null)
				tripStsData.setKafkaProcessingTS(Long.valueOf(stsMsg.getKafkaProcessingTS()));
			
			if(stsMsg.getDocument() != null){
				tripStsData.setTripId(stsMsg.getDocument().getTripID());
				tripStsData.setGpsTripDist(stsMsg.getDocument().getGpsTripDist());
				tripStsData.setVTripMotionDuration(stsMsg.getDocument().getVTripMotionDuration());
				tripStsData.setVMaxThrottlePaddleDuration(
						stsMsg.getDocument().getVMaxThrottlePaddleDuration());
				tripStsData.setVTripAccelerationTime(stsMsg.getDocument().getVTripAccelerationTime());
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
			}
			
			logger.info("tripStsData.getTripCalVehTimeDiffInHr : "+tripStsData.getTripCalVehTimeDiffInHr());
			logger.info("driving Time : "+tripStsData.getTripCalGpsVehTimeDiff() +" idle: "+tripStsData.getVIdleDuration());
			
		} catch (Exception e) {
			logger.error(
					"Issue while mapping deserialized status object to trip status object :: " + e);
			logger.error("Issue while processing record :: "+stsMsg);
		}
		return tripStsData;
	}
}
