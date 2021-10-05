/* alter table master.notificationhistory alter column email_id type varchar(120) ;
alter table master.notificationrecipient alter column email_id type varchar(120) ; --250
alter table master.scheduledreportrecipient alter column email type varchar(120) ; --text
alter table master.driver alter column email type varchar(120) ; --100
alter table master.account alter column email type varchar(120) ; --50
*/

--select *,'update master.report set feature_id=''{'||feature_id||'}'' where id='||id||';' from master.report order by 1;

--alter table master.report drop constraint fk_report_featureid_feature_id;
--ALTER TABLE  master.report ALTER COLUMN feature_id TYPE INTEGER[] USING array[feature_id]::INTEGER[];

CREATE TABLE if not exists master.subreport 
(
	report_id  int not null,
	sub_report_id int not null,
	feature_id int not null
)	
TABLESPACE pg_default;

ALTER TABLE  master.subreport 
    OWNER to pgdbadmin;


Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_subreport_reportid_report_id' AND table_name='subreport'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.subreport
			ADD CONSTRAINT fk_subreport_reportid_report_id FOREIGN KEY (report_id) REFERENCES  master.report (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;

Do $$
begin
if not exists(
	SELECT 1 FROM information_schema.table_constraints 
	WHERE constraint_name='fk_subreport_featureid_feature_id' AND table_name='subreport'
		and constraint_type='FOREIGN KEY')
then	
	begin
		ALTER TABLE  master.subreport
			ADD CONSTRAINT fk_subreport_featureid_feature_id FOREIGN KEY (feature_id) REFERENCES  master.feature (id);
			--USING INDEX TABLESPACE pg_default;
	end;
end if;
end;
$$;



