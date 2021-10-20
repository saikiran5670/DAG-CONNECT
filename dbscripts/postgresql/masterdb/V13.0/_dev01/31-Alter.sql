--*********************--on dev1 only till 04-Oct-2021
CREATE TABLE if not exists master.otascheduledcompaign 
(
	id  serial not null,
	campaign_id varchar(100) not null,
	vin varchar(17) not null,
	scheduled_datetime bigint not null,
	created_at bigint not null,
	created_by bigint not null
)	
TABLESPACE pg_default;

ALTER TABLE  master.otascheduledcompaign 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_otascheduledcompaign_id' AND table_name='otascheduledcompaign'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.otascheduledcompaign 
			ADD CONSTRAINT pk_otascheduledcompaign_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

CREATE TABLE if not exists master.otacampaigncatching
(
	id  serial not null,
	campaign_id varchar(100) not null,
	release_notes text not null,
	code varchar(8) not null,
	created_at bigint not null,
	modified_at bigint 
)	
TABLESPACE pg_default;

ALTER TABLE  master.otacampaigncatching 
    OWNER to pgadmin;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='pk_otacampaigncatching_id' AND table_name='otacampaigncatching'
		and constraint_type='PRIMARY KEY')
then	
	begin
		ALTER TABLE  master.otacampaigncatching 
			ADD CONSTRAINT pk_otacampaigncatching_id PRIMARY KEY (id)
			USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

alter table master.otascheduledcompaign add column if not exists baseline uuid;
alter table master.otascheduledcompaign add column if not exists timestamp_boash_api bigint;
alter table master.otascheduledcompaign add column if not exists Status char(1);


--**************on dev1 only
alter table master.account alter column salutation drop not null;
alter table master.accountpreference alter column currency_id drop not null;