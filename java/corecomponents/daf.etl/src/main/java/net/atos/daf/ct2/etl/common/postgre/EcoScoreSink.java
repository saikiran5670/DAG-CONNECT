package net.atos.daf.ct2.etl.common.postgre;

import java.io.Serializable;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.sql.Connection;
import java.sql.DriverManager;
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
						logger.info("EcoScore records inserted to ecoscore table :: "+tripData.getTripId());
					}
				}
			}
		} catch (Exception e) {
			logger.error("Issue while calling invoke() in EcoScoreSink :: " + e);
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
			
			Class.forName(envParams.get(ETLConstants.POSTGRE_SQL_DRIVER));
			String dbUrl = createValidUrlToConnectPostgreSql(envParams.get(ETLConstants.DATAMART_POSTGRE_SERVER_NAME),
						Integer.parseInt(envParams.get(ETLConstants.DATAMART_POSTGRE_PORT)),
						envParams.get(ETLConstants.DATAMART_POSTGRE_DATABASE_NAME),
						envParams.get(ETLConstants.DATAMART_POSTGRE_USER),
						envParams.get(ETLConstants.DATAMART_POSTGRE_PASSWORD));
			connection = DriverManager.getConnection(dbUrl);
			
			logger.info("In EcoScore sink connection done" + connection);
			ecoScoreDao.setConnection(connection);
			ecoScoreQryStmt = connection.prepareStatement(ETLQueries.ECOSCORE_INSERT_STATEMENT);
		}catch (Exception e) {
			// TODO: handle exception both logger and throw is not required
			logger.error("Issue while establishing Postgre connection in Trip streaming Job EcoScore Sink :: " + e);
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
		if (ecoScoreQryStmt != null) {
			ecoScoreQryStmt.close();
		}
		logger.info("In close() of EcoScoreSink :: ");

		if (connection != null) {
			logger.info("Releasing connection from EcoScoreSink ETL Job");
			connection.close();
		}
	}

	private String createValidUrlToConnectPostgreSql(String serverNm, int port, String databaseNm, String userNm,
			String password) throws Exception {

		String encodedPassword = encodeValue(password);
		String url = serverNm + ":" + port + "/" + databaseNm + "?" + "user=" + userNm + "&" + "password="
				+ encodedPassword + ETLConstants.POSTGRE_SQL_SSL_MODE;
	
		return url;
	}
	
	private String encodeValue(String value) {
		try {
			return URLEncoder.encode(value, StandardCharsets.UTF_8.toString());
		} catch (UnsupportedEncodingException ex) {
			throw new RuntimeException(ex.getCause());
		}
	}
}