package net.atos.daf.ct2.common.realtime.postgresql;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.text.ParseException;
import java.util.ArrayList;
import java.util.List;

import org.apache.flink.api.common.typeinfo.Types;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.postgre.bo.WarningStastisticsPojo;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.WarningStatisticsDao;

public class WarningStatisticsSink extends RichSinkFunction<KafkaRecord<Monitor>> implements Serializable {

	private static final long serialVersionUID = 1L;

	Logger logger = LoggerFactory.getLogger(WarningStatisticsSink.class);
	Connection connection = null;

	private List<Monitor> queue;
	private List<Monitor> synchronizedCopy;

	private WarningStatisticsDao warningDao;
	private PreparedStatement updateWarningCommonTrip;

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

		// log.info("########## In LiveFleet Drive Time Management ##############");
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		warningDao = new WarningStatisticsDao();
		try {

			connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(DafConstants.DATAMART_POSTGRE_USER),
					envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));

			warningDao.setConnection(connection);
			System.out.println("warning connection created");

		} catch (Exception e) {

			logger.error("Error in Warning statistics Open method" + e.getMessage());

		}

	}

	public void invoke(KafkaRecord<Monitor> monitor) throws Exception {

		Monitor row = monitor.getValue();
		Integer messageTen = new Integer(10);
		Integer messageFour = new Integer(4);
		String vin;
		if (messageTen.equals(row.getMessageType()) || messageFour.equals(row.getMessageType())) {

			queue = new ArrayList<Monitor>();
			synchronizedCopy = new ArrayList<Monitor>();

			try {

				queue.add(row);

				if (queue.size() >= 1) {

					synchronized (synchronizedCopy) {
						synchronizedCopy = new ArrayList<Monitor>(queue);
						queue.clear();

						for (Monitor moniterData : synchronizedCopy) {
							// String tripID = "NOT AVAILABLE";

							/*
							 * if (moniterData.getDocument().getTripID() != null) { String tripID =
							 * moniterData.getDocument().getTripID(); }
							 */
						if(row.getVin()!=(null))	{
							vin=row.getVin();
						} else {
							vin=row.getVid();
						}
						//Long lastestProcessedMessageTimeStamp =warningDao.read(row.getMessageType(),vin);

							if (messageTen.equals(row.getMessageType())) {
								
								

								/*
								 * WarningStastisticsPojo warningDetail =
								 * WarningStatisticsCalculation(moniterData,
								 * messageTen,lastestProcessedMessageTimeStamp);
								 */
								
								WarningStastisticsPojo warningDetail = WarningStatisticsCalculation(moniterData,
										messageTen);
								warningDao.warning_insert(warningDetail);
								System.out.println("warning Inserted");
								logger.info("Warning records inserted to warning table :: ");
							}

							if (messageFour.equals(row.getMessageType())) {

								/*
								 * WarningStastisticsPojo warningDetail =
								 * WarningStatisticsCalculation(moniterData,
								 * messageFour,lastestProcessedMessageTimeStamp);
								 */
								WarningStastisticsPojo warningDetail = WarningStatisticsCalculation(moniterData,
										messageFour);
								warningDao.warning_insert(warningDetail);
								System.out.println("warning Inserted");
							}

						}
					}
				}
			} catch (Exception e) {
				logger.error("Error in Warning statistics Invoke method" + e.getMessage());
				e.printStackTrace();
			}

		}
		System.out.println("Invoke Finish Warning");
	}

	public WarningStastisticsPojo WarningStatisticsCalculation(Monitor row, Integer messageType) {
		
		WarningStastisticsPojo warningDetail = new WarningStastisticsPojo();
		//,Long lastestProcessedMessageTimeStamp-----add in parameter
		if (messageType == 10) {

			System.out.println("Inside warning calculation type 10");

			warningDetail.setTripId(row.getDocument().getTripID());
			warningDetail.setVin(row.getVin());
			warningDetail.setVid(row.getVid());
			try {
				warningDetail.setWarningTimeStamp(TimeFormatter.getInstance()
						.convertUTCToEpochMilli(row.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));
			} catch (ParseException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}

			warningDetail.setWarningClass(row.getDocument().getVWarningClass());
			warningDetail.setWarningNumber(row.getDocument().getVWarningNumber());

			if (row.getGpsLatitude() != null) {
				warningDetail.setLatitude(row.getGpsLatitude().doubleValue());
			} else {
				warningDetail.setLatitude(0.0);
			}

			if (row.getGpsLongitude() != null) {
				warningDetail.setLongitude(row.getGpsLongitude().doubleValue());
			} else {
				warningDetail.setLongitude(0.0);
			}

			if (row.getGpsHeading() != null) {
				warningDetail.setHeading(row.getGpsHeading().doubleValue());
			} else {
				warningDetail.setHeading(0.0);
			}

			// Vehicle Health Status
			if ((row.getDocument().getVWarningClass() != null)) {
				if (row.getDocument().getVWarningClass() >= 4 && row.getDocument().getVWarningClass() <= 7) {

					warningDetail.setVehicleHealthStatusType("T");

				} else if (row.getDocument().getVWarningClass() >= 8 && row.getDocument().getVWarningClass() <= 11) {

					warningDetail.setVehicleHealthStatusType("V");

				} else {
					warningDetail.setVehicleHealthStatusType("N");
				}
			} else {

				warningDetail.setVehicleHealthStatusType("Q ");
			}

			// Vehicle Driving status
			if (row.getDocument().getVWheelBasedSpeed() != null) {
				if (row.getDocument().getVWheelBasedSpeed() > 0) {
					warningDetail.setVehicleDrivingStatusType("D");
				} /*
					 * else if(row.getDocument().getVEngineSpeed()!=null &&
					 * row.getDocument().getVWheelBasedSpeed() == 0 &&
					 * row.getDocument().getVEngineSpeed() > 0) {
					 * warningDetail.setVehicleDrivingStatusType("I"); }
					 */
				else if (row.getDocument().getVWheelBasedSpeed() == 0) {
					warningDetail.setVehicleDrivingStatusType("S");
				}
			} else {
				warningDetail.setVehicleDrivingStatusType("Q");
			}

			
				warningDetail.setDriverID(null);
			

			// Warning Type
			if (row.getVEvtID() != null) {
				if (row.getVEvtID() == 44 || row.getVEvtID() == 46) {
					warningDetail.setWarningType("A");
				} else if (row.getVEvtID() == 45) {
					warningDetail.setWarningType("D");
				} else {
					warningDetail.setWarningType("Q");
				}
			} else {
				warningDetail.setWarningType("Q");
			}

			warningDetail.setDistanceUntilNextService(null);
			warningDetail.setOdometerVal(null);
			
				
			//warningDetail.setLastestProcessedMessageTimeStamp(lastestProcessedMessageTimeStamp);----revert it
			warningDetail.setLastestProcessedMessageTimeStamp(null);
			
			warningDetail.setCreatedAt(TimeFormatter.getInstance().getCurrentUTCTimeInSec());
			if(row.getMessageType()!=null) {
				warningDetail.setMessageType(row.getMessageType());
				} else {
					warningDetail.setMessageType(null);
				}
			
			// warningDetail.setModifiedAt(null);

			System.out.println("in vehicle warning class message 10---" + row.getVin());
			System.out.println("warning calculation Finished message 10");

		} else if (messageType == 4) {

			warningDetail.setTripId(null);
			warningDetail.setVin(row.getVin());
			warningDetail.setVid(row.getVid());
			try {
				warningDetail.setWarningTimeStamp(TimeFormatter.getInstance()
						.convertUTCToEpochMilli(row.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));
			} catch (ParseException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}

			warningDetail.setWarningClass(null);
			warningDetail.setWarningNumber(null);

			if (row.getGpsLatitude() != null) {
				warningDetail.setLatitude(row.getGpsLatitude().doubleValue());
			} else {
				warningDetail.setLatitude(0.0);
			}

			if (row.getGpsLongitude() != null) {
				warningDetail.setLongitude(row.getGpsLongitude().doubleValue());
			} else {
				warningDetail.setLongitude(0.0);
			}

			if (row.getGpsHeading() != null) {
				warningDetail.setHeading(row.getGpsHeading().doubleValue());
			} else {
				warningDetail.setHeading(0.0);
			}

			warningDetail.setVehicleHealthStatusType(null);

			// Vehicle Driving status
			// Vehicle Driving status
			if (row.getDocument().getVWheelBasedSpeed() != null) {
				if (row.getDocument().getVWheelBasedSpeed() > 0) {
					warningDetail.setVehicleDrivingStatusType("D");
				} /*
					 * else if (row.getDocument().getVEngineSpeed()!=null &&
					 * row.getDocument().getVWheelBasedSpeed() == 0 &&
					 * row.getDocument().getVEngineSpeed() > 0) {
					 * warningDetail.setVehicleDrivingStatusType("I"); }
					 */
				else if (row.getDocument().getVWheelBasedSpeed() == 0) {
					warningDetail.setVehicleDrivingStatusType("S");
				}
			} else {
				warningDetail.setVehicleDrivingStatusType("Q");
			}

			if (row.getDocument().getDriverID() != null) {
				warningDetail.setDriverID(row.getDocument().getDriverID());
			} else {
				warningDetail.setDriverID(null);
			}

			// Warning Type
			if (row.getVEvtID() != null) {
				if (row.getVEvtID() == 44 || row.getVEvtID() == 46) {
					warningDetail.setWarningType("A");
				} else if (row.getVEvtID() == 45) {
					warningDetail.setWarningType("D");
				} else {
					warningDetail.setWarningType("Q");
				}
			} else {
				warningDetail.setWarningType("Q");
			}

			if(row.getDocument().getVDistanceUntilService()!=null) {
			warningDetail.setDistanceUntilNextService(row.getDocument().getVDistanceUntilService().longValue());
			} else {
				warningDetail.setDistanceUntilNextService(null);
			}
			
			if(row.getDocument().getVDist()!=null) {
			warningDetail.setOdometerVal(row.getDocument().getVDist().longValue());
			} else {
				warningDetail.setOdometerVal(null);
			}
			warningDetail.setCreatedAt(TimeFormatter.getInstance().getCurrentUTCTimeInSec());
			
			if(row.getMessageType()!=null) {
			warningDetail.setMessageType(row.getMessageType());
			} else {
				warningDetail.setMessageType(null);
			}

			System.out.println("in vehicle warning class Message 4---" + row.getVin());
			System.out.println("warning calculation Finished Message 4");

		}

		return warningDetail;

	}

	/*
	 * public WarningStastisticsPojo WarningStatisticsCalculationTypeTen(Monitor
	 * row) { System.out.println("Inside warning calculation");
	 * 
	 * WarningStastisticsPojo warningDetail = new WarningStastisticsPojo();
	 * warningDetail.setTripId(row.getDocument().getTripID());
	 * warningDetail.setVin(row.getVin()); warningDetail.setVid(row.getVid()); try {
	 * warningDetail.setWarningTimeStamp(TimeFormatter.getInstance()
	 * .convertUTCToEpochMilli(row.getEvtDateTime().toString(),
	 * DafConstants.DTM_TS_FORMAT)); } catch (ParseException e) { // TODO
	 * Auto-generated catch block e.printStackTrace(); }
	 * 
	 * warningDetail.setWarningClass(row.getDocument().getVWarningClass());
	 * warningDetail.setWarningNumber(row.getDocument().getVWarningNumber());
	 * 
	 * if (row.getGpsLatitude() != null) {
	 * warningDetail.setLatitude(row.getGpsLatitude().doubleValue()); } else {
	 * warningDetail.setLatitude(0.0); }
	 * 
	 * if (row.getGpsLongitude() != null) {
	 * warningDetail.setLongitude(row.getGpsLongitude().doubleValue()); } else {
	 * warningDetail.setLongitude(0.0); }
	 * 
	 * if (row.getGpsHeading() != null) {
	 * warningDetail.setHeading(row.getGpsHeading().doubleValue()); } else {
	 * warningDetail.setHeading(0.0); }
	 * 
	 * if (row.getDocument().getVWarningClass() >= 4 &&
	 * row.getDocument().getVWarningClass() <= 7) {
	 * 
	 * warningDetail.setVehicleHealthStatusType("T");
	 * 
	 * } else if (row.getDocument().getVWarningClass() >= 8 &&
	 * row.getDocument().getVWarningClass() <= 11) {
	 * 
	 * warningDetail.setVehicleHealthStatusType("V");
	 * 
	 * } else { warningDetail.setVehicleHealthStatusType("N"); }
	 * 
	 * warningDetail.setVehicleDrivingStatusType(null);
	 * 
	 * if (row.getDocument().getDriverID() != null) {
	 * warningDetail.setDriverID(row.getDocument().getDriverID()); } else {
	 * warningDetail.setDriverID("unknown"); }
	 * 
	 * if (row.getVEvtID() == 44 || row.getVEvtID() == 46) {
	 * warningDetail.setWarningType("A"); } else if (row.getVEvtID() == 45) {
	 * warningDetail.setWarningType("D"); } else {
	 * warningDetail.setWarningType("Q"); }
	 * 
	 * // warningDetail.getDistanceUntilNextService(null); //
	 * warningDetail.getOdometer(null);
	 * warningDetail.setCreatedAt(TimeFormatter.getInstance().getCurrentUTCTimeInSec
	 * ()); // warningDetail.setModifiedAt(null);
	 * 
	 * System.out.println("in vehicle warning class---" + row.getVin());
	 * System.out.println("warning calculation Finished"); return warningDetail;
	 * 
	 * }
	 */
}
