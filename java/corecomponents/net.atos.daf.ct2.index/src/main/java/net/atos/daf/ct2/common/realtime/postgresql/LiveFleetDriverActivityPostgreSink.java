package net.atos.daf.ct2.common.realtime.postgresql;

import java.io.Serializable;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.common.realtime.dataprocess.IndexDataProcess;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;

@SuppressWarnings({})
public class LiveFleetDriverActivityPostgreSink extends RichSinkFunction<KafkaRecord<Index>> implements Serializable {
	/*
	 * This class is used to write Index message data in a postgres table.
	 */

	private static Logger log = LoggerFactory.getLogger(IndexDataProcess.class);

	private static final long serialVersionUID = 1L;

	// String livefleetdriver = "INSERT INTO
	// livefleet.livefleet_trip_driver_activity (trip_id , trip_start_time_stamp ,
	// trip_end_time_stamp , activity_date, vin , driver_id , code , start_time ,
	// end_time , duration , created_at_m2m , created_at_kafka , created_at_dm ,
	// modified_at , last_processed_message_time_stamp ) VALUES ( ? , ? , ? , ? , ?
	// , ? , ? , ? , ? , ? , ? , ? , ? , ? ,?) ";
	// String readquery = "SELECT * FROM livefleet.livefleet_trip_driver_activity
	// WHERE trip_start_time_stamp !=0 AND trip_id = ?";

	private PreparedStatement statement_driver;
	private PreparedStatement stmt;
	private PreparedStatement statement_trip;
	Connection connection = null;

	String livefleetdriver = null;
	String readquery = null;
	String livefleettrip = null;

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

		// this function is used to set up a connection to postgres table

		log.info("########## In LiveFleet Driver Activity ##############");
		log.info("########## In LiveFleet trip statistics ##############");

		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		livefleettrip = envParams.get(DafConstants.QUERY_LIVEFLEET_TRIP_STATISTICS);
		livefleetdriver = envParams.get(DafConstants.QUERY_DRIVER_ACTIVITY);
		readquery = envParams.get(DafConstants.QUERY_DRIVER_ACTIVITY_READ);

		System.out.println("livefleetdriver --> " + livefleetdriver);
		System.out.println("readquery --> " + readquery);
		System.out.println("livefleettrip --> " + livefleettrip);

		try {

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

			statement_driver = connection.prepareStatement(livefleetdriver);
			statement_trip = connection.prepareStatement(livefleettrip);
			stmt = connection.prepareStatement(readquery);

		} catch (Exception e) {
			log.error("Error in Live fleet driver" + e.getMessage());

		}

	}

	public void invoke(KafkaRecord<Index> row) throws Exception {
		// this function is used to write data into postgres table

		int varVEvtid = row.getValue().getVEvtID();
		long trip_Start_time = 0;
		String varTripID = row.getValue().getDocument().getTripID();

		stmt.setString(1, varTripID);

		ResultSet rs = stmt.executeQuery();

		while (rs.next()) {

			trip_Start_time = rs.getLong("trip_start_time_stamp");

		}
		rs.close();

		if (row.getValue().getDocument().getTripID() != null)
			statement_driver.setString(1, row.getValue().getDocument().getTripID());
		else
			statement_driver.setString(1, row.getValue().getRoName());

		if (varVEvtid == 4)
			statement_driver.setLong(2, row.getValue().getReceivedTimestamp());
		// trip_start_time_stamp
		else
			statement_driver.setLong(2, 0);

		if (varVEvtid == 5)
			statement_driver.setLong(3, row.getValue().getReceivedTimestamp()); // trip_end_time_stamp
		else
			statement_driver.setLong(3, 0);

		if (row.getValue().getReceivedTimestamp() != null)
			statement_driver.setLong(4, row.getValue().getReceivedTimestamp()); // Activity_date
		else
			statement_driver.setLong(4, 0);

		if (row.getValue().getVin() != null)
			statement_driver.setString(5, (String) row.getValue().getVin());
		else
			statement_driver.setString(5, (String) row.getValue().getVid());

		if (row.getValue().getDriverID() != null)
			statement_driver.setString(6, (String) row.getValue().getDriverID()); // Driver ID
		else
			statement_driver.setLong(6, 0);

		if (row.getValue().getDocument().getDriver1WorkingState() != null)
			statement_driver.setInt(7, row.getValue().getDocument().getDriver1WorkingState()); // Working state code
		else
			statement_driver.setLong(7, 0);

		if ((row.getValue().getDocument().getDriver1CardInserted() == true)
				&& ((row.getValue().getDocument().getDriver1WorkingState() == 1)
						|| (row.getValue().getDocument().getDriver1WorkingState() == 2)
						|| (row.getValue().getDocument().getDriver1WorkingState() == 3)))
			statement_driver.setLong(8, row.getValue().getReceivedTimestamp()); // start_time
		else
			statement_driver.setInt(8, 0);

		if ((row.getValue().getDocument().getDriver1CardInserted() == false)
				|| ((row.getValue().getDocument().getDriver1CardInserted() == true)
						&& (row.getValue().getDocument().getDriver1WorkingState() == 3)))
			statement_driver.setLong(9, row.getValue().getReceivedTimestamp()); // end_time
		else
			statement_driver.setInt(9, 0);

		statement_driver.setLong(10, (row.getValue().getReceivedTimestamp() - trip_Start_time)); // Duration

		statement_driver.setLong(11, row.getValue().getReceivedTimestamp()); // created_at_m2m
		statement_driver.setLong(12, row.getValue().getReceivedTimestamp()); // created_at_kafka
		statement_driver.setLong(13, row.getValue().getReceivedTimestamp()); // created_at_dm
		statement_driver.setLong(14, row.getValue().getReceivedTimestamp()); // modified_at
		statement_driver.setLong(15, row.getValue().getReceivedTimestamp()); // last_processed_message_time

		statement_driver.executeUpdate();

		if (varTripID != null) {
			statement_trip.setString(1, row.getValue().getDocument().getTripID()); // tripID
		}

		else {
			statement_trip.setString(1, row.getValue().getRoName());
		}

		if (row.getValue().getVin() != null)
			statement_trip.setString(2, (String) row.getValue().getVin()); // vin
		else
			statement_trip.setString(2, (String) row.getValue().getVid());

		if (varVEvtid == 4)
			statement_trip.setLong(3, row.getValue().getReceivedTimestamp()); // trip_start_time_stamp

		else
			statement_trip.setLong(3, 0);

		statement_trip.setLong(4, row.getValue().getReceivedTimestamp()); // trip_end_time_stamp

		statement_trip.setString(5, (String) row.getValue().getDriverID());// DriverID

		if (varVEvtid == 4)
			statement_trip.setDouble(6, row.getValue().getGpsLatitude()); // start_position_lattitude

		else
			statement_trip.setDouble(6, 0);

		if (varVEvtid == 4)
			statement_trip.setDouble(7, row.getValue().getGpsLongitude()); // start_position_longitude

		else
			statement_trip.setDouble(7, 0);

		// 8 start_position - blank

		statement_trip.setDouble(9, row.getValue().getGpsLatitude());
		statement_trip.setDouble(10, row.getValue().getGpsLatitude());

		// 11 last_known_position - blank

		Integer[] ttvalue = row.getValue().getDocument().getTt_ListValue(); // 12 vehicle status

		int status = ttvalue[ttvalue.length - 1];

		if (status == 0) {
			statement_trip.setInt(12, 2);
		}

		if (status == 1 || status == 2 || status == 3) {
			statement_trip.setInt(12, 1);
		}

		if (status == 4 || status == 5 || status == 6) {
			statement_trip.setInt(12, 3);
		}

		if (status == 7) {
			statement_trip.setInt(12, 4);
		}

		statement_trip.setInt(13, row.getValue().getDocument().getDriver1WorkingState()); // driver1 working state

		// 14 vehicle_health_status - blank

		Integer[] tacho = row.getValue().getDocument().getTotalTachoMileage(); // 15 odometer value

		int odometer_val = tacho[tacho.length - 1];

		statement_trip.setInt(15, odometer_val);

		statement_trip.setLong(17, row.getValue().getReceivedTimestamp()); // last_processed_message_time

		statement_trip.setString(18, (String) row.getValue().getDocument().getDriver2ID()); // Driver2ID

		statement_trip.setInt(19, row.getValue().getDocument().getDriver2WorkingState()); // driver2 working state

		statement_trip.setLong(20, row.getValue().getReceivedTimestamp()); // created_at_m2m

		statement_trip.setLong(21, row.getValue().getReceivedTimestamp()); // created_at_kafka

		statement_trip.setLong(22, row.getValue().getReceivedTimestamp()); // created_at_dm

		statement_trip.setLong(23, row.getValue().getReceivedTimestamp()); // modified_at

		statement_trip.executeUpdate();

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

		log.error("Error");

		log.info("In Close");

	}

}
