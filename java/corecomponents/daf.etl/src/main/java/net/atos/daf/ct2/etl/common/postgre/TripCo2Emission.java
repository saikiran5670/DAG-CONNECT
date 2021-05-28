package net.atos.daf.ct2.etl.common.postgre;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.SQLException;

import org.apache.flink.api.common.functions.RichFlatMapFunction;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.util.Collector;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.etl.common.bo.TripStatusData;
import net.atos.daf.ct2.etl.common.util.ETLConstants;
import net.atos.daf.ct2.etl.common.util.ETLQueries;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;
import net.atos.daf.postgre.dao.Co2MasterDao;

public class TripCo2Emission extends RichFlatMapFunction<TripStatusData, TripStatusData> {
	private static final Logger logger = LoggerFactory.getLogger(TripCo2Emission.class);

	private static final long serialVersionUID = 1L;
	private Connection masterConnection;
	private Co2MasterDao cmDao;
	PreparedStatement co2CoEfficientQry;

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		super.open(parameters);
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		try {
			masterConnection = PostgreDataSourceConnection.getInstance().getDataSourceConnection(
					envParams.get(ETLConstants.MASTER_POSTGRE_SERVER_NAME),
					Integer.parseInt(envParams.get(ETLConstants.MASTER_POSTGRE_PORT)),
					envParams.get(ETLConstants.MASTER_POSTGRE_DATABASE_NAME),
					envParams.get(ETLConstants.MASTER_POSTGRE_USER),
					envParams.get(ETLConstants.MASTER_POSTGRE_PASSWORD));
			cmDao = new Co2MasterDao();
			cmDao.setConnection(masterConnection);
			co2CoEfficientQry = masterConnection.prepareStatement(ETLQueries.CO2_COEFFICIENT_QRY);
			
		}catch (Exception e) {
			// TODO: handle exception both logger and throw is not required
			logger.error("Issue while establishing Postgre connection in Trip streaming Job :: " + e);
			throw e;
		}

	}

	@Override
	public void flatMap(TripStatusData stsData,
			Collector<TripStatusData> out){

		try {
			double co2CoEfficient = cmDao.read(co2CoEfficientQry, stsData.getVin());
			double co2Emission = 0;
			if (co2CoEfficient != 0 && stsData.getVUsedFuel() != null)
				co2Emission = (stsData.getVUsedFuel() * co2CoEfficient) / 1000;

			logger.info("tripId : "+stsData.getTripId() +" vin : " + stsData.getVin() + " co2CoEfficient :" + co2CoEfficient + " co2Emission :"+co2Emission);
			stsData.setCo2Emission(co2Emission);
			out.collect(stsData);
		} catch (Exception e) {
			// TODO error suppressed , cross verify scenarios
			logger.error("Issue while processing Co2CoEfficient trip statisctics job :: " + e);
		}
	}

	@Override
	public void close() throws Exception{
		try {
			
			if (masterConnection != null) {
				masterConnection.close();
			}
		} catch (SQLException e) {
			// TODO Need to check if logging and throw is required
			logger.error("Issue while Closing Postgre table connection :: ", e);
			throw e;
		}
	}

}