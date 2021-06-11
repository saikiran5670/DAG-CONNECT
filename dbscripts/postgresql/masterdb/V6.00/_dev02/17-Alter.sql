--For sprint 3 (DB) and sprint 4 (Dev)

CREATE TABLE if not exists master.ecoscoreprofile 
(
	id serial not null, 
	organization_id  int , 
	name  varchar(50) not null, 
	description  varchar(100), 
	default_es_version_type char(1), 
	state  char(1) not null, 
	created_at  bigint not null, 
	created_by  int not null, 
	modified_at  bigint , 
	modified_by  int

)	
TABLESPACE pg_default;

ALTER TABLE  master.ecoscoreprofile  
    OWNER to pgdbadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_ecoscoreprofile_id' AND table_name='ecoscoreprofile '
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.ecoscoreprofile  
			ADD CONSTRAINT pk_ecoscoreprofile_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_ecoscoreprofile_organizationid_organization_id' AND table_name='ecoscoreprofile'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.ecoscoreprofile 
			ADD CONSTRAINT fk_ecoscoreprofile_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;



CREATE TABLE if not exists master.ecoscoreprofilekpi 
(
	id serial not null, 
	ecoscore_profile_id  int not null, 
	data_attribute_id  int not null, 
	limit_type  char(1) not null, 
	limit_val  decimal not null, 
	target_val  decimal not null, 
	lower_val  decimal not null, 
	upper_val  decimal not null, 
	created_at  bigint not null, 
	created_by  int not null, 
	modified_at  bigint , 
	modified_by  int
)	
TABLESPACE pg_default;

