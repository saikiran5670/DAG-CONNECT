using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.alert.entity
{
    public class MapperRepo
    {
        public IEnumerable<Alert> GetAlertList(IEnumerable<AlertResult> alertResult)
        {
            List<Alert> alertList = new List<Alert>();

            //Lookups are implemeted to avoid inserting duplicate entry of same id into the list
            Dictionary<int, Alert> alertLookup = new Dictionary<int, Alert>();
            Dictionary<int, Notification> notificationLookup = new Dictionary<int, Notification>();

            Dictionary<int, AlertUrgencyLevelRef> alertUrgencyLevelRefLookup = new Dictionary<int, AlertUrgencyLevelRef>();
            Dictionary<int, AlertFilterRef> alertFilterRefLookup = new Dictionary<int, AlertFilterRef>();
            Dictionary<int, AlertLandmarkRef> alertLandmarkRefLookup = new Dictionary<int, AlertLandmarkRef>();

            Dictionary<int, NotificationRecipient> notificationRecipientRefLookup = new Dictionary<int, NotificationRecipient>();
            Dictionary<int, NotificationLimit> notificationLimitkRefLookup = new Dictionary<int, NotificationLimit>();
            Dictionary<int, NotificationAvailabilityPeriod> notificationAvailabilityPeriodLookup = new Dictionary<int, NotificationAvailabilityPeriod>();

            Alert alert = new Alert();
            Notification notification = new Notification();
            AlertUrgencyLevelRef alertUrgencyLevelRef = new AlertUrgencyLevelRef();
            AlertFilterRef alertFilterRef = new AlertFilterRef();
            AlertLandmarkRef alertLandmarkRef = new AlertLandmarkRef();
            NotificationRecipient notificationRecipient = new NotificationRecipient();
            NotificationLimit notificationLimit = new NotificationLimit();
            NotificationAvailabilityPeriod notificationAvailabilityPeriod = new NotificationAvailabilityPeriod();

            foreach (var alertItem in alertResult)
            {
                if (!alertLookup.TryGetValue(Convert.ToInt32(alertItem.ale_id), out alert))
                    alertLookup.Add(Convert.ToInt32(alertItem.ale_id), alert = ToAlertModel(alertItem));

                if (alert.Notifications == null)
                    alert.Notifications = new List<Notification>();
                if (alert.AlertUrgencyLevelRefs == null)
                    alert.AlertUrgencyLevelRefs = new List<AlertUrgencyLevelRef>();
                if (alert.AlertLandmarkRefs == null)
                    alert.AlertLandmarkRefs = new List<AlertLandmarkRef>();

                if (alertItem.aleurg_id > 0 && alertItem.ale_id == alertItem.aleurg_alert_id)
                {
                    if (!alertUrgencyLevelRefLookup.TryGetValue(Convert.ToInt32(alertItem.aleurg_id), out alertUrgencyLevelRef))
                    {
                        alertUrgencyLevelRefLookup.Add(Convert.ToInt32(alertItem.aleurg_id), alertUrgencyLevelRef = ToAlertUrgencyLevelRefModel(alertItem));

                        if (alertItem.alefil_id > 0 && alertItem.aleurg_id == alertItem.alefil_alert_urgency_level_id)
                        {
                            if (!alertFilterRefLookup.TryGetValue(Convert.ToInt32(alertItem.alefil_id), out alertFilterRef))
                            {
                                alertFilterRefLookup.Add(Convert.ToInt32(alertItem.alefil_id), alertFilterRef = ToAlertFilterRefModel(alertItem));
                                alertUrgencyLevelRef.AlertFilterRefs.Add(alertFilterRef);
                            }
                        }
                        alert.AlertUrgencyLevelRefs.Add(alertUrgencyLevelRef);
                    }
                }
                if (alertItem.alelan_id > 0 && alertItem.ale_id == alertItem.alelan_alert_id)
                {
                    if (!alertLandmarkRefLookup.TryGetValue(Convert.ToInt32(alertItem.alelan_id), out alertLandmarkRef))
                    {
                        alertLandmarkRefLookup.Add(Convert.ToInt32(alertItem.alelan_id), alertLandmarkRef = ToAlertLandmarkRefModel(alertItem));
                        alert.AlertLandmarkRefs.Add(alertLandmarkRef);
                    }
                }
                if (alertItem.noti_id > 0 && alertItem.ale_id == alertItem.noti_alert_id)
                {
                    if (!notificationLookup.TryGetValue(Convert.ToInt32(alertItem.noti_id), out notification))
                    {
                        notificationLookup.Add(Convert.ToInt32(alertItem.noti_id), notification = ToNotificationModel(alertItem));
                        if (alertItem.notava_id > 0 && alertItem.notava_notification_id == alertItem.noti_id)
                        {
                            if (!notificationAvailabilityPeriodLookup.TryGetValue(Convert.ToInt32(alertItem.notava_id), out notificationAvailabilityPeriod))
                            {
                                notificationAvailabilityPeriodLookup.Add(Convert.ToInt32(alertItem.notava_id), notificationAvailabilityPeriod = ToNotificationAvailabilityPeriodModel(alertItem));
                                notification.NotificationAvailabilityPeriods.Add(notificationAvailabilityPeriod);
                            }
                        }
                        if (alertItem.notlim_id > 0 && alertItem.notava_notification_id == alertItem.noti_id)
                        {
                            if (!notificationLimitkRefLookup.TryGetValue(Convert.ToInt32(alertItem.notlim_id), out notificationLimit))
                            {
                                notificationLimitkRefLookup.Add(Convert.ToInt32(alertItem.notlim_id), notificationLimit = ToNotificationLimitModel(alertItem));
                                notification.NotificationLimits.Add(notificationLimit);
                            }
                        }
                        if (alertItem.notrec_id > 0 && alertItem.notava_notification_id == alertItem.noti_id)
                        {
                            if (!notificationRecipientRefLookup.TryGetValue(Convert.ToInt32(alertItem.notrec_id), out notificationRecipient))
                            {
                                notificationRecipientRefLookup.Add(Convert.ToInt32(alertItem.notrec_id), notificationRecipient = ToNotificationRecipientModel(alertItem));
                                notification.NotificationRecipients.Add(notificationRecipient);
                            }
                        }
                        alert.Notifications.Add(notification);
                    }
                }
            }
            foreach (var keyValuePair in alertLookup)
            {
                //add alert object along with child tables to alert list 
                alertList.Add(keyValuePair.Value);
            }
            return alertList;
        }
        public Alert ToAlertModel(AlertResult request)
        {
            Alert alert = new Alert();
            alert.Id = request.ale_id;
            alert.OrganizationId = request.ale_organization_id;
            alert.Name = request.ale_name;
            alert.Category = request.ale_category;
            alert.Type = request.ale_type;
            alert.ValidityPeriodType = request.ale_validity_period_type;
            alert.ValidityStartDate = request.ale_validity_start_date;
            alert.ValidityEndDate = request.ale_validity_end_date;
            alert.VehicleGroupId = request.ale_vehicle_group_id;
            alert.State = request.ale_state;
            alert.CreatedAt = request.alefil_created_at;
            alert.CreatedBy = request.ale_created_by;
            alert.ModifiedAt = request.alefil_modified_at;
            alert.ModifiedBy = request.ale_modified_by;
            alert.ApplyOn = request.ale_applyon;
            alert.VehicleName = request.vehiclename;
            alert.VehicleGroupName = request.vehiclegroupname;
            alert.AlertUrgencyLevelRefs = new List<AlertUrgencyLevelRef>();
            alert.Notifications = new List<Notification>();
            alert.AlertLandmarkRefs = new List<AlertLandmarkRef>();
            return alert;
        }
        public AlertUrgencyLevelRef ToAlertUrgencyLevelRefModel(AlertResult request)
        {
            AlertUrgencyLevelRef alertUrgencyLevelRef = new AlertUrgencyLevelRef();
            alertUrgencyLevelRef.Id = request.aleurg_id;
            alertUrgencyLevelRef.AlertId = request.aleurg_alert_id;
            alertUrgencyLevelRef.UrgencyLevelType = request.aleurg_urgency_level_type;
            alertUrgencyLevelRef.ThresholdValue = request.aleurg_threshold_value;
            alertUrgencyLevelRef.UnitType = request.aleurg_unit_type;
            if(request.aleurg_day_type!=null)
            { 
                for (int i = 0; i < request.aleurg_day_type.Length; i++)
                {
                    alertUrgencyLevelRef.DayType[i]= request.aleurg_day_type.Get(i);
                }
            }
            alertUrgencyLevelRef.PeriodType = request.aleurg_period_type;
            alertUrgencyLevelRef.UrgencylevelStartDate = request.aleurg_urgencylevel_start_date;
            alertUrgencyLevelRef.UrgencylevelEndDate = request.aleurg_urgencylevel_end_date;
            alertUrgencyLevelRef.State = request.aleurg_state;
            alertUrgencyLevelRef.CreatedAt = request.aleurg_created_at;
            alertUrgencyLevelRef.ModifiedAt = request.aleurg_modified_at;
            alertUrgencyLevelRef.AlertFilterRefs = new List<AlertFilterRef>();
            //if (request.aleurg_AlertFilterRefs.Count > 0)
            //{
            //    foreach (var item in request.aleurg_AlertFilterRefs)
            //    {
            //        alertUrgencyLevelRef.AlertFilterRefs.Add(ToAlertFilterRefModel(item));
            //    }
            //}
            return alertUrgencyLevelRef;
        }
        public AlertFilterRef ToAlertFilterRefModel(AlertResult request)
        {
            AlertFilterRef alertFilterRef = new AlertFilterRef();
            alertFilterRef.Id = request.alefil_id;
            alertFilterRef.AlertId = request.alefil_alert_id;
            alertFilterRef.AlertUrgencyLevelId = request.alefil_alert_urgency_level_id;
            alertFilterRef.FilterType = request.alefil_filter_type;
            alertFilterRef.ThresholdValue = request.alefil_threshold_value;
            alertFilterRef.UnitType = request.alefil_unit_type;
            alertFilterRef.LandmarkType = request.alefil_landmark_type;
            alertFilterRef.RefId = request.alefil_ref_id;
            alertFilterRef.PositionType = request.alefil_position_type;
            if (request.aleurg_day_type != null)
            {
                for (int i = 0; i < request.aleurg_day_type.Length; i++)
                {
                    alertFilterRef.DayType[i] = request.aleurg_day_type.Get(i);
                }
            }
            alertFilterRef.PeriodType = request.alefil_period_type;
            alertFilterRef.FilterStartDate = request.alefil_filter_start_date;
            alertFilterRef.FilterEndDate = request.alefil_filter_end_date;
            alertFilterRef.State = request.alefil_state;
            alertFilterRef.CreatedAt = request.alefil_created_at;
            alertFilterRef.ModifiedAt = request.alefil_modified_at;
            return alertFilterRef;
        }
        public AlertLandmarkRef ToAlertLandmarkRefModel(AlertResult request)
        {
            AlertLandmarkRef alertLandmarkRef = new AlertLandmarkRef();
            alertLandmarkRef.Id = request.alelan_id;
            alertLandmarkRef.AlertId = request.alelan_alert_id;
            alertLandmarkRef.LandmarkType = request.alelan_landmark_type;
            alertLandmarkRef.RefId = request.alelan_ref_id;
            alertLandmarkRef.UnitType = request.alelan_unit_type;
            alertLandmarkRef.Distance = request.alelan_distance;
            alertLandmarkRef.State = request.alelan_state;
            alertLandmarkRef.CreatedAt = request.alelan_created_at;
            alertLandmarkRef.ModifiedAt = request.alelan_modified_at;
            return alertLandmarkRef;
        }
        public Notification ToNotificationModel(AlertResult request)
        {
            Notification notification = new Notification();
            notification.Id = request.noti_id;
            notification.AlertId = request.noti_alert_id;
            notification.AlertUrgencyLevelType = request.noti_alert_urgency_level_type;
            notification.FrequencyType = request.noti_frequency_type;
            notification.FrequencyThreshholdValue = request.noti_frequency_threshhold_value;
            notification.ValidityType = request.noti_validity_type;
            notification.CreatedBy = request.noti_created_by;
            notification.ModifiedBy = request.noti_modified_by;
            notification.State = request.noti_state;
            notification.CreatedAt = request.noti_created_at;
            notification.ModifiedAt = request.noti_modified_at;
            notification.NotificationRecipients = new List<NotificationRecipient>();
            notification.NotificationLimits = new List<NotificationLimit>();
            notification.NotificationAvailabilityPeriods = new List<NotificationAvailabilityPeriod>();
            return notification;
        }
        public NotificationRecipient ToNotificationRecipientModel(AlertResult request)
        {
            NotificationRecipient notificationRecipient = new NotificationRecipient();
            notificationRecipient.Id = request.notrec_id;
            notificationRecipient.NotificationId = request.notrec_notification_id;
            notificationRecipient.RecipientLabel = request.notrec_recipient_label;
            notificationRecipient.AccountGroupId = request.notrec_account_group_id;
            notificationRecipient.NotificationModeType = request.notrec_notification_mode_type;
            notificationRecipient.PhoneNo = request.notrec_phone_no;
            notificationRecipient.Sms = request.notrec_sms;
            notificationRecipient.EmailId = request.notrec_email_id;
            notificationRecipient.EmailSub = request.notrec_email_sub;
            notificationRecipient.EmailText = request.notrec_email_text;
            notificationRecipient.WsUrl = request.notrec_ws_url;
            notificationRecipient.WsType = request.notrec_ws_type;
            notificationRecipient.WsText = request.notrec_ws_text;
            notificationRecipient.WsLogin = request.notrec_ws_login;
            notificationRecipient.WsPassword = request.notrec_ws_password;
            notificationRecipient.State = request.notrec_state;
            notificationRecipient.CreatedAt = request.notrec_created_at;
            notificationRecipient.ModifiedAt = request.notrec_modified_at;
            return notificationRecipient;
        }
        public NotificationLimit ToNotificationLimitModel(AlertResult request)
        {
            NotificationLimit notificationLimit = new NotificationLimit();
            notificationLimit.Id = request.notlim_id;
            notificationLimit.NotificationId = request.notlim_notification_id;
            notificationLimit.NotificationModeType = request.notlim_notification_mode_type;
            notificationLimit.MaxLimit = request.notlim_max_limit;
            notificationLimit.NotificationPeriodType = request.notlim_notification_period_type;
            notificationLimit.PeriodLimit = request.notlim_period_limit;
            notificationLimit.State = request.notlim_state;
            notificationLimit.CreatedAt = request.notlim_created_at;
            notificationLimit.ModifiedAt = request.notlim_modified_at;
            return notificationLimit;
        }
        public NotificationAvailabilityPeriod ToNotificationAvailabilityPeriodModel(AlertResult request)
        {
            NotificationAvailabilityPeriod notificationAvailabilityPeriod = new NotificationAvailabilityPeriod();
            notificationAvailabilityPeriod.Id = request.notava_id;
            notificationAvailabilityPeriod.NotificationId = request.notava_notification_id;
            notificationAvailabilityPeriod.AvailabilityPeriodType = request.notava_availability_period_type;
            notificationAvailabilityPeriod.PeriodType = request.notava_period_type;
            notificationAvailabilityPeriod.StartTime = request.notava_start_time;
            notificationAvailabilityPeriod.EndTime = request.notava_end_time;
            notificationAvailabilityPeriod.State = request.notava_state;
            notificationAvailabilityPeriod.CreatedAt = request.notava_created_at;
            notificationAvailabilityPeriod.ModifiedAt = request.notava_modified_at;
            return notificationAvailabilityPeriod;
        }

    }
}
