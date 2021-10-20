--*****************on Dev1 only 04-Oct-2021
alter table tripdetail.tripalert alter column trip_id drop not null;
alter table tripdetail.tripalert alter column alert_id drop not null;

CREATE TABLE if not exists tripdetail.tripalertotaconfigparam 
(
	id  serial not null,
	trip_alert_id int not null,
	vin varchar(17) not null,
	campaign uuid not null,
	baseline uuid not null,
	status_code char(1) not null,
	status varchar(100) not null,
	campaign_id varchar(100) not null,
	subject varchar(100) not null,
	time_stamp bigint not null
)	
TABLESPACE pg_default;

ALTER TABLE  livefleet.index_message_data 
    OWNER to pgadmin;
	
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

alter table tripdetail.tripalertotaconfigparam alter column status_code type int USING status_code::integer;

ALTER TABLE master.driver DROP CONSTRAINT uk_driver_driverid;

ALTER TABLE master.driver
    ADD CONSTRAINT uk_driver_driveridorgid UNIQUE (organization_id,driver_id);

update livefleet.livefleet_current_trip_statistics
set vehicle_driving_status_type='S'
--select * from livefleet.livefleet_current_trip_statistics
where vehicle_driving_status_type is null or vehicle_driving_status_type='';

update livefleet.livefleet_current_trip_statistics
set vehicle_health_status_type='N'
--select * from livefleet.livefleet_current_trip_statistics
where vehicle_health_status_type is null or vehicle_health_status_type='';
