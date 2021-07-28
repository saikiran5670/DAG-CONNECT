update master.reportattribute 
set name = (select replace(name,'EcoScore.General.','') from master.dataattribute
where name='EcoScore.General.AverageGrossweight') 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.General.AverageGrossweight') and report_id=10;	

update master.reportattribute 
set name = (select replace(name,'EcoScore.General.','') from master.dataattribute
where name='EcoScore.General.Distance') 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.General.Distance') and report_id=10;	

update master.reportattribute 
set name = (select replace(name,'EcoScore.General.','') from master.dataattribute
where name='EcoScore.General.NumberOfTrips') 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.General.NumberOfTrips') and report_id=10;	

update master.reportattribute 
set name = (select replace(name,'EcoScore.General.','') from master.dataattribute
where name='EcoScore.General.NumberOfVehicles') 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.General.NumberOfVehicles') and report_id=10;	

update master.reportattribute 
set name = (select replace(name,'EcoScore.General.','') from master.dataattribute
where name='EcoScore.General.AverageDistancePerDay') 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.General.AverageDistancePerDay') and report_id=10;	


update master.reportattribute 
set name = (select replace(name,'EcoScore.DriverPerformance.','') from master.dataattribute
where name='EcoScore.DriverPerformance.EcoScore') 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.EcoScore') and report_id=10;	

update master.reportattribute 
set name = (select replace(name,'EcoScore.DriverPerformance.','') from master.dataattribute
where name='EcoScore.DriverPerformance.FuelConsumption') 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.FuelConsumption') and report_id=10;	

update master.reportattribute 
set name = (select replace(name,'EcoScore.DriverPerformance.FuelConsumption.','') from master.dataattribute
where name='EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage') 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage') and report_id=10;	

update master.reportattribute 
set name = 'CruiseControlUsage30' 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage30-50km/h(%)') and report_id=10;	

update master.reportattribute 
set name = 'CruiseControlUsage50' 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage50-75km/h(%)') and report_id=10;	

update master.reportattribute 
set name = 'CruiseControlUsage75' 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage>75km/h(%)') and report_id=10;	

update master.reportattribute 
set name = 'PTOUsage' 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.FuelConsumption.PTOUsage(%)') and report_id=10;	

update master.reportattribute 
set name = (select replace(name,'EcoScore.DriverPerformance.FuelConsumption.','') from master.dataattribute
where name='EcoScore.DriverPerformance.FuelConsumption.PTODuration') 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.FuelConsumption.PTODuration') and report_id=10;	

update master.reportattribute 
set name = (select replace(name,'EcoScore.DriverPerformance.FuelConsumption.','') from master.dataattribute
where name='EcoScore.DriverPerformance.FuelConsumption.AverageDrivingSpeed') 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.FuelConsumption.AverageDrivingSpeed') and report_id=10;	

update master.reportattribute 
set name = (select replace(name,'EcoScore.DriverPerformance.FuelConsumption.','') from master.dataattribute
where name='EcoScore.DriverPerformance.FuelConsumption.AverageSpeed') 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.FuelConsumption.AverageSpeed') and report_id=10;	

update master.reportattribute 
set name = 'HeavyThrottling' 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.FuelConsumption.HeavyThrottling(%)') and report_id=10;	

update master.reportattribute 
set name = (select replace(name,'EcoScore.DriverPerformance.FuelConsumption.','') from master.dataattribute
where name='EcoScore.DriverPerformance.FuelConsumption.HeavyThrottleDuration') 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.FuelConsumption.HeavyThrottleDuration') and report_id=10;	

update master.reportattribute 
set name = 'Idling'
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.FuelConsumption.Idling(%)') and report_id=10;	

update master.reportattribute 
set name = (select replace(name,'EcoScore.DriverPerformance.FuelConsumption.','') from master.dataattribute
where name='EcoScore.DriverPerformance.FuelConsumption.IdleDuration') 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.FuelConsumption.IdleDuration') and report_id=10;	

update master.reportattribute 
set name = (select replace(name,'EcoScore.DriverPerformance.','') from master.dataattribute
where name='EcoScore.DriverPerformance.BrakingScore') 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.BrakingScore') and report_id=10;	

update master.reportattribute 
set name = 'HarshBraking' 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.BrakingScore.HarshBraking(%)') and report_id=10;	

update master.reportattribute 
set name = (select replace(name,'EcoScore.DriverPerformance.BrakingScore.','') from master.dataattribute
where name='EcoScore.DriverPerformance.BrakingScore.HarshBrakeDuration') 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.BrakingScore.HarshBrakeDuration') and report_id=10;	

update master.reportattribute 
set name = 'Braking' 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.BrakingScore.Braking(%)') and report_id=10;	

