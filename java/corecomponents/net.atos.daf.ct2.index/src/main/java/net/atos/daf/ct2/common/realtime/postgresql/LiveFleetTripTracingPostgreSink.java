package net.atos.daf.ct2.common.realtime.postgresql;

import java.io.Serializable;
import java.sql.Connection;
import java.text.ParseException;


import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;



import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.postgre.bo.Co2Master;
import net.atos.daf.postgre.bo.LiveFleetPojo;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.Co2MasterDao;
import net.atos.daf.postgre.dao.LiveFleetPosition;
import net.atos.daf.postgre.dao.LivefleetCurrentTripStatisticsDao;

public class LiveFleetTripTracingPostgreSink extends RichSinkFunction<Index> implements Serializable {

	private static final long serialVersionUID = 1L;
	Logger logger = LoggerFactory.getLogger(LiveFleetTripTracingPostgreSink.class);

	String livefleetposition = null;
	Connection connection = null;

	Connection masterConnection = null;
	LiveFleetPosition positionDAO;
	LivefleetCurrentTripStatisticsDao currentTripDAO;
	Co2MasterDao cmDAO;
	Co2Master cmData;
	//private List<Index> queue;
	//private List<Index> synchronizedCopy;

	Long Fuel_consumption = 0L;
	
	public void invoke(Index index) throws Exception {
		logger.info("inside invoke of LiveFleetPosition Management ");
		try {
		LiveFleetPojo currentPosition = tripCalculation(index);

		positionDAO.insert(currentPosition);
		logger.info("Data inserted in Live fleet position :: "+currentPosition.getTripId());
		} catch(Exception e) {
			logger.error("error in invoke of LiveFleetCurrentTripPostgreSink" + e.getMessage());
		}
		
	}

/*	public void invoke232(KafkaRecord<Index> index) throws Exception {

		queue = new ArrayList<Index>();
		synchronizedCopy = new ArrayList<Index>();

		Index row = index.getValue();

		try {

			queue.add(row);

			cmData = cmDAO.read(row.getVid()); // CO2 coefficient data read from
												// master table

			if (queue.size() >= 1) {

				synchronized (synchronizedCopy) {
					synchronizedCopy = new ArrayList<Index>(queue);
					queue.clear();
					//TODO---size of queue--put logger
					for (Index indexData : synchronizedCopy) {
						
						logger.info("inside LiveFleet Sink class :{}");
						String tripID = "NOT AVAILABLE";

						if (indexData.getDocument().getTripID() != null) {
							tripID = indexData.getDocument().getTripID();
						}

						Double drivingTime = 0.0;
						String vin;
						if (indexData.getVin() != null)
							vin = indexData.getVin();
						else
							vin = indexData.getVid();
						
						if (indexData.getVEvtID() != 4) {
							
							LiveFleetPojo previousRecordInfo = positionDAO.read(vin, tripID);
							//logger.info("inside LiveFleet Sink class after read :{}");
							////TODO----log end time along with trip id & vin
							if (previousRecordInfo != null) {
								logger.info("inside LiveFleet previousRecordInfo is not null :{}");
								Double previousMessageTimeStamp = previousRecordInfo.getMessageTimestamp();
								Double currentMessageTimeStamp = (double) TimeFormatter.getInstance()
										.convertUTCToEpochMilli(indexData.getEvtDateTime().toString(),
												DafConstants.DTM_TS_FORMAT);
								Long idleDuration=0L;
								if(indexData.getVIdleDuration()!=null) {
									idleDuration=(indexData.getVIdleDuration() * 1000); //later need to keep in constant
								}
								drivingTime = ((currentMessageTimeStamp - previousMessageTimeStamp)
										+ previousRecordInfo.getDrivingTime() - idleDuration);
							}
							//System.out.println("drivingTime-->" + drivingTime);
							logger.info("inside LiveFleet drivingTime-->:{}"+ drivingTime);
						}

					//	LiveFleetPojo currentPosition = tripCalculation(row, drivingTime);

					//	positionDAO.insert(currentPosition);
						//logger.info("Data inserted in Live fleet position :: "+currentPosition.getTripId());

					}
				}
			}
		} catch (Exception e) {
			logger.error("Error in Live fleet position invoke method" + e.getMessage());
			e.printStackTrace();
		}

	} */

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

		logger.info("########## In LiveFleet Position ##############");
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

