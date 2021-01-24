 package net.atos.daf.ct2.common.realtime.postgresql;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;

import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;

public class LiveFleetDataPostgreSink extends  RichSinkFunction<KafkaRecord<Index>> {
	

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	String livefleettest = "INSERT INTO public.livefleettest(vid, gpsaltitude, gpslatitude,gpslongitude,gpsheading) VALUES (?, ?, ?, ?, ?) ";
	
	  private PreparedStatement statement;
	 
	  public void invoke(KafkaRecord<Index> row) throws Exception {
		  
		  System.out.println("In Invoke");
		  
			/*
			 * System.out.println("VID" + row.getVID());
			 * 
			 * statement.setString(1,(String)row.getVID());
			 * //statement.setString(1,(String)row.getGPSDateTime());
			 * statement.setDouble(2,row.getGPSAltitude()); statement.setDouble(3,
			 * row.getGPSLatitude()); System.out.println("getGPSLatitude" +
			 * row.getGPSLatitude()); statement.setDouble(4, row.getGPSLongitude());
			 * statement.setDouble(5, row.getGPSHeading());
			 */
		  
		  statement.executeUpdate();
		  
		  System.out.println("In Invoke DONE");
		  
	  }

	  @Override
	  public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
		  System.out.println("IN Open");
		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();
	    Class.forName(envParams.get(DafConstants.POSTGRE_SQL_DRIVER));
	    String dbUrl = createValidUrlToConnectPostgreSql(envParams);
	    System.out.println("Before Connection");
	    Connection connection = DriverManager.getConnection(dbUrl);
	    statement = connection.prepareStatement(livefleettest);
	  }

	private String createValidUrlToConnectPostgreSql(ParameterTool envParams) throws Exception {
		
		System.out.println("In Password Function");

		String password = envParams.get(DafConstants.POSTGRE_SQL_PASSWORD);

		StringBuffer passwordBuffer = new StringBuffer();
		int dataLength = password.length();
		for (int i = 0; i < dataLength; i++) {
			char charecterAt = password.charAt(i);
			if (charecterAt == '%') {
				passwordBuffer.append("%25");
			} /*
				 * else if (charecterAt == '+') {
				 * passwordBuffer.append("<plus>"); }
				 */
			else if (charecterAt == '\\') {
				passwordBuffer.append("\\");
			} else {
				passwordBuffer.append(charecterAt);
			}
		}

		String url = envParams.get(DafConstants.POSTGRE_SQL_URL) + "password=" + passwordBuffer.toString()
				+ DafConstants.POSTGRE_SQL_SSL_MODE;
		
		System.out.println(" Password Function DONE");

		return url;

	}
         
     
	@Override
	public void close() throws Exception {
		
		System.out.println("In Close");
		
	}
	  

}
