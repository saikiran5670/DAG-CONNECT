package net.atos.daf.ct2.service.realtime;

import java.io.Serializable;
import java.sql.Connection;
import java.text.ParseException;
import java.util.List;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.ProcessFunction;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.postgre.PostgreDataSourceConnection;
import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.pojo.standard.Monitor;
import net.atos.daf.ct2.pojo.standard.Warning;
import net.atos.daf.ct2.props.AlertConfigProp;

import net.atos.daf.postgre.bo.WarningStastisticsPojo;
import net.atos.daf.postgre.dao.DTCWarningMasterDao;
import net.atos.daf.postgre.dao.WarningStatisticsDao;

public class RepairMaintenance extends ProcessFunction<Monitor, Monitor> implements Serializable {

	private static final Logger logger = LoggerFactory.getLogger(RepairMaintenance.class);
	private static final long serialVersionUID = 1L;
	Connection masterConnection = null;
	private DTCWarningMasterDao DTCWarning;
	private WarningStatisticsDao warningDao;
	Connection connection = null;
	private ParameterTool parameterTool;
	
	
	 public RepairMaintenance(ParameterTool parameterTool1){
		 parameterTool=parameterTool1;
	        
	    }
	@Override
	public void processElement(Monitor moniter, ProcessFunction<Monitor, Monitor>.Context context,
			Collector<Monitor> collector) throws Exception {

		try {
			String vin;

			if (moniter.getVin() == null || moniter.getVin().isEmpty()) {
				vin = moniter.getVid();
			} else {
				vin = moniter.getVin();
			}
			if (moniter.getDocument().getVWarningClass() != null && moniter.getDocument().getVWarningNumber() != null) {
				boolean warniningInDB = DTCWarning.read(moniter.getDocument().getVWarningClass(),
						moniter.getDocument().getVWarningNumber());
				logger.info("warnining detail present in master table  :{} msg UUD :: {}",
						warniningInDB, moniter.getJobName());

				Long lastProcessedTimeStamp = warningDao.readRepairMaintenamce(moniter.getMessageType(), vin,
						moniter.getDocument().getVWarningClass(), moniter.getDocument().getVWarningNumber());
				logger.info("Last processed message time stamp for monitering :{} msg UUD :: {}",
						lastProcessedTimeStamp, moniter.getJobName());

				if (warniningInDB && moniter.getVEvtID() == 46) {
					if (lastProcessedTimeStamp != null) {
						logger.info("warning alreday present in warning table for VEvtId--46 :{} msg UUD :: {}",
								lastProcessedTimeStamp, moniter.getJobName());

					} else {
						WarningStastisticsPojo warningNewRowDetail = WarningStatisticsCalculation(moniter,
								lastProcessedTimeStamp,moniter.getDocument().getVWarningClass(),moniter.getDocument().getVWarningNumber());
						warningDao.warning_insert(warningNewRowDetail);
						logger.info("warning inserted in warning table for VEvtId--46 :{} msg UUD :: {} from alert",
								warningNewRowDetail, moniter.getJobName());
						warningDao.warningUpdateMessageTenCommonTrip(warningNewRowDetail);
						logger.info("Warning records updated in current trip table from alert:: ", warningNewRowDetail);

					}

					collector.collect(moniter);

				} else if (warniningInDB && moniter.getVEvtID() == 44) {

					collector.collect(moniter);
				}
			}

			if (moniter.getVEvtID() == 63) {

				List<Warning> warningList = moniter.getDocument().getWarningObject().getWarningList();

				for (Warning warning : warningList) {
					boolean dbWarning = DTCWarning.read(warning.getWarningClass(), warning.getWarningNumber());
					if (dbWarning) {
						
						Long lastProcessedTimeStamp = warningDao.readRepairMaintenamce(moniter.getMessageType(), vin,
								warning.getWarningClass(), warning.getWarningNumber());
						if (lastProcessedTimeStamp == null) {
							logger.info("warning not present in warning table");

							WarningStastisticsPojo warningNewRowDetail = WarningStatisticsCalculation(moniter,
									lastProcessedTimeStamp,warning.getWarningClass(), warning.getWarningNumber());
							warningDao.warning_insert(warningNewRowDetail);
							logger.info("warning inserted in warning table for VEvtId--63 :{} msg UUD :: {} from alert",
									warningNewRowDetail, moniter.getJobName());
							moniter.getDocument().setVWarningClass(warning.getWarningClass());
							warningDao.warningUpdateMessageTenCommonTrip(warningNewRowDetail);
							logger.info("Warning records updated in current trip table from alert:: ", warningNewRowDetail);

						}
						collector.collect(moniter);
					}

					
				}

			}

		} catch (Exception e) {
			logger.info("Issue while preparing data for Read And Maintenance :{} msg UUD :: {}", moniter,moniter.getJobName());
			e.printStackTrace();
		}
	}