update master.reportattribute 
set name = (select replace(name,'EcoScore.DriverPerformance.BrakingScore.','') from master.dataattribute
where name='EcoScore.DriverPerformance.BrakingScore.BrakeDuration') 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.BrakingScore.BrakeDuration') and report_id=10;	

update master.reportattribute 
set name = (select replace(name,'EcoScore.DriverPerformance.','') from master.dataattribute
where name='EcoScore.DriverPerformance.AnticipationScore') 
where data_attribute_id=(select id from master.dataattribute where name='EcoScore.DriverPerformance.AnticipationScore') and report_id=10;	

update master.ecoscorekpi set data_attribute_id=(select id from master.dataattribute where name ='EcoScore') where data_attribute_id is null and name like '%Eco-Score%';
update master.ecoscorekpi set data_attribute_id=(select id from master.dataattribute where name like '%EcoScore.%30-50%') where data_attribute_id is null and name like '%Cruise%30-50%';
update master.ecoscorekpi set data_attribute_id=(select id from master.dataattribute where name like '%EcoScore.%50-75%') where data_attribute_id is null and name like '%Cruise%50-75%';
update master.ecoscorekpi set data_attribute_id=(select id from master.dataattribute where name like '%EcoScore.%>75%') where data_attribute_id is null and name like '%Cruise%>75%';


/*
--TRanslaation
"rp_fu_report_calendarview_activevehicles"
"rp_fu_report_calendarview_averageweight"
"rp_fu_report_calendarview_distance"
"rp_fu_report_calendarview_drivingtime"
"rp_fu_report_calendarview_expensiontype"
"rp_fu_report_calendarview_idleduration"
"rp_fu_report_calendarview_mileagebasedutilization"
"rp_fu_report_calendarview_timebasedutilization"
"rp_fu_report_calendarview_totaltrips"

select 'rp_fu_report_calendarview_'||lower(replace(d.name,'Report.CalendarView.','')), r.name,d.name,ra.* from master.report r inner join master.reportattribute ra on r.id=ra.report_id
inner join master.dataattribute d on ra.data_attribute_id=d.id
where lower(r.name) like '%uti%' 
--and 
--ra.key is null
--lower(d.name) like '%health%'
--ra.name is not null
and d.name like ()
order by 2

update master.reportattribute set type='S' where data_attribute_id in (192
,188,
190,
191,
220,
189,
194,
195,
193,
184,
186,
185,
187,
17,
24,
28,
44,
55,
84,
98,
100,
107,
119,
132,
135,
137,
18,
182,
183,
99,
123) and report_id=5 and type is null;
*/

--update master.reportpreference set key ='rp_fu_report_calendarview_'+
update master.reportattribute set key =(select  'rp_fu_report_calendarview_'||lower(replace(name,'Report.CalendarView.','')) from master.dataattribute where id=192) 
where report_id=5 and data_attribute_id=192;
update master.reportattribute set key =(select  'rp_fu_report_calendarview_'||lower(replace(name,'Report.CalendarView.','')) 
from master.dataattribute where id=188) 
where report_id=5 and data_attribute_id=188;
update master.reportattribute set key =(select  'rp_fu_report_calendarview_'||lower(replace(name,'Report.CalendarView.','')) 
from master.dataattribute where id=190) 
where report_id=5 and data_attribute_id=190;
update master.reportattribute set key =(select  'rp_fu_report_calendarview_'||lower(replace(name,'Report.CalendarView.','')) 
from master.dataattribute where id=191) 
where report_id=5 and data_attribute_id=191;
update master.reportattribute set key =(select  'rp_fu_report_calendarview_'||lower(replace(name,'Report.CalendarView.','')) 
from master.dataattribute where id=220) 
where report_id=5 and data_attribute_id=220;
update master.reportattribute set key =(select  'rp_fu_report_calendarview_'||lower(replace(name,'Report.CalendarView.','')) 
from master.dataattribute where id=189) 
where report_id=5 and data_attribute_id=189;
update master.reportattribute set key =(select  'rp_fu_report_calendarview_'||lower(replace(name,'Report.CalendarView.','')) 
from master.dataattribute where id=194) 
where report_id=5 and data_attribute_id=194;
update master.reportattribute set key =(select  'rp_fu_report_calendarview_'||lower(replace(name,'Report.CalendarView.','')) 
from master.dataattribute where id=195) 
where report_id=5 and data_attribute_id=195;
update master.reportattribute set key =(select  'rp_fu_report_calendarview_'||lower(replace(name,'Report.CalendarView.','')) 
from master.dataattribute where id=193) 
where report_id=5 and data_attribute_id=193;

