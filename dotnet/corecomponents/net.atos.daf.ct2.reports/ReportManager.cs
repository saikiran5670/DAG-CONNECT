using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reports.repository;
using System.Linq;
using net.atos.daf.ct2.reports.entity.fleetFuel;

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
        public async Task<List<DriversActivities>> GetDriversActivity(DriverActivityFilter DriverActivityFilter)
        {
            List<DriversActivities> driverActivities = await _reportRepository.GetDriversActivity(DriverActivityFilter);
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
        public async Task<List<DriversActivities>> GetDriverActivity(DriverActivityFilter DriverActivityFilter) => await _reportRepository.GetDriversActivity(DriverActivityFilter);

        public async Task<List<Driver>> GetDriversByVIN(long startDateTime, long endDateTime, List<string> vin)
        {
            return await _reportRepository.GetDriversByVIN(startDateTime, endDateTime, vin);
        }
        public async Task<object> GetReportSearchParameterByVIN(int reportID, long startDateTime, long endDateTime, List<string> vin)
        {
            return await _reportRepository.GetReportSearchParameterByVIN(reportID, startDateTime, endDateTime, vin);
        }
        #endregion

        #region Eco Score Report

        #region Eco Score Report - Create Profile

        public async Task<bool> CreateEcoScoreProfile(EcoScoreProfileDto dto)
        {
            return await _reportRepository.CreateEcoScoreProfile(dto);
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
            var isExist = await _reportRepository.CheckEcoScoreProfileIsExist(ecoScoreProfileDto.OrganizationId, ecoScoreProfileDto.Name);
            if (isExist)// check if profile is avilable in DB or not
            {
                string versionType = await _reportRepository.IsEcoScoreProfileBasicOrAdvance(ecoScoreProfileDto.Id);
                bool isGlobalProfile = await _reportRepository.GetGlobalProfile(ecoScoreProfileDto.Id);
                if (!string.IsNullOrEmpty(versionType))// check if it is basic or advance versiontype= "B" or "A"
                {
                    if (isAdminRights)// admin rights with level 10 & 20
                    {
                        ecoScoreProfileDto.Name = null;
                        return await _reportRepository.UpdateEcoScoreProfile(ecoScoreProfileDto); // DAF Admin – Not Allowed Update Profile Name 
                    }
                }
                else if (versionType == null)
                {
                    if (isGlobalProfile)
                    {
                        if (isAdminRights)
                        {
                            return await _reportRepository.UpdateEcoScoreProfile(ecoScoreProfileDto);
                        }
                    }
                    else
                    {
                        return await _reportRepository.UpdateEcoScoreProfile(ecoScoreProfileDto);
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

            if (!string.IsNullOrEmpty(versionType))
            {
                bool isGlobalProfile = await _reportRepository.GetGlobalProfile(profileId);

                if (isGlobalProfile)
                {
                    if (isAdminRights)
                    {
                        ecoScoreProfileId = await _reportRepository.DeleteEcoScoreProfile(profileId);
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    ecoScoreProfileId = await _reportRepository.DeleteEcoScoreProfile(profileId);
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

        public async Task<bool> CheckIfReportUserPreferencesExist(int reportId, int accountId, int organizationId)
        {
            return await _reportRepository.CheckIfReportUserPreferencesExist(reportId, accountId, organizationId);
        }

        public async Task<IEnumerable<ReportUserPreference>> GetReportUserPreferences(int reportId, int accountId, int organizationId)
        {
            return await _reportRepository.GetReportUserPreferences(reportId, accountId, organizationId);
        }

        public async Task<IEnumerable<ReportUserPreference>> GetPrivilegeBasedReportUserPreferences(int reportId, int accountId, int roleId,
                                                                                       int organizationId, int contextOrgId)
        {
            return await _reportRepository.GetPrivilegeBasedReportUserPreferences(reportId, accountId, roleId, organizationId, contextOrgId);
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
        public async Task<List<FleetOverviewDetails>> GetFleetOverviewDetails(FleetOverviewFilter fleetOverviewFilter)
        {
            List<FleetOverviewDetails> fleetOverviewDetails = await _reportRepository.GetFleetOverviewDetails(fleetOverviewFilter);
            return fleetOverviewDetails;
        }
        public async Task<List<DriverFilter>> GetDriverList(List<string> vins)
        {
            List<DriverFilter> lstDriver = await _reportRepository.GetDriverList(vins);
            return lstDriver;
        }
        public async Task<List<WarningDetails>> GetWarningDetails(List<int> warningClass, List<int> warningNumber, string lngCode)
        {
            List<WarningDetails> lstWarningDetails = await _reportRepository.GetWarningDetails(warningClass, warningNumber, lngCode);
            return lstWarningDetails;
        }
        public async Task<List<DriverDetails>> GetDriverDetails(List<int> driverIds)
        {
            List<DriverDetails> lstDriverDetails = await _reportRepository.GetDriverDetails(driverIds);
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
            // List<FleetFuelDetailsByDriver> lstFleetFuelDetailsUpdated = await PrepareDetails(lstFleetFuelDetails, fleetFuelFilters.LanguageCode);
            return lstFleetFuelDetails;
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

        public async Task<List<FleetFuelDetails>> GetFleetFuelTripDetailsByVehicle(FleetFuelFilter fleetFuelFilters)
        {
            List<FleetFuelDetails> lstFleetFuelTripDetails = await _reportRepository.GetFleetFuelTripDetailsByVehicle(fleetFuelFilters);
            return lstFleetFuelTripDetails;
        }

        public async Task<List<FleetFuelDetails>> GetFleetFuelTripDetailsByDriver(FleetFuelFilterDriver fleetFuelFilters)
        {
            List<FleetFuelDetails> lstFleetFuelTripDetails = await _reportRepository.GetFleetFuelTripDetailsByDriver(fleetFuelFilters);
            return lstFleetFuelTripDetails;
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
                    string averageTrafficClassificationValue = averageTrafficClassification.Where(idl => idl.MaxValue <= item.AverageTrafficClassification && idl.MinValue >= item.AverageTrafficClassification).Select(item => item.Value).FirstOrDefault();
                    item.AverageTrafficClassificationValue = averageTrafficClassificationValue;
                }
            });
            return fleetFuelDetails;

        }
        #endregion

        #region Eco-Score Data service

        public Task<bool> GetKPIInfo(EcoScoreDataServiceRequest request) => _reportRepository.GetKPIInfo(request);

        public Task<bool> GetChartInfo(EcoScoreDataServiceRequest request) => _reportRepository.GetChartInfo(request);

        #endregion

        #region VehicleHealthStatus
        public async Task<List<VehicleHealthResult>> GetVehicleHealthStatus(VehicleHealthStatusRequest vehicleHealthStatusRequest)
        {
            vehicleHealthStatusRequest.WarningType = !string.IsNullOrEmpty(vehicleHealthStatusRequest.TripId) ? "A" : string.Empty;
            var data = await _reportRepository.GetVehicleHealthStatus(vehicleHealthStatusRequest);
            return data;
        }
        #endregion
    }
}