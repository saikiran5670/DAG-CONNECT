package net.atos.daf.ct2.common.realtime.postgresql;

import java.io.Serializable;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.time.LocalDateTime;
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

		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		try {

			connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(DafConstants.DATAMART_POSTGRE_USER),
					envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));
			currentTripDAO.setConnection(connection);

		} catch (Exception e) {
			log.error("Error in LiveFleet Current TRip Statstics" + e.getMessage());
			e.printStackTrace();

		}

	}

	public void invoke(KafkaRecord<Index> index) {
		// this function is used to write data into postgres table

		// Live Fleet CURRENT TRIP Activity
		Index row = index.getValue();
		System.out.println("Invoke Started trip statistic Sink :: " + row);

		currentTripPojo = new TripStatisticsPojo();

		try {
			queue.add(row);
			if (queue.size() >= 1) {
				synchronized (synchronizedCopy) {
					synchronizedCopy = new ArrayList<Index>(queue);
					queue.clear();

					for (Index indexValue : synchronizedCopy) {

						System.out.println("INDEX-VALUE FOR CURRENT TRIP : " + indexValue);

						if (indexValue.getVin() != null)
							currentTripPojo.setVin(indexValue.getVin()); // not null
						else
							currentTripPojo.setVin(indexValue.getVid());

						try {

							currentTripPojo.setEnd_time_stamp(TimeFormatter.getInstance().convertUTCToEpochMilli(
									indexValue.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));
							currentTripPojo.setDriver1ID(indexValue.getDriverID()); // not null

							if (indexValue.getVUsedFuel() != null)
								currentTripPojo.setFuel_consumption(Long.valueOf(indexValue.getVUsedFuel().longValue()));
							
							if(indexValue.getVDist() != null)
								currentTripPojo.setOdometer_val(Long.valueOf(indexValue.getVDist().longValue()));

							if (indexValue.getDocument() != null) {
								currentTripPojo.setTripId(indexValue.getDocument().getTripID()); // not null
							}

							currentTripPojo.setDistance_until_next_service(null); // to be populated from monitoring

							currentTripPojo.setLast_received_position_lattitude(indexValue.getGpsLatitude());
							currentTripPojo.setLast_received_position_longitude(indexValue.getGpsLongitude());
							currentTripPojo.setLast_received_position_heading(indexValue.getGpsHeading());
							currentTripPojo.setLast_geolocation_address_id(null);
							currentTripPojo.setLast_processed_message_timestamp(
									TimeFormatter.getInstance().convertUTCToEpochMilli(
											indexValue.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));
							currentTripPojo.setStart_geolocation_address_id(null);
							
							// calculate the vehicle_driving_status_type
							long vWheelSpeed = 0;
							if (indexValue.getDocument() != null
									&& indexValue.getDocument().getVWheelBasedSpeed() != null)
								vWheelSpeed = indexValue.getDocument().getVWheelBasedSpeed().longValue();

							if (vWheelSpeed > 0)
								currentTripPojo.setVehicle_driving_status_type('D'); // DRIVING if wheelspeed >
																						// 0
							else if (vWheelSpeed == 0) {
								if (indexValue.getDocument() != null) {
									 if	(indexValue.getDocument().getVEngineSpeed() != null
										&& indexValue.getDocument().getVEngineSpeed().longValue() > 0)
									currentTripPojo.setVehicle_driving_status_type('I'); // IDLING if wheelspeed
																							// = 0 but
																							// enginespeed > 0
								else
									currentTripPojo.setVehicle_driving_status_type('S'); // STOPPED if
																							// wheelspped = 0
								}
								else 
									currentTripPojo.setVehicle_driving_status_type('S');
							}
							
							System.out.println(
									" aftr vWheelSpeed and vehicle_driving_status_type calculation, CURRENT TRIP POJO BEFORE UPDATE : " + currentTripPojo);

							
							currentTripPojo.setModified_at(TimeFormatter.getInstance().getCurrentUTCTime());

						} catch (Exception e) {
							System.out.println("catch in first exception modified" + e.getMessage());
							e.printStackTrace();
						}

						System.out.println("CURRENT TRIP POJO BEFORE VAREVTID = 4 CHECK : " + currentTripPojo
								+ " vevtId ::" + indexValue.getVEvtID());

						int varVEvtid = 0;

						if (indexValue.getVEvtID() != null)
							varVEvtid = indexValue.getVEvtID().intValue();

						System.out.println("varVEvtid" + varVEvtid);

						try {
							if (varVEvtid != 4) { // trip exists, so update trip start fields (coming from index message)
								
								CurrentTrip current_trip_start_var = null;

								if (indexValue.getDocument() != null) {
									if (indexValue.getDocument().getTripID() != null)
										current_trip_start_var = currentTripDAO
												.read(indexValue.getDocument().getTripID());
								}
								
								System.out.println("read obj current_trip_start_var :: " + current_trip_start_var);
								if (current_trip_start_var != null) {

									currentTripPojo.setStart_time_stamp(
											Long.valueOf(current_trip_start_var.getEnd_time_stamp()));
									currentTripPojo.setStart_position_lattitude(
											current_trip_start_var.getStart_position_lattitude());
									currentTripPojo.setStart_position_longitude(
											current_trip_start_var.getStart_position_longitude());
									currentTripPojo.setStart_position_heading(
											current_trip_start_var.getStart_position_heading());

									System.out
											.println(" aftr pos CURRENT TRIP POJO BEFORE UPDATE : " + currentTripPojo);
									// calculate the driving_time
									long driving_time = current_trip_start_var.getDriving_time();

									Long idleDuration=0L;
									if(indexValue.getVIdleDuration()!=null) {
										idleDuration=(indexValue.getVIdleDuration()) * 1000;  //later we will keep in constant
									}
									
									driving_time += (currentTripPojo.getEnd_time_stamp()
											- currentTripPojo.getStart_time_stamp())- idleDuration;
									currentTripPojo.setDriving_time(driving_time);
									
									//calculate the trip distance
									long totalTripDistance = current_trip_start_var.getTrip_distance();
									long prevOdometerVal = current_trip_start_var.getOdometer_val();
									if (indexValue.getVDist() != null) {
										totalTripDistance += ( indexValue.getVDist().longValue() - prevOdometerVal );
									}
									currentTripPojo.setTrip_distance(Long.valueOf(totalTripDistance));
									

									System.out.println("CURRENT TRIP POJO BEFORE UPDATE : " + currentTripPojo);

									currentTripDAO.update(currentTripPojo);
								} else {
									System.out.println(
											"Received other index data before start message :: " + currentTripPojo);

								}

							}

							else { // trip starts, so insert
								
									if (indexValue.getEvtDateTime() != null)
										currentTripPojo.setStart_time_stamp(TimeFormatter.getInstance()
												.convertUTCToEpochMilli(indexValue.getEvtDateTime(), DafConstants.DTM_TS_FORMAT));

									currentTripPojo.setStart_position_lattitude(indexValue.getGpsLatitude());
									currentTripPojo.setStart_position_longitude(indexValue.getGpsLongitude());
									currentTripPojo.setStart_position_heading(indexValue.getGpsHeading());
									currentTripPojo.setDriving_time(0L);
									currentTripPojo.setTrip_distance(0L);
									currentTripPojo.setCreated_at(TimeFormatter.getInstance().getCurrentUTCTime());

									//NOTE: warning and vehicle health status fields - to be populated from monitoring
									// messages; ONLY - vehicle health status field - mapped 'N' at the time of trip start
									// warning fields are mapped NULL at the time of trip start 
									currentTripPojo.setVehicle_health_status_type('N');
									currentTripPojo.setLatest_warning_class(null);
									currentTripPojo.setLatest_warning_number(null);
									currentTripPojo.setLatest_warning_type(null);
									currentTripPojo.setLatest_warning_timestamp(null);
									currentTripPojo.setLatest_warning_position_latitude(null);
									currentTripPojo.setLatest_warning_position_longitude(null);
									currentTripPojo.setLatest_warning_geolocation_address_id(null);

									System.out.println("CURRENT TRIP POJO BEFORE INSERT :: " + currentTripPojo);

									currentTripDAO.insert(currentTripPojo);
									
							}
						} catch (Exception e) {
							System.out.println("exception in insert or update" + e.getMessage());
							e.printStackTrace();
						}
					}

				}
			}
		} catch (Exception e) {
			System.out.println("EXCEPTION WHILE PROCESSING TRIP STATISTICS DATA = " + row);

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