update master.reportattribute set key =(select  'rp_fu_report_charts_'||lower(replace(name,'Report.Charts.','')) 
from master.dataattribute where id=184) 
where report_id=5 and data_attribute_id=184;
update master.reportattribute set key =(select  'rp_fu_report_charts_'||lower(replace(name,'Report.Charts.','')) 
from master.dataattribute where id=186) 
where report_id=5 and data_attribute_id=186;
update master.reportattribute set key =(select  'rp_fu_report_charts_'||lower(replace(name,'Report.Charts.','')) 
from master.dataattribute where id=185) 
where report_id=5 and data_attribute_id=185;
update master.reportattribute set key =(select  'rp_fu_report_charts_'||lower(replace(name,'Report.Charts.','')) 
from master.dataattribute where id=187) 
where report_id=5 and data_attribute_id=187;

update master.reportattribute set key =(select  'rp_fu_report_details_'||lower(replace(name,'Report.Details.','')) 
from master.dataattribute where id=17) 
where report_id=5 and data_attribute_id=17;
update master.reportattribute set key =(select  'rp_fu_report_details_'||lower(replace(name,'Report.Details.','')) 
from master.dataattribute where id=24) 
where report_id=5 and data_attribute_id=24;
update master.reportattribute set key =(select  'rp_fu_report_details_'||lower(replace(name,'Report.Details.','')) 
from master.dataattribute where id=28) 
where report_id=5 and data_attribute_id=28;
update master.reportattribute set key =(select  'rp_fu_report_details_'||lower(replace(name,'Report.Details.','')) 
from master.dataattribute where id=44) 
where report_id=5 and data_attribute_id=44;
update master.reportattribute set key =(select  'rp_fu_report_details_'||lower(replace(name,'Report.Details.','')) 
from master.dataattribute where id=55) 
where report_id=5 and data_attribute_id=55;
update master.reportattribute set key =(select  'rp_fu_report_details_'||lower(replace(name,'Report.Details.','')) 
from master.dataattribute where id=84) 
where report_id=5 and data_attribute_id=84;
update master.reportattribute set key =(select  'rp_fu_report_details_'||lower(replace(name,'Report.Details.','')) 
from master.dataattribute where id=98) 
where report_id=5 and data_attribute_id=98;
update master.reportattribute set key =(select  'rp_fu_report_details_'||lower(replace(name,'Report.Details.','')) 
from master.dataattribute where id=100) 
where report_id=5 and data_attribute_id=100;
update master.reportattribute set key =(select  'rp_fu_report_details_'||lower(replace(name,'Report.Details.','')) 
from master.dataattribute where id=107) 
where report_id=5 and data_attribute_id=107;
update master.reportattribute set key =(select  'rp_fu_report_details_'||lower(replace(name,'Report.Details.','')) 
from master.dataattribute where id=119) 
where report_id=5 and data_attribute_id=119;
update master.reportattribute set key =(select  'rp_fu_report_details_'||lower(replace(name,'Report.Details.','')) 
from master.dataattribute where id=132) 
where report_id=5 and data_attribute_id=132;
update master.reportattribute set key =(select  'rp_fu_report_details_'||lower(replace(name,'Report.Details.','')) 
from master.dataattribute where id=135) 
where report_id=5 and data_attribute_id=135;
update master.reportattribute set key =(select  'rp_fu_report_details_'||lower(replace(name,'Report.Details.','')) 
from master.dataattribute where id=137) 
where report_id=5 and data_attribute_id=137;

