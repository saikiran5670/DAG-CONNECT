package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.postgre.bo.Co2Master;

public class Co2MasterDao implements Serializable {

	private static final String READ_CO2_COEFFICIENT = "SELECT fuel_type,coefficient from master.co2coefficient";

	private Connection connection;

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}

	

	public Co2Master read() throws TechnicalException, SQLException {

		PreparedStatement stmt_read_co2_coefficient = null;
		ResultSet rs_position = null;
		Co2Master cm= new Co2Master();

		try {

			stmt_read_co2_coefficient = connection.prepareStatement(READ_CO2_COEFFICIENT);
			rs_position = stmt_read_co2_coefficient.executeQuery();
			System.out.println("in CorMaster DAO");
			while (rs_position.next()) {

				if (rs_position.getString("fuel_type").equals("D")) {
					cm.setCoefficient_D(rs_position.getDouble("coefficient"));
					System.out.println("coefficient of D --  " + rs_position.getDouble("coefficient"));

				} else {
					cm.setCoefficient_B(rs_position.getDouble("coefficient"));
					System.out.println("coefficient of B --  " + rs_position.getDouble("coefficient"));
				}

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

}
