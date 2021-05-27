package net.atos.daf.ct2.etl.common.postgre;

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

import net.atos.daf.common.ct2.exception.TechnicalException;
//import net.atos.daf.common.ct2.postgre.PostgreDataSourceConnection;
import net.atos.daf.common.ct2.util.DAFConstants;
import net.atos.daf.ct2.etl.common.util.ETLConstants;
import net.atos.daf.postgre.bo.Trip;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.TripSinkDao;

public class TripSink extends RichSinkFunction<Trip> implements Serializable{
	

	/**
	* 
	*/
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LoggerFactory.getLogger(TripSink.class);

	private PreparedStatement statement;
	private Connection connection;
	private Connection masterConnection;
	private List<Trip> queue;//=new ArrayList<Trip>();
	private List<Trip> synchronizedCopy;// = new ArrayList<Trip>();
	TripSinkDao tripDao; //= new TripSinkDao();
	//Co2MasterDao cmDAO;
	
	/*String query = "INSERT INTO tripdetail.trip_statistics( trip_id, vin, start_time_stamp, end_time_stamp, veh_message_distance, etl_gps_distance, idle_duration"
			+ ", average_speed, average_weight, start_odometer, last_odometer, start_position_lattitude, start_position_longitude, end_position_lattitude"
			+ ", end_position_longitude, veh_message_fuel_consumed, etl_gps_fuel_consumed, veh_message_driving_time"
			+ ", etl_gps_driving_time, message_received_timestamp, message_inserted_into_kafka_timestamp, message_inserted_into_hbase_timestamp, message_processed_by_etl_process_timestamp"
			+ ", co2_emission, fuel_consumption, max_speed, average_gross_weight_comb, pto_duration, harsh_brake_duration, heavy_throttle_duration"
			+ ", cruise_control_distance_30_50, cruise_control_distance_50_75, cruise_control_distance_more_than_75"
			+ ", average_traffic_classification, cc_fuel_consumption, v_cruise_control_fuel_consumed_for_cc_fuel_consumption, v_cruise_control_dist_for_cc_fuel_consumption"
			+ ", fuel_consumption_cc_non_active, idling_consumption, dpa_score, driver1_id, driver2_id, etl_gps_trip_time, is_ongoing_trip) "
			+ "  VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"
			//+ " ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?,?)"
			+ "  ON CONFLICT (trip_id) "
			+ "  DO UPDATE SET  vin = ?, start_time_stamp = ?, end_time_stamp = ?, veh_message_distance = ?, etl_gps_distance = ?, idle_duration = ?, average_speed = ?"
			+ ", average_weight = ?, start_odometer = ?, last_odometer = ?, start_position_lattitude = ?, start_position_longitude = ?, end_position_lattitude = ?"
			+ ", end_position_longitude = ?, veh_message_fuel_consumed = ?, etl_gps_fuel_consumed = ?"
			+ ", veh_message_driving_time = ?, etl_gps_driving_time = ?, message_received_timestamp = ?, message_inserted_into_kafka_timestamp =?, message_inserted_into_hbase_timestamp = ?"
			+ ", message_processed_by_etl_process_timestamp = ?, co2_emission = ?, fuel_consumption = ?, max_speed = ?, average_gross_weight_comb = ?"
			+ ", pto_duration = ?, harsh_brake_duration = ?, heavy_throttle_duration = ?, cruise_control_distance_30_50 = ?"
			+ ", cruise_control_distance_50_75 = ?, cruise_control_distance_more_than_75 = ?, average_traffic_classification = ?"
			+ ", cc_fuel_consumption = ?, v_cruise_control_fuel_consumed_for_cc_fuel_consumption = ?, v_cruise_control_dist_for_cc_fuel_consumption = ?"
			+ ", fuel_consumption_cc_non_active = ?, idling_consumption = ?, dpa_score = ?, driver1_id = ?, driver2_id = ?, etl_gps_trip_time = ?, is_ongoing_trip = ?";
*/
	  
