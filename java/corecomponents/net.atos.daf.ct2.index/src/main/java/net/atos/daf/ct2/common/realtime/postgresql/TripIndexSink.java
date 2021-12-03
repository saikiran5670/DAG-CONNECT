package net.atos.daf.ct2.common.realtime.postgresql;

import java.io.Serializable;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.postgre.bo.IndexTripData;
import net.atos.daf.postgre.dao.LiveFleetTripIndexDao;

public class TripIndexSink extends RichSinkFunction<IndexTripData> implements Serializable {

	/**
	* 
	*/
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LoggerFactory.getLogger(TripIndexSink.class);

	private PreparedStatement statement;
	private Connection connection;
	LiveFleetTripIndexDao tripIndexDao;
	private PreparedStatement tripIndexQry;

	//private List<IndexTripData> queue;
	//private List<IndexTripData> synchronizedCopy;

	@Override
	public void invoke(IndexTripData rec) throws Exception {
		tripIndexDao.insert(rec, tripIndexQry);
		/*try {
			queue.add(rec);

			if (queue.size() >= 1) {
				synchronized (synchronizedCopy) {
					synchronizedCopy = new ArrayList<IndexTripData>(queue);
					queue.clear();
					for (IndexTripData tripData : synchronizedCopy) {
						tripIndexDao.insert(tripData, tripIndexQry);
					}
				}
			}
		} catch (Exception e) {
			logger.error("Issue while calling invoke() in TripIndexSink :: " + e);
			e.printStackTrace();
		}*/

	}

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();
		tripIndexDao = new LiveFleetTripIndexDao();
		//queue = new ArrayList<IndexTripData>();
		//synchronizedCopy = new ArrayList<IndexTripData>();
		
		try {
			/*connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(DafConstants.DATAMART_POSTGRE_USER),
					envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));*/
			
			Class.forName(envParams.get(DafConstants.POSTGRE_SQL_DRIVER));
			String dbUrl = createValidUrlToConnectPostgreSql(
					envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(DafConstants.DATAMART_POSTGRE_USER),
					envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));
			connection = DriverManager.getConnection(dbUrl);
						
			logger.info("In TripIndexSink connection done" + connection);
			tripIndexDao.setConnection(connection);
			tripIndexQry = connection.prepareStatement(DafConstants.TRIP_INDEX_INSERT_STATEMENT);
		}catch (Exception e) {
			// TODO: handle exception both logger and throw is not required
			logger.error("Issue while establishing Postgre connection in Trip Index streaming Job :: " + e);
			logger.error("serverNm :: "+envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME) +" port :: "+Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)));
			logger.error("databaseNm :: "+envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME) +" user :: "+envParams.get(DafConstants.DATAMART_POSTGRE_USER) + " pwd :: "+envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));
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
		logger.debug("In close() of TripIndexSink :: ");

		if (connection != null) {
			logger.info("Releasing connection from TripIndexSink Job");
			connection.close();
		}
	}
	
	private String createValidUrlToConnectPostgreSql(String serverNm, int port, String databaseNm, String userNm,
			String password) throws Exception {

		String encodedPassword = encodeValue(password);
		String url = serverNm + ":" + port + "/" + databaseNm + "?" + "user=" + userNm + "&" + "password="
				+ encodedPassword + DafConstants.POSTGRE_SQL_SSL_MODE;

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