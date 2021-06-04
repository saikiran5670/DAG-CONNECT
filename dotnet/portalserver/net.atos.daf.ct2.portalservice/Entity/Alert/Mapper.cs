using net.atos.daf.ct2.alertservice;
using net.atos.daf.ct2.vehicleservice;
using PortalAlertEntity = net.atos.daf.ct2.portalservice.Entity.Alert;

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
            //request.PositionType = entity.PositionType;
            //for (int i = 0; i < entity.DayType.Length; i++)
            //{
            //    request.DayType.Add(entity.DayType[i]);
            //}
            //request.PeriodType = entity.PeriodType;
            //request.FilterStartDate = entity.FilterStartDate;
            //request.FilterEndDate = entity.FilterEndDate;
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
            if (entity.AlertTimingDetails.Count > 0)
            {
                foreach (var item in entity.AlertTimingDetails)
                {
                    request.AlertTimingDetail.Add(MapAlertTimingDetailEntity(item));
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
            //request.PositionType = entity.PositionType;
            //for (int i = 0; i < entity.DayType.Length; i++)
            //{
            //    request.DayType.Add(entity.DayType[i]);
            //}
            //request.PeriodType = entity.PeriodType;
            //request.FilterStartDate = entity.FilterStartDate;
            //request.FilterEndDate = entity.FilterEndDate;
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
            if (entity.AlertTimingDetails.Count > 0)
            {
                foreach (var item in entity.AlertTimingDetails)
                {
                    request.AlertTimingDetail.Add(ToAlertTimingDetailEntityRequest(item));
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

        public EnumTranslation MapEnumTranslation(net.atos.daf.ct2.alertservice.EnumTranslation enumTrans)
        {
            EnumTranslation objenumtrans = new EnumTranslation();
            objenumtrans.Id = enumTrans.Id;
            objenumtrans.Type = string.IsNullOrEmpty(enumTrans.Type) ? string.Empty : enumTrans.Type;
            objenumtrans.Enum = string.IsNullOrEmpty(enumTrans.Enum) ? string.Empty : enumTrans.Enum;
            objenumtrans.ParentEnum = string.IsNullOrEmpty(enumTrans.ParentEnum) ? string.Empty : enumTrans.ParentEnum;
            objenumtrans.Key = string.IsNullOrEmpty(enumTrans.Key) ? string.Empty : enumTrans.Key;
            return objenumtrans;
        }

        public VehicleGroup MapVehicleGroup(VehicleGroupList vehiclegroup)
        {
            VehicleGroup objvehiclegroup = new VehicleGroup();
            objvehiclegroup.VehicleGroupId = vehiclegroup.VehicleGroupId;
            objvehiclegroup.VehicleGroupName = string.IsNullOrEmpty(vehiclegroup.VehicleGroupName) ? string.Empty : vehiclegroup.VehicleGroupName;
            objvehiclegroup.VehicleId = vehiclegroup.VehicleId;
            objvehiclegroup.VehicleName = string.IsNullOrEmpty(vehiclegroup.VehicleName) ? string.Empty : vehiclegroup.VehicleName;
            objvehiclegroup.Vin = string.IsNullOrEmpty(vehiclegroup.Vin) ? string.Empty : vehiclegroup.Vin;
            objvehiclegroup.RegNo = string.IsNullOrEmpty(vehiclegroup.RegNo) ? string.Empty : vehiclegroup.RegNo;
            objvehiclegroup.SubcriptionStatus = vehiclegroup.SubcriptionStatus;
            return objvehiclegroup;
        }

        public AlertTimingDetail ToAlertTimingDetailEntity(AlertTimingDetailRequest request)
        {
            AlertTimingDetailEdit alertTimingDetail = new AlertTimingDetailEdit();
            alertTimingDetail.Id = request.Id;
            alertTimingDetail.Type = request.Type;
            alertTimingDetail.RefId = request.RefId;
            alertTimingDetail.PeriodType = request.PeriodType;
            alertTimingDetail.StartDate = request.StartDate;
            alertTimingDetail.EndDate = request.EndDate;
            alertTimingDetail.State = request.State;
            return alertTimingDetail;
        }
        public AlertTimingDetailRequest MapAlertTimingDetailEntity(AlertTimingDetailEdit request)
        {
            AlertTimingDetailRequest alertTimingDetailRequest = new AlertTimingDetailRequest();
            alertTimingDetailRequest.Id = request.Id;
            alertTimingDetailRequest.Type = request.Type;
            alertTimingDetailRequest.RefId = request.RefId;
            alertTimingDetailRequest.PeriodType = string.IsNullOrEmpty(request.PeriodType) ? string.Empty : request.PeriodType;
            alertTimingDetailRequest.StartDate = request.StartDate;
            alertTimingDetailRequest.EndDate = request.EndDate;
            alertTimingDetailRequest.State = request.State;
            return alertTimingDetailRequest;
        }
        public AlertTimingDetailRequest MapAlertTimingDetailEntity(AlertTimingDetail request)
        {
            AlertTimingDetailRequest alertTimingDetailRequest = new AlertTimingDetailRequest();
            alertTimingDetailRequest.Type = request.Type;
            alertTimingDetailRequest.RefId = request.RefId;
            alertTimingDetailRequest.PeriodType = string.IsNullOrEmpty(request.PeriodType) ? string.Empty : request.PeriodType;
            alertTimingDetailRequest.StartDate = request.StartDate;
            alertTimingDetailRequest.EndDate = request.EndDate;
            alertTimingDetailRequest.State = request.State;
            return alertTimingDetailRequest;
        }
        public AlertTimingDetailRequest ToAlertTimingDetailEntityRequest(AlertTimingDetailEdit request)
        {
            AlertTimingDetailRequest alertTimingDetailRequest = new AlertTimingDetailRequest();
            alertTimingDetailRequest.Id = request.Id;
            alertTimingDetailRequest.Type = request.Type;
            alertTimingDetailRequest.RefId = request.RefId;
            alertTimingDetailRequest.PeriodType = string.IsNullOrEmpty(request.PeriodType) ? string.Empty : request.PeriodType;
            alertTimingDetailRequest.StartDate = request.StartDate;
            alertTimingDetailRequest.EndDate = request.EndDate;
            alertTimingDetailRequest.State = request.State;
            return alertTimingDetailRequest;
        }

    }
}
