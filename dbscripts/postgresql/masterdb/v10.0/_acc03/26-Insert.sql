insert into master.vehiclemsgtriggertype (id,name) select 1   ,'TIMER_EVENT' where not exists (select 1 from master.vehiclemsgtriggertype where id=1    and name='TIMER_EVENT');
insert into master.vehiclemsgtriggertype (id,name) select 14 ,'POWER_ON / IGNITION_ON_EVENT' where not exists (select 1 from master.vehiclemsgtriggertype where id=14  and name='POWER_ON / IGNITION_ON_EVENT');
insert into master.vehiclemsgtriggertype (id,name) select 15 ,'POWER_OFF / IGNITION_OFF_EVENT' where not exists (select 1 from master.vehiclemsgtriggertype where id=15  and name='POWER_OFF / IGNITION_OFF_EVENT');
insert into master.vehiclemsgtriggertype (id,name) select 24 ,'DRIVER1_WORKINGSTATE_UPDATED' where not exists (select 1 from master.vehiclemsgtriggertype where id=24  and name='DRIVER1_WORKINGSTATE_UPDATED');
insert into master.vehiclemsgtriggertype (id,name) select 25 ,'DRIVER2_WORKINGSTATE_UPDATED' where not exists (select 1 from master.vehiclemsgtriggertype where id=25  and name='DRIVER2_WORKINGSTATE_UPDATED');
insert into master.vehiclemsgtriggertype (id,name) select 26 ,'TELL_TALE' where not exists (select 1 from master.vehiclemsgtriggertype where id=26  and name='TELL_TALE');
insert into master.vehiclemsgtriggertype (id,name) select 28 ,'PTO_ON' where not exists (select 1 from master.vehiclemsgtriggertype where id=28  and name='PTO_ON');
insert into master.vehiclemsgtriggertype (id,name) select 29 ,'PTO_OFF' where not exists (select 1 from master.vehiclemsgtriggertype where id=29  and name='PTO_OFF');
insert into master.vehiclemsgtriggertype (id,name) select 30 ,'DRIVER_LOGON' where not exists (select 1 from master.vehiclemsgtriggertype where id=30  and name='DRIVER_LOGON');
insert into master.vehiclemsgtriggertype (id,name) select 31 ,'DRIVER_LOGOFF' where not exists (select 1 from master.vehiclemsgtriggertype where id=31  and name='DRIVER_LOGOFF');
insert into master.vehiclemsgtriggertype (id,name) select 32 ,'DISTANCE_TRAVELED' where not exists (select 1 from master.vehiclemsgtriggertype where id=32  and name='DISTANCE_TRAVELED');

insert into master.driverauthequipment (id,name) select 1,'RESERVED' where not exists (select 1 from master.driverauthequipment where id=1 and name='RESERVED');
insert into master.driverauthequipment (id,name) select 2,'DRIVER_CARD' where not exists (select 1 from master.driverauthequipment where id=2 and name='DRIVER_CARD');
insert into master.driverauthequipment (id,name) select 3,'CONTROL_CARD' where not exists (select 1 from master.driverauthequipment where id=3 and name='CONTROL_CARD');
insert into master.driverauthequipment (id,name) select 4,'COMPANY_CARD' where not exists (select 1 from master.driverauthequipment where id=4 and name='COMPANY_CARD');
insert into master.driverauthequipment (id,name) select 5,'MANUFACTURING_CARD' where not exists (select 1 from master.driverauthequipment where id=5 and name='MANUFACTURING_CARD');
insert into master.driverauthequipment (id,name) select 6,'VEHICLE_UNIT' where not exists (select 1 from master.driverauthequipment where id=6 and name='VEHICLE_UNIT');
insert into master.driverauthequipment (id,name) select 7,'MOTION_SENSOR' where not exists (select 1 from master.driverauthequipment where id=7 and name='MOTION_SENSOR');

insert into master.telltale (id,name) select 1,'Cooling air conditioning' where not exists (select 1 from master.telltale where id=1 and name='Cooling air conditioning');
insert into master.telltale (id,name) select 2,'High beam, main beam' where not exists (select 1 from master.telltale where id=2 and name='High beam, main beam');
insert into master.telltale (id,name) select 3,'Low beam, dipped beam' where not exists (select 1 from master.telltale where id=3 and name='Low beam, dipped beam');
insert into master.telltale (id,name) select 4,'Turn signals' where not exists (select 1 from master.telltale where id=4 and name='Turn signals');
insert into master.telltale (id,name) select 5,'Hazard warning' where not exists (select 1 from master.telltale where id=5 and name='Hazard warning');
insert into master.telltale (id,name) select 6,'Provision for the disabled or handicapped persons' where not exists (select 1 from master.telltale where id=6 and name='Provision for the disabled or handicapped persons');
insert into master.telltale (id,name) select 7,'Parking Brake' where not exists (select 1 from master.telltale where id=7 and name='Parking Brake');
insert into master.telltale (id,name) select 8,'Brake failure/brake system malfunction' where not exists (select 1 from master.telltale where id=8 and name='Brake failure/brake system malfunction');
insert into master.telltale (id,name) select 9,'Hatch open' where not exists (select 1 from master.telltale where id=9 and name='Hatch open');
insert into master.telltale (id,name) select 10,'Fuel level' where not exists (select 1 from master.telltale where id=10 and name='Fuel level');
insert into master.telltale (id,name) select 11,'Engine coolant temperature' where not exists (select 1 from master.telltale where id=11 and name='Engine coolant temperature');
insert into master.telltale (id,name) select 12,'Battery charging condition' where not exists (select 1 from master.telltale where id=12 and name='Battery charging condition');
insert into master.telltale (id,name) select 13,'Engine oil' where not exists (select 1 from master.telltale where id=13 and name='Engine oil');
insert into master.telltale (id,name) select 14,'Position lights, side lights' where not exists (select 1 from master.telltale where id=14 and name='Position lights, side lights');
insert into master.telltale (id,name) select 15,'Front fog light' where not exists (select 1 from master.telltale where id=15 and name='Front fog light');
insert into master.telltale (id,name) select 16,'Rear fog light' where not exists (select 1 from master.telltale where id=16 and name='Rear fog light');
insert into master.telltale (id,name) select 17,'Park Heating' where not exists (select 1 from master.telltale where id=17 and name='Park Heating');
insert into master.telltale (id,name) select 18,'Engine / Mil indicator' where not exists (select 1 from master.telltale where id=18 and name='Engine / Mil indicator');
insert into master.telltale (id,name) select 19,'Service, call for maintenance' where not exists (select 1 from master.telltale where id=19 and name='Service, call for maintenance');
insert into master.telltale (id,name) select 20,'Transmission fluid temperature' where not exists (select 1 from master.telltale where id=20 and name='Transmission fluid temperature');
insert into master.telltale (id,name) select 21,'Transmission failure/malfunction' where not exists (select 1 from master.telltale where id=21 and name='Transmission failure/malfunction');
insert into master.telltale (id,name) select 22,'Anti-lock brake system failure' where not exists (select 1 from master.telltale where id=22 and name='Anti-lock brake system failure');
insert into master.telltale (id,name) select 23,'Worn brake linings' where not exists (select 1 from master.telltale where id=23 and name='Worn brake linings');
insert into master.telltale (id,name) select 24,'Windscreen washer fluid/windshield washer fluid' where not exists (select 1 from master.telltale where id=24 and name='Windscreen washer fluid/windshield washer fluid');
insert into master.telltale (id,name) select 25,'Tire failure/malfunction' where not exists (select 1 from master.telltale where id=25 and name='Tire failure/malfunction');
insert into master.telltale (id,name) select 26,'Malfunction/general failure' where not exists (select 1 from master.telltale where id=26 and name='Malfunction/general failure');
insert into master.telltale (id,name) select 27,'Engine oil temperature' where not exists (select 1 from master.telltale where id=27 and name='Engine oil temperature');
insert into master.telltale (id,name) select 28,'Engine oil level' where not exists (select 1 from master.telltale where id=28 and name='Engine oil level');
insert into master.telltale (id,name) select 29,'Engine coolant level' where not exists (select 1 from master.telltale where id=29 and name='Engine coolant level');
insert into master.telltale (id,name) select 30,'Steering fluid level' where not exists (select 1 from master.telltale where id=30 and name='Steering fluid level');
insert into master.telltale (id,name) select 31,'Steering failure' where not exists (select 1 from master.telltale where id=31 and name='Steering failure');
insert into master.telltale (id,name) select 32,'Height Control (Levelling)' where not exists (select 1 from master.telltale where id=32 and name='Height Control (Levelling)');
insert into master.telltale (id,name) select 33,'Retarder' where not exists (select 1 from master.telltale where id=33 and name='Retarder');
insert into master.telltale (id,name) select 34,'Engine Emission system failure (Mil indicator)' where not exists (select 1 from master.telltale where id=34 and name='Engine Emission system failure (Mil indicator)');
insert into master.telltale (id,name) select 35,'ESC indication' where not exists (select 1 from master.telltale where id=35 and name='ESC indication');
insert into master.telltale (id,name) select 36,'Brake lights' where not exists (select 1 from master.telltale where id=36 and name='Brake lights');
insert into master.telltale (id,name) select 37,'Articulation' where not exists (select 1 from master.telltale where id=37 and name='Articulation');
insert into master.telltale (id,name) select 38,'Stop Request' where not exists (select 1 from master.telltale where id=38 and name='Stop Request');
insert into master.telltale (id,name) select 39,'Pram request' where not exists (select 1 from master.telltale where id=39 and name='Pram request');
insert into master.telltale (id,name) select 40,'Bus stop brake' where not exists (select 1 from master.telltale where id=40 and name='Bus stop brake');
insert into master.telltale (id,name) select 41,'AdBlue level' where not exists (select 1 from master.telltale where id=41 and name='AdBlue level');
insert into master.telltale (id,name) select 42,'Raising' where not exists (select 1 from master.telltale where id=42 and name='Raising');
insert into master.telltale (id,name) select 43,'Lowering' where not exists (select 1 from master.telltale where id=43 and name='Lowering');
insert into master.telltale (id,name) select 44,'Kneeling' where not exists (select 1 from master.telltale where id=44 and name='Kneeling');
insert into master.telltale (id,name) select 45,'Engine compartment temperature' where not exists (select 1 from master.telltale where id=45 and name='Engine compartment temperature');
insert into master.telltale (id,name) select 46,'Auxiliary air pressure' where not exists (select 1 from master.telltale where id=46 and name='Auxiliary air pressure');
insert into master.telltale (id,name) select 47,'Air filter clogged' where not exists (select 1 from master.telltale where id=47 and name='Air filter clogged');
insert into master.telltale (id,name) select 48,'Fuel filter differential pressure' where not exists (select 1 from master.telltale where id=48 and name='Fuel filter differential pressure');
insert into master.telltale (id,name) select 49,'Seat belt' where not exists (select 1 from master.telltale where id=49 and name='Seat belt');
insert into master.telltale (id,name) select 50,'EBS' where not exists (select 1 from master.telltale where id=50 and name='EBS');
insert into master.telltale (id,name) select 51,'Lane departure indication' where not exists (select 1 from master.telltale where id=51 and name='Lane departure indication');
insert into master.telltale (id,name) select 52,'Advanced emergency braking system' where not exists (select 1 from master.telltale where id=52 and name='Advanced emergency braking system');
insert into master.telltale (id,name) select 53,'ACC' where not exists (select 1 from master.telltale where id=53 and name='ACC');
insert into master.telltale (id,name) select 54,'Trailer connected' where not exists (select 1 from master.telltale where id=54 and name='Trailer connected');
insert into master.telltale (id,name) select 55,'ABS Trailer 1,2' where not exists (select 1 from master.telltale where id=55 and name='ABS Trailer 1,2');
insert into master.telltale (id,name) select 56,'Airbag' where not exists (select 1 from master.telltale where id=56 and name='Airbag');
insert into master.telltale (id,name) select 57,'EBS Trailer 1,2' where not exists (select 1 from master.telltale where id=57 and name='EBS Trailer 1,2');
insert into master.telltale (id,name) select 58,'Tachograph indication' where not exists (select 1 from master.telltale where id=58 and name='Tachograph indication');
insert into master.telltale (id,name) select 59,'ESC switched off' where not exists (select 1 from master.telltale where id=59 and name='ESC switched off');
insert into master.telltale (id,name) select 60,'Lane departure warning switched off' where not exists (select 1 from master.telltale where id=60 and name='Lane departure warning switched off');
insert into master.telltale (id,name) select 61,'Engine emission filter (Soot Filter)' where not exists (select 1 from master.telltale where id=61 and name='Engine emission filter (Soot Filter)');
insert into master.telltale (id,name) select 62,'Electric motor failures' where not exists (select 1 from master.telltale where id=62 and name='Electric motor failures');
insert into master.telltale (id,name) select 63,'AdBlue tampering' where not exists (select 1 from master.telltale where id=63 and name='AdBlue tampering');
insert into master.telltale (id,name) select 64,'Multiplex System' where not exists (select 1 from master.telltale where id=64 and name='Multiplex System');

