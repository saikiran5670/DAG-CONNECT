package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

import lombok.Data;
import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.postgre.bo.CurrentTrip;
import net.atos.daf.postgre.bo.TripStatisticsPojo;
import net.atos.daf.postgre.util.DafConstants;

public class LivefleetCurrentTripStatisticsDao implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private Connection connection;

	/** SQL statement for insert. */
	private static final String READ_CURRENT_TRIP = "SELECT * FROM livefleet.livefleet_current_trip_statistics WHERE trip_id = ? ORDER BY created_at_m2m ASC limit 1";
	private static final String INSERT_CURRENT_TRIP = "INSERT INTO livefleet.livefleet_current_trip_statistics ( trip_id   , vin        ,start_time_stamp          ,end_time_stamp                ,driver1_id          ,start_position_lattitude              ,start_position_longitude             ,start_position                ,last_received_position_lattitude             , last_received_position_longitude  ,last_known_position                ,vehicle_status ,driver1_status ,vehicle_health_status  ,last_odometer_val ,distance_until_next_service ,last_processed_message_time_stamp ,driver2_id ,driver2_status ,created_at_m2m ,created_at_kafka ,created_at_dm_ ,modified_at, fuel_consumption ) VALUES (? ,?         ,?            ,?            ,?            ,?            ,?            ,?            ,?            ,?            ,?                ,?            ,?            ,? ,? ,?   ,?            ,?            ,?            ,?            ,?            ,?            ,?  ,?  )";

	public void insert(TripStatisticsPojo row, int distance_until_next_service)
			throws TechnicalException, SQLException {
		PreparedStatement stmt_insert_current_trip = null;
		try {

			if (null != row && null != (connection = getConnection())) {
				stmt_insert_current_trip = connection.prepareStatement(INSERT_CURRENT_TRIP,
						ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);
				stmt_insert_current_trip = fillStatement(stmt_insert_current_trip, row);
				stmt_insert_current_trip.addBatch();
				stmt_insert_current_trip.executeBatch();
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}

	private PreparedStatement fillStatement(PreparedStatement stmt_insert_current_trip,
			TripStatisticsPojo tripStatistics) throws SQLException {

		if (tripStatistics.getTripId() != null)
			stmt_insert_current_trip.setString(1, tripStatistics.getTripId());
		else
			stmt_insert_current_trip.setString(1, DafConstants.UNKNOWN);

		if (tripStatistics.getVin() != null)
			stmt_insert_current_trip.setString(2, tripStatistics.getVin());
		else
			stmt_insert_current_trip.setString(2, tripStatistics.getVid());

		if (tripStatistics.getStart_time_stamp() != null)
			stmt_insert_current_trip.setLong(3, tripStatistics.getStart_time_stamp());
		else
			stmt_insert_current_trip.setLong(3, 0);

		if (tripStatistics.getEnd_time_stamp() != null)
			stmt_insert_current_trip.setLong(4, tripStatistics.getEnd_time_stamp());
		else
			stmt_insert_current_trip.setLong(4, 0);

		if (tripStatistics.getDriver1ID() != null)
			stmt_insert_current_trip.setString(5, tripStatistics.getDriver1ID());
		else
			stmt_insert_current_trip.setString(5, DafConstants.UNKNOWN);

		if (tripStatistics.getStart_position_lattitude() != null)
			stmt_insert_current_trip.setDouble(6, tripStatistics.getStart_position_lattitude());
		else
			stmt_insert_current_trip.setDouble(6, 0);

		if (tripStatistics.getStart_position_longitude() != null)
			stmt_insert_current_trip.setDouble(7, tripStatistics.getStart_position_longitude());
		else
			stmt_insert_current_trip.setDouble(7, 0);

		if (tripStatistics.getStart_position() != null)
			stmt_insert_current_trip.setString(8, tripStatistics.getStart_position());
		else
			stmt_insert_current_trip.setString(8, DafConstants.UNKNOWN);

		if (tripStatistics.getLast_recieved_position_lattitude() != null)
			stmt_insert_current_trip.setDouble(9, tripStatistics.getLast_recieved_position_lattitude());
		else
			stmt_insert_current_trip.setDouble(9, 0);

		if (tripStatistics.getLast_recieved_position_longitude() != null)
			stmt_insert_current_trip.setDouble(10, tripStatistics.getLast_recieved_position_longitude());
		else
			stmt_insert_current_trip.setDouble(10, 0);

		if (tripStatistics.getLast_known_position() != null)
			stmt_insert_current_trip.setString(11, tripStatistics.getLast_known_position());
		else
			stmt_insert_current_trip.setString(11, DafConstants.UNKNOWN);

		if (tripStatistics.getVehicle_status() != null)
			stmt_insert_current_trip.setInt(12, tripStatistics.getVehicle_status());
		else
			stmt_insert_current_trip.setInt(12, 0);

		if (tripStatistics.getDriver1_status() != null)
			stmt_insert_current_trip.setInt(13, tripStatistics.getDriver1_status());
		else
			stmt_insert_current_trip.setInt(13, 0);

		if (tripStatistics.getVehicle_health_status() != null)
			stmt_insert_current_trip.setInt(14, tripStatistics.getVehicle_health_status());
		else
			stmt_insert_current_trip.setInt(14, 0);

		if (tripStatistics.getLast_odometer_val() != null)
			stmt_insert_current_trip.setInt(15, tripStatistics.getLast_odometer_val());
		else
			stmt_insert_current_trip.setInt(15, 0);

		if (tripStatistics.getDistance_until_next_service() != null)
			stmt_insert_current_trip.setInt(16, tripStatistics.getDistance_until_next_service());
		else
			stmt_insert_current_trip.setInt(16, 0);

		if (tripStatistics.getLast_processed_message_timestamp() != null)
			stmt_insert_current_trip.setLong(17, tripStatistics.getLast_processed_message_timestamp());
		else
			stmt_insert_current_trip.setLong(17, 0);

		if (tripStatistics.getDriver2ID() != null)
			stmt_insert_current_trip.setString(18, tripStatistics.getDriver2ID());
		else
			stmt_insert_current_trip.setString(18, DafConstants.UNKNOWN);

		if (tripStatistics.getDriver2_status() != null)
			stmt_insert_current_trip.setInt(19, tripStatistics.getDriver2_status());
		else
			stmt_insert_current_trip.setInt(19, 0);

		if (tripStatistics.getCreated_at_m2m() != null)
			stmt_insert_current_trip.setLong(20, tripStatistics.getCreated_at_m2m());
		else
			stmt_insert_current_trip.setLong(20, 0);

		if (tripStatistics.getCreated_at_kafka() != null)
			stmt_insert_current_trip.setLong(21, tripStatistics.getCreated_at_kafka());
		else
			stmt_insert_current_trip.setLong(21, 0);

		if (tripStatistics.getCreated_at_dm() != null)
			stmt_insert_current_trip.setLong(22, tripStatistics.getCreated_at_dm());
		else
			stmt_insert_current_trip.setLong(22, 0);

		if (tripStatistics.getModified_at() != null)
			stmt_insert_current_trip.setLong(23, tripStatistics.getModified_at());
		else
			stmt_insert_current_trip.setLong(23, 0);

		if (tripStatistics.getFuel_consumption() != null)
			stmt_insert_current_trip.setInt(24, tripStatistics.getFuel_consumption());
		else
			stmt_insert_current_trip.setInt(24, 0);

		return stmt_insert_current_trip;
	}

	/*
	 * private PreparedStatement fillStatement(PreparedStatement
	 * stmt_insert_current_trip, Index row) throws SQLException {
	 * 
	 * long start_time_stamp = 0; // long end_time_stamp = 0; double
	 * start_position_lattitude = 0; double start_position_longitude = 0; int
	 * distance_until_next_service = 0;
	 * System.out.println("Inside current trip fillStatement"); String varTripID
	 * = row.getDocument().getTripID(); int varVEvtid = row.getVEvtID();
	 * 
	 * if (varTripID != null) { stmt_insert_current_trip.setString(1,
	 * row.getDocument().getTripID()); // 1 // tripID
	 * System.out.println(" Current trip id " + row.getDocument().getTripID());
	 * }
	 * 
	 * else { stmt_insert_current_trip.setString(1, row.getRoName()); }
	 * 
	 * if (row.getVin() != null) stmt_insert_current_trip.setString(2, (String)
	 * row.getVin()); // 2 // vin else stmt_insert_current_trip.setString(2,
	 * (String) row.getVid());
	 * 
	 * if (varVEvtid == 4) stmt_insert_current_trip.setLong(3,
	 * row.getReceivedTimestamp()); // 3 // trip_start_time_stamp
	 * 
	 * else stmt_insert_current_trip.setLong(3, start_time_stamp);
	 * 
	 * stmt_insert_current_trip.setLong(4, row.getReceivedTimestamp()); // 4 //
	 * trip_end_time_stamp
	 * 
	 * stmt_insert_current_trip.setString(5, (String) row.getDriverID());// 5 //
	 * DriverID
	 * 
	 * if (varVEvtid == 4) { stmt_insert_current_trip.setDouble(6,
	 * row.getGpsLatitude()); // 6 // start_position_lattitude
	 * System.out.println("start position lattitude " + row.getGpsLatitude()); }
	 * else { stmt_insert_current_trip.setDouble(6, start_position_lattitude); }
	 * 
	 * if (varVEvtid == 4) stmt_insert_current_trip.setDouble(7,
	 * row.getGpsLongitude()); // 7 // start_position_longitude
	 * 
	 * else stmt_insert_current_trip.setDouble(7, start_position_longitude);
	 * 
	 * stmt_insert_current_trip.setString(8, " "); // 8 start_position - blank
	 * 
	 * stmt_insert_current_trip.setDouble(9, row.getGpsLatitude()); // 9 //
	 * last_received_position_lattitude stmt_insert_current_trip.setDouble(10,
	 * row.getGpsLongitude()); // 10 // last_received_position_longitude
	 * 
	 * stmt_insert_current_trip.setString(11, " "); // 11 last_known_position -
	 * // blank
	 * 
	 * Integer[] ttvalue = row.getDocument().getTt_ListValue(); // 12// vehicle
	 * status
	 * 
	 * if (ttvalue.length == 0) { System.out.println("ttvalue is empty");
	 * stmt_insert_current_trip.setInt(12, 0);
	 * 
	 * } else { int status = ttvalue[ttvalue.length - 1];
	 * 
	 * if (status == 0) { stmt_insert_current_trip.setInt(12, 2); }
	 * 
	 * if (status == 1 || status == 2 || status == 3) {
	 * stmt_insert_current_trip.setInt(12, 1); }
	 * 
	 * if (status == 4 || status == 5 || status == 6) {
	 * stmt_insert_current_trip.setInt(12, 3); }
	 * 
	 * if (status == 7) { stmt_insert_current_trip.setInt(12, 4); } }
	 * stmt_insert_current_trip.setInt(13,
	 * row.getDocument().getDriver1WorkingState()); // 13 // driver1 // working
	 * // state
	 * 
	 * stmt_insert_current_trip.setInt(14, 0); // 14 for time being //
	 * vehicle_health_status - blank // (inserting 0)
	 * 
	 * Integer[] tacho = row.getDocument().getTotalTachoMileage(); // 15 //
	 * odometer // value
	 * 
	 * if (tacho.length == 0) { System.out.println("odometer is empty");
	 * stmt_insert_current_trip.setInt(15, 0); } else { int odometer_val =
	 * tacho[tacho.length - 1];
	 * 
	 * stmt_insert_current_trip.setInt(15, odometer_val); }
	 * 
	 * stmt_insert_current_trip.setInt(16, distance_until_next_service); //
	 * distance_until_next_service
	 * 
	 * stmt_insert_current_trip.setLong(17, row.getReceivedTimestamp()); // 17
	 * // last_processed_message_time
	 * 
	 * stmt_insert_current_trip.setString(18, (String)
	 * row.getDocument().getDriver2ID()); // 18 // Driver2ID
	 * 
	 * stmt_insert_current_trip.setInt(19,
	 * row.getDocument().getDriver2WorkingState()); // 19 // driver2 // working
	 * // state
	 * 
	 * stmt_insert_current_trip.setLong(20, row.getReceivedTimestamp()); // 20
	 * // created_at_m2m
	 * 
	 * stmt_insert_current_trip.setLong(21, row.getReceivedTimestamp()); // 21
	 * // created_at_kafka
	 * 
	 * stmt_insert_current_trip.setLong(22, row.getReceivedTimestamp()); // 22
	 * // created_at_dm
	 * 
	 * stmt_insert_current_trip.setLong(23, row.getReceivedTimestamp()); // 23
	 * // modified_at stmt_insert_current_trip.setLong(24, row.getVUsedFuel());
	 * // 24 Fuel_consumption //stmt_insert_current_trip.setLong(24, 100);
	 * 
	 * return stmt_insert_current_trip;
	 * 
	 * }
	 */

	public CurrentTrip read(String tripId, int num) throws TechnicalException, SQLException {

		PreparedStatement stmt_read_current_trip = null;
		ResultSet rs_trip = null;
	
		CurrentTrip currentTripdata = new CurrentTrip();

		try {

			if (null != tripId && null != (connection = getConnection())) {

				stmt_read_current_trip = connection.prepareStatement(READ_CURRENT_TRIP, ResultSet.TYPE_SCROLL_SENSITIVE,
						ResultSet.CONCUR_UPDATABLE);
				stmt_read_current_trip.setString(1, tripId);

				rs_trip = stmt_read_current_trip.executeQuery();

				if (num == 1) {

					while (rs_trip.first()) {

						currentTripdata.setStart_time_stamp(rs_trip.getLong("start_time_stamp"));

						currentTripdata.setEnd_time_stamp(rs_trip.getLong("end_time_stamp"));

						currentTripdata.setStart_position_lattitude(rs_trip.getDouble("start_position_lattitude"));

						currentTripdata.setStart_position_longitude(rs_trip.getDouble("start_position_longitude"));

					}

				} else if (num == 2) {
					while (rs_trip.last()) {
						currentTripdata.setFuel_consumption(rs_trip.getLong("fuel_consumption"));
					}
				}
				rs_trip.close();

			}
		} catch (SQLException e) {

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

		return currentTripdata;
	}

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}

}
