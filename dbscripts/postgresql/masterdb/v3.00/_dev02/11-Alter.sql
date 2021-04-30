alter table master.passwordpolicy add column if not exists  is_reminder_sent boolean not null default false;
alter table auditlog.audittrail add column if not exists  role_id int;
alter table auditlog.audittrail add column if not exists organization_id int;
--alter table master.landmarkgroup add column if not exists description varchar(100)  ;
--alter table master.category add column if not exists description varchar(100)  ;
alter table master.dataattributeset alter column state set default 'A'; 

--For sprint 1 (DB) and sprint 2 (Dev)

CREATE TABLE if not exists master.category 
(
	id serial not null, 
	organization_id  int ,
	name varchar(100) not null, 
	icon_id  int null, 
	type char(1) not null,
	parent_id  int,
	state  char(1),
	created_at bigint not null, 
	created_by int not null, 
	modified_at bigint , 
	modified_by int
)
TABLESPACE pg_default;

ALTER TABLE  master.category 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_category_id' AND table_name='category'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.category 
			ADD CONSTRAINT pk_category_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_category_organizationid_organization_id' AND table_name='category'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.category 
			ADD CONSTRAINT fk_category_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


CREATE TABLE if not exists master.landmark
(
	id serial not null, 
	organization_id int ,
	category_id int not null, 
	sub_category_id int ,
	name varchar(50) not null, 
	address varchar(100), 
	city varchar(50) not null,
	country varchar(20), 
	zipcode varchar(20),  
	type  char(1) not null, 
	latitude decimal not null, 
	longitude decimal not null, 
	distance  decimal ,
	trip_id  int ,
	state char(1) not null default 'A', 
	created_at bigint not null, 
	created_by int not null, 
	modified_at bigint , 
	modified_by int
)
TABLESPACE pg_default;

ALTER TABLE  master.landmark 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_landmark_id' AND table_name='landmark'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.landmark 
			ADD CONSTRAINT pk_landmark_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_landmark_organizationid_organization_id' AND table_name='landmark'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.landmark 
			ADD CONSTRAINT fk_landmark_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_landmark_categoryid_category_id' AND table_name='landmark'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.landmark 
			ADD CONSTRAINT fk_landmark_categoryid_category_id FOREIGN KEY (category_id) REFERENCES  master.category (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_landmark_subcategoryid_category_id' AND table_name='landmark'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.landmark 
			ADD CONSTRAINT fk_landmark_subcategoryid_category_id FOREIGN KEY (sub_category_id) REFERENCES  master.category (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.nodes
(
	id serial not null, 
	landmark_id int not null,
	seq_no  int , 
	latitude  decimal , 
	longitude  decimal, 
	state char(1) not null default 'A', 
	created_at bigint not null, 
	created_by int not null, 
	modified_at bigint , 
	modified_by int
)
TABLESPACE pg_default;

ALTER TABLE  master.nodes 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_nodes_id' AND table_name='nodes'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.nodes 
			ADD CONSTRAINT pk_nodes_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_nodes_landmarkid_landmark_id' AND table_name='nodes'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.nodes 
			ADD CONSTRAINT fk_nodes_landmarkid_landmark_id FOREIGN KEY (landmark_id) REFERENCES  master.landmark (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


CREATE TABLE if not exists master.landmarkgroup
(
	id serial not null, 
	organization_id   int , 
	name  varchar(50)  , 
	icon_id  int, 
	state  char(1), 
	created_at bigint not null, 
	created_by int not null, 
	modified_at bigint , 
	modified_by int
)
TABLESPACE pg_default;

ALTER TABLE  master.landmarkgroup 
    OWNER to pgdbadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_landmarkgroup_id' AND table_name='landmarkgroup'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.landmarkgroup 
			ADD CONSTRAINT pk_landmarkgroup_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_landmarkgroup_organizationid_organization_id' AND table_name='landmarkgroup'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.landmarkgroup 
			ADD CONSTRAINT fk_landmarkgroup_organizationid_organization_id FOREIGN KEY (organization_id) REFERENCES  master.organization (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_landmarkgroup_iconid_icon_id' AND table_name='landmarkgroup'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.landmarkgroup 
			ADD CONSTRAINT fk_landmarkgroup_iconid_icon_id FOREIGN KEY (icon_id) REFERENCES  master.icon (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.landmarkgroupref
(
	id serial not null, 
	landmark_group_id  int not null, 
	type  char(1) not null , 
	ref_id  int not null
)
TABLESPACE pg_default;

ALTER TABLE  master.landmarkgroupref 
    OWNER to pgdbadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_landmarkgroupref_id' AND table_name='landmarkgroupref'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.landmarkgroupref 
			ADD CONSTRAINT pk_landmarkgroupref_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_landmarkgroupref_landmarkgroupid_landmarkgroup_id' AND table_name='landmarkgroupref'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.landmarkgroupref 
			ADD CONSTRAINT fk_landmarkgroupref_landmarkgroupid_landmarkgroup_id FOREIGN KEY (landmark_group_id) REFERENCES  master.landmarkgroup (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;


alter table master.landmarkgroup add column if not exists description varchar(100)  ;
alter table master.category add column if not exists description varchar(100)  ;



