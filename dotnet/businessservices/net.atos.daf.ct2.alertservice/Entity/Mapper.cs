using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.alert.entity;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;

namespace net.atos.daf.ct2.alertservice.Entity
{
    public class Mapper
    {
        public EnumTranslation MapEnumTranslation(net.atos.daf.ct2.alert.entity.EnumTranslation enumTrans)
        {
            EnumTranslation objenumtrans = new EnumTranslation();
            objenumtrans.Id = enumTrans.Id;
            objenumtrans.Type = string.IsNullOrEmpty(enumTrans.Type) ? string.Empty : enumTrans.Type;
            objenumtrans.Enum = string.IsNullOrEmpty(enumTrans.Enum) ? string.Empty : enumTrans.Enum;
            objenumtrans.ParentEnum = string.IsNullOrEmpty(enumTrans.ParentEnum) ? string.Empty : enumTrans.ParentEnum;
            objenumtrans.Key = string.IsNullOrEmpty(enumTrans.Key) ? string.Empty : enumTrans.Key;
            return objenumtrans;
        }
       

        public Alert ToAlertEntity(AlertRequest request)
        {
            Alert alert = new Alert();
            alert.Id = request.Id;
            alert.OrganizationId = request.OrganizationId;
            alert.Name = request.Name;
            alert.Category = request.Category;
            alert.Type = request.Type;
            alert.ValidityPeriodType = request.ValidityPeriodType;
            alert.ValidityStartDate = request.ValidityStartDate;
            alert.ValidityEndDate = request.ValidityEndDate;
            alert.VehicleGroupId = request.VehicleGroupId;
            alert.State= request.State;
            alert.CreatedAt = request.CreatedAt;
            alert.CreatedBy = request.CreatedBy;
            alert.ModifiedAt = request.ModifiedAt;
            alert.ModifiedBy = request.ModifiedBy;
            alert.AlertUrgencyLevelRefs = new List<AlertUrgencyLevelRef>();
            if (request.AlertUrgencyLevelRefs.Count > 0)
            {
                foreach (var item in request.AlertUrgencyLevelRefs)
                {
                    alert.AlertUrgencyLevelRefs.Add(ToAlertUrgencyLevelRefEntity(item));                    
                }
            }            
            alert.Notifications = new List<Notification>();
            if (request.Notifications.Count > 0)
            {
                foreach (var item in request.Notifications)
                {
                    alert.Notifications.Add(ToNotificationEntity(item));
                }
            }
            alert.AlertLandmarkRefs = new List<AlertLandmarkRef>();
            if (request.AlertLandmarkRefs.Count > 0)
            {
                foreach (var item in request.AlertLandmarkRefs)
                {
                    alert.AlertLandmarkRefs.Add(ToAlertLandmarkRefEntity(item));
                }
            }
            return alert;
        }

        public AlertRequest MapAlertEntity(Alert request)
        {
            AlertRequest alert = new AlertRequest();
            alert.Id = request.Id;
            alert.OrganizationId = request.OrganizationId;
            alert.Name = request.Name;
            alert.Category = request.Category;
            alert.Type = request.Type;
            alert.ValidityPeriodType = request.ValidityPeriodType;
            alert.ValidityStartDate = request.ValidityStartDate;
            alert.ValidityEndDate = request.ValidityEndDate;
            alert.VehicleGroupId = request.VehicleGroupId;
            alert.State = request.State;
            alert.CreatedAt = request.CreatedAt;
            alert.CreatedBy = request.CreatedBy;
            alert.ModifiedAt = request.ModifiedAt;
            alert.ModifiedBy = request.ModifiedBy;
           
            if (request.AlertUrgencyLevelRefs.Count > 0)
            {
                foreach (var item in request.AlertUrgencyLevelRefs)
                {
                    alert.AlertUrgencyLevelRefs.Add(MapAlertUrgencyLevelRefEntity(item));
                }
            }
            
            if (request.Notifications.Count > 0)
            {
                foreach (var item in request.Notifications)
                {
                    alert.Notifications.Add(MapNotificationEntity(item));
                }
            }
          
            if (request.AlertLandmarkRefs.Count > 0)
            {
                foreach (var item in request.AlertLandmarkRefs)
                {
                    alert.AlertLandmarkRefs.Add(MapAlertLandmarkRefEntity(item));
                }
            }
            return alert;
        }

