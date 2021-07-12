
CREATE TABLE if not exists master.whitelistipdetails 
(
	id serial not null, 
	organization_id int not null ,
	account_id int not null,
	start_ip_range text,
	end_ip_range text
)	
TABLESPACE pg_default;

ALTER TABLE  master.whitelistipdetails  
    OWNER to pgdbdmadmin  ;
	
Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_whitelistipdetails_id' AND table_name='whitelistipdetails '
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.whitelistipdetails  
			ADD CONSTRAINT pk_whitelistipdetails_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists  master.idlingconsumption  
(
	id serial NOT NULL,   
	min_val decimal NOT NULL, 
	max_val decimal , 
	--description varchar(50) NOT NULL , 
	key varchar(100)
)
TABLESPACE pg_default;

ALTER TABLE master.idlingconsumption  
    OWNER to pgdbdmadmin  ;

Do $$

begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_idlingconsumption_id' AND table_name='idlingconsumption'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.idlingconsumption 
			ADD CONSTRAINT pk_idlingconsumption_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


CREATE TABLE if not exists  master.averagetrafficclassification  
(
	id serial NOT NULL,   
	min_val decimal NOT NULL, 
	max_val decimal , 
	--description varchar(50) NOT NULL ,
	key varchar(100)
)
TABLESPACE pg_default;

ALTER TABLE master.averagetrafficclassification  
    OWNER to pgdbdmadmin  ;

Do $$

begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_averagetrafficclassification_id' AND table_name='averagetrafficclassification'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.averagetrafficclassification 
			ADD CONSTRAINT pk_averagetrafficclassification_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

alter table master.reportattribute add column if not exists id serial not null;
alter table master.reportattribute add column if not exists sub_attribute_ids int[];
alter table master.reportattribute add column if not exists type char(1) ;
alter table master.reportattribute add column if not exists key varchar(100);
alter table master.reportattribute add column if not exists name varchar(50) ;
alter table master.reportpreference add column if not exists reportattribute_id  int;


alter table master.reportattribute drop constraint pk_reportdef_reportid_dataattributeid;
Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_reportattribute_id' AND table_name='reportattribute'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.reportattribute 
			ADD CONSTRAINT pk_reportattribute_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_reportpreference_reportattributeid_reportattribute_id' AND table_name='reportpreference'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.reportpreference 
			ADD CONSTRAINT fk_reportpreference_reportattributeid_reportattribute_id FOREIGN KEY (reportattribute_id) REFERENCES  master.reportattribute (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

--alter table  master.dataattribute drop column column_name_for_ui;
alter table master.dataattribute alter column name type varchar(250);



alter table master.ecoscorekpi add column if not exists data_attribute_id int ;
alter table master.report add column if not exists support_driver_sch_rep char(1) not null default 'N';

CREATE TABLE if not exists  master.country  
(
	id serial NOT NULL,   
	region_type char(1) , 
	code varchar(3) not null, 
	name varchar(50) NOT NULL 
)
TABLESPACE pg_default;

ALTER TABLE master.country  
    OWNER to pgdbdmadmin  ;

Do $$

begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_country_id' AND table_name='country'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.country 
			ADD CONSTRAINT pk_country_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

alter table master.ecoscoreprofilekpi alter column limit_val drop not null;
alter table master.scheduledreport add column if not exists is_mail_send boolean;
alter table master.scheduledreport add column if not exists file_name varchar(100);
