﻿using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reports.repository;
using System.Linq;
using net.atos.daf.ct2.reports.entity.fleetFuel;
using System;

namespace net.atos.daf.ct2.reports
{
    public class ReportManager : IReportManager
    {
        private readonly IReportRepository _reportRepository;

        public ReportManager(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        #region Select User Preferences

        public Task<IEnumerable<ReportDetails>> GetReportDetails()
        {
            return _reportRepository.GetReportDetails();
        }

        public Task<bool> CheckIfUserPreferencesExist(int reportId, int accountId, int organizationId)
        {
            return _reportRepository.CheckIfUserPreferencesExist(reportId, accountId, organizationId);
        }

        public Task<IEnumerable<UserPreferenceReportDataColumn>> GetReportUserPreference(int reportId, int accountId, int organizationId)
        {
            return _reportRepository.GetReportUserPreference(reportId, accountId, organizationId);
        }

        public Task<IEnumerable<UserPreferenceReportDataColumn>> GetRoleBasedDataColumn(int reportId, int accountId, int roleId,
                                                                                       int organizationId, int contextOrgId)
        {
            return _reportRepository.GetRoleBasedDataColumn(reportId, accountId, roleId, organizationId, contextOrgId);
        }
        #endregion

        #region Create User Preferences
        public async Task<int> CreateUserPreference(UserPreferenceCreateRequest objUserPreferenceRequest)
        {
            return await _reportRepository.CreateUserPreference(objUserPreferenceRequest);
        }
        #endregion

        #region Get Vins from data mart trip_statistics
        public Task<IEnumerable<VehicleFromTripDetails>> GetVinsFromTripStatistics(IEnumerable<string> vinList)
        {
            return _reportRepository.GetVinsFromTripStatistics(vinList);
        }
        #endregion

        #region Trip Report Table Details

        public async Task<List<TripDetails>> GetFilteredTripDetails(TripFilterRequest tripFilter,
                                                                    bool isLiveFleetRequired = true) => await _reportRepository.GetFilteredTripDetails(tripFilter, isLiveFleetRequired);

        #endregion

        #region Driver Time management Report
        /// <summary>
        /// Fetch Multiple Drivers activity data and group by name with all type duraion aggregate
        /// </summary>
        /// <param name="DriverActivityFilter">Filters for driver activity with VIN and Driver ID </param>
        /// <returns></returns>
        public async Task<List<DriversActivities>> GetDriversActivity(DriverActivityFilter driverActivityFilter)
        {
            List<DriversActivities> driverActivities = await _reportRepository.GetDriversActivity(driverActivityFilter);
            List<DriversActivities> combineDriverActivities = new List<DriversActivities>();
            combineDriverActivities = driverActivities.GroupBy(activityGroup => activityGroup.DriverId)
                                                      .Select(activityItem => new DriversActivities
                                                      {
                                                          DriverName = activityItem.FirstOrDefault().DriverName,
                                                          DriverId = activityItem.FirstOrDefault().DriverId,
                                                          ActivityDate = activityItem.FirstOrDefault().ActivityDate,
                                                          Code = activityItem.FirstOrDefault().Code,
                                                          VIN = activityItem.FirstOrDefault().VIN,
                                                          StartTime = activityItem.Max(mx => mx.StartTime),
                                                          EndTime = activityItem.Max(mn => mn.EndTime),
                                                          AvailableTime = activityItem.Sum(c => c.AvailableTime),
                                                          DriveTime = activityItem.Sum(c => c.DriveTime),
                                                          RestTime = activityItem.Sum(c => c.RestTime),
                                                          WorkTime = activityItem.Sum(c => c.WorkTime),
                                                          ServiceTime = activityItem.Sum(c => c.ServiceTime),
                                                      }).ToList();
            return combineDriverActivities;

        }

        /// <summary>
        /// Fetch Single driver activities data by Day group
        /// </summary>
        /// <param name="DriverActivityFilter">Filters for driver activity with VIN and Driver ID </param>
        /// <returns></returns>
        public async Task<List<DriversActivities>> GetDriverActivity(DriverActivityFilter driverActivityFilter) => await _reportRepository.GetDriversActivity(driverActivityFilter);

        public async Task<List<Driver>> GetDriversByVIN(long startDateTime, long endDateTime, List<string> vin, int organizationId)
        {
            return await _reportRepository.GetDriversByVIN(startDateTime, endDateTime, vin, organizationId);
        }
        public async Task<object> GetReportSearchParameterByVIN(int reportID, long startDateTime, long endDateTime, List<string> vin)
        {
            return await _reportRepository.GetReportSearchParameterByVIN(reportID, startDateTime, endDateTime, vin);
        }

        /// <summary>
        /// Fetch Single driver activities data for Stack Bar chart
        /// </summary>
        /// <param name="DriverActivityChartFilter">Filters for driver activity with VIN and Driver ID </param>
        /// <returns></returns>
        public async Task<List<DriverActivityChart>> GetDriversActivityChartDetails(DriverActivityChartFilter driverActivityFilter) => await _reportRepository.GetDriversActivityChartDetails(driverActivityFilter);
        #endregion

        #region Eco Score Report

        #region Eco Score Report - Create Profile

        public async Task<int> CreateEcoScoreProfile(EcoScoreProfileDto dto)
        {
            var isExist = await _reportRepository.CheckEcoScoreProfileIsExist(dto.OrganizationId, dto.Name, dto.Id);
            if (!isExist)// check if profile is avilable in DB or not
            {
                return await _reportRepository.CreateEcoScoreProfile(dto);
            }
            else
                return -1;
        }

        public async Task<int> GetEcoScoreProfilesCount(int orgId)
        {
            return await _reportRepository.GetEcoScoreProfilesCount(orgId);
        }

        #endregion

        #region Eco Score Report - Get Profile and KPI Details
        public async Task<List<EcoScoreProfileDto>> GetEcoScoreProfiles(int orgId)
        {
            return await _reportRepository.GetEcoScoreProfiles(orgId);
        }

        public async Task<EcoScoreProfileDto> GetEcoScoreProfileKPIDetails(int profileId)
        {
            return await _reportRepository.GetEcoScoreProfileKPIDetails(profileId);
        }
        #endregion

        #region  Eco Score Report - Update/Delete Profile
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ecoScoreProfileDto"></param>
        /// <param name="isAdminRights"></param>
        /// <returns> return -2 = Is a default profile, Can't update.</returns>
        /// /// <returns> return -1 = does not exist to update.</returns>
        public async Task<int> UpdateEcoScoreProfile(EcoScoreProfileDto ecoScoreProfileDto, bool isAdminRights)
        {
            // Default Profile for basic and advance -	DAF Admin – Not Allowed Update Profile Name , Allowed  Rest profile KPIs modifications  2) Org Admin – nothing Allowed
            // Custom profile(Global) -	DAF Admin – All allowed 2) Org Admin – nothing Allowed
            // Custom profile(Org) – DAF Admin – All allowed  2)Org Admin – Allowed(Based on Role and Subscription)
            if (ecoScoreProfileDto.IsDAFStandard)
            {
                ecoScoreProfileDto.OrganizationId = 0;
            }
            var isExist = await _reportRepository.CheckEcoScoreProfileIsExist(ecoScoreProfileDto.OrganizationId, ecoScoreProfileDto.Name, ecoScoreProfileDto.Id);
            if (!isExist)// check if profile is avilable in DB or not
            {
                string versionType = await _reportRepository.IsEcoScoreProfileBasicOrAdvance(ecoScoreProfileDto.Id);
                bool isGlobalProfile = await _reportRepository.GetGlobalProfile(ecoScoreProfileDto.Id);
                if (!string.IsNullOrEmpty(versionType))// check if it is basic or advance versiontype= "B" or "A"
                {
                    if (isAdminRights)// admin rights with level 10 & 20
                    {
                        ecoScoreProfileDto.Name = null;
                        return await _reportRepository.UpdateEcoScoreProfile(ecoScoreProfileDto, isAdminRights); // DAF Admin – Not Allowed Update Profile Name 
                    }
                }
                else if (versionType == null)
                {
                    if (isGlobalProfile)
                    {
                        if (isAdminRights)
                        {
                            return await _reportRepository.UpdateEcoScoreProfile(ecoScoreProfileDto, isAdminRights);
                        }
                        return -3;
                    }
                    else
                    {
                        return await _reportRepository.UpdateEcoScoreProfile(ecoScoreProfileDto, isAdminRights);
                    }
                }
                return -2;
            }
            else
                return -1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="isAdminRights"></param>
        /// <returns> return -2 = Is a default profile, Can't delete.</returns>
        /// /// <returns> return -1 = Is a global profile, Can't delete.</returns>
        public async Task<int> DeleteEcoScoreProfile(int profileId, bool isAdminRights)
        {
            int ecoScoreProfileId;
            string versionType = await _reportRepository.IsEcoScoreProfileBasicOrAdvance(profileId);

            if (string.IsNullOrEmpty(versionType))
            {
                bool isGlobalProfile = await _reportRepository.GetGlobalProfile(profileId);

                if (isGlobalProfile)
                {
                    if (isAdminRights)
                    {
                        ecoScoreProfileId = await _reportRepository.DeleteEcoScoreProfile(profileId);
                        return ecoScoreProfileId;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return await _reportRepository.DeleteEcoScoreProfile(profileId);
                }

            }
            return -2;
        }
        #endregion

        #region Eco Score Report By All Drivers
        public async Task<List<EcoScoreReportByAllDrivers>> GetEcoScoreReportByAllDrivers(EcoScoreReportByAllDriversRequest request)
        {
            //Update Target Profile for User Preferences
            await _reportRepository.UpdateEcoScoreTargetProfile(request);

            List<EcoScoreReportByAllDrivers> lstDriverRanking = await _reportRepository.GetEcoScoreReportByAllDrivers(request);
            var lstByAllDrivers = new List<EcoScoreReportByAllDrivers>();
            var objEcoScoreKPI = await _reportRepository.GetEcoScoreTargetProfileKPIValues(request.TargetProfileId);
            if (objEcoScoreKPI != null)
            {
                foreach (var driver in lstDriverRanking)
                {
                    //< Min = Red
                    if (driver.EcoScoreRanking <= objEcoScoreKPI.MinValue)
                        driver.EcoScoreRankingColor = RankingColor.Red.ToString();
                    //> Target = Green
                    else if (driver.EcoScoreRanking >= objEcoScoreKPI.TargetValue)
                        driver.EcoScoreRankingColor = RankingColor.Green.ToString();
                    //Between Min and Target = Amber
                    else
                        driver.EcoScoreRankingColor = RankingColor.Amber.ToString();

                    lstByAllDrivers.Add(driver);
                }
                return lstByAllDrivers;
            }
            else
                return lstDriverRanking;
        }
        #endregion

        #region Eco Score Report - User Preferences

        public async Task<bool> CreateReportUserPreference(ReportUserPreferenceCreateRequest request)
        {
            return await _reportRepository.CreateReportUserPreference(request);
        }

        public async Task<bool> CheckIfReportUserPreferencesExist(int reportId, int accountId, int organizationId, int[] featureIds)
        {
            return await _reportRepository.CheckIfReportUserPreferencesExist(reportId, accountId, organizationId, featureIds);
        }

        public async Task<IEnumerable<ReportUserPreference>> GetReportUserPreferences(int reportId, int accountId, int organizationId, int[] featureIds)
        {
            return await _reportRepository.GetReportUserPreferences(reportId, accountId, organizationId, featureIds);
        }

        public async Task<IEnumerable<ReportUserPreference>> GetPrivilegeBasedReportUserPreferences(int reportId, int accountId, int roleId,
                                                                                       int organizationId, int contextOrgId, int[] featureId)
        {
            return await _reportRepository.GetPrivilegeBasedReportUserPreferences(reportId, accountId, roleId, organizationId, contextOrgId, featureId);
        }
        public async Task<IEnumerable<ReportUserPreference>> GetReportDataAttributes(int[] featureIds, int reportId)
        {
            return await _reportRepository.GetReportDataAttributes(featureIds, reportId);
        }

        public async Task<IEnumerable<int>> GetReportFeatureId(int reportId)
        {
            return await _reportRepository.GetReportFeatureId(reportId);
        }

        public async Task<IEnumerable<ReportUserPreference>> GetReportDataAttributes(List<int> reportIds)
        {
            return await _reportRepository.GetReportDataAttributes(reportIds);
        }

        public async Task<SubReportDto> CheckIfSubReportExist(int reportId)
        {
            return await _reportRepository.CheckIfSubReportExist(reportId);
        }

        #endregion

        #region Eco Score Report Compare Drivers
        public async Task<List<EcoScoreReportCompareDrivers>> GetEcoScoreReportCompareDrivers(EcoScoreReportCompareDriversRequest request)
        {
            return await _reportRepository.GetEcoScoreReportCompareDrivers(request);
        }

        public async Task<List<EcoScoreCompareReportAtttributes>> GetEcoScoreCompareReportAttributes(int reportId, int targetProfileId)
        {
            return await _reportRepository.GetEcoScoreCompareReportAttributes(reportId, targetProfileId);
        }
        #endregion

        #region Eco Score Report Single Driver
        public async Task<List<EcoScoreReportSingleDriver>> GetEcoScoreReportSingleDriver(EcoScoreReportSingleDriverRequest request)
        {
            var lstSingleDriver = new List<EcoScoreReportSingleDriver>();

            var objOverallDriver = await _reportRepository.GetEcoScoreReportOverallDriver(request);
            if (objOverallDriver != null)
                lstSingleDriver.AddRange(objOverallDriver);

            var objOverallCompany = await _reportRepository.GetEcoScoreReportOverallCompany(request);
            if (objOverallDriver != null)
                lstSingleDriver.AddRange(objOverallCompany);

            var lstVINDriver = await _reportRepository.GetEcoScoreReportVINDriver(request);
            if (lstVINDriver.Count > 0)
                lstSingleDriver.AddRange(lstVINDriver);

            var lstVINCompany = await _reportRepository.GetEcoScoreReportVINCompany(request);
            if (lstVINCompany.Count > 0)
                lstSingleDriver.AddRange(lstVINCompany);

            return lstSingleDriver;
        }
        public async Task<List<EcoScoreSingleDriverBarPieChart>> GetEcoScoreAverageGrossWeightChartData(EcoScoreReportSingleDriverRequest request)
        {
            return await _reportRepository.GetEcoScoreAverageGrossWeightChartData(request);
        }
        public async Task<List<EcoScoreSingleDriverBarPieChart>> GetEcoScoreAverageDrivingSpeedChartData(EcoScoreReportSingleDriverRequest request)
        {
            return await _reportRepository.GetEcoScoreAverageDrivingSpeedChartData(request);
        }
        public async Task<List<EcoScoreReportSingleDriver>> GetEcoScoreReportTrendlineData(EcoScoreReportSingleDriverRequest request)
        {
            var lstSingleDriver = new List<EcoScoreReportSingleDriver>();
            try
            {
                var objOverallDriver = await _reportRepository.GetEcoScoreReportOverallDriverForTrendline(request);
                if (objOverallDriver != null)
                    lstSingleDriver.AddRange(objOverallDriver);
                var objOverallCompany = await _reportRepository.GetEcoScoreReportOverallCompanyForTrendline(request);
                if (objOverallCompany != null)
                    lstSingleDriver.AddRange(objOverallCompany);
                var lstVINDriver = await _reportRepository.GetEcoScoreReportVINDriverForTrendline(request);
                if (lstVINDriver.Count > 0)
                    lstSingleDriver.AddRange(lstVINDriver);
                var lstVINCompany = await _reportRepository.GetEcoScoreReportVinCompanyForTrendline(request);
                if (lstVINCompany != null)
                    lstSingleDriver.AddRange(lstVINCompany);
            }
            catch (Exception)
            {
                throw;
            }

            return lstSingleDriver;
        }
        #endregion

        #endregion

        #region Fleet Utilizaiton Report
        public async Task<List<FleetUtilizationDetails>> GetFleetUtilizationDetails(FleetUtilizationFilter fleetFilter)
        {
            List<FleetUtilizationDetails> lstFleetUtilizationDetails = await _reportRepository.GetFleetUtilizationDetails(fleetFilter);
            return lstFleetUtilizationDetails;
        }

        public async Task<List<Calender_Fleetutilization>> GetCalenderData(FleetUtilizationFilter tripFilters)
        {
            List<Calender_Fleetutilization> lstFleetUtilizationDetails = await _reportRepository.GetCalenderData(tripFilters);
            return lstFleetUtilizationDetails;
        }

        #endregion

        #region FleetOverview
        public async Task<List<AlertCategory>> GetAlertCategoryList()
        {
            List<AlertCategory> lstAlertCategory = await _reportRepository.GetAlertCategoryList();
            return lstAlertCategory;
        }
        public async Task<List<FilterProperty>> GetAlertLevelList()
        {
            List<FilterProperty> lstAlertLevel = await _reportRepository.GetAlertLevelList();
            return lstAlertLevel;
        }
        public async Task<List<FilterProperty>> GetHealthStatusList()
        {
            List<FilterProperty> lstHealthStatus = await _reportRepository.GetHealthStatusList();
            return lstHealthStatus;
        }
        public async Task<List<FilterProperty>> GetOtherFilter()
        {
            List<FilterProperty> lstHealthStatus = await _reportRepository.GetOtherFilter();
            return lstHealthStatus;
        }
        public async Task<List<FleetOverviewDetails>> GetFleetOverviewDetails_NeverMoved(FleetOverviewFilter fleetOverviewFilter)
        {
            List<FleetOverviewDetails> fleetOverviewDetails = await _reportRepository.GetFleetOverviewDetails_NeverMoved(fleetOverviewFilter);
            return fleetOverviewDetails;
        }
        public async Task<List<FleetOverviewDetails>> GetFleetOverviewDetails_NeverMoved_NoWarnings(FleetOverviewFilter fleetOverviewFilter)
        {
            List<FleetOverviewDetails> fleetOverviewDetails = await _reportRepository.GetFleetOverviewDetails_NeverMoved_NoWarnings(fleetOverviewFilter);
            return fleetOverviewDetails;
        }
        public async Task<List<FleetOverviewDetails>> GetFleetOverviewDetails(FleetOverviewFilter fleetOverviewFilter)
        {
            List<FleetOverviewDetails> fleetOverviewDetails = await _reportRepository.GetFleetOverviewDetails(fleetOverviewFilter);
            return fleetOverviewDetails;
        }
        public async Task<List<DriverFilter>> GetDriverList(List<string> vins, int organizationId)
        {
            List<DriverFilter> lstDriver = await _reportRepository.GetDriverList(vins, organizationId);
            return lstDriver;
        }
        public async Task<List<WarningDetails>> GetWarningDetails(List<int> warningClass, List<int> warningNumber, string lngCode)
        {
            List<WarningDetails> lstWarningDetails = await _reportRepository.GetWarningDetails(warningClass, warningNumber, lngCode);
            return lstWarningDetails;
        }
        public async Task<List<DriverDetails>> GetDriverDetails(List<string> driverIds, int organizationId)
        {
            List<DriverDetails> lstDriverDetails = await _reportRepository.GetDriverDetails(driverIds, organizationId);
            return lstDriverDetails;
        }


        #endregion

        #region Feet Fuel Report

        public async Task<List<FleetFuelDetails>> GetFleetFuelDetailsByVehicle(FleetFuelFilter fleetFuelFilters)
        {
            List<FleetFuelDetails> lstFleetFuelDetails = await _reportRepository.GetFleetFuelDetailsByVehicle(fleetFuelFilters);
            List<FleetFuelDetails> lstFleetFuelDetailsUpdated = await PrepareDetails(lstFleetFuelDetails, fleetFuelFilters.LanguageCode);
            return lstFleetFuelDetailsUpdated;
        }

        public async Task<List<FleetFuelDetailsByDriver>> GetFleetFuelDetailsByDriver(FleetFuelFilter fleetFuelFilters)
        {
            List<FleetFuelDetailsByDriver> lstFleetFuelDetails = await _reportRepository.GetFleetFuelDetailsByDriver(fleetFuelFilters);
            List<FleetFuelDetailsByDriver> lstFleetFuelDetailsUpdated = await PrepareDetails(lstFleetFuelDetails, fleetFuelFilters.LanguageCode);
            return lstFleetFuelDetailsUpdated;
        }

        public async Task<List<FleetFuel_VehicleGraph>> GetFleetFuelDetailsForVehicleGraphs(FleetFuelFilter fleetFuelFilters)
        {
            List<FleetFuel_VehicleGraph> lstFleetFuelDetails = await _reportRepository.GetFleetFuelDetailsForVehicleGraphs(fleetFuelFilters);
            return lstFleetFuelDetails;
        }
        public async Task<List<FleetFuel_VehicleGraph>> GetFleetFuelDetailsForDriverGraphs(FleetFuelFilter fleetFuelFilters)
        {
            List<FleetFuel_VehicleGraph> lstFleetFuelDetails = await _reportRepository.GetFleetFuelDetailsForDriverGraphs(fleetFuelFilters);
            return lstFleetFuelDetails;
        }

        public async Task<List<FleetFuelDetails>> GetFleetFuelTripDetailsByVehicle(FleetFuelFilter fleetFuelFilters, bool isLiveFleetRequired = true)
        {
            List<FleetFuelDetails> lstFleetFuelTripDetails = await _reportRepository.GetFleetFuelTripDetailsByVehicle(fleetFuelFilters, isLiveFleetRequired);
            List<FleetFuelDetails> lstFleetFuelDetailsUpdated = await PrepareDetails(lstFleetFuelTripDetails, fleetFuelFilters.LanguageCode);
            return lstFleetFuelDetailsUpdated;
        }

        public async Task<List<FleetFuelDetails>> GetFleetFuelTripDetailsByDriver(FleetFuelFilterDriver fleetFuelFilters)
        {
            List<FleetFuelDetails> lstFleetFuelTripDetails = await _reportRepository.GetFleetFuelTripDetailsByDriver(fleetFuelFilters);
            List<FleetFuelDetails> lstFleetFuelDetailsUpdated = await PrepareDetails(lstFleetFuelTripDetails, fleetFuelFilters.LanguageCode);
            return lstFleetFuelDetailsUpdated;
        }
        /// <summary>
        /// To apply formula and mapped values according to language code
        /// </summary>
        /// <param name="fleetFuelDetails">List of Fleet Fuel result without formula</param>
        /// <param name="languageCode">requested language code </param>
        /// <returns>list of details with formulated values</returns>
        private async Task<List<FleetFuelDetails>> PrepareDetails(List<FleetFuelDetails> fleetFuelDetails, string languageCode)
        {

            List<CO2Coefficient> co2CoEfficientData = await _reportRepository.GetCO2CoEfficientData();
            List<IdlingConsumption> idlingConsumption = await _reportRepository.GetIdlingConsumptionData(languageCode);
            List<AverageTrafficClassification> averageTrafficClassification = await _reportRepository.GetAverageTrafficClassificationData(languageCode);

            Parallel.ForEach(fleetFuelDetails, item =>
            {
                // Mapping expected value (as Modrate, Good, Very Good) from range
                double idlConsumptionHighValue = idlingConsumption.Where(idl => idl.MaxValue <= 0).Select(item => item.MinValue).FirstOrDefault();
                if (((float)item.IdlingConsumption / 1000) > idlConsumptionHighValue)
                {
                    string idlConsumptionValue = idlingConsumption.Where(idl => idl.MaxValue <= 0).Select(item => item.Value).FirstOrDefault();
                    item.IdlingConsumptionValue = idlConsumptionValue;
                }
                else
                {
                    string idlConsumptionValue = idlingConsumption.Where(idl => idl.MaxValue <= ((float)item.IdlingConsumption / 1000) && idl.MinValue >= ((float)item.IdlingConsumption / 1000)).Select(item => item.Value).FirstOrDefault();
                    item.IdlingConsumptionValue = idlConsumptionValue;
                }

                // Mapping expected trafic level (as Low, Mid, High) from range
                double averageTrafficClassificationMaxValue = averageTrafficClassification.Where(idl => idl.MaxValue <= 0).Select(item => item.MinValue).FirstOrDefault();
                if (item.AverageTrafficClassification > averageTrafficClassificationMaxValue)
                {
                    string averageTrafficClassificationValue = averageTrafficClassification.Where(idl => idl.MaxValue <= 0).Select(item => item.Value).FirstOrDefault();
                    item.AverageTrafficClassificationValue = averageTrafficClassificationValue;
                }
                else
                {
                    string averageTrafficClassificationValue = averageTrafficClassification.Where(idl => idl.MaxValue > item.AverageTrafficClassification && idl.MinValue <= item.AverageTrafficClassification).Select(item => item.Value).FirstOrDefault();
                    item.AverageTrafficClassificationValue = averageTrafficClassificationValue;
                }
            });
            return fleetFuelDetails;

        }

        private async Task<List<FleetFuelDetailsByDriver>> PrepareDetails(List<FleetFuelDetailsByDriver> fleetFuelDetails, string languageCode)
        {

            List<CO2Coefficient> co2CoEfficientData = await _reportRepository.GetCO2CoEfficientData();
            List<IdlingConsumption> idlingConsumption = await _reportRepository.GetIdlingConsumptionData(languageCode);
            List<AverageTrafficClassification> averageTrafficClassification = await _reportRepository.GetAverageTrafficClassificationData(languageCode);

            Parallel.ForEach(fleetFuelDetails, item =>
            {
                // Mapping expected value (as Modrate, Good, Very Good) from range
                double idlConsumptionHighValue = idlingConsumption.Where(idl => idl.MaxValue <= 0).Select(item => item.MinValue).FirstOrDefault();
                if (item.IdlingConsumption > idlConsumptionHighValue)
                {
                    string idlConsumptionValue = idlingConsumption.Where(idl => idl.MaxValue <= 0).Select(item => item.Value).FirstOrDefault();
                    item.IdlingConsumptionValue = idlConsumptionValue;
                }
                else
                {
                    string idlConsumptionValue = idlingConsumption.Where(idl => idl.MaxValue <= item.IdlingConsumption && idl.MinValue >= item.IdlingConsumption).Select(item => item.Value).FirstOrDefault();
                    item.IdlingConsumptionValue = idlConsumptionValue;
                }

                // Mapping expected trafic level (as Low, Mid, High) from range
                double averageTrafficClassificationMaxValue = averageTrafficClassification.Where(idl => idl.MaxValue <= 0).Select(item => item.MinValue).FirstOrDefault();
                if (item.AverageTrafficClassification > averageTrafficClassificationMaxValue)
                {
                    string averageTrafficClassificationValue = averageTrafficClassification.Where(idl => idl.MaxValue <= 0).Select(item => item.Value).FirstOrDefault();
                    item.AverageTrafficClassificationValue = averageTrafficClassificationValue;
                }
                else
                {
                    string averageTrafficClassificationValue = averageTrafficClassification.Where(idl => idl.MaxValue > item.AverageTrafficClassification && idl.MinValue <= item.AverageTrafficClassification).Select(item => item.Value).FirstOrDefault();
                    item.AverageTrafficClassificationValue = averageTrafficClassificationValue;
                }
            });
            return fleetFuelDetails;

        }
        #endregion

        #region Eco-Score Data service

        public async Task<EcoScoreKPIInfoDataServiceResponse> GetKPIInfo(EcoScoreDataServiceRequest request)
        {
            var kpiInfo = await _reportRepository.GetKPIInfo(request);
            var response = MapEcoScoreKPIInfoDataReponse(kpiInfo);
            return response;
        }

        public async Task<EcoScoreChartInfoDataServiceResponse> GetChartInfo(EcoScoreDataServiceRequest request)
        {
            var chartInfo = await _reportRepository.GetChartInfo(request);
            var response = MapEcoScoreChartInfoDataReponse(chartInfo);
            return response;
        }

        private EcoScoreKPIInfoDataServiceResponse MapEcoScoreKPIInfoDataReponse(dynamic records)
        {
            if (records is null)
                return null;

            var response = new EcoScoreKPIInfoDataServiceResponse { KPIInfo = new List<KPIInfo>() };

            foreach (var kpiInfo in records)
            {
                if (kpiInfo.starttimestamp != null)
                {
                    var kpiInfoResponse = new KPIInfo();
                    kpiInfoResponse.StartTimestamp = kpiInfo.starttimestamp;
                    kpiInfoResponse.EndTimestamp = kpiInfo.endtimestamp;
                    kpiInfoResponse.AnticipationScore = new KPI(kpiInfo.anticipationscore_total, kpiInfo.anticipationscore_count);
                    kpiInfoResponse.BrakingScore = new KPI(kpiInfo.brakingscore_total, kpiInfo.brakingscore_count);
                    kpiInfoResponse.FuelConsumption = new KPI(kpiInfo.fuelconsumption_total, kpiInfo.fuelconsumption_count);
                    kpiInfoResponse.Ecoscore = new KPI(kpiInfo.ecoscore_total, kpiInfo.ecoscore_count);
                    kpiInfoResponse.NumberOfTrips = kpiInfo.numberoftrips;
                    kpiInfoResponse.NumberOfVehicles = kpiInfo.numberofvehicles;
                    kpiInfoResponse.AverageGrossWeight = new KPI(kpiInfo.averagegrossweight_total, kpiInfo.averagegrossweight_count);
                    kpiInfoResponse.Distance = new KPI(kpiInfo.distance_total, kpiInfo.distance_count);
                    kpiInfoResponse.AverageDistancePerDay = new KPI(kpiInfo.averagedistanceperday_total, kpiInfo.averagedistanceperday_count);
                    kpiInfoResponse.CruiseControlUsage = new KPI(kpiInfo.cruisecontrolusage_total, kpiInfo.cruisecontrolusage_count);
                    kpiInfoResponse.CruiseControlUsage3050kmph = new KPI(kpiInfo.cruisecontrolusage30_total, kpiInfo.cruisecontrolusage30_count);
                    kpiInfoResponse.CruiseControlUsage5075kmph = new KPI(kpiInfo.cruisecontrolusage50_total, kpiInfo.cruisecontrolusage50_count);
                    kpiInfoResponse.CruiseControlUsage75kmph = new KPI(kpiInfo.cruisecontrolusage75_total, kpiInfo.cruisecontrolusage75_count);
                    kpiInfoResponse.PTOPercentage = new KPI(kpiInfo.ptousage_total, kpiInfo.ptousage_count);
                    kpiInfoResponse.PTODuration = new KPI(kpiInfo.ptoduration_total, kpiInfo.ptoduration_count);
                    kpiInfoResponse.AverageDrivingSpeed = new KPI(kpiInfo.averagedrivingspeed_total, kpiInfo.averagedrivingspeed_count);
                    kpiInfoResponse.AverageSpeed = new KPI(kpiInfo.averagespeed_total, kpiInfo.averagespeed_count);
                    kpiInfoResponse.HeavyThrottlingPercentage = new KPI(kpiInfo.heavythrottling_total, kpiInfo.heavythrottling_count);
                    kpiInfoResponse.HeavyThrottlingDuration = new KPI(kpiInfo.heavythrottleduration_total, kpiInfo.heavythrottleduration_count);
                    kpiInfoResponse.IdlingPercentage = new KPI(kpiInfo.idling_total, kpiInfo.idling_count);
                    kpiInfoResponse.IdleDuration = new KPI(kpiInfo.idleduration_total, kpiInfo.idleduration_count);
                    kpiInfoResponse.HarshBrakePercentage = new KPI(kpiInfo.harshbraking_total, kpiInfo.harshbraking_count);
                    kpiInfoResponse.HarshBrakeDuration = new KPI(kpiInfo.harshbrakeduration_total, kpiInfo.harshbrakeduration_count);
                    kpiInfoResponse.BrakingDuration = new KPI(kpiInfo.brakeduration_total, kpiInfo.brakeduration_count);
                    kpiInfoResponse.BrakingPercentage = new KPI(kpiInfo.braking_total, kpiInfo.braking_count);

                    response.KPIInfo.Add(kpiInfoResponse);
                }
            }

            return response;
        }

        private EcoScoreChartInfoDataServiceResponse MapEcoScoreChartInfoDataReponse(dynamic records)
        {
            if (records is null)
                return null;

            var response = new EcoScoreChartInfoDataServiceResponse { ChartInfo = new List<ChartInfo>() };

            foreach (var chartInfo in records)
            {
                if (chartInfo.starttimestamp != null)
                {
                    var chartInfoResponse = new ChartInfo();
                    chartInfoResponse.StartTimestamp = chartInfo.starttimestamp;
                    chartInfoResponse.EndTimestamp = chartInfo.endtimestamp;
                    chartInfoResponse.AnticipationScore = new KPI(chartInfo.anticipationscore_total, chartInfo.anticipationscore_count);
                    chartInfoResponse.BrakingScore = new KPI(chartInfo.brakingscore_total, chartInfo.brakingscore_count);
                    chartInfoResponse.FuelConsumption = new KPI(chartInfo.fuelconsumption_total, chartInfo.fuelconsumption_count);
                    chartInfoResponse.Ecoscore = new KPI(chartInfo.ecoscore_total, chartInfo.ecoscore_count);

                    response.ChartInfo.Add(chartInfoResponse);
                }
            }
            return response;
        }

        private int CalculateAggregationCount(AggregateType aggregateType, long startTimestamp, long endTimestamp)
        {
            var startDate = new DateTime(1970, 1, 1).AddMilliseconds(startTimestamp);
            var endDate = new DateTime(1970, 1, 1).AddMilliseconds(endTimestamp);
            var noOfDays = Math.Ceiling((endDate - startDate).TotalDays);
            int result;

            switch (aggregateType)
            {
                case AggregateType.DAY:
                    result = (int)noOfDays;
                    return result > 365 ? 365 : result;
                case AggregateType.WEEK:
                    result = (int)noOfDays / 7;
                    return result > 52 ? 52 : result;
                case AggregateType.MONTH:
                    result = (int)noOfDays / 30;
                    return result > 12 ? 12 : result;
            }
            return 0;

        }

        #endregion

        #region VehicleHealthStatus
        public async Task<List<VehicleHealthResult>> GetVehicleHealthStatus(VehicleHealthStatusRequest vehicleHealthStatusRequest)
        {
            vehicleHealthStatusRequest.WarningType = !string.IsNullOrEmpty(vehicleHealthStatusRequest.TripId) ? "A" : string.Empty;
            var data = await _reportRepository.GetVehicleHealthStatus(vehicleHealthStatusRequest);
            return data;
        }
        #endregion

        #region Fuel Deviation Report Table Details        
        public Task<IEnumerable<FuelDeviation>> GetFilteredFuelDeviation(FuelDeviationFilter fuelDeviationFilters)
        {
            return _reportRepository.GetFilteredFuelDeviation(fuelDeviationFilters);
        }

        public Task<IEnumerable<FuelDeviationCharts>> GetFuelDeviationCharts(FuelDeviationFilter fuelDeviationFilters)
        {
            return _reportRepository.GetFuelDeviationCharts(fuelDeviationFilters);
        }

        #endregion

        #region LogBook
        public async Task<IEnumerable<LogbookTripAlertDetails>> GetLogbookSearchParameter(List<string> vins)
        {
            return await _reportRepository.GetLogbookSearchParameter(vins);
        }

        public async Task<List<FilterProperty>> GetAlertLevelList(List<string> enums)
        {
            return await _reportRepository.GetAlertLevelList(enums);
        }
        public async Task<List<AlertCategory>> GetAlertCategoryList(List<string> enums)
        {
            return await _reportRepository.GetAlertCategoryList(enums);
        }
        public async Task<IEnumerable<EnumTranslation>> GetAlertCategory()
        {
            try
            {
                return await _reportRepository.GetAlertCategory();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<LogbookDetails>> GetLogbookDetails(LogbookDetailsFilter logbookFilter)
        {
            return await _reportRepository.GetLogbookDetails(logbookFilter);
        }

        public async Task<List<AlertThresholdDetails>> GetThresholdDetails(List<int> alertId, List<string> alertLevel)
        {
            return await _reportRepository.GetThresholdDetails(alertId, alertLevel);
        }
        #endregion

        #region Fuel Benchmark Report
        //public Task<IEnumerable<FuelBenchmark>> GetFuelBenchmarks(FuelBenchmark fuelBenchmarkFilter)
        //{
        //    return _reportRepository.GetFuelBenchmarks(fuelBenchmarkFilter);
        //}
        public async Task<FuelBenchmarkDetails> GetFuelBenchmarkDetails(FuelBenchmarkFilter fuelBenchmarkFilter)
        {
            var fuelConsumptionCalculation = await _reportRepository.GetFuelBenchmarkDetail(fuelBenchmarkFilter);

            FuelBenchmarkDetails fuelBenchmarkDetails = new FuelBenchmarkDetails();
            if (fuelConsumptionCalculation != null)
            {
                var vehicleRanking = await _reportRepository.GetFuelBenchmarkRanking(fuelBenchmarkFilter);
                fuelBenchmarkDetails.NumberOfActiveVehicles = vehicleRanking.Count();
                fuelBenchmarkDetails.NumberOfTotalVehicles = fuelConsumptionCalculation.Numbersofactivevehicle;
                fuelBenchmarkDetails.TotalMileage = fuelConsumptionCalculation.Totalmileage;
                fuelBenchmarkDetails.TotalFuelConsumed = fuelConsumptionCalculation.Totalfuelconsumed;
                fuelBenchmarkDetails.AverageFuelConsumption = fuelConsumptionCalculation.Averagefuelconsumption;
                fuelBenchmarkDetails.Ranking = new List<Ranking>();
                fuelBenchmarkDetails.Ranking = vehicleRanking;
            }
            return fuelBenchmarkDetails;
        }
        #endregion

        #region Vehicle Performance Report
        public async Task<VehiclePerformanceChartTemplate> GetVehPerformanceChartTemplate(VehiclePerformanceRequest vehiclePerformanceRequest)
        {
            var enginetemplate = await _reportRepository.GetVehPerformanceChartTemplate(vehiclePerformanceRequest);

            //We will bind data here
            return enginetemplate;
        }
        public async Task<VehiclePerformanceSummary> GetVehPerformanceSummaryDetails(string vin)
        {
            return await _reportRepository.GetVehPerformanceSummaryDetails(vin);

        }
        public async Task<VehiclePerformanceData> GetVehPerformanceBubbleChartData(VehiclePerformanceRequest vehiclePerformanceRequest)
        {
            PerformanceChartMatrix objmat = new PerformanceChartMatrix();
            VehiclePerformanceData charts = new VehiclePerformanceData();
            var chartRawdata = await _reportRepository.GetVehPerformanceBubbleChartData(vehiclePerformanceRequest);
            var rangedata = await _reportRepository.GetRangeData(vehiclePerformanceRequest.PerformanceType);
            if (vehiclePerformanceRequest.PerformanceType == "B")
            {
                charts = objmat.GetcombinedmatrixBrake(chartRawdata.Where(i => i.ColumnIndex != null).ToList(), vehiclePerformanceRequest.PerformanceType, rangedata.ToList(), chartRawdata.Sum(i => i.TripDuration));
            }
            else if (vehiclePerformanceRequest.PerformanceType == "S")
            {
                charts = objmat.GetcombinedmatrixRoadSpeed(chartRawdata.Where(i => i.ColumnIndex != null).ToList(), vehiclePerformanceRequest.PerformanceType, rangedata.ToList(), chartRawdata.Sum(i => i.TripDuration));
            }
            else
            {
                charts = objmat.Getcombinedmatrix(chartRawdata.Where(i => i.ColumnIndex != null).ToList(), vehiclePerformanceRequest.PerformanceType, rangedata.ToList(), chartRawdata.Sum(i => i.TripDuration));
            }

            //CalculateKPIData(List<IndexWiseChartData> vehicleChartDatas, double tripDuration, List<KpiDataRange> rangedata)
            //charts.ChartData = chartdata;
            //// chartdata = vehiclePerformanceRequest.PerformanceType == "B" ? chartdata.Select(x => { x.Value = -x.Value; return x; }).ToList() : chartdata;
            //charts.PieChartData = objmat.CalculateKPIData(chartdata, chartRawdata.Sum(i => i.TripDuration), await _reportRepository.GetRangeData(vehiclePerformanceRequest.PerformanceType));
            return charts;
        }
        public async Task<List<VehPerformanceProperty>> GetVehPerformanceType()
        {
            return await _reportRepository.GetVehPerformanceType();
        }
        #endregion
    }
}