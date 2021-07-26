package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.Arrays;

import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.postgre.bo.CurrentTrip;
import net.atos.daf.postgre.bo.TripStatisticsPojo;

public class LivefleetCurrentTripStatisticsDao implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private Connection connection;

	/** SQL statement for insert. */
	private static final String READ_CURRENT_TRIP = "SELECT * FROM livefleet.livefleet_current_trip_statistics WHERE trip_id = ? ORDER BY created_at ASC limit 1";
	private static final String INSERT_CURRENT_TRIP = "INSERT INTO livefleet.livefleet_current_trip_statistics ( trip_id , vin , start_time_stamp , end_time_stamp , "
			+ "driver1_id , trip_distance , driving_time , fuel_consumption , vehicle_driving_status_type , odometer_val , distance_until_next_service , latest_received_position_lattitude , "
			+ "latest_received_position_longitude , latest_received_position_heading , latest_geolocation_address_id , start_position_lattitude , start_position_longitude , start_position_heading , "
			+ "start_geolocation_address_id , latest_processed_message_time_stamp , vehicle_health_status_type , latest_warning_class , latest_warning_number , latest_warning_type , latest_warning_timestamp , "
			+ "latest_warning_position_latitude , latest_warning_position_longitude , latest_warning_geolocation_address_id , created_at , modified_at ) "
			+ "VALUES (? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ? , ?  )";
	private static final String UPDATE_CURRENT_TRIP = "UPDATE livefleet.livefleet_current_trip_statistics  SET trip_id = ? , vin = ? , start_time_stamp = ? , "
			+ "end_time_stamp = ? , driver1_id = ? , trip_distance = ? , driving_time = ? , fuel_consumption = ? , vehicle_driving_status_type = ? , odometer_val = ? , "
			+ "distance_until_next_service = ? , latest_received_position_lattitude = ? , latest_received_position_longitude = ? , latest_received_position_heading = ? , "
			+ "latest_geolocation_address_id = ? , start_position_lattitude = ? , start_position_longitude = ? , start_position_heading = ? , start_geolocation_address_id = ? , " 
			+ "latest_processed_message_time_stamp = ? , vehicle_health_status_type = ? , latest_warning_class = ? , latest_warning_number = ? , latest_warning_type = ? , "
			+ "latest_warning_timestamp = ? , latest_warning_position_latitude = ? , latest_warning_position_longitude = ? , latest_warning_geolocation_address_id = ? , "
			+ "created_at = ? , modified_at = ? WHERE trip_id = ? ";
	
	public void insert(TripStatisticsPojo row)
			throws TechnicalException, SQLException {
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
			System.out.println("Issue in LivefleetCurrentTripStatisticsDao  " +trip_stats_insert_stmt);
			e.printStackTrace();
		}
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
			stmt_insert_current_trip.setString(21, String.valueOf(Character.valueOf(' ')));
		
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

	public CurrentTrip read(String tripId) throws TechnicalException, SQLException {

		PreparedStatement stmt_read_current_trip = null;
		ResultSet rs_trip = null;
	
		CurrentTrip currentTripdata = null;

		try {

			if (null != tripId && null != (connection = getConnection())) 
			{

				stmt_read_current_trip = connection.prepareStatement(READ_CURRENT_TRIP, ResultSet.TYPE_SCROLL_SENSITIVE,ResultSet.CONCUR_UPDATABLE);
				stmt_read_current_trip.setString(1, tripId);
				
				System.out.println("READ_CURRENT_TRIP query : " + stmt_read_current_trip);
				
				rs_trip = stmt_read_current_trip.executeQuery();
				
				currentTripdata = new CurrentTrip();
				currentTripdata.setTrip_id(tripId);
				System.out.println("rs_trip "+rs_trip);

				while(rs_trip.next()){
				//while (rs_trip.first()) {
	
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
				}
				//}
				System.out.println("RESULTSET RECEIVED from READ_CURRENT_TRIP for tripId = " + tripId + " is " + currentTripdata.toString());
				
				
				rs_trip.close();

			}
		
		} catch (SQLException e) {
			System.out.println("issue in sql read" +stmt_read_current_trip);
			e.printStackTrace();
			
		}  catch(Exception e) {
			System.out.println("issue in read" +stmt_read_current_trip);
			e.printStackTrace();
		}
		finally {

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
				trip_stats_update_stmt = fillStatement(trip_stats_update_stmt,row);
				trip_stats_update_stmt.setString(31, row.getTripId());
				
				System.out.println("UPDATE_CURRENT_TRIP query : " + trip_stats_update_stmt);
				
				int num = trip_stats_update_stmt.executeUpdate() ;
				
				System.out.println("NUMBER OF ROWS updated by UPDATE_CURRENT_TRIP is " + num);
				

			}
		} catch (SQLException e) {
			System.out.println("Issue in sql update trip statistics::"+trip_stats_update_stmt);
			e.printStackTrace();
		}  catch (Exception e) {
			
			System.out.println("Issue in update trip statistics::"+trip_stats_update_stmt);
			e.printStackTrace();
		} finally {
			
			if (trip_stats_update_stmt != null) try { trip_stats_update_stmt.close(); } catch (SQLException ignore) {
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
