using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.alert.entity
{
    public class AlertResult
    {
        public int ale_id { get; set; }
        public int ale_organization_id { get; set; }
        public string ale_name { get; set; }
        public string ale_category { get; set; }
        public string ale_type { get; set; }
        public string ale_validity_period_type { get; set; }
        public long ale_validity_start_date { get; set; }
        public long ale_validity_end_date { get; set; }
        public int ale_vehicle_group_id { get; set; }
        public string ale_state { get; set; }
        public long ale_created_at { get; set; }
        public int ale_created_by { get; set; }
        public long ale_modified_at { get; set; }
        public int ale_modified_by { get; set; }
        public int aleurg_id { get; set; }
        public int aleurg_alert_id { get; set; }
        public string aleurg_urgency_level_type { get; set; }
        public int aleurg_threshold_value { get; set; }
        public string aleurg_unit_type { get; set; }
        public BitArray aleurg_day_type { get; set; }
        public string aleurg_period_type { get; set; }
        public int aleurg_urgencylevel_start_date { get; set; }
        public int aleurg_urgencylevel_end_date { get; set; }
        public string aleurg_state { get; set; }
        public long aleurg_created_at { get; set; }
        public long aleurg_modified_at { get; set; }
        public int alefil_id { get; set; }
        public int alefil_alert_id { get; set; }
        public int alefil_alert_urgency_level_id { get; set; }
        public string alefil_filter_type { get; set; }
        public int alefil_threshold_value { get; set; }
        public string alefil_unit_type { get; set; }
        public string alefil_landmark_type { get; set; }
        public int alefil_ref_id { get; set; }
        public string alefil_position_type { get; set; }
        public BitArray alefil_day_type { get; set; }
        public string alefil_period_type { get; set; }
        public int alefil_filter_start_date { get; set; }
        public int alefil_filter_end_date { get; set; }
        public string alefil_state { get; set; }
        public long alefil_created_at { get; set; }
        public long alefil_modified_at { get; set; }
        public int alelan_id { get; set; }
        public int alelan_alert_id { get; set; }
        public string alelan_landmark_type { get; set; }
        public int alelan_ref_id { get; set; }
        public int alelan_distance { get; set; }
        public string alelan_unit_type { get; set; }
        public string alelan_state { get; set; }
        public long alelan_created_at { get; set; }
        public long alelan_modified_at { get; set; }
        public int noti_id { get; set; }
        public int noti_alert_id { get; set; }
        public string noti_alert_urgency_level_type { get; set; }
        public string noti_frequency_type { get; set; }
        public int noti_frequency_threshhold_value { get; set; }
        public string noti_validity_type { get; set; }
        public string noti_state { get; set; }
        public long noti_created_at { get; set; }
        public int noti_created_by { get; set; }
        public long noti_modified_at { get; set; }
        public int noti_modified_by { get; set; }
        public int notrec_id { get; set; }
        public int notrec_notification_id { get; set; }
        public string notrec_recipient_label { get; set; }
        public int notrec_account_group_id { get; set; }
        public string notrec_notification_mode_type { get; set; }
        public string notrec_phone_no { get; set; }
        public string notrec_sms { get; set; }
        public string notrec_email_id { get; set; }
        public string notrec_email_sub { get; set; }
        public string notrec_email_text { get; set; }
        public string notrec_ws_url { get; set; }
        public string notrec_ws_type { get; set; }
        public string notrec_ws_text { get; set; }
        public string notrec_ws_login { get; set; }
        public string notrec_ws_password { get; set; }
        public string notrec_state { get; set; }
        public long notrec_created_at { get; set; }
        public long notrec_modified_at { get; set; }
        public int notlim_id { get; set; }
        public int notlim_notification_id { get; set; }
        public string notlim_notification_mode_type { get; set; }
        public int notlim_max_limit { get; set; }
        public string notlim_notification_period_type { get; set; }
        public int notlim_period_limit { get; set; }
        public string notlim_state { get; set; }
        public long notlim_created_at { get; set; }
        public long notlim_modified_at { get; set; }
        public int notava_id { get; set; }
        public int notava_notification_id { get; set; }
        public string notava_availability_period_type { get; set; }
        public string notava_period_type { get; set; }
        public int notava_start_time { get; set; }
        public int notava_end_time { get; set; }
        public string notava_state { get; set; }
        public long notava_created_at { get; set; }
        public long notava_modified_at { get; set; }
        public string vehiclename { get; set; }
        public string vehiclegroupname { get; set; }
    }
}
