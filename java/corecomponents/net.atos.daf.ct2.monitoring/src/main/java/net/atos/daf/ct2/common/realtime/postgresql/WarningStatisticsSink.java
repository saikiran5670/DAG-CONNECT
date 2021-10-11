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
import net.atos.daf.ct2.pojo.standard.Warning;
import net.atos.daf.postgre.bo.WarningStastisticsPojo;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.DTCWarningMasterDao;
import net.atos.daf.postgre.dao.WarningStatisticsDao;

public class WarningStatisticsSink extends RichSinkFunction<KafkaRecord<Monitor>> implements Serializable {

	private static final long serialVersionUID = 1L;

	Logger logger = LoggerFactory.getLogger(WarningStatisticsSink.class);
	Connection connection = null;

	private List<Monitor> queue;
	private List<Monitor> synchronizedCopy;

	private WarningStatisticsDao warningDao;
	private PreparedStatement updateWarningCommonTrip;
	private DTCWarningMasterDao DTCWarning;

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
			// System.out.println("warning connection created");

		} catch (Exception e) {

			logger.error("Error in Warning statistics Open method" + e.getMessage());

		}

	}

	public void invoke(KafkaRecord<Monitor> monitor) throws Exception {

		Monitor row = monitor.getValue();
		Integer messageTen = new Integer(10);
		Integer messageFour = new Integer(4);
		String vin;
		if ((messageTen.equals(row.getMessageType()) || messageFour.equals(row.getMessageType()))
				&& (row.getVEvtID() == 44 || row.getVEvtID() == 45 || row.getVEvtID() == 46 || row.getVEvtID() == 63)) {

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
							if (row.getVin() != (null)) {
								vin = row.getVin();
							} else {
								vin = row.getVid();
							}
							Long lastestProcessedMessageTimeStamp = warningDao.read(row.getMessageType(), vin);
							// System.out.println("lastestProcessedMessageTimeStamp value --" +
							// lastestProcessedMessageTimeStamp);

							if (messageTen.equals(row.getMessageType()) && row.getVEvtID() != 63) {

								WarningStastisticsPojo warningDetail = WarningStatisticsCalculation(moniterData,
										messageTen, lastestProcessedMessageTimeStamp,
										row.getDocument().getVWarningClass(), row.getDocument().getVWarningNumber());

								warningDao.warning_insert(warningDetail);
								logger.info("warning inserted in warning table for VEvtId not 63 :{)", warningDetail);
								// System.out.println("warning message 10 Inserted");
								warningDao.warningUpdateMessageTenCommonTrip(warningDetail);
								// System.out.println("warning updated for message 10 in another table");
								logger.info("Warning records inserted to warning table :: ");
								logger.info("Warning records updated in current trip table:: ", warningDetail);
							}

							if (messageTen.equals(row.getMessageType()) && row.getVEvtID() == 63) {

								List<Warning> warningList = row.getDocument().getWarningObject().getWarningList();

								for (Warning warning : warningList) {

									Long lastProcessedTimeStamp = warningDao.readRepairMaintenamce(row.getMessageType(),
											vin, warning.getWarningClass(), warning.getWarningNumber());
									if (lastProcessedTimeStamp == null) {
										// System.out.println("warning not present in warning table");

										WarningStastisticsPojo warningNewRowDetail = WarningStatisticsCalculation(row,
												messageTen, lastestProcessedMessageTimeStamp, warning.getWarningClass(),
												warning.getWarningNumber());
										warningDao.warning_insert(warningNewRowDetail);
										logger.info("warning inserted in warning table for VEvtId--63 :{)",
												warningNewRowDetail);
										// System.out.println("warning message 10 Inserted for evtId-63");
										warningDao.warningUpdateMessageTenCommonTrip(warningNewRowDetail);
										// System.out.println("warning updated for message 10 in another table for
										// evtId-63");
										logger.info("Warning records updated in current trip table:: ",
												warningNewRowDetail);

									}

								}
								
								//opposite side to update in DB for EVTId =45
								boolean warningPresent=false;
								List<Integer> toDeactivate= new ArrayList<>();
								List<WarningStastisticsPojo> activeWarnings=warningDao.readReturnListofActiveMsg(row.getMessageType(), row.getVin());
								List<Warning> warningList63 = row.getDocument().getWarningObject().getWarningList();
								if(activeWarnings!=null && !activeWarnings.isEmpty() && warningList63!=null && !warningList63.isEmpty()) {
								logger.info("activeWarnings list size ", activeWarnings.size());
								logger.info("warningList63 list size ", warningList63.size());
								for(WarningStastisticsPojo activeWarning :activeWarnings) {
									int warningClass= activeWarning.getWarningClass();
									int warningNumber= activeWarning.getWarningNumber();
										for( Warning war63 : warningList63) {
										 
											if(war63.getWarningClass().equals(warningClass) && war63.getWarningNumber().equals(warningNumber)) {
												warningPresent=true;
												break;
											} /*
												 * else { //update toDeactivate.add(warning.getId()); }
												 */ 
											 
									 }
										 if(warningPresent==false) { //update database here to deactivate warning for
											 toDeactivate.add(activeWarning.getId());
											 
											 
									 }
									
										 warningPresent=false; 
									
								} }
								logger.info("toDeactivate list size ", toDeactivate.size());
								
								warningDao.DeactivatWarningUpdate(toDeactivate);
							}

						}
					}
				}
			} catch (Exception e) {
				logger.error("Error in Warning statistics Invoke method" + e.getMessage());
				e.printStackTrace();
			}

		}
		// System.out.println("Invoke Finish Warning");
	}

	public WarningStastisticsPojo WarningStatisticsCalculation(Monitor row, Integer messageType,
			Long lastestProcessedMessageTimeStamp, Integer warningClass, Integer warningNumber) {

		WarningStastisticsPojo warningDetail = new WarningStastisticsPojo();
		// ,Long lastestProcessedMessageTimeStamp-----add in parameter
		if (messageType == 10) {

			// System.out.println("Inside warning calculation type 10");

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

			// warningDetail.setWarningClass(row.getDocument().getVWarningClass());
			// warningDetail.setWarningNumber(row.getDocument().getVWarningNumber());

			warningDetail.setWarningClass(warningClass);
			warningDetail.setWarningNumber(warningNumber);

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
			if (warningClass != null) {
				if (warningClass >= 4 && warningClass <= 7) {

					warningDetail.setVehicleHealthStatusType("T");

				} else if (warningClass >= 8 && warningClass <= 10) {

					warningDetail.setVehicleHealthStatusType("V");

				} else {
					warningDetail.setVehicleHealthStatusType("N");
				}
			} else {

				warningDetail.setVehicleHealthStatusType("N");
			}

			// Vehicle Driving status
			if (row.getDocument().getVWheelBasedSpeed() != null) {
				if (row.getDocument().getVWheelBasedSpeed() > 0) {
					warningDetail.setVehicleDrivingStatusType("D");
				} else if (row.getDocument().getVEngineSpeed() != null && row.getDocument().getVWheelBasedSpeed() == 0
						&& row.getDocument().getVEngineSpeed() > 0) {
					warningDetail.setVehicleDrivingStatusType("I");
				}

				else if (row.getDocument().getVWheelBasedSpeed() == 0) {
					warningDetail.setVehicleDrivingStatusType("S");
				}
			} else {
				warningDetail.setVehicleDrivingStatusType("I");
			}

			warningDetail.setDriverID(null);

			// Warning Type
			if (row.getVEvtID() != null) {
				if (row.getVEvtID() == 44 || row.getVEvtID() == 46 || row.getVEvtID() == 63) {
					warningDetail.setWarningType("A");
				} else if (row.getVEvtID() == 45) {
					warningDetail.setWarningType("D");
				} 
			} 

			warningDetail.setDistanceUntilNextService(null);
			warningDetail.setOdometerVal(null);

			if (lastestProcessedMessageTimeStamp != null) {
				warningDetail.setLastestProcessedMessageTimeStamp(lastestProcessedMessageTimeStamp);
			} else {
				warningDetail.setLastestProcessedMessageTimeStamp(null);
			}

			warningDetail.setCreatedAt(TimeFormatter.getInstance().getCurrentUTCTime());
			if (row.getMessageType() != null) {
				warningDetail.setMessageType(row.getMessageType());
			} else {
				warningDetail.setMessageType(null);
			}

			// warningDetail.setModifiedAt(null);

			// System.out.println("in vehicle warning class message 10---" + row.getVin());
			// System.out.println("warning calculation Finished message 10");

			logger.info("warning calculation finished {}", warningDetail);

		}

		return warningDetail;

	}

	@Override
	public void close() throws Exception {

		super.close();

		logger.info("In close() of Warning :: ");

		if (connection != null) {
			logger.info("Releasing connection from Warning job");
			connection.close();
		}

	}
}
