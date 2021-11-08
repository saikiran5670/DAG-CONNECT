package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.postgre.bo.Co2Master;
import net.atos.daf.postgre.bo.LiveFleetPojo;
import net.atos.daf.postgre.util.DafConstants;

public class LiveFleetPosition implements Serializable {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	Logger logger = LoggerFactory.getLogger(LiveFleetPosition.class);

	private Connection connection;

	private static final String READ_LIVEFLEET_POSITION = "SELECT * from livefleet.livefleet_position_statistics WHERE vin = ? and trip_id=? ORDER BY created_at_m2m DESC limit 1";
	private static final String INSERT_LIVEFLEET_POSITION = "INSERT INTO livefleet.livefleet_position_statistics ( trip_id    , vin    ,message_time_stamp    ,gps_altitude    ,gps_heading    ,gps_latitude    ,gps_longitude    ,co2_emission    ,fuel_consumption    , last_odometer_val  ,distance_until_next_service    , created_at_m2m    ,created_at_kafka    ,created_at_dm , veh_message_type,  vehicle_msg_trigger_type_id, created_datetime,received_datetime,gps_speed,gps_datetime,wheelbased_speed,tachgraph_speed,driver1_id, vehicle_msg_trigger_additional_info, driver_auth_equipment_type_id, card_replacement_index, oem_driver_id_type, oem_driver_id, pto_id, telltale_id, oem_telltale , telltale_state_id, driving_time, "
			+ "total_vehicle_distance, total_engine_hours, total_engine_fuel_used, gross_combination_vehicle_weight, engine_speed, fuel_level1, catalyst_fuel_level, driver2_id, driver1_working_state, driver2_working_state, driver2_auth_equipment_type_id, driver2_card_replacement_index, oem_driver2_id_type, oem_driver2_id, ambient_air_temperature, engine_coolant_temperature, service_brake_air_pressure_circuit1, service_brake_air_pressure_circuit2 "
			+ ") VALUES (?,	?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,? ,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,? ,? , ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?  )";
		
	public boolean insert(LiveFleetPojo currentPosition)
			throws TechnicalException, SQLException {
		PreparedStatement stmt_insert_livefleet_position= null;;
		//System.out.println("Inside insert of Trip");
		boolean result = false;
	
		try {

			if (null != currentPosition && null != (connection = getConnection())) {
				stmt_insert_livefleet_position = connection.prepareStatement(INSERT_LIVEFLEET_POSITION);

				stmt_insert_livefleet_position = fillStatement(stmt_insert_livefleet_position, currentPosition);
				
				stmt_insert_livefleet_position.addBatch();
				stmt_insert_livefleet_position.executeBatch();
			} else {
				if(connection == null) {
					logger.error(" Issue in Position Statistics connection is null : " + connection);
					throw new TechnicalException("Position Statistics connection is null :: ");
				}
		} 
		} catch (SQLException e) {
			logger.error("Error in Live fleet position insert method" + e.getMessage());
			logger.error("Error in Live fleet position insert method" + stmt_insert_livefleet_position);
			e.printStackTrace();
		} catch (Exception e) {
			logger.error("Error in Live fleet position insert method" + e.getMessage());
			logger.error("Error in Live fleet position insert method" + stmt_insert_livefleet_position);
			e.printStackTrace();
		}
		
		return result;
	}

