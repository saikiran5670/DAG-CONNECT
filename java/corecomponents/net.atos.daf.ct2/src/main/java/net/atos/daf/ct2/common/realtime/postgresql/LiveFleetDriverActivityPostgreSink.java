package net.atos.daf.ct2.common.realtime.postgresql;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.postgresql.jdbc3.Jdbc3PoolingDataSource;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.postgre.PostgreDataSourceConnection;
import net.atos.daf.ct2.common.util.DafConstants;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.standard.Index;

@SuppressWarnings({})
public class LiveFleetDriverActivityPostgreSink extends RichSinkFunction<KafkaRecord<Index>> implements Serializable {

	private static Logger logger = LoggerFactory.getLogger(LiveFleetDriverActivityPostgreSink.class);

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	String livefleetdriver = "INSERT INTO livefleet.livefleet_trip_driver_activity  (trip_id	, trip_start_time_stamp	, trip_end_time_stamp	, activity_date,  vin	, driver_id	, code	, start_time	, end_time	, duration	, created_at_m2m	, created_at_kafka	, created_at_dm	, modified_at	, last_processed_message_time_stamp	) VALUES ( ?	, ?	, ?	, ?	, ?	, ?	, ?	, ?	, ?	, ?	, ?	, ?	, ?	, ?	,?) ";
	String readquery = "SELECT * FROM livefleet.livefleet_trip_driver_activity WHERE trip_start_time_stamp !=0 AND trip_id = ?";
	private PreparedStatement statement;
	private PreparedStatement stmt;
	Connection connection = null;

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

		ParameterTool envParams = (ParameterTool) getRuntimeContext().getExecutionConfig().getGlobalJobParameters();

		
		/*
		 * Class.forName(envParams.get(DafConstants.POSTGRE_SQL_DRIVER)); String dbUrl =
		 * createValidUrlToConnectPostgreSql(envParams); connection =
		 * DriverManager.getConnection(dbUrl);
		 */
		 

		Jdbc3PoolingDataSource dataSource = PostgreDataSourceConnection.getDataSource(
				envParams.get(DafConstants.DATAMART_POSTGRE_SERVER_NAME),
				Integer.parseInt(envParams.get(DafConstants.DATAMART_POSTGRE_PORT)),
				envParams.get(DafConstants.DATAMART_POSTGRE_DATABASE_NAME),
				envParams.get(DafConstants.DATAMART_POSTGRE_USER),
				envParams.get(DafConstants.DATAMART_POSTGRE_PASSWORD));
		
		try {
			
			connection = PostgreDataSourceConnection.getDataSourceConnection(dataSource);

			statement = connection.prepareStatement(livefleetdriver);
			stmt = connection.prepareStatement(readquery);
			
		}catch(Exception e) {
			logger.error("Error in Live fleet driver" + e.getMessage());
			
		}

		

	}

	public void invoke(KafkaRecord<Index> row) throws Exception {

		int varVEvtid = row.getValue().getVEvtID();
		long trip_Start_time = 0;
		String varTripID = row.getValue().getDocument().getTripID();
		
		

		stmt.setString(1, varTripID);

		ResultSet rs = stmt.executeQuery();

		while (rs.next()) {
			String Trip_ID = rs.getString("trip_id");
			trip_Start_time = rs.getLong("trip_start_time_stamp");// ("trip_start_time_stamp");
			String vid = rs.getString("vin");
			
			// Display values

			System.out.println("Duration : " + (row.getValue().getReceivedTimestamp() - trip_Start_time));

			// statement.setLong(10, (row.getReceivedTimestamp()-trip_Start_time) );
			// //Duration

		}
		rs.close();

		if (row.getValue().getDocument().getTripID() != null)
			statement.setString(1, row.getValue().getDocument().getTripID()); // tripID
		else
			statement.setString(1, row.getValue().getRoName());

		if (varVEvtid == 4)
			statement.setLong(2, row.getValue().getReceivedTimestamp()); // trip_start_time_stamp
		else
			statement.setLong(2, 0);

		if (varVEvtid == 5)
			statement.setLong(3, row.getValue().getReceivedTimestamp()); // trip_end_time_stamp
		else
			statement.setLong(3, 0);

		statement.setLong(4, row.getValue().getReceivedTimestamp()); // Activityy_date
		statement.setString(5, (String) row.getValue().getVin()); // vin
		statement.setString(6, (String) row.getValue().getDriverID()); // driver_id
		statement.setInt(7, row.getValue().getDocument().getDriver1WorkingState()); // code

		if ((row.getValue().getDocument().getDriver1CardInserted() == true)
				&& ((row.getValue().getDocument().getDriver1WorkingState() == 1)
						|| (row.getValue().getDocument().getDriver1WorkingState() == 2)
						|| (row.getValue().getDocument().getDriver1WorkingState() == 3)))
			statement.setLong(8, row.getValue().getReceivedTimestamp()); // start_time
		else
			statement.setInt(8, 0);

		if ((row.getValue().getDocument().getDriver1CardInserted() == false)
				|| ((row.getValue().getDocument().getDriver1CardInserted() == true)
						&& (row.getValue().getDocument().getDriver1WorkingState() == 3)))
			statement.setLong(9, row.getValue().getReceivedTimestamp()); // end_time
		else
			statement.setInt(9, 0);

		// statement.setDouble(10,
		// (row.getValue().getReceivedTimestamp()-trip_Start_time) ); //Duration
		statement.setLong(10, (row.getValue().getReceivedTimestamp() - trip_Start_time)); // Duration

		statement.setLong(11, row.getValue().getReceivedTimestamp()); // created_at_m2m
		statement.setLong(12, row.getValue().getReceivedTimestamp()); // created_at_kafka
		statement.setLong(13, row.getValue().getReceivedTimestamp()); // created_at_dm
		statement.setLong(14, row.getValue().getReceivedTimestamp()); // modified_at
		statement.setLong(15, row.getValue().getReceivedTimestamp()); // last_processed_message_time

		statement.executeUpdate();

	}

	
	@Override
	public void close() throws Exception {

		connection.close();

		logger.error("Error");

		System.out.println("In Close");

	}

}
