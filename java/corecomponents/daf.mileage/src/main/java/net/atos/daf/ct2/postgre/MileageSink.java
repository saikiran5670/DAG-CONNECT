package net.atos.daf.ct2.postgre;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;

import net.atos.daf.ct2.bo.TripMileage;
import net.atos.daf.ct2.util.MileageConstants;
import net.atos.daf.postgre.connection.PostgreDataSourceConnection;

public class MileageSink extends RichSinkFunction<TripMileage> implements Serializable{
	

	/**
	* 
	*/
	private static final long serialVersionUID = 1L;
	private PreparedStatement statement;
	private Connection connection;
	
	String query = "INSERT INTO mileage.vehiclemileage( vin, evt_timestamp, odo_mileage, odo_distance, real_distance, gps_distance, modified_at)"
			+ " VALUES (?, ?, ?, ?, ?, ?, ?)"
			+ "  ON CONFLICT (vin) "
			+ "  DO UPDATE SET evt_timestamp = ?, odo_mileage = ?, odo_distance = ?, real_distance = ?, gps_distance = ?, modified_at = ?" ;
			
	@Override			  
	public void invoke(TripMileage rec) throws Exception {

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

		System.out.println("mileage data for veh "+rec);
		statement.addBatch();
		statement.executeBatch();
	}

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();
			
		connection=PostgreDataSourceConnection.getInstance().getDataSourceConnection(envParams.get(MileageConstants.DATAMART_POSTGRE_SERVER_NAME),
				Integer.parseInt(envParams.get(MileageConstants.DATAMART_POSTGRE_PORT)),
				envParams.get(MileageConstants.DATAMART_POSTGRE_DATABASE_NAME),
				envParams.get(MileageConstants.DATAMART_POSTGRE_USER),
				envParams.get(MileageConstants.DATAMART_POSTGRE_PASSWORD));
		System.out.println("In trip sink connection done" + connection);
		statement = connection.prepareStatement(query);
		
	}

	@Override
    public void close() throws Exception {
        if (statement != null) {
        	statement.close();
        }
        System.out.println("In close() of tripSink :: ");
        
        if (connection != null) {
        	System.out.println("Releasing connection from Trip Job");
            connection.close();
        }
             
        super.close(); 
		
    }
	
}