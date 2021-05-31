--For sprint 3 (DB) and sprint 4 (Dev)

INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
							 key, level,state)   
SELECT 858,'Admin#Organization-Scope','B',null,'feat_admin#organization-scope',20,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='Admin#Organization-Scope');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_admin#organization-scope','Admin#Organization-Scope',
(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_admin#organization-scope');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_admin#organization-scope',(select id from master.menu where name = 'Account Role Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_admin#organization-scope' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));


INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_admin#organization-scope',(select id from master.menu where name = 'Package Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_admin#organization-scope' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_admin#organization-scope',(select id from master.menu where name = 'Organisation Relationship Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_admin#organization-scope' 
and ref_id=(select id from master.menu where name = 'Orgnisation Relationship Management'));


INSERT INTO master.report  (name , key)   
SELECT 'Trip Report','lblTripReport' WHERE NOT EXISTS  (   SELECT 1   FROM master.report  WHERE name = 'Trip Report');
INSERT INTO master.report  (name , key)   
SELECT 'Trip Tracing','lblTripTracing' WHERE NOT EXISTS  (   SELECT 1   FROM master.report  WHERE name = 'Trip Tracing');
INSERT INTO master.report  (name , key)   
SELECT 'Advanced Fleet Fuel Report','lblAdvancedFleetFuelReport' WHERE NOT EXISTS  (   SELECT 1   FROM master.report  WHERE name = 'Advanced Fleet Fuel Report');
INSERT INTO master.report  (name , key)   
SELECT 'Fleet Fuel Report','lblFleetFuelReport' WHERE NOT EXISTS  (   SELECT 1   FROM master.report  WHERE name = 'Fleet Fuel Report');
INSERT INTO master.report  (name , key)   
SELECT 'Fleet Utilisation Report','lblFleetUtilisation' WHERE NOT EXISTS  (   SELECT 1   FROM master.report  WHERE name = 'Fleet Utilisation Report');
INSERT INTO master.report  (name , key)   
SELECT 'Fuel Benchmarking','lblFuelBenchmarking' WHERE NOT EXISTS  (   SELECT 1   FROM master.report  WHERE name = 'Fuel Benchmarking');
INSERT INTO master.report  (name , key)   
SELECT 'Fuel Deviation Report','lblFuelDeviationReport' WHERE NOT EXISTS  (   SELECT 1   FROM master.report  WHERE name = 'Fuel Deviation Report');
INSERT INTO master.report  (name , key)   
SELECT 'Vehicle Performance Report','lblVehiclePerformanceReport' WHERE NOT EXISTS  (   SELECT 1   FROM master.report  WHERE name = 'Vehicle Performance Report');
INSERT INTO master.report  (name , key)   
SELECT 'Drive Time Management','lblDriveTimeManagement' WHERE NOT EXISTS  (   SELECT 1   FROM master.report  WHERE name = 'Drive Time Management');
INSERT INTO master.report  (name , key)   
SELECT 'Eco-Score Report','lblECOScoreReport' WHERE NOT EXISTS  (   SELECT 1   FROM master.report  WHERE name = 'Eco-Score Report');


UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Averagespeed' , KEY = 'da_report_averagespeed'	WHERE NAME = 'Report.Averagespeed(km/h)'	;  
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.AverageWeight', KEY = 'da_report_averageweight'	WHERE NAME = 'Report.AverageWeight(t)'	;  
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Odometer', KEY = 'da_report_odometer'	WHERE NAME = 'Report.Odometer(km)'	;  --


