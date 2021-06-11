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

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.common.realtime.dataprocess.IndexDataProcess;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.postgre.bo.CurrentTrip;
import net.atos.daf.postgre.bo.Trip;
import net.atos.daf.postgre.bo.TripStatisticsPojo;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.LiveFleetDriverActivityDao;
import net.atos.daf.postgre.dao.LiveFleetPosition;
import net.atos.daf.postgre.dao.LivefleetCurrentTripStatisticsDao;

//@SuppressWarnings({})
public class LiveFleetCurrentTripPostgreSink extends RichSinkFunction<KafkaRecord<Index>> implements Serializable {
	/*
	 * This class is used to write Index message data in a postgres table.
	 */

	private static Logger log = LoggerFactory.getLogger(IndexDataProcess.class);

	private static final long serialVersionUID = 1L;

	Connection connection = null;
	// Connection connection2 = null;

	String livefleettrip = null;
	String readtrip = null;
	String readposition = null;

	private List<Index> queue = new ArrayList<Index>();
	private List<Index> synchronizedCopy = new ArrayList<Index>();

	LivefleetCurrentTripStatisticsDao currentTripDAO;
	LiveFleetPosition positionDAO;
	TripStatisticsPojo currentTripPojo;

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

		try {

			connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(DafConstants.DATAMART_POSTGRE_USER),
					envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));
			currentTripDAO.setConnection(connection);
			positionDAO.setConnection(connection);

			/*
			 * connection2 = PostgreDataSourceConnection.getInstance().
			 * getDataSourceConnection(
			 * envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
			 * Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT
			 * )), envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
			 * envParams.get(DafConstants.DATAMART_POSTGRE_USER),
			 * envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));
			 * positionDAO.setConnection(connection2);
			 */
		} catch (Exception e) {
			log.error("Error in LiveFleet Current TRip Statstics" + e.getMessage());
			e.printStackTrace();

		}

	}

	public void invoke(KafkaRecord<Index> index) throws Exception {
		// this function is used to write data into postgres table

		// Live Fleet CURRENT TRIP Activity
		Index row = index.getValue();
		currentTripPojo = new TripStatisticsPojo();

		try {
			queue.add(row);
			if (queue.size() >= 1) {
				synchronized (synchronizedCopy) {
					synchronizedCopy = new ArrayList<Index>(queue);
					queue.clear();

					for (Index indexValue : synchronizedCopy) {

						currentTripPojo.setTripId(indexValue.getDocument().getTripID());
						currentTripPojo.setVid(indexValue.getVid());
						currentTripPojo.setVin(indexValue.getVin());

						currentTripPojo.setEnd_time_stamp(TimeFormatter.getInstance()
								.convertUTCToEpochMilli(row.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));
						currentTripPojo.setDriver1ID(indexValue.getDriverID());

						currentTripPojo.setStart_position("");
						currentTripPojo.setLast_recieved_position_lattitude(indexValue.getGpsLatitude());
						currentTripPojo.setLast_recieved_position_longitude(indexValue.getGpsLongitude());
						currentTripPojo.setLast_known_position("");

						Integer[] ttvalue = row.getDocument().getTt_ListValue(); // 12//
																					// vehicle
																					// status
						currentTripPojo.setVehicle_status(0);
						/*if (ttvalue.length == 0) {
							currentTripPojo.setVehicle_status(0);

						} else {
							int status = ttvalue[ttvalue.length - 1];

							if (status == 0) {
								currentTripPojo.setVehicle_status(2);
							}

							if (status == 1 || status == 2 || status == 3) {
								currentTripPojo.setVehicle_status(1);
							}

							if (status == 4 || status == 5 || status == 6) {
								currentTripPojo.setVehicle_status(3);
							}

							if (status == 7) {
								currentTripPojo.setVehicle_status(4);
							}
						}*/

						// currentTripPojo.setVehicle_status(null); //Look after
						currentTripPojo.setDriver1_status(indexValue.getDocument().getDriver1WorkingState());
						currentTripPojo.setVehicle_health_status(null); // it is
																		// not
																		// present
																		// in
																		// index
																		// message
																		// POJO

					
						//timebeing dummy value set
						//Integer[] tacho = row.getDocument().getTotalTachoMileage(); // 15
						currentTripPojo.setLast_odometer_val(0);
						// odometer_value

						/*if (tacho == null || tacho.length == 0) {
							currentTripPojo.setLast_odometer_val(0);
						} else {
							System.out.println("tacho.length-->" + tacho.length);
							System.out.println("tacho.length -1-->" + (tacho.length - 1));
							System.out.println("odometer_val-->" + tacho[tacho.length - 1]);
							if (null != tacho[tacho.length - 1]) {
								int odometer_val = tacho[tacho.length - 1];
								currentTripPojo.setLast_odometer_val(odometer_val);
							} else {
								currentTripPojo.setLast_odometer_val(0);
							}

						}*/

						currentTripPojo.setLast_processed_message_timestamp(
								TimeFormatter.getInstance().getCurrentUTCTimeInSec());
						currentTripPojo.setDriver2ID(indexValue.getDocument().getDriver2ID());
						currentTripPojo.setDriver2_status(indexValue.getDocument().getDriver2WorkingState());
						currentTripPojo.setCreated_at_m2m(indexValue.getReceivedTimestamp());
						currentTripPojo.setCreated_at_kafka(Long.parseLong(indexValue.getKafkaProcessingTS()));
						currentTripPojo.setCreated_at_dm(TimeFormatter.getInstance().getCurrentUTCTimeInSec());
						currentTripPojo.setModified_at(null);
						currentTripPojo.setFuel_consumption(indexValue.getVUsedFuel());

						int varVEvtid = indexValue.getVEvtID();
						if (varVEvtid != 4) {

							CurrentTrip current_trip_start_var = currentTripDAO.read(
									index.getValue().getDocument().getTripID(), DafConstants.CURRENT_TRIP_INDICATOR);

							currentTripPojo.setStart_time_stamp(current_trip_start_var.getStart_time_stamp());

							currentTripPojo
									.setStart_position_longitude(current_trip_start_var.getStart_position_longitude());

							currentTripPojo
									.setStart_position_lattitude(current_trip_start_var.getStart_position_lattitude());

						} else {

							currentTripPojo.setStart_time_stamp(TimeFormatter.getInstance().convertUTCToEpochMilli(
									row.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));
							currentTripPojo.setStart_position_lattitude(indexValue.getGpsLatitude());
							currentTripPojo.setStart_position_longitude(indexValue.getGpsLongitude());

						}

						int distance_until_next_service = positionDAO.read(index.getValue().getVin());

						currentTripPojo.setDistance_until_next_service(distance_until_next_service);

						currentTripDAO.insert(currentTripPojo, distance_until_next_service);
					}

				}
			}
		} catch (Exception e) {
			e.printStackTrace();
		}

	}

	@Override
	public void close() throws Exception {

		connection.close();

		log.error("Error");

		log.info("In Close");

	}

}