update master.reportattribute set key =(select  'rp_fu_report_general_'||lower(replace(name,'Report.General.','')) 
from master.dataattribute where id=18) 
where report_id=5 and data_attribute_id=18;
update master.reportattribute set key =(select  'rp_fu_report_general_'||lower(replace(name,'Report.General.','')) 
from master.dataattribute where id=182) 
where report_id=5 and data_attribute_id=182;
update master.reportattribute set key =(select  'rp_fu_report_general_'||lower(replace(name,'Report.General.','')) 
from master.dataattribute where id=183) 
where report_id=5 and data_attribute_id=183;
update master.reportattribute set key =(select  'rp_fu_report_general_'||lower(replace(name,'Report.General.','')) 
from master.dataattribute where id=99) 
where report_id=5 and data_attribute_id=99;
update master.reportattribute set key =(select  'rp_fu_report_general_'||lower(replace(name,'Report.General.','')) 
from master.dataattribute where id=123) 
where report_id=5 and data_attribute_id=123;


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_calendarview_activevehicles','Report.CalendarView.ActiveVehicles',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_calendarview_activevehicles');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_calendarview_activevehicles',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_calendarview_activevehicles' 
and ref_id=0);


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_calendarview_averageweight','Report.CalendarView.AverageWeight',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_calendarview_averageweight');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_calendarview_averageweight',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_calendarview_averageweight' 
and ref_id=0);

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_calendarview_distance','Report.CalendarView.Distance',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_calendarview_distance');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_calendarview_distance',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_calendarview_distance' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_calendarview_drivingtime','Report.CalendarView.DrivingTime',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_calendarview_drivingtime');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_calendarview_drivingtime',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_calendarview_drivingtime' 
and ref_id=0);


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_calendarview_expensiontype','Report.CalendarView.ExpensionType',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_calendarview_expensiontype');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_calendarview_expensiontype',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_calendarview_expensiontype' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_calendarview_idleduration','Report.CalendarView.IdleDuration',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_calendarview_idleduration');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_calendarview_idleduration',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_calendarview_idleduration' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_calendarview_mileagebasedutilization','Report.CalendarView.MileageBasedUtilization',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_calendarview_mileagebasedutilization');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_calendarview_mileagebasedutilization',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_calendarview_mileagebasedutilization' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_calendarview_timebasedutilization','Report.CalendarView.TimeBasedUtilization',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_calendarview_timebasedutilization');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_calendarview_timebasedutilization',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_calendarview_timebasedutilization' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_calendarview_totaltrips','Report.CalendarView.TotalTrips',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_calendarview_totaltrips');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_calendarview_totaltrips',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_calendarview_totaltrips' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_charts_distanceperday','Report.Charts.DistancePerDay',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_charts_distanceperday');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_charts_distanceperday',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_charts_distanceperday' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_charts_mileagebasedutilization','Report.Charts.MileageBasedUtilization',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_charts_mileagebasedutilization');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_charts_mileagebasedutilization',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_charts_mileagebasedutilization' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_charts_numberofvehiclesperday','Report.Charts.NumberOfVehiclesPerDay',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_charts_numberofvehiclesperday');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_charts_numberofvehiclesperday',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_charts_numberofvehiclesperday' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_charts_timebasedutilization','Report.Charts.TimeBasedUtilization',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_charts_timebasedutilization');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_charts_timebasedutilization',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_charts_timebasedutilization' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_details_averagedistanceperday','Report.Details.AverageDistancePerDay',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_details_averagedistanceperday');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_details_averagedistanceperday',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_details_averagedistanceperday' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_details_averagespeed','Report.Details.AverageSpeed',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_details_averagespeed');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_details_averagespeed',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_details_averagespeed' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_details_averageweightpertrip','Report.Details.AverageWeightPerTrip',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_details_averageweightpertrip');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_details_averageweightpertrip',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_details_averageweightpertrip' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_details_distance','Report.Details.Distance',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_details_distance');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_details_distance',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_details_distance' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_details_drivingtime','Report.Details.DrivingTime',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_details_drivingtime');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_details_drivingtime',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_details_drivingtime' 
and ref_id=0);


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_details_idleduration','Report.Details.IdleDuration',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_details_idleduration');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_details_idleduration',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_details_idleduration' 
and ref_id=0);

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_details_numberoftrips','Report.Details.NumberOfTrips',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_details_numberoftrips');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_details_numberoftrips',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_details_numberoftrips' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_details_odometer','Report.Details.Odometer',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_details_odometer');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_details_odometer',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_details_odometer' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_details_registrationnumber','Report.Details.RegistrationNumber',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_details_registrationnumber');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_details_registrationnumber',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_details_registrationnumber' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_details_stoptime','Report.Details.StopTime',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_details_stoptime');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_details_stoptime',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_details_stoptime' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_details_triptime','Report.Details.TripTime',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_details_triptime');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_details_triptime',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_details_triptime' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_details_vehiclename','Report.Details.VehicleName',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_details_vehiclename');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_details_vehiclename',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_details_vehiclename' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_details_vin','Report.Details.VIN',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_details_vin');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_details_vin',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_details_vin' 
and ref_id=0);


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_general_averagedistanceperday','Report.General.AverageDistancePerDay',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_general_averagedistanceperday');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_general_averagedistanceperday',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_general_averagedistanceperday' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_general_idleduration','Report.General.IdleDuration',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_general_idleduration');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_general_idleduration',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_general_idleduration' 
and ref_id=0);


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_general_numberoftrips','Report.General.NumberOfTrips',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_general_numberoftrips');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_general_numberoftrips',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_general_numberoftrips' 
and ref_id=0);

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_general_numberofvehicles','Report.General.NumberOfVehicles',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_general_numberofvehicles');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_general_numberofvehicles',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_general_numberofvehicles' 
and ref_id=0);



INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','rp_fu_report_general_totaldistance','Report.General.TotalDistance',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'rp_fu_report_general_totaldistance');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_general_totaldistance',0,'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_general_totaldistance' 
and ref_id=0);


--************************************************


INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
                             key, level,state)   
SELECT 808,'api.rfms3','G',null,'feat_api_rfms3',40,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='api.rfms3');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_api_rfms3','api.rfms3',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_api_rfms3');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3',(select id from master.menu where name = 'Account Role Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3',(select id from master.menu where name = 'Package Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3',(select id from master.menu where name = 'Organisation Relationship Management'),'M'
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));

--*****
INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
                             key, level,state)   
SELECT 809,'api.rfms3.vehicles','F',null,'feat_api_rfms3_vehicles',40,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='api.rfms3.vehicles');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_api_rfms3_vehicles','api.rfms3.vehicles',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_api_rfms3_vehicles');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_vehicles',(select id from master.menu where name = 'Account Role Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_vehicles' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_vehicles',(select id from master.menu where name = 'Package Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_vehicles' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_vehicles',(select id from master.menu where name = 'Organisation Relationship Management'),'M'
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_vehicles' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));

--*****
INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
                             key, level,state)   
SELECT 810,'api.rfms3.positions','F',null,'feat_api_rfms3_positions',40,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='api.rfms3.positions');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_api_rfms3_positions','api.rfms3.positions',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_api_rfms3_positions');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_positions',(select id from master.menu where name = 'Account Role Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_positions' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_positions',(select id from master.menu where name = 'Package Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_positions' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_positions',(select id from master.menu where name = 'Organisation Relationship Management'),'M'
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_positions' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));

--*****
INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
                             key, level,state)   
SELECT 811,'api.rfms3.status','F',null,'feat_api_rfms3_status',40,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='api.rfms3.status');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_api_rfms3_status','api.rfms3.status',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_api_rfms3_status');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_status',(select id from master.menu where name = 'Account Role Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_status' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_status',(select id from master.menu where name = 'Package Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_status' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_status',(select id from master.menu where name = 'Organisation Relationship Management'),'M'
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_status' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));

--*****
INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
                             key, level,state)   
SELECT 812,'api.rfms3#rate1','B',null,'feat_api_rfms3_rate1',40,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='api.rfms3#rate1');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_api_rfms3_rate1','api.rfms3#rate1',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_api_rfms3_rate1');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_rate1',(select id from master.menu where name = 'Account Role Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_rate1' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_rate1',(select id from master.menu where name = 'Package Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_rate1' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_rate1',(select id from master.menu where name = 'Organisation Relationship Management'),'M'
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_rate1' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));

--*****
INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
                             key, level,state)   
SELECT 813,'api.rfms3#position-data','B',null,'feat_api_rfms3_position_data',40,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='api.rfms3#position-data');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_api_rfms3_position_data','api.rfms3#position-data',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_api_rfms3_position_data');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_position_data',(select id from master.menu where name = 'Account Role Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_position_data' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_position_data',(select id from master.menu where name = 'Package Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_position_data' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_position_data',(select id from master.menu where name = 'Organisation Relationship Management'),'M'
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_position_data' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));

--*****
INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
                             key, level,state)   
SELECT 814,'api.rfms3#accumulated-data','B',null,'feat_api_rfms3_accumulated_data',40,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='api.rfms3#accumulated-data');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_api_rfms3_accumulated_data','api.rfms3#accumulated-data',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_api_rfms3_accumulated_data');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_accumulated_data',(select id from master.menu where name = 'Account Role Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_accumulated_data' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_accumulated_data',(select id from master.menu where name = 'Package Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_accumulated_data' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_accumulated_data',(select id from master.menu where name = 'Organisation Relationship Management'),'M'
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_accumulated_data' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));

--*****
INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
                             key, level,state)   
SELECT 815,'api.rfms3#snapshot-data','B',null,'feat_api_rfms3_snapshot_data',40,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='api.rfms3#snapshot-data');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_api_rfms3_snapshot_data','api.rfms3#snapshot-data',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_api_rfms3_snapshot_data');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_snapshot_data',(select id from master.menu where name = 'Account Role Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_snapshot_data' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_snapshot_data',(select id from master.menu where name = 'Package Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_snapshot_data' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_snapshot_data',(select id from master.menu where name = 'Organisation Relationship Management'),'M'
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_snapshot_data' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));

--*****
INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
                             key, level,state)   