UPDATE MASTER.DATAATTRIBUTE SET COLUMN_NAME_FOR_UI  =  'Vehicle Name' 	WHERE NAME = 'Report.Vehiclename'	;
UPDATE MASTER.DATAATTRIBUTE SET COLUMN_NAME_FOR_UI  =  'VIN' 	WHERE NAME = 'Report.VIN'	;
UPDATE MASTER.DATAATTRIBUTE SET COLUMN_NAME_FOR_UI  =  'Registration Plate number' 	WHERE NAME = 'Report.RegistrationNumber'	;
UPDATE MASTER.DATAATTRIBUTE SET COLUMN_NAME_FOR_UI  =  'Start Date'	WHERE NAME = 'Report.StartDate'	;
UPDATE MASTER.DATAATTRIBUTE SET COLUMN_NAME_FOR_UI  =  'End Date'	WHERE NAME = 'Report.EndDate'	;
UPDATE MASTER.DATAATTRIBUTE SET COLUMN_NAME_FOR_UI  =  'Distance'	WHERE NAME = 'Report.Distance'	;
UPDATE MASTER.DATAATTRIBUTE SET COLUMN_NAME_FOR_UI  =  'Idle Duration'	WHERE NAME = 'Report.IdleDuration'	;
UPDATE MASTER.DATAATTRIBUTE SET COLUMN_NAME_FOR_UI  =  'Average Speed'	WHERE NAME = 'Report.Averagespeed'	;  
UPDATE MASTER.DATAATTRIBUTE SET COLUMN_NAME_FOR_UI  =  'Average Weight'	WHERE NAME = 'Report.AverageWeight'	;  
UPDATE MASTER.DATAATTRIBUTE SET COLUMN_NAME_FOR_UI  =  'Odometer'	WHERE NAME = 'Report.Odometer'	;  
UPDATE MASTER.DATAATTRIBUTE SET COLUMN_NAME_FOR_UI  =  'Start Position'	WHERE NAME = 'Report.StartPosition'	;
UPDATE MASTER.DATAATTRIBUTE SET COLUMN_NAME_FOR_UI  =  'End Position'	WHERE NAME = 'Report.EndPosition'	;
UPDATE MASTER.DATAATTRIBUTE SET COLUMN_NAME_FOR_UI  =  'Fuel Consumed'	WHERE NAME = 'Report.FuelConsumed'	;  
UPDATE MASTER.DATAATTRIBUTE SET COLUMN_NAME_FOR_UI  =  'Driving Time'	WHERE NAME = 'Report.Drivingtime'	;
UPDATE MASTER.DATAATTRIBUTE SET COLUMN_NAME_FOR_UI  =  'Alerts'	WHERE NAME = 'Report.Alerts'	;
UPDATE MASTER.DATAATTRIBUTE SET COLUMN_NAME_FOR_UI  =  'Events'	WHERE NAME = 'Report.Events'	;


UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.Vehiclename' 	WHERE NAME = 'Report.Vehiclename'	;
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.VIN' 	WHERE NAME = 'Report.VIN'	;
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.RegistrationNumber'  	WHERE NAME = 'Report.RegistrationNumber'	;
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.StartDate' 	WHERE NAME = 'Report.StartDate'	;
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.EndDate' 	WHERE NAME = 'Report.EndDate'	;
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.Distance' 	WHERE NAME = 'Report.Distance'	;
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.IdleDuration' 	WHERE NAME = 'Report.IdleDuration'	;
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.Averagespeed' 	WHERE NAME = 'Report.Averagespeed'	;  
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.AverageWeight' 	WHERE NAME = 'Report.AverageWeight'	;  
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.Odometer' 	WHERE NAME = 'Report.Odometer'	;  
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.StartPosition' 	WHERE NAME = 'Report.StartPosition'	;
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.EndPosition' 	WHERE NAME = 'Report.EndPosition'	;
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.FuelConsumed' 	WHERE NAME = 'Report.FuelConsumed'	;  
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.Drivingtime' 	WHERE NAME = 'Report.Drivingtime'	;
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.Alerts' 	WHERE NAME = 'Report.Alerts'	;
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.Events' 	WHERE NAME = 'Report.Events'	;

insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.registrationnumber')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.registrationnumber'));

insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.vin')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.vin'));


insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.events')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.events'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.alerts')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.alerts'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.drivingtime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.drivingtime'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.fuelconsumed')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.fuelconsumed'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.endposition')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.endposition'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.startposition')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.startposition'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.odometer')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.odometer'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.averageweight')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.averageweight'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.averagespeed')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.averagespeed'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.idleduration')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.idleduration'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.distance')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.distance'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.enddate')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.enddate'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.startdate')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.startdate'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.vehiclename')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'trip report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.vehiclename'));


update master.menu set name='Organisation relationship Management' where lower(name)=lower('Orgnisation relationship Management');

