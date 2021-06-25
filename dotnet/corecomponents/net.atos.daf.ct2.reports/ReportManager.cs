﻿using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reports.repository;
using System.Linq;

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
                    if (driver.EcoScoreRanking < objEcoScoreKPI.MinValue)
                        driver.EcoScoreRankingColor = RankingColor.RED.ToString();
                    //> Target = Green
                    else if (driver.EcoScoreRanking > objEcoScoreKPI.TargetValue)
                        driver.EcoScoreRankingColor = RankingColor.GREEN.ToString();
                    //Between Min and Target = Amber
                    else
                        driver.EcoScoreRankingColor = RankingColor.AMBER.ToString();

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

        #endregion

        #endregion

        #region Fleet Utilizaiton Report
        public async Task<List<FleetUtilizationDetails>> GetFleetUtilizationDetails(FleetUtilizationFilter FleetFilter)
        {
            List<FleetUtilizationDetails> lstFleetUtilizationDetails = await _reportRepository.GetFleetUtilizationDetails(FleetFilter);
            return lstFleetUtilizationDetails;
        }

        public async Task<List<Calender_Fleetutilization>> GetCalenderData(FleetUtilizationFilter tripFilters)
        {
            List<Calender_Fleetutilization> lstFleetUtilizationDetails = await _reportRepository.GetCalenderData(tripFilters);
            return lstFleetUtilizationDetails;
        }

        #endregion
    }
}
