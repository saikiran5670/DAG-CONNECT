--organization (Master data (Org for DAF Admin) is also required)
CREATE TABLE if not exists master.organization 
(
	id serial not null, 
	org_id varchar (100) not null,
	type varchar(50) ,  
	name varchar (100) not null,
	address_type varchar (50),
	street varchar (50),
	street_number varchar (50),
	postal_code varchar (15),
	city varchar(50),
	country_code varchar(20),
	reference_date bigint not null,
	optout_status boolean not null default false,
	optout_status_changed_date bigint,
	is_active boolean not null default true
)
TABLESPACE pg_default;

ALTER TABLE  master.organization 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_organization_id' AND table_name='organization'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.organization 
			ADD CONSTRAINT pk_organization_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='uk_organization_orgid' AND table_name='organization'
		and constraint_type='UNIQUE')
then	
	begin
		ALTER TABLE  master.organization 
			ADD CONSTRAINT uk_organization_orgid UNIQUE  (org_id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


CREATE TABLE if not exists  master.group
(
	 id serial not null,
	 object_type char(1) not null, ------------------vehicle/account
	 group_type char(1) not null, -------------------single/group/dynamic
	 argument varchar(50),
	 function_enum char(1),
	 organization_id int not null,
	 ref_id int,
	 name varchar(50) not null,
	 description varchar(100)
)
TABLESPACE pg_default;

ALTER TABLE  master.group
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_group_id' AND table_name='group'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.group 
			ADD CONSTRAINT pk_group_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_group_organizationid_organization_id' AND table_name='group'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.group 
			ADD CONSTRAINT fk_group_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--groupref
CREATE TABLE if not exists  master.groupref 
(
    group_id integer NOT NULL,
    ref_id integer NOT NULL
)
TABLESPACE pg_default;

ALTER TABLE  master.groupref 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_groupref_groupid_refid' AND table_name='groupref'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.groupref 
			ADD CONSTRAINT pk_groupref_groupid_refid PRIMARY KEY (group_id, ref_id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_groupref_groupid_group_id' AND table_name='groupref'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.groupref 
			ADD CONSTRAINT fk_groupref_groupid_group_id FOREIGN KEY (group_id) REFERENCES  master.group (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--account  (Master data (one DAF Admin account) is also required)
CREATE TABLE if not exists  master.account 
(
	id serial not null,
	email varchar(50) not null,
	salutation varchar(5) not null,
	first_name varchar(30) not null,
	last_name varchar(20),
	dob bigint,
	type char(1) not null ----------------
)
TABLESPACE pg_default;

ALTER TABLE  master.account 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_account_id' AND table_name='account'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.account 
			ADD CONSTRAINT pk_account_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='uk_account_email' AND table_name='account'
		and constraint_type='UNIQUE')
then	
	begin
		ALTER TABLE  master.account 
			ADD CONSTRAINT uk_account_email UNIQUE  (email)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--accountorg (Master data is also required)
CREATE TABLE if not exists  master.accountorg 
(
	id serial not null,
	account_id int not null,
	organization_id int not null,
	start_date bigint not null,
	end_date bigint,
	is_active boolean not null default true
)
TABLESPACE pg_default;

ALTER TABLE  master.accountorg 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_accountorg_id' AND table_name='accountorg'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.accountorg 
			ADD CONSTRAINT pk_accountorg_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accountorg_accountid_account_id' AND table_name='accountorg'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accountorg 
			ADD CONSTRAINT fk_accountorg_accountid_account_id FOREIGN KEY (account_id) REFERENCES  master.account (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accountorg_organizationid_organization_id' AND table_name='accountorg'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accountorg 
			ADD CONSTRAINT fk_accountorg_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--role (Master data is also required)
CREATE TABLE if not exists  master.role 
(
	id serial not null,
	organization_id int,
	name varchar(50) not null,
	is_active boolean not null default true, ----------
	created_date bigint,
	created_by int,
	updated_date bigint,
	updated_by int,
	description varchar(120)
)
TABLESPACE pg_default;

ALTER TABLE  master.role 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_role_id' AND table_name='role'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.role 
			ADD CONSTRAINT pk_role_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_role_organizationid_organization_id' AND table_name='role'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.role 
			ADD CONSTRAINT fk_role_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--accountrole (Master data is also required)
CREATE TABLE if not exists  master.accountrole 
(
	account_id int not null,
	organization_id  int not null,
	role_id  int not null,
	start_date bigint not null,
	end_date bigint
)
TABLESPACE pg_default;

ALTER TABLE  master.accountrole 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_accountrole_accountid_organizationid_roleid_startdate' AND table_name='accountrole'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.accountrole 
			ADD CONSTRAINT pk_accountrole_accountid_organizationid_roleid_startdate PRIMARY KEY (account_id, organization_id, role_id, start_date)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accountrole_accountid' AND table_name='accountrole'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accountrole 
			ADD CONSTRAINT fk_accountrole_accountid FOREIGN KEY (account_id) REFERENCES  master.account (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accountrole_organizationid' AND table_name='accountrole'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accountrole 
			ADD CONSTRAINT fk_accountrole_organizationid FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accountrole_roleid' AND table_name='accountrole'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accountrole 
			ADD CONSTRAINT fk_accountrole_roleid FOREIGN KEY (role_id) REFERENCES  master.role (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


--DataAttribute (Master data is also required)
CREATE TABLE if not exists  master.dataattribute 
(
	id serial not null,
	name varchar(50) not null,
	description varchar(100) 
)
TABLESPACE pg_default;

ALTER TABLE  master.dataattribute 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_dataattribute_id' AND table_name='dataattribute'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.dataattribute 
			ADD CONSTRAINT pk_dataattribute_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--DataAttributeSet (Master data is also required)
CREATE TABLE if not exists  master.dataattributeset 
(
	id serial not null,
	name varchar(50) not null,
	description varchar(100), 
	is_exlusive boolean not null default true
)
TABLESPACE pg_default;

ALTER TABLE  master.dataattributeset 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_dataattributeset_id' AND table_name='dataattributeset'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.dataattributeset 
			ADD CONSTRAINT pk_dataattributeset_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--dataattributesetattribute (Master data is also required)
CREATE TABLE if not exists  master.dataattributesetattribute 
(
	data_attribute_set_id int not null,
	data_attribute_id int not null
)
TABLESPACE pg_default;

ALTER TABLE  master.dataattributesetattribute 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_dataattributesetattribute_dataattributesetid_dataattributeid' AND table_name='dataattributesetattribute'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.dataattributesetattribute 
			ADD CONSTRAINT pk_dataattributesetattribute_dataattributesetid_dataattributeid PRIMARY KEY (data_attribute_set_id, data_attribute_id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_dataattributesetattribute_dasid_dataattributeset_id' AND table_name='dataattributesetattribute'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.dataattributesetattribute 
			ADD CONSTRAINT fk_dataattributesetattribute_dasid_dataattributeset_id FOREIGN KEY (data_attribute_set_id) REFERENCES  master.dataattributeset (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_dataattributesetattribute_dataattributeid_dataattribute_id' AND table_name='dataattributesetattribute'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.dataattributesetattribute 
			ADD CONSTRAINT fk_dataattributesetattribute_dataattributeid_dataattribute_id FOREIGN KEY (data_attribute_id) REFERENCES  master.dataattribute (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--feature (Master data is also required)
CREATE TABLE if not exists  master.feature 
(
	id serial not null,
	name varchar(50) not null,
	description varchar(100),
	type char(1) not null,
	is_active boolean not null default true,
	data_attribute_set_id int 
)
TABLESPACE pg_default;

ALTER TABLE  master.feature 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_feature_id' AND table_name='feature'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.feature 
			ADD CONSTRAINT pk_feature_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_feature_dataattributesetid_dataattributeset_id' AND table_name='feature'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.feature 
			ADD CONSTRAINT fk_feature_dataattributesetid_dataattributeset_id FOREIGN KEY (data_attribute_set_id) REFERENCES  master.dataattributeset (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--featureset (Master data is also required)
CREATE TABLE if not exists  master.featureset 
(
	id serial not null,
	name varchar(50) not null,
	description varchar(100),
	is_active boolean not null default true,
	is_custom_feature_set boolean not null default false
)
TABLESPACE pg_default;

ALTER TABLE  master.featureset 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_featureset_id' AND table_name='featureset'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.featureset 
			ADD CONSTRAINT pk_featureset_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--featuresetfeature (Master data is also required)
CREATE TABLE if not exists  master.featuresetfeature 
(
	feature_set_id int not null,
	feature_id int not null
)
TABLESPACE pg_default;

ALTER TABLE  master.featuresetfeature 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_featuresetfeature_featuresetid_featureid' AND table_name='featuresetfeature'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.featuresetfeature 
			ADD CONSTRAINT pk_featuresetfeature_featuresetid_featureid PRIMARY KEY (feature_set_id, feature_id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_featuresetfeature_featuresetid_featureset_id' AND table_name='featuresetfeature'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.featuresetfeature 
			ADD CONSTRAINT fk_featuresetfeature_featuresetid_featureset_id FOREIGN KEY (feature_set_id) REFERENCES  master.featureset (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_featuresetfeature_featureid_feature_id' AND table_name='featuresetfeature'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.featuresetfeature 
			ADD CONSTRAINT fk_featuresetfeature_featureid_feature_id FOREIGN KEY (feature_id) REFERENCES  master.feature (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--rolefeatureset (Master data is also required)
CREATE TABLE if not exists  master.rolefeatureset 
(
	feature_set_id int not null,
	role_id int not null
)
TABLESPACE pg_default;

ALTER TABLE  master.rolefeatureset 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_rolefeatureset_featuresetid_roleid' AND table_name='rolefeatureset'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.rolefeatureset 
			ADD CONSTRAINT pk_rolefeatureset_featuresetid_roleid PRIMARY KEY (feature_set_id, role_id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_rolefeatureset_featuresetid_featureset_id' AND table_name='rolefeatureset'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.rolefeatureset 
			ADD CONSTRAINT fk_rolefeatureset_featuresetid_featureset_id FOREIGN KEY (feature_set_id) REFERENCES  master.featureset (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_rolefeatureset_roleid_role_id' AND table_name='rolefeatureset'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.rolefeatureset 
			ADD CONSTRAINT fk_rolefeatureset_roleid_role_id FOREIGN KEY (role_id) REFERENCES  master.role (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--menu (Master data is also required)
CREATE TABLE if not exists  master.menu 
(
	id serial not null,
	name varchar(50) not null,
	description varchar(100),
	is_active boolean not null default true,
	parent_id int ,
	feature_id int not null
)
TABLESPACE pg_default;

ALTER TABLE  master.menu 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_menu_id' AND table_name='menu'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.menu 
			ADD CONSTRAINT pk_menu_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_menu_featureid_feature_id' AND table_name='menu'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.menu 
			ADD CONSTRAINT fk_menu_featureid_feature_id FOREIGN KEY (feature_id) REFERENCES  master.feature (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--accessrelationship
CREATE TABLE if not exists  master.accessrelationship 
(
	id serial not null,
	access_type char(1) not null,
	account_group_id int not null,
	vehicle_group_id int not null
)
TABLESPACE pg_default;

ALTER TABLE  master.accessrelationship 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_accessrelationship_id' AND table_name='accessrelationship'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.accessrelationship 
			ADD CONSTRAINT pk_accessrelationship_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accessrelationship_accountgroupid_group_id' AND table_name='accessrelationship'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accessrelationship 
			ADD CONSTRAINT fk_accessrelationship_accountgroupid_group_id FOREIGN KEY (account_group_id) REFERENCES  master.group (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accessrelationship_vehiclegroupid_group_id' AND table_name='accessrelationship'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accessrelationship 
			ADD CONSTRAINT fk_accessrelationship_vehiclegroupid_group_id FOREIGN KEY (vehicle_group_id) REFERENCES  master.group (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--vehicle
CREATE TABLE if not exists  master.vehicle 
(
	id serial not null,
	organization_id int ,
	name varchar(50),
	vin varchar(50) not null,
	license_plate_number varchar(50),
	status char(1) ,
	status_changed_date bigint,
	termination_date bigint,
	vid varchar(50),
	type char(1) ,
	model varchar(50) ,
	tcu_id varchar(50),
	tcu_serial_number varchar(50) ,
	tcu_brand varchar(50) ,
	tcu_version varchar(8),
	is_tcu_register boolean ,
	reference_date bigint 
)
TABLESPACE pg_default;

ALTER TABLE  master.vehicle 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_vehicle_id' AND table_name='vehicle'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.vehicle 
			ADD CONSTRAINT pk_vehicle_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_vehicle_organizationid_organization_id' AND table_name='vehicle'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.vehicle 
			ADD CONSTRAINT fk_vehicle_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='uk_vehicle_vin' AND table_name='vehicle'
		and constraint_type='UNIQUE')
then	
	begin
		ALTER TABLE  master.vehicle 
			ADD CONSTRAINT uk_vehicle_vin UNIQUE  (vin)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--vehicleproperties 
CREATE TABLE if not exists  master.vehicleproperties 
(
	id serial not null,
	vehicle_id int not null,
	manufacture_date bigint,
	registration_date bigint,
	delivery_date bigint,
	make varchar(50),
	series varchar(50),
	length int,
	widht int,
	height int,
	weight int,
	engine_id varchar(50),
	engine_type  varchar(50),
	engine_power int,
	engine_coolant  varchar(50),
	engine_emission_level  varchar(50),
	chasis_id varchar(50),
	is_chasis_side_skirts boolean not null default true,
	is_chasis_side_collar boolean not null default true,
	chasis_rear_overhang int,
	chasis_fuel_tank_number int,
	chasis_fuel_tank_volume int,
	driveline_axle_configuration varchar(50),
	driveline_wheel_base varchar(50),
	driveline_tire_size varchar(50),
	driveline_front_axle_position int,
	driveline_front_axle_load int,
	driveline_rear_axle_position int,
	driveline_rear_axle_load int,
	driveline_rear_axle_ratio int,
	transmission_gearbox_id varchar(50),
	transmission_gearbox_type  varchar(50),
	cabin_id varchar(50),
	cabin_color_id varchar(50),
	cabin_color_value varchar(50)
)
TABLESPACE pg_default;

ALTER TABLE  master.vehicleproperties  
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_vehicleproperty_id' AND table_name='vehicleproperties'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.vehicleproperties  
			ADD CONSTRAINT pk_vehicleproperty_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_vehicleproperty_vehicleid_vehicle_id' AND table_name='vehicleproperties'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.vehicleproperties  
			ADD CONSTRAINT fk_vehicleproperty_vehicleid_vehicle_id FOREIGN KEY (vehicle_id) REFERENCES  master.vehicle (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--vehicleoptinoptout
CREATE TABLE if not exists  master.vehicleoptinoptout 
(
	id serial not null,
	ref_id int not null,
	account_id int not null,
	status char(1) not null,
	status_changed_date bigint not null,
	type char(1) not null
)
TABLESPACE pg_default;

ALTER TABLE  master.vehicleoptinoptout 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_vehicleoptinoptout_id' AND table_name='vehicleoptinoptout'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.vehicleoptinoptout 
			ADD CONSTRAINT pk_vehicleoptinoptout_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_vehicleoptinoptout_accountid_account_id' AND table_name='vehicleoptinoptout'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.vehicleoptinoptout 
			ADD CONSTRAINT fk_vehicleoptinoptout_accountid_account_id FOREIGN KEY (account_id) REFERENCES  master.account (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--orgrelationship
CREATE TABLE if not exists  master.orgrelationship 
(
	id serial not null,
	organization_id int not null,
	feature_set_id int not null,
	name varchar(50) not null,
	description varchar(100)
)
TABLESPACE pg_default;

ALTER TABLE  master.orgrelationship 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_orgrelationship_id' AND table_name='orgrelationship'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.orgrelationship 
			ADD CONSTRAINT pk_orgrelationship_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_orgrelationship_organizationid_organization_id' AND table_name='orgrelationship'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.orgrelationship 
			ADD CONSTRAINT fk_orgrelationship_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_orgrelationship_featuresetid_featureset_id' AND table_name='orgrelationship'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.orgrelationship 
			ADD CONSTRAINT fk_orgrelationship_featuresetid_featureset_id FOREIGN KEY (feature_set_id) REFERENCES  master.featureset (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--orgrelationshipmapping
CREATE TABLE if not exists  master.orgrelationshipmapping 
(
	id serial not null,
	relationship_id int not null,
	type char(1) not null,
	organization_id int not null,
	vehicle_id int null,
	vehicle_group_id int null
)
TABLESPACE pg_default;

ALTER TABLE  master.orgrelationshipmapping 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_orgrelationshipmapping_id' AND table_name='orgrelationshipmapping'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.orgrelationshipmapping 
			ADD CONSTRAINT pk_orgrelationshipmapping_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_orgrelationshipmapping_organizationid_organization_id' AND table_name='orgrelationshipmapping'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.orgrelationshipmapping 
			ADD CONSTRAINT fk_orgrelationshipmapping_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_orgrelationshipmapping_relationshipid_orgrelationship_id' AND table_name='orgrelationshipmapping'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.orgrelationshipmapping 
			ADD CONSTRAINT fk_orgrelationshipmapping_relationshipid_orgrelationship_id FOREIGN KEY (relationship_id) REFERENCES  master.orgrelationship (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_orgrelationshipmapping_vehicleid_vehicle_id' AND table_name='orgrelationshipmapping'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.orgrelationshipmapping 
			ADD CONSTRAINT fk_orgrelationshipmapping_vehicleid_vehicle_id FOREIGN KEY (vehicle_id) REFERENCES  master.vehicle (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_orgrelationshipmapping_vehiclegroupid_group_id' AND table_name='orgrelationshipmapping'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.orgrelationshipmapping 
			ADD CONSTRAINT fk_orgrelationshipmapping_vehiclegroupid_group_id FOREIGN KEY (vehicle_group_id) REFERENCES  master.group (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--package
CREATE TABLE if not exists  master.package 
(
	id serial not null,
	feature_set_id int not null,
	code varchar(10),
	name varchar(50) not null,
	type char(1) not null,
	service_subscriber_type char(1) not null,
	short_description varchar(50),
	long_description varchar(100),
	is_default boolean not null default false,
	start_date bigint,
	end_date bigint,
	is_active boolean not null default true
)
TABLESPACE pg_default;

ALTER TABLE  master.package 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_package_id' AND table_name='package'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.package 
			ADD CONSTRAINT pk_package_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_package_featuresetid_featureset_id' AND table_name='package'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.package 
			ADD CONSTRAINT fk_package_featuresetid_featureset_id FOREIGN KEY (feature_set_id) REFERENCES  master.featureset (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--subscription
CREATE TABLE if not exists  master.subscription 
(
	id serial not null,
	organization_id int not null,
	order_id varchar(50),
	order_date bigint
)
TABLESPACE pg_default;

ALTER TABLE  master.subscription 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_subscription_id' AND table_name='subscription'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.subscription 
			ADD CONSTRAINT pk_subscription_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_subscription_organizationid_organization_id' AND table_name='subscription'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.subscription 
			ADD CONSTRAINT fk_subscription_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--packagesubscription
CREATE TABLE if not exists  master.packagesubscription 
(
	id serial not null,
	subscription_id int not null,
	package_id int not null,
	type char(1) not null,
	status char(1) not null,
	start_date bigint not null,
	end_date bigint
)
TABLESPACE pg_default;

ALTER TABLE  master.packagesubscription 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_packagesubscription_id' AND table_name='packagesubscription'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.packagesubscription 
			--ADD CONSTRAINT pk_packagesubscription_subscriptionid_packageid_subscriptionstartdate PRIMARY KEY (subscription_id, package_id, subscription_start_date)
			ADD CONSTRAINT pk_packagesubscription_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_packagesubscription_subscriptionid_subscription_id' AND table_name='packagesubscription'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.packagesubscription 
			ADD CONSTRAINT fk_packagesubscription_subscriptionid_subscription_id FOREIGN KEY (subscription_id) REFERENCES  master.subscription (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_packagesubscription_packageid_package_id' AND table_name='packagesubscription'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.packagesubscription 
			ADD CONSTRAINT fk_packagesubscription_packageid_package_id FOREIGN KEY (package_id) REFERENCES  master.package (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--subscribedpackagevehicle
CREATE TABLE if not exists  master.subscribedpackagevehicle 
(
	id serial not null,
	subscription_id int not null,
	package_id int not null,
	vehicle_id int not null
)
TABLESPACE pg_default;

ALTER TABLE  master.subscribedpackagevehicle 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_subscribedpackagevehicle_id' AND table_name='subscribedpackagevehicle'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.subscribedpackagevehicle 
			--ADD CONSTRAINT pk_subscribedpackagevehicle_subscriptionid_vehicleid PRIMARY KEY (subscription_id, vehicle_id)
			ADD CONSTRAINT pk_subscribedpackagevehicle_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_subscribedpackagevehicle_subscriptionid_subscription_id' AND table_name='subscribedpackagevehicle'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.subscribedpackagevehicle 
			ADD CONSTRAINT fk_subscribedpackagevehicle_subscriptionid_subscription_id FOREIGN KEY (subscription_id) REFERENCES  master.subscription (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_subscribedpackagevehicle_packageid_package_id' AND table_name='subscribedpackagevehicle'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.subscribedpackagevehicle 
			ADD CONSTRAINT fk_subscribedpackagevehicle_packageid_package_id FOREIGN KEY (package_id) REFERENCES  master.package (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_subscribedpackagevehicle_vehicleid_vehicle_id' AND table_name='subscribedpackagevehicle'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.subscribedpackagevehicle 
			ADD CONSTRAINT fk_subscribedpackagevehicle_vehicleid_vehicle_id FOREIGN KEY (vehicle_id) REFERENCES  master.vehicle (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


--timezone (Master data is also required)
CREATE TABLE if not exists  master.timezone 
(
    id	Serial Not Null,
	short_name	Varchar(10) not null,
	name	Varchar(100) not null,
	ut_coff_set	Varchar(50) not null
)

TABLESPACE pg_default;

ALTER TABLE  master.timezone 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_timezone_id' AND table_name='timezone'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.timezone 
			ADD CONSTRAINT pk_timezone_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;

	end;
end if;
end;
$$;

------------------------------------------------

 --audittrail
CREATE TABLE if not exists  logs.audittrail 
(
    id serial NOT NULL, 
    created_at bigint NOT NULL, 
    performed_at bigint NOT NULL, 
    performed_by integer, 
    component_name character varying(50) NOT NULL, 
    service_name character varying(50) NOT NULL, 
    event_type char(1) NOT NULL, 
    event_status character varying(10) NOT NULL, 
    message character varying(1000), 
    sourceobject_id integer, 
    targetobject_id integer, 
    updated_data json
)
TABLESPACE pg_default;

ALTER TABLE  logs.audittrail 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_audittrail_id' AND table_name='audittrail'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  logs.audittrail 
			ADD CONSTRAINT pk_audittrail_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

------------------------------------------------------------
--languages (Master data is also required)
CREATE TABLE if not exists  translation.languages 
(
	id serial NOT NULL,  
	name varchar (50) NOT NULL,  
	code varchar (8) NOT NULL, 
	key varchar (100) NOT NULL, 
	description varchar (100) 
)
TABLESPACE pg_default;

ALTER TABLE  translation.languages 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_languages_id' AND table_name='languages'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  translation.languages 
			ADD CONSTRAINT pk_languages_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='uk_languages_code' AND table_name='languages'
		and constraint_type='UNIQUE')
then	
	begin
		ALTER TABLE  translation.languages 
			ADD CONSTRAINT uk_languages_code UNIQUE  (code)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


--translation (Master data is also required)
CREATE TABLE if not exists  translation.translation 
(
	id serial NOT NULL,   
	code varchar (8) NOT NULL,  
	type char(1) NOT NULL,
	name varchar (250) NOT NULL,
	value varchar (1000) NOT NULL,
	created_at bigint NOT NULL ,
	modified_at bigint 
)
TABLESPACE pg_default;

ALTER TABLE  translation.translation 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_translation_id' AND table_name='translation'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  translation.translation 
			ADD CONSTRAINT pk_translation_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_translation_code_languages_code' AND table_name='translation'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  translation.translation 
			ADD CONSTRAINT fk_translation_code_languages_code FOREIGN KEY (code) REFERENCES  translation.languages (code);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--translationgrouping (Master data is also required)
CREATE TABLE if not exists  translation.translationgrouping 
(
	id serial NOT NULL,    
	name varchar (100)NOT NULL,     
	ref_id int NOT NULL,    
	type char(1) NOT NULL    
)
TABLESPACE pg_default;

ALTER TABLE  translation.translationgrouping 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_translationgrouping_id' AND table_name='translationgrouping'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  translation.translationgrouping 
			ADD CONSTRAINT pk_translationgrouping_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--icons
CREATE TABLE if not exists  translation.icons 
(
	id serial NOT NULL,     
	name varchar (50) NOT NULL,     
	url varchar (255)  NOT NULL,    
	type char(1)  NOT NULL,    
	key varchar (100)  NOT NULL
)
TABLESPACE pg_default;

ALTER TABLE  translation.icons 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_icons_id' AND table_name='icons'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  translation.icons 
			ADD CONSTRAINT pk_icons_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

-----------------------------------------------------------

--accountpreference 
CREATE TABLE if not exists  master.accountpreference 
(
	id serial NOT NULL,      
	ref_id int NOT NULL,      
	type char(1) NOT NULL,      
	language_id int NOT NULL,      
	timezone_id int NOT NULL,      
	currency_type char(1) NOT NULL,      
	unit_type char(1) NOT NULL,      
	vehicle_display_type char(1) NOT NULL,      
	date_format_type char(1) NOT NULL,      
	driver_id Varchar(16) ,      
	is_active boolean not null default true
)
TABLESPACE pg_default;

ALTER TABLE  master.accountpreference 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_accountpreference_id' AND table_name='accountpreference'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.accountpreference 
			ADD CONSTRAINT pk_accountpreference_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accountpreference_languageid_languages_id' AND table_name='accountpreference'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accountpreference 
			ADD CONSTRAINT fk_accountpreference_languageid_languages_id FOREIGN KEY (language_id) REFERENCES  translation.languages (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accountpreference_timezoneid_timezone_id' AND table_name='accountpreference'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accountpreference 
			ADD CONSTRAINT fk_accountpreference_timezoneid_timezone_id FOREIGN KEY (timezone_id) REFERENCES  master.timezone (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--accountblob  
CREATE TABLE if not exists  master.accountblob 
(
	id serial NOT NULL,      
	account_id int NOT NULL,      
	image_url varchar(255) NOT NULL,      
	image_type char(1) NOT NULL      
)
TABLESPACE pg_default;

ALTER TABLE  master.accountblob 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_accountblob_id' AND table_name='accountblob'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.accountblob 
			ADD CONSTRAINT pk_accountblob_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accountblob_accountid_account_id' AND table_name='accountblob'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accountblob 
			ADD CONSTRAINT fk_accountblob_accountid_account_id FOREIGN KEY (account_id) REFERENCES  master.account (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--accountsession 
CREATE TABLE if not exists  master.accountsession 
(
	id serial NOT NULL,      
	ip_address  varchar(20) NOT NULL,       
	last_session_refresh bigint NOT NULL,
	session_started_at bigint NOT NULL,      
	sessoin_expired_at bigint NOT NULL,      
	account_id int NOT NULL,      
	created_at bigint NOT NULL
)
TABLESPACE pg_default;

ALTER TABLE  master.accountsession 
    OWNER to pgadmin;

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

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accountsession_accountid_account_id' AND table_name='accountsession'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accountsession 
			ADD CONSTRAINT fk_accountsession_accountid_account_id FOREIGN KEY (account_id) REFERENCES  master.account (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--accounttoken 
CREATE TABLE if not exists  master.accounttoken 
(
	id serial NOT NULL,      
	user_name varchar(100) NOT NULL,      
	access_token varchar(200) NOT NULL,      
	expire_in int NOT NULL,      
	refresh_token varchar(200) ,      
	refresh_expire_in int ,      
	account_id int NOT NULL,      
	type char(1) NOT NULL,      
	session_id int NOT NULL,      
	scope varchar(50)  NOT NULL,      
	idp_type char(1) NOT NULL,      
	created_at bigint NOT NULL
)
TABLESPACE pg_default;

ALTER TABLE  master.accounttoken 
    OWNER to pgadmin;


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
	WHERE constraint_name='fk_accounttoken_accountid_account_id' AND table_name='accounttoken'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accounttoken 
			ADD CONSTRAINT fk_accounttoken_accountid_account_id FOREIGN KEY (account_id) REFERENCES  master.account (id);
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


--accountassertion 
CREATE TABLE if not exists  master.accountassertion 
(
	id serial NOT NULL,      
	key varchar(20) NOT NULL,      
	value varchar(100),      
	account_id int NOT NULL,      
	session_id int NOT NULL,      
	created_at bigint NOT NULL
)
TABLESPACE pg_default;

ALTER TABLE  master.accountassertion 
    OWNER to pgadmin;


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

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accountassertion_accountid_account_id' AND table_name='accountassertion'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accountassertion 
			ADD CONSTRAINT fk_accountassertion_accountid_account_id FOREIGN KEY (account_id) REFERENCES  master.account (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

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

--tcu
-- CREATE TABLE if not exists  master.tcu
-- (
	-- id serial not null,
	-- tcu_id varchar(50) not null,
	-- tcu_serial_number varchar(50) not null,
	-- brand varchar(50) ,
	-- version varchar(8) 
	-- )
-- TABLESPACE pg_default;

-- ALTER TABLE  master.tcu 
    -- OWNER to pgadmin;

-- Do $$
-- begin
-- if not exists(
	-- SELECT 1 FROM information_schema.table_constraints 
	-- WHERE constraint_name='pk_tcu_id' AND table_name='tcu'
		-- and constraint_type='PRIMARY KEY')
-- then	
	-- begin
		-- ALTER TABLE  master.tcu 
			-- ADD CONSTRAINT pk_tcu_id PRIMARY KEY (id)
			-- USING INDEX TABLESPACE pg_default;
	-- end;
-- end if;
-- end;
-- $$;

-- Do $$
-- begin
-- if not exists(
	-- SELECT 1 FROM information_schema.table_constraints 
	-- WHERE constraint_name='uk_tcu_tcuserialnumber' AND table_name='tcu_serial_number'
		-- and constraint_type='UNIQUE')
-- then	
	-- begin
		-- ALTER TABLE  master.tcu 
			-- ADD CONSTRAINT uk_tcu_tcuserialnumber UNIQUE  (tcu_serial_number)
			-- USING INDEX TABLESPACE pg_default;
	-- end;
-- end if;
-- end;
-- $$;


-- --tcuvehiclemapping
-- CREATE TABLE if not exists  master.tcuvehiclemapping 
-- (
	-- id serial not null,
	-- tcu_id 	int NOT NULL,      
 	-- vehicle_id 	int NOT NULL,      
	-- is_tcu_register boolean not null default true,      
	-- reference_date bigint
	-- )
-- TABLESPACE pg_default;

-- ALTER TABLE  master.tcuvehiclemapping 
    -- OWNER to pgadmin;

-- Do $$
-- begin
-- if not exists(
	-- SELECT 1 FROM information_schema.table_constraints 
	-- WHERE constraint_name='pk_tcuvehiclemapping_id' AND table_name='tcuvehiclemapping'
		-- and constraint_type='PRIMARY KEY')
-- then	
	-- begin
		-- ALTER TABLE  master.tcuvehiclemapping 
			-- ADD CONSTRAINT pk_tcuvehiclemapping_id PRIMARY KEY (id)
			-- USING INDEX TABLESPACE pg_default;
	-- end;
-- end if;
-- end;
-- $$;

-- Do $$
-- begin
-- if not exists(
	-- SELECT 1 FROM information_schema.table_constraints 
	-- WHERE constraint_name='fk_tcuvehiclemapping_vehicleid_vehicle_id' AND table_name='tcuvehiclemapping'
		-- and constraint_type='FOREIGN KEY')
-- then	
	-- begin
		-- ALTER TABLE  master.tcuvehiclemapping 
			-- ADD CONSTRAINT fk_tcuvehiclemapping_vehicleid_vehicle_id FOREIGN KEY (vehicle_id) REFERENCES  master.vehicle (id);
			-- --USING INDEX TABLESPACE pg_default;
	-- end;
-- end if;
-- end;
-- $$;

-- Do $$
-- begin
-- if not exists(
	-- SELECT 1 FROM information_schema.table_constraints 
	-- WHERE constraint_name='fk_tcuvehiclemapping_tcuid_tcu_id' AND table_name='tcuvehiclemapping'
		-- and constraint_type='FOREIGN KEY')
-- then	
	-- begin
		-- ALTER TABLE  master.tcuvehiclemapping 
			-- ADD CONSTRAINT fk_tcuvehiclemapping_tcuid_tcu_id FOREIGN KEY (tcu_id) REFERENCES  master.tcu (id);
			-- --USING INDEX TABLESPACE pg_default;
	-- end;
-- end if;
-- end;
-- $$;
