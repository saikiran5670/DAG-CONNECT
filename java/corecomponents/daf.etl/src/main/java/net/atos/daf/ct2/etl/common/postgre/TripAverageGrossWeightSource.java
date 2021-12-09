package net.atos.daf.ct2.etl.common.postgre;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.Objects;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.source.RichSourceFunction;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.etl.common.bo.TripGrossWeight;
import net.atos.daf.ct2.etl.common.util.ETLConstants;
import net.atos.daf.ct2.etl.common.util.ETLQueries;
import net.atos.daf.postgre.connection.PostgreConnection;

public class TripAverageGrossWeightSource extends RichSourceFunction<TripGrossWeight> implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LoggerFactory.getLogger(TripAverageGrossWeightSource.class);

	private Connection connection=null;
	PreparedStatement averageGrossStmt=null;
	ResultSet rs= null;
	private Long startDt=0L;
	private Long endDt=0L;
		
	public TripAverageGrossWeightSource(Long startDt, Long endDt){
		this.startDt = startDt;
		this.endDt = endDt;
	}
	
	@Override
	public void run(org.apache.flink.streaming.api.functions.source.SourceFunction.SourceContext<TripGrossWeight> ctx)
			throws Exception {
		
		try {

			if(Objects.nonNull(rs)){
				while(rs.next()) {
					
					TripGrossWeight tripGrossWt = new TripGrossWeight();
					tripGrossWt.setTripId(rs.getString("trip_id"));
					tripGrossWt.setTachographSpeed(rs.getLong("max_speed"));
					tripGrossWt.setVGrossWtSum(Double.valueOf(rs.getLong("gross_weight_sum")));
					tripGrossWt.setVGrossWtCmbCount(rs.getLong("gross_weight_dist"));
					logger.info("Received info for gross weight calculation :: {}",tripGrossWt);
					ctx.collect(tripGrossWt);
				}
			}
			
		} catch (Exception e) {
			logger.error("Issue in TripAverageGrossWeightSource while calling run() ::{} ", e.getMessage());
			e.printStackTrace();
		}
	}
	
	 @Override
     public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		 ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();
			
		 try {
			super.open(parameters);
			connection = PostgreConnection.getInstance().getConnection(
					envParams.get(ETLConstants.DATAMART_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(ETLConstants.DATAMART_POSTGRE_PORT)),
					envParams.get(ETLConstants.DATAMART_POSTGRE_DATABASE_NAME),
					envParams.get(ETLConstants.DATAMART_POSTGRE_USER),
					envParams.get(ETLConstants.DATAMART_POSTGRE_PASSWORD),envParams.get(ETLConstants.POSTGRE_SQL_DRIVER));
			
				logger.info("In TripAverageGrossWeightSource connection done:{}", connection);
				
				averageGrossStmt = connection.prepareStatement(ETLQueries.TRIP_AVG_GROSS_WEIGHT_QRY);
				averageGrossStmt.setLong(1, startDt);
				averageGrossStmt.setLong(2, endDt);
				rs = averageGrossStmt.executeQuery();
								
		} catch (Exception e) {
			// TODO: handle exception both logger and throw is not required
			logger.error("Issue while establishing Postgre connection in TripAverageGrossWeightSource ::{} ", e);
			logger.error("serverNm ::{},  port ::{} ",envParams.get(ETLConstants.MASTER_POSTGRE_SERVER_NAME), Integer.parseInt(envParams.get(ETLConstants.MASTER_POSTGRE_PORT)));
			logger.error("databaseNm ::{},  user ::{}, pwd ::{} ",envParams.get(ETLConstants.MASTER_POSTGRE_DATABASE_NAME), envParams.get(ETLConstants.MASTER_POSTGRE_USER), envParams.get(ETLConstants.MASTER_POSTGRE_PASSWORD));
			logger.error("connection ::{} ", connection);
			throw e;
		}
     }

	@Override
	public void cancel() {
		try {
			super.close();
			logger.info("Releasing connection in TripAverageGrossWeightSource ::{}, statement::{} ", connection, averageGrossStmt);

			if (Objects.nonNull(averageGrossStmt)) {
				averageGrossStmt.close();
			}
			
			if (Objects.nonNull(connection)) {
				connection.close();
			}
		} catch (SQLException e) {
			logger.error("Sql Issue while calling close in TripAverageGrossWeightSource ::{} ", e.getMessage());
			e.printStackTrace();
		} catch (Exception e) {
			logger.error("Issue while calling close in TripAverageGrossWeightSource ::{} ", e.getMessage());
			e.printStackTrace();
		}
	}

}
