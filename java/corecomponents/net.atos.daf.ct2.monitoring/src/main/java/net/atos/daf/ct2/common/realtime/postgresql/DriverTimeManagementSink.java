package net.atos.daf.ct2.common.realtime.postgresql;

import java.io.Serializable;
import java.sql.Connection;
import java.text.ParseException;
import java.util.ArrayList;
import java.util.List;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.apache.kafka.common.utils.Exit;


import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.common.realtime.dataprocess.MonitorDataProcess;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.postgre.bo.DriverActivityPojo;
import net.atos.daf.postgre.bo.TwoMinuteRulePojo;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.LiveFleetDriverActivityDao;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

public class DriverTimeManagementSink extends RichSinkFunction<Monitor> implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	//Logger logger = LoggerFactory.getLogger(DriverTimeManagementSink.class);
	private static final Logger logger = LogManager.getLogger(MonitorDataProcess.class);

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
		
		 int pId = getRuntimeContext().getIndexOfThisSubtask();
		 logger.info("PID value {}",  pId);

		driverDAO = new LiveFleetDriverActivityDao();
		// System.out.println("read Query--->" +
		// net.atos.daf.postgre.util.DafConstants.DRIVER_ACTIVITY_READ);
		// System.out.println("update Query--->" +
		// net.atos.daf.postgre.util.DafConstants.DRIVER_ACTIVITY_UPDATE);
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
	
	public void invoke(Monitor monitor) throws Exception {
		 net.atos.daf.ct2.common.models.Monitor monitorChild=(net.atos.daf.ct2.common.models.Monitor)monitor;
		 System.out.println("inside invoke");
		 
		DriverActivityPojo DriverDetailsD1 = driverActivityCalculation(monitorChild, true);
		driverDAO.driver_insert(DriverDetailsD1);
		
		DriverActivityPojo DriverDetailsD2 = driverActivityCalculation(monitorChild, false);
		driverDAO.driver_insert(DriverDetailsD2);
		
		
	}



	public DriverActivityPojo driverActivityCalculation( net.atos.daf.ct2.common.models.Monitor row, boolean driverIdentification) {

		DriverActivityPojo driverActivity = new DriverActivityPojo();
		System.out.println("inside calculation");

		driverActivity.setTripId(row.getDocument().getTripID());
		driverActivity.setVid(row.getVid());
		// DriverActivity.setVin(null);
		// DriverActivity.setTripStartTimeStamp(Types);
		// DriverActivity.setTripEndTimeStamp(null);
		try {
			/*
			 * driverActivity.setActivityDate(TimeFormatter.getInstance()
			 * .convertUTCToEpochMilli(row.getEvtDateTime().toString(),
			 * DafConstants.DTM_TS_FORMAT));
			 */
			
			driverActivity.setActivityDate(row.getStartTime());
		} catch (Exception e) {
			// TODO Auto-generated catch block
			logger.error("Error in Live fleet position, ActivityDate calculation" + e.getMessage());
			e.printStackTrace();
		}

		if (driverIdentification == true) {
			// Driver 1

			if(row.getDocument().getDriverID()!=null && ! row.getDocument().getDriverID().isEmpty()) {
				driverActivity.setDriverID(row.getDocument().getDriverID());
			} else {
				driverActivity.setDriverID("Unknown");
			}
			//driverActivity.setCode(row.getDocument().getDriver1WorkingState().toString());
			driverActivity.setCode(row.getDriverState());
			driverActivity.setIsDriver1(true);
			driverActivity.setLogicalCode(row.getDocument().getDriver1WorkingState().toString());
		} else {
			// Driver 2

			if(row.getDocument().getDriver2ID()!=null && ! row.getDocument().getDriver2ID().isEmpty()) {
				driverActivity.setDriverID(row.getDocument().getDriver2ID());
			} else {
				driverActivity.setDriverID("Unknown");
			}
			driverActivity.setCode(row.getDriverState());
			driverActivity.setIsDriver1(false);
			driverActivity.setLogicalCode(row.getDocument().getDriver2WorkingState().toString());
		}

		/*
		 * try { driverActivity.setStartTime(TimeFormatter.getInstance()
		 * .convertUTCToEpochMilli(row.getEvtDateTime().toString(),
		 * DafConstants.DTM_TS_FORMAT)); } catch (ParseException e) { // TODO
		 * Auto-generated catch block e.printStackTrace(); } // start time
		 * 
		 * try { driverActivity.setEndTime(TimeFormatter.getInstance()
		 * .convertUTCToEpochMilli(row.getEvtDateTime().toString(),
		 * DafConstants.DTM_TS_FORMAT)); } catch (ParseException e) { // TODO
		 * Auto-generated catch block
		 * logger.error("Error in Live fleet position, setEndDate" + e.getMessage());
		 * e.printStackTrace(); } // end-time
		 */
		
		driverActivity.setStartTime(row.getStartTime());
		driverActivity.setEndTime(row.getEndTime());
		
		driverActivity.setDuration(row.getDuration()); // it will be null when record creates.

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

	@Override
	public void close() throws Exception {
		super.close();

		logger.info("In close() of DriverActivity :: ");

		if (connection != null) {
			logger.info("Releasing connection from DriverActivity job");
			connection.close();
		}
		if (masterConnection != null) {
			logger.info("Releasing connection from DriverActivity job");
			masterConnection.close();
		}

	}

}