insert into master.telltalestate (id,name) select 0,'OFF' where not exists (select 1 from master.telltalestate where id=0 and name='OFF');
insert into master.telltalestate (id,name) select 1,'ON & RED' where not exists (select 1 from master.telltalestate where id=1 and name='ON & RED');
insert into master.telltalestate (id,name) select 2,'ON & YELLOW' where not exists (select 1 from master.telltalestate where id=2 and name='ON & YELLOW');
insert into master.telltalestate (id,name) select 3,'ON & INFO' where not exists (select 1 from master.telltalestate where id=3 and name='ON & INFO');
insert into master.telltalestate (id,name) select 4,'Reserved_4 (FMS Tell Tale Status)' where not exists (select 1 from master.telltalestate where id=4 and name='Reserved (FMS Tell Tale Status)');
insert into master.telltalestate (id,name) select 5,'Reserved_5 (FMS Tell Tale Status)' where not exists (select 1 from master.telltalestate where id=5 and name='Reserved (FMS Tell Tale Status)');
insert into master.telltalestate (id,name) select 6,'Reserved_6 (FMS Tell Tale Status)' where not exists (select 1 from master.telltalestate where id=6 and name='Reserved (FMS Tell Tale Status)');
insert into master.telltalestate (id,name) select 7,'Not Available (FMS Tell Tale Status)' where not exists (select 1 from master.telltalestate where id=7 and name='Not Available (FMS Tell Tale Status)');

insert into master.co2coefficient(description,fuel_type,coefficient)
select 'Gas','G',2.9 where not exists (select 1 from master.co2coefficient where description='Gas');
insert into master.co2coefficient(description,fuel_type,coefficient)
select 'Motor Gasoline','M',2.8 where not exists (select 1 from master.co2coefficient where description='Motor Gasoline');
insert into master.co2coefficient(description,fuel_type,coefficient)
select 'Bio Gasoline','O',1.8 where not exists (select 1 from master.co2coefficient where description='Bio Gasoline');


--Dashboard
INSERT INTO master.report  (name , key)   
SELECT 'Dashboard','lblDashboard' WHERE NOT EXISTS  (   SELECT 1   FROM master.report  WHERE name = 'Dashboard');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','lblDashboard','Dashboard',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard' and name ='lblDashboard');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'lblDashboard',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'lblDashboard' 
and ref_id=0);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard'	,'G',	'da_dashboard'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard'	);


INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.FleetKPI'	,'G',	'da_dashboard_fleetkpi'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.FleetKPI'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.FleetKPI.CO2Emission'	,'A',	'da_dashboard_fleetkpi_co2emission'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.FleetKPI.CO2Emission'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.FleetKPI.TotalDistance'	,'A',	'da_dashboard_fleetkpi_totaldistance'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.FleetKPI.TotalDistance'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.FleetKPI.DrivingTime'	,'A',	'da_dashboard_fleetkpi_drivingtime'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.FleetKPI.DrivingTime'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.FleetKPI.FuelConsumption'	,'A',	'da_dashboard_fleetkpi_fuelconsumption'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.FleetKPI.FuelConsumption'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.FleetKPI.FuelUsedIdling'	,'A',	'da_dashboard_fleetkpi_fuelusedidling'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.FleetKPI.FuelUsedIdling'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.FleetKPI.IdlingTime'	,'A',	'da_dashboard_fleetkpi_idlingtime'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.FleetKPI.IdlingTime'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.FleetKPI.FuelConsumed'	,'A',	'da_dashboard_fleetkpi_fuelconsumed'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.FleetKPI.FuelConsumed'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.TodayLiveVehicle'	,'G',	'da_dashboard_todaylivevehicle'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.TodayLiveVehicle'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.TodayLiveVehicle.Distance'	,'A',	'da_dashboard_todaylivevehicle_distance'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.TodayLiveVehicle.Distance'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.TodayLiveVehicle.DrivingTime'	,'A',	'da_dashboard_todaylivevehicle_drivingtime'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.TodayLiveVehicle.DrivingTime'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.TodayLiveVehicle.Drivers'	,'A',	'da_dashboard_todaylivevehicle_drivers'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.TodayLiveVehicle.Drivers'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.TodayLiveVehicle.CriticalAlerts'	,'A',	'da_dashboard_todaylivevehicle_criticalalerts'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.TodayLiveVehicle.CriticalAlerts'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.TodayLiveVehicle.TimeBasedUtilizationRate'	,'A',	'da_dashboard_todaylivevehicle_timebasedutilizationrate'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.TodayLiveVehicle.TimeBasedUtilizationRate'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.TodayLiveVehicle.DistanceBasedUtilizationRate'	,'A',	'da_dashboard_todaylivevehicle_distancebasedutilizationrate'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.TodayLiveVehicle.DistanceBasedUtilizationRate'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.TodayLiveVehicle.ActiveVehicles'	,'A',	'da_dashboard_todaylivevehicle_activevehicles'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.TodayLiveVehicle.ActiveVehicles'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.VehicleUtilization'	,'G',	'da_dashboard_vehicleutilization'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.VehicleUtilization'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.VehicleUtilization.DistanceBasedUtilizationRate'	,'A',	'da_dashboard_vehicleutilization_distancebasedutilizationrate'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.VehicleUtilization.DistanceBasedUtilizationRate'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.VehicleUtilization.TimeBasedUtilizationRate'	,'A',	'da_dashboard_vehicleutilization_timebasedutilizationrate'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.VehicleUtilization.TimeBasedUtilizationRate'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.VehicleUtilization.DistancePerDay'	,'A',	'da_dashboard_vehicleutilization_distanceperday'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.VehicleUtilization.DistancePerDay'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.VehicleUtilization.ActiveVehiclesPerDay'	,'A',	'da_dashboard_vehicleutilization_activevehiclesperday'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.VehicleUtilization.ActiveVehiclesPerDay'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.AlertLast24Hours'	,'G',	'da_dashboard_alertlast24hours'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.AlertLast24Hours'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.AlertLast24Hours.LevelAlerts'	,'A',	'da_dashboard_alertlast24hours_levelalerts'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.AlertLast24Hours.LevelAlerts'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.AlertLast24Hours.TotalAlerts'	,'A',	'da_dashboard_alertlast24hours_totalalerts'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.AlertLast24Hours.TotalAlerts'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.AlertLast24Hours.LogisticAlert'	,'A',	'da_dashboard_alertlast24hours_logisticalert'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.AlertLast24Hours.LogisticAlert'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.AlertLast24Hours.Fuel&DriverAlerts'	,'A',	'da_dashboard_alertlast24hours_fueldriveralerts'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.AlertLast24Hours.Fuel&DriverAlerts'	);

INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Dashboard.AlertLast24Hours.Repair&MaintenanceAlerts'	,'A',	'da_dashboard_alertlast24hours_repairmaintenancealerts'	,	'Fuel Increase Events'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Dashboard.AlertLast24Hours.Repair&MaintenanceAlerts'	);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard','Dashboard',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard' and name ='da_dashboard');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard'),array [
(SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.VehicleUtilization'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.AlertLast24Hours')
],'C','rp_db_dashboard',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard','Dashboard',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_fleetkpi','Dashboard.FleetKPI',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.FleetKPI' and name ='da_dashboard_fleetkpi');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_fleetkpi',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_fleetkpi' 
and ref_id=0);


INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI'),array [
(SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.CO2Emission'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.TotalDistance'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.DrivingTime'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.FuelConsumption'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.FuelUsedIdling'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.IdlingTime'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.FuelConsumed')],'C','rp_db_dashboard_fleetkpi',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_fleetkpi','Fleet KPI (Component view settings)',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_fleetkpi');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_fleetkpi',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_fleetkpi' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_fleetkpi_co2emission','Dashboard.FleetKPI.CO2Emission',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.FleetKPI.CO2Emission' and name ='da_dashboard_fleetkpi_co2emission');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_fleetkpi_co2emission',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_fleetkpi_co2emission' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.CO2Emission'),null,'S','rp_db_dashboard_fleetkpi_co2emission',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.CO2Emission'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_fleetkpi_co2emission','CO2 Emission',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_fleetkpi_co2emission');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_fleetkpi_co2emission',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_fleetkpi_co2emission' 
and ref_id=0);
-----------------------------------
INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_fleetkpi_totaldistance','Dashboard.FleetKPI.TotalDistance',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.FleetKPI.TotalDistance' and name ='da_dashboard_fleetkpi_totaldistance');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_fleetkpi_totaldistance',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_fleetkpi_totaldistance' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.TotalDistance'),null,'S','rp_db_dashboard_fleetkpi_totaldistance',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.TotalDistance'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_fleetkpi_totaldistance','Total Distance',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_fleetkpi_totaldistance');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_fleetkpi_totaldistance',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_fleetkpi_totaldistance' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_fleetkpi_drivingtime','Dashboard.FleetKPI.DrivingTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.FleetKPI.DrivingTime' and name ='da_dashboard_fleetkpi_drivingtime');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_fleetkpi_drivingtime',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_fleetkpi_drivingtime' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.DrivingTime'),null,'S','rp_db_dashboard_fleetkpi_drivingtime',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.DrivingTime'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_fleetkpi_drivingtime','Driving Time',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_fleetkpi_drivingtime');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_fleetkpi_drivingtime',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_fleetkpi_drivingtime' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_fleetkpi_fuelconsumption','Dashboard.FleetKPI.FuelConsumption',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.FleetKPI.FuelConsumption' and name ='da_dashboard_fleetkpi_fuelconsumption');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_fleetkpi_fuelconsumption',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_fleetkpi_fuelconsumption' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.FuelConsumption'),null,'S','rp_db_dashboard_fleetkpi_fuelconsumption',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.FuelConsumption'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_fleetkpi_fuelconsumption','Fuel Consumption',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_fleetkpi_fuelconsumption');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_fleetkpi_fuelconsumption',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_fleetkpi_fuelconsumption' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_fleetkpi_fuelusedidling','Dashboard.FleetKPI.FuelUsedIdling',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.FleetKPI.FuelUsedIdling' and name ='da_dashboard_fleetkpi_fuelusedidling');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_fleetkpi_fuelusedidling',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_fleetkpi_fuelusedidling' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.FuelUsedIdling'),null,'S','rp_db_dashboard_fleetkpi_fuelusedidling',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.FuelUsedIdling'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_fleetkpi_fuelusedidling','Fuel Used Idling',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_fleetkpi_fuelusedidling');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_fleetkpi_fuelusedidling',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_fleetkpi_fuelusedidling' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_fleetkpi_idlingtime','Dashboard.FleetKPI.IdlingTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.FleetKPI.IdlingTime' and name ='da_dashboard_fleetkpi_idlingtime');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_fleetkpi_idlingtime',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_fleetkpi_idlingtime' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.IdlingTime'),null,'S','rp_db_dashboard_fleetkpi_idlingtime',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.IdlingTime'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_fleetkpi_idlingtime','Idling Time',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_fleetkpi_idlingtime');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_fleetkpi_idlingtime',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_fleetkpi_idlingtime' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_fleetkpi_fuelconsumed','Dashboard.FleetKPI.FuelConsumed',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.FleetKPI.FuelConsumed' and name ='da_dashboard_fleetkpi_fuelconsumed');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_fleetkpi_fuelconsumed',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_fleetkpi_fuelconsumed' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.FuelConsumed'),null,'S','rp_db_dashboard_fleetkpi_fuelconsumed',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.FleetKPI.FuelConsumed'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_fleetkpi_fuelconsumed','Fuel Consumed',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_fleetkpi_fuelconsumed');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_fleetkpi_fuelconsumed',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_fleetkpi_fuelconsumed' 
and ref_id=0);
--------------------------------

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_todaylivevehicle','Dashboard.TodayLiveVehicle',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.TodayLiveVehicle' and name ='da_dashboard_todaylivevehicle');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_todaylivevehicle',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_todaylivevehicle' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle'),array [
(SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.Distance'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.DrivingTime'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.Drivers'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.CriticalAlerts'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.TimeBasedUtilizationRate'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.DistanceBasedUtilizationRate'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.ActiveVehicles')],'C','rp_db_dashboard_todaylivevehicle',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_todaylivevehicle','Today Live Vehicle (Component view settings)',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_todaylivevehicle');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_todaylivevehicle',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_todaylivevehicle' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_todaylivevehicle_distance','Dashboard.TodayLiveVehicle.Distance',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.TodayLiveVehicle.Distance' and name ='da_dashboard_todaylivevehicle_distance');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_todaylivevehicle_distance',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_todaylivevehicle_distance' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.Distance'),null,'S',
'rp_db_dashboard_todaylivevehicle_distance',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.Distance'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_todaylivevehicle_distance','Distance',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_todaylivevehicle_distance');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_todaylivevehicle_distance',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_todaylivevehicle_distance' 
and ref_id=0);

