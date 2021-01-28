ALTER USER pgrdm_dbcreator_tst1 WITH REPLICATION;
GRANT USAGE ON SCHEMA MASTER TO pgrdm_dbcreator_tst1;
GRANT ALL PRIVILEGES ON TABLE master.vehicle TO pgrdm_dbcreator_tst1;
CREATE PUBLICATION dbz_publication FOR TABLE master.vehicle WITH (publish = 'insert, update'); 

