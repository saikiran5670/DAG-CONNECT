
alter table master.notificationrecipient drop column notification_id;

CREATE TABLE if not exists master.notificationrecipientref 
(
	
	id serial not null, 
	alert_id int not null ,
	notification_id  int not null, 
	recipient_id int not null ,
	state char(1) not null,
	created_at bigint,
	modified_at bigint
)	
TABLESPACE pg_default;

ALTER TABLE  master.notificationrecipientref  
    OWNER to pgdbmadmin;
	
Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_notificationrecipientref_id' AND table_name='notificationrecipientref '
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.notificationrecipientref  
			ADD CONSTRAINT pk_notificationrecipientref_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_notificationrecipientref_alertid_alert_id' AND table_name='notificationrecipientref'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.notificationrecipientref 
			ADD CONSTRAINT fk_notificationrecipientref_alertid_alert_id FOREIGN KEY (alert_id) REFERENCES  master.alert (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_notificationrecipientref_notificationid_notification_id' AND table_name='notificationrecipientref'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.notificationrecipientref 
			ADD CONSTRAINT fk_notificationrecipientref_notificationid_notification_id FOREIGN KEY (notification_id) REFERENCES  master.notification (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_notificationrecipientref_recipientid_notificationrec_id' AND table_name='notificationrecipientref'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.notificationrecipientref 
			ADD CONSTRAINT fk_notificationrecipientref_recipientid_notificationrec_id FOREIGN KEY (recipient_id) REFERENCES  master.notificationrecipient (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


CREATE TABLE if not exists master.notificationhistory 
(
	id serial not null, 
	organization_id int not null,
	trip_id varchar(50) not null,
	vehicle_id int not null,
	alert_id int not null,
	notification_id int not null,
	recipient_id int not null,
	notification_mode_type char(1) not null,
	phone_no  varchar(100),
	email_id varchar(250),
	ws_url varchar(250),
	notification_sent_date bigint not null,
	status char(1) not null
)	
TABLESPACE pg_default;

ALTER TABLE  master.notificationhistory  
    OWNER to pgdbmadmin;
	
Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_notificationhistory_id' AND table_name='notificationhistory '
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.notificationhistory  
			ADD CONSTRAINT pk_notificationhistory_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_notificationhistory_vehicleid_vehicle_id' AND table_name='notificationhistory'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.notificationhistory 
			ADD CONSTRAINT fk_notificationhistory_vehicleid_vehicle_id FOREIGN KEY (vehicle_id) REFERENCES  master.vehicle (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_notificationhistory_organizationid_organization_id' AND table_name='notificationhistory'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.notificationhistory 
			ADD CONSTRAINT fk_notificationhistory_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;





Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_notificationhistory_alertid_alert_id' AND table_name='notificationhistory'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.notificationhistory 
			ADD CONSTRAINT fk_notificationhistory_alertid_alert_id FOREIGN KEY (alert_id) REFERENCES  master.alert (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_notificationhistory_notificationid_notification_id' AND table_name='notificationhistory'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.notificationhistory 
			ADD CONSTRAINT fk_notificationhistory_notificationid_notification_id FOREIGN KEY (notification_id) REFERENCES  master.notification (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_notificationhistory_recipientid_notificationrec_id' AND table_name='notificationhistory'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.notificationhistory 
			ADD CONSTRAINT fk_notificationhistory_recipientid_notificationrec_id FOREIGN KEY (recipient_id) REFERENCES  master.notificationrecipient (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

alter table master.notificationlimit add column if not exists recipient_id int;



alter table master.reportpreference add column if not exists expension_type char(1);
alter table master.accountorg add column if not exists created_by int;

alter table master.scheduledreport add column if not exists is_mail_send boolean;
alter table master.scheduledreport add column if not exists file_name varchar(100);
alter table master.reportscheduler drop column file_name ;
ALTER TABLE master.role ALTER COLUMN feature_set_id DROP NOT NULL;
alter table master.ecoscoreprofilekpi alter column limit_val drop not null;

