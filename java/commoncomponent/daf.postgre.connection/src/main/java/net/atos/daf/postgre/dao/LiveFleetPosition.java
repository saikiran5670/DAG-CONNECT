package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.postgre.bo.Co2Master;
import net.atos.daf.postgre.bo.LiveFleetPojo;
import net.atos.daf.postgre.util.DafConstants;

public class LiveFleetPosition implements Serializable {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private Connection connection;

	private static final String READ_LIVEFLEET_POSITION = "SELECT * from livefleet.livefleet_position_statistics WHERE vin = ? and trip_id=? ORDER BY created_at_m2m DESC limit 1";
	private static final String INSERT_LIVEFLEET_POSITION = "INSERT INTO livefleet.livefleet_position_statistics ( trip_id    , vin    ,message_time_stamp    ,gps_altitude    ,gps_heading    ,gps_latitude    ,gps_longitude    ,co2_emission    ,fuel_consumption    , last_odometer_val  ,distance_until_next_service    , created_at_m2m    ,created_at_kafka    ,created_at_dm , veh_message_type,  vehicle_msg_trigger_type_id, created_datetime,received_datetime,gps_speed,gps_datetime,wheelbased_speed,tachgraph_speed,driver1_id, vehicle_msg_trigger_additional_info, driver_auth_equipment_type_id, card_replacement_index, oem_driver_id_type, oem_driver_id, pto_id, telltale_id, oem_telltale , telltale_state_id, driving_time) VALUES (?,	?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,? ,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,? ,?  )";
		public boolean insert(LiveFleetPojo currentPosition)
			throws TechnicalException, SQLException {
		PreparedStatement stmt_insert_livefleet_position;
		System.out.println("Inside insert of Trip");
		boolean result = false;
	
		try {

			if (null != currentPosition && null != (connection = getConnection())) {
				stmt_insert_livefleet_position = connection.prepareStatement(INSERT_LIVEFLEET_POSITION);

				stmt_insert_livefleet_position = fillStatement(stmt_insert_livefleet_position, currentPosition);
				
				stmt_insert_livefleet_position.addBatch();
				stmt_insert_livefleet_position.executeBatch();
			}
		} catch (SQLException e) {
						e.printStackTrace();
		}
		System.out.println("Inside insert of Trip End");
		return result;
	}

