--For sprint 2 (DB) and sprint 3 (Dev)
CREATE TABLE if not exists translation.enumtranslation
(
	id serial not null, 
	type  char(1) not null,
	enum char(1) not null,
	parent_enum char(1),
	key  varchar(100) not null
	
)
TABLESPACE pg_default;

ALTER TABLE  translation.enumtranslation 
    OWNER to pgadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_enumtranslation_id' AND table_name='enumtranslation'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  translation.enumtranslation 
			ADD CONSTRAINT pk_enumtranslation_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


CREATE TABLE if not exists master.alert
(
	id serial not null, 
	organization_id  int not null,
	name  varchar(50) not null,
	category  char(1) not null,
	type  char(1) not null,
	validity_period_type  char(1),
	validity_start_date  bigint ,
	validity_end_date  bigint,
	vehicle_group_id  int not null,
	state  char(1) not null,
	created_at  bigint not null,
	created_by  int not null,
	modified_at  bigint,
	modified_by  int
)
TABLESPACE pg_default;

ALTER TABLE  master.alert 
    OWNER to pgadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_alert_id' AND table_name='alert'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.alert 
			ADD CONSTRAINT pk_alert_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_alert_organizationid_organization_id' AND table_name='alert'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.alert 
			ADD CONSTRAINT fk_alert_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.alertlandmarkref
(
	id serial not null, 
	alert_id  int not null, 
	landmark_type  char(1) not null, 
	ref_id  int not null, 
	distance  decimal, 
	unit_type  char(1), 
	state  char(1) not null, 
	created_at  bigint not null, 
	modified_at  bigint
)
TABLESPACE pg_default;

