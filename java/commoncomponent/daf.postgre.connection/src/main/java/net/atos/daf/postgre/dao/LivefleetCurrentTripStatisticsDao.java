package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.Arrays;
import java.util.Objects;

import org.apache.flink.shaded.curator4.org.apache.curator.shaded.com.google.common.base.Strings;

import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.postgre.bo.CurrentTrip;
import net.atos.daf.postgre.bo.TripStatisticsPojo;

public class LivefleetCurrentTripStatisticsDao implements Serializable {

	private static final long serialVersionUID = 1L;

	private Connection connection;

	private static final String READ_CURRENT_TRIP = "SELECT * FROM livefleet.livefleet_current_trip_statistics WHERE trip_id = ? AND trip_id IS NOT NULL ORDER BY id DESC limit 1";
	private static final String INSERT_CURRENT_TRIP = "INSERT INTO livefleet.livefleet_current_trip_statistics ( trip_id , vin , start_time_stamp , end_time_stamp , "
			+ "driver1_id , trip_distance , driving_time , fuel_consumption , vehicle_driving_status_type , odometer_val , distance_until_next_service , latest_received_position_lattitude , "
			+ "latest_received_position_longitude , latest_received_position_heading , latest_geolocation_address_id , start_position_lattitude , start_position_longitude , start_position_heading , "
			+ "start_geolocation_address_id , latest_processed_message_time_stamp , vehicle_health_status_type , latest_warning_class , latest_warning_number , latest_warning_type , latest_warning_timestamp , "
			+ "latest_warning_position_latitude , latest_warning_position_longitude , latest_warning_geolocation_address_id , created_at , modified_at ) "
			+ "VALUES (? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ?  )";
	private static final String UPDATE_CURRENT_TRIP = "UPDATE livefleet.livefleet_current_trip_statistics  SET end_time_stamp = ? , driver1_id = ? , trip_distance = ? , "
			+ "driving_time = ? , fuel_consumption = ? , vehicle_driving_status_type = ? , odometer_val = ? , distance_until_next_service = ? , latest_received_position_lattitude = ? , "
			+ "latest_received_position_longitude = ? , latest_received_position_heading = ? , latest_geolocation_address_id = ? , latest_processed_message_time_stamp = ? , "
			+ "modified_at = ? WHERE trip_id = ? ";

	
	public void insert(TripStatisticsPojo row) throws TechnicalException, SQLException {
		PreparedStatement trip_stats_insert_stmt = null;
		try {

			if (null != row && null != (connection = getConnection())) {
				trip_stats_insert_stmt = connection.prepareStatement(INSERT_CURRENT_TRIP,
						ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);
				trip_stats_insert_stmt = fillStatement(trip_stats_insert_stmt, row);

				System.out.println("INSERT_CURRENT_TRIP query : " + trip_stats_insert_stmt);

				trip_stats_insert_stmt.addBatch();
				int[] res = trip_stats_insert_stmt.executeBatch();

				System.out.println("NUMBER OF ROWS inserted by INSERT_CURRENT_TRIP query is " + Arrays.toString(res));

			}
		} catch (SQLException e) {
			System.out.println("Issue in LivefleetCurrentTripStatisticsDao  " + trip_stats_insert_stmt);
			e.printStackTrace();
		} catch (Exception e) {
			System.out.println("Issue in LivefleetCurrentTripStatisticsDao insertion   " + trip_stats_insert_stmt);
		}
	}

	private PreparedStatement fillUpdateStatement(PreparedStatement stmt_update_existing_trip,
			TripStatisticsPojo tripStatistics) throws SQLException {

		if (tripStatistics.getEnd_time_stamp() != null)
			stmt_update_existing_trip.setLong(1, tripStatistics.getEnd_time_stamp());
		else
			stmt_update_existing_trip.setLong(1, 0);

		if (tripStatistics.getDriver1ID() != null)
			stmt_update_existing_trip.setString(2, tripStatistics.getDriver1ID());
		else
			stmt_update_existing_trip.setString(2, "");

		if (tripStatistics.getTrip_distance() != null)
			stmt_update_existing_trip.setLong(3, tripStatistics.getTrip_distance());
		else
			stmt_update_existing_trip.setLong(3, 0);

		if (tripStatistics.getDriving_time() != null)
			stmt_update_existing_trip.setLong(4, tripStatistics.getDriving_time());
		else
			stmt_update_existing_trip.setLong(4, 0);

		if (tripStatistics.getFuel_consumption() != null)
			stmt_update_existing_trip.setLong(5, tripStatistics.getFuel_consumption());
		else
			stmt_update_existing_trip.setLong(5, 0);

		if (tripStatistics.getVehicle_driving_status_type() != null)
			stmt_update_existing_trip.setString(6, String.valueOf(tripStatistics.getVehicle_driving_status_type()));
		else 
			stmt_update_existing_trip.setString(6, String.valueOf(Character.valueOf(' ')));
		 

		if (tripStatistics.getOdometer_val() != null)
			stmt_update_existing_trip.setLong(7, tripStatistics.getOdometer_val());
		else
			stmt_update_existing_trip.setLong(7, 0);

		if (tripStatistics.getDistance_until_next_service() != null)
			stmt_update_existing_trip.setLong(8, tripStatistics.getDistance_until_next_service());
		else
			stmt_update_existing_trip.setLong(8, 0);

		if (tripStatistics.getLast_received_position_lattitude() != null)
			stmt_update_existing_trip.setDouble(9, tripStatistics.getLast_received_position_lattitude());
		else
			stmt_update_existing_trip.setDouble(9, 255);

		if (tripStatistics.getLast_received_position_longitude() != null)
			stmt_update_existing_trip.setDouble(10, tripStatistics.getLast_received_position_longitude());
		else
			stmt_update_existing_trip.setDouble(10, 255);

		if (tripStatistics.getLast_received_position_heading() != null)
			stmt_update_existing_trip.setDouble(11, tripStatistics.getLast_received_position_heading());
		else
			stmt_update_existing_trip.setDouble(11, 0);

		if (tripStatistics.getLast_geolocation_address_id() != null)
			stmt_update_existing_trip.setLong(12, tripStatistics.getLast_geolocation_address_id());
		else
			stmt_update_existing_trip.setLong(12, 0);

		if (tripStatistics.getLast_processed_message_timestamp() != null)
			stmt_update_existing_trip.setLong(13, tripStatistics.getLast_processed_message_timestamp());
		else
			stmt_update_existing_trip.setLong(13, 0);

		/*
		 * if (tripStatistics.getVehicle_health_status_type() != null)
		 * stmt_update_existing_trip.setString(14,
		 * String.valueOf(tripStatistics.getVehicle_health_status_type())); else
		 * stmt_update_existing_trip.setString(14, String.valueOf(Character.valueOf('
		 * ')));
		 * 
		 * 
		 * if (tripStatistics.getLatest_warning_class() != null)
		 * stmt_update_existing_trip.setLong(15,
		 * tripStatistics.getLatest_warning_class()); else
		 * stmt_update_existing_trip.setLong(15, 0);
		 * 
		 * 
		 * if (tripStatistics.getLatest_warning_number() != null)
		 * stmt_update_existing_trip.setLong(16,
		 * tripStatistics.getLatest_warning_number()); else
		 * stmt_update_existing_trip.setLong(16, 0);
		 * 
		 * 
		 * if (tripStatistics.getLatest_warning_type() != null)
		 * stmt_update_existing_trip.setString(17,
		 * String.valueOf(tripStatistics.getLatest_warning_type())); else
		 * stmt_update_existing_trip.setString(17, String.valueOf(Character.valueOf('
		 * ')));
		 * 
		 * 
		 * if (tripStatistics.getLatest_warning_timestamp() != null)
		 * stmt_update_existing_trip.setLong(18,
		 * tripStatistics.getLatest_warning_timestamp()); else
		 * stmt_update_existing_trip.setLong(18, 0);
		 * 
		 * 
		 * if (tripStatistics.getLatest_warning_position_latitude() != null)
		 * stmt_update_existing_trip.setDouble(19,
		 * tripStatistics.getLatest_warning_position_latitude()); else
		 * stmt_update_existing_trip.setDouble(19, 0);
		 * 
		 * 
		 * if (tripStatistics.getLatest_warning_position_longitude() != null)
		 * stmt_update_existing_trip.setDouble(20,
		 * tripStatistics.getLatest_warning_position_longitude()); else
		 * stmt_update_existing_trip.setDouble(20, 0);
		 * 
		 * 
		 * if (tripStatistics.getLatest_warning_geolocation_address_id() != null)
		 * stmt_update_existing_trip.setLong(21,
		 * tripStatistics.getLatest_warning_geolocation_address_id()); else
		 * stmt_update_existing_trip.setLong(21, 0);
		 */

		if (tripStatistics.getModified_at() != null)
			stmt_update_existing_trip.setLong(14, tripStatistics.getModified_at());
		else
			stmt_update_existing_trip.setLong(14, 0);

		return stmt_update_existing_trip;
	}

