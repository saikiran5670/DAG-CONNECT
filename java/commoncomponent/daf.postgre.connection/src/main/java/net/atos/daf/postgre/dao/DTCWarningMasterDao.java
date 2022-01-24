package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.exception.TechnicalException;

public class DTCWarningMasterDao implements Serializable {

	// private static final String READ_DTC_WARNING = "select v.vin, v.vid,
	// v.fuel_type, c.coefficient from master.vehicle v inner join
	// master.co2coefficient c on v.fuel_type = c.fuel_type WHERE v.vid=? limit 1";
	private static final String READ_DTC_WARNING = "select * from master.dtcwarning where class=? and number=?";

	private static final Logger logger = LoggerFactory.getLogger(DTCWarningMasterDao.class);

	private Connection connection;

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}

	public boolean read(Integer warningClass, Integer WarningNumber) throws TechnicalException, SQLException {

		PreparedStatement stmtReadDTCWarning = null;
		ResultSet rsPosition = null;
		
		
		boolean warning=false;

		try {

			stmtReadDTCWarning = connection.prepareStatement(READ_DTC_WARNING);
			 stmtReadDTCWarning.setInt(1,warningClass);
			 stmtReadDTCWarning.setInt(2,WarningNumber);
			 
			rsPosition = stmtReadDTCWarning.executeQuery();

			while (rsPosition.next()) {
				warning=true;
				break;
			}

		} catch (SQLException e) {
			logger.error("Issue while reading DTCWarningMasterDao value for trip Tracing job :: " + e.getMessage());
			logger.error("Issue while reading DTCWarningMasterDao value for trip statistics job :: ",stmtReadDTCWarning);
			// throw e;
		} catch (Exception e) {
			logger.error("Issue while reading DTCWarningMasterDao value for trip Tracing job :: " + e.getMessage());
			logger.error("Issue while reading DTCWarningMasterDao value for trip Tracing job :: ",stmtReadDTCWarning);
			// throw e;
		} finally {
			if (null != rsPosition) {
				try {
					rsPosition.close();
				} catch (Exception e) {
					logger.error("Issue while closing DTCWarningMasterDao resultset :: " + e.getMessage());
					// throw e;
				}
			}
		}
		return warning;
	}
	
	


public boolean readMonitor(Integer warningClass, Integer WarningNumber,PreparedStatement stmtReadDTCWarning) throws TechnicalException, SQLException {

	//PreparedStatement stmtReadDTCWarning = null;
	ResultSet rsPosition = null;
	
	
	boolean warning=false;

	try {

		//stmtReadDTCWarning = connection.prepareStatement(READ_DTC_WARNING);
		 stmtReadDTCWarning.setInt(1,warningClass);
		 stmtReadDTCWarning.setInt(2,WarningNumber);
		 
		 
		rsPosition = stmtReadDTCWarning.executeQuery();

		while (rsPosition.next()) {
			warning=true;
			break;
		}

	} catch (SQLException e) {
		logger.error("Issue while reading DTCWarningMasterDao value for trip Tracing job :: " + e.getMessage());
		logger.error("Issue while reading DTCWarningMasterDao value for trip statistics job :: ",stmtReadDTCWarning);
		// throw e;
	} catch (Exception e) {
		logger.error("Issue while reading DTCWarningMasterDao value for trip Tracing job :: " + e.getMessage());
		logger.error("Issue while reading DTCWarningMasterDao value for trip Tracing job :: ",stmtReadDTCWarning);
		// throw e;
	} finally {
		if (null != rsPosition) {
			try {
				rsPosition.close();
			} catch (Exception e) {
				logger.error("Issue while closing DTCWarningMasterDao resultset :: " + e.getMessage());
				// throw e;
			}
		}
	}
	return warning;
}


}
	