ALTER TABLE  master.alertlandmarkref 
    OWNER to pgadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_alertlandmarkref_id' AND table_name='alertlandmarkref'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.alertlandmarkref 
			ADD CONSTRAINT pk_alertlandmarkref_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_alertlandmarkref_alertid_alert_id' AND table_name='alertlandmarkref'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.alertlandmarkref 
			ADD CONSTRAINT fk_alertlandmarkref_alertid_alert_id FOREIGN KEY (alert_id) REFERENCES  master.alert (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.alerturgencylevelref
(
	id serial not null, 
	alert_id  int not null, 
	urgency_level_type  char(1) not null, 
	threshold_value  decimal, 
	unit_type  char(1), 
	day_type  bit(7), 
	period_type  char(1), 
	urgencylevel_start_date  bigint, 
	urgencylevel_end_date  bigint, 
	state  char(1) not null, 
	created_at  bigint not null, 
	modified_at  bigint
)
TABLESPACE pg_default;

ALTER TABLE  master.alerturgencylevelref 
    OWNER to pgadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_alerturgencylevelref_id' AND table_name='alerturgencylevelref'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.alerturgencylevelref 
			ADD CONSTRAINT pk_alerturgencylevelref_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_alerturgencylevelref_alertid_alert_id' AND table_name='alerturgencylevelref'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.alerturgencylevelref 
			ADD CONSTRAINT fk_alerturgencylevelref_alertid_alert_id FOREIGN KEY (alert_id) REFERENCES  master.alert (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.alertfilterref
(
	id serial not null, 
	alert_id  int not null, 
	alert_urgency_level_id  int ,
	filter_type  char(1) not null, 
	threshold_value  decimal, 
	unit_type  char(1), 
	landmark_type  char(1), 
	ref_id int, 
	position_type  char(1), 
	day_type  bit(7), 
	period_type  char(1), 
	filter_start_date  bigint , 
	filter_end_date  bigint, 
	state  char(1) not null, 
	created_at  bigint not null, 
	modified_at  bigint

)
TABLESPACE pg_default;

ALTER TABLE  master.alertfilterref 
    OWNER to pgadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_alertfilterref_id' AND table_name='alertfilterref'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.alertfilterref 
			ADD CONSTRAINT pk_alertfilterref_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_alertfilterref_alertid_alert_id' AND table_name='alertfilterref'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.alertfilterref 
			ADD CONSTRAINT fk_alertfilterref_alertid_alert_id FOREIGN KEY (alert_id) REFERENCES  master.alert (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.notification
(
	id serial not null, 
	alert_id  int not null, 
	alert_urgency_level_type  char(1) , 
	frequency_type char(1) not null, 
	frequency_threshhold_value  int, 
	validity_type  char(1) not null, 
	state  char(1) not null, 
	created_at  bigint not null, 
	created_by  int not null, 
	modified_at  bigint, 
	modified_by  int
)
TABLESPACE pg_default;

ALTER TABLE  master.notification 
    OWNER to pgadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_notification_id' AND table_name='notification'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.notification 
			ADD CONSTRAINT pk_notification_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_notification_alertid_alert_id' AND table_name='notification'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.notification 
			ADD CONSTRAINT fk_notification_alertid_alert_id FOREIGN KEY (alert_id) REFERENCES  master.alert (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.notificationavailabilityperiod
(
	id serial not null, 
	notification_id  int not null,
	availability_period_type  char(1) not null ,
	period_type  char(1),
	start_time  bigint,
	end_time  bigint,
	state  char(1),
	created_at  bigint  not null,
	modified_at  bigint

)
TABLESPACE pg_default;

ALTER TABLE  master.notificationavailabilityperiod 
    OWNER to pgadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_notificationavailabilityperiod_id' AND table_name='notificationavailabilityperiod'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.notificationavailabilityperiod 
			ADD CONSTRAINT pk_notificationavailabilityperiod_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_notificationap_notificationid_notification_id' AND table_name='notificationavailabilityperiod'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.notificationavailabilityperiod 
			ADD CONSTRAINT fk_notificationap_notificationid_notification_id FOREIGN KEY (notification_id) REFERENCES  master.notification (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.notificationrecipient
(
	id serial not null, 
	notification_id  int not null, 
	recipient_label  varchar(50) not null, 
	account_group_id  int,
	notification_mode_type  char(1) not null, 
	phone_no  varchar(100),
	sms  text,
	email_id  varchar(250),
	email_sub  varchar(250),
	email_text  text,
	ws_url  varchar(250),
	ws_type  char(1),
	ws_text  text,
	ws_login  varchar(50),
	ws_password  varchar(50),
	state  char(1) not null, 
	created_at  bigint not null, 
	modified_at  bigint

)
TABLESPACE pg_default;

ALTER TABLE  master.notificationrecipient 
    OWNER to pgadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_notificationrecipient_id' AND table_name='notificationrecipient'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.notificationrecipient 
			ADD CONSTRAINT pk_notificationrecipient_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_notificationrecipient_notificationid_notification_id' AND table_name='notificationrecipient'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.notificationrecipient 
			ADD CONSTRAINT fk_notificationrecipient_notificationid_notification_id FOREIGN KEY (notification_id) REFERENCES  master.notification (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.notificationlimit
(
	id serial not null, 
	notification_id  int not null, 
	notification_mode_type  char(1) not null, 
	max_limit  int not null, 
	notification_period_type  char(1) not null, 
	period_limit  int not null, 
	state  char(1) not null, 
	created_at  bigint not null, 
	modified_at  bigint

)
TABLESPACE pg_default;

ALTER TABLE  master.notificationlimit 
    OWNER to pgadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_notificationlimit_id' AND table_name='notificationlimit'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.notificationlimit 
			ADD CONSTRAINT pk_notificationlimit_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_notificationlimit_notificationid_notification_id' AND table_name='notificationlimit'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.notificationlimit 
			ADD CONSTRAINT fk_notificationlimit_notificationid_notification_id FOREIGN KEY (notification_id) REFERENCES  master.notification (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.notificationtemplate
(
	id serial not null, 
	alert_category_type  char(1) not null, 
	alert_type  char(1) not null, 
	text  text not null, 
	created_at  bigint not null, 
	modified_at  bigint

)
TABLESPACE pg_default;

ALTER TABLE  master.notificationtemplate 
    OWNER to pgadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_notificationtemplate_id' AND table_name='notificationtemplate'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.notificationtemplate 
			ADD CONSTRAINT pk_notificationtemplate_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


CREATE TABLE if not exists master.corridorproperties
(
	id serial not null, 
	landmark_id int not null,
	is_transport_data boolean not null default false,
	is_traffic_flow boolean not null default false,
	no_of_trailers int,
	is_explosive boolean not null default false,
	is_gas boolean not null default false,
	is_flammable boolean not null default false,
	is_combustible boolean not null default false,
	is_organic boolean not null default false,
	is_poison boolean not null default false,
	is_radio_active boolean not null default false,
	is_corrosive boolean not null default false,
	is_poisonous_inhalation boolean not null default false,
	is_warm_harm boolean not null default false,
	is_other boolean not null default false,
	toll_road_type char(1),
	motorway_type char(1),
	boat_ferries_type char(1),
	rail_ferries_type char(1),
	tunnels_type char(1),
	dirt_road_type char(1),
	vehicle_height decimal,
	vehicle_width decimal,
	vehicle_length decimal,
	vehicle_limited_weight decimal,
	vehicle_weight_per_axle decimal,
	created_at bigint not null, 
	modified_at bigint 
)
TABLESPACE pg_default;

ALTER TABLE  master.corridorproperties 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_corridorproperties_id' AND table_name='corridorproperties'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.corridorproperties 
			ADD CONSTRAINT pk_corridorproperties_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_corridorproperties_landmarkid_landmark_id' AND table_name='corridorproperties'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.corridorproperties 
			ADD CONSTRAINT fk_corridorproperties_landmarkid_landmark_id FOREIGN KEY (landmark_id) REFERENCES  master.landmark (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.corridorviastop
(
	id serial not null, 
	landmark_id int not null,
	latitude double precision,
	longitude double precision,
	name varchar(100)
)	
TABLESPACE pg_default;

ALTER TABLE  master.corridorviastop 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_corridorviastop_id' AND table_name='corridorviastop'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.corridorviastop 
			ADD CONSTRAINT pk_corridorviastop_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_corridorviastop_landmarkid_landmark_id' AND table_name='corridorviastop'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.corridorviastop 
			ADD CONSTRAINT fk_corridorviastop_landmarkid_landmark_id FOREIGN KEY (landmark_id) REFERENCES  master.landmark (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;




CREATE TABLE if not exists master.corridortrips
(
	id serial not null, 
	landmark_id int not null,
	trip_id varchar(50),
	start_date bigint,
	end_date bigint,
	driver_id1 varchar(19),
	driver_id2 varchar(19),
	start_latitude double precision,
	start_longitude double precision,
	end_latitude double precision,
	end_longitude double precision,
	start_position varchar(100),
	end_position varchar(100),
	distance int
)	
TABLESPACE pg_default;

ALTER TABLE  master.corridortrips 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_corridortrips_id' AND table_name='corridortrips'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.corridortrips 
			ADD CONSTRAINT pk_corridortrips_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_corridortrips_landmarkid_landmark_id' AND table_name='corridortrips'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.corridortrips 
			ADD CONSTRAINT fk_corridortrips_landmarkid_landmark_id FOREIGN KEY (landmark_id) REFERENCES  master.landmark (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

alter table master.corridortrips add column if not exists state char(1);
update master.corridortrips set state='A';
alter table master.corridortrips alter column state set not null;

alter table master.landmark drop column if exists  trip_id;
alter table master.landmark add column if not exists  width int;
alter table master.landmark alter column city drop not null;
alter table master.landmark alter column category_id drop not null;

alter table master.nodes add column if not exists  trip_id varchar(50);
alter table master.nodes add column if not exists  address varchar(100);

update master.vehicle set vin=left(vin,17) where length(vin)>17;
ALTER TABLE master.vehicle ALTER COLUMN vin TYPE varchar(17) ; -- from 50 to 17
ALTER TABLE master.landmark ALTER COLUMN name TYPE varchar(100) ; --from 50 to 100

alter table master.corridorviastop add column if not exists state char(1);
update master.corridorviastop set state='A';
alter table master.corridorviastop alter column state set not null;

alter table master.accounttoken add column if not exists role_id int;
alter table master.accounttoken add column if not exists organization_id int;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accounttoken_roleid_role_id' AND table_name='accounttoken'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accounttoken 
			ADD CONSTRAINT fk_accounttoken_roleid_role_id FOREIGN KEY (role_id) REFERENCES  master.role (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accounttoken_organizationid_organization_id' AND table_name='accounttoken'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accounttoken 
			ADD CONSTRAINT fk_accounttoken_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

