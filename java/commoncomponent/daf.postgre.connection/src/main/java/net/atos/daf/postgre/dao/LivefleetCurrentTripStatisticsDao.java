package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.Arrays;
import java.util.Objects;

import org.apache.flink.shaded.curator4.org.apache.curator.shaded.com.google.common.base.Strings;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.esotericsoftware.minlog.Log;

import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.postgre.bo.CurrentTrip;
import net.atos.daf.postgre.bo.TripStatisticsPojo;
import net.atos.daf.postgre.bo.WarningStatisticsPojo;

public class LivefleetCurrentTripStatisticsDao implements Serializable {

	private static final long serialVersionUID = 1L;
	private static Logger logger = LoggerFactory.getLogger(LivefleetCurrentTripStatisticsDao.class);

	private Connection connection;

	private static final String READ_CURRENT_TRIP = "SELECT * FROM livefleet.livefleet_current_trip_statistics WHERE trip_id = ? AND trip_id IS NOT NULL ORDER BY id DESC limit 1";
	private static final String INSERT_CURRENT_TRIP = "INSERT INTO livefleet.livefleet_current_trip_statistics ( trip_id , vin , start_time_stamp , end_time_stamp , "
			+ "driver1_id , trip_distance , driving_time , fuel_consumption , vehicle_driving_status_type , odometer_val , distance_until_next_service , latest_received_position_lattitude , "
			+ "latest_received_position_longitude , latest_received_position_heading , latest_geolocation_address_id , start_position_lattitude , start_position_longitude , start_position_heading , "
			+ "start_geolocation_address_id , latest_processed_message_time_stamp , vehicle_health_status_type , latest_warning_class , latest_warning_number , latest_warning_type , latest_warning_timestamp , "
			+ "latest_warning_position_latitude , latest_warning_position_longitude , latest_warning_geolocation_address_id , created_at , modified_at ) "
			+ "VALUES (? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ?  )";
	/*private static final String UPDATE_CURRENT_TRIP = "UPDATE livefleet.livefleet_current_trip_statistics  SET end_time_stamp = ? , driver1_id = ? , trip_distance = ? , "
			+ "driving_time = ? , fuel_consumption = ? , vehicle_driving_status_type = ? , odometer_val = ? , distance_until_next_service = ? , latest_received_position_lattitude = ? , "
			+ "latest_received_position_longitude = ? , latest_received_position_heading = ? , latest_geolocation_address_id = ? , latest_processed_message_time_stamp = ? , "
			+ "modified_at = ? WHERE trip_id = ? "; */
	
	
	private static final String UPDATE_CURRENT_TRIP = "UPDATE livefleet.livefleet_current_trip_statistics  SET end_time_stamp = ? , trip_distance = ? , "
			+ "driving_time = ? , fuel_consumption = ? , vehicle_driving_status_type = ? , odometer_val = ? , latest_received_position_lattitude = ? , "
			+ "latest_received_position_longitude = ? , latest_received_position_heading = ? , latest_processed_message_time_stamp = ? , "
			+ "modified_at = ? WHERE trip_id = ? ";
	
	public static final String	READ_LATEST_WARNING_STATUS_AT_TRIP_START = "select vehicle_health_status_type, warning_class, warning_number, warning_type, warning_time_stamp"
			 + ", latitude, longitude, message_type, trip_id, vin from livefleet.livefleet_warning_statistics where vin = ? ORDER BY id DESC limit 1 ";
	
	/*
	 * public static final String READ_LATEST_WARNING_STATUS_AT_TRIP_START =
	 * "select distinct on (vin) vehicle_health_status_type, warning_class, warning_number, warning_type, warning_time_stamp"
	 * +
	 * ", latitude, longitude, message_type, trip_id, vin from livefleet.livefleet_warning_statistics where vin = ? order by vin, warning_time_stamp desc "
	 * ;
	 */
	