			logger.error("Error in Live fleet position" + e.getMessage());

		}

	}

	public LiveFleetPojo tripCalculation(Index row) {
		LiveFleetPojo currentPosition = new LiveFleetPojo();
		System.out.println("Inside Trip Calculation");
		/*
		 * int varVEvtid = 0; if (row.getVEvtID() != null) { varVEvtid =
		 * row.getVEvtID(); }
		 */
		
		try {
			cmData = cmDAO.read(row.getVid());
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
		Long Fuel_consumption = row.getVUsedFuel();

		if (Fuel_consumption != null) {

			double co2emission = (Fuel_consumption * cmData.getCoefficient()) / 1000;
			//System.out.println("co2emission-->" + co2emission);
			currentPosition.setCo2Emission(co2emission); // co2emission
			currentPosition.setFuelConsumption(Fuel_consumption.doubleValue());// fuel_consumption
		} else {
			currentPosition.setCo2Emission(0.0); // co2 emission
			currentPosition.setFuelConsumption(0.0); // fuel_consumption
		}
		
		if(row.getVDist()!=null)
			currentPosition.setLastOdometerValue(row.getVDist().intValue());
		else
			currentPosition.setLastOdometerValue(0);

		/*
		 * Long[] tachomileageArray = row.getDocument().getTotalTachoMileage(); int
		 * tachomileagelength = 0; if (tachomileageArray != null) tachomileagelength =
		 * tachomileageArray.length; // long[] longtachoMlArray =
		 * Arrays.stream(tachomileageArray).mapToLong(i -> // i).toArray(); try { if
		 * (tachomileagelength > 0) {
		 * 
		 * if (tachomileageArray[tachomileagelength - 1] != null &&
		 * !"null".equals(tachomileageArray[tachomileagelength - 1])) {
		 * 
		 * System.out.println("odometer value--" + tachomileageArray[tachomileagelength
		 * - 1]);
		 * currentPosition.setLastOdometerValue(tachomileageArray[tachomileagelength -
		 * 1].intValue()); } else { System.out.println("odometer value inside else--" +
		 * tachomileageArray); currentPosition.setLastOdometerValue(0); } } else {
		 * 
		 * System.out.println("odometer value when odometer less then 0--" +
		 * tachomileageArray); currentPosition.setLastOdometerValue(0); } } catch
		 * (Exception e) {
		 * 
		 * currentPosition.setLastOdometerValue(0); System.out.println(); }
		 */

		/*
		 * if (varVEvtid == 26 || varVEvtid == 28 || varVEvtid == 29 || varVEvtid == 32
		 * || varVEvtid == 42 || varVEvtid == 43 || varVEvtid == 44 || varVEvtid == 45
		 * || varVEvtid == 46) {
		 * currentPosition.setLastOdometerValue(row.getDocument().getVTachographSpeed())
		 * ;// TotalTachoMileage } else { currentPosition.setLastOdometerValue(0); }
		 */

		/*
		 * if (varVEvtid == 42 || varVEvtid == 43) {
		 * currentPosition.setDistUntilNextService(row.getDocument().
		 * getVDistanceUntilService());// distance_until_next_service } else {
		 * currentPosition.setDistUntilNextService(0); }
		 */

		currentPosition.setDistUntilNextService(0);

		currentPosition.setVehMessageType(DafConstants.Index);

		currentPosition.setVehicleMsgTriggerTypeId(row.getVEvtID());

		try {
			//TODO---log start time
			currentPosition.setCreatedDatetime(TimeFormatter.getInstance()
					.convertUTCToEpochMilli(row.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));
			//TODO---log end time
		} catch (ParseException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		currentPosition.setReceivedDatetime(row.getReceivedTimestamp());

		if (row.getDocument().getGpsSpeed() != null)
			currentPosition.setGpsSpeed(row.getDocument().getGpsSpeed().doubleValue());

		if (row.getGpsDateTime() != null) {
			try {
				currentPosition.setGpsDatetime(TimeFormatter.getInstance()
						.convertUTCToEpochMilli(row.getGpsDateTime().toString(), DafConstants.DTM_TS_FORMAT));
			} catch (ParseException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}

		if (row.getDocument().getVWheelBasedSpeed() != null)
			currentPosition.setWheelbasedSpeed(row.getDocument().getVWheelBasedSpeed().doubleValue());

		if(row.getDriverID() !=null && ! row.getDriverID().isEmpty()) {
			currentPosition.setDriver1Id(row.getDriverID());
		} else {
			currentPosition.setDriver1Id("Unknown");
		}
		//currentPosition.setDriver1Id(row.getDriverID());
		
		//Driving Calculation for memory load
		//currentPosition.setDrivingTime(drivingTime.intValue());
		if(row.getNumSeq()!=null) {
		currentPosition.setDrivingTime(row.getNumSeq().intValue());
		} else {
			currentPosition.setDrivingTime(0);
		}
		
		
	//vehicle status API fields
		currentPosition.setTotal_vehicle_distance(row.getVDist());
		currentPosition.setTotal_engine_hours(row.getDocument().getVEngineTotalHours());
		currentPosition.setTotal_engine_fuel_used(row.getVCumulatedFuel());
		currentPosition.setGross_combination_vehicle_weight(row.getDocument().getVGrossWeightCombination());
		currentPosition.setEngine_speed(row.getDocument().getVEngineSpeed());
		currentPosition.setFuel_level1(row.getDocument().getVFuelLevel1());
		currentPosition.setCatalyst_fuel_level(row.getDocument().getVDEFTankLevel());
		if(row.getDocument().getDriver2ID()!= null && ! row.getDocument().getDriver2ID().isEmpty()) {
					currentPosition.setDriver2_id(row.getDocument().getDriver2ID());
				} else {
					currentPosition.setDriver2_id("Unknown");
				}
		currentPosition.setDriver1_working_state(row.getDocument().getDriver1WorkingState());
		currentPosition.setDriver2_working_state(row.getDocument().getDriver2WorkingState());
		currentPosition.setAmbient_air_temperature(row.getDocument().getVAmbiantAirTemperature());
		currentPosition.setEngine_coolant_temperature(row.getDocument().getVEngineCoolantTemperature());
		currentPosition.setService_brake_air_pressure_circuit1(row.getDocument().getVServiceBrakeAirPressure1());
		//currentPosition.setService_brake_air_pressure_circuit2(row.getDocument().getVServiceBrakeAirPressure2());)
		currentPosition.setService_brake_air_pressure_circuit2(row.getDocument().getVServiceBrakeAirPressure2());

		//System.out.println("Inside Trip Calculation in end");
		logger.info("inside Inside Trip Calculation in end :{}");
		//System.out.println("data inserted");
		} catch(Exception e) {
			logger.error("error in trip calculation" + e.getMessage());
			e.printStackTrace();
		}
		return currentPosition;

	}

	@Override
	public void close() throws Exception {

		connection.close();
		masterConnection.close();

		logger.info("In Close");

	}

}
