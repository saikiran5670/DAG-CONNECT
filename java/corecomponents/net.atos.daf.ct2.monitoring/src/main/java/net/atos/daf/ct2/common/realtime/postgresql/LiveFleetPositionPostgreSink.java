package net.atos.daf.ct2.common.realtime.postgresql;

import java.io.Serializable;
import java.sql.Connection;
import java.util.ArrayList;
import java.util.List;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.common.realtime.dataprocess.MonitorDataProcess;
//import net.atos.daf.ct2.common.realtime.pojo.monitordata.MonitorMessage;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.postgre.bo.Co2Master;
import net.atos.daf.postgre.bo.CurrentTrip;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.Co2MasterDao;
import net.atos.daf.postgre.dao.LiveFleetPosition;
import net.atos.daf.postgre.dao.LivefleetCurrentTripStatisticsDao;

public class LiveFleetPositionPostgreSink extends RichSinkFunction<KafkaRecord<Monitor>> implements Serializable {

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

	

	private List<Monitor> queue;
	private List<Monitor> synchronizedCopy;

	public void invoke(KafkaRecord<Monitor> monitor) throws Exception {
		
		
		queue = new ArrayList<Monitor>();
		synchronizedCopy = new ArrayList<Monitor>();

		Monitor row = monitor.getValue();

		try {
			queue.add(row);
			Co2Master cmData = cmDAO.read(); //CO2 coefficient data read from master table 
			if (queue.size() >= 1) {
				
				System.out.println("inside syncronized");
				synchronized (synchronizedCopy) {
					synchronizedCopy = new ArrayList<Monitor>(queue);
					queue.clear();
					for (Monitor monitorData : synchronizedCopy) {
						String tripID = "NOT AVAILABLE";

						if (monitorData.getDocument().getTripID() != null) {
							tripID = monitorData.getDocument().getTripID();
						}
						System.out.println("tripID -- >  " + tripID);
						System.out.println("DafConstants.FUEL_CONSUMPTION_INDICATOR --> "
								+ DafConstants.FUEL_CONSUMPTION_INDICATOR);
						CurrentTrip currentTripData = currentTripDAO.read(tripID, DafConstants.FUEL_CONSUMPTION_INDICATOR);
						//Co2Master cmData = cmDAO.read();

						Long Fuel_consumption = currentTripData.getFuel_consumption();
						positionDAO.insert(monitorData, Fuel_consumption, cmData);
						//positionDAO.insert(monitorData, 100l, cmData);
						// jPAPostgreDao.saveTripDetails(synchronizedCopy);
						System.out.println("monitor save done");
						// System.out.println("anshu1");
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

	/*
	 * private String createValidUrlToConnectPostgreSql(String serverNm, int
	 * port, String databaseNm, String userNm, String password) throws Exception
	 * {
	 * 
	 * String encodedPassword = encodeValue(password); String url = serverNm +
	 * ":" + port + "/" + databaseNm + "?" + "user=" + userNm + "&" +
	 * "password=" + encodedPassword + DafConstants.POSTGRE_SQL_SSL_MODE;
	 * 
	 * log.info("Valid Url = " + url);
	 * 
	 * return url; }
	 * 
	 * private static String encodeValue(String value) { try { return
	 * URLEncoder.encode(value, StandardCharsets.UTF_8.toString()); } catch
	 * (UnsupportedEncodingException ex) { throw new
	 * RuntimeException(ex.getCause()); } }
	 */

	@Override
	public void close() throws Exception {

		connection.close();
		masterConnection.close();

		log.info("In Close");

	}

}