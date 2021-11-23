package net.atos.daf.ct2.common.realtime.dataprocess;

import java.util.Objects;

import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.common.realtime.postgresql.TripIndexSink;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.common.util.FlinkKafkaIndexDataConsumer;
import net.atos.daf.ct2.common.util.FlinkUtil;
import net.atos.daf.ct2.common.util.TripIndexAuditTrail;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.postgre.bo.IndexTripData;

public class TripIndexStreamingJob {
	private static Logger logger = LoggerFactory.getLogger(TripIndexStreamingJob.class);

	public static void main(String[] args) throws Exception {

		ParameterTool envParams = null;

		try {
			logger.debug(" In TripIndexDataJob :: ");
			
			ParameterTool params = ParameterTool.fromArgs(args);
			if (params.get("input") != null)
				envParams = ParameterTool.fromPropertiesFile(params.get("input"));

			final StreamExecutionEnvironment env = envParams.get("flink.streaming.evn").equalsIgnoreCase("default") ?
					StreamExecutionEnvironment.getExecutionEnvironment() :
					FlinkUtil.createStreamExecutionEnvironment(envParams,envParams.get(DafConstants.INDEX_TRIPJOB));

			env.getConfig().setGlobalJobParameters(envParams);

			// Call Audit Trail
			TripIndexAuditTrail.auditTrail(envParams, DafConstants.AUDIT_EVENT_STATUS_START,
					DafConstants.TRIP_INDEX_JOB_NAME, "Trip Index Streaming Job Started",
					DafConstants.AUDIT_CREATE_EVENT_TYPE);
			FlinkKafkaIndexDataConsumer flinkKafkaConsumer = new FlinkKafkaIndexDataConsumer();

			SingleOutputStreamOperator<IndexTripData> indexTripData = flinkKafkaConsumer
					.connectToKafkaTopic(envParams, env)
					.keyBy(rec ->rec.getValue().getVin()!=null ? rec.getValue().getVin() : rec.getValue().getVid())
					.map(new MapFunction<KafkaRecord<Index>, IndexTripData>() {
						/**
						 * 
						 */
						private static final long serialVersionUID = 1L;

						@Override
						public IndexTripData map(KafkaRecord<Index> kafkaRec) {
							return fetchIndexTripData(kafkaRec.getValue());
						}
					});

			indexTripData.addSink(new TripIndexSink());
			
			env.execute(envParams.get(DafConstants.INDEX_TRIP_STREAMING_JOB_NAME));

		} catch (Exception e) {
			//tripIndexData.createAuditMap(DafConstants.AUDIT_EVENT_STATUS_FAIL, "TripIndex streaming job failed ::" + e.getMessage());
			TripIndexAuditTrail.auditTrail(envParams, DafConstants.AUDIT_EVENT_STATUS_FAIL,
					DafConstants.TRIP_INDEX_JOB_NAME, "Trip Index Streaming Job Failed"+ e.getMessage(),
					DafConstants.AUDIT_CREATE_EVENT_TYPE);
			
			logger.error("Issue TripIndexData failed, reason :: " + e);
			e.printStackTrace();
		}

	}

	public static IndexTripData fetchIndexTripData(Index idxMsg) {
		IndexTripData indexTripData = new IndexTripData();

		try {
			
			indexTripData.setVid(idxMsg.getVid());
			
			if(Objects.nonNull(idxMsg.getVin()))
				indexTripData.setVin(idxMsg.getVin());
			else
				indexTripData.setVin(idxMsg.getVid());
			
			if (idxMsg.getEvtDateTime() != null) {
				indexTripData.setEvtDateTime(TimeFormatter.getInstance().convertUTCToEpochMilli(
						idxMsg.getEvtDateTime(), DafConstants.DTM_TS_FORMAT));
			}else
				indexTripData.setEvtDateTime(DafConstants.ZERO_VAL);

			if (idxMsg.getDocument() != null) {
				if(Objects.nonNull(idxMsg.getDocument().getTripID()))
					indexTripData.setTripId(idxMsg.getDocument().getTripID());
				else
					indexTripData.setTripId("UNKNOWN");
				
				if(Objects.nonNull(idxMsg.getDocument().getVGrossWeightCombination()))
					indexTripData.setVGrossWeightCombination(idxMsg.getDocument().getVGrossWeightCombination());
				else
					indexTripData.setVGrossWeightCombination(DafConstants.ZERO_VAL);
				
				indexTripData.setDriver2Id(idxMsg.getDocument().getDriver2ID());
				
				if(Objects.nonNull(idxMsg.getDocument().getVTachographSpeed()))
					indexTripData.setVTachographSpeed(idxMsg.getDocument().getVTachographSpeed());
				else
					indexTripData.setVTachographSpeed(0);
								
			}

			indexTripData.setDriverId(idxMsg.getDriverID());
			indexTripData.setJobName(idxMsg.getJobName());
			
			if(Objects.nonNull(idxMsg.getIncrement()))
				indexTripData.setIncrement(idxMsg.getIncrement());
			else
				indexTripData.setIncrement(DafConstants.ZERO_VAL);
			
			if(Objects.nonNull(idxMsg.getVDist()))
				indexTripData.setVDist(idxMsg.getVDist());
			else
				indexTripData.setVDist(DafConstants.ZERO_VAL);
			
			
			if(Objects.nonNull(idxMsg.getVEvtID()))
				indexTripData.setVEvtId(idxMsg.getVEvtID());
			else
				indexTripData.setVEvtId(0);
			
		} catch (Exception e) {
			logger.error("Issue while mapping deserialized Index object to indexTripData object :: " + e);
			logger.error("Issue while processing Index record :: " + idxMsg);
		}
		return indexTripData;
	}
	
}
