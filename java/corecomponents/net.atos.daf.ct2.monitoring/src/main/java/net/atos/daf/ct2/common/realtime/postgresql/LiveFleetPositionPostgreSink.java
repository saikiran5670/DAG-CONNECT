package net.atos.daf.ct2.common.realtime.postgresql;

import java.io.Serializable;
import java.sql.Connection;
import java.text.ParseException;
import java.util.ArrayList;
import java.util.List;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.common.realtime.dataprocess.MonitorDataProcess;
//import net.atos.daf.ct2.common.realtime.pojo.monitordata.MonitorMessage;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.postgre.bo.Co2Master;
import net.atos.daf.postgre.bo.CurrentTrip;
import net.atos.daf.postgre.bo.LiveFleetPojo;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.Co2MasterDao;
import net.atos.daf.postgre.dao.LiveFleetPosition;
import net.atos.daf.postgre.dao.LivefleetCurrentTripStatisticsDao;

public class LiveFleetPositionPostgreSink extends RichSinkFunction<KafkaRecord<Monitor>> implements Serializable {/*

	private static final long serialVersionUID = 1L;
	Logger log = LoggerFactory.getLogger(MonitorDataProcess.class);

	// String livefleetposition = "INSERT INTO
	// livefleet.livefleet_position_statistics ( trip_id , vin
	// ,message_time_stamp
	// ,gps_altitude ,gps_heading ,gps_latitude ,gps_longitude ,co2_emission
	// ,fuel_consumption , last_odometer_val ,distance_until_next_service ,
	// created_at_m2m ,created_at_kafka ,created_at_dm ) VALUES (? ,? ,? ,? ,?
	// ,? ,?
	// ,? ,? ,? ,? ,? ,? ,? )";

	String livefleetposition = null;

	// private PreparedStatement statement;
	Connection connection = null;

	Connection masterConnection = null;
	LiveFleetPosition positionDAO;
	LivefleetCurrentTripStatisticsDao currentTripDAO;
	Co2MasterDao cmDAO;
	Co2Master cmData;

	LiveFleetPojo currentPosition;// later we have to implement it inside invoke

	private List<Monitor> queue;
	private List<Monitor> synchronizedCopy;

	Long Fuel_consumption = 0L;

	public void invoke(KafkaRecord<Monitor> monitor) throws Exception {

		queue = new ArrayList<Monitor>();
		synchronizedCopy = new ArrayList<Monitor>();

		Monitor row = monitor.getValue();

		currentPosition = new LiveFleetPojo();

		try {

			queue.add(row);
			
			cmData = cmDAO.read(row.getVid()); // CO2 coefficient data read from
												// master table
			
			if (queue.size() >= 1) {

				synchronized (synchronizedCopy) {
					synchronizedCopy = new ArrayList<Monitor>(queue);
					queue.clear();
					for (Monitor monitorData : synchronizedCopy) {
						String tripID = "NOT AVAILABLE";

						if (monitorData.getDocument().getTripID() != null) {
							tripID = monitorData.getDocument().getTripID();
						}
						
						CurrentTrip currentTripData = currentTripDAO.read(tripID,
								DafConstants.FUEL_CONSUMPTION_INDICATOR);
						// Co2Master cmData = cmDAO.read();

						Fuel_consumption = currentTripData.getFuel_consumption();
						currentPosition = tripCalculation(row, currentPosition);

						positionDAO.insert(currentPosition);

					}
				}
			}
		} catch (Exception e) {
			e.printStackTrace();
		}

	}

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

		log.info("########## In LiveFleet Position ##############");
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		livefleetposition = envParams.get(DafConstants.QUERY_LIVEFLEET_POSITION);
		positionDAO = new LiveFleetPosition();
		currentTripDAO = new LivefleetCurrentTripStatisticsDao();
		cmDAO = new Co2MasterDao();

		try {

			connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(DafConstants.DATAMART_POSTGRE_USER),
					envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));

			positionDAO.setConnection(connection);

			masterConnection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(DafConstants.MASTER_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(DafConstants.MASTER_POSTGRE_PORT)),
					envParams.get(DafConstants.MASTER_POSTGRE_DATABASE_NAME),
					envParams.get(DafConstants.MASTER_POSTGRE_USER),
					envParams.get(DafConstants.MASTER_POSTGRE_PASSWORD));
			cmDAO.setConnection(masterConnection);

		} catch (Exception e) {

			log.error("Error in Live fleet position" + e.getMessage());

		}

	}

	public LiveFleetPojo tripCalculation(Monitor row, LiveFleetPojo currentPosition) {

		int varVEvtid = 0;
		if (row.getVEvtID() != null) {
			varVEvtid = row.getVEvtID();
		}

		double varGPSLongi = 0;
		if (row.getGpsLongitude() != null) {
			varGPSLongi = row.getGpsLongitude();
		}
		currentPosition.setTripId(row.getDocument().getTripID());
		currentPosition.setVid(row.getVid());
		currentPosition.setVin(row.getVin());

		try {
			currentPosition.setMessageTimestamp((double) TimeFormatter.getInstance()
					.convertUTCToEpochMilli(row.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));
		} catch (ParseException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		currentPosition.setCreated_at_m2m(row.getReceivedTimestamp());
		currentPosition.setCreated_at_kafka(Long.parseLong(row.getKafkaProcessingTS()));
		currentPosition.setCreated_at_dm(TimeFormatter.getInstance().getCurrentUTCTimeInSec());

		if (varGPSLongi == 255.0) {
			currentPosition.setGpsAltitude(255.0); // gpsAltitude
			currentPosition.setGpsHeading(255.0); // gpsHeading
			currentPosition.setGpsLatitude(255.0); // gpsLatitude
			currentPosition.setGpsLongitude(255.0); // gpsLongitude
		} else {
			if (row.getGpsAltitude() != null) {
				currentPosition.setGpsAltitude(row.getGpsAltitude().doubleValue());
			} else {
				currentPosition.setGpsAltitude(0.0);
			}
			if (row.getGpsHeading() != null) {
				currentPosition.setGpsHeading(row.getGpsHeading().doubleValue());
			} else {
				currentPosition.setGpsHeading(0.0);
			}
			if (row.getGpsLatitude() != null) {
				currentPosition.setGpsLatitude(row.getGpsLatitude().doubleValue());
			} else {
				currentPosition.setGpsLatitude(0.0);
			}
			if (row.getGpsLongitude() != null) {
				currentPosition.setGpsLongitude(row.getGpsLongitude().doubleValue());
			} else {
				currentPosition.setGpsLongitude(0.0);
			}
		}

		if (Fuel_consumption != null) {
			double co2emission = (Fuel_consumption * cmData.getCoefficient()) / 1000;
			currentPosition.setCo2Emission(co2emission); // co2emission
			currentPosition.setFuelConsumption(Fuel_consumption.doubleValue());// fuel_consumption
		} else {
			currentPosition.setCo2Emission(0.0); // co2 emission
			currentPosition.setFuelConsumption(0.0); // fuel_consumption
		}

		if (varVEvtid == 26 || varVEvtid == 28 || varVEvtid == 29 || varVEvtid == 32 || varVEvtid == 42
				|| varVEvtid == 43 || varVEvtid == 44 || varVEvtid == 45 || varVEvtid == 46) {
			currentPosition.setLastOdometerValue(row.getDocument().getVTachographSpeed());// TotalTachoMileage
		} else {
			currentPosition.setLastOdometerValue(0);
		}

		if (varVEvtid == 42 || varVEvtid == 43) {
			currentPosition.setDistUntilNextService(row.getDocument().getVDistanceUntilService());// distance_until_next_service
		} else {
			currentPosition.setDistUntilNextService(0);
		}

		return currentPosition;

	}

	@Override
	public void close() throws Exception {

		connection.close();
		masterConnection.close();

		log.info("In Close");

	}

*/}