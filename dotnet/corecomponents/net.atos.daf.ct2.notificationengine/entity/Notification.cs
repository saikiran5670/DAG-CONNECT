using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.notificationengine.entity
{
    public class Notification
    {
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
        public int Ale_organization_id { get; set; }
        public string Ale_name { get; set; }
        public string Vehicle_group_vehicle_name { get; set; }
    }

}
