package net.atos.daf.ct2.common.realtime.postgresql;

import java.io.Serializable;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.connector.jdbc.JdbcConnectionOptions;
import org.apache.flink.connector.jdbc.JdbcExecutionOptions;
import org.apache.flink.connector.jdbc.JdbcSink;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.utc.TimeFormatter;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.postgre.bo.IndexTripData;
import net.atos.daf.postgre.connection.PostgreConnection;

public class TripIndexJdbcSink implements Serializable{

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LoggerFactory.getLogger(TripIndexJdbcSink.class);
	
	public void saveTripIndexData(SingleOutputStreamOperator<IndexTripData> indexTripData, ParameterTool envParams){
		
		String jdbcUrl = new StringBuilder(envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME))
				.append(":" + Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)) + "/")
                .append(envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME))
                .append("?user=" + envParams.get(DafConstants.DATAMART_POSTGRE_USER))
                .append("&password=" + PostgreConnection.getInstance().encodeValue(envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD)))
                .append(DafConstants.POSTGRE_SQL_SSL_MODE)
                .toString();
		logger.info("jdbcUrl ::{}",jdbcUrl);
		
		indexTripData.addSink(JdbcSink.sink(
				DafConstants.TRIP_INDEX_INSERT_STATEMENT,
                (statement, rec) -> {
                	statement.setString(1, rec.getTripId());
                	statement.setString(2, rec.getVin());
            		statement.setInt(3, rec.getVTachographSpeed());
            		statement.setLong(4, rec.getVGrossWeightCombination());
            		statement.setString(5, rec.getDriver2Id());
            		statement.setString(6, rec.getDriverId());
            		statement.setString(7, rec.getJobName());
            		statement.setLong(8, rec.getIncrement());
            		statement.setLong(9, rec.getVDist());
            		statement.setLong(10, rec.getEvtDateTime());
            		statement.setInt(11, rec.getVEvtId());
            		statement.setLong(12, TimeFormatter.getInstance().getCurrentUTCTime());
                    
                },
                JdbcExecutionOptions.builder()
		                .withBatchSize(Integer.parseInt(envParams.get(DafConstants.JDBC_EXEC_OPTION_BATCH_SIZE)))
		                .withBatchIntervalMs(Integer.parseInt(envParams.get(DafConstants.JDBC_EXEC_OPTION_BATCH_INTERVAL_MILLISEC)))
		                .withMaxRetries(Integer.parseInt(envParams.get(DafConstants.JDBC_EXEC_OPTION_BATCH_MAX_RETRIES)))
                        .build(),
                new JdbcConnectionOptions.JdbcConnectionOptionsBuilder()
                        .withUrl(jdbcUrl)
                        .withDriverName(envParams.get(DafConstants.POSTGRE_SQL_DRIVER))
                        .build()));
		
		/*JdbcSink.sink(
                "insert into books (id, title, authors, year) values (?, ?, ?, ?)",
                (statement, book) -> {
                    statement.setLong(1, book.id);
                    statement.setString(2, book.title);
                    statement.setString(3, book.authors);
                    statement.setInt(4, book.year);
                },
                JdbcExecutionOptions.builder()
                        .withBatchSize(1000)
                        .withBatchIntervalMs(200)
                        .withMaxRetries(5)
                        .build(),
                new JdbcConnectionOptions.JdbcConnectionOptionsBuilder()
                        .withUrl("jdbc:postgresql://dbhost:5432/postgresdb")
                        .withDriverName("org.postgresql.Driver")
                        .withUsername("someUser")
                        .withPassword("somePassword")
                        .build()
        ));*/
	
	}

}
