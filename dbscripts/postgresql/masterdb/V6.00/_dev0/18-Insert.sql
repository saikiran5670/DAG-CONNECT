--Eco Score Functional
INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
							 key, level,state)   
SELECT 311,'Report.ECOScoreReport.Advance','F',null,'feat_report_ecoscorereport_advance',40,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='Report.ECOScoreReport.Advance');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_report_ecoscorereport_advance','Report.ECOScoreReport.Advance',
(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_report_ecoscorereport_advance');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_report_ecoscorereport_advance',(select id from master.menu where name = 'Account Role Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_report_ecoscorereport_advance' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_report_ecoscorereport_advance',(select id from master.menu where name = 'Package Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_report_ecoscorereport_advance' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_report_ecoscorereport_advance',(select id from master.menu where name = 'Organisation Relationship Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_report_ecoscorereport_advance' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));

--Eco Score Behavrial
INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
							 key, level,state)   
SELECT 312,'Report#ECOScoreReport#Manage','B',null,'feat_report_ecoscore_manage',30,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='Report#ECOScoreReport#Manage');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_report_ecoscore_manage','Report#ECOScoreReport#Manage',
(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_report_ecoscore_manage');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_report_ecoscore_manage',(select id from master.menu where name = 'Account Role Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_report_ecoscore_manage' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_report_ecoscore_manage',(select id from master.menu where name = 'Package Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_report_ecoscore_manage' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_report_ecoscore_manage',(select id from master.menu where name = 'Organisation Relationship Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_report_ecoscore_manage' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));

--Eco Score Behavrial
INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
							 key, level,state)   
SELECT 313,'Report#ECOScoreReport#Usage','B',null,'feat_report_ecoscore_usage',40,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='Report#ECOScoreReport#Usage');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_report_ecoscore_usage','Report#ECOScoreReport#Usage',
(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_report_ecoscore_manage');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_report_ecoscore_usage',(select id from master.menu where name = 'Account Role Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_report_ecoscore_usage' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));


INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_report_ecoscore_usage',(select id from master.menu where name = 'Package Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_report_ecoscore_usage' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_report_ecoscore_usage',(select id from master.menu where name = 'Organisation Relationship Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_report_ecoscore_usage' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));

update translation.translationgrouping set type = 'M' where upper(type) <> 'M';


INSERT INTO master.report  (name , key)   
SELECT 'Vehicle Health','lblVehicleHealth' WHERE NOT EXISTS  (   SELECT 1   FROM master.report  WHERE name = 'Vehicle Health');

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'VehicleHealth.RefreshedScreenAfter'	,'A',	'da_vehicle_health_refreshed_screen_after'	,	'Refreshed Screen After'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'VehicleHealth.RefreshedScreenAfter'	);

insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'vehicle health'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'vehiclehealth.refreshedscreenafter')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'vehicle health') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'vehiclehealth.refreshedscreenafter'));

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','da_vehicle_health_refreshed_screen_after','Refreshed Screen After',
(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'da_vehicle_health_refreshed_screen_after');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_vehicle_health_refreshed_screen_after',(select id from master.menu where name = 'Fleet Overview'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_vehicle_health_refreshed_screen_after' 
and ref_id=(select id from master.menu where name = 'Fleet Overview'));


update master.report set feature_id=	301 where name='Trip Report';
update master.report set feature_id=	302 where name='Trip Tracing';
update master.report set feature_id=	303 where name='Advanced Fleet Fuel Report';
update master.report set feature_id=	304 where name='Fleet Fuel Report';
update master.report set feature_id=	305 where name='Fleet Utilisation Report';
update master.report set feature_id=	306 where name='Fuel Benchmarking';
update master.report set feature_id=	307 where name='Fuel Deviation Report';
update master.report set feature_id=	308 where name='Vehicle Performance Report';
update master.report set feature_id=	309 where name='Drive Time Management';
update master.report set feature_id=	310 where name='Eco-Score Report';

update translation.enumtranslation set feature_id=401 where key='enumtype_enteringzone';
update translation.enumtranslation set feature_id=402 where key='enumtype_exitingzone';
update translation.enumtranslation set feature_id=403 where key='enumtype_exitingcorridor';
update translation.enumtranslation set feature_id=404 where key='enumtype_excessiveunderutilizationindays(batch)';
update translation.enumtranslation set feature_id=406 where key='enumtype_excessivedistancedone(trip)';
update translation.enumtranslation set feature_id=407 where key='enumtype_excessivedrivingduration(trip)';
update translation.enumtranslation set feature_id=408 where key='enumtype_excessiveglobalmileage(trip)';
update translation.enumtranslation set feature_id=409 where key='enumtype_hoursofservice(realtime)';
update translation.enumtranslation set feature_id=410 where key='enumtype_excessiveaveragespeed(realtime)';
update translation.enumtranslation set feature_id=405 where key='enumtype_excessiveunderutilizationinhours(batch)';
update translation.enumtranslation set feature_id=411 where key='enumtype_excessiveidling(realtime)';
update translation.enumtranslation set feature_id=412 where key='enumtype_fuelconsumed';
update translation.enumtranslation set feature_id=413 where key='enumtype_fuelincreaseduringstop(realtime)';
update translation.enumtranslation set feature_id=414 where key='enumtype_fuellossduringstop(realtime)';
update translation.enumtranslation set feature_id=415 where key='enumtype_fuellossduringtrip(realtime)';
update translation.enumtranslation set feature_id=416 where key='enumtype_statuschangetostopnow';
update translation.enumtranslation set feature_id=417 where key='enumtype_statuschangetoservicenow';

update master.menu set sort_id=sort_id+1 where sort_id>23;

INSERT INTO master.feature  (id, name , type ,  data_attribute_set_id , key, level,state)   
SELECT 460,'Configuration.EcoScoreProfileManagement','F',null,'feat_config_ecoscoreprofilemanagement',30,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='Configuration.EcoScoreProfileManagement');

