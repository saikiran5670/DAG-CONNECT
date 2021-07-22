package net.atos.daf.ct2.common.realtime.postgresql;

import java.io.Serializable;
import java.sql.Connection;
import java.text.ParseException;
import java.util.ArrayList;
import java.util.List;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.apache.kafka.common.utils.Exit;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.common.realtime.dataprocess.MonitorDataProcess;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.postgre.bo.DriverActivityPojo;
import net.atos.daf.postgre.bo.TwoMinuteRulePojo;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.LiveFleetDriverActivityDao;

public class DriverTimeManagementSink extends RichSinkFunction<KafkaRecord<Monitor>> implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	Logger logger = LoggerFactory.getLogger(MonitorDataProcess.class);

	// private PreparedStatement statement;
	Connection connection = null;

	Connection masterConnection = null;

	LiveFleetDriverActivityDao driverDAO;

	private List<Monitor> queue;
	private List<Monitor> synchronizedCopy;

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

		logger.info("########## In LiveFleet Drive Time Management ##############");
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		driverDAO = new LiveFleetDriverActivityDao();
		System.out.println("read Query--->" + net.atos.daf.postgre.util.DafConstants.DRIVER_ACTIVITY_READ);
		System.out.println("update Query--->" + net.atos.daf.postgre.util.DafConstants.DRIVER_ACTIVITY_UPDATE);
		try {

			connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(DafConstants.DATAMART_POSTGRE_USER),
					envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));

			driverDAO.setConnection(connection);

		} catch (Exception e) {

			logger.error("Error in Live fleet position" + e.getMessage());

		}

	}

	public void invoke(KafkaRecord<Monitor> monitor) throws Exception {

		Monitor row = monitor.getValue();
		if("abcd".equalsIgnoreCase(row.getDocument().getTripID())) {
			System.out.println("findigs--" + row);
		}
		
		Integer value = new Integer(7);
		if (value.equals(row.getMessageType())) {
			// if (row.getMessageType().equals(7)) {
			queue = new ArrayList<Monitor>();
			synchronizedCopy = new ArrayList<Monitor>();

			try {

				queue.add(row);

				if (queue.size() >= 1) {

					System.out.println("inside syncronized");
					synchronized (synchronizedCopy) {
						synchronizedCopy = new ArrayList<Monitor>(queue);
						queue.clear();
						int i=0;
						/*
						 * for (Monitor monitorData : synchronizedCopy) { if
						 * (("driver1").equalsIgnoreCase(monitorData.getDocument().getDriverID())) {
						 * System.out.print("moniterData sequence: "+i +
						 * "eventTime:"+monitorData.getEvtDateTime()); i++; } else {
						 * System.out.println("no driver1"); }
						 * 
						 * }
						 */
					
						for (Monitor monitorData : synchronizedCopy) {
							Long currentEndTime = null;

							try {
								currentEndTime = (TimeFormatter.getInstance().convertUTCToEpochMilli(
										monitorData.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));
							} catch (ParseException e) {
								// TODO Auto-generated catch block
								logger.error("Error in Live fleet position- error in parsing event Date Time" + e.getMessage());
								e.printStackTrace();
							}

							// -----**** DRIVER 1

							logger.info("Read Driver details from previous row");
							TwoMinuteRulePojo previousDriverOneDetails=null;
							if(null!=monitorData.getDocument().getDriver1WorkingState()) {
								 previousDriverOneDetails = driverDAO.driver_read(
										monitorData.getDocument().getDriverID(),
										monitorData.getDocument().getDriver1WorkingState().toString());
							} else {
								break;
							}
							


							if (previousDriverOneDetails != null) {
								Long driverOneStartTime = previousDriverOneDetails.getStart_time();
								String previousCode1 = previousDriverOneDetails.getCode();

								Long duration1 = currentEndTime - driverOneStartTime;

								if (previousCode1.equalsIgnoreCase("2") && duration1 <= 120000) {
									logger.info("Two minute rule calculation");
									String logicalCode = monitorData.getDocument().getDriver1WorkingState().toString();
									driverDAO.driver_update(monitorData.getDocument().getDriverID(), currentEndTime, duration1,
											logicalCode);
									logger.info("Driver1 records updated in driver table with twoMinuiteRule :: ");

								} else {
									driverDAO.driver_update(monitorData.getDocument().getDriverID(), currentEndTime, duration1,
											previousCode1);
									logger.info("Driver1 records updated in driver table :: ");
								}
								DriverActivityPojo DriverDetailsD1 = driverActivityCalculation(monitorData, true);
								driverDAO.driver_insert(DriverDetailsD1);
								logger.info("Driver1 records inserted in driver table :: ");

							} else {

								DriverActivityPojo DriverDetailsD1 = driverActivityCalculation(monitorData, true);
								driverDAO.driver_insert(DriverDetailsD1);
								logger.info("Driver1 new records inserted in driver table :: ");
							}
							
							// --------** DRIVER 2

							TwoMinuteRulePojo previousDriver2Details = driverDAO.driver_read(
									monitorData.getDocument().getDriver2ID(),
									monitorData.getDocument().getDriver2WorkingState().toString());

							if (previousDriver2Details != null) {
								
								Long driverTwoStartTime = previousDriver2Details.getStart_time();
								String previousDriverTwoCode = previousDriver2Details.getCode();

								Long duration2 = currentEndTime - driverTwoStartTime;
								if (previousDriverTwoCode.equalsIgnoreCase("2") && duration2 <= 120000) {

									String FormattedCode = monitorData.getDocument().getDriver2WorkingState()
											.toString();

									driverDAO.driver_update(monitorData.getDocument().getDriver2ID(), currentEndTime,
											duration2, FormattedCode);
									logger.info("Driver2 records updated in driver table with twoMinuiteRule :: ");

								} else {
									/*
									 * driverDAO.driver_update(monitorData.getDocument().getDriver2ID(), endTime,
									 * duration2,monitorData.getDocument().getDriver2WorkingState().toString());
									 */

									

									driverDAO.driver_update(monitorData.getDocument().getDriver2ID(), currentEndTime,
											duration2, previousDriverTwoCode);
									logger.info("Driver2 record updated in driver table :: ");
								}

								DriverActivityPojo DriverDetailsD2 = driverActivityCalculation(monitorData,false);

								
								driverDAO.driver_insert(DriverDetailsD2);
								logger.info("Driver2 record inserted in driver table :: ");

							} else {

								DriverActivityPojo DriverDetailsD2 = driverActivityCalculation(monitorData, false);

								

								driverDAO.driver_insert(DriverDetailsD2);
								logger.info("Driver2 new record inserted in driver table :: ");
							}

							/*
							 * driverStartTime =
							 * driverDAO.driver_read(monitorData.getDocument().getDriver2ID(),
							 * monitorData.getDocument().getDriver2WorkingState().toString());
							 * 
							 * if (driverStartTime != null) {
							 * 
							 * Long duration = endTime - driverStartTime;
							 * driverDAO.driver_update(monitorData.getDocument().getDriver2ID(), endTime,
							 * duration);
							 * 
							 * DriverActivityPojo DriverDetailsD2 = DriverActivityCalculation(monitorData,
							 * DriverActivity, false);
							 * 
							 * driverDAO.driver_insert(DriverDetailsD2);
							 * 
							 * } else {
							 * 
							 * DriverActivityPojo DriverDetailsD2 = DriverActivityCalculation(monitorData,
							 * DriverActivity, false);
							 * 
							 * driverDAO.driver_insert(DriverDetailsD2); }
							 */

							// -----------------
						}
					}
				}
			} catch (Exception e) {
				logger.error("Error in Live fleet position, Invoke Method" + e.getMessage());
				e.printStackTrace();
			}

		}

	}

	public DriverActivityPojo driverActivityCalculation(Monitor row, boolean driverIdentification) {

		DriverActivityPojo driverActivity = new DriverActivityPojo();

		driverActivity.setTripId(row.getDocument().getTripID());
		driverActivity.setVid(row.getVid());
		// DriverActivity.setVin(null);
		// DriverActivity.setTripStartTimeStamp(Types);
		// DriverActivity.setTripEndTimeStamp(null);
		try {
			driverActivity.setActivityDate(TimeFormatter.getInstance()
					.convertUTCToEpochMilli(row.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));
		} catch (ParseException e) {
			// TODO Auto-generated catch block
			logger.error("Error in Live fleet position, ActivityDate calculation" + e.getMessage());
			e.printStackTrace();
		} 

		if (driverIdentification == true) {
			// Driver 1

			driverActivity.setDriverID(row.getDocument().getDriverID());
			driverActivity.setCode(row.getDocument().getDriver1WorkingState().toString());
			driverActivity.setIsDriver1(true);
			driverActivity.setLogicalCode(row.getDocument().getDriver1WorkingState().toString());
		} else {
			// Driver 2

			driverActivity.setDriverID(row.getDocument().getDriver2ID());
			driverActivity.setCode(row.getDocument().getDriver2WorkingState().toString());
			driverActivity.setIsDriver1(false);
			driverActivity.setLogicalCode(row.getDocument().getDriver2WorkingState().toString());
		}

		try {
			driverActivity.setStartTime(TimeFormatter.getInstance()
					.convertUTCToEpochMilli(row.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));
		} catch (ParseException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} // start time

		try {
			driverActivity.setEndTime(TimeFormatter.getInstance()
					.convertUTCToEpochMilli(row.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));
		} catch (ParseException e) {
			// TODO Auto-generated catch block
			logger.error("Error in Live fleet position, setEndDate" + e.getMessage());
			e.printStackTrace();
		} // end-time

		driverActivity.setDuration(null); // it will be null when record creates.

		driverActivity.setCreatedAtDm(TimeFormatter.getInstance().getCurrentUTCTimeInSec());
		// DriverActivity.setCreated_at_kafka(row.getReceivedTimestamp());
		driverActivity.setCreatedAtKafka(Long.parseLong(row.getKafkaProcessingTS()));
		driverActivity.setCreatedAtM2m(row.getReceivedTimestamp());
		// DriverActivity.setModified_at(TimeFormatter.getInstance().getCurrentUTCTimeInSec());
		driverActivity.setModifiedAt(null); // it will be null when record
											// creates.
		driverActivity.setLastProcessedMessageTimestamp(TimeFormatter.getInstance().getCurrentUTCTimeInSec());
		driverActivity.setVin(row.getVin());
		System.out.println("in driver activity sink class---" + row.getVin());
		return driverActivity;

	}

}