SELECT 816,'api.rfms3#uptime-data','B',null,'feat_api_rfms3_uptime_data',40,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='api.rfms3#uptime-data');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_api_rfms3_uptime_data','api.rfms3#uptime-data',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_api_rfms3_uptime_data');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_uptime_data',(select id from master.menu where name = 'Account Role Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_uptime_data' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_uptime_data',(select id from master.menu where name = 'Package Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_uptime_data' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_uptime_data',(select id from master.menu where name = 'Organisation Relationship Management'),'M'
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_uptime_data' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));


--****************
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'L','I',null ,'enumfueleventtype_increase' WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumfueleventtype_increase');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','enumfueleventtype_increase','Increase',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumfueleventtype_increase');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumfueleventtype_increase',(select id from master.menu where name = 'Fuel Deviation Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumfueleventtype_increase' 
and ref_id=(select id from master.menu where name = 'Fuel Deviation Report'));

INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'L','D',null ,'enumfueleventtype_decrease' WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumfueleventtype_decrease');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','enumfueleventtype_decrease','Decrease',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumfueleventtype_decrease');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumfueleventtype_decrease',(select id from master.menu where name = 'Fuel Deviation Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumfueleventtype_decrease' 
and ref_id=(select id from master.menu where name = 'Fuel Deviation Report'));

--************

INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'I','S',null ,'enumvehicleactivitytype_stop' WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumvehicleactivitytype_stop');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','enumvehicleactivitytype_stop','Stop',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumvehicleactivitytype_stop');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehicleactivitytype_stop',(select id from master.menu where name = 'Fuel Deviation Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumvehicleactivitytype_stop' 
and ref_id=(select id from master.menu where name = 'Fuel Deviation Report'));

INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'I','R',null ,'enumvehicleactivitytype_running' WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumvehicleactivitytype_running');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','L','enumvehicleactivitytype_running','Running',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumvehicleactivitytype_running');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehicleactivitytype_running',(select id from master.menu where name = 'Fuel Deviation Report'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumvehicleactivitytype_running' 
and ref_id=(select id from master.menu where name = 'Fuel Deviation Report'));

update master.feature set name='api.ecoscore-data' where name='api.eco-score';
update translation.translation  set value='api.ecoscore-data' where value like 'api.eco-score';


INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Report.Driver.VehicleDetails.IdlingWithPTO(%)'	,'A',	'rp_ff_report_driver_vehicledetails_idlingwithptoper'	,	
'Expension Type'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Report.Driver.VehicleDetails.IdlingWithPTO(%)'	);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_ff_report_driver_vehicledetails_idlingwithptoper','Report.Driver.VehicleDetails.IdlingWithPTO(%)',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Report.Driver.VehicleDetails.IdlingWithPTO(%)');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_ff_report_driver_vehicledetails_idlingwithptoper',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_ff_report_driver_vehicledetails_idlingwithptoper' 
and ref_id=0);


INSERT INTO master.dataattribute (name,TYPE,KEY,column_name_for_ui)	
SELECT 	'Report.Vehicle.VehicleDetails.IdlingWithPTO(%)'	,'A',	'rp_ff_report_vehicle_vehicledetails_idlingwithptoper'	,	
'Expension Type'	
WHERE NOT EXISTS (SELECT 1 FROM master.dataattribute WHERE name = 	'Report.Vehicle.VehicleDetails.IdlingWithPTO(%)'	);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_ff_report_vehicle_vehicledetails_idlingwithptoper','Report.Vehicle.VehicleDetails.IdlingWithPTO(%)',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE value = 'Report.Vehicle.VehicleDetails.IdlingWithPTO(%)');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_ff_report_vehicle_vehicledetails_idlingwithptoper',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_ff_report_vehicle_vehicledetails_idlingwithptoper' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_dtm_report_chart_zoomchart','Report.Chart.ZoomChart',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_dtm_report_chart_zoomchart');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_dtm_report_chart_zoomchart',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_dtm_report_chart_zoomchart' 
and ref_id=0);

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_fu_report_calendarview_expensiontype','Report.CalendarView.ExpensionType',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_fu_report_calendarview_expensiontype');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_fu_report_calendarview_expensiontype',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_fu_report_calendarview_expensiontype' 
and ref_id=0);


delete  from  translation.translationgrouping where 
name ='enumhealthstatus_noaction' and id not in (select min(id) 
from translation.translationgrouping where  name ='enumhealthstatus_noaction' group by name,ref_id having count(*) >1);

update translation.enumtranslation set key='enumvehactivitystatus_nevermoved' where key='enumhealthstatus_nevermoved';
update translation.enumtranslation set key='enumvehactivitystatus_driving' where key='enumhealthstatus_driving';
update translation.enumtranslation set key='enumvehactivitystatus_idle' where key='enumhealthstatus_idle';
update translation.enumtranslation set key='enumvehactivitystatus_unknown' where key='enumhealthstatus_unknown';
update translation.enumtranslation set key='enumvehactivitystatus_stopped' where key='enumhealthstatus_stopped';


