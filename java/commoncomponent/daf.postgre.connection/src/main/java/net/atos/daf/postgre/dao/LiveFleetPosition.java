package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;

import java.sql.ResultSet;
import java.sql.SQLException;

import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;

public class LiveFleetPosition implements Serializable {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private Connection connection;

	private static final String READ_LIVEFLEET_POSITION = "SELECT distance_until_next_service from livefleet.livefleet_position_statistics WHERE vin = ? ORDER BY created_at_m2m DESC limit 1";
	private static final String INSERT_LIVEFLEET_POSITION = "INSERT INTO livefleet.livefleet_position_statistics ( trip_id    , vin    ,message_time_stamp    ,gps_altitude    ,gps_heading    ,gps_latitude    ,gps_longitude    ,co2_emission    ,fuel_consumption    , last_odometer_val  ,distance_until_next_service    , created_at_m2m    ,created_at_kafka    ,created_at_dm    ) VALUES (?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    )";

	public boolean insert(Monitor row) throws TechnicalException, SQLException {
		PreparedStatement stmt_insert_livefleet_position;

		boolean result = false;
		// String varTripID = row.getDocument().getTripID(); /// taken
		/// in two classes

		try {

			if (null != row && null != (connection = getConnection())) {
				stmt_insert_livefleet_position = connection.prepareStatement(INSERT_LIVEFLEET_POSITION);
				stmt_insert_livefleet_position = fillStatement(stmt_insert_livefleet_position, row);
				System.out.println("inside LiveFleetDriverActivityDao Insert");
				stmt_insert_livefleet_position.addBatch();
				stmt_insert_livefleet_position.executeBatch();
			}
		} catch (SQLException e) {
			System.out.println("inside catch LiveFleetDriverActivityDao Insert");
			e.printStackTrace();
		}

		return result;
	}

	public int read(String vin) throws TechnicalException, SQLException {

		PreparedStatement stmt_read_livefleet_position = null;
		ResultSet rs_position = null;
		int distance_until_next_service = 0;

		try {

			if (null != vin && null != (connection = getConnection())) {

				stmt_read_livefleet_position = connection.prepareStatement(READ_LIVEFLEET_POSITION);
				stmt_read_livefleet_position.setString(1, vin);

				rs_position = stmt_read_livefleet_position.executeQuery();
				System.out.println("inside livefleet position");
				while (rs_position.next()) {

					distance_until_next_service = rs_position.getInt("distance_until_next_service");

				}
				System.out.println("outside livefleet position while loop");
				rs_position.close();
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
		return distance_until_next_service;
	}

	private PreparedStatement fillStatement(PreparedStatement stmt_insert_livefleet_position, Monitor row)
			throws SQLException {
		int varVEvtid = 0;
		if (row.getVEvtID() != null) {

			varVEvtid = row.getVEvtID();
		}

		double varGPSLongi = 0;
		if (row.getGpsLongitude() != null) {
			varGPSLongi = row.getGpsLongitude();
		}

		long varReceievedTimeStamp = 0;
		if (row.getReceivedTimestamp() != null) {

			varReceievedTimeStamp = row.getReceivedTimestamp();
		}

		String varTripID = row.getDocument().getTripID();

		if (varTripID != null) {
			stmt_insert_livefleet_position.setString(1, row.getDocument().getTripID()); // tripID
		}

		else if (row.getRoName() != null) {
			stmt_insert_livefleet_position.setString(1, row.getRoName());
		} else {
			stmt_insert_livefleet_position.setString(1, "TRIP_ID NOT AVAILABLE");
		}

		if (row.getVin() != null)
			stmt_insert_livefleet_position.setString(2, (String) row.getVin()); // vin
		else if (row.getVid() != null) {
			stmt_insert_livefleet_position.setString(2, (String) row.getVid());
		} else {
			stmt_insert_livefleet_position.setString(2, "VIN NOT AVAILABLE");
		}

		if (varReceievedTimeStamp != 0) {
			stmt_insert_livefleet_position.setLong(3, row.getReceivedTimestamp()); // message_time_stamp
			stmt_insert_livefleet_position.setLong(12, row.getReceivedTimestamp()); // created_at_m2m
			stmt_insert_livefleet_position.setLong(13, row.getReceivedTimestamp()); // created_at_kafka
			stmt_insert_livefleet_position.setLong(14, row.getReceivedTimestamp()); // created_at_dm
		}

		else {
			stmt_insert_livefleet_position.setLong(3, 0); // message_time_stamp
			stmt_insert_livefleet_position.setLong(12, 0); // created_at_m2m
			stmt_insert_livefleet_position.setLong(13, 0); // created_at_kafka
			stmt_insert_livefleet_position.setLong(14, 0); // created_at_dm
		}

		if (varGPSLongi == 255.0) {

			stmt_insert_livefleet_position.setDouble(4, 255.0);
			stmt_insert_livefleet_position.setDouble(5, 255.0);
			stmt_insert_livefleet_position.setDouble(6, 255.0);
			stmt_insert_livefleet_position.setDouble(7, 255.0);

		} else {

			if (row.getGpsAltitude() != null)
				stmt_insert_livefleet_position.setDouble(4, row.getGpsAltitude());
			else
				stmt_insert_livefleet_position.setDouble(4, 0);
			if (row.getGpsHeading() != null)
				stmt_insert_livefleet_position.setDouble(5, row.getGpsHeading());
			else
				stmt_insert_livefleet_position.setDouble(5, 0);
			if (row.getGpsLatitude() != null)
				stmt_insert_livefleet_position.setDouble(6, row.getGpsLatitude());
			else
				stmt_insert_livefleet_position.setDouble(6, 0);
			if (row.getGpsLongitude() != null)
				stmt_insert_livefleet_position.setDouble(7, row.getGpsLongitude());
			else
				stmt_insert_livefleet_position.setDouble(7, 0);

		}

		if (row.getDocument().getVFuelLevel1() != null)
			stmt_insert_livefleet_position.setDouble(8, row.getDocument().getVFuelLevel1()); // CO2 Emission
																								// Have to apply formula
		else
			stmt_insert_livefleet_position.setDouble(8, 0); // CO2 Emission //Have to apply formula

		if (row.getDocument().getVFuelLevel1() != null)
			stmt_insert_livefleet_position.setDouble(9, row.getDocument().getVFuelLevel1()); // fuel_consumption
		else
			stmt_insert_livefleet_position.setDouble(9, 0); // fuel_consumption

		if (varVEvtid == 26 || varVEvtid == 28 || varVEvtid == 29 || varVEvtid == 32 || varVEvtid == 42
				|| varVEvtid == 43 || varVEvtid == 44 || varVEvtid == 45 || varVEvtid == 46) {
			stmt_insert_livefleet_position.setLong(10, row.getDocument().getVTachographSpeed()); // TotalTachoMileage
		} else {
			stmt_insert_livefleet_position.setLong(10, 0);
		}

		if (varVEvtid == 42 || varVEvtid == 43) {
			stmt_insert_livefleet_position.setLong(11, row.getDocument().getVDistanceUntilService());// distance_until_next_service

		} else {
			stmt_insert_livefleet_position.setLong(11, 0);
		}

		return stmt_insert_livefleet_position;
	}

	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}

}
