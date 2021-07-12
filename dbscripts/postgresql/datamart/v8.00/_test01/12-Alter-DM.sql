CREATE TABLE if not exists livefleet.livefleet_trip_fuel_deviation 
(
	id  serial not null,
	trip_id  varchar(50) not null,
	vin  varchar(17) not null,
	fuel_event_type  char(1) not null,
	vehicle_activity_type char(1) not null,
	event_time  bigint not null,
	fuel_difference decimal,
	odometer_val bigint,
	latitude  double precision,
	longitude  double precision,
	heading  double precision,
	geolocation_address_id int,
	created_at bigint
)	
TABLESPACE pg_default;

ALTER TABLE  livefleet.livefleet_trip_fuel_deviation 
    OWNER to pgdbdmadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_tripfueldeviation_id' AND table_name='livefleet_trip_fuel_deviation'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  livefleet.livefleet_trip_fuel_deviation  
			ADD CONSTRAINT pk_tripfueldeviation_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

drop table livefleet.livefleet_current_trip_statistics ;
drop table livefleet.livefleet_warning_statistics ;

CREATE TABLE if not exists livefleet.livefleet_warning_statistics 
(
	id  serial not null,
	trip_id  varchar(45) ,
	vin  varchar(17) not null,
	warning_time_stamp  bigint not null,
	warning_class  int not null,
	warning_number  int not null,
	latitude  double precision,
	longitude  double precision,
	heading  double precision,
	vehicle_health_status_type  char(1) not null,
	vehicle_driving_status_type  char(1) not null,
	driver1_id  varchar(19) ,
	warning_type  char(1) not null,
	distance_until_next_service  bigint,
	odometer_val  bigint ,
	lastest_processed_message_time_stamp  bigint not null,
	created_at  bigint not null,
	modified_at  bigint
)	
TABLESPACE pg_default;

ALTER TABLE  livefleet.livefleet_warning_statistics 
    OWNER to pgdbdmadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_livefleet_warning_statistics_id' AND table_name='livefleet_warning_statistics'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  livefleet.livefleet_warning_statistics  
			ADD CONSTRAINT pk_livefleet_warning_statistics_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists livefleet.livefleet_current_trip_statistics 
(
	id  serial not null,
	trip_id  varchar(45) not null,
	vin  varchar(17) not null,
	start_time_stamp  bigint not null ,
	end_time_stamp  bigint,
	driver1_id  varchar(19),
	trip_distance  int,
	driving_time  int,
	fuel_consumption  int,
	vehicle_driving_status_type  char(1) not null,
	odometer_val  bigint,
	distance_until_next_service  bigint,
	latest_received_position_lattitude  double precision,
	latest_received_position_longitude  double precision,
	latest_received_position_heading  double precision,
	latest_geolocation_address_id  int,
	start_position_lattitude  double precision,
	start_position_longitude  double precision,
	start_position_heading  double precision,
	start_geolocation_address_id  int,
	latest_processed_message_time_stamp  bigint not null,
	vehicle_health_status_type  char(1) not null,
	latest_warning_class  int,
	latest_warning_number  int,
	latest_warning_type  char(1),
	latest_warning_timestamp  bigint,
	latest_warning_position_latitude  double precision,
	latest_warning_position_longitude  double precision,
	latest_warning_geolocation_address_id  int,
	created_at  bigint not null,
	modified_at  bigint
)	
TABLESPACE pg_default;

ALTER TABLE  livefleet.livefleet_current_trip_statistics 
    OWNER to pgdbdmadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_livefleet_current_trip_statistics_id' AND table_name='livefleet_current_trip_statistics'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  livefleet.livefleet_current_trip_statistics  
			ADD CONSTRAINT pk_livefleet_current_trip_statistics_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


