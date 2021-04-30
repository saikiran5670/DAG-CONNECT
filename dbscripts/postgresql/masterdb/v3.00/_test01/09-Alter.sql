
Alter table master.account add column if not exists latest_accepted_tac_ver_no varchar(10) ;  

Alter table master.accountorg add column if not exists state char(1) ;
Alter table master.featureset add column if not exists state char(1) ;
Alter table master.account add column if not exists state char(1) ;
Alter table master.accountpreference add column if not exists state char(1) ;
Alter table master.role add column if not exists state char(1) ;
Alter table master.organization add column if not exists state char(1) ;
Alter table master.driver add column if not exists state char(1) ;
Alter table master.menu add column if not exists state char(1) ;
Alter table master.subscription add column if not exists state char(1) ;
Alter table master.feature add column if not exists state char(1) ;
Alter table master.dataattributeset add column if not exists state char(1) ;
Alter table master.orgrelationship add column if not exists state char(1) ;
Alter table master.package add column if not exists state char(1) ;
--Alter table master.feature add column if not exists status char(1) ;

update  master.accountorg set state ='D' where is_active=false;
update  master.featureset set state = 'D' where is_active=false;
update  master.account set state = 'D' where is_active=false;
update  master.accountpreference set state = 'D' where is_active=false;
update  master.role set state = 'D' where is_active=false;
update  master.organization set state = 'D' where is_active=false;
update  master.driver set state = 'D' where is_active=false;
update  master.menu set state = 'D' where is_active=false;
update  master.subscription set state = 'D' where is_active=false;
update  master.feature set state = 'D' where is_active=false;
update  master.dataattributeset set state = 'D' where is_active=false;
update  master.orgrelationship set state = 'D' where is_active=false;
update  master.package set state = 'D' where is_active=false;

update  master.accountorg set state ='A' where is_active=true;
update  master.featureset set state ='A' where is_active=true;
update  master.account set state ='A' where is_active=true;
update  master.accountpreference set state ='A' where is_active=true;
update  master.role set state ='A' where is_active=true;
update  master.organization set state ='A' where is_active=true;
update  master.driver set state ='A' where is_active=true;
update  master.menu set state ='A' where is_active=true;
update  master.subscription set state ='A' where is_active=true;
--update  master.feature set state ='A' where status='A';
update  master.dataattributeset set state ='A' where is_active=true;
update  master.orgrelationship set state ='A' where is_active=true;
update  master.package set state ='A' where status='A';

--update  master.feature update state ='I' where status='I';
update  master.package set state ='I' where status='I';

Alter table master.accountorg drop column if exists is_active;
Alter table master.featureset drop column if exists is_active;
Alter table master.account drop column if exists is_active;
Alter table master.accountpreference drop column if exists is_active;
Alter table master.role drop column if exists is_active;
Alter table master.organization drop column if exists is_active;
Alter table master.driver drop column if exists is_active;
Alter table master.menu drop column if exists is_active;
Alter table master.subscription drop column if exists is_active;
Alter table master.feature drop column if exists is_active;
Alter table master.dataattributeset drop column if exists is_active;
Alter table master.orgrelationship drop column if exists is_active;
Alter table master.package drop column if exists is_active;

Alter table master.feature drop column if exists status;
Alter table master.package drop column if exists status;


Alter table master.accountorg alter column state set not null ;
Alter table master.featureset alter column state set not null ;
Alter table master.account alter column state set not null ;
Alter table master.accountpreference alter column state set not null ;
Alter table master.role alter column state set not null ;
Alter table master.organization alter column state set not null ;
Alter table master.driver alter column state set not null ;
Alter table master.menu alter column state set not null ;
Alter table master.subscription alter column state set not null ;
Alter table master.feature alter column state set not null ;
Alter table master.dataattributeset alter column state set not null ;
Alter table master.orgrelationship alter column state set not null ;
Alter table master.package alter column  state set not null ;


