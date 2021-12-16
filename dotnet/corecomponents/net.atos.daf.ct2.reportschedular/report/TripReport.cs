﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.map;
using net.atos.daf.ct2.reports;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportscheduler.helper;
using net.atos.daf.ct2.reportscheduler.report;
using net.atos.daf.ct2.reportscheduler.repository;
using net.atos.daf.ct2.template;
using net.atos.daf.ct2.unitconversion;
using net.atos.daf.ct2.unitconversion.ENUM;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.visibility;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.account.report
{
    public class TripReport : IReport
    {
        private readonly IReportSchedulerRepository _reportSchedularRepository;
        private readonly IVisibilityManager _visibilityManager;
        private readonly ITemplateManager _templateManager;
        private readonly IUnitConversionManager _unitConversionManager;
        private readonly IUnitManager _unitManager;
        private readonly EmailEventType _evenType;
        private readonly EmailContentType _contentType;
        private readonly MapHelper _mapHelper;

        public string VIN { get; private set; }
        public string TimeZoneName { get; private set; }
        public string DateFormatName { get; private set; }
        public string VehicleName { get; private set; }
        public string RegistrationNo { get; private set; }
        public long FromDate { get; private set; }
        public long ToDate { get; private set; }
        public string TimeFormatName { get; private set; }
        public UnitToConvert UnitToConvert { get; private set; }
        public bool IsAllParameterSet { get; private set; } = false;
        public ReportCreationScheduler ReportSchedulerData { get; private set; }
        public IReportManager ReportManager { get; }
        public string DateTimeFormat { get; private set; }
        public int FeatureId { get; }

        public TripReport(IReportManager reportManager,
                          IReportSchedulerRepository reportSchedularRepository,
                          IVisibilityManager visibilityManager, ITemplateManager templateManager,
                          IUnitConversionManager unitConversionManager, IUnitManager unitManager,
                          EmailEventType evenType, EmailContentType contentType, IMapManager mapManager, int featureId)
        {
            ReportManager = reportManager;
            _reportSchedularRepository = reportSchedularRepository;
            _visibilityManager = visibilityManager;
            _templateManager = templateManager;
            _unitConversionManager = unitConversionManager;
            _unitManager = unitManager;
            _evenType = evenType;
            _contentType = contentType;
            _mapHelper = new reportscheduler.helper.MapHelper(mapManager);
            FeatureId = featureId;
        }

        public void SetParameters(ReportCreationScheduler reportSchedulerData, IEnumerable<VehicleList> vehicleLists)
        {
            FromDate = reportSchedulerData.StartDate;
            ToDate = reportSchedulerData.EndDate;
            var vehicleList = vehicleLists.FirstOrDefault();
            VIN = vehicleList?.VIN;
            VehicleName = vehicleList?.VehicleName ?? string.Empty;
            RegistrationNo = vehicleList?.RegistrationNo ?? string.Empty;
            ReportSchedulerData = reportSchedulerData;
            TimeZoneName = reportSchedulerData.TimeZoneId > 0 ? TimeZoneSingleton.GetInstance(_reportSchedularRepository).GetTimeZoneName(reportSchedulerData.TimeZoneId) : TimeConstants.UTC;
            DateFormatName = reportSchedulerData.DateFormatId > 0 ? DateFormatSingleton.GetInstance(_reportSchedularRepository).GetDateFormatName(reportSchedulerData.DateFormatId) : FormatConstants.DATE_FORMAT;
            TimeFormatName = reportSchedulerData.TimeFormatId > 0 ? TimeFormatSingleton.GetInstance(_reportSchedularRepository).GetTimeFormatName(reportSchedulerData.TimeFormatId) : FormatConstants.TIME_FORMAT_24;
            UnitToConvert = reportSchedulerData.UnitId > 0 ? UnitNameSingleton.GetInstance(_reportSchedularRepository).GetUnitName(reportSchedulerData.UnitId) : UnitToConvert.Metric;
            DateTimeFormat = $"{DateFormatName} {TimeFormatName}";
            IsAllParameterSet = true;
        }

        public Task<string> GenerateSummary()
        {
            //if (!IsAllParameterSet) throw new Exception(TripReportConstants.ALL_PARAM_MSG);
            //var fromDate = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(FromDate, TripReportConstants.UTC, $"{DateFormatName} {TimeFormatName}"));
            //var toDate = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(ToDate, TripReportConstants.UTC, $"{DateFormatName} {TimeFormatName}"));
            //StringBuilder html = new StringBuilder();
            //html.AppendFormat(ReportTemplate.REPORT_SUMMARY_TEMPLATE,
            //           fromDate.ToString(DateTimeFormat),
            //           toDate.ToString(DateTimeFormat),
            //           VIN, VehicleName, RegistrationNo
            //  
            var html = string.Empty;
            return Task.FromResult<string>(html.ToString());
        }

        public async Task<string> GenerateTable()
        {
            var tripFilter = new TripFilterRequest { StartDateTime = FromDate, EndDateTime = ToDate, VIN = VIN, OrganizationId = ReportSchedulerData.OrganizationId };
            tripFilter.FeatureIds = (List<int>)await _reportSchedularRepository.GetfeaturesByAccountAndOrgId(ReportSchedulerData.CreatedBy, ReportSchedulerData.OrganizationId);

            List<visibility.entity.VehicleDetailsAccountVisibilityForAlert> vehicleDetailsAccountVisibilty = new List<visibility.entity.VehicleDetailsAccountVisibilityForAlert>();

            IEnumerable<visibility.entity.VehicleDetailsAccountVisibilityForAlert> vehicleAccountVisibiltyList
               = await _visibilityManager.GetVehicleByAccountVisibilityForAlert(ReportSchedulerData.CreatedBy, ReportSchedulerData.OrganizationId, ReportSchedulerData.OrganizationId, tripFilter.FeatureIds.ToArray());
            //append visibile vins
            vehicleDetailsAccountVisibilty.AddRange(vehicleAccountVisibiltyList);
            //remove duplicate vins by key as vin
            vehicleDetailsAccountVisibilty = vehicleDetailsAccountVisibilty.GroupBy(c => c.Vin, (key, c) => c.FirstOrDefault()).ToList();

            tripFilter.AlertVIN = vehicleDetailsAccountVisibilty.Where(x => x.Vin == VIN).Select(x => x.Vin).FirstOrDefault();
            if (string.IsNullOrEmpty(tripFilter.AlertVIN))
            {
                throw new Exception(TripReportConstants.NO_ALERT_VISIBILITY_MSG);
            }
            var result = await ReportManager.GetFilteredTripDetails(tripFilter, false);
            //string res = JsonConvert.SerializeObject(result);
            //var tripReportDetails = JsonConvert.DeserializeObject<List<TripReportDetails>>(res);
            var tripReportPdfDetails = new List<TripReportPdfDetails>();
            foreach (var tripData in result)
            {
                tripReportPdfDetails.Add(
                    new TripReportPdfDetails
                    {
                        StartDate = TimeZoneHelper.GetDateTimeFromUTC(tripData.StartTimeStamp, TimeZoneName, DateTimeFormat),
                        EndDate = TimeZoneHelper.GetDateTimeFromUTC(tripData.EndTimeStamp, TimeZoneName, DateTimeFormat),
                        //VIN = tripData.VIN,
                        Distance = await _unitConversionManager.GetDistance(tripData.Distance, DistanceUnit.Meter, UnitToConvert),
                        IdleDuration = await _unitConversionManager.GetTimeSpan(tripData.IdleDuration, TimeUnit.Seconds, UnitToConvert),
                        AverageSpeed = await _unitConversionManager.GetSpeed(tripData.AverageSpeed, SpeedUnit.MeterPerMilliSec, UnitToConvert),
                        AverageWeight = await _unitConversionManager.GetWeight(tripData.AverageWeight, WeightUnit.KiloGram, UnitToConvert),
                        Odometer = await _unitConversionManager.GetDistance(tripData.Odometer, DistanceUnit.Meter, UnitToConvert),
                        StartPosition = string.IsNullOrEmpty(tripData.StartPosition) ? await _mapHelper.GetAddress(tripData.StartPositionLattitude, tripData.StartPositionLongitude) : tripData.StartPosition,
                        EndPosition = string.IsNullOrEmpty(tripData.EndPosition) ? await _mapHelper.GetAddress(tripData.EndPositionLattitude, tripData.EndPositionLongitude) : tripData.EndPosition,
                        //FuelConsumed = tripData.FuelConsumed,
                        DrivingTime = await _unitConversionManager.GetTimeSpan(tripData.DrivingTime, TimeUnit.Seconds, UnitToConvert),
                        Alerts = tripData.Alert,
                        Events = tripData.Events,
                        FuelConsumed100km = await _unitConversionManager.GetVolumePer100Km(tripData.Distance, tripData.FuelConsumed, VolumeUnit.MilliLiter, UnitToConvert)
                    });
            }
            var html = ReportHelper
                        .ToDataTableAndGenerateHTML<TripReportPdfDetails>
                            (tripReportPdfDetails);
            //, await _reportSchedularRepository.GetColumnName(ReportSchedulerData.ReportId, ReportSchedulerData.Code)
            return await Task.FromResult<string>(html);
        }

        public async Task<string> GenerateTemplate(byte[] logoBytes)
        {
            if (!IsAllParameterSet) throw new Exception(TripReportConstants.ALL_PARAM_MSG);
            var fromDate = TimeZoneHelper.GetDateTimeFromUTC(FromDate, TimeZoneName, DateTimeFormat);
            var toDate = TimeZoneHelper.GetDateTimeFromUTC(ToDate, TimeZoneName, DateTimeFormat);

            StringBuilder html = new StringBuilder();

            html.AppendFormat(ReportTemplateSingleto.
                                    GetInstance()
                                    .GetReportTemplate(_templateManager, ReportSchedulerData.ReportId, _evenType,
                                                        _contentType, ReportSchedulerData.Code)
                              , logoBytes != null ? string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(logoBytes))
                                                : ImageSingleton.GetInstance().GetDefaultLogo()
                              , ImageSingleton.GetInstance().GetLogo()
                              , fromDate
                              , "All", VIN
                              , toDate
                              , VehicleName, RegistrationNo
                              , await GenerateTable()
                              , await _unitManager.GetDistanceUnit(UnitToConvert)
                              , await _unitManager.GetTimeSpanUnit(UnitToConvert)
                              , await _unitManager.GetSpeedUnit(UnitToConvert)
                              , await _unitManager.GetWeightUnit(UnitToConvert)
                              , await _unitManager.GetDistanceUnit(UnitToConvert)
                              , await _unitManager.GetVolumePerDistanceUnit(UnitToConvert)
                              , await _unitManager.GetTimeSpanUnit(UnitToConvert)
                );
            return html.ToString();
        }
    }
}