------------------------------------------

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_todaylivevehicle_drivingtime','Dashboard.TodayLiveVehicle.DrivingTime',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.TodayLiveVehicle.DrivingTime' and name ='da_dashboard_todaylivevehicle_drivingtime');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_todaylivevehicle_drivingtime',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_todaylivevehicle_drivingtime' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.DrivingTime'),null,'S',
'rp_db_dashboard_todaylivevehicle_drivingtime',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.DrivingTime'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_todaylivevehicle_drivingtime','Driving Time',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_todaylivevehicle_drivingtime');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_todaylivevehicle_drivingtime',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_todaylivevehicle_drivingtime' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_todaylivevehicle_drivers','Dashboard.TodayLiveVehicle.Drivers',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.TodayLiveVehicle.Drivers' and name ='da_dashboard_todaylivevehicle_drivers');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_todaylivevehicle_drivers',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_todaylivevehicle_drivers' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.Drivers'),null,'S',
'rp_db_dashboard_todaylivevehicle_drivers',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.Drivers'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_todaylivevehicle_drivers','Drivers',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_todaylivevehicle_drivers');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_todaylivevehicle_drivers',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_todaylivevehicle_drivers' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_todaylivevehicle_criticalalerts','Dashboard.TodayLiveVehicle.CriticalAlerts',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.TodayLiveVehicle.CriticalAlerts' and name ='da_dashboard_todaylivevehicle_criticalalerts');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_todaylivevehicle_criticalalerts',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_todaylivevehicle_criticalalerts' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.CriticalAlerts'),null,'S',
'rp_db_dashboard_todaylivevehicle_criticalalerts',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.CriticalAlerts'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_todaylivevehicle_criticalalerts','Distance',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_todaylivevehicle_criticalalerts');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_todaylivevehicle_criticalalerts',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_todaylivevehicle_criticalalerts' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_todaylivevehicle_timebasedutilizationrate','Dashboard.TodayLiveVehicle.TimeBasedUtilizationRate',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.TodayLiveVehicle.TimeBasedUtilizationRate' and name ='da_dashboard_todaylivevehicle_timebasedutilizationrate');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_todaylivevehicle_timebasedutilizationrate',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_todaylivevehicle_timebasedutilizationrate' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.TimeBasedUtilizationRate'),null,'S',
'rp_db_dashboard_todaylivevehicle_timebasedutilizationrate',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.TimeBasedUtilizationRate'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_todaylivevehicle_timebasedutilizationrate','Time Based Utilization Rate',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_todaylivevehicle_timebasedutilizationrate');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_todaylivevehicle_timebasedutilizationrate',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_todaylivevehicle_timebasedutilizationrate' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_todaylivevehicle_distancebasedutilizationrate','Dashboard.TodayLiveVehicle.DistanceBasedUtilizationRate',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.TodayLiveVehicle.DistanceBasedUtilizationRate' and name ='da_dashboard_todaylivevehicle_distancebasedutilizationrate');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_todaylivevehicle_distancebasedutilizationrate',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_todaylivevehicle_distancebasedutilizationrate' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.DistanceBasedUtilizationRate'),null,'S',
'rp_db_dashboard_todaylivevehicle_distancebasedutilizationrate',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.DistanceBasedUtilizationRate'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_todaylivevehicle_distancebasedutilizationrate','Distance Based Utilization Rate',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_todaylivevehicle_distancebasedutilizationrate');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_todaylivevehicle_distancebasedutilizationrate',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_todaylivevehicle_distancebasedutilizationrate' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_todaylivevehicle_activevehicles','Dashboard.TodayLiveVehicle.ActiveVehicles',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.TodayLiveVehicle.ActiveVehicles' and name ='da_dashboard_todaylivevehicle_activevehicles');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_todaylivevehicle_activevehicles',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_todaylivevehicle_activevehicles' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.ActiveVehicles'),null,'S',
'rp_db_dashboard_todaylivevehicle_activevehicles',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.TodayLiveVehicle.ActiveVehicles'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_todaylivevehicle_activevehicles','Active Vehicles',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_todaylivevehicle_activevehicles');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_todaylivevehicle_activevehicles',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_todaylivevehicle_activevehicles' 
and ref_id=0);
---------------------------------
INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_vehicleutilization','Dashboard.VehicleUtilization',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.VehicleUtilization' and name ='da_dashboard_vehicleutilization');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_vehicleutilization',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_vehicleutilization' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.VehicleUtilization'),array [
(SELECT id from master.dataattribute WHERE name = 'Dashboard.VehicleUtilization.DistanceBasedUtilizationRate'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.VehicleUtilization.TimeBasedUtilizationRate'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.VehicleUtilization.DistancePerDay'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.VehicleUtilization.ActiveVehiclesPerDay')],'C','rp_db_dashboard_vehicleutilization',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.VehicleUtilization'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_vehicleutilization','Vehicle Utilization (Component view settings)',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_vehicleutilization');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_vehicleutilization',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_vehicleutilization' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_vehicleutilization_distancebasedutilizationrate','Dashboard.VehicleUtilization.DistanceBasedUtilizationRate',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.VehicleUtilization.DistanceBasedUtilizationRate' and name ='da_dashboard_vehicleutilization_distancebasedutilizationrate');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_vehicleutilization_distancebasedutilizationrate',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_vehicleutilization_distancebasedutilizationrate' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.VehicleUtilization.DistanceBasedUtilizationRate'),null,'S',
'rp_db_dashboard_vehicleutilization_distancebasedutilizationrate',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.VehicleUtilization.DistanceBasedUtilizationRate'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_vehicleutilization_distancebasedutilizationrate','Distance Based Utilization Rate',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_vehicleutilization_distancebasedutilizationrate');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_vehicleutilization_distancebasedutilizationrate',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_vehicleutilization_distancebasedutilizationrate' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_vehicleutilization_timebasedutilizationrate','Dashboard.VehicleUtilization.TimeBasedUtilizationRate',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.VehicleUtilization.TimeBasedUtilizationRate' and name ='da_dashboard_vehicleutilization_timebasedutilizationrate');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_vehicleutilization_timebasedutilizationrate',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_vehicleutilization_timebasedutilizationrate' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.VehicleUtilization.TimeBasedUtilizationRate'),null,'S',
'rp_db_dashboard_vehicleutilization_timebasedutilizationrate',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.VehicleUtilization.TimeBasedUtilizationRate'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_vehicleutilization_timebasedutilizationrate','Time Based Utilization Rate',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_vehicleutilization_timebasedutilizationrate');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_vehicleutilization_timebasedutilizationrate',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_vehicleutilization_timebasedutilizationrate' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_vehicleutilization_distanceperday','Dashboard.VehicleUtilization.DistancePerDay',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.VehicleUtilization.DistancePerDay' and name ='da_dashboard_vehicleutilization_distanceperday');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_vehicleutilization_distanceperday',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_vehicleutilization_distanceperday' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.VehicleUtilization.DistancePerDay'),null,'S',
'rp_db_dashboard_vehicleutilization_distanceperday',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.VehicleUtilization.DistancePerDay'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_vehicleutilization_distanceperday','Distance Per Day',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_vehicleutilization_distanceperday');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_vehicleutilization_distanceperday',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_vehicleutilization_distanceperday' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_vehicleutilization_activevehiclesperday','Dashboard.VehicleUtilization.ActiveVehiclesPerDay',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.VehicleUtilization.ActiveVehiclesPerDay' and name ='da_dashboard_vehicleutilization_activevehiclesperday');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_vehicleutilization_activevehiclesperday',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_vehicleutilization_activevehiclesperday' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.VehicleUtilization.ActiveVehiclesPerDay'),null,'S',
'rp_db_dashboard_vehicleutilization_activevehiclesperday',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.VehicleUtilization.ActiveVehiclesPerDay'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_vehicleutilization_activevehiclesperday','Active Vehicles Per Day',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_vehicleutilization_activevehiclesperday');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_vehicleutilization_activevehiclesperday',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_vehicleutilization_activevehiclesperday' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_alertlast24hours','Dashboard.AlertLast24Hours',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.AlertLast24Hours' and name ='da_dashboard_alertlast24hours');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_alertlast24hours',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_alertlast24hours' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.AlertLast24Hours'),array [
(SELECT id from master.dataattribute WHERE name = 'Dashboard.AlertLast24Hours.LevelAlerts'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.AlertLast24Hours.TotalAlerts'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.AlertLast24Hours.LogisticAlert'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.AlertLast24Hours.Repair&MaintenanceAlerts'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.AlertLast24Hours.Fuel&DriverAlerts')],'C','rp_db_dashboard_alertlast24hours',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.AlertLast24Hours'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_alertlast24hours','Alert Last 24 Hours (Component view settings)',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_alertlast24hours');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_alertlast24hours',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_alertlast24hours' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_alertlast24hours_levelalerts','Dashboard.AlertLast24Hours.LevelAlerts',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.AlertLast24Hours.LevelAlerts' and name ='da_dashboard_alertlast24hours_levelalerts');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_alertlast24hours_levelalerts',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_alertlast24hours_levelalerts' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.AlertLast24Hours.LevelAlerts'),null,'S',
'rp_db_dashboard_alertlast24hours_levelalerts',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.AlertLast24Hours.LevelAlerts'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_alertlast24hours_levelalerts','Level Alerts',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_alertlast24hours_levelalerts');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_alertlast24hours_levelalerts',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_alertlast24hours_levelalerts' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_alertlast24hours_totalalerts','Dashboard.AlertLast24Hours.TotalAlerts',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.AlertLast24Hours.TotalAlerts' and name ='da_dashboard_alertlast24hours_totalalerts');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_alertlast24hours_totalalerts',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_alertlast24hours_totalalerts' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.AlertLast24Hours.TotalAlerts'),null,'S',
'rp_db_dashboard_alertlast24hours_totalalerts',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.AlertLast24Hours.TotalAlerts'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_alertlast24hours_totalalerts','Total Alerts',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_alertlast24hours_totalalerts');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_alertlast24hours_totalalerts',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_alertlast24hours_totalalerts' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_alertlast24hours_logisticalert','Dashboard.AlertLast24Hours.LogisticAlert',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.AlertLast24Hours.LogisticAlert' and name ='da_dashboard_alertlast24hours_logisticalert');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_alertlast24hours_logisticalert',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_alertlast24hours_logisticalert' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.AlertLast24Hours.LogisticAlert'),null,'S',
'rp_db_dashboard_alertlast24hours_logisticalert',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.AlertLast24Hours.LogisticAlert'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_alertlast24hours_logisticalert','Logistic Alert',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_alertlast24hours_logisticalert');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_alertlast24hours_logisticalert',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_alertlast24hours_logisticalert' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_alertlast24hours_fueldriveralerts','Dashboard.AlertLast24Hours.Fuel&DriverAlerts',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.AlertLast24Hours.Fuel&DriverAlerts' and name ='da_dashboard_alertlast24hours_fueldriveralerts');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_alertlast24hours_fueldriveralerts',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_alertlast24hours_fueldriveralerts' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.AlertLast24Hours.Fuel&DriverAlerts'),null,'S',
'rp_db_dashboard_alertlast24hours_fueldriveralerts',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.AlertLast24Hours.Fuel&DriverAlerts'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_alertlast24hours_fueldriveralerts','Fuel & Driver Alerts',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_alertlast24hours_fueldriveralerts');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_alertlast24hours_fueldriveralerts',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_alertlast24hours_fueldriveralerts' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_dashboard_alertlast24hours_repairmaintenancealerts','Dashboard.AlertLast24Hours.Repair&MaintenanceAlerts',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Dashboard.AlertLast24Hours.Repair&MaintenanceAlerts' and name ='da_dashboard_alertlast24hours_repairmaintenancealerts');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'da_dashboard_alertlast24hours_repairmaintenancealerts',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'da_dashboard_alertlast24hours_repairmaintenancealerts' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Dashboard'),
(SELECT id from master.dataattribute WHERE name = 'Dashboard.AlertLast24Hours.Repair&MaintenanceAlerts'),null,'S',
'rp_db_dashboard_alertlast24hours_repairmaintenancealerts',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Dashboard') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Dashboard.AlertLast24Hours.Repair&MaintenanceAlerts'));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_db_dashboard_alertlast24hours_repairmaintenancealerts','Repair & Maintenance Alerts',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_db_dashboard_alertlast24hours_repairmaintenancealerts');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_db_dashboard_alertlast24hours_repairmaintenancealerts',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_db_dashboard_alertlast24hours_repairmaintenancealerts' 
and ref_id=0);


--X-Ray for language translation
INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
                             key, level,state)   
SELECT 520,'Admin#TranslationManagement#Inspect','B',null,'feat_admin_translationmanagement_inspect',20,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='Admin#TranslationManagement#Inspect');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_admin_translationmanagement_inspect','Admin#TranslationManagement#Inspect',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_admin_translationmanagement_inspect');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_admin_translationmanagement_inspect',(select id from master.menu where name = 'Account Role Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_admin_translationmanagement_inspect' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_admin_translationmanagement_inspect',(select id from master.menu where name = 'Package Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_admin_translationmanagement_inspect' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_admin_translationmanagement_inspect',(select id from master.menu where name = 'Organisation Relationship Management'),'M'
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_admin_translationmanagement_inspect' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));

INSERT INTO translation.language (name ,    code ,   key ,   description) 
select 'X-Ray','xr-XR','dlanguage_Xray',null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.language  WHERE code = 'xr-XR');

--api provisioning data
INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
                             key, level,state)   
