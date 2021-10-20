package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Types;

import org.apache.logging.log4j.LogManager;


import net.atos.daf.common.ct2.exception.TechnicalException;

import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.postgre.bo.DriverActivityPojo;
import net.atos.daf.postgre.bo.TwoMinuteRulePojo;
import net.atos.daf.postgre.util.DafConstants;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

public class LiveFleetDriverActivityDao implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	 private static final Logger log = LogManager.getLogger(LiveFleetDriverActivityDao.class);
	private Connection connection;
	/** SQL statement for insert. */
	private static final String LIVEFLEET_DRIVER_INSERT = "INSERT INTO livefleet.livefleet_trip_driver_activity  (trip_id    , trip_start_time_stamp , trip_end_time_stamp   , activity_date,  vin   , driver_id     , code  , start_time    , end_time      , duration      , created_at_m2m        , created_at_kafka      , created_at_dm , modified_at   , last_processed_message_time_stamp ,is_driver1, logical_code    ) VALUES ( ?, ?, ?, ?   , ?,?, ?, ?, ?, ?       , ?     , ?     , ?     , ? ,?    ,?, ?)";
	private static final String LIVEFLEET_DRIVER_READ = "SELECT * FROM livefleet.livefleet_trip_driver_activity WHERE trip_start_time_stamp !=0 AND trip_id = ? AND trip_id IS NOT NULL";
	private static final String DRIVER_ACTIVITY_READ = "select code, start_time, duration from livefleet.livefleet_trip_driver_activity  where driver_id = ? AND driver_id IS NOT NULL order by id DESC limit 1";
	private static final String DRIVER_ACTIVITY_UPDATE = "UPDATE livefleet.livefleet_trip_driver_activity  SET end_time = ?, duration = ?, modified_at = extract(epoch from now()) * 1000, logical_code = ? WHERE driver_id IN ( SELECT driver_id FROM livefleet.livefleet_trip_driver_activity WHERE driver_id = ?  ORDER BY id DESC LIMIT 1 ) AND id IN ( SELECT id FROM livefleet.livefleet_trip_driver_activity WHERE driver_id = ? ORDER BY id DESC LIMIT 1 )";
	//private static final String DRIVER_ACTIVITY_UPDATE ="UPDATE livefleet.livefleet_trip_driver_activity  SET end_time = ?, duration = ?, modified_at = extract(epoch from now()) * 1000, logical_code = ? WHERE id = ( SELECT id FROM livefleet.livefleet_trip_driver_activity WHERE driver_id = ? ORDER BY id DESC LIMIT 1 )";
	
	
	
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
			log.error("inside catch LiveFleetDriverActivityDao Insert" + e.getMessage());
			e.printStackTrace();
		}

		return result;
	}

	public boolean driver_insert(DriverActivityPojo DriverDetails) throws TechnicalException, SQLException {
		PreparedStatement stmt_insert_driver_activity;

		boolean result = false;
		try {
			log.info("inside insert for Driver management ");
			if (null != DriverDetails && null != (connection = getConnection())) {

				stmt_insert_driver_activity = connection.prepareStatement(LIVEFLEET_DRIVER_INSERT,
						ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);
				stmt_insert_driver_activity = fillStatement(stmt_insert_driver_activity, DriverDetails);
				log.info("Insert Driver query--" + stmt_insert_driver_activity);
				stmt_insert_driver_activity.addBatch();
				stmt_insert_driver_activity.executeBatch();
				log.info("data inserted for driver-->" + DriverDetails.getDriverID());
			}
		} catch (SQLException e) {
			log.error("inside catch LiveFleetDriverActivityDao Driver Insert" + e.getMessage());
			e.printStackTrace();
		}

		return result;
	}

	public void driver_update(String DriverID, Long endTime, Long duration, String logicalCode) throws TechnicalException, SQLException {

		PreparedStatement stmt_update_driver_activity = null;
		//ResultSet rs_driver = null;

		try {

			if (null != DriverID && null != (connection = getConnection())) {

				stmt_update_driver_activity = connection.prepareStatement(DRIVER_ACTIVITY_UPDATE,
						ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);

				stmt_update_driver_activity.setLong(1, endTime);
				stmt_update_driver_activity.setLong(2, duration);
				
				//to be unexecute
				stmt_update_driver_activity.setString(3, logicalCode);
				
				stmt_update_driver_activity.setString(4, DriverID);
				stmt_update_driver_activity.setString(5, DriverID);
				log.info("Update Driver query--" + stmt_update_driver_activity);
				
				int i = stmt_update_driver_activity.executeUpdate() ;
				//rs_driver.close();

			}
		} catch (SQLException e) {
			log.error("inside catch LiveFleetDriverActivityDao Driver Update" + e.getMessage());
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

	public TwoMinuteRulePojo driver_read(String DriverID, String code) throws TechnicalException, SQLException {
		PreparedStatement stmt_read_driver_activity = null;
		ResultSet rs_driver = null;
		String oldCode = null;
		Long start_time = null;
		Long duration = null;
		TwoMinuteRulePojo driverDetails= null;
		

		try {

			if (null != DriverID && null!=code && null != (connection = getConnection())) {

				stmt_read_driver_activity = connection.prepareStatement(DRIVER_ACTIVITY_READ,
						ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);
				stmt_read_driver_activity.setString(1, DriverID);
				
				System.out.println("===============READ STATEMENT - DRIVER ACTIVITY DAO==============");
				System.out.println(stmt_read_driver_activity.toString());

				rs_driver = stmt_read_driver_activity.executeQuery();

				while (rs_driver.next()) {
					oldCode = rs_driver.getString("code");

					start_time = rs_driver.getLong("start_time");
					duration = rs_driver.getLong("duration");
					driverDetails = new TwoMinuteRulePojo();
					
					driverDetails.setCode(oldCode);
					driverDetails.setStart_time(start_time);
					driverDetails.setDuration(duration);
					
					
				}
				
				

				rs_driver.close();

			} else {
				log.info("drived id or code is null");
			}
		} catch (SQLException e) {
			log.error("inside catch LiveFleetDriverActivityDao Two Minute rule driver read" + e.getMessage());
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
		return driverDetails;

	}

	private PreparedStatement fillStatement(PreparedStatement stmt_insert_driver_activity, DriverActivityPojo row)
			throws SQLException {
		
		//need to add logical code here

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
			stmt_insert_driver_activity.setString(5, row.getVid());
		}
		
		
		
		if (row.getDriverID() != null)
			stmt_insert_driver_activity.setString(6, (String) row.getDriverID()); // 6-DriverID
		else
			stmt_insert_driver_activity.setLong(6, DafConstants.DTM_NULL_VAL);

		if (row.getCode() != null)
			stmt_insert_driver_activity.setString(7, row.getCode()); // 7-Working_state_code
		else
			stmt_insert_driver_activity.setString(7, "");

		if (row.getStartTime() != null) {
			stmt_insert_driver_activity.setLong(8, row.getStartTime()); // 8-start_time
		} else
			stmt_insert_driver_activity.setInt(8, DafConstants.DTM_NULL_VAL_int);

		if ((row.getEndTime() != null)) {

			stmt_insert_driver_activity.setLong(9, row.getEndTime()); // 9-end_time
		} else
			stmt_insert_driver_activity.setInt(9, DafConstants.DTM_NULL_VAL_int);

		stmt_insert_driver_activity.setLong(10, row.getDuration()); // 10-Duration

		if(row.getCreatedAtM2m()!=null) {
			stmt_insert_driver_activity.setLong(11, row.getCreatedAtM2m()); // 11-created_at_m2m
		} else {
			stmt_insert_driver_activity.setLong(11, Types.NULL);
		}
		stmt_insert_driver_activity.setLong(12, row.getCreatedAtKafka()); // 12-created_at_kafka
		stmt_insert_driver_activity.setLong(13, row.getCreatedAtDm()); // 13-created_at_dm
		stmt_insert_driver_activity.setLong(14, Types.NULL); // 14-modified_at
		stmt_insert_driver_activity.setLong(15, row.getLastProcessedMessageTimestamp()); // 15-last_processed_message_time
		stmt_insert_driver_activity.setBoolean(16, row.getIsDriver1()); // 16-is_Driver_flag

		if(row.getCode()!=null) {
			stmt_insert_driver_activity.setString(17, row.getCode());
		} else {
			stmt_insert_driver_activity.setString(17, "");
		}
		
		
		return stmt_insert_driver_activity;

	}

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
				rs_driver.close();

			}
		} catch (SQLException e) {
			log.error("inside catch LiveFleetDriverActivityDao  DRiver read" + e.getMessage());
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
