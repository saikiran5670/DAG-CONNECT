﻿using net.atos.daf.ct2.alertservice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortalAlertEntity=net.atos.daf.ct2.portalservice.Entity.Alert;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class Mapper
    {
		public AlertRequest ToAlertRequest(PortalAlertEntity.Alert entity)
		{
			AlertRequest request = new AlertRequest();
            //request.Id = entity.Id;
            request.OrganizationId = entity.OrganizationId;
            request.Name = entity.Name;
			request.Category = entity.Category;
			request.Type = entity.Type;
			request.ValidityPeriodType = entity.ValidityPeriodType;
			request.ValidityStartDate = entity.ValidityStartDate;
			request.ValidityEndDate = entity.ValidityEndDate;
			request.VehicleGroupId = entity.VehicleGroupId;
            request.State = entity.State;
            //request.CreatedAt = entity.CreatedAt;
            request.CreatedBy = entity.CreatedBy;
            //request.ModifiedAt = entity.ModifiedAt;
            //request.ModifiedBy = entity.ModifiedBy;
            //request.AlertUrgencyLevelRefs = new List<AlertUrgencyLevelRef>();
            if (entity.AlertUrgencyLevelRefs.Count > 0)
			{
				foreach (var item in entity.AlertUrgencyLevelRefs)
				{
					request.AlertUrgencyLevelRefs.Add(ToAlertUrgencyLevelRefRequest(item));
				}
			}
			//request.Notifications = new List<Notification>();
			if (entity.Notifications.Count > 0)
			{
				foreach (var item in entity.Notifications)
				{
					request.Notifications.Add(ToNotificationRequest(item));
				}
			}
			//request.AlertLandmarkRefs = new List<AlertLandmarkRef>();
			if (entity.AlertLandmarkRefs.Count > 0)
			{
				foreach (var item in entity.AlertLandmarkRefs)
				{
					request.AlertLandmarkRefs.Add(ToAlertLandmarkRequest(item));
				}
			}
			return request;
		}
		public AlertUrgencyLevelRefRequest ToAlertUrgencyLevelRefRequest(PortalAlertEntity.AlertUrgencyLevelRef entity)
		{
			AlertUrgencyLevelRefRequest request = new AlertUrgencyLevelRefRequest();
			//request.Id = entity.Id;
			//request.AlertId = entity.AlertId;
			request.UrgencyLevelType = entity.UrgencyLevelType;
			request.ThresholdValue = entity.ThresholdValue;
			request.UnitType = entity.UnitType;
			for (int i = 0; i < entity.DayType.Length; i++)
			{
				request.DayType.Add(entity.DayType[i]);
			}
            request.PeriodType = entity.PeriodType;
			request.UrgencylevelStartDate = entity.UrgencylevelStartDate;
			request.UrgencylevelEndDate = entity.UrgencylevelEndDate;
			//request.State = entity.State;
			//request.CreatedAt = entity.CreatedAt;
			//request.ModifiedAt = entity.ModifiedAt;
			//request.AlertFilterRefs = new List<AlertFilterRef>();
			if (entity.AlertFilterRefs.Count > 0)
			{
				foreach (var item in entity.AlertFilterRefs)
				{
					request.AlertFilterRefs.Add(ToAlertFilterRefRequest(item));
				}
			}
			return request;
		}
		public AlertFilterRefRequest ToAlertFilterRefRequest(PortalAlertEntity.AlertFilterRef entity)
		{
			AlertFilterRefRequest request = new AlertFilterRefRequest();
			//request.Id = entity.Id;
			//request.AlertId = entity.AlertId;
			request.AlertUrgencyLevelId = entity.AlertUrgencyLevelId;
			request.FilterType = entity.FilterType;
			request.ThresholdValue = entity.ThresholdValue;
			request.UnitType = entity.UnitType;
			request.LandmarkType = entity.LandmarkType;
			request.RefId = entity.RefId;
			request.PositionType = entity.PositionType;
			for (int i = 0; i < entity.DayType.Length; i++)
			{
				request.DayType.Add(entity.DayType[i]);
			}
			request.PeriodType = entity.PeriodType;
			request.FilterStartDate = entity.FilterStartDate;
			request.FilterEndDate = entity.FilterEndDate;
			//request.State = entity.State;
			//request.CreatedAt = entity.CreatedAt;
			//request.ModifiedAt = entity.ModifiedAt;
			return request;
		}
		public AlertLandmarkRefRequest ToAlertLandmarkRequest(PortalAlertEntity.AlertLandmarkRef entity)
		{
			AlertLandmarkRefRequest request = new AlertLandmarkRefRequest();
			//request.Id = entity.Id;
			//request.AlertId = entity.AlertId;
			request.LandmarkType = entity.LandmarkType;
			request.RefId = entity.RefId;
			request.UnitType = entity.UnitType;
			request.Distance = entity.Distance;
			//request.State = entity.State;
			//request.CreatedAt = entity.CreatedAt;
			//request.ModifiedAt = entity.ModifiedAt;
			return request;
		}
		public NotificationRequest ToNotificationRequest(PortalAlertEntity.Notification entity)
		{
			NotificationRequest request = new NotificationRequest();
			//request.Id = entity.Id;
			//request.AlertId = entity.AlertId;
			request.AlertUrgencyLevelType = entity.AlertUrgencyLevelType;
			request.FrequencyType = entity.FrequencyType;
			request.FrequencyThreshholdValue = entity.FrequencyThreshholdValue;
			request.ValidityType = entity.ValidityType;
			request.CreatedBy = entity.CreatedBy;
			//request.ModifiedBy = entity.ModifiedBy;
			//request.State = entity.State;
			//request.CreatedAt = entity.CreatedAt;
			//request.ModifiedAt = entity.ModifiedAt;
			//request.NotificationRecipients = new List<NotificationRecipientRequest>();
			if (entity.NotificationRecipients.Count > 0)
			{
				foreach (var item in entity.NotificationRecipients)
				{
					request.NotificationRecipients.Add(ToNotificationRecipientRequest(item));
				}
			}
			//request.NotificationLimits = new List<NotificationLimit>();
			if (entity.NotificationLimits.Count > 0)
			{
				foreach (var item in entity.NotificationLimits)
				{
					request.NotificationLimits.Add(ToNotificationLimitRequest(item));
				}
			}
			//request.NotificationAvailabilityPeriods = new List<NotificationAvailabilityPeriod>();
			if (entity.NotificationAvailabilityPeriods.Count > 0)
			{
				foreach (var item in entity.NotificationAvailabilityPeriods)
				{
					request.NotificationAvailabilityPeriods.Add(ToNotificationAvailabilityPeriodRequest(item));
				}
			}
			return request;
		}
		public NotificationRecipientRequest ToNotificationRecipientRequest(PortalAlertEntity.NotificationRecipient entity)
		{
			NotificationRecipientRequest request = new NotificationRecipientRequest();
			//request.Id = entity.Id;
			//request.NotificationId = entity.NotificationId;
			request.RecipientLabel = entity.RecipientLabel;
			request.AccountGroupId = entity.AccountGroupId;
			request.NotificationModeType = entity.NotificationModeType;
			request.PhoneNo = entity.PhoneNo;
			request.Sms = entity.Sms;
			request.EmailId = entity.EmailId;
			request.EmailSub = entity.EmailSub;
			request.EmailText = entity.EmailText;
			request.WsUrl = entity.WsUrl;
			request.WsType = entity.WsType;
			request.WsText = entity.WsText;
			request.WsLogin = entity.WsLogin;
			request.WsPassword = entity.WsPassword;
			//request.State = entity.State;
			//request.CreatedAt = entity.CreatedAt;
			//request.ModifiedAt = entity.ModifiedAt;
			return request;
		}
		public NotificationLimitRequest ToNotificationLimitRequest(PortalAlertEntity.NotificationLimit entity)
		{
			NotificationLimitRequest request = new NotificationLimitRequest();
			//request.Id = entity.Id;
			//request.NotificationId = entity.NotificationId;
			request.NotificationModeType = entity.NotificationModeType;
			request.MaxLimit = entity.MaxLimit;
			request.NotificationPeriodType = entity.NotificationPeriodType;
			request.PeriodLimit = entity.PeriodLimit;
			//request.State = entity.State;
			//request.CreatedAt = entity.CreatedAt;
			//request.ModifiedAt = entity.ModifiedAt;
			return request;
		}
		public NotificationAvailabilityPeriodRequest ToNotificationAvailabilityPeriodRequest(PortalAlertEntity.NotificationAvailabilityPeriod entity)
		{
			NotificationAvailabilityPeriodRequest request = new NotificationAvailabilityPeriodRequest();
			//request.Id = entity.Id;
			//request.NotificationId = entity.NotificationId;
			request.AvailabilityPeriodType = entity.AvailabilityPeriodType;
			request.PeriodType = entity.PeriodType;
			request.StartTime = entity.StartTime;
			request.EndTime = entity.EndTime;
			//request.State = entity.State;
			//request.CreatedAt = entity.CreatedAt;
			//request.ModifiedAt = entity.ModifiedAt;
			return request;
		}

		public AlertRequest ToAlertEditRequest(PortalAlertEntity.AlertEdit entity)
		{
			AlertRequest request = new AlertRequest();
			request.Id = entity.Id;
			request.OrganizationId = entity.OrganizationId;
			request.Name = entity.Name;
			//request.Category = entity.Category;
			//request.Type = entity.Type;
			request.ValidityPeriodType = entity.ValidityPeriodType;
			request.ValidityStartDate = entity.ValidityStartDate;
			request.ValidityEndDate = entity.ValidityEndDate;
			request.VehicleGroupId = entity.VehicleGroupId;
			//request.CreatedAt = entity.CreatedAt;
			//request.CreatedBy = entity.CreatedBy;
			//request.ModifiedAt = entity.ModifiedAt;
			request.ModifiedBy = entity.ModifiedBy;
			//request.AlertUrgencyLevelRefs = new List<AlertUrgencyLevelRef>();
			if (entity.AlertUrgencyLevelRefs.Count > 0)
			{
				foreach (var item in entity.AlertUrgencyLevelRefs)
				{
					request.AlertUrgencyLevelRefs.Add(ToAlertEditUrgencyLevelRefRequest(item));
				}
			}
			//request.Notifications = new List<Notification>();
			if (entity.Notifications.Count > 0)
			{
				foreach (var item in entity.Notifications)
				{
					request.Notifications.Add(ToNotificationEditRequest(item));
				}
			}
			//request.AlertLandmarkRefs = new List<AlertLandmarkRef>();
			if (entity.AlertLandmarkRefs.Count > 0)
			{
				foreach (var item in entity.AlertLandmarkRefs)
				{
					request.AlertLandmarkRefs.Add(ToAlertEditLandmarkRequest(item));
				}
			}
			return request;
		}

		public AlertUrgencyLevelRefRequest ToAlertEditUrgencyLevelRefRequest(PortalAlertEntity.AlertUrgencyLevelRefEdit entity)
		{
			AlertUrgencyLevelRefRequest request = new AlertUrgencyLevelRefRequest();
			request.Id = entity.Id;
			request.AlertId = entity.AlertId;
			request.UrgencyLevelType = entity.UrgencyLevelType;
			request.ThresholdValue = entity.ThresholdValue;
			request.UnitType = entity.UnitType;
			//request.DayType = entity.DayType.ToArray();
			request.PeriodType = entity.PeriodType;
			request.UrgencylevelStartDate = entity.UrgencylevelStartDate;
			request.UrgencylevelEndDate = entity.UrgencylevelEndDate;
			//request.State = entity.State;
			//request.CreatedAt = entity.CreatedAt;
			//request.ModifiedAt = entity.ModifiedAt;
			//request.AlertFilterRefs = new List<AlertFilterRef>();
			if (entity.AlertFilterRefs.Count > 0)
			{
				foreach (var item in entity.AlertFilterRefs)
				{
					request.AlertFilterRefs.Add(ToAlertEditFilterRefRequest(item));
				}
			}
			return request;
		}

		public AlertFilterRefRequest ToAlertEditFilterRefRequest(PortalAlertEntity.AlertFilterRefEdit entity)
		{
			AlertFilterRefRequest request = new AlertFilterRefRequest();
			request.Id = entity.Id;
			request.AlertId = entity.AlertId;
			request.AlertUrgencyLevelId = entity.AlertUrgencyLevelId;
			request.FilterType = entity.FilterType;
			request.ThresholdValue = entity.ThresholdValue;
			request.UnitType = entity.UnitType;
			request.LandmarkType = entity.LandmarkType;
			request.RefId = entity.RefId;
			request.PositionType = entity.PositionType;
			//request.DayType = entity.DayType.ToArray();
			request.PeriodType = entity.PeriodType;
			request.FilterStartDate = entity.FilterStartDate;
			request.FilterEndDate = entity.FilterEndDate;
			//request.State = entity.State;
			//request.CreatedAt = entity.CreatedAt;
			//request.ModifiedAt = entity.ModifiedAt;
			return request;
		}

		public AlertLandmarkRefRequest ToAlertEditLandmarkRequest(PortalAlertEntity.AlertLandmarkRefEdit entity)
		{
			AlertLandmarkRefRequest request = new AlertLandmarkRefRequest();
			request.Id = entity.Id;
			request.AlertId = entity.AlertId;
			request.LandmarkType = entity.LandmarkType;
			request.RefId = entity.RefId;
			request.UnitType = entity.UnitType;
			request.Distance = entity.Distance;
			//request.State = entity.State;
			//request.CreatedAt = entity.CreatedAt;
			//request.ModifiedAt = entity.ModifiedAt;
			return request;
		}

		public NotificationRequest ToNotificationEditRequest(PortalAlertEntity.NotificationEdit entity)
		{
			NotificationRequest request = new NotificationRequest();
			request.Id = entity.Id;
			request.AlertId = entity.AlertId;
			request.AlertUrgencyLevelType = entity.AlertUrgencyLevelType;
			request.FrequencyType = entity.FrequencyType;
			request.FrequencyThreshholdValue = entity.FrequencyThreshholdValue;
			request.ValidityType = entity.ValidityType;
			//request.CreatedBy = entity.CreatedBy;
			request.ModifiedBy = entity.ModifiedBy;
			//request.State = entity.State;
			//request.CreatedAt = entity.CreatedAt;
			//request.ModifiedAt = entity.ModifiedAt;
			//request.NotificationRecipients = new List<NotificationRecipientRequest>();
			if (entity.NotificationRecipients.Count > 0)
			{
				foreach (var item in entity.NotificationRecipients)
				{
					request.NotificationRecipients.Add(ToNotificationEditRecipientRequest(item));
				}
			}
			//request.NotificationLimits = new List<NotificationLimit>();
			if (entity.NotificationLimits.Count > 0)
			{
				foreach (var item in entity.NotificationLimits)
				{
					request.NotificationLimits.Add(ToNotificationEditLimitRequest(item));
				}
			}
			//request.NotificationAvailabilityPeriods = new List<NotificationAvailabilityPeriod>();
			if (entity.NotificationAvailabilityPeriods.Count > 0)
			{
				foreach (var item in entity.NotificationAvailabilityPeriods)
				{
					request.NotificationAvailabilityPeriods.Add(ToNotificationAvailabilityPeriodRequest(item));
				}
			}
			return request;
		}

		public NotificationRecipientRequest ToNotificationEditRecipientRequest(PortalAlertEntity.NotificationRecipientEdit entity)
		{
			NotificationRecipientRequest request = new NotificationRecipientRequest();
			request.Id = entity.Id;
			request.NotificationId = entity.NotificationId;
			request.RecipientLabel = entity.RecipientLabel;
			request.AccountGroupId = entity.AccountGroupId;
			request.NotificationModeType = entity.NotificationModeType;
			request.PhoneNo = entity.PhoneNo;
			request.Sms = entity.Sms;
			request.EmailId = entity.EmailId;
			request.EmailSub = entity.EmailSub;
			request.EmailText = entity.EmailText;
			request.WsUrl = entity.WsUrl;
			request.WsType = entity.WsType;
			request.WsText = entity.WsText;
			request.WsLogin = entity.WsLogin;
			request.WsPassword = entity.WsPassword;
			//request.State = entity.State;
			//request.CreatedAt = entity.CreatedAt;
			//request.ModifiedAt = entity.ModifiedAt;
			return request;
		}

		public NotificationLimitRequest ToNotificationEditLimitRequest(PortalAlertEntity.NotificationLimitEdit entity)
		{
			NotificationLimitRequest request = new NotificationLimitRequest();
			request.Id = entity.Id;
			request.NotificationId = entity.NotificationId;
			request.NotificationModeType = entity.NotificationModeType;
			request.MaxLimit = entity.MaxLimit;
			request.NotificationPeriodType = entity.NotificationPeriodType;
			request.PeriodLimit = entity.PeriodLimit;
			//request.State = entity.State;
			//request.CreatedAt = entity.CreatedAt;
			//request.ModifiedAt = entity.ModifiedAt;
			return request;
		}

		public NotificationAvailabilityPeriodRequest ToNotificationEditAvailabilityPeriodRequest(PortalAlertEntity.NotificationAvailabilityPeriodEdit entity)
		{
			NotificationAvailabilityPeriodRequest request = new NotificationAvailabilityPeriodRequest();
			request.Id = entity.Id;
			request.NotificationId = entity.NotificationId;
			request.AvailabilityPeriodType = entity.AvailabilityPeriodType;
			request.PeriodType = entity.PeriodType;
			request.StartTime = entity.StartTime;
			request.EndTime = entity.EndTime;
			//request.State = entity.State;
			//request.CreatedAt = entity.CreatedAt;
			//request.ModifiedAt = entity.ModifiedAt;
			return request;
		}
	}
}
