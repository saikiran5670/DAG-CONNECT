Alter table master.dataattribute add column if not exists type char(1) not null ;
Alter table master.dataattribute add column if not exists key varchar(250) not null;
ALTER TABLE master.dataattribute ALTER COLUMN id TYPE bigint ;

Alter table master.dataattributeset ALTER COLUMN is_exlusive TYPE char(1) ;
Alter table master.dataattributeset add column if not exists created_at bigint not null default extract(epoch from now()) * 1000; 
Alter table master.dataattributeset add column if not exists created_by int not null;
Alter table master.dataattributeset add column if not exists modified_at bigint ;
Alter table master.dataattributeset add column if not exists modified_by int ;

Alter table master.feature add column if not exists key varchar(250);
Alter table master.feature drop column if exists description;
ALTER TABLE master.feature ALTER COLUMN id TYPE bigint ;

Alter table master.featureset drop column if exists is_custom_feature_set;
Alter table master.featureset add column if not exists created_at bigint default extract(epoch from now()) * 1000; 
Alter table master.featureset add column if not exists created_by int ;
Alter table master.featureset add column if not exists modified_at bigint ;
Alter table master.featureset add column if not exists modified_by int ;

--Report
CREATE TABLE if not exists master.report 
(
	id serial not null, 
	name varchar (100) not null
)
TABLESPACE pg_default;

ALTER TABLE  master.report 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_report_id' AND table_name='report'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.report 
			ADD CONSTRAINT pk_report_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--Reportdef
CREATE TABLE if not exists master.reportdef 
(
	report_id int not null, 
	data_attribute_id int not null
)
TABLESPACE pg_default;