SELECT 818,'api.provisioning-data','F',null,'feat_api_provisioning_data',30,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='api.provisioning-data');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_api_provisioning_data','api.provisioning-data',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_api_provisioning_data');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_provisioning_data',(select id from master.menu where name = 'Account Role Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_provisioning_data' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_provisioning_data',(select id from master.menu where name = 'Package Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_provisioning_data' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_provisioning_data',(select id from master.menu where name = 'Organisation Relationship Management'),'M'
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_provisioning_data' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));


delete from master.emailtemplatelabels  where email_template_id in (
select id from master.emailtemplate where event_name='AlertNotificationEmail');

delete from master.emailtemplate  where event_name='AlertNotificationEmail';

insert into master.emailtemplate (type,feature_id,event_name,description,created_at,created_by,modified_at,modified_by)
select 'H',null,'AlertNotificationEmail','<!DOCTYPE html>
<html>
<head>
<style>
body div {{
 margin-bottom: 5px;
}}
.mainContainer {{
 margin:20px;
 width:100%;
 font-family: Roboto, "Helvetica Neue", sans-serif;
 font-size: 15px;
}}
.alertLevel {{
  margin: 8px 0;
}}
.alertCriticalLevel {{
background:rgb(255, 0, 0) !important;
color:white;
}}
.alertWarningLevel {{
background:rgb(255, 191, 0) !important;
color: black;
}}
.alertAdvisoryLevel {{
background:rgb(255, 255, 0) !important;
color:black;
}}
</style>
</head>
  <body>

<table style="width: 100%;">
  <tr>
  <td><img style="margin:20px 0px" align="left" width="180px" height="80px"  src="{0}"></td>
</tr>
</table>
    
<div class="mainContainer">
<h2>[lblAlertName]: <span style="color: blue;">{1}</span></h2>
  <h3><span class="{2}" style="padding:5px;">{3}</span></h3>
  <div>[lblDefinedThreshold]:{4} </div>
  <div>[lblActualThresholdValue]:{5}</div>
<div>[lblAlertCategory]: {6}</div>
<div>[lblVehicleGroup]: {7}</div>
<div>[lblDateTime]: {8}</div>

<p>{9}</p>
<br/>

<p>[lblQuestionText]<br/>

<a href="mailto: "{10}">{10}</a></p>
<br/>

    <p>[lblWithKindRegards]</p>
    <p><strong>[lblSignatureLine1]</strong></p>

</div>
  </body>
</html>',(select extract(epoch from now()) * 1000), (select min(id) from master.account where lower(first_name) like '%atos%'),null,null
where not exists (   SELECT 1   FROM master.emailtemplate   WHERE event_name ='AlertNotificationEmail');


insert into master.emailtemplatelabels (email_template_id,key)
select (select id from master.emailtemplate 
where event_name='AlertNotificationEmail'),'lblAlertName'
where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblAlertName' and email_template_id in (select id from master.emailtemplate where event_name='AlertNotificationEmail'));

insert into master.emailtemplatelabels (email_template_id,key)
select (select id from master.emailtemplate where event_name='AlertNotificationEmail'),'lblAlertLevel'
where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblAlertLevel' and email_template_id in (select id from master.emailtemplate where event_name='AlertNotificationEmail'));

insert into master.emailtemplatelabels (email_template_id,key)
select (select id from master.emailtemplate where event_name='AlertNotificationEmail'),'lblDefinedThreshold'
where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblDefinedThreshold' and email_template_id in (select id from master.emailtemplate where event_name='AlertNotificationEmail'));

insert into master.emailtemplatelabels (email_template_id,key)
select (select id from master.emailtemplate where event_name='AlertNotificationEmail'),'lblActualThresholdValue'
where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblActualThresholdValue' and email_template_id in (select id from master.emailtemplate where event_name='AlertNotificationEmail'));

insert into master.emailtemplatelabels (email_template_id,key)
select (select id from master.emailtemplate where event_name='AlertNotificationEmail'),'lblAlertCategory'
where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblAlertCategory' and email_template_id in (select id from master.emailtemplate where event_name='AlertNotificationEmail'));

insert into master.emailtemplatelabels (email_template_id,key)
select (select id from master.emailtemplate where event_name='AlertNotificationEmail'),'lblDateTime'
where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblDateTime' and email_template_id in (select id from master.emailtemplate where event_name='AlertNotificationEmail'));

insert into master.emailtemplatelabels (email_template_id,key)
select (select id from master.emailtemplate where event_name='AlertNotificationEmail'),'lblQuestionText'
where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblQuestionText' and email_template_id in (select id from master.emailtemplate where event_name='AlertNotificationEmail'));

insert into master.emailtemplatelabels (email_template_id,key)
select (select id from master.emailtemplate where event_name='AlertNotificationEmail'),'lblWithKindRegards'
where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblWithKindRegards' and email_template_id in (select id from master.emailtemplate where event_name='AlertNotificationEmail'));

insert into master.emailtemplatelabels (email_template_id,key)
select (select id from master.emailtemplate where event_name='AlertNotificationEmail'),'lblSignatureLine1'
where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblSignatureLine1' and email_template_id in (select id from master.emailtemplate where event_name='AlertNotificationEmail'));

insert into master.emailtemplatelabels (email_template_id,key)
select (select id from master.emailtemplate where event_name='AlertNotificationEmail'),'lblVehicleGroup'
where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblVehicleGroup' and email_template_id in (select id from master.emailtemplate where event_name='AlertNotificationEmail'));

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)   
SELECT 'EN-GB','L','lblAlertName','Alert Name',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblAlertName');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)   
SELECT 'EN-GB','L','lblAlertLevel','Critical',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblAlertLevel');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)   
SELECT 'EN-GB','L','lblDefinedThreshold','Defined Threshold',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblDefinedThreshold');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)   
SELECT 'EN-GB','L','lblActualThresholdValue','Actual Value',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblActualThresholdValue');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)   
SELECT 'EN-GB','L','lblAlertCategory','Category',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblAlertCategory');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)   
SELECT 'EN-GB','L','lblDateTime','Date/time',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblDateTime');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)   
SELECT 'EN-GB','L','lblQuestionText','If you have any query/questions please Email us at',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblQuestionText');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)   
SELECT 'EN-GB','L','lblWithKindRegards','With Kind Regards,',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblWithKindRegards');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)   
SELECT 'EN-GB','L','lblSignatureLine1','DAF Connect Support',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblSignatureLine1');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)   
SELECT 'EN-GB','L','lblVehicleGroup','Vehicle/group',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblVehicleGroup');


insert into master.emailtemplate (type,feature_id,event_name,description,created_at,created_by,modified_at,modified_by)
select 'H',null,'FuelDeviationReport','<!doctype html>
<html>
<head>
<style>
.detailsDiv {{
  border: none;
  background-color: lightblue;    
  text-align: left
}}
.reportDetailsTable {{
  border-collapse: collapse;
  width: 100%;
}}

.reportDetailsTable td, #reportDetailsTable th {{
  border: 1px solid;
  padding: 8px;
}}

.reportDetailsTable tr:nth-child(even){{background-color: #ffffff;}}

.reportDetailsTable th {{
  border: 1px solid;
  padding-top: 12px;
  padding-bottom: 12px;
  text-align: center;
  background-color: lightblue;
  color: black;
}}
thead {{ display: table-header-group }}
tfoot {{ display: table-row-group }}
tr {{ page-break-inside: avoid }}
</style>
</head>
  <body>
  <table style="width: 100%;">
  <tr>
  <td><img style="margin:20px 00px" align="left" width="380px" height="80px"  src="{0}"></td>
		<td><h2 style="text-align: center">[lblFuelDeviationReportDetails]</h2></td>
	<td><img style="margin:0px 0px" align="right" width="180px" height="80px" src="{16}"></td>
	</tr>
	</table>
    
	<div class="detailsDiv">
	  <table  style="width: 100%;">
		<tr>
			<td style="width: 50%;" align="left"><p style="margin-left: 15%;"><strong>[lblFrom] : </strong>  {2}</P></td>
			<td style="width: 50%;" align="left"><p style="margin-left: 15%;"><strong>[lblVehicleGroup] : </strong>  {3}</P></td>
		</tr>
		<tr>
			<td style="width: 50%;" align="left"><p style="margin-left: 15%;"><strong>[lblTo] : </strong>  {4}</P></td>
			<td style="width: 50%;" align="left"><p style="margin-left: 15%;"><strong>[lblVehicleName] : </strong>  {5}</P></td>
			</tr>
	  </table>
	</div><br/><br/>
	<table style="width: 100%;">
	<tr>
	<td>
	<div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
		<div fxLayoutAlign="left">
			<span>[lblFuelIncreaseEvents]</span>
		</div>
		<div fxLayout="column" fxLayoutAlign="left">
			<span style="font: 500 14px/32px Roboto, "Helvetica Neue", sans-serif;">{6}</span>
		</div>
	</div>
	</td>
	<td>
	<div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
		<div class="areaWidth min-width-35-per" fxLayout="column" fxLayoutAlign="left">
			<span>[lblFuelDecreaseEvents]</span>
		</div>
		<div fxLayout="column" fxLayoutAlign="left">
			<span style="font: 500 14px/32px Roboto, "Helvetica Neue", sans-serif;">{7}</span>
		</div>
	</div>
	</td>
	<td>
	<div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
		<div class="areaWidth min-width-35-per" fxLayout="column" fxLayoutAlign="left">
			<span>[lblVehiclesWithFuelEvents]</span>
		</div>
		<div fxLayout="column" fxLayoutAlign="left">
			<span style="font: 500 14px/32px Roboto, "Helvetica Neue", sans-serif;">{8}</span>
		</div>
	</div>
	</td>
	</tr>
	</table><br/><br/>
	
	<table class="reportDetailsTable">
		<thead>
			<th>[lblType]</th>
			<th>[lblDifference]</th>
			<th>[lblVehicleName]</th>
			<th>[lblVIN]</th>
			<th>[lblRegPlateNumber]</th>
			<th>[lblDate]</th>
			<th>[lblOdometer] ({9})</th>
			<th>[lblStartDate]</th>
			<th>[lblEndDate]</th>
			<th>[lblDistance] ({10})</th>
			<th>[lblIdleDuration] ({11})</th>
			<th>[lblAverageSpeed] ({12})</th>
			<th>[lblAverageWeight] ({13})</th>
			<th>[lblStartPosition]</th>
			<th>[lblEndPosition]</th>
			<th>[lblFuelConsumed] ({14})</th>
			<th>[lblDrivingTime] ({15})</th>
			<th>[lblAlerts]</th>
		</thead>
		{1}
	</table>
  </body>
</html>',(select extract(epoch from now()) * 1000), (select min(id) from master.account where lower(first_name) like '%atos%'),null,null
where not exists (   SELECT 1   FROM master.emailtemplate   WHERE event_name ='FuelDeviationReport');

insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblFuelDeviationReportDetails' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblFuelDeviationReportDetails' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblFrom' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblFrom' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblVehicleGroup' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblVehicleGroup' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblTo' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblTo' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblVehicleName' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblVehicleName' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblFuelIncreaseEvents' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblFuelIncreaseEvents' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblFuelDecreaseEvents' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblFuelDecreaseEvents' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblVehiclesWithFuelEvents' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblVehiclesWithFuelEvents' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblType' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblType' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblDifference' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblDifference' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblVIN' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblVIN' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblRegPlateNumber' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblRegPlateNumber' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblDate' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblDate' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblOdometer' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblOdometer' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblEndDate' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblEndDate' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblDistance' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblDistance' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblIdleDuration' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblIdleDuration' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblAverageSpeed' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblAverageSpeed' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblAverageWeight' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblAverageWeight' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblStartPosition' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblStartPosition' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblEndPosition' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblEndPosition' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblFuelConsumed' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblFuelConsumed' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblDrivingTime' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblDrivingTime' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FuelDeviationReport'),'lblAlerts' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblAlerts' and email_template_id in (select id from master.emailtemplate where event_name='FuelDeviationReport'));

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblFuelDeviationReportDetails','Fuel Deviation Report Details',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblFuelDeviationReportDetails');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblFrom','From',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblFrom');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblVehicleGroup','Vehicle Group',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblVehicleGroup');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblTo','To',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblTo');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblVehicleName','Vehicle Name',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblVehicleName');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblFuelIncreaseEvents','Fuel Increase Events',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblFuelIncreaseEvents');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblFuelDecreaseEvents','Fuel Decrease Events',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblFuelDecreaseEvents');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblVehiclesWithFuelEvents','Vehicles With Fuel Events',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblVehiclesWithFuelEvents');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblType','Type',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblType');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblDifference','Difference',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblDifference');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblVIN','VIN',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblVIN');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblRegPlateNumber','Reg Plate Number',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblRegPlateNumber');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblDate','Date',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblDate');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblOdometer','Odometer Start Date',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblOdometer');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblEndDate','End Date',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblEndDate');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblDistance','Distance',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblDistance');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblIdleDuration','Idle Duration',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblIdleDuration');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblAverageSpeed','Average Speed',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblAverageSpeed');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblAverageWeight','Average Weight',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblAverageWeight');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblStartPosition','Start Position',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblStartPosition');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblEndPosition','End Position',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblEndPosition');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblFuelConsumed','Fuel Consumed',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblFuelConsumed');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblDrivingTime','Driving Time',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblDrivingTime');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblAlerts','Alerts',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblAlerts');

