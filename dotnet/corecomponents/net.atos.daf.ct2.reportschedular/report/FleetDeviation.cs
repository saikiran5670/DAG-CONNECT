using System;
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
using net.atos.daf.ct2.reportscheduler.ENUM;
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
    public class FuelDeviation : IReport
    {
        private readonly IReportSchedulerRepository _reportSchedularRepository;
        private readonly IVisibilityManager _visibilityManager;
        private readonly ITemplateManager _templateManager;
        private readonly IUnitConversionManager _unitConversionManager;
        private readonly IUnitManager _unitManager;
        private readonly EmailEventType _evenType;
        private readonly EmailContentType _contentType;
        private readonly IMapManager _mapManager;
        private readonly MapHelper _mapHelper;

        public string VIN { get; private set; }
        public string TimeZoneName { get; private set; }
        public string DateFormatName { get; private set; }
        public string VehicleName { get; private set; }
        public string RegistrationNo { get; private set; }
        public long FromDate { get; private set; }
        public long ToDate { get; private set; }
        public IEnumerable<VehicleList> VehicleLists { get; private set; }
        public string VehicleGroups { get; private set; }
        public string VehicleNames { get; private set; }
        public IEnumerable<string> VINs { get; private set; }
        public string TimeFormatName { get; private set; }
        public UnitToConvert UnitToConvert { get; private set; }
        public bool IsAllParameterSet { get; private set; } = false;
        public ReportCreationScheduler ReportSchedulerData { get; private set; }
        public IReportManager ReportManager { get; }
        public string DateTimeFormat { get; private set; }
        public long TotalIdleDuration { get; private set; }
        public IEnumerable<reports.entity.FuelDeviation> FuelDeviationDetails { get; private set; }

        public FuelDeviation(IReportManager reportManager,
                          IReportSchedulerRepository reportSchedularRepository,
                          IVisibilityManager visibilityManager, ITemplateManager templateManager,
                          IUnitConversionManager unitConversionManager, IUnitManager unitManager,
                          EmailEventType evenType, EmailContentType contentType, IMapManager mapManager)
        {
            ReportManager = reportManager;
            _reportSchedularRepository = reportSchedularRepository;
            _visibilityManager = visibilityManager;
            _templateManager = templateManager;
            _unitConversionManager = unitConversionManager;
            _unitManager = unitManager;
            _evenType = evenType;
            _contentType = contentType;
            _mapManager = mapManager;
            _mapHelper = new MapHelper(_mapManager);
        }

        public void SetParameters(ReportCreationScheduler reportSchedulerData, IEnumerable<VehicleList> vehicleLists)
        {
            FromDate = reportSchedulerData.StartDate;
            ToDate = reportSchedulerData.EndDate;
            VehicleLists = vehicleLists;
            VINs = vehicleLists.Select(s => s.VIN).Distinct();
            //VIN = vehicleList.VIN;
            //VehicleName = vehicleList.VehicleName;
            //RegistrationNo = vehicleList.RegistrationNo;
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
            var html = string.Empty;
            return Task.FromResult<string>(html.ToString());
        }

        public async Task<string> GenerateTable()
        {
            var result = await ReportManager.GetFilteredFuelDeviation(new FuelDeviationFilter { StartDateTime = FromDate, EndDateTime = ToDate, VINs = VINs.ToList() });
            var fuelDeviationPdfDetails = new List<FuelDeviationPdfPdfDetails>();
            TotalIdleDuration = result.Sum(s => s.IdleDuration);
            foreach (var item in result)
            {
                fuelDeviationPdfDetails.Add(
                    new FuelDeviationPdfPdfDetails
                    {
                        Type = ReportHelper.GetFuelDeviationType((FuelType)item.FuelEventType, (VehicleActvityType)item.VehicleActivityType),
                        VIN = item.VIN,
                        FuelDiffernce = item.FuelDiffernce,
                        RegistrationNo = item.RegistrationNo,
                        VehicleName = item.VehicleName,
                        EventTime = TimeZoneHelper.GetDateTimeFromUTC(item.EventTime, TimeZoneName, DateTimeFormat),
                        Odometer = await _unitConversionManager.GetDistance(item.Odometer, DistanceUnit.Meter, UnitToConvert),
                        StartDate = TimeZoneHelper.GetDateTimeFromUTC(item.StartTimeStamp, TimeZoneName, DateTimeFormat),
                        EndDate = TimeZoneHelper.GetDateTimeFromUTC(item.EndTimeStamp, TimeZoneName, DateTimeFormat),
                        Distance = await _unitConversionManager.GetDistance(item.Distance, DistanceUnit.Meter, UnitToConvert),
                        IdleDuration = await _unitConversionManager.GetTimeSpan(item.IdleDuration, TimeUnit.Seconds, UnitToConvert),
                        AverageSpeed = (int)await _unitConversionManager.GetSpeed(item.AverageSpeed, SpeedUnit.MeterPerMilliSec, UnitToConvert),
                        AverageWeight = await _unitConversionManager.GetWeight(item.AverageWeight, WeightUnit.KiloGram, UnitToConvert),
                        StartPosition = string.IsNullOrEmpty(item.StartPosition) ? await _mapHelper.GetAddress(item.StartPositionLattitude, item.StartPositionLongitude) : item.StartPosition,
                        EndPosition = string.IsNullOrEmpty(item.EndPosition) ? await _mapHelper.GetAddress(item.EndPositionLattitude, item.EndPositionLongitude) : item.EndPosition,
                        FuelConsumed = await _unitConversionManager.GetVolume(item.FuelConsumed, VolumeUnit.MilliLiter, UnitToConvert),
                        DrivingTime = await _unitConversionManager.GetTimeSpan(item.DrivingTime, TimeUnit.Seconds, UnitToConvert),
                        Alerts = item.Alerts
                    });
            }
            FuelDeviationDetails = result;
            var html = ReportHelper
                        .ToDataTableAndGenerateHTML<FuelDeviationPdfPdfDetails>
                            (fuelDeviationPdfDetails);
            //, await _reportSchedularRepository.GetColumnName(ReportSchedulerData.ReportId, ReportSchedulerData.Code)
            return await Task.FromResult<string>(html);
        }

        public async Task<string> GenerateTemplate(byte[] logoBytes)
        {
            if (!IsAllParameterSet) throw new Exception(TripReportConstants.ALL_PARAM_MSG);
            var fromDate = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(FromDate, TimeConstants.UTC, $"{DateFormatName} {TimeFormatName}"));
            var toDate = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(ToDate, TimeConstants.UTC, $"{DateFormatName} {TimeFormatName}"));

            StringBuilder html = new StringBuilder();
            //ReportTemplateSingleto.
            //                        GetInstance()
            //                        .GetReportTemplate(_templateManager, ReportSchedulerData.ReportId, _evenType,
            //                                        _contentType, ReportSchedulerData.Code)
            var timeSpanUnit = await _unitManager.GetTimeSpanUnit(UnitToConvert);
            var distanceUnit = await _unitManager.GetDistanceUnit(UnitToConvert);
            var fuelIncrease = (char)FuelType.Increase;
            var fuelDecrease = (char)FuelType.Decrease;
            html.AppendFormat(ReportTemplateContants.REPORT_TEMPLATE_FUEL_DEVIATION
            //, Path.Combine(Directory.GetCurrentDirectory(), "assets", "style.css")
                              , logoBytes != null ? string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(logoBytes))
                                                : ImageSingleton.GetInstance().GetDefaultLogo()
                              , await GenerateTable()
                              , fromDate.ToString(DateTimeFormat)
                              , VehicleLists.Any(s => !string.IsNullOrEmpty(s.VehicleGroupName)) ? string.Join(',', VehicleLists.Select(s => s.VehicleGroupName).Distinct().ToArray()) : "All"
                              , toDate.ToString(DateTimeFormat)
                              , string.Join(',', VehicleLists.Select(s => s.VehicleName).Distinct().ToArray())
                              , FuelDeviationDetails.Where(w => w.FuelEventType == fuelIncrease).Count()
                              , FuelDeviationDetails.Where(w => w.FuelEventType == fuelDecrease).Count()
                              , FuelDeviationDetails.Select(w => w.VIN).Distinct().Count()
                              , distanceUnit
                              , distanceUnit
                              , timeSpanUnit
                              , await _unitManager.GetSpeedUnit(UnitToConvert)
                              , await _unitManager.GetWeightUnit(UnitToConvert)
                              , await _unitManager.GetVolumeUnit(UnitToConvert)
                              , timeSpanUnit
                              , ImageSingleton.GetInstance().GetLogo()
                );
            return html.ToString();
        }
    }
}
