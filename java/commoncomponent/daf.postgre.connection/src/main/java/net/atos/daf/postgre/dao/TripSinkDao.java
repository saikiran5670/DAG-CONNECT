package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Array;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.SQLException;
import java.sql.Types;

import org.postgresql.util.PGobject;
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

	public void insert(Trip dataObject, PreparedStatement tripInsertQry) throws TechnicalException {
		try {
			if (null != dataObject && null != (connection = getConnection())) {

				tripInsertQry = fillStatement(tripInsertQry, dataObject, connection);
				tripInsertQry.addBatch();
				tripInsertQry.executeBatch();
				
			} else {
				if (connection == null) {
					logger.error(" Issue trip connection is null : " + connection);
					throw new TechnicalException("Trip Datamart connection is null :: ");
				}
			}
		} catch (SQLException e) {
			logger.error("Sql Issue while inserting data to tripStatistic table : " + e.getMessage());
			logger.error("Issue while inserting trip record :: " + tripInsertQry);
			e.printStackTrace();
		} catch (Exception e) {
			logger.error("Issue while inserting data to tripStatistic table : " + e.getMessage());
			logger.error("Issue while inserting trip record :: " + tripInsertQry);
			e.printStackTrace();
		}

	}

	private PreparedStatement fillStatement(PreparedStatement statement, Trip rec, Connection connection) throws SQLException, Exception {

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
			statement.setLong(5, rec.getGpsTripDist());
		else
			statement.setLong(5, 0);

		if (rec.getTripCalDist() != null)
			statement.setLong(6, rec.getTripCalDist());
		else
			statement.setLong(6, 0);

		if (rec.getVIdleDuration() != null)
			statement.setLong(7, rec.getVIdleDuration());
		else
			statement.setLong(7, 0);

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
			statement.setLong(16, rec.getVUsedFuel());
		else
			statement.setLong(16, 0);

		if (rec.getTripCalUsedFuel() != null)
			statement.setLong(17, rec.getTripCalUsedFuel());
		else
			statement.setLong(17, 0);

		if (rec.getVTripMotionDuration() != null)
			statement.setLong(18, rec.getVTripMotionDuration());
		else
			statement.setLong(18, 0);

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

		if (rec.getTripCalHarshBrakeDuration() != null)
			statement.setDouble(29, rec.getTripCalHarshBrakeDuration());
		else
			statement.setDouble(29, 0);

		if (rec.getTripCalHeavyThrottleDuration() != null)
			statement.setDouble(30, rec.getTripCalHeavyThrottleDuration());
		else
			statement.setDouble(30, 0);

		if (rec.getTripCalCrsCntrlDist25To50() != null)
			statement.setLong(31, rec.getTripCalCrsCntrlDist25To50());
		else
			statement.setLong(31, 0);

		if (rec.getTripCalCrsCntrlDist50To75() != null)
			statement.setLong(32, rec.getTripCalCrsCntrlDist50To75());
		else
			statement.setLong(32, 0);

		if (rec.getTripCalCrsCntrlDistAbv75() != null)
			statement.setLong(33, rec.getTripCalCrsCntrlDistAbv75());
		else
			statement.setLong(33, 0);

		if (rec.getTripCalAvgTrafficClsfn() != null)
			statement.setDouble(34, rec.getTripCalAvgTrafficClsfn());
		else
			statement.setDouble(34, 0);

		if (rec.getTripCalCCFuelConsumption() != null)
			statement.setDouble(35, rec.getTripCalCCFuelConsumption());
		else
			statement.setDouble(35, 0);

		if (rec.getVCruiseControlFuelConsumed() != null)
			statement.setLong(36, rec.getVCruiseControlFuelConsumed());
		else
			statement.setLong(36, 0);

		if (rec.getVCruiseControlDist() != null)
			statement.setLong(37, rec.getVCruiseControlDist());
		else
			statement.setLong(37, 0);

		if (rec.getTripCalfuelNonActiveCnsmpt() != null)
			statement.setDouble(38, rec.getTripCalfuelNonActiveCnsmpt());
		else
			statement.setDouble(38, 0);

		if (rec.getVIdleFuelConsumed() != null)
			statement.setLong(39, rec.getVIdleFuelConsumed());
		else
			statement.setLong(39, 0);

		if (rec.getTripCalDpaScore() != null)
			statement.setDouble(40, rec.getTripCalDpaScore());
		else
			statement.setDouble(40, 0);

		if(rec.getDriverId() != null)
			statement.setString(41, rec.getDriverId());
		else
			statement.setString(41, DafConstants.UNKNOWN);
		
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
			statement.setLong(46, rec.getNumberOfIndexMessage());
		else
			statement.setLong(46, 0);
		

		if (rec.getVPTODuration() != null)
			statement.setLong(47, rec.getVPTODuration());
		else
			statement.setLong(47, 0);
		

		if (rec.getVHarshBrakeDuration() != null)
			statement.setLong(48, rec.getVHarshBrakeDuration());
		else
			statement.setLong(48, 0);
		

		if (rec.getVBrakeDuration() != null)
			statement.setLong(49, rec.getVBrakeDuration());
		else
			statement.setInt(49, 0);
		
		if (rec.getVMaxThrottlePaddleDuration() != null)
			statement.setLong(50, rec.getVMaxThrottlePaddleDuration());
		else
			statement.setLong(50, 0);
		
		if (rec.getVTripAccelerationTime() != null)
			statement.setLong(51, rec.getVTripAccelerationTime());
		else
			statement.setLong(51, 0);
		
		if (rec.getVTripDPABrakingCount() != null)
			statement.setLong(52, rec.getVTripDPABrakingCount());
		else
			statement.setLong(52, 0);
		
		if (rec.getVTripDPAAnticipationCount() != null)
			statement.setLong(53, rec.getVTripDPAAnticipationCount());
		else
			statement.setInt(53, 0);
				
		if (rec.getVSumTripDPABrakingScore() != null)
			statement.setLong(54, rec.getVSumTripDPABrakingScore());
		else
			statement.setLong(54, 0);


		if (rec.getVSumTripDPAAnticipationScore() != null)
			statement.setLong(55, rec.getVSumTripDPAAnticipationScore());
		else
			statement.setInt(55, 0);

		if (rec.getVTripIdleWithoutPTODuration() != null)
			statement.setLong(56, rec.getVTripIdleWithoutPTODuration());
		else
			statement.setLong(56, 0);
		
		if (rec.getVTripIdlePTODuration() != null)
			statement.setLong(57, rec.getVTripIdlePTODuration());
		else
			statement.setLong(57, 0);
		
		PGobject jsonObject = new PGobject();
		jsonObject.setType("json");
		
		//statement.setString(58, rec.getRpmTorque());
		jsonObject.setValue(rec.getRpmTorque());
		statement.setObject(58, jsonObject);
		
		if (rec.getAbsRpmTorque() != null)
			statement.setLong(59, rec.getAbsRpmTorque());
		else
			statement.setLong(59, Types.NULL);
		
		if (rec.getOrdRpmTorque() != null)
			statement.setLong(60, rec.getOrdRpmTorque());
		else
			statement.setLong(60, Types.NULL);
		
		Array nonZeroRpmTorqueMatrixArray =null;
		Array numValRpmTorqueArray =null;
		Array clmnIdnxRpmTorqueArray =null;
		
		if(rec.getNonZeroRpmTorqueMatrix() != null)
			nonZeroRpmTorqueMatrixArray = connection.createArrayOf("BIGINT", rec.getNonZeroRpmTorqueMatrix());
		
		statement.setArray(61, nonZeroRpmTorqueMatrixArray);
		
		if(rec.getNumValRpmTorque() != null)
			numValRpmTorqueArray = connection.createArrayOf("BIGINT", rec.getNumValRpmTorque());
		
		statement.setArray(62, numValRpmTorqueArray);
		
		if(rec.getClmnIdnxRpmTorque() != null)
			clmnIdnxRpmTorqueArray = connection.createArrayOf("BIGINT", rec.getClmnIdnxRpmTorque());
		
		statement.setArray(63, clmnIdnxRpmTorqueArray);
				
		jsonObject.setValue(rec.getRpmSpeed());
		statement.setObject(64, jsonObject);
		
		if (rec.getAbsRpmSpeed() != null)
			statement.setLong(65, rec.getAbsRpmSpeed());
		else
			statement.setLong(65, Types.NULL);
		
		if (rec.getOrdRpmSpeed() != null)
			statement.setLong(66, rec.getOrdRpmSpeed());
		else
			statement.setLong(66, Types.NULL);
		
		Array nonZeroRpmSpeedMatrixArray =null;
		Array numValRpmSpeedArray =null;
		Array clmnIdnxRpmSpeedArray =null;
		
		if(rec.getNonZeroRpmSpeedMatrix() != null)
			nonZeroRpmSpeedMatrixArray = connection.createArrayOf("BIGINT", rec.getNonZeroRpmSpeedMatrix());
		
		statement.setArray(67, nonZeroRpmSpeedMatrixArray);
		
		if(rec.getNumValRpmSpeed() != null)
			numValRpmSpeedArray = connection.createArrayOf("BIGINT", rec.getNumValRpmSpeed());
		
		statement.setArray(68, numValRpmSpeedArray);
		
		if(rec.getClmnIdnxRpmSpeed() != null)
			clmnIdnxRpmSpeedArray = connection.createArrayOf("BIGINT", rec.getClmnIdnxRpmSpeed());
		
		statement.setArray(69, clmnIdnxRpmSpeedArray);
				
		//statement.setString(70, rec.getAclnSpeed());
		jsonObject.setValue(rec.getAclnSpeed());
		statement.setObject(70, jsonObject);
		
		if (rec.getAbsAclnSpeed() != null)
			statement.setLong(71, rec.getAbsAclnSpeed());
		else
			statement.setLong(71, Types.NULL);
		
		if (rec.getOrdAclnSpeed() != null)
			statement.setLong(72, rec.getOrdAclnSpeed());
		else
			statement.setLong(72, Types.NULL);
		
		Array nonZeroAclnSpeedMatrixArray =null;
		Array numValAclnSpeedArray =null;
		Array clmnIdnxAclnSpeedArray =null;
		Array nonZeroBrakePedalAclnSpeedMatrix = null;
		
		if(rec.getNonZeroAclnSpeedMatrix() != null)
			nonZeroAclnSpeedMatrixArray = connection.createArrayOf("BIGINT", rec.getNonZeroAclnSpeedMatrix());
		
		if(rec.getNonZeroBrakePedalAclnSpeedMatrix() != null)
			nonZeroBrakePedalAclnSpeedMatrix = connection.createArrayOf("BIGINT", rec.getNonZeroBrakePedalAclnSpeedMatrix());
		
		
		statement.setArray(73, nonZeroAclnSpeedMatrixArray);
		statement.setArray(74, nonZeroBrakePedalAclnSpeedMatrix);
		
		if(rec.getNumValAclnSpeed() != null)
			numValAclnSpeedArray = connection.createArrayOf("BIGINT", rec.getNumValAclnSpeed());
		
		statement.setArray(75, numValAclnSpeedArray);
		
		if(rec.getClmnIdnxAclnSpeed() != null)
			clmnIdnxAclnSpeedArray = connection.createArrayOf("BIGINT", rec.getClmnIdnxAclnSpeed());
		
		statement.setArray(76, clmnIdnxAclnSpeedArray);
		
		
		if (rec.getVin() != null) {
			statement.setString(77, rec.getVin());
		} else if (rec.getVid() != null) {
			statement.setString(77, rec.getVid());
		} else
			statement.setString(77, DafConstants.UNKNOWN);

		if (rec.getStartDateTime() != null)
			statement.setLong(78, rec.getStartDateTime());
		else
			statement.setLong(78, 0);

		if (rec.getEndDateTime() != null)
			statement.setLong(79, rec.getEndDateTime());
		else
			statement.setLong(79, 0);

		if (rec.getGpsTripDist() != null)
			statement.setLong(80, rec.getGpsTripDist());
		else
			statement.setLong(80, 0);

		if (rec.getTripCalDist() != null)
			statement.setLong(81, rec.getTripCalDist());
		else
			statement.setLong(81, 0);

		if (rec.getVIdleDuration() != null)
			statement.setLong(82, rec.getVIdleDuration());
		else
			statement.setLong(82, 0);

		if (rec.getTripCalAvgSpeed() != null)
			statement.setDouble(83, rec.getTripCalAvgSpeed());
		else
			statement.setDouble(83, 0);

		if (rec.getVGrossWeightCombination() != null)
			statement.setDouble(84, rec.getVGrossWeightCombination());
		else
			statement.setDouble(84, 0);

		if (rec.getGpsStartVehDist() != null)
			statement.setLong(85, rec.getGpsStartVehDist());
		else
			statement.setLong(85, 0);

		if (rec.getGpsStopVehDist() != null)
			statement.setLong(86, rec.getGpsStopVehDist());
		else
			statement.setLong(86, 0);

		if (rec.getGpsStartLatitude() != null)
			statement.setDouble(87, rec.getGpsStartLatitude());
		else
			statement.setDouble(87, 0);

		if (rec.getGpsStartLongitude() != null)
			statement.setDouble(88, rec.getGpsStartLongitude());
		else
			statement.setDouble(88, 0);

		if (rec.getGpsEndLatitude() != null)
			statement.setDouble(89, rec.getGpsEndLatitude());
		else
			statement.setDouble(89, 0);

		if (rec.getGpsEndLongitude() != null)
			statement.setDouble(90, rec.getGpsEndLongitude());
		else
			statement.setDouble(90, 0);

		if (rec.getVUsedFuel() != null)
			statement.setLong(91, rec.getVUsedFuel());
		else
			statement.setLong(91, 0);

		if (rec.getTripCalUsedFuel() != null)
			statement.setLong(92, rec.getTripCalUsedFuel());
		else
			statement.setLong(92, 0);

		if (rec.getVTripMotionDuration() != null)
			statement.setLong(93, rec.getVTripMotionDuration());
		else
			statement.setLong(93, 0);

		if (rec.getTripCalDrivingTm() != null)
			statement.setLong(94, rec.getTripCalDrivingTm());
		else
			statement.setLong(94, 0);

		if (rec.getReceivedTimestamp() != null)
			statement.setLong(95, rec.getReceivedTimestamp());
		else
			statement.setLong(95, 0);

		if (rec.getKafkaProcessingTS() != null)
			statement.setLong(96, rec.getKafkaProcessingTS());
		else
			statement.setLong(96, 0);

		if (rec.getTripProcessingTS() != null)
			statement.setLong(97, rec.getTripProcessingTS());
		else
			statement.setLong(97, 0);

		if (rec.getEtlProcessingTS() != null)
			statement.setLong(98, rec.getEtlProcessingTS());
		else
			statement.setLong(98, 0);

		if (rec.getTripCalC02Emission() != null)
			statement.setDouble(99, rec.getTripCalC02Emission());
		else
			statement.setDouble(99, 0);

		if (rec.getTripCalFuelConsumption() != null)
			statement.setDouble(100, rec.getTripCalFuelConsumption());
		else
			statement.setDouble(100, 0);

		if (rec.getVTachographSpeed() != null)
			statement.setDouble(101, rec.getVTachographSpeed());
		else
			statement.setDouble(101, 0);

		if (rec.getTripCalAvgGrossWtComb() != null)
			statement.setDouble(102, rec.getTripCalAvgGrossWtComb());
		else
			statement.setDouble(102, 0);

		if (rec.getTripCalPtoDuration() != null)
			statement.setDouble(103, rec.getTripCalPtoDuration());
		else
			statement.setDouble(103, 0);

		if (rec.getTripCalHarshBrakeDuration() != null)
			statement.setDouble(104, rec.getTripCalHarshBrakeDuration());
		else
			statement.setDouble(104, 0);

		if (rec.getTripCalHeavyThrottleDuration() != null)
			statement.setDouble(105, rec.getTripCalHeavyThrottleDuration());
		else
			statement.setDouble(105, 0);

		if (rec.getTripCalCrsCntrlDist25To50() != null)
			statement.setLong(106, rec.getTripCalCrsCntrlDist25To50());
		else
			statement.setLong(106, 0);

		if (rec.getTripCalCrsCntrlDist50To75() != null)
			statement.setLong(107, rec.getTripCalCrsCntrlDist50To75());
		else
			statement.setLong(107, 0);

		if (rec.getTripCalCrsCntrlDistAbv75() != null)
			statement.setLong(108, rec.getTripCalCrsCntrlDistAbv75());
		else
			statement.setLong(108, 0);

		if (rec.getTripCalAvgTrafficClsfn() != null)
			statement.setDouble(109, rec.getTripCalAvgTrafficClsfn());
		else
			statement.setDouble(109, 0);

		if (rec.getTripCalCCFuelConsumption() != null)
			statement.setDouble(110, rec.getTripCalCCFuelConsumption());
		else
			statement.setDouble(110, 0);

		if (rec.getVCruiseControlFuelConsumed() != null)
			statement.setLong(111, rec.getVCruiseControlFuelConsumed());
		else
			statement.setLong(111, 0);

		if (rec.getVCruiseControlDist() != null)
			statement.setLong(112, rec.getVCruiseControlDist());
		else
			statement.setLong(112, 0);

		if (rec.getTripCalfuelNonActiveCnsmpt() != null)
			statement.setDouble(113, rec.getTripCalfuelNonActiveCnsmpt());
		else
			statement.setDouble(113, 0);

		if (rec.getVIdleFuelConsumed() != null)
			statement.setLong(114, rec.getVIdleFuelConsumed());
		else
			statement.setLong(114, 0);

		if (rec.getTripCalDpaScore() != null)
			statement.setDouble(115, rec.getTripCalDpaScore());
		else
			statement.setDouble(115, 0);

		if(rec.getDriverId() != null)
			statement.setString(116, rec.getDriverId());
		else
			statement.setString(116, DafConstants.UNKNOWN);
		
		statement.setString(117, rec.getDriver2Id());

		if (rec.getTripCalGpsVehTime() != null)
			statement.setLong(118, rec.getTripCalGpsVehTime());
		else
			statement.setLong(118, 0);

		statement.setBoolean(119, Boolean.FALSE);

		if (rec.getVGrossWtSum() != null)
			statement.setDouble(120, rec.getVGrossWtSum());
		else
			statement.setDouble(120, 0);

		if (rec.getNumberOfIndexMessage() != null)
			statement.setLong(121, rec.getNumberOfIndexMessage());
		else
			statement.setInt(121, 0);
		
		if (rec.getVPTODuration() != null)
			statement.setLong(122, rec.getVPTODuration());
		else
			statement.setLong(122, 0);
	
		if (rec.getVHarshBrakeDuration() != null)
			statement.setLong(123, rec.getVHarshBrakeDuration());
		else
			statement.setLong(123, 0);
	
		if (rec.getVBrakeDuration() != null)
			statement.setLong(124, rec.getVBrakeDuration());
		else
			statement.setLong(124, 0);
		
		if (rec.getVMaxThrottlePaddleDuration() != null)
			statement.setLong(125, rec.getVMaxThrottlePaddleDuration());
		else
			statement.setLong(125, 0);
		
		if (rec.getVTripAccelerationTime() != null)
			statement.setLong(126, rec.getVTripAccelerationTime());
		else
			statement.setLong(126, 0);
		
		if (rec.getVTripDPABrakingCount() != null)
			statement.setLong(127, rec.getVTripDPABrakingCount());
		else
			statement.setLong(127, 0);
		
		if (rec.getVTripDPAAnticipationCount() != null)
			statement.setLong(128, rec.getVTripDPAAnticipationCount());
		else
			statement.setLong(128, 0);
				
		if (rec.getVSumTripDPABrakingScore() != null)
			statement.setLong(129, rec.getVSumTripDPABrakingScore());
		else
			statement.setInt(129, 0);

		if (rec.getVSumTripDPAAnticipationScore() != null)
			statement.setLong(130, rec.getVSumTripDPAAnticipationScore());
		else
			statement.setLong(130, 0);

		if (rec.getVTripIdleWithoutPTODuration() != null)
			statement.setLong(131, rec.getVTripIdleWithoutPTODuration());
		else
			statement.setLong(131, 0);
		
		if (rec.getVTripIdlePTODuration() != null)
			statement.setLong(132, rec.getVTripIdlePTODuration());
		else
			statement.setLong(132, 0);
		
		jsonObject.setValue(rec.getRpmTorque()); 
		statement.setObject(133, jsonObject);
		
		if (rec.getAbsRpmTorque() != null)
			statement.setLong(134, rec.getAbsRpmTorque());
		else
			statement.setLong(134, Types.NULL);
		
		if (rec.getOrdRpmTorque() != null)
			statement.setLong(135, rec.getOrdRpmTorque());
		else
			statement.setLong(135, Types.NULL);
		
		
		if(rec.getNonZeroRpmTorqueMatrix() != null)
			nonZeroRpmTorqueMatrixArray = connection.createArrayOf("BIGINT", rec.getNonZeroRpmTorqueMatrix());
		
		statement.setArray(136, nonZeroRpmTorqueMatrixArray);
		
		if(rec.getNumValRpmTorque() != null)
			numValRpmTorqueArray = connection.createArrayOf("BIGINT", rec.getNumValRpmTorque());
		
		statement.setArray(137, numValRpmTorqueArray);
		
		if(rec.getClmnIdnxRpmTorque() != null)
			clmnIdnxRpmTorqueArray = connection.createArrayOf("BIGINT", rec.getClmnIdnxRpmTorque());
		
		statement.setArray(138, clmnIdnxRpmTorqueArray);
				
		jsonObject.setValue(rec.getRpmSpeed());
		statement.setObject(139, jsonObject);
		
		if (rec.getAbsRpmSpeed() != null)
			statement.setLong(140, rec.getAbsRpmSpeed());
		else
			statement.setLong(140, Types.NULL);
		
		if (rec.getOrdRpmSpeed() != null)
			statement.setLong(141, rec.getOrdRpmSpeed());
		else
			statement.setLong(141, Types.NULL);
		
		
		if(rec.getNonZeroRpmSpeedMatrix() != null)
			nonZeroRpmSpeedMatrixArray = connection.createArrayOf("BIGINT", rec.getNonZeroRpmSpeedMatrix());
		
		statement.setArray(142, nonZeroRpmSpeedMatrixArray);
		
		if(rec.getNumValRpmSpeed() != null)
			numValRpmSpeedArray = connection.createArrayOf("BIGINT", rec.getNumValRpmSpeed());
		
		statement.setArray(143, numValRpmTorqueArray);
		
		if(rec.getClmnIdnxRpmSpeed() != null)
			clmnIdnxRpmSpeedArray = connection.createArrayOf("BIGINT", rec.getClmnIdnxRpmSpeed());
		
		statement.setArray(144, clmnIdnxRpmTorqueArray);
				
		jsonObject.setValue(rec.getAclnSpeed()); 
		statement.setObject(145, jsonObject);
		
		if (rec.getAbsAclnSpeed() != null)
			statement.setLong(146, rec.getAbsAclnSpeed());
		else
			statement.setLong(146, Types.NULL);
		
		if (rec.getOrdAclnSpeed() != null)
			statement.setLong(147, rec.getOrdAclnSpeed());
		else
			statement.setLong(147, Types.NULL);
		
		if(rec.getNonZeroAclnSpeedMatrix() != null)
			nonZeroAclnSpeedMatrixArray = connection.createArrayOf("BIGINT", rec.getNonZeroAclnSpeedMatrix());
		
		if(rec.getNonZeroBrakePedalAclnSpeedMatrix() != null)
			nonZeroBrakePedalAclnSpeedMatrix = connection.createArrayOf("BIGINT", rec.getNonZeroBrakePedalAclnSpeedMatrix());
		
		
		statement.setArray(148, nonZeroAclnSpeedMatrixArray);
		statement.setArray(149, nonZeroBrakePedalAclnSpeedMatrix);
		
		if(rec.getNumValAclnSpeed() != null)
			numValAclnSpeedArray = connection.createArrayOf("BIGINT", rec.getNumValAclnSpeed());
		
		statement.setArray(150, numValAclnSpeedArray);
		
		if(rec.getClmnIdnxAclnSpeed() != null)
			clmnIdnxAclnSpeedArray = connection.createArrayOf("BIGINT", rec.getClmnIdnxAclnSpeed());
		
		statement.setArray(151, clmnIdnxAclnSpeedArray);
		

		return statement;

	}

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}

}
