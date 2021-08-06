package net.atos.daf.ct2.postgre;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.bo.FuelDeviation;
import net.atos.daf.ct2.util.FuelDeviationConstants;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;

public class FuelDeviationSink extends RichSinkFunction<FuelDeviation> implements Serializable {

	/**
	* 
	*/
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LogManager.getLogger(FuelDeviationSink.class);
	private PreparedStatement statement;
	private Connection connection;

	String query = "INSERT INTO livefleet.livefleet_trip_fuel_deviation(trip_id, vin, fuel_event_type, vehicle_activity_type, event_time, fuel_difference "
			+ ", odometer_val, latitude, longitude, heading, created_at)" + " VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

	@Override
	public void invoke(FuelDeviation rec) throws Exception {

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

		logger.info("FuelDeviation data for veh: " + rec);
		statement.addBatch();
		statement.executeBatch();
	}

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();
		try {
			connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_USER),
					envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_PASSWORD));
			logger.info("In FuelDeviation sink connection done" + connection);
			statement = connection.prepareStatement(query);
		} catch (Exception e) {
			logger.error("Issue while establishing Postgre connection in FuelDeviation streaming Job :: " + e);
			logger.error("serverNm :: " + envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_SERVER_NAME)
					+ " port :: " + Integer.parseInt(envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_PORT)));
			logger.error("databaseNm :: " + envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_DATABASE_NAME)
					+ " user :: " + envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_USER) + " pwd :: "
					+ envParams.get(FuelDeviationConstants.DATAMART_POSTGRE_PASSWORD));
			logger.error("connection :: " + connection);
			throw e;
		}

	}

	@Override
	public void close() throws Exception {
		super.close();
		if (statement != null) {
			statement.close();
		}
		logger.info("In close() of FuelDeviationSink :: ");

		if (connection != null) {
			logger.info("Releasing connection from FuelDeviation Job");
			connection.close();
		}

	}

}