package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.SQLException;
import java.util.Objects;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.postgre.bo.IndexTripData;
import net.atos.daf.postgre.util.DafConstants;

public class LiveFleetTripIndexDao implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static Logger logger = LoggerFactory.getLogger(LiveFleetTripIndexDao.class);

	private Connection connection;

	public void insert(IndexTripData indexTripData, PreparedStatement tripIndexQry) throws TechnicalException {
		try {
			if (Objects.nonNull(indexTripData)) {

				tripIndexQry = fillStatement(tripIndexQry, indexTripData);
				//logger.info("LiveFleetTripIndexDao indexTripData:: "+indexTripData);
				//tripIndexQry.addBatch();
				//tripIndexQry.executeBatch();
				tripIndexQry.execute();
			} 
		} catch (SQLException e) {
			logger.error("Sql Issue while inserting data to indexMessageData table :{} ", e.getMessage());
			logger.error("Issue while inserting TripIndex record :: {}", tripIndexQry);
			logger.error("Issue while inserting TripIndex, connection ::{}, indexTripData :{} " , connection, indexTripData);
			e.printStackTrace();
		} catch (Exception e) {
			logger.error("Issue while inserting data to indexMessageData table :{} ", e.getMessage());
			logger.error("Issue while inserting TripIndex record :: {}", tripIndexQry);
			logger.error("Issue while inserting TripIndex, connection ::{}, indexTripData :{} " , connection, indexTripData);
			e.printStackTrace();
		}

	}

	private PreparedStatement fillStatement(PreparedStatement statement, IndexTripData rec) throws Exception {

		statement.setString(1, rec.getTripId());

		//logger.info("TripIndex Sink TripId : " + rec.getTripId() + " Increment : " + rec.getIncrement());
		
		if (rec.getVin() != null) {
			statement.setString(2, rec.getVin());
		}else
			statement.setString(2, DafConstants.UNKNOWN);
		
		statement.setInt(3, rec.getVTachographSpeed());
		statement.setLong(4, rec.getVGrossWeightCombination());
		statement.setString(5, rec.getDriver2Id());
		statement.setString(6, rec.getDriverId());
		statement.setString(7, rec.getJobName());
		statement.setLong(8, rec.getIncrement());
		statement.setLong(9, rec.getVDist());
		statement.setLong(10, rec.getEvtDateTime());
		statement.setInt(11, rec.getVEvtId());
		statement.setLong(12, TimeFormatter.getInstance().getCurrentUTCTime());
		
		return statement;
	
	}

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}

}
