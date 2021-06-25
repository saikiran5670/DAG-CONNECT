package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Types;

import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.postgre.bo.DriverActivityPojo;

import net.atos.daf.postgre.util.DafConstants;

public class LiveFleetDriverActivityDao implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private Connection connection;
	/** SQL statement for insert. */
	//private static final String LIVEFLEET_DRIVER_INSERT = "INSERT INTO livefleet.livefleet_trip_driver_activity  (trip_id    , trip_start_time_stamp , trip_end_time_stamp   , activity_date,  vin   , driver_id     , code  , start_time    , end_time      , duration      , created_at_m2m        , created_at_kafka      , created_at_dm , modified_at   , last_processed_message_time_stamp ,is_driver1    ) VALUES ( ?, ?, ?, ?   , ?,?, ?, ?, ?, ?       , ?     , ?     , ?     , ? ,?    ,?)";
	private static final String LIVEFLEET_DRIVER_INSERT = "INSERT INTO livefleet.livefleet_trip_driver_activity  (trip_id    , trip_start_time_stamp , trip_end_time_stamp   , activity_date,  vin   , driver_id     , code  , start_time    , end_time      , duration      , created_at_m2m        , created_at_kafka      , created_at_dm , modified_at   , last_processed_message_time_stamp ,is_driver1 ,logical_code   ) VALUES ( ?, ?, ?, ?   , ?,?, ?, ?, ?, ?       , ?     , ?     , ?     , ? ,?    ,?, ?)";
	private static final String LIVEFLEET_DRIVER_READ = "SELECT * FROM livefleet.livefleet_trip_driver_activity WHERE trip_start_time_stamp !=0 AND trip_id = ?";
	private static final String DRIVER_ACTIVITY_READ = "select code,start_time from livefleet.livefleet_trip_driver_activity  where driver_id = ? order by id DESC limit 1";
	private static final String DRIVER_ACTIVITY_UPDATE = "UPDATE livefleet.livefleet_trip_driver_activity  SET end_time = ?, duration = ?, modified_at = extract(epoch from now()) * 1000 WHERE driver_id IN ( SELECT driver_id FROM livefleet.livefleet_trip_driver_activity WHERE driver_id = ? ORDER BY id DESC LIMIT 1 ) AND id IN ( SELECT id FROM livefleet.livefleet_trip_driver_activity WHERE driver_id = ? ORDER BY id DESC LIMIT 1 )";

	public boolean insert(Index row, Long trip_Start_time) throws TechnicalException, SQLException {
		PreparedStatement stmt_insert_driver_activity;

		boolean result = false;

		try {

			if (null != row && null != (connection = getConnection())) {

				stmt_insert_driver_activity = connection.prepareStatement(LIVEFLEET_DRIVER_INSERT,
						ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);

				stmt_insert_driver_activity.addBatch();
				stmt_insert_driver_activity.executeBatch();
			}
		} catch (SQLException e) {
			System.out.println("inside catch LiveFleetDriverActivityDao Insert");
			e.printStackTrace();
		}

		return result;
	}

	public boolean driver_insert(DriverActivityPojo DriverDetails) throws TechnicalException, SQLException {
		PreparedStatement stmt_insert_driver_activity;

		boolean result = false;
		try {

			if (null != DriverDetails && null != (connection = getConnection())) {

				stmt_insert_driver_activity = connection.prepareStatement(LIVEFLEET_DRIVER_INSERT,
						ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);
				stmt_insert_driver_activity = fillStatement(stmt_insert_driver_activity, DriverDetails);

				stmt_insert_driver_activity.addBatch();
				stmt_insert_driver_activity.executeBatch();
			}
		} catch (SQLException e) {
			System.out.println("inside catch LiveFleetDriverActivityDao Insert");
			e.printStackTrace();
		}

		return result;
	}

	public void driver_update(String DriverID, Long endTime, Long duration) throws TechnicalException, SQLException {

		PreparedStatement stmt_update_driver_activity = null;
		//ResultSet rs_driver = null;

		try {

			if (null != DriverID && null != (connection = getConnection())) {

				stmt_update_driver_activity = connection.prepareStatement(DRIVER_ACTIVITY_UPDATE,
						ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);

				stmt_update_driver_activity.setLong(1, endTime);
				stmt_update_driver_activity.setLong(2, duration);
				stmt_update_driver_activity.setString(3, DriverID);
				stmt_update_driver_activity.setString(4, DriverID);
				
				int i = stmt_update_driver_activity.executeUpdate() ;//executeQuery();
				//rs_driver.close();

			}
		} catch (SQLException e) {
			e.printStackTrace();
		} finally {

			/*if (null != rs_driver) {

				try {
					rs_driver.close();
				} catch (SQLException ignore) {
					*//** ignore any errors here *//*
				}
			}*/
		}

	}

	public Long driver_read(String DriverID, String code) throws TechnicalException, SQLException {
		PreparedStatement stmt_read_driver_activity = null;
		ResultSet rs_driver = null;
		String newCode;
		Long start_time = null;
		

		try {

			if (null != DriverID && null != (connection = getConnection())) {

				stmt_read_driver_activity = connection.prepareStatement(DRIVER_ACTIVITY_READ,
						ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);
				stmt_read_driver_activity.setString(1, DriverID);

				rs_driver = stmt_read_driver_activity.executeQuery();

				while (rs_driver.next()) {
					newCode = rs_driver.getString("code");

					start_time = rs_driver.getLong("start_time");
				}

				rs_driver.close();

			}
		} catch (SQLException e) {
			e.printStackTrace();
		} finally {

			if (null != rs_driver) {

				try {
					rs_driver.close();
				} catch (SQLException ignore) {
					/** ignore any errors here */
				}
			}
		}
		return start_time;

	}

	private PreparedStatement fillStatement(PreparedStatement stmt_insert_driver_activity, DriverActivityPojo row)
			throws SQLException {

		stmt_insert_driver_activity.setString(1, row.getTripId()); // 1-tripid
		//stmt_insert_driver_activity.setLong(2, row.getTripStartTimeStamp()); // 2-trip_start_time
		stmt_insert_driver_activity.setLong(2, Types.NULL); // 2-trip_start_time
		//stmt_insert_driver_activity.setLong(3, row.getTripEndTimeStamp()); // 3-trip_end_time_stamp
		stmt_insert_driver_activity.setLong(3, Types.NULL);

		if (row.getActivityDate() != null) {
			stmt_insert_driver_activity.setLong(4, row.getActivityDate()); // 4-ACTIVITY_DATE
		} else
			stmt_insert_driver_activity.setLong(4, DafConstants.DTM_NULL_VAL);

		if (row.getVin() != null) {
			System.out.println("Vin for Driver Activity--->"+ row.getVin());
		stmt_insert_driver_activity.setString(5, row.getVin()); // 5-vin
		} else {
			stmt_insert_driver_activity.setString(5, "");
		}
		
		
		
		if (row.getDriverID() != null)
			stmt_insert_driver_activity.setString(6, (String) row.getDriverID()); // 6-DriverID
		else
			stmt_insert_driver_activity.setLong(6, DafConstants.DTM_NULL_VAL);

		//TODO Temporary fix
		/*if (row.getCode() != null)
			stmt_insert_driver_activity.setString(7, row.getCode()); // 7-Working_state_code
		else
			stmt_insert_driver_activity.setString(7, "");*/
		if (row.getCode() != null){
			
			if("2".equals(row.getCode()) && row.getTripId() != null)
				stmt_insert_driver_activity.setString(7, "3"); // 7-Working_state_code
			else
				stmt_insert_driver_activity.setString(7, row.getCode());
			
		}else
			stmt_insert_driver_activity.setString(7, "");
		

		if (row.getStartTime() != null) {
			stmt_insert_driver_activity.setLong(8, row.getStartTime()); // 8-start_time
		} else
			stmt_insert_driver_activity.setInt(8, DafConstants.DTM_NULL_VAL_int);

		if ((row.getEndTime() != null)) {

			stmt_insert_driver_activity.setLong(9, row.getEndTime()); // 9-end_time
		} else
			stmt_insert_driver_activity.setInt(9, DafConstants.DTM_NULL_VAL_int);

		stmt_insert_driver_activity.setLong(10, Types.NULL); // 10-Duration

		stmt_insert_driver_activity.setLong(11, row.getCreatedAtM2m()); // 11-created_at_m2m
		stmt_insert_driver_activity.setLong(12, row.getCreatedAtKafka()); // 12-created_at_kafka
		stmt_insert_driver_activity.setLong(13, row.getCreatedAtDm()); // 13-created_at_dm
		stmt_insert_driver_activity.setLong(14, Types.NULL); // 14-modified_at
		stmt_insert_driver_activity.setLong(15, row.getLastProcessedMessageTimestamp()); // 15-last_processed_message_time
		stmt_insert_driver_activity.setBoolean(16, row.getIsDriver1()); // 16-is_Driver_flag
		
		//TODO Temporary fix
		stmt_insert_driver_activity.setString(17, row.getCode());

		return stmt_insert_driver_activity;

	}

	/*
	 * 
	 * 
	 * private PreparedStatement fillStatement(PreparedStatement
	 * stmt_insert_driver_activity, Index row, Long trip_Start_time) throws
	 * SQLException {
	 * 
	 * int varVEvtid = row.getVEvtID();
	 * 
	 * if (row.getDocument().getTripID() != null)
	 * stmt_insert_driver_activity.setString(1, row.getDocument().getTripID());
	 * // 1 // trip // id else stmt_insert_driver_activity.setString(1,
	 * row.getRoName());
	 * 
	 * if (varVEvtid == 4) stmt_insert_driver_activity.setLong(2,
	 * row.getReceivedTimestamp()); // 2 // trip_start_time_stamp
	 * 
	 * else stmt_insert_driver_activity.setLong(2, 0);
	 * 
	 * if (varVEvtid == 5) stmt_insert_driver_activity.setLong(3,
	 * row.getReceivedTimestamp()); // 3 // trip_end_time_stamp else
	 * stmt_insert_driver_activity.setLong(3, 0);
	 * 
	 * if (row.getReceivedTimestamp() != null)
	 * stmt_insert_driver_activity.setLong(4, row.getReceivedTimestamp()); // 4
	 * // Activity_date else stmt_insert_driver_activity.setLong(4, 0);
	 * 
	 * if (row.getVin() != null) stmt_insert_driver_activity.setString(5,
	 * (String) row.getVin()); // 5 // vin else
	 * stmt_insert_driver_activity.setString(5, (String) row.getVid());
	 * 
	 * if (row.getDriverID() != null) stmt_insert_driver_activity.setString(6,
	 * (String) row.getDriverID()); // 6 // Driver // ID else
	 * stmt_insert_driver_activity.setLong(6, 0);
	 * 
	 * if (row.getDocument().getDriver1WorkingState() != null)
	 * stmt_insert_driver_activity.setInt(7,
	 * row.getDocument().getDriver1WorkingState()); // 7 // Working // state //
	 * code else stmt_insert_driver_activity.setLong(7, 0);
	 * 
	 * if ((row.getDocument().getDriver1CardInserted() == true) &&
	 * ((row.getDocument().getDriver1WorkingState() == 1) ||
	 * (row.getDocument().getDriver1WorkingState() == 2) ||
	 * (row.getDocument().getDriver1WorkingState() == 3)))
	 * stmt_insert_driver_activity.setLong(8, row.getReceivedTimestamp()); // 8
	 * // start_time else stmt_insert_driver_activity.setInt(8, 0);
	 * 
	 * if ((row.getDocument().getDriver1CardInserted() == false) ||
	 * ((row.getDocument().getDriver1CardInserted() == true) &&
	 * (row.getDocument().getDriver1WorkingState() == 0)))
	 * stmt_insert_driver_activity.setLong(9, row.getReceivedTimestamp()); // 9
	 * // end_time else stmt_insert_driver_activity.setInt(9, 0);
	 * 
	 * stmt_insert_driver_activity.setLong(10, (row.getReceivedTimestamp() -
	 * trip_Start_time)); // 10 // Duration
	 * 
	 * stmt_insert_driver_activity.setLong(11, row.getReceivedTimestamp()); //
	 * 11 // created_at_m2m stmt_insert_driver_activity.setLong(12,
	 * row.getReceivedTimestamp()); // 12 // created_at_kafka
	 * stmt_insert_driver_activity.setLong(13, row.getReceivedTimestamp()); //
	 * 13 // created_at_dm stmt_insert_driver_activity.setLong(14,
	 * row.getReceivedTimestamp()); // 14 // modified_at
	 * stmt_insert_driver_activity.setLong(15, row.getReceivedTimestamp()); //
	 * 15 // last_processed_message_time
	 * 
	 * System.out.println("trip activity starting and VEvt ID is --> " +
	 * varVEvtid);
	 * 
	 * return stmt_insert_driver_activity;
	 * 
	 * }
	 * 
	 */

	public Long read(String tripId) throws TechnicalException, SQLException {

		PreparedStatement stmt_read_driver_activity = null;
		ResultSet rs_driver = null;
		long trip_Start_time = 0;

		try {

			if (null != tripId && null != (connection = getConnection())) {
				// stmt_read_driver_activity =
				// connection.prepareStatement(LIVEFLEET_DRIVER_READ);
				stmt_read_driver_activity = connection.prepareStatement(LIVEFLEET_DRIVER_READ,
						ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);
				stmt_read_driver_activity.setString(1, tripId);

				System.out.println("inside read function of LiveFleet DriverActivity");
				rs_driver = stmt_read_driver_activity.executeQuery();
				while (rs_driver.next()) {

					trip_Start_time = rs_driver.getLong("trip_start_time_stamp");

				}
				System.out.println("outside while function of LiveFleet DriverActivity");

				rs_driver.close();

			}
		} catch (SQLException e) {
			e.printStackTrace();
		} finally {

			if (null != rs_driver) {

				try {
					rs_driver.close();
				} catch (SQLException ignore) {
					/** ignore any errors here */
				}
			}
		}
		return trip_Start_time;
	}

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}
}
