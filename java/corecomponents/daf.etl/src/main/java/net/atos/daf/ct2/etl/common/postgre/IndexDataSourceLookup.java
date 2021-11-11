package net.atos.daf.ct2.etl.common.postgre;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.SQLException;
import java.util.Comparator;
import java.util.List;
import java.util.Objects;
import java.util.stream.Collectors;
import java.util.stream.Stream;

import org.apache.flink.api.common.functions.RichFlatMapFunction;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.etl.common.bo.TripAggregatedData;
import net.atos.daf.ct2.etl.common.util.ETLConstants;
import net.atos.daf.ct2.etl.common.util.ETLQueries;
import net.atos.daf.postgre.bo.IndexTripData;
import net.atos.daf.postgre.connection.PostgreConnection;
import net.atos.daf.postgre.dao.ReadIndexDataDao;

public class IndexDataSourceLookup extends RichFlatMapFunction<TripAggregatedData, TripAggregatedData> {
	private static final Logger logger = LoggerFactory.getLogger(IndexDataSourceLookup.class);

	private static final long serialVersionUID = 1L;
	private Connection connection;
	private ReadIndexDataDao tripIdxDao;
	PreparedStatement tripIndexStmt;
	private Long vGrossWtThreshold = 0L;

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		super.open(parameters);
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		try {
			/*connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(ETLConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(ETLConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(ETLConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(ETLConstants.DATAMART_POSTGRE_USER),
					envParams.get(ETLConstants.DATAMART_POSTGRE_PASSWORD));*/
			
			if(envParams.get(ETLConstants.VEHICLE_GROSS_WEIGHT_THRESHOLD) != null)
				vGrossWtThreshold = Long.valueOf(envParams.get(ETLConstants.VEHICLE_GROSS_WEIGHT_THRESHOLD));
			
			connection = PostgreConnection.getInstance().getConnection(
					envParams.get(ETLConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(ETLConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(ETLConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(ETLConstants.DATAMART_POSTGRE_USER),
					envParams.get(ETLConstants.DATAMART_POSTGRE_PASSWORD),envParams.get(ETLConstants.POSTGRE_SQL_DRIVER));
			
			logger.info("In IndexDataSourceLookup sink connection done :{} ", connection);
			
			tripIdxDao = new ReadIndexDataDao();
			tripIdxDao.setConnection(connection);
			tripIndexStmt = connection.prepareStatement(ETLQueries.TRIP_INDEX_READ_STATEMENT);
			logger.info("In IndexDataSourceLookup prepared statement done :{} ", tripIndexStmt);
			
		}catch (Exception e) {
			// TODO: handle exception both logger and throw is not required
			logger.error("Issue while establishing Postgre connection in TripGranularData Job ::{} ", e);
			logger.error("serverNm ::{}, port ::{} ",envParams.get(ETLConstants.MASTER_POSTGRE_SERVER_NAME), Integer.parseInt(envParams.get(ETLConstants.MASTER_POSTGRE_PORT)));
			logger.error("databaseNm ::{},  user ::{}, pwd ::{} ", envParams.get(ETLConstants.MASTER_POSTGRE_DATABASE_NAME), envParams.get(ETLConstants.MASTER_POSTGRE_USER), envParams.get(ETLConstants.MASTER_POSTGRE_PASSWORD));
			logger.error("connection ::{} ", connection);
			throw e;
		}

	}

	@Override
	public void flatMap(TripAggregatedData stsData,
			Collector<TripAggregatedData> out){

		try {
			List<IndexTripData> tripIdxLst = tripIdxDao.read(tripIndexStmt, stsData.getTripId());
		/*
		Collections.sort(tripIdxLst,
				new Comparator<IndexTripData>() {
					@Override
					public int compare(
							IndexTripData obj1,
							IndexTripData obj2) {
						return Long.compare(obj1.getEvtDateTime(), obj2.getEvtDateTime());
					}
				});
		
		
		for(IndexTripData tripData : tripIdxLst){
			//logger.info("lookup data for trip :: "+tripData);
			Tuple11<String, String, String, Integer, Long, String, Long, Long, Long, Integer, String> tuple11 = new Tuple11<>();
			
			Long vGrossWeightCombination = tripData.getVGrossWeightCombination();
			int grossWtRec = ETLConstants.ONE;
			if (vGrossWtThreshold.compareTo(vGrossWeightCombination) < 0) {
				logger.info("Ignored index record increment: " + tripData.getIncrement() + " vGrossWeightCombination : "
						+ vGrossWeightCombination);
				vGrossWeightCombination = ETLConstants.ZERO_VAL;
				grossWtRec = ETLConstants.ZERO;
			}
			tuple11.setFields(tripData.getTripId(), tripData.getVin(), tripData.getDriver2Id(), tripData.getVTachographSpeed(), 
					vGrossWeightCombination, tripData.getJobName(), tripData.getEvtDateTime(),
					tripData.getVDist(), tripData.getIncrement(), grossWtRec, tripData.getDriverId());

			logger.info("final lookup data for trip :: "+tuple11);
			out.collect(tuple11);
			}*/
		
		Stream<IndexTripData>  streamData = tripIdxLst.stream().distinct().sorted(new Comparator<IndexTripData>() {
			@Override
			public int compare(
					IndexTripData indxData1,
					IndexTripData indxData2) {
				return Long.compare(indxData1.getEvtDateTime(), indxData2.getEvtDateTime());
			}
		});
		
		List<IndexTripData> indxLst = streamData.collect(Collectors.toList());

		long vDistDiff = 0;
		long prevVDist = 0;
		
		String driver2Id = null;
		String driverId = null;
		Long grossWtRec = ETLConstants.ZERO_VAL;
		Integer maxSpeed =0;
		Double vGrossWtSum = 0.0;
		Double calAvgGrossWtSum = 0.0;
		Long calVDistDiff =0L;
		
		for (IndexTripData indxData : indxLst) {
			
			if(4 != indxData.getVEvtId()){
				vDistDiff =indxData.getVDist() - prevVDist ;
				
				/*if(vDistDiff == 0)
					vDistDiff = 1;*/
			}
			prevVDist = indxData.getVDist();
			
			if (vGrossWtThreshold.compareTo(indxData.getVGrossWeightCombination()) < 0) {
				indxData.setVGrossWeightCombination(0L);
			}else
				grossWtRec = grossWtRec +1;
			
			if(Objects.nonNull(driver2Id) && Objects.nonNull(indxData.getDriver2Id()))
				driver2Id= indxData.getDriver2Id();
		
			if(Objects.nonNull(driverId) && Objects.nonNull(indxData.getDriverId()))
				driverId= indxData.getDriverId();
			
			if(indxData.getVTachographSpeed() > maxSpeed)
				maxSpeed = indxData.getVTachographSpeed();
			vGrossWtSum = vGrossWtSum + indxData.getVGrossWeightCombination();
			calAvgGrossWtSum  = calAvgGrossWtSum + (Double.valueOf(indxData.getVGrossWeightCombination()) * vDistDiff);
			
			calVDistDiff = calVDistDiff + vDistDiff;
			
			logger.info("New approach Final grossObj indxData:{} vGrossWtSum:{} vDistDiff:{} calAvgGrossWtSum :{}",indxData , vGrossWtSum, vDistDiff, calAvgGrossWtSum);
			
		}
		
		stsData.setDriver2Id(driver2Id);
		if(!Objects.nonNull(stsData.getDriverId()))
				stsData.setDriverId(driverId);
		if(0 != grossWtRec)
			stsData.setVGrossWeightCombination(vGrossWtSum/grossWtRec);
		else
			stsData.setVGrossWeightCombination(0.0);
		
		stsData.setVTachographSpeed(Double.valueOf(maxSpeed));
		
		/*if(0 != stsData.getTripCalDist())
			stsData.setTripCalAvgGrossWtComb(calAvgGrossWtSum/stsData.getTripCalDist());
		else
			stsData.setTripCalAvgGrossWtComb(0.0);*/
		
		stsData.setTripCalAvgGrossWtComb(calAvgGrossWtSum);
				
		stsData.setVGrossWtSum(vGrossWtSum);
		stsData.setNumberOfIndexMessage(grossWtRec);
		stsData.setVGrossWtCmbCount(calVDistDiff);
		stsData.setTripProcessingTS(TimeFormatter.getInstance().getCurrentUTCTime());
		
		logger.info("Final trip statistics {} ",stsData);
		out.collect(stsData);
} catch (Exception e) {
			// TODO error suppressed , cross verify scenarios
			logger.error("Issue while processing TripGranularData job :: " + stsData);
			logger.error("Issue while processing TripGranularData job :: " + e.getMessage());
		}
	}

	@Override
	public void close() throws Exception{
		try {
			
			super.close();
			
			if(Objects.nonNull(tripIndexStmt))
				tripIndexStmt.close();
			
			if (connection != null) {
				connection.close();
			}
		} catch (SQLException e) {
			// TODO Need to check if logging and throw is required
			logger.error("Issue while Closing Postgre table connection :: ", e);
			throw e;
		}
	}
	
}