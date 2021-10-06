CREATE TABLE if not exists livefleet.index_message_data 
(
	id  serial not null,
	trip_id varchar(45) not null,
	vin varchar(17) not null,
	tachograph_speed int,
	gross_weight_combination bigint,
	driver2_id varchar(19),
	driver1_id varchar(19),
	jobname  varchar(50),
	increment int,
	distance bigint,
	event_datetime bigint,
	event_id int,
	created_at bigint
)	
TABLESPACE pg_default;

ALTER TABLE  livefleet.index_message_data 
    OWNER to pgdbadmin;
	
Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_index_message_data_id' AND table_name='index_message_data'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  livefleet.index_message_data 
			ADD CONSTRAINT pk_index_message_data_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


CREATE TABLE if not exists tripdetail.notificationviewhistory 
(
	id  serial not null,
	trip_id varchar(45) not null,
	vin varchar(17) not null, 
	alert_category char(1) not null,
	alert_type char(1) not null,
	alert_id int not null,
	alert_generated_time bigint not null,
	organization_id int not null,
	account_id int not null,
	alert_view_timestamp bigint not null,
	trip_alert_id int not null	
)	
TABLESPACE pg_default;

ALTER TABLE  tripdetail.notificationviewhistory 
    OWNER to pgdbadmin;
	
Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_notificationviewhistory_id' AND table_name='notificationviewhistory'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  tripdetail.notificationviewhistory 
			ADD CONSTRAINT pk_notificationviewhistory_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_notificationviewhistory_tripalertid_tripalert_id' AND table_name='notificationviewhistory'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  tripdetail.notificationviewhistory 
			ADD CONSTRAINT fk_notificationviewhistory_tripalertid_tripalert_id FOREIGN KEY (trip_alert_id) REFERENCES  tripdetail.tripalert (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

create index idx_indexmessagedata_tripid on livefleet.index_message_data (trip_id);

alter table livefleet.livefleet_position_statistics add column if not exists total_vehicle_distance  bigint;
alter table livefleet.livefleet_position_statistics add column if not exists total_engine_hours  bigint;
alter table livefleet.livefleet_position_statistics add column if not exists total_engine_fuel_used  bigint;
alter table livefleet.livefleet_position_statistics add column if not exists gross_combination_vehicle_weight  bigint;
alter table livefleet.livefleet_position_statistics add column if not exists engine_speed  bigint;
alter table livefleet.livefleet_position_statistics add column if not exists fuel_level1  decimal;
alter table livefleet.livefleet_position_statistics add column if not exists catalyst_fuel_level  decimal;
alter table livefleet.livefleet_position_statistics add column if not exists driver2_id  varchar(19);
alter table livefleet.livefleet_position_statistics add column if not exists driver1_working_state  int;
alter table livefleet.livefleet_position_statistics add column if not exists driver2_working_state  int;
alter table livefleet.livefleet_position_statistics add column if not exists driver2_auth_equipment_type_id  int;
alter table livefleet.livefleet_position_statistics add column if not exists driver2_card_replacement_index  varchar(10);
alter table livefleet.livefleet_position_statistics add column if not exists oem_driver2_id_type  varchar(10);
alter table livefleet.livefleet_position_statistics add column if not exists oem_driver2_id  varchar(19);
alter table livefleet.livefleet_position_statistics add column if not exists ambient_air_temperature  int;
alter table livefleet.livefleet_position_statistics add column if not exists engine_coolant_temperature  int;
alter table livefleet.livefleet_position_statistics add column if not exists service_brake_air_pressure_circuit1  bigint;
alter table livefleet.livefleet_position_statistics add column if not exists service_brake_air_pressure_circuit2  bigint;

alter table tripdetail.trip_statistics add column if not exists duration_wheelbase_speed_over_zero   bigint;
alter table tripdetail.trip_statistics add column if not exists distance_cruise_control_active   bigint;
alter table tripdetail.trip_statistics add column if not exists duration_cruise_control_active   bigint;
alter table tripdetail.trip_statistics add column if not exists fuel_consumption_during_cruise_active   bigint;
alter table tripdetail.trip_statistics add column if not exists duration_wheelbase_speed_zero   bigint;
alter table tripdetail.trip_statistics add column if not exists fuel_during_wheelbase_speed_zero   bigint;
alter table tripdetail.trip_statistics add column if not exists fuel_wheelbase_speed_over_zero   bigint;
alter table tripdetail.trip_statistics add column if not exists brake_pedal_counter_speed_over_zero   int;
alter table tripdetail.trip_statistics add column if not exists distance_brake_pedal_active_speed_over_zero   int;
alter table tripdetail.tripalertdaytimeconfigparam add column if not exists  calendar_day_timeperiod_type  char(1);
--******

alter table tripdetail.notificationviewhistory drop constraint fk_notificationviewhistory_tripalertid_tripalert_id;

alter table tripdetail.trip_statistics add column if not exists pto_active_class_pto_duration BIGint;
alter table tripdetail.trip_statistics add column if not exists pto_active_class_pto_fuel_consumed BIGint;
alter table tripdetail.trip_statistics add column if not exists acceleration_pedal_pos_class_distr  json;
alter table tripdetail.trip_statistics add column if not exists acceleration_pedal_pos_class_min_range  int;
alter table tripdetail.trip_statistics add column if not exists acceleration_pedal_pos_class_max_range  int;
alter table tripdetail.trip_statistics add column if not exists acceleration_pedal_pos_class_distr_step int;
alter table tripdetail.trip_statistics add column if not exists acceleration_pedal_pos_class_distr_array_time  int[];

alter table tripdetail.trip_statistics add column if not exists retarder_torque_class_distr json;
alter table tripdetail.trip_statistics add column if not exists retarder_torque_class_min_range Int;
alter table tripdetail.trip_statistics add column if not exists retarder_torque_class_max_range Int;
alter table tripdetail.trip_statistics add column if not exists retarder_torque_class_distr_step int;
alter table tripdetail.trip_statistics add column if not exists retarder_torque_class_distr_array_time  int[];

alter table tripdetail.trip_statistics add column if not exists engine_torque_engine_load_class_distr json;
alter table tripdetail.trip_statistics add column if not exists engine_torque_engine_load_class_min_range Int;
alter table tripdetail.trip_statistics add column if not exists engine_torque_engine_load_class_max_range Int;
alter table tripdetail.trip_statistics add column if not exists engine_torque_engine_load_class_distr_step int;
alter table tripdetail.trip_statistics add column if not exists engine_torque_engine_load_class_distr_array_time  int[];

alter table tripdetail.trip_statistics add column if not exists pto_distance bigint;
alter table tripdetail.trip_statistics add column if not exists trip_idle_pto_fuel_consumed bigint;
alter table tripdetail.trip_statistics add column if not exists idling_consumption_with_pto bigint;

alter table tripdetail.trip_statistics alter column idling_consumption_with_pto type decimal ;


Update tripdetail.trip_statistics set driver1_id='Unknown'
where driver1_id is null or driver1_id='' or driver1_id='UNKNOWN' ;

Update tripdetail.trip_statistics set driver2_id='Unknown'
where driver2_id is null or driver2_id='' or driver2_id='UNKNOWN';

Update tripdetail.ecoscoredata set driver1_id='Unknown'
where driver1_id is null or driver1_id='' or driver1_id='UNKNOWN' ;

Update livefleet.livefleet_trip_driver_activity set driver_id='Unknown'
where driver_id is null or driver_id='' or driver_id='UNKNOWN' ;

Update livefleet.livefleet_current_trip_statistics set driver1_id='Unknown'
where driver1_id is null or driver1_id='' or driver1_id='UNKNOWN' ;

Update livefleet.livefleet_warning_statistics set driver1_id='Unknown'
where driver1_id is null or driver1_id='' or driver1_id='UNKNOWN' ;

Update livefleet.livefleet_position_statistics set driver1_id='Unknown'
where driver1_id is null or driver1_id='' or driver1_id='UNKNOWN' ;

Update livefleet.livefleet_position_statistics set driver2_id='Unknown'
where driver2_id is null or driver2_id='' or driver2_id='UNKNOWN';