	public void insert(TripStatisticsPojo row) throws TechnicalException, SQLException {
		PreparedStatement trip_stats_insert_stmt = null;
		try {

			if (null != row && null != (connection = getConnection())) {
				trip_stats_insert_stmt = connection.prepareStatement(INSERT_CURRENT_TRIP,
						ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);
				trip_stats_insert_stmt = fillStatement(trip_stats_insert_stmt, row);

				//System.out.println("INSERT_CURRENT_TRIP query : " + trip_stats_insert_stmt);

				trip_stats_insert_stmt.addBatch();
				int[] res = trip_stats_insert_stmt.executeBatch();

				//System.out.println("NUMBER OF ROWS inserted by INSERT_CURRENT_TRIP query is " + Arrays.toString(res));
				logger.info("data inserted for trip  ::" + row.getTripId());
				//System.out.println("data inserted for trip  ::" + row.getTripId());

			}
		} catch (SQLException e) {
			logger.error("sql exception, inside catch LiveFleetCurrentTripStatisticsDao insert" + e.getMessage());
			//System.out.println("Issue in LivefleetCurrentTripStatisticsDao  " + trip_stats_insert_stmt);
			e.printStackTrace();
		} catch (Exception e) {
			logger.error(" Error inside catch LiveFleetCurrentTripStatisticsDao update" + e.getMessage());
			e.printStackTrace();
			//System.out.println("Issue in LivefleetCurrentTripStatisticsDao insertion   " + trip_stats_insert_stmt);
		}
	}

