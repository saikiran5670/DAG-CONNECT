package net.atos.daf.ct2.etl.common.postgre;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.util.ArrayList;
import java.util.List;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.etl.common.util.ETLConstants;
import net.atos.daf.ct2.etl.common.util.ETLQueries;
import net.atos.daf.postgre.bo.Trip;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.TripSinkDao;

public class TripSink extends RichSinkFunction<Trip> implements Serializable {

	/**
	* 
	*/
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LoggerFactory.getLogger(TripSink.class);

	private PreparedStatement statement;
	private Connection connection;
	private List<Trip> queue;
	private List<Trip> synchronizedCopy;
	TripSinkDao tripDao;
	private PreparedStatement tripStatisticQry;

	@Override
	public void invoke(Trip rec) throws Exception {

		try {
			queue.add(rec);

			if (queue.size() >= 1) {
				logger.info("inside syncronized");
				synchronized (synchronizedCopy) {
					synchronizedCopy = new ArrayList<Trip>(queue);
					queue.clear();
					for (Trip tripData : synchronizedCopy) {
						logger.info(
								"tripId :: " + tripData.getTripId() + " co2Emi ::" + tripData.getTripCalC02Emission());
						tripDao.insert(tripData, tripStatisticQry);
						logger.info("Trip records inserted to trip table :: "+tripData.getTripId());
					}
				}
			}
		} catch (Exception e) {
			logger.error("Issue while calling invoke() in TripSink :: " + e);
			e.printStackTrace();
		}

	}

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();
		tripDao = new TripSinkDao();
		queue = new ArrayList<Trip>();
		synchronizedCopy = new ArrayList<Trip>();
		
		try {
			connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(ETLConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(ETLConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(ETLConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(ETLConstants.DATAMART_POSTGRE_USER),
					envParams.get(ETLConstants.DATAMART_POSTGRE_PASSWORD));
			logger.info("In trip sink connection done" + connection);
			tripDao.setConnection(connection);
			tripStatisticQry = connection.prepareStatement(ETLQueries.TRIP_INSERT_STATEMENT);
		}catch (Exception e) {
			// TODO: handle exception both logger and throw is not required
			logger.error("Issue while establishing Postgre connection in Trip streaming Job :: " + e);
			logger.error("serverNm :: "+envParams.get(ETLConstants.DATAMART_POSTGRE_SERVER_NAME) +" port :: "+Integer.parseInt(envParams.get(ETLConstants.DATAMART_POSTGRE_PORT)));
			logger.error("databaseNm :: "+envParams.get(ETLConstants.DATAMART_POSTGRE_DATABASE_NAME) +" user :: "+envParams.get(ETLConstants.DATAMART_POSTGRE_USER) + " pwd :: "+envParams.get(ETLConstants.DATAMART_POSTGRE_PASSWORD));
			logger.error("connection :: " + connection);
			throw e;
		}
	}

	// @SuppressWarnings("unchecked")
	@Override
	public void close() throws Exception {
		super.close();
		if (statement != null) {
			statement.close();
		}
		logger.info("In close() of tripSink :: ");

		if (connection != null) {
			logger.info("Releasing connection from Trip Job");
			connection.close();
		}
	}

}