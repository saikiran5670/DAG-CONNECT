package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;

public class LiveFleetDriverActivityDao implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private Connection connection;
	/** SQL statement for insert. */
	private static final String LIVEFLEET_DRIVER_INSERT = "INSERT INTO livefleet.livefleet_trip_driver_activity  (trip_id    , trip_start_time_stamp , trip_end_time_stamp   , activity_date,  vin   , driver_id     , code  , start_time    , end_time      , duration      , created_at_m2m        , created_at_kafka      , created_at_dm , modified_at   , last_processed_message_time_stamp     ) VALUES ( ?, ?, ?, ?   , ?,?, ?, ?, ?, ?       , ?     , ?     , ?     , ?     ,?)";
	private static final String LIVEFLEET_DRIVER_READ = "SELECT * FROM livefleet.livefleet_trip_driver_activity WHERE trip_start_time_stamp !=0 AND trip_id = ?";

	public boolean insert(Index row, Long trip_Start_time) throws TechnicalException, SQLException {
		PreparedStatement stmt_insert_driver_activity;

		boolean result = false;
		// String varTripID = row.getDocument().getTripID(); /// taken
		/// in two classes
		

		try {

			if (null != row && null != (connection = getConnection())) {
				stmt_insert_driver_activity = connection.prepareStatement(LIVEFLEET_DRIVER_INSERT);
				stmt_insert_driver_activity = fillStatement(stmt_insert_driver_activity, row, trip_Start_time);
				System.out.println("inside LiveFleetDriverActivityDao Insert");
				stmt_insert_driver_activity.addBatch();
				stmt_insert_driver_activity.executeBatch();
			}
		} catch (SQLException e) {
			System.out.println("inside catch LiveFleetDriverActivityDao Insert");
			e.printStackTrace();
		}

		return result;
	}

	private PreparedStatement fillStatement(PreparedStatement stmt_insert_driver_activity, Index row,
			Long trip_Start_time) throws SQLException {

		int varVEvtid = row.getVEvtID();

		if (row.getDocument().getTripID() != null)
			stmt_insert_driver_activity.setString(1, row.getDocument().getTripID()); // 1
																						// trip
																						// id
		else
			stmt_insert_driver_activity.setString(1, row.getRoName());

		if (varVEvtid == 4)
			stmt_insert_driver_activity.setLong(2, row.getReceivedTimestamp()); // 2
																				// trip_start_time_stamp

		else
			stmt_insert_driver_activity.setLong(2, 0);

		if (varVEvtid == 5)
			stmt_insert_driver_activity.setLong(3, row.getReceivedTimestamp()); // 3
																				// trip_end_time_stamp
		else
			stmt_insert_driver_activity.setLong(3, 0);

		if (row.getReceivedTimestamp() != null)
			stmt_insert_driver_activity.setLong(4, row.getReceivedTimestamp()); // 4
																				// Activity_date
		else
			stmt_insert_driver_activity.setLong(4, 0);

		if (row.getVin() != null)
			stmt_insert_driver_activity.setString(5, (String) row.getVin()); // 5
																				// vin
		else
			stmt_insert_driver_activity.setString(5, (String) row.getVid());

		if (row.getDriverID() != null)
			stmt_insert_driver_activity.setString(6, (String) row.getDriverID()); // 6
																					// Driver
																					// ID
		else
			stmt_insert_driver_activity.setLong(6, 0);

		if (row.getDocument().getDriver1WorkingState() != null)
			stmt_insert_driver_activity.setInt(7, row.getDocument().getDriver1WorkingState()); // 7
																								// Working
																								// state
																								// code
		else
			stmt_insert_driver_activity.setLong(7, 0);

		if ((row.getDocument().getDriver1CardInserted() == true) && ((row.getDocument().getDriver1WorkingState() == 1)
				|| (row.getDocument().getDriver1WorkingState() == 2)
				|| (row.getDocument().getDriver1WorkingState() == 3)))
			stmt_insert_driver_activity.setLong(8, row.getReceivedTimestamp()); // 8
																				// start_time
		else
			stmt_insert_driver_activity.setInt(8, 0);

		if ((row.getDocument().getDriver1CardInserted() == false)
				|| ((row.getDocument().getDriver1CardInserted() == true)
						&& (row.getDocument().getDriver1WorkingState() == 0)))
			stmt_insert_driver_activity.setLong(9, row.getReceivedTimestamp()); // 9
																				// end_time
		else
			stmt_insert_driver_activity.setInt(9, 0);

		stmt_insert_driver_activity.setLong(10, (row.getReceivedTimestamp() - trip_Start_time)); // 10
																									// Duration

		stmt_insert_driver_activity.setLong(11, row.getReceivedTimestamp()); // 11
																				// created_at_m2m
		stmt_insert_driver_activity.setLong(12, row.getReceivedTimestamp()); // 12
																				// created_at_kafka
		stmt_insert_driver_activity.setLong(13, row.getReceivedTimestamp()); // 13
																				// created_at_dm
		stmt_insert_driver_activity.setLong(14, row.getReceivedTimestamp()); // 14
																				// modified_at
		stmt_insert_driver_activity.setLong(15, row.getReceivedTimestamp()); // 15
																				// last_processed_message_time

		System.out.println("trip activity starting and VEvt ID is --> " + varVEvtid);

		return stmt_insert_driver_activity;

	}

	public Long read(String tripId) throws TechnicalException, SQLException {

		PreparedStatement stmt_read_driver_activity = null;
		ResultSet rs_driver = null;
		long trip_Start_time = 0;

		try {

			if (null != tripId && null != (connection = getConnection())) {
				stmt_read_driver_activity = connection.prepareStatement(LIVEFLEET_DRIVER_READ);

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