update translation.translation set name='enumvehactivitystatus_nevermoved' where name='enumhealthstatus_nevermoved';
update translation.translation set name='enumvehactivitystatus_driving' where name='enumhealthstatus_driving';
update translation.translation set name='enumvehactivitystatus_idle' where name='enumhealthstatus_idle';
update translation.translation set name='enumvehactivitystatus_unknown' where name='enumhealthstatus_unknown';
update translation.translation set name='enumvehactivitystatus_stopped' where name='enumhealthstatus_stopped';


INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehactivitystatus_nevermoved',(select id from master.menu where name = 'Fleet Overview'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumvehactivitystatus_nevermoved' 
				   and ref_id=(select id from master.menu where name = 'Fleet Overview'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehactivitystatus_driving',(select id from master.menu where name = 'Fleet Overview'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumvehactivitystatus_driving' 
				   and ref_id=(select id from master.menu where name = 'Fleet Overview'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehactivitystatus_idle',(select id from master.menu where name = 'Fleet Overview'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumvehactivitystatus_idle' 
				   and ref_id=(select id from master.menu where name = 'Fleet Overview'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehactivitystatus_unknown',(select id from master.menu where name = 'Fleet Overview'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumvehactivitystatus_unknown' 
				   and ref_id=(select id from master.menu where name = 'Fleet Overview'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvehactivitystatus_stopped',(select id from master.menu where name = 'Fleet Overview'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumvehactivitystatus_stopped' 
				   and ref_id=(select id from master.menu where name = 'Fleet Overview'));


update master.feature set level=30 where lower(name) like '%rfms3%';

INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
                             key, level,state)   
SELECT 817,'api.rfms3.auth','F',null,'feat_api_rfms3_auth',30,'A' 
WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='api.rfms3.auth');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_api_rfms3_auth','api.rfms3.auth',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_api_rfms3_auth');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_auth',(select id from master.menu where name = 'Account Role Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_auth' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_auth',(select id from master.menu where name = 'Package Management'),'M' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_auth' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_rfms3_auth',(select id from master.menu where name = 'Organisation Relationship Management'),'M'
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_rfms3_auth' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));





insert into master.emailtemplate (type,feature_id,event_name,description,created_at,created_by,modified_at,modified_by)
select 'H',null,'AlertNotificationEmail','<!doctype html>
<html>
<head>
</head>
  <body>
    <img src="{0}">
    <h2>[lblDAFConnect]</h2>
    <h3>[lblReportHeading]</h3>
    <p>[lblGreeting] {1},</p>
	<p> {2} </p>
    <p>[lblReportBodyText1]</p>    
    </br>
    <p>[lblWithKindRegards]</p>
    <p><strong>[lblSignatureLine1]</strong></p>
  </body>
</html>',(select extract(epoch from now()) * 1000), (select min(id) from master.account where lower(first_name) like '%atos%'),null,null
where not exists (   SELECT 1   FROM master.emailtemplate   WHERE event_name ='AlertNotificationEmail');

insert into master.emailtemplatelabels (email_template_id,key)
select (select id from master.emailtemplate where event_name='AlertNotificationEmail'),'lblDAFConnect'
where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblDAFConnect' and email_template_id in (select id from master.emailtemplate where event_name='AlertNotificationEmail'));

insert into master.emailtemplatelabels (email_template_id,key)
select (select id from master.emailtemplate where event_name='AlertNotificationEmail'),'lblReportHeading'
where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblReportHeading' and email_template_id in (select id from master.emailtemplate where event_name='AlertNotificationEmail'));

insert into master.emailtemplatelabels (email_template_id,key)
select (select id from master.emailtemplate where event_name='AlertNotificationEmail'),'lblGreeting'
where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblGreeting' and email_template_id in (select id from master.emailtemplate where event_name='AlertNotificationEmail'));

insert into master.emailtemplatelabels (email_template_id,key)
select (select id from master.emailtemplate where event_name='AlertNotificationEmail'),'lblReportBodyText1'
where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblReportBodyText1' and email_template_id in (select id from master.emailtemplate where event_name='AlertNotificationEmail'));

insert into master.emailtemplatelabels (email_template_id,key)
select (select id from master.emailtemplate where event_name='AlertNotificationEmail'),'lblWithKindRegards'
where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblWithKindRegards' and email_template_id in (select id from master.emailtemplate where event_name='AlertNotificationEmail'));

