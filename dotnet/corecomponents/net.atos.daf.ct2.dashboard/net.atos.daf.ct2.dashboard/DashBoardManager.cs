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

            // Calculate last change duration from current filters
            TimeSpan dayDifference = endDateTime - startDateTime;
            int days = dayDifference.Days;

            // To calculate last duration date range
            DateTime lastStartdate = startDateTime.AddDays(days * -1);
            DateTime lastEnddate = startDateTime.AddDays(-1);

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
        public async Task<TodayLiveVehicleResponse> GetTodayLiveVinData(TodayLiveVehicleRequest objTodayLiveVehicleRequest)
        {
            return await _dashboardRepository.GetTodayLiveVinData(objTodayLiveVehicleRequest);
        }
    }


}
