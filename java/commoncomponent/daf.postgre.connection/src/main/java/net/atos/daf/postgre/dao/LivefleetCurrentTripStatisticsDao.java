package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.postgre.bo.CurrentTrip;

public class LivefleetCurrentTripStatisticsDao implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private Connection connection;

	/** SQL statement for insert. */
	private static final String READ_CURRENT_TRIP = "SELECT * FROM livefleet.livefleet_current_trip_statistics WHERE trip_id = ? ORDER BY created_at_m2m ASC limit 1";
	private static final String INSERT_CURRENT_TRIP = "INSERT INTO livefleet.livefleet_current_trip_statistics ( trip_id   , vin        ,start_time_stamp          ,end_time_stamp                ,driver1_id          ,start_position_lattitude              ,start_position_longitude             ,start_position                ,last_received_position_lattitude             , last_received_position_longitude  ,last_known_position                ,vehicle_status ,driver1_status ,vehicle_health_status  ,last_odometer_val ,distance_until_next_service ,last_processed_message_time_stamp ,driver2_id ,driver2_status ,created_at_m2m ,created_at_kafka ,created_at_dm_ ,modified_at, fuel_consumption) VALUES (? ,?         ,?            ,?            ,?            ,?            ,?            ,?            ,?            ,?            ,?                ,?            ,?            ,? ,? ,?   ,?            ,?            ,?            ,?            ,?            ,?            ,?     ,?       )";

	public void insert(Index row, Integer distance_until_next_service) throws TechnicalException, SQLException {
		PreparedStatement stmt_insert_current_trip = null;
		try {

			if (null != row && null != (connection = getConnection())) {
				stmt_insert_current_trip = connection.prepareStatement(INSERT_CURRENT_TRIP);
				stmt_insert_current_trip = fillStatement(stmt_insert_current_trip, row ,distance_until_next_service);
				stmt_insert_current_trip.addBatch();
				System.out.println("before current trip execute batch ");
				stmt_insert_current_trip.executeBatch();
				System.out.println("after executeBatch Current Trip");
			}
		} catch (SQLException e) {
			System.out.println("in catch of LiveFleet Current TRip Statstics");
			e.printStackTrace();
		}
	}

	private PreparedStatement fillStatement(PreparedStatement stmt_insert_current_trip, Index row,Integer distance_until_next_service)
			throws SQLException {

		long start_time_stamp = 0;
		// long end_time_stamp = 0;
		double start_position_lattitude = 0;
		double start_position_longitude = 0;
		
		String varTripID = row.getDocument().getTripID();
		int varVEvtid = row.getVEvtID();

		if (varTripID != null) {
			stmt_insert_current_trip.setString(1, row.getDocument().getTripID()); // 1
																					// tripID
			System.out.println("trip id " + row.getDocument().getTripID());
		}

		else {
			stmt_insert_current_trip.setString(1, row.getRoName());
		}

		if (row.getVin() != null)
			stmt_insert_current_trip.setString(2, (String) row.getVin()); // 2
																			// vin
		else
			stmt_insert_current_trip.setString(2, (String) row.getVid());

		if (varVEvtid == 4)
			stmt_insert_current_trip.setLong(3, row.getReceivedTimestamp()); // 3
																				// trip_start_time_stamp

		else
			stmt_insert_current_trip.setLong(3, start_time_stamp);

		stmt_insert_current_trip.setLong(4, row.getReceivedTimestamp()); // 4
																			// trip_end_time_stamp

		stmt_insert_current_trip.setString(5, (String) row.getDriverID());// 5
																			// DriverID

		if (varVEvtid == 4) {
			stmt_insert_current_trip.setDouble(6, row.getGpsLatitude()); // 6
																			// start_position_lattitude
			System.out.println("start position lattitude " + row.getGpsLatitude());
		} else {
			stmt_insert_current_trip.setDouble(6, start_position_lattitude);
		}

		if (varVEvtid == 4)
			stmt_insert_current_trip.setDouble(7, row.getGpsLongitude()); // 7
																			// start_position_longitude

		else
			stmt_insert_current_trip.setDouble(7, start_position_longitude);

		stmt_insert_current_trip.setString(8, " "); // 8 start_position - blank

		stmt_insert_current_trip.setDouble(9, row.getGpsLatitude()); // 9
																		// last_received_position_lattitude
		stmt_insert_current_trip.setDouble(10, row.getGpsLongitude()); // 10
																		// last_received_position_longitude

		stmt_insert_current_trip.setString(11, " "); // 11 last_known_position -
														// blank

		Integer[] ttvalue = row.getDocument().getTt_ListValue(); // 12// vehicle status

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
		stmt_insert_current_trip.setInt(13, row.getDocument().getDriver1WorkingState()); // 13
																							// driver1
																							// working
																							// state

		stmt_insert_current_trip.setInt(14, 0); // 14 for time being
												// vehicle_health_status - blank
												// (inserting 0)

		Integer[] tacho = row.getDocument().getTotalTachoMileage(); // 15
																	// odometer
																	// value

		if (tacho.length == 0) {
			System.out.println("odometer is empty");
			stmt_insert_current_trip.setInt(15, 0);
		} else {
			int odometer_val = tacho[tacho.length - 1];

			stmt_insert_current_trip.setInt(15, odometer_val);
		}

		stmt_insert_current_trip.setInt(16, distance_until_next_service.intValue()); // distance_until_next_service

		stmt_insert_current_trip.setLong(17, row.getReceivedTimestamp()); // 17
																			// last_processed_message_time

		stmt_insert_current_trip.setString(18, (String) row.getDocument().getDriver2ID()); // 18
																							// Driver2ID

		stmt_insert_current_trip.setInt(19, row.getDocument().getDriver2WorkingState()); // 19
																							// driver2
																							// working
																							// state

		stmt_insert_current_trip.setLong(20, row.getReceivedTimestamp()); // 20
																			// created_at_m2m

		stmt_insert_current_trip.setLong(21, row.getReceivedTimestamp()); // 21
																			// created_at_kafka

		stmt_insert_current_trip.setLong(22, row.getReceivedTimestamp()); // 22
																			// created_at_dm

		stmt_insert_current_trip.setLong(23, row.getReceivedTimestamp()); // 23
																			// modified_at
		
		stmt_insert_current_trip.setLong(24, row.getVUsedFuel()); // 24 vusedFuel

		return stmt_insert_current_trip;

	}

	public CurrentTrip read(String tripId) throws TechnicalException, SQLException {
		PreparedStatement stmt_read_current_trip = null;
		ResultSet rs_trip = null;
		long trip_Start_time = 0;
		long start_time_stamp = 0;

		CurrentTrip currentTripdata = new CurrentTrip();

		System.out.println("Inside read current trip");

		// if (varVEvtid != 4) {---put this check at ur end kalyan

		try {

			if (null != tripId && null != (connection = getConnection())) {
				stmt_read_current_trip = connection.prepareStatement(READ_CURRENT_TRIP);
				stmt_read_current_trip.setString(1, tripId);

				rs_trip = stmt_read_current_trip.executeQuery();

				while (rs_trip.next()) {

					currentTripdata.setStart_time_stamp(rs_trip.getLong("start_time_stamp"));

					currentTripdata.setEnd_time_stamp(rs_trip.getLong("end_time_stamp"));

					currentTripdata.setStart_position_lattitude(rs_trip.getDouble("start_position_lattitude"));

					currentTripdata.setStart_position_longitude(rs_trip.getDouble("start_position_longitude"));

					System.out.println("inside query , result set data" + start_time_stamp);
				}
				rs_trip.close();

			}
		} catch (SQLException e) {
			System.out.println("inside catch read current trip");
			e.printStackTrace();
		} finally {

			if (null != rs_trip) {

				try {
					rs_trip.close();
				} catch (SQLException ignore) {
					/** ignore any errors here */

				}
			}
		}
		System.out.println("trip_Start_time" + trip_Start_time);
		return currentTripdata;
	}

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}

}
