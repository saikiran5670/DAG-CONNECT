package net.atos.daf.ct2.common.realtime.postgresql;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.Timestamp;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.postgresql.jdbc3.Jdbc3PoolingDataSource;

import net.atos.daf.common.ct2.postgre.PostgreDataSourceConnection;
//import net.atos.daf.ct2.common.realtime.pojo.monitordata.MonitorMessage;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Monitor;

public class LiveFleetPositionPostgreSink extends RichSinkFunction<KafkaRecord<Monitor>> {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	// vehicledatamart
	// livefleet_position_statistics
	String livefleetposition = "INSERT INTO livefleet.livefleet_position_statistics ( trip_id	, vin	,message_time_stamp	,gps_altitude	,gps_heading	,gps_latitude	,gps_longitude	,co2_emission	,fuel_consumption	, last_odometer_val  ,distance_until_next_service	, created_at_m2m	,created_at_kafka	,created_at_dm	) VALUES (?	,?	,?	,?	,?	,?	,?	,?	,?	,?	,?	,?	,?	,?	)";

	private PreparedStatement statement;
	Connection connection = null;

	public void invoke(KafkaRecord<Monitor> row) throws Exception {

		int varVEvtid = row.getValue().getVEvtID();
		double varGPSLongi = row.getValue().getGpsLongitude();
		String varTripID = row.getValue().getDocument().getTripID();

		if (varTripID != null) {
			statement.setString(1, row.getValue().getDocument().getTripID()); // tripID
		}

		else {
			statement.setString(1, row.getValue().getRoName());
		}

		statement.setString(2, row.getValue().getVin()); // vehicle_id
		statement.setLong(3, row.getValue().getReceivedTimestamp()); // message_time_stamp

		if (varGPSLongi == 255.0) {

			statement.setDouble(4, 255.0);
			statement.setDouble(5, 255.0);
			statement.setDouble(6, 255.0);
			statement.setDouble(7, 255.0);

		} else {
			statement.setDouble(4, row.getValue().getGpsAltitude()); // gps_altitude
			statement.setDouble(5, row.getValue().getGpsHeading()); // gps_heading
			statement.setDouble(6, row.getValue().getGpsLatitude()); // gps_latitude
			statement.setDouble(7, row.getValue().getGpsLongitude()); // gps_longitude

		}

		statement.setDouble(8, row.getValue().getDocument().getVFuelLevel1()); // CO2 Emission // Have to apply formula
																				// here
		statement.setDouble(9, row.getValue().getDocument().getVFuelLevel1()); // fuel_consumption

		if (varVEvtid == 26 || varVEvtid == 28 || varVEvtid == 29 || varVEvtid == 32 || varVEvtid == 42
				|| varVEvtid == 43 || varVEvtid == 44 || varVEvtid == 45 || varVEvtid == 46) {
			statement.setLong(10, row.getValue().getDocument().getVTachographSpeed()); // TotalTachoMileage
		} else {
			statement.setLong(10, 0);
		}

		if (varVEvtid == 42 || varVEvtid == 43) {
			statement.setLong(11, row.getValue().getDocument().getVDistanceUntilService());// distance_until_next_service

		} else {
			statement.setLong(11, 0);
		}

		statement.setLong(12, row.getValue().getReceivedTimestamp()); // created_at_m2m
		statement.setLong(13, row.getValue().getReceivedTimestamp()); // created_at_kafka
		statement.setLong(14, row.getValue().getReceivedTimestamp()); // created_at_dm

		statement.executeUpdate();

	}

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		/*
		 * Class.forName(envParams.get(DafConstants.POSTGRE_SQL_DRIVER)); String dbUrl =
		 * createValidUrlToConnectPostgreSql(envParams); connection =
		 * DriverManager.getConnection(dbUrl);
		 */

		Jdbc3PoolingDataSource dataSource = PostgreDataSourceConnection.getDataSource(
				envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
				Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)),
				envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
				envParams.get(DafConstants.DATAMART_POSTGRE_USER),
				envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));

		connection = PostgreDataSourceConnection.getDataSourceConnection(dataSource);

		// statement =
		// connection.prepareStatement(DAFConstants.POSTGRE_QUERY_LIVEFLEET_POSITION);
		// //QUERY EXECUTION
		statement = connection.prepareStatement(livefleetposition); // QUERY EXECUTION
	}

	
	@Override
	public void close() throws Exception {

		connection.close();

		System.out.println("In Close");

	}

}