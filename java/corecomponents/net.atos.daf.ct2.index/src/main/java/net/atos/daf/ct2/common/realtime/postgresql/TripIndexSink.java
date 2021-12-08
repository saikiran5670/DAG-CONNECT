package net.atos.daf.ct2.common.realtime.postgresql;

import java.io.Serializable;
import java.sql.Connection;
//import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.SQLException;
import java.util.Objects;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.postgre.bo.IndexTripData;
import net.atos.daf.postgre.connection.PostgreConnection;
import net.atos.daf.postgre.dao.LiveFleetTripIndexDao;

public class TripIndexSink extends RichSinkFunction<IndexTripData> implements Serializable {

	/**
	* 
	*/
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LoggerFactory.getLogger(TripIndexSink.class);

	private Connection connection;
	LiveFleetTripIndexDao tripIndexDao;
	private PreparedStatement tripIndexQry;
	private ParameterTool envParams;

	@Override
	public void invoke(IndexTripData rec) throws Exception {
		try {
			tripIndexDao.insert(rec, tripIndexQry);
		} catch (SQLException e) {
			tryLostConnection(envParams);
			tripIndexDao.insert(rec, tripIndexQry);
		}

	}

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();
		tripIndexDao = new LiveFleetTripIndexDao();
		
		try {
			/*connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(DafConstants.DATAMART_POSTGRE_USER),
					envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));*/
					
			getConnection(envParams);
			
		}catch (Exception e) {
			// TODO: handle exception both logger and throw is not required
			logger.error("Issue while establishing Postgre connection in Trip Index streaming Job :: " + e);
			logger.error("serverNm :: "+envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME) +" port :: "+Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)));
			logger.error("databaseNm :: "+envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME) +" user :: "+envParams.get(DafConstants.DATAMART_POSTGRE_USER) + " pwd :: "+envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));
			logger.error("connection :: " + connection);
			throw e;
		}
	}

	@Override
	public void close() throws Exception {
		super.close();
		logger.info("Releasing connection in TripIndexSink ::{}, statement::{} ", connection, tripIndexQry);
		if (Objects.nonNull(tripIndexQry)) {
			tripIndexQry.close();
		}
		
		if (Objects.nonNull(connection)) {
			connection.close();
			logger.debug("Releasing connection from TripIndexSink Job ::{}",connection);
			connection = null;
		}
	}
	
	private void getConnection(ParameterTool envParams) throws NumberFormatException, Exception{
		
		connection = PostgreConnection.getInstance().getConnection(
				envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
				Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)),
				envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
				envParams.get(DafConstants.DATAMART_POSTGRE_USER),
				envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD),envParams.get(DafConstants.POSTGRE_SQL_DRIVER));
		
		logger.info("In TripIndexSink connection established::{}" ,connection);
				
		tripIndexDao.setConnection(connection);
		tripIndexQry = connection.prepareStatement(DafConstants.TRIP_INDEX_INSERT_STATEMENT);
	}
	
	private boolean tryLostConnection(ParameterTool envParams) {
		
		logger.info("Before establishing Lostconnection in TripIndexSink ",TimeFormatter.getInstance().getCurrentUTCTime());
		while(Objects.isNull(connection)){
			try {
				Thread.sleep(Long.parseLong(envParams.get(DafConstants.CONNECTION_RETRY_TIME_MILLI)));
				getConnection(envParams);
			}catch (Exception e) {
				logger.error("Issue while trying for new connection in TripIndexSink ::{}",e.getMessage());
			}
			
		}
		logger.info("Lostconnection established in TripIndexSink ",TimeFormatter.getInstance().getCurrentUTCTime());
		return true;
	}
		
}