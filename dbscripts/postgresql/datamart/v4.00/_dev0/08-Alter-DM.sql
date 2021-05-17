CREATE TABLE if not exists tripdetail.tripalerts
(
	id serial not null, 
	trip_id  varchar(36) not null, 
	vin  varchar(17) not null, 
	category_type  char(1) not null, 
	type  char(1) not null, 
	name  varchar(50) not null, 
	param_urgency_level_type  char(1), 
	param_urgency_level_threshold_value  decimal, 
	param_urgency_level_threshold_value_unit_type  char(1), 
	param_urgency_level_value_at_alert_time  decimal, 
	param_landmark_type  char(1), 
	param_landmark_name varchar(50), 
	param_landmark_threshold_value  decimal, 
	param_landmark_threshold_value_unit_type  char(1), 
	param_landmark_value_at_alert_time  decimal, 
	param_filter_duration_threshold_value  decimal, 
	param_filter_duration_threshold_value_unit_type  char(1), 
	param_filter_duration_value_at_alert_time  decimal, 
	param_filter_distance_threshold_value  decimal, 
	param_filter_distance_threshold_value_unit_type  char(1), 
	param_filter_distance_value_at_alert_time  decimal, 
	param_filter_occurrences_threshold_value  decimal, 
	param_filter_occurrences_value_at_alert_time  decimal, 
	param_filter_day_threshold_value  bit(7), 
	param_filter_from_time_threshold_value  bigint, 
	param_filter_to_time_threshold_value  bigint, 
	param_filter_from_time_value_at_alert_time  bigint, 
	param_filter_to_time_value_at_alert_time bigint, 
	param_filter_landmark_type  char(1), 
	param_filter_landmark_name varchar(50), 
	param_filter_landmark_position_type char(1), 
	param_filter_landmark_threshold_value  decimal, 
	param_filter_landmark_threshold_value_unit_type  char(1), 
	param_filter_landmark_value_at_alert_time  decimal, 
	latitude  decimal, 
	longitude  decimal, 
	alert_generated_time  bigint not null, 
	event_timestamp  bigint not null, 
	created_at  bigint not null, 
	modified_at  bigint
	
)
TABLESPACE pg_default;

ALTER TABLE  tripdetail.tripalerts 
    OWNER to pgadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_tripalerts_id' AND table_name='tripalerts'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  tripdetail.tripalerts 
			ADD CONSTRAINT pk_tripalerts_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Alter table master.vehicle add column if not exists created_at bigint;  
Alter table master.vehicle add column if not exists modified_at bigint;  

update master.vehicle set created_at =(select extract(epoch from now()- interval '3 days'  ) * 1000);

Alter table master.vehicle alter column created_at set not null;  

Alter table livefleet.livefleet_trip_driver_activity add column if not exists is_driver1 boolean;  

update livefleet.livefleet_trip_driver_activity  
set is_driver1=true 
where trip_id in (select trip_id from tripdetail.trip_statistics where driver1_id is not null) ;

update livefleet.livefleet_trip_driver_activity  
set is_driver1=false
where trip_id in (select trip_id from tripdetail.trip_statistics where driver2_id is not null) ;

update livefleet.livefleet_trip_driver_activity  
set is_driver1=false
where is_driver1 is null;

Alter table livefleet.livefleet_trip_driver_activity alter column is_driver1 set not null;

ALTER TABLE master.vehicle ALTER COLUMN registration_no TYPE varchar(50) ; --from 25 to 50
ALTER TABLE master.vehicle ALTER COLUMN vin TYPE varchar(17) ; --from 50 to 17