insert into master.menu (id ,name ,parent_id ,  feature_id,seq_no,key,state,url,sort_id) 
select 44,'EcoScore Profile Management',(select id from master.menu where name='Configuration'),
(select id from master.feature where name='Configuration.EcoScoreProfileManagement'),8,'lblEcoScoreProfileManagement','A','ecoscoreprofilemanagement',24 
WHERE NOT EXISTS  (   SELECT 1   FROM master.menu   WHERE name ='EcoScore Profile Management');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','feat_config_ecoscoreprofilemanagement','Configuration.EcoScoreProfileManagement',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Configuration.EcoScoreProfileManagement');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_config_ecoscoreprofilemanagement',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_config_ecoscoreprofilemanagement' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_config_ecoscoreprofilemanagement',(select id from master.menu where name = 'Account Role Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_config_ecoscoreprofilemanagement' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_config_ecoscoreprofilemanagement',(select id from master.menu where name = 'Package Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_config_ecoscoreprofilemanagement' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_config_ecoscoreprofilemanagement',(select id from master.menu where name = 'Organisation Relationship Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_config_ecoscoreprofilemanagement' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));

INSERT INTO master.ecoscoresection (name  , key  , description , description_key  )
select 'Eco-Score','es_ecoscore','The Eco-Score represents the Overall Performance of the driver. The Eco-Score is the rounded down average of the Anticipation and Braking Scores. The Fuel Consumption is not part of the Eco-Score calculation.','es_ecoscore_desc'
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscoresection  WHERE name = 'Eco-Score' );

INSERT INTO master.ecoscoresection (name  , key  , description , description_key  )
select 'Fuel Consumption','es_fuel_consumption','The Fuel Consumption is not part of the Eco-Score calculation.','es_fuel_consumption_desc'
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscoresection  WHERE name = 'Fuel Consumption' );

INSERT INTO master.ecoscoresection (name  , key  , description , description_key)
select 'Braking Score','es_braking_score','The Eco-Score is the rounded down average of the  Braking Scores.','es_braking_score_desc'
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscoresection  WHERE name = 'Braking Score' );

INSERT INTO master.ecoscoresection (name  , key    , description , description_key)
select 'Anticipation Score','es_anticipation_score','The Eco-Score is the rounded down average of the Anticipation score','es_anticipation_score_desc'
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscoresection  WHERE name = 'Anticipation Score' );

INSERT INTO master.ecoscoresection (name  , key    , description , description_key)
select 'Other Fuel Consumption Indicators','es_other_fuel_consumption_indicators','The other  Fuel Consumption is not part of the Eco-Score calculation.','es_other_fuel_consumption_indicators_desc'
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscoresection  WHERE name = 'Other Fuel Consumption Indicators' );

INSERT INTO master.ecoscorekpi (name  ,  key  ,  limit_type  , range_value_type  , max_upper_value, section_id  , unit_required_type, seq_no)
select 'Eco-Score','es_ecoscore','N','D',100,(select id from master.ecoscoresection where lower(name ) like '%eco-score%'),'N',1
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Eco-Score' );

INSERT INTO master.ecoscorekpi (name  ,  key  ,  limit_type  , range_value_type  , max_upper_value, section_id  , unit_required_type, seq_no)
select 'Fuel Consumption','es_fuel_consumption','X','D',null,(select id from master.ecoscoresection where lower(name ) like 'fuel consumpti%'),'Y',	1
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Fuel Consumption' );

INSERT INTO master.ecoscorekpi (name    , key  ,  limit_type  , range_value_type  , max_upper_value, section_id  , unit_required_type, seq_no)
select 'Cruise Control Usage','es_cruise_control_usage','N','D',100,(select id from master.ecoscoresection where lower(name ) like 'fuel consumpti%'),'N',2
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Cruise Control Usage' );

INSERT INTO master.ecoscorekpi (name    , key  ,  limit_type  , range_value_type  , max_upper_value, section_id  , unit_required_type, seq_no)
select 'Cruise Control Usage 30-50%','es_cruise_control_usage_30_50','N','D',100,(select id from master.ecoscoresection where lower(name ) like 'fuel consumpti%'),'Y',3
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Cruise Control Usage 30-50%' );

INSERT INTO master.ecoscorekpi (name   , key  ,  limit_type  , range_value_type  , max_upper_value, section_id  , unit_required_type, seq_no)
select 'Cruise Control Usage 50-75%','es_cruise_control_usage_50_75','N','D',100,(select id from master.ecoscoresection where lower(name ) like 'fuel consumpti%'),'Y',4
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Cruise Control Usage 50-75%' );

INSERT INTO master.ecoscorekpi (name   , key  ,  limit_type  , range_value_type  , max_upper_value, section_id  , unit_required_type, seq_no)
select 'Cruise Control Usage >75%','es_cruise_control_usage_75_100','N','D',100,(select id from master.ecoscoresection where lower(name ) like 'fuel consumpti%'),'Y',5
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Cruise Control Usage >75%%' );

INSERT INTO master.ecoscorekpi (name    , key  ,  limit_type  , range_value_type  , max_upper_value, section_id  , unit_required_type, seq_no)
select 'PTO Usage (%)','es_pto_usage','X','D',100,(select id from master.ecoscoresection where lower(name ) like 'fuel consumpti%'),'N',6
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'PTO Usage (%)' );

INSERT INTO master.ecoscorekpi (name    , key  ,  limit_type  , range_value_type  , max_upper_value, section_id , unit_required_type, seq_no)
select 'PTO Duration','es_pto_duration','X','T',14400,(select id from master.ecoscoresection where lower(name ) like 'fuel consumpti%'),'Y',7
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'PTO Duration' );

INSERT INTO master.ecoscorekpi (name    , key  ,  limit_type  , range_value_type  , max_upper_value, section_id , unit_required_type, seq_no)
select 'Average Driving Speed','es_average_diving_speed','X','D',200,(select id from master.ecoscoresection where lower(name ) like 'fuel consumpti%'),'Y',8
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Average Driving Speed' );

INSERT INTO master.ecoscorekpi (name    , key  ,  limit_type  , range_value_type  , max_upper_value, section_id , unit_required_type, seq_no)
select 'Average Speed','es_average_speed','X','D',200,(select id from master.ecoscoresection where lower(name ) like 'fuel consumpti%'),'Y',9
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Average Speed' );

INSERT INTO master.ecoscorekpi (name    , key  ,  limit_type  , range_value_type  , max_upper_value, section_id , unit_required_type, seq_no)
select 'Heavy Throttling (%)','es_heavy_throttling','X','D',100,(select id from master.ecoscoresection where lower(name ) like 'fuel consumpti%'),'N',10
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Heavy Throttling (%)' );

