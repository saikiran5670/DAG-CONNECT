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

import net.atos.daf.ct2.etl.common.bo.FuelCoEfficient;
import net.atos.daf.ct2.etl.common.util.ETLConstants;
import net.atos.daf.ct2.etl.common.util.ETLQueries;
import net.atos.daf.postgre.connection.PostgreConnection;

public class FuelCoefficientSource extends RichSourceFunction<FuelCoEfficient> implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LoggerFactory.getLogger(FuelCoefficientSource.class);

	private Connection masterConnection=null;
	PreparedStatement fuelCoEfficientStmt=null;
	ResultSet rs= null;
	private volatile boolean isCancelled=false;

	@Override
	public void run(org.apache.flink.streaming.api.functions.source.SourceFunction.SourceContext<FuelCoEfficient> ctx)
			throws Exception {
		
		try {
			while(!isCancelled)
			{
				if (Objects.nonNull(rs) && rs.next()) {
					FuelCoEfficient fuelCoefficient = new FuelCoEfficient();
					fuelCoefficient.setFuelCoefficient(rs.getDouble("coefficient"));
					fuelCoefficient.setFuelType(rs.getString("fuel_type"));
					logger.info("fuelCoefficient rec :: {}", fuelCoefficient);
					ctx.collect(fuelCoefficient);
				}
			}
		} catch (Exception e) {
			logger.error("Issue in FuelCoefficientSource while calling run() ::{} ", e);
			e.printStackTrace();
		}
	}
	
	 @Override
     public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		 ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();
			
		 try {
			super.open(parameters);
			 masterConnection = PostgreConnection.getInstance().getConnection(
						envParams.get(ETLConstants.MASTER_POSTGRE_SERVER_NAME),
						Integer.parseInt(envParams.get(ETLConstants.MASTER_POSTGRE_PORT)),
						envParams.get(ETLConstants.MASTER_POSTGRE_DATABASE_NAME),
						envParams.get(ETLConstants.MASTER_POSTGRE_USER),
						envParams.get(ETLConstants.MASTER_POSTGRE_PASSWORD),envParams.get(ETLConstants.POSTGRE_SQL_DRIVER));
				
				logger.info("In FuelCoefficientSource connection done:{}", masterConnection);
				
				fuelCoEfficientStmt = masterConnection.prepareStatement(ETLQueries.FUEL_COEFFICIENT_QRY);
				rs = fuelCoEfficientStmt.executeQuery();
								
		} catch (Exception e) {
			// TODO: handle exception both logger and throw is not required
			logger.error("Issue while establishing Postgre connection in FuelCoefficientSource ::{} ", e);
			logger.error("serverNm ::{},  port ::{} ",envParams.get(ETLConstants.MASTER_POSTGRE_SERVER_NAME), Integer.parseInt(envParams.get(ETLConstants.MASTER_POSTGRE_PORT)));
			logger.error("databaseNm ::{},  user ::{}, pwd ::{} ",envParams.get(ETLConstants.MASTER_POSTGRE_DATABASE_NAME), envParams.get(ETLConstants.MASTER_POSTGRE_USER), envParams.get(ETLConstants.MASTER_POSTGRE_PASSWORD));
			logger.error("masterConnection ::{} ", masterConnection);
			throw e;
		}
     }

	@Override
	public void cancel() {
		try {
			super.close();

			isCancelled = true;
			if (Objects.nonNull(fuelCoEfficientStmt)) {
				fuelCoEfficientStmt.close();
			}
			logger.info("In cancel() of FuelCoefficientSource :: ");

			if (Objects.nonNull(masterConnection)) {
				logger.info("Releasing connection from FuelCoefficientSource Job");
				masterConnection.close();
			}
		} catch (SQLException e) {
			logger.error("Sql Issue while calling close in FuelCoefficientSource ::{} ", e);
			e.printStackTrace();
		} catch (Exception e) {
			logger.error("Issue while calling close in FuelCoefficientSource ::{} ", e);
			e.printStackTrace();
		}
	}

}
