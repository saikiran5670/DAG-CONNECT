--For sprint 3 (DB) and sprint 4 (Dev)

CREATE TABLE if not exists master.alerttimingdetail 
(
	id serial not null, 
	type  char(1) not null, 
	ref_id  int not null, 
	day_type  bit(7) not null, 
	period_type  char(1) not null, 
	start_date  bigint , 
	end_date  bigint, 
	state char(1) not null, 
	created_at  bigint not null, 
	modified_at  bigint
)	
TABLESPACE pg_default;

ALTER TABLE  master.alerttimingdetail  
    OWNER to pgadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_alerttimingdetail_id' AND table_name='alerttimingdetail '
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.alerttimingdetail  
			ADD CONSTRAINT pk_alerttimingdetail_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

drop table master.notificationavailabilityperiod;
alter table master.alertfilterref drop column if exists day_type;
alter table master.alertfilterref drop column if exists period_type;
alter table master.alertfilterref drop column if exists filter_start_date;
alter table master.alertfilterref drop column if exists filter_end_date;
alter table  master.reportdef rename to reportattribute;

CREATE TABLE if not exists master.schedulereport
(
	id serial not null, 
	organization_id int not null,
	report_id int not null,
	vehicle_group_id int ,
	driver_id int,
	frequency_type char(1) not null,
	status char(1) not null,
	type char(1) not null,
	file_name varchar(100),
	start_date bigint ,
	end_date bigint,
	icon_id int ,
	code char(8),
	last_schedule_run_date bigint,
	next_schedule_run_date bigint,
	created_at bigint not null,
	created_by int not null,
	modified_at bigint not null,
	modified_by int not null
)	
TABLESPACE pg_default;

ALTER TABLE  master.schedulereport 
    OWNER to pgadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_schedulereport_id' AND table_name='schedulereport'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.schedulereport 
			ADD CONSTRAINT pk_schedulereport_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_schedulereport_organizationid_organization_id' AND table_name='schedulereport'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.schedulereport 
			ADD CONSTRAINT fk_schedulereport_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_schedulereport_iconid_icon_id' AND table_name='schedulereport'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.schedulereport 
			ADD CONSTRAINT fk_schedulereport_iconid_icon_id FOREIGN KEY (icon_id) REFERENCES  master.icon (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_schedulereport_vehiclegroupid_group_id' AND table_name='schedulereport'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.schedulereport 
			ADD CONSTRAINT fk_schedulereport_vehiclegroupid_group_id FOREIGN KEY (vehicle_group_id) REFERENCES  master.group (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_schedulereport_driverid_driver_id' AND table_name='schedulereport'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.schedulereport 
			ADD CONSTRAINT fk_schedulereport_driverid_driver_id FOREIGN KEY (driver_id) REFERENCES  master.driver (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


CREATE TABLE if not exists master.scheduledreportrecipient
(
	id serial not null, 
	schedule_report_id int not null,
	account_group_id int not null,
	email text ,
	state char(1) not null,
	created_at bigint not null,
	modified_at bigint not null
)	
TABLESPACE pg_default;

ALTER TABLE  master.scheduledreportrecipient 
    OWNER to pgadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_scheduledreportrecipient_id' AND table_name='scheduledreportrecipient'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.scheduledreportrecipient 
			ADD CONSTRAINT pk_scheduledreportrecipient_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_scheduledreportrecipient_schedulereportid_schedulereport_id' AND table_name='scheduledreportrecipient'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.scheduledreportrecipient 
			ADD CONSTRAINT fk_scheduledreportrecipient_schedulereportid_schedulereport_id FOREIGN KEY (schedule_report_id) REFERENCES  master.schedulereport (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_scheduledreportrecipient_accountgroupid_group_id' AND table_name='scheduledreportrecipient'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.scheduledreportrecipient 
			ADD CONSTRAINT fk_scheduledreportrecipient_accountgroupid_group_id FOREIGN KEY (account_group_id) REFERENCES  master.group (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


CREATE TABLE if not exists master.scheduledreport
(
	id serial not null, 
	schedule_report_id int not null,
	report bytea not null,
	token uuid,
	downloaded_at bigint,
	valid_till bigint not null,
	created_at bigint not null
)	
TABLESPACE pg_default;

ALTER TABLE  master.scheduledreport 
    OWNER to pgadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_scheduledreport_id' AND table_name='scheduledreport'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.scheduledreport 
			ADD CONSTRAINT pk_scheduledreport_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_scheduledreport_schedulereportid_schedulereport_id' AND table_name='scheduledreport'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.scheduledreport 
			ADD CONSTRAINT fk_scheduledreport_schedulereportid_schedulereport_id FOREIGN KEY (schedule_report_id) REFERENCES  master.schedulereport (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

alter table master.report add column if not exists key char(100) not null;

drop table master.reportpreference;

CREATE TABLE if not exists master.reportpreference
(
    id serial not null,
	organization_id int not null,	
	account_id int not null,
	report_id int not null,
	--report_section_type char(1) not null,
    type char(1) not null,
    data_attribute_id int NOT NULL,
	state char(1) not null,
	chart_type char(1),
	created_at bigint,
	modified_at bigint
)	
TABLESPACE pg_default;

ALTER TABLE  master.reportpreference 
    OWNER to pgadmin;


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
	WHERE constraint_name='fk_reportpreference_organizationid_organization_id' AND table_name='reportpreference'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.reportpreference 
			ADD CONSTRAINT fk_reportpreference_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
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


ALTER TABLE master.dataattribute ADD COLUMN IF NOT EXISTS column_name_for_ui VARCHAR(150);

ALTER TABLE master.notificationtemplate  ADD COLUMN IF NOT EXISTS subject VARCHAR(150);


