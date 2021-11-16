package net.atos.daf.ct2.common.realtime.postgresql;

import java.io.Serializable;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.util.ArrayList;
import java.util.List;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.common.realtime.dataprocess.IndexDataProcess;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;
import net.atos.daf.postgre.bo.Trip;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.LiveFleetDriverActivityDao;

@SuppressWarnings({})
public class LiveFleetDriverActivityPostgreSink extends RichSinkFunction<KafkaRecord<Index>> implements Serializable {
	/*
	 * This class is used to write Index message data in a postgres table.
	 */

	private static Logger log = LoggerFactory.getLogger(IndexDataProcess.class);

	private static final long serialVersionUID = 1L;

	private List<Index> queue;
	private List<Index> synchronizedCopy;

	Connection connection = null;
	String livefleetdriver = null;
	String driverreadquery = null;

	LiveFleetDriverActivityDao driverDAO;

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

		// this function is used to set up a connection to postgres table

		log.info("########## In LiveFleet Driver Activity ##############");

		driverDAO = new LiveFleetDriverActivityDao();

		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		livefleetdriver = envParams.get(DafConstants.QUERY_DRIVER_ACTIVITY);
		driverreadquery = envParams.get(DafConstants.QUERY_DRIVER_ACTIVITY_READ);

		log.debug("livefleetdriver --> " + livefleetdriver);
		log.debug("driverreadquery --> " + driverreadquery);

		try {

			connection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(DafConstants.DATAMART_POSTGRE_USER),
					envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));
			driverDAO.setConnection(connection);

		} catch (Exception e) {
			log.error("Error in Live fleet driver" + e.getMessage());
			log.error("Exception {}",e);

		}

	}

	public void invoke(KafkaRecord<Index> index) throws Exception {
		// this function is used to write data into postgres table

		// Live Fleet Driver Activity
		Index row = index.getValue();
		queue = new ArrayList<Index>();
		synchronizedCopy = new ArrayList<Index>();

		try {
			queue.add(row);
			if (queue.size() >= 1) {
				log.debug("inside syncronized");
				synchronized (synchronizedCopy) {
					synchronizedCopy = new ArrayList<Index>(queue);
					queue.clear();
					Long trip_Start_time = driverDAO.read(index.getValue().getDocument().getTripID());
					driverDAO.insert(row, trip_Start_time);
					log.info("Driver activity save done");

				}
			}
		} catch (Exception e) {
			log.error("Exception {}",e.getMessage());
			log.error("Exception {}",e);
		}

	}

	/*
	 * private String createValidUrlToConnectPostgreSql(String serverNm, int port,
	 * String databaseNm, String userNm, String password) throws Exception {
	 * 
	 * String encodedPassword = encodeValue(password); String url = serverNm + ":" +
	 * port + "/" + databaseNm + "?" + "user=" + userNm + "&" + "password=" +
	 * encodedPassword + DafConstants.POSTGRE_SQL_SSL_MODE;
	 * 
	 * log.info("Valid Url = " + url);
	 * 
	 * return url; }
	 * 
	 * private static String encodeValue(String value) { try { return
	 * URLEncoder.encode(value, StandardCharsets.UTF_8.toString()); } catch
	 * (UnsupportedEncodingException ex) { throw new
	 * RuntimeException(ex.getCause()); } }
	 */

	@Override
	public void close() throws Exception {

		connection.close();
		log.debug("In Close");

	}

}
