package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.postgre.bo.IndexTripData;

public class ReadIndexDataDao implements Serializable {
	
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LoggerFactory.getLogger(ReadIndexDataDao.class);
	
	private Connection connection;

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}
	
	public List<IndexTripData> read(PreparedStatement tripIdxQry, String tripId) {
		ResultSet rs = null;
		List<IndexTripData> indexTripList = new ArrayList<IndexTripData>();

		try {
			tripIdxQry.setString(1, tripId);
			//logger.info(" tripIdxQry :: "+tripIdxQry);
			rs = tripIdxQry.executeQuery();
			while (rs.next()) {
				indexTripList.add(map(rs));
			}

		} catch (SQLException e) {
			logger.error("Issue while reading granular data from TripIndex job :: " + tripIdxQry);
			logger.error("Issue while reading granular data from TripIndex job :: " + e);
			// throw e;
		} catch (Exception e) {
			logger.error("Issue while reading granular data from TripIndex job :: " + tripIdxQry);
			logger.error("Issue while reading granular data from TripIndex job :: " + e);
			// throw e;
		} finally {
			if (null != rs) {
				try {
					rs.close();
				} catch (Exception e) {
					logger.error("Issue while closing TripIndex resultset :: " + e);
					// throw e;
				}
			}
		}
		return indexTripList;
	}
	
	private IndexTripData map(ResultSet resultSet) throws SQLException {
		IndexTripData indexTripData = new IndexTripData(); 
		
	    indexTripData.setTripId(resultSet.getString("trip_id"));
		indexTripData.setVin(resultSet.getString("vin"));
		indexTripData.setVTachographSpeed(resultSet.getInt("tachograph_speed"));
		indexTripData.setVGrossWeightCombination(resultSet.getLong("gross_weight_combination"));  
		indexTripData.setDriver2Id(resultSet.getString("driver2_id"));
		indexTripData.setDriverId(resultSet.getString("driver1_id"));
		indexTripData.setJobName(resultSet.getString("jobname"));
		indexTripData.setIncrement(resultSet.getLong("increment"));
		indexTripData.setVDist(resultSet.getLong("distance"));
		indexTripData.setEvtDateTime(resultSet.getLong("event_datetime"));
		indexTripData.setVEvtId(resultSet.getInt("event_id"));
	    
	    return indexTripData;
	}
}
