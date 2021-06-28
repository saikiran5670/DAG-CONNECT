
update master.feature set name='Configuration#VehicleManagement#VehicleConnectionSetting',
key='feat_vehiclemanagement_vehicleconnectionsetting' where id=461;

update translation.translation set name='feat_vehiclemanagement_vehicleconnectionsetting',value='Configuration#VehicleManagement#VehicleConnectionSetting'
where name='feat_vehiclemanagement_updatestatus';

update translation.translationgrouping set name='feat_vehiclemanagement_vehicleconnectionsetting' where name='feat_vehiclemanagement_updatestatus';

update master.feature set name='Configuration#UserAccount#DriverID',type='B'
where id=455;

update  master.menu set state='D' where id=7;

/*
--Owner ADMIN
INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
                             key, level,state)   
SELECT 520,'Admin.AccountManagement.OwnerAdmin','F',null,'feat_accountmanagement_owneradmin',40,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='Admin.AccountManagement.OwnerAdmin');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_accountmanagement_owneradmin','Admin.AccountManagement.OwnerAdmin',
(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_accountmanagement_owneradmin');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_accountmanagement_owneradmin',(select id from master.menu where name = 'Account Role Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_accountmanagement_owneradmin' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_accountmanagement_owneradmin',(select id from master.menu where name = 'Package Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_accountmanagement_owneradmin' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_accountmanagement_owneradmin',(select id from master.menu where name = 'Organisation Relationship Management'),'M'
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_accountmanagement_owneradmin' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));

--PackageCatalogueReport
INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
                             key, level,state)   
SELECT 521,'Admin.PackageManagement.PackageCatalogueReport','F',null,'feat_packagemanagement_packagecataloguereport',20,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='Admin.PackageManagement.PackageCatalogueReport');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_packagemanagement_packagecataloguereport','Admin.PackageManagement.PackageCatalogueReport',
(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_packagemanagement_packagecataloguereport');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_packagemanagement_packagecataloguereport',(select id from master.menu where name = 'Account Role Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_packagemanagement_packagecataloguereport' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_packagemanagement_packagecataloguereport',(select id from master.menu where name = 'Package Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_packagemanagement_packagecataloguereport' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_packagemanagement_packagecataloguereport',(select id from master.menu where name = 'Organisation Relationship Management'),'M'
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_packagemanagement_packagecataloguereport' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));

*/
--EcoScoreProfile
insert into master.ecoscoreprofile (organization_id,name,description,default_es_version_type,state,created_at,created_by,modified_at,modified_by)
select null,'Default Profile','The profile-default is used for default profile','D','A',(select extract(epoch from now()) * 1000),
(select id from master.account where lower(first_name) like '%global%' ),null,null
where not exists (select 1 from master.ecoscoreprofile where name='Default Profile') ;

insert into master.ecoscoreprofile (organization_id,name,description,default_es_version_type,state,created_at,created_by,modified_at,modified_by)
select null,'Advanced Default Profile','The profile-default-advanced is used for default profile advanced mode','A','A',(select extract(epoch from now()) * 1000),
(select id from master.account where lower(first_name) like '%global%' ),null,null
where not exists (select 1 from master.ecoscoreprofile where name='Advanced Default Profile') ;

insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'N',10,10,0,10,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Eco-Score') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Eco-Score'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'X',0,0,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Fuel Consumption') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Fuel Consumption'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'N',0,0,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Cruise Control Usage') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Cruise Control Usage'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'N',0,0,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Cruise Control Usage 30-50%') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Cruise Control Usage 30-50%'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'N',0,0,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Cruise Control Usage 50-75%') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Cruise Control Usage 50-75%'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'N',0,0,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Cruise Control Usage >75%') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Cruise Control Usage >75%'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'X',0,0,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='PTO Usage (%)') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='PTO Usage (%)'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'X',0,0,0,14400,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='PTO Duration') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='PTO Duration'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'X',0,0,0,30,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Average Driving Speed') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Average Driving Speed'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'X',0,0,0,90,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Average Speed') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Average Speed'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'X',73.1,48.9,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Heavy Throttling (%)') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Heavy Throttling (%)'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'X',10600,3560,0,14400,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Heavy Throttle Duration') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Heavy Throttle Duration'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'X',54.9,23.7,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Idling (%)') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Idling (%)'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'X',0,0,0,14400,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Idle Duration') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Idle Duration'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'N',5,7.5,0,10,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Braking Score') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Braking Score'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'X',0,0,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Harsh Braking (%)') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Harsh Braking (%)'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'X',0,0,0,14400,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Harsh Brake Duration') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Harsh Brake Duration'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'X',0,0,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Braking (%)') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Braking (%)'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'X',0,0,0,14400,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Brake Duration') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Brake Duration'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'N',5,7.5,0,10,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Anticipation Score') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Anticipation Score'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'',null,28.78,5.43,66.41,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Average Gross Weight') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Average Gross Weight'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Default Profile'),'',null,452.66,0.68,2337.46,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Average Distance per day') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Average Distance per day'));


insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'N',5,7.5,0,10,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Eco-Score') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Eco-Score'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'X',30.14,27.13,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Fuel Consumption') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Fuel Consumption'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'N',38.26,62.91,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Cruise Control Usage') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Cruise Control Usage'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'N',0,0,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Cruise Control Usage 30-50%') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Cruise Control Usage 30-50%'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'N',0,0,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Cruise Control Usage 50-75%') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Cruise Control Usage 50-75%'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'N',0,0,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Cruise Control Usage >75%') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Cruise Control Usage >75%'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'X',0,0,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='PTO Usage (%)') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='PTO Usage (%)'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'X',0,0,0,14400,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='PTO Duration') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='PTO Duration'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'X',90,60,60,90,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Average Driving Speed') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Average Driving Speed'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'X',0,0,0,90,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Average Speed') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Average Speed'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'X',6.88,4.25,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Heavy Throttling (%)') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Heavy Throttling (%)'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'X',0,0,0,14400,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Heavy Throttle Duration') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Heavy Throttle Duration'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'X',10.38,7.3,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Idling (%)') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Idling (%)'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'X',0,0,0,14400,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Idle Duration') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Idle Duration'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'N',5,7.5,0,10,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Braking Score') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Braking Score'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'X',18.18,12.63,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Harsh Braking (%)') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Harsh Braking (%)'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'X',0,0,0,14400,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Harsh Brake Duration') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Harsh Brake Duration'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'X',4.87,3.24,0,100,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Braking (%)') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Braking (%)'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'X',0,0,0,14400,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Brake Duration') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Brake Duration'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'N',5,7.5,0,10,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Anticipation Score') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Anticipation Score'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'',null,28.78,5.43,66.41,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Average Gross Weight') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Average Gross Weight'));
insert into master.ecoscoreprofilekpi (ecoscore_profile_id, limit_type, limit_val,target_val,lower_val,upper_val, created_at,created_by,modified_at,modified_by,ecoscore_kpi_id) select (select id from master.ecoscoreprofile where name='Advanced Default Profile'),'',null,452.66,0.68,2337.46,(select extract(epoch from now()) * 1000),(select min(id) from master.account where lower(first_name) like '%atos%'),null,null,(select id from master.ecoscorekpi where name ='Average Distance per day') where not exists (select 1 from master.ecoscoreprofilekpi where ecoscore_profile_id=(select id from master.ecoscoreprofile where name='Advanced Default Profile') and  ecoscore_kpi_id =(select id from master.ecoscorekpi where name ='Average Distance per day'));
	
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Report.CalendarView.ExpensionType'	,'A',	'da_report_calendarview_expensiontype'	,	
'Expension Type'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Report.CalendarView.ExpensionType'	);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_calendarview_expensiontype','Report.CalendarView.ExpensionType',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Report.CalendarView.ExpensionType');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_calendarview_expensiontype',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_calendarview_expensiontype' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),
(SELECT id from master.dataattribute WHERE name = 'Report.CalendarView.ExpensionType')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Report.CalendarView.ExpensionType'));


INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_details_stoptime','Report.Details.StopTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'da_report_details_stoptime');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_details_drivingtime','Report.Details.DrivingTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'da_report_details_drivingtime');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_details_averagedistanceperday','Report.Details.AverageDistancePerDay',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'da_report_details_averagedistanceperday');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_details_triptime','Report.Details.TripTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'da_report_details_triptime');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_details_vehiclename','Report.Details.VehicleName',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'da_report_details_vehiclename');



INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_calendarview_activevehicles',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_calendarview_activevehicles' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_calendarview_averageweight',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_calendarview_averageweight' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_calendarview_distance',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_calendarview_distance' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_calendarview_drivingtime',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_calendarview_drivingtime' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_calendarview_expensiontype',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_calendarview_expensiontype' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_calendarview_idleduration',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_calendarview_idleduration' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_calendarview_mileagebasedutilization',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_calendarview_mileagebasedutilization' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_calendarview_timebasedutilization',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_calendarview_timebasedutilization' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_calendarview_totaltrips',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_calendarview_totaltrips' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_charts_distanceperday',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_charts_distanceperday' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_charts_mileagebasedutilization',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_charts_distanceperday' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_charts_numberofvehiclesperday',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_charts_numberofvehiclesperday' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_charts_timebasedutilization',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_charts_timebasedutilization' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_averagespeed',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_averagespeed' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_averageweightpertrip',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_averageweightpertrip' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_distance',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_distance' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_idleduration',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_idleduration' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_numberoftrips',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_numberoftrips' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_odometer',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_odometer' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_registrationnumber',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_registrationnumber' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_vin',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_vin' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_general_averagedistanceperday',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_general_averagedistanceperday' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_general_idleduration',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_general_idleduration' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_general_numberoftrips',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_general_numberoftrips' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_general_numberofvehicles',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_general_numberofvehicles' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_general_totaldistance',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_general_totaldistance' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_stoptime',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_stoptime' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_drivingtime',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_drivingtime' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_averagedistanceperday',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_averagedistanceperday' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_triptime',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_triptime' 
and ref_id=0);

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_vehiclename',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_vehiclename' 
and ref_id=0);




