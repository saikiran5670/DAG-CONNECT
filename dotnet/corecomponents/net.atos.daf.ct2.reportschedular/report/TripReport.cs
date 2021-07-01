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
    public class TripReport : IReport
    {
        private readonly IReportSchedulerRepository _reportSchedularRepository;
        private readonly IVisibilityManager _visibilityManager;
        private readonly ITemplateManager _templateManager;
        private readonly IUnitConversionManager _unitConversionManager;
        private readonly EmailEventType _evenType;
        private readonly EmailContentType _contentType;

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


        public TripReport(IReportManager reportManager,
                          IReportSchedulerRepository reportSchedularRepository,
                          IVisibilityManager visibilityManager, ITemplateManager templateManager,
                          IUnitConversionManager unitConversionManager, EmailEventType evenType, EmailContentType contentType)
        {
            ReportManager = reportManager;
            _reportSchedularRepository = reportSchedularRepository;
            _visibilityManager = visibilityManager;
            _templateManager = templateManager;
            _unitConversionManager = unitConversionManager;
            _evenType = evenType;
            _contentType = contentType;
        }

        public async Task SetParameters(ReportCreationScheduler reportSchedulerData)
        {
            FromDate = reportSchedulerData.StartDate;
            ToDate = reportSchedulerData.EndDate;
            var vehicleAssociationList = await _visibilityManager.GetVehicleByAccountVisibility(reportSchedulerData.CreatedBy, reportSchedulerData.OrganizationId);
            if (vehicleAssociationList.Count() == 0)
            {
                throw new Exception(TripReportConstants.NO_ASSOCIATION_MSG);
            }

            var vehicleList = await _reportSchedularRepository.GetVehicleListForSingle(reportSchedulerData.Id);
            if (vehicleList == null)
            {
                throw new Exception(TripReportConstants.NO_VEHICLE_MSG);
            }

            if (vehicleList != null && vehicleAssociationList.Where(w => w.VehicleId == vehicleList.Id).Count() == 0)
            {
                throw new Exception(string.Format(TripReportConstants.NO_VEHICLE_ASSOCIATION_MSG, vehicleList.VIN));
            }

            VIN = vehicleList.VIN;
            VehicleName = vehicleList.VehicleName;
            RegistrationNo = vehicleList.RegistrationNo;
            ReportSchedulerData = reportSchedulerData;
            TimeZoneName = reportSchedulerData.TimeZoneId > 0 ? TimeZoneSingleton.GetInstance(_reportSchedularRepository).GetTimeZoneName(reportSchedulerData.TimeZoneId) : TripReportConstants.UTC;
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
            var result = await ReportManager.GetFilteredTripDetails(new TripFilterRequest { StartDateTime = FromDate, EndDateTime = ToDate, VIN = VIN }, false);
            string res = JsonConvert.SerializeObject(result);
            var tripReportDetails = JsonConvert.DeserializeObject<List<TripReportDetails>>(res);
            var tripReportPdfDetails = new List<TripReportPdfDetails>();
            foreach (var tripData in tripReportDetails)
            {
                tripReportPdfDetails.Add(
                    new TripReportPdfDetails
                    {
                        StartDate = TimeZoneHelper.GetDateTimeFromUTC(tripData.StartTimeStamp, TimeZoneName, DateTimeFormat),
                        EndDate = TimeZoneHelper.GetDateTimeFromUTC(tripData.EndTimeStamp, TimeZoneName, DateTimeFormat),
                        //VIN = tripData.VIN,
                        Distance = (int)await _unitConversionManager.GetDistance(tripData.Distance, DistanceUnit.Meter, UnitToConvert),
                        IdleDuration = tripData.IdleDuration,
                        AverageSpeed = tripData.AverageSpeed,
                        AverageWeight = tripData.AverageWeight,
                        Odometer = tripData.Odometer,
                        StartPosition = tripData.StartPosition,
                        EndPosition = tripData.EndPosition,
                        //FuelConsumed = tripData.FuelConsumed,
                        DrivingTime = tripData.DrivingTime,
                        Alerts = tripData.Alert,
                        Events = tripData.Events,
                        FuelConsumed100km = Math.Round(tripData.FuelConsumed100km, 2)
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
            var fromDate = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(FromDate, TripReportConstants.UTC, $"{DateFormatName} {TimeFormatName}"));
            var toDate = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(ToDate, TripReportConstants.UTC, $"{DateFormatName} {TimeFormatName}"));

            StringBuilder html = new StringBuilder();
            //ReportTemplateSingleto.
            //                        GetInstance(_templateManager, ReportSchedulerData.ReportId, _evenType,
            //                                    _contentType, ReportSchedulerData.Code)
            //                        .GetReportTemplate(_templateManager, ReportSchedulerData.ReportId, _evenType,
            //                                        _contentType, ReportSchedulerData.Code)
            html.AppendFormat(ReportTemplateContants.REPORT_TEMPLATE
                              //, Path.Combine(Directory.GetCurrentDirectory(), "assets", "style.css")
                              , logoBytes != null ? string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(logoBytes))
                                                : ImageSingleton.GetInstance().GetDefaultLogo()
                              , ImageSingleton.GetInstance().GetLogo()
                              , fromDate.ToString(DateTimeFormat)
                              , "All", VIN
                              , toDate.ToString(DateTimeFormat)
                              , VehicleName, RegistrationNo
                              , await GenerateTable()
                );
            return html.Replace("{{", "{").Replace("}}", "}").ToString();
        }
    }
}
