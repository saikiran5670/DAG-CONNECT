﻿using System.Collections;

namespace net.atos.daf.ct2.alert.entity
{
    public class AlertResult
    {
        public int Ale_id { get; set; }
        public int Ale_organization_id { get; set; }
        public string Ale_name { get; set; }
        public string Ale_category { get; set; }
        public string Ale_type { get; set; }
        public string Ale_validity_period_type { get; set; }
        public long Ale_validity_start_date { get; set; }
        public long Ale_validity_end_date { get; set; }
        public int Ale_vehicle_group_id { get; set; }
        public string Ale_state { get; set; }
        public long Ale_created_at { get; set; }
        public int Ale_created_by { get; set; }
        public long Ale_modified_at { get; set; }
        public int Ale_modified_by { get; set; }
        public string Ale_applyon { get; set; }
        public int Aleurg_id { get; set; }
        public int Aleurg_alert_id { get; set; }
        public string Aleurg_urgency_level_type { get; set; }
        public int Aleurg_threshold_value { get; set; }
        public string Aleurg_unit_type { get; set; }
        public BitArray Aleurg_day_type { get; set; }
        public string Aleurg_period_type { get; set; }
        public long Aleurg_urgencylevel_start_date { get; set; }
        public long Aleurg_urgencylevel_end_date { get; set; }
        public string Aleurg_state { get; set; }
        public long Aleurg_created_at { get; set; }
        public long Aleurg_modified_at { get; set; }
        public int Aletimeurg_id { get; set; }
        public string Aletimeurg_type { get; set; }
        public int Aletimeurg_ref_id { get; set; }
        public BitArray Aletimeurg_day_type { get; set; }
        public string Aletimeurg_period_type { get; set; }
        public long Aletimeurg_start_date { get; set; }
        public long Aletimeurg_end_date { get; set; }
        public string Aletimeurg_state { get; set; }
        public long Aletimeurg_created_at { get; set; }
        public long Aletimeurg_modified_at { get; set; }
        public int Alefil_id { get; set; }
        public int Alefil_alert_id { get; set; }
        public int Alefil_alert_urgency_level_id { get; set; }
        public string Alefil_filter_type { get; set; }
        public int Alefil_threshold_value { get; set; }
        public string Alefil_unit_type { get; set; }
        public string Alefil_landmark_type { get; set; }
        public int Alefil_ref_id { get; set; }
        public string Alefil_position_type { get; set; }
        public BitArray Alefil_day_type { get; set; }
        public string Alefil_period_type { get; set; }
        public long Alefil_filter_start_date { get; set; }
        public long Alefil_filter_end_date { get; set; }
        public string Alefil_state { get; set; }
        public long Alefil_created_at { get; set; }
        public long Alefil_modified_at { get; set; }
        public int Aletimefil_id { get; set; }
        public string Aletimefil_type { get; set; }
        public int Aletimefil_ref_id { get; set; }
        public BitArray Aletimefil_day_type { get; set; }
        public string Aletimefil_period_type { get; set; }
        public long Aletimefil_start_date { get; set; }
        public long Aletimefil_end_date { get; set; }
        public string Aletimefil_state { get; set; }
        public long Aletimefil_created_at { get; set; }
        public long Aletimefil_modified_at { get; set; }
        public int Alelan_id { get; set; }
        public int Alelan_alert_id { get; set; }
        public string Alelan_landmark_type { get; set; }
        public int Alelan_ref_id { get; set; }
        public int Alelan_distance { get; set; }
        public string Alelan_unit_type { get; set; }
        public string Alelan_state { get; set; }
        public long Alelan_created_at { get; set; }
        public long Alelan_modified_at { get; set; }
        public int Noti_id { get; set; }
        public int Noti_alert_id { get; set; }
        public string Noti_alert_urgency_level_type { get; set; }
        public string Noti_frequency_type { get; set; }
        public int Noti_frequency_threshhold_value { get; set; }
        public string Noti_validity_type { get; set; }
        public string Noti_state { get; set; }
        public long Noti_created_at { get; set; }
        public int Noti_created_by { get; set; }
        public long Noti_modified_at { get; set; }
        public int Noti_modified_by { get; set; }
        public int Aletimenoti_id { get; set; }
        public int Aletimenoti_ref_id { get; set; }
        public string Aletimenoti_type { get; set; }
        public BitArray Aletimenoti_day_type { get; set; }
        public string Aletimenoti_period_type { get; set; }
        public long Aletimenoti_start_date { get; set; }
        public long Aletimenoti_end_date { get; set; }
        public string Aletimenoti_state { get; set; }
        public long Aletimenoti_created_at { get; set; }
        public long Aletimenoti_modified_at { get; set; }
        public int Notrec_id { get; set; }
        public int Notrec_notification_id { get; set; }
        public string Notrec_recipient_label { get; set; }
        public int Notrec_account_group_id { get; set; }
        public string Notrec_notification_mode_type { get; set; }
        public string Notrec_phone_no { get; set; }
        public string Notrec_sms { get; set; }
        public string Notrec_email_id { get; set; }
        public string Notrec_email_sub { get; set; }
        public string Notrec_email_text { get; set; }
        public string Notrec_ws_url { get; set; }
        public string Notrec_ws_type { get; set; }
        public string Notrec_ws_text { get; set; }
        public string Notrec_ws_login { get; set; }
        public string Notrec_ws_password { get; set; }
        public string Notrec_state { get; set; }
        public long Notrec_created_at { get; set; }
        public long Notrec_modified_at { get; set; }
        public int Notlim_id { get; set; }
        public int Notlim_notification_id { get; set; }
        public string Notlim_notification_mode_type { get; set; }
        public int Notlim_max_limit { get; set; }
        public string Notlim_notification_period_type { get; set; }
        public int Notlim_period_limit { get; set; }
        public string Notlim_state { get; set; }
        public long Notlim_created_at { get; set; }
        public long Notlim_modified_at { get; set; }
        public int Notava_id { get; set; }
        public int Notava_notification_id { get; set; }
        public string Notava_availability_period_type { get; set; }
        public string Notava_period_type { get; set; }
        public int Notava_start_time { get; set; }
        public int Notava_end_time { get; set; }
        public string Notava_state { get; set; }
        public long Notava_created_at { get; set; }
        public long Notava_modified_at { get; set; }
        public string Vehiclename { get; set; }
        public string Vehiclegroupname { get; set; }
        public string Vin { get; set; }
        public string Regno { get; set; }
        public int Notlim_recipient_id { get; set; }
        public int Notref_id { get; set; }
        public int Notref_alert_id { get; set; }
        public int Notref_notification_id { get; set; }
        public int Notref_recipient_id { get; set; }
        public string Notref_state { get; set; }
        public long Notref_created_at { get; set; }
        public long Notref_modified_at { get; set; }
    }
}