ALTER TABLE  master.reportdef 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_reportdef_reportid_dataattributeid' AND table_name='reportdef'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.reportdef 
			ADD CONSTRAINT pk_reportdef_reportid_dataattributeid PRIMARY KEY (report_id,data_attribute_id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_reportdef_reportid_report_id' AND table_name='reportdef'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.reportdef 
			ADD CONSTRAINT fk_reportdef_reportid_report_id FOREIGN KEY (report_id) REFERENCES  master.report (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_reportdef_dataattributeid_dataattribute_id' AND table_name='reportdef'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.reportdef 
			ADD CONSTRAINT fk_reportdef_dataattributeid_dataattribute_id FOREIGN KEY (data_attribute_id) REFERENCES  master.dataattribute (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--
CREATE TABLE if not exists master.reportpreference 
(
	id serial not null,	
	account_id int not null,
	report_id int not null,
	data_attribute_id int not null,
	is_exlusive char(1) not null
)
TABLESPACE pg_default;

ALTER TABLE  master.reportpreference 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_reportpreference_id' AND table_name='reportpreference'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.reportpreference 
			ADD CONSTRAINT pk_reportpreference_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_reportpreference_accountid_account_id' AND table_name='reportpreference'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.reportpreference
			ADD CONSTRAINT fk_reportpreference_accountid_account_id FOREIGN KEY (account_id) REFERENCES  master.account (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_reportpreference_reportid_report_id' AND table_name='reportpreference'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.reportpreference
			ADD CONSTRAINT fk_reportpreference_reportid_report_id FOREIGN KEY (report_id) REFERENCES  master.report (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_reportpreference_dataattributeid_dataattribute_id' AND table_name='reportpreference'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.reportpreference 
			ADD CONSTRAINT fk_reportpreference_dataattributeid_dataattribute_id FOREIGN KEY (data_attribute_id) REFERENCES  master.dataattribute (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Alter table master.menu drop column if exists description;
Alter table master.menu add column if not exists seq_no varchar(250);
Alter table master.menu add column if not exists key varchar(250);
Alter table master.menu add column if not exists url varchar(250);

Alter table master.vehicleproperties drop constraint if exists fk_vehicleproperty_vehicleid_vehicle_id;
Alter table master.vehicleproperties drop column if exists vehicle_id;

Alter table master.vehicle add column if not exists vehicle_property_id int;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_vehicle_vehiclepropertyid_vehicleproperties_id' AND table_name='vehicle'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.vehicle 
			ADD CONSTRAINT fk_vehicle_vehiclepropertyid_vehicleproperties_id FOREIGN KEY (vehicle_property_id) REFERENCES  master.vehicleproperties (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Alter table master.account add column if not exists created_at bigint not null default extract(epoch from now()) * 1000;  
Alter table master.group add column if not exists created_at bigint not null default extract(epoch from now()) * 1000; 
Alter table master.role add column if not exists created_at bigint not null default extract(epoch from now()) * 1000; 
Alter table master.vehicle add column if not exists created_at bigint not null default extract(epoch from now()) * 1000; 


Alter table master.package drop column if exists service_subscriber_type;
Alter table master.package drop column if exists long_description;
Alter table master.package drop column if exists is_active;

Alter table master.account drop column if exists dob;
Alter table master.account add column if not exists driver_id Varchar(16); 
Alter table master.account add column if not exists preference_id int  ;
Alter table master.account add column if not exists blob_id int ;
Alter table master.organization add column if not exists preference_id int ;
Alter table master.accountpreference drop column if exists ref_id;
Alter table master.accountblob drop column if exists image_url;
Alter table master.accountblob drop column if exists account_id;
Alter table master.accountblob drop column if exists image_url;
Alter table master.accountblob add column if not exists image bytea ;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_account_preferenceid_accountpreference_id' AND table_name='account'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.account 
			ADD CONSTRAINT fk_account_preferenceid_accountpreference_id FOREIGN KEY (preference_id) REFERENCES  master.accountpreference (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_account_blobid_accountblob_id' AND table_name='account'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.account 
			ADD CONSTRAINT fk_account_blobid_accountblob_id FOREIGN KEY (blob_id) REFERENCES  master.accountblob (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_organization_preferenceid_accountpreference_id' AND table_name='organization'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.organization 
			ADD CONSTRAINT fk_organization_preferenceid_accountpreference_id FOREIGN KEY (preference_id) REFERENCES  master.accountpreference (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Alter table master.vehicleproperties drop column if exists series;
Alter table master.vehicleproperties add column if not exists series_id Varchar(50); 
Alter table master.vehicleproperties add column if not exists series_vehicle_Range Varchar(50); 
Alter table master.vehicleproperties ADD column if not exists model_year int;
Alter table master.vehicleproperties drop column if exists chasis_fuel_tank_number ;
Alter table master.vehicleproperties drop column if exists chasis_fuel_tank_volume;
--Alter table master.vehicleproperties add column if not exists chasis_fuel_tank_numbers int;
--Alter table master.vehicleproperties add column if not exists chasis_fuel_tank1_volume int;
--Alter table master.vehicleproperties add column if not exists chasis_fuel_tank2_volume int;
Alter table master.vehicleproperties drop column if exists driveline_front_axle_position  ;
Alter table master.vehicleproperties drop column if exists driveline_front_axle_load  ;
Alter table master.vehicleproperties drop column if exists driveline_rear_axle_position  ;
Alter table master.vehicleproperties drop column if exists driveline_rear_axle_load  ;
Alter table master.vehicleproperties drop column if exists driveline_rear_axle_ratio  ;
Alter table master.vehicleproperties add column if not exists cabin_type Varchar(50); 
Alter table master.vehicleproperties add column if not exists cabin_roofspoiler Varchar(50); 
Alter table master.vehicleproperties add column if not exists electronic_control_unit_type Varchar(50); 
Alter table master.vehicleproperties add column if not exists electronic_control_unit_name Varchar(50); 
Alter table master.vehicleproperties add column if not exists weight_type Varchar(50); 
Alter table master.vehicleproperties drop column if exists cabin_color_id   ;
Alter table master.vehicleproperties drop column if exists cabin_color_value  ;
Alter table master.vehicleproperties drop column if exists registration_date   ;
Alter table master.vehicleproperties drop column if exists is_chasis_side_skirts;
Alter table master.vehicleproperties drop column if exists is_chasis_side_collar;
Alter table master.vehicleproperties add column if not exists chasis_side_skirts Varchar(50); 
Alter table master.vehicleproperties add column if not exists chasis_side_collar Varchar(50); 
Alter table master.vehicleproperties alter column driveline_wheel_base type decimal USING driveline_wheel_base::decimal;
Alter table master.vehicleproperties drop column if exists widht;
Alter table master.vehicleproperties add column if not exists width int; 

Alter table master.vehicle drop column if exists model;
Alter table master.vehicle ADD column if not exists model_ID varchar(50);
Alter table master.vehicle alter column type type varchar(50);



CREATE TABLE if not exists master.vehicleaxleproperties 
(
	id serial not null,
	vehicle_id int not null,
	axle_type char(1),
	position int,
	type varchar(50),
	springs varchar(50) ,
	load int,
	ratio int 
)
TABLESPACE pg_default;

ALTER TABLE  master.vehicleaxleproperties 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_vehicleaxleproperties_id' AND table_name='vehicleaxleproperties'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.vehicleaxleproperties 
			ADD CONSTRAINT pk_vehicleaxleproperties_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_vehicleaxleproperties_vehicleid_vehicle_id' AND table_name='vehicleaxleproperties'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.vehicleaxleproperties 
			ADD CONSTRAINT fk_vehicleaxleproperties_vehicleid_vehicle_id FOREIGN KEY (vehicle_id) REFERENCES  master.vehicle (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;



ALTER SCHEMA logs RENAME TO auditlog;
ALTER SCHEMA auditlog OWNER TO pgdbadmin;


--advice
CREATE TABLE if not exists master.advice 
(
	id serial not null, 
	description varchar(500) not null,
	key varchar(100) not null
)
TABLESPACE pg_default;

ALTER TABLE  master.advice 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_advice_id' AND table_name='advice'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.advice 
			ADD CONSTRAINT pk_advice_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--warning
CREATE TABLE if not exists master.warning 
(
	id serial not null, 
	class  varchar(50) not null,
	number  varchar(100) not null,
	description  varchar(500) not null,
	key  varchar(100) not null,
	advice_id  int
)
TABLESPACE pg_default;

ALTER TABLE  master.warning 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_warning_id' AND table_name='warning'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.warning 
			ADD CONSTRAINT pk_warning_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_warning_adviceid_advice_id' AND table_name='warning'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.warning 
			ADD CONSTRAINT fk_warning_adviceid_advice_id FOREIGN KEY (advice_id) REFERENCES  master.advice (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--Driver
CREATE TABLE if not exists master.driver 
(
	id serial not null, 
	organization_id int not null,
	driver_id_ext varchar(19) not null,
	salutation varchar(5) not null,
	first_name varchar(30) not null,
	last_name varchar(20),
	dob bigint,
	status	char(1)not null,
	is_active boolean not null default true
)
TABLESPACE pg_default;

ALTER TABLE  master.driver 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_driver_id' AND table_name='driver'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.driver 
			ADD CONSTRAINT pk_driver_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_driver_organizationid_organization_id' AND table_name='driver'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.driver 
			ADD CONSTRAINT fk_driver_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--driveroptinoptout
CREATE TABLE if not exists  master.driveroptinoptout 
(
	id serial not null,
	organization_id int not null,
	driver_id int not null,
	status char(1) not null,
	modified_at bigint not null,
	modified_by int not null
	
)
TABLESPACE pg_default;

ALTER TABLE  master.driveroptinoptout 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_driveroptinoptout_id' AND table_name='driveroptinoptout'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.driveroptinoptout 
			ADD CONSTRAINT pk_driveroptinoptout_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_driveroptinoptout_organizationid_organization_id' AND table_name='driveroptinoptout'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.driveroptinoptout 
			ADD CONSTRAINT fk_driveroptinoptout_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_driveroptinoptout_driverid_driver_id' AND table_name='driveroptinoptout'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.driveroptinoptout 
			ADD CONSTRAINT fk_driveroptinoptout_driverid_driver_id FOREIGN KEY (driver_id) REFERENCES  master.driver (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Alter table master.feature add column if not exists level int not null;
Alter table master.role add column if not exists level int not null;

Alter table master.vehicleproperties add column if not exists type_id Varchar(50); 
Alter table master.dataattributeset add column if not exists is_active boolean default true;


CREATE TABLE if not exists  master.vehiclefueltankproperties
(
	id serial not null,
	vehicle_id int not null,
	chasis_fuel_tank_number int,
	chasis_fuel_tank_volume int
)
TABLESPACE pg_default;

ALTER TABLE  master.vehiclefueltankproperties 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_vehiclefueltankproperties_id' AND table_name='vehiclefueltankproperties'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.vehiclefueltankproperties 
			ADD CONSTRAINT pk_vehiclefueltankproperties_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_vehiclefueltankproperties_vehicleid_vehicle_id' AND table_name='vehiclefueltankproperties'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.vehiclefueltankproperties 
			ADD CONSTRAINT fk_vehiclefueltankproperties_vehicleid_vehicle_id FOREIGN KEY (vehicle_id) REFERENCES  master.vehicle (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


--translatedhistory
CREATE TABLE if not exists master.translationupload 
(
	id serial not null, 
	file_name varchar(50) not null,
	description varchar(250),
	file_size int,
	success_count int not null,
	failure_count int not null,
	created_at bigint not null,
	craeted_by int not null
)
TABLESPACE pg_default;

ALTER TABLE  master.translationupload 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_translationupload_id' AND table_name='translationupload'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.translationupload 
			ADD CONSTRAINT pk_translationupload_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

-- --orgconfiguration
-- CREATE TABLE if not exists master.orgconfiguration 
-- (
	-- id serial not null, 
	-- organization_id int not null,
	-- vehicle_default_status char (1),
	-- driver_default_status char (1) 
	
-- )
-- TABLESPACE pg_default;

-- ALTER TABLE  master.orgconfiguration 
    -- OWNER to pgdbadmin;

-- Do $$
-- begin
-- if not exists(
	-- SELECT 1 FROM information_schema.table_constraints 
	-- WHERE constraint_name='pk_orgconfiguration_id' AND table_name='orgconfiguration'
		-- and constraint_type='PRIMARY KEY')
-- then	
	-- begin
		-- ALTER TABLE  master.orgconfiguration 
			-- ADD CONSTRAINT pk_orgconfiguration_id PRIMARY KEY (id)
			-- USING INDEX TABLESPACE pg_default;
	-- end;
-- end if;
-- end;
-- $$;

-- Do $$
-- begin
-- if not exists(
	-- SELECT 1 FROM information_schema.table_constraints 
	-- WHERE constraint_name='fk_orgconfiguration_organizationid_organization_id' AND table_name='orgconfiguration'
		-- and constraint_type='FOREIGN KEY')
-- then	
	-- begin
		-- ALTER TABLE  master.orgconfiguration 
			-- ADD CONSTRAINT fk_orgconfiguration_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			-- --USING INDEX TABLESPACE pg_default;
	-- end;
-- end if;
-- end;
-- $$;

--oem
CREATE TABLE if not exists master.oem 
(
	id serial not null, 
	name varchar(50) not null, 
	code varchar(25), 
	vin_prefix varchar(5) not null, 
	oem_organisation_id int
	
)
TABLESPACE pg_default;

ALTER TABLE  master.oem 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_oem_id' AND table_name='oem'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.oem 
			ADD CONSTRAINT pk_oem_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Alter table master.vehicle add column if not exists oem_id int; 
Alter table master.vehicle add column if not exists oem_organisation_id int; 


Alter table master.driver add column if not exists opt_in char(1) not null;
Alter table master.driver add column if not exists modified_at bigint ;
Alter table master.driver add column if not exists modified_by int ;
Alter table master.driver add column if not exists created_at bigint not null default extract(epoch from now()) * 1000; 


Alter table master.orgrelationship add column if not exists code varchar(50);
Alter table master.orgrelationshipmapping drop column if  exists organization_id ;
Alter table master.orgrelationshipmapping drop column if  exists type ;
Alter table master.orgrelationshipmapping add column if not exists owner_org_id int not null;
Alter table master.orgrelationshipmapping add column if not exists created_org_id int not null;
Alter table master.orgrelationshipmapping add column if not exists target_org_id  int not null;
Alter table master.orgrelationshipmapping add column if not exists start_date  bigint not null;
Alter table master.orgrelationshipmapping add column if not exists end_date  bigint;
Alter table master.orgrelationshipmapping add column if not exists allow_chain_rel_to_target_org boolean default false;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_orgrelationshipmapping_ownerorgid_organization_id' AND table_name='orgrelationshipmapping'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.orgrelationshipmapping 
			ADD CONSTRAINT fk_orgrelationshipmapping_ownerorgid_organization_id FOREIGN KEY (owner_org_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_orgrelationshipmapping_createdorgid_organization_id' AND table_name='orgrelationshipmapping'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.orgrelationshipmapping 
			ADD CONSTRAINT fk_orgrelationshipmapping_createdorgid_organization_id FOREIGN KEY (created_org_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_orgrelationshipmapping_targetorgid_organization_id' AND table_name='orgrelationshipmapping'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.orgrelationshipmapping 
			ADD CONSTRAINT fk_orgrelationshipmapping_targetorgid_organization_id FOREIGN KEY (target_org_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

 
 drop table master.PackageSubscription;
 drop table master.subscribedpackagevehicle;
 
Alter table master.package drop column if  exists code ;
Alter table master.package add column if not exists packagecode varchar(20) not null;
Alter table master.package drop column if  exists short_description ;
Alter table master.package add column if not exists description varchar(100);
Alter table master.package add column if not exists is_active boolean not null default true;
Alter table master.package drop column if  exists  is_default;
Alter table master.package drop column if  exists start_date ;
Alter table master.package drop column if  exists end_date ;

Alter table master.subscription drop column if  exists order_id ;
Alter table master.subscription drop column if  exists order_date ;
Alter table master.subscription add column if not exists subscription_id varchar(50) not null;
Alter table master.subscription add column if not exists type char(1) not null;
Alter table master.subscription add column if not exists package_code varchar(20) not null;
Alter table master.subscription add column if not exists package_id int not null;
Alter table master.subscription add column if not exists vehicle_id int;
Alter table master.subscription add column if not exists subscription_start_date bigint not null;
Alter table master.subscription add column if not exists subscription_end_date bigint;
Alter table master.subscription add column if not exists is_active boolean not null default true;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_subscription_packageid_package_id' AND table_name='subscription'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.subscription 
			ADD CONSTRAINT fk_subscription_packageid_package_id FOREIGN KEY (package_id) REFERENCES  master.package (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_subscription_vehicleid_vehicle_id' AND table_name='subscription'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.subscription 
			ADD CONSTRAINT fk_subscription_vehicleid_vehicle_id FOREIGN KEY (vehicle_id) REFERENCES  master.vehicle (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Alter table master.organization add column if not exists vehicle_default_opt_in char(1);
Alter table master.organization add column if not exists driver_default_opt_in char(1);
Alter table master.organization drop column if  exists optout_status;
Alter table master.organization drop column if  exists optout_status_changed_date;

Alter table master.vehicle add column if not exists opt_in char(1);
Alter table master.vehicle add column if not exists modified_at bigint;
Alter table master.vehicle add column if not exists is_ota boolean not null default false;
Alter table master.vehicle add column if not exists modified_by bigint;

Alter table master.vehicleoptinoptout add column if not exists organization_id bigint;
Alter table master.vehicleoptinoptout add column if not exists vehicle_id bigint;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_vehicleoptinoptout_vehicleid_vehicle_id' AND table_name='vehicleoptinoptout'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.vehicleoptinoptout 
			ADD CONSTRAINT fk_vehicleoptinoptout_vehicleid_vehicle_id FOREIGN KEY (vehicle_id) REFERENCES  master.vehicle (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_vehicleoptinoptout_organizationid_organization_id' AND table_name='vehicleoptinoptout'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.vehicleoptinoptout 
			ADD CONSTRAINT fk_vehicleoptinoptout_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Alter table master.vehicleoptinoptout add column if not exists opt_in char(1) not null;
Alter table master.vehicleoptinoptout add column if not exists created_at bigint not null;
Alter table master.vehicleoptinoptout add column if not exists modified_at bigint;
Alter table master.vehicleoptinoptout add column if not exists is_ota boolean not null default false;
Alter table master.vehicleoptinoptout add column if not exists modified_by bigint;

Alter table master.driver add column if not exists opt_in char(1) not null;
Alter table master.driver add column if not exists modified_at bigint;
Alter table master.driver add column if not exists modified_by bigint;

Alter table master.vehicleoptinoptout drop column if  exists ref_id;
Alter table master.vehicleoptinoptout drop column if  exists account_id;
Alter table master.vehicleoptinoptout drop column if  exists status_changed_date;
Alter table master.vehicleoptinoptout drop column if  exists type;

Alter table master.driveroptinoptout add column if not exists opt_in char(1) not null;
Alter table master.driveroptinoptout add column if not exists created_at bigint;

--orgconfiguration
CREATE TABLE if not exists master.drivertemplate 
(
	id serial not null, 
	column_name int not null,
	key char (100)  not null
	
)
TABLESPACE pg_default;

ALTER TABLE  master.drivertemplate 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_drivertemplate_id' AND table_name='drivertemplate'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.drivertemplate 
			ADD CONSTRAINT pk_drivertemplate_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


--orgconfiguration
CREATE TABLE if not exists master.featurebasedobjects 
(
	id serial not null, 
	menu_id int not null,
	feature_id int not null,
	label_name char (100)  not null
	
)
TABLESPACE pg_default;

ALTER TABLE  master.featurebasedobjects 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_featurebasedobjects_id' AND table_name='featurebasedobjects'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.featurebasedobjects 
			ADD CONSTRAINT pk_featurebasedobjects_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_featurebasedobjects_menuid_menu_id' AND table_name='featurebasedobjects'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.featurebasedobjects 
			ADD CONSTRAINT fk_featurebasedobjects_menuid_menu_id FOREIGN KEY (menu_id) REFERENCES  master.menu (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_featurebasedobjects_featureid_feature_id' AND table_name='featurebasedobjects'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.featurebasedobjects 
			ADD CONSTRAINT fk_featurebasedobjects_featureid_feature_id FOREIGN KEY (feature_id) REFERENCES  master.feature (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_featurebasedobjects_menuid_menu_id' AND table_name='featurebasedobjects'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.featurebasedobjects 
			ADD CONSTRAINT fk_featurebasedobjects_menuid_menu_id FOREIGN KEY (menu_id) REFERENCES  master.menu (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.resetpasswordtoken 
(
	id serial not null, 
	account_id int not null,
	token_secret uuid,
	status varchar(25),
	expiry_at bigint,
	created_at bigint
)
TABLESPACE pg_default;

ALTER TABLE  master.resetpasswordtoken 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_resetpasswordtoken_id' AND table_name='resetpasswordtoken'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.resetpasswordtoken 
			ADD CONSTRAINT pk_resetpasswordtoken_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_resetpasswordtoken_accountid_account_id' AND table_name='resetpasswordtoken'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.resetpasswordtoken 
			ADD CONSTRAINT fk_resetpasswordtoken_accountid_account_id FOREIGN KEY (account_id) REFERENCES  master.account (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


--select * from master.accountsession

Alter table master.accounttoken add column if not exists id serial not null;
Alter table master.accountassertion add column if not exists id serial not null;
ALTER TABLE master.accountsession  rename COLUMN id to session_id ;
Alter table master.accountsession add column if not exists id serial not null;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_accounttoken_id' AND table_name='accounttoken'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.accounttoken 
			ADD CONSTRAINT pk_accounttoken_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_accountassertion_id' AND table_name='accountassertion'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.accountassertion 
			ADD CONSTRAINT pk_accountassertion_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

	
alter table master.accountassertion drop constraint fk_accountassertion_sessionid_accountsession_id;
alter table master.accounttoken drop constraint fk_accounttoken_sessionid_accountsession_id;
alter table master.accountsession drop constraint pk_accountsession_id;		


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_accountsession_id' AND table_name='accountsession'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.accountsession 
			ADD CONSTRAINT pk_accountsession_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


alter table master.accountassertion drop column if exists session_id ;
alter table master.accounttoken drop column if exists session_id ;

alter table master.accountassertion add column if not exists session_id int not null;
alter table master.accounttoken add column if not exists session_id int not null;



Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accountassertion_sessionid_accountsession_id' AND table_name='accountassertion'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accountassertion 
			ADD CONSTRAINT fk_accountassertion_sessionid_accountsession_id FOREIGN KEY (session_id) REFERENCES  master.accountsession (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accounttoken_sessionid_accountsession_id' AND table_name='accounttoken'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accounttoken 
			ADD CONSTRAINT fk_accounttoken_sessionid_accountsession_id FOREIGN KEY (session_id) REFERENCES  master.accountsession (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

----------------------
--Alter table master.feature alter column key set not null;

--update master.featureset set created_by=4;
--select * from master.account order by 1
Alter table master.featureset alter column created_by set not null;

--select * from master.menu where seq_no is null
Alter table master.menu alter column  seq_no set not null;

--Alter table master.menu alter column  key set not null;

--select * from master.vehicleproperties
--update master.vehicle set vehicle_property_id=1 where vehicle_property_id is null
Alter table master.vehicle alter column  vehicle_property_id set not null;

--select * from master.accountblob 

--Alter table master.accountblob alter column image set not null ;

--select * from master.feature where level is null
--update master.feature set level=40 where level is null
Alter table master.feature alter column  level set not null ;

--select * from master.role
--update master.role set level=40 where level is null
Alter table master.role alter column level set not null;

--select * from master.oem
--select * from master.organization order by 1
--update master.vehicle set oem_id=1,oem_organisation_id=111
Alter table master.vehicle alter column oem_id set not null; 
Alter table master.vehicle alter column oem_organisation_id set not null; 

Alter table master.driver add column if not exists created_at bigint not null default extract(epoch from now()) * 1000; 

--update master.organization set  vehicle_default_opt_in='U',driver_default_opt_in='U'
Alter table master.organization alter column  vehicle_default_opt_in set not null;
Alter table master.organization alter column  driver_default_opt_in set not null;

--update master.vehicle set opt_in='H'
Alter table master.vehicle alter column opt_in set not null;

--select * from master.vehicleoptinoptout
--select * from master.vehicle  order by 1
--update master.vehicleoptinoptout set organization_id =1,vehicle_id=5
Alter table master.vehicleoptinoptout alter column organization_id set not null;
Alter table master.vehicleoptinoptout alter column vehicle_id set not null;


ALTER TABLE master.vehicle ALTER COLUMN vehicle_property_id DROP NOT NULL;
alter table master.role drop column created_date;
alter table master.role rename column updated_date to modified_at;
alter table master.role rename column updated_by to modified_by;
alter table master.driver drop column if exists salutation;
alter table master.driver drop column if exists dob;
alter table master.driver add column if not exists email varchar(100);

drop table if exists master.translationupload;

CREATE TABLE if not exists translation.translationupload 
(
	id serial not null, 
	file_name varchar(50) not null,
	description varchar(250),
	file_size int,
	success_count int not null,
	failure_count int not null,
	created_at bigint not null,
	craeted_by int not null
)
TABLESPACE pg_default;

ALTER TABLE  translation.translationupload 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_translationupload_id' AND table_name='translationupload'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  translation.translationupload 
			ADD CONSTRAINT pk_translationupload_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

alter table translation.translationupload add column if not exists file bytea not null;
alter table translation.translationupload add column if not exists added_count int;
alter table translation.translationupload add column if not exists updated_count varchar(100);
alter table translation.translationupload drop column if exists success_count;
alter table master.orgrelationship add column if not exists is_active boolean not null default true;
alter table master.orgrelationship add column if not exists level int;
alter table master.orgrelationshipmapping rename column allow_chain_rel_to_target_org to allow_chain;
alter table translation.translationupload drop column if exists craeted_by;
alter table translation.translationupload add column if not exists created_by int;
alter table master.driver alter column  email type varchar(100);
ALTER TABLE master.drivertemplate ALTER COLUMN column_name TYPE varchar(50) ;
ALTER TABLE master.feature add COLUMN if not exists state char(1) ;
ALTER TABLE master.menu ALTER COLUMN seq_no TYPE int USING seq_no::integer;
alter table master.dataattributeset alter column modified_by drop not null;
ALTER TABLE master.subscription add COLUMN if not exists is_zuora_package boolean not null  ;
ALTER TABLE master.dataattributeset ALTER COLUMN is_exlusive TYPE boolean USING is_exlusive::boolean;
ALTER TABLE master.dataattributeset ALTER COLUMN is_exlusive set  not null;
ALTER TABLE translation.translationgrouping ALTER COLUMN name TYPE varchar(350) ;
ALTER TABLE master.driver ALTER COLUMN first_name DROP NOT NULL;
ALTER TABLE master.driver ALTER COLUMN opt_in SET NOT NULL;
Alter table master.orgrelationship add column if not exists created_at bigint not null default extract(epoch from now()) * 1000;
Alter table master.orgrelationshipmapping add column if not exists created_at bigint not null;	
alter table master.oem alter column vin_prefix drop not null;
ALTER TABLE master.vehicleproperties ALTER COLUMN length TYPE varchar(50);
ALTER TABLE master.vehicleproperties ALTER COLUMN height TYPE varchar(50);
ALTER TABLE master.vehicleproperties ALTER COLUMN weight TYPE varchar(50);
ALTER TABLE master.vehicleproperties ALTER COLUMN engine_power TYPE varchar(50);
ALTER TABLE master.vehicleproperties ALTER COLUMN chasis_rear_overhang TYPE varchar(50);
ALTER TABLE master.vehicleproperties ALTER COLUMN driveline_wheel_base TYPE varchar(50);
ALTER TABLE master.vehicleproperties ALTER COLUMN model_year TYPE varchar(50);
ALTER TABLE master.vehicleproperties ALTER COLUMN width TYPE varchar(50);
ALTER TABLE master.vehiclefueltankproperties ALTER COLUMN chasis_fuel_tank_number TYPE varchar(50);
ALTER TABLE master.vehiclefueltankproperties ALTER COLUMN chasis_fuel_tank_volume TYPE varchar(50);
ALTER TABLE master.vehicleaxleproperties ALTER COLUMN position TYPE varchar(50);
ALTER TABLE master.vehicleaxleproperties ALTER COLUMN load TYPE varchar(50);
ALTER TABLE master.vehicleaxleproperties ALTER COLUMN ratio TYPE varchar(50);
ALTER TABLE master.vehicleaxleproperties add COLUMN if not exists TYPE varchar(50);
alter table master.vehicleaxleproperties add column if not exists size varchar(50);
alter table master.vehicleaxleproperties add column if not exists is_wheel_tire_size_replaced boolean not null;
Alter table master.vehicle add column if not exists fuel_type char(1) not null default 'D';
ALTER TABLE master.accounttoken ALTER COLUMN access_token TYPE character varying(1500) ;
ALTER TABLE master.dataattributeset ALTER COLUMN name TYPE character varying(250) ;
ALTER TABLE master.package add COLUMN if not exists created_at bigint not null default extract(epoch from now()) * 1000;
ALTER TABLE master.package add COLUMN if not exists status char(1) not null default 'A';
ALTER TABLE master.vehicle ALTER COLUMN fuel_type TYPE character varying(50);
ALTER TABLE master.vehicle ALTER fuel_type DROP DEFAULT;
ALTER TABLE master.vehicle ALTER COLUMN fuel_type set default 'Diesel' ;
ALTER TABLE master.vehicle ALTER COLUMN fuel_type set not null ;

alter table master.vehicle alter column fuel_type drop default;
alter table master.vehicle alter column fuel_type drop not null;
ALTER TABLE master.organization ALTER COLUMN name DROP NOT NULL;
ALTER TABLE master.orgrelationshipmapping ALTER COLUMN target_org_id DROP NOT NULL;

CREATE TABLE if not exists  master.co2coefficient  
(
	id serial NOT NULL,   
	description varchar (25) NOT NULL, 
	fuel_type char (1) , 
	coefficient float NULL 
)
TABLESPACE pg_default;

ALTER TABLE master.co2coefficient  
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_co2coefficient_id' AND table_name='co2coefficient'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.co2coefficient 
			ADD CONSTRAINT pk_co2coefficient_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists  master.passwordpolicy
(
    id serial NOT NULL ,
    account_id integer NOT NULL,
    modified_at bigint,
    failed_login_attempts integer NOT NULL DEFAULT 0,
    locked_until bigint,
    account_lock_attempts integer NOT NULL DEFAULT 0,
    is_blocked boolean NOT NULL DEFAULT false
)
TABLESPACE pg_default;

ALTER TABLE master.co2coefficient  
    OWNER to pgdbadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_passwordpolicy_id' AND table_name='passwordpolicy'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.passwordpolicy 
			ADD CONSTRAINT pk_passwordpolicy_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='uk_passwordpolicy_account_id' AND table_name='passwordpolicy'
		and constraint_type='UNIQUE')
then	
	begin
		ALTER TABLE  master.passwordpolicy 
			ADD CONSTRAINT uk_passwordpolicy_account_id UNIQUE  (account_id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

/*
alter table master.vehicleproperties drop column if exists length;
alter table master.vehicleproperties drop column if exists height;
alter table master.vehicleproperties drop column if exists weight;
*/
----Till here scripts executed on Dev0/Dev1 and Dev2 envs




