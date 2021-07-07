package net.atos.daf.postgre.util;

public class DafConstants {
	//Postgre Sql
	

	
		public static final String POSTGRE_SQL_MAXIMUM_CONNECTION ="maximum_connection";
		public static final String POSTGRE_SQL_SSL_MODE = "&sslmode=require";
		
		public static final String UNKNOWN = "UNKNOWN";
		public static  int RETRY_COUNTER=0;
		public static final int MAX_RETRIES=3;
		public static final int MAX_CONNECTIONS =30;
		public static final Long DTM_NULL_VAL =0L;
		public static final int DTM_NULL_VAL_int =0;
		
		public static final String DRIVER_ACTIVITY_READ = "select code,start_time from livefleet.livefleet_trip_driver_activity  where driver_id = ? order by id DESC limit 1";
		public static final String DRIVER_ACTIVITY_UPDATE = "UPDATE livefleet.livefleet_trip_driver_activity  SET end_time = ?, duration = ?, modified_at = extract(epoch from now()) * 1000 WHERE driver_id IN ( SELECT driver_id FROM livefleet.livefleet_trip_driver_activity WHERE driver_id = ? ORDER BY id DESC LIMIT 1 ) AND id IN ( SELECT id FROM livefleet.livefleet_trip_driver_activity WHERE driver_id = ? ORDER BY id DESC LIMIT 1 )";
		public static final String TRIP_LEVEL_AGGREGATION = "T";
}
