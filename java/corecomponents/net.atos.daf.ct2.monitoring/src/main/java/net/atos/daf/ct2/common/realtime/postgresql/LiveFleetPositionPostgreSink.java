package net.atos.daf.ct2.common.realtime.postgresql;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.common.realtime.dataprocess.MonitorDataProcess;
//import net.atos.daf.ct2.common.realtime.pojo.monitordata.MonitorMessage;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Monitor;

public class LiveFleetPositionPostgreSink extends RichSinkFunction<KafkaRecord<Monitor>> {

	private static final long serialVersionUID = 1L;
	Logger log = LoggerFactory.getLogger(MonitorDataProcess.class);

	// String livefleetposition = "INSERT INTO
	// livefleet.livefleet_position_statistics ( trip_id , vin ,message_time_stamp
	// ,gps_altitude ,gps_heading ,gps_latitude ,gps_longitude ,co2_emission
	// ,fuel_consumption , last_odometer_val ,distance_until_next_service ,
	// created_at_m2m ,created_at_kafka ,created_at_dm ) VALUES (? ,? ,? ,? ,? ,? ,?
	// ,? ,? ,? ,? ,? ,? ,? )";

	String livefleetposition = null;

	private PreparedStatement statement;
	Connection connection = null;

	public void invoke(KafkaRecord<Monitor> row) throws Exception {

		int varVEvtid = 0;

		if (row.getValue().getVEvtID() != null) {

			varVEvtid = row.getValue().getVEvtID();
		}
		double varGPSLongi = row.getValue().getGpsLongitude();

		String varTripID = row.getValue().getDocument().getTripID();

		if (varTripID != null) {
			statement.setString(1, row.getValue().getDocument().getTripID()); // tripID
		}

		else if (row.getValue().getRoName() != null) {
			statement.setString(1, row.getValue().getRoName());
		} else {
			statement.setString(1, "TRIP_ID NOT AVAILABLE");
		}

		if (row.getValue().getVin() != null)
			statement.setString(2, (String) row.getValue().getVin()); // vin
		else if (row.getValue().getVid() != null) {
			statement.setString(2, (String) row.getValue().getVid());
		} else {
			statement.setString(2, "VIN NOT AVAILABLE");
		}

		if (row.getValue().getReceivedTimestamp() != 0 || row.getValue().getReceivedTimestamp() != null) {
			statement.setLong(3, row.getValue().getReceivedTimestamp()); // message_time_stamp
			statement.setLong(12, row.getValue().getReceivedTimestamp()); // created_at_m2m
			statement.setLong(13, row.getValue().getReceivedTimestamp()); // created_at_kafka
			statement.setLong(14, row.getValue().getReceivedTimestamp()); // created_at_dm
		}

		else {
			statement.setLong(3, 0); // message_time_stamp
			statement.setLong(12, 0); // created_at_m2m
			statement.setLong(13, 0); // created_at_kafka
			statement.setLong(14, 0); // created_at_dm
		}

		if (varGPSLongi == 255.0) {

			statement.setDouble(4, 255.0);
			statement.setDouble(5, 255.0);
			statement.setDouble(6, 255.0);
			statement.setDouble(7, 255.0);

		} else {

			if (row.getValue().getGpsAltitude() != null)
				statement.setDouble(4, row.getValue().getGpsAltitude());
			else
				statement.setDouble(4, 0);
			if (row.getValue().getGpsHeading() != null)
				statement.setDouble(5, row.getValue().getGpsHeading());
			else
				statement.setDouble(5, 0);
			if (row.getValue().getGpsLatitude() != null)
				statement.setDouble(6, row.getValue().getGpsLatitude());
			else
				statement.setDouble(6, 0);
			if (row.getValue().getGpsLongitude() != null)
				statement.setDouble(7, row.getValue().getGpsLongitude());
			else
				statement.setDouble(7, 0);

		}

		if (row.getValue().getDocument().getVFuelLevel1() != null)
			statement.setDouble(8, row.getValue().getDocument().getVFuelLevel1()); // CO2 Emission // Have to apply
																					// formula
		else
			statement.setDouble(8, 0); // CO2 Emission // Have to apply formula

		if (row.getValue().getDocument().getVFuelLevel1() != null)
			statement.setDouble(9, row.getValue().getDocument().getVFuelLevel1()); // fuel_consumption
		else
			statement.setDouble(9, 0); // fuel_consumption

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

		statement.executeUpdate();

	}

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

		log.info("########## In LiveFleet Position ##############");
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		livefleetposition = envParams.get(DafConstants.QUERY_LIVEFLEET_POSITION);

		try {
			//

			// Jdbc3PoolingDataSource dataSource =
			// PostgreDataSourceConnection.getDataSource(
			// envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
			// Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)),
			// envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
			// envParams.get(DafConstants.DATAMART_POSTGRE_USER),
			// envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));
			// connection = PostgreDataSourceConnection.getDataSourceConnection(dataSource);

			Class.forName(envParams.get(DafConstants.POSTGRE_SQL_DRIVER));
			String dbUrl = createValidUrlToConnectPostgreSql(envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(DafConstants.DATAMART_POSTGRE_USER),
					envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));
			connection = DriverManager.getConnection(dbUrl);

			log.info("Connect created individually ::: " + connection);

			statement = connection.prepareStatement(livefleetposition); // QUERY EXECUTION

		} catch (Exception e) {

			log.error("Error in Live fleet position" + e.getMessage());

		}

	}

	private String createValidUrlToConnectPostgreSql(String serverNm, int port, String databaseNm, String userNm,
			String password) throws Exception {

		String encodedPassword = encodeValue(password);
		String url = serverNm + ":" + port + "/" + databaseNm + "?" + "user=" + userNm + "&" + "password="
				+ encodedPassword + DafConstants.POSTGRE_SQL_SSL_MODE;

		log.info("Valid Url = " + url);

		return url;
	}

	private static String encodeValue(String value) {
		try {
			return URLEncoder.encode(value, StandardCharsets.UTF_8.toString());
		} catch (UnsupportedEncodingException ex) {
			throw new RuntimeException(ex.getCause());
		}
	}

	@Override
	public void close() throws Exception {

		connection.close();

		log.info("In Close");

	}

}