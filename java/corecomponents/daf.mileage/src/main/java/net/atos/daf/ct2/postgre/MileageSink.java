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

import net.atos.daf.ct2.bo.TripMileage;
import net.atos.daf.ct2.util.MileageConstants;
import net.atos.daf.postgre.connection.PostgreConnection;

public class MileageSink extends RichSinkFunction<TripMileage> implements Serializable{
	

	/**
	* 
	*/
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LogManager.getLogger(MileageSink.class);
	private PreparedStatement statement;
	private Connection connection;
	
	String query = "INSERT INTO mileage.vehiclemileage( vin, evt_timestamp, odo_mileage, odo_distance, real_distance, gps_distance, modified_at)"
			+ " VALUES (?, ?, ?, ?, ?, ?, ?)"
			+ "  ON CONFLICT (vin) "
			+ "  DO UPDATE SET evt_timestamp = ?, odo_mileage = ?, odo_distance = ?, real_distance = ?, gps_distance = ?, modified_at = ?" ;
			
	@Override			  
	public void invoke(TripMileage rec) throws Exception {

		try {
			statement.setString(1, rec.getVin());
			statement.setLong(2, rec.getEvtDateTime());
			statement.setLong(3, rec.getOdoMileage());
			statement.setDouble(4, rec.getOdoDistance());
			statement.setDouble(5, rec.getRealDistance());
			statement.setDouble(6, rec.getGpsDistance());
			statement.setLong(7, rec.getModifiedAt());
			
			statement.setLong(8, rec.getEvtDateTime());
			statement.setLong(9, rec.getOdoMileage());
			statement.setDouble(10, rec.getOdoDistance());
			statement.setDouble(11, rec.getRealDistance());
			statement.setDouble(12, rec.getGpsDistance());
			statement.setLong(13, rec.getModifiedAt());

			logger.debug("mileage data calculated for veh ::{} ",rec);
			statement.execute();
		} catch (SQLException e) {
			logger.error("Sql Issue while inserting data to vehiclemileage table ::{} ", e.getMessage());
			logger.error("Sql Issue while inserting vehiclemileage record :: {}", statement);
			throw e;
		} catch (Exception e) {
			logger.error("Issue while inserting data to vehiclemileage table ::{} ", e.getMessage());
			logger.error("Issue while inserting vehiclemileage record ::{} ", statement);
			e.printStackTrace();
		}
	}

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();
			
		try {
			/*connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(envParams.get(MileageConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(MileageConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(MileageConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(MileageConstants.DATAMART_POSTGRE_USER),
					envParams.get(MileageConstants.DATAMART_POSTGRE_PASSWORD));*/
			connection = PostgreConnection.getInstance().getConnection(
					envParams.get(MileageConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(MileageConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(MileageConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(MileageConstants.DATAMART_POSTGRE_USER),
					envParams.get(MileageConstants.DATAMART_POSTGRE_PASSWORD),envParams.get(MileageConstants.POSTGRE_SQL_DRIVER));
			
			logger.debug("In Mileage sink connection done::{}" , connection);
			statement = connection.prepareStatement(query);
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
			if (Objects.nonNull(statement)) {
				statement.close();
			}
			logger.info("In close() of mileageSink :: ");

			if (Objects.nonNull(connection)) {
				logger.debug("Releasing connection from Mileage Job");
				connection.close();
			}
		} catch (Exception e) {
			logger.error("Issue while calling MileageSink close :: {}", e);
			throw e;
		}

	}
	
}