UPDATE MASTER.FEATURE SET NAME = 'Alerts.LogisticsAlerts.EnteringZone' , KEY = 'feat_alerts_logisticsalerts_enteringzone' WHERE ID = 401;
UPDATE MASTER.FEATURE SET NAME = 'Alerts.LogisticsAlerts.ExitingZone' , KEY = 'feat_alerts_logisticsalerts_exitingzone' WHERE ID = 402;
UPDATE MASTER.FEATURE SET NAME = 'Alerts.LogisticsAlerts.ExitingCorridor' , KEY = 'feat_alerts_logisticsalerts_exitingcorridor' WHERE ID = 403;
UPDATE MASTER.FEATURE SET NAME = 'Alerts.LogisticsAlerts.ExcessiveUnderUtilisationInDays(Batch)' , KEY = 'feat_alerts_logisticsalerts_excessiveunderutilisationindays(batch)' WHERE ID = 404;
UPDATE MASTER.FEATURE SET NAME = 'Alerts.LogisticsAlerts.ExcessiveUnderUtilisationInHours(Batch)' , KEY = 'feat_alerts_logisticsalerts_excessiveunderutilisationinhours(batch)' WHERE ID = 405;
UPDATE MASTER.FEATURE SET NAME = 'Alerts.LogisticsAlerts.ExcessiveDistanceDone(Trip)' , KEY = 'feat_alerts_logisticsalerts_excessivedistancedone(trip)' WHERE ID = 406;
UPDATE MASTER.FEATURE SET NAME = 'Alerts.LogisticsAlerts.ExcessiveDrivingDuration(Trip)' , KEY = 'feat_alerts_logisticsalerts_excessivedrivingduration(trip)' WHERE ID = 407;
UPDATE MASTER.FEATURE SET NAME = 'Alerts.LogisticsAlerts.ExcessiveGlobalMileage(Trip)' , KEY = 'feat_alerts_logisticsalerts_excessiveglobalmileage(trip)' WHERE ID = 408;
UPDATE MASTER.FEATURE SET NAME = 'Alerts.LogisticsAlerts.HoursofService(Realtime)' , KEY = 'feat_alerts_logisticsalerts_hoursofservice(realtime)' WHERE ID = 409;
UPDATE MASTER.FEATURE SET NAME = 'Alerts.FuelandDriverPerformance.ExcessiveAverageSpeed(Realtime)' , KEY = 'feat_alerts_fuelanddriverperformance_excessiveaveragespeed(realtime)' WHERE ID = 410;
UPDATE MASTER.FEATURE SET NAME = 'Alerts.FuelandDriverPerformance.ExcessiveIdling(Realtime)' , KEY = 'feat_alerts_fuelanddriverperformance_excessiveidling(realtime)' WHERE ID = 411;
UPDATE MASTER.FEATURE SET NAME = 'Alerts.FuelandDriverPerformance.FuelConsumed' , KEY = 'feat_alerts_fuelanddriverperformance_fuelconsumed' WHERE ID = 412;
UPDATE MASTER.FEATURE SET NAME = 'Alerts.FuelandDriverPerformance.FuelIncreaseDuringStop(Realtime)' , KEY = 'feat_alerts_fuelanddriverperformance_fuelincreaseduringstop(realtime)' WHERE ID = 413;
UPDATE MASTER.FEATURE SET NAME = 'Alerts.FuelandDriverPerformance.FuelLossDuringStop(Realtime)' , KEY = 'feat_alerts_fuelanddriverperformance_fuellossduringstop(realtime)' WHERE ID = 414;
UPDATE MASTER.FEATURE SET NAME = 'Alerts.FuelandDriverPerformance.FuelLossDuringTrip(Realtime)' , KEY = 'feat_alerts_fuelanddriverperformance_fuellossduringtrip(realtime)' WHERE ID = 415;

INSERT INTO MASTER.FEATURE (ID,NAME,TYPE,KEY,LEVEL,STATE) 
    SELECT 416,'Alerts.RepairandMaintenance.StatusChangetoStopNow'	,	'F'	,	'feat_alerts_repairandmaintenance_statuschangetostopnow'	,	40	,	'A'		
WHERE NOT EXISTS (SELECT 1 FROM MASTER.FEATURE  WHERE lower(name) = 'alerts.repairandmaintenance.statuschangetostopnow');
INSERT INTO MASTER.FEATURE (ID,NAME,TYPE,KEY,LEVEL,STATE)  
     SELECT 417,'Alerts.RepairandMaintenance.StatusChangetoServiceNow'	,	'F'	,	'feat_alerts_repairandmaintenance_statuschangetoservicenow'	,	40	,	'A'	
	WHERE NOT EXISTS (SELECT 1 FROM MASTER.FEATURE  WHERE lower(name) = 'alerts.repairandmaintenance.statuschangetoservicenow');


UPDATE translation.enumtranslation SET PARENT_ENUM = 'L' WHERE lower(KEY) = 'enumtype_excessiveunderutilizationinhours(batch)';


--Trip report

UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.VehicleName', KEY = 'da_report_details_vehiclename' 	WHERE NAME = 'Report.Details.Vehiclename'	;
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.AverageSpeed' , KEY = 'da_report_details_averagespeed'	WHERE NAME = 'Report.Details.Averagespeed'	;
UPDATE MASTER.DATAATTRIBUTE SET NAME  =  'Report.Details.DrivingTime' , KEY = 'da_report_details_drivingtime'	WHERE NAME = 'Report.Details.Drivingtime'	;


UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_averagespeed' , VALUE = 'Report.Details.AverageSpeed' WHERE VALUE = 'Report.Averagespeed(km/h)';
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_odometer' , VALUE = 'Report.Details.Odometer' WHERE VALUE = 'Report.Odometer(km)';
UPDATE TRANSLATION.TRANSLATION SET NAME = 'da_report_averageweight' , VALUE = 'Report.Details.AverageWeight' WHERE VALUE = 'Report.AverageWeight(t)';

UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME = 'da_report_averagespeed' WHERE NAME = 'da_report_averagespeed(km/h)';
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME = 'da_report_averageweight' WHERE NAME = 'da_report_averageweight(t)';
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME = 'da_report_odometer' WHERE NAME = 'da_report_odometer(km)';

--Alerts

UPDATE TRANSLATION.TRANSLATION SET NAME =	'feat_alerts_logisticsalerts_enteringzone'	, VALUE =	'Alerts.LogisticsAlerts.EnteringZone'	WHERE VALUE = 	'Alert.EnteringPOIZone'	;
UPDATE TRANSLATION.TRANSLATION SET NAME =	'feat_alerts_logisticsalerts_exitingzone', VALUE =	'Alerts.LogisticsAlerts.ExitingZone'	WHERE VALUE = 	'Alert.ExitingPOIZone'	;
UPDATE TRANSLATION.TRANSLATION SET NAME =	'feat_alerts_logisticsalerts_exitingcorridor'	, VALUE =	'Alerts.LogisticsAlerts.ExitingCorridor'	WHERE VALUE = 	'Alert.OutofCorridor'	;;
UPDATE TRANSLATION.TRANSLATION SET NAME =	'feat_alerts_logisticsalerts_excessivedistancedone(trip)'	, VALUE =	'Alerts.LogisticsAlerts.ExcessiveDistanceDone(Trip)'	WHERE VALUE = 	'Alert.ExcessiveDistanceDone'	;
UPDATE TRANSLATION.TRANSLATION SET NAME =	'feat_alerts_logisticsalerts_excessivedrivingduration(trip)'	, VALUE =	'Alerts.LogisticsAlerts.ExcessiveDrivingDuration(Trip)'	WHERE VALUE = 	'Alert.ExcessiveDrivingDuration';
UPDATE TRANSLATION.TRANSLATION SET NAME =	'feat_alerts_logisticsalerts_excessiveglobalmileage(trip)'	, VALUE =	'Alerts.LogisticsAlerts.ExcessiveGlobalMileage(Trip)'	WHERE VALUE = 	'Alert.ExcessiveGlobalMileage'	;
UPDATE TRANSLATION.TRANSLATION SET NAME =	'feat_alerts_logisticsalerts_hoursofservice(realtime)'	, VALUE =	'Alerts.LogisticsAlerts.HoursofService(Realtime)'	WHERE VALUE = 	'Alert.HoursofService'	;
UPDATE TRANSLATION.TRANSLATION SET NAME =	'feat_alerts_fuelanddriverperformance_excessiveaveragespeed(realtime)'	, VALUE =	'Alerts.FuelandDriverPerformance.ExcessiveAverageSpeed(Realtime)'	WHERE VALUE = 	'Alert.ExcessiveAverageSpeed'	;
UPDATE TRANSLATION.TRANSLATION SET NAME =	'feat_alerts_fuelanddriverperformance_excessiveidling(realtime)'	, VALUE =	'Alerts.FuelandDriverPerformance.ExcessiveIdling(Realtime)'	WHERE VALUE = 	'Alert.Excessiveidles'	;
UPDATE TRANSLATION.TRANSLATION SET NAME =	'feat_alerts_fuelanddriverperformance_fuelincreaseduringstop(realtime)'	, VALUE =	'Alerts.FuelandDriverPerformance.FuelIncreaseDuringStop(Realtime)'	WHERE VALUE = 	'Alert.Fuelincreaseduringstop'	;
UPDATE TRANSLATION.TRANSLATION SET NAME =	'feat_alerts_fuelanddriverperformance_fuellossduringstop(realtime)'	, VALUE =	'Alerts.FuelandDriverPerformance.FuelLossDuringStop(Realtime)'	WHERE VALUE = 	'Alert.Fuellossduringstop'	;
UPDATE TRANSLATION.TRANSLATION SET NAME =	'feat_alerts_fuelanddriverperformance_fuellossduringtrip(realtime)'	, VALUE =	'Alerts.FuelandDriverPerformance.FuelLossDuringTrip(Realtime)'	WHERE VALUE = 	'Alert.Fuellossduringtrip'	;



INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','feat_alerts_logisticsalerts_excessiveunderutilisationindays(batch)','Alerts.LogisticsAlerts.ExcessiveUnderUtilisationInDays(Batch)',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'alerts.logisticsalerts.excessiveunderutilisationindays(batch)');


INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','feat_alerts_logisticsalerts_excessiveunderutilisationinhours(batch)','Alerts.LogisticsAlerts.ExcessiveUnderUtilisationInHours(Batch)',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'alerts.logisticsalerts.excessiveunderutilisationinhours(batch)');


INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','feat_alerts_fuelanddriverperformance_fuelconsumed','Alerts.FuelandDriverPerformance.FuelConsumed',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'alerts.fuelanddriverperformance.fuelconsumed');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','feat_alerts_repairandmaintenance_statuschangetostopnow','Alerts.RepairandMaintenance.StatusChangetoStopNow',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'alerts.repairandmaintenance.statuschangetostopnow');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','feat_alerts_repairandmaintenance_statuschangetoservicenow','Alerts.RepairandMaintenance.StatusChangetoServiceNow',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'alerts.repairandmaintenance.statuschangetoservicenow');


UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =	'feat_alerts_logisticsalerts_enteringzone'	WHERE NAME = 	'feat_alert_enteringpoizone'	;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =	'feat_alerts_logisticsalerts_exitingzone'	WHERE NAME = 	'feat_alert_exitingpoizone'	;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =	'feat_alerts_logisticsalerts_exitingcorridor'	WHERE NAME = 	'feat_alert_outofcorridor'	;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =	'feat_alerts_logisticsalerts_excessivedistancedone(trip)'	WHERE NAME = 	'feat_alert_excessivedistancedone'	;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =	'feat_alerts_logisticsalerts_excessivedrivingduration(trip)'	WHERE NAME = 	'feat_alert_excessivedrivingduration'	;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =	'feat_alerts_logisticsalerts_excessiveglobalmileage(trip)'	WHERE NAME = 	'feat_alert_excessiveglobalmileage'	;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =	'feat_alerts_logisticsalerts_hoursofservice(realtime)'	WHERE NAME = 	'feat_alert_hoursofservice'	;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =	'feat_alerts_fuelanddriverperformance_excessiveaveragespeed(realtime)'	WHERE NAME = 	'feat_alert_excessiveaveragespeed'	;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =	'feat_alerts_fuelanddriverperformance_excessiveidling(realtime)'	WHERE NAME = 	'feat_alert_excessiveidles'	;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =	'feat_alerts_fuelanddriverperformance_fuelincreaseduringstop(realtime)'	WHERE NAME = 	'feat_alert_fuelincreaseduringstop'	;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =	'feat_alerts_fuelanddriverperformance_fuellossduringstop(realtime)'	WHERE NAME = 	'feat_alert_fuellossduringstop'	;
UPDATE TRANSLATION.TRANSLATIONGROUPING SET NAME =	'feat_alerts_fuelanddriverperformance_fuellossduringtrip(realtime)'	WHERE NAME = 	'feat_alert_fuellossduringtrip'	;



INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_alerts_logisticsalerts_excessiveunderutilisationindays(batch)',(select id from master.menu where name = 'Alerts'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_alerts_logisticsalerts_excessiveunderutilisationindays(batch)' 
and ref_id=(select id from master.menu where name = 'Alerts'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_alerts_logisticsalerts_excessiveunderutilisationinhours(batch)',(select id from master.menu where name = 'Alerts'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_alerts_logisticsalerts_excessiveunderutilisationinhours(batch)' 
and ref_id=(select id from master.menu where name = 'Alerts'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_alerts_fuelanddriverperformance_fuelconsumed',(select id from master.menu where name = 'Alerts'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_alerts_fuelanddriverperformance_fuelconsumed' 
and ref_id=(select id from master.menu where name = 'Alerts'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_alerts_repairandmaintenance_statuschangetostopnow',(select id from master.menu where name = 'Alerts'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_alerts_repairandmaintenance_statuschangetostopnow' 
and ref_id=(select id from master.menu where name = 'Alerts'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_alerts_repairandmaintenance_statuschangetoservicenow',(select id from master.menu where name = 'Alerts'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_alerts_repairandmaintenance_statuschangetoservicenow' 
and ref_id=(select id from master.menu where name = 'Alerts'));


--------Trip fleet utilization

--Details
UPDATE master.dataattribute SET name = 'Report.Details.NumberOfTrips',		column_name_for_ui = 'Number Of Trips'			WHERE name = 'Report.NumberOfTrips'	;
UPDATE master.dataattribute SET name = 'Report.Details.TripTime',		column_name_for_ui = 'Trip Time'			WHERE name = 'Report.Triptime'	;
UPDATE master.dataattribute SET name = 'Report.Details.StopTime',		column_name_for_ui = 'Stop Time'			WHERE name = 'Report.Stoptime'	;
UPDATE master.dataattribute SET name = 'Report.Details.AverageWeightPerTrip',	key ='da_report_averageweightpertrip',	column_name_for_ui = 'Average Weight Per Trip'			WHERE name = 'Report.Averageweightpertrip(t)'	;
UPDATE master.dataattribute SET name = 'Report.Details.AverageDistancePerDay',		column_name_for_ui = 'Average Distance Per Day'			WHERE name = 'Report.Averagedistanceperday'	;

--General
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.General.IdleDuration'	,'A',	'da_report_idleduration'	,	'Idle Duration'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.general.idleduration'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.General.NumberOfTrips'	,'A',	'da_report_numberoftrips'	,	'Number Of Trips'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.general.numberoftrips'	);

UPDATE master.dataattribute SET name = 'Report.General.NumberOfVehicles',		column_name_for_ui = 'Number of Vehicles'			WHERE name = 'Report.Numberofvehicles'	;
UPDATE master.dataattribute SET name = 'Report.General.TotalDistance',		column_name_for_ui = 'Total Distance'			WHERE name = 'Report.TotalDistancecovered'	;
UPDATE master.dataattribute SET name = 'Report.General.AverageDistancePerDay',		column_name_for_ui = 'Average Distance Per Day'			WHERE name = 'Report.AverageDistanceperday(km)'	;

--Charts

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.Charts.DistancePerDay'	,'A',	'da_report_distanceperday'	,	'Distance Per Day'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.charts.distanceperday'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.Charts.NumberOfVehiclesPerDay'	,'A',	'da_report_numberofvehiclesperday'	,	'Number Of Vehicles Per Day'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.charts.numberofvehiclesperday'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.Charts.MileageBasedUtilization'	,'A',	'da_report_mileagebasedutilization'	,	'Mileage Based Utilization'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.charts.mileagebasedutilization'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.Charts.TimeBasedUtilization'	,'A',	'da_report_timebasedutilization'	,	'Time Based Utilization'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.charts.timebasedutilization'	);

--Calender view

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.CalendarView.AverageWeight'	,'A',	'da_report_averageweight'	,	'Average Weight'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.calendarview.averageweight'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.CalendarView.IdleDuration'	,'A',	'da_report_idleduration'	,	'Idle Duration'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.calendarview.idleduration'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.CalendarView.Distance'	,'A',	'da_report_distance'	,	'Distance'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.calendarview.distance'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.CalendarView.DrivingTime'	,'A',	'da_report_drivingtime'	,	'Driving Time'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.calendarview.drivingtime'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.CalendarView.ActiveVehicles'	,'A',	'da_report_activevehicles'	,	'Active Vehicles'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.calendarview.activevehicles'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.CalendarView.TotalTrips'	,'A',	'da_report_totaltrips'	,	'Total Trips'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.calendarview.totaltrips'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.CalendarView.MileageBasedUtilization'	,'A',	'da_report_mileagebasedutilization'	,	'Mileage Based Utilization'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.calendarview.mileagebasedutilization'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.CalendarView.TimeBasedUtilization'	,'A',	'da_report_timebasedutilization'	,	'Time Based Utilization'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.calendarview.timebasedutilization'	);


--Report Attribute entries

INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.general.numberofvehicles')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.general.numberofvehicles'));
				  
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.general.totaldistance')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.general.totaldistance'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.general.numberoftrips')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.general.numberoftrips'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.general.averagedistanceperday')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.general.averagedistanceperday'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.general.idleduration')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.general.idleduration'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.details.vehiclename')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.details.vehiclename'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.details.vin')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.details.vin'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.details.registrationnumber')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.details.registrationnumber'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.details.distance')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.details.distance'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.details.numberoftrips')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.details.numberoftrips'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.details.triptime')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.details.triptime'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.details.drivingtime')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.details.drivingtime'));
				  
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.details.idleduration')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.details.idleduration'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.details.stoptime')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.details.stoptime'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.details.averagespeed')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.details.averagespeed'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.details.odometer')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.details.odometer'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.details.averageweightpertrip')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.details.averageweightpertrip'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.details.averagedistanceperday')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.details.averagedistanceperday'));
				  

--report attributes charts entries

INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.charts.distanceperday')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.charts.distanceperday'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.charts.numberofvehiclesperday')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.charts.numberofvehiclesperday'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.charts.mileagebasedutilization')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.charts.mileagebasedutilization'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.charts.timebasedutilization')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.charts.timebasedutilization'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.calendarview.averageweight')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.calendarview.averageweight'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.calendarview.idleduration')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.calendarview.idleduration'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.calendarview.distance')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.calendarview.distance'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.calendarview.drivingtime')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.calendarview.drivingtime'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.calendarview.activevehicles')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.calendarview.activevehicles'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.calendarview.totaltrips')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.calendarview.totaltrips'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.calendarview.mileagebasedutilization')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.calendarview.mileagebasedutilization'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.calendarview.timebasedutilization')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'fleet utilisation report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.calendarview.timebasedutilization'));

				  
--Translation