insert into master.emailtemplate (type,feature_id,event_name,description,created_at,created_by,modified_at,modified_by)
select 'H',null,'FleetUtilisationReport','<!doctype html>
<html>
<head>
    <style>
.detailsDiv {{
  border: none;
  background-color: lightblue;
  text-align: left
}}
.reportDetailsTable {{
  border-collapse: collapse;
  width: 100%;
}}

.reportDetailsTable td, #reportDetailsTable th {{
  border: 1px solid;
  padding: 8px;
}}

.reportDetailsTable tr:nth-child(even){{background-color: #ffffff;}}

.reportDetailsTable th {{
  border: 1px solid;
  padding-top: 12px;
  padding-bottom: 12px;
  text-align: center;
  background-color: lightblue;
  color: black;
}}
thead {{ display: table-header-group }}
tfoot {{ display: table-row-group }}
tr {{ page-break-inside: avoid }}
    </style>
</head>
<body>
    <table style="width: 100%;">
        <tr>
            <td><img style="margin:20px 00px" align="left" width="380px" height="80px" src="{0}"></td>
            <td><h2 style="text-align: center">[lblFleetUtilisationReportDetails]</h2></td>
            <td><img style="margin:0px 0px" align="right" width="180px" height="80px" src="{23}"></td>
        </tr>
    </table>

    <div class="detailsDiv">
        <table style="width: 100%;">
            <tr>
                <td style="width: 50%;" align="left"><p style="margin-left: 15%;"><strong>[lblFrom] : </strong>  {2}</p></td>
                <td style="width: 50%;" align="left"><p style="margin-left: 15%;"><strong>[lblVehicleGroup] : </strong>  {3}</p></td>
            </tr>
            <tr>
                <td style="width: 50%;" align="left"><p style="margin-left: 15%;"><strong>[lblTo] : </strong>  {4}</p></td>
                <td style="width: 50%;" align="left"><p style="margin-left: 15%;"><strong>[lblVehicleName] : </strong>  {5}</p></td>
            </tr>
        </table>
    </div><br /><br />
    <table style="width: 100%;">
        <tr>
            <td>
                <div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
                    <div fxLayoutAlign="left">
                        <span>[lblNumberOfVehicles]</span>
                    </div>
                    <div fxLayout="column" fxLayoutAlign="left">
                        <span style="font: 500 14px/32px Roboto, "Helvetica Neue", sans-serif;">{6}</span>
                    </div>
                </div>
            </td>
            <td>
                <div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
                    <div class="areaWidth min-width-35-per" fxLayout="column" fxLayoutAlign="left">
                        <span>[lblTotalDistance]</span>
                    </div>
                    <div fxLayout="column" fxLayoutAlign="left">
                        <span style="font: 500 14px/32px Roboto, "Helvetica Neue", sans-serif;">{7}{8}</span>
                    </div>
                </div>
            </td>
            <td>
                <div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
                    <div class="areaWidth min-width-35-per" fxLayout="column" fxLayoutAlign="left">
                        <span>[lblNumberOfTrips]</span>
                    </div>
                    <div fxLayout="column" fxLayoutAlign="left">
                        <span style="font: 500 14px/32px Roboto, "Helvetica Neue", sans-serif;">{9}</span>
                    </div>
                </div>
            </td>
            <td>
                <div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
                    <div fxLayout="column" fxLayoutAlign="right">
                        <span>[lblAverageDistancePerDay]</span>
                    </div>
                    <div fxLayout="column" fxLayoutAlign="right">
                        <span style="font: 500 14px/32px Roboto, "Helvetica Neue", sans-serif;">{10} {11}</span>
                    </div>
                </div>
            </td>
            <td>
                <div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
                    <div fxLayout="column" fxLayoutAlign="right">
                        <span>[lblIdleDuration]</span>
                    </div>
                    <div fxLayout="column" fxLayoutAlign="right">
                        <span style="font: 500 14px/32px Roboto, "Helvetica Neue", sans-serif;">{12} {13}</span>
                    </div>
                </div>
            </td>
        </tr>
    </table><br /><br />
    
                      <table class="reportDetailsTable">
                          <thead>
                          <th>[lblVehicleName]</th>
                          <th>[lblVIN]</th>
                          <th>[lblRegistrationNumber]</th>
                          <th>[lblDistance]({14})</th>
                          <th>[lblNumberOfTrips]</th>
                          <th>[lblTripTime]({15})</th>
                          <th>[lblDrivingTime] ({16})</th>
                          <th>[lblIdleDuration] ({17})</th>
                          <th>[lblStopTime] ({18})</th>
                          <th>[lblAverageSpeed] ({19})</th>
                          <th>[lblAverageWeight] ({20})</th>
                          <th>[lblAverageDistance] ({21})</th>
                          <th>[lblOdometer] ({22})</th>
                          </thead>
                          {1}
                      </table>
                  
</body>
</html>',(select extract(epoch from now()) * 1000), (select min(id) from master.account where lower(first_name) like '%atos%'),null,null
where not exists (   SELECT 1   FROM master.emailtemplate   WHERE event_name ='FleetUtilisationReport');

insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblFleetUtilisationReportDetails' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblFleetUtilisationReportDetails' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblFrom' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblFrom' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblVehicleGroup' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblVehicleGroup' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblTo' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblTo' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblVehicleName' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblVehicleName' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblNumberOfVehicles' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblNumberOfVehicles' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblTotalDistance' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblTotalDistance' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblNumberOfTrips' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblNumberOfTrips' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblAverageDistancePerDay' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblAverageDistancePerDay' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblIdleDuration' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblIdleDuration' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblVIN' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblVIN' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblRegistrationNumber' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblRegistrationNumber' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblDistance' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblDistance' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblNumberOfTrips' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblNumberOfTrips' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblTripTime' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblTripTime' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblDrivingTime' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblDrivingTime' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblStopTime' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblStopTime' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblAverageSpeed' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblAverageSpeed' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblAverageWeight' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblAverageWeight' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblAverageDistance' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblAverageDistance' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetUtilisationReport'),'lblOdometer' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblOdometer' and email_template_id in (select id from master.emailtemplate where event_name='FleetUtilisationReport'));

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblFleetUtilisationReportDetails','Fleet Utilisation Report Details',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblFleetUtilisationReportDetails');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblFrom','From',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblFrom');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblVehicleGroup','Vehicle Group',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblVehicleGroup');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblTo','To',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblTo');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblVehicleName','Vehicle Name',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblVehicleName');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblNumberOfVehicles','Number Of Vehicles',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblNumberOfVehicles');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblTotalDistance','Total Distance',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblTotalDistance');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblNumberOfTrips','Number Of Trips',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblNumberOfTrips');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblAverageDistancePerDay','Average Distance Per Day',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblAverageDistancePerDay');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblIdleDuration','Idle Duration',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblIdleDuration');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblVIN','VIN',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblVIN');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblRegistrationNumber','Registration Number',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblRegistrationNumber');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblDistance','Distance',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblDistance');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblNumberOfTrips','Number Of Trips',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblNumberOfTrips');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblTripTime','Trip Time',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblTripTime');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblDrivingTime','Driving Time',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblDrivingTime');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblStopTime','Stop Time',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblStopTime');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblAverageSpeed','Average Speed',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblAverageSpeed');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblAverageWeight','Average Weight',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblAverageWeight');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblAverageDistance','Average Distance',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblAverageDistance');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblOdometer','Odometer',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblOdometer');

insert into master.emailtemplate (type,feature_id,event_name,description,created_at,created_by,modified_at,modified_by)
select 'H',null,'EcoScoreReport','<!doctype html>
<html>
<head>
<style>
.detailsDiv {{
  border: none;
  background-color: lightblue;    
  text-align: left
}}
.reportDetailsTable {{
  border-collapse: collapse;
  width: 100%;
}}

.reportDetailsTable td, #reportDetailsTable th {{
  border: 1px solid;
  padding: 8px;
}}

.reportDetailsTable tr:nth-child(even){{background-color: #ffffff;}}

.reportDetailsTable th {{
  border: 1px solid;
  padding-top: 12px;
  padding-bottom: 12px;
  text-align: center;
  background-color: lightblue;
  color: black;
}}
thead {{ display: table-header-group }}
tfoot {{ display: table-row-group }}
tr {{ page-break-inside: avoid }}
</style>
</head>
  <body>
  <table style="width: 100%;">
  <tr>
  <td><img style="margin:20px 00px" align="left" width="380px" height="80px"  src="{0}"></td>
		<td><h2 style="text-align: center">[lblEcoScoreReportDetails]</h2></td>
	<td><img style="margin:0px 0px" align="right" width="180px" height="80px" src="{9}"></td>
	</tr>
	</table>
    
	<div class="detailsDiv">
	  <table  style="width: 100%;">
		<tr>
			<td style="width: 14%;"  height="30" align="left"><strong>[lblFrom] : </strong></td>
			<td style="width: 14%;" align="left"><strong>[lblTo] : </strong></td>
			<td style="width: 14%;" height="30" align="left"><strong>[lblVehicle] : </strong></td>
			<td style="width: 14%;" height="30" align="left"><strong>[lblVehicleGroup] : </strong></td>
			<td style="width: 14%;" align="left"><strong>[lblDriverId] : </strong></td>
			<td style="width: 14%;" align="left"><strong>[lblDriverName] : </strong></td>
			<td style="width: 14%;" align="left"><strong>[lblDriverOption] : </strong></td>
		</tr>
		<tr>
			<td  style="width: 14%;" align="left">{2}</td>
			<td  style="width: 14%;" align="left">{3}</td>
			<td  style="width: 14%;" align="left">{4}</td>
			<td  style="width: 14%;" align="left">{5}</td>
			<td  style="width: 14%;" align="left">{6}</td>
			<td  style="width: 14%;" align="left">{7}</td>
			<td  style="width: 14%;" align="left">{8}</td>
		</tr>
	  </table>
	</div><br/><br/>
	
	<table class="reportDetailsTable">
		<thead>
			<th>[lblRanking] </th>
			<th>[lblDriverName]</th>
			<th>[lblDriverId]</th>
			<th>[lblEcoScore]</th>
		</thead>
		{1}
	</table>
  </body>
</html>',(select extract(epoch from now()) * 1000), (select min(id) from master.account where lower(first_name) like '%atos%'),null,null
where not exists (   SELECT 1   FROM master.emailtemplate   WHERE event_name ='EcoScoreReport');

insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='EcoScoreReport'),'lblEcoScoreReportDetails' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblEcoScoreReportDetails' and email_template_id in (select id from master.emailtemplate where event_name='EcoScoreReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='EcoScoreReport'),'lblFrom' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblFrom' and email_template_id in (select id from master.emailtemplate where event_name='EcoScoreReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='EcoScoreReport'),'lblVehicleGroup' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblVehicleGroup' and email_template_id in (select id from master.emailtemplate where event_name='EcoScoreReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='EcoScoreReport'),'lblVehicleVIN' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblVehicleVIN' and email_template_id in (select id from master.emailtemplate where event_name='EcoScoreReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='EcoScoreReport'),'lblTo' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblTo' and email_template_id in (select id from master.emailtemplate where event_name='EcoScoreReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='EcoScoreReport'),'lblDriverName' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblDriverName' and email_template_id in (select id from master.emailtemplate where event_name='EcoScoreReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='EcoScoreReport'),'lblDriverOption' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblDriverOption' and email_template_id in (select id from master.emailtemplate where event_name='EcoScoreReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='EcoScoreReport'),'lblRanking' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblRanking' and email_template_id in (select id from master.emailtemplate where event_name='EcoScoreReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='EcoScoreReport'),'lblDriverId' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblDriverId' and email_template_id in (select id from master.emailtemplate where event_name='EcoScoreReport'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='EcoScoreReport'),'lblEcoScore' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblEcoScore' and email_template_id in (select id from master.emailtemplate where event_name='EcoScoreReport'));

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblEcoScoreReportDetails','Eco Score Report Details',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblEcoScoreReportDetails');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblFrom','From',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblFrom');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblVehicleGroup','Vehicle Group',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblVehicleGroup');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblVehicleVIN','Vehicle VIN',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblVehicleVIN');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblTo','To',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblTo');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblDriverName','Driver Name',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblDriverName');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblDriverOption','Driver Option',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblDriverOption');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblRanking','Ranking',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblRanking');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblDriverId','Driver Id',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblDriverId');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblEcoScore','Eco Score',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblEcoScore');

insert into master.emailtemplate (type,feature_id,event_name,description,created_at,created_by,modified_at,modified_by)
select 'H',null,'FleetFuelReportSingleVehicle','<!doctype html>
<html>
<head>
    <style>
.detailsDiv {{
  border: none;
  background-color: lightblue;
  text-align: left
}}
.reportDetailsTable {{
  border-collapse: collapse;
  width: 100%;
}}

.reportDetailsTable td, #reportDetailsTable th {{
  border: 1px solid;
  padding: 8px;
}}

