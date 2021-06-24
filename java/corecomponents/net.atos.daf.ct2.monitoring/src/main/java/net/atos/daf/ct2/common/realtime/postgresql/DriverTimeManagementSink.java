package net.atos.daf.ct2.common.realtime.postgresql;

import java.io.Serializable;
import java.sql.Connection;
import java.text.ParseException;
import java.util.ArrayList;
import java.util.List;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.common.realtime.dataprocess.MonitorDataProcess;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.postgre.bo.DriverActivityPojo;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.LiveFleetDriverActivityDao;

public class DriverTimeManagementSink extends RichSinkFunction<KafkaRecord<Monitor>> implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	Logger log = LoggerFactory.getLogger(MonitorDataProcess.class);


	// private PreparedStatement statement;
	Connection connection = null;

	Connection masterConnection = null;

	LiveFleetDriverActivityDao driverDAO;

	DriverActivityPojo DriverActivity;

	private List<Monitor> queue;
	private List<Monitor> synchronizedCopy;

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

		log.info("########## In LiveFleet Drive Time Management ##############");
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		driverDAO = new LiveFleetDriverActivityDao();
		System.out.println("read Query--->"+net.atos.daf.postgre.util.DafConstants.DRIVER_ACTIVITY_READ);
		System.out.println("update Query--->"+net.atos.daf.postgre.util.DafConstants.DRIVER_ACTIVITY_UPDATE);
		try {
			
			connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(DafConstants.DATAMART_POSTGRE_USER),
					envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));

			driverDAO.setConnection(connection);

		} catch (Exception e) {

			log.error("Error in Live fleet position" + e.getMessage());

		}

	}

	public void invoke(KafkaRecord<Monitor> monitor) throws Exception {

		Monitor row = monitor.getValue();
		Integer value=new Integer(7);
		if (value.equals(row.getMessageType())) {
		//if (row.getMessageType().equals(7)) {
			queue = new ArrayList<Monitor>();
			synchronizedCopy = new ArrayList<Monitor>();

			DriverActivity = new DriverActivityPojo();

			try {
				
				queue.add(row);

				if (queue.size() >= 1) {

					System.out.println("inside syncronized");
					synchronized (synchronizedCopy) {
						synchronizedCopy = new ArrayList<Monitor>(queue);
						queue.clear();
						Long endTime = null;
						for (Monitor monitorData : synchronizedCopy) {

							try {
								endTime = (TimeFormatter.getInstance().convertUTCToEpochMilli(
										monitorData.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT ));
							} catch (ParseException e) {
								// TODO Auto-generated catch block
								e.printStackTrace();
							}

							// -----**** DRIVER 1
							
							Long driverStartTime = driverDAO.driver_read(monitorData.getDocument().getDriverID(),
									monitorData.getDocument().getDriver1WorkingState().toString());
							
							if (driverStartTime != null) {
								
								Long duration = endTime - driverStartTime;
								
								driverDAO.driver_update(monitorData.getDocument().getDriverID(), endTime, duration);
								DriverActivityPojo DriverDetailsD1 = DriverActivityCalculation(monitorData,
										DriverActivity, true);

								driverDAO.driver_insert(DriverDetailsD1);

							} else {
								
								DriverActivityPojo DriverDetailsD1 = DriverActivityCalculation(monitorData,
										DriverActivity, true);

								driverDAO.driver_insert(DriverDetailsD1);
							}
							/// --------------------------------
							// --------** DRIVER 2

							
							driverStartTime = driverDAO.driver_read(monitorData.getDocument().getDriver2ID(),
									monitorData.getDocument().getDriver2WorkingState().toString());

							
							if (driverStartTime != null) {

								
								Long duration = endTime - driverStartTime;
								driverDAO.driver_update(monitorData.getDocument().getDriver2ID(), endTime, duration);

								DriverActivityPojo DriverDetailsD2 = DriverActivityCalculation(monitorData,
										DriverActivity, false);

								driverDAO.driver_insert(DriverDetailsD2);

							} else {
								
								DriverActivityPojo DriverDetailsD2 = DriverActivityCalculation(monitorData,
										DriverActivity, false);

								driverDAO.driver_insert(DriverDetailsD2);
							}

							// -----------------
						}
					}
				}
			} catch (Exception e) {
				e.printStackTrace();
			}

		}

	}

	public DriverActivityPojo DriverActivityCalculation(Monitor row, DriverActivityPojo DriverActivity,
			boolean driverIdentification) {

		DriverActivity.setTripId(null);
		DriverActivity.setVid(row.getVid());
		//DriverActivity.setVin(null);
		//DriverActivity.setTripStartTimeStamp(Types);
		//DriverActivity.setTripEndTimeStamp(null);
		try {
			DriverActivity.setActivityDate(TimeFormatter.getInstance()
					.convertUTCToEpochMilli(row.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT ));
		} catch (ParseException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} // activity_date

		
		if (driverIdentification == true) {
			// Driver 1
			
			DriverActivity.setDriverID(row.getDocument().getDriverID());
			DriverActivity.setCode(row.getDocument().getDriver1WorkingState().toString());
			DriverActivity.setIsDriver1(true);
		} else {
			// Driver 2
			
			DriverActivity.setDriverID(row.getDocument().getDriver2ID());
			DriverActivity.setCode(row.getDocument().getDriver2WorkingState().toString());
			DriverActivity.setIsDriver1(false);

		}

		try {
			DriverActivity.setStartTime(TimeFormatter.getInstance()
					.convertUTCToEpochMilli(row.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));
		} catch (ParseException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} // start time

		try {
			DriverActivity.setEndTime(TimeFormatter.getInstance()
					.convertUTCToEpochMilli(row.getEvtDateTime().toString(), DafConstants.DTM_TS_FORMAT));
		} catch (ParseException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} // end-time

		DriverActivity.setDuration(null); // it will be null when record creates.

		DriverActivity.setCreatedAtDm(TimeFormatter.getInstance().getCurrentUTCTimeInSec());
		// DriverActivity.setCreated_at_kafka(row.getReceivedTimestamp());
		DriverActivity.setCreatedAtKafka(Long.parseLong(row.getKafkaProcessingTS()));
		DriverActivity.setCreatedAtM2m(row.getReceivedTimestamp());
		// DriverActivity.setModified_at(TimeFormatter.getInstance().getCurrentUTCTimeInSec());
		DriverActivity.setModifiedAt(null); // it will be null when record
											// creates.
		DriverActivity.setLastProcessedMessageTimestamp(TimeFormatter.getInstance().getCurrentUTCTimeInSec());
		DriverActivity.setVin(row.getVin());
		System.out.println("in driver activity sink class---"+row.getVin());
		return DriverActivity;

	}

}