        public AlertUrgencyLevelRef ToAlertUrgencyLevelRefEntity(AlertUrgencyLevelRefRequest request)
        {
            AlertUrgencyLevelRef alertUrgencyLevelRef = new AlertUrgencyLevelRef();
            alertUrgencyLevelRef.Id = request.Id;
            alertUrgencyLevelRef.AlertId = request.AlertId;
            alertUrgencyLevelRef.UrgencyLevelType = request.UrgencyLevelType;
            alertUrgencyLevelRef.ThresholdValue = request.ThresholdValue;
            alertUrgencyLevelRef.UnitType = request.UnitType;
            alertUrgencyLevelRef.DayType = request.DayType.ToArray();
            alertUrgencyLevelRef.PeriodType = request.PeriodType;
            alertUrgencyLevelRef.UrgencylevelStartDate = request.UrgencylevelStartDate;
            alertUrgencyLevelRef.UrgencylevelEndDate = request.UrgencylevelEndDate;
            alertUrgencyLevelRef.State = request.State;
            alertUrgencyLevelRef.CreatedAt = request.CreatedAt;
            alertUrgencyLevelRef.ModifiedAt = request.ModifiedAt;
            alertUrgencyLevelRef.AlertFilterRefs = new List<AlertFilterRef>();
            if (request.AlertFilterRefs.Count > 0)
            {
                foreach (var item in request.AlertFilterRefs)
                {
                    alertUrgencyLevelRef.AlertFilterRefs.Add(ToAlertFilterRefEntity(item));
                }
            }
            return alertUrgencyLevelRef;
        }

        public AlertUrgencyLevelRefRequest MapAlertUrgencyLevelRefEntity(AlertUrgencyLevelRef request)
        {
            AlertUrgencyLevelRefRequest alertUrgencyLevelRef = new AlertUrgencyLevelRefRequest();
            alertUrgencyLevelRef.Id = request.Id;
            alertUrgencyLevelRef.AlertId = request.AlertId;
            alertUrgencyLevelRef.UrgencyLevelType = request.UrgencyLevelType;
            alertUrgencyLevelRef.ThresholdValue = request.ThresholdValue;
            alertUrgencyLevelRef.UnitType = request.UnitType;
            //alertUrgencyLevelRef.DayType = request.DayType;
            alertUrgencyLevelRef.PeriodType = request.PeriodType;
            alertUrgencyLevelRef.UrgencylevelStartDate = request.UrgencylevelStartDate;
            alertUrgencyLevelRef.UrgencylevelEndDate = request.UrgencylevelEndDate;
            alertUrgencyLevelRef.State = request.State;
            alertUrgencyLevelRef.CreatedAt = request.CreatedAt;
            alertUrgencyLevelRef.ModifiedAt = request.ModifiedAt;
            
            if (request.AlertFilterRefs.Count > 0)
            {
                foreach (var item in request.AlertFilterRefs)
                {
                    alertUrgencyLevelRef.AlertFilterRefs.Add(MapAlertFilterRefEntity(item));
                }
            }
            return alertUrgencyLevelRef;
        }

        public AlertFilterRef ToAlertFilterRefEntity(AlertFilterRefRequest request)
        {
            AlertFilterRef alertFilterRef = new AlertFilterRef();
            alertFilterRef.Id = request.Id;
            alertFilterRef.AlertId = request.AlertId;
            alertFilterRef.AlertUrgencyLevelId = request.AlertUrgencyLevelId;
            alertFilterRef.FilterType = request.FilterType;
            alertFilterRef.ThresholdValue = request.ThresholdValue;
            alertFilterRef.UnitType = request.UnitType;
            alertFilterRef.LandmarkType = request.LandmarkType;
            alertFilterRef.RefId = request.RefId;
            alertFilterRef.PositionType = request.PositionType;
            alertFilterRef.DayType = request.DayType.ToArray();
            alertFilterRef.PeriodType = request.PeriodType;
            alertFilterRef.FilterStartDate = request.FilterStartDate;
            alertFilterRef.FilterEndDate = request.FilterEndDate;
            alertFilterRef.State = request.State;
            alertFilterRef.CreatedAt = request.CreatedAt;
            alertFilterRef.ModifiedAt = request.ModifiedAt;
            return alertFilterRef;
        }

