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
import net.atos.daf.postgre.bo.EcoScore;
import net.atos.daf.postgre.connection.PostgreConnection;
//import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.EcoScoreDao;

public class EcoScoreSink extends RichSinkFunction<EcoScore> implements Serializable {

	/**
	* 
	*/
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LoggerFactory.getLogger(EcoScoreSink.class);

	//private PreparedStatement statement;
	private Connection connection;
	private List<EcoScore> queue;
	private List<EcoScore> synchronizedCopy;
	EcoScoreDao ecoScoreDao;
	private PreparedStatement ecoScoreQryStmt;

	@Override
	public void invoke(EcoScore rec) throws Exception {

		try {
			queue.add(rec);

			if (queue.size() >= 1) {
				logger.info("inside syncronized");
				synchronized (synchronizedCopy) {
					synchronizedCopy = new ArrayList<EcoScore>(queue);
					queue.clear();
					for (EcoScore tripData : synchronizedCopy) {
						ecoScoreDao.insert(tripData, ecoScoreQryStmt);
						logger.info("EcoScore records inserted to ecoscore table ::{} ",tripData.getTripId());
					}
				}
			}
		} catch (Exception e) {
			logger.error("Issue while calling invoke() in EcoScoreSink :: {}", e.getMessage());
			e.printStackTrace();
		}

	}

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();
		ecoScoreDao = new EcoScoreDao();
		queue = new ArrayList<EcoScore>();
		synchronizedCopy = new ArrayList<EcoScore>();
		
		try {
			/*connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(ETLConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(ETLConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(ETLConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(ETLConstants.DATAMART_POSTGRE_USER),
					envParams.get(ETLConstants.DATAMART_POSTGRE_PASSWORD));*/
						
			connection = PostgreConnection.getInstance().getConnection(
					envParams.get(ETLConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(ETLConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(ETLConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(ETLConstants.DATAMART_POSTGRE_USER),
					envParams.get(ETLConstants.DATAMART_POSTGRE_PASSWORD),envParams.get(ETLConstants.POSTGRE_SQL_DRIVER));
			logger.info("In EcoScore sink connection done:{}", connection);
			ecoScoreDao.setConnection(connection);
			ecoScoreQryStmt = connection.prepareStatement(ETLQueries.ECOSCORE_INSERT_STATEMENT);
			logger.info("In EcoScore sink prepared statement done:{}", ecoScoreQryStmt);
		}catch (Exception e) {
			// TODO: handle exception both logger and throw is not required
			logger.error("Issue while establishing Postgre connection in Trip streaming Job EcoScore Sink ::{} ", e);
			logger.error("serverNm ::{}, port ::{} ",envParams.get(ETLConstants.DATAMART_POSTGRE_SERVER_NAME), Integer.parseInt(envParams.get(ETLConstants.DATAMART_POSTGRE_PORT)));
			logger.error("databaseNm ::{}, user ::{}, pwd ::{} ",envParams.get(ETLConstants.DATAMART_POSTGRE_DATABASE_NAME), envParams.get(ETLConstants.DATAMART_POSTGRE_USER), envParams.get(ETLConstants.DATAMART_POSTGRE_PASSWORD));
			logger.error("connection :: {}", connection);
			throw e;
		}
	}

	// @SuppressWarnings("unchecked")
	@Override
	public void close() throws Exception {
		super.close();
		if (ecoScoreQryStmt != null) {
			ecoScoreQryStmt.close();
		}
		logger.debug("In close() of EcoScoreSink :: ");

		if (connection != null) {
			logger.info("Releasing connection from EcoScoreSink ETL Job");
			connection.close();
		}
	}
	
}