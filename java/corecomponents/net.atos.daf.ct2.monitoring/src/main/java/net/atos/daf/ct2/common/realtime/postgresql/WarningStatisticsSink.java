package net.atos.daf.ct2.common.realtime.postgresql;

import static net.atos.daf.ct2.common.util.Utils.convertDateToMillis;
import static net.atos.daf.ct2.common.util.Utils.getCurrentTimeInUTC;

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
import net.atos.daf.postgre.bo.WarningStatisticsPojo;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.DTCWarningMasterDao;
import net.atos.daf.postgre.dao.WarningStatisticsDao;

public class WarningStatisticsSink extends RichSinkFunction<KafkaRecord<Monitor>> implements Serializable {

	private static final long serialVersionUID = 1L;

	private static final Logger logger = LoggerFactory.getLogger(WarningStatisticsSink.class);
	Connection connection = null;

	private List<Monitor> queue;
	private List<Monitor> synchronizedCopy;

	private WarningStatisticsDao warningDao;
	private PreparedStatement updateWarningCommonTrip;
	private DTCWarningMasterDao DTCWarning;
	private PreparedStatement statement;
	private PreparedStatement updateStatementCurrentTrip;
	private PreparedStatement updateStatementList;
	private PreparedStatement readStatementList;
	private PreparedStatement readStatement;
	//private PreparedStatement deactivateWarningStatement;
	
//convertDateToMillis(indexData.getEvtDateTime())
	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
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
			statement = connection.prepareStatement(DafConstants.LIVEFLEET_WARNING_INSERT);
			updateStatementCurrentTrip=connection.prepareStatement(DafConstants.LIVEFLEET_CURRENT_TRIP_STATISTICS_UPDATE_TEN);
			updateStatementList=connection.prepareStatement(DafConstants.LIVEFLEET_WARNING_UPDATELIST);
			readStatementList=connection.prepareStatement(DafConstants.LIVEFLEET_WARNING_READLIST);
			readStatement=connection.prepareStatement(DafConstants.REPAITM_MAINTENANCE_WARNING_READ);
			//deactivateWarningStatement=connection.prepareStatement(DafConstants.LIVEFLEET_WARNING_DEACTIVATE);
			

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

		/*	queue = new ArrayList<Monitor>();
			synchronizedCopy = new ArrayList<Monitor>();

			try {

				queue.add(row);

				if (queue.size() >= 1) {

					synchronized (synchronizedCopy) {
						synchronizedCopy = new ArrayList<Monitor>(queue);
						queue.clear();

						for (Monitor moniterData : synchronizedCopy) {*/
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
							
							
							if (messageTen.equals(row.getMessageType()) && (row.getVEvtID()==44 || row.getVEvtID()==45)) {

								WarningStatisticsPojo warningDetail = WarningStatisticsCalculation(row,
										messageTen, 
										row.getDocument().getVWarningClass(), row.getDocument().getVWarningNumber());

								warningDao.warning_insertMonitor(warningDetail,statement);
								logger.debug("warning inserted in warning table for VEvtId not 63 :{)", warningDetail);
								warningDao.warningUpdateMessageTenCommonTripMonitor(warningDetail,updateStatementCurrentTrip);
								logger.info("Warning records inserted to warning table :: {}",warningDetail);
								logger.debug("Warning records updated in current trip table:: ", warningDetail);
							}
							
						/*	if(messageTen.equals(row.getMessageType()) && row.getVEvtID() == 45) {
								
								WarningStatisticsPojo warningDetail = WarningStatisticsCalculation(row,
										messageTen, 
										row.getDocument().getVWarningClass(), row.getDocument().getVWarningNumber());
								
								warningDao.DeactivatSingleWarning(row, deactivateWarningStatement);
								warningDao.warningUpdateMessageTenCommonTripMonitor(warningDetail,updateStatementCurrentTrip);
								
							}*/
							if(messageTen.equals(row.getMessageType()) && row.getVEvtID() == 46) {
								boolean warningStatus = warningDao.readRepairMaintenamceMonitor(row.getMessageType(), vin,row.getDocument().getVWarningClass(),row.getDocument().getVWarningNumber(),readStatement);
								if(warningStatus) {
									WarningStatisticsPojo warningDetail = WarningStatisticsCalculation(row,
											messageTen, 
											row.getDocument().getVWarningClass(), row.getDocument().getVWarningNumber());
									warningDao.warning_insertMonitor(warningDetail,statement);
									logger.debug("warning inserted in warning table for VEvtId not 63 :{)", warningDetail);
									warningDao.warningUpdateMessageTenCommonTripMonitor(warningDetail,updateStatementCurrentTrip);
									logger.info("Warning records inserted to warning table :: {}",warningDetail);
									logger.debug("Warning records updated in current trip table:: ", warningDetail);
								}
								
							}