        public AlertFilterRefRequest MapAlertFilterRefEntity(AlertFilterRef request)
        {
            AlertFilterRefRequest alertFilterRef = new AlertFilterRefRequest();
            alertFilterRef.Id = request.Id;
            alertFilterRef.AlertId = request.AlertId;
            alertFilterRef.AlertUrgencyLevelId = request.AlertUrgencyLevelId;
            alertFilterRef.FilterType = request.FilterType;
            alertFilterRef.ThresholdValue = request.ThresholdValue;
            alertFilterRef.UnitType = request.UnitType;
            alertFilterRef.LandmarkType = request.LandmarkType;
            alertFilterRef.RefId = request.RefId;
            alertFilterRef.PositionType = request.PositionType;
            //alertFilterRef.DayType = request.DayType.ToArray();
            alertFilterRef.PeriodType = request.PeriodType;
            alertFilterRef.FilterStartDate = request.FilterStartDate;
            alertFilterRef.FilterEndDate = request.FilterEndDate;
            alertFilterRef.State = request.State;
            alertFilterRef.CreatedAt = request.CreatedAt;
            alertFilterRef.ModifiedAt = request.ModifiedAt;
            return alertFilterRef;
        }

        public Notification ToNotificationEntity(NotificationRequest request)
        {
            Notification notification = new Notification();
            notification.Id = request.Id;
            notification.AlertId = request.AlertId;
            notification.AlertUrgencyLevelType = request.AlertUrgencyLevelType;
            notification.FrequencyType = request.FrequencyType;
            notification.FrequencyThreshholdValue = request.FrequencyThreshholdValue;
            notification.ValidityType = request.ValidityType;
            notification.CreatedBy = request.CreatedBy;
            notification.ModifiedBy = request.ModifiedBy;
            notification.State = request.State;
            notification.CreatedAt = request.CreatedAt;
            notification.ModifiedAt = request.ModifiedAt;
            notification.NotificationRecipients = new List<NotificationRecipient>();
            if (request.NotificationRecipients.Count > 0)
            {
                foreach (var item in request.NotificationRecipients)
                {
                    notification.NotificationRecipients.Add(ToNotificationRecipientEntity(item));
                }
            }
            notification.NotificationLimits = new List<NotificationLimit>();
            if (request.NotificationLimits.Count > 0)
            {
                foreach (var item in request.NotificationLimits)
                {
                    notification.NotificationLimits.Add(ToNotificationLimitEntity(item));
                }
            }
            notification.NotificationAvailabilityPeriods = new List<NotificationAvailabilityPeriod>();
            if (request.NotificationAvailabilityPeriods.Count > 0)
            {
                foreach (var item in request.NotificationAvailabilityPeriods)
                {
                    notification.NotificationAvailabilityPeriods.Add(ToNotificationAvailabilityPeriodEntity(item));
                }
            }
            return notification;
        }

        public NotificationRequest MapNotificationEntity(Notification request)
        {
            NotificationRequest notification = new NotificationRequest();
            notification.Id = request.Id;
            notification.AlertId = request.AlertId;
            notification.AlertUrgencyLevelType = request.AlertUrgencyLevelType;
            notification.FrequencyType = request.FrequencyType;
            notification.FrequencyThreshholdValue = request.FrequencyThreshholdValue;
            notification.ValidityType = request.ValidityType;
            notification.CreatedBy = request.CreatedBy;
            notification.ModifiedBy = request.ModifiedBy;
            notification.State = request.State;
            notification.CreatedAt = request.CreatedAt;
            notification.ModifiedAt = request.ModifiedAt;
            
            if (request.NotificationRecipients.Count > 0)
            {
                foreach (var item in request.NotificationRecipients)
                {
                    notification.NotificationRecipients.Add(MapNotificationRecipientEntity(item));
                }
            }
            
            if (request.NotificationLimits.Count > 0)
            {
                foreach (var item in request.NotificationLimits)
                {
                    notification.NotificationLimits.Add(MapNotificationLimitEntity(item));
                }
            }
           
            if (request.NotificationAvailabilityPeriods.Count > 0)
            {
                foreach (var item in request.NotificationAvailabilityPeriods)
                {
                    notification.NotificationAvailabilityPeriods.Add(MapNotificationAvailabilityPeriodEntity(item));
                }
            }
            return notification;
        }

        public NotificationRecipient ToNotificationRecipientEntity(NotificationRecipientRequest request)
        {
            NotificationRecipient notificationRecipient = new NotificationRecipient();
            notificationRecipient.Id = request.Id;
            notificationRecipient.NotificationId = request.NotificationId;
            notificationRecipient.RecipientLabel = request.RecipientLabel;
            notificationRecipient.AccountGroupId = request.AccountGroupId;
            notificationRecipient.NotificationModeType = request.NotificationModeType;
            notificationRecipient.PhoneNo = request.PhoneNo;
            notificationRecipient.Sms = request.Sms;
            notificationRecipient.EmailId = request.EmailId;
            notificationRecipient.EmailSub = request.EmailSub;
            notificationRecipient.EmailText = request.EmailText;
            notificationRecipient.WsUrl = request.WsUrl;
            notificationRecipient.WsType = request.WsType;
            notificationRecipient.WsText = request.WsText;
            notificationRecipient.WsLogin = request.WsLogin;
            notificationRecipient.WsPassword = request.WsPassword;
            notificationRecipient.State = request.State;
            notificationRecipient.CreatedAt = request.CreatedAt;
            notificationRecipient.ModifiedAt = request.ModifiedAt;
            return notificationRecipient;
        }

        public NotificationRecipientRequest MapNotificationRecipientEntity(NotificationRecipient request)
        {
            NotificationRecipientRequest notificationRecipient = new NotificationRecipientRequest();
            notificationRecipient.Id = request.Id;
            notificationRecipient.NotificationId = request.NotificationId;
            notificationRecipient.RecipientLabel = request.RecipientLabel;
            notificationRecipient.AccountGroupId = request.AccountGroupId;
            notificationRecipient.NotificationModeType = request.NotificationModeType;
            notificationRecipient.PhoneNo = request.PhoneNo;
            notificationRecipient.Sms = request.Sms;
            notificationRecipient.EmailId = request.EmailId;
            notificationRecipient.EmailSub = request.EmailSub;
            notificationRecipient.EmailText = request.EmailText;
            notificationRecipient.WsUrl = request.WsUrl;
            notificationRecipient.WsType = request.WsType;
            notificationRecipient.WsText = request.WsText;
            notificationRecipient.WsLogin = request.WsLogin;
            notificationRecipient.WsPassword = request.WsPassword;
            notificationRecipient.State = request.State;
            notificationRecipient.CreatedAt = request.CreatedAt;
            notificationRecipient.ModifiedAt = request.ModifiedAt;
            return notificationRecipient;
        }

        public NotificationLimit ToNotificationLimitEntity(NotificationLimitRequest request)
        {
            NotificationLimit notificationLimit = new NotificationLimit();
            notificationLimit.Id = request.Id;
            notificationLimit.NotificationId = request.NotificationId;
            notificationLimit.NotificationModeType = request.NotificationModeType;
            notificationLimit.MaxLimit = request.MaxLimit;
            notificationLimit.NotificationPeriodType = request.NotificationPeriodType;
            notificationLimit.PeriodLimit = request.PeriodLimit;
            notificationLimit.State = request.State;
            notificationLimit.CreatedAt = request.CreatedAt;
            notificationLimit.ModifiedAt = request.ModifiedAt;
            return notificationLimit;
        }

        public NotificationLimitRequest MapNotificationLimitEntity(NotificationLimit request)
        {
            NotificationLimitRequest notificationLimit = new NotificationLimitRequest();
            notificationLimit.Id = request.Id;
            notificationLimit.NotificationId = request.NotificationId;
            notificationLimit.NotificationModeType = request.NotificationModeType;
            notificationLimit.MaxLimit = request.MaxLimit;
            notificationLimit.NotificationPeriodType = request.NotificationPeriodType;
            notificationLimit.PeriodLimit = request.PeriodLimit;
            notificationLimit.State = request.State;
            notificationLimit.CreatedAt = request.CreatedAt;
            notificationLimit.ModifiedAt = request.ModifiedAt;
            return notificationLimit;
        }

        public NotificationAvailabilityPeriod ToNotificationAvailabilityPeriodEntity(NotificationAvailabilityPeriodRequest request)
        {
            NotificationAvailabilityPeriod notificationAvailabilityPeriod = new NotificationAvailabilityPeriod();
            notificationAvailabilityPeriod.Id = request.Id;
            notificationAvailabilityPeriod.NotificationId = request.NotificationId;
            notificationAvailabilityPeriod.AvailabilityPeriodType = request.AvailabilityPeriodType;
            notificationAvailabilityPeriod.PeriodType = request.PeriodType;
            notificationAvailabilityPeriod.StartTime = request.StartTime;
            notificationAvailabilityPeriod.EndTime = request.EndTime;
            notificationAvailabilityPeriod.State = request.State;
            notificationAvailabilityPeriod.CreatedAt = request.CreatedAt;
            notificationAvailabilityPeriod.ModifiedAt = request.ModifiedAt;
            return notificationAvailabilityPeriod;
        }

        public NotificationAvailabilityPeriodRequest MapNotificationAvailabilityPeriodEntity(NotificationAvailabilityPeriod request)
        {
            NotificationAvailabilityPeriodRequest notificationAvailabilityPeriod = new NotificationAvailabilityPeriodRequest();
            notificationAvailabilityPeriod.Id = request.Id;
            notificationAvailabilityPeriod.NotificationId = request.NotificationId;
            notificationAvailabilityPeriod.AvailabilityPeriodType = request.AvailabilityPeriodType;
            notificationAvailabilityPeriod.PeriodType = request.PeriodType;
            notificationAvailabilityPeriod.StartTime = request.StartTime;
            notificationAvailabilityPeriod.EndTime = request.EndTime;
            notificationAvailabilityPeriod.State = request.State;
            notificationAvailabilityPeriod.CreatedAt = request.CreatedAt;
            notificationAvailabilityPeriod.ModifiedAt = request.ModifiedAt;
            return notificationAvailabilityPeriod;
        }

        public AlertLandmarkRef ToAlertLandmarkRefEntity(AlertLandmarkRefRequest request)
        {
            AlertLandmarkRef alertLandmarkRef = new AlertLandmarkRef();
            alertLandmarkRef.Id = request.Id;
            alertLandmarkRef.AlertId = request.AlertId;
            alertLandmarkRef.LandmarkType = request.LandmarkType;
            alertLandmarkRef.RefId = request.RefId;
            alertLandmarkRef.UnitType = request.UnitType;
            alertLandmarkRef.Distance = request.Distance;
            alertLandmarkRef.State = request.State;
            alertLandmarkRef.CreatedAt = request.CreatedAt;
            alertLandmarkRef.ModifiedAt = request.ModifiedAt;
            return alertLandmarkRef;
        }

        public AlertLandmarkRefRequest MapAlertLandmarkRefEntity(AlertLandmarkRef request)
        {
            AlertLandmarkRefRequest alertLandmarkRef = new AlertLandmarkRefRequest();
            alertLandmarkRef.Id = request.Id;
            alertLandmarkRef.AlertId = request.AlertId;
            alertLandmarkRef.LandmarkType = request.LandmarkType;
            alertLandmarkRef.RefId = request.RefId;
            alertLandmarkRef.UnitType = request.UnitType;
            alertLandmarkRef.Distance = request.Distance;
            alertLandmarkRef.State = request.State;
            alertLandmarkRef.CreatedAt = request.CreatedAt;
            alertLandmarkRef.ModifiedAt = request.ModifiedAt;
            return alertLandmarkRef;
        }
    }
}
