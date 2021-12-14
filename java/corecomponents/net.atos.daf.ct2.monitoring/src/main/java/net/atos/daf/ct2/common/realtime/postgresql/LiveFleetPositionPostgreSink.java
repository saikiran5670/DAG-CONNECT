package net.atos.daf.ct2.common.realtime.postgresql;

import static net.atos.daf.ct2.common.util.Utils.convertDateToMillis;
import static net.atos.daf.ct2.common.util.Utils.getCurrentTimeInUTC;

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
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.postgre.bo.Co2Master;
import net.atos.daf.postgre.bo.CurrentTrip;
import net.atos.daf.postgre.bo.LiveFleetPojo;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.Co2MasterDao;
import net.atos.daf.postgre.dao.LiveFleetPosition;
import net.atos.daf.postgre.dao.LivefleetCurrentTripStatisticsDao;

public class LiveFleetPositionPostgreSink extends RichSinkFunction<KafkaRecord<Monitor>> implements Serializable {

	private static final long serialVersionUID = 1L;
	Logger logger = LoggerFactory.getLogger(LiveFleetPositionPostgreSink.class);

	String livefleetposition = null;
	Connection connection = null;
	Connection masterConnection = null;
	LiveFleetPosition positionDAO;
	LivefleetCurrentTripStatisticsDao currentTripDAO;
	Co2MasterDao cmDAO;
	Co2Master cmData;

	LiveFleetPojo currentPosition;// later we have to implement it inside invoke
	Long Fuel_consumption = 0L;

	public void invoke(KafkaRecord<Monitor> monitor) throws Exception {




		Monitor row = monitor.getValue();
		Integer messageEight = new Integer(8);
		if (messageEight.equals(row.getMessageType()) && (row.getVEvtID() == 26 )) {

			try {

				currentPosition= new LiveFleetPojo();

				currentPosition = tripCalculation(row);

				positionDAO.insert(currentPosition);

				logger.info("Monitoring :: Live rFMS Monitoring record inserted successfully");

			} catch (Exception e) {
				logger.info("Monitoring ::  Live rFMS Monitoring record insertion failed !!! {}",e);
			}
		}

	}

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

		logger.info("########## In Monitoring: LiveFleet Position  ##############");
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

			/*	masterConnection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(DafConstants.MASTER_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(DafConstants.MASTER_POSTGRE_PORT)),
					envParams.get(DafConstants.MASTER_POSTGRE_DATABASE_NAME),
					envParams.get(DafConstants.MASTER_POSTGRE_USER),
					envParams.get(DafConstants.MASTER_POSTGRE_PASSWORD));
			cmDAO.setConnection(masterConnection); */

		} catch (Exception e) {

			logger.error("Error in Monitoring :: Live fleet position " + e.getMessage());

		}

	}

	public LiveFleetPojo tripCalculation(Monitor row) {

		double varGPSLongi = 0;
		if (row.getGpsLongitude() != null) {
			varGPSLongi = row.getGpsLongitude();
		}
		//currentPosition.setTripId(row.getDocument().getTripID());
		currentPosition.setVid(row.getVid());
		currentPosition.setVin(row.getVin());
		currentPosition.setMessageTimestamp((double)convertDateToMillis(row.getEvtDateTime()));
		currentPosition.setCreated_at_m2m(row.getReceivedTimestamp());
		currentPosition.setCreated_at_kafka(Long.parseLong(row.getKafkaProcessingTS()));
		currentPosition.setCreated_at_dm(getCurrentTimeInUTC());

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
		

		currentPosition.setVehMessageType(DafConstants.Monitor);

		currentPosition.setVehicleMsgTriggerTypeId(row.getVEvtID());
		currentPosition.setCreatedDatetime(convertDateToMillis(row.getEvtDateTime()));
		currentPosition.setReceivedDatetime(row.getReceivedTimestamp());

		if (row.getDocument().getGpsSpeed() != null)
			currentPosition.setGpsSpeed(row.getDocument().getGpsSpeed().doubleValue());

		if (row.getGpsDateTime() != null) {
			currentPosition.setGpsDatetime(convertDateToMillis(row.getGpsDateTime()));
		}
		
		if (row.getDocument().getVWheelBasedSpeed() != null)
			currentPosition.setWheelbasedSpeed(row.getDocument().getVWheelBasedSpeed().doubleValue());
				
		if (row.getDocument().getVDist() != null) {
			currentPosition.setLastOdometerValue(row.getDocument().getVDist().intValue());
		} else {
			currentPosition.setLastOdometerValue(null);
		}

		
		//vehicle status API fields
		currentPosition.setTotal_vehicle_distance(row.getDocument().getVDist());
		//currentPosition.setTotal_engine_hours(row.getDocument().getVEngineTotalHours());
		//currentPosition.setTotal_engine_fuel_used(row.getDocument().getVCumulatedFuel());
		//currentPosition.setGross_combination_vehicle_weight(row.getDocument().getVGrossWeightCombination());
		//currentPosition.setEngine_speed(row.getDocument().getVEngineSpeed());
		currentPosition.setFuel_level1(row.getDocument().getVFuelLevel1());
		currentPosition.setCatalyst_fuel_level(row.getDocument().getVDEFTankLevel());
		//currentPosition.setDriver2_id(row.getDocument().getDriver2ID());
		//currentPosition.setDriver1_working_state(row.getDocument().getDriver1WorkingState());
		//currentPosition.setDriver2_working_state(row.getDocument().getDriver2WorkingState());
		//currentPosition.setAmbient_air_temperature(row.getDocument().getVAmbiantAirTemperature());
		currentPosition.setEngine_coolant_temperature(row.getDocument().getVEngineCoolantTemperature());
		currentPosition.setService_brake_air_pressure_circuit1(row.getDocument().getVServiceBrakeAirPressure1());
		currentPosition.setService_brake_air_pressure_circuit2(row.getDocument().getVServiceBrakeAirPressure2());
		
		
		
		
		currentPosition.setDriverAuthEquipmentTypeId(DafConstants.DRIVER_CARD);
		currentPosition.setDriver2AuthEquipmentTypeId(DafConstants.DRIVER_CARD);
		currentPosition.setPtoId(DafConstants.PTO_ID);
		currentPosition.setTelltaleId(row.getDocument().getTtId());
		currentPosition.setTelltaleStateId(row.getDocument().getTtValue());
		//currentPosition.setCreated_at_dm(Fuel_consumption)
		
		logger.info("Monitoring :: rFMS Monitoring calculated (to be inserted) ::\n" + currentPosition);
		
		return currentPosition;

	}

	@Override
	public void close() throws Exception {
		if (connection != null) {
			logger.debug("Releasing connection from rFMS Monitoring job");
			connection.close();
		}
		
		if (masterConnection != null) {
			logger.debug("Releasing connection from rFMS Monitoring job");
			masterConnection.close();
		}
	}

}