Alter table livefleet.livefleet_current_trip_statistics add column if not exists fuel_consumption  decimal ;	
ALTER TABLE livefleet.livefleet_position_statistics ALTER COLUMN co2_emission TYPE numeric;
