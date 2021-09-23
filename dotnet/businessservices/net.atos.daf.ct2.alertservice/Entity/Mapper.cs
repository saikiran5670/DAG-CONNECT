﻿using System.Collections.Generic;
using System.Linq;
using net.atos.daf.ct2.alert.entity;

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

        public DuplicateAlert ToDupliacteAlert(DuplicateAlertType request)
        {
            var alert = new DuplicateAlert();
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
            return alert;
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
            alert.State = request.State;
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
            alert.ValidityPeriodType = string.IsNullOrEmpty(request.ValidityPeriodType) ? string.Empty : request.ValidityPeriodType;
            alert.ValidityStartDate = request.ValidityStartDate;
            alert.ValidityEndDate = request.ValidityEndDate;
            alert.VehicleGroupId = request.VehicleGroupId;
            alert.State = request.State;
            alert.CreatedAt = request.CreatedAt;
            alert.CreatedBy = request.CreatedBy;
            alert.ModifiedAt = request.ModifiedAt;
            alert.ModifiedBy = request.ModifiedBy;
            alert.VehicleName = string.IsNullOrEmpty(request.VehicleName) ? string.Empty : request.VehicleName;
            alert.VehicleGroupName = string.IsNullOrEmpty(request.VehicleGroupName) ? string.Empty : request.VehicleGroupName;
            alert.Vin = string.IsNullOrEmpty(request.Vin) ? string.Empty : request.Vin;
            alert.RegNo = string.IsNullOrEmpty(request.RegNo) ? string.Empty : request.RegNo;
            alert.ApplyOn = string.IsNullOrEmpty(request.ApplyOn) ? string.Empty : request.ApplyOn;
            if (request.AlertUrgencyLevelRefs.Count > 0)
            {
                foreach (var item in request.AlertUrgencyLevelRefs)
                {
                    alert.AlertUrgencyLevelRefs.Add(MapAlertUrgencyLevelRefEntity(item));
                }
            }

            if (request.Notifications.Count > 0)
            {
                IEnumerable<List<NotificationRecipient>> notificationRecepobj = null;
                notificationRecepobj = request.Notifications.Select(x => x.NotificationRecipients);
                if (notificationRecepobj.Count() > 1)
                {
                    foreach (var item in request.Notifications)
                    {
                        if (!item.NotificationRecipients.Any())
                        {
                            item.NotificationRecipients = notificationRecepobj.FirstOrDefault();
                        }
                    }
                }

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
            alertUrgencyLevelRef.AlertTimingDetails = new List<AlertTimingDetail>();
            if (request.AlertTimingDetail.Count > 0)
            {
                foreach (var item in request.AlertTimingDetail)
                {
                    alertUrgencyLevelRef.AlertTimingDetails.Add(ToAlertTimingDetailEntity(item));
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
            alertUrgencyLevelRef.UnitType = string.IsNullOrEmpty(request.UnitType) ? string.Empty : request.UnitType;
            //alertUrgencyLevelRef.DayType = request.DayType;
            alertUrgencyLevelRef.PeriodType = string.IsNullOrEmpty(request.PeriodType) ? string.Empty : request.PeriodType;
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
            if (request.AlertTimingDetails.Count > 0)
            {
                foreach (var item in request.AlertTimingDetails)
                {
                    alertUrgencyLevelRef.AlertTimingDetail.Add(MapAlertTimingDetailUrgencyEntity(item));
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
            //alertFilterRef.DayType = request.DayType.ToArray();
            //alertFilterRef.PeriodType = request.PeriodType;
            //alertFilterRef.FilterStartDate = request.FilterStartDate;
            //alertFilterRef.FilterEndDate = request.FilterEndDate;
            alertFilterRef.State = request.State;
            alertFilterRef.CreatedAt = request.CreatedAt;
            alertFilterRef.ModifiedAt = request.ModifiedAt;
            alertFilterRef.AlertTimingDetails = new List<AlertTimingDetail>();
            if (request.AlertTimingDetail.Count > 0)
            {
                foreach (var item in request.AlertTimingDetail)
                {
                    alertFilterRef.AlertTimingDetails.Add(ToAlertTimingDetailEntity(item));
                }
            }
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
            alertFilterRef.UnitType = string.IsNullOrEmpty(request.UnitType) ? string.Empty : request.UnitType;
            alertFilterRef.LandmarkType = string.IsNullOrEmpty(request.LandmarkType) ? string.Empty : request.LandmarkType; ;
            alertFilterRef.RefId = request.RefId;
            alertFilterRef.PositionType = string.IsNullOrEmpty(request.PositionType) ? string.Empty : request.PositionType; ;
            //alertFilterRef.DayType = request.DayType.ToArray();
            //alertFilterRef.PeriodType = string.IsNullOrEmpty(request.PeriodType) ? string.Empty : request.PeriodType;
            //alertFilterRef.FilterStartDate = request.FilterStartDate;
            //alertFilterRef.FilterEndDate = request.FilterEndDate;
            alertFilterRef.State = request.State;
            alertFilterRef.CreatedAt = request.CreatedAt;
            alertFilterRef.ModifiedAt = request.ModifiedAt;
            if (request.AlertTimingDetails.Count > 0)
            {
                foreach (var item in request.AlertTimingDetails)
                {
                    alertFilterRef.AlertTimingDetail.Add(MapAlertTimingDetailEntity(item));
                }
            }
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
            notification.AlertTimingDetails = new List<AlertTimingDetail>();
            if (request.AlertTimingDetail.Count > 0)
            {
                foreach (var item in request.AlertTimingDetail)
                {
                    notification.AlertTimingDetails.Add(ToAlertTimingDetailEntity(item));
                }
            }
            return notification;
        }

        public NotificationRequest MapNotificationEntity(Notification request)
        {
            NotificationRequest notification = new NotificationRequest();
            notification.Id = request.Id;
            notification.AlertId = request.AlertId;
            notification.AlertUrgencyLevelType = string.IsNullOrEmpty(request.AlertUrgencyLevelType) ? string.Empty : request.AlertUrgencyLevelType;
            notification.FrequencyType = request.FrequencyType;
            notification.FrequencyThreshholdValue = request.FrequencyThreshholdValue;
            notification.ValidityType = string.IsNullOrEmpty(request.ValidityType) ? string.Empty : request.ValidityType;
            notification.CreatedBy = request.CreatedBy;
            notification.ModifiedBy = request.ModifiedBy;
            notification.State = request.State;
            notification.CreatedAt = request.CreatedAt;
            notification.ModifiedAt = request.ModifiedAt;

            if (request.NotificationRecipients.Count > 0)
            {
                foreach (var item in request.NotificationRecipients)
                {
                    notification.NotificationRecipients.Add(MapNotificationRecipientMultiEntity(item, notification.Id));
                }
            }
            if (request.AlertTimingDetails.Count > 0)
            {
                foreach (var item in request.AlertTimingDetails)
                {
                    notification.AlertTimingDetail.Add(MapAlertTimingDetailNotificationEntity(item));
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
            notificationRecipient.NotificationLimits = new List<NotificationLimit>();
            if (request.NotificationLimits.Count > 0)
            {
                foreach (var item in request.NotificationLimits)
                {
                    notificationRecipient.NotificationLimits.Add(ToNotificationLimitEntity(item));
                }
            }
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
            notificationRecipient.PhoneNo = string.IsNullOrEmpty(request.PhoneNo) ? string.Empty : request.PhoneNo;
            notificationRecipient.Sms = string.IsNullOrEmpty(request.Sms) ? string.Empty : request.Sms;
            notificationRecipient.EmailId = string.IsNullOrEmpty(request.EmailId) ? string.Empty : request.EmailId;
            notificationRecipient.EmailSub = string.IsNullOrEmpty(request.EmailSub) ? string.Empty : request.EmailSub;
            notificationRecipient.EmailText = string.IsNullOrEmpty(request.EmailText) ? string.Empty : request.EmailText; ;
            notificationRecipient.WsUrl = string.IsNullOrEmpty(request.WsUrl) ? string.Empty : request.WsUrl; ;
            notificationRecipient.WsType = string.IsNullOrEmpty(request.WsType) ? string.Empty : request.WsType; ;
            notificationRecipient.WsText = string.IsNullOrEmpty(request.WsText) ? string.Empty : request.WsText; ;
            notificationRecipient.WsLogin = string.IsNullOrEmpty(request.WsLogin) ? string.Empty : request.WsLogin; ;
            notificationRecipient.WsPassword = string.IsNullOrEmpty(request.WsPassword) ? string.Empty : request.WsPassword; ;
            notificationRecipient.State = request.State;
            notificationRecipient.CreatedAt = request.CreatedAt;
            notificationRecipient.ModifiedAt = request.ModifiedAt;
            if (request.NotificationLimits != null)
            {
                if (request.NotificationLimits.Count > 0)
                {
                    foreach (var item in request.NotificationLimits)
                    {
                        notificationRecipient.NotificationLimits.Add(MapNotificationLimitEntity(item));
                    }
                }
            }
            return notificationRecipient;
        }

        public NotificationRecipientRequest MapNotificationRecipientMultiEntity(NotificationRecipient request, int notificationId)
        {
            NotificationRecipientRequest notificationRecipient = new NotificationRecipientRequest();
            notificationRecipient.Id = request.Id;
            notificationRecipient.NotificationId = request.NotificationId;
            notificationRecipient.RecipientLabel = request.RecipientLabel;
            notificationRecipient.AccountGroupId = request.AccountGroupId;
            notificationRecipient.NotificationModeType = request.NotificationModeType;
            notificationRecipient.PhoneNo = string.IsNullOrEmpty(request.PhoneNo) ? string.Empty : request.PhoneNo;
            notificationRecipient.Sms = string.IsNullOrEmpty(request.Sms) ? string.Empty : request.Sms;
            notificationRecipient.EmailId = string.IsNullOrEmpty(request.EmailId) ? string.Empty : request.EmailId;
            notificationRecipient.EmailSub = string.IsNullOrEmpty(request.EmailSub) ? string.Empty : request.EmailSub;
            notificationRecipient.EmailText = string.IsNullOrEmpty(request.EmailText) ? string.Empty : request.EmailText; ;
            notificationRecipient.WsUrl = string.IsNullOrEmpty(request.WsUrl) ? string.Empty : request.WsUrl; ;
            notificationRecipient.WsType = string.IsNullOrEmpty(request.WsType) ? string.Empty : request.WsType; ;
            notificationRecipient.WsText = string.IsNullOrEmpty(request.WsText) ? string.Empty : request.WsText; ;
            notificationRecipient.WsLogin = string.IsNullOrEmpty(request.WsLogin) ? string.Empty : request.WsLogin; ;
            notificationRecipient.WsPassword = string.IsNullOrEmpty(request.WsPassword) ? string.Empty : request.WsPassword; ;
            notificationRecipient.State = request.State;
            notificationRecipient.CreatedAt = request.CreatedAt;
            notificationRecipient.ModifiedAt = request.ModifiedAt;
            if (request.NotificationLimits != null)
            {
                if (request.NotificationLimits.Count > 0)
                {
                    foreach (var item in request.NotificationLimits)
                    {
                        if (item.NotificationId == notificationId)
                        {
                            notificationRecipient.NotificationLimits.Add(MapNotificationLimitEntity(item));
                        }
                    }
                }
            }
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
            notificationLimit.RecipientId = request.RecipientId;
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
            notificationLimit.RecipientId = request.RecipientId;
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
            notificationAvailabilityPeriod.PeriodType = string.IsNullOrEmpty(request.PeriodType) ? string.Empty : request.PeriodType;
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
            alertLandmarkRef.UnitType = string.IsNullOrEmpty(request.UnitType) ? string.Empty : request.UnitType;
            alertLandmarkRef.Distance = request.Distance;
            alertLandmarkRef.State = request.State;
            alertLandmarkRef.CreatedAt = request.CreatedAt;
            alertLandmarkRef.ModifiedAt = request.ModifiedAt;
            return alertLandmarkRef;
        }
        public AlertTimingDetail ToAlertTimingDetailEntity(AlertTimingDetailRequest request)
        {
            AlertTimingDetail alertTimingDetail = new AlertTimingDetail();
            alertTimingDetail.Id = request.Id;
            alertTimingDetail.Type = request.Type;
            alertTimingDetail.RefId = request.RefId;
            alertTimingDetail.DayType = request.DayType.ToArray();
            alertTimingDetail.PeriodType = request.PeriodType;
            alertTimingDetail.StartDate = request.StartDate;
            alertTimingDetail.EndDate = request.EndDate;
            alertTimingDetail.State = request.State;
            alertTimingDetail.CreatedAt = request.CreatedAt;
            alertTimingDetail.ModifiedAt = request.ModifiedAt;
            return alertTimingDetail;
        }
        public AlertTimingDetailRequest MapAlertTimingDetailEntity(AlertTimingDetail request)
        {
            AlertTimingDetailRequest alertTimingDetailRequest = new AlertTimingDetailRequest();
            alertTimingDetailRequest.Id = request.Id;
            alertTimingDetailRequest.Type = request.Type;
            alertTimingDetailRequest.RefId = request.RefId;
            for (int i = 0; i < request.DayType.Length; i++)
            {
                alertTimingDetailRequest.DayType.Add(request.DayType[i]);
            }
            alertTimingDetailRequest.PeriodType = string.IsNullOrEmpty(request.PeriodType) ? string.Empty : request.PeriodType;
            alertTimingDetailRequest.StartDate = request.StartDate;
            alertTimingDetailRequest.EndDate = request.EndDate;
            alertTimingDetailRequest.State = request.State;
            alertTimingDetailRequest.CreatedAt = request.CreatedAt;
            alertTimingDetailRequest.ModifiedAt = request.ModifiedAt;
            return alertTimingDetailRequest;
        }
        public AlertTimingDetailRequest MapAlertTimingDetailUrgencyEntity(AlertTimingDetail request)
        {
            AlertTimingDetailRequest alertTimingDetailRequest = new AlertTimingDetailRequest();
            alertTimingDetailRequest.Id = request.Id;
            alertTimingDetailRequest.Type = request.Type;
            alertTimingDetailRequest.RefId = request.RefId;
            for (int i = 0; i < request.DayType.Length; i++)
            {
                alertTimingDetailRequest.DayType.Add(request.DayType[i]);
            }
            alertTimingDetailRequest.PeriodType = string.IsNullOrEmpty(request.PeriodType) ? string.Empty : request.PeriodType;
            alertTimingDetailRequest.StartDate = request.StartDate;
            alertTimingDetailRequest.EndDate = request.EndDate;
            alertTimingDetailRequest.State = request.State;
            alertTimingDetailRequest.CreatedAt = request.CreatedAt;
            alertTimingDetailRequest.ModifiedAt = request.ModifiedAt;
            return alertTimingDetailRequest;
        }
        public AlertTimingDetailRequest MapAlertTimingDetailNotificationEntity(AlertTimingDetail request)
        {
            AlertTimingDetailRequest alertTimingDetailRequest = new AlertTimingDetailRequest();
            alertTimingDetailRequest.Id = request.Id;
            alertTimingDetailRequest.Type = request.Type;
            alertTimingDetailRequest.RefId = request.RefId;
            for (int i = 0; i < request.DayType.Length; i++)
            {
                alertTimingDetailRequest.DayType.Add(request.DayType[i]);
            }
            alertTimingDetailRequest.PeriodType = string.IsNullOrEmpty(request.PeriodType) ? string.Empty : request.PeriodType;
            alertTimingDetailRequest.StartDate = request.StartDate;
            alertTimingDetailRequest.EndDate = request.EndDate;
            alertTimingDetailRequest.State = request.State;
            alertTimingDetailRequest.CreatedAt = request.CreatedAt;
            alertTimingDetailRequest.ModifiedAt = request.ModifiedAt;
            return alertTimingDetailRequest;
        }

        public NotificationTemplate MapNotificationTemplate(net.atos.daf.ct2.alertservice.NotificationTemplate notificationTemplate)
        {
            NotificationTemplate objNotificationTemplate = new NotificationTemplate();
            objNotificationTemplate.Id = notificationTemplate.Id;
            objNotificationTemplate.AlertCategoryType = string.IsNullOrEmpty(notificationTemplate.AlertCategoryType) ? string.Empty : notificationTemplate.AlertCategoryType;
            objNotificationTemplate.AlertType = string.IsNullOrEmpty(notificationTemplate.AlertType) ? string.Empty : notificationTemplate.AlertType;
            objNotificationTemplate.Text = string.IsNullOrEmpty(notificationTemplate.Text) ? string.Empty : notificationTemplate.Text;
            objNotificationTemplate.Subject = string.IsNullOrEmpty(notificationTemplate.Subject) ? string.Empty : notificationTemplate.Subject;
            return objNotificationTemplate;
        }
        public List<NotificationViewHistory> GetNotificationViewHistoryEntity(NotificationViewRequest notificationViewRequest)
        {
            List<NotificationViewHistory> lstnotificationView = new List<NotificationViewHistory>();
            foreach (var item in notificationViewRequest.NotificationView)
            {
                NotificationViewHistory notificationViewHistory = new NotificationViewHistory();
                notificationViewHistory.AccountId = item.AccountId;
                notificationViewHistory.AlertId = item.AlertId;
                notificationViewHistory.AlertCategory = item.AlertCategory;
                notificationViewHistory.AlertType = item.AlertType;
                notificationViewHistory.AlertGeneratedTime = item.AlertGeneratedTime;
                notificationViewHistory.OrganizationId = item.OrganizationId;
                notificationViewHistory.TripAlertId = item.TripAlertId;
                notificationViewHistory.TripId = item.TripId;
                notificationViewHistory.Vin = item.Vin;
                notificationViewHistory.AlertViewTimestamp = item.AlertViewTimestamp;
                lstnotificationView.Add(notificationViewHistory);
            }
            return lstnotificationView;
        }

        public OfflineNotificationResponse ToOfflineNotificationResponse(OfflinePushNotification offlinePushNotification)
        {
            OfflineNotificationResponse offlineNotificationResponse = new OfflineNotificationResponse();
            offlineNotificationResponse.NotAccResponse = new NotificationAccountResponse();
            offlineNotificationResponse.NotAccResponse.NotificationCount = offlinePushNotification.NotificationAccount.NotificationCount;
            offlineNotificationResponse.NotAccResponse.AccountId = offlineNotificationResponse.NotAccResponse.AccountId;
            if (offlinePushNotification.NotificationDisplayProp != null)
            {
                foreach (var item in offlinePushNotification.NotificationDisplayProp)
                {
                    NotificationDisplayResponse notificationDisplayResponse = new NotificationDisplayResponse();
                    notificationDisplayResponse.AlertId = item.AlertId;
                    notificationDisplayResponse.AlertName = item.AlertName ?? string.Empty;
                    notificationDisplayResponse.AlertCategory = item.AlertCategory ?? string.Empty;
                    notificationDisplayResponse.AlertCategoryKey = item.AlertCategoryKey ?? string.Empty;
                    notificationDisplayResponse.AlertType = item.AlertType ?? string.Empty;
                    notificationDisplayResponse.AlertTypeKey = item.AlertTypeKey ?? string.Empty;
                    notificationDisplayResponse.AlertUrgency = item.UrgencyLevel ?? string.Empty;
                    notificationDisplayResponse.UrgencyTypeKey = item.UrgencyTypeKey ?? string.Empty;
                    notificationDisplayResponse.TripId = item.TripId ?? string.Empty;
                    notificationDisplayResponse.TripAlertId = item.TripAlertId;
                    notificationDisplayResponse.VehicleGroupId = item.VehicleGroupId;
                    notificationDisplayResponse.VehicleGroupName = item.VehicleGroupName ?? string.Empty;
                    notificationDisplayResponse.VehicleName = item.VehicleName ?? string.Empty;
                    notificationDisplayResponse.VehicleLicencePlate = item.VehicleLicencePlate ?? string.Empty;
                    notificationDisplayResponse.Vin = item.Vin ?? string.Empty;
                    notificationDisplayResponse.AlertGeneratedTime = item.AlertGeneratedTime;
                    notificationDisplayResponse.AccountId = item.AccountId;
                    notificationDisplayResponse.OrganizationId = item.OrganizationId;
                    offlineNotificationResponse.NotificationResponse.Add(notificationDisplayResponse);

                }
            }
            return offlineNotificationResponse;
        }
    }
}
