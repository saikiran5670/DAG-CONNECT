--alter table tripdetail.trip_statistics alter column average_traffic_classification type decimal;
--alter table tripdetail.trip_statistics alter column idling_consumption type decimal;
alter table tripdetail.trip_statistics add column if not exists msg_gross_weight_combinition int; 
alter table tripdetail.trip_statistics rename column message_inserted_into_hbase_timestamp to etl_trip_record_insertion_time ;
alter table tripdetail.trip_statistics add column if not exists no_of_total_index_message int; 
alter table tripdetail.trip_statistics alter column average_traffic_classification type decimal USING average_traffic_classification::numeric; 

alter table livefleet.livefleet_trip_driver_activity add column if not exists logical_code char(1) ; 
