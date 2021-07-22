package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Types;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.postgre.bo.WarningStastisticsPojo;
import net.atos.daf.postgre.util.DafConstants;

public class WarningStatisticsDao implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	Logger logger = LoggerFactory.getLogger(WarningStatisticsDao.class);
	private Connection connection;
	/** SQL statement for insert. */
	/*
	 * private static final String LIVEFLEET_DRIVER_INSERT =
	 * "INSERT INTO livefleet.livefleet_trip_driver_activity  (trip_id    , trip_start_time_stamp , trip_end_time_stamp   , activity_date,  vin   , driver_id     , code  , start_time    , end_time      , duration      , created_at_m2m        , created_at_kafka      , created_at_dm , modified_at   , last_processed_message_time_stamp ,is_driver1, logical_code    ) VALUES ( ?, ?, ?, ?   , ?,?, ?, ?, ?, ?       , ?     , ?     , ?     , ? ,?    ,?, ?)"
	 * ; private static final String LIVEFLEET_DRIVER_READ =
	 * "SELECT * FROM livefleet.livefleet_trip_driver_activity WHERE trip_start_time_stamp !=0 AND trip_id = ?"
	 * ; private static final String DRIVER_ACTIVITY_READ =
	 * "select code, start_time, duration from livefleet.livefleet_trip_driver_activity  where driver_id = ? order by id DESC limit 1"
	 * ; private static final String DRIVER_ACTIVITY_UPDATE =
	 * "UPDATE livefleet.livefleet_trip_driver_activity  SET end_time = ?, duration = ?, modified_at = extract(epoch from now()) * 1000, logical_code = ? WHERE driver_id IN ( SELECT driver_id FROM livefleet.livefleet_trip_driver_activity WHERE driver_id = ? ORDER BY id DESC LIMIT 1 ) AND id IN ( SELECT id FROM livefleet.livefleet_trip_driver_activity WHERE driver_id = ? ORDER BY id DESC LIMIT 1 )"
	 * ;
	 */

	private static final String LIVEFLEET_WARNING_INSERT = "INSERT INTO livefleet.livefleet_warning_statistics(trip_id , vin   , warning_time_stamp,	warning_class,	warning_number,	latitude,	longitude,	heading,	vehicle_health_status_type,	vehicle_driving_status_type,	driver1_id,	warning_type,	distance_until_next_service,	odometer_val,	lastest_processed_message_time_stamp,	created_at, modified_at,	message_type) VALUES ( ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
	// private static final String LIVEFLEET_WARNING_READ ="select
	// warning_time_stamp from livefleet.livefleet_warning_statistics where vin = ?
	// AND message_type=? vin IS NOT NULL order by id DESC limit 1";

	public void warning_insert(WarningStastisticsPojo warningDetail) throws TechnicalException, SQLException {
		PreparedStatement stmt_insert_warning_statistics;
		System.out.println("warning dao insert class");

		try {

			if (null != warningDetail && null != (connection = getConnection())) {

				stmt_insert_warning_statistics = connection.prepareStatement(LIVEFLEET_WARNING_INSERT,
						ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);
				stmt_insert_warning_statistics = fillStatement(stmt_insert_warning_statistics, warningDetail);

				stmt_insert_warning_statistics.addBatch();
				stmt_insert_warning_statistics.executeBatch();
			}
		} catch (SQLException e) {
			// System.out.println("inside catch LiveFleetDriverActivityDao Insert");
			logger.error("Error in inside catch LiveFleetWarning Statistics Insert" + e.getMessage());
			e.printStackTrace();
		} catch (Exception e) {
			logger.error("Error in inside catch LiveFleetWarning Statistics Insert : " + e.getMessage());
			e.printStackTrace();
		}

	}

	/*
	 * public void warningUpdateMessageTenCommonTrip(WarningStastisticsPojo
	 * warningDetail) throws TechnicalException, SQLException { PreparedStatement
	 * updateWarningCommonTrip; System.out.println("warning dao insert class"); try
	 * {
	 * 
	 * if (null != warningDetail && null != (connection = getConnection())) {
	 * 
	 * updateWarningCommonTrip =
	 * connection.prepareStatement(LIVEFLEET_WARNING_INSERT,
	 * ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);
	 * updateWarningCommonTrip = fillStatement(updateWarningCommonTrip,
	 * warningDetail);
	 * 
	 * 
	 * updateWarningCommonTrip.setLong(1, endTime);
	 * updateWarningCommonTrip.setLong(2, duration);
	 * 
	 * updateWarningCommonTrip.setString(4, DriverID);
	 * updateWarningCommonTrip.setString(5, DriverID);
	 * 
	 * 
	 * updateWarningCommonTrip.executeUpdate(); } } catch (SQLException e) { logger.
	 * error("Sql Issue while updating data in common trip statistics table : " +
	 * e.getMessage()); e.printStackTrace(); } catch (Exception e) {
	 * logger.error("Issue while inserting data in common trip statistics table : "
	 * + e.getMessage()); e.printStackTrace(); }
	 * 
	 * }
	 */

	/*
	 * public void warningUpdateMessageFourCommonTrip(WarningStastisticsPojo
	 * warningDetail) throws TechnicalException, SQLException { PreparedStatement
	 * updateWarningCommonTrip; System.out.println("warning dao insert class"); try
	 * {
	 * 
	 * if (null != warningDetail && null != (connection = getConnection())) {
	 * 
	 * updateWarningCommonTrip =
	 * connection.prepareStatement(LIVEFLEET_WARNING_INSERT,
	 * ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);
	 * updateWarningCommonTrip = fillStatement(updateWarningCommonTrip,
	 * warningDetail);
	 * 
	 * 
	 * updateWarningCommonTrip.setLong(1, endTime);
	 * updateWarningCommonTrip.setLong(2, duration);
	 * 
	 * updateWarningCommonTrip.setString(4, DriverID);
	 * updateWarningCommonTrip.setString(5, DriverID);
	 * 
	 * 
	 * updateWarningCommonTrip.executeUpdate(); } } catch (SQLException e) { logger.
	 * error("Sql Issue while updating data in common trip statistics table : " +
	 * e.getMessage()); e.printStackTrace(); } catch (Exception e) {
	 * logger.error("Issue while inserting data in common trip statistics table : "
	 * + e.getMessage()); e.printStackTrace(); }
	 * 
	 * }
	 */

	/*
	 * public Long read(Integer messageType,String vin) throws TechnicalException,
	 * SQLException {
	 * 
	 * PreparedStatement stmt_read_warning_statistics = null; ResultSet rs_position
	 * = null; Long lastestProcessedMessageTimeStamp=null;
	 * 
	 * try {
	 * 
	 * if (null != messageType && null != (connection = getConnection())) {
	 * 
	 * stmt_read_warning_statistics =
	 * connection.prepareStatement(LIVEFLEET_WARNING_READ);
	 * 
	 * stmt_read_warning_statistics.setString(1, vin);
	 * stmt_read_warning_statistics.setInt(2, messageType);
	 * 
	 * rs_position = stmt_read_warning_statistics.executeQuery();
	 * 
	 * while (rs_position.next()) {
	 * lastestProcessedMessageTimeStamp=rs_position.getLong(
	 * "lastest_processed_message_time_stamp"); }
	 * 
	 * rs_position.close(); }
	 * 
	 * } catch (SQLException e) {
	 * logger.error("Error in Warning statistics read method : " + e.getMessage());
	 * e.printStackTrace(); } catch (Exception e) {
	 * logger.error("Error in Warning statistics read method : " + e.getMessage());
	 * e.printStackTrace(); } finally {
	 * 
	 * if (null != rs_position) {
	 * 
	 * try { rs_position.close(); } catch (SQLException ignore) {
	 *//** ignore any errors here *//*
									 * } } }
									 * 
									 * return lastestProcessedMessageTimeStamp;
									 * 
									 * }
									 */

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}

	private PreparedStatement fillStatement(PreparedStatement stmt_insert_warning_statistics,
			WarningStastisticsPojo warningDetail) throws SQLException {
		System.out.println("warning dao fill statement");

		if (warningDetail.getTripId() != null)
			stmt_insert_warning_statistics.setString(1, warningDetail.getTripId());
		else
			stmt_insert_warning_statistics.setString(1, DafConstants.UNKNOWN);

		if (warningDetail.getVin() != null)
			stmt_insert_warning_statistics.setString(2, warningDetail.getVin());
		else
			stmt_insert_warning_statistics.setString(2, warningDetail.getVid());

		if (warningDetail.getWarningTimeStamp() != null)
			stmt_insert_warning_statistics.setDouble(3, warningDetail.getWarningTimeStamp());
		else
			stmt_insert_warning_statistics.setDouble(3, 0);

		if (warningDetail.getWarningClass() != null)
			stmt_insert_warning_statistics.setDouble(4, warningDetail.getWarningClass());
		else
			stmt_insert_warning_statistics.setDouble(4, 0);

		if (warningDetail.getWarningNumber() != null)
			stmt_insert_warning_statistics.setDouble(5, warningDetail.getWarningNumber());
		else
			stmt_insert_warning_statistics.setDouble(5, 0);

		if (warningDetail.getLatitude() != null)
			stmt_insert_warning_statistics.setDouble(6, warningDetail.getLatitude());
		else
			stmt_insert_warning_statistics.setDouble(6, 0);

		if (warningDetail.getLongitude() != null)
			stmt_insert_warning_statistics.setDouble(7, warningDetail.getLongitude());
		else
			stmt_insert_warning_statistics.setDouble(7, 0);

		if (warningDetail.getHeading() != null)
			stmt_insert_warning_statistics.setDouble(8, warningDetail.getHeading());
		else
			stmt_insert_warning_statistics.setDouble(8, 0);

		if (warningDetail.getVehicleHealthStatusType() != null)
			stmt_insert_warning_statistics.setString(9, warningDetail.getVehicleHealthStatusType());
		else
			stmt_insert_warning_statistics.setString(9, "");

		if (warningDetail.getVehicleDrivingStatusType() != null)
			stmt_insert_warning_statistics.setString(10, warningDetail.getVehicleDrivingStatusType());
		else
			stmt_insert_warning_statistics.setString(10, "");

		// stmt_insert_warning_statistics.setString(9, "A");

		// stmt_insert_warning_statistics.setString(10, "A");

		if (warningDetail.getDriverID() != null) {
			stmt_insert_warning_statistics.setString(11, warningDetail.getDriverID());
		} else {
			stmt_insert_warning_statistics.setString(11, "no value");
		}

		if (warningDetail.getWarningType() != null) {
			stmt_insert_warning_statistics.setString(12, warningDetail.getWarningType());
		} else {
			stmt_insert_warning_statistics.setString(12, "");
		}

		if (warningDetail.getDistanceUntilNextService() != null) {
			stmt_insert_warning_statistics.setLong(13, warningDetail.getDistanceUntilNextService());
		} else {
			stmt_insert_warning_statistics.setLong(13, Types.NULL);
		}

		if (warningDetail.getOdometerVal() != null) {
			stmt_insert_warning_statistics.setLong(14, warningDetail.getOdometerVal());
		} else {
			stmt_insert_warning_statistics.setLong(14, Types.NULL);
		}

		
		//stmt_insert_warning_statistics.setLong(15, warningDetail.getLastestProcessedMessageTimeStamp());
		if(warningDetail.getLastestProcessedMessageTimeStamp()!=null) {
			stmt_insert_warning_statistics.setLong(15, warningDetail.getLastestProcessedMessageTimeStamp());
		} else {
			stmt_insert_warning_statistics.setLong(15, Types.NULL);
		}
		
		
		stmt_insert_warning_statistics.setLong(16, warningDetail.getCreatedAt());
		stmt_insert_warning_statistics.setLong(17, Types.NULL);
		if (warningDetail.getMessageType() != null) {
			stmt_insert_warning_statistics.setInt(18, warningDetail.getMessageType());
		} else {
			stmt_insert_warning_statistics.setInt(18, 18);
		}
		System.out.println("warning dao fill statementFinished");

		return stmt_insert_warning_statistics;
	}

	/*
	 * private PreparedStatement fillStatementCommonTripMessage(PreparedStatement
	 * stmt_insert_warning_statistics, WarningStastisticsPojo warningDetail) throws
	 * SQLException { System.out.println("warning dao fill statement");
	 * 
	 * if (warningDetail.getWarningClass() != null)
	 * stmt_insert_warning_statistics.setDouble(4, warningDetail.getWarningClass());
	 * else stmt_insert_warning_statistics.setDouble(4, 0);
	 * 
	 * if (warningDetail.getWarningNumber() != null)
	 * stmt_insert_warning_statistics.setDouble(5,
	 * warningDetail.getWarningNumber()); else
	 * stmt_insert_warning_statistics.setDouble(5, 0);
	 * 
	 * if (warningDetail.getVehicleHealthStatusType() != null)
	 * stmt_insert_warning_statistics.setString(9,
	 * warningDetail.getVehicleHealthStatusType()); else
	 * stmt_insert_warning_statistics.setString(9, "");
	 * 
	 * // stmt_insert_warning_statistics.setString(9, "A");
	 * 
	 * // stmt_insert_warning_statistics.setString(10, "A");
	 * 
	 * if (warningDetail.getDistanceUntilNextService() != null) {
	 * stmt_insert_warning_statistics.setLong(13,
	 * warningDetail.getDistanceUntilNextService()); } else {
	 * stmt_insert_warning_statistics.setLong(13, Types.NULL); }
	 * 
	 * stmt_insert_warning_statistics.setLong(15,
	 * warningDetail.getLastestProcessedMessageTimeStamp());
	 * stmt_insert_warning_statistics.setLong(16, warningDetail.getCreatedAt());
	 * stmt_insert_warning_statistics.setLong(17, Types.NULL); if
	 * (warningDetail.getMessageType() != null) {
	 * stmt_insert_warning_statistics.setInt(18, warningDetail.getMessageType()); }
	 * else { stmt_insert_warning_statistics.setInt(18, 18); }
	 * System.out.println("warning dao fill statementFinished");
	 * 
	 * return stmt_insert_warning_statistics; }
	 */
}