	public WarningStastisticsPojo WarningStatisticsCalculation(Monitor row, Long lastestProcessedMessageTimeStamp, Integer warningClass, Integer warningNumber) {

		WarningStastisticsPojo warningDetail = new WarningStastisticsPojo();
		// ,Long lastestProcessedMessageTimeStamp-----add in parameter

		logger.info("Inside warning calculation type 10");

		warningDetail.setTripId(null);
		warningDetail.setVin(row.getVin());
		warningDetail.setVid(row.getVid());
		try {
			warningDetail.setWarningTimeStamp(TimeFormatter.getInstance()
					.convertUTCToEpochMilli(row.getEvtDateTime().toString(), "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'"));

		} catch (ParseException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		//warningDetail.setWarningClass(row.getDocument().getVWarningClass());
		//warningDetail.setWarningNumber(row.getDocument().getVWarningNumber());

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
		//if ((row.getDocument().getVWarningClass() != null)) {
		if (warningClass != null) {
			if (warningClass >= 4 && warningClass <= 7) {

				warningDetail.setVehicleHealthStatusType("T");

			} else if (warningClass >= 8 && warningClass <= 11) {

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
			warningDetail.setVehicleDrivingStatusType("");
		}

		warningDetail.setDriverID(null);

		// Warning Type
		if (row.getVEvtID() != null) {
			if (row.getVEvtID() == 44 || row.getVEvtID() == 46) {
				warningDetail.setWarningType("A");
			} else if (row.getVEvtID() == 45) {
				warningDetail.setWarningType("D");
			} else {
				warningDetail.setWarningType("");
			}
		} else {
			warningDetail.setWarningType("");
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

		

		return warningDetail;

	}

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

		 ParameterTool envParams= parameterTool;
		DTCWarning = new DTCWarningMasterDao();
		warningDao = new WarningStatisticsDao();
		try {

			masterConnection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					
					  envParams.get(AlertConfigProp.MASTER_POSTGRE_SERVER_NAME),
					  Integer.parseInt(envParams.get(AlertConfigProp.MASTER_POSTGRE_PORT)),
					  envParams.get(AlertConfigProp.MASTER_POSTGRE_DATABASE_NAME),
					  envParams.get(AlertConfigProp.MASTER_POSTGRE_USER),
					  envParams.get(AlertConfigProp.MASTER_POSTGRE_PASSWORD));

			connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(	envParams.get(AlertConfigProp.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(AlertConfigProp.DATAMART_POSTGRE_PORT)),
					envParams.get(AlertConfigProp.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(AlertConfigProp.DATAMART_POSTGRE_USER),
					envParams.get(AlertConfigProp.DATAMART_POSTGRE_PASSWORD));
			warningDao.setConnection(connection);
			DTCWarning.setConnection(masterConnection);

		} catch (Exception e) {

			logger.error("Error in Repair & Maintenance connection msg UUD :: {}" , e.getMessage());

		}

	}

	@Override
	public void close() throws Exception {

		super.close();

		logger.info("In close() of Repair and Maintenance :: ");

		if (connection != null) {
			logger.info("Releasing connection from Repair & Maintenance job msg UUD :: {}");
			connection.close();
		}
		if (masterConnection != null) {
			logger.info("Releasing connection from  Repair & Maintenance job msg UUD :: {}");
			masterConnection.close();
		}

	}

}
