using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class ReportSchedulerResult
    {
        public int Repsch_id { get; set; }
        public int Repsch_organization_id { get; set; }
        public int Repsch_report_id { get; set; }
        public string Rep_reportname { get; set; }
        public string Repsch_frequency_type { get; set; }
        public string Repsch_status { get; set; }
        public string Repsch_type { get; set; }
        public string Repsch_file_name { get; set; }
        public long Repsch_start_date { get; set; }
        public long Repsch_end_date { get; set; }
        public string Repsch_code { get; set; }
        public long Repsch_last_schedule_run_date { get; set; }
        public long Repsch_next_schedule_run_date { get; set; }
        public long Repsch_created_at { get; set; }
        public int Repsch_created_by { get; set; }
        public long Repsch_modified_at { get; set; }
        public int Repsch_modified_by { get; set; }
        public string Repsch_mail_subject { get; set; }
        public string Repsch_mail_description { get; set; }
        public long Repsch_report_dispatch_time { get; set; }
        public int Driveref_report_schedule_id { get; set; }
        public int Driveref_driver_id { get; set; }
        public string Dr_driverName { get; set; }
        public string Driveref_state { get; set; }
        public long Driveref_created_at { get; set; }
        public int Driveref_created_by { get; set; }
        public long Driveref_modified_at { get; set; }
        public int Driveref_modified_by { get; set; }
        public int Receipt_id { get; set; }
        public int Receipt_schedule_report_id { get; set; }
        public string Receipt_email { get; set; }
        public string Receipt_state { get; set; }
        public long Receipt_created_at { get; set; }
        public long Receipt_modified_at { get; set; }
        public int Vehref_report_schedule_id { get; set; }
        public int Vehref_vehicle_group_id { get; set; }
        public string Vehref_state { get; set; }
        public long Vehref_created_at { get; set; }
        public int Vehref_created_by { get; set; }
        public long Vehref_modified_at { get; set; }
        public int Vehref_modified_by { get; set; }
        public int Vehicleid { get; set; }
        public string Vin { get; set; }
        public string Regno { get; set; }
        public string Vehiclename { get; set; }
        public string Vehiclegroupname { get; set; }
        public string Vehiclegrouptype { get; set; }
        public string Functionenum { get; set; }
        public int Schrep_id { get; set; }
        public int Schrep_schedule_report_id { get; set; }
        public long Schrep_downloaded_at { get; set; }
        public long Schrep_valid_till { get; set; }
        public long Schrep_created_at { get; set; }
        public long Schrep_start_date { get; set; }
        public long Schrep_end_date { get; set; }
    }

}
