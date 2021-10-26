package net.atos.daf.ct2.etl.trip;

import java.util.Objects;
import java.util.Properties;

import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.typeinfo.BasicTypeInfo;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.io.jdbc.JDBCInputFormat;
import org.apache.flink.api.java.typeutils.RowTypeInfo;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.BroadcastStream;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.etl.common.audittrail.TripAuditTrail;
import net.atos.daf.ct2.etl.common.bo.FuelCoEfficient;
import net.atos.daf.ct2.etl.common.bo.TripAggregatedData;
import net.atos.daf.ct2.etl.common.bo.TripStatusData;
import net.atos.daf.ct2.etl.common.kafka.FlinkKafkaStatusMsgConsumer;
import net.atos.daf.ct2.etl.common.postgre.EcoScoreSink;
import net.atos.daf.ct2.etl.common.postgre.TripSink;
import net.atos.daf.ct2.etl.common.postgre.VehicleFuelTypeLookup;
import net.atos.daf.ct2.etl.common.processing.TripAggregationProcessor;
import net.atos.daf.ct2.etl.common.processing.TripCalculations;
import net.atos.daf.ct2.etl.common.util.ETLConstants;
import net.atos.daf.ct2.etl.common.util.FlinkUtil;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Distribution;
import net.atos.daf.ct2.pojo.standard.SpareMatrixAcceleration;
import net.atos.daf.ct2.pojo.standard.SparseMatrix;
import net.atos.daf.ct2.pojo.standard.Status;
import net.atos.daf.ct2.serde.KafkaMessageSerializeSchema;
import net.atos.daf.postgre.bo.EcoScore;
import net.atos.daf.postgre.bo.Trip;

public class TripEtlStreamingJob {
	private static Logger logger = LoggerFactory.getLogger(TripEtlStreamingJob.class);

