--For sprint 2 (DB) and sprint 3 (Dev)
--Alert Category
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'C','L',null ,'enumcategory_logisticsalerts' WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumcategory_logisticsalerts');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'C','F',null ,'enumcategory_fuelanddriverperformance' WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumcategory_fuelanddriverperformance');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'C','R',null ,'enumcategory_repairandmaintenance' WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumcategory_repairandmaintenance');

--Alert Type
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'T','N','L' ,'enumtype_enteringzone' WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumcategory_enteringzone');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'T','X','L' ,'enumtype_exitingzone'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumcategory_exitingzone');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'T','C','L' ,'enumtype_exitingcorridor'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumcategory_exitingcorridor');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'T','Y','L' ,'enumtype_excessiveunderutilizationindays(batch)'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumtype_excessiveunderutilizationindays(batch)');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'T','D','L' ,'enumtype_excessivedistancedone(trip)'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumtype_excessivedistancedone(trip)');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'T','U','L' ,'enumtype_excessivedrivingduration(trip)'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumtype_excessivedrivingduration(trip)');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'T','G','L' ,'enumtype_excessiveglobalmileage(trip)'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumtype_excessiveglobalmileage(trip)');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'T','S','L' ,'enumtype_hoursofservice(realtime)'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumtype_hoursofservice(realtime)');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'T','A','F','enumtype_excessiveaveragespeed(realtime)'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumtype_excessiveaveragespeed(realtime)');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'T','H','F' ,'enumtype_excessiveunderutilizationinhours(batch)'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumtype_excessiveunderutilizationinhours(batch)');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'T','I','F' ,'enumtype_excessiveidling(realtime)'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumtype_excessiveidling(realtime)');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'T','F','F' ,'enumtype_fuelconsumed'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumtype_fuelconsumed');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'T','P','F' ,'enumtype_fuelincreaseduringstop(realtime)'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumtype_fuelincreaseduringstop(realtime)');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'T','L','F' ,'enumtype_fuellossduringstop(realtime)'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumtype_fuellossduringstop(realtime)');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'T','T','F' ,'enumtype_fuellossduringtrip(realtime)'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumtype_fuellossduringtrip(realtime)');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'T','O','R' ,'enumtype_statuschangetostopnow'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumtype_statuschangetostopnow');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'T','E','R' ,'enumtype_statuschangetoservicenow'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumtype_statuschangetoservicenow');

--Alert validity period
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'V','A',null ,'enumvalidityperiod_always'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumvalidityperiod_always');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'V','C',null ,'enumvalidityperiod_custom'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumvalidityperiod_custom');

--Alert urgency level
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'U','C',null ,'enumurgencylevel_critical'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumurgencylevel_critical');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'U','W',null ,'enumurgencylevel_warning'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumurgencylevel_warning');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'U','A',null ,'enumurgencylevel_advisory'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumurgencylevel_advisory');

--Alert optional filter type
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'O','T',null ,'enumfiltertype_timerange'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumfiltertype_timerange');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'O','D',null ,'enumfiltertype_duration'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumfiltertype_duration');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'O','N',null ,'enoumfiltertype_noofoccurances'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enoumfiltertype_noofoccurances');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'O','P',null ,'enumfiltertype_poi'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumfiltertype_poi');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'O','T',null ,'enumfiltertype_distance'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumfiltertype_distance');

--alert notification frequency
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'F','O',null ,'enumnotificationfrequency_once'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumnotificationfrequency_once');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'F','T',null ,'enumnotificationfrequency_eachtime'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumnotificationfrequency_eachtime');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'F','E',null ,'enumnotificationfrequency_eachnumberofoccurrence'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumnotificationfrequency_eachnumberofoccurrence');

--alert notification validity
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'A','A',null ,'enumnotificationvalidity_always'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumnotificationvalidity_always');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'A','W',null ,'enumnotificationvalidity_workday'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumnotificationvalidity_workday');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'A','E',null ,'enumnotificationvalidity_weekend'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumnotificationvalidity_weekend');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'A','C',null ,'enumnotificationvalidity_customperiod'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumnotificationvalidity_customperiod');