insert into master.emailtemplatelabels (email_template_id,key)
select (select id from master.emailtemplate where event_name='AlertNotificationEmail'),'lblSignatureLine1'
where not exists (   SELECT 1   FROM master.emailtemplatelabels   WHERE key ='lblSignatureLine1' and email_template_id in (select id from master.emailtemplate where event_name='AlertNotificationEmail'));




update master.emailtemplate set description='<!doctype html>
<html>
<head>
<style>
.detailsDiv {{
  border: none;
  background-color: lightblue;    
  text-align: center
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
		<img style="margin:20px 50px" align="left" width="180px" height="80px"  src="{0}">
		<img style="margin:20px 50px" align="right" width="180px" height="80px" src="{1}"><br/><br/><br/><br/>
	
    <h2 style="margin:50px 50px">[lblTripReportDetails]</h2>
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
</html>' where event_name='TripReport';

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Fleet Fuel Report'),
(SELECT id from master.dataattribute WHERE name = 'Report.Driver.VehicleDetails.IdlingWithPTO(%)'),
null,'S','rp_ff_report_driver_vehicledetails_idlingwithpto(%)',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Fleet Fuel Report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Report.Driver.VehicleDetails.IdlingWithPTO(%)' ));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_ff_report_driver_vehicledetails_idlingwithpto(%)','Idling With PTO (%)',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_ff_report_driver_vehicledetails_idlingwithpto(%)');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_ff_report_driver_vehicledetails_idlingwithpto(%)',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_ff_report_driver_vehicledetails_idlingwithpto(%)' 
and ref_id=0);

INSERT INTO master.reportattribute (report_id,data_attribute_id, sub_attribute_ids,type ,key,name) 
SELECT (SELECT id from MASTER.REPORT WHERE name = 'Fleet Fuel Report'),
(SELECT id from master.dataattribute WHERE name = 'Report.Vehicle.VehicleDetails.IdlingWithPTO(%)'),
null,'S','rp_ff_report_vehicle_vehicledetails_idlingwithpto(%)',null
WHERE NOT EXISTS (SELECT 1 FROM master.reportattribute WHERE REPORT_ID = 
				  (SELECT id from MASTER.REPORT WHERE name = 'Fleet Fuel Report') AND
				  DATA_ATTRIBUTE_ID = (SELECT id from master.dataattribute WHERE name = 'Report.Vehicle.VehicleDetails.IdlingWithPTO(%)' ));

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','rp_ff_report_vehicle_vehicledetails_idlingwithpto(%)','Idling With PTO (%)',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'rp_ff_report_vehicle_vehicledetails_idlingwithpto(%)');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'rp_ff_report_vehicle_vehicledetails_idlingwithpto(%)',0,'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'rp_ff_report_vehicle_vehicledetails_idlingwithpto(%)' 
and ref_id=0);



update master.dataattribute set key='da_report_vehicle_vehicledetails_idlingwithpto(%)'
--select * from master.dataattribute
where name='Report.Vehicle.VehicleDetails.IdlingWithPTO(%)';

update master.dataattribute set key='da_report_driver_vehicledetails_idlingwithpto(%)'
--select * from master.dataattribute
where name='Report.Driver.VehicleDetails.IdlingWithPTO(%)';

--select * from translation.translation where name='da_report_vehicle_vehicledetails_idlingwithpto(%)'

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_vehicle_vehicledetails_idlingwithpto(%)','Report.Vehicle.VehicleDetails.IdlingWithPTO(%)',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'da_report_vehicle_vehicledetails_idlingwithpto(%)');

INSERT INTO TRANSLATION.TRANSLATION (CODE,TYPE,NAME,VALUE,created_at)  
SELECT 'EN-GB','L','da_report_driver_vehicledetails_idlingwithpto(%)','Report.Driver.VehicleDetails.IdlingWithPTO(%)',
(select extract(epoch from now()) * 1000)
WHERE NOT EXISTS (SELECT 1 FROM TRANSLATION.TRANSLATION WHERE name = 'da_report_driver_vehicledetails_idlingwithpto(%)');


update translation.translation set value ='Idling Without PTO (hh:mm:ss)' where name='rp_ff_report_driver_vehicledetails_idlingwithoutpto';
update translation.translation set value ='Idling With PTO (hh:mm:ss)' where name='rp_ff_report_driver_vehicledetails_idlingwithpto';
update translation.translation set value ='Idling Without PTO (hh:mm:ss)' where name='rp_ff_report_vehicle_vehicledetails_idlingwithoutpto';
update translation.translation set value ='Idling With PTO (hh:mm:ss)' where name='rp_ff_report_vehicle_vehicledetails_idlingwithpto';


update master.menu 
set name='Fleet Overview',key='lblfleetoverview',url='fleetoverview'
where lower(name)='live fleet';
