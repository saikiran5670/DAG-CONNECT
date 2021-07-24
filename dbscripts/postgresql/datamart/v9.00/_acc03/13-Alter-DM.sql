
alter table tripdetail.tripalert drop column param_filter_distance_threshold_value;
alter table tripdetail.tripalert drop column param_filter_distance_threshold_value_unit_type;
alter table tripdetail.tripalert drop column param_filter_distance_value_at_alert_time;
alter table tripdetail.tripalert rename column  message_timestamp to processed_message_time_stamp;
alter table tripdetail.tripalert add column if not exists urgency_level_type  char(1);

alter table tripdetail.ecoscoredata drop column stop_fuel;
alter table tripdetail.ecoscoredata add column if not exists acceleration_time bigint;
drop table tripdetail.tripalerts;

CREATE TABLE if not exists tripdetail.tripalertdaytimeconfigparam 
(
	id  serial not null,
	trip_alert_id  int not null,
	from_date_threshold_value  bigint,
	to_date_threshold_value  bigint,
	timeperiod_type  char(1) not null,
	day_threshold_value  bit(7),
	from_daytime_threshold_value  bigint,
	to_daytime_threshold_value  bigint,
	date_breached_value  bigint not null,
	day_breached_value  bit(7) not null,
	daytime_breached_value  bigint	not null
)	
TABLESPACE pg_default;

ALTER TABLE  tripdetail.tripalertdaytimeconfigparam 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_tripalertdaytimeconfigparam_id' AND table_name='tripalertdaytimeconfigparam'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  tripdetail.tripalertdaytimeconfigparam  
			ADD CONSTRAINT pk_tripalertdaytimeconfigparam_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_tripalertdaytimeconfigparam_tripalertid_tripalert_id' AND table_name='tripalertdaytimeconfigparam'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  tripdetail.tripalertdaytimeconfigparam 
			ADD CONSTRAINT fk_tripalertdaytimeconfigparam_tripalertid_tripalert_id FOREIGN KEY (trip_alert_id) REFERENCES  tripdetail.tripalert (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists tripdetail.tripalertlandmarkconfigparam 
(
	id  serial not null,
	trip_alert_id  int not null,
	landmark_type  char(1) not null,
	landmark_id int not null,
	landmark_name varchar(50),
	landmark_position_type char(1) ,
	landmark_threshold_value  decimal not null,
	landmark_threshold_value_unit_type  char(1),
	landmark_breached_value  decimal	
)	
TABLESPACE pg_default;

ALTER TABLE  tripdetail.tripalertlandmarkconfigparam 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_tripalertlandmarkconfigparam_id' AND table_name='tripalertlandmarkconfigparam'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  tripdetail.tripalertlandmarkconfigparam  
			ADD CONSTRAINT pk_tripalertlandmarkconfigparam_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_tripalertlandmarkconfigparam_tripalertid_tripalert_id' AND table_name='tripalertlandmarkconfigparam'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  tripdetail.tripalertlandmarkconfigparam 
			ADD CONSTRAINT fk_tripalertlandmarkconfigparam_tripalertid_tripalert_id FOREIGN KEY (trip_alert_id) REFERENCES  tripdetail.tripalert (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists tripdetail.tripalertgenconfigparam 
(
	id  serial not null,
	trip_alert_id  int not null,
	param_type  char(1) not null,
	threshold_value  decimal not null,
	threshold_value_unit_type  char(1) not null,
	breached_value  decimal not null
)	
TABLESPACE pg_default;

ALTER TABLE  tripdetail.tripalertgenconfigparam 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_tripalertgenconfigparam_id' AND table_name='tripalertgenconfigparam'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  tripdetail.tripalertgenconfigparam  
			ADD CONSTRAINT pk_tripalertgenconfigparam_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_tripalertgenconfigparam_tripalertid_tripalert_id' AND table_name='tripalertgenconfigparam'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  tripdetail.tripalertgenconfigparam 
			ADD CONSTRAINT fk_tripalertgenconfigparam_tripalertid_tripalert_id FOREIGN KEY (trip_alert_id) REFERENCES  tripdetail.tripalert (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

alter table tripdetail.ecoscoredata rename column gross_weight_combination to gross_weight_combination_total ;
alter table tripdetail.ecoscoredata add column if not exists  gross_weight_combination_count  decimal;
alter table tripdetail.ecoscoredata add column if not exists  trip_acceleration_time decimal;
alter table tripdetail.ecoscoredata  drop column if exists stop_fuel ;
alter table tripdetail.ecoscoredata  drop column if exists start_fuel ;

alter table tripdetail.ecoscoredata add column if not exists is_ongoing_trip boolean default false;
alter table tripdetail.trip_statistics alter column is_ongoing_trip  set default false;
alter table livefleet.livefleet_warning_statistics add column if not exists message_type int ;