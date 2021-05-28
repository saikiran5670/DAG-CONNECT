package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.SQLException;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.postgre.bo.Trip;
import net.atos.daf.postgre.util.DafConstants;

public class TripSinkDao implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static Logger logger = LoggerFactory.getLogger(TripSinkDao.class);

	private Connection connection;

	/** SQL statement for insert. */
	private static final String INSERT_STATEMENT = "INSERT INTO tripdetail.trip_statistics( trip_id, vin, start_time_stamp, end_time_stamp, veh_message_distance, etl_gps_distance, idle_duration"
			+ ", average_speed, average_weight, start_odometer, last_odometer, start_position_lattitude, start_position_longitude, end_position_lattitude"
			+ ", end_position_longitude, veh_message_fuel_consumed, etl_gps_fuel_consumed, veh_message_driving_time"
			+ ", etl_gps_driving_time, message_received_timestamp, message_inserted_into_kafka_timestamp, etl_trip_record_insertion_time, message_processed_by_etl_process_timestamp"
			+ ", co2_emission, fuel_consumption, max_speed, average_gross_weight_comb, pto_duration, harsh_brake_duration, heavy_throttle_duration"
			+ ", cruise_control_distance_30_50, cruise_control_distance_50_75, cruise_control_distance_more_than_75"
			+ ", average_traffic_classification, cc_fuel_consumption, v_cruise_control_fuel_consumed_for_cc_fuel_consumption, v_cruise_control_dist_for_cc_fuel_consumption"
			+ ", fuel_consumption_cc_non_active, idling_consumption, dpa_score, driver1_id, driver2_id, etl_gps_trip_time, is_ongoing_trip, msg_gross_weight_combinition, no_of_total_index_message) "
			+ "  VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"
			// + " ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?,
			// ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?,
			// ?,?)"
			+ "  ON CONFLICT (trip_id) "
			+ "  DO UPDATE SET  vin = ?, start_time_stamp = ?, end_time_stamp = ?, veh_message_distance = ?, etl_gps_distance = ?, idle_duration = ?, average_speed = ?"
			+ ", average_weight = ?, start_odometer = ?, last_odometer = ?, start_position_lattitude = ?, start_position_longitude = ?, end_position_lattitude = ?"
			+ ", end_position_longitude = ?, veh_message_fuel_consumed = ?, etl_gps_fuel_consumed = ?"
			+ ", veh_message_driving_time = ?, etl_gps_driving_time = ?,message_received_timestamp = ?, message_inserted_into_kafka_timestamp =?, etl_trip_record_insertion_time = ?"
			+ ", message_processed_by_etl_process_timestamp = ?, co2_emission = ?, fuel_consumption = ?, max_speed = ?, average_gross_weight_comb = ?"
			+ ", pto_duration = ?, harsh_brake_duration = ?, heavy_throttle_duration = ?, cruise_control_distance_30_50 = ?"
			+ ", cruise_control_distance_50_75 = ?, cruise_control_distance_more_than_75 = ?, average_traffic_classification = ?"
			+ ", cc_fuel_consumption = ?, v_cruise_control_fuel_consumed_for_cc_fuel_consumption = ?, v_cruise_control_dist_for_cc_fuel_consumption = ?"
			+ ", fuel_consumption_cc_non_active = ?, idling_consumption = ?, dpa_score = ?, driver1_id = ?, driver2_id = ?, etl_gps_trip_time = ?, is_ongoing_trip = ?"
			+ ", msg_gross_weight_combinition = ?, no_of_total_index_message =? ";

	public boolean insert(Trip dataObject) throws TechnicalException {

		PreparedStatement stmt = null;
		boolean result = false;

		try {

			if (null != dataObject && null != (connection = getConnection())) {

				stmt = connection.prepareStatement(INSERT_STATEMENT);
				stmt = fillStatement(stmt, dataObject);

				stmt.addBatch();
				stmt.executeBatch();
				logger.info("after executeBatch ");
			}
		} catch (SQLException e) {
			logger.info("Sql Issue while inserting data to tripStatistic table : "+e.getMessage());
			e.printStackTrace();
		}catch (Exception e) {
			logger.info("Issue while inserting data to tripStatistic table : "+e.getMessage());
			e.printStackTrace();
		}

		return result;
	}

	private PreparedStatement fillStatement(PreparedStatement statement, Trip rec) throws SQLException, Exception {

		statement.setString(1, rec.getTripId());

		logger.info("Sink TripId : " + rec.getTripId() + " VIN : " + rec.getVin() + " VID : " + rec.getVid());

		if (rec.getVin() != null) {
			statement.setString(2, rec.getVin());
		} else if (rec.getVid() != null) {
			statement.setString(2, rec.getVid());
		} else
			statement.setString(2, DafConstants.UNKNOWN);

		if (rec.getStartDateTime() != null)
			statement.setLong(3, rec.getStartDateTime());
		else
			statement.setLong(3, 0);

		if (rec.getEndDateTime() != null)
			statement.setLong(4, rec.getEndDateTime());
		else
			statement.setLong(4, 0);

		if (rec.getGpsTripDist() != null)
			statement.setInt(5, rec.getGpsTripDist());
		else
			statement.setInt(5, 0);

		if (rec.getTripCalDist() != null)
			statement.setLong(6, rec.getTripCalDist());
		else
			statement.setLong(6, 0);

		if (rec.getVIdleDuration() != null)
			statement.setInt(7, rec.getVIdleDuration());
		else
			statement.setInt(7, 0);

		if (rec.getTripCalAvgSpeed() != null)
			statement.setDouble(8, rec.getTripCalAvgSpeed());
		else
			statement.setDouble(8, 0);

		if (rec.getVGrossWeightCombination() != null)
			statement.setDouble(9, rec.getVGrossWeightCombination());
		else
			statement.setDouble(9, 0);

		if (rec.getGpsStartVehDist() != null)
			statement.setLong(10, rec.getGpsStartVehDist());
		else
			statement.setLong(10, 0);

		if (rec.getGpsStopVehDist() != null)
			statement.setLong(11, rec.getGpsStopVehDist());
		else
			statement.setLong(11, 0);

		if (rec.getGpsStartLatitude() != null)
			statement.setDouble(12, rec.getGpsStartLatitude());
		else
			statement.setDouble(12, 0);

		if (rec.getGpsStartLongitude() != null)
			statement.setDouble(13, rec.getGpsStartLongitude());
		else
			statement.setDouble(13, 0);

		if (rec.getGpsEndLatitude() != null)
			statement.setDouble(14, rec.getGpsEndLatitude());
		else
			statement.setDouble(14, 0);

		if (rec.getGpsEndLongitude() != null)
			statement.setDouble(15, rec.getGpsEndLongitude());
		else
			statement.setDouble(15, 0);

		if (rec.getVUsedFuel() != null)
			statement.setInt(16, rec.getVUsedFuel());
		else
			statement.setInt(16, 0);

		if (rec.getTripCalUsedFuel() != null)
			statement.setLong(17, rec.getTripCalUsedFuel());
		else
			statement.setLong(17, 0);

		if (rec.getVTripMotionDuration() != null)
			statement.setInt(18, rec.getVTripMotionDuration());
		else
			statement.setInt(18, 0);

		if (rec.getTripCalDrivingTm() != null)
			statement.setLong(19, rec.getTripCalDrivingTm());
		else
			statement.setLong(19, 0);

		if (rec.getReceivedTimestamp() != null)
			statement.setLong(20, rec.getReceivedTimestamp());
		else
			statement.setLong(20, 0);

		if (rec.getKafkaProcessingTS() != null)
			statement.setLong(21, rec.getKafkaProcessingTS());
		else
			statement.setLong(21, 0);

		if (rec.getTripProcessingTS() != null)
			statement.setLong(22, rec.getTripProcessingTS());
		else
			statement.setLong(22, 0);

		if (rec.getEtlProcessingTS() != null)
			statement.setLong(23, rec.getEtlProcessingTS());
		else
			statement.setLong(23, 0);
		
		logger.info("CO2Emission==" + rec.getTripCalC02Emission());
		if (rec.getTripCalC02Emission() != null)
			statement.setDouble(24, rec.getTripCalC02Emission());
		else
			statement.setDouble(24, 0);

		if (rec.getTripCalFuelConsumption() != null)
			statement.setDouble(25, rec.getTripCalFuelConsumption());
		else
			statement.setDouble(25, 0);

		if (rec.getVTachographSpeed() != null)
			statement.setDouble(26, rec.getVTachographSpeed());
		else
			statement.setDouble(26, 0);

		if (rec.getTripCalAvgGrossWtComb() != null)
			statement.setDouble(27, rec.getTripCalAvgGrossWtComb());
		else
			statement.setDouble(27, 0);

		if (rec.getTripCalPtoDuration() != null)
			statement.setDouble(28, rec.getTripCalPtoDuration());
		else
			statement.setDouble(28, 0);

		if (rec.getTriCalHarshBrakeDuration() != null)
			statement.setDouble(29, rec.getTriCalHarshBrakeDuration());
		else
			statement.setDouble(29, 0);

		if (rec.getTripCalHeavyThrottleDuration() != null)
			statement.setDouble(30, rec.getTripCalHeavyThrottleDuration());
		else
			statement.setDouble(30, 0);

		if (rec.getTripCalCrsCntrlDistBelow50() != null)
			statement.setDouble(31, rec.getTripCalCrsCntrlDistBelow50());
		else
			statement.setDouble(31, 0);

		if (rec.getTripCalCrsCntrlDistAbv50() != null)
			statement.setDouble(32, rec.getTripCalCrsCntrlDistAbv50());
		else
			statement.setDouble(32, 0);

		if (rec.getTripCalCrsCntrlDistAbv75() != null)
			statement.setDouble(33, rec.getTripCalCrsCntrlDistAbv75());
		else
			statement.setDouble(33, 0);

		if (rec.getTripCalAvgTrafficClsfn() != null)
			statement.setDouble(34, rec.getTripCalAvgTrafficClsfn());
		else
			statement.setDouble(34, 0);

		if (rec.getTripCalCCFuelConsumption() != null)
			statement.setDouble(35, rec.getTripCalCCFuelConsumption());
		else
			statement.setDouble(35, 0);

		if (rec.getVCruiseControlFuelConsumed() != null)
			statement.setInt(36, rec.getVCruiseControlFuelConsumed());
		else
			statement.setInt(36, 0);

		if (rec.getVCruiseControlDist() != null)
			statement.setInt(37, rec.getVCruiseControlDist());
		else
			statement.setInt(37, 0);

		if (rec.getTripCalfuelNonActiveCnsmpt() != null)
			statement.setDouble(38, rec.getTripCalfuelNonActiveCnsmpt());
		else
			statement.setDouble(38, 0);

		if (rec.getVIdleFuelConsumed() != null)
			statement.setInt(39, rec.getVIdleFuelConsumed());
		else
			statement.setInt(39, 0);

		if (rec.getTripCalDpaScore() != null)
			statement.setDouble(40, rec.getTripCalDpaScore());
		else
			statement.setDouble(40, 0);

		statement.setString(41, rec.getDriverId());
		statement.setString(42, rec.getDriver2Id());

		if (rec.getTripCalGpsVehTime() != null)
			statement.setLong(43, rec.getTripCalGpsVehTime());
		else
			statement.setLong(43, 0);

		statement.setBoolean(44, Boolean.FALSE);
		
		if (rec.getVGrossWtSum() != null)
			statement.setDouble(45, rec.getVGrossWtSum());
		else
			statement.setDouble(45, 0);

		if (rec.getNumberOfIndexMessage() != null)
			statement.setInt(46, rec.getNumberOfIndexMessage());
		else
			statement.setInt(46, 0);

		
		if (rec.getVin() != null) {
			statement.setString(47, rec.getVin());
		} else if (rec.getVid() != null) {
			statement.setString(47, rec.getVid());
		} else
			statement.setString(47, DafConstants.UNKNOWN);

		if (rec.getStartDateTime() != null)
			statement.setLong(48, rec.getStartDateTime());
		else
			statement.setLong(48, 0);

		if (rec.getEndDateTime() != null)
			statement.setLong(49, rec.getEndDateTime());
		else
			statement.setLong(49, 0);

		if (rec.getGpsTripDist() != null)
			statement.setInt(50, rec.getGpsTripDist());
		else
			statement.setInt(50, 0);

		if (rec.getTripCalDist() != null)
			statement.setLong(51, rec.getTripCalDist());
		else
			statement.setLong(51, 0);

		if (rec.getVIdleDuration() != null)
			statement.setInt(52, rec.getVIdleDuration());
		else
			statement.setInt(52, 0);

		if (rec.getTripCalAvgSpeed() != null)
			statement.setDouble(53, rec.getTripCalAvgSpeed());
		else
			statement.setDouble(53, 0);

		if (rec.getVGrossWeightCombination() != null)
			statement.setDouble(54, rec.getVGrossWeightCombination());
		else
			statement.setDouble(54, 0);

		if (rec.getGpsStartVehDist() != null)
			statement.setLong(55, rec.getGpsStartVehDist());
		else
			statement.setLong(55, 0);

		if (rec.getGpsStopVehDist() != null)
			statement.setLong(56, rec.getGpsStopVehDist());
		else
			statement.setLong(56, 0);

		if (rec.getGpsStartLatitude() != null)
			statement.setDouble(57, rec.getGpsStartLatitude());
		else
			statement.setDouble(57, 0);

		if (rec.getGpsStartLongitude() != null)
			statement.setDouble(58, rec.getGpsStartLongitude());
		else
			statement.setDouble(58, 0);

		if (rec.getGpsEndLatitude() != null)
			statement.setDouble(59, rec.getGpsEndLatitude());
		else
			statement.setDouble(59, 0);

		if (rec.getGpsEndLongitude() != null)
			statement.setDouble(60, rec.getGpsEndLongitude());
		else
			statement.setDouble(60, 0);

		if (rec.getVUsedFuel() != null)
			statement.setInt(61, rec.getVUsedFuel());
		else
			statement.setInt(61, 0);

		if (rec.getTripCalUsedFuel() != null)
			statement.setLong(62, rec.getTripCalUsedFuel());
		else
			statement.setLong(62, 0);

		if (rec.getVTripMotionDuration() != null)
			statement.setInt(63, rec.getVTripMotionDuration());
		else
			statement.setInt(63, 0);

		if (rec.getTripCalDrivingTm() != null)
			statement.setLong(64, rec.getTripCalDrivingTm());
		else
			statement.setLong(64, 0);

		if (rec.getReceivedTimestamp() != null)
			statement.setLong(65, rec.getReceivedTimestamp());
		else
			statement.setLong(65, 0);

		if (rec.getKafkaProcessingTS() != null)
			statement.setLong(66, rec.getKafkaProcessingTS());
		else
			statement.setLong(66, 0);

		if (rec.getTripProcessingTS() != null)
			statement.setLong(67, rec.getTripProcessingTS());
		else
			statement.setLong(67, 0);

		if (rec.getEtlProcessingTS() != null)
			statement.setLong(68, rec.getEtlProcessingTS());
		else
			statement.setLong(68, 0);

		if (rec.getTripCalC02Emission() != null)
			statement.setDouble(69, rec.getTripCalC02Emission());
		else
			statement.setDouble(69, 0);

		if (rec.getTripCalFuelConsumption() != null)
			statement.setDouble(70, rec.getTripCalFuelConsumption());
		else
			statement.setDouble(70, 0);

		if (rec.getVTachographSpeed() != null)
			statement.setDouble(71, rec.getVTachographSpeed());
		else
			statement.setDouble(71, 0);

		if (rec.getTripCalAvgGrossWtComb() != null)
			statement.setDouble(72, rec.getTripCalAvgGrossWtComb());
		else
			statement.setDouble(72, 0);

		if (rec.getTripCalPtoDuration() != null)
			statement.setDouble(73, rec.getTripCalPtoDuration());
		else
			statement.setDouble(73, 0);

		if (rec.getTriCalHarshBrakeDuration() != null)
			statement.setDouble(74, rec.getTriCalHarshBrakeDuration());
		else
			statement.setDouble(74, 0);

		if (rec.getTripCalHeavyThrottleDuration() != null)
			statement.setDouble(75, rec.getTripCalHeavyThrottleDuration());
		else
			statement.setDouble(75, 0);

		if (rec.getTripCalCrsCntrlDistBelow50() != null)
			statement.setDouble(76, rec.getTripCalCrsCntrlDistBelow50());
		else
			statement.setDouble(76, 0);

		if (rec.getTripCalCrsCntrlDistAbv50() != null)
			statement.setDouble(77, rec.getTripCalCrsCntrlDistAbv50());
		else
			statement.setDouble(77, 0);

		if (rec.getTripCalCrsCntrlDistAbv75() != null)
			statement.setDouble(78, rec.getTripCalCrsCntrlDistAbv75());
		else
			statement.setDouble(78, 0);

		if (rec.getTripCalAvgTrafficClsfn() != null)
			statement.setDouble(79, rec.getTripCalAvgTrafficClsfn());
		else
			statement.setDouble(79, 0);

		if (rec.getTripCalCCFuelConsumption() != null)
			statement.setDouble(80, rec.getTripCalCCFuelConsumption());
		else
			statement.setDouble(80, 0);

		if (rec.getVCruiseControlFuelConsumed() != null)
			statement.setInt(81, rec.getVCruiseControlFuelConsumed());
		else
			statement.setInt(81, 0);

		if (rec.getVCruiseControlDist() != null)
			statement.setInt(82, rec.getVCruiseControlDist());
		else
			statement.setInt(82, 0);

		// if (rec.getTripCalfuelNonActiveCnsmpt() != null)
		statement.setDouble(83, rec.getTripCalfuelNonActiveCnsmpt());
		// else
		// statement.setDouble(81, 0);

		if (rec.getVIdleFuelConsumed() != null)
			statement.setInt(84, rec.getVIdleFuelConsumed());
		else
			statement.setInt(84, 0);

		if (rec.getTripCalDpaScore() != null)
			statement.setDouble(85, rec.getTripCalDpaScore());
		else
			statement.setDouble(85, 0);

		statement.setString(86, rec.getDriverId());
		statement.setString(87, rec.getDriver2Id());

		if (rec.getTripCalGpsVehTime() != null)
			statement.setLong(88, rec.getTripCalGpsVehTime());
		else
			statement.setLong(88, 0);

		statement.setBoolean(89, Boolean.FALSE);
		
		if (rec.getVGrossWtSum() != null)
			statement.setDouble(90, rec.getVGrossWtSum());
		else
			statement.setDouble(90, 0);

		if (rec.getNumberOfIndexMessage() != null)
			statement.setInt(91, rec.getNumberOfIndexMessage());
		else
			statement.setInt(91, 0);

		return statement;
		/*
		 * if(dataObject.getVin() != null){ statement.setString(2,
		 * dataObject.getVin()); }else if(dataObject.getVid() != null){
		 * statement.setString(2, dataObject.getVid()); }else
		 * statement.setString(2, DafConstants.UNKNOWN);
		 * 
		 * 
		 * if (dataObject.getStartDateTime() != null) statement.setLong(3,
		 * dataObject.getStartDateTime()); else statement.setLong(3, 0);
		 * 
		 * 
		 * statement.setLong(4, dataObject.getEndDateTime());
		 * 
		 * 
		 * 
		 * statement.setInt(5, dataObject.getGpsTripDist());
		 * 
		 * 
		 * 
		 * statement.setLong(6, dataObject.getTripCalDist());
		 * 
		 * 
		 * 
		 * statement.setInt(7, dataObject.getVIdleDuration());
		 * 
		 * 
		 * 
		 * statement.setDouble(8, dataObject.getTripCalAvgSpeed());
		 * 
		 * 
		 * statement.setDouble(9, dataObject.getVGrossWeightCombination());
		 * 
		 * 
		 * 
		 * statement.setLong(10, dataObject.getGpsStartVehDist());
		 * 
		 * 
		 * statement.setLong(11, dataObject.getGpsStopVehDist());
		 * 
		 * 
		 * 
		 * statement.setDouble(12, dataObject.getGpsStartLatitude());
		 * 
		 * 
		 * 
		 * statement.setDouble(13, dataObject.getGpsStartLongitude());
		 * 
		 * 
		 * 
		 * statement.setDouble(14, dataObject.getGpsEndLatitude());
		 * 
		 * 
		 * 
		 * statement.setDouble(15, dataObject.getGpsEndLongitude());
		 * 
		 * 
		 * 
		 * statement.setInt(16, dataObject.getVUsedFuel());
		 * 
		 * 
		 * 
		 * statement.setLong(17, dataObject.getTripCalUsedFuel());
		 * 
		 * 
		 * 
		 * statement.setInt(18, dataObject.getVTripMotionDuration());
		 * 
		 * 
		 * 
		 * statement.setLong(19, dataObject.getTripCalDrivingTm());
		 * 
		 * 
		 * statement.setLong(20, dataObject.getReceivedTimestamp());
		 * 
		 * 
		 * 
		 * statement.setLong(21, dataObject.getKafkaProcessingTS());
		 * 
		 * 
		 * 
		 * statement.setLong(22, dataObject.getHbaseInsertionTS());
		 * 
		 * 
		 * 
		 * statement.setLong(23, dataObject.getEtlProcessingTS());
		 * 
		 * 
		 * 
		 * statement.setDouble(24, dataObject.getTripCalC02Emission());
		 * 
		 * 
		 * 
		 * statement.setDouble(25, dataObject.getTripCalFuelConsumption());
		 * 
		 * 
		 * 
		 * statement.setDouble(26, dataObject.getVTachographSpeed());
		 * 
		 * 
		 * 
		 * statement.setDouble(27, dataObject.getTripCalAvgGrossWtComb());
		 * 
		 * 
		 * 
		 * statement.setDouble(28, dataObject.getTripCalPtoDuration());
		 * 
		 * 
		 * 
		 * statement.setDouble(29, dataObject.getTriCalHarshBrakeDuration());
		 * 
		 * 
		 * 
		 * statement.setDouble(30,
		 * dataObject.getTripCalHeavyThrottleDuration());
		 * 
		 * 
		 * 
		 * statement.setDouble(31, dataObject.getTripCalCrsCntrlDistBelow50());
		 * 
		 * 
		 * 
		 * statement.setDouble(32, dataObject.getTripCalCrsCntrlDistAbv50());
		 * 
		 * 
		 * 
		 * statement.setDouble(33, dataObject.getTripCalCrsCntrlDistAbv75());
		 * 
		 * 
		 * 
		 * statement.setDouble(34, dataObject.getTripCalAvgTrafficClsfn());
		 * 
		 * 
		 * 
		 * statement.setDouble(35, dataObject.getTripCalCCFuelConsumption());
		 * 
		 * 
		 * 
		 * statement.setInt(36, dataObject.getVCruiseControlFuelConsumed());
		 * 
		 * 
		 * 
		 * statement.setInt(37, dataObject.getVCruiseControlDist());
		 * 
		 * 
		 * 
		 * statement.setDouble(38, dataObject.getTripCalfuelNonActiveCnsmpt());
		 * 
		 * 
		 * statement.setInt(39, dataObject.getVIdleFuelConsumed());
		 * 
		 * 
		 * 
		 * statement.setDouble(40, dataObject.getTripCalDpaScore());
		 * 
		 * 
		 * statement.setString(41, dataObject.getDriverId());
		 * statement.setString(42, dataObject.getDriver2Id());
		 * 
		 * 
		 * statement.setLong(43, dataObject.getTripCalGpsVehTime());
		 * 
		 * 
		 * statement.setBoolean(44, Boolean.FALSE);
		 * 
		 * if(dataObject.getVin() != null){ statement.setString(45,
		 * dataObject.getVin()); }else if(dataObject.getVid() != null){
		 * statement.setString(45, dataObject.getVid()); }else
		 * statement.setString(45, DafConstants.UNKNOWN);
		 * 
		 * if (dataObject.getStartDateTime() != null) statement.setLong(46,
		 * dataObject.getStartDateTime()); else statement.setLong(46, 0);
		 * 
		 * if (dataObject.getEndDateTime() != null) statement.setLong(47,
		 * dataObject.getEndDateTime()); else statement.setLong(47, 0);
		 * 
		 * 
		 * statement.setInt(48, dataObject.getGpsTripDist());
		 * 
		 * 
		 * 
		 * statement.setLong(49, dataObject.getTripCalDist());
		 * 
		 * 
		 * 
		 * statement.setInt(50, dataObject.getVIdleDuration());
		 * 
		 * 
		 * 
		 * statement.setDouble(51, dataObject.getTripCalAvgSpeed());
		 * 
		 * 
		 * statement.setDouble(52, dataObject.getVGrossWeightCombination());
		 * 
		 * 
		 * 
		 * statement.setLong(53, dataObject.getGpsStartVehDist());
		 * 
		 * 
		 * 
		 * statement.setLong(54, dataObject.getGpsStopVehDist());
		 * 
		 * 
		 * 
		 * statement.setDouble(55, dataObject.getGpsStartLatitude());
		 * 
		 * 
		 * 
		 * statement.setDouble(56, dataObject.getGpsStartLongitude());
		 * 
		 * 
		 * 
		 * statement.setDouble(57, dataObject.getGpsEndLatitude());
		 * 
		 * 
		 * 
		 * statement.setDouble(58, dataObject.getGpsEndLongitude());
		 * 
		 * 
		 * 
		 * statement.setInt(59, dataObject.getVUsedFuel());
		 * 
		 * 
		 * 
		 * statement.setLong(60, dataObject.getTripCalUsedFuel());
		 * 
		 * 
		 * 
		 * statement.setInt(61, dataObject.getVTripMotionDuration());
		 * 
		 * 
		 * 
		 * statement.setLong(62, dataObject.getTripCalDrivingTm());
		 * 
		 * 
		 * 
		 * statement.setLong(63, dataObject.getReceivedTimestamp());
		 * 
		 * 
		 * 
		 * statement.setLong(64, dataObject.getKafkaProcessingTS());
		 * 
		 * 
		 * 
		 * statement.setLong(65, dataObject.getHbaseInsertionTS());
		 * 
		 * 
		 * 
		 * statement.setLong(66, dataObject.getEtlProcessingTS());
		 * 
		 * 
		 * 
		 * statement.setDouble(67, dataObject.getTripCalC02Emission());
		 * 
		 * 
		 * 
		 * statement.setDouble(68, dataObject.getTripCalFuelConsumption());
		 * 
		 * 
		 * 
		 * statement.setDouble(69, dataObject.getVTachographSpeed());
		 * 
		 * 
		 * 
		 * statement.setDouble(70, dataObject.getTripCalAvgGrossWtComb());
		 * 
		 * 
		 * 
		 * statement.setDouble(71, dataObject.getTripCalPtoDuration());
		 * 
		 * 
		 * 
		 * statement.setDouble(72, dataObject.getTriCalHarshBrakeDuration());
		 * 
		 * 
		 * 
		 * statement.setDouble(73,
		 * dataObject.getTripCalHeavyThrottleDuration());
		 * 
		 * 
		 * 
		 * statement.setDouble(74, dataObject.getTripCalCrsCntrlDistBelow50());
		 * 
		 * 
		 * 
		 * statement.setDouble(75, dataObject.getTripCalCrsCntrlDistAbv50());
		 * 
		 * 
		 * 
		 * statement.setDouble(76, dataObject.getTripCalCrsCntrlDistAbv75());
		 * 
		 * 
		 * 
		 * statement.setDouble(77, dataObject.getTripCalAvgTrafficClsfn());
		 * 
		 * 
		 * statement.setDouble(78, dataObject.getTripCalCCFuelConsumption());
		 * 
		 * 
		 * 
		 * statement.setInt(79, dataObject.getVCruiseControlFuelConsumed());
		 * 
		 * 
		 * statement.setInt(80, dataObject.getVCruiseControlDist());
		 * 
		 * 
		 * //if (dataObject.getTripCalfuelNonActiveCnsmpt() != null)
		 * statement.setDouble(81, dataObject.getTripCalfuelNonActiveCnsmpt());
		 * // else // statement.setDouble(81, 0);
		 * 
		 * 
		 * statement.setInt(82, dataObject.getVIdleFuelConsumed());
		 * 
		 * 
		 * 
		 * statement.setDouble(83, dataObject.getTripCalDpaScore());
		 * 
		 * 
		 * statement.setString(84, dataObject.getDriverId());
		 * statement.setString(85, dataObject.getDriver2Id());
		 * 
		 * 
		 * statement.setLong(86, dataObject.getTripCalGpsVehTime());
		 * 
		 * 
		 * statement.setBoolean(87, Boolean.FALSE);
		 * 
		 * System.out.println("Prepared data for trip :: "+dataObject.getTripId(
		 * ));
		 */

	}

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}

}
