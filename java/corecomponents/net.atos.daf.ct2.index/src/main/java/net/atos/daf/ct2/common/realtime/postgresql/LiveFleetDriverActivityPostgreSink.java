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

	private PreparedStatement statement_driver;
	private PreparedStatement stmt;
	private PreparedStatement statement_trip;
	Connection connection = null;
	private PreparedStatement stmt1;

	String livefleetdriver = null;
	String readquery = null;
	String livefleettrip = null;
	String readtrip = null;

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

		// this function is used to set up a connection to postgres table

		log.info("########## In LiveFleet Driver Activity ##############");
		log.info("########## In LiveFleet trip statistics ##############");

		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		livefleettrip = envParams.get(DafConstants.QUERY_LIVEFLEET_TRIP_STATISTICS);
		livefleetdriver = envParams.get(DafConstants.QUERY_DRIVER_ACTIVITY);
		readquery = envParams.get(DafConstants.QUERY_DRIVER_ACTIVITY_READ);
		readtrip = envParams.get(DafConstants.QUERY_LIVEFLEET_TRIP_READ);

		System.out.println("livefleetdriver --> " + livefleetdriver);
		System.out.println("readquery --> " + readquery);
		System.out.println("livefleettrip --> " + livefleettrip);
		System.out.println("livefleettripread --> " + readtrip);

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
			stmt1 = connection.prepareStatement(readtrip);

		} catch (Exception e) {
			log.error("Error in Live fleet driver" + e.getMessage());

		}

	}

	public void invoke(KafkaRecord<Index> row) throws Exception {
		// this function is used to write data into postgres table
		
		//Live Fleet Driver Activity

		int varVEvtid = row.getValue().getVEvtID();
		long trip_Start_time = 0;
		String varTripID = row.getValue().getDocument().getTripID();

		String trip_ID = "";
		long start_time_stamp = 0;
		long end_time_stamp = 0;
		double start_position_lattitude = 0;
		double start_position_longitude = 0;

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
		
		
		//Live Fleet CURRENT TRIP Activity

		String KafkaTripID = row.getValue().getDocument().getTripID();

		stmt1.setString(1, KafkaTripID);

		ResultSet rs1 = stmt1.executeQuery();

		while (rs1.next()) {

			start_time_stamp = rs1.getLong("start_time_stamp");

			end_time_stamp = rs1.getLong("end_time_stamp");

			start_position_lattitude = rs1.getDouble("start_position_lattitude");

			start_position_longitude = rs1.getDouble("start_position_longitude");
		}
		rs1.close();

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
			statement_trip.setLong(3, start_time_stamp);

		statement_trip.setLong(4, row.getValue().getReceivedTimestamp()); // trip_end_time_stamp

		statement_trip.setString(5, (String) row.getValue().getDriverID());// DriverID

		if (varVEvtid == 4)
			statement_trip.setDouble(6, row.getValue().getGpsLatitude()); // start_position_lattitude

		else
			statement_trip.setDouble(6, start_position_lattitude);

		if (varVEvtid == 4)
			statement_trip.setDouble(7, row.getValue().getGpsLongitude()); // start_position_longitude

		else
			statement_trip.setDouble(7, start_position_longitude);

		statement_trip.setString(8, " "); // 8 start_position - blank

		statement_trip.setDouble(9, row.getValue().getGpsLatitude());
		statement_trip.setDouble(10, row.getValue().getGpsLatitude());

		statement_trip.setString(11, " "); // 11 last_known_position - blank

		Integer[] ttvalue = row.getValue().getDocument().getTt_ListValue(); // 12 vehicle status
		if (ttvalue.length == 0) {
			System.out.println("ttvalue is empty");
			statement_trip.setInt(12, 0);

		} else {
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
		}
		statement_trip.setInt(13, row.getValue().getDocument().getDriver1WorkingState()); // driver1 working state

		statement_trip.setInt(14, 0); // 14 for time being vehicle_health_status - blank (inserting 0)

		Integer[] tacho = row.getValue().getDocument().getTotalTachoMileage(); // 15 odometer value

		if (tacho.length == 0) {
			System.out.println("odometer is empty");
			statement_trip.setInt(15, 0);
		} else {
			int odometer_val = tacho[tacho.length - 1];

			statement_trip.setInt(15, odometer_val);
		}

		statement_trip.setInt(16, 0); // for time being distance_until_next_service - blank (inserting 0)

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
