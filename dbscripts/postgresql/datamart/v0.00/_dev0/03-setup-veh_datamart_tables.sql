CREATE TABLE if not exists master.vehicle
(
	id	serial not null, 
	vin	varchar(50) not null, 
	vid  varchar(50) not null, 
	registration_no  varchar(25) , 
	type  varchar(50) not null, 
	engine_type  varchar(50) not null, 
	model_type  varchar(50) not null
)
TABLESPACE pg_default;

ALTER TABLE  master.vehicle 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_vehicle_id' AND table_name='vehicle'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.vehicle  
			ADD CONSTRAINT pk_vehicle_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='uk_vehicle_vin' AND table_name='vehicle'
		and constraint_type='UNIQUE')
then	
	begin
		ALTER TABLE  master.vehicle 
			ADD CONSTRAINT uk_vehicle_vin UNIQUE  (vin)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

-----------
CREATE TABLE if not exists master.driver
(
	id	serial not null, 
	driver_id  varchar(19) not null, 
	first_name  varchar(50) not null, 
	last_name  varchar(50) 
)
TABLESPACE pg_default;

ALTER TABLE  master.driver 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_driver_id' AND table_name='driver'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.driver  
			ADD CONSTRAINT pk_driver_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='uk_driver_driverid' AND table_name='driver'
		and constraint_type='UNIQUE')
then	
	begin
		ALTER TABLE  master.driver 
			ADD CONSTRAINT uk_driver_driverid UNIQUE  (driver_id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.warning_details
(
	id	serial not null, 
	code  varchar(20) not null, 
	name varchar(50) not null, 
	description  varchar(100) , 
	advice  varchar(500) 
)
TABLESPACE pg_default;

ALTER TABLE  master.warning_details 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_warningdetails_id' AND table_name='warning_details'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.warning_details  
			ADD CONSTRAINT pk_warningdetails_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='uk_warningdetails_code' AND table_name='warning_details'
		and constraint_type='UNIQUE')
then	
	begin
		ALTER TABLE  master.warning_details 
			ADD CONSTRAINT uk_warningdetails_code UNIQUE  (code)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--trip_statistics 
CREATE TABLE if not exists tripdetail.trip_statistics
(
	id	serial not null, 
	trip_id	varchar(50) not null, 
	vin	varchar(50) not null, 
	start_time_stamp bigint not null, 
	end_time_stamp	bigint,
	veh_message_distance	int,
	etl_gps_distance	bigint,
	idle_duration	int,
	average_speed	decimal,
	average_weight	decimal,
	start_odometer	bigint,
	last_odometer	bigint,
	start_position_lattitude	double precision,
	start_position_longitude	double precision,
	end_position_lattitude	double precision,
	end_position_longitude	double precision,
	start_position	varchar(100),
	end_position	varchar(100),
	veh_message_fuel_consumed	int,
	etl_gps_fuel_consumed	bigint,
	veh_message_driving_time	int,
	etl_gps_driving_time	int,
	no_of_alerts	int,
	no_of_events	int,
	message_received_timestamp	bigint,
	message_inserted_into_kafka_timestamp	bigint,
	message_inserted_into_hbase_timestamp	bigint,
	message_processed_by_ETL_process_timestamp	bigint,
	co2_emission	decimal,
	fuel_consumption	decimal,
	max_speed	decimal,
	average_gross_weight_comb	decimal,
	pto_duration	decimal,
	harsh_brake_duration	decimal,
	heavy_throttle_duration	decimal,
	cruise_control_distance_30_50	decimal,
	cruise_control_distance_50_75	decimal,
	cruise_control_distance_more_than_75	decimal,
	average_traffic_classification	varchar(50),
	cc_fuel_consumption	decimal,
	v_cruise_control_fuel_consumed_for_cc_fuel_consumption	int,
	v_cruise_control_dist_for_cc_fuel_consumption	int,
	fuel_consumption_cc_non_active	decimal,
	idling_consumption	int,
	dpa_score	decimal,
	endurance_brake	decimal,
	coasting	decimal,
	eco_rolling	decimal,
	driver1_id	varchar(19),
	driver2_id	varchar(19),
	etl_gps_trip_time	int,
	is_ongoing_trip  boolean 
)
TABLESPACE pg_default;