--alert notification period
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'P','D',null ,'enumnotificationperiod_days'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumnotificationperiod_days');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'P','W',null ,'enumnotificationperiod_weeks'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumnotificationperiod_weeks');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'P','M',null ,'enumnotificationperiod_minutes'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumnotificationperiod_minutes');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'P','Y',null ,'enumnotificationperiod_hours'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumnotificationperiod_hours');

--alert notification mode
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'M','E',null ,'enumnotificationmode_email'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumnotificationmode_email');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'M','S',null ,'enumnotificationmode_sms'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumnotificationmode_sms');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'M','W',null ,'enumnotificationmode_webservice'WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumnotificationmode_webservice');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumcategory_logisticsalerts','Logistics Alerts',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumcategory_logisticsalerts');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumcategory_fuelanddriverperformance','Fuel and Driver Performance',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumcategory_fuelanddriverperformance');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumcategory_repairandmaintenance','Repair and Maintenance',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumcategory_repairandmaintenance');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumtype_enteringzone','Entering Zone',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumtype_enteringzone');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumtype_exitingzone','Exiting Zone',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumtype_exitingzone');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumtype_exitingcorridor','Exiting Corridor',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumtype_exitingcorridor');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumtype_excessiveunderutilizationindays(batch)','Excessive Under Utilization in Days(Batch)',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumtype_excessiveunderutilizationindays(batch)');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumtype_excessivedistancedone(trip)','Excessive Distance Done(Trip)',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumtype_excessivedistancedone(trip)');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumtype_excessivedrivingduration(trip)','Excessive Driving Duration(Trip)',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumtype_excessivedrivingduration(trip)');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumtype_excessiveglobalmileage(trip)','Excessive Global Mileage(Trip)',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumtype_excessiveglobalmileage(trip)');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumtype_hoursofservice(realtime)','Hours of Service(Realtime)',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumtype_hoursofservice(realtime)');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumtype_excessiveaveragespeed(realtime)','Excessive Average Speed(Realtime)',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumtype_excessiveaveragespeed(realtime)');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumtype_excessiveunderutilizationinhours(batch)','Excessive Under Utilization in Hours(Batch)',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumtype_excessiveunderutilizationinhours(batch)');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumtype_excessiveidling(realtime)','Excessive Idling(Realtime)',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumtype_excessiveidling(realtime)');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumtype_fuelconsumed','Fuel Consumed',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumtype_fuelconsumed');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumtype_fuelincreaseduringstop(realtime)','Fuel Increase During Stop(Realtime)',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumtype_fuelincreaseduringstop(realtime)');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumtype_fuellossduringstop(realtime)','Fuel Loss During Stop(Realtime)',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumtype_fuellossduringstop(realtime)');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumtype_fuellossduringtrip(realtime)','Fuel Loss During Trip(Realtime)',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumtype_fuellossduringtrip(realtime)');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumtype_statuschangetostopnow','Status Change to Stop Now',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumtype_statuschangetostopnow');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumtype_statuschangetoservicenow','Status Change to Service Now',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumtype_statuschangetoservicenow');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumvalidityperiod_always','Always',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumvalidityperiod_always');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumvalidityperiod_custom','Custom',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumvalidityperiod_custom');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumurgencylevel_critical','Critical',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumurgencylevel_critical');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumurgencylevel_warning','Warning',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumurgencylevel_warning');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumurgencylevel_advisory','Advisory',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumurgencylevel_advisory');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumfiltertype_timerange','TimeRange',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumfiltertype_timerange');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumfiltertype_duration','Duration',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumfiltertype_duration');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enoumfiltertype_noofoccurances','No of Occurances',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enoumfiltertype_noofoccurances');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumfiltertype_poi','POI',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumfiltertype_poi');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumfiltertype_distance','Distance',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumfiltertype_distance');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumnotificationfrequency_once','Once',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumnotificationfrequency_once');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumnotificationfrequency_eachtime','Each Time',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumnotificationfrequency_eachtime');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumnotificationfrequency_eachnumberofoccurrence','Each Number of Occurrence',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumnotificationfrequency_eachnumberofoccurrence');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumnotificationvalidity_always','Always',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumnotificationvalidity_always');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumnotificationvalidity_workday','Workday',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumnotificationvalidity_workday');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumnotificationvalidity_weekend','Weekend',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumnotificationvalidity_weekend');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumnotificationvalidity_customperiod','Custom Period',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumnotificationvalidity_customperiod');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumnotificationperiod_days','Days',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumnotificationperiod_days');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumnotificationperiod_weeks','Weeks',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumnotificationperiod_weeks');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumnotificationperiod_minutes','Minutes',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumnotificationperiod_minutes');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumnotificationperiod_hours','Hours',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumnotificationperiod_hours');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumnotificationmode_email','Email',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumnotificationmode_email');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumnotificationmode_sms','SMS',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumnotificationmode_sms');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumnotificationmode_webservice','Web Service',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumnotificationmode_webservice');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumcategory_logisticsalerts',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumcategory_logisticsalerts' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumcategory_fuelanddriverperformance',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumcategory_fuelanddriverperformance' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumcategory_repairandmaintenance',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumcategory_repairandmaintenance' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumtype_enteringzone',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumtype_enteringzone' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumtype_exitingzone',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumtype_exitingzone' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumtype_exitingcorridor',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumtype_exitingcorridor' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumtype_excessiveunderutilizationindays(batch)',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumtype_excessiveunderutilizationindays(batch)' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumtype_excessivedistancedone(trip)',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumtype_excessivedistancedone(trip)' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumtype_excessivedrivingduration(trip)',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumtype_excessivedrivingduration(trip)' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumtype_excessiveglobalmileage(trip)',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumtype_excessiveglobalmileage(trip)' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumtype_hoursofservice(realtime)',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumtype_hoursofservice(realtime)' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumtype_excessiveaveragespeed(realtime)',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumtype_excessiveaveragespeed(realtime)' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumtype_excessiveunderutilizationinhours(batch)',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumtype_excessiveunderutilizationinhours(batch)' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumtype_excessiveidling(realtime)',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumtype_excessiveidling(realtime)' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumtype_fuelconsumed',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumtype_fuelconsumed' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumtype_fuelincreaseduringstop(realtime)',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumtype_fuelincreaseduringstop(realtime)' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumtype_fuellossduringstop(realtime)',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumtype_fuellossduringstop(realtime)' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumtype_fuellossduringtrip(realtime)',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumtype_fuellossduringtrip(realtime)' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumtype_statuschangetostopnow',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumtype_statuschangetostopnow' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumtype_statuschangetoservicenow',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumtype_statuschangetoservicenow' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvalidityperiod_always',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumvalidityperiod_always' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumvalidityperiod_custom',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumvalidityperiod_custom' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumurgencylevel_critical',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumurgencylevel_critical' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumurgencylevel_warning',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumurgencylevel_warning' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumurgencylevel_advisory',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumurgencylevel_advisory' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumfiltertype_timerange',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumfiltertype_timerange' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumfiltertype_duration',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumfiltertype_duration' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enoumfiltertype_noofoccurances',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enoumfiltertype_noofoccurances' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumfiltertype_poi',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumfiltertype_poi' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumfiltertype_distance',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumfiltertype_distance' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumnotificationfrequency_once',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumnotificationfrequency_once' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumnotificationfrequency_eachtime',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumnotificationfrequency_eachtime' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumnotificationfrequency_eachnumberofoccurrence',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumnotificationfrequency_eachnumberofoccurrence' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumnotificationvalidity_always',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumnotificationvalidity_always' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumnotificationvalidity_workday',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumnotificationvalidity_workday' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumnotificationvalidity_weekend',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumnotificationvalidity_weekend' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumnotificationvalidity_customperiod',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumnotificationvalidity_customperiod' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumnotificationperiod_days',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumnotificationperiod_days' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumnotificationperiod_weeks',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumnotificationperiod_weeks' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumnotificationperiod_minutes',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumnotificationperiod_minutes' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumnotificationperiod_hours',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumnotificationperiod_hours' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumnotificationmode_email',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumnotificationmode_email' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumnotificationmode_sms',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumnotificationmode_sms' and ref_id=(select id from master.menu where name = 'Alerts'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumnotificationmode_webservice',(select id from master.menu where name = 'Alerts'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumnotificationmode_webservice' and ref_id=(select id from master.menu where name = 'Alerts'));

INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
							 key, level,state)   
SELECT 807,'api.vehicle-namelist','F',null,'feat_api_vehicle-namelist',30,'A' WHERE NOT EXISTS  
(   SELECT 1   FROM master.feature   WHERE name ='api.vehicle-namelist');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_api_vehicle-namelist','api.vehicle-namelist',
(select extract(epoch from now()) * 1000),null WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   
WHERE code = 'EN-GB' and name = 'feat_api_vehicle-namelist');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_vehicle-namelist',(select id from master.menu where name = 'Account Role Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_vehicle-namelist' 
and ref_id=(select id from master.menu where name = 'Account Role Management'));


INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_vehicle-namelist',(select id from master.menu where name = 'Package Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_vehicle-namelist' 
and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_vehicle-namelist',(select id from master.menu where name = 'Organisation Relationship Management'),'L' 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_vehicle-namelist' 
and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));

INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'R','T',null ,'enumcorridortype_existingtrip' WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumcorridortype_existingtrip');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'R','R',null ,'enumcorridortype_routecalculating' WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumcorridortype_routecalculating');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'E','T',null ,'enumcorridorexclusion_strictexclude' WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumcorridorexclusion_strictexclude');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'E','O',null ,'enumcorridorexclusion_softexclude' WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumcorridorexclusion_softexclude');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'E','A',null ,'enumcorridorexclusion_avoid' WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumcorridorexclusion_avoid');
INSERT INTO translation.enumtranslation  (type ,enum, parent_enum, key)   
SELECT 'E','D',null ,'enumcorridorexclusion_default' WHERE NOT EXISTS  (   SELECT 1   FROM translation.enumtranslation   WHERE key ='enumcorridorexclusion_default');


INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumcorridortype_existingtrip','Existing Trip',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumcorridortype_existingtrip');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumcorridortype_routecalculating','Route Calculating',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumcorridortype_routecalculating');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumcorridorexclusion_strictexclude','Strict Exclude',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumcorridorexclusion_strictexclude');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumcorridorexclusion_softexclude','Soft Exclude',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumcorridorexclusion_softexclude');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumcorridorexclusion_avoid','Avoid',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumcorridorexclusion_avoid');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  
SELECT 'EN-GB','D','enumcorridorexclusion_default','Default',(select extract(epoch from now()) * 1000),null 
WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'enumcorridorexclusion_default');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumcorridortype_existingtrip',(select id from master.menu where name = 'Landmarks'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumcorridortype_existingtrip' and ref_id=(select id from master.menu where name = 'Landmarks'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumcorridorexclusion_default',(select id from master.menu where name = 'Landmarks'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumcorridorexclusion_default' and ref_id=(select id from master.menu where name = 'Landmarks'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumcorridorexclusion_avoid',(select id from master.menu where name = 'Landmarks'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumcorridorexclusion_avoid' and ref_id=(select id from master.menu where name = 'Landmarks'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumcorridorexclusion_softexclude',(select id from master.menu where name = 'Landmarks'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumcorridorexclusion_softexclude' and ref_id=(select id from master.menu where name = 'Landmarks'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumcorridorexclusion_strictexclude',(select id from master.menu where name = 'Landmarks'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumcorridorexclusion_strictexclude' and ref_id=(select id from master.menu where name = 'Landmarks'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'enumcorridortype_routecalculating',(select id from master.menu where name = 'Landmarks'),'D' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'enumcorridortype_routecalculating' and ref_id=(select id from master.menu where name = 'Landmarks'));


