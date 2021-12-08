package net.atos.daf.ct2.etl.trip;

import java.util.Objects;

import org.apache.flink.api.common.functions.MapFunction;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.etl.common.audittrail.TripAuditTrail;
import net.atos.daf.ct2.etl.common.bo.TripGrossWeight;
import net.atos.daf.ct2.etl.common.postgre.GrossWeightEcoScoreJdbcSink;
import net.atos.daf.ct2.etl.common.postgre.GrossWeightTripJdbcSink;
import net.atos.daf.ct2.etl.common.postgre.TripAverageGrossWeightSource;
import net.atos.daf.ct2.etl.common.util.ETLConstants;

public class TripETLBatchJob {
	private static Logger logger = LoggerFactory.getLogger(TripETLBatchJob.class);

	public static void main(String[] args) throws Exception {

		ParameterTool envParams = null;
		try {
			logger.debug(" In TripETLBatchJob :: ");
			ParameterTool params = ParameterTool.fromArgs(args);
			if (params.get("input") != null)
				envParams = ParameterTool.fromPropertiesFile(params.get("input"));
			
			final StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();
			 
	        /*final StreamExecutionEnvironment env = envParams.get("flink.streaming.evn").equalsIgnoreCase("default") ?
					StreamExecutionEnvironment.getExecutionEnvironment() : FlinkUtil.createStreamExecutionEnvironment(envParams);
	         */
			env.getConfig().setGlobalJobParameters(envParams);
		    	
	        Long startDt = 0L;
	        Long endDt = 0L;
	        
	        if (params.get("startDt") != null)
	        	startDt = Long.valueOf(params.get("startDt"));
			
	        if (params.get("endDt") != null)
	        	endDt = Long.valueOf(params.get("endDt"));
	      
	        SingleOutputStreamOperator<TripGrossWeight> tripGrossWtStream = env
	        		.addSource(new TripAverageGrossWeightSource(startDt, endDt))
	        		.name("Trip GrossWeight Source")
	        		.map(new MapFunction<TripGrossWeight, TripGrossWeight>() {

						/**
						 * 
						 */
						private static final long serialVersionUID = 1L;

						@Override
						public TripGrossWeight map(TripGrossWeight value) throws Exception {
							if(Objects.isNull(value.getVGrossWtCmbCount()))
								value.setVGrossWtCmbCount(ETLConstants.ZERO_VAL);
							
							if(Objects.isNull(value.getTachographSpeed()))
								value.setTachographSpeed(ETLConstants.ZERO_VAL);
							
							if(Objects.isNull(value.getVGrossWtSum()))
								value.setVGrossWtSum(ETLConstants.DOUBLE_ZERO_VAL);
							
							if(0 != value.getVGrossWtCmbCount())
								value.setTripCalAvgGrossWtComb(value.getVGrossWtSum()/value.getVGrossWtCmbCount());
							else
								value.setTripCalAvgGrossWtComb(ETLConstants.DOUBLE_ZERO_VAL);
							
							logger.info("TripETLBatchJob Gross weight data ::{}",value);
							return value;
						}
	        			
					});
			 
	        GrossWeightTripJdbcSink tripSinkObj = new GrossWeightTripJdbcSink();
	        GrossWeightEcoScoreJdbcSink ecoScoreSinkObj = new GrossWeightEcoScoreJdbcSink();
			tripSinkObj.updateTripData(tripGrossWtStream, envParams);
			ecoScoreSinkObj.updateEcoscoreData(tripGrossWtStream, envParams);
			
			env.execute("Trip ETL Batch Job");

		} catch (Exception e) {

			// Call Audit Trail
			TripAuditTrail.auditTrail(envParams, ETLConstants.AUDIT_EVENT_STATUS_FAIL, ETLConstants.TRIP_STREAMING_JOB_NAME,
					"Trip ETL Streaming Job Failed" + e.getMessage(), ETLConstants.AUDIT_CREATE_EVENT_TYPE);

			logger.error(" TripETLBatchJob failed, reason ::{} ", e.getMessage());
			e.printStackTrace();
		}

	}
	
}