UPDATE TRANSLATION.TRANSLATION SET name = 'da_report_averageweightpertrip' , VALUE = 'Report.Details.AverageWeightPerTrip' WHERE VALUE = 'Report.Averageweightpertrip(t)';


INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_distanceperday','Report.Charts.DistancePerDay',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.charts.distanceperday');


INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_numberofvehiclesperday','Report.Charts.NumberOfVehiclesPerDay',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.charts.numberofvehiclesperday');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_mileagebasedutilization','Report.Charts.MileageBasedUtilization',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.charts.mileagebasedutilization');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_timebasedutilization','Report.Charts.TimeBasedUtilization',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.charts.timebasedutilization');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_averageweight','Report.CalendarView.AverageWeight',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.calendarview.averageweight');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_idleduration','Report.CalendarView.IdleDuration',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.calendarview.idleduration');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_distance','Report.CalendarView.Distance',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.calendarview.distance');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_drivingtime','Report.CalendarView.DrivingTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.calendarview.drivingtime');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_activevehicles','Report.CalendarView.ActiveVehicles',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.calendarview.activevehicles');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_totaltrips','Report.CalendarView.TotalTrips',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.calendarview.totaltrips');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_mileagebasedutilization','Report.CalendarView.MileageBasedUtilization',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.calendarview.mileagebasedutilization');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_timebasedutilization','Report.CalendarView.TimeBasedUtilization',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.calendarview.timebasedutilization');		  

--Translation grouping

UPDATE TRANSLATION.TRANSLATIONGROUPING SET name = 'da_report_averageweightpertrip' WHERE name = 'da_report_averageweightpertrip(t)';


INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_distanceperday',(select id from master.menu where name = 'Fleet Utilisation'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_distanceperday' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));


INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_numberofvehiclesperday',(select id from master.menu where name = 'Fleet Utilisation'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_numberofvehiclesperday' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_mileagebasedutilization',(select id from master.menu where name = 'Fleet Utilisation'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_mileagebasedutilization' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_timebasedutilization',(select id from master.menu where name = 'Fleet Utilisation'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_timebasedutilization' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_averageweight',(select id from master.menu where name = 'Fleet Utilisation'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_averageweight' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_idleduration',(select id from master.menu where name = 'Fleet Utilisation'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_idleduration' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_distance',(select id from master.menu where name = 'Fleet Utilisation'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_distance' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_drivingtime',(select id from master.menu where name = 'Fleet Utilisation'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_drivingtime' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_activevehicles',(select id from master.menu where name = 'Fleet Utilisation'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_activevehicles' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_totaltrips',(select id from master.menu where name = 'Fleet Utilisation'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_totaltrips' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_mileagebasedutilization',(select id from master.menu where name = 'Fleet Utilisation'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_mileagebasedutilization' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_timebasedutilization',(select id from master.menu where name = 'Fleet Utilisation'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_timebasedutilization' 
and ref_id=(select id from master.menu where name = 'Fleet Utilisation'));

--Schedule Report
--Data attributes

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.ReportType'	,'A',	'da_report_reporttype'	,	'Report Type'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.reporttype'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.VehicleGroup'	,'A',	'da_report_vehiclegroup'	,	'Vehicle Group'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.vehiclegroup'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.Frequency'	,'A',	'da_report_frequency'	,	'Frequency'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.frequency'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.Recipients'	,'A',	'da_report_recipients'	,	'Recipients'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.recipients'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.LastRun'	,'A',	'da_report_lastrun'	,	'Last Run'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.lastrun'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.NextRun'	,'A',	'da_report_nextrun'	,	'Next Run'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.nextrun'	);
INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	SELECT 	'Report.Status'	,'A',	'da_report_status'	,	'Status'	WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE lower(name) = 	'report.status'	);

-- Report

INSERT INTO master.report  (name,key)  SELECT 'Schedule Report', 'lblScheduleReport'  WHERE NOT EXISTS (SELECT 1 FROM master.report WHERE lower(name) = 'schedule report');


--Report Attributes

INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'schedule report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.reporttype')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'schedule report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.reporttype'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'schedule report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.vehiclegroup')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'schedule report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.vehiclegroup'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'schedule report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.vehicle')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'schedule report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.vehicle'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'schedule report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.frequency')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'schedule report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.frequency'));

INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'schedule report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.recipients')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'schedule report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.recipients'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'schedule report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.lastrun')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'schedule report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.lastrun'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'schedule report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.nextrun')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'schedule report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.nextrun'));
				  
INSERT INTO master.reportattribute (report_id,data_attribute_id) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'schedule report'),(SELECT id from master.dataattribute WHERE lower(name) = 'report.status')
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'schedule report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE lower(name) = 'report.status'));
				  

