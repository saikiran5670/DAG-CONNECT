package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.SQLException;
import java.util.Objects;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.postgre.bo.EcoScore;
import net.atos.daf.postgre.util.DafConstants;
import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.common.ct2.utc.TimeFormatter;

public class EcoScoreDao implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static Logger logger = LoggerFactory.getLogger(EcoScoreDao.class);

	private Connection connection;

	public void insert(EcoScore ecoScoreData, PreparedStatement ecoScoreInsertQry) throws TechnicalException {
		try {
			if (Objects.nonNull(ecoScoreData)) {

				ecoScoreInsertQry = fillStatement(ecoScoreInsertQry, ecoScoreData);
				ecoScoreInsertQry.execute();
				
				logger.info("EcoScore records inserted to ecoscore table :: "+ecoScoreData.getTripId());
			} 
		} catch (SQLException e) {
			logger.error("Sql Issue while inserting data to ecoscore table :{}, connection:{} " , e.getMessage(), connection);
			logger.error("Issue while inserting EcoScore record ::{} " , ecoScoreInsertQry);
			e.printStackTrace();
		} catch (Exception e) {
			logger.error("Issue while inserting data to ecoscore table : {} " , e.getMessage());
			logger.error("Issue while inserting ecoscore record ::{} " , ecoScoreInsertQry);
			e.printStackTrace();
		}

	}

	private PreparedStatement fillStatement(PreparedStatement statement, EcoScore rec) throws SQLException, Exception {

		statement.setString(1, rec.getTripId());

		logger.info("EcoScore Sink TripId : " + rec.getTripId() + " VIN : " + rec.getVin() + " VID : " + rec.getVin());

		if (rec.getVin() != null) {
			statement.setString(2, rec.getVin());
		}else
			statement.setString(2, DafConstants.UNKNOWN);

		if (rec.getStartDateTime() != null)
			statement.setLong(3, rec.getStartDateTime());
		else
			statement.setLong(3, 0);

		if (rec.getEndDateTime() != null)
			statement.setLong(4, rec.getEndDateTime());
		else
			statement.setLong(4, 0);
		
		if(rec.getDriverId() != null && !(DafConstants.BLANK).equals(rec.getDriverId()))
			statement.setString(5, rec.getDriverId());
		else
			statement.setString(5, DafConstants.UNKNOWN_CASE_VAL);
		
		if (rec.getTripCalDist() != null)
			statement.setLong(6, rec.getTripCalDist());
		else
			statement.setLong(6, 0);


		if (rec.getVSumTripDPABrakingScore() != null)
			statement.setLong(7, rec.getVSumTripDPABrakingScore());
		else
			statement.setLong(7, 0);

		if (rec.getVTripDPABrakingCount() != null)
			statement.setLong(8, rec.getVTripDPABrakingCount());
		else
			statement.setLong(8, 0);

		if (rec.getVSumTripDPAAnticipationScore() != null)
			statement.setLong(9, rec.getVSumTripDPAAnticipationScore());
		else
			statement.setLong(9, 0);

		if (rec.getVTripDPAAnticipationCount() != null)
			statement.setLong(10, rec.getVTripDPAAnticipationCount());
		else
			statement.setLong(10, 0);

		if (rec.getTripCalAvgGrossWtComb() != null)
			statement.setDouble(11, rec.getTripCalAvgGrossWtComb());
		else
			statement.setDouble(11, 0);

		if (rec.getTripCalUsedFuel() != null)
			statement.setLong(12, rec.getTripCalUsedFuel());
		else
			statement.setLong(12, 0);

		if (rec.getVPTODuration() != null)
			statement.setLong(13, rec.getVPTODuration());
		else
			statement.setLong(13, 0);
		
		if (rec.getVIdleDuration() != null)
			statement.setLong(14, rec.getVIdleDuration());
		else
			statement.setLong(14, 0);
		
		if (rec.getVMaxThrottlePaddleDuration() != null)
			statement.setLong(15, rec.getVMaxThrottlePaddleDuration());
		else
			statement.setLong(15, 0);

		if (rec.getVCruiseControlDist() != null)
			statement.setLong(16, rec.getVCruiseControlDist());
		else
			statement.setLong(16, 0);

		if (rec.getTripCalCrsCntrlDist25To50() != null)
			statement.setLong(17, rec.getTripCalCrsCntrlDist25To50());
		else
			statement.setLong(17, 0);

		if (rec.getTripCalCrsCntrlDist50To75() != null)
			statement.setLong(18, rec.getTripCalCrsCntrlDist50To75());
		else
			statement.setLong(18, 0);
		
		if (rec.getTripCalCrsCntrlDistAbv75() != null)
			statement.setLong(19, rec.getTripCalCrsCntrlDistAbv75());
		else
			statement.setLong(19, 0);
		
		if (rec.getTachoVGrossWtCmbSum() != null)
			statement.setDouble(20, rec.getTachoVGrossWtCmbSum());
		else
			statement.setDouble(20, 0);
		
		if (rec.getVHarshBrakeDuration() != null)
			statement.setLong(21, rec.getVHarshBrakeDuration());
		else
			statement.setLong(21, 0);

		if (rec.getVBrakeDuration() != null)
			statement.setLong(22, rec.getVBrakeDuration());
		else
			statement.setLong(22, 0);
		
		statement.setLong(23, TimeFormatter.getInstance().getCurrentUTCTime());
		
		//harcoded - clarify column
		statement.setString(24, DafConstants.TRIP_LEVEL_AGGREGATION);
		
		if (rec.getEndDateTime() != null)
			statement.setLong(25, rec.getEndDateTime());
		else
			statement.setLong(25, 0);
		
		if (rec.getVGrossWtCmbCount() != null)
			statement.setLong(26, rec.getVGrossWtCmbCount());
		else
			statement.setLong(26, 0);
		
		if (rec.getVTripAccelerationTime() != null)
			statement.setLong(27, rec.getVTripAccelerationTime());
		else
			statement.setLong(27, 0);
		
		statement.setBoolean(28, Boolean.FALSE);
		
		//Update Record
		if (rec.getVin() != null) {
			statement.setString(29, rec.getVin());
		}else
			statement.setString(29, DafConstants.UNKNOWN);

		if (rec.getStartDateTime() != null)
			statement.setLong(30, rec.getStartDateTime());
		else
			statement.setLong(30, 0);

		if (rec.getEndDateTime() != null)
			statement.setLong(31, rec.getEndDateTime());
		else
			statement.setLong(31, 0);
		
		if(rec.getDriverId() != null && !(DafConstants.BLANK).equals(rec.getDriverId()))
			statement.setString(32, rec.getDriverId());
		else
			statement.setString(32, DafConstants.UNKNOWN_CASE_VAL);
		
		if (rec.getTripCalDist() != null)
			statement.setLong(33, rec.getTripCalDist());
		else
			statement.setLong(33, 0);

		if (rec.getVSumTripDPABrakingScore() != null)
			statement.setLong(34, rec.getVSumTripDPABrakingScore());
		else
			statement.setLong(34, 0);

		if (rec.getVTripDPABrakingCount() != null)
			statement.setLong(35, rec.getVTripDPABrakingCount());
		else
			statement.setLong(35, 0);

		if (rec.getVSumTripDPAAnticipationScore() != null)
			statement.setLong(36, rec.getVSumTripDPAAnticipationScore());
		else
			statement.setLong(36, 0);

		if (rec.getVTripDPAAnticipationCount() != null)
			statement.setLong(37, rec.getVTripDPAAnticipationCount());
		else
			statement.setLong(37, 0);

		if (rec.getTripCalAvgGrossWtComb() != null)
			statement.setDouble(38, rec.getTripCalAvgGrossWtComb());
		else
			statement.setDouble(38, 0);

		if (rec.getTripCalUsedFuel() != null)
			statement.setLong(39, rec.getTripCalUsedFuel());
		else
			statement.setLong(39, 0);

		if (rec.getVPTODuration() != null)
			statement.setLong(40, rec.getVPTODuration());
		else
			statement.setLong(40, 0);
		
		if (rec.getVIdleDuration() != null)
			statement.setLong(41, rec.getVIdleDuration());
		else
			statement.setLong(41, 0);
		
		if (rec.getVMaxThrottlePaddleDuration() != null)
			statement.setLong(42, rec.getVMaxThrottlePaddleDuration());
		else
			statement.setLong(42, 0);

		if (rec.getVCruiseControlDist() != null)
			statement.setLong(43, rec.getVCruiseControlDist());
		else
			statement.setLong(43, 0);

		if (rec.getTripCalCrsCntrlDist25To50() != null)
			statement.setLong(44, rec.getTripCalCrsCntrlDist25To50());
		else
			statement.setLong(44, 0);

		if (rec.getTripCalCrsCntrlDist50To75() != null)
			statement.setLong(45, rec.getTripCalCrsCntrlDist50To75());
		else
			statement.setLong(45, 0);
		
		if (rec.getTripCalCrsCntrlDistAbv75() != null)
			statement.setLong(46, rec.getTripCalCrsCntrlDistAbv75());
		else
			statement.setLong(46, 0);
		
		if (rec.getTachoVGrossWtCmbSum() != null)
			statement.setDouble(47, rec.getTachoVGrossWtCmbSum());
		else
			statement.setDouble(47, 0);
		
		if (rec.getVHarshBrakeDuration() != null)
			statement.setLong(48, rec.getVHarshBrakeDuration());
		else
			statement.setLong(48, 0);

		if (rec.getVBrakeDuration() != null)
			statement.setLong(49, rec.getVBrakeDuration());
		else
			statement.setLong(49, 0);
		
		statement.setLong(50, TimeFormatter.getInstance().getCurrentUTCTime());
		
		if (rec.getEndDateTime() != null)
			statement.setLong(51, rec.getEndDateTime());
		else
			statement.setLong(51, 0);
		
		if (rec.getVGrossWtCmbCount() != null)
			statement.setLong(52, rec.getVGrossWtCmbCount());
		else
			statement.setLong(52, 0);
		
		if (rec.getVTripAccelerationTime() != null)
			statement.setLong(53, rec.getVTripAccelerationTime());
		else
			statement.setLong(53, 0);

		return statement;

	}

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}

}
