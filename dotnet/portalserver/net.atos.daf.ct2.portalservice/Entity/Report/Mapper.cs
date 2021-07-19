using System;
using System.Collections.Generic;
using System.Linq;
using net.atos.daf.ct2.reportservice;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public partial class Mapper
    {
        internal reportservice.UserPreferenceCreateRequest MapCreateUserPrefences(UserPreferenceCreateRequest objUserPreferenceCreateRequest)
        {
            reportservice.UserPreferenceCreateRequest obj
                   = new reportservice.UserPreferenceCreateRequest();

            obj.OrganizationId = objUserPreferenceCreateRequest.OrganizationId;
            obj.ReportId = objUserPreferenceCreateRequest.ReportId;
            obj.AccountId = objUserPreferenceCreateRequest.AccountId;
            obj.ReportId = objUserPreferenceCreateRequest.ReportId;
            //obj.Type = objUserPreferenceCreateRequest.Type.ToString();
            //obj.CharType = objUserPreferenceCreateRequest.ChartType.ToString();
            obj.CreatedAt = objUserPreferenceCreateRequest.CreatedAt;
            obj.ModifiedAt = objUserPreferenceCreateRequest.ModifiedAt;

            for (int i = 0; i < objUserPreferenceCreateRequest.AtributesShowNoShow.Count; i++)
            {
                obj.AtributesShowNoShow.Add(new reportservice.Atribute()
                {
                    DataAttributeId = objUserPreferenceCreateRequest.AtributesShowNoShow[i].DataAttributeId,
                    State = objUserPreferenceCreateRequest.AtributesShowNoShow[i].State.ToString(),
                    Type = objUserPreferenceCreateRequest.AtributesShowNoShow[i].Type,
                    CharType = objUserPreferenceCreateRequest.AtributesShowNoShow[i].ChartType,
                    ThresholdType = objUserPreferenceCreateRequest.AtributesShowNoShow[i].ThresholdType,
                    ThresholdValue = objUserPreferenceCreateRequest.AtributesShowNoShow[i].ThresholdValue
                });
            }
            return obj;
        }

        internal reportservice.CreateEcoScoreProfileRequest MapCreateEcoScoreProfile(EcoScoreProfileCreateRequest request)
        {
            var grpcRequest = new reportservice.CreateEcoScoreProfileRequest
            {
                Name = request.Name,
                Description = request.Description,
                IsDAFStandard = request.IsDAFStandard
            };

            foreach (var kpi in request.ProfileKPIs)
            {
                grpcRequest.ProfileKPIs.Add(new reportservice.CreateEcoScoreProfileKPI()
                {
                    KPIId = kpi.KPIId,
                    LimitValue = kpi.LimitValue,
                    TargetValue = kpi.TargetValue,
                    LowerValue = kpi.LowerValue,
                    UpperValue = kpi.UpperValue,
                    LimitType = Convert.ToString(kpi.LimitType)
                });
            }
            return grpcRequest;
        }

        internal reportservice.UpdateEcoScoreProfileRequest MapUpdateEcoScoreProfile(EcoScoreProfileUpdateRequest request)
        {
            var grpcRequest = new reportservice.UpdateEcoScoreProfileRequest();

            grpcRequest.ProfileId = request.ProfileId;
            grpcRequest.Name = request.Name;
            grpcRequest.Description = request.Description;

            foreach (var kpi in request.ProfileKPIs)
            {
                grpcRequest.ProfileKPIs.Add(new reportservice.CreateEcoScoreProfileKPI()
                {
                    KPIId = kpi.KPIId,
                    LimitValue = kpi.LimitValue,
                    TargetValue = kpi.TargetValue,
                    LowerValue = kpi.LowerValue,
                    UpperValue = kpi.UpperValue
                });
            }
            return grpcRequest;
        }

        internal reportservice.GetEcoScoreReportByAllDriversRequest MapEcoScoreReportByAllDriver(EcoScoreReportByAllDriversRequest request)
        {
            var grpcRequest = new reportservice.GetEcoScoreReportByAllDriversRequest
            {
                StartDateTime = request.StartDateTime,
                EndDateTime = request.EndDateTime,
                MinTripDistance = request.MinTripDistance,
                MinDriverTotalDistance = request.MinDriverTotalDistance,
                TargetProfileId = request.TargetProfileId,
                ReportId = request.ReportId
            };
            grpcRequest.VINs.AddRange(request.VINs);
            return grpcRequest;
        }

        internal reportservice.GetEcoScoreReportCompareDriversRequest MapEcoScoreReportCompareDriver(EcoScoreReportCompareDriversRequest request)
        {
            var grpcRequest = new reportservice.GetEcoScoreReportCompareDriversRequest
            {
                StartDateTime = request.StartDateTime,
                EndDateTime = request.EndDateTime,
                MinTripDistance = request.MinTripDistance,
                MinDriverTotalDistance = request.MinDriverTotalDistance,
                TargetProfileId = request.TargetProfileId,
                ReportId = request.ReportId
            };
            grpcRequest.VINs.AddRange(request.VINs);
            grpcRequest.DriverIds.AddRange(request.DriverIds);
            return grpcRequest;
        }

        internal reportservice.GetEcoScoreReportSingleDriverRequest MapEcoScoreReportSingleDriver(EcoScoreReportSingleDriverRequest request)
        {
            var grpcRequest = new reportservice.GetEcoScoreReportSingleDriverRequest
            {
                StartDateTime = request.StartDateTime,
                EndDateTime = request.EndDateTime,
                DriverId = request.DriverId,
                MinTripDistance = request.MinTripDistance,
                MinDriverTotalDistance = request.MinDriverTotalDistance,
                TargetProfileId = request.TargetProfileId,
                ReportId = request.ReportId
            };
            grpcRequest.VINs.AddRange(request.VINs);
            return grpcRequest;
        }

        /// <summary>
        /// Initially created for Eco Score report. Later can be generalized.
        /// </summary>
        /// <param name="objUserPreferenceCreateRequest"></param>
        /// <param name="accountId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        internal reportservice.ReportUserPreferenceCreateRequest MapCreateReportUserPreferences(ReportUserPreferenceCreateRequest objUserPreferenceCreateRequest,
                                                                                                int accountId, int orgId)
        {
            reportservice.ReportUserPreferenceCreateRequest objRequest = new reportservice.ReportUserPreferenceCreateRequest();

            objRequest.ReportId = objUserPreferenceCreateRequest.ReportId;
            objRequest.AccountId = accountId;
            objRequest.OrganizationId = orgId;

            foreach (var attribute in objUserPreferenceCreateRequest.Attributes)
            {
                objRequest.Attributes.Add(new reportservice.UserPreferenceAttribute()
                {
                    DataAttributeId = attribute.DataAttributeId,
                    State = (int)attribute.State.ToCharArray().First(),
                    Type = (int)attribute.PreferenceType.ToCharArray().First(),
                    ChartType = !string.IsNullOrEmpty(attribute.ChartType) ? (int)attribute.ChartType.ToCharArray().First() : 0,
                    ThresholdType = !string.IsNullOrEmpty(attribute.ThresholdType) ? (int)attribute.ThresholdType.ToCharArray().First() : 0,
                    ThresholdValue = attribute.ThresholdValue
                });
            }
            return objRequest;
        }

        public net.atos.daf.ct2.portalservice.Entity.POI.POIResponse ToPOIEntity(poiservice.POIData poiResponseData)
        {
            net.atos.daf.ct2.portalservice.Entity.POI.POIResponse poi = new net.atos.daf.ct2.portalservice.Entity.POI.POIResponse();
            poi.Id = poiResponseData.Id;
            poi.OrganizationId = poiResponseData.OrganizationId != null ? poiResponseData.OrganizationId.Value : 0;
            poi.CategoryId = poiResponseData.CategoryId;
            poi.SubCategoryId = poiResponseData.SubCategoryId != null ? poiResponseData.SubCategoryId.Value : 0;
            poi.Name = poiResponseData.Name;
            poi.SubCategoryName = poiResponseData.SubCategoryName;
            poi.CategoryName = poiResponseData.CategoryName;
            //poi.Type = poiResponseData.Type;
            poi.Address = poiResponseData.Address;
            poi.City = poiResponseData.City;
            poi.Country = poiResponseData.Country;
            poi.Zipcode = poiResponseData.Zipcode;
            poi.Latitude = Convert.ToDouble(poiResponseData.Latitude);
            poi.Longitude = Convert.ToDouble(poiResponseData.Longitude);
            //poi.Distance = Convert.ToDouble(poiResponseData.Distance);
            poi.State = poiResponseData.State;
            poi.CreatedAt = poiResponseData.CreatedAt;
            poi.Icon = poiResponseData.Icon != null ? poiResponseData.Icon.ToByteArray() : new Byte[] { };
            return poi;
        }

        public ReportFleetOverviewFilter ToFleetOverviewEntity(FleetOverviewFilterResponse fleetOverviewFilterResponse)
        {
            ReportFleetOverviewFilter reportFleetOverview = new ReportFleetOverviewFilter();
            reportFleetOverview.VehicleGroups = new List<VehicleGroup>();
            foreach (var item in fleetOverviewFilterResponse.FleetOverviewVGFilterResponse)
            {
                VehicleGroup vehicleGroup = new VehicleGroup();
                vehicleGroup.VehicleGroupId = item.VehicleGroupId;
                vehicleGroup.VehicleGroupName = fleetOverviewFilterResponse.AssociatedVehicleRequest.Where(x => x.VehicleGroupId == item.VehicleGroupId).Select(x => x.VehicleGroupName).FirstOrDefault();
                vehicleGroup.VehicleId = item.VehicleId;
                vehicleGroup.FeatureName = item.FeatureName;
                vehicleGroup.FeatureKey = item.FeatureKey;
                vehicleGroup.Subscribe = item.Subscribe;
                reportFleetOverview.VehicleGroups.Add(vehicleGroup);
            }
            reportFleetOverview.AlertLevel = new List<FilterProperty>();
            foreach (var item in fleetOverviewFilterResponse.ALFilterResponse)
            {
                FilterProperty alertLevel = new FilterProperty();
                alertLevel.Name = item.Name;
                alertLevel.Value = item.Value;
                reportFleetOverview.AlertLevel.Add(alertLevel);
            }
            reportFleetOverview.AlertCategory = new List<FilterProperty>();
            foreach (var item in fleetOverviewFilterResponse.ACFilterResponse)
            {
                FilterProperty alertCategory = new FilterProperty();
                alertCategory.Name = item.Name;
                alertCategory.Value = item.Value;
                reportFleetOverview.AlertCategory.Add(alertCategory);
            }
            reportFleetOverview.HealthStatus = new List<FilterProperty>();
            foreach (var item in fleetOverviewFilterResponse.HSFilterResponse)
            {
                FilterProperty healthStatus = new FilterProperty();
                healthStatus.Name = item.Name;
                healthStatus.Value = item.Value;
                reportFleetOverview.HealthStatus.Add(healthStatus);
            }
            reportFleetOverview.OtherFilter = new List<FilterProperty>();
            foreach (var item in fleetOverviewFilterResponse.OFilterResponse)
            {
                FilterProperty other = new FilterProperty();
                other.Name = item.Name;
                other.Value = item.Value;
                reportFleetOverview.OtherFilter.Add(other);
            }
            reportFleetOverview.DriverList = new List<DriverFilter>();
            foreach (var item in fleetOverviewFilterResponse.DriverList)
            {
                DriverFilter driver = new DriverFilter();
                driver.DriverId = item.DriverId;
                driver.FirstName = item.FirstName;
                driver.LastName = item.LastName;
                driver.OrganizationId = item.OrganizationId;
                reportFleetOverview.DriverList.Add(driver);
            }
            reportFleetOverview.FleetOverviewAlerts = new List<FleetOverviewAlert>();
            foreach (var item in fleetOverviewFilterResponse.LogbookTripAlertDetailsRequest)
            {
                FleetOverviewAlert fleetOverviewAlert = new FleetOverviewAlert()
                {
                    AlertId = item.AlertId,
                    AlertName = item.AlertName,
                    AlertType = item.AlertType,
                    AlertLevel = item.AlertLevel,
                    CategoryType = item.AlertCategoryType,
                    AlertLatitude = item.AlertLatitude,
                    AlertLongitude = item.AlertLongitude,
                    AlertGeolocationAddressId = item.AlertGeolocationAddressId,
                    AlertGeolocationAddress = item.AlertGeolocationAddress,
                    AlertTime = item.AlertGeneratedTime,
                    ProcessedMessageTimestamp = item.ProcessedMessageTimestamp
                };
                reportFleetOverview.FleetOverviewAlerts.Add(fleetOverviewAlert);

            }

            return reportFleetOverview;
        }

    }
}