INSERT INTO master.ecoscorekpi (name    , key  ,  limit_type  , range_value_type  , max_upper_value, section_id , unit_required_type, seq_no)
select 'Heavy Throttle Duration','es_heavy_throttle_duration','X','T',14400,(select id from master.ecoscoresection where lower(name ) like 'fuel consumpti%'),'Y',11
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Heavy Throttle Duration' );

INSERT INTO master.ecoscorekpi (name  , key  ,  limit_type  , range_value_type  , max_upper_value, section_id, unit_required_type, seq_no)
select 'Idling (%)','es_idling','X','D',100,(select id from master.ecoscoresection where lower(name ) like 'fuel consumpti%'),'N',12
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Idling (%)' );

INSERT INTO master.ecoscorekpi (name    , key  ,  limit_type  , range_value_type  , max_upper_value, section_id  , unit_required_type, seq_no)
select 'Idle Duration','es_idle_duration','X','T',14400,(select id from master.ecoscoresection where lower(name ) like 'fuel consumpti%'),'Y',13
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Idle Duration' );

INSERT INTO master.ecoscorekpi (name    , key  ,  limit_type  , range_value_type  , max_upper_value, section_id , unit_required_type, seq_no)
select 'Braking Score','es_braking_score','N','D',100,(select id from master.ecoscoresection where lower(name ) like '%braking scor%'),'N',1
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Braking Score' );

INSERT INTO master.ecoscorekpi (name    , key  ,  limit_type  , range_value_type  , max_upper_value, section_id , unit_required_type, seq_no)
select 'Harsh Braking (%)','es_harsh_braking','X','D',100,(select id from master.ecoscoresection where lower(name ) like '%braking scor%'),'N',2
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Harsh Braking (%)' );

INSERT INTO master.ecoscorekpi (name    , key  ,  limit_type  , range_value_type  , max_upper_value, section_id , unit_required_type, seq_no)
select 'Harsh Brake Duration','es_harsh_brake_duration','X','T',14400,(select id from master.ecoscoresection where lower(name ) like '%braking scor%'),'Y',3
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Harsh Brake Duration' );

INSERT INTO master.ecoscorekpi (name    , key  ,  limit_type  , range_value_type  , max_upper_value, section_id , unit_required_type, seq_no)
select 'Braking (%)','es_braking','X','D',100,(select id from master.ecoscoresection where lower(name ) like '%braking scor%'),'N',4
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Braking (%)' );

INSERT INTO master.ecoscorekpi (name    , key  ,  limit_type  , range_value_type  , max_upper_value, section_id , unit_required_type, seq_no)
select 'Brake Duration','es_brake_duration','X','T',14400,(select id from master.ecoscoresection where lower(name ) like '%braking scor%'),'Y',5
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Brake Duration' );

INSERT INTO master.ecoscorekpi (name    , key  ,  limit_type  , range_value_type  , max_upper_value, section_id , unit_required_type, seq_no)
select 'Anticipation Score','es_anticipation_score','N','D',100,(select id from master.ecoscoresection where lower(name ) like '%anticipation scor%'),'N',1
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Anticipation Score' );

INSERT INTO master.ecoscorekpi (name    , key  ,  limit_type  , range_value_type  , max_upper_value, section_id , unit_required_type, seq_no)
select 'Average Gross Weight','es_average_gross_weight',null,'D',70,(select id from master.ecoscoresection where lower(name ) like '%other fuel consumption indicato%'),'N',1
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Average Gross Weight' );

INSERT INTO master.ecoscorekpi (name    , key  ,  limit_type  , range_value_type  , max_upper_value, section_id , unit_required_type, seq_no)
select 'Average Distance per day','es_average_distance_per_day',null,'D',3000,(select id from master.ecoscoresection where lower(name ) like '%other fuel consumption indicato%'),'N',2
WHERE NOT EXISTS  (   SELECT 1   FROM master.ecoscorekpi  WHERE name = 'Average Distance per day' );

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_ecoscore','Eco-Score',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Eco-Score');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_ecoscore',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_ecoscore' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_fuel_consumption','Fuel Consumption',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Fuel Consumption');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_fuel_consumption',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_fuel_consumption' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_braking_score','Braking Score',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Braking Score');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_braking_score',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_braking_score' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_anticipation_score','Anticipation Score',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Anticipation Score');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_anticipation_score',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_anticipation_score' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_other_fuel_consumption_indicators','Other Fuel Consumption Indicators',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Other Fuel Consumption Indicators');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_other_fuel_consumption_indicators',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_other_fuel_consumption_indicators' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_cruise_control_usage','Cruise Control Usage',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Cruise Control Usage');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_cruise_control_usage',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_cruise_control_usage' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_cruise_control_usage_30_50','Cruise Control Usage 30-50%',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Cruise Control Usage 30-50%');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_cruise_control_usage_30_50',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_cruise_control_usage_30_50' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_cruise_control_usage_50_75','Cruise Control Usage 50-75%',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Cruise Control Usage 50-75%');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_cruise_control_usage_50_75',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_cruise_control_usage_50_75' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_cruise_control_usage_75_100','Cruise Control Usage >75%',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Cruise Control Usage >75%');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_cruise_control_usage_75_100',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_cruise_control_usage_75_100' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_pto_usage','PTO Usage (%)',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'PTO Usage (%)');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_pto_usage',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_pto_usage' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_average_diving_speed','Average Driving Speed',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Average Driving Speed');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_average_diving_speed',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_average_diving_speed' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_average_speed','Average Speed',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Average Speed');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_average_speed',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_average_speed' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_heavy_throttling','Heavy Throttling (%)',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Heavy Throttling (%)');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_heavy_throttling',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_heavy_throttling' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_heavy_throttle_duration','Heavy Throttle Duration',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Heavy Throttle Duration');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_heavy_throttle_duration',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_heavy_throttle_duration' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_idling','Idling (%)',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Idling (%)');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_idling',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_idling' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_idle_duration','Idle Duration',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Idle Duration');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_idle_duration',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_idle_duration' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_harsh_braking','Harsh Braking (%)',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Harsh Braking (%)');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_harsh_braking',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_harsh_braking' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_harsh_brake_duration','Harsh Brake Duration',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Harsh Brake Duration');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_harsh_brake_duration',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_harsh_brake_duration' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_braking','Braking (%)',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Braking (%)');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_braking',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_braking' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_brake_duration','Brake Duration',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Brake Duration');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_brake_duration',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_brake_duration' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_average_gross_weight','Average Gross Weight',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Average Gross Weight');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_average_gross_weight',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_average_gross_weight' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_average_distance_per_day','Average Distance per day',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'Average Distance per day');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_average_distance_per_day',(select id from master.menu where name = 'EcoScore Profile Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_average_distance_per_day' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_ecoscore_desc','The Eco-Score represents the Overall Performance of the driver. The Eco-Score is the rounded down average of the Anticipation and Braking Scores. The Fuel Consumption is not part of the Eco-Score calculation.',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'The Eco-Score represents the Overall Performance of the driver. The Eco-Score is the rounded down average of the Anticipation and Braking Scores. The Fuel Consumption is not part of the Eco-Score calculation.');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_ecoscore_desc',(select id from master.menu where name = 'EcoScore Profile Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_ecoscore_desc' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_fuel_consumption_desc','The Fuel Consumption is not part of the Eco-Score calculation.',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'The Fuel Consumption is not part of the Eco-Score calculation.');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_fuel_consumption_desc',(select id from master.menu where name = 'EcoScore Profile Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_fuel_consumption_desc' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_braking_score_desc','The Eco-Score is the rounded down average of the  Braking Scores.',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'The Eco-Score is the rounded down average of the  Braking Scores.');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_braking_score_desc',(select id from master.menu where name = 'EcoScore Profile Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_braking_score_desc' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_anticipation_score_desc','The Eco-Score is the rounded down average of the Anticipation score',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'The Eco-Score is the rounded down average of the Anticipation score');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_anticipation_score_desc',(select id from master.menu where name = 'EcoScore Profile Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_anticipation_score_desc' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','es_other_fuel_consumption_indicators_desc','The other  Fuel Consumption is not part of the Eco-Score calculation.',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'The other  Fuel Consumption is not part of the Eco-Score calculation.');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'es_other_fuel_consumption_indicators_desc',(select id from master.menu where name = 'EcoScore Profile Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'es_other_fuel_consumption_indicators_desc' 
and ref_id=(select id from master.menu where name = 'EcoScore Profile Management'));


