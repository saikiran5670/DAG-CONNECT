CREATE SCHEMA  mileage 
    AUTHORIZATION pgdbmadmin;
CREATE TABLE if not exists mileage.vehiclemileage
(
	id serial not null, 
	--vehicle_id  int,
	vin varchar(17) not null,
	modified_at  bigint,
	evt_timestamp  bigint,
	odo_mileage  decimal,
	odo_distance  decimal,
	real_distance  decimal,
	gps_distance  decimal	
	
)
TABLESPACE pg_default;

ALTER TABLE  mileage.vehiclemileage 
    OWNER to pgdbmadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_vehiclemileage_id' AND table_name='vehiclemileage'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  mileage.vehiclemileage 
			ADD CONSTRAINT pk_vehiclemileage_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

-- Do $$
-- begin
-- if not exists(
	-- SELECT 1 FROM information_schema.table_constraints 
	-- WHERE constraint_name='fk_vehiclemileage_vehicleid_vehicle_id' AND table_name='vehiclemileage'
		-- and constraint_type='FOREIGN KEY')
-- then	
	-- begin
		-- ALTER TABLE  mileage.vehiclemileage 
			-- ADD CONSTRAINT fk_vehiclemileage_vehicleid_vehicle_id FOREIGN KEY (vehicle_id) REFERENCES  master.vehicle (id);
			-- --USING INDEX TABLESPACE pg_default;
	-- end;
-- end if;
-- end;
-- $$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='uk_vehiclemileage_vin' AND table_name='vehiclemileage'
		and constraint_type='UNIQUE')
then	
	begin
		ALTER TABLE  mileage.vehiclemileage 
			ADD CONSTRAINT uk_vehiclemileage_vin UNIQUE  (vin)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