	  @Override
	public void invoke(Trip rec) throws Exception {

		try {
			queue.add(rec);
			//Co2Master cmData = cmDAO.read();
			
			if (queue.size() >= 1) {
				logger.info("inside syncronized");
				synchronized (synchronizedCopy) {
					synchronizedCopy = new ArrayList<Trip>(queue);
					queue.clear();
					for (Trip tripData : synchronizedCopy) {
						/*if(tripData.getVUsedFuel()!=null)
							tripData.setTripCalC02Emission((tripData.getVUsedFuel() * cmData.getCoefficient_D()) / 1000);*/
					logger.info("tripId :: "+tripData.getTripId() +" co2Emi ::"+tripData.getTripCalC02Emission());
					tripDao.insert(tripData);
					// jPAPostgreDao.saveTripDetails(synchronizedCopy);
					logger.info("save done");
					// logger.info("anshu1");
					}
				}
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		/*statement.setString(1, rec.getTripId());
		
		//TODO only for testing
		logger.info("Sink TripId : "+rec.getTripId() +" VIN : "+rec.getVin() +" VID : "+rec.getVid());
		
		if(rec.getVin() != null){
			statement.setString(2, rec.getVin());
		}else if(rec.getVid() != null){
			statement.setString(2, rec.getVid());
		}else
			statement.setString(2, ETLConstants.UNKNOWN);
		
		if (rec.getStartDateTime() != null)
			statement.setLong(3, rec.getStartDateTime());
		else
			statement.setLong(3, 0);

		if (rec.getEndDateTime() != null)
			statement.setLong(4, rec.getEndDateTime());
		else
			statement.setLong(4, 0);

		if (rec.getGpsTripDist() != null)
			statement.setInt(5, rec.getGpsTripDist());
		else
			statement.setInt(5, 0);

		if (rec.getTripCalDist() != null)
			statement.setLong(6, rec.getTripCalDist());
		else
			statement.setLong(6, 0);

		if (rec.getVIdleDuration() != null)
			statement.setInt(7, rec.getVIdleDuration());
		else
			statement.setInt(7, 0);

		if (rec.getTripCalAvgSpeed() != null)
			statement.setDouble(8, rec.getTripCalAvgSpeed());
		else
			statement.setDouble(8, 0);

		if (rec.getVGrossWeightCombination() != null)
			statement.setDouble(9, rec.getVGrossWeightCombination());
		else
			statement.setDouble(9, 0);

		if (rec.getGpsStartVehDist() != null)
			statement.setLong(10, rec.getGpsStartVehDist());
		else
			statement.setLong(10, 0);

		if (rec.getGpsStopVehDist() != null)
			statement.setLong(11, rec.getGpsStopVehDist());
		else
			statement.setLong(11, 0);

		if (rec.getGpsStartLatitude() != null)
			statement.setDouble(12, rec.getGpsStartLatitude());
		else
			statement.setDouble(12, 0);

		if (rec.getGpsStartLongitude() != null)
			statement.setDouble(13, rec.getGpsStartLongitude());
		else
			statement.setDouble(13, 0);

		if (rec.getGpsEndLatitude() != null)
			statement.setDouble(14, rec.getGpsEndLatitude());
		else
			statement.setDouble(14, 0);

		if (rec.getGpsEndLongitude() != null)
			statement.setDouble(15, rec.getGpsEndLongitude());
		else
			statement.setDouble(15, 0);

		if (rec.getVUsedFuel() != null)
			statement.setInt(16, rec.getVUsedFuel());
		else
			statement.setInt(16, 0);

		if (rec.getTripCalUsedFuel() != null)
			statement.setLong(17, rec.getTripCalUsedFuel());
		else
			statement.setLong(17, 0);

		if (rec.getVTripMotionDuration() != null)
			statement.setInt(18, rec.getVTripMotionDuration());
		else
			statement.setInt(18, 0);

		if (rec.getTripCalDrivingTm() != null)
			statement.setLong(19, rec.getTripCalDrivingTm());
		else
			statement.setLong(19, 0);

		if (rec.getReceivedTimestamp() != null)
			statement.setLong(20, rec.getReceivedTimestamp());
		else
			statement.setLong(20, 0);

		if (rec.getKafkaProcessingTS() != null)
			statement.setLong(21, rec.getKafkaProcessingTS());
		else
			statement.setLong(21, 0);

		if (rec.getTripProcessingTS() != null)
			statement.setLong(22, rec.getTripProcessingTS());
		else
			statement.setLong(22, 0);

		if (rec.getEtlProcessingTS() != null)
			statement.setLong(23, rec.getEtlProcessingTS());
		else
			statement.setLong(23, 0);

		if (rec.getTripCalC02Emission() != null)
			statement.setDouble(24, rec.getTripCalC02Emission());
		else
			statement.setDouble(24, 0);

		if (rec.getTripCalFuelConsumption() != null)
			statement.setDouble(25, rec.getTripCalFuelConsumption());
		else
			statement.setDouble(25, 0);

		if (rec.getVTachographSpeed() != null)
			statement.setDouble(26, rec.getVTachographSpeed());
		else
			statement.setDouble(26, 0);

		if (rec.getTripCalAvgGrossWtComb() != null)
			statement.setDouble(27, rec.getTripCalAvgGrossWtComb());
		else
			statement.setDouble(27, 0);

		if (rec.getTripCalPtoDuration() != null)
			statement.setDouble(28, rec.getTripCalPtoDuration());
		else
			statement.setDouble(28, 0);

		if (rec.getTriCalHarshBrakeDuration() != null)
			statement.setDouble(29, rec.getTriCalHarshBrakeDuration());
		else
			statement.setDouble(29, 0);

		if (rec.getTripCalHeavyThrottleDuration() != null)
			statement.setDouble(30, rec.getTripCalHeavyThrottleDuration());
		else
			statement.setDouble(30, 0);

		if (rec.getTripCalCrsCntrlDistBelow50() != null)
			statement.setDouble(31, rec.getTripCalCrsCntrlDistBelow50());
		else
			statement.setDouble(31, 0);

		if (rec.getTripCalCrsCntrlDistAbv50() != null)
			statement.setDouble(32, rec.getTripCalCrsCntrlDistAbv50());
		else
			statement.setDouble(32, 0);

		if (rec.getTripCalCrsCntrlDistAbv75() != null)
			statement.setDouble(33, rec.getTripCalCrsCntrlDistAbv75());
		else
			statement.setDouble(33, 0);

		if (rec.getTripCalAvgTrafficClsfn() != null)
			statement.setDouble(34, rec.getTripCalAvgTrafficClsfn());
		else
			statement.setDouble(34, 0);

		if (rec.getTripCalCCFuelConsumption() != null)
			statement.setDouble(35, rec.getTripCalCCFuelConsumption());
		else
			statement.setDouble(35, 0);

		if (rec.getVCruiseControlFuelConsumed() != null)
			statement.setInt(36, rec.getVCruiseControlFuelConsumed());
		else
			statement.setInt(36, 0);

		if (rec.getVCruiseControlDist() != null)
			statement.setInt(37, rec.getVCruiseControlDist());
		else
			statement.setInt(37, 0);

		if (rec.getTripCalfuelNonActiveCnsmpt() != null)
			statement.setDouble(38, rec.getTripCalfuelNonActiveCnsmpt());
		else
			statement.setDouble(38, 0);

		if (rec.getVIdleFuelConsumed() != null)
			statement.setInt(39, rec.getVIdleFuelConsumed());
		else
			statement.setInt(39, 0);

		if (rec.getTripCalDpaScore() != null)
			statement.setDouble(40, rec.getTripCalDpaScore());
		else
			statement.setDouble(40, 0);

		statement.setString(41, rec.getDriverId());
		statement.setString(42, rec.getDriver2Id());

		if (rec.getTripCalGpsVehTime() != null)
			statement.setLong(43, rec.getTripCalGpsVehTime());
		else
			statement.setLong(43, 0);

		statement.setBoolean(44, Boolean.FALSE);

		if(rec.getVin() != null){
			statement.setString(45, rec.getVin());
		}else if(rec.getVid() != null){
			statement.setString(45, rec.getVid());
		}else
			statement.setString(45, ETLConstants.UNKNOWN);
		
		if (rec.getStartDateTime() != null)
			statement.setLong(46, rec.getStartDateTime());
		else
			statement.setLong(46, 0);

		if (rec.getEndDateTime() != null)
			statement.setLong(47, rec.getEndDateTime());
		else
			statement.setLong(47, 0);

		if (rec.getGpsTripDist() != null)
			statement.setInt(48, rec.getGpsTripDist());
		else
			statement.setInt(48, 0);

		if (rec.getTripCalDist() != null)
			statement.setLong(49, rec.getTripCalDist());
		else
			statement.setLong(49, 0);

		if (rec.getVIdleDuration() != null)
			statement.setInt(50, rec.getVIdleDuration());
		else
			statement.setInt(50, 0);

		if (rec.getTripCalAvgSpeed() != null)
			statement.setDouble(51, rec.getTripCalAvgSpeed());
		else
			statement.setDouble(51, 0);

		if (rec.getVGrossWeightCombination() != null)
			statement.setDouble(52, rec.getVGrossWeightCombination());
		else
			statement.setDouble(52, 0);

		if (rec.getGpsStartVehDist() != null)
			statement.setLong(53, rec.getGpsStartVehDist());
		else
			statement.setLong(53, 0);

		if (rec.getGpsStopVehDist() != null)
			statement.setLong(54, rec.getGpsStopVehDist());
		else
			statement.setLong(54, 0);

		if (rec.getGpsStartLatitude() != null)
			statement.setDouble(55, rec.getGpsStartLatitude());
		else
			statement.setDouble(55, 0);

		if (rec.getGpsStartLongitude() != null)
			statement.setDouble(56, rec.getGpsStartLongitude());
		else
			statement.setDouble(56, 0);

		if (rec.getGpsEndLatitude() != null)
			statement.setDouble(57, rec.getGpsEndLatitude());
		else
			statement.setDouble(57, 0);

		if (rec.getGpsEndLongitude() != null)
			statement.setDouble(58, rec.getGpsEndLongitude());
		else
			statement.setDouble(58, 0);

		if (rec.getVUsedFuel() != null)
			statement.setInt(59, rec.getVUsedFuel());
		else
			statement.setInt(59, 0);

		if (rec.getTripCalUsedFuel() != null)
			statement.setLong(60, rec.getTripCalUsedFuel());
		else
			statement.setLong(60, 0);

		if (rec.getVTripMotionDuration() != null)
			statement.setInt(61, rec.getVTripMotionDuration());
		else
			statement.setInt(61, 0);

		if (rec.getTripCalDrivingTm() != null)
			statement.setLong(62, rec.getTripCalDrivingTm());
		else
			statement.setLong(62, 0);

		if (rec.getReceivedTimestamp() != null)
			statement.setLong(63, rec.getReceivedTimestamp());
		else
			statement.setLong(63, 0);
		
		if (rec.getKafkaProcessingTS() != null)
			statement.setLong(64, rec.getKafkaProcessingTS());
		else
			statement.setLong(64, 0);

		if (rec.getTripProcessingTS() != null)
			statement.setLong(65, rec.getTripProcessingTS());
		else
			statement.setLong(65, 0);

		if (rec.getEtlProcessingTS() != null)
			statement.setLong(66, rec.getEtlProcessingTS());
		else
			statement.setLong(66, 0);

		if (rec.getTripCalC02Emission() != null)
			statement.setDouble(67, rec.getTripCalC02Emission());
		else
			statement.setDouble(67, 0);

		if (rec.getTripCalFuelConsumption() != null)
			statement.setDouble(68, rec.getTripCalFuelConsumption());
		else
			statement.setDouble(68, 0);

		if (rec.getVTachographSpeed() != null)
			statement.setDouble(69, rec.getVTachographSpeed());
		else
			statement.setDouble(69, 0);

		if (rec.getTripCalAvgGrossWtComb() != null)
			statement.setDouble(70, rec.getTripCalAvgGrossWtComb());
		else
			statement.setDouble(70, 0);

		if (rec.getTripCalPtoDuration() != null)
			statement.setDouble(71, rec.getTripCalPtoDuration());
		else
			statement.setDouble(71, 0);

		if (rec.getTriCalHarshBrakeDuration() != null)
			statement.setDouble(72, rec.getTriCalHarshBrakeDuration());
		else
			statement.setDouble(72, 0);

		if (rec.getTripCalHeavyThrottleDuration() != null)
			statement.setDouble(73, rec.getTripCalHeavyThrottleDuration());
		else
			statement.setDouble(73, 0);

		if (rec.getTripCalCrsCntrlDistBelow50() != null)
			statement.setDouble(74, rec.getTripCalCrsCntrlDistBelow50());
		else
			statement.setDouble(74, 0);

		if (rec.getTripCalCrsCntrlDistAbv50() != null)
			statement.setDouble(75, rec.getTripCalCrsCntrlDistAbv50());
		else
			statement.setDouble(75, 0);

		if (rec.getTripCalCrsCntrlDistAbv75() != null)
			statement.setDouble(76, rec.getTripCalCrsCntrlDistAbv75());
		else
			statement.setDouble(76, 0);

		if (rec.getTripCalAvgTrafficClsfn() != null)
			statement.setDouble(77, rec.getTripCalAvgTrafficClsfn());
		else
			statement.setDouble(77, 0);

		if (rec.getTripCalCCFuelConsumption() != null)
			statement.setDouble(78, rec.getTripCalCCFuelConsumption());
		else
			statement.setDouble(78, 0);

		if (rec.getVCruiseControlFuelConsumed() != null)
			statement.setInt(79, rec.getVCruiseControlFuelConsumed());
		else
			statement.setInt(79, 0);

		if (rec.getVCruiseControlDist() != null)
			statement.setInt(80, rec.getVCruiseControlDist());
		else
			statement.setInt(80, 0);

		//if (rec.getTripCalfuelNonActiveCnsmpt() != null)
			statement.setDouble(81, rec.getTripCalfuelNonActiveCnsmpt());
//		else
//			statement.setDouble(81, 0);

		if (rec.getVIdleFuelConsumed() != null)
			statement.setInt(82, rec.getVIdleFuelConsumed());
		else
			statement.setInt(82, 0);

		if (rec.getTripCalDpaScore() != null)
			statement.setDouble(83, rec.getTripCalDpaScore());
		else
			statement.setDouble(83, 0);

		statement.setString(84, rec.getDriverId());
		statement.setString(85, rec.getDriver2Id());

		if (rec.getTripCalGpsVehTime() != null)
			statement.setLong(86, rec.getTripCalGpsVehTime());
		else
			statement.setLong(86, 0);

		statement.setBoolean(87, Boolean.FALSE);
		
		logger.info("Prepared data for trip :: "+rec.getTripId());

		statement.addBatch();
		statement.executeBatch();*/
	}

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();
		
		tripDao = new TripSinkDao();
		queue =new ArrayList<Trip>();
		synchronizedCopy = new ArrayList<Trip>();
		//cmDAO = new Co2MasterDao();
		connection=PostgreDataSourceConnection.getInstance().getDataSourceConnection(envParams.get(ETLConstants.DATAMART_POSTGRE_SERVER_NAME),
				Integer.parseInt(envParams.get(ETLConstants.DATAMART_POSTGRE_PORT)),
				envParams.get(ETLConstants.DATAMART_POSTGRE_DATABASE_NAME),
				envParams.get(ETLConstants.DATAMART_POSTGRE_USER),
				envParams.get(ETLConstants.DATAMART_POSTGRE_PASSWORD));
		logger.info("In trip sink connection done" + connection);
		tripDao.setConnection(connection);
		
		/*masterConnection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
				envParams.get(ETLConstants.MASTER_POSTGRE_SERVER_NAME),
				Integer.parseInt(envParams.get(ETLConstants.MASTER_POSTGRE_PORT)),
				envParams.get(ETLConstants.MASTER_POSTGRE_DATABASE_NAME),
				envParams.get(ETLConstants.MASTER_POSTGRE_USER),
				envParams.get(ETLConstants.MASTER_POSTGRE_PASSWORD));
		cmDAO.setConnection(masterConnection);*/
		
		
		/*//TODOonly for testing remove
 		logger.info("envParams.get(ETLConstants.POSTGRE_SQL_SERVER_NAME) :: "+envParams.get(ETLConstants.DATAMART_POSTGRE_SERVER_NAME));
		logger.info("envParams.get(ETLConstants.POSTGRE_SQL_PORT) :: "+envParams.get(ETLConstants.DATAMART_POSTGRE_PORT));
		logger.info("envParams.get(ETLConstants.POSTGRE_SQL_DATABASE_NAME) :: "+envParams.get(ETLConstants.DATAMART_POSTGRE_DATABASE_NAME));
		logger.info("envParams.get(ETLConstants.POSTGRE_SQL_USER) :: "+envParams.get(ETLConstants.DATAMART_POSTGRE_USER));
		logger.info("envParams.get(ETLConstants.POSTGRE_SQL_PASSWORD) :: "+envParams.get(ETLConstants.DATAMART_POSTGRE_PASSWORD));
		
		Class.forName(envParams.get(ETLConstants.POSTGRE_SQL_DRIVER));
		String dbUrl = createValidUrlToConnectPostgreSql(envParams.get(ETLConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(ETLConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(ETLConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(ETLConstants.DATAMART_POSTGRE_USER),
					envParams.get(ETLConstants.DATAMART_POSTGRE_PASSWORD));
		connection = DriverManager.getConnection(dbUrl);
		logger.info("Connect created individually ::: " + connection);
		
		statement = connection.prepareStatement(query); */
		
//		PostgreDataSourceConnection connectionInstance=PostgreDataSourceConnection.getInstance();
//		connection=connectionInstance.getDataSourceConnection("jdbc:postgresql://dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com", 5432, "vehicledatamart", "W%PQ1AI}Y97", "W%PQ1AI}Y97");
//		statement = connection.prepareStatement(query);
	}

	//@SuppressWarnings("unchecked")
	@Override
    public void close() throws Exception {
        if (statement != null) {
        	statement.close();
        }
        logger.info("In close() of tripSink :: ");
        
        if (connection != null) {
        	logger.info("Releasing connection from Trip Job");
            connection.close();
        }
        if (masterConnection != null) {
        	logger.info("Releasing Master connection from Trip Job");
        	masterConnection.close();
        }
       
        super.close(); 
		
    }
	
	
	private String createValidUrlToConnectPostgreSql(String serverNm, int port, String databaseNm, String userNm,
			String password) throws TechnicalException {

		String encodedPassword = encodeValue(password);
		String url = serverNm + ":" + port + "/" + databaseNm + "?" + "user=" + userNm + "&" + "password="
				+ encodedPassword + DAFConstants.POSTGRE_SQL_SSL_MODE;

		logger.info("Valid Url = " + url);

		return url;
	}
	
	private static String encodeValue(String value) {
		try {
			return URLEncoder.encode(value, StandardCharsets.UTF_8.toString());
		} catch (UnsupportedEncodingException ex) {
			throw new RuntimeException(ex.getCause());
		}
	}
}