							if (messageTen.equals(row.getMessageType()) && row.getVEvtID() == 63) {
								
								List<WarningStatisticsPojo> activeWarnings=warningDao.readReturnListofActiveMsg(row.getMessageType(), row.getVin(),readStatementList);
								List<Warning> warningList63 = row.getDocument().getWarningObject().getWarningList();
								//List<WarningStatisticsPojo> toAddWaringList= new ArrayList<>();
								
								//To insert missed rows in db for message 63
								boolean warningInDB=false; 
								//List<WarningStatisticsPojo> toInsert= new ArrayList<>();
								
								for(Warning warning63 : warningList63) {
									int activeWarningClass=	warning63.getWarningClass();
									int activeWarningNumber= warning63.getWarningNumber();
									
									for(WarningStatisticsPojo warningPreseentInDB :activeWarnings) {

										if(warningPreseentInDB.getWarningNumber().equals(activeWarningNumber) && warningPreseentInDB.getWarningClass().equals(activeWarningClass)) {
											warningInDB=true;
											break;
										}
										
									}

									if(warningInDB==false) { //update database to insert warning
										WarningStatisticsPojo warningRowToInsert = WarningStatisticsCalculation(row,
												messageTen, warning63.getWarningClass(),
												warning63.getWarningNumber());
										warningDao.warning_insertMonitor(warningRowToInsert,statement);
										warningDao.warningUpdateMessageTenCommonTripMonitor(warningRowToInsert,updateStatementCurrentTrip);
										
										logger.info("warning inserted in warning table for VEvtId--63 :{}",
												warningRowToInsert);
									}
									warningInDB=false;

								}
									
									
									
									
								//opposite side to update in DB for EVTId =45
								boolean warningPresent=false;
								List<Integer> toDeactivate= new ArrayList<>();
								
								
								if(activeWarnings!=null && !activeWarnings.isEmpty() && warningList63!=null && !warningList63.isEmpty()) {
								logger.debug("activeWarnings list size " + activeWarnings.size());
								logger.debug("warningList63 list size "  + warningList63.size());
								for(WarningStatisticsPojo activeWarning :activeWarnings) {


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
								logger.debug("toDeactivate list size " + toDeactivate.size());
								
								warningDao.DeactivatWarningUpdate(toDeactivate,updateStatementList);
								logger.debug("update done-sink class ");
							}

						}
		/*}
				}
			} catch (Exception e) {
				logger.error("Error in Warning statistics Invoke method {}" , e.getMessage());
			}

		}*/
	}

	public WarningStatisticsPojo WarningStatisticsCalculation(Monitor row, Integer messageType, Integer warningClass, Integer warningNumber) {

		WarningStatisticsPojo warningDetail = new WarningStatisticsPojo();
		if (messageType == 10) {
			warningDetail.setTripId(null);
			warningDetail.setVin(row.getVin());
			warningDetail.setVid(row.getVid());
			
				/*warningDetail.setWarningTimeStamp(TimeFormatter.getInstance()
						.convertUTCToEpochMilli(row.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));*/
				
				warningDetail.setWarningTimeStamp(convertDateToMillis(row.getEvtDateTime()));
			

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

			//if (lastestProcessedMessageTimeStamp != null) {
				//warningDetail.setLastestProcessedMessageTimeStamp(lastestProcessedMessageTimeStamp);
			//} else {
				warningDetail.setLastestProcessedMessageTimeStamp(0L);
			//}

			//warningDetail.setCreatedAt(TimeFormatter.getInstance().getCurrentUTCTime());
			
			warningDetail.setCreatedAt(getCurrentTimeInUTC());
			
			if (row.getMessageType() != null) {
				warningDetail.setMessageType(row.getMessageType());
			} else {
				warningDetail.setMessageType(null);
			}
			logger.info("warning calculation finished {}", warningDetail);

		}

		return warningDetail;

	}

	@Override
	public void close() throws Exception {

		super.close();
		if (statement != null) {
			statement.close();
		}
		
		if (updateStatementCurrentTrip != null) {
			updateStatementCurrentTrip.close();
		}
		
		if (updateStatementList != null) {
			updateStatementList.close();
		}
		
		if (readStatementList != null) {
			readStatementList.close();
		}
		
		if (readStatement != null) {
			readStatement.close();
		}
		
		
		logger.info("In close() of Warning :: ");

		if (connection != null) {
			logger.debug("Releasing connection from Warning job");
			connection.close();
		}

	}
}