	public static void main(String[] args) throws Exception {

		ParameterTool envParams = null;
		try {
			logger.info(" In TripStreamingJob :: ");
			ParameterTool params = ParameterTool.fromArgs(args);
			if (params.get("input") != null)
				envParams = ParameterTool.fromPropertiesFile(params.get("input"));

			final StreamExecutionEnvironment env = FlinkUtil.createStreamExecutionEnvironment(envParams);
			env.getConfig().setGlobalJobParameters(envParams);
			//final StreamTableEnvironment tableEnv = FlinkUtil.createStreamTableEnvironment(env);
			
			TripAggregationProcessor tripAggregationNew = new TripAggregationProcessor();
			
			// Call Audit Trail
						TripAuditTrail.auditTrail(envParams, ETLConstants.AUDIT_EVENT_STATUS_START, ETLConstants.TRIP_STREAMING_JOB_NAME,
								"Trip Streaming Job Started", ETLConstants.AUDIT_CREATE_EVENT_TYPE);
			
			// Map to status data
			SingleOutputStreamOperator<TripStatusData> statusDataStream = FlinkKafkaStatusMsgConsumer
					.consumeStatusMsgs(envParams, env)
					.rebalance()
					.map(new MapFunction<KafkaRecord<Status>, TripStatusData>() {
						/**
						 * 
						 */
						private static final long serialVersionUID = 1L;
						ObjectMapper jsonMapper = new ObjectMapper();
						@Override
						public TripStatusData map(KafkaRecord<Status> kafkaRec) {
							return fetchStatusData(kafkaRec.getValue(), jsonMapper);
						}
					}).filter(rec -> {
						if (!Objects.nonNull(rec.getTripId())) {
							logger.info(" Issue TripId is null, ignoring {} ", rec);
						}
						return Objects.nonNull(rec.getTripId());
					});//.keyBy(rec -> Objects.nonNull(rec.getVin())? rec.getVin() : rec.getVid());
			
							
			String jdbcUrl = new StringBuilder(envParams.get(ETLConstants.MASTER_POSTGRE_SERVER_NAME))
	                .append(":" + Integer.parseInt(envParams.get(ETLConstants.MASTER_POSTGRE_PORT)) + "/")
	                .append(envParams.get(ETLConstants.MASTER_POSTGRE_DATABASE_NAME))
	                .append("?user=" + envParams.get(ETLConstants.MASTER_POSTGRE_USER))
	                .append("&password=" + envParams.get(ETLConstants.MASTER_POSTGRE_PASSWORD))
	                .append("&sslmode=require")
	                .toString();
		
	        TypeInformation<?>[] fieldTypes = new TypeInformation<?>[] {
	        	BasicTypeInfo.DOUBLE_TYPE_INFO,
	        	BasicTypeInfo.STRING_TYPE_INFO};
	        
	        RowTypeInfo vehicleFuelInfo = new RowTypeInfo(fieldTypes);
	        JDBCInputFormat jdbcInputFormat = JDBCInputFormat
	                .buildJDBCInputFormat()
	                .setDrivername(envParams.get(ETLConstants.POSTGRE_SQL_DRIVER))
	                .setDBUrl(jdbcUrl)
	                .setQuery("select coefficient , fuel_type from master.co2coefficient ")
	                .setRowTypeInfo(vehicleFuelInfo)
	                .finish();
	        

	      SingleOutputStreamOperator<FuelCoEfficient> dbVehicleStatusStream = 
	        env.createInput(jdbcInputFormat).map(row -> { FuelCoEfficient fuelCoff = new FuelCoEfficient();
	        fuelCoff.setFuelCoefficient(Double.valueOf(String.valueOf(row.getField(0))));
	        fuelCoff.setFuelType(String.valueOf(row.getField(1))); return fuelCoff;  });
	        
	  	MapStateDescriptor<String, Double> fuelCoffStreamMapState = new MapStateDescriptor<String, Double>("fuelCoffState",
				TypeInformation.of(String.class), TypeInformation.of(Double.class));  
	  	
	  	BroadcastStream<FuelCoEfficient> fuelCoffBroadcast = dbVehicleStatusStream.broadcast(fuelCoffStreamMapState);
	  	
	  	//Temp fix need to be removed
	  	SingleOutputStreamOperator<TripStatusData> statusDataStreamWithFuelType =  statusDataStream.filter(rec -> Objects.nonNull(rec.getFuelType()));
	  	SingleOutputStreamOperator<TripStatusData> statusDataStreamWithOutFuelType =  statusDataStream.filter(rec -> Objects.isNull(rec.getFuelType())).flatMap(new VehicleFuelTypeLookup());
	  	DataStream<TripStatusData> statusDataStreamFinal = statusDataStreamWithOutFuelType.union(statusDataStreamWithFuelType);
	  	
	  	SingleOutputStreamOperator<TripAggregatedData> tripStsWithCo2Emission = new TripCalculations().calculateTripStatistics(statusDataStreamFinal, fuelCoffBroadcast);
	  		
	  	SingleOutputStreamOperator<TripAggregatedData> tripAggrData;
		
		if(ETLConstants.TRUE.equals(envParams.get(ETLConstants.LOOKPUP_HBASE_TABLE)))
			tripAggrData = tripAggregationNew.getHbaseLookUpData(tripStsWithCo2Emission, envParams);
		else
			tripAggrData =tripAggregationNew.getTripGranularData(tripStsWithCo2Emission);
						
		DataStream<Trip> finalTripData = tripAggrData.map(new MapToTripData()).filter(rec -> Objects.nonNull(rec));
		DataStream<EcoScore> ecoScoreData = tripAggrData.map(new MapToEcoScoreData()).filter(rec -> Objects.nonNull(rec));
		//DataStream<EcoScore> ecoScoreData = tripAggregationNew.getEcoScoreData(tripAggrData, tableEnv).filter(rec -> Objects.nonNull(rec));
	  	
		ecoScoreData.addSink(new EcoScoreSink()).name("EcoScore Sink");
		finalTripData.addSink(new TripSink()).name("Trip Sink");
		
		if (ETLConstants.TRUE.equals(envParams.get(ETLConstants.EGRESS_TRIP_AGGR_DATA))){
				ObjectMapper mapper = new ObjectMapper();
			  finalTripData.map(new MapFunction<Trip, KafkaRecord<String>>(){
				  private static final long serialVersionUID = 1L;

					@Override
					public KafkaRecord<String> map(Trip rec) throws Exception {
						KafkaRecord<String> kafkaRec = new KafkaRecord<>();
						kafkaRec.setKey(rec.getTripId());
						try {
							kafkaRec.setValue(mapper.writeValueAsString(rec));
						} catch (JsonProcessingException e) {
							logger.error("Issue while parsing Trip into JSON: " + e.getMessage());
						}
						logger.info("Aggregated Json trip structure :: "+kafkaRec);

						return kafkaRec;
					
					}
			  }
			 ).addSink(new FlinkKafkaProducer<KafkaRecord<String>>(envParams.get(ETLConstants.EGRESS_TRIP_AGGR_TOPIC_NAME),
						new KafkaMessageSerializeSchema<String>(envParams.get(ETLConstants.EGRESS_TRIP_AGGR_TOPIC_NAME)), getSinkProperties(envParams),
						FlinkKafkaProducer.Semantic.AT_LEAST_ONCE)).name("Egress Data");
			}
				
			env.execute(envParams.get(ETLConstants.ETL_STREAMING_JOB_NAME));

		} catch (Exception e) {

			// Call Audit Trail
			TripAuditTrail.auditTrail(envParams, ETLConstants.AUDIT_EVENT_STATUS_FAIL, ETLConstants.TRIP_STREAMING_JOB_NAME,
					"Trip Streaming Job Failed" + e.getMessage(), ETLConstants.AUDIT_CREATE_EVENT_TYPE);

			logger.error(" TripStreamingJob failed, reason :: " + e);
			e.printStackTrace();
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
	
	public static TripStatusData fetchStatusData(Status stsMsg, ObjectMapper jsonMapper)
	{
		TripStatusData tripStsData = null;
		try {
			
			tripStsData = new TripStatusData();
			tripStsData.setEtlProcessingTS(TimeFormatter.getInstance().getCurrentUTCTime());
			tripStsData.setDriverId(stsMsg.getDriverID());
			tripStsData.setVid(stsMsg.getVid());
			
			if(Objects.nonNull(stsMsg.getVin()))
				tripStsData.setVin(stsMsg.getVin());
			else
				tripStsData.setVin(stsMsg.getVid());
			
			tripStsData.setNumberOfIndexMessage(stsMsg.getNumberOfIndexMessage());
			tripStsData.setFuelType(stsMsg.getFuelType());
			tripStsData.setRoName(stsMsg.getRoName());
			
			/*if (stsMsg.getEventDateTimeFirstIndex() != null) {
				tripStsData.setStartDateTime(TimeFormatter.getInstance().convertUTCToEpochMilli(
						stsMsg.getEventDateTimeFirstIndex().toString(),
						ETLConstants.DATE_FORMAT));
			} else {
				if (stsMsg.getGpsStartDateTime() != null)
					tripStsData.setStartDateTime(TimeFormatter.getInstance().convertUTCToEpochMilli(
							stsMsg.getGpsStartDateTime().toString(),
							ETLConstants.DATE_FORMAT));
			}*/
			
			long evtDateTimeFirstIndex = convertDateStringToTS(stsMsg.getEventDateTimeFirstIndex(),  stsMsg);
			if (evtDateTimeFirstIndex != 0) {
				tripStsData.setStartDateTime(evtDateTimeFirstIndex);
			} else {
				long gpsStartDateTime = convertDateStringToTS(stsMsg.getGpsStartDateTime(),  stsMsg);
				
				if (gpsStartDateTime != 0)
					tripStsData.setStartDateTime(gpsStartDateTime);
			}

			/*if (stsMsg.getEvtDateTime() != null) {
				tripStsData.setEndDateTime(TimeFormatter.getInstance().convertUTCToEpochMilli(
						stsMsg.getEvtDateTime().toString(),
						ETLConstants.DATE_FORMAT));
			} else {
				if (stsMsg.getGpsEndDateTime() != null)
					tripStsData.setEndDateTime(TimeFormatter.getInstance().convertUTCToEpochMilli(
							stsMsg.getGpsEndDateTime().toString(),
							ETLConstants.DATE_FORMAT));
			}*/
			
			long evtDateTime = convertDateStringToTS(stsMsg.getEvtDateTime(),  stsMsg);
			if (evtDateTime != 0) {
				tripStsData.setEndDateTime(evtDateTime);
			} else {
				long gpsEndDateTime = convertDateStringToTS(stsMsg.getGpsEndDateTime(),  stsMsg);
				if (gpsEndDateTime != 0)
					tripStsData.setEndDateTime(gpsEndDateTime);
			}
			
			if (stsMsg.getGpsStopVehDist() != null)
				tripStsData.setGpsStopVehDist(Long.valueOf(stsMsg.getGpsStopVehDist()));

			if (stsMsg.getGpsStartVehDist() != null)
				tripStsData.setGpsStartVehDist(Long.valueOf(stsMsg.getGpsStartVehDist()));

			tripStsData.setGpsStartLatitude(stsMsg.getGpsStartLatitude());
			tripStsData.setGpsStartLongitude(stsMsg.getGpsStartLongitude());
			tripStsData.setGpsEndLatitude(stsMsg.getGpsEndLatitude());
			tripStsData.setGpsEndLongitude(stsMsg.getGpsEndLongitude());
			
			if (Objects.nonNull(stsMsg.getVUsedFuel()))
				tripStsData.setVUsedFuel(stsMsg.getVUsedFuel());
			else
				tripStsData.setVUsedFuel(ETLConstants.ZERO_VAL);

			if (Objects.nonNull(stsMsg.getVStopFuel()))
				tripStsData.setVStopFuel(Long.valueOf(stsMsg.getVStopFuel()));
			else
				tripStsData.setVStopFuel(ETLConstants.ZERO_VAL);

			if (stsMsg.getVStartFuel() != null)
				tripStsData.setVStartFuel(Long.valueOf(stsMsg.getVStartFuel()));
			else
				tripStsData.setVStartFuel(ETLConstants.ZERO_VAL);

			tripStsData.setReceivedTimestamp(stsMsg.getReceivedTimestamp());
			
			if(Objects.nonNull(stsMsg.getVIdleDuration()))
				tripStsData.setVIdleDuration(stsMsg.getVIdleDuration());
			else
				tripStsData.setVIdleDuration(ETLConstants.ZERO_VAL);
			
			if(Objects.nonNull(stsMsg.getVptoDuration()))
				tripStsData.setVPTODuration(stsMsg.getVptoDuration());
			else
				tripStsData.setVPTODuration(ETLConstants.ZERO_VAL);
			
			if(Objects.nonNull(stsMsg.getVHarshBrakeDuration()))
				tripStsData.setVHarshBrakeDuration(stsMsg.getVHarshBrakeDuration());
			else
				tripStsData.setVHarshBrakeDuration(ETLConstants.ZERO_VAL);
			
			if(Objects.nonNull(stsMsg.getVBrakeDuration()))
				tripStsData.setVBrakeDuration(stsMsg.getVBrakeDuration());
			else
				tripStsData.setVBrakeDuration(ETLConstants.ZERO_VAL);
			
			if(Objects.nonNull(stsMsg.getVCruiseControlDist()))
				tripStsData.setVCruiseControlDist(stsMsg.getVCruiseControlDist());
			else
				tripStsData.setVCruiseControlDist(ETLConstants.ZERO_VAL);
			
			if (tripStsData.getStartDateTime() != null && tripStsData.getEndDateTime() != null){
				tripStsData.setTripCalGpsVehTimeDiff(TimeFormatter.getInstance().subPastUtcTmFrmCurrentUtcTm(
						tripStsData.getStartDateTime(), tripStsData.getEndDateTime()));
			}else
				tripStsData.setTripCalGpsVehTimeDiff(ETLConstants.ZERO_VAL);

			if (tripStsData.getGpsStopVehDist() != null && tripStsData.getGpsStartVehDist() != null){
				tripStsData.setTripCalGpsVehDistDiff(
						tripStsData.getGpsStopVehDist() - tripStsData.getGpsStartVehDist());
			}else
				tripStsData.setTripCalGpsVehDistDiff(ETLConstants.ZERO_VAL);
			
			if(tripStsData.getTripCalGpsVehTimeDiff() != null){
				double timeDiffInHr = (tripStsData.getTripCalGpsVehTimeDiff()).doubleValue() /3600000;
				//double timeDiffInsec = (tripStsData.getTripCalGpsVehTimeDiff()).doubleValue() /1000;
				tripStsData.setTripCalVehTimeDiffInHr(timeDiffInHr);
			}else
				tripStsData.setTripCalVehTimeDiffInHr(0.0);

			// tripStsData.set(hbaseInsertionTS);
			if(stsMsg.getKafkaProcessingTS() != null)
				tripStsData.setKafkaProcessingTS(Long.valueOf(stsMsg.getKafkaProcessingTS()));
			
			if(Objects.nonNull(stsMsg.getDocument())){
				tripStsData.setTripId(stsMsg.getDocument().getTripID());
				tripStsData.setGpsTripDist(stsMsg.getDocument().getGpsTripDist());
				tripStsData.setVTripMotionDuration(stsMsg.getDocument().getVTripMotionDuration());
				
				if(Objects.nonNull(stsMsg.getDocument().getVMaxThrottlePaddleDuration()))
					tripStsData.setVMaxThrottlePaddleDuration(stsMsg.getDocument().getVMaxThrottlePaddleDuration());
				else
					tripStsData.setVMaxThrottlePaddleDuration(ETLConstants.ZERO_VAL);
				
				if(Objects.nonNull(stsMsg.getDocument().getVTripAccelerationTime()))
					tripStsData.setVTripAccelerationTime(stsMsg.getDocument().getVTripAccelerationTime());
				else
					tripStsData.setVTripAccelerationTime(ETLConstants.ZERO_VAL);
				
				if(Objects.nonNull(stsMsg.getDocument().getVTripDPABrakingCount()))
					tripStsData.setVTripDPABrakingCount(stsMsg.getDocument().getVTripDPABrakingCount());
				else
					tripStsData.setVTripDPABrakingCount(ETLConstants.ZERO_VAL);
					
				if(Objects.nonNull(stsMsg.getDocument().getVTripDPAAnticipationCount()))
					tripStsData.setVTripDPAAnticipationCount(stsMsg.getDocument().getVTripDPAAnticipationCount());
				else
					tripStsData.setVTripDPAAnticipationCount(ETLConstants.ZERO_VAL);
				
				if(Objects.nonNull(stsMsg.getDocument().getVCruiseControlFuelConsumed()))
					tripStsData.setVCruiseControlFuelConsumed(stsMsg.getDocument().getVCruiseControlFuelConsumed());
				else
					tripStsData.setVCruiseControlFuelConsumed(ETLConstants.ZERO_VAL);
				
				tripStsData.setVIdleFuelConsumed(stsMsg.getDocument().getVIdleFuelConsumed());
				
				if(Objects.nonNull(stsMsg.getDocument().getVSumTripDPABrakingScore()))
					tripStsData.setVSumTripDPABrakingScore(stsMsg.getDocument().getVSumTripDPABrakingScore());
				else
					tripStsData.setVSumTripDPABrakingScore(ETLConstants.ZERO_VAL);

				if(Objects.nonNull(stsMsg.getDocument().getVSumTripDPAAnticipationScore()))
					tripStsData.setVSumTripDPAAnticipationScore(stsMsg.getDocument().getVSumTripDPAAnticipationScore());
				else
					tripStsData.setVSumTripDPAAnticipationScore(ETLConstants.ZERO_VAL);
				
				tripStsData.setVTripIdlePTODuration(stsMsg.getDocument().getVTripIdlePTODuration());
				tripStsData.setVTripIdleWithoutPTODuration(stsMsg.getDocument().getVTripIdleWithoutPTODuration());
				
				if(stsMsg.getDocument().getVCruiseControlDistanceDistr() != null){
					Long[] distrArrayInt = stsMsg.getDocument().getVCruiseControlDistanceDistr().getDistrArrayInt();
					if(distrArrayInt != null){
						int cruiseDistrSz = distrArrayInt.length;
						
						if(cruiseDistrSz >1)
							tripStsData.setTripCalCrsCntrlDist25To50(distrArrayInt[1]);
						
						if(cruiseDistrSz >2)
							tripStsData.setTripCalCrsCntrlDist50To75(distrArrayInt[2]);
						
						if(cruiseDistrSz >3){
							Long totalCruiseAbv75 =0L;
							for(int i =3; i < cruiseDistrSz ; i++ )
								totalCruiseAbv75 = totalCruiseAbv75 + distrArrayInt[i];
							
							tripStsData.setTripCalCrsCntrlDistAbv75(totalCruiseAbv75);
						}
						
					}
					//distrArrayInt[new Integer(1)];
				}
				//Rpm Speed
				if(Objects.nonNull(stsMsg.getDocument().getVRpmTorque())){
					SparseMatrix vRpmTorque = stsMsg.getDocument().getVRpmTorque();
					
					tripStsData.setRpmTorque(convertToJson(vRpmTorque, jsonMapper));
					
					tripStsData.setOrdRpmTorque(vRpmTorque.getOrd());
					tripStsData.setAbsRpmTorque(vRpmTorque.getAbs());
					tripStsData.setNonZeroRpmTorqueMatrix(vRpmTorque.getA());
					tripStsData.setNumValRpmTorque(vRpmTorque.getIa());
					tripStsData.setClmnIdnxRpmTorque(vRpmTorque.getJa());
				}
				
				//Road Speed
				if(Objects.nonNull(stsMsg.getDocument().getVSpeedRpm())){
					SparseMatrix vSpeedRpm = stsMsg.getDocument().getVSpeedRpm();
					
					tripStsData.setRpmSpeed(convertToJson(vSpeedRpm, jsonMapper));
					
					tripStsData.setOrdRpmSpeed(vSpeedRpm.getOrd());
					tripStsData.setAbsRpmSpeed(vSpeedRpm.getAbs());
					tripStsData.setNonZeroRpmSpeedMatrix(vSpeedRpm.getA());
					tripStsData.setNumValRpmSpeed(vSpeedRpm.getIa());
					tripStsData.setClmnIdnxRpmSpeed(vSpeedRpm.getJa());
				}
								
				//Acelaration Speed
				if(Objects.nonNull(stsMsg.getDocument().getVAccelerationSpeed())){
					SpareMatrixAcceleration aclnMatrix = stsMsg.getDocument().getVAccelerationSpeed();
					
					tripStsData.setAclnSpeed(convertToJson(aclnMatrix, jsonMapper));
					tripStsData.setOrdAclnSpeed(aclnMatrix.getOrd());
					tripStsData.setAbsAclnSpeed(aclnMatrix.getAbs());
					tripStsData.setNonZeroAclnSpeedMatrix(aclnMatrix.getA());
					tripStsData.setNumValAclnSpeed(aclnMatrix.getIa());
					tripStsData.setClmnIdnxAclnSpeed(aclnMatrix.getJa());
					tripStsData.setNonZeroBrakePedalAclnSpeedMatrix(aclnMatrix.getA_VBrake());
				}	
				
				//Vehicle Status API
				//tripStsData.setVTripMotionDuration(stsMsg.getDocument().getVTripMotionDuration());
				//tripStsData.setVCruiseControlDist(stsMsg.getVCruiseControlDist());
				//tripStsData.setVCruiseControlFuelConsumed(stsMsg.getDocument().getVCruiseControlFuelConsumed());
				//tripStsData.setVTripIdleWithoutPTODuration(stsMsg.getDocument().getVTripIdleWithoutPTODuration());
				if(Objects.nonNull(stsMsg.getDocument().getVTripIdlePTOFuelConsumed()))
					tripStsData.setVTripIdlePTOFuelConsumed(stsMsg.getDocument().getVTripIdlePTOFuelConsumed());
				else
					tripStsData.setVTripIdlePTOFuelConsumed(ETLConstants.ZERO_VAL);
				
				if(Objects.nonNull(stsMsg.getVptoDist()))
					tripStsData.setVPtoDist(stsMsg.getVptoDist());
				else
					tripStsData.setVPtoDist(ETLConstants.ZERO_VAL);
				
				tripStsData.setVTripCruiseControlDuration(stsMsg.getDocument().getVTripCruiseControlDuration());
				tripStsData.setVTripIdleWithoutPTOFuelConsumed(stsMsg.getDocument().getVTripIdleWithoutPTOFuelConsumed());
				tripStsData.setVTripMotionFuelConsumed(stsMsg.getDocument().getVTripMotionFuelConsumed());
				tripStsData.setVTripMotionBrakeCount(stsMsg.getDocument().getVTripMotionBrakeCount());
				tripStsData.setVTripMotionBrakeDist(stsMsg.getDocument().getVTripMotionBrakeDist());
				tripStsData.setVTripMotionPTODuration(stsMsg.getDocument().getVTripMotionPTODuration());
				tripStsData.setVTripMotionPTOFuelConsumed(stsMsg.getDocument().getVTripMotionPTOFuelConsumed());
				
				//Acceleration Pedal Distr
				if(Objects.nonNull(stsMsg.getDocument().getVAccelerationPedalDistr())){
					Distribution accelerationPedalDistr = stsMsg.getDocument().getVAccelerationPedalDistr();
					
					tripStsData.setAclnPedalDistr(convertToJson(accelerationPedalDistr, jsonMapper));
					tripStsData.setAclnMinRangeInt(accelerationPedalDistr.getDistrMinRangeInt());
					tripStsData.setAclnMaxRangeInt(accelerationPedalDistr.getDistrMaxRangeInt());
					tripStsData.setAclnDistrStep(accelerationPedalDistr.getDistrStep());
					tripStsData.setAclnDistrArrayTime(accelerationPedalDistr.getDistrArrayTime());
				}
			
				//Retarder Torque Distr
				if(Objects.nonNull(stsMsg.getDocument().getVRetarderTorqueActualDistr())){
					Distribution retarderTorqueDistr = stsMsg.getDocument().getVRetarderTorqueActualDistr();
					
					tripStsData.setVRetarderTorqueActualDistr(convertToJson(retarderTorqueDistr, jsonMapper));
					tripStsData.setVRetarderTorqueMinRangeInt(retarderTorqueDistr.getDistrMinRangeInt());
					tripStsData.setVRetarderTorqueMaxRangeInt(retarderTorqueDistr.getDistrMaxRangeInt());
					tripStsData.setVRetarderTorqueDistrStep(retarderTorqueDistr.getDistrStep());
					tripStsData.setVRetarderTorqueDistrArrayTime(retarderTorqueDistr.getDistrArrayTime());
				}
							
				//EngineLoad At EngineSpeed Distr
				if(Objects.nonNull(stsMsg.getDocument().getVEngineLoadAtEngineSpeedDistr())){
					Distribution engTorqDistr = stsMsg.getDocument().getVEngineLoadAtEngineSpeedDistr();
					
					tripStsData.setVEngineLoadAtEngineSpeedDistr(convertToJson(engTorqDistr, jsonMapper));
					tripStsData.setVEngineLoadMinRangeInt(engTorqDistr.getDistrMinRangeInt());
					tripStsData.setVEngineLoadMaxRangeInt(engTorqDistr.getDistrMaxRangeInt());
					tripStsData.setVEngineLoadDistrStep(engTorqDistr.getDistrStep());
					tripStsData.setVEngineLoadDistrArrayTime(engTorqDistr.getDistrArrayTime());
				}
				
				
			}
		
			logger.info("On load tripStsData : {}",tripStsData);
		} catch (Exception e) {
			logger.error(
					"Issue while mapping deserialized status object to trip status object :: " + e);
			logger.error("Issue while processing record :: "+stsMsg);
		}
		return tripStsData;
	}
	
	private static String convertToJson(Object obj, ObjectMapper jsonMapper){
		try {
			return jsonMapper.writeValueAsString(obj);
		} catch (JsonProcessingException e) {
			logger.error("Issue while parsing Trip into JSON: " +obj  +"  Exception: "+ e.getMessage() );
			return null;
		}
		
	}
	
	private static long convertDateStringToTS(String dateStr, Status stsMsg){
		try {
			if(Objects.nonNull(dateStr)){
				return TimeFormatter.getInstance().convertUTCToEpochMilli(
						dateStr, ETLConstants.DATE_FORMAT);
			}else{
				return 0;
			}
		} catch (Exception e) {
			logger.error("Issue while converting Date String to epoch milli : "+dateStr + " message :"+ stsMsg  +"  Exception: "+ e.getMessage() );
			return 0;
		}
	}
		
  }

	  class MapToTripData implements MapFunction<TripAggregatedData, Trip> {
	
		private static final long serialVersionUID = 1L;
		private static Logger logger = LoggerFactory.getLogger(MapToTripData.class);
	
		@Override
		public Trip map(TripAggregatedData tripAggrData) throws Exception {
			try {
	
				Trip tripData = new Trip();
				
				tripData.setTripId(tripAggrData.getTripId());
				tripData.setVid(tripAggrData.getVid());
				tripData.setVin(tripAggrData.getVin());
				tripData.setStartDateTime(tripAggrData.getStartDateTime());
				tripData.setEndDateTime(tripAggrData.getEndDateTime());
				tripData.setGpsTripDist(tripAggrData.getGpsTripDist());
				tripData.setTripCalDist(tripAggrData.getTripCalDist());
				tripData.setVIdleDuration(tripAggrData.getVIdleDuration());
				tripData.setTripCalAvgSpeed(tripAggrData.getTripCalAvgSpeed());
				tripData.setVGrossWeightCombination(tripAggrData.getVGrossWeightCombination());
				tripData.setGpsStartVehDist(tripAggrData.getGpsStartVehDist());
				tripData.setGpsStopVehDist(tripAggrData.getGpsStopVehDist());
				tripData.setGpsStartLatitude(tripAggrData.getGpsStartLatitude());
				tripData.setGpsStartLongitude(tripAggrData.getGpsStartLongitude());
				tripData.setGpsEndLatitude(tripAggrData.getGpsEndLatitude());
				tripData.setGpsEndLongitude(tripAggrData.getGpsEndLongitude());
				tripData.setVUsedFuel(tripAggrData.getVUsedFuel());
				tripData.setTripCalUsedFuel(tripAggrData.getTripCalUsedFuel());
				tripData.setVTripMotionDuration(tripAggrData.getVTripMotionDuration());
				tripData.setTripCalDrivingTm(tripAggrData.getTripCalDrivingTm());
				tripData.setReceivedTimestamp(tripAggrData.getReceivedTimestamp());
				tripData.setTripCalC02Emission(tripAggrData.getTripCalC02Emission());
				tripData.setTripCalFuelConsumption(tripAggrData.getTripCalFuelConsumption());
				tripData.setVTachographSpeed(tripAggrData.getVTachographSpeed());
				tripData.setTripCalAvgGrossWtComb(tripAggrData.getTripCalAvgGrossWtComb());
				tripData.setTripCalPtoDuration(tripAggrData.getTripCalPtoDuration());
				tripData.setTripCalHarshBrakeDuration(tripAggrData.getTripCalHarshBrakeDuration());
				tripData.setTripCalHeavyThrottleDuration(tripAggrData.getTripCalHeavyThrottleDuration());
				tripData.setTripCalCrsCntrlDist25To50(tripAggrData.getTripCalCrsCntrlDist25To50());
				tripData.setTripCalCrsCntrlDist50To75(tripAggrData.getTripCalCrsCntrlDist50To75());
				tripData.setTripCalCrsCntrlDistAbv75(tripAggrData.getTripCalCrsCntrlDistAbv75());
				tripData.setTripCalAvgTrafficClsfn(tripAggrData.getTripCalAvgTrafficClsfn());
				tripData.setTripCalCCFuelConsumption(tripAggrData.getTripCalCCFuelConsumption());
				tripData.setVCruiseControlFuelConsumed(tripAggrData.getVCruiseControlFuelConsumed());
				tripData.setVCruiseControlDist(tripAggrData.getVCruiseControlDist());
				tripData.setVIdleFuelConsumed(tripAggrData.getVIdleFuelConsumed());
				tripData.setTripCalfuelNonActiveCnsmpt(tripAggrData.getTripCalfuelNonActiveCnsmpt());
				tripData.setTripCalDpaScore(tripAggrData.getTripCalDpaScore());
				tripData.setDriverId(tripAggrData.getDriverId());
				tripData.setDriver2Id(tripAggrData.getDriver2Id());
				tripData.setTripCalGpsVehTime(tripAggrData.getTripCalGpsVehTime());
				tripData.setTripProcessingTS(tripAggrData.getTripProcessingTS());
				tripData.setEtlProcessingTS(tripAggrData.getEtlProcessingTS());
				tripData.setKafkaProcessingTS(tripAggrData.getKafkaProcessingTS());
				tripData.setVGrossWtSum(tripAggrData.getVGrossWtSum());
				tripData.setNumberOfIndexMessage(tripAggrData.getNumberOfIndexMessage());
				tripData.setVTripDPABrakingCount(tripAggrData.getVTripDPABrakingCount());
				tripData.setVTripDPAAnticipationCount(tripAggrData.getVTripDPAAnticipationCount());
				tripData.setVSumTripDPABrakingScore(tripAggrData.getVSumTripDPABrakingScore());
				tripData.setVSumTripDPAAnticipationScore(tripAggrData.getVSumTripDPAAnticipationScore());
				tripData.setVHarshBrakeDuration(tripAggrData.getVHarshBrakeDuration());
				tripData.setVBrakeDuration(tripAggrData.getVBrakeDuration());
				tripData.setVTripIdlePTODuration(tripAggrData.getVTripIdlePTODuration());
				tripData.setVTripIdleWithoutPTODuration(tripAggrData.getVTripIdleWithoutPTODuration());
				tripData.setVPTODuration(tripAggrData.getVPTODuration());
				tripData.setVMaxThrottlePaddleDuration(tripAggrData.getVMaxThrottlePaddleDuration());
				tripData.setVTripAccelerationTime(tripAggrData.getVTripAccelerationTime());
				tripData.setRpmTorque(tripAggrData.getRpmTorque());
				tripData.setAbsRpmTorque(tripAggrData.getAbsRpmTorque());
				tripData.setOrdRpmTorque(tripAggrData.getOrdRpmTorque());
				tripData.setNonZeroRpmTorqueMatrix(tripAggrData.getNonZeroRpmTorqueMatrix());
				tripData.setNumValRpmTorque(tripAggrData.getNumValRpmTorque());
				tripData.setClmnIdnxRpmTorque(tripAggrData.getClmnIdnxRpmTorque());
				tripData.setRpmSpeed(tripAggrData.getRpmSpeed());
				tripData.setAbsRpmSpeed(tripAggrData.getAbsRpmSpeed());
				tripData.setOrdRpmSpeed(tripAggrData.getOrdRpmSpeed());
				tripData.setNonZeroRpmSpeedMatrix(tripAggrData.getNonZeroRpmSpeedMatrix());
				tripData.setNumValRpmSpeed(tripAggrData.getNumValRpmSpeed());
				tripData.setClmnIdnxRpmSpeed(tripAggrData.getClmnIdnxRpmSpeed());
				tripData.setAclnSpeed(tripAggrData.getAclnSpeed());
				tripData.setAbsAclnSpeed(tripAggrData.getAbsAclnSpeed());
				tripData.setOrdAclnSpeed(tripAggrData.getOrdAclnSpeed());
				tripData.setNonZeroAclnSpeedMatrix(tripAggrData.getNonZeroAclnSpeedMatrix());
				tripData.setNonZeroBrakePedalAclnSpeedMatrix(tripAggrData.getNonZeroBrakePedalAclnSpeedMatrix());
				tripData.setNumValAclnSpeed(tripAggrData.getNumValAclnSpeed());
				tripData.setClmnIdnxAclnSpeed(tripAggrData.getClmnIdnxAclnSpeed());
				
				tripData.setVTripIdlePTOFuelConsumed(tripAggrData.getVTripIdlePTOFuelConsumed());
				tripData.setVPtoDist(tripAggrData.getVPtoDist());
				tripData.setIdlingConsumptionWithPTO(tripAggrData.getIdlingConsumptionWithPTO());
				tripData.setVTripCruiseControlDuration(tripAggrData.getVTripCruiseControlDuration());
				tripData.setVTripIdleWithoutPTOFuelConsumed(tripAggrData.getVTripIdleWithoutPTOFuelConsumed());
				tripData.setVTripMotionFuelConsumed(tripAggrData.getVTripMotionFuelConsumed());
				tripData.setVTripMotionBrakeCount(tripAggrData.getVTripMotionBrakeCount());
				tripData.setVTripMotionBrakeDist(tripAggrData.getVTripMotionBrakeDist());
				tripData.setVTripMotionPTODuration(tripAggrData.getVTripMotionPTODuration());
				tripData.setVTripMotionPTOFuelConsumed(tripAggrData.getVTripMotionPTOFuelConsumed());
				tripData.setAclnPedalDistr(tripAggrData.getAclnPedalDistr());
				tripData.setAclnMinRangeInt(tripAggrData.getAclnMinRangeInt());
				tripData.setAclnMaxRangeInt(tripAggrData.getAclnMaxRangeInt());
				tripData.setAclnDistrStep(tripAggrData.getAclnDistrStep());
				tripData.setAclnDistrArrayTime(tripAggrData.getAclnDistrArrayTime());
				tripData.setVRetarderTorqueActualDistr(tripAggrData.getVRetarderTorqueActualDistr());
				tripData.setVRetarderTorqueMinRangeInt(tripAggrData.getVRetarderTorqueMinRangeInt());
				tripData.setVRetarderTorqueMaxRangeInt(tripAggrData.getVRetarderTorqueMaxRangeInt());
				tripData.setVRetarderTorqueDistrStep(tripAggrData.getVRetarderTorqueDistrStep());
				tripData.setVRetarderTorqueDistrArrayTime(tripAggrData.getVRetarderTorqueDistrArrayTime());
				tripData.setVEngineLoadAtEngineSpeedDistr(tripAggrData.getVEngineLoadAtEngineSpeedDistr());
				tripData.setVEngineLoadMinRangeInt(tripAggrData.getVEngineLoadMinRangeInt());
				tripData.setVEngineLoadMaxRangeInt(tripAggrData.getVEngineLoadMaxRangeInt());
				tripData.setVEngineLoadDistrStep(tripAggrData.getVEngineLoadDistrStep());
				tripData.setVEngineLoadDistrArrayTime(tripAggrData.getVEngineLoadDistrArrayTime());
				tripData.setRoName(tripAggrData.getRoName());
				
				return tripData;
			} catch (Exception e) {
				logger.info("Issue while mapping TripData :{}",tripAggrData);
				e.printStackTrace();
				return null;
			}
		
		}
		
	 }	
	  
	class MapToEcoScoreData implements MapFunction<TripAggregatedData, EcoScore> {

		private static final long serialVersionUID = 1L;
		private static Logger logger = LoggerFactory.getLogger(MapToEcoScoreData.class);

		@Override
		public EcoScore map(TripAggregatedData tripAggrData) throws Exception {
			try {
				EcoScore ecoScoreData = new EcoScore();
				ecoScoreData.setTripId(tripAggrData.getTripId());
				ecoScoreData.setVin(tripAggrData.getVin());
				ecoScoreData.setStartDateTime(tripAggrData.getStartDateTime());
				ecoScoreData.setEndDateTime(tripAggrData.getEndDateTime());
				ecoScoreData.setDriverId(tripAggrData.getDriverId());
				ecoScoreData.setTripCalDist(tripAggrData.getTripCalDist());
				ecoScoreData.setVTripDPABrakingCount(tripAggrData.getVTripDPABrakingCount());
				ecoScoreData.setVTripDPAAnticipationCount(tripAggrData.getVTripDPAAnticipationCount());
				ecoScoreData.setVSumTripDPABrakingScore(tripAggrData.getVSumTripDPABrakingScore());
				ecoScoreData.setVSumTripDPAAnticipationScore(tripAggrData.getVSumTripDPAAnticipationScore());
				ecoScoreData.setTripCalAvgGrossWtComb(tripAggrData.getTripCalAvgGrossWtComb());
				ecoScoreData.setTripCalUsedFuel(tripAggrData.getTripCalUsedFuel());
				ecoScoreData.setVPTODuration(tripAggrData.getVPTODuration());
				ecoScoreData.setVIdleDuration(tripAggrData.getVIdleDuration());
				ecoScoreData.setVMaxThrottlePaddleDuration(tripAggrData.getVMaxThrottlePaddleDuration());
				ecoScoreData.setVCruiseControlDist(tripAggrData.getVCruiseControlDist());
				ecoScoreData.setTripCalCrsCntrlDist25To50(tripAggrData.getTripCalCrsCntrlDist25To50());
				ecoScoreData.setTripCalCrsCntrlDist50To75(tripAggrData.getTripCalCrsCntrlDist50To75());
				ecoScoreData.setTripCalCrsCntrlDistAbv75(tripAggrData.getTripCalCrsCntrlDistAbv75());
				ecoScoreData.setTachoVGrossWtCmbSum(tripAggrData.getVGrossWtSum());
				ecoScoreData.setVHarshBrakeDuration(tripAggrData.getVHarshBrakeDuration());
				ecoScoreData.setVBrakeDuration(tripAggrData.getVBrakeDuration());
				ecoScoreData.setTripProcessingTS(tripAggrData.getTripProcessingTS());
				ecoScoreData.setEtlProcessingTS(tripAggrData.getEtlProcessingTS());
				ecoScoreData.setKafkaProcessingTS(tripAggrData.getKafkaProcessingTS());
				ecoScoreData.setVTripAccelerationTime(tripAggrData.getVTripAccelerationTime());
				ecoScoreData.setVGrossWtCmbCount(tripAggrData.getVGrossWtCmbCount());
				
				logger.info("Final ecoScoreData ::{}",ecoScoreData);

				return ecoScoreData;
			} catch (Exception e) {
				logger.info("Issue while mapping EcoScoreData :{}",tripAggrData);
				e.printStackTrace();
				return null;
			}
		}
}
