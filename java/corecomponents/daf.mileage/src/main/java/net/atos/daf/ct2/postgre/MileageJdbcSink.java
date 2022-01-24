package net.atos.daf.ct2.postgre;

import java.io.Serializable;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.connector.jdbc.JdbcConnectionOptions;
import org.apache.flink.connector.jdbc.JdbcExecutionOptions;
import org.apache.flink.connector.jdbc.JdbcSink;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.ct2.bo.TripMileage;
import net.atos.daf.ct2.util.MileageConstants;
import net.atos.daf.postgre.connection.PostgreConnection;
import net.atos.daf.postgre.util.DafConstants;

public class MileageJdbcSink  implements Serializable{

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LogManager.getLogger(MileageJdbcSink.class);
	
	public void saveMileageData(SingleOutputStreamOperator<TripMileage> mileageData, ParameterTool envParams)throws Exception{
		
		String jdbcUrl = new StringBuilder(envParams.get(MileageConstants.DATAMART_POSTGRE_SERVER_NAME))
				.append(":" + Integer.parseInt(envParams.get(MileageConstants.DATAMART_POSTGRE_PORT)) + "/")
                .append(envParams.get(MileageConstants.DATAMART_POSTGRE_DATABASE_NAME))
                .append("?user=" + envParams.get(MileageConstants.DATAMART_POSTGRE_USER))
                .append("&password=" + PostgreConnection.getInstance().encodeValue(envParams.get(MileageConstants.DATAMART_POSTGRE_PASSWORD)))
                .append(MileageConstants.POSTGRE_SQL_SSL_MODE)
                .toString();
		logger.debug("Mileage jdbcUrl ::{}",jdbcUrl);
		
		mileageData.addSink(JdbcSink.sink(
				MileageConstants.MILEAGE_QRY,
                (statement, rec) -> {
                	statement.setString(1, rec.getVin());
        			statement.setLong(2, rec.getEvtDateTime());
        			statement.setLong(3, rec.getOdoMileage());
        			statement.setDouble(4, rec.getOdoDistance());
        			statement.setDouble(5, rec.getRealDistance());
        			statement.setDouble(6, rec.getGpsDistance());
        			statement.setLong(7, rec.getModifiedAt());
        			
        			statement.setLong(8, rec.getEvtDateTime());
        			statement.setLong(9, rec.getOdoMileage());
        			statement.setDouble(10, rec.getOdoDistance());
        			statement.setDouble(11, rec.getRealDistance());
        			statement.setDouble(12, rec.getGpsDistance());
        			statement.setLong(13, rec.getModifiedAt());
        			logger.info("mileage data calculated for veh :::{} ",rec);
                },
                JdbcExecutionOptions.builder()
		                .withBatchSize(Integer.parseInt(envParams.get(DafConstants.JDBC_EXEC_OPTION_BATCH_SIZE)))
		                .withBatchIntervalMs(Integer.parseInt(envParams.get(DafConstants.JDBC_EXEC_OPTION_BATCH_INTERVAL_MILLISEC)))
		                .withMaxRetries(Integer.parseInt(envParams.get(DafConstants.JDBC_EXEC_OPTION_BATCH_MAX_RETRIES)))
                        .build(),
                new JdbcConnectionOptions.JdbcConnectionOptionsBuilder()
                        .withUrl(jdbcUrl)
                        .withDriverName(envParams.get(MileageConstants.POSTGRE_SQL_DRIVER))
                        .build()));
	
	
	}

}
