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

public class Co2MasterDao implements Serializable {
	
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	//private static final String READ_CO2_COEFFICIENT = "SELECT fuel_type,coefficient from master.co2coefficient";
	private static final String READ_CO2_COEFFICIENT = "select v.vin, v.vid, v.fuel_type, c.coefficient from master.vehicle v inner join master.co2coefficient c on v.fuel_type = c.fuel_type WHERE v.vid=? limit 1";
	private static final Logger logger = LoggerFactory.getLogger(Co2MasterDao.class);
	
	private Connection connection;

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}

	

	public Co2Master read(String vid) throws TechnicalException, SQLException {

		PreparedStatement stmt_read_co2_coefficient = null;
		ResultSet rs_position = null;
		Co2Master cm= new Co2Master();

		try {

			stmt_read_co2_coefficient = connection.prepareStatement(READ_CO2_COEFFICIENT);
			stmt_read_co2_coefficient.setString(1,vid);
			rs_position = stmt_read_co2_coefficient.executeQuery();
			System.out.println("in CorMaster DAO");
			while (rs_position.next()) {

				/*
				 * if (rs_position.getString("fuel_type").equals("D")) {
				 * cm.setCoefficient_D(rs_position.getDouble("coefficient"));
				 * System.out.println("coefficient of D --  " +
				 * rs_position.getDouble("coefficient"));
				 * 
				 * } else { cm.setCoefficient_B(rs_position.getDouble("coefficient"));
				 * System.out.println("coefficient of B --  " +
				 * rs_position.getDouble("coefficient")); }
				 */
				
				cm.setCoefficient(rs_position.getDouble("coefficient"));

			}

		} catch (SQLException e) {
			e.printStackTrace();
		} finally {

			if (null != rs_position) {

				try {
					rs_position.close();
				} catch (SQLException ignore) {
					/** ignore any errors here */
				}
			}
		}

		return cm;

	}

	public double read(PreparedStatement co2CoEfficientQry, String vin) {
		ResultSet rs = null;
		double co2CoEfficient = 0;
		try {
			co2CoEfficientQry.setString(1, vin);
			rs = co2CoEfficientQry.executeQuery();
			while (rs.next()) {
				co2CoEfficient = rs.getDouble("coefficient");
			}

		} catch (SQLException e) {
			logger.error("Issue while reading Co2CoEfficient value for trip statistics job :: " + e);
			// throw e;
		} catch (Exception e) {
			logger.error("Issue while reading Co2CoEfficient value for trip statistics job :: " + e);
			// throw e;
		} finally {
			if (null != rs) {
				try {
					rs.close();
				} catch (Exception e) {
					logger.error("Issue while closing Co2CoEfficient resultset :: " + e);
					// throw e;
				}
			}
		}
		return co2CoEfficient;
	}
}
