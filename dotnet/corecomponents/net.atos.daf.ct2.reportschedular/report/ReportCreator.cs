using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.account.report;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.map;
using net.atos.daf.ct2.reports;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportscheduler.helper;
using net.atos.daf.ct2.reportscheduler.repository;
using net.atos.daf.ct2.template;
using net.atos.daf.ct2.unitconversion;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.visibility;

namespace net.atos.daf.ct2.reportscheduler.report
{
    public class ReportCreator : IReportCreator
    {
        private readonly ILogger<ReportCreator> _logger;
        private readonly IConverter _generatePdf;
        private readonly IReportManager _reportManager;
        private readonly IReportSchedulerRepository _reportSchedulerRepository;
        private readonly IVisibilityManager _visibilityManager;
        private readonly ITemplateManager _templateManager;
        private readonly IUnitConversionManager _unitConversionManager;
        private readonly IUnitManager _unitManager;
        private readonly IMapManager _mapManager;

        public string ReportName { get; private set; }
        public string ReportKey { get; private set; }
        public IReport Report { get; private set; }
        public bool IsAllParameterSet { get; private set; } = false;
        public ReportCreationScheduler ReportSchedulerData { get; private set; }

        public ReportCreator(ILogger<ReportCreator> logger,
                            IConverter generatePdf, IReportManager reportManager,
                             IReportSchedulerRepository reportSchedularRepository,
                             IVisibilityManager visibilityManager, ITemplateManager templateManager,
                             IUnitConversionManager unitConversionManager, IUnitManager unitManager, IConfiguration configuration, IMapManager mapManager)
        {
            _generatePdf = generatePdf;
            _reportManager = reportManager;
            _reportSchedulerRepository = reportSchedularRepository;
            _visibilityManager = visibilityManager;
            _templateManager = templateManager;
            _unitConversionManager = unitConversionManager;
            _unitManager = unitManager;
            _mapManager = mapManager;
            ReportSingleton.GetInstance().SetDAFSupportEmailId(configuration["ReportCreationScheduler:DAFSupportEmailId"] ?? string.Empty);
            _logger = logger;
        }

        public void SetParameters(ReportCreationScheduler reportSchedulerData)
        {
            ReportSchedulerData = reportSchedulerData;
            ReportName = reportSchedulerData.ReportName;
            ReportKey = reportSchedulerData.ReportKey = reportSchedulerData.ReportKey?.Trim();
            Report = InitializeReport(ReportKey);
            IsAllParameterSet = true;
        }

        private IReport InitializeReport(string reportKey) =>
        reportKey switch
        {
            ReportNameConstants.REPORT_TRIP => new TripReport(_reportManager, _reportSchedulerRepository, _visibilityManager,
                                                              _templateManager, _unitConversionManager, _unitManager, EmailEventType.TripReport, EmailContentType.Html, _mapManager),
            ReportNameConstants.REPORT_FLEET_UTILISATION => new FleetUtilisation(_reportManager, _reportSchedulerRepository, _visibilityManager,
                                                              _templateManager, _unitConversionManager, _unitManager, EmailEventType.FleetUtilisation, EmailContentType.Html),
            ReportNameConstants.REPORT_FLEET_FUEL => new FleetFuel(_reportManager, _reportSchedulerRepository, _visibilityManager,
                                                              _templateManager, _unitConversionManager, _unitManager, EmailEventType.FleetFuel, EmailContentType.Html, _mapManager),
            ReportNameConstants.REPORT_FUEL_DEVIATION => new FuelDeviation(_reportManager, _reportSchedulerRepository, _visibilityManager,
                                                              _templateManager, _unitConversionManager, _unitManager, EmailEventType.FleetFuel, EmailContentType.Html, _mapManager),
            _ => throw new ArgumentException(message: "invalid Report Key value", paramName: nameof(reportKey)),
        };

