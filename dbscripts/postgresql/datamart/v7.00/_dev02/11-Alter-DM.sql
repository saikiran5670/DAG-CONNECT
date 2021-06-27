CREATE TABLE if not exists tripdetail.tripalert 
(
	id  serial not null,
	trip_id  varchar(36) not null,
	vin  varchar(17) not null,
	category_type  char(1) not null,
	type  char(1) not null,
	name  varchar(50) ,
	alert_id  int not null,
	param_filter_distance_threshold_value  decimal,
	param_filter_distance_threshold_value_unit_type  char(1),
	param_filter_distance_value_at_alert_time  decimal,
	latitude  double precision,
	longitude  double precision,
	alert_generated_time  bigint,
	message_timestamp  bigint,
	created_at  bigint,
	modified_at  bigint
)	
TABLESPACE pg_default;

ALTER TABLE  tripdetail.tripalert 
    OWNER to pgadmin;
	
Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_tripalert_id' AND table_name='tripalert'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  tripdetail.tripalert  
			ADD CONSTRAINT pk_tripalert_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;
	


CREATE TABLE if not exists tripdetail.ecoscoredata 
(
	id  serial not null,
	trip_id  varchar(50) not null,
	vin  varchar(17) not null,
	start_time  bigint not null,
	end_time  bigint not null,
	driver1_id  varchar(19) not null,
	driver2_id  varchar(19) ,
	etl_trip_distance  integer ,
	dpa_braking_score  int,
	dpa_braking_count  int,
	dpa_anticipation_score  int,
	dpa_anticipation_count  int,
	gross_weight_combination  decimal,
	used_fuel  bigint,
	pto_duration  int , 
	idle_duration int ,
	heavy_throttle_pedal_duration  int,
	start_fuel  bigint,
	stop_fuel  bigint
)	
TABLESPACE pg_default;

ALTER TABLE  tripdetail.ecoscoredata 
    OWNER to pgadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_ecoscoredata_id' AND table_name='ecoscoredata'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  tripdetail.ecoscoredata  
			ADD CONSTRAINT pk_ecoscoredata_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

ALTER TABLE tripdetail.ecoscoredata ADD CONSTRAINT uk_ecoscoredata_tripid UNIQUE (trip_id);