-----Scheduled report, Drive time management, Trip report and Fleet utulization report changes

--Scheduled Report
-- DataAttributes

UPDATE MASTER.DATAATTRIBUTE SET name = 'ScheduledReport.ReportType' , key = 'da_scheduledreport_reporttype' WHERE name = 'Report.ReportType';
UPDATE MASTER.DATAATTRIBUTE SET name = 'ScheduledReport.VehicleGroup' , key = 'da_scheduledreport_vehiclegroup' WHERE name = 'Report.VehicleGroup';
UPDATE MASTER.DATAATTRIBUTE SET name = 'ScheduledReport.Frequency' , key = 'da_scheduledreport_frequency' WHERE name = 'Report.Frequency';
UPDATE MASTER.DATAATTRIBUTE SET name = 'ScheduledReport.Recipients' , key = 'da_scheduledreport_recipients' WHERE name = 'Report.Recipients';
UPDATE MASTER.DATAATTRIBUTE SET name = 'ScheduledReport.LastRun' , key = 'da_scheduledreport_lastrun' WHERE name = 'Report.LastRun';
UPDATE MASTER.DATAATTRIBUTE SET name = 'ScheduledReport.NextRun' , key = 'da_scheduledreport_nextrun' WHERE name = 'Report.NextRun';
UPDATE MASTER.DATAATTRIBUTE SET name = 'ScheduledReport.Status' , key = 'da_scheduledreport_status' WHERE name = 'Report.Status';


-- Translation

UPDATE TRANSLATION.TRANSLATION set NAME = 'da_scheduledreport_reporttype', VALUE = 'ScheduledReport.ReportType' WHERE VALUE = 'Report.ReportType';
UPDATE TRANSLATION.TRANSLATION set NAME = 'da_scheduledreport_vehiclegroup', VALUE = 'ScheduledReport.VehicleGroup' WHERE VALUE = 'Report.VehicleGroup';
UPDATE TRANSLATION.TRANSLATION set NAME = 'da_scheduledreport_frequency', VALUE = 'ScheduledReport.Frequency' WHERE VALUE = 'Report.Frequency';
UPDATE TRANSLATION.TRANSLATION set NAME = 'da_scheduledreport_recipients', VALUE = 'ScheduledReport.Recipients' WHERE VALUE = 'Report.Recipients';
UPDATE TRANSLATION.TRANSLATION set NAME = 'da_scheduledreport_lastrun', VALUE = 'ScheduledReport.LastRun' WHERE VALUE = 'Report.LastRun';
UPDATE TRANSLATION.TRANSLATION set NAME = 'da_scheduledreport_nextrun', VALUE = 'ScheduledReport.NextRun' WHERE VALUE = 'Report.NextRun';
UPDATE TRANSLATION.TRANSLATION set NAME = 'da_scheduledreport_status', VALUE = 'ScheduledReport.Status' WHERE VALUE = 'Report.Status';

--Translation Grouping

UPDATE TRANSLATION.TRANSLATIONGROUPING set NAME = 'da_scheduledreport_reporttype'		WHERE NAME = 'da_report_reporttype';
UPDATE TRANSLATION.TRANSLATIONGROUPING set NAME = 'da_scheduledreport_vehiclegroup'	 	WHERE NAME = 'da_report_vehiclegroup';
UPDATE TRANSLATION.TRANSLATIONGROUPING set NAME = 'da_scheduledreport_frequency'		WHERE NAME = 'da_report_frequency';
UPDATE TRANSLATION.TRANSLATIONGROUPING set NAME = 'da_scheduledreport_recipients'		WHERE NAME = 'da_report_recipients';
UPDATE TRANSLATION.TRANSLATIONGROUPING set NAME = 'da_scheduledreport_lastrun'			WHERE NAME = 'da_report_lastrun';
UPDATE TRANSLATION.TRANSLATIONGROUPING set NAME = 'da_scheduledreport_nextrun'			WHERE NAME = 'da_report_nextrun';
UPDATE TRANSLATION.TRANSLATIONGROUPING set NAME = 'da_scheduledreport_status'			WHERE NAME = 'da_report_status';
--UPDATE TRANSLATION.TRANSLATIONGROUPING set NAME = 'da_scheduledreport_vehicle'			WHERE NAME = 'da_report_vehicle';

