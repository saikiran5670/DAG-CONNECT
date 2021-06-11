package net.atos.daf.postgre.dao;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.postgre.bo.Co2Master;
import net.atos.daf.postgre.bo.LiveFleetPojo;
import net.atos.daf.postgre.util.DafConstants;

public class LiveFleetPosition implements Serializable {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private Connection connection;

	private static final String READ_LIVEFLEET_POSITION = "SELECT distance_until_next_service from livefleet.livefleet_position_statistics WHERE vin = ? ORDER BY created_at_m2m DESC limit 1";
	private static final String INSERT_LIVEFLEET_POSITION = "INSERT INTO livefleet.livefleet_position_statistics ( trip_id    , vin    ,message_time_stamp    ,gps_altitude    ,gps_heading    ,gps_latitude    ,gps_longitude    ,co2_emission    ,fuel_consumption    , last_odometer_val  ,distance_until_next_service    , created_at_m2m    ,created_at_kafka    ,created_at_dm    ) VALUES (?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    ,?    )";

		public boolean insert(LiveFleetPojo currentPosition)
			throws TechnicalException, SQLException {
		PreparedStatement stmt_insert_livefleet_position;

		boolean result = false;
	
		try {

			if (null != currentPosition && null != (connection = getConnection())) {
				stmt_insert_livefleet_position = connection.prepareStatement(INSERT_LIVEFLEET_POSITION);

				stmt_insert_livefleet_position = fillStatement(stmt_insert_livefleet_position, currentPosition);
				
				stmt_insert_livefleet_position.addBatch();
				stmt_insert_livefleet_position.executeBatch();
			}
		} catch (SQLException e) {
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
				
				while (rs_position.next()) {

					distance_until_next_service = rs_position.getInt("distance_until_next_service");

				}
				
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

	private PreparedStatement fillStatement(PreparedStatement stmt_insert_livefleet_position,
			LiveFleetPojo currentPosition) throws SQLException {

		if (currentPosition.getTripId() != null)
			stmt_insert_livefleet_position.setString(1, currentPosition.getTripId());
		else
			stmt_insert_livefleet_position.setString(1, DafConstants.UNKNOWN);

		if (currentPosition.getVin() != null)
			stmt_insert_livefleet_position.setString(2, currentPosition.getVin());
		else
			stmt_insert_livefleet_position.setString(2, currentPosition.getVid());

		if (currentPosition.getMessageTimestamp() != null)
			stmt_insert_livefleet_position.setDouble(3, currentPosition.getMessageTimestamp());
		else
			stmt_insert_livefleet_position.setDouble(3, 0);

		if (currentPosition.getGpsAltitude() != null)
			stmt_insert_livefleet_position.setDouble(4, currentPosition.getGpsAltitude());
		else
			stmt_insert_livefleet_position.setDouble(4, 0);

		if (currentPosition.getGpsHeading() != null)
			stmt_insert_livefleet_position.setDouble(5, currentPosition.getGpsHeading());
		else
			stmt_insert_livefleet_position.setDouble(5, 0);

		if (currentPosition.getGpsLatitude() != null)
			stmt_insert_livefleet_position.setDouble(6, currentPosition.getGpsLatitude());
		else
			stmt_insert_livefleet_position.setDouble(6, 0);

		if (currentPosition.getGpsLongitude() != null)
			stmt_insert_livefleet_position.setDouble(7, currentPosition.getGpsLongitude());
		else
			stmt_insert_livefleet_position.setDouble(7, 0);

		if (currentPosition.getCo2Emission() != null)
			stmt_insert_livefleet_position.setDouble(8, currentPosition.getCo2Emission());
		else
			stmt_insert_livefleet_position.setDouble(8, 0);

		if (currentPosition.getFuelConsumption() != null)
			stmt_insert_livefleet_position.setDouble(9, currentPosition.getFuelConsumption());
		else
			stmt_insert_livefleet_position.setDouble(9, 0);

		if (currentPosition.getLastOdometerValue() != null)
			stmt_insert_livefleet_position.setDouble(10, currentPosition.getLastOdometerValue());
		else
			stmt_insert_livefleet_position.setDouble(10, 0);

		if (currentPosition.getDistUntilNextService() != null)
			stmt_insert_livefleet_position.setDouble(11, currentPosition.getDistUntilNextService());
		else
			stmt_insert_livefleet_position.setDouble(11, 0);

		if (currentPosition.getCreated_at_m2m() != null)
			stmt_insert_livefleet_position.setDouble(12, currentPosition.getCreated_at_m2m());
		else
			stmt_insert_livefleet_position.setDouble(12, 0);

		if (currentPosition.getCreated_at_kafka() != null)
			stmt_insert_livefleet_position.setDouble(13, currentPosition.getCreated_at_kafka());
		else
			stmt_insert_livefleet_position.setDouble(13, 0);

		if (currentPosition.getCreated_at_kafka() != null)
			stmt_insert_livefleet_position.setDouble(14, currentPosition.getCreated_at_dm());
		else
			stmt_insert_livefleet_position.setDouble(14, 0);

		return stmt_insert_livefleet_position;
	}

	/*
	 * private PreparedStatement fillStatement(PreparedStatement
	 * stmt_insert_livefleet_position, Monitor row, Long fuel_consumption,
	 * Co2Master cm) throws SQLException { int varVEvtid = 0; if
	 * (row.getVEvtID() != null) {
	 * 
	 * varVEvtid = row.getVEvtID(); }
	 * 
	 * double varGPSLongi = 0; if (row.getGpsLongitude() != null) { varGPSLongi
	 * = row.getGpsLongitude(); }
	 * 
	 * long varReceievedTimeStamp = 0; if (row.getReceivedTimestamp() != null) {
	 * 
	 * varReceievedTimeStamp = row.getReceivedTimestamp(); }
	 * 
	 * String varTripID = row.getDocument().getTripID();
	 * 
	 * if (varTripID != null) { stmt_insert_livefleet_position.setString(1,
	 * row.getDocument().getTripID()); // tripID }
	 * 
	 * else if (row.getRoName() != null) {
	 * stmt_insert_livefleet_position.setString(1, row.getRoName()); } else {
	 * stmt_insert_livefleet_position.setString(1, "TRIP_ID NOT AVAILABLE"); }
	 * 
	 * if (row.getVin() != null) stmt_insert_livefleet_position.setString(2,
	 * (String) row.getVin()); // vin else if (row.getVid() != null) {
	 * stmt_insert_livefleet_position.setString(2, (String) row.getVid()); }
	 * else { stmt_insert_livefleet_position.setString(2, "VIN NOT AVAILABLE");
	 * }
	 * 
	 * if (varReceievedTimeStamp != 0) {
	 * stmt_insert_livefleet_position.setLong(3, row.getReceivedTimestamp()); //
	 * message_time_stamp stmt_insert_livefleet_position.setLong(12,
	 * row.getReceivedTimestamp()); // created_at_m2m
	 * stmt_insert_livefleet_position.setLong(13, row.getReceivedTimestamp());
	 * // created_at_kafka stmt_insert_livefleet_position.setLong(14,
	 * row.getReceivedTimestamp()); // created_at_dm }
	 * 
	 * else { stmt_insert_livefleet_position.setLong(3, 0); //
	 * message_time_stamp stmt_insert_livefleet_position.setLong(12, 0); //
	 * created_at_m2m stmt_insert_livefleet_position.setLong(13, 0); //
	 * created_at_kafka stmt_insert_livefleet_position.setLong(14, 0); //
	 * created_at_dm }
	 * 
	 * if (varGPSLongi == 255.0) {
	 * 
	 * stmt_insert_livefleet_position.setDouble(4, 255.0);
	 * stmt_insert_livefleet_position.setDouble(5, 255.0);
	 * stmt_insert_livefleet_position.setDouble(6, 255.0);
	 * stmt_insert_livefleet_position.setDouble(7, 255.0);
	 * 
	 * } else {
	 * 
	 * if (row.getGpsAltitude() != null)
	 * stmt_insert_livefleet_position.setDouble(4, row.getGpsAltitude()); else
	 * stmt_insert_livefleet_position.setDouble(4, 0); if (row.getGpsHeading()
	 * != null) stmt_insert_livefleet_position.setDouble(5,
	 * row.getGpsHeading()); else stmt_insert_livefleet_position.setDouble(5,
	 * 0); if (row.getGpsLatitude() != null)
	 * stmt_insert_livefleet_position.setDouble(6, row.getGpsLatitude()); else
	 * stmt_insert_livefleet_position.setDouble(6, 0); if (row.getGpsLongitude()
	 * != null) stmt_insert_livefleet_position.setDouble(7,
	 * row.getGpsLongitude()); else stmt_insert_livefleet_position.setDouble(7,
	 * 0);
	 * 
	 * }
	 * 
	 * if (fuel_consumption != null) {
	 * 
	 * double co2emission = (fuel_consumption * cm.getCoefficient_D()) / 1000;
	 * 
	 * System.out.println("  In fillStatement co2emission-- > " + co2emission);
	 * 
	 * stmt_insert_livefleet_position.setDouble(8, co2emission); // co2emission
	 * 
	 * System.out.println(" In fillStatement fuel_consumption --> " +
	 * fuel_consumption);
	 * 
	 * stmt_insert_livefleet_position.setDouble(9, fuel_consumption); //
	 * fuel_consumption
	 * 
	 * }
	 * 
	 * else {
	 * 
	 * stmt_insert_livefleet_position.setDouble(8, 0); // co2 emission
	 * 
	 * stmt_insert_livefleet_position.setDouble(9, 0); // fuel_consumption
	 * 
	 * }
	 * 
	 * if (varVEvtid == 26 || varVEvtid == 28 || varVEvtid == 29 || varVEvtid ==
	 * 32 || varVEvtid == 42 || varVEvtid == 43 || varVEvtid == 44 || varVEvtid
	 * == 45 || varVEvtid == 46) { stmt_insert_livefleet_position.setLong(10,
	 * row.getDocument().getVTachographSpeed()); // TotalTachoMileage } else {
	 * stmt_insert_livefleet_position.setLong(10, 0); }
	 * 
	 * if (varVEvtid == 42 || varVEvtid == 43) {
	 * stmt_insert_livefleet_position.setLong(11,
	 * row.getDocument().getVDistanceUntilService());//
	 * distance_until_next_service
	 * 
	 * } else { stmt_insert_livefleet_position.setLong(11, 0); }
	 * 
	 * return stmt_insert_livefleet_position; }
	 */
	public Connection getConnection() {
		return connection;
	}

	public void setConnection(Connection connection) {
		this.connection = connection;
	}

}
