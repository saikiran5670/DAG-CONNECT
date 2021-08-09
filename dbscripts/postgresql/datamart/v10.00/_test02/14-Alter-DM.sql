
alter table livefleet.livefleet_position_statistics add column if not exists veh_message_type char(1) default 'I';
update livefleet.livefleet_position_statistics  set veh_message_type='I';
alter table livefleet.livefleet_position_statistics alter column veh_message_type set not null;
alter table livefleet.livefleet_position_statistics add column if not exists vehicle_msg_trigger_type_id int;
alter table livefleet.livefleet_position_statistics add column if not exists created_datetime bigint;
alter table livefleet.livefleet_position_statistics add column if not exists received_datetime bigint;
alter table livefleet.livefleet_position_statistics add column if not exists gps_speed decimal;
alter table livefleet.livefleet_position_statistics add column if not exists gps_datetime bigint;
alter table livefleet.livefleet_position_statistics add column if not exists wheelbased_speed decimal;
alter table livefleet.livefleet_position_statistics add column if not exists tachgraph_speed decimal;
alter table livefleet.livefleet_position_statistics add column if not exists driver1_id varchar(19);
alter table livefleet.livefleet_position_statistics add column if not exists vehicle_msg_trigger_additional_info varchar(100);
alter table livefleet.livefleet_position_statistics add column if not exists driver_auth_equipment_type_id int;
alter table livefleet.livefleet_position_statistics add column if not exists card_replacement_index varchar(10);
alter table livefleet.livefleet_position_statistics add column if not exists oem_driver_id_type varchar(10);
alter table livefleet.livefleet_position_statistics add column if not exists oem_driver_id varchar(19);
alter table livefleet.livefleet_position_statistics add column if not exists pto_id varchar(50);
alter table livefleet.livefleet_position_statistics add column if not exists telltale_id int;
alter table livefleet.livefleet_position_statistics add column if not exists oem_telltale varchar(50);
alter table livefleet.livefleet_position_statistics add column if not exists telltale_state_id int;


CREATE TABLE if not exists tripdetail.multi_day_trip_statistics --(master data)
(
	id  serial not null,
	livefleet_curr_trip_id  int not null,
	trip_id  varchar(45) not null,
	vin  varchar(17) not null,
	driver1_id  varchar(19) ,
	activity_datetime  bigint not null,
	trip_distance  int,
	driving_time  int
)	
TABLESPACE pg_default;

ALTER TABLE  tripdetail.multi_day_trip_statistics  
    OWNER to pgdbdmadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_multidaytripstatistics_id' AND table_name='multi_day_trip_statistics'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  tripdetail.multi_day_trip_statistics  
			ADD CONSTRAINT pk_multidaytripstatistics_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


alter table tripdetail.trip_statistics add column if not exists rpm_torque  json    ;
alter table tripdetail.trip_statistics add column if not exists abs_rpm_torque  int;
alter table tripdetail.trip_statistics add column if not exists ord_rpm_torque  int;
alter table tripdetail.trip_statistics add column if not exists nonzero_matrix_val_rpm_torque  int[];
alter table tripdetail.trip_statistics add column if not exists num_val_rpm_torque  int[];
alter table tripdetail.trip_statistics add column if not exists col_index_rpm_torque  int[];
alter table tripdetail.trip_statistics add column if not exists speed_rpm  json     ;
alter table tripdetail.trip_statistics add column if not exists abs_speed_rpm  int;
alter table tripdetail.trip_statistics add column if not exists ord_speed_rpm  int;
alter table tripdetail.trip_statistics add column if not exists nonzero_matrix_val_speed_rpm  int[];
alter table tripdetail.trip_statistics add column if not exists num_val_speed_rpm  int[];
alter table tripdetail.trip_statistics add column if not exists col_index_speed_rpm  int[];
alter table tripdetail.trip_statistics add column if not exists acceleration_speed  json   ;                          
alter table tripdetail.trip_statistics add column if not exists abs_acceleration_speed  int;
alter table tripdetail.trip_statistics add column if not exists ord_acceleration_speed  int;
alter table tripdetail.trip_statistics add column if not exists nonzero_matrix_val_acceleration_speed int[];
alter table tripdetail.trip_statistics add column if not exists nonzero_matrix_val_brake_pedal_acceleration_speed  int[];
alter table tripdetail.trip_statistics add column if not exists num_val_acceleration_speed  int[];
alter table tripdetail.trip_statistics add column if not exists col_index_acceleration_speed  int[];

--**************
alter table tripdetail.trip_statistics alter column abs_rpm_torque  type bigint;
alter table tripdetail.trip_statistics alter column ord_rpm_torque  type bigint;
alter table tripdetail.trip_statistics alter column nonzero_matrix_val_rpm_torque type bigint[];
alter table tripdetail.trip_statistics alter column num_val_rpm_torque  type bigint[];
alter table tripdetail.trip_statistics alter column col_index_rpm_torque  type bigint[];
alter table tripdetail.trip_statistics alter column abs_speed_rpm  type bigint;
alter table tripdetail.trip_statistics alter column ord_speed_rpm  type bigint;
alter table tripdetail.trip_statistics alter column nonzero_matrix_val_speed_rpm  type bigint[];
alter table tripdetail.trip_statistics alter column num_val_speed_rpm  type bigint[];
alter table tripdetail.trip_statistics alter column col_index_speed_rpm  type bigint[];
alter table tripdetail.trip_statistics alter column abs_acceleration_speed  type bigint;
alter table tripdetail.trip_statistics alter column ord_acceleration_speed  type bigint;
alter table tripdetail.trip_statistics alter column nonzero_matrix_val_acceleration_speed type bigint[];
alter table tripdetail.trip_statistics alter column nonzero_matrix_val_brake_pedal_acceleration_speed  type bigint[];
alter table tripdetail.trip_statistics alter column num_val_acceleration_speed  type bigint[];
alter table tripdetail.trip_statistics alter column col_index_acceleration_speed  type bigint[];
alter table livefleet.livefleet_position_statistics add column if not exists driving_time int;