        public async Task<bool> GenerateReport()
        {
            if (!IsAllParameterSet) throw new Exception("Report Creation all Parameters are not set.");

            Report.SetParameters(ReportSchedulerData, await GetVehicleDetails());
            var pdf = await GetHtmlToPdfDocument();
            //var test = _generatePdf.Convert(pdf);
            //return true;
            return await _reportSchedulerRepository
                           .InsertReportPDF(new ScheduledReport
                           {
                               Report = _generatePdf.Convert(pdf),
                               ScheduleReportId = ReportSchedulerData.Id,
                               StartDate = ReportSchedulerData.StartDate,
                               EndDate = ReportSchedulerData.EndDate,
                               Token = Guid.NewGuid(),
                               FileName = $"{ReportSchedulerData.ReportName}_{ReportSchedulerData.Id}_{DateTime.Now.ToString("ddMMyyyyHHmmss")}",
                               CreatedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now),
                               ValidTill = UTCHandling.GetUTCFromDateTime(DateTime.Now.AddMonths(3)),
                               IsMailSend = false
                           }) > 0;
        }

        private async Task<HtmlToPdfDocument> GetHtmlToPdfDocument()
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = GetOrientation(),
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 }
                //Out = $@"C:\POC\All\{ ReportSchedulerData.ReportName }_{ ReportSchedulerData.Id }_{ DateTime.Now.ToString("ddMMyyyyHHmmss") }.pdf"
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = await Report.GenerateTemplate(await GetLogoImage()),
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Left = $"Support: {ReportSingleton.GetInstance().GetDAFSupportEmailId()}", Spacing = 0 }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            return pdf;
        }

        private Orientation GetOrientation()
        {
            if (ReportKey == ReportNameConstants.REPORT_TRIP)
            {
                return Orientation.Portrait;
            }
            return Orientation.Landscape;
        }
        private PaperKind GetPaperKind()
        {
            if (ReportKey == ReportNameConstants.REPORT_FLEET_FUEL || ReportKey == ReportNameConstants.REPORT_FLEET_UTILISATION)
            {
                return PaperKind.A3;
            }
            return PaperKind.A4;
        }

        private async Task<IEnumerable<VehicleList>> GetVehicleDetails()
        {
            List<VehicleList> vehicleList = new List<VehicleList>();
            if (ReportKey == ReportNameConstants.REPORT_TRIP)
            {
                var vehicle = await _reportSchedulerRepository.GetVehicleListForSingle(ReportSchedulerData.Id);
                if (vehicle != null)
                    vehicleList.Add(vehicle);
            }
            else
            {
                vehicleList.AddRange(await _reportSchedulerRepository.GetVehicleList(ReportSchedulerData.Id));
            }

            if (vehicleList == null || vehicleList.Count == 0)
            {
                throw new Exception(TripReportConstants.NO_VEHICLE_MSG);
            }
            var vinData = string.Join(',', vehicleList.Select(s => s.VIN).ToArray());
            var vehicleAssociationList = await _visibilityManager.GetVehicleByAccountVisibility(ReportSchedulerData.CreatedBy, ReportSchedulerData.OrganizationId);
            if (vehicleAssociationList == null || vehicleAssociationList.Count() == 0)
            {
                throw new Exception(TripReportConstants.NO_ASSOCIATION_MSG);
            }
            var removeVehicleList = new List<int>();
            foreach (var item in vehicleList)
            {
                if (!vehicleAssociationList.Any(w => w.VehicleId == item.Id))
                {
                    removeVehicleList.Add(item.Id);
                }
            }
            vehicleList.RemoveAll(p => removeVehicleList.Contains(p.Id));
            if (vehicleList.Count == 0)
            {
                throw new Exception(string.Format(TripReportConstants.NO_VEHICLE_ASSOCIATION_MSG, vinData));
            }
            return vehicleList;
            //var lst = new List<VehicleList>();
            //lst.Add(vehicleList.Where(w => w.VIN == "XLR0998HGFFT76657").FirstOrDefault());
            //return lst;
        }

        private async Task<byte[]> GetLogoImage()
        {
            var reportLogo = await _reportSchedulerRepository.GetReportLogo(ReportSchedulerData.CreatedBy);
            return reportLogo?.Image;
        }
    }
}
