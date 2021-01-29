ALTER USER pgrdm_dbcreator_tst1 WITH REPLICATION;
GRANT USAGE ON SCHEMA master TO pgrdm_dbcreator_tst1;
GRANT ALL PRIVILEGES ON TABLE master.vehicle TO pgrdm_dbcreator_tst1;
CREATE PUBLICATION dbz_publication FOR TABLE master.vehicle WITH (publish = 'insert, update'); 

GRANT USAGE ON SCHEMA master TO pgrdm_dbcreator_tst1;
GRANT USAGE ON SCHEMA tripdetail TO pgrdm_dbcreator_tst1;
GRANT USAGE ON SCHEMA livefleet TO pgrdm_dbcreator_tst1;

GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA master TO pgrdm_dbcreator_tst1;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA tripdetail TO pgrdm_dbcreator_tst1;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA livefleet TO pgrdm_dbcreator_tst1;

GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA master TO pgrdm_dbcreator_tst1;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA tripdetail TO pgrdm_dbcreator_tst1;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA livefleet TO pgrdm_dbcreator_tst1;