	private PreparedStatement fillStatement(PreparedStatement stmt_insert_current_trip,
			TripStatisticsPojo tripStatistics) throws SQLException {

		if (tripStatistics.getTripId() != null)
			stmt_insert_current_trip.setString(1, tripStatistics.getTripId());
		else
			stmt_insert_current_trip.setString(1, "");

		if (tripStatistics.getVin() != null)
			stmt_insert_current_trip.setString(2, tripStatistics.getVin());
		else
			stmt_insert_current_trip.setString(2, "");

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
			stmt_insert_current_trip.setString(5, "");

		if (tripStatistics.getTrip_distance() != null)
			stmt_insert_current_trip.setLong(6, tripStatistics.getTrip_distance());
		else
			stmt_insert_current_trip.setLong(6, 0);

		if (tripStatistics.getDriving_time() != null)
			stmt_insert_current_trip.setLong(7, tripStatistics.getDriving_time());
		else
			stmt_insert_current_trip.setLong(7, 0);

		if (tripStatistics.getFuel_consumption() != null)
			stmt_insert_current_trip.setLong(8, tripStatistics.getFuel_consumption());
		else
			stmt_insert_current_trip.setLong(8, 0);

		if (tripStatistics.getVehicle_driving_status_type() != null)
			stmt_insert_current_trip.setString(9, String.valueOf(tripStatistics.getVehicle_driving_status_type()));
		else
			stmt_insert_current_trip.setString(9, String.valueOf(Character.valueOf(' ')));

		if (tripStatistics.getOdometer_val() != null)
			stmt_insert_current_trip.setLong(10, tripStatistics.getOdometer_val());
		else
			stmt_insert_current_trip.setLong(10, 0);

		if (tripStatistics.getDistance_until_next_service() != null)
			stmt_insert_current_trip.setLong(11, tripStatistics.getDistance_until_next_service());
		else
			stmt_insert_current_trip.setLong(11, 0);

		if (tripStatistics.getLast_received_position_lattitude() != null)
			stmt_insert_current_trip.setDouble(12, tripStatistics.getLast_received_position_lattitude());
		else
			stmt_insert_current_trip.setDouble(12, 255);

		if (tripStatistics.getLast_received_position_longitude() != null)
			stmt_insert_current_trip.setDouble(13, tripStatistics.getLast_received_position_longitude());
		else
			stmt_insert_current_trip.setDouble(13, 255);

		if (tripStatistics.getLast_received_position_heading() != null)
			stmt_insert_current_trip.setDouble(14, tripStatistics.getLast_received_position_heading());
		else
			stmt_insert_current_trip.setDouble(14, 0);

		if (tripStatistics.getLast_geolocation_address_id() != null)
			stmt_insert_current_trip.setLong(15, tripStatistics.getLast_geolocation_address_id());
		else
			stmt_insert_current_trip.setLong(15, 0);

		if (tripStatistics.getStart_position_lattitude() != null)
			stmt_insert_current_trip.setDouble(16, tripStatistics.getStart_position_lattitude());
		else
			stmt_insert_current_trip.setDouble(16, 255);

		if (tripStatistics.getStart_position_longitude() != null)
			stmt_insert_current_trip.setDouble(17, tripStatistics.getStart_position_longitude());
		else
			stmt_insert_current_trip.setDouble(17, 255);

		if (tripStatistics.getStart_position_heading() != null)
			stmt_insert_current_trip.setDouble(18, tripStatistics.getStart_position_heading());
		else
			stmt_insert_current_trip.setDouble(18, 0);

		if (tripStatistics.getStart_geolocation_address_id() != null)
			stmt_insert_current_trip.setLong(19, tripStatistics.getStart_geolocation_address_id());
		else
			stmt_insert_current_trip.setLong(19, 0);

		if (tripStatistics.getLast_processed_message_timestamp() != null)
			stmt_insert_current_trip.setLong(20, tripStatistics.getLast_processed_message_timestamp());
		else
			stmt_insert_current_trip.setLong(20, 0);

		if (tripStatistics.getVehicle_health_status_type() != null)
			stmt_insert_current_trip.setString(21, String.valueOf(tripStatistics.getVehicle_health_status_type()));
		else
			stmt_insert_current_trip.setString(21, String.valueOf(Character.valueOf('N')));

		if (tripStatistics.getLatest_warning_class() != null)
			stmt_insert_current_trip.setLong(22, tripStatistics.getLatest_warning_class());
		else
			stmt_insert_current_trip.setLong(22, 0);

		if (tripStatistics.getLatest_warning_number() != null)
			stmt_insert_current_trip.setLong(23, tripStatistics.getLatest_warning_number());
		else
			stmt_insert_current_trip.setLong(23, 0);

		if (tripStatistics.getLatest_warning_type() != null)
			stmt_insert_current_trip.setString(24, String.valueOf(tripStatistics.getLatest_warning_type()));
		else
			stmt_insert_current_trip.setString(24, String.valueOf(Character.valueOf(' ')));

		if (tripStatistics.getLatest_warning_timestamp() != null)
			stmt_insert_current_trip.setLong(25, tripStatistics.getLatest_warning_timestamp());
		else
			stmt_insert_current_trip.setLong(25, 0);

		if (tripStatistics.getLatest_warning_position_latitude() != null)
			stmt_insert_current_trip.setDouble(26, tripStatistics.getLatest_warning_position_latitude());
		else
			stmt_insert_current_trip.setDouble(26, 0);

		if (tripStatistics.getLatest_warning_position_longitude() != null)
			stmt_insert_current_trip.setDouble(27, tripStatistics.getLatest_warning_position_longitude());
		else
			stmt_insert_current_trip.setDouble(27, 0);

		if (tripStatistics.getLatest_warning_geolocation_address_id() != null)
			stmt_insert_current_trip.setLong(28, tripStatistics.getLatest_warning_geolocation_address_id());
		else
			stmt_insert_current_trip.setLong(28, 0);

		if (tripStatistics.getCreated_at() != null)
			stmt_insert_current_trip.setLong(29, tripStatistics.getCreated_at());
		else
			stmt_insert_current_trip.setLong(29, 0);

		if (tripStatistics.getModified_at() != null)
			stmt_insert_current_trip.setLong(30, tripStatistics.getModified_at());
		else
			stmt_insert_current_trip.setLong(30, 0);

		return stmt_insert_current_trip;
	}

