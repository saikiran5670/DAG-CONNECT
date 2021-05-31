ALTER TABLE livefleet.livefleet_trip_driver_activity
    ALTER COLUMN trip_id DROP NOT NULL;
ALTER TABLE livefleet.livefleet_trip_driver_activity
    ALTER COLUMN trip_start_time_stamp DROP NOT NULL;
ALTER TABLE livefleet.livefleet_trip_driver_activity
    ALTER COLUMN vin DROP NOT NULL;
ALTER TABLE livefleet.livefleet_trip_driver_activity
    ALTER COLUMN code type char(1) ;


--alter table tripdetail.trip_statistics alter column average_traffic_classification type decimal;
--alter table tripdetail.trip_statistics alter column idling_consumption type decimal;
alter table tripdetail.trip_statistics add column if not exists msg_gross_weight_combinition int; 
alter table tripdetail.trip_statistics rename column message_inserted_into_hbase_timestamp to etl_trip_record_insertion_time ;
alter table tripdetail.trip_statistics add column if not exists no_of_total_index_message int; 
alter table tripdetail.trip_statistics alter column average_traffic_classification type decimal USING average_traffic_classification::numeric; 

--vehicle data mart

CREATE TABLE if not exists master.geolocationaddress  
(
	id serial not null, 
	longitude double precision not null, 
	latitude double precision not null, 
	address varchar(150) not null, 
	created_at bigint not null, 
	modified_at bigint
)	
TABLESPACE pg_default;

ALTER TABLE  master.geolocationaddress
    OWNER to pgdbadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_geolocationaddress_id' AND table_name='geolocationaddress'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.geolocationaddress 
			ADD CONSTRAINT pk_geolocationaddress_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;
