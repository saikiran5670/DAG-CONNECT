package net.atos.daf.ct2.postgre;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.SQLException;
import java.util.Objects;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.bo.TripMileage;
import net.atos.daf.ct2.util.MileageConstants;
import net.atos.daf.postgre.connection.PostgreConnection;
import net.atos.daf.postgre.util.DafConstants;

public class MileageSink extends RichSinkFunction<TripMileage> implements Serializable{
	

	/**
	* 
	*/
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LogManager.getLogger(MileageSink.class);
	private PreparedStatement statement;
	private Connection connection;
	private ParameterTool envParams;
	
	@Override			  
	public void invoke(TripMileage rec) throws Exception {
		try {
			insertMileageData(rec, statement);
		} catch (SQLException e) {
			tryLostConnection(envParams);
			insertMileageData(rec, statement);
		}

	}

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();
			
		try {
			/*connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(envParams.get(MileageConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(MileageConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(MileageConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(MileageConstants.DATAMART_POSTGRE_USER),
					envParams.get(MileageConstants.DATAMART_POSTGRE_PASSWORD));*/
			getConnection(envParams);
		} catch (Exception e) {
			// TODO: handle exception both logger and throw is not required
			logger.error("Issue while establishing Postgre connection in Mileage streaming Job :: {}" , e);
			logger.error("serverNm ::{} , port ::{}  ", envParams.get(MileageConstants.DATAMART_POSTGRE_SERVER_NAME) 
					, Integer.parseInt(envParams.get(MileageConstants.DATAMART_POSTGRE_PORT)));
			logger.error("databaseNm :: {},  user ::{},  pwd :: {} ", envParams.get(MileageConstants.DATAMART_POSTGRE_DATABASE_NAME) 
					, envParams.get(MileageConstants.DATAMART_POSTGRE_USER) 
					, envParams.get(MileageConstants.DATAMART_POSTGRE_PASSWORD));
			logger.error("connection :: {}" , connection);
			throw e;
		}

	}

	@Override
	public void close() throws Exception {
		try {
			super.close();
			logger.info("Releasing connection in MileageSink ::{}, statement::{} ", connection, statement);
			if (Objects.nonNull(statement)) {
				statement.close();
			}
			
			if (Objects.nonNull(connection)) {
				logger.debug("Releasing connection from Mileage Job");
				connection.close();
				connection=null;
			}
		} catch (Exception e) {
			logger.error("Issue while calling MileageSink close :: {}", e);
			throw e;
		}

	}
	
	
	private void getConnection(ParameterTool envParams) throws NumberFormatException, Exception{
		
		connection = PostgreConnection.getInstance().getConnection(
				envParams.get(MileageConstants.DATAMART_POSTGRE_SERVER_NAME),
				Integer.parseInt(envParams.get(MileageConstants.DATAMART_POSTGRE_PORT)),
				envParams.get(MileageConstants.DATAMART_POSTGRE_DATABASE_NAME),
				envParams.get(MileageConstants.DATAMART_POSTGRE_USER),
				envParams.get(MileageConstants.DATAMART_POSTGRE_PASSWORD),envParams.get(MileageConstants.POSTGRE_SQL_DRIVER));
		
		logger.info("In Mileage sink connection established::{}" , connection);
		statement = connection.prepareStatement(MileageConstants.MILEAGE_QRY);
	}
	
	
	private boolean tryLostConnection(ParameterTool envParams) {
		
		logger.info("Before establishing Lostconnection in MileageSink ",TimeFormatter.getInstance().getCurrentUTCTime());
		while(Objects.isNull(connection)){
			try {
				Thread.sleep(Long.parseLong(envParams.get(DafConstants.CONNECTION_RETRY_TIME_MILLI)));
				getConnection(envParams);
			}catch (Exception e) {
				logger.error("Issue while trying for new connection in MileageSink::{} ",e.getMessage());
			}
			
		}
		logger.info("Lostconnection established in MileageSink ",TimeFormatter.getInstance().getCurrentUTCTime());
		return true;
	}
	
	private void insertMileageData(TripMileage rec, PreparedStatement ps)throws SQLException{
		try {
			if (Objects.nonNull(rec)) {

				ps.setString(1, rec.getVin());
				ps.setLong(2, rec.getEvtDateTime());
				ps.setLong(3, rec.getOdoMileage());
				ps.setDouble(4, rec.getOdoDistance());
				ps.setDouble(5, rec.getRealDistance());
				ps.setDouble(6, rec.getGpsDistance());
				ps.setLong(7, rec.getModifiedAt());
				
				ps.setLong(8, rec.getEvtDateTime());
				ps.setLong(9, rec.getOdoMileage());
				ps.setDouble(10, rec.getOdoDistance());
				ps.setDouble(11, rec.getRealDistance());
				ps.setDouble(12, rec.getGpsDistance());
				ps.setLong(13, rec.getModifiedAt());
				
				ps.execute();
			} 
		} catch (SQLException e) {
			logger.error("Sql Issue while inserting data to vehiclemileage table ::{} ", e.getMessage());
			logger.error("Sql Issue while inserting vehiclemileage record :: {}", ps);
			e.printStackTrace();
			throw e;
		} catch (Exception e) {
			logger.error("Issue while inserting data to vehiclemileage table ::{} ", e.getMessage());
			logger.error("Issue while inserting vehiclemileage record ::{} ", statement);
			e.printStackTrace();
		}
	}
}