	public CurrentTrip read(String tripId) throws TechnicalException, SQLException {

		PreparedStatement stmt_read_current_trip = null;
		ResultSet rs_trip = null;

		CurrentTrip currentTripdata = null;

		try {

			if (null != tripId && null != (connection = getConnection())) {

				stmt_read_current_trip = connection.prepareStatement(READ_CURRENT_TRIP, ResultSet.TYPE_SCROLL_SENSITIVE,
						ResultSet.CONCUR_UPDATABLE);
				stmt_read_current_trip.setString(1, tripId);

				System.out.println("READ_CURRENT_TRIP query : " + stmt_read_current_trip);

				rs_trip = stmt_read_current_trip.executeQuery();

				System.out.println("rs_trip " + rs_trip);

				while (rs_trip.next()) {

					currentTripdata = new CurrentTrip();
					currentTripdata.setTrip_id(tripId);

					currentTripdata.setStart_time_stamp(rs_trip.getLong("start_time_stamp"));
					currentTripdata.setEnd_time_stamp(rs_trip.getLong("end_time_stamp"));
					currentTripdata.setStart_position_lattitude(rs_trip.getDouble("start_position_lattitude"));
					currentTripdata.setStart_position_longitude(rs_trip.getDouble("start_position_longitude"));
					currentTripdata.setStart_position_heading(rs_trip.getDouble("start_position_heading"));
					currentTripdata.setStart_geolocation_address_id(rs_trip.getInt("start_geolocation_address_id"));
					currentTripdata.setLast_position_lattitude(rs_trip.getDouble("latest_received_position_lattitude"));
					currentTripdata.setLast_position_longitude(rs_trip.getDouble("latest_received_position_longitude"));
					currentTripdata.setLast_position_heading(rs_trip.getDouble("latest_received_position_heading"));
					currentTripdata.setLast_geolocation_address_id(rs_trip.getInt("latest_geolocation_address_id"));
					currentTripdata.setDriving_time(rs_trip.getLong("driving_time"));
					currentTripdata.setTrip_distance(rs_trip.getLong("trip_distance"));
					currentTripdata.setFuel_consumption(rs_trip.getLong("fuel_consumption"));
					currentTripdata.setOdometer_val(rs_trip.getLong("odometer_val"));
				}
				// }
				
				if(!Objects.isNull(currentTripdata))
					System.out.println("RESULTSET RECEIVED from READ_CURRENT_TRIP for tripId = " + tripId + " is "
						+ currentTripdata.toString());
				else 
					System.out.println("RESULTSET RECEIVED from READ_CURRENT_TRIP for tripId = " + tripId + " is NULL");

				rs_trip.close();

			}

		} catch (SQLException e) {
			System.out.println("issue in sql read" + stmt_read_current_trip);
			e.printStackTrace();

		} catch (Exception e) {
			System.out.println("issue in read" + stmt_read_current_trip);
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

	public void update(TripStatisticsPojo row) throws TechnicalException, SQLException {

		PreparedStatement trip_stats_update_stmt = null;

		try {

			if (null != row && null != (connection = getConnection())) {

				trip_stats_update_stmt = connection.prepareStatement(UPDATE_CURRENT_TRIP,
						ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);
				trip_stats_update_stmt = fillUpdateStatement(trip_stats_update_stmt, row);
				trip_stats_update_stmt.setString(15, row.getTripId());

				System.out.println("UPDATE_CURRENT_TRIP query : " + trip_stats_update_stmt);

				int num = trip_stats_update_stmt.executeUpdate();

				System.out.println("NUMBER OF ROWS updated by UPDATE_CURRENT_TRIP is " + num);

			}
		} catch (SQLException e) {
			System.out.println("Issue in sql update trip statistics::" + trip_stats_update_stmt);
			e.printStackTrace();
		} catch (Exception e) {

			System.out.println("Issue in update trip statistics::" + trip_stats_update_stmt);
			e.printStackTrace();
		} finally {

			if (trip_stats_update_stmt != null)
				try {
					trip_stats_update_stmt.close();
				} catch (SQLException ignore) {
					/** ignore any errors here */
				}

		}

	}

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}


}