.reportDetailsTable tr:nth-child(even){{background-color: #ffffff;}}

.reportDetailsTable th {{
  border: 1px solid;
  padding-top: 12px;
  padding-bottom: 12px;
  text-align: center;
  background-color: lightblue;
  color: black;
}}
thead {{ display: table-header-group }}
tfoot {{ display: table-row-group }}
tr {{ page-break-inside: avoid }}
    </style>
</head>
<body>
    <table style="width: 100%;">
        <tr>
            <td><img style="margin:20px 00px" align="left" width="380px" height="80px" src="{0}"></td>
            <td><h2 style="text-align: center">[lblFleetFuelReportDetails]</h2></td>
            <td><img style="margin:0px 0px" align="right" width="180px" height="80px" src="{29}"></td>
        </tr>
    </table>

    <div class="detailsDiv">
        <table style="width: 100%;">
            <tr>
                <td style="width: 25%;" align="left"><p style="margin-left: 10%;"><strong>[lblFrom] : </strong>  {2}</p></td>
                <td style="width: 25%;" align="left"><p style="margin-left: 10%;"><strong>[lblTo] : </strong>  {3}</p></td>
                <td style="width: 25%;" align="left"><p style="margin-left: 10%;"><strong>[lblVehicleGroup] : </strong>  {4}</p></td>
                <td style="width: 25%;" align="left"><p style="margin-left: 10%;"><strong>[lblVehicleName] : </strong>  {5}</p></td>
            </tr>
        </table>
    </div><br /><br />
    <table style="width: 100%;">
        <tr>
            <td>
                <div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
                    <div class="areaWidth min-width-35-per" fxLayout="column" fxLayoutAlign="center">
                        <span>[lblNumberOfTrips]</span>
                    </div>
                    <div fxLayout="column" fxLayoutAlign="center">
                        <span style="font: 500 14px/32px Roboto, " Helvetica Neue", sans-serif;">{6}</span>
                    </div>
                </div>
            </td>
            <td>
                <div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
                    <div class="areaWidth min-width-35-per" fxLayout="column" fxLayoutAlign="center">
                        <span>[lblTotalDistance]</span>
                    </div>
                    <div fxLayout="column" fxLayoutAlign="center">
                        <span style="font: 500 14px/32px Roboto, " Helvetica Neue", sans-serif;">{7}{8}</span>
                    </div>
                </div>
            </td>
            <td>
                <div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
                    <div fxLayoutAlign="center">
                        <span>[lblFuelConsumed]</span>
                    </div>
                    <div fxLayout="column" fxLayoutAlign="center">
                        <span style="font: 500 14px/32px Roboto, " Helvetica Neue", sans-serif;">{9} {10}</span>
                    </div>
                </div>
            </td>
            <td>
                <div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
                    <div fxLayout="column" fxLayoutAlign="center">
                        <span>[lblIdleDuration]</span>
                    </div>
                    <div fxLayout="column" fxLayoutAlign="center">
                        <span style="font: 500 14px/32px Roboto, " Helvetica Neue", sans-serif;">{11} {12}</span>
                    </div>
                </div>
            </td>
            <td>
                <div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
                    <div fxLayout="column" fxLayoutAlign="center">
                        <span>[lblFuelConsumption]</span>
                    </div>
                    <div fxLayout="column" fxLayoutAlign="center">
                        <span style="font: 500 14px/32px Roboto, " Helvetica Neue", sans-serif;">{13} {14}</span>
                    </div>
                </div>
            </td>
            <td>
                <div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
                    <div fxLayout="column" fxLayoutAlign="center">
                        <span>[lblCO2Emission]</span>
                    </div>
                    <div fxLayout="column" fxLayoutAlign="center">
                        <span style="font: 500 14px/32px Roboto, " Helvetica Neue", sans-serif;">{15} {16}</span>
                    </div>
                </div>
            </td>
        </tr>
    </table><br /><br />
    <strong>All Trip Details</strong>
    <br />
    <div class="detailsDiv">
        <table style="width: 100%;">
            <tr>
                <td style="width: 30%;"><p><strong>Vehicle Name : </strong>  {17}</p></td>
                <td style="width: 30%;"><p><strong>VIN : </strong>  {18}</p></td>
                <td style="width: 30%;"><p><strong>Registration No. : </strong>  {19}</p></td>
            </tr>
        </table>
    </div><br />
          <table class="reportDetailsTable">
              <thead>
              <th>[lblVehicleName]</th>
              <th>[lblVIN]</th>
              <th>[lblRegistrationNumber]</th>
              <th>[lblstartDate]</th>
              <th>[lblendDate]</th>
              <th>[lblDistance] ({20})</th>
              <th>[lblStartPosition]</th>
              <th>[lblEndPosition]</th>
              <th>[lblFuelConsumed] ({21})</th>
              <th>[lblFuelConsumption] ({22})</th>
              <th>[lblIdleDuration] (%)</th>
              <th>[lblCruiseControlDistance30-50] (%)</th>
              <th>[lblCruiseControlDistance50-75] (%)</th>
              <th>[lblCruiseControlDistance>75] (%)</th>
              <th>[lblCO2Emission] ({23})</th>
              <th>[lblHeavyThrottleDuration] (%)</th>
              <th>[lblHarshBrakeDuration] (%)</th>
              <th>[lblAverageTrafficClassification]</th>
              <th>[lblCCFuelConsumption] ({24})</th>
              <th>[lblFuelconsumptionCCnonactive] ({25})</th>
              <th>[lblIdlingConsumption]</th>
              <th>[lblDPAScore]</th>
              <th>[lblGrossWeightComb] ({26})</th>
              <th>[lblPTODuration] (%)</th>
              <th>[lblMaxSpeed] ({27})</th>
              <th>[lblAverageSpeed]({28})</th>
              </thead>
              {1}
          </table>
</body>
</html>',(select extract(epoch from now()) * 1000), (select min(id) from master.account where lower(first_name) like '%atos%'),null,null
where not exists (   SELECT 1   FROM master.emailtemplate   WHERE event_name ='FleetFuelReportSingleVehicle');

insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblFleetFuelReportDetails' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblFleetFuelReportDetails' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblFrom' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblFrom' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblVehicleGroup' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblVehicleGroup' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblVehicleName' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblVehicleName' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblTo' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblTo' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblNumberOfTrips' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblNumberOfTrips' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblTotalDistance' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblTotalDistance' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblFuelConsumed' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblFuelConsumed' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblIdleDuration' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblIdleDuration' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblFuelConsumption' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblFuelConsumption' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblCO2Emission' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblCO2Emission' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblVIN' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblVIN' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblRegistrationNumber' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblRegistrationNumber' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblstartDate' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblstartDate' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblendDate' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblendDate' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblDistance' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblDistance' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblStartPosition' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblStartPosition' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblEndPosition' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblEndPosition' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblCruiseControlDistance30-50' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblCruiseControlDistance30-50' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblCruiseControlDistance50-75' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblCruiseControlDistance50-75' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblCruiseControlDistance>75' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblCruiseControlDistance>75' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblHeavyThrottleDuration' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblHeavyThrottleDuration' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblHarshBrakeDuration' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblHarshBrakeDuration' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblAverageTrafficClassification' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblAverageTrafficClassification' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblCCFuelConsumption' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblCCFuelConsumption' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblFuelconsumptionCCnonactive' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblFuelconsumptionCCnonactive' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblIdlingConsumption' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblIdlingConsumption' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblDPAScore' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblDPAScore' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblGrossWeightComb' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblGrossWeightComb' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblPTODuration' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblPTODuration' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblAverageSpeed' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblAverageSpeed' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'),'lblMaxSpeed' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblMaxSpeed' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportSingleVehicle'));

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblFleetFuelReportDetails','Fleet Fuel Report Details',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblFleetFuelReportDetails');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblFrom','From',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblFrom');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblVehicleGroup','Vehicle Group',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblVehicleGroup');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblVehicleName','Vehicle Name',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblVehicleName');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblTo','To',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblTo');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblNumberOfTrips','Number Of Trips',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblNumberOfTrips');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblTotalDistance','Total Distance',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblTotalDistance');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblFuelConsumed','Fuel Consumed',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblFuelConsumed');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblIdleDuration','Idle Duration',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblIdleDuration');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblFuelConsumption','Fuel Consumption',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblFuelConsumption');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblCO2Emission','CO2 Emission',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblCO2Emission');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblVIN','VIN',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblVIN');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblRegistrationNumber','Registration Number',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblRegistrationNumber');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblstartDate','Start Date',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblstartDate');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblendDate','End Date',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblendDate');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblDistance','Distance',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblDistance');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblStartPosition','Start Position',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblStartPosition');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblEndPosition','End Position',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblEndPosition');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblCruiseControlDistance30-50','Cruise Control Distance 30-50',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblCruiseControlDistance30-50');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblCruiseControlDistance50-75','Cruise Control Distance 50-75',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblCruiseControlDistance50-75');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblCruiseControlDistance>75','Cruise Control Distance > 75',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblCruiseControlDistance>75');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblHeavyThrottleDuration','Heavy Throttle Duration',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblHeavyThrottleDuration');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblHarshBrakeDuration','Harsh Brake Duration',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblHarshBrakeDuration');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblAverageTrafficClassification','Average Traffic Classification',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblAverageTrafficClassification');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblCCFuelConsumption','CC Fuel Consumption',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblCCFuelConsumption');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblFuelconsumptionCCnonactive','Fuel consumption CC non active',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblFuelconsumptionCCnonactive');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblIdlingConsumption','Idling Consumption',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblIdlingConsumption');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblDPAScore','DPA Score',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblDPAScore');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblGrossWeightComb','Gross Weight Comb',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblGrossWeightComb');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblPTODuration','PTO Duration',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblPTODuration');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblAverageSpeed','Average Speed',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblAverageSpeed');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblMaxSpeed','Max Speed',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblMaxSpeed');

insert into master.emailtemplate (type,feature_id,event_name,description,created_at,created_by,modified_at,modified_by)
select 'H',null,'FleetFuelReportAllVehicles','<!doctype html>
<html>
<head>
    <style>
.detailsDiv {{
  border: none;
  background-color: lightblue;
  text-align: left
}}
.reportDetailsTable {{
  border-collapse: collapse;
  width: 100%;
}}

.reportDetailsTable td, #reportDetailsTable th {{
  border: 1px solid;
  padding: 8px;
}}

