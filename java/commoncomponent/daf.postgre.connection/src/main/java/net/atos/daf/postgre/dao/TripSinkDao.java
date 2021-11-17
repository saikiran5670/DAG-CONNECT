package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Array;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.SQLException;
import java.sql.Types;
import java.util.Objects;

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

	public void insert(Trip dataObject, PreparedStatement tripInsertQry) throws TechnicalException, Exception {
		try {
			if(Objects.nonNull(dataObject)) {

				tripInsertQry = fillStatement(tripInsertQry, dataObject, connection);
				tripInsertQry.execute();
			} 
		} catch (SQLException e) {
			logger.error("Sql Issue while inserting data to tripStatistic table ::{} ", e.getMessage());
			logger.error("Issue while inserting trip record :: {}", tripInsertQry);
			throw e;
			//throw an error to abort the Job
			
		} catch (Exception e) {
			logger.error("Issue while inserting data to tripStatistic table ::{} ", e.getMessage());
			logger.error("Issue while inserting trip record ::{} ", tripInsertQry);
			e.printStackTrace();
		}

	}

	private PreparedStatement fillStatement(PreparedStatement statement, Trip rec, Connection connection) throws SQLException, Exception {

		statement.setString(1, rec.getTripId());
		
		if(Objects.nonNull(rec.getVin())) {
			statement.setString(2, rec.getVin());
		} else if(Objects.nonNull(rec.getVid())) {
			statement.setString(2, rec.getVid());
		} else
			statement.setString(2, DafConstants.UNKNOWN);

		if(Objects.nonNull(rec.getStartDateTime()))
			statement.setLong(3, rec.getStartDateTime());
		else
			statement.setLong(3, 0);

		if(Objects.nonNull(rec.getEndDateTime()))
			statement.setLong(4, rec.getEndDateTime());
		else
			statement.setLong(4, 0);

		if(Objects.nonNull(rec.getGpsTripDist()))
			statement.setLong(5, rec.getGpsTripDist());
		else
			statement.setLong(5, 0);

		if(Objects.nonNull(rec.getTripCalDist()))
			statement.setLong(6, rec.getTripCalDist());
		else
			statement.setLong(6, 0);

		if(Objects.nonNull(rec.getVIdleDuration()))
			statement.setLong(7, rec.getVIdleDuration());
		else
			statement.setLong(7, 0);

		if(Objects.nonNull(rec.getTripCalAvgSpeed()))
			statement.setDouble(8, rec.getTripCalAvgSpeed());
		else
			statement.setDouble(8, 0);

		if(Objects.nonNull(rec.getVGrossWeightCombination()))
			statement.setDouble(9, rec.getVGrossWeightCombination());
		else
			statement.setDouble(9, 0);

		if(Objects.nonNull(rec.getGpsStartVehDist()))
			statement.setLong(10, rec.getGpsStartVehDist());
		else
			statement.setLong(10, 0);

		if(Objects.nonNull(rec.getGpsStopVehDist()))
			statement.setLong(11, rec.getGpsStopVehDist());
		else
			statement.setLong(11, 0);

		if(Objects.nonNull(rec.getGpsStartLatitude()))
			statement.setDouble(12, rec.getGpsStartLatitude());
		else
			statement.setDouble(12, 0);

		if(Objects.nonNull(rec.getGpsStartLongitude()))
			statement.setDouble(13, rec.getGpsStartLongitude());
		else
			statement.setDouble(13, 0);

		if(Objects.nonNull(rec.getGpsEndLatitude()))
			statement.setDouble(14, rec.getGpsEndLatitude());
		else
			statement.setDouble(14, 0);

		if(Objects.nonNull(rec.getGpsEndLongitude()))
			statement.setDouble(15, rec.getGpsEndLongitude());
		else
			statement.setDouble(15, 0);

		if(Objects.nonNull(rec.getVUsedFuel()))
			statement.setLong(16, rec.getVUsedFuel());
		else
			statement.setLong(16, 0);

		if(Objects.nonNull(rec.getTripCalUsedFuel()))
			statement.setLong(17, rec.getTripCalUsedFuel());
		else
			statement.setLong(17, 0);

		if(Objects.nonNull(rec.getVTripMotionDuration()))
			statement.setLong(18, rec.getVTripMotionDuration());
		else
			statement.setLong(18, 0);

		if(Objects.nonNull(rec.getTripCalDrivingTm()))
			statement.setLong(19, rec.getTripCalDrivingTm());
		else
			statement.setLong(19, 0);

		if(Objects.nonNull(rec.getReceivedTimestamp()))
			statement.setLong(20, rec.getReceivedTimestamp());
		else
			statement.setLong(20, 0);

		if(Objects.nonNull(rec.getKafkaProcessingTS()))
			statement.setLong(21, rec.getKafkaProcessingTS());
		else
			statement.setLong(21, 0);

		if(Objects.nonNull(rec.getTripProcessingTS()))
			statement.setLong(22, rec.getTripProcessingTS());
		else
			statement.setLong(22, 0);

		if(Objects.nonNull(rec.getEtlProcessingTS()))
			statement.setLong(23, rec.getEtlProcessingTS());
		else
			statement.setLong(23, 0);

		if(Objects.nonNull(rec.getTripCalC02Emission()))
			statement.setDouble(24, rec.getTripCalC02Emission());
		else
			statement.setDouble(24, 0);

		if(Objects.nonNull(rec.getTripCalFuelConsumption()))
			statement.setDouble(25, rec.getTripCalFuelConsumption());
		else
			statement.setDouble(25, 0);

		if(Objects.nonNull(rec.getVTachographSpeed()))
			statement.setDouble(26, rec.getVTachographSpeed());
		else
			statement.setDouble(26, 0);

		if(Objects.nonNull(rec.getTripCalAvgGrossWtComb()))
			statement.setDouble(27, rec.getTripCalAvgGrossWtComb());
		else
			statement.setDouble(27, 0);

		if(Objects.nonNull(rec.getTripCalPtoDuration()))
			statement.setDouble(28, rec.getTripCalPtoDuration());
		else
			statement.setDouble(28, 0);

		if(Objects.nonNull(rec.getTripCalHarshBrakeDuration()))
			statement.setDouble(29, rec.getTripCalHarshBrakeDuration());
		else
			statement.setDouble(29, 0);

		if(Objects.nonNull(rec.getTripCalHeavyThrottleDuration()))
			statement.setDouble(30, rec.getTripCalHeavyThrottleDuration());
		else
			statement.setDouble(30, 0);

		if(Objects.nonNull(rec.getTripCalCrsCntrlDist25To50()))
			statement.setLong(31, rec.getTripCalCrsCntrlDist25To50());
		else
			statement.setLong(31, 0);

		if(Objects.nonNull(rec.getTripCalCrsCntrlDist50To75()))
			statement.setLong(32, rec.getTripCalCrsCntrlDist50To75());
		else
			statement.setLong(32, 0);

		if(Objects.nonNull(rec.getTripCalCrsCntrlDistAbv75()))
			statement.setLong(33, rec.getTripCalCrsCntrlDistAbv75());
		else
			statement.setLong(33, 0);

		if(Objects.nonNull(rec.getTripCalAvgTrafficClsfn()))
			statement.setDouble(34, rec.getTripCalAvgTrafficClsfn());
		else
			statement.setDouble(34, 0);

		if(Objects.nonNull(rec.getTripCalCCFuelConsumption()))
			statement.setDouble(35, rec.getTripCalCCFuelConsumption());
		else
			statement.setDouble(35, 0);

		if(Objects.nonNull(rec.getVCruiseControlFuelConsumed()))
			statement.setLong(36, rec.getVCruiseControlFuelConsumed());
		else
			statement.setLong(36, 0);

		if(Objects.nonNull(rec.getVCruiseControlDist()))
			statement.setLong(37, rec.getVCruiseControlDist());
		else
			statement.setLong(37, 0);

		if(Objects.nonNull(rec.getTripCalfuelNonActiveCnsmpt()))
			statement.setDouble(38, rec.getTripCalfuelNonActiveCnsmpt());
		else
			statement.setDouble(38, 0);

		if(Objects.nonNull(rec.getVIdleFuelConsumed()))
			statement.setLong(39, rec.getVIdleFuelConsumed());
		else
			statement.setLong(39, 0);

		if(Objects.nonNull(rec.getTripCalDpaScore()))
			statement.setDouble(40, rec.getTripCalDpaScore());
		else
			statement.setDouble(40, 0);

		if(Objects.nonNull(rec.getDriverId()) && !(DafConstants.BLANK).equals(rec.getDriverId()))
			statement.setString(41, rec.getDriverId());
		else
			statement.setString(41, DafConstants.UNKNOWN_CASE_VAL);
		
		if(Objects.nonNull(rec.getDriver2Id()) && !(DafConstants.BLANK).equals(rec.getDriver2Id()))
			statement.setString(42, rec.getDriver2Id());
		else
			statement.setString(42, DafConstants.UNKNOWN_CASE_VAL);
		

		if(Objects.nonNull(rec.getTripCalGpsVehTime()))
			statement.setLong(43, rec.getTripCalGpsVehTime());
		else
			statement.setLong(43, 0);

		statement.setBoolean(44, Boolean.FALSE);

		if(Objects.nonNull(rec.getVGrossWtSum()))
			statement.setDouble(45, rec.getVGrossWtSum());
		else
			statement.setDouble(45, 0);

		if(Objects.nonNull(rec.getNumberOfIndexMessage()))
			statement.setLong(46, rec.getNumberOfIndexMessage());
		else
			statement.setLong(46, 0);
		

		if(Objects.nonNull(rec.getVPTODuration()))
			statement.setLong(47, rec.getVPTODuration());
		else
			statement.setLong(47, 0);
		

		if(Objects.nonNull(rec.getVHarshBrakeDuration()))
			statement.setLong(48, rec.getVHarshBrakeDuration());
		else
			statement.setLong(48, 0);
		

		if(Objects.nonNull(rec.getVBrakeDuration()))
			statement.setLong(49, rec.getVBrakeDuration());
		else
			statement.setInt(49, 0);
		
		if(Objects.nonNull(rec.getVMaxThrottlePaddleDuration()))
			statement.setLong(50, rec.getVMaxThrottlePaddleDuration());
		else
			statement.setLong(50, 0);
		
		if(Objects.nonNull(rec.getVTripAccelerationTime()))
			statement.setLong(51, rec.getVTripAccelerationTime());
		else
			statement.setLong(51, 0);
		
		if(Objects.nonNull(rec.getVTripDPABrakingCount()))
			statement.setLong(52, rec.getVTripDPABrakingCount());
		else
			statement.setLong(52, 0);
		
		if(Objects.nonNull(rec.getVTripDPAAnticipationCount()))
			statement.setLong(53, rec.getVTripDPAAnticipationCount());
		else
			statement.setInt(53, 0);
				
		if(Objects.nonNull(rec.getVSumTripDPABrakingScore()))
			statement.setLong(54, rec.getVSumTripDPABrakingScore());
		else
			statement.setLong(54, 0);


		if(Objects.nonNull(rec.getVSumTripDPAAnticipationScore()))
			statement.setLong(55, rec.getVSumTripDPAAnticipationScore());
		else
			statement.setInt(55, 0);

		if(Objects.nonNull(rec.getVTripIdleWithoutPTODuration()))
			statement.setLong(56, rec.getVTripIdleWithoutPTODuration());
		else
			statement.setLong(56, 0);
		
		if(Objects.nonNull(rec.getVTripIdlePTODuration()))
			statement.setLong(57, rec.getVTripIdlePTODuration());
		else
			statement.setLong(57, 0);
		
		PGobject jsonObject = new PGobject();
		jsonObject.setType("json");
		
		//statement.setString(58, rec.getRpmTorque());
		jsonObject.setValue(rec.getRpmTorque());
		statement.setObject(58, jsonObject);
		
		if(Objects.nonNull(rec.getAbsRpmTorque()))
			statement.setLong(59, rec.getAbsRpmTorque());
		else
			statement.setLong(59, Types.NULL);
		
		if(Objects.nonNull(rec.getOrdRpmTorque()))
			statement.setLong(60, rec.getOrdRpmTorque());
		else
			statement.setLong(60, Types.NULL);
		
		Array nonZeroRpmTorqueMatrixArray =null;
		Array numValRpmTorqueArray =null;
		Array clmnIdnxRpmTorqueArray =null;
		
		if(Objects.nonNull(rec.getNonZeroRpmTorqueMatrix()))
			nonZeroRpmTorqueMatrixArray = connection.createArrayOf("BIGINT", rec.getNonZeroRpmTorqueMatrix());
		
		statement.setArray(61, nonZeroRpmTorqueMatrixArray);
		
		if(Objects.nonNull(rec.getNumValRpmTorque()))
			numValRpmTorqueArray = connection.createArrayOf("BIGINT", rec.getNumValRpmTorque());
		
		statement.setArray(62, numValRpmTorqueArray);
		
		if(Objects.nonNull(rec.getClmnIdnxRpmTorque()))
			clmnIdnxRpmTorqueArray = connection.createArrayOf("BIGINT", rec.getClmnIdnxRpmTorque());
		
		statement.setArray(63, clmnIdnxRpmTorqueArray);
				
		jsonObject.setValue(rec.getRpmSpeed());
		statement.setObject(64, jsonObject);
		
		if(Objects.nonNull(rec.getAbsRpmSpeed()))
			statement.setLong(65, rec.getAbsRpmSpeed());
		else
			statement.setLong(65, Types.NULL);
		
		if(Objects.nonNull(rec.getOrdRpmSpeed()))
			statement.setLong(66, rec.getOrdRpmSpeed());
		else
			statement.setLong(66, Types.NULL);
		
		Array nonZeroRpmSpeedMatrixArray =null;
		Array numValRpmSpeedArray =null;
		Array clmnIdnxRpmSpeedArray =null;
		
		if(Objects.nonNull(rec.getNonZeroRpmSpeedMatrix()))
			nonZeroRpmSpeedMatrixArray = connection.createArrayOf("BIGINT", rec.getNonZeroRpmSpeedMatrix());
		
		statement.setArray(67, nonZeroRpmSpeedMatrixArray);
		
		if(Objects.nonNull(rec.getNumValRpmSpeed()))
			numValRpmSpeedArray = connection.createArrayOf("BIGINT", rec.getNumValRpmSpeed());
		
		statement.setArray(68, numValRpmSpeedArray);
		
		if(Objects.nonNull(rec.getClmnIdnxRpmSpeed()))
			clmnIdnxRpmSpeedArray = connection.createArrayOf("BIGINT", rec.getClmnIdnxRpmSpeed());
		
		statement.setArray(69, clmnIdnxRpmSpeedArray);
				
		//statement.setString(70, rec.getAclnSpeed());
		jsonObject.setValue(rec.getAclnSpeed());
		statement.setObject(70, jsonObject);
		
		if(Objects.nonNull(rec.getAbsAclnSpeed()))
			statement.setLong(71, rec.getAbsAclnSpeed());
		else
			statement.setLong(71, Types.NULL);
		
		if(Objects.nonNull(rec.getOrdAclnSpeed()))
			statement.setLong(72, rec.getOrdAclnSpeed());
		else
			statement.setLong(72, Types.NULL);
		
		Array nonZeroAclnSpeedMatrixArray =null;
		Array numValAclnSpeedArray =null;
		Array clmnIdnxAclnSpeedArray =null;
		Array nonZeroBrakePedalAclnSpeedMatrix = null;
		
		if(Objects.nonNull(rec.getNonZeroAclnSpeedMatrix()))
			nonZeroAclnSpeedMatrixArray = connection.createArrayOf("BIGINT", rec.getNonZeroAclnSpeedMatrix());
		
		if(Objects.nonNull(rec.getNonZeroBrakePedalAclnSpeedMatrix()))
			nonZeroBrakePedalAclnSpeedMatrix = connection.createArrayOf("BIGINT", rec.getNonZeroBrakePedalAclnSpeedMatrix());
		
		
		statement.setArray(73, nonZeroAclnSpeedMatrixArray);
		statement.setArray(74, nonZeroBrakePedalAclnSpeedMatrix);
		
		if(Objects.nonNull(rec.getNumValAclnSpeed()))
			numValAclnSpeedArray = connection.createArrayOf("BIGINT", rec.getNumValAclnSpeed());
		
		statement.setArray(75, numValAclnSpeedArray);
		
		if(Objects.nonNull(rec.getClmnIdnxAclnSpeed()))
			clmnIdnxAclnSpeedArray = connection.createArrayOf("BIGINT", rec.getClmnIdnxAclnSpeed());
		
		statement.setArray(76, clmnIdnxAclnSpeedArray);
		
		/////////////////////
		if(Objects.nonNull(rec.getVTripIdlePTOFuelConsumed()))
			statement.setLong(77, rec.getVTripIdlePTOFuelConsumed());
		else
			statement.setLong(77, Types.NULL);
		
		if(Objects.nonNull(rec.getVTripCruiseControlDuration()))
			statement.setLong(78, rec.getVTripCruiseControlDuration());
		else
			statement.setLong(78, Types.NULL);
		
		if(Objects.nonNull(rec.getVTripIdleWithoutPTOFuelConsumed()))
			statement.setLong(79, rec.getVTripIdleWithoutPTOFuelConsumed());
		else
			statement.setLong(79, Types.NULL);
		
		if(Objects.nonNull(rec.getVTripMotionFuelConsumed()))
			statement.setLong(80, rec.getVTripMotionFuelConsumed());
		else
			statement.setLong(80, Types.NULL);
		
		if(Objects.nonNull(rec.getVTripMotionBrakeCount()))
			statement.setLong(81, rec.getVTripMotionBrakeCount());
		else
			statement.setLong(81, Types.NULL);
		
		if(Objects.nonNull(rec.getVTripMotionBrakeDist()))
			statement.setLong(82, rec.getVTripMotionBrakeDist());
		else
			statement.setLong(82, Types.NULL);
		
		if(Objects.nonNull(rec.getVTripMotionPTODuration()))
			statement.setLong(83, rec.getVTripMotionPTODuration());
		else
			statement.setLong(83, Types.NULL);
		
		if(Objects.nonNull(rec.getVTripMotionPTOFuelConsumed()))
			statement.setLong(84, rec.getVTripMotionPTOFuelConsumed());
		else
			statement.setLong(84, Types.NULL);
		
		jsonObject.setValue(rec.getAclnPedalDistr());
		statement.setObject(85, jsonObject);
		
		if(Objects.nonNull(rec.getAclnMinRangeInt()))
			statement.setLong(86, rec.getAclnMinRangeInt());
		else
			statement.setLong(86, Types.NULL);
		
		if(Objects.nonNull(rec.getAclnMaxRangeInt()))
			statement.setLong(87, rec.getAclnMaxRangeInt());
		else
			statement.setLong(87, Types.NULL);
		
		if(Objects.nonNull(rec.getAclnDistrStep()))
			statement.setLong(88, rec.getAclnDistrStep());
		else
			statement.setLong(88, Types.NULL);
		
		Array aclnDistrArrayTime = null;
		if(Objects.nonNull(rec.getAclnDistrArrayTime()))
			aclnDistrArrayTime = connection.createArrayOf("BIGINT", rec.getAclnDistrArrayTime());
		
		statement.setArray(89, aclnDistrArrayTime);
		
		jsonObject.setValue(rec.getVRetarderTorqueActualDistr());
		statement.setObject(90, jsonObject);
		
		if(Objects.nonNull(rec.getVRetarderTorqueMinRangeInt()))
			statement.setLong(91, rec.getVRetarderTorqueMinRangeInt());
		else
			statement.setLong(91, Types.NULL);
		
		if(Objects.nonNull(rec.getVRetarderTorqueMaxRangeInt()))
			statement.setLong(92, rec.getVRetarderTorqueMaxRangeInt());
		else
			statement.setLong(92, Types.NULL);
		
		if(Objects.nonNull(rec.getVRetarderTorqueDistrStep()))
			statement.setLong(93, rec.getVRetarderTorqueDistrStep());
		else
			statement.setLong(93, Types.NULL);
		
		Array vRetarderTorqueDistrArrayTime = null;
		if(Objects.nonNull(rec.getVRetarderTorqueDistrArrayTime()))
			vRetarderTorqueDistrArrayTime = connection.createArrayOf("BIGINT", rec.getVRetarderTorqueDistrArrayTime());
		
		statement.setArray(94, vRetarderTorqueDistrArrayTime);
		
		jsonObject.setValue(rec.getVEngineLoadAtEngineSpeedDistr());
		statement.setObject(95, jsonObject);
		
		if(Objects.nonNull(rec.getVEngineLoadMinRangeInt()))
			statement.setLong(96, rec.getVEngineLoadMinRangeInt());
		else
			statement.setLong(96, Types.NULL);
		
		if(Objects.nonNull(rec.getVEngineLoadMaxRangeInt()))
			statement.setLong(97, rec.getVEngineLoadMaxRangeInt());
		else
			statement.setLong(97, Types.NULL);
		
		if(Objects.nonNull(rec.getVEngineLoadDistrStep()))
			statement.setLong(98, rec.getVEngineLoadDistrStep());
		else
			statement.setLong(98, Types.NULL);
		
		Array vEngineLoadDistrArrayTime = null;
		if(Objects.nonNull(rec.getVEngineLoadDistrArrayTime()))
			vEngineLoadDistrArrayTime = connection.createArrayOf("BIGINT", rec.getVEngineLoadDistrArrayTime());
		
		statement.setArray(99, vEngineLoadDistrArrayTime);
		
		
		if(Objects.nonNull(rec.getVPtoDist()))
			statement.setDouble(100, rec.getVPtoDist());
		else
			statement.setDouble(100, 0);
		
		if(Objects.nonNull(rec.getIdlingConsumptionWithPTO()))
			statement.setDouble(101, rec.getIdlingConsumptionWithPTO());
		else
			statement.setDouble(101, 0);
		
		//////////////////////
		
		
		if(Objects.nonNull(rec.getVin())) {
			statement.setString(102, rec.getVin());
		} else if(Objects.nonNull(rec.getVid())) {
			statement.setString(102, rec.getVid());
		} else
			statement.setString(102, DafConstants.UNKNOWN);

		if(Objects.nonNull(rec.getStartDateTime()))
			statement.setLong(103, rec.getStartDateTime());
		else
			statement.setLong(103, 0);

		if(Objects.nonNull(rec.getEndDateTime()))
			statement.setLong(104, rec.getEndDateTime());
		else
			statement.setLong(104, 0);

		if(Objects.nonNull(rec.getGpsTripDist()))
			statement.setLong(105, rec.getGpsTripDist());
		else
			statement.setLong(105, 0);

		if(Objects.nonNull(rec.getTripCalDist()))
			statement.setLong(106, rec.getTripCalDist());
		else
			statement.setLong(106, 0);

		if(Objects.nonNull(rec.getVIdleDuration()))
			statement.setLong(107, rec.getVIdleDuration());
		else
			statement.setLong(107, 0);

		if(Objects.nonNull(rec.getTripCalAvgSpeed()))
			statement.setDouble(108, rec.getTripCalAvgSpeed());
		else
			statement.setDouble(108, 0);

		if(Objects.nonNull(rec.getVGrossWeightCombination()))
			statement.setDouble(109, rec.getVGrossWeightCombination());
		else
			statement.setDouble(109, 0);

		if(Objects.nonNull(rec.getGpsStartVehDist()))
			statement.setLong(110, rec.getGpsStartVehDist());
		else
			statement.setLong(110, 0);

		if(Objects.nonNull(rec.getGpsStopVehDist()))
			statement.setLong(111, rec.getGpsStopVehDist());
		else
			statement.setLong(111, 0);

		if(Objects.nonNull(rec.getGpsStartLatitude()))
			statement.setDouble(112, rec.getGpsStartLatitude());
		else
			statement.setDouble(112, 0);

		if(Objects.nonNull(rec.getGpsStartLongitude()))
			statement.setDouble(113, rec.getGpsStartLongitude());
		else
			statement.setDouble(113, 0);

		if(Objects.nonNull(rec.getGpsEndLatitude()))
			statement.setDouble(114, rec.getGpsEndLatitude());
		else
			statement.setDouble(114, 0);

		if(Objects.nonNull(rec.getGpsEndLongitude()))
			statement.setDouble(115, rec.getGpsEndLongitude());
		else
			statement.setDouble(115, 0);

		if(Objects.nonNull(rec.getVUsedFuel()))
			statement.setLong(116, rec.getVUsedFuel());
		else
			statement.setLong(116, 0);

		if(Objects.nonNull(rec.getTripCalUsedFuel()))
			statement.setLong(117, rec.getTripCalUsedFuel());
		else
			statement.setLong(117, 0);

		if(Objects.nonNull(rec.getVTripMotionDuration()))
			statement.setLong(118, rec.getVTripMotionDuration());
		else
			statement.setLong(118, 0);

		if(Objects.nonNull(rec.getTripCalDrivingTm()))
			statement.setLong(119, rec.getTripCalDrivingTm());
		else
			statement.setLong(119, 0);

		if(Objects.nonNull(rec.getReceivedTimestamp()))
			statement.setLong(120, rec.getReceivedTimestamp());
		else
			statement.setLong(120, 0);

		if(Objects.nonNull(rec.getKafkaProcessingTS()))
			statement.setLong(121, rec.getKafkaProcessingTS());
		else
			statement.setLong(121, 0);

		if(Objects.nonNull(rec.getTripProcessingTS()))
			statement.setLong(122, rec.getTripProcessingTS());
		else
			statement.setLong(122, 0);

		if(Objects.nonNull(rec.getEtlProcessingTS()))
			statement.setLong(123, rec.getEtlProcessingTS());
		else
			statement.setLong(123, 0);

		if(Objects.nonNull(rec.getTripCalC02Emission()))
			statement.setDouble(124, rec.getTripCalC02Emission());
		else
			statement.setDouble(124, 0);

		if(Objects.nonNull(rec.getTripCalFuelConsumption()))
			statement.setDouble(125, rec.getTripCalFuelConsumption());
		else
			statement.setDouble(125, 0);

		if(Objects.nonNull(rec.getVTachographSpeed()))
			statement.setDouble(126, rec.getVTachographSpeed());
		else
			statement.setDouble(126, 0);

		if(Objects.nonNull(rec.getTripCalAvgGrossWtComb()))
			statement.setDouble(127, rec.getTripCalAvgGrossWtComb());
		else
			statement.setDouble(127, 0);

		if(Objects.nonNull(rec.getTripCalPtoDuration()))
			statement.setDouble(128, rec.getTripCalPtoDuration());
		else
			statement.setDouble(128, 0);

		if(Objects.nonNull(rec.getTripCalHarshBrakeDuration()))
			statement.setDouble(129, rec.getTripCalHarshBrakeDuration());
		else
			statement.setDouble(129, 0);

		if(Objects.nonNull(rec.getTripCalHeavyThrottleDuration()))
			statement.setDouble(130, rec.getTripCalHeavyThrottleDuration());
		else
			statement.setDouble(130, 0);

		if(Objects.nonNull(rec.getTripCalCrsCntrlDist25To50()))
			statement.setLong(131, rec.getTripCalCrsCntrlDist25To50());
		else
			statement.setLong(131, 0);

		if(Objects.nonNull(rec.getTripCalCrsCntrlDist50To75()))
			statement.setLong(132, rec.getTripCalCrsCntrlDist50To75());
		else
			statement.setLong(132, 0);

		if(Objects.nonNull(rec.getTripCalCrsCntrlDistAbv75()))
			statement.setLong(133, rec.getTripCalCrsCntrlDistAbv75());
		else
			statement.setLong(133, 0);

		if(Objects.nonNull(rec.getTripCalAvgTrafficClsfn()))
			statement.setDouble(134, rec.getTripCalAvgTrafficClsfn());
		else
			statement.setDouble(134, 0);

		if(Objects.nonNull(rec.getTripCalCCFuelConsumption()))
			statement.setDouble(135, rec.getTripCalCCFuelConsumption());
		else
			statement.setDouble(135, 0);

		if(Objects.nonNull(rec.getVCruiseControlFuelConsumed()))
			statement.setLong(136, rec.getVCruiseControlFuelConsumed());
		else
			statement.setLong(136, 0);

		if(Objects.nonNull(rec.getVCruiseControlDist()))
			statement.setLong(137, rec.getVCruiseControlDist());
		else
			statement.setLong(137, 0);

		if(Objects.nonNull(rec.getTripCalfuelNonActiveCnsmpt()))
			statement.setDouble(138, rec.getTripCalfuelNonActiveCnsmpt());
		else
			statement.setDouble(138, 0);

		if(Objects.nonNull(rec.getVIdleFuelConsumed()))
			statement.setLong(139, rec.getVIdleFuelConsumed());
		else
			statement.setLong(139, 0);

		if(Objects.nonNull(rec.getTripCalDpaScore()))
			statement.setDouble(140, rec.getTripCalDpaScore());
		else
			statement.setDouble(140, 0);

		if(Objects.nonNull(rec.getDriverId())  && !(DafConstants.BLANK).equals(rec.getDriverId()) )
			statement.setString(141, rec.getDriverId());
		else
			statement.setString(141, DafConstants.UNKNOWN_CASE_VAL);
		
		if(Objects.nonNull(rec.getDriver2Id())  && !(DafConstants.BLANK).equals(rec.getDriver2Id()) )
			statement.setString(142, rec.getDriver2Id());
		else
			statement.setString(142, DafConstants.UNKNOWN_CASE_VAL);
		
		if(Objects.nonNull(rec.getTripCalGpsVehTime()))
			statement.setLong(143, rec.getTripCalGpsVehTime());
		else
			statement.setLong(143, 0);

		statement.setBoolean(144, Boolean.FALSE);

		if(Objects.nonNull(rec.getVGrossWtSum()))
			statement.setDouble(145, rec.getVGrossWtSum());
		else
			statement.setDouble(145, 0);

		if(Objects.nonNull(rec.getNumberOfIndexMessage()))
			statement.setLong(146, rec.getNumberOfIndexMessage());
		else
			statement.setInt(146, 0);
		
		if(Objects.nonNull(rec.getVPTODuration()))
			statement.setLong(147, rec.getVPTODuration());
		else
			statement.setLong(147, 0);
	
		if(Objects.nonNull(rec.getVHarshBrakeDuration()))
			statement.setLong(148, rec.getVHarshBrakeDuration());
		else
			statement.setLong(148, 0);
	
		if(Objects.nonNull(rec.getVBrakeDuration()))
			statement.setLong(149, rec.getVBrakeDuration());
		else
			statement.setLong(149, 0);
		
		if(Objects.nonNull(rec.getVMaxThrottlePaddleDuration()))
			statement.setLong(150, rec.getVMaxThrottlePaddleDuration());
		else
			statement.setLong(150, 0);
		
		if(Objects.nonNull(rec.getVTripAccelerationTime()))
			statement.setLong(151, rec.getVTripAccelerationTime());
		else
			statement.setLong(151, 0);
		
		if(Objects.nonNull(rec.getVTripDPABrakingCount()))
			statement.setLong(152, rec.getVTripDPABrakingCount());
		else
			statement.setLong(152, 0);
		
		if(Objects.nonNull(rec.getVTripDPAAnticipationCount()))
			statement.setLong(153, rec.getVTripDPAAnticipationCount());
		else
			statement.setLong(153, 0);
				
		if(Objects.nonNull(rec.getVSumTripDPABrakingScore()))
			statement.setLong(154, rec.getVSumTripDPABrakingScore());
		else
			statement.setInt(154, 0);

		if(Objects.nonNull(rec.getVSumTripDPAAnticipationScore()))
			statement.setLong(155, rec.getVSumTripDPAAnticipationScore());
		else
			statement.setLong(155, 0);

		if(Objects.nonNull(rec.getVTripIdleWithoutPTODuration()))
			statement.setLong(156, rec.getVTripIdleWithoutPTODuration());
		else
			statement.setLong(156, 0);
		
		if(Objects.nonNull(rec.getVTripIdlePTODuration()))
			statement.setLong(157, rec.getVTripIdlePTODuration());
		else
			statement.setLong(157, 0);
		
		jsonObject.setValue(rec.getRpmTorque()); 
		statement.setObject(158, jsonObject);
		
		if(Objects.nonNull(rec.getAbsRpmTorque()))
			statement.setLong(159, rec.getAbsRpmTorque());
		else
			statement.setLong(159, Types.NULL);
		
		if(Objects.nonNull(rec.getOrdRpmTorque()))
			statement.setLong(160, rec.getOrdRpmTorque());
		else
			statement.setLong(160, Types.NULL);
		
		
		if(Objects.nonNull(rec.getNonZeroRpmTorqueMatrix()))
			nonZeroRpmTorqueMatrixArray = connection.createArrayOf("BIGINT", rec.getNonZeroRpmTorqueMatrix());
		
		statement.setArray(161, nonZeroRpmTorqueMatrixArray);
		
		if(Objects.nonNull(rec.getNumValRpmTorque()))
			numValRpmTorqueArray = connection.createArrayOf("BIGINT", rec.getNumValRpmTorque());
		
		statement.setArray(162, numValRpmTorqueArray);
		
		if(Objects.nonNull(rec.getClmnIdnxRpmTorque()))
			clmnIdnxRpmTorqueArray = connection.createArrayOf("BIGINT", rec.getClmnIdnxRpmTorque());
		
		statement.setArray(163, clmnIdnxRpmTorqueArray);
				
		jsonObject.setValue(rec.getRpmSpeed());
		statement.setObject(164, jsonObject);
		
		if(Objects.nonNull(rec.getAbsRpmSpeed()))
			statement.setLong(165, rec.getAbsRpmSpeed());
		else
			statement.setLong(165, Types.NULL);
		
		if(Objects.nonNull(rec.getOrdRpmSpeed()))
			statement.setLong(166, rec.getOrdRpmSpeed());
		else
			statement.setLong(166, Types.NULL);
		
		
		if(Objects.nonNull(rec.getNonZeroRpmSpeedMatrix()))
			nonZeroRpmSpeedMatrixArray = connection.createArrayOf("BIGINT", rec.getNonZeroRpmSpeedMatrix());
		
		statement.setArray(167, nonZeroRpmSpeedMatrixArray);
		
		if(Objects.nonNull(rec.getNumValRpmSpeed()))
			numValRpmSpeedArray = connection.createArrayOf("BIGINT", rec.getNumValRpmSpeed());
		
		statement.setArray(168, numValRpmSpeedArray);
		
		if(Objects.nonNull(rec.getClmnIdnxRpmSpeed()))
			clmnIdnxRpmSpeedArray = connection.createArrayOf("BIGINT", rec.getClmnIdnxRpmSpeed());
		
		statement.setArray(169, clmnIdnxRpmSpeedArray);
				
		jsonObject.setValue(rec.getAclnSpeed()); 
		statement.setObject(170, jsonObject);
		
		if(Objects.nonNull(rec.getAbsAclnSpeed()))
			statement.setLong(171, rec.getAbsAclnSpeed());
		else
			statement.setLong(171, Types.NULL);
		
		if(Objects.nonNull(rec.getOrdAclnSpeed()))
			statement.setLong(172, rec.getOrdAclnSpeed());
		else
			statement.setLong(172, Types.NULL);
		
		if(Objects.nonNull(rec.getNonZeroAclnSpeedMatrix()))
			nonZeroAclnSpeedMatrixArray = connection.createArrayOf("BIGINT", rec.getNonZeroAclnSpeedMatrix());
		
		if(Objects.nonNull(rec.getNonZeroBrakePedalAclnSpeedMatrix()))
			nonZeroBrakePedalAclnSpeedMatrix = connection.createArrayOf("BIGINT", rec.getNonZeroBrakePedalAclnSpeedMatrix());
		
		
		statement.setArray(173, nonZeroAclnSpeedMatrixArray);
		statement.setArray(174, nonZeroBrakePedalAclnSpeedMatrix);
		
		if(Objects.nonNull(rec.getNumValAclnSpeed()))
			numValAclnSpeedArray = connection.createArrayOf("BIGINT", rec.getNumValAclnSpeed());
		
		statement.setArray(175, numValAclnSpeedArray);
		
		if(Objects.nonNull(rec.getClmnIdnxAclnSpeed()))
			clmnIdnxAclnSpeedArray = connection.createArrayOf("BIGINT", rec.getClmnIdnxAclnSpeed());
		
		statement.setArray(176, clmnIdnxAclnSpeedArray);
		
		/////////////////////
		if(Objects.nonNull(rec.getVTripIdlePTOFuelConsumed()))
			statement.setLong(177, rec.getVTripIdlePTOFuelConsumed());
		else
			statement.setLong(177, Types.NULL);
		
		if(Objects.nonNull(rec.getVTripCruiseControlDuration()))
			statement.setLong(178, rec.getVTripCruiseControlDuration());
		else
			statement.setLong(178, Types.NULL);
		
		if(Objects.nonNull(rec.getVTripIdleWithoutPTOFuelConsumed()))
			statement.setLong(179, rec.getVTripIdleWithoutPTOFuelConsumed());
		else
			statement.setLong(179, Types.NULL);
		
		if(Objects.nonNull(rec.getVTripMotionFuelConsumed()))
			statement.setLong(180, rec.getVTripMotionFuelConsumed());
		else
			statement.setLong(180, Types.NULL);
		
		if(Objects.nonNull(rec.getVTripMotionBrakeCount()))
			statement.setLong(181, rec.getVTripMotionBrakeCount());
		else
			statement.setLong(181, Types.NULL);
		
		if(Objects.nonNull(rec.getVTripMotionBrakeDist()))
			statement.setLong(182, rec.getVTripMotionBrakeDist());
		else
			statement.setLong(182, Types.NULL);
		
		if(Objects.nonNull(rec.getVTripMotionPTODuration()))
			statement.setLong(183, rec.getVTripMotionPTODuration());
		else
			statement.setLong(183, Types.NULL);
		
		if(Objects.nonNull(rec.getVTripMotionPTOFuelConsumed()))
			statement.setLong(184, rec.getVTripMotionPTOFuelConsumed());
		else
			statement.setLong(184, Types.NULL);
		
		jsonObject.setValue(rec.getAclnPedalDistr());
		statement.setObject(185, jsonObject);
		
		if(Objects.nonNull(rec.getAclnMinRangeInt()))
			statement.setLong(186, rec.getAclnMinRangeInt());
		else
			statement.setLong(186, Types.NULL);
		
		if(Objects.nonNull(rec.getAclnMaxRangeInt()))
			statement.setLong(187, rec.getAclnMaxRangeInt());
		else
			statement.setLong(187, Types.NULL);
		
		if(Objects.nonNull(rec.getAclnDistrStep()))
			statement.setLong(188, rec.getAclnDistrStep());
		else
			statement.setLong(188, Types.NULL);
		
		if(Objects.nonNull(rec.getAclnDistrArrayTime()))
		aclnDistrArrayTime = connection.createArrayOf("BIGINT", rec.getAclnDistrArrayTime());
		
		statement.setArray(189, aclnDistrArrayTime);
		
		jsonObject.setValue(rec.getVRetarderTorqueActualDistr());
		statement.setObject(190, jsonObject);
		
		if(Objects.nonNull(rec.getVRetarderTorqueMinRangeInt()))
			statement.setLong(191, rec.getVRetarderTorqueMinRangeInt());
		else
			statement.setLong(191, Types.NULL);
		
		if(Objects.nonNull(rec.getVRetarderTorqueMaxRangeInt()))
			statement.setLong(192, rec.getVRetarderTorqueMaxRangeInt());
		else
			statement.setLong(192, Types.NULL);

		if(Objects.nonNull(rec.getVRetarderTorqueDistrStep()))
			statement.setLong(193, rec.getVRetarderTorqueDistrStep());
		else
			statement.setLong(193, Types.NULL);
		
		if(Objects.nonNull(rec.getVRetarderTorqueDistrArrayTime()))
		vRetarderTorqueDistrArrayTime = connection.createArrayOf("BIGINT", rec.getVRetarderTorqueDistrArrayTime());
		
		statement.setArray(194, vRetarderTorqueDistrArrayTime);
		
		jsonObject.setValue(rec.getVEngineLoadAtEngineSpeedDistr());
		statement.setObject(195, jsonObject);
		
		if(Objects.nonNull(rec.getVEngineLoadMinRangeInt()))
			statement.setLong(196, rec.getVEngineLoadMinRangeInt());
		else
			statement.setLong(196, Types.NULL);
		
		if(Objects.nonNull(rec.getVEngineLoadMaxRangeInt()))
			statement.setLong(197, rec.getVEngineLoadMaxRangeInt());
		else
			statement.setLong(197, Types.NULL);
		
		if(Objects.nonNull(rec.getVEngineLoadDistrStep()))
			statement.setLong(198, rec.getVEngineLoadDistrStep());
		else
			statement.setLong(198, Types.NULL);
				
		if(Objects.nonNull(rec.getVEngineLoadDistrArrayTime()))
		vEngineLoadDistrArrayTime = connection.createArrayOf("BIGINT", rec.getVEngineLoadDistrArrayTime());
		
		statement.setArray(199, vEngineLoadDistrArrayTime);

		if(Objects.nonNull(rec.getVPtoDist()))
			statement.setDouble(200, rec.getVPtoDist());
		else
			statement.setDouble(200, 0);
		
		if(Objects.nonNull(rec.getIdlingConsumptionWithPTO()))
			statement.setDouble(201, rec.getIdlingConsumptionWithPTO());
		else
			statement.setDouble(201, 0);
//////////////////////

		

		return statement;

	}

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}

}
