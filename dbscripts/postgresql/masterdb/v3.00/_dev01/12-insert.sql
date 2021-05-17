
--For sprint 1 (DB) and sprint 2 (Dev)
INSERT INTO master.feature  (id, name , type ,  data_attribute_set_id , key, level,state)   SELECT 458,'Configuration.Landmarks.GlobalPoi','F',null,'feat_configuration_landmarks_globalpoi',20,'A' WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='Configuration.Landmarks.GlobalPoi');
INSERT INTO master.feature  (id, name , type ,  data_attribute_set_id , key, level,state)   SELECT 459,'Configuration.Landmarks.GlobalCategory','F',null,'feat_configuration_landmarks_globalcategory',20,'A' WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='Configuration.Landmarks.GlobalCategory');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  SELECT 'EN-GB','L','feat_configuration_landmarks_globalpoi','Configuration.Landmarks.GlobalPoi',(select extract(epoch from now()) * 1000),null WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_configuration_landmarks_globalpoi');
INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)  SELECT 'EN-GB','L','feat_configuration_landmarks_globalcategory','Configuration.Landmarks.GlobalCategory',(select extract(epoch from now()) * 1000),null WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_configuration_landmarks_globalcategory');
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   SELECT 'feat_configuration_landmarks_globalpoi',(select id from master.menu where name = 'Account Role Management'),'L' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_configuration_landmarks_globalpoi' and ref_id=(select id from master.menu where name = 'Account Role Management'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   SELECT 'feat_configuration_landmarks_globalcategory',(select id from master.menu where name = 'Account Role Management'),'L' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_configuration_landmarks_globalcategory' and ref_id=(select id from master.menu where name = 'Account Role Management'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   SELECT 'feat_configuration_landmarks_globalpoi',(select id from master.menu where name = 'Package Management'),'L' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_configuration_landmarks_globalpoi' and ref_id=(select id from master.menu where name = 'Package Management'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   SELECT 'feat_configuration_landmarks_globalcategory',(select id from master.menu where name = 'Package Management'),'L' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_configuration_landmarks_globalcategory' and ref_id=(select id from master.menu where name = 'Package Management'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   SELECT 'feat_configuration_landmarks_globalpoi',(select id from master.menu where name = 'Organisation Relationship Management'),'L' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_configuration_landmarks_globalpoi' and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));
INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   SELECT 'feat_configuration_landmarks_globalcategory',(select id from master.menu where name = 'Organisation Relationship Management'),'L' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_configuration_landmarks_globalcategory' and ref_id=(select id from master.menu where name = 'Organisation Relationship Management'));

INSERT INTO master.feature  (id, name , type  , data_attribute_set_id ,
							 key, level,state)   
SELECT 806,'api.vehicle-mileage','F',null,'feat_api_vehicle-mileage',30,'A' WHERE NOT EXISTS  (   SELECT 1   FROM master.feature   WHERE name ='api.vehicle-mileage');

INSERT INTO translation.translation  (code ,    type ,  name ,  value ,  created_at ,  modified_at)
SELECT 'EN-GB','L','feat_api_vehicle-mileage','api.vehicle-mileage',
(select extract(epoch from now()) * 1000),null WHERE NOT EXISTS  (   SELECT 1   FROM translation.translation   WHERE code = 'EN-GB' and name = 'feat_api_vehicle-mileage');

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_vehicle-mileage',(select id from master.menu where name = 'Account Role Management'),'L' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_vehicle-mileage' and ref_id=(select id from master.menu where name = 'Account Role Management'));


INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_vehicle-mileage',(select id from master.menu where name = 'Package Management'),'L' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_vehicle-mileage' and ref_id=(select id from master.menu where name = 'Package Management'));

INSERT INTO translation.translationgrouping  (name ,       ref_id ,      type)   
SELECT 'feat_api_vehicle-mileage',(select id from master.menu where name = 'Organisation Relationship Management'),'L' WHERE NOT EXISTS  (   SELECT 1   FROM translation.translationgrouping  WHERE name = 'feat_api_vehicle-mileage' and ref_id=(select id from master.menu where name = 'Orgnisation Relationship Management'));

