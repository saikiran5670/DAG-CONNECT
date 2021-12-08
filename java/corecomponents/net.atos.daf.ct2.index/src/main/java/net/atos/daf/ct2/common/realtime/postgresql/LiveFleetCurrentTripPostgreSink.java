package net.atos.daf.ct2.common.realtime.postgresql;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.SQLException;
import java.text.ParseException;
import java.util.ArrayList;
import java.util.List;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.common.realtime.dataprocess.IndexDataProcess;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.postgre.bo.TripStatisticsPojo;
import net.atos.daf.postgre.bo.WarningStatisticsPojo;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.LivefleetCurrentTripStatisticsDao;

//@SuppressWarnings({})
public class LiveFleetCurrentTripPostgreSink extends RichSinkFunction<Index> implements Serializable {
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

	LivefleetCurrentTripStatisticsDao currentTripDAO = null;
	//LiveFleetPosition positionDAO = null;
	TripStatisticsPojo currentTripPojo = null;
	WarningStatisticsPojo warnStatsPojo = null;
	
	/*Statement createWarnStatusStmt = null;
	PreparedStatement updateWarnStatusStmt = null;
	PreparedStatement readWarnStatusStmt = null;
	ResultSet rs_warn_vehicle_health_status = null; */
	

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
		} 
		catch (Exception e) {
			log.error("Error in LiveFleet Current Trip Statstics: Problem in open - connection setup: " + e.getMessage());
			e.printStackTrace();

		}
		
	}

	public void invoke(Index index) {
		try {
			log.info("meesage received Current Trip Statistics--" + index.toString());
			if(index.getVEvtID()!=4) {
				TripStatisticsPojo Trip=tripCalculationUpdate(index);
				currentTripDAO.update(Trip);
			} else {
				TripStatisticsPojo currentTrip=	tripCalculation(index);
				currentTripDAO.insert(currentTrip);
				
			}
		} catch (TechnicalException e) {
			// TODO Auto-generated catch block
			log.error("Technical error in invoke of LiveFleetCurrentTripPostgreSink" + e.getMessage());
			e.printStackTrace();
			
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			log.error("error in invoke of LiveFleetCurrentTripPostgreSink" + e.getMessage());
			e.printStackTrace();
		}
		
		
	}
	
	public Character drivingStatus(Index index) {
		long vWheelSpeed = 0;
		Character driveStatus='S';
		if (index.getDocument() != null
				&& index.getDocument().getVWheelBasedSpeed() != null)
			vWheelSpeed = index.getDocument().getVWheelBasedSpeed().longValue();

		if (vWheelSpeed > 0)
			//currentTripPojo.setVehicle_driving_status_type('D'); // DRIVING if wheelspeed >
			driveStatus='D';											// 0
		else if (vWheelSpeed == 0) {
			if (index.getDocument() != null) {
				if	(index.getDocument().getVEngineSpeed() != null
						&& index.getDocument().getVEngineSpeed().longValue() > 0)
					//currentTripPojo.setVehicle_driving_status_type('I'); // IDLING if wheelspeed
					driveStatus='I';															// = 0 but
				// enginespeed > 0
				else
					//currentTripPojo.setVehicle_driving_status_type('S'); // STOPPED if
					driveStatus='S';															// wheelspped = 0
			}
			else 
				//currentTripPojo.setVehicle_driving_status_type('S');
				driveStatus='S';	
		}
		return driveStatus;
	}
	
	public TripStatisticsPojo tripCalculationUpdate(Index index) {
		currentTripPojo = new TripStatisticsPojo();
		log.debug("inside update method");
		if (index.getDocument() != null) 
			currentTripPojo.setTripId(index.getDocument().getTripID());

		currentTripPojo.setDriving_time(index.getNumSeq());
		currentTripPojo.setTrip_distance(index.getVCumulatedFuel());

		if (index.getVUsedFuel() != null)
			currentTripPojo.setFuel_consumption(index.getVUsedFuel().longValue());

		if(index.getVEvtID()==5)
			currentTripPojo.setVehicle_driving_status_type('S');
		else
			currentTripPojo.setVehicle_driving_status_type(drivingStatus(index)); 
		currentTripPojo.setOdometer_val(index.getVDist());
		//currentTripPojo.setLast_geolocation_address_id(null);
		currentTripPojo.setLast_received_position_lattitude(index.getGpsLatitude());
		currentTripPojo.setLast_received_position_longitude(index.getGpsLongitude());
		currentTripPojo.setLast_received_position_heading(index.getGpsHeading());


		currentTripPojo.setModified_at(TimeFormatter.getInstance().getCurrentUTCTime());

		try {
			currentTripPojo.setEnd_time_stamp(TimeFormatter.getInstance().convertUTCToEpochMilli(index.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));
			currentTripPojo.setLast_processed_message_timestamp(TimeFormatter.getInstance().convertUTCToEpochMilli(index.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));

		} catch (ParseException e) {
			// TODO Auto-generated catch block
			log.error("ParseException {}",e);
		}



		return currentTripPojo;

	}
	public TripStatisticsPojo tripCalculation(Index index) {

		currentTripPojo = new TripStatisticsPojo();
		log.debug("inside trip calculation");

		if (index.getDocument() != null) 
			currentTripPojo.setTripId(index.getDocument().getTripID());
		//currentTripPojo.setVin(index.getVin());  //***************

		if (index.getVin() != null) 
			currentTripPojo.setVin(index.getVin()); // not null
		else 
			currentTripPojo.setVin(index.getVid());
		try {
			currentTripPojo.setStart_time_stamp(TimeFormatter.getInstance().convertUTCToEpochMilli(index.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));
			currentTripPojo.setEnd_time_stamp(TimeFormatter.getInstance().convertUTCToEpochMilli(index.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));
			currentTripPojo.setLast_processed_message_timestamp(TimeFormatter.getInstance().convertUTCToEpochMilli(index.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));


		} catch (ParseException e) {
			// TODO Auto-generated catch block
			log.error("catch in first exception modified" + e.getMessage());
			log.error("ParseException {}",e);
		}

		if(index.getDriverID()!=null && !index.getDriverID().isEmpty() && index.getDriverID()!=" ")
			currentTripPojo.setDriver1ID(index.getDriverID()); // not null
		else 
			currentTripPojo.setDriver1ID("Unknown"); //Unknown, if null

		if (index.getVUsedFuel() != null)
			currentTripPojo.setFuel_consumption(index.getVUsedFuel().longValue());

		if(index.getVDist() != null)
			currentTripPojo.setOdometer_val(Long.valueOf(index.getVDist().longValue()));

		currentTripPojo.setDistance_until_next_service(null); // to be populated from monitoring

		currentTripPojo.setLast_received_position_lattitude(index.getGpsLatitude());
		currentTripPojo.setLast_received_position_longitude(index.getGpsLongitude());
		currentTripPojo.setLast_received_position_heading(index.getGpsHeading());
		currentTripPojo.setLast_geolocation_address_id(null);
		currentTripPojo.setStart_geolocation_address_id(null);
		currentTripPojo.setVehicle_driving_status_type(drivingStatus(index)); 
		
		//currentTripPojo.setModified_at(TimeFormatter.getInstance().getCurrentUTCTime());
		currentTripPojo.setModified_at(null);


		currentTripPojo.setTrip_distance(0L);
		currentTripPojo.setDriving_time(0L);

		currentTripPojo.setStart_position_lattitude(index.getGpsLatitude());
		currentTripPojo.setStart_position_longitude(index.getGpsLongitude());
		currentTripPojo.setStart_position_heading(index.getGpsHeading());

		currentTripPojo.setCreated_at(TimeFormatter.getInstance().getCurrentUTCTime());

		//NOTE: warning and vehicle health status fields - to be populated from monitoring
		// messages; ONLY - vehicle health status field - mapped 'N' at the time of trip start
		// warning fields are mapped NULL at the time of trip start 

		try {

			warnStatsPojo=currentTripDAO.readWarning(index.getVin()!=null ? index.getVin() : index.getVid());
			log.info("LATEST WARNING STATUS RECEIVED from LIVEFLEET_WARNING_STATUS FOR vin/vid {}",index.getVin()!=null ? index.getVin() : index.getVid());

		} catch(Exception e) {
			log.error("Error in LiveFleet Current Trip Statstics: Failed read latest warning status table {} {}" , e.getMessage(),index);
		}

		if(warnStatsPojo!=null) {
			if(warnStatsPojo.getVehicleHealthStatusType()!=null && !warnStatsPojo.getVehicleHealthStatusType().isEmpty()) {
				currentTripPojo.setVehicle_health_status_type((warnStatsPojo.getVehicleHealthStatusType().length() > 0) ? 
						warnStatsPojo.getVehicleHealthStatusType().charAt(0) : 'N');
			} else {
				currentTripPojo.setVehicle_health_status_type('N');
			}
			currentTripPojo.setLatest_warning_class((warnStatsPojo.getWarningClass()!=null) ? warnStatsPojo.getWarningClass().longValue() : null);
			currentTripPojo.setLatest_warning_number((warnStatsPojo.getWarningNumber()!=null) ? warnStatsPojo.getWarningNumber().longValue() : null);

			if(warnStatsPojo.getWarningType()!=null && !warnStatsPojo.getWarningType().isEmpty()) {
				currentTripPojo.setLatest_warning_type((warnStatsPojo.getWarningType().length() > 0) ? warnStatsPojo.getWarningType().charAt(0) : null);
			} else {
				currentTripPojo.setLatest_warning_type(null);
			}
			currentTripPojo.setLatest_warning_timestamp((warnStatsPojo.getWarningTimeStamp()!=null) ? warnStatsPojo.getWarningTimeStamp() : null);
			currentTripPojo.setLatest_warning_position_latitude((warnStatsPojo.getLatitude()!=null) ? warnStatsPojo.getLatitude() : null);
			currentTripPojo.setLatest_warning_position_longitude((warnStatsPojo.getLongitude()!=null) ? warnStatsPojo.getLongitude() : null);
			currentTripPojo.setLatest_warning_geolocation_address_id(null);
		}

		return currentTripPojo;
	}
	
	
	
	

	@Override
	public void close() throws Exception {
		connection.close();
		log.debug("In Close");

	}

}