--Drive Time Management
--DataAttributes
--All Drivers
--General section

UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.AllDriver.General.DriversCount' ,KEY = 'da_report_alldriver_general_driverscount', COLUMN_NAME_FOR_UI = 'DriversCount' WHERE NAME = 'Report.General.DriversCount';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.AllDriver.General.TotalDriveTime',KEY = 'da_report_alldriver_general_totaldrivetime', COLUMN_NAME_FOR_UI = 'Total Drive Time' WHERE NAME = 'Report.General.TotalDriveTime';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.AllDriver.General.TotalWorkTime',KEY = 'da_report_alldriver_general_totalworktime', COLUMN_NAME_FOR_UI = 'Total Work Time'  WHERE NAME = 'Report.General.TotalWorkTime';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.AllDriver.General.TotalAvailableTime',KEY = 'da_report_alldriver_general_totalavailabletime', COLUMN_NAME_FOR_UI = 'Total Available Time' WHERE NAME = 'Report.General.TotalAvailableTime';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.AllDriver.General.TotalRestTime',KEY = 'da_report_alldriver_general_totalresttime', COLUMN_NAME_FOR_UI = 'Total Rest Time' WHERE NAME = 'Report.General.TotalRestTime';

--Details section

UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.AllDriver.Details.DriverId' ,KEY = 'da_report_alldriver_details_driverid', COLUMN_NAME_FOR_UI = 'Driver Id' WHERE NAME = 'Report.Details.DriverId';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.AllDriver.Details.DriverName' ,KEY = 'da_report_alldriver_details_drivername', COLUMN_NAME_FOR_UI = 'Driver Name' WHERE NAME = 'Report.Details.DriverName';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.AllDriver.Details.EndTime' ,KEY = 'da_report_alldriver_details_endtime', COLUMN_NAME_FOR_UI = 'End Time' WHERE NAME = 'Report.Details.EndTime';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.AllDriver.Details.StartTime' ,KEY = 'da_report_alldriver_details_starttime', COLUMN_NAME_FOR_UI = 'Start Time' WHERE NAME = 'Report.Details.StartTime';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.AllDriver.Details.WorkTime' ,KEY = 'da_report_alldriver_details_worktime', COLUMN_NAME_FOR_UI = 'Work Time' WHERE NAME = 'Report.Details.WorkTime';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.AllDriver.Details.AvailableTime' ,KEY = 'da_report_alldriver_details_availabletime', COLUMN_NAME_FOR_UI = 'Available Time' WHERE NAME = 'Report.Details.AvailableTime';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.AllDriver.Details.ServiceTime' ,KEY = 'da_report_alldriver_details_servicetime', COLUMN_NAME_FOR_UI = 'Service Time' WHERE NAME = 'Report.Details.ServiceTime';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.AllDriver.Details.RestTime' ,KEY = 'da_report_alldriver_details_resttime', COLUMN_NAME_FOR_UI = 'Rest Time' WHERE NAME = 'Report.Details.RestTime';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.AllDriver.Details.DriveTime' ,KEY = 'da_report_alldriver_details_drivetime', COLUMN_NAME_FOR_UI = 'Drive Time' WHERE NAME = 'Report.Details.DriveTime';

--SpecificDriver
--General

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.SpecificDriver.General.DriverId'	,'A',	'da_report_specificdriver_general_driverid'	,	'Driver Id'									WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.specificdriver.general.driverid'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.SpecificDriver.General.DriverName'	,'A',	'da_report_specificdriver_general_drivername'	,	'Driver Name'							WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.specificdriver.general.drivername'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.SpecificDriver.General.TotalDriveTime'	,'A',	'da_report_specificdriver_general_totaldrivetime'	,	'Total Drive Time'				WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.specificdriver.general.totaldrivetime'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.SpecificDriver.General.TotalWorkTime'	,'A',	'da_report_specificdriver_general_totalworktime'	,	'Total Work Time'				WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.specificdriver.general.totalworktime'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.SpecificDriver.General.TotalAvailableTime'	,'A',	'da_report_specificdriver_general_totalavailabletime'	,	'Total Available Time'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.specificdriver.general.totalavailabletime'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.SpecificDriver.General.TotalRestTime'	,'A',	'da_report_specificdriver_general_totalresttime'	,	'Total Rest Time'				WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.specificdriver.general.totalresttime'	);

--Details

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.SpecificDriver.Details.DriverId' ,'A', 'da_report_specificdriver_details_driverid',  'Driver Id' 				  WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.specificdriver.details.driverid'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.SpecificDriver.Details.DriverName' ,'A', 'da_report_specificdriver_details_drivername',  'Driver Name' 		  WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.specificdriver.details.drivername'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.SpecificDriver.Details.EndTime' ,'A', 'da_report_specificdriver_details_endtime',  'End Time' 					  WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.specificdriver.details.endtime'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.SpecificDriver.Details.StartTime' ,'A', 'da_report_specificdriver_details_starttime',  'Start Time' 			  WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.specificdriver.details.starttime'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.SpecificDriver.Details.WorkTime' ,'A', 'da_report_specificdriver_details_worktime',  'Work Time' 				  WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.specificdriver.details.worktime'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.SpecificDriver.Details.AvailableTime' ,'A', 'da_report_specificdriver_details_availabletime',  'Available Time'   WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.specificdriver.details.availabletime'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.SpecificDriver.Details.ServiceTime' ,'A', 'da_report_specificdriver_details_servicetime',  'Service Time'		  WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.specificdriver.details.servicetime'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.SpecificDriver.Details.RestTime' ,'A', 'da_report_specificdriver_details_resttime',  'Rest Time' 				  WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.specificdriver.details.resttime'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.SpecificDriver.Details.DriveTime' ,'A', 'da_report_specificdriver_details_drivetime',  'Drive Time' 			  WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.specificdriver.details.drivetime'	);

--Charts

