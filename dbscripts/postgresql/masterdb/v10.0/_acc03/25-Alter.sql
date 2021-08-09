alter table master.accountpreference add column if not exists page_refresh_time int default 2;
--update master.accountpreference set page_refresh_time=2;
alter table master.accountpreference alter column page_refresh_time set not null ;

alter table master.report add column if not exists scheduled_report char(1) default 'N'; --not null default 2;
update master.report set scheduled_report='Y' where id in 
(select id from master.report where (lower(name) like '%trip report%'
 or lower(name) like '%fleet fuel report' or lower(name) like '%fuel deviation report%' 
 or lower(name) like '%fleet utilisation report%' or lower(name) like '%ecoscore%') 
 and lower(name) not like '%advance%'  );
 

CREATE TABLE if not exists master.vehiclemsgtriggertype --(master data)
(
	id int not null, 
	name varchar(50)
)	
TABLESPACE pg_default;

ALTER TABLE  master.vehiclemsgtriggertype  
    OWNER to pgdbmadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_vehiclemsgtriggertype_id' AND table_name='vehiclemsgtriggertype'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.vehiclemsgtriggertype  
			ADD CONSTRAINT pk_vehiclemsgtriggertype_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.driverauthequipment --(master data)
(
	id int not null, 
	name varchar(50)
)	
TABLESPACE pg_default;

ALTER TABLE  master.driverauthequipment  
    OWNER to pgdbmadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_driverauthequipment_id' AND table_name='driverauthequipment'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.driverauthequipment  
			ADD CONSTRAINT pk_driverauthequipment_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.telltale --(master data)
(
	id int not null, 
	name varchar(50)
)	
TABLESPACE pg_default;

ALTER TABLE  master.telltale  
    OWNER to pgdbmadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_telltale_id' AND table_name='telltale'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.telltale  
			ADD CONSTRAINT pk_telltale_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


CREATE TABLE if not exists master.telltalestate --(master data)
(
	id int not null, 
	name varchar(50)
)	
TABLESPACE pg_default;

ALTER TABLE  master.telltalestate  
    OWNER to pgdbmadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_telltalestate_id' AND table_name='telltalestate'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.telltalestate  
			ADD CONSTRAINT pk_telltalestate_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


alter table master.package alter column description type text;
alter table  master.notificationhistory  add column if not exists translation_id varchar(50);
alter table  master.notificationhistory  add column if not exists retry int;




--engine_type 
--vehicle_performance_type --enumtranslation
--kpi_type --enumtranslation

-- CREATE TABLE if not exists master.enginetyperef --(master data)
-- (
	-- id serial not null, 
	-- engine_type varchar(10) ,
	-- mapped_engine_type varchar(10)
-- )	
-- TABLESPACE pg_default;

-- ALTER TABLE  master.enginetyperef  
    -- OWNER to pgdbmadmin;
	
-- Do $$
-- begin
-- if not exists(
	-- SELECT 1 FROM information_schema.table_constraints 
	-- WHERE constraint_name='pk_enginetyperef_id' AND table_name='enginetyperef'
		-- and constraint_type='PRIMARY KEY')
-- then	
	-- begin
		-- ALTER TABLE  master.enginetyperef  
			-- ADD CONSTRAINT pk_enginetyperef_id PRIMARY KEY (id)
			-- USING INDEX TABLESPACE pg_default;
	-- end;
-- end if;
-- end;
-- $$;

-- CREATE TABLE if not exists master.enginetypeaxisref --(master data)
-- (
	-- index  int not null,
	-- engine_type_id  int not null,
	-- vehicle_performance_type  char(1) not null,
	-- axis_type  char(1) not null,
	-- axis_value  varchar(10) not null
-- )	
-- TABLESPACE pg_default;

-- ALTER TABLE  master.enginetypeaxisref  
    -- OWNER to pgdbmadmin;
	
	
	
-- -- Do $$
-- -- begin
-- -- if not exists(
	-- -- SELECT 1 FROM information_schema.table_constraints 
	-- -- WHERE constraint_name='pk_vehicleperformanceaxisref_id' AND table_name='vehicleperformanceaxisref'
		-- -- and constraint_type='PRIMARY KEY')
-- -- then	
	-- -- begin
		-- -- ALTER TABLE  master.vehicleperformanceaxisref  
			-- -- ADD CONSTRAINT pk_vehicleperformanceaxisref_id PRIMARY KEY (id)
			-- -- USING INDEX TABLESPACE pg_default;
	-- -- end;
-- -- end if;
-- -- end;
-- -- $$;

-- Do $$
-- begin
-- if not exists(
	-- SELECT 1 FROM information_schema.table_constraints 
	-- WHERE constraint_name='fk_enginetypeaxisref_enginetypeid_enginetyperef_id' AND table_name='enginetypeaxisref'
		-- and constraint_type='FOREIGN KEY')
