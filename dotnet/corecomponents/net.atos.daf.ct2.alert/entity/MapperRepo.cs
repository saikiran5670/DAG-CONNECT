using System;
using System.Collections.Generic;

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

            Dictionary<Tuple<int, int>, NotificationRecipient> notificationRecipientRefLookup = new Dictionary<Tuple<int, int>, NotificationRecipient>();
            Dictionary<Tuple<int, int, int>, NotificationLimit> notificationLimitkRefLookup = new Dictionary<Tuple<int, int, int>, NotificationLimit>();
            Dictionary<int, NotificationAvailabilityPeriod> notificationAvailabilityPeriodLookup = new Dictionary<int, NotificationAvailabilityPeriod>();

            NotificationAvailabilityPeriod notificationAvailabilityPeriod = new NotificationAvailabilityPeriod();
            Dictionary<int, AlertTimingDetail> alertTimingUrgencyLookup = new Dictionary<int, AlertTimingDetail>();
            Dictionary<int, AlertTimingDetail> alertTimingFilterLookup = new Dictionary<int, AlertTimingDetail>();
            Dictionary<int, AlertTimingDetail> alertTimingNotificationLookup = new Dictionary<int, AlertTimingDetail>();
            foreach (var alertItem in alertResult)
            {
                if (!alertLookup.TryGetValue(Convert.ToInt32(alertItem.Ale_id), out Alert alert))
                    alertLookup.Add(Convert.ToInt32(alertItem.Ale_id), alert = ToAlertModel(alertItem));

                if (alert.Notifications == null)
                    alert.Notifications = new List<Notification>();
                if (alert.AlertUrgencyLevelRefs == null)
                    alert.AlertUrgencyLevelRefs = new List<AlertUrgencyLevelRef>();
                if (alert.AlertLandmarkRefs == null)
                    alert.AlertLandmarkRefs = new List<AlertLandmarkRef>();

                if (alertItem.Aleurg_id > 0 && alertItem.Ale_id == alertItem.Aleurg_alert_id)
                {
                    if (!alertUrgencyLevelRefLookup.TryGetValue(Convert.ToInt32(alertItem.Aleurg_id), out AlertUrgencyLevelRef alertUrgencyLevelRef))
                    {
                        alertUrgencyLevelRefLookup.Add(Convert.ToInt32(alertItem.Aleurg_id), alertUrgencyLevelRef = ToAlertUrgencyLevelRefModel(alertItem));
                        alert.AlertUrgencyLevelRefs.Add(alertUrgencyLevelRef);
                    }
                    if (alertItem.Alefil_id > 0 && alertItem.Aleurg_id == alertItem.Alefil_alert_urgency_level_id)
                    {
                        if (!alertFilterRefLookup.TryGetValue(Convert.ToInt32(alertItem.Alefil_id), out AlertFilterRef alertFilterRef))
                        {
                            //var alertFilterRefDetails = ToAlertFilterRefModel(alertItem);                           
                            alertFilterRefLookup.Add(Convert.ToInt32(alertItem.Alefil_id), alertFilterRef = ToAlertFilterRefModel(alertItem));
                            alertUrgencyLevelRef.AlertFilterRefs.Add(alertFilterRef);
                        }
                        if (alertItem.Aletimefil_id > 0 && alertItem.Aletimefil_ref_id == alertItem.Alefil_id)
                        {
                            if (!alertTimingFilterLookup.TryGetValue(Convert.ToInt32(alertItem.Aletimefil_id), out _))
                            {
                                var alertTimingalertFilter = ToAlertTimingDetailModel(alertItem, 'F');
                                alertTimingFilterLookup.Add(Convert.ToInt32(alertItem.Aletimefil_id), alertTimingalertFilter);
                                alertFilterRef.AlertTimingDetails.Add(alertTimingalertFilter);
                            }
                        }
                    }

                    if (alertItem.Aletimeurg_id > 0 && alertItem.Aletimeurg_ref_id == alertItem.Aleurg_id)
                    {
                        if (!alertTimingUrgencyLookup.TryGetValue(Convert.ToInt32(alertItem.Aletimeurg_id), out _))
                        {
                            var alertTimingUrgencyFilter = ToAlertTimingDetailModel(alertItem, 'U');
                            alertTimingUrgencyLookup.Add(Convert.ToInt32(alertItem.Aletimeurg_id), alertTimingUrgencyFilter);
                            alertUrgencyLevelRef.AlertTimingDetails.Add(alertTimingUrgencyFilter);
                        }
                    }
                }
                if (alertItem.Alelan_id > 0 && alertItem.Ale_id == alertItem.Alelan_alert_id)
                {
                    if (!alertLandmarkRefLookup.TryGetValue(Convert.ToInt32(alertItem.Alelan_id), out _))
                    {
                        var alertLandmarkRef = ToAlertLandmarkRefModel(alertItem);
                        alertLandmarkRefLookup.Add(Convert.ToInt32(alertItem.Alelan_id), alertLandmarkRef);
                        alert.AlertLandmarkRefs.Add(alertLandmarkRef);
                    }
                }
                if (alertItem.Noti_id > 0 && alertItem.Ale_id == alertItem.Noti_alert_id)
                {
                    if (!notificationLookup.TryGetValue(Convert.ToInt32(alertItem.Noti_id), out Notification notification))
                    {
                        notificationLookup.Add(Convert.ToInt32(alertItem.Noti_id), notification = ToNotificationModel(alertItem));
                        alert.Notifications.Add(notification);
                    }
                    if (alertItem.Aletimenoti_id > 0 && alertItem.Aletimenoti_ref_id == alertItem.Noti_id)
                    {
                        if (!alertTimingNotificationLookup.TryGetValue(Convert.ToInt32(alertItem.Aletimenoti_id), out _))
                        {
                            var alertTimingNotificationFilter = ToAlertTimingDetailModel(alertItem, 'N');
                            alertTimingNotificationLookup.Add(Convert.ToInt32(alertItem.Aletimenoti_id), alertTimingNotificationFilter);
                            notification.AlertTimingDetails.Add(alertTimingNotificationFilter);
                        }
                    }
                    //if (alertItem.notava_id > 0 && alertItem.notava_notification_id == alertItem.noti_id)
                    //{
                    //    if (!notificationAvailabilityPeriodLookup.TryGetValue(Convert.ToInt32(alertItem.notava_id), out notificationAvailabilityPeriod))
                    //    {
                    //        notificationAvailabilityPeriodLookup.Add(Convert.ToInt32(alertItem.notava_id), notificationAvailabilityPeriod = ToNotificationAvailabilityPeriodModel(alertItem));
                    //        notification.NotificationAvailabilityPeriods.Add(notificationAvailabilityPeriod);
                    //    }
                    //}
                    if (alertItem.Notrec_id > 0 && alertItem.Notref_notification_id == alertItem.Noti_id && alertItem.Notref_recipient_id == alertItem.Notrec_id && alertItem.Notref_alert_id == alertItem.Ale_id)
                    {
                        if (!notificationRecipientRefLookup.TryGetValue(Tuple.Create(Convert.ToInt32(alertItem.Notrec_id), alertItem.Notref_alert_id), out NotificationRecipient notificationRecipient))
                        {
                            notificationRecipient = ToNotificationRecipientModel(alertItem);
                            notificationRecipientRefLookup.Add(Tuple.Create(Convert.ToInt32(alertItem.Notrec_id), alertItem.Notref_alert_id), notificationRecipient);
                            notification.NotificationRecipients.Add(notificationRecipient);
                        }
                        if (alertItem.Notlim_id > 0 && alertItem.Notlim_recipient_id == alertItem.Notrec_id)
                        {
                            if (!notificationLimitkRefLookup.TryGetValue(Tuple.Create(Convert.ToInt32(alertItem.Notlim_id), alertItem.Notlim_notification_id, alertItem.Notref_alert_id), out _))
                            {
                                var notificationLimit = ToNotificationLimitModel(alertItem);
                                notificationLimitkRefLookup.Add(Tuple.Create(Convert.ToInt32(alertItem.Notlim_id), alertItem.Notlim_notification_id, alertItem.Notref_alert_id), notificationLimit);
                                notificationRecipient.NotificationLimits.Add(notificationLimit);
                            }
                        }
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
            alert.Id = request.Ale_id;
            alert.OrganizationId = request.Ale_organization_id;
            alert.Name = request.Ale_name;
            alert.Category = request.Ale_category;
            alert.Type = request.Ale_type;
            alert.ValidityPeriodType = request.Ale_validity_period_type;
            alert.ValidityStartDate = request.Ale_validity_start_date;
            alert.ValidityEndDate = request.Ale_validity_end_date;
            alert.VehicleGroupId = request.Ale_vehicle_group_id;
            alert.State = request.Ale_state;
            alert.CreatedAt = request.Alefil_created_at;
            alert.CreatedBy = request.Ale_created_by;
            alert.ModifiedAt = request.Alefil_modified_at;
            alert.ModifiedBy = request.Ale_modified_by;
            alert.ApplyOn = request.Ale_applyon;
            alert.Vin = request.Vin;
            alert.RegNo = request.Regno;
            alert.VehicleName = request.Vehiclename;
            alert.VehicleGroupName = request.Vehiclegroupname;
            alert.AlertUrgencyLevelRefs = new List<AlertUrgencyLevelRef>();
            alert.Notifications = new List<Notification>();
            alert.AlertLandmarkRefs = new List<AlertLandmarkRef>();
            return alert;
        }
        public AlertUrgencyLevelRef ToAlertUrgencyLevelRefModel(AlertResult request)
        {
            AlertUrgencyLevelRef alertUrgencyLevelRef = new AlertUrgencyLevelRef();
            alertUrgencyLevelRef.Id = request.Aleurg_id;
            alertUrgencyLevelRef.AlertId = request.Aleurg_alert_id;
            alertUrgencyLevelRef.UrgencyLevelType = request.Aleurg_urgency_level_type;
            alertUrgencyLevelRef.ThresholdValue = request.Aleurg_threshold_value;
            alertUrgencyLevelRef.UnitType = request.Aleurg_unit_type;
            if (request.Aleurg_day_type != null)
            {
                for (int i = 0; i < request.Aleurg_day_type.Length; i++)
                {
                    alertUrgencyLevelRef.DayType[i] = request.Aleurg_day_type.Get(i);
                }
            }
            alertUrgencyLevelRef.PeriodType = request.Aleurg_period_type;
            alertUrgencyLevelRef.UrgencylevelStartDate = request.Aleurg_urgencylevel_start_date;
            alertUrgencyLevelRef.UrgencylevelEndDate = request.Aleurg_urgencylevel_end_date;
            alertUrgencyLevelRef.State = request.Aleurg_state;
            alertUrgencyLevelRef.CreatedAt = request.Aleurg_created_at;
            alertUrgencyLevelRef.ModifiedAt = request.Aleurg_modified_at;
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
            alertFilterRef.Id = request.Alefil_id;
            alertFilterRef.AlertId = request.Alefil_alert_id;
            alertFilterRef.AlertUrgencyLevelId = request.Alefil_alert_urgency_level_id;
            alertFilterRef.FilterType = request.Alefil_filter_type;
            alertFilterRef.ThresholdValue = request.Alefil_threshold_value;
            alertFilterRef.UnitType = request.Alefil_unit_type;
            alertFilterRef.LandmarkType = request.Alefil_landmark_type;
            alertFilterRef.RefId = request.Alefil_ref_id;
            alertFilterRef.PositionType = request.Alefil_position_type;
            //if (request.Aleurg_day_type != null)
            //{
            //    for (int i = 0; i < request.Aleurg_day_type.Length; i++)
            //    {
            //        alertFilterRef.DayType[i] = request.Aleurg_day_type.Get(i);
            //    }
            //}
            //alertFilterRef.PeriodType = request.Alefil_period_type;
            //alertFilterRef.FilterStartDate = request.Alefil_filter_start_date;
            //alertFilterRef.FilterEndDate = request.Alefil_filter_end_date;
            alertFilterRef.State = request.Alefil_state;
            alertFilterRef.CreatedAt = request.Alefil_created_at;
            alertFilterRef.ModifiedAt = request.Alefil_modified_at;
            return alertFilterRef;
        }
        public AlertLandmarkRef ToAlertLandmarkRefModel(AlertResult request)
        {
            AlertLandmarkRef alertLandmarkRef = new AlertLandmarkRef();
            alertLandmarkRef.Id = request.Alelan_id;
            alertLandmarkRef.AlertId = request.Alelan_alert_id;
            alertLandmarkRef.LandmarkType = request.Alelan_landmark_type;
            alertLandmarkRef.RefId = request.Alelan_ref_id;
            alertLandmarkRef.UnitType = request.Alelan_unit_type;
            alertLandmarkRef.Distance = request.Alelan_distance;
            alertLandmarkRef.State = request.Alelan_state;
            alertLandmarkRef.CreatedAt = request.Alelan_created_at;
            alertLandmarkRef.ModifiedAt = request.Alelan_modified_at;
            return alertLandmarkRef;
        }
        public Notification ToNotificationModel(AlertResult request)
        {
            Notification notification = new Notification();
            notification.Id = request.Noti_id;
            notification.AlertId = request.Noti_alert_id;
            notification.AlertUrgencyLevelType = request.Noti_alert_urgency_level_type;
            notification.FrequencyType = request.Noti_frequency_type;
            notification.FrequencyThreshholdValue = request.Noti_frequency_threshhold_value;
            notification.ValidityType = request.Noti_validity_type;
            notification.CreatedBy = request.Noti_created_by;
            notification.ModifiedBy = request.Noti_modified_by;
            notification.State = request.Noti_state;
            notification.CreatedAt = request.Noti_created_at;
            notification.ModifiedAt = request.Noti_modified_at;
            notification.NotificationRecipients = new List<NotificationRecipient>();
            //notification.NotificationAvailabilityPeriods = new List<NotificationAvailabilityPeriod>();
            return notification;
        }
        public NotificationRecipient ToNotificationRecipientModel(AlertResult request)
        {
            NotificationRecipient notificationRecipient = new NotificationRecipient();
            notificationRecipient.Id = request.Notrec_id;
            notificationRecipient.NotificationId = request.Notrec_notification_id;
            notificationRecipient.RecipientLabel = request.Notrec_recipient_label;
            notificationRecipient.AccountGroupId = request.Notrec_account_group_id;
            notificationRecipient.NotificationModeType = request.Notrec_notification_mode_type;
            notificationRecipient.PhoneNo = request.Notrec_phone_no;
            notificationRecipient.Sms = request.Notrec_sms;
            notificationRecipient.EmailId = request.Notrec_email_id;
            notificationRecipient.EmailSub = request.Notrec_email_sub;
            notificationRecipient.EmailText = request.Notrec_email_text;
            notificationRecipient.WsUrl = request.Notrec_ws_url;
            notificationRecipient.WsType = request.Notrec_ws_type;
            notificationRecipient.WsText = request.Notrec_ws_text;
            notificationRecipient.WsLogin = request.Notrec_ws_login;
            notificationRecipient.WsPassword = request.Notrec_ws_password;
            notificationRecipient.State = request.Notrec_state;
            notificationRecipient.CreatedAt = request.Notrec_created_at;
            notificationRecipient.ModifiedAt = request.Notrec_modified_at;
            notificationRecipient.NotificationLimits = new List<NotificationLimit>();
            return notificationRecipient;
        }

        public AlertTimingDetail ToAlertTimingDetailModel(AlertResult request, char alerttimetype)
        {
            AlertTimingDetail alerttimingdetail = new AlertTimingDetail();
            if (alerttimetype == 'U')
            {
                alerttimingdetail.Id = request.Aletimeurg_id;
                alerttimingdetail.Type = request.Aletimeurg_type;
                alerttimingdetail.RefId = request.Aletimeurg_ref_id;
                if (request.Aletimeurg_day_type != null)
                {
                    for (int i = 0; i < request.Aletimeurg_day_type.Length; i++)
                    {
                        alerttimingdetail.DayType[i] = request.Aletimeurg_day_type.Get(i);
                    }
                }
                alerttimingdetail.PeriodType = request.Aletimeurg_period_type;
                alerttimingdetail.StartDate = request.Aletimeurg_start_date;
                alerttimingdetail.EndDate = request.Aletimeurg_end_date;
                alerttimingdetail.State = request.Aletimeurg_state;
                alerttimingdetail.CreatedAt = request.Aletimeurg_created_at;
                alerttimingdetail.ModifiedAt = request.Aletimeurg_modified_at;
            }
            else if (alerttimetype == 'F')
            {
                alerttimingdetail.Id = request.Aletimefil_id;
                alerttimingdetail.Type = request.Aletimefil_type;
                alerttimingdetail.RefId = request.Aletimefil_ref_id;
                if (request.Aletimefil_day_type != null)
                {
                    for (int i = 0; i < request.Aletimefil_day_type.Length; i++)
                    {
                        alerttimingdetail.DayType[i] = request.Aletimefil_day_type.Get(i);
                    }
                }
                alerttimingdetail.PeriodType = request.Aletimefil_period_type;
                alerttimingdetail.StartDate = request.Aletimefil_start_date;
                alerttimingdetail.EndDate = request.Aletimefil_end_date;
                alerttimingdetail.State = request.Aletimefil_state;
                alerttimingdetail.CreatedAt = request.Aletimefil_created_at;
                alerttimingdetail.ModifiedAt = request.Aletimefil_modified_at;
            }
            else if (alerttimetype == 'N')
            {
                alerttimingdetail.Id = request.Aletimenoti_id;
                alerttimingdetail.Type = request.Aletimenoti_type;
                alerttimingdetail.RefId = request.Aletimenoti_ref_id;
                if (request.Aletimenoti_day_type != null)
                {
                    for (int i = 0; i < request.Aletimenoti_day_type.Length; i++)
                    {
                        alerttimingdetail.DayType[i] = request.Aletimenoti_day_type.Get(i);
                    }
                }
                alerttimingdetail.PeriodType = request.Aletimenoti_period_type;
                alerttimingdetail.StartDate = request.Aletimenoti_start_date;
                alerttimingdetail.EndDate = request.Aletimenoti_end_date;
                alerttimingdetail.State = request.Aletimenoti_state;
                alerttimingdetail.CreatedAt = request.Aletimenoti_created_at;
                alerttimingdetail.ModifiedAt = request.Aletimenoti_modified_at;
            }
            return alerttimingdetail;
        }
        public NotificationLimit ToNotificationLimitModel(AlertResult request)
        {
            NotificationLimit notificationLimit = new NotificationLimit();
            notificationLimit.Id = request.Notlim_id;
            notificationLimit.NotificationId = request.Notlim_notification_id;
            notificationLimit.NotificationModeType = request.Notlim_notification_mode_type;
            notificationLimit.MaxLimit = request.Notlim_max_limit;
            notificationLimit.NotificationPeriodType = request.Notlim_notification_period_type;
            notificationLimit.PeriodLimit = request.Notlim_period_limit;
            notificationLimit.State = request.Notlim_state;
            notificationLimit.CreatedAt = request.Notlim_created_at;
            notificationLimit.ModifiedAt = request.Notlim_modified_at;
            notificationLimit.RecipientId = request.Notlim_recipient_id;
            return notificationLimit;
        }
        public NotificationAvailabilityPeriod ToNotificationAvailabilityPeriodModel(AlertResult request)
        {
            NotificationAvailabilityPeriod notificationAvailabilityPeriod = new NotificationAvailabilityPeriod();
            notificationAvailabilityPeriod.Id = request.Notava_id;
            notificationAvailabilityPeriod.NotificationId = request.Notava_notification_id;
            notificationAvailabilityPeriod.AvailabilityPeriodType = request.Notava_availability_period_type;
            notificationAvailabilityPeriod.PeriodType = request.Notava_period_type;
            notificationAvailabilityPeriod.StartTime = request.Notava_start_time;
            notificationAvailabilityPeriod.EndTime = request.Notava_end_time;
            notificationAvailabilityPeriod.State = request.Notava_state;
            notificationAvailabilityPeriod.CreatedAt = request.Notava_created_at;
            notificationAvailabilityPeriod.ModifiedAt = request.Notava_modified_at;
            return notificationAvailabilityPeriod;
        }

        public NotificationRecipientRef ToNotificationRecipientRefModel(AlertResult request)
        {
            NotificationRecipientRef notificationRecipientRef = new NotificationRecipientRef();
            notificationRecipientRef.Id = request.Notref_id;
            notificationRecipientRef.NotificationId = request.Notref_notification_id;
            notificationRecipientRef.AlertId = request.Notref_alert_id;
            notificationRecipientRef.RecipientId = request.Notref_recipient_id;
            notificationRecipientRef.State = request.Notref_state;
            notificationRecipientRef.CreatedAt = request.Notref_created_at;
            notificationRecipientRef.ModifiedAt = request.Notref_modified_at;
            return notificationRecipientRef;
        }

    }
}
