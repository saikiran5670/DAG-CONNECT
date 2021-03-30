package net.atos.daf.ct2.common.realtime.postgresql;

import java.io.Serializable;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.util.ArrayList;
import java.util.List;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.common.realtime.dataprocess.IndexDataProcess;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.postgre.bo.CurrentTrip;
import net.atos.daf.postgre.bo.Trip;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.LiveFleetDriverActivityDao;
import net.atos.daf.postgre.dao.LiveFleetPosition;
import net.atos.daf.postgre.dao.LivefleetCurrentTripStatisticsDao;

@SuppressWarnings({})
public class LiveFleetCurrentTripPostgreSink extends RichSinkFunction<KafkaRecord<Index>> implements Serializable {
	/*
	 * This class is used to write Index message data in a postgres table.
	 */

	private static Logger log = LoggerFactory.getLogger(IndexDataProcess.class);

	private static final long serialVersionUID = 1L;

	Connection connection = null;
	//Connection connection2 = null;

	String livefleettrip = null;
	String readtrip = null;
	String readposition = null;

	private List<Index> queue = new ArrayList<Index>();
	private List<Index> synchronizedCopy = new ArrayList<Index>();

	LivefleetCurrentTripStatisticsDao currentTripDAO;
	LiveFleetPosition positionDAO;

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

		// this function is used to set up a connection to postgres table

		log.info("########## In LiveFleet current trip statistics ##############");

		currentTripDAO = new LivefleetCurrentTripStatisticsDao();
		positionDAO = new LiveFleetPosition();

		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		livefleettrip = envParams.get(DafConstants.QUERY_LIVEFLEET_TRIP_STATISTICS);

		readtrip = envParams.get(DafConstants.QUERY_LIVEFLEET_TRIP_READ);
		readposition = envParams.get(DafConstants.QUERY_LIVEFLEET_POSITION_READ);

		System.out.println("livefleettrip --> " + livefleettrip);
		System.out.println("livefleettripread --> " + readtrip);
		System.out.println("livefleetposition --> " + readposition);

		try {
			System.out.println("in LiveFleet Current TRip Statstics open try");

			connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(DafConstants.DATAMART_POSTGRE_USER),
					envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));
			currentTripDAO.setConnection(connection);
			positionDAO.setConnection(connection);
			
			
			/*connection2 = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(DafConstants.DATAMART_POSTGRE_USER),
					envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));
			positionDAO.setConnection(connection2);*/
			System.out.println("After Connection of LiveFleet Current TRip Statstics");

		} catch (Exception e) {
			log.error("Error in LiveFleet Current TRip Statstics" + e.getMessage());
			e.printStackTrace();

		}

	}

	public void invoke(KafkaRecord<Index> index) throws Exception {
		// this function is used to write data into postgres table

		// Live Fleet CURRENT TRIP Activity
		Index row = index.getValue();
		
		System.out.println("in current trip Invoke above try");

		try {
			queue.add(row);
			if (queue.size() >= 1) {
				System.out.println("inside current trip syncronized");
				synchronized (synchronizedCopy) {
					synchronizedCopy = new ArrayList<Index>(queue);
					queue.clear();
					
					System.out.println("In current trip INVOKE");
					for (Index indexValue : synchronizedCopy) {
						
						int varVEvtid = indexValue.getVEvtID();
						if (varVEvtid != 4) {

							CurrentTrip current_trip_start_var = currentTripDAO
									.read(index.getValue().getDocument().getTripID() ,DafConstants.CURRENT_TRIP_INDICATOR );

							indexValue.setReceivedTimestamp(current_trip_start_var.getStart_time_stamp());

							indexValue.setGpsLongitude(current_trip_start_var.getStart_position_longitude());

							indexValue.setGpsLatitude(current_trip_start_var.getStart_position_lattitude());

						}
						//make it Integer
						int distance_until_next_service = positionDAO.read(index.getValue().getVin());

						currentTripDAO.insert(indexValue, distance_until_next_service);
						// jPAPostgreDao.saveTripDetails(synchronizedCopy);
						System.out.println(" current trip save done");
						// System.out.println("anshu1");
						
					}
					
					

				}
			}
		} catch (Exception e) {
			e.printStackTrace();
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
	 */

	/*
	 * private static String encodeValue(String value) { try { return
	 * URLEncoder.encode(value, StandardCharsets.UTF_8.toString()); } catch
	 * (UnsupportedEncodingException ex) { throw new
	 * RuntimeException(ex.getCause()); } }
	 */

	@Override
	public void close() throws Exception {

		connection.close();

		log.error("Error");

		log.info("In Close");

	}

}
