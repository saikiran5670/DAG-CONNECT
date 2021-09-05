package net.atos.daf.ct2.etl.common.postgre;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.SQLException;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

import org.apache.flink.api.common.functions.RichFlatMapFunction;
import org.apache.flink.api.java.tuple.Tuple11;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.etl.common.bo.TripStatusData;
import net.atos.daf.ct2.etl.common.util.ETLConstants;
import net.atos.daf.ct2.etl.common.util.ETLQueries;
import net.atos.daf.postgre.bo.IndexTripData;
import net.atos.daf.postgre.dao.ReadIndexDataDao;

public class TripGranularData extends RichFlatMapFunction<TripStatusData, Tuple11<String, String, String, Integer, Long, String, Long, Long, Long, Integer, String>> {
	private static final Logger logger = LoggerFactory.getLogger(TripGranularData.class);

	private static final long serialVersionUID = 1L;
	private Connection connection;
	private ReadIndexDataDao tripIdxDao;
	PreparedStatement tripIndexQry;
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
			
			Class.forName(envParams.get(ETLConstants.POSTGRE_SQL_DRIVER));
			String dbUrl = createValidUrlToConnectPostgreSql(envParams.get(ETLConstants.DATAMART_POSTGRE_SERVER_NAME),
						Integer.parseInt(envParams.get(ETLConstants.DATAMART_POSTGRE_PORT)),
						envParams.get(ETLConstants.DATAMART_POSTGRE_DATABASE_NAME),
						envParams.get(ETLConstants.DATAMART_POSTGRE_USER),
						envParams.get(ETLConstants.DATAMART_POSTGRE_PASSWORD));
			connection = DriverManager.getConnection(dbUrl);
			tripIdxDao = new ReadIndexDataDao();
			tripIdxDao.setConnection(connection);
			tripIndexQry = connection.prepareStatement(ETLQueries.TRIP_INDEX_READ_STATEMENT);
			
		}catch (Exception e) {
			// TODO: handle exception both logger and throw is not required
			logger.error("Issue while establishing Postgre connection in TripGranularData Job :: " + e);
			logger.error("serverNm :: "+envParams.get(ETLConstants.MASTER_POSTGRE_SERVER_NAME) +" port :: "+Integer.parseInt(envParams.get(ETLConstants.MASTER_POSTGRE_PORT)));
			logger.error("databaseNm :: "+envParams.get(ETLConstants.MASTER_POSTGRE_DATABASE_NAME) +" user :: "+envParams.get(ETLConstants.MASTER_POSTGRE_USER) + " pwd :: "+envParams.get(ETLConstants.MASTER_POSTGRE_PASSWORD));
			logger.error("connection :: " + connection);
			throw e;
		}

	}

	@Override
	public void flatMap(TripStatusData stsData,
			Collector<Tuple11<String, String, String, Integer, Long, String, Long, Long, Long, Integer, String>> out){

		try {
			
			List<IndexTripData> tripIdxLst = tripIdxDao.read(tripIndexQry, stsData.getTripId());
			
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
			}

		} catch (Exception e) {
			// TODO error suppressed , cross verify scenarios
			logger.error("Issue while processing TripGranularData job :: " + stsData);
			logger.error("Issue while processing TripGranularData job :: " + e.getMessage());
		}
	}

	@Override
	public void close() throws Exception{
		try {
			
			if (connection != null) {
				connection.close();
			}
		} catch (SQLException e) {
			// TODO Need to check if logging and throw is required
			logger.error("Issue while Closing Postgre table connection :: ", e);
			throw e;
		}
	}
	
	private String createValidUrlToConnectPostgreSql(String serverNm, int port, String databaseNm, String userNm,
			String password) throws Exception {

		String encodedPassword = encodeValue(password);
		String url = serverNm + ":" + port + "/" + databaseNm + "?" + "user=" + userNm + "&" + "password="
				+ encodedPassword + ETLConstants.POSTGRE_SQL_SSL_MODE;

		return url;
	}
	
	private String encodeValue(String value) {
		try {
			return URLEncoder.encode(value, StandardCharsets.UTF_8.toString());
		} catch (UnsupportedEncodingException ex) {
			throw new RuntimeException(ex.getCause());
		}
	}

}