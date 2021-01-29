ALTER USER pgrdm_dbcreator_dev2 WITH REPLICATION;
GRANT USAGE ON SCHEMA MASTER TO pgrdm_dbcreator_dev2;
GRANT ALL PRIVILEGES ON TABLE master.vehicle TO pgrdm_dbcreator_dev2;
CREATE PUBLICATION dbz_publication FOR TABLE master.vehicle WITH (publish = 'insert, update'); 