ALTER TABLE  tripdetail.trip_statistics 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_tripstatistics_id' AND table_name='trip_statistics'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  tripdetail.trip_statistics  
			ADD CONSTRAINT pk_tripstatistics_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='uk_tripstatistics_tripid' AND table_name='trip_statistics'
		and constraint_type='UNIQUE')
then	
	begin
		ALTER TABLE  tripdetail.trip_statistics 
			ADD CONSTRAINT uk_tripstatistics_tripid UNIQUE  (trip_id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--Do $$
--begin
--if not exists(
--	SELECT 1 FROM information_schema.table_constraints 
--	WHERE constraint_name='fk_tripstatistics_vin_vehicle_vin' AND table_name='trip_statistics'
--		and constraint_type='FOREIGN KEY')
--then	
--	begin
--		ALTER TABLE  tripdetail.trip_statistics 
--			ADD CONSTRAINT fk_tripstatistics_vin_vehicle_vin FOREIGN KEY (vin) REFERENCES  master.vehicle (vin);
--			--USING INDEX TABLESPACE pg_default;
--	end;
--end if;
--end;
--$$;

--Do $$
--begin
--if not exists(
--	SELECT 1 FROM information_schema.table_constraints 
--	WHERE constraint_name='fk_tripstatistics_driver1id_driver_driverid' AND table_name='trip_statistics'
--		and constraint_type='FOREIGN KEY')
--then	
--	begin
--		ALTER TABLE  tripdetail.trip_statistics 
--			ADD CONSTRAINT fk_tripstatistics_driver1id_driver_driverid FOREIGN KEY (driver1_id) REFERENCES  master.driver (driver_id);
--			--USING INDEX TABLESPACE pg_default;
--	end;
--end if;
--end;
--$$;

--Do $$
--begin
--if not exists(
--	SELECT 1 FROM information_schema.table_constraints 
--	WHERE constraint_name='fk_tripstatistics_driver2id_driver_driverid' AND table_name='trip_statistics'
--		and constraint_type='FOREIGN KEY')
--then	
--	begin
--		ALTER TABLE  tripdetail.trip_statistics 
--			ADD CONSTRAINT fk_tripstatistics_driver2id_driver_driverid FOREIGN KEY (driver2_id) REFERENCES  master.driver (driver_id);
--			--USING INDEX TABLESPACE pg_default;
--	end;
--end if;
--end;
--$$;

----------------

CREATE TABLE if not exists livefleet.livefleet_position_statistics
(
	id  serial not null, 
	trip_id  varchar(36) not null, 
	vin  varchar(50) not null, 
	message_time_stamp  bigint not null, 
	gps_altitude  int, 
	gps_heading  double precision, 
	gps_latitude  double precision, 
	gps_longitude  double precision, 
	co2_emission  int, 
	fuel_consumption  decimal, 
	last_odometer_val  int, 
	distance_until_next_service  int, 
	created_at_m2m  bigint, 
	created_at_kafka   bigint, 
	created_at_dm  bigint
)
TABLESPACE pg_default;

ALTER TABLE  livefleet.livefleet_position_statistics
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_livefleetposstat_id' AND table_name='livefleet_position_statistics'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  livefleet.livefleet_position_statistics
			ADD CONSTRAINT pk_livefleetposstat_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--Do $$
--begin
--if not exists(
--	SELECT 1 FROM information_schema.table_constraints 
--	WHERE constraint_name='fk_livefleetposstat_vin_vehicle_vin' AND table_name='livefleet_position_statistics'
--		and constraint_type='FOREIGN KEY')
--then	
--	begin
--		ALTER TABLE  livefleet.livefleet_position_statistics
--			ADD CONSTRAINT fk_livefleetposstat_vin_vehicle_vin FOREIGN KEY (vin) REFERENCES  master.vehicle (vin);
--			--USING INDEX TABLESPACE pg_default;
--	end;
--end if;
--end;
--$$;

CREATE TABLE if not exists livefleet.livefleet_current_trip_statistics
(
	id  serial not null, 
	trip_id  varchar(36) not null, 
	vin  varchar(50) not null, 
	start_time_stamp  bigint  not null,
	end_time_stamp  bigint,
	driver1_id  varchar(19) not null,
	start_position_lattitude  double precision,
	start_position_longitude  double precision,
	start_position  varchar(150),
	last_received_position_lattitude  double precision,
	last_received_position_longitude  double precision,
	last_known_position  varchar(150),
	vehicle_status  int,
	driver1_status  int,
	vehicle_health_status  int,
	last_odometer_val  int,
	distance_until_next_service  int,
	last_processed_message_time_stamp  bigint not null,
	driver2_id  varchar(19),
	driver2_status  int,
	created_at_m2m  bigint,
	created_at_kafka   bigint,
	created_at_dm_  bigint,
	modified_at  bigint
)
TABLESPACE pg_default;

ALTER TABLE  livefleet.livefleet_current_trip_statistics
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_livefleetcurtripstat_id' AND table_name='livefleet_current_trip_statistics'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  livefleet.livefleet_current_trip_statistics
			ADD CONSTRAINT pk_livefleetcurtripstat_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--Do $$
--begin
--if not exists(
--	SELECT 1 FROM information_schema.table_constraints 
--	WHERE constraint_name='fk_livefleetcurtripstat_driver1id_driver_driverid' AND table_name='livefleet_current_trip_statistics'
--		and constraint_type='FOREIGN KEY')
--then	
--	begin
--		ALTER TABLE  livefleet.livefleet_current_trip_statistics 
--			ADD CONSTRAINT fk_livefleetcurtripstat_driver1id_driver_driverid FOREIGN KEY (driver1_id) REFERENCES  master.driver (driver_id);
--			--USING INDEX TABLESPACE pg_default;
--	end;
--end if;
--end;
--$$;

--Do $$
--begin
--if not exists(
--	SELECT 1 FROM information_schema.table_constraints 
--	WHERE constraint_name='fk_livefleetcurtripstat_driver2id_driver_driverid' AND table_name='livefleet_current_trip_statistics'
--		and constraint_type='FOREIGN KEY')
--then	
--	begin
--		ALTER TABLE  livefleet.livefleet_current_trip_statistics 
--			ADD CONSTRAINT fk_livefleetcurtripstat_driver2id_driver_driverid FOREIGN KEY (driver2_id) REFERENCES  master.driver (driver_id);
--			--USING INDEX TABLESPACE pg_default;
--	end;
--end if;
--end;
--$$;

--Do $$
--begin
--if not exists(
--	SELECT 1 FROM information_schema.table_constraints 
--	WHERE constraint_name='fk_livefleetcurtripstat_vin_vehicle_vin' AND table_name='livefleet_current_trip_statistics'
--		and constraint_type='FOREIGN KEY')
--then	
--	begin
--		ALTER TABLE  livefleet.livefleet_current_trip_statistics
--			ADD CONSTRAINT fk_livefleetcurtripstat_vin_vehicle_vin FOREIGN KEY (vin) REFERENCES  master.vehicle (vin);
--			--USING INDEX TABLESPACE pg_default;
--	end;
--end if;
--end;
--$$;

CREATE TABLE if not exists livefleet.livefleet_trip_driver_activity
(
	id  serial not null, 
	trip_id  varchar(36) not null, 
	trip_start_time_stamp  bigint not null, 
	trip_end_time_stamp  bigint, 
	activity_date  bigint not null, 
	vin  varchar(50) not null, 
	driver_id  varchar(19) not null, 
	code  int not null,  
	start_time  bigint not null, 
	end_time  bigint, 
	duration  int, 
	created_at_m2m  bigint, 
	created_at_kafka   bigint, 
	created_at_dm  bigint, 
	modified_at  bigint, 
	last_processed_message_time_stamp  bigint
)
TABLESPACE pg_default;

ALTER TABLE  livefleet.livefleet_trip_driver_activity
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_livefleettripdrvact_id' AND table_name='livefleet_trip_driver_activity'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  livefleet.livefleet_trip_driver_activity
			ADD CONSTRAINT pk_livefleettripdrvact_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--Do $$
--begin
--if not exists(
--	SELECT 1 FROM information_schema.table_constraints 
--	WHERE constraint_name='fk_livefleettripdrvact_driverid_driver_driverid' AND table_name='livefleet_trip_driver_activity'
--		and constraint_type='FOREIGN KEY')
--then	
--	begin
--		ALTER TABLE  livefleet.livefleet_trip_driver_activity 
--			ADD CONSTRAINT fk_livefleettripdrvact_driverid_driver_driverid FOREIGN KEY (driver_id) REFERENCES  master.driver (driver_id);
--			--USING INDEX TABLESPACE pg_default;
--	end;
--end if;
--end;
--$$;

--Do $$
--begin
--if not exists(
--	SELECT 1 FROM information_schema.table_constraints 
--	WHERE constraint_name='fk_livefleettripdrvact_vin_vehicle_vin' AND table_name='livefleet_trip_driver_activity'
--		and constraint_type='FOREIGN KEY')
--then	
--	begin
--		ALTER TABLE  livefleet.livefleet_trip_driver_activity
--			ADD CONSTRAINT fk_livefleettripdrvact_vin_vehicle_vin FOREIGN KEY (vin) REFERENCES  master.vehicle (vin);
--			--USING INDEX TABLESPACE pg_default;
--	end;
--end if;
--end;
--$$;

CREATE TABLE if not exists livefleet.livefleet_warning_statistics
(
	id  serial not null, 
	trip_id  varchar(36) not null, 
	vin  varchar(50) not null, 
	warning_time_stamp  bigint not null, 
	warning_code  varchar(19) not null, 
	activated_time  bigint not null, 
	deactivated_time  bigint , 
	engine_speed  int,
	ad_blue_level  decimal,
	fuel_level  decimal,
	created_at_m2m  bigint,
	created_at_kafka  bigint,
	created_at_dm  bigint,
	modified_at  bigint,
	last_odometer  int,
	distance_until_next_service  int,
	vehicle_health_status  int,
	vehicle_status  int,
	driver1_id  varchar(19),
	driver1_status  int,
	driver2_id  varchar(19),
	driver2_status  int
)
TABLESPACE pg_default;

ALTER TABLE  livefleet.livefleet_warning_statistics
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_livefleetwarnstat_id' AND table_name='livefleet_warning_statistics'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  livefleet.livefleet_warning_statistics
			ADD CONSTRAINT pk_livefleetwarnstat_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--Do $$
--begin
--if not exists(
--	SELECT 1 FROM information_schema.table_constraints 
--	WHERE constraint_name='fk_livefleetwarnstat_driver1id_driver_driverid' AND table_name='livefleet_warning_statistics'
--		and constraint_type='FOREIGN KEY')
--then	
--	begin
--		ALTER TABLE  livefleet.livefleet_warning_statistics 
--			ADD CONSTRAINT fk_livefleetwarnstat_driver1id_driver_driverid FOREIGN KEY (driver1_id) REFERENCES  master.driver (driver_id);
--			--USING INDEX TABLESPACE pg_default;
--	end;
--end if;
--end;
--$$;

--Do $$
--begin
--if not exists(
--	SELECT 1 FROM information_schema.table_constraints 
--	WHERE constraint_name='fk_livefleetwarnstat_driver2id_driver_driverid' AND table_name='livefleet_warning_statistics'
--		and constraint_type='FOREIGN KEY')
--then	
--	begin
--		ALTER TABLE  livefleet.livefleet_warning_statistics 
--			ADD CONSTRAINT fk_livefleetwarnstat_driver2id_driver_driverid FOREIGN KEY (driver2_id) REFERENCES  master.driver (driver_id);
--			--USING INDEX TABLESPACE pg_default;
--	end;
--end if;
--end;
--$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_livefleetwarnstat_warningid_warningdetails_code' AND table_name='livefleet_warning_statistics'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  livefleet.livefleet_warning_statistics 
			ADD CONSTRAINT fk_livefleetwarnstat_warningid_warningdetails_code FOREIGN KEY (warning_code) REFERENCES  master.warning_details (code);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--Do $$
--begin
--if not exists(
--	SELECT 1 FROM information_schema.table_constraints 
--	WHERE constraint_name='fk_livefleetwarnstat_vin_vehicle_vin' AND table_name='livefleet_warning_statistics'
--		and constraint_type='FOREIGN KEY')
--then	
--	begin
--		ALTER TABLE  livefleet.livefleet_warning_statistics
--			ADD CONSTRAINT fk_livefleetwarnstat_vin_vehicle_vin FOREIGN KEY (vin) REFERENCES  master.vehicle (vin);
--			--USING INDEX TABLESPACE pg_default;
--	end;
--end if;
--end;
--$$;
