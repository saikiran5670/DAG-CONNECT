package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Types;
import java.util.ArrayList;
import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.postgre.bo.IndexTripData;
import net.atos.daf.postgre.bo.WarningStatisticsPojo;
import net.atos.daf.postgre.util.DafConstants;

public class WarningStatisticsDao implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	Logger logger = LoggerFactory.getLogger(WarningStatisticsDao.class);
	private Connection connection;
	/** SQL statement for insert. */

	private static final String LIVEFLEET_WARNING_INSERT = "INSERT INTO livefleet.livefleet_warning_statistics(trip_id , vin   , warning_time_stamp,	warning_class,	warning_number,	latitude,	longitude,	heading,	vehicle_health_status_type,	vehicle_driving_status_type,	driver1_id,	warning_type,	distance_until_next_service,	odometer_val,	lastest_processed_message_time_stamp,	created_at, modified_at,	message_type) VALUES ( ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
	private static final String LIVEFLEET_WARNING_READ = "select warning_time_stamp from livefleet.livefleet_warning_statistics where vin = ? AND message_type=10 order by id DESC limit 1";

	private static final String LIVEFLEET_CURRENT_TRIP_STATISTICS_UPDATE_TEN = "UPDATE livefleet.livefleet_current_trip_statistics  SET latest_received_position_lattitude = ? , latest_received_position_longitude = ? , latest_received_position_heading = ? , latest_processed_message_time_stamp = ? , vehicle_health_status_type = ? , latest_warning_class = ? ,latest_warning_number = ? , latest_warning_type = ? , latest_warning_timestamp = ? , latest_warning_position_latitude = ? , latest_warning_position_longitude = ? WHERE trip_id = ( SELECT trip_id FROM livefleet.livefleet_current_trip_statistics WHERE vin = ? ORDER BY id DESC LIMIT 1 )";

	private static final String REPAITM_MAINTENANCE_WARNING_READ = "select warning_type, warning_time_stamp from livefleet.livefleet_warning_statistics where message_type=? and vin = ? and warning_class = ? and warning_number= ? AND vin IS NOT NULL order by warning_time_stamp DESC limit 1";

	private static final String LIVEFLEET_WARNING_READLIST = "select id, warning_class,	warning_number from livefleet.livefleet_warning_statistics where vin = ? AND  message_type=10 and warning_type='A'  order by id DESC";
	private static final String LIVEFLEET_WARNING_UPDATELIST = "UPDATE livefleet.livefleet_warning_statistics set warning_type='D' where id = ANY (?)";

	/*
	 * private static final String LIVEFLEET_WARNING_STATUS_FOR_CURR_TRIP_STATS =
	 * "select distinct on (trip_id, vin) vehicle_health_status_type, warning_class, warning_number, warning_type, warning_time_stamp, latitude, longitude, trip_id, vin "
	 * + "from livefleet.livefleet_warning_statistics where trip_id=? and vin=? " +
	 * "order by trip_id, vin, warning_time_stamp desc";
	 */

	public void warning_insert(WarningStatisticsPojo warningDetail) throws TechnicalException, SQLException {
		PreparedStatement stmt_insert_warning_statistics=null;

		try {

			if (null != warningDetail && null != (connection = getConnection())) {

				stmt_insert_warning_statistics = connection.prepareStatement(LIVEFLEET_WARNING_INSERT,
						ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);
				stmt_insert_warning_statistics = fillStatement(stmt_insert_warning_statistics, warningDetail);

				stmt_insert_warning_statistics.addBatch();
				stmt_insert_warning_statistics.executeBatch();
				// System.out.println("data inserted in warning table");
				logger.info("warning dao --inserted for message 10--" + warningDetail.getVin());
			}else {
				if(connection == null) {
					logger.error(" Issue -- Warning connection is null : " + connection);
					throw new TechnicalException("Warning connection is null :: ");
				}
		}
		} catch (SQLException e) {
			// System.out.println("inside catch LiveFleetDriverActivityDao Insert");
			logger.error("Error in inside catch LiveFleetWarning Statistics Insert" + e.getMessage());
			logger.error("Error in inside catch LiveFleetWarning Statistics Insert" + stmt_insert_warning_statistics);
			e.printStackTrace();
		} catch (Exception e) {
			logger.error("Error in inside catch LiveFleetWarning Statistics Insert : " + e.getMessage());
			e.printStackTrace();
		}

	}

	public void warningUpdateMessageTenCommonTrip(WarningStatisticsPojo warningDetail)
			throws TechnicalException, SQLException {
		PreparedStatement updateWarningCommonTrip = null;
		// System.out.println("warning dao udate for message ten before try in message
		// 10");
		try {

			if (null != warningDetail && null != (connection = getConnection())) {
				// System.out.println("warning dao udate for message ten");
				logger.info("Warning class update started --vin-{}  time {}",  warningDetail.getVin(), java.time.LocalTime.now());
				updateWarningCommonTrip = connection.prepareStatement(LIVEFLEET_CURRENT_TRIP_STATISTICS_UPDATE_TEN,
						ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);
				// updateWarningCommonTrip = fillStatement(updateWarningCommonTrip,
				// warningDetail);

				updateWarningCommonTrip.setDouble(1, warningDetail.getLatitude());
				updateWarningCommonTrip.setDouble(2, warningDetail.getLongitude());
				updateWarningCommonTrip.setDouble(3, warningDetail.getHeading());
				updateWarningCommonTrip.setDouble(4, warningDetail.getWarningTimeStamp());
				updateWarningCommonTrip.setString(5, warningDetail.getVehicleHealthStatusType());

				// System.out.println( " warningDetail.getWarningClass()
				// ::"+warningDetail.getWarningClass());
				if (warningDetail.getWarningClass() != null)
					updateWarningCommonTrip.setInt(6, warningDetail.getWarningClass());
				else
					updateWarningCommonTrip.setInt(6, 0);

				// System.out.println( " warningDetail.getWarningNumber()
				// ::"+warningDetail.getWarningNumber());

				if (warningDetail.getWarningNumber() != null)
					updateWarningCommonTrip.setInt(7, warningDetail.getWarningNumber());
				else
					updateWarningCommonTrip.setInt(7, 0);

				updateWarningCommonTrip.setString(8, warningDetail.getWarningType());
				updateWarningCommonTrip.setLong(9, warningDetail.getWarningTimeStamp());
				updateWarningCommonTrip.setDouble(10, warningDetail.getLatitude());
				updateWarningCommonTrip.setDouble(11, warningDetail.getLongitude());
				// updateWarningCommonTrip.setString(12,
				// warningDetail.getVehicleDrivingStatusType());
				if (warningDetail.getVin() != null) {
					updateWarningCommonTrip.setString(12, warningDetail.getVin());
				} else {
					updateWarningCommonTrip.setString(12, warningDetail.getVid());
				}

				updateWarningCommonTrip.executeUpdate();
				// System.out.println("warning dao --updated for another table for message 10");
				//logger.info("warning dao --updated for another table for message 10--" + warningDetail.getVin());
				logger.info("Warning class update finished --vin-{}  time {}",  warningDetail.getVin(), java.time.LocalTime.now());
			} else {
				if(connection == null) {
					logger.error(" Issue -- Warning connection is null  while update: " + connection);
					throw new TechnicalException("Warning connection is null while update:: ");
				}
		}
		} catch (SQLException e) {
			logger.error("Sql Issue while updating data in warning statistics table : " + e.getMessage());
			logger.error("Sql Issue while updating data in warning statistics table : " + updateWarningCommonTrip);
			
			e.printStackTrace();
		} catch (Exception e) {
			logger.error("Issue while inserting data in warning statistics table : " + e.getMessage());
			
			e.printStackTrace();
		}

	}

	/*
	 * public WarningStatisticsPojo readLatestWarnStatus(String tripId, String vin)
	 * throws TechnicalException, SQLException {
	 * 
	 * PreparedStatement stmt_read_warn_status_curr_trip = null; ResultSet
	 * rs_warn_status_curr_trip = null; WarningStatisticsPojo latestWarningStatus =
	 * null;
	 * 
	 * try {
	 * 
	 * if (null != tripId && null != (connection = getConnection())) {
	 * stmt_read_warn_status_curr_trip =
	 * connection.prepareStatement(LIVEFLEET_WARNING_STATUS_FOR_CURR_TRIP_STATS,
	 * ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);
	 * if(!tripId.isEmpty() && null != tripId )
	 * stmt_read_warn_status_curr_trip.setString(1, tripId); else
	 * stmt_read_warn_status_curr_trip.setString(1, ""); if(!vin.isEmpty() && null
	 * != vin ) stmt_read_warn_status_curr_trip.setString(2, vin); else
	 * stmt_read_warn_status_curr_trip.setString(2, "");
	 * 
	 * System.out.println("LIVEFLEET_WARNING_STATUS_FOR_CURR_TRIP_STATS query : " +
	 * stmt_read_warn_status_curr_trip); rs_warn_status_curr_trip =
	 * stmt_read_warn_status_curr_trip.executeQuery();
	 * 
	 * System.out.println("rs_warn_status_curr_trip " + rs_warn_status_curr_trip);
	 * 
	 * //vehicle_health_status_type, warning_class, warning_number, warning_type,
	 * warning_time_stamp, latitude, longitude, trip_id, vin while
	 * (rs_warn_status_curr_trip.next()) { latestWarningStatus = new
	 * WarningStatisticsPojo();
	 * latestWarningStatus.setVehicleHealthStatusType(rs_warn_status_curr_trip.
	 * getString("vehicle_health_status_type"));
	 * latestWarningStatus.setWarningClass(rs_warn_status_curr_trip.getInt(
	 * "warning_class"));
	 * latestWarningStatus.setWarningNumber(rs_warn_status_curr_trip.getInt(
	 * "warning_number"));
	 * latestWarningStatus.setWarningType(rs_warn_status_curr_trip.getString(
	 * "warning_type"));
	 * latestWarningStatus.setWarningTimeStamp(rs_warn_status_curr_trip.getLong(
	 * "warning_time_stamp"));
	 * latestWarningStatus.setLatitude(rs_warn_status_curr_trip.getDouble("latitude"
	 * )); latestWarningStatus.setLongitude(rs_warn_status_curr_trip.getDouble(
	 * "longitude"));
	 * latestWarningStatus.setTripId(rs_warn_status_curr_trip.getString("trip_id"));
	 * latestWarningStatus.setVin(rs_warn_status_curr_trip.getString("vin"));
	 * 
	 * } // } System.out.
	 * println("LATEST WARNING STATUS RECEIVED from LIVEFLEET_WARNING_STATUS FOR tripId = "
	 * + tripId + " is " + latestWarningStatus.toString());
	 * 
	 * rs_warn_status_curr_trip.close();
	 * 
	 * }
	 * 
	 * } catch (SQLException e) {
	 * logger.error("Issue while reading latest warning status for tripId : " +
	 * tripId); e.printStackTrace();
	 * 
	 * } catch (Exception e) {
	 * logger.error("Issue while reading latest warning status for tripId : " +
	 * tripId); e.printStackTrace(); } finally {
	 * 
	 * if (null != rs_warn_status_curr_trip) {
	 * 
	 * try { rs_warn_status_curr_trip.close(); } catch (SQLException ignore) {
	 *//** ignore any errors here *//*
									 * 
									 * } } }
									 * 
									 * return latestWarningStatus;
									 * 
									 * }
									 */

	public Long read(Integer messageType, String vin) throws TechnicalException, SQLException {

		PreparedStatement stmt_read_warning_statistics = null;
		ResultSet rs_position = null;
		Long lastestProcessedMessageTimeStamp = null;
		logger.info("Warning class read started --vin-{}  time {}",  vin, java.time.LocalTime.now());
		try {

			if (null != messageType && null != (connection = getConnection())) {

				stmt_read_warning_statistics = connection.prepareStatement(LIVEFLEET_WARNING_READ);

				stmt_read_warning_statistics.setString(1, vin);
				// stmt_read_warning_statistics.setInt(2, messageType);

				rs_position = stmt_read_warning_statistics.executeQuery();

				while (rs_position.next()) {
					lastestProcessedMessageTimeStamp = rs_position.getLong("warning_time_stamp");
				}

				rs_position.close();
			}

		} catch (SQLException e) {
			logger.error("Error in Warning statistics read method : " + e.getMessage());
			e.printStackTrace();
		} catch (Exception e) {
			logger.error("Error in Warning statistics read method : " + e.getMessage());
			e.printStackTrace();
		} finally {

			if (null != rs_position) {

				try {
					rs_position.close();
				} catch (SQLException ignore) {
					/** ignore any errors here */
				}
			}
		}
		logger.info("Warning class read done.before return--vin-{}  time {}",  vin, java.time.LocalTime.now());
		return lastestProcessedMessageTimeStamp;

	}

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}

	private PreparedStatement fillStatement(PreparedStatement stmt_insert_warning_statistics,
			WarningStatisticsPojo warningDetail) throws SQLException {
		// System.out.println("warning dao fill statement");

		if (warningDetail.getTripId() != null)
			stmt_insert_warning_statistics.setString(1, warningDetail.getTripId());
		else
			stmt_insert_warning_statistics.setString(1, "");

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
			stmt_insert_warning_statistics.setString(9, String.valueOf(warningDetail.getVehicleHealthStatusType()));
		else
			stmt_insert_warning_statistics.setString(9, String.valueOf(Character.valueOf(' ')));

		if (warningDetail.getVehicleDrivingStatusType() != null)
			stmt_insert_warning_statistics.setString(10, warningDetail.getVehicleDrivingStatusType());
		else
			stmt_insert_warning_statistics.setString(10, "");

		// stmt_insert_warning_statistics.setString(9, "A");

		// stmt_insert_warning_statistics.setString(10, "A");

		if (warningDetail.getDriverID() != null) {
			stmt_insert_warning_statistics.setString(11, warningDetail.getDriverID());
		} else {
			stmt_insert_warning_statistics.setString(11, "");
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

		// stmt_insert_warning_statistics.setLong(15,
		// warningDetail.getLastestProcessedMessageTimeStamp());
		// System.out.println("last processed time stamp--" +
		// warningDetail.getLastestProcessedMessageTimeStamp());
		if (warningDetail.getLastestProcessedMessageTimeStamp() != null) {
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
		// System.out.println("warning dao fill statementFinished");

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

	public Long readRepairMaintenamce(Integer messageType, String vin, Integer warningClass, Integer warningNumber)
			throws TechnicalException, SQLException {

		PreparedStatement stmt_read_warning_statistics = null;
		ResultSet rs_position = null;
		Long lastestProcessedMessageTimeStamp = null;
		String warningType = null;

		try {

			if (null != messageType && null != (connection = getConnection())) {
				
				logger.info("Warning class read for repairMaintenance started --vin-{}  time {}",  vin, java.time.LocalTime.now());

				stmt_read_warning_statistics = connection.prepareStatement(REPAITM_MAINTENANCE_WARNING_READ);

				stmt_read_warning_statistics.setInt(1, messageType);
				stmt_read_warning_statistics.setString(2, vin);

				stmt_read_warning_statistics.setInt(3, warningClass);
				stmt_read_warning_statistics.setInt(4, warningNumber);

				rs_position = stmt_read_warning_statistics.executeQuery();
				// System.out.println("query :" + stmt_read_warning_statistics);

				while (rs_position.next()) {

					warningType = rs_position.getString("warning_type");
					if (warningType.equals("A")) {
						lastestProcessedMessageTimeStamp = rs_position.getLong("warning_time_stamp");
					}
				}

				rs_position.close();
			}

		} catch (SQLException e) {
			logger.error("Error in Warning statistics read method : " + e.getMessage());
			e.printStackTrace();
		} catch (Exception e) {
			logger.error("Error in Warning statistics read method : " + e.getMessage());
			e.printStackTrace();
		} finally {

			if (null != rs_position) {

				try {
					rs_position.close();
				} catch (SQLException ignore) {
					/** ignore any errors here */
				}
			}
		}
		logger.info("Warning class read finished, before return --vin-{}  time {}",  vin, java.time.LocalTime.now());
		return lastestProcessedMessageTimeStamp;

	}

	public List<WarningStatisticsPojo> readReturnListofActiveMsg(Integer messageType, String vin)
			throws TechnicalException, SQLException {

		PreparedStatement stmt_read_warning_statistics = null;
		ResultSet rs_position = null;
		// Long lastestProcessedMessageTimeStamp = null;

		List<WarningStatisticsPojo> warningActiveList = new ArrayList<>();

		try {

			if (null != messageType && null != (connection = getConnection())) {
				logger.info("Warning class read list for active messages started --vin-{}  time {}",  vin, java.time.LocalTime.now());
				stmt_read_warning_statistics = connection.prepareStatement(LIVEFLEET_WARNING_READLIST);

				stmt_read_warning_statistics.setString(1, vin);

				rs_position = stmt_read_warning_statistics.executeQuery();
				logger.info("readed list : " + stmt_read_warning_statistics);

				/*
				 * if(rs_position.getFetchSize()>0) { logger.info("size of list : " +
				 * rs_position.getFetchSize()); warningActiveList= new ArrayList<>(); }
				 */

				while (rs_position.next()) {
					warningActiveList.add(map(rs_position));
					//logger.info("warning list is ready ");

					// rs_position.getInt(0)

					// lastestProcessedMessageTimeStamp = rs_position.getLong("warning_time_stamp");
					logger.info("Warning class reading list finished for active messages  --vin-{}  time {}, listSize {}",  vin, java.time.LocalTime.now(), warningActiveList.size());
					
				}

				rs_position.close();
			}

		} catch (SQLException e) {
			logger.error("Error in Warning statistics read method : " + stmt_read_warning_statistics);
			e.printStackTrace();
		} catch (Exception e) {
			logger.error("Error in Warning statistics read method : " + e.getMessage());
			e.printStackTrace();
		} finally {

			if (null != rs_position) {

				try {
					rs_position.close();
				} catch (SQLException ignore) {
					/** ignore any errors here */
				}
			}
		}
		
		return warningActiveList;

	}

	private WarningStatisticsPojo map(ResultSet resultSet) throws SQLException {
		WarningStatisticsPojo warningData = new WarningStatisticsPojo();
		try {

			//logger.info("inside map function--: ");
			warningData.setVin(resultSet.getString("vin"));
			warningData.setId(resultSet.getInt("id"));
			warningData.setWarningClass(resultSet.getInt("warning_class"));
			warningData.setWarningNumber(resultSet.getInt("warning_number"));
			warningData.setWarningType(resultSet.getString("warning_type"));
			//logger.info("warningDataObject is ready to return: ");
		} catch (Exception e) {
			logger.error("Error in map method : " + e.getMessage());
		}
		// id

		return warningData;

	}

	public void DeactivatWarningUpdate(List<Integer> warningList) throws TechnicalException, SQLException {
		PreparedStatement updateWarningCommonTrip = null;

		try {

			if (null != warningList && !warningList.isEmpty() && null != (connection = getConnection())) {
				logger.info("Warning class update started for list readed --listsize-{}  time {}",  warningList.size(), java.time.LocalTime.now());
				
				updateWarningCommonTrip = connection.prepareStatement(LIVEFLEET_WARNING_UPDATELIST,
						ResultSet.TYPE_SCROLL_SENSITIVE, ResultSet.CONCUR_UPDATABLE);

				updateWarningCommonTrip.setArray(1, connection.createArrayOf("integer", warningList.toArray()));

				logger.info("query prepared for deactivate update " + updateWarningCommonTrip);
				updateWarningCommonTrip.executeUpdate();
				logger.info("Warning class update finished for list readed --listsize-{}  time {}",  warningList.size(), java.time.LocalTime.now());
				
				//logger.info("update executed for 63 " + updateWarningCommonTrip);

			}
		} catch (SQLException e) {
			logger.error("Sql Issue while updating list in warning statistics : " + updateWarningCommonTrip);

			e.printStackTrace();
		} catch (Exception e) {
			logger.error("Issue while inserting updating list in warning statistics  : " + e.getMessage());
			// System.out.println("sql-exception in update for message 10" +
			// e.getMessage());
			e.printStackTrace();
		}

	}
}
