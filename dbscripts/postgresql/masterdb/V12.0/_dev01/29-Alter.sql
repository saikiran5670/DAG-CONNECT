/* alter table master.notificationhistory alter column email_id type varchar(120) ;
alter table master.notificationrecipient alter column email_id type varchar(120) ; --250
alter table master.scheduledreportrecipient alter column email type varchar(120) ; --text
alter table master.driver alter column email type varchar(120) ; --100
alter table master.account alter column email type varchar(120) ; --50
*/

--select *,'update master.report set feature_id=''{'||feature_id||'}'' where id='||id||';' from master.report order by 1;

--alter table master.report drop constraint fk_report_featureid_feature_id;
--ALTER TABLE  master.report ALTER COLUMN feature_id TYPE INTEGER[] USING array[feature_id]::INTEGER[];
/*
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
*/
ALTER TABLE  master.country add COLUMN dial_code varchar(8) ;

insert into master.country (region_type,code,name) select '','RU','Russia' where not exists (select 1 from master.country where name='Russia' and code ='RU');
insert into master.country (region_type,code,name) select '','ZA','South-Africa' where not exists (select 1 from master.country where name='South-Africa' and code ='ZA');
insert into master.country (region_type,code,name) select '','UA','Ukraine' where not exists (select 1 from master.country where name='Ukraine' and code ='UA');
insert into master.country (region_type,code,name) select '','BY','Belarus' where not exists (select 1 from master.country where name='Belarus' and code ='BY');
insert into master.country (region_type,code,name) select '','NO','Norway' where not exists (select 1 from master.country where name='Norway' and code ='NO');
insert into master.country (region_type,code,name) select '','NZ','New-Zealand' where not exists (select 1 from master.country where name='New-Zealand' and code ='NZ');
insert into master.country (region_type,code,name) select '','RS','Serbia' where not exists (select 1 from master.country where name='Serbia' and code ='RS');

update master.country set dial_code='+43' where name='AUSTRIA';
update master.country set dial_code='+32' where name='BELGIUM';
update master.country set dial_code='+359' where name='BULGARIA';
update master.country set dial_code='+41' where name='SWITZERLAND';
update master.country set dial_code='+357' where name='CYPRUS';
update master.country set dial_code='+420' where name='CZECH REPUBLIC';
update master.country set dial_code='+49' where name='GERMANY';
update master.country set dial_code='+45' where name='DENMARK';
update master.country set dial_code='+372' where name='ESTONIA';
update master.country set dial_code='+34' where name='SPAIN';
update master.country set dial_code='+358' where name='FINLAND';
update master.country set dial_code='+33' where name='FRANCE';
update master.country set dial_code='+44' where name='UNITED KINGDOM';
update master.country set dial_code='+30' where name='GREECE';
update master.country set dial_code='+385' where name='CROATIA';
update master.country set dial_code='+36' where name='HUNGARY';
update master.country set dial_code='+353' where name='IRELAND';
update master.country set dial_code='+39' where name='ITALY';
update master.country set dial_code='+370' where name='LITHUANIA';
update master.country set dial_code='+352' where name='LUXEMBOURG';
update master.country set dial_code='+371' where name='LATVIA';
update master.country set dial_code='+356' where name='MALTA';
update master.country set dial_code='+31' where name='NETHERLANDS';
update master.country set dial_code='+48' where name='POLAND';
update master.country set dial_code='+351' where name='PORTUGAL';
update master.country set dial_code='+40' where name='ROMANIA';
update master.country set dial_code='+46' where name='SWEDEN';
update master.country set dial_code='+386' where name='SLOVENIA';
update master.country set dial_code='+421' where name='SLOVAKIA';
update master.country set dial_code='+90' where name='Turkey';
update master.country set dial_code='+1' where name='UNITED STATES';
update master.country set dial_code='+7' where name='Russia';
update master.country set dial_code='+27' where name='South-Africa';
update master.country set dial_code='+380' where name='Ukraine';
update master.country set dial_code='+375' where name='Belarus';
update master.country set dial_code='+47' where name='Norway';
update master.country set dial_code='+64' where name='New-Zealand';
update master.country set dial_code='+381' where name='Serbia';

ALTER TABLE  master.country alter COLUMN dial_code set not null;

alter table master.timeformat add column if not exists code varchar(8);
alter table master.unit add column if not exists code varchar(8);
alter table master.vehicledisplay add column if not exists code varchar(8);
update master.timeformat set code='12H' where name='12 Hours';
update master.timeformat set code='24H' where name='24 Hours';
update master.unit set code='Metric' where name='Metric';
update master.unit set code='Imperial' where name='Imperial';
update master.vehicledisplay set code='Name' where name='Name';
update master.vehicledisplay set code='Regno' where name='Vehicle Registration Number';
update master.vehicledisplay set code='VIN' where name='Vehicle Identification Number';
alter table master.timeformat alter column code set not null;
alter table master.unit alter column code set not null;
alter table master.vehicledisplay alter column code set not null;

alter table master.orgrelationshipmapping drop constraint fk_orgrelationshipmapping_vehiclegroupid_group_id;