INSERT INTO master.dataattribute (name,TYPE,KEY)	SELECT 	'Report.SpecificDriver.Details.Charts' ,'A', 'da_report_specificdriver_details_charts' 			  WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.specificdriver.details.charts'	);


--Translation

UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_alldriver_details_driverid', VALUE = 'Report.AllDriver.Details.DriverId' WHERE VALUE = 'Report.DriverId';                                                                                                                                                                                                                               
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_alldriver_details_drivername', VALUE = 'Report.AllDriver.Details.DriverName' WHERE VALUE = 'Report.DriverName';                                                                                                                                                                                                                               
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_alldriver_details_endtime', VALUE = 'Report.AllDriver.Details.EndTime' WHERE VALUE = 'Report.EndTime';                                                                                                                                                                                                                               
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_alldriver_details_starttime', VALUE = 'Report.AllDriver.Details.StartTime' WHERE VALUE = 'Report.StartTime';                                                                                                                                                                                                                               
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_alldriver_details_worktime', VALUE = 'Report.AllDriver.Details.WorkTime' WHERE VALUE = 'Report.WorkTime';                                                                                                                                                                                                                               
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_alldriver_details_availabletime', VALUE = 'Report.AllDriver.Details.AvailableTime' WHERE VALUE = 'Report.AvailableTime';                                                                                                                                                                                                                               
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_alldriver_details_servicetime', VALUE = 'Report.AllDriver.Details.ServiceTime' WHERE VALUE = 'Report.ServiceTime';                                                                                                                                                                                                                               
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_alldriver_details_resttime', VALUE = 'Report.AllDriver.Details.RestTime' WHERE VALUE = 'Report.RestTime';                                                                                                                                                                                                                               
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_alldriver_details_drivetime', VALUE = 'Report.AllDriver.Details.DriveTime' WHERE VALUE = 'Report.DriveTime';                                                                                                                                                                                                                               

--Report Attributes

insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.general.driverid')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.general.driverid'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.general.drivername')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.general.drivername'));
				  

insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.general.totaldrivetime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.general.totaldrivetime'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.general.totalworktime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.general.totalworktime'));


insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.general.totalavailabletime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.general.totalavailabletime'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.general.totalresttime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.general.totalresttime'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.driverid')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.driverid'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.drivername')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.drivername'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.endtime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.endtime'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.starttime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.starttime'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.worktime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.worktime'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.availabletime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.availabletime'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.servicetime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.servicetime'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.resttime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.resttime'));

insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.drivetime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.drivetime'));
				  

insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.charts')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.specificdriver.details.charts'));
				  

--Translation

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_specificdriver_general_driverid','Report.SpecificDriver.General.DriverId',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.specificdriver.general.driverid');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_specificdriver_general_drivername','Report.SpecificDriver.General.DriverName',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.specificdriver.general.drivername');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_specificdriver_general_totaldrivetime','Report.SpecificDriver.General.TotalDriveTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.specificdriver.general.totaldrivetime');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_specificdriver_general_totalworktime','Report.SpecificDriver.General.TotalWorkTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.specificdriver.general.totalworktime');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_specificdriver_general_totalavailabletime','Report.SpecificDriver.General.TotalAvailableTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.specificdriver.general.totalavailabletime');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_specificdriver_general_totalresttime','Report.SpecificDriver.General.TotalRestTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.specificdriver.general.totalresttime');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_specificdriver_details_driverid','Report.SpecificDriver.Details.DriverId',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.specificdriver.details.driverid');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_specificdriver_details_drivername','Report.SpecificDriver.Details.DriverName',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.specificdriver.details.drivername');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_specificdriver_details_endtime','Report.SpecificDriver.Details.EndTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.specificdriver.details.endtime');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_specificdriver_details_starttime','Report.SpecificDriver.Details.StartTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.specificdriver.details.starttime');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_specificdriver_details_worktime','Report.SpecificDriver.Details.WorkTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.specificdriver.details.worktime');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_specificdriver_details_availabletime','Report.SpecificDriver.Details.AvailableTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.specificdriver.details.availabletime');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_specificdriver_details_servicetime','Report.SpecificDriver.Details.ServiceTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.specificdriver.details.servicetime');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_specificdriver_details_resttime','Report.SpecificDriver.Details.RestTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.specificdriver.details.resttime');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_specificdriver_details_drivetime','Report.SpecificDriver.Details.DriveTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.specificdriver.details.drivetime');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_specificdriver_details_charts','Report.SpecificDriver.Details.Charts',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.specificdriver.details.charts');


--Translation Grouping


INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_specificdriver_general_driverid',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_specificdriver_general_driverid' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));


INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_specificdriver_general_drivername',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_specificdriver_general_drivername' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_specificdriver_general_totaldrivetime',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_specificdriver_general_totaldrivetime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_specificdriver_general_totalworktime',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_specificdriver_general_totalworktime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_specificdriver_general_totalavailabletime',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_specificdriver_general_totalavailabletime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_specificdriver_general_totalresttime',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_specificdriver_general_totalresttime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_specificdriver_details_driverid',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_specificdriver_details_driverid' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_specificdriver_details_drivername',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_specificdriver_details_drivername' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_specificdriver_details_endtime',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_specificdriver_details_endtime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_specificdriver_details_starttime',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_specificdriver_details_starttime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_specificdriver_details_worktime',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_specificdriver_details_worktime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_specificdriver_details_availabletime',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_specificdriver_details_availabletime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_specificdriver_details_servicetime',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_specificdriver_details_servicetime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_specificdriver_details_resttime',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_specificdriver_details_resttime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_specificdriver_details_drivetime',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_specificdriver_details_drivetime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_specificdriver_details_charts',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_specificdriver_details_charts' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

--Key fix

UPDATE MASTER.DATAATTRIBUTE da 
SET KEY = a.new_key
FROM (SELECT id,name,key,'da_report_'||replace(substring (lower(name),8),'.','_') as new_key from master.dataattribute where key like'da_report%' and replace(substring (lower(name),8),'.','') <> replace(substring(key,11),'_','')  order by name asc)  a
WHERE a.id = da.id
and a.name = da.name
and a.key = da.key;

