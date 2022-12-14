using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.dashboard.entity;
using net.atos.daf.ct2.dashboard.repository;
using net.atos.daf.ct2.dashboard.common;

namespace net.atos.daf.ct2.dashboard
{
    public class DashBoardManager : IDashBoardManager
    {
        private readonly IDashBoardRepository _dashboardRepository;

        public DashBoardManager(IDashBoardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }
        /// <summary>
        /// Get Fleet KPI Details for dashboard
        /// </summary>
        /// <param name="fleetKpiFilter">Filter reqires start and end date with visible VINs.</param>
        /// <returns>All VINs cumulative details from selected range.</returns>
        public async Task<FleetKpi> GetFleetKPIDetails(FleetKpiFilter fleetKpiFilter)
        {
            FleetKpi lstFleetKPIDetails = await GetCurrentFleetKPIDetails(fleetKpiFilter);
            if (lstFleetKPIDetails != null)
            {
                FleetKpi lstLastFleetKPIDetails = await GetLastFleetKPIDetails(fleetKpiFilter);
                if (lstLastFleetKPIDetails != null) { lstFleetKPIDetails.LastChangeKpi = lstLastFleetKPIDetails; }
            }
            return lstFleetKPIDetails;
        }
        /// <summary>
        /// To calculate and return selected period data for Fleet KPI
        /// </summary>
        /// <param name="fleetFuelFilters">Selected KPI filters object which is required for data retrival</param>
        /// <returns>Return selected period data according to selected date range and VINs</returns>
        private async Task<FleetKpi> GetCurrentFleetKPIDetails(FleetKpiFilter fleetKpiFilter)
        {
            FleetKpi lstFleetKPIDetails = await _dashboardRepository.GetFleetKPIDetails(fleetKpiFilter);
            return lstFleetKPIDetails;
        }
        /// <summary>
        /// To calculate and return last period data for Fleet KPI
        /// </summary>
        /// <param name="fleetFuelFilters">Selected KPI filters object which is used for current data retrival</param>
        /// <returns>Return last period data according to selected date range duration and VINs</returns>
        private async Task<FleetKpi> GetLastFleetKPIDetails(FleetKpiFilter fleetKpiFilter)
        {
            DateTime startDateTime = fleetKpiFilter.StartDateTime.UnixToDateTime();
            DateTime endDateTime = fleetKpiFilter.EndDateTime.UnixToDateTime();
            // To calculate last duration date range
            DateTime lastStartdate = startDateTime.AddDays(-7);
            DateTime lastEnddate = startDateTime;

            // To prepare copy of existing object for query
            FleetKpiFilter lastFleetKpiFilter = new FleetKpiFilter
            {
                VINs = fleetKpiFilter.VINs,
                // converting time to milisecoed
                StartDateTime = lastStartdate.ToUnixMiliSecTime(),
                EndDateTime = lastEnddate.ToUnixMiliSecTime()
            };

            return await _dashboardRepository.GetFleetKPIDetails(lastFleetKpiFilter);
        }

        public async Task<List<Alert24Hours>> GetLastAlert24Hours(Alert24HoursFilter alert24HoursFilter)
        {
            List<Alert24Hours> alert24hours = await _dashboardRepository.GetLastAlert24Hours(alert24HoursFilter);
            return alert24hours;
        }
        public async Task<List<AlertOrgMap>> GetAlertNameOrgList(int organizationId, List<int> featureIds)
        {
            List<AlertOrgMap> alerts = await _dashboardRepository.GetAlertNameOrgList(organizationId, featureIds);
            return alerts;
        }
        public async Task<TodayLiveVehicleResponse> GetTodayLiveVinData(TodayLiveVehicleRequest objTodayLiveVehicleRequest)
        {
            TodayLiveVehicleResponse objTodayResponse = new TodayLiveVehicleResponse();
            var todayData = await _dashboardRepository.GetTodayLiveVinData(objTodayLiveVehicleRequest);
            if (todayData != null && todayData.Count > 0)
            {
                objTodayResponse.TodayActiveVinCount = objTodayResponse.DriverCount = todayData.Count;//Current trip table driver1_id data is not in sink with driver table Data processingteam need to do that
                for (int i = 0; i < todayData.Count; i++)
                {
                    objTodayResponse.TodayDistanceBasedUtilization += todayData[i].TodayDistance;
                    objTodayResponse.Distance += todayData[i].TodayDistance;
                    objTodayResponse.TodayTimeBasedUtilizationRate += todayData[i].TodayDrivingTime;
                    objTodayResponse.DrivingTime += todayData[i].TodayDrivingTime;
                }
            }
            var yesterdayData = await _dashboardRepository.GetYesterdayLiveVinData(objTodayLiveVehicleRequest);
            if (yesterdayData != null && yesterdayData.Count > 0)
            {
                objTodayResponse.YesterdayActiveVinCount = yesterdayData.Count;
                for (int i = 0; i < yesterdayData.Count; i++)
                {
                    objTodayResponse.YesterdayDistanceBasedUtilization += yesterdayData[i].YesterdayDistance;
                    objTodayResponse.YesterdayTimeBasedUtilizationRate += yesterdayData[i].YesterdayDrivingTime;
                }
            }
            return objTodayResponse;
        }

        #region Utilization
        public async Task<List<Chart_Fleetutilization>> GetUtilizationchartsData(FleetKpiFilter tripFilters)
        {
            return await _dashboardRepository.GetUtilizationchartsData(tripFilters);
        }
        #endregion

    }


}