ALTER TABLE  master.ecoscoreprofilekpi  
    OWNER to pgdbadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_ecoscoreprofilekpi_id' AND table_name='ecoscoreprofilekpi '
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.ecoscoreprofilekpi  
			ADD CONSTRAINT pk_ecoscoreprofilekpi_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_ecoscoreprofilekpi_ecoscoreprofileid_ecoscoreprofile_id' AND table_name='ecoscoreprofilekpi'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.ecoscoreprofilekpi 
			ADD CONSTRAINT fk_ecoscoreprofilekpi_ecoscoreprofileid_ecoscoreprofile_id FOREIGN KEY (ecoscore_profile_id) REFERENCES  master.ecoscoreprofile (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_ecoscoreprofilekpi_dataattributeid_dataattribute_id' AND table_name='ecoscoreprofilekpi'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.ecoscoreprofilekpi 
			ADD CONSTRAINT fk_ecoscoreprofilekpi_dataattributeid_dataattribute_id FOREIGN KEY (data_attribute_id) REFERENCES  master.dataattribute (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

alter table master.schedulereport rename to reportscheduler;
alter table master.reportscheduler add column if not exists mail_subject varchar(120);
alter table master.reportscheduler add column if not exists mail_description varchar(250);
alter table master.reportscheduler add column if not exists report_dispatch_time bigint;
alter table master.scheduledreportrecipient drop column account_group_id;
alter table master.accountpreference add column if not exists icon_id int;
alter table master.reportpreference add column if not exists report_section_type char(1);
alter table master.reportpreference add column if not exists threshold_limit_type char(1);
alter table master.reportpreference add column if not exists threshold_value decimal;
--alter table master.reportpreference add column if not exists threshold_unit_type decimal;
alter table master.reportpreference add column if not exists threshold_frequency_type char(1);

alter table master.scheduledreport add column if not exists start_date bigint;
alter table master.scheduledreport add column if not exists end_date bigint;
--alter table master.scheduledreport drop column icon_id;
alter table master.reportscheduler drop column vehicle_group_id;
alter table master.reportscheduler drop column driver_id;

alter table master.reportpreference add column if not exists ecoscore_profile_id int;
alter table master.report add column if not exists feature_id int;
alter table master.ecoscoreprofilekpi drop column data_attribute_id;
--alter table master.dataattribute drop column limit_type;
--alter table master.dataattribute drop column kpi_type;
alter table master.ecoscoreprofilekpi add column if not exists ecoscore_kpi_id int not null;
alter table translation.enumtranslation add column if not exists feature_id int;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_report_featureid_feature_id' AND table_name='report'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.report 
			ADD CONSTRAINT fk_report_featureid_feature_id FOREIGN KEY (feature_id) REFERENCES  master.feature (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


CREATE TABLE if not exists master.scheduledreportvehicleref 
(
	
	report_schedule_id  int not null, 
	vehicle_group_id  int not null, 
	state char(1) not null,
	created_at  bigint not null, 
	created_by  int not null, 
	modified_at  bigint , 
	modified_by  int

)	
TABLESPACE pg_default;

ALTER TABLE  master.scheduledreportvehicleref  
    OWNER to pgdbadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_scheduledreportvehicleref_reportscheduleid_reportschedule_id' AND table_name='scheduledreportvehicleref'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.scheduledreportvehicleref 
			ADD CONSTRAINT fk_scheduledreportvehicleref_reportscheduleid_reportschedule_id FOREIGN KEY (report_schedule_id) REFERENCES  master.reportscheduler (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_scheduledreportvehicleref_vehiclegroupid_group_id' AND table_name='scheduledreportvehicleref'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.scheduledreportvehicleref 
			ADD CONSTRAINT fk_scheduledreportvehicleref_vehiclegroupid_group_id FOREIGN KEY (vehicle_group_id) REFERENCES  master.group (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;



CREATE TABLE if not exists master.scheduledreportdriverref 
(
	
	report_schedule_id  int not null, 
	driver_id  int not null, 
	state char(1) not null,
	created_at  bigint not null, 
	created_by  int not null, 
	modified_at  bigint , 
	modified_by  int

)	
TABLESPACE pg_default;

ALTER TABLE  master.scheduledreportdriverref  
    OWNER to pgdbadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_scheduledreportdriverref_reportscheduleid_reportschedule_id' AND table_name='scheduledreportdriverref'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.scheduledreportdriverref 
			ADD CONSTRAINT fk_scheduledreportdriverref_reportscheduleid_reportschedule_id FOREIGN KEY (report_schedule_id) REFERENCES  master.reportscheduler (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_scheduledreportdriverref_driverid_driver_id' AND table_name='scheduledreportdriverref'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.scheduledreportdriverref 
			ADD CONSTRAINT fk_scheduledreportdriverref_driverid_driver_id FOREIGN KEY (driver_id) REFERENCES  master.driver (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.ecoscoresection 
(
	
	id serial not null, 
	name  varchar(50) not null, 
	description varchar(500),
	key  varchar(100) not null,
	description_key varchar(100)
)	
TABLESPACE pg_default;

ALTER TABLE  master.ecoscoresection  
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_ecoscoresection_id' AND table_name='ecoscoresection '
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.ecoscoresection  
			ADD CONSTRAINT pk_ecoscoresection_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.ecoscorekpi 
(
	
	id serial not null, 
	name  varchar(50) not null, 
	key  varchar(100) not null,
	limit_type  char(1) , 
	range_value_type  char(1) not null, 
	max_upper_value decimal,
	section_id   int not null,
	unit_required_type char(1),
	seq_no int
)	
TABLESPACE pg_default;

ALTER TABLE  master.ecoscorekpi  
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_ecoscorekpi_id' AND table_name='ecoscorekpi '
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.ecoscorekpi  
			ADD CONSTRAINT pk_ecoscorekpi_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_ecoscorekpi_sectionid_ecoscoresection_id' AND table_name='ecoscorekpi'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.ecoscorekpi 
			ADD CONSTRAINT fk_ecoscorekpi_sectionid_ecoscoresection_id FOREIGN KEY (section_id) REFERENCES  master.ecoscoresection (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.ecoscoreprofileuserpreference 
(
	
	id serial not null, 
	organization_id  int not null, 
	account_id  int not null, 
	--profile_id  int , 
	kpi_id  int not null, 
	type  char(1) not null, 
	state  char(1) , 
	created_at  bigint, 
	modified_at  bigint
)	
TABLESPACE pg_default;

ALTER TABLE  master.ecoscoreprofileuserpreference  
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_ecoscoreprofileuserpreference_id' AND table_name='ecoscoreprofileuserpreference '
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.ecoscoreprofileuserpreference  
			ADD CONSTRAINT pk_ecoscoreprofileuserpreference_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_ecoscoreprofileuserpreference_organizationid_organization_id' AND table_name='ecoscoreprofileuserpreference'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.ecoscoreprofileuserpreference 
			ADD CONSTRAINT fk_ecoscoreprofileuserpreference_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_ecoscoreprofileuserpreference_accountid_account_id' AND table_name='ecoscoreprofileuserpreference'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.ecoscoreprofileuserpreference 
			ADD CONSTRAINT fk_ecoscoreprofileuserpreference_accountid_account_id FOREIGN KEY (account_id) REFERENCES  master.account (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


--Do $$
--begin
--if not exists(
--	SELECT 1 FROM information_schema.table_constraints 
--	WHERE constraint_name='fk_ecoscoreprofileuserpreference_profileid_ecoscoreprofile_id' AND table_name='ecoscoreprofileuserpreference'
--		and constraint_type='FOREIGN KEY')
--then	
--	begin
--		ALTER TABLE  master.ecoscoreprofileuserpreference 
--			ADD CONSTRAINT fk_ecoscoreprofileuserpreference_profileid_ecoscoreprofile_id FOREIGN KEY (profile_id) REFERENCES  master.ecoscoreprofile (id);
--			--USING INDEX TABLESPACE pg_default;
--	end;
--end if;
--end;
--$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_ecoscoreprofileuserpreference_kpiid_ecoscorekpi_id' AND table_name='ecoscoreprofileuserpreference'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.ecoscoreprofileuserpreference 
			ADD CONSTRAINT fk_ecoscoreprofileuserpreference_kpiid_ecoscorekpi_id FOREIGN KEY (kpi_id) REFERENCES  master.ecoscorekpi (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

ALTER TABLE MASTER.SCHEDULEDREPORT ADD COLUMN  IF NOT EXISTS START_DATE BIGINT;
ALTER TABLE MASTER.SCHEDULEDREPORT ADD COLUMN  IF NOT EXISTS END_DATE BIGINT;
ALTER TABLE MASTER.REPORTSCHEDULER DROP COLUMN ICON_ID;

