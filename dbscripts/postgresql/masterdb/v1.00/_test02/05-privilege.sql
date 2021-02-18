GRANT USAGE ON SCHEMA master TO pgrmd_dbcreator_tst2;
GRANT USAGE ON SCHEMA logs TO pgrmd_dbcreator_tst2;
GRANT USAGE ON SCHEMA translation TO pgrmd_dbcreator_tst2;

GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA master TO pgrmd_dbcreator_tst2;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA logs TO pgrmd_dbcreator_tst2;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA translation TO pgrmd_dbcreator_tst2;

GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA master TO pgrmd_dbcreator_tst2;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA logs TO pgrmd_dbcreator_tst2;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA translation TO pgrmd_dbcreator_tst2;


