using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.email.Enum;
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
    public class FleetUtilisation : IReport
    {
        private readonly IReportSchedulerRepository _reportSchedularRepository;
        private readonly IVisibilityManager _visibilityManager;
        private readonly ITemplateManager _templateManager;
        private readonly IUnitConversionManager _unitConversionManager;
        private readonly IUnitManager _unitManager;
        private readonly EmailEventType _evenType;
        private readonly EmailContentType _contentType;

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
        internal List<FleetUtilizationPdfDetails> FleetUtilisationPdfDetails { get; private set; }
        public long TotalIdleDuration { get; private set; }

        public FleetUtilisation(IReportManager reportManager,
                          IReportSchedulerRepository reportSchedularRepository,
                          IVisibilityManager visibilityManager, ITemplateManager templateManager,
                          IUnitConversionManager unitConversionManager, IUnitManager unitManager, EmailEventType evenType, EmailContentType contentType)
        {
            ReportManager = reportManager;
            _reportSchedularRepository = reportSchedularRepository;
            _visibilityManager = visibilityManager;
            _templateManager = templateManager;
            _unitConversionManager = unitConversionManager;
            _unitManager = unitManager;
            _evenType = evenType;
            _contentType = contentType;
        }

        public void SetParameters(ReportCreationScheduler reportSchedulerData, IEnumerable<VehicleList> vehicleLists)
        {
            FromDate = reportSchedulerData.StartDate;
            ToDate = reportSchedulerData.EndDate;
            VehicleLists = vehicleLists;
            VINs = vehicleLists.Select(s => s.VIN);
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
            var result = await ReportManager.GetFleetUtilizationDetails(new FleetUtilizationFilter { StartDateTime = FromDate, EndDateTime = ToDate, VIN = VINs.ToList() });
            var fleetUtilisationPdfDetails = new List<FleetUtilizationPdfDetails>();
            TotalIdleDuration = result.Sum(s => s.IdleDuration);
            foreach (var item in result)
            {
                fleetUtilisationPdfDetails.Add(
                    new FleetUtilizationPdfDetails
                    {
                        VehicleName = item.VehicleName,
                        VIN = item.VIN,
                        RegistrationNumber = item.RegistrationNumber,
                        Distance = (int)await _unitConversionManager.GetDistance(item.Distance, DistanceUnit.Meter, UnitToConvert),
                        NumberOfTrips = item.NumberOfTrips,
                        TripTime = await _unitConversionManager.GetTimeSpan(item.TripTime, TimeUnit.MiliSeconds, UnitToConvert),
                        DrivingTime = await _unitConversionManager.GetTimeSpan(item.DrivingTime, TimeUnit.Seconds, UnitToConvert),
                        IdleDuration = await _unitConversionManager.GetTimeSpan(item.IdleDuration, TimeUnit.Seconds, UnitToConvert),
                        StopTime = await _unitConversionManager.GetTimeSpan(item.StopTime, TimeUnit.MiliSeconds, UnitToConvert),
                        AverageDistancePerDay = (int)await _unitConversionManager.GetDistance(item.AverageDistancePerDay, DistanceUnit.Meter, UnitToConvert),
                        AverageSpeed = (int)await _unitConversionManager.GetSpeed(item.AverageSpeed, SpeedUnit.MeterPerMilliSec, UnitToConvert),
                        AverageWeightPerTrip = (int)await _unitConversionManager.GetWeight(item.AverageWeightPerTrip, WeightUnit.KiloGram, UnitToConvert),
                        Odometer = (int)await _unitConversionManager.GetDistance(item.Odometer, DistanceUnit.Meter, UnitToConvert)

                    });
            }
            FleetUtilisationPdfDetails = fleetUtilisationPdfDetails;
            var html = ReportHelper
                        .ToDataTableAndGenerateHTML<FleetUtilizationPdfDetails>
                            (fleetUtilisationPdfDetails);
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

            html.AppendFormat(ReportTemplateContants.REPORT_TEMPLATE_FLEET_UTILISATION
            //, Path.Combine(Directory.GetCurrentDirectory(), "assets", "style.css")
                              , logoBytes != null ? string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(logoBytes))
                                                : ImageSingleton.GetInstance().GetDefaultLogo()
                              , await GenerateTable()
                              , fromDate.ToString(DateTimeFormat)
                              , VehicleLists.Any(s => !string.IsNullOrEmpty(s.VehicleGroupName)) ? string.Join(',', VehicleLists.Select(s => s.VehicleGroupName).ToArray()) : "All"
                              , toDate.ToString(DateTimeFormat)
                              , string.Join(',', VehicleLists.Select(s => s.VehicleName).ToArray())
                              , FleetUtilisationPdfDetails.Count()
                              , FleetUtilisationPdfDetails.Sum(s => s.Distance)
                              , distanceUnit
                              , FleetUtilisationPdfDetails.Sum(s => s.NumberOfTrips)
                              , FleetUtilisationPdfDetails.Sum(s => s.AverageDistancePerDay)
                              , distanceUnit
                              , await _unitConversionManager.GetTimeSpan(TotalIdleDuration, TimeUnit.Seconds, UnitToConvert)
                              , timeSpanUnit
                              , distanceUnit
                              , timeSpanUnit
                              , timeSpanUnit
                              , timeSpanUnit
                              , timeSpanUnit
                              , distanceUnit
                              , await _unitManager.GetSpeedUnit(UnitToConvert)
                              , await _unitManager.GetWeightUnit(UnitToConvert)
                              , distanceUnit
                              , ImageSingleton.GetInstance().GetLogo()
                ); ;
            //return html.Replace("{{", "{").Replace("}}", "}").ToString();
            return html.ToString();
        }
    }
}