.reportDetailsTable tr:nth-child(even){{background-color: #ffffff;}}

.reportDetailsTable th {{
  border: 1px solid;
  padding-top: 12px;
  padding-bottom: 12px;
  text-align: center;
  background-color: lightblue;
  color: black;
}}
thead {{ display: table-header-group }}
tfoot {{ display: table-row-group }}
tr {{ page-break-inside: avoid }}
    </style>
</head>
<body>
    <table style="width: 100%;">
        <tr>
            <td><img style="margin:20px 00px" align="left" width="380px" height="80px" src="{0}"></td>
            <td><h2 style="text-align: center">[lblFleetFuelReportDetails]</h2></td>
            <td><img style="margin:0px 0px" align="right" width="180px" height="80px" src="{29}"></td>
        </tr>
    </table>
	
	<table class="reportDetailsTable">
        <thead>
        <th>[lblRanking]</th>
        <th>[lblVehicleName]</th>
		<th>[lblVIN]</th>
        <th>[lblRegistrationNumber]</th>
        <th>[lblConsumption] ({27})</th>
        </thead>
        {28}
    </table>
<br/><br/>
    <div class="detailsDiv">
        <table style="width: 100%;">
            <tr>
                <td style="width: 25%;" align="left"><p style="margin-left: 10%;"><strong>[lblFrom] : </strong>  {2}</p></td>
                <td style="width: 25%;" align="left"><p style="margin-left: 10%;"><strong>[lblTo] : </strong>  {3}</p></td>
				<td style="width: 25%;" align="left"><p style="margin-left: 10%;"><strong>[lblVehicleGroup] : </strong>  {4}</p></td>
                <td style="width: 25%;" align="left"><p style="margin-left: 10%;"><strong>[lblVehicleName] : </strong>  {5}</p></td>
            </tr>
        </table>
    </div><br /><br />
    <table style="width: 100%;">
        <tr>       
            <td>
                <div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
                    <div class="areaWidth min-width-35-per" fxLayout="column" fxLayoutAlign="center">
                        <span>[lblNumberOfTrips]</span>
                    </div>
                    <div fxLayout="column" fxLayoutAlign="center">
                        <span style="font: 500 14px/32px Roboto, " Helvetica Neue", sans-serif;">{6}</span>
                    </div>
                </div>
            </td>
			<td>
                <div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
                    <div class="areaWidth min-width-35-per" fxLayout="column" fxLayoutAlign="center">
                        <span>[lblTotalDistance]</span>
                    </div>
                    <div fxLayout="column" fxLayoutAlign="center">
                        <span style="font: 500 14px/32px Roboto, " Helvetica Neue", sans-serif;">{7}{8}</span>
                    </div>
                </div>
            </td>
			<td>
                <div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
                    <div fxLayoutAlign="center">
                        <span>[lblFuelConsumed]</span>
                    </div>
                    <div fxLayout="column" fxLayoutAlign="center">
                        <span style="font: 500 14px/32px Roboto, " Helvetica Neue", sans-serif;">{9} {10}</span>
                    </div>
                </div>
            </td>
			<td>
                <div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
                    <div fxLayout="column" fxLayoutAlign="center">
                        <span>[lblIdleDuration]</span>
                    </div>
                    <div fxLayout="column" fxLayoutAlign="center">
                        <span style="font: 500 14px/32px Roboto, " Helvetica Neue", sans-serif;">{11} {12}</span>
                    </div>
                </div>
            </td>
            <td>
                <div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
                    <div fxLayout="column" fxLayoutAlign="center">
                        <span>[lblFuelConsumption]</span>
                    </div>
                    <div fxLayout="column" fxLayoutAlign="center">
                        <span style="font: 500 14px/32px Roboto, " Helvetica Neue", sans-serif;">{13} {14}</span>
                    </div>
                </div>
            </td>
            <td>
                <div style="padding: 20px; margin-bottom: 10px; background: #e7e7e7;">
                    <div fxLayout="column" fxLayoutAlign="center">
                        <span>[lblCO2Emission]</span>
                    </div>
                    <div fxLayout="column" fxLayoutAlign="center">
                        <span style="font: 500 14px/32px Roboto, " Helvetica Neue", sans-serif;">{15} {16}</span>
                    </div>
                </div>
            </td>
        </tr>
    </table><br /><br />

                  <table class="reportDetailsTable">
                      <thead>
                      <th>[lblVehicleName]</th>
                      <th>[lblVIN]</th>
                      <th>[lblRegistrationNumber]</th>
                      <th>[lblDistance]({17})</th>
                      <th>[lblAverageDistancePerDay] ({18})</th>
                      <th>[lblAverageSpeed]({19})</th>
                      <th>[lblMaxSpeed] ({20})</th>
                      <th>[lblNoOfTrips]</th>
                      <th>[lblAverageGrossWeightComb] ({21})</th>
                      <th>[lblFuelConsumed] ({22})</th>
                      <th>[lblFuelConsumption] ({23})</th>
                      <th>[lblCO2Emission] ({24})</th>
                      <th>[lblIdleDuration] (%)</th>
                      <th>[lblPTODuration] (%)</th>
                      <th>[lblHarshBrakeDuration] (%)</th>
                      <th>[lblHeavyThrottleDuration] (%)</th>
                      <th>[lblCruiseControlDistance30-50] (%)</th>
                      <th>[lblCruiseControlDistance50-75] (%)</th>
                      <th>[lblCruiseControlDistance>75] (%)</th>
                      <th>[lblAverageTrafficClassification]</th>
                      <th>[lblCCFuelConsumption] ({25})</th>
                      <th>[lblFuelconsumptionCCnonactive] ({26})</th>
                      <th>[lblIdlingConsumption]</th>
                      <th>[lblDPAScore]</th>
                      </thead>
                      {1}
                  </table>

</body>
</html>',(select extract(epoch from now()) * 1000), (select min(id) from master.account where lower(first_name) like '%atos%'),null,null
where not exists (   SELECT 1   FROM master.emailtemplate   WHERE event_name ='FleetFuelReportAllVehicles');

insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblFleetFuelReportDetails' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblFleetFuelReportDetails' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblFrom' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblFrom' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblVehicleGroup' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblVehicleGroup' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblVehicleName' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblVehicleName' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblTo' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblTo' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblNumberOfTrips' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblNumberOfTrips' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblTotalDistance' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblTotalDistance' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblFuelConsumed' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblFuelConsumed' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblIdleDuration' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblIdleDuration' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblFuelConsumption' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblFuelConsumption' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblCO2Emission' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblCO2Emission' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblVIN' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblVIN' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblRegistrationNumber' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblRegistrationNumber' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblDistance' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblDistance' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblAverageDistancePerDay' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblAverageDistancePerDay' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblAverageSpeed' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblAverageSpeed' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblMaxSpeed' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblMaxSpeed' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblNoOfTrips' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblNoOfTrips' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblAverageGrossWeightComb' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblAverageGrossWeightComb' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblPTODuration' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblPTODuration' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblHarshBrakeDuration' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblHarshBrakeDuration' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblHeavyThrottleDuration' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblHeavyThrottleDuration' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblCruiseControlDistance30-50' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblCruiseControlDistance30-50' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblCruiseControlDistance50-75' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblCruiseControlDistance50-75' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblCruiseControlDistance>75' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblCruiseControlDistance>75' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblAverageTrafficClassification' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblAverageTrafficClassification' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblCCFuelConsumption' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblCCFuelConsumption' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblFuelconsumptionCCnonactive' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblFuelconsumptionCCnonactive' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblIdlingConsumption' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblIdlingConsumption' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblDPAScore' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblDPAScore' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblRanking' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblRanking' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));
insert into master.emailtemplatelabels (email_template_id,key) select (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'),'lblConsumption' where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblConsumption' and email_template_id in (select id from master.emailtemplate where event_name='FleetFuelReportAllVehicles'));


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblFleetFuelReportDetails','Fleet Fuel Report Details',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblFleetFuelReportDetails');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblFrom','From',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblFrom');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblVehicleGroup','Vehicle Group',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblVehicleGroup');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblVehicleName','Vehicle Name',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblVehicleName');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblTo','To',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblTo');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblNumberOfTrips','Number Of Trips',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblNumberOfTrips');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblTotalDistance','Total Distance',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblTotalDistance');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblFuelConsumed','Fuel Consumed',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblFuelConsumed');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblIdleDuration','Idle Duration',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblIdleDuration');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblFuelConsumption','Fuel Consumption',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblFuelConsumption');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblCO2Emission','CO2 Emission',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblCO2Emission');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblVIN','VIN',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblVIN');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblRegistrationNumber','Registration Number',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblRegistrationNumber');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblDistance','Distance',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblDistance');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblAverageDistancePerDay','Average Distance Per Day',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblAverageDistancePerDay');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblAverageSpeed','Average Speed',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblAverageSpeed');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblMaxSpeed','Max Speed',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblMaxSpeed');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblNoOfTrips','No Of Trips',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblNoOfTrips');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblAverageGrossWeightComb','Average Gross Weight Comb',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblAverageGrossWeightComb');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblPTODuration','PTO Duration',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblPTODuration');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblHarshBrakeDuration','Harsh Brake Duration',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblHarshBrakeDuration');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblHeavyThrottleDuration','Heavy Throttle Duration',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblHeavyThrottleDuration');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblCruiseControlDistance30-50','Cruise Control Distance 30-50',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblCruiseControlDistance30-50');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblCruiseControlDistance50-75','Cruise Control Distance 50-75',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblCruiseControlDistance50-75');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblCruiseControlDistance>75','Cruise Control Distance > 75',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblCruiseControlDistance>75');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblAverageTrafficClassification','Average Traffic Classification',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblAverageTrafficClassification');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblCCFuelConsumption','CC Fuel Consumption',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblCCFuelConsumption');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblFuelconsumptionCCnonactive','Fuel consumption CC non active',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblFuelconsumptionCCnonactive');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblIdlingConsumption','Idling Consumption',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblIdlingConsumption');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblDPAScore','DPA Score',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblDPAScore');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblRanking','Ranking',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblRanking');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)    SELECT 'EN-GB','L','lblConsumption','Consumption',(select extract(epoch from now()) * 1000),null  WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'lblConsumption');




insert into master.vehicleperformancetemplate (engine_type ,template ,is_default) select 'MX-13','MX-11, MX-13',true where not exists (select 1 from master.vehicleperformancetemplate where engine_type='MX-13' and template='MX-11, MX-13');
insert into master.vehicleperformancetemplate (engine_type ,template ,is_default) select 'MX-11','MX-11, MX-13',true where not exists (select 1 from master.vehicleperformancetemplate where engine_type='MX-11' and template='MX-11, MX-13');
insert into master.vehicleperformancetemplate (engine_type ,template ,is_default) select 'MX','MX-11, MX-13',true where not exists (select 1 from master.vehicleperformancetemplate where engine_type='MX' and template='MX-11, MX-13');
insert into master.vehicleperformancetemplate (engine_type ,template ,is_default) select 'PX-7','PX-5, PX7',true where not exists (select 1 from master.vehicleperformancetemplate where engine_type='PX-7' and template='PX-5, PX7');
insert into master.vehicleperformancetemplate (engine_type ,template ,is_default) select 'PX-5','PX-5, PX7',true where not exists (select 1 from master.vehicleperformancetemplate where engine_type='PX-5' and template='PX-5, PX7');
insert into master.vehicleperformancetemplate (engine_type ,template ,is_default) select 'FR','PX-5, PX7',true where not exists (select 1 from master.vehicleperformancetemplate where engine_type='FR' and template='PX-5, PX7');
insert into master.vehicleperformancetemplate (engine_type ,template ,is_default) select 'GR','PX-5, PX7',true where not exists (select 1 from master.vehicleperformancetemplate where engine_type='GR' and template='PX-5, PX7');
insert into master.vehicleperformancetemplate (engine_type ,template ,is_default) select 'PR','PX-5, PX7',true where not exists (select 1 from master.vehicleperformancetemplate where engine_type='PR' and template='PX-5, PX7');
insert into master.vehicleperformancetemplate (engine_type ,template ,is_default) select '','MX-11, MX-13',true where not exists (select 1 from master.vehicleperformancetemplate where engine_type='' and template='MX-11, MX-13');

insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','E',-1,'-1',array['600','900','1100','1200','1300','1500','1700','1900','2100','>2100'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='E' and index='-1');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','E',0,'10',array['S','A','A','A','A','A','A','P','E','N'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='E' and index='0');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','E',1,'20',array['A','A','O','O','O','A','A','P','P','N'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='E' and index='1');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','E',2,'30',array['A','A','O','O','O','A','A','P','P','N'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='E' and index='2');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','E',3,'40',array['A','A','O','O','O','A','A','P','P','N'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='E' and index='3');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','E',4,'50',array['A','A','A','A','A','A','A','P','P','N'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='E' and index='4');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','E',5,'60',array['A','A','A','A','A','A','A','P','P','N'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='E' and index='5');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','E',6,'70',array['A','A','A','A','A','A','A','P','N','N'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='E' and index='6');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','E',7,'80',array['N','A','A','A','A','A','A','N','N','N'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='E' and index='7');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','E',8,'90',array['N','A','A','A','A','A','N','N','N','N'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='E' and index='8');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','E',9,'100',array['N','N','A','A','A','N','N','N','N','N'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='E' and index='9');

insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','S',-1,'-1',array['15','40','50','60','70','80','85','90','100','125'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='S' and index='-1');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','S',0,'600',array['S','A','A','A','A','A','A','A','A','A'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='S' and index='0');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','S',1,'900',array['A','A','A','A','A','A','A','A','A','A'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='S' and index='1');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','S',2,'1100',array['A','A','A','A','A','A','A','A','A','A'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='S' and index='2');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','S',3,'1200',array['A','A','A','A','A','A','A','A','A','A'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='S' and index='3');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','S',4,'1300',array['A','A','A','A','A','A','A','A','A','A'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='S' and index='4');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','S',5,'1500',array['A','A','A','A','A','A','A','A','A','A'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='S' and index='5');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','S',6,'1700',array['A','A','A','A','A','A','A','A','A','A'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='S' and index='6');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','S',7,'1900',array['P','P','P','P','P','P','P','P','P','P'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='S' and index='7');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','S',8,'2100',array['E','E','E','E','E','E','E','E','E','E'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='S' and index='8');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select 'MX-11, MX-13','S',9,'>2100',array['N','N','N','N','N','N','N','N','N','N'] where not exists (select 1 from master.performancematrix where template='MX-11, MX-13' and vehicle_performance_type='S' and index='9');

insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select '','B',-1,'-1',array['15','40','50','60','70','80','85','90','100','125'] where not exists (select 1 from master.performancematrix where template='' and vehicle_performance_type='B' and index='-1');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select '','B',0,'-0.25',array['O','O','O','O','O','O','O','O','O','O'] where not exists (select 1 from master.performancematrix where template='' and vehicle_performance_type='B' and index='0');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select '','B',1,'-0.5',array['O','O','O','O','O','O','O','O','O','O'] where not exists (select 1 from master.performancematrix where template='' and vehicle_performance_type='B' and index='1');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select '','B',2,'-0.75',array['A','A','A','A','A','A','A','A','A','A'] where not exists (select 1 from master.performancematrix where template='' and vehicle_performance_type='B' and index='2');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select '','B',3,'-1',array['A','A','A','A','A','A','A','A','A','A'] where not exists (select 1 from master.performancematrix where template='' and vehicle_performance_type='B' and index='3');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select '','B',4,'-1.25',array['M','M','M','M','M','M','M','M','M','M'] where not exists (select 1 from master.performancematrix where template='' and vehicle_performance_type='B' and index='4');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select '','B',5,'-1.5',array['M','M','M','M','M','M','M','M','M','M'] where not exists (select 1 from master.performancematrix where template='' and vehicle_performance_type='B' and index='5');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select '','B',6,'-2',array['T','T','T','T','T','T','T','T','T','T'] where not exists (select 1 from master.performancematrix where template='' and vehicle_performance_type='B' and index='6');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select '','B',7,'-2.5',array['T','T','T','T','T','T','T','T','T','T'] where not exists (select 1 from master.performancematrix where template='' and vehicle_performance_type='B' and index='7');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select '','B',8,'-3',array['H','H','H','H','H','H','H','H','H','H'] where not exists (select 1 from master.performancematrix where template='' and vehicle_performance_type='B' and index='8');
insert into master.performancematrix (template ,vehicle_performance_type,index ,range ,row) select '','B',9,'<-3',array['H','H','H','H','H','H','H','H','H','H'] where not exists (select 1 from master.performancematrix where template='' and vehicle_performance_type='B' and index='9');


INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'Y','E',null ,'enumvehicleperformancetype_engineloadcollective' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   
WHERE key ='enumvehicleperformancetype_engineloadcollective');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','enumvehicleperformancetype_engineloadcollective',
'Engine Load Collective',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   
WHERE code = 'EN-GB' and name = 'enumvehicleperformancetype_engineloadcollective');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehicleperformancetype_engineloadcollective',
(select id from master.menu where name = 'Vehicle Performance Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  
WHERE name = 'enumvehicleperformancetype_engineloadcollective' 
and ref_id=(select id from master.menu where name = 'Vehicle Performance Report'));

INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'Y','S',null ,'enumvehicleperformancetype_roadspeedcollective' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   
WHERE key ='enumvehicleperformancetype_roadspeedcollective');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','enumvehicleperformancetype_roadspeedcollective',
'Road Speed Collective',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   
WHERE code = 'EN-GB' and name = 'enumvehicleperformancetype_roadspeedcollective');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehicleperformancetype_roadspeedcollective',
(select id from master.menu where name = 'Vehicle Performance Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  
WHERE name = 'enumvehicleperformancetype_roadspeedcollective' 
and ref_id=(select id from master.menu where name = 'Vehicle Performance Report'));

INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'Y','B',null ,'enumvehicleperformancetype_brakingbehavior' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   
WHERE key ='enumvehicleperformancetype_brakingbehavior');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','enumvehicleperformancetype_brakingbehavior',
'Braking Behavior',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   
WHERE code = 'EN-GB' and name = 'enumvehicleperformancetype_brakingbehavior');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehicleperformancetype_brakingbehavior',
(select id from master.menu where name = 'Vehicle Performance Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  
WHERE name = 'enumvehicleperformancetype_brakingbehavior' 
and ref_id=(select id from master.menu where name = 'Vehicle Performance Report'));

INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'N','A',null ,'enumvehicleperformancekpi_acceptable' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   
WHERE key ='enumvehicleperformancekpi_acceptable');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','enumvehicleperformancekpi_acceptable',
'Acceptable',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   
WHERE code = 'EN-GB' and name = 'enumvehicleperformancekpi_acceptable');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehicleperformancekpi_acceptable',
(select id from master.menu where name = 'Vehicle Performance Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  
WHERE name = 'enumvehicleperformancekpi_acceptable' 
and ref_id=(select id from master.menu where name = 'Vehicle Performance Report'));

INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'N','N',null ,'enumvehicleperformancekpi_donotoperate' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   
WHERE key ='enumvehicleperformancekpi_donotoperate');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','enumvehicleperformancekpi_donotoperate',
'Do Not Operate',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   
WHERE code = 'EN-GB' and name = 'enumvehicleperformancekpi_donotoperate');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehicleperformancekpi_donotoperate',
(select id from master.menu where name = 'Vehicle Performance Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  
WHERE name = 'enumvehicleperformancekpi_donotoperate' 
and ref_id=(select id from master.menu where name = 'Vehicle Performance Report'));

INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'N','O',null ,'enumvehicleperformancekpi_optimum' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   
WHERE key ='enumvehicleperformancekpi_optimum');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','enumvehicleperformancekpi_optimum',
'Optimum',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   
WHERE code = 'EN-GB' and name = 'enumvehicleperformancekpi_optimum');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehicleperformancekpi_optimum',
(select id from master.menu where name = 'Vehicle Performance Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  
WHERE name = 'enumvehicleperformancekpi_optimum' 
and ref_id=(select id from master.menu where name = 'Vehicle Performance Report'));

INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'N','P',null ,'enumvehicleperformancekpi_okifpowerrequired' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   
WHERE key ='enumvehicleperformancekpi_okifpowerrequired');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','enumvehicleperformancekpi_okifpowerrequired',
'Ok If Power Required',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   
WHERE code = 'EN-GB' and name = 'enumvehicleperformancekpi_okifpowerrequired');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehicleperformancekpi_okifpowerrequired',
(select id from master.menu where name = 'Vehicle Performance Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  
WHERE name = 'enumvehicleperformancekpi_okifpowerrequired' 
and ref_id=(select id from master.menu where name = 'Vehicle Performance Report'));

INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'N','E',null ,'enumvehicleperformancekpi_okenginebrakeapplied' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   
WHERE key ='enumvehicleperformancekpi_okenginebrakeapplied');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','enumvehicleperformancekpi_okenginebrakeapplied',
'OK / Engine Brake Applied',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   
WHERE code = 'EN-GB' and name = 'enumvehicleperformancekpi_okenginebrakeapplied');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehicleperformancekpi_okenginebrakeapplied',
(select id from master.menu where name = 'Vehicle Performance Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  
WHERE name = 'enumvehicleperformancekpi_okenginebrakeapplied' 
and ref_id=(select id from master.menu where name = 'Vehicle Performance Report'));

INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'N','U',null ,'enumvehicleperformancekpi_upshiftpossible' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   
WHERE key ='enumvehicleperformancekpi_upshiftpossible');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','enumvehicleperformancekpi_upshiftpossible',
'Upshift Possible',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   
WHERE code = 'EN-GB' and name = 'enumvehicleperformancekpi_upshiftpossible');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehicleperformancekpi_upshiftpossible',
(select id from master.menu where name = 'Vehicle Performance Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  
WHERE name = 'enumvehicleperformancekpi_upshiftpossible' 
and ref_id=(select id from master.menu where name = 'Vehicle Performance Report'));

INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'N','D',null ,'enumvehicleperformancekpi_downshiftpossible' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   
WHERE key ='enumvehicleperformancekpi_downshiftpossible');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','enumvehicleperformancekpi_downshiftpossible',
'Downshift Possible',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   
WHERE code = 'EN-GB' and name = 'enumvehicleperformancekpi_downshiftpossible');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehicleperformancekpi_downshiftpossible',
(select id from master.menu where name = 'Vehicle Performance Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  
WHERE name = 'enumvehicleperformancekpi_downshiftpossible' 
and ref_id=(select id from master.menu where name = 'Vehicle Performance Report'));

INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'N','S',null ,'enumvehicleperformancekpi_stationary' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   
WHERE key ='enumvehicleperformancekpi_stationary');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','enumvehicleperformancekpi_stationary',
'Stationary',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   
WHERE code = 'EN-GB' and name = 'enumvehicleperformancekpi_stationary');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehicleperformancekpi_stationary',
(select id from master.menu where name = 'Vehicle Performance Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  
WHERE name = 'enumvehicleperformancekpi_stationary' 
and ref_id=(select id from master.menu where name = 'Vehicle Performance Report'));

INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'N','M',null ,'enumvehicleperformancekpi_moderate' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   
WHERE key ='enumvehicleperformancekpi_moderate');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','enumvehicleperformancekpi_moderate',
'Moderate',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   
WHERE code = 'EN-GB' and name = 'enumvehicleperformancekpi_moderate');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehicleperformancekpi_moderate',
(select id from master.menu where name = 'Vehicle Performance Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  
WHERE name = 'enumvehicleperformancekpi_moderate' 
and ref_id=(select id from master.menu where name = 'Vehicle Performance Report'));

INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'N','T',null ,'enumvehicleperformancekpi_strong' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   
WHERE key ='enumvehicleperformancekpi_strong');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','enumvehicleperformancekpi_strong',
'Strong',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   
WHERE code = 'EN-GB' and name = 'enumvehicleperformancekpi_strong');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehicleperformancekpi_strong',
(select id from master.menu where name = 'Vehicle Performance Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  
WHERE name = 'enumvehicleperformancekpi_strong' 
and ref_id=(select id from master.menu where name = 'Vehicle Performance Report'));

INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'N','H',null ,'enumvehicleperformancekpi_harshbrake' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   
WHERE key ='enumvehicleperformancekpi_harshbrake');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','enumvehicleperformancekpi_harshbrake',
'Harsh Brake',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   
WHERE code = 'EN-GB' and name = 'enumvehicleperformancekpi_harshbrake');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehicleperformancekpi_harshbrake',
(select id from master.menu where name = 'Vehicle Performance Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  
WHERE name = 'enumvehicleperformancekpi_harshbrake' 
and ref_id=(select id from master.menu where name = 'Vehicle Performance Report'));

update master.emailtemplate set description='<!doctype html>
<html>
<head>
<style>
.detailsDiv {{
  border: none;
  background-color: lightblue;    
  text-align: left
}}
.reportDetailsTable {{
  border-collapse: collapse;
  width: 100%;
}}

.reportDetailsTable td, #reportDetailsTable th {{
  border: 1px solid;
  padding: 8px;
}}

.reportDetailsTable tr:nth-child(even){{background-color: #ffffff;}}

.reportDetailsTable th {{
  border: 1px solid;
  padding-top: 12px;
  padding-bottom: 12px;
  text-align: center;
  background-color: lightblue;
  color: black;
}}
thead {{ display: table-header-group }}
tfoot {{ display: table-row-group }}
tr {{ page-break-inside: avoid }}
</style>
</head>
  <body>
	<table style="width: 100%;">
        <tr>
            <td><img style="margin:20px 00px" align="left" width="380px" height="80px" src="{0}"></td>
            <td><h2 style="text-align: center">[lblTripReportDetails]</h2></td>
            <td><img style="margin:0px 0px" align="right" width="180px" height="80px" src="{1}"></td>
        </tr>
    </table>

	
	<div class="detailsDiv">
	  <table  style="width: 100%;">
		<tr>
			<td style="width: 30%;"><p><strong>[lblFrom] : </strong>  {2}</P></td>
			<td style="width: 30%;"><p><strong>[lblVehicleGroup] : </strong>  {3}</P></td>
			<td style="width: 30%;"><p><strong>[lblVehicleVIN] : </strong>  {4}</P></td>
		</tr>
		<tr>
			<td style="width: 30%;"><p><strong>[lblTo] : </strong>  {5}</P></td>
			<td style="width: 30%;"><p><strong>[lblVehicleName] : </strong>  {6}</P></td>
			<td style="width: 30%;"><p><strong>[lblRegPlateNumber] : </strong>  {7}</P></td>
		</tr>
	  </table>
	</div><br/><br/>
	
	<table class="reportDetailsTable">
		<thead>
			<th>[lblStartDate] </th>
			<th>[lblEndDate]</th>
			<th>[lblDistance] ({9})</th>
			<th>[lblIdleDuration] ({10})</th>
			<th>[lblAverageSpeed] ({11})</th>
			<th>[lblAverageWeight] ({12})</th>
			<th>[lblOdometer] ({13})</th>
			<th>[lblStartPosition]</th>
			<th>[lblEndPosition]</th>
			<th>[lblFuelConsumption] ({14})</th>
			<th>[lblDrivingTime] ({15})</th>
			<th>[lblAlerts]</th>
			<th>[lblEvents]</th>
		</thead>
		{8}
	</table>
  </body>
</html>'
where event_name='TripReport';

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'xr-XR',TYPE,NAME,NAME,created_at
from TRANSLATION.TRANSLATION;


--************************

insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'E',0,'',0,10where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='E' and index=0);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'E',1,'O',10,20where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='E' and index=1);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'E',2,'A',20,30where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='E' and index=2);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'E',3,'P',30,40where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='E' and index=3);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'E',4,'E',40,50where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='E' and index=4);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'E',5,'N',50,60where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='E' and index=5);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'E',6,'S',60,70where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='E' and index=6);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'E',7,'D',70,80where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='E' and index=7);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'E',8,'U',80,90where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='E' and index=8);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'E',9,'',90,125where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='E' and index=9);

insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'S',0,'',0,15where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='S' and index=0);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'S',1,'O',15,40where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='S' and index=1);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'S',2,'A',40,50where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='S' and index=2);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'S',3,'P',50,60where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='S' and index=3);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'S',4,'E',60,70where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='S' and index=4);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'S',5,'N',70,80where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='S' and index=5);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'S',6,'S',80,85where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='S' and index=6);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'S',7,'D',85,90where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='S' and index=7);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'S',8,'U',90,100where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='S' and index=8);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'S',9,'',100,125where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='S' and index=9);

insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'B',9,'',null,-3 where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='B' and index=9);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'B',8,'',-3,-2.5 where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='B' and index=8);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'B',7,'',-2.5,-2 where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='B' and index=7);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'B',6,'',-2,-1.5 where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='B' and index=6);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'B',5,'H',-1.5,-1.25 where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='B' and index=5);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'B',4,'T',-1.25,-1 where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='B' and index=4);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'B',3,'M',-1,-0.75 where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='B' and index=3);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'B',2,'A',-0.75,-0.5 where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='B' and index=2);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'B',1,'O',-0.5,-0.25 where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='B' and index=1);
insert into master.performancekpiranges (vehicle_performance_type  ,index  ,kpi  ,lower_val  ,upper_val) select 'B',0,'',-0.25,-0.1 where not exists (select 1 from master.performancekpiranges where vehicle_performance_type='B' and index=0);

