package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.SQLException;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.postgre.bo.EcoScore;
import net.atos.daf.postgre.util.DafConstants;
import net.atos.daf.common.ct2.exception.TechnicalException;

public class EcoScoreDao implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static Logger logger = LoggerFactory.getLogger(EcoScoreDao.class);

	private Connection connection;

	public void insert(EcoScore ecoScoreData, PreparedStatement ecoScoreInsertQry) throws TechnicalException {
		try {
			if (null != ecoScoreData && null != (connection = getConnection())) {

				ecoScoreInsertQry = fillStatement(ecoScoreInsertQry, ecoScoreData);
				ecoScoreInsertQry.addBatch();
				ecoScoreInsertQry.executeBatch();
				
			} else {
				if (connection == null) {
					logger.error(" Issue EcoScore connection is null : " + connection);
					throw new TechnicalException("EcoScore Datamart connection is null :: ");
				}
			}
		} catch (SQLException e) {
			logger.error("Sql Issue while inserting data to ecoscore table : " + e.getMessage());
			logger.error("Issue while inserting EcoScore record :: " + ecoScoreInsertQry);
			e.printStackTrace();
		} catch (Exception e) {
			logger.error("Issue while inserting data to ecoscore table : " + e.getMessage());
			logger.error("Issue while inserting ecoscore record :: " + ecoScoreInsertQry);
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
		
		statement.setString(5, rec.getDriverId());
		statement.setString(6, rec.getDriver2Id());

		if (rec.getTripCalDist() != null)
			statement.setLong(7, rec.getTripCalDist());
		else
			statement.setLong(7, 0);


		if (rec.getVSumTripDPABrakingScore() != null)
			statement.setDouble(8, rec.getVSumTripDPABrakingScore());
		else
			statement.setDouble(8, 0);

		if (rec.getVTripDPABrakingCount() != null)
			statement.setDouble(9, rec.getVTripDPABrakingCount());
		else
			statement.setDouble(9, 0);

		if (rec.getVSumTripDPAAnticipationScore() != null)
			statement.setLong(10, rec.getVSumTripDPAAnticipationScore());
		else
			statement.setLong(10, 0);

		if (rec.getVTripDPAAnticipationCount() != null)
			statement.setLong(11, rec.getVTripDPAAnticipationCount());
		else
			statement.setLong(11, 0);

		if (rec.getVGrossWeightCombination() != null)
			statement.setDouble(12, rec.getVGrossWeightCombination());
		else
			statement.setDouble(12, 0);

		if (rec.getTripCalUsedFuel() != null)
			statement.setLong(13, rec.getTripCalUsedFuel());
		else
			statement.setLong(13, 0);

		if (rec.getTripCalPtoDuration() != null)
			statement.setDouble(14, rec.getTripCalPtoDuration());
		else
			statement.setDouble(14, 0);
		
		if (rec.getVIdleDuration() != null)
			statement.setInt(15, rec.getVIdleDuration());
		else
			statement.setInt(15, 0);
		
		if (rec.getTripCalHeavyThrottleDuration() != null)
			statement.setDouble(16, rec.getTripCalHeavyThrottleDuration());
		else
			statement.setDouble(16, 0);

		if (rec.getVStopFuel() != null)
			statement.setLong(17, rec.getVStopFuel());
		else
			statement.setLong(17, 0);

		if (rec.getVStartFuel() != null)
			statement.setLong(18, rec.getVStartFuel());
		else
			statement.setLong(18, 0);
		
		
		if (rec.getVin() != null) {
			statement.setString(19, rec.getVin());
		}else
			statement.setString(19, DafConstants.UNKNOWN);

		if (rec.getStartDateTime() != null)
			statement.setLong(20, rec.getStartDateTime());
		else
			statement.setLong(20, 0);

		if (rec.getEndDateTime() != null)
			statement.setLong(21, rec.getEndDateTime());
		else
			statement.setLong(21, 0);
		
		statement.setString(22, rec.getDriverId());
		statement.setString(23, rec.getDriver2Id());

		if (rec.getTripCalDist() != null)
			statement.setLong(24, rec.getTripCalDist());
		else
			statement.setLong(24, 0);


		if (rec.getVSumTripDPABrakingScore() != null)
			statement.setDouble(25, rec.getVSumTripDPABrakingScore());
		else
			statement.setDouble(25, 0);

		if (rec.getVTripDPABrakingCount() != null)
			statement.setDouble(26, rec.getVTripDPABrakingCount());
		else
			statement.setDouble(26, 0);

		if (rec.getVSumTripDPAAnticipationScore() != null)
			statement.setLong(27, rec.getVSumTripDPAAnticipationScore());
		else
			statement.setLong(27, 0);

		if (rec.getVTripDPAAnticipationCount() != null)
			statement.setLong(28, rec.getVTripDPAAnticipationCount());
		else
			statement.setLong(28, 0);

		if (rec.getVGrossWeightCombination() != null)
			statement.setDouble(29, rec.getVGrossWeightCombination());
		else
			statement.setDouble(29, 0);

		if (rec.getTripCalUsedFuel() != null)
			statement.setLong(30, rec.getTripCalUsedFuel());
		else
			statement.setLong(30, 0);

		if (rec.getTripCalPtoDuration() != null)
			statement.setDouble(31, rec.getTripCalPtoDuration());
		else
			statement.setDouble(31, 0);
		
		if (rec.getVIdleDuration() != null)
			statement.setInt(32, rec.getVIdleDuration());
		else
			statement.setInt(32, 0);
		
		if (rec.getTripCalHeavyThrottleDuration() != null)
			statement.setDouble(33, rec.getTripCalHeavyThrottleDuration());
		else
			statement.setDouble(33, 0);

		if (rec.getVStopFuel() != null)
			statement.setLong(34, rec.getVStopFuel());
		else
			statement.setLong(34, 0);

		if (rec.getVStartFuel() != null)
			statement.setLong(35, rec.getVStartFuel());
		else
			statement.setLong(35, 0);


		return statement;

	}

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}

}