--drop table master.landingpagedisplay;
alter table master.accountpreference drop constraint fk_accountpreference_landingpagedisplayid_landingpagedisplay_id; 
alter table   master.landingpagedisplay rename to landingpagedisplay_notinuse;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_landingpagedisplay_landingpagedisplayid_menu_id' AND table_name='accountpreference'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accountpreference 
			ADD CONSTRAINT fk_landingpagedisplay_landingpagedisplayid_menu_id FOREIGN KEY (landing_page_display_id) REFERENCES  master.menu (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Alter table master.role add column if not exists code varchar(50) ;  
update master.role set code='PLADM';
Alter table master.role alter column code set not null ;  

--Alter table master.organization add column if not exists description varchar(100) ;  
ALTER TABLE master.subscription ALTER COLUMN subscription_id drop not null;
update master.subscription set subscription_id =null;
ALTER TABLE master.subscription ALTER COLUMN subscription_id TYPE bigint USING subscription_id::bigint;
update master.subscription set subscription_id =id;
ALTER TABLE master.subscription ALTER COLUMN subscription_id set not null;
alter table auditlog.audittrail add column if not exists  role_id int;
alter table auditlog.audittrail add column if not exists organization_id int;

drop table master.warning;
drop table master.advice;

alter table master.menu add column if not exists sort_id int ;

update master.menu set sort_id=id where id<22;

update master.menu set sort_id=id+2 where id>=22;


CREATE TABLE if not exists master.icon 
(

	id  serial not null, 
	icon bytea , 
	type char(1) not null,
	warning_class int,
	warning_number int,
	name varchar(50),
	color_name char(1), 
	state char(1),
	created_at  bigint not null default extract(epoch from now()) * 1000, 
	created_by  int not null, 
	modified_at  bigint, 
	modified_by  int
)
TABLESPACE pg_default;

ALTER TABLE  master.icon 
    OWNER to pgdbmadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_icon_id' AND table_name='icon'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.icon 
			ADD CONSTRAINT pk_icon_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


CREATE TABLE if not exists master.dtcwarning 
(

	id  serial not null, 
	code  varchar(8) not null, 
	type  char(1), 
	veh_type  char(1), 
	class  int not null, 
	number int not null, 
	description text not null, 
	advice text not null, 
	expires_at  bigint, 
	icon_id int,
	created_at  bigint not null default extract(epoch from now()) * 1000, 
	created_by  int not null, 
	modified_at  bigint, 
	modified_by  int
)
TABLESPACE pg_default;

ALTER TABLE  master.dtcwarning 
    OWNER to pgdbmadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_dtcwarning_id' AND table_name='dtcwarning'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.dtcwarning 
			ADD CONSTRAINT pk_dtcwarning_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_dtcwarning_iconid_icon_id' AND table_name='dtcwarning'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.dtcwarning 
			ADD CONSTRAINT fk_dtcwarning_iconid_icon_id FOREIGN KEY (icon_id) REFERENCES  master.icon (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


--termsandcondition
--termsandcondition
CREATE TABLE if not exists master.termsandcondition 
(
	id serial not null, 
	version_no  varchar(10) not null,
	code  varchar(8) not null,
	description  bytea not null,
	state char(1) not null,
	start_date  bigint not null,
	end_date  bigint,
	created_at bigint not null default extract(epoch from now()) * 1000,
	created_by int not null,
	modified_at bigint,
	modified_by  int
)
TABLESPACE pg_default;

ALTER TABLE  master.termsandcondition 
    OWNER to pgdbmadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_termsandcondition_id' AND table_name='termsandcondition'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.termsandcondition 
			ADD CONSTRAINT pk_termsandcondition_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


CREATE TABLE if not exists master.accounttermsacondition 
(
	id serial not null, 
	organization_id  int not null,
	account_id  int not null,
	terms_and_condition_id int not null,
	accepted_date bigint null
	
)
TABLESPACE pg_default;

ALTER TABLE  master.accounttermsacondition 
    OWNER to pgdbmadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_accounttermsacondition_id' AND table_name='accounttermsacondition'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.accounttermsacondition 
			ADD CONSTRAINT pk_accounttermsacondition_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accounttermsacondition_organizationid_organization_id' AND table_name='accounttermsacondition'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accounttermsacondition 
			ADD CONSTRAINT fk_accounttermsacondition_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accounttermsacondition_accountid_account_id' AND table_name='accounttermsacondition'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accounttermsacondition 
			ADD CONSTRAINT fk_accounttermsacondition_accountid_account_id FOREIGN KEY (account_id) REFERENCES  master.account (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accounttermsacondition_tacid_termsandcondition_id' AND table_name='accounttermsacondition'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accounttermsacondition 
			ADD CONSTRAINT fk_accounttermsacondition_tacid_termsandcondition_id FOREIGN KEY (terms_and_condition_id) REFERENCES  master.termsandcondition (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


CREATE TABLE if not exists master.emailtemplate 
(
	id serial not null, 
	type  char(1) not null,
	feature_id  int ,
	event_name  varchar(50) not null,
	description  text not null,
	created_at  bigint not null default extract(epoch from now()) * 1000,
	created_by  int not null,
	modified_at bigint,
	modified_by  int
	 
	
)
TABLESPACE pg_default;

ALTER TABLE  master.emailtemplate 
    OWNER to pgdbmadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_emailtemplate_id' AND table_name='emailtemplate'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.emailtemplate 
			ADD CONSTRAINT pk_emailtemplate_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


CREATE TABLE if not exists master.emailtemplatelabels 
(
	id serial not null, 
	email_template_id  int not null,
	key  varchar(100) not null
 
	 
	
)
TABLESPACE pg_default;

ALTER TABLE  master.emailtemplatelabels 
    OWNER to pgdbmadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_emailtemplatelabels_id' AND table_name='emailtemplatelabels'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.emailtemplatelabels 
			ADD CONSTRAINT pk_emailtemplatelabels_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_emailtemplatelabels_emailtemplateid_emailtemplate_id' AND table_name='emailtemplatelabels'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.emailtemplatelabels 
			ADD CONSTRAINT fk_emailtemplatelabels_emailtemplateid_emailtemplate_id FOREIGN KEY (email_template_id) REFERENCES  master.emailtemplate (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

drop table master.passwordpolicy;

CREATE TABLE if not exists master.passwordpolicy
(
    id serial NOT NULL ,
    account_id integer NOT NULL,
    modified_at bigint,
    failed_login_attempts integer NOT NULL DEFAULT 0,
    locked_until bigint,
    account_lock_attempts integer NOT NULL DEFAULT 0,
    is_blocked boolean NOT NULL DEFAULT false,
    last_login bigint,
    CONSTRAINT passwordpolicy_pkey PRIMARY KEY (id),
    CONSTRAINT passwordpolicy_account_id_key UNIQUE (account_id)
)
TABLESPACE pg_default;

ALTER TABLE master.passwordpolicy
    OWNER to pgdbmadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_passwordpolicy_accountid_account_id' AND table_name='passwordpolicy'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.passwordpolicy 
			ADD CONSTRAINT fk_passwordpolicy_accountid_account_id FOREIGN KEY (account_id) REFERENCES  master.account (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

alter table master.passwordpolicy add column if not exists  is_reminder_sent boolean not null default false;