--Translation

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_reporttype','Report.ReportType',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.reporttype');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_vehiclegroup','Report.VehicleGroup',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.vehiclegroup');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_frequency','Report.Frequency',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.frequency');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_recipients','Report.Recipients',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.recipients');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_lastrun','Report.LastRun',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.lastrun');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_nextrun','Report.NextRun',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.nextrun');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_status','Report.Status',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE lower(value) = 'report.status');


-- Translation grouping

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_reporttype',(select id from master.menu where name = 'Report Scheduler'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_reporttype' 
and ref_id=(select id from master.menu where name = 'Report Scheduler'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_vehiclegroup',(select id from master.menu where name = 'Report Scheduler'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_vehiclegroup' 
and ref_id=(select id from master.menu where name = 'Report Scheduler'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_frequency',(select id from master.menu where name = 'Report Scheduler'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_frequency' 
and ref_id=(select id from master.menu where name = 'Report Scheduler'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_recipients',(select id from master.menu where name = 'Report Scheduler'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_recipients' 
and ref_id=(select id from master.menu where name = 'Report Scheduler'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_lastrun',(select id from master.menu where name = 'Report Scheduler'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_lastrun' 
and ref_id=(select id from master.menu where name = 'Report Scheduler'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_nextrun',(select id from master.menu where name = 'Report Scheduler'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_nextrun' 
and ref_id=(select id from master.menu where name = 'Report Scheduler'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_status',(select id from master.menu where name = 'Report Scheduler'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_status' 
and ref_id=(select id from master.menu where name = 'Report Scheduler'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_report_vehicle',(select id from master.menu where name = 'Report Scheduler'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_report_vehicle' 
and ref_id=(select id from master.menu where name = 'Report Scheduler'));

--Drive time management

--General section

UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.General.DriversCount', COLUMN_NAME_FOR_UI = 'Drivers Count' WHERE NAME = 'Report.DriversCount';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.General.TotalDriveTime', COLUMN_NAME_FOR_UI = 'Total Drive Time' WHERE NAME = 'Report.TotalDriveTime';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.General.TotalWorkTime', COLUMN_NAME_FOR_UI = 'Total Work Time'  WHERE NAME = 'Report.TotalWorkTime';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.General.TotalAvailableTime', COLUMN_NAME_FOR_UI = 'Total Available Time' WHERE NAME = 'Report.TotalAvailableTime';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.General.TotalRestTime', COLUMN_NAME_FOR_UI = 'Total Rest Time' WHERE NAME = 'Report.TotalRestTime';

--Details section

UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.Details.DriverId' , COLUMN_NAME_FOR_UI = 'Driver Id' WHERE NAME = 'Report.Driver';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.Details.DriverName', COLUMN_NAME_FOR_UI = 'Driver Name' WHERE NAME = 'Report.DriverName';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.Details.EndTime', COLUMN_NAME_FOR_UI = 'End Time' WHERE NAME = 'Report.EndTime';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.Details.StartTime', COLUMN_NAME_FOR_UI = 'Start Time' WHERE NAME = 'Report.StartTime';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.Details.WorkTime', COLUMN_NAME_FOR_UI = 'Work Time' WHERE NAME = 'Report.WorkTime';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.Details.AvailableTime', COLUMN_NAME_FOR_UI = 'Available Time' WHERE NAME = 'Report.AvailableTime';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.Details.ServiceTime', COLUMN_NAME_FOR_UI = 'Service Time' WHERE NAME = 'Report.ServiceTime';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.Details.RestTime', COLUMN_NAME_FOR_UI = 'Rest Time' WHERE NAME = 'Report.RestTime';
UPDATE MASTER.DATAATTRIBUTE SET NAME = 'Report.Details.DriveTime', COLUMN_NAME_FOR_UI = 'Drive Time' WHERE NAME = 'Report.DriveTime';

insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.general.driverscount')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.general.driverscount'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.general.totaldrivetime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.general.totaldrivetime'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.general.totalworktime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.general.totalworktime'));


insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.general.totalavailabletime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.general.totalavailabletime'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.general.totalresttime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.general.totalresttime'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.driverid')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.driverid'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.drivername')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.drivername'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.endtime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.endtime'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.starttime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.starttime'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.worktime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.worktime'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.availabletime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.availabletime'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.servicetime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.servicetime'));
				  
insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.resttime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.resttime'));

insert into MASTER.REPORTATTRIBUTE (REPORT_ID,DATA_ATTRIBUTE_ID) 
SELECT (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management'),(SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.drivetime')
WHERE NOT EXISTS (SELECT 1 FROM MASTER.REPORTATTRIBUTE WHERE REPORT_ID = (SELECT id from MASTER.REPORT WHERE lower(name) = 'drive time management') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from MASTER.DATAATTRIBUTE WHERE lower(name) = 'report.details.drivetime'));
				  


