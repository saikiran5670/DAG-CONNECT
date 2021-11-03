CREATE TABLE if not exists master.accountmigration
(
	id serial not null, 
	account_id int not null,
	state char(1) not null
)	
TABLESPACE pg_default;

ALTER TABLE  master.accountmigration  
    OWNER to pgadmin;
	
Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_accountmigration_id' AND table_name='accountmigration'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.accountmigration  
			ADD CONSTRAINT pk_accountmigration_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_accountmigration_accountid_account_id' AND table_name='accountmigration'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.accountmigration 
			ADD CONSTRAINT fk_accountmigration_accountid_account_id FOREIGN KEY (account_id) REFERENCES  master.account (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;