alter table livefleet.livefleet_current_trip_statistics alter column trip_id type varchar(45);
alter table tripdetail.trip_statistics alter column trip_id type varchar(45);
alter table livefleet.livefleet_position_statistics alter column trip_id type varchar(45);
alter table livefleet.livefleet_trip_driver_activity alter column trip_id type varchar(45);
alter table livefleet.livefleet_current_trip_statistics alter column trip_id type varchar(45);
alter table tripdetail.ecoscoredata alter column trip_id type varchar(45);
alter table tripdetail.tripalert alter column trip_id type varchar(45);
alter table tripdetail.tripalerts alter column trip_id type varchar(45);

alter table tripdetail.ecoscoredata drop column driver2_id;
alter table tripdetail.ecoscoredata rename column etl_trip_distance to trip_distance;

alter table tripdetail.ecoscoredata add column if not exists cruise_control_usage  decimal;
alter table tripdetail.ecoscoredata add column if not exists cruise_control_usage_30_50  decimal;
alter table tripdetail.ecoscoredata add column if not exists cruise_control_usage_50_75  decimal;
alter table tripdetail.ecoscoredata add column if not exists cruise_control_usage_75  decimal;
alter table tripdetail.ecoscoredata add column if not exists tacho_gross_weight_combination  decimal;
alter table tripdetail.ecoscoredata add column if not exists harsh_brake_duration  int;
alter table tripdetail.ecoscoredata add column if not exists brake_duration  int;

alter table tripdetail.ecoscoredata add column if not exists created_at  bigint ;
update tripdetail.ecoscoredata set created_at=(select extract(epoch from now()) * 1000);
alter table tripdetail.ecoscoredata alter column  created_at set not null;

alter table tripdetail.ecoscoredata add column if not exists granular_level_type  char(1);
update tripdetail.ecoscoredata set granular_level_type='T';
alter table tripdetail.ecoscoredata alter column  granular_level_type set not null;


alter table tripdetail.ecoscoredata add column if not exists lastest_processed_message_time_stamp  bigint ;
alter table tripdetail.ecoscoredata add column if not exists modified_at bigint ;

alter table tripdetail.ecoscoredata drop column start_fuel;
alter table tripdetail.ecoscoredata drop column end_fuel;



CREATE TABLE if not exists tripdetail.vehiclealertref
(
	id  serial not null,
	vin  varchar(17) not null,
	alert_id int not null,
	state char(1),
	created_at bigint
)	
TABLESPACE pg_default;

ALTER TABLE  tripdetail.vehiclealertref
    OWNER to pgdbdmadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_vehiclealertref_id' AND table_name='livefleet_trip_fuel_deviation'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  tripdetail.vehiclealertref
			ADD CONSTRAINT pk_vehiclealertref_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


alter table tripdetail.trip_statistics add column if not exists veh_message_pto_duration bigint;
alter table tripdetail.trip_statistics add column if not exists veh_message_harsh_brake_duration bigint;
alter table tripdetail.trip_statistics add column if not exists veh_message_brake_duration bigint;
alter table tripdetail.trip_statistics add column if not exists veh_message_max_throttle_paddle_duration bigint;
alter table tripdetail.trip_statistics add column if not exists veh_message_accelerationt_time bigint;
alter table tripdetail.trip_statistics add column if not exists veh_message_dpabraking_count int;
alter table tripdetail.trip_statistics add column if not exists veh_message_dpaanticipation_count int;
alter table tripdetail.trip_statistics add column if not exists veh_message_dpabraking_score bigint;
alter table tripdetail.trip_statistics add column if not exists veh_message_dpaanticipation_score bigint;
alter table tripdetail.trip_statistics add column if not exists veh_message_idle_without_ptoduration bigint;
alter table tripdetail.trip_statistics add column if not exists veh_message_idle_ptoduration bigint;



alter table master.vehicle alter column engine_type drop not null;
alter table master.vehicle alter column model_type drop not null;

alter table tripdetail.trip_statistics alter column etl_gps_driving_time type bigint;
alter table tripdetail.trip_statistics alter column etl_gps_trip_time type bigint;