-- then	
	-- begin
		-- ALTER TABLE  master.enginetypeaxisref 
			-- ADD CONSTRAINT fk_enginetypeaxisref_enginetypeid_enginetyperef_id FOREIGN KEY (engine_type_id) REFERENCES  master.enginetyperef (id);
			-- --USING INDEX TABLESPACE pg_default;
	-- end;
-- end if;
-- end;
-- $$;


-- CREATE TABLE if not exists master.enginetypeaxiskpiref --(master data)
-- (
	-- index  int not null,
	-- engine_type_id  int not null,
	-- vehicle_performance_type  char(1)  not null,
	-- veh_per_to_y_index  int not null,
	-- kpi_type  char(1) not null,
-- )	
-- TABLESPACE pg_default;

-- ALTER TABLE  master.enginetypeaxiskpiref  
    -- OWNER to pgdbmadmin;
	
-- -- Do $$
-- -- begin
-- -- if not exists(
	-- -- SELECT 1 FROM information_schema.table_constraints 
	-- -- WHERE constraint_name='pk_enginetypeaxiskpiref_id' AND table_name='enginetypeaxiskpiref'
		-- -- and constraint_type='PRIMARY KEY')
-- -- then	
	-- -- begin
		-- -- ALTER TABLE  master.enginetypeaxiskpiref  
			-- -- ADD CONSTRAINT pk_enginetypeaxiskpiref_id PRIMARY KEY (id)
			-- -- USING INDEX TABLESPACE pg_default;
	-- -- end;
-- -- end if;
-- -- end;
-- -- $$;


-- Do $$
-- begin
-- if not exists(
	-- SELECT 1 FROM information_schema.table_constraints 
	-- WHERE constraint_name='fk_enginetypeaxiskpiref_enginetypeid_enginetyperef_id' AND table_name='enginetypeaxiskpiref'
		-- and constraint_type='FOREIGN KEY')
-- then	
	-- begin
		-- ALTER TABLE  master.enginetypeaxiskpiref 
			-- ADD CONSTRAINT fk_enginetypeaxiskpiref_enginetypeid_enginetyperef_id FOREIGN KEY (engine_type_id) REFERENCES  master.enginetyperef (id);
			-- --USING INDEX TABLESPACE pg_default;
	-- end;
-- end if;
-- end;
-- $$;


-- drop table master.enginetyperef;
-- drop table master.enginetypeaxisref ;
-- drop table master.enginetypeaxiskpiref;

CREATE TABLE if not exists master.vehicleperformancetemplate --(master data)
(
   id  serial not null,
   --vehicle_performance_type char(1) not null,
   --engine_type varchar[]  not null       ,
   engine_type varchar(10)        ,
   template varchar(100) not null ,
   --key varchar(100) not null ,  	
   is_default boolean          
)	
TABLESPACE pg_default;

ALTER TABLE  master.vehicleperformancetemplate  
    OWNER to pgdbmadmin;
	
Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_vehicleperformancetemplate_id' AND table_name='vehicleperformancetemplate'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.vehicleperformancetemplate  
			ADD CONSTRAINT pk_vehicleperformancetemplate_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.performancematrix --(master data)
(
   id serial not null,
   --veh_per_template_id int,
   template varchar(100) not null ,
   vehicle_performance_type char(1) not null,
   index int not null,                        
   range varchar(8) not null,        
   row varchar[] not null             
)	
TABLESPACE pg_default;

ALTER TABLE  master.performancematrix  
    OWNER to pgdbmadmin;
	
Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_performancematrix_id' AND table_name='performancematrix'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.performancematrix  
			ADD CONSTRAINT pk_performancematrix_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

-- Do $$
-- begin
-- if not exists(
	-- SELECT 1 FROM information_schema.table_constraints 
	-- WHERE constraint_name='fk_performancematrix_vehpertemplateid_vehpertemplate_id' AND table_name='performancematrix'
		-- and constraint_type='FOREIGN KEY')
-- then	
	-- begin
		-- ALTER TABLE  master.performancematrix 
			-- ADD CONSTRAINT fk_performancematrix_vehpertemplateid_vehpertemplate_id FOREIGN KEY (veh_per_template_id) REFERENCES  master.vehicleperformancetemplate (id);
			-- --USING INDEX TABLESPACE pg_default;
	-- end;
-- end if;
-- end;
-- $$;



--*****************
CREATE TABLE if not exists master.performancekpiranges --(master data)
(
	id   serial not null,
	vehicle_performance_type  char(1) not null,
	index  int ,           
	kpi  char(1),
	lower_val  decimal,
	upper_val  decimal
)	
TABLESPACE pg_default;

ALTER TABLE  master.performancekpiranges  
    OWNER to pgdbmadmin;
	
Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_performancekpiranges_id' AND table_name='performancekpiranges'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.performancekpiranges  
			ADD CONSTRAINT pk_performancekpiranges_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

alter table master.account alter column salutation drop not null;