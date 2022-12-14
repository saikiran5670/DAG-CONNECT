GRANT USAGE ON SCHEMA master TO pgrmd_dbcreator_tst1;
GRANT USAGE ON SCHEMA logs TO pgrmd_dbcreator_tst1;
GRANT USAGE ON SCHEMA translation TO pgrmd_dbcreator_tst1;

GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA master TO pgrmd_dbcreator_tst1;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA logs TO pgrmd_dbcreator_tst1;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA translation TO pgrmd_dbcreator_tst1;

GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA master TO pgrmd_dbcreator_tst1;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA logs TO pgrmd_dbcreator_tst1;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA translation TO pgrmd_dbcreator_tst1;

ALTER USER pgrmd_dbcreator_tst1 WITH REPLICATION;
GRANT USAGE ON SCHEMA master TO pgrmd_dbcreator_tst1;
GRANT ALL PRIVILEGES ON TABLE master.vehicle TO pgrmd_dbcreator_tst1;
CREATE PUBLICATION dbz_publication FOR TABLE master.vehicle WITH (publish = 'insert, update'); 