	public LiveFleetPojo read(String vin, String tripId) throws TechnicalException, SQLException {

		PreparedStatement stmtReadLivefleetPosition = null;
		ResultSet rs_position = null;
		LiveFleetPojo previousRecordInfo=null;
		
		try {

			if (null != vin && null != (connection = getConnection())) {
				previousRecordInfo= new LiveFleetPojo();

				stmtReadLivefleetPosition = connection.prepareStatement(READ_LIVEFLEET_POSITION);
				stmtReadLivefleetPosition.setString(1, vin);
				stmtReadLivefleetPosition.setString(2, tripId);

				rs_position = stmtReadLivefleetPosition.executeQuery();
				
				while (rs_position.next()) {
					
					previousRecordInfo.setDrivingTime(rs_position.getInt("driving_time"));
					previousRecordInfo.setMessageTimestamp(rs_position.getDouble("message_time_stamp"));
					System.out.println("driving Time inside read--" + rs_position.getInt("driving_time"));
				}
				
				rs_position.close();
			}

		} catch (SQLException e) {
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

		return previousRecordInfo;

	}

	private PreparedStatement fillStatement(PreparedStatement stmt_insert_livefleet_position,
			LiveFleetPojo currentPosition) throws SQLException {
		
		if (currentPosition.getTripId() != null)
			stmt_insert_livefleet_position.setString(1, currentPosition.getTripId());
		else
			stmt_insert_livefleet_position.setString(1, DafConstants.UNKNOWN);

		if (currentPosition.getVin() != null)
			stmt_insert_livefleet_position.setString(2, currentPosition.getVin());
		else
			stmt_insert_livefleet_position.setString(2, currentPosition.getVid());

		if (currentPosition.getMessageTimestamp() != null)
			stmt_insert_livefleet_position.setDouble(3, currentPosition.getMessageTimestamp());
		else
			stmt_insert_livefleet_position.setDouble(3, 0);

		if (currentPosition.getGpsAltitude() != null)
			stmt_insert_livefleet_position.setDouble(4, currentPosition.getGpsAltitude());
		else
			stmt_insert_livefleet_position.setDouble(4, 0);

		if (currentPosition.getGpsHeading() != null)
			stmt_insert_livefleet_position.setDouble(5, currentPosition.getGpsHeading());
		else
			stmt_insert_livefleet_position.setDouble(5, 0);

		if (currentPosition.getGpsLatitude() != null)
			stmt_insert_livefleet_position.setDouble(6, currentPosition.getGpsLatitude());
		else
			stmt_insert_livefleet_position.setDouble(6, 0);

		if (currentPosition.getGpsLongitude() != null)
			stmt_insert_livefleet_position.setDouble(7, currentPosition.getGpsLongitude());
		else
			stmt_insert_livefleet_position.setDouble(7, 0);

		if (currentPosition.getCo2Emission() != null)
			stmt_insert_livefleet_position.setDouble(8, currentPosition.getCo2Emission());
		else
			stmt_insert_livefleet_position.setDouble(8, 0);

		if (currentPosition.getFuelConsumption() != null)
			stmt_insert_livefleet_position.setDouble(9, currentPosition.getFuelConsumption());
		else
			stmt_insert_livefleet_position.setDouble(9, 0);

		if (currentPosition.getLastOdometerValue() != null)
			stmt_insert_livefleet_position.setDouble(10, currentPosition.getLastOdometerValue());
		else
			stmt_insert_livefleet_position.setDouble(10, 0);

		if (currentPosition.getDistUntilNextService() != null)
			stmt_insert_livefleet_position.setDouble(11, currentPosition.getDistUntilNextService());
		else
			stmt_insert_livefleet_position.setDouble(11, 0);

		if (currentPosition.getCreated_at_m2m() != null)
			stmt_insert_livefleet_position.setDouble(12, currentPosition.getCreated_at_m2m());
		else
			stmt_insert_livefleet_position.setDouble(12, 0);

		if (currentPosition.getCreated_at_kafka() != null)
			stmt_insert_livefleet_position.setDouble(13, currentPosition.getCreated_at_kafka());
		else
			stmt_insert_livefleet_position.setDouble(13, 0);

		if (currentPosition.getCreated_at_kafka() != null)
			stmt_insert_livefleet_position.setDouble(14, currentPosition.getCreated_at_dm());
		else
			stmt_insert_livefleet_position.setDouble(14, 0);
		
		stmt_insert_livefleet_position.setString(15, "I");
		
		if (currentPosition.getVehicleMsgTriggerTypeId() != null)
			stmt_insert_livefleet_position.setInt(16, currentPosition.getVehicleMsgTriggerTypeId());
		else
			stmt_insert_livefleet_position.setInt(16, 0);
		
		if (currentPosition.getCreatedDatetime() != null)
			stmt_insert_livefleet_position.setLong(17, currentPosition.getCreatedDatetime());
		else
			stmt_insert_livefleet_position.setLong(17, 0);
	
					
		if (currentPosition.getReceivedDatetime() != null)
			stmt_insert_livefleet_position.setLong(18, currentPosition.getReceivedDatetime());
		else
			stmt_insert_livefleet_position.setLong(18, 0);
		
		if (currentPosition.getGpsSpeed() != null)
			stmt_insert_livefleet_position.setDouble(19, currentPosition.getGpsSpeed());
		else
			stmt_insert_livefleet_position.setDouble(19, 0);
		
		if (currentPosition.getGpsDatetime() != null)
			stmt_insert_livefleet_position.setLong(20, currentPosition.getGpsDatetime());
		else
			stmt_insert_livefleet_position.setLong(20, 0);
		
		if (currentPosition.getWheelbasedSpeed() != null)
			stmt_insert_livefleet_position.setDouble(21, currentPosition.getWheelbasedSpeed());
		else
			stmt_insert_livefleet_position.setDouble(21, 0);
		
		if (currentPosition.getTachgraphSpeed() != null)
			stmt_insert_livefleet_position.setDouble(22, currentPosition.getTachgraphSpeed());
		else
			stmt_insert_livefleet_position.setDouble(22, 0);
		
		if (currentPosition.getDriver1Id() != null)
			stmt_insert_livefleet_position.setString(23, currentPosition.getDriver1Id());
		else
			stmt_insert_livefleet_position.setString(23, "");
		
		if (currentPosition.getVehicleMsgTriggerAdditionalInfo() != null)
			stmt_insert_livefleet_position.setString(24, currentPosition.getVehicleMsgTriggerAdditionalInfo());
		else
			stmt_insert_livefleet_position.setString(24, "");
		
		if (currentPosition.getDriverAuthEquipmentTypeId() != null)
			stmt_insert_livefleet_position.setInt(25, currentPosition.getDriverAuthEquipmentTypeId());
		else
			stmt_insert_livefleet_position.setInt(25, 0);
		
		if (currentPosition.getCardReplacementIndex() != null)
			stmt_insert_livefleet_position.setString(26, currentPosition.getCardReplacementIndex());
		else
			stmt_insert_livefleet_position.setString(26, "");
		
		if (currentPosition.getOem_driver_id_type() != null)
			stmt_insert_livefleet_position.setString(27, currentPosition.getOem_driver_id_type());
		else
			stmt_insert_livefleet_position.setString(27, "");
		
		if (currentPosition.getOem_driver_id() != null)
			stmt_insert_livefleet_position.setString(28, currentPosition.getOem_driver_id());
		else
			stmt_insert_livefleet_position.setString(28, "");
		
		if (currentPosition.getPto_id() != null)
			stmt_insert_livefleet_position.setString(29, currentPosition.getPto_id());
		else
			stmt_insert_livefleet_position.setString(29, "");
		
		if (currentPosition.getTelltale_id() != null)
			stmt_insert_livefleet_position.setInt(30, currentPosition.getTelltale_id());
		else
			stmt_insert_livefleet_position.setInt(30, 0);
		
		if (currentPosition.getOem_telltale() != null)
			stmt_insert_livefleet_position.setString(31, currentPosition.getOem_telltale());
		else
			stmt_insert_livefleet_position.setString(31, "");
		
		if (currentPosition.getTelltale_state_id() != null)
			stmt_insert_livefleet_position.setInt(32, currentPosition.getTelltale_state_id());
		else
			stmt_insert_livefleet_position.setInt(32, 0);
		
		if (currentPosition.getDrivingTime() != null)
			stmt_insert_livefleet_position.setInt(33, currentPosition.getDrivingTime());
		else
			stmt_insert_livefleet_position.setInt(33, 0);
		
	
		System.out.println("Inside fillstatement End");
		return stmt_insert_livefleet_position;
	}

	
	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}

}