UPDATE TRANSLATION.TRANSLATION da 
SET NAME = a.new_key
FROM (SELECT id,name,VALUE,'da_report_'||replace(substring (lower(VALUE),8),'.','_') as new_key from TRANSLATION.TRANSLATION where NAME like'da_report%' and replace(substring (lower(VALUE),8),'.','') <> replace(substring(NAME,11),'_','')  order by name asc)  a
WHERE a.id = da.id
and a.name = da.name
and a.VALUE = da.VALUE;



UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_details_alerts'                  , VALUE = 'Report.Details.Alerts' WHERE VALUE = 'Report.Alerts';
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_details_averagedistanceperday'   , VALUE = 'Report.Details.AverageDistancePerDay' WHERE VALUE = 'Report.AverageDistancePerDay';
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_details_distance'                , VALUE = 'Report.Details.Distance' WHERE VALUE = 'Report.Distance';
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_details_drivingtime'             , VALUE = 'Report.Details.DrivingTime' WHERE VALUE = 'Report.DrivingTime';
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_details_enddate'                 , VALUE = 'Report.Details.EndDate' WHERE VALUE = 'Report.EndDate';
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_details_endposition'             , VALUE = 'Report.Details.EndPosition' WHERE VALUE = 'Report.EndPosition';
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_details_events'                  , VALUE = 'Report.Details.Events' WHERE VALUE = 'Report.Events';
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_details_fuelconsumed'            , VALUE = 'Report.Details.FuelConsumed' WHERE VALUE = 'Report.FuelConsumed';
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_details_idleduration'            , VALUE = 'Report.Details.IdleDuration' WHERE VALUE = 'Report.IdleDuration';
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_details_numberoftrips'           , VALUE = 'Report.Details.NumberOfTrips' WHERE VALUE = 'Report.NumberOfTrips';
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_details_registrationnumber'      , VALUE = 'Report.Details.RegistrationNumber' WHERE VALUE = 'Report.RegistrationNumber';
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_details_startdate'               , VALUE = 'Report.Details.StartDate' WHERE VALUE = 'Report.StartDate';
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_details_startposition'           , VALUE = 'Report.Details.StartPosition' WHERE VALUE = 'Report.StartPosition';
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_details_stoptime'                , VALUE = 'Report.Details.StopTime' WHERE VALUE = 'Report.StopTime';
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_details_triptime'                , VALUE = 'Report.Details.TripTime' WHERE VALUE = 'Report.TripTime';
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_details_vehiclename'             , VALUE = 'Report.Details.VehicleName' WHERE VALUE = 'Report.VehicleName';
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_details_vin'                     , VALUE = 'Report.Details.VIN' WHERE VALUE = 'Report.VIN';


INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_general_idleduration','Report.General.IdleDuration',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.general.idleduration');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_general_numberoftrips','Report.General.NumberOfTrips',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.general.numberoftrips');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_general_numberofvehicles','Report.General.NumberOfVehicles',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.general.numberofvehicles');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_general_totaldistance','Report.General.TotalDistance',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.general.totaldistance');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_general_averagedistanceperday','Report.General.AverageDistancePerDay',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.general.averagedistanceperday');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_alldriver_general_driverscount','Report.AllDriver.General.DriversCount',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.alldriver.general.driverscount');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_alldriver_general_totaldrivetime','Report.AllDriver.General.TotalDriveTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.alldriver.general.totaldrivetime');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_alldriver_general_totalworktime','Report.AllDriver.General.TotalWorkTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.alldriver.general.totalworktime');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_alldriver_general_totalavailabletime','Report.AllDriver.General.TotalAvailableTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.alldriver.general.totalavailabletime');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_alldriver_general_totalresttime','Report.AllDriver.General.TotalRestTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.alldriver.general.totalresttime');


UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME = 'da_report_charts_distanceperday' WHERE NAME = 'da_report_distanceperday' AND REF_ID = 10 ;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME = 'da_report_charts_numberofvehiclesperday' WHERE NAME = 'da_report_numberofvehiclesperday' AND REF_ID = 10 ;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME = 'da_report_charts_mileagebasedutilization' WHERE NAME = 'da_report_mileagebasedutilization' AND REF_ID = 10 ;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME = 'da_report_charts_timebasedutilization' WHERE NAME = 'da_report_timebasedutilization' AND REF_ID = 10 ;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME = 'da_report_calendarview_averageweight' WHERE NAME = 'da_report_averageweight' AND REF_ID = 10 ;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME = 'da_report_calendarview_idleduration' WHERE NAME = 'da_report_idleduration' AND REF_ID = 10 ;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME = 'da_report_calendarview_distance' WHERE NAME = 'da_report_distance' AND REF_ID = 10 ;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME = 'da_report_calendarview_drivingtime' WHERE NAME = 'da_report_drivingtime' AND REF_ID = 10 ;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME = 'da_report_calendarview_activevehicles' WHERE NAME = 'da_report_activevehicles' AND REF_ID = 10 ;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME = 'da_report_calendarview_totaltrips' WHERE NAME = 'da_report_totaltrips' AND REF_ID = 10 ;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME = 'da_report_general_idleduration' WHERE NAME = 'da_report_idleduration' AND REF_ID = 10 ;


INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_calendarview_mileagebasedutilization',(select id from master.menu where name = 'Fleet Utilisation'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_calendarview_mileagebasedutilization' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_calendarview_timebasedutilization',(select id from master.menu where name = 'Fleet Utilisation'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_calendarview_timebasedutilization' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_calendarview_idleduration',(select id from master.menu where name = 'Fleet Utilisation'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_calendarview_idleduration' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_general_numberoftrips',(select id from master.menu where name = 'Fleet Utilisation'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_general_numberoftrips' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_general_numberofvehicles',(select id from master.menu where name = 'Fleet Utilisation'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_general_numberofvehicles' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_general_averagedistanceperday',(select id from master.menu where name = 'Fleet Utilisation'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_general_averagedistanceperday' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));


INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_general_numberoftrips',(select id from master.menu where name = 'Fleet Utilisation'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_general_numberoftrips' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_numberoftrips',(select id from master.menu where name = 'Fleet Utilisation'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_numberoftrips' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_triptime',(select id from master.menu where name = 'Fleet Utilisation'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_triptime' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_general_idleduration',(select id from master.menu where name = 'Fleet Utilisation'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_general_idleduration' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_general_totaldistance',(select id from master.menu where name = 'Fleet Utilisation'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_general_totaldistance' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_stoptime',(select id from master.menu where name = 'Fleet Utilisation'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_distance_stoptime' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_averageweightpertrip',(select id from master.menu where name = 'Fleet Utilisation'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_averageweightpertrip' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_averagedistanceperday',(select id from master.menu where name = 'Fleet Utilisation'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_averagedistanceperday' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

--Trip report Translation grouping

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_vehiclename' 	,(select id from master.menu where name = 'Trip Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_vehiclename' 
and ref_id=(select id from master.menu where name = 'Trip Report'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_vin' 	,(select id from master.menu where name = 'Trip Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_vin' 
and ref_id=(select id from master.menu where name = 'Trip Report'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_registrationnumber',(select id from master.menu where name = 'Trip Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_registrationnumber' 
and ref_id=(select id from master.menu where name = 'Trip Report'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_startdate' 	,(select id from master.menu where name = 'Trip Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_startdate' 
and ref_id=(select id from master.menu where name = 'Trip Report'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_enddate' 	,(select id from master.menu where name = 'Trip Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_enddate' 
and ref_id=(select id from master.menu where name = 'Trip Report'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_distance' 	,(select id from master.menu where name = 'Trip Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_distance' 
and ref_id=(select id from master.menu where name = 'Trip Report'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_idleduration' ,(select id from master.menu where name = 'Trip Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_idleduration' 
and ref_id=(select id from master.menu where name = 'Trip Report'));
	
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_averagespeed' 	,(select id from master.menu where name = 'Trip Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_averagespeed' 
and ref_id=(select id from master.menu where name = 'Trip Report'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_averageweight' ,(select id from master.menu where name = 'Trip Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_averageweight' 
and ref_id=(select id from master.menu where name = 'Trip Report'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_odometer' 	,(select id from master.menu where name = 'Trip Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_odometer' 
and ref_id=(select id from master.menu where name = 'Trip Report'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_startposition' ,(select id from master.menu where name = 'Trip Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_startposition' 
and ref_id=(select id from master.menu where name = 'Trip Report'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_endposition' 	,(select id from master.menu where name = 'Trip Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_endposition' 
and ref_id=(select id from master.menu where name = 'Trip Report'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_fuelconsumed' ,(select id from master.menu where name = 'Trip Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_fuelconsumed' 
and ref_id=(select id from master.menu where name = 'Trip Report'));
	
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_drivingtime' 	,(select id from master.menu where name = 'Trip Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_drivingtime' 
and ref_id=(select id from master.menu where name = 'Trip Report'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_alerts' ,(select id from master.menu where name = 'Trip Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_alerts' 
and ref_id=(select id from master.menu where name = 'Trip Report'));
	
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_details_events' 	,(select id from master.menu where name = 'Trip Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_details_events' 
and ref_id=(select id from master.menu where name = 'Trip Report'));


UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =  'da_report_vehicle' WHERE NAME ='da_scheduledreport_vehicle'                     AND REF_ID = 28;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =  'da_report_drivername' WHERE NAME ='da_report_alldriver_details_drivername'         AND REF_ID = 28;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =  'da_report_endtime' WHERE NAME ='da_report_alldriver_details_endtime'            AND REF_ID = 28;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =  'da_report_starttime' WHERE NAME ='da_report_alldriver_details_starttime'          AND REF_ID = 28;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =  'da_report_worktime' WHERE NAME ='da_report_alldriver_details_worktime'           AND REF_ID = 28;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =  'da_report_servicetime' WHERE NAME ='da_report_alldriver_details_servicetime'        AND REF_ID = 28;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =  'da_report_resttime' WHERE NAME ='da_report_alldriver_details_resttime'           AND REF_ID = 28;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =  'da_report_drivetime' WHERE NAME ='da_report_alldriver_details_drivetime'          AND REF_ID = 28;



INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_scheduledreport_vehicle',(select id from master.menu where name = 'Report Scheduler'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_scheduledreport_vehicle' 
and ref_id=(select id from master.menu where name = 'Report Scheduler'));

--Drive time management -- all drivers  -- Translation grouping

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_alldriver_general_driverscount',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_alldriver_general_driverscount' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_alldriver_general_totaldrivetime',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_alldriver_general_totaldrivetime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_alldriver_general_totalworktime',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_alldriver_general_totalworktime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_alldriver_general_totalavailabletime',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_alldriver_general_totalavailabletime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_alldriver_general_totalresttime',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_alldriver_general_totalresttime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_alldriver_details_driverid',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_alldriver_details_driverid' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_alldriver_details_drivername', (select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_alldriver_details_drivername' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_alldriver_details_endtime',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_alldriver_details_endtime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_alldriver_details_starttime', (select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_alldriver_details_starttime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_alldriver_details_worktime',(select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_alldriver_details_worktime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));
 
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_alldriver_details_availabletime', (select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_alldriver_details_availabletime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_alldriver_details_servicetime', (select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_alldriver_details_servicetime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));
 
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_alldriver_details_resttime', (select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_alldriver_details_resttime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_alldriver_details_drivetime', (select id from master.menu where name = 'Drive Time Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_alldriver_details_drivetime' 
and ref_id=(select id from master.menu where name = 'Drive Time Management'));

update translation.translation set value='Organisation Relationship Management' where name='lblOrgnisationRelationshipManagement';

---

INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
                             key, level,state)   
SELECT 461,'Configuration#VehicleManagement#UpdateStatus','B',null,'feat_vehiclemanagement_updatestatus',30,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='Configuration#VehicleManagement#UpdateStatus');

 

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_vehiclemanagement_updatestatus','Configuration#VehicleManagement#UpdateStatus',
(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_vehiclemanagement_updatestatus');

 

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_vehiclemanagement_updatestatus',(select id from master.menu where name = 'Account Role Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_vehiclemanagement_updatestatus' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));

 

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_vehiclemanagement_updatestatus',(select id from master.menu where name = 'Package Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_vehiclemanagement_updatestatus' 
and ref_id=(select id from master.menu where name = 'Package Management'));

 

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_vehiclemanagement_updatestatus',(select id from master.menu where name = 'Organisation Relationship Management'),'M'
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_vehiclemanagement_updatestatus' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));

