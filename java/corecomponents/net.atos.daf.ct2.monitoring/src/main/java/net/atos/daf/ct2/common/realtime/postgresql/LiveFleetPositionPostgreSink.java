package net.atos.daf.ct2.common.realtime.postgresql;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
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
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.LiveFleetPosition;

public class LiveFleetPositionPostgreSink extends RichSinkFunction<KafkaRecord<Monitor>> implements Serializable {

	private static final long serialVersionUID = 1L;
	Logger log = LoggerFactory.getLogger(MonitorDataProcess.class);

	// String livefleetposition = "INSERT INTO
	// livefleet.livefleet_position_statistics ( trip_id , vin ,message_time_stamp
	// ,gps_altitude ,gps_heading ,gps_latitude ,gps_longitude ,co2_emission
	// ,fuel_consumption , last_odometer_val ,distance_until_next_service ,
	// created_at_m2m ,created_at_kafka ,created_at_dm ) VALUES (? ,? ,? ,? ,? ,? ,?
	// ,? ,? ,? ,? ,? ,? ,? )";

	String livefleetposition = null;

	//private PreparedStatement statement;
	Connection connection = null;
	LiveFleetPosition positionDAO;

	private List<Monitor> queue;
	private List<Monitor> synchronizedCopy;

	public void invoke(KafkaRecord<Monitor> monitor) throws Exception {
		queue = new ArrayList<Monitor>();
		synchronizedCopy = new ArrayList<Monitor>();

		Monitor row = monitor.getValue();

		try {
			queue.add(row);
			if (queue.size() >= 1) {
				System.out.println("inside syncronized");
				synchronized (synchronizedCopy) {
					synchronizedCopy = new ArrayList<Monitor>(queue);
					queue.clear();
					for (Monitor monitorData : synchronizedCopy) {
					positionDAO.insert(monitorData);
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
		try {

			connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(DafConstants.DATAMART_POSTGRE_USER),
					envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));

			positionDAO.setConnection(connection);

		} catch (Exception e) {

			log.error("Error in Live fleet position" + e.getMessage());

		}

	}

	/*
	 * private String createValidUrlToConnectPostgreSql(String serverNm, int port,
	 * String databaseNm, String userNm, String password) throws Exception {
	 * 
	 * String encodedPassword = encodeValue(password); String url = serverNm + ":" +
	 * port + "/" + databaseNm + "?" + "user=" + userNm + "&" + "password=" +
	 * encodedPassword + DafConstants.POSTGRE_SQL_SSL_MODE;
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

		log.info("In Close");

	}

}