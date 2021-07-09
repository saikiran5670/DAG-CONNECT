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
			if (null != ecoScoreData && null != (connection = getConnection())) {

				ecoScoreInsertQry = fillStatement(ecoScoreInsertQry, ecoScoreData);
				ecoScoreInsertQry.addBatch();
				ecoScoreInsertQry.executeBatch();
				
				logger.info("EcoScore records inserted to ecoscore table :: "+ecoScoreData.getTripId());
				
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
		
		if(rec.getDriverId() != null)
			statement.setString(5, rec.getDriverId());
		else
			statement.setString(5, DafConstants.UNKNOWN);
		
		if (rec.getTripCalDist() != null)
			statement.setLong(6, rec.getTripCalDist());
		else
			statement.setLong(6, 0);


		if (rec.getVSumTripDPABrakingScore() != null)
			statement.setDouble(7, rec.getVSumTripDPABrakingScore());
		else
			statement.setDouble(7, 0);

		if (rec.getVTripDPABrakingCount() != null)
			statement.setDouble(8, rec.getVTripDPABrakingCount());
		else
			statement.setDouble(8, 0);

		if (rec.getVSumTripDPAAnticipationScore() != null)
			statement.setLong(9, rec.getVSumTripDPAAnticipationScore());
		else
			statement.setLong(9, 0);

		if (rec.getVTripDPAAnticipationCount() != null)
			statement.setLong(10, rec.getVTripDPAAnticipationCount());
		else
			statement.setLong(10, 0);

		if (rec.getVGrossWeightCombination() != null)
			statement.setDouble(11, rec.getVGrossWeightCombination());
		else
			statement.setDouble(11, 0);

		if (rec.getTripCalUsedFuel() != null)
			statement.setLong(12, rec.getTripCalUsedFuel());
		else
			statement.setLong(12, 0);

		if (rec.getTripCalPtoDuration() != null)
			statement.setDouble(13, rec.getTripCalPtoDuration());
		else
			statement.setDouble(13, 0);
		
		if (rec.getVIdleDuration() != null)
			statement.setInt(14, rec.getVIdleDuration());
		else
			statement.setInt(14, 0);
		
		if (rec.getTripCalHeavyThrottleDuration() != null)
			statement.setDouble(15, rec.getTripCalHeavyThrottleDuration());
		else
			statement.setDouble(15, 0);

		if (rec.getTripCalCrsCntrlUsage() != null)
			statement.setDouble(16, rec.getTripCalCrsCntrlUsage());
		else
			statement.setDouble(16, 0);

		if (rec.getTripCalCrsCntrlDist25To50() != null)
			statement.setDouble(17, rec.getTripCalCrsCntrlDist25To50());
		else
			statement.setDouble(17, 0);

		if (rec.getTripCalCrsCntrlDist50To75() != null)
			statement.setDouble(18, rec.getTripCalCrsCntrlDist50To75());
		else
			statement.setDouble(18, 0);
		
		if (rec.getTripCalCrsCntrlDistAbv75() != null)
			statement.setDouble(19, rec.getTripCalCrsCntrlDistAbv75());
		else
			statement.setDouble(19, 0);
		
		if (rec.getTachoVGrossWtCmbSum() != null)
			statement.setDouble(20, rec.getTachoVGrossWtCmbSum());
		else
			statement.setDouble(20, 0);
		
		if (rec.getVHarshBrakeDuration() != null)
			statement.setInt(21, rec.getVHarshBrakeDuration());
		else
			statement.setDouble(21, 0);

		if (rec.getVBrakeDuration() != null)
			statement.setInt(22, rec.getVBrakeDuration());
		else
			statement.setDouble(22, 0);
		
		statement.setLong(23, TimeFormatter.getInstance().getCurrentUTCTime());
		
		//harcoded - clarify column
		statement.setString(24, DafConstants.TRIP_LEVEL_AGGREGATION);
		statement.setLong(25, TimeFormatter.getInstance().getCurrentUTCTime());
		
				
		if (rec.getVin() != null) {
			statement.setString(26, rec.getVin());
		}else
			statement.setString(26, DafConstants.UNKNOWN);

		if (rec.getStartDateTime() != null)
			statement.setLong(27, rec.getStartDateTime());
		else
			statement.setLong(27, 0);

		if (rec.getEndDateTime() != null)
			statement.setLong(28, rec.getEndDateTime());
		else
			statement.setLong(28, 0);
		
		if(rec.getDriverId() != null)
			statement.setString(29, rec.getDriverId());
		else
			statement.setString(29, DafConstants.UNKNOWN);
		
		if (rec.getTripCalDist() != null)
			statement.setLong(30, rec.getTripCalDist());
		else
			statement.setLong(30, 0);


		if (rec.getVSumTripDPABrakingScore() != null)
			statement.setDouble(31, rec.getVSumTripDPABrakingScore());
		else
			statement.setDouble(31, 0);

		if (rec.getVTripDPABrakingCount() != null)
			statement.setDouble(32, rec.getVTripDPABrakingCount());
		else
			statement.setDouble(32, 0);

		if (rec.getVSumTripDPAAnticipationScore() != null)
			statement.setLong(33, rec.getVSumTripDPAAnticipationScore());
		else
			statement.setLong(33, 0);

		if (rec.getVTripDPAAnticipationCount() != null)
			statement.setLong(34, rec.getVTripDPAAnticipationCount());
		else
			statement.setLong(34, 0);

		if (rec.getVGrossWeightCombination() != null)
			statement.setDouble(35, rec.getVGrossWeightCombination());
		else
			statement.setDouble(35, 0);

		if (rec.getTripCalUsedFuel() != null)
			statement.setLong(36, rec.getTripCalUsedFuel());
		else
			statement.setLong(36, 0);

		if (rec.getTripCalPtoDuration() != null)
			statement.setDouble(37, rec.getTripCalPtoDuration());
		else
			statement.setDouble(37, 0);
		
		if (rec.getVIdleDuration() != null)
			statement.setInt(38, rec.getVIdleDuration());
		else
			statement.setInt(38, 0);
		
		if (rec.getTripCalHeavyThrottleDuration() != null)
			statement.setDouble(39, rec.getTripCalHeavyThrottleDuration());
		else
			statement.setDouble(39, 0);

		if (rec.getTripCalCrsCntrlUsage() != null)
			statement.setDouble(40, rec.getTripCalCrsCntrlUsage());
		else
			statement.setDouble(40, 0);

		if (rec.getTripCalCrsCntrlDist25To50() != null)
			statement.setDouble(41, rec.getTripCalCrsCntrlDist25To50());
		else
			statement.setDouble(41, 0);

		if (rec.getTripCalCrsCntrlDist50To75() != null)
			statement.setDouble(42, rec.getTripCalCrsCntrlDist50To75());
		else
			statement.setDouble(42, 0);
		
		if (rec.getTripCalCrsCntrlDistAbv75() != null)
			statement.setDouble(43, rec.getTripCalCrsCntrlDistAbv75());
		else
			statement.setDouble(43, 0);
		
		if (rec.getTachoVGrossWtCmbSum() != null)
			statement.setDouble(44, rec.getTachoVGrossWtCmbSum());
		else
			statement.setDouble(44, 0);
		
		if (rec.getVHarshBrakeDuration() != null)
			statement.setInt(45, rec.getVHarshBrakeDuration());
		else
			statement.setDouble(45, 0);

		if (rec.getVBrakeDuration() != null)
			statement.setInt(46, rec.getVBrakeDuration());
		else
			statement.setDouble(46, 0);
		
		statement.setLong(47, TimeFormatter.getInstance().getCurrentUTCTime());

		return statement;

	}

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}

}