	private PreparedStatement fillUpdateStatement(PreparedStatement stmt_update_existing_trip,
			TripStatisticsPojo tripStatistics) throws SQLException {

		if (tripStatistics.getEnd_time_stamp() != null)
			stmt_update_existing_trip.setLong(1, tripStatistics.getEnd_time_stamp());
		else
			stmt_update_existing_trip.setLong(1, 0);

		/*
		 * if (tripStatistics.getDriver1ID() != null)
		 * stmt_update_existing_trip.setString(2, tripStatistics.getDriver1ID()); else
		 * stmt_update_existing_trip.setString(2, "");
		 */

		if (tripStatistics.getTrip_distance() != null)
			stmt_update_existing_trip.setLong(2, tripStatistics.getTrip_distance());
		else
			stmt_update_existing_trip.setLong(2, 0);

		if (tripStatistics.getDriving_time() != null)
			stmt_update_existing_trip.setLong(3, tripStatistics.getDriving_time());
		else
			stmt_update_existing_trip.setLong(3, 0);

		if (tripStatistics.getFuel_consumption() != null)
			stmt_update_existing_trip.setLong(4, tripStatistics.getFuel_consumption());
		else
			stmt_update_existing_trip.setLong(4, 0);

		if (tripStatistics.getVehicle_driving_status_type() != null)
			stmt_update_existing_trip.setString(5, String.valueOf(tripStatistics.getVehicle_driving_status_type()));
		else 
			stmt_update_existing_trip.setString(5, String.valueOf(Character.valueOf(' ')));
		 

		if (tripStatistics.getOdometer_val() != null)
			stmt_update_existing_trip.setLong(6, tripStatistics.getOdometer_val());
		else
			stmt_update_existing_trip.setLong(6, 0);

		/*
		 * if (tripStatistics.getDistance_until_next_service() != null)
		 * stmt_update_existing_trip.setLong(8,
		 * tripStatistics.getDistance_until_next_service()); else
		 * stmt_update_existing_trip.setLong(8, 0);
		 */

		if (tripStatistics.getLast_received_position_lattitude() != null)
			stmt_update_existing_trip.setDouble(7, tripStatistics.getLast_received_position_lattitude());
		else
			stmt_update_existing_trip.setDouble(7, 255);

		if (tripStatistics.getLast_received_position_longitude() != null)
			stmt_update_existing_trip.setDouble(8, tripStatistics.getLast_received_position_longitude());
		else
			stmt_update_existing_trip.setDouble(8, 255);

		if (tripStatistics.getLast_received_position_heading() != null)
			stmt_update_existing_trip.setDouble(9, tripStatistics.getLast_received_position_heading());
		else
			stmt_update_existing_trip.setDouble(9, 0);

		/*
		 * if (tripStatistics.getLast_geolocation_address_id() != null)
		 * stmt_update_existing_trip.setLong(12,
		 * tripStatistics.getLast_geolocation_address_id()); else
		 * stmt_update_existing_trip.setLong(12, 0);
		 */

		if (tripStatistics.getLast_processed_message_timestamp() != null)
			stmt_update_existing_trip.setLong(10, tripStatistics.getLast_processed_message_timestamp());
		else
			stmt_update_existing_trip.setLong(10, 0);

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
			stmt_update_existing_trip.setLong(11, tripStatistics.getModified_at());
		else
			stmt_update_existing_trip.setLong(11, 0);
		System.out.println("finish update fill");

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
		logger.info("read started CurrentTripStatistics tripId -{}  time-{}", tripId , java.time.LocalTime.now());

		try {

			if (null != tripId && null != (connection = getConnection())) {

				stmt_read_current_trip = connection.prepareStatement(READ_CURRENT_TRIP, ResultSet.TYPE_SCROLL_SENSITIVE,
						ResultSet.CONCUR_UPDATABLE);
				stmt_read_current_trip.setString(1, tripId);

				//System.out.println("READ_CURRENT_TRIP query : " + stmt_read_current_trip);

				rs_trip = stmt_read_current_trip.executeQuery();

				//System.out.println("rs_trip " + rs_trip);

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
					/*
					 * System.out.println("RESULTSET RECEIVED from READ_CURRENT_TRIP for tripId = "
					 * + tripId + " is " + currentTripdata.toString());
					 */
				logger.info("read finished CurrentTripStatistics tripId -{}  time-{}", tripId , java.time.LocalTime.now());
				
				else 
					//System.out.println("RESULTSET RECEIVED from READ_CURRENT_TRIP for tripId = " + tripId + " is NULL");
				logger.info("RESULTSET RECEIVED from READ_CURRENT_TRIP for tripId is null"+ tripId);

				rs_trip.close();

			}

		} catch (SQLException e) {
			//System.out.println("issue in sql read" + stmt_read_current_trip);
			e.printStackTrace();

		} catch (Exception e) {
			//System.out.println("issue in read" + stmt_read_current_trip);
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
			//System.out.println("inside try");
			if (null != row && null != (connection = getConnection())) {

				trip_stats_update_stmt = connection.prepareStatement(UPDATE_CURRENT_TRIP,
						ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);
				trip_stats_update_stmt = fillUpdateStatement(trip_stats_update_stmt, row);
				trip_stats_update_stmt.setString(12, row.getTripId());

				//System.out.println("UPDATE_CURRENT_TRIP query : " + trip_stats_update_stmt);

				int num = trip_stats_update_stmt.executeUpdate();

				//System.out.println("NUMBER OF ROWS updated by UPDATE_CURRENT_TRIP is " + num);
				logger.info("Row updated in CurrentTripStatistics for TripId is" + row.getTripId());

			}
		} catch (SQLException e) {
			//System.out.println("Issue in sql update trip statistics::" + trip_stats_update_stmt);
			logger.error("sql exception inside catch LiveFleetCurrentTripStatisticsDao update" + e.getMessage());
			e.printStackTrace();
		} catch (Exception e) {
			logger.error("error inside catch LiveFleetCurrentTripStatisticsDao update" + e.getMessage());
			//System.out.println("Issue in update trip statistics::" + trip_stats_update_stmt);
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

	public WarningStatisticsPojo readWarning(String vin) throws TechnicalException, SQLException {

		PreparedStatement readWarnStatusStmt = null;
		//ResultSet rs_trip = null;
		ResultSet rs_warn_vehicle_health_status = null;
		//CurrentTrip currentTripdata = null;
		WarningStatisticsPojo warnStatsPojo = null;

		try {

			if (null != vin && null != (connection = getConnection())) {
				logger.info("read started CurrentTripStatistics for warning  vin -{}  time-{}", vin , java.time.LocalTime.now());
				readWarnStatusStmt = connection.prepareStatement(READ_LATEST_WARNING_STATUS_AT_TRIP_START, ResultSet.TYPE_SCROLL_SENSITIVE,
						ResultSet.CONCUR_UPDATABLE);
				readWarnStatusStmt.setString(1, vin);

				//System.out.println("READ_CURRENT_TRIP query : " + readWarnStatusStmt);
				logger.info("Current Trip query to read warning data"+ readWarnStatusStmt);

				rs_warn_vehicle_health_status = readWarnStatusStmt.executeQuery();

				//System.out.println("rs_warn_vehicle_health_status " + rs_warn_vehicle_health_status);

				while (rs_warn_vehicle_health_status.next()) {
					
					warnStatsPojo= new WarningStatisticsPojo();

					if (rs_warn_vehicle_health_status.getString("warning_type").equalsIgnoreCase("A")) {
						warnStatsPojo.setVehicleHealthStatusType(rs_warn_vehicle_health_status.getString("vehicle_health_status_type"));
						warnStatsPojo.setWarningClass(rs_warn_vehicle_health_status.getInt("warning_class"));
						warnStatsPojo.setWarningNumber(rs_warn_vehicle_health_status.getInt("warning_number"));
						warnStatsPojo.setWarningType(rs_warn_vehicle_health_status.getString("warning_type"));
						warnStatsPojo.setWarningTimeStamp(rs_warn_vehicle_health_status.getLong("warning_time_stamp"));
						warnStatsPojo.setLatitude(rs_warn_vehicle_health_status.getDouble("latitude")); 
						warnStatsPojo.setLongitude(rs_warn_vehicle_health_status.getDouble("longitude"));
						warnStatsPojo.setMessageType(rs_warn_vehicle_health_status.getInt("message_type"));
						warnStatsPojo.setTripId(rs_warn_vehicle_health_status.getString("trip_id"));
						warnStatsPojo.setVin(rs_warn_vehicle_health_status.getString("vin"));
					}
				}
				if(!Objects.isNull(warnStatsPojo))
					/*
					 * System.out.println("RESULTSET RECEIVED from READ_CURRENT_TRIP for tripId = "
					 * + vin + " is " + warnStatsPojo.toString());
					 */
				logger.info("read finished CurrentTripStatistics for warning  vin -{}  time-{}", vin , java.time.LocalTime.now());
				
				else 
					//System.out.println("RESULTSET RECEIVED from READ_CURRENT_TRIP for tripId = " + vin + " is NULL");
				logger.info("RESULTSET RECEIVED from READ_CURRENT_TRIP for tripId is null" +vin);

				rs_warn_vehicle_health_status.close();

			}

		} catch (SQLException e) {
			//System.out.println("Issue in sql update trip statistics::" + trip_stats_update_stmt);
			logger.error("Issue in sql read warning from currentTrip" + readWarnStatusStmt);
			logger.error("Issue in sql read warning from currentTrip" + e.getMessage());
			e.printStackTrace();
		} catch (Exception e) {
			logger.error("Issue in sql read warning from currentTrip" + readWarnStatusStmt);
			logger.error("Issue in sql read warning from currentTrip" + e.getMessage());
			e.printStackTrace();
		} finally {

			if (null != rs_warn_vehicle_health_status) {

				try {
					rs_warn_vehicle_health_status.close();
				} catch (SQLException ignore) {
					/** ignore any errors here */

				}
			}
		}

		return warnStatsPojo;
	}
	
	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}


}
