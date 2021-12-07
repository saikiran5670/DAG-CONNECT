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
import net.atos.daf.ct2.bo.FuelDeviation;
import net.atos.daf.ct2.util.FuelDeviationConstants;
import net.atos.daf.postgre.connection.PostgreConnection;

public class FuelDeviationSink extends RichSinkFunction<FuelDeviation> implements Serializable {

	/**
	* 
	*/
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LogManager.getLogger(FuelDeviationSink.class);
	private PreparedStatement statement;
	private Connection connection;

	String query = "INSERT INTO livefleet.livefleet_trip_fuel_deviation(trip_id, vin, fuel_event_type, vehicle_activity_type, event_time, fuel_difference "
			+ ", odometer_val, latitude, longitude, heading, created_at) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

	@Override
	public void invoke(FuelDeviation rec) throws Exception {

		try {
			statement.setString(1, rec.getTripId());
			statement.setString(2, rec.getVin());
			statement.setString(3, rec.getFuelEvtType());
			statement.setString(4, rec.getVehActivityType());
			statement.setLong(5, rec.getEvtDateTime());
			statement.setDouble(6, rec.getFuelDiff());
			statement.setLong(7, rec.getVDist());
			statement.setDouble(8, rec.getGpsLatitude());
			statement.setDouble(9, rec.getGpsLongitude());
			statement.setDouble(10, rec.getGpsHeading());
			statement.setDouble(11, TimeFormatter.getInstance().getCurrentUTCTime());

			logger.info("FuelDeviation data for veh::{} ", rec);
			statement.execute();
		}  catch (SQLException e) {
			logger.error("Sql Issue while inserting data to fuelDeviation table ::{} ", e.getMessage());
			logger.error("Sql Issue while inserting fuelDeviation record :: {}", statement);
			e.printStackTrace();
			throw e;
		}catch (Exception e) {
			logger.error("Issue while inserting data to fuelDeviation table ::{} ", e.getMessage());
			logger.error("Issue while inserting to fuelDeviation record :: {}", statement);
			logger.error("Issue while inserting to fuelDeviation, connection ::{}, FuelDeviation rec ::{} " , connection, rec);
			e.printStackTrace();
		}
	}

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();
		try {
			/*connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_USER),
					envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_PASSWORD));*/
			connection = PostgreConnection.getInstance().getConnection(
					envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_USER),
					envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_PASSWORD),envParams.get(FuelDeviationConstants.POSTGRE_SQL_DRIVER));
			logger.info("In FuelDeviation sink connection done ::{} ", connection);
			statement = connection.prepareStatement(query);
		} catch (Exception e) {
			logger.error("Issue while establishing Postgre connection in FuelDeviation streaming Job ::{} ", e);
			logger.error("serverNm :: {}, port :: {} ", envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_SERVER_NAME)
					, Integer.parseInt(envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_PORT)));
			logger.error("databaseNm ::{}, user ::{}, pwd :: {} ", envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_DATABASE_NAME)
					, envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_USER)
					, envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_PASSWORD));
			logger.error("connection ::{} ", connection);
			throw e;
		}

	}

	@Override
	public void close() throws Exception {
		try {
			super.close();
			logger.info("Releasing connection in FuelDeviationSink ::{}, statement::{} ", connection, statement);
			
			if(Objects.nonNull(statement)) {
				statement.close();
			}
			
			if (Objects.nonNull(connection)) {
				logger.debug("Releasing connection from FuelDeviation Job");
				connection.close();
			}
		} catch (Exception e) {
			logger.error("Issue while calling close in FuelDeviation streaming Job ::{} ", e);
			throw e;
		}

	}

}