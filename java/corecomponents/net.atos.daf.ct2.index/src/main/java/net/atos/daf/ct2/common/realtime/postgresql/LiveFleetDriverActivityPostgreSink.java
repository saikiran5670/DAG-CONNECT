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

	private PreparedStatement stmt_insert_driver_activity;
	private PreparedStatement stmt_read_driver_activity;
	private PreparedStatement stmt_insert_current_trip;
	private PreparedStatement stmt_read_current_trip;
	private PreparedStatement stmt_read_livefleet_position;

	Connection connection = null;
	String livefleetdriver = null;
	String driverreadquery = null;
	String livefleettrip = null;
	String readtrip = null;
	String readposition = null;

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

		// this function is used to set up a connection to postgres table

		log.info("########## In LiveFleet Driver Activity ##############");
		log.info("########## In LiveFleet trip statistics ##############");

		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		livefleettrip = envParams.get(DafConstants.QUERY_LIVEFLEET_TRIP_STATISTICS);
		livefleetdriver = envParams.get(DafConstants.QUERY_DRIVER_ACTIVITY);
		driverreadquery = envParams.get(DafConstants.QUERY_DRIVER_ACTIVITY_READ);
		readtrip = envParams.get(DafConstants.QUERY_LIVEFLEET_TRIP_READ);
		readposition = envParams.get(DafConstants.QUERY_LIVEFLEET_POSITION_READ);

		System.out.println("livefleetdriver --> " + livefleetdriver);
		System.out.println("driverreadquery --> " + driverreadquery);
		System.out.println("livefleettrip --> " + livefleettrip);
		System.out.println("livefleettripread --> " + readtrip);
		System.out.println("livefleetposition --> " + readposition);

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

			stmt_insert_driver_activity = connection.prepareStatement(livefleetdriver);
			stmt_insert_current_trip = connection.prepareStatement(livefleettrip);
			stmt_read_driver_activity = connection.prepareStatement(driverreadquery);
			stmt_read_current_trip = connection.prepareStatement(readtrip);
			stmt_read_livefleet_position = connection.prepareStatement(readposition);

		} catch (Exception e) {
			log.error("Error in Live fleet driver" + e.getMessage());

		}

	}

	public void invoke(KafkaRecord<Index> row) throws Exception {
		// this function is used to write data into postgres table

		// Live Fleet Driver Activity

		int varVEvtid = row.getValue().getVEvtID();
		long trip_Start_time = 0;
		String varTripID = row.getValue().getDocument().getTripID();
		String vin = row.getValue().getVin();

		long start_time_stamp = 0;
		long end_time_stamp = 0;
		double start_position_lattitude = 0;
		double start_position_longitude = 0;
		int distance_until_next_service = 0;

		stmt_read_driver_activity.setString(1, varTripID);

		ResultSet rs_driver = stmt_read_driver_activity.executeQuery();

		while (rs_driver.next()) {

			trip_Start_time = rs_driver.getLong("trip_start_time_stamp");

		}
		rs_driver.close();

		if (row.getValue().getDocument().getTripID() != null)
			stmt_insert_driver_activity.setString(1, row.getValue().getDocument().getTripID()); // 1 trip id
		else
			stmt_insert_driver_activity.setString(1, row.getValue().getRoName());

		if (varVEvtid == 4)
			stmt_insert_driver_activity.setLong(2, row.getValue().getReceivedTimestamp()); // 2 trip_start_time_stamp

		else
			stmt_insert_driver_activity.setLong(2, 0);

		if (varVEvtid == 5)
			stmt_insert_driver_activity.setLong(3, row.getValue().getReceivedTimestamp()); // 3 trip_end_time_stamp
		else
			stmt_insert_driver_activity.setLong(3, 0);

		if (row.getValue().getReceivedTimestamp() != null)
			stmt_insert_driver_activity.setLong(4, row.getValue().getReceivedTimestamp()); // 4 Activity_date
		else
			stmt_insert_driver_activity.setLong(4, 0);

		if (row.getValue().getVin() != null)
			stmt_insert_driver_activity.setString(5, (String) row.getValue().getVin()); // 5 vin
		else
			stmt_insert_driver_activity.setString(5, (String) row.getValue().getVid());

		if (row.getValue().getDriverID() != null)
			stmt_insert_driver_activity.setString(6, (String) row.getValue().getDriverID()); // 6 Driver ID
		else
			stmt_insert_driver_activity.setLong(6, 0);

		if (row.getValue().getDocument().getDriver1WorkingState() != null)
			stmt_insert_driver_activity.setInt(7, row.getValue().getDocument().getDriver1WorkingState()); // 7 Working
																											// state
																											// code
		else
			stmt_insert_driver_activity.setLong(7, 0);

		if ((row.getValue().getDocument().getDriver1CardInserted() == true)
				&& ((row.getValue().getDocument().getDriver1WorkingState() == 1)
						|| (row.getValue().getDocument().getDriver1WorkingState() == 2)
						|| (row.getValue().getDocument().getDriver1WorkingState() == 3)))
			stmt_insert_driver_activity.setLong(8, row.getValue().getReceivedTimestamp()); // 8 start_time
		else
			stmt_insert_driver_activity.setInt(8, 0);

		if ((row.getValue().getDocument().getDriver1CardInserted() == false)
				|| ((row.getValue().getDocument().getDriver1CardInserted() == true)
						&& (row.getValue().getDocument().getDriver1WorkingState() == 3)))
			stmt_insert_driver_activity.setLong(9, row.getValue().getReceivedTimestamp()); // 9 end_time
		else
			stmt_insert_driver_activity.setInt(9, 0);

		stmt_insert_driver_activity.setLong(10, (row.getValue().getReceivedTimestamp() - trip_Start_time)); // 10
																											// Duration

		stmt_insert_driver_activity.setLong(11, row.getValue().getReceivedTimestamp()); // 11 created_at_m2m
		stmt_insert_driver_activity.setLong(12, row.getValue().getReceivedTimestamp()); // 12 created_at_kafka
		stmt_insert_driver_activity.setLong(13, row.getValue().getReceivedTimestamp()); // 13 created_at_dm
		stmt_insert_driver_activity.setLong(14, row.getValue().getReceivedTimestamp()); // 14 modified_at
		stmt_insert_driver_activity.setLong(15, row.getValue().getReceivedTimestamp()); // 15
																						// last_processed_message_time

		stmt_insert_driver_activity.executeUpdate();

		// Live Fleet CURRENT TRIP Activity

		System.out.println("trip activity starting and VEvt ID is --> " + varVEvtid);

		System.out.println("outside query");

		// To read trip statistics table data

		if (varVEvtid != 4) {

			stmt_read_current_trip.setString(1, varTripID);

			ResultSet rs_trip = stmt_read_current_trip.executeQuery();

			System.out.println("inside query");

			while (rs_trip.next()) {

				start_time_stamp = rs_trip.getLong("start_time_stamp");

				end_time_stamp = rs_trip.getLong("end_time_stamp");

				start_position_lattitude = rs_trip.getDouble("start_position_lattitude");

				start_position_longitude = rs_trip.getDouble("start_position_longitude");

				System.out.println("inside query , result set data" + start_time_stamp);
			}
			rs_trip.close();
		}
		// close

		// To read position statistics table data

		stmt_read_livefleet_position.setString(1, vin);

		ResultSet rs_position = stmt_read_livefleet_position.executeQuery();

		while (rs_position.next()) {

			distance_until_next_service = rs_position.getInt("distance_until_next_service");

		}
		rs_position.close();
		// close

		if (varTripID != null) {
			stmt_insert_current_trip.setString(1, row.getValue().getDocument().getTripID()); // 1 tripID
			System.out.println("trip id " + row.getValue().getDocument().getTripID());
		}

		else {
			stmt_insert_current_trip.setString(1, row.getValue().getRoName());
		}

		if (row.getValue().getVin() != null)
			stmt_insert_current_trip.setString(2, (String) row.getValue().getVin()); // 2 vin
		else
			stmt_insert_current_trip.setString(2, (String) row.getValue().getVid());

		if (varVEvtid == 4)
			stmt_insert_current_trip.setLong(3, row.getValue().getReceivedTimestamp()); // 3 trip_start_time_stamp

		else
			stmt_insert_current_trip.setLong(3, start_time_stamp);

		stmt_insert_current_trip.setLong(4, row.getValue().getReceivedTimestamp()); // 4 trip_end_time_stamp

		stmt_insert_current_trip.setString(5, (String) row.getValue().getDriverID());// 5 DriverID

		if (varVEvtid == 4) {
			stmt_insert_current_trip.setDouble(6, row.getValue().getGpsLatitude()); // 6 start_position_lattitude
			System.out.println("start position lattitude " + row.getValue().getGpsLatitude());
		} else {
			stmt_insert_current_trip.setDouble(6, start_position_lattitude);
		}

		if (varVEvtid == 4)
			stmt_insert_current_trip.setDouble(7, row.getValue().getGpsLongitude()); // 7 start_position_longitude

		else
			stmt_insert_current_trip.setDouble(7, start_position_longitude);

		stmt_insert_current_trip.setString(8, " "); // 8 start_position - blank

		stmt_insert_current_trip.setDouble(9, row.getValue().getGpsLatitude()); // 9 last_received_position_lattitude
		stmt_insert_current_trip.setDouble(10, row.getValue().getGpsLatitude()); // 10 last_received_position_longitude

		stmt_insert_current_trip.setString(11, " "); // 11 last_known_position - blank

		Integer[] ttvalue = row.getValue().getDocument().getTt_ListValue(); // 12 vehicle status

		if (ttvalue.length == 0) {
			System.out.println("ttvalue is empty");
			stmt_insert_current_trip.setInt(12, 0);

		} else {
			int status = ttvalue[ttvalue.length - 1];

			if (status == 0) {
				stmt_insert_current_trip.setInt(12, 2);
			}

			if (status == 1 || status == 2 || status == 3) {
				stmt_insert_current_trip.setInt(12, 1);
			}

			if (status == 4 || status == 5 || status == 6) {
				stmt_insert_current_trip.setInt(12, 3);
			}

			if (status == 7) {
				stmt_insert_current_trip.setInt(12, 4);
			}
		}
		stmt_insert_current_trip.setInt(13, row.getValue().getDocument().getDriver1WorkingState()); // 13 driver1
																									// working state

		stmt_insert_current_trip.setInt(14, 0); // 14 for time being vehicle_health_status - blank (inserting 0)

		Integer[] tacho = row.getValue().getDocument().getTotalTachoMileage(); // 15 odometer value

		if (tacho.length == 0) {
			System.out.println("odometer is empty");
			stmt_insert_current_trip.setInt(15, 0);
		} else {
			int odometer_val = tacho[tacho.length - 1];

			stmt_insert_current_trip.setInt(15, odometer_val);
		}

		stmt_insert_current_trip.setInt(16, distance_until_next_service); // distance_until_next_service

		stmt_insert_current_trip.setLong(17, row.getValue().getReceivedTimestamp()); // 17 last_processed_message_time

		stmt_insert_current_trip.setString(18, (String) row.getValue().getDocument().getDriver2ID()); // 18 Driver2ID

		stmt_insert_current_trip.setInt(19, row.getValue().getDocument().getDriver2WorkingState()); // 19 driver2
																									// working state

		stmt_insert_current_trip.setLong(20, row.getValue().getReceivedTimestamp()); // 20 created_at_m2m

		stmt_insert_current_trip.setLong(21, row.getValue().getReceivedTimestamp()); // 21 created_at_kafka

		stmt_insert_current_trip.setLong(22, row.getValue().getReceivedTimestamp()); // 22 created_at_dm

		stmt_insert_current_trip.setLong(23, row.getValue().getReceivedTimestamp()); // 23 modified_at

		stmt_insert_current_trip.executeUpdate();

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
