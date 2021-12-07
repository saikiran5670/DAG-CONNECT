package net.atos.daf.postgre.util;

public class DafConstants {
	//Postgre Sql
	

	
		public static final String POSTGRE_SQL_MAXIMUM_CONNECTION ="maximum_connection";
		public static final String POSTGRE_SQL_SSL_MODE = "&sslmode=require";
		
		public static final String UNKNOWN = "UNKNOWN";
		public static final String UNKNOWN_CASE_VAL = "Unknown";
		public static final String BLANK = "";
		public static  int RETRY_COUNTER=0;
		public static final int MAX_RETRIES=3;
		public static final int MAX_CONNECTIONS =95;
		public static final Long DTM_NULL_VAL =0L;
		public static final int DTM_NULL_VAL_int =0;
		
		public static final String DRIVER_ACTIVITY_READ = "select code,start_time from livefleet.livefleet_trip_driver_activity  where driver_id = ? order by id DESC limit 1";
		public static final String DRIVER_ACTIVITY_UPDATE = "UPDATE livefleet.livefleet_trip_driver_activity  SET end_time = ?, duration = ?, modified_at = extract(epoch from now()) * 1000 WHERE driver_id IN ( SELECT driver_id FROM livefleet.livefleet_trip_driver_activity WHERE driver_id = ? ORDER BY id DESC LIMIT 1 ) AND id IN ( SELECT id FROM livefleet.livefleet_trip_driver_activity WHERE driver_id = ? ORDER BY id DESC LIMIT 1 )";
		public static final String TRIP_LEVEL_AGGREGATION = "T";
		public static final Long ZERO =0L;
		
		public static final String JDBC_EXEC_OPTION_BATCH_SIZE = "jdbc.execution.options.batch.size";
		public static final String JDBC_EXEC_OPTION_BATCH_INTERVAL_MILLISEC ="jdbc.execution.options.batch.interval.millisec";
		public static final String JDBC_EXEC_OPTION_BATCH_MAX_RETRIES = "jdbc.execution.options.batch.max.retries";
}