	public LiveFleetPojo read(String vin, String tripId) throws TechnicalException, SQLException {
//TODO----log start time along with trip id & vin
		PreparedStatement stmtReadLivefleetPosition = null;
		ResultSet rs_position = null;
		LiveFleetPojo previousRecordInfo=null;
		
		
		try {
			
			if (null != vin && null != (connection = getConnection())) {
				
				stmtReadLivefleetPosition = connection.prepareStatement(READ_LIVEFLEET_POSITION);
				stmtReadLivefleetPosition.setString(1, vin);
				stmtReadLivefleetPosition.setString(2, tripId);

				rs_position = stmtReadLivefleetPosition.executeQuery();
				
				while (rs_position.next()) {
					previousRecordInfo= new LiveFleetPojo();
					previousRecordInfo.setDrivingTime(rs_position.getInt("driving_time"));
					previousRecordInfo.setMessageTimestamp(rs_position.getDouble("message_time_stamp"));
					//System.out.println("driving Time inside read--" + rs_position.getInt("driving_time"));
				}
				
				rs_position.close();
			}

		} catch (SQLException e) {
			logger.error("Error in Live fleet position read method" + e.getMessage());
			
			e.printStackTrace();
		} finally {

			if (null != rs_position) {

				try {
					rs_position.close();
				} catch (SQLException ignore) {
					/** ignore any errors here */
					logger.error("Error in Livefleet finally close");
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

		if (currentPosition.getTotal_vehicle_distance() != null)
			stmt_insert_livefleet_position.setLong(34, currentPosition.getTotal_vehicle_distance());
		else
			stmt_insert_livefleet_position.setLong(34, 0);
		
		if (currentPosition.getTotal_engine_hours() != null)
			stmt_insert_livefleet_position.setLong(35, currentPosition.getTotal_engine_hours());
		else
			stmt_insert_livefleet_position.setLong(35, 0);
		
		if (currentPosition.getTotal_engine_fuel_used() != null)
			stmt_insert_livefleet_position.setLong(36, currentPosition.getTotal_engine_fuel_used());
		else
			stmt_insert_livefleet_position.setLong(36, 0);
		
		if (currentPosition.getGross_combination_vehicle_weight() != null)
			stmt_insert_livefleet_position.setLong(37, currentPosition.getGross_combination_vehicle_weight());
		else
			stmt_insert_livefleet_position.setLong(37, 0);
		
		if (currentPosition.getEngine_speed() != null)
			stmt_insert_livefleet_position.setLong(38, currentPosition.getEngine_speed());
		else
			stmt_insert_livefleet_position.setLong(38, 0);
		
		if (currentPosition.getFuel_level1() != null)
			stmt_insert_livefleet_position.setDouble(39, currentPosition.getFuel_level1());
		else
			stmt_insert_livefleet_position.setDouble(39, 0);
		
		if (currentPosition.getCatalyst_fuel_level() != null)
			stmt_insert_livefleet_position.setInt(40, currentPosition.getCatalyst_fuel_level());
		else
			stmt_insert_livefleet_position.setInt(40, 0);
		
		if (currentPosition.getDriver2_id() != null)
			stmt_insert_livefleet_position.setString(41, currentPosition.getDriver2_id());
		else
			stmt_insert_livefleet_position.setString(41, "");
		
		if (currentPosition.getDriver1_working_state() != null)
			stmt_insert_livefleet_position.setInt(42, currentPosition.getDriver1_working_state());
		else
			stmt_insert_livefleet_position.setInt(42, 0);
		
		if (currentPosition.getDriver2_working_state() != null)
			stmt_insert_livefleet_position.setInt(43, currentPosition.getDriver2_working_state());
		else
			stmt_insert_livefleet_position.setInt(43, 0);
		
		if (currentPosition.getDriver2_auth_equipment_type_id() != null)
			stmt_insert_livefleet_position.setInt(44, currentPosition.getDriver2_auth_equipment_type_id());
		else
			stmt_insert_livefleet_position.setInt(44, 0);
		
		if (currentPosition.getDriver2_card_replacement_index() != null)
			stmt_insert_livefleet_position.setString(45, currentPosition.getDriver2_card_replacement_index());
		else
			stmt_insert_livefleet_position.setString(45, "");
		
		if (currentPosition.getOem_driver2_id_type() != null)
			stmt_insert_livefleet_position.setString(46, currentPosition.getOem_driver2_id_type());
		else
			stmt_insert_livefleet_position.setString(46, "");
		
		if (currentPosition.getOem_driver2_id() != null)
			stmt_insert_livefleet_position.setString(47, currentPosition.getOem_driver2_id());
		else
			stmt_insert_livefleet_position.setString(47, "");
		
		if (currentPosition.getAmbient_air_temperature() != null)
			stmt_insert_livefleet_position.setLong(48, currentPosition.getAmbient_air_temperature());
		else
			stmt_insert_livefleet_position.setLong(48, 0);
		
		if (currentPosition.getEngine_coolant_temperature() != null)
			stmt_insert_livefleet_position.setInt(49, currentPosition.getEngine_coolant_temperature());
		else
			stmt_insert_livefleet_position.setInt(49, 0);
		
		if (currentPosition.getService_brake_air_pressure_circuit1() != null)
			stmt_insert_livefleet_position.setLong(50, currentPosition.getService_brake_air_pressure_circuit1());
		else
			stmt_insert_livefleet_position.setLong(50, 0);
		
		if (currentPosition.getService_brake_air_pressure_circuit2() != null)
			stmt_insert_livefleet_position.setLong(51, currentPosition.getService_brake_air_pressure_circuit2());
		else
			stmt_insert_livefleet_position.setLong(51, 0);
		
	
		//System.out.println("Inside fillstatement End");
		logger.info("Inside fillstatement End :{}");
		return stmt_insert_livefleet_position;
	}

	
	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}

}
