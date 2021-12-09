package net.atos.daf.ct2.etl.common.postgre;

import java.io.Serializable;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.connector.jdbc.JdbcConnectionOptions;
import org.apache.flink.connector.jdbc.JdbcExecutionOptions;
import org.apache.flink.connector.jdbc.JdbcSink;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import net.atos.daf.ct2.etl.common.bo.TripGrossWeight;
import net.atos.daf.ct2.etl.common.util.ETLConstants;
import net.atos.daf.ct2.etl.common.util.ETLQueries;
import net.atos.daf.postgre.connection.PostgreConnection;
import net.atos.daf.postgre.util.DafConstants;

public class GrossWeightEcoScoreJdbcSink  implements Serializable{

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LogManager.getLogger(GrossWeightEcoScoreJdbcSink.class);
	
	public void updateEcoscoreData(SingleOutputStreamOperator<TripGrossWeight> grossWt, ParameterTool envParams)throws Exception{
		
		String jdbcUrl = new StringBuilder(envParams.get(ETLConstants.DATAMART_POSTGRE_SERVER_NAME))
				.append(":" + Integer.parseInt(envParams.get(ETLConstants.DATAMART_POSTGRE_PORT)) + "/")
                .append(envParams.get(ETLConstants.DATAMART_POSTGRE_DATABASE_NAME))
                .append("?user=" + envParams.get(ETLConstants.DATAMART_POSTGRE_USER))
                .append("&password=" + PostgreConnection.getInstance().encodeValue(envParams.get(ETLConstants.DATAMART_POSTGRE_PASSWORD)))
                .append(ETLConstants.POSTGRE_SQL_SSL_MODE)
                .toString();
		logger.debug("GrossWeightEcoScoreJdbcSink jdbcUrl ::{}",jdbcUrl);
		
		//gross_weight_combination_total ?, tacho_gross_weight_combination =?, gross_weight_combination_count =? where trip_id = ?
		grossWt.addSink(JdbcSink.sink(
				ETLQueries.ECOSCORE_AVG_GROSS_WEIGHT_UPDATE_QRY,
                (statement, rec) -> {
                	statement.setDouble(1, rec.getTripCalAvgGrossWtComb());
                	statement.setDouble(2, rec.getVGrossWtSum());
                	statement.setLong(3, rec.getVGrossWtCmbCount());
                	statement.setString(4, rec.getTripId());
                	
        			logger.debug("GrossWeightEcoScoreJdbcSink data for veh :::{} ",statement);
                },
                JdbcExecutionOptions.builder()
		                .withBatchSize(Integer.parseInt(envParams.get(DafConstants.JDBC_EXEC_OPTION_BATCH_SIZE)))
		                .withBatchIntervalMs(Integer.parseInt(envParams.get(DafConstants.JDBC_EXEC_OPTION_BATCH_INTERVAL_MILLISEC)))
		                .withMaxRetries(Integer.parseInt(envParams.get(DafConstants.JDBC_EXEC_OPTION_BATCH_MAX_RETRIES)))
                        .build(),
                new JdbcConnectionOptions.JdbcConnectionOptionsBuilder()
                        .withUrl(jdbcUrl)
                        .withDriverName(envParams.get(ETLConstants.POSTGRE_SQL_DRIVER))
                        .build())).name("Gross weight ecoscore sink");
	
	
	}

}
