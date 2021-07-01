using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.account.report;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.reports;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportscheduler.repository;
using net.atos.daf.ct2.template;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.visibility;

namespace net.atos.daf.ct2.reportscheduler.report
{
    public class ReportCreator : IReportCreator
    {
        private readonly ILogger<ReportCreator> _logger;
        private readonly IConverter _generatePdf;
        private readonly IReportManager _reportManager;
        private readonly IReportSchedulerRepository _reportSchedularRepository;
        private readonly IVisibilityManager _visibilityManager;
        private readonly ITemplateManager _templateManager;

        public string ReportName { get; private set; }
        public string ReportKey { get; private set; }
        public IReport Report { get; private set; }
        public bool IsAllParameterSet { get; private set; } = false;
        public ReportCreationScheduler ReportSchedulerData { get; private set; }

        public ReportCreator(ILogger<ReportCreator> logger,
                            IConverter generatePdf, IReportManager reportManager,
                             IReportSchedulerRepository reportSchedularRepository,
                             IVisibilityManager visibilityManager, ITemplateManager templateManager)
        {
            _generatePdf = generatePdf;
            _reportManager = reportManager;
            _reportSchedularRepository = reportSchedularRepository;
            _visibilityManager = visibilityManager;
            _templateManager = templateManager;
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
            ReportNameConstants.REPORT_TRIP => new TripReport(_reportManager, _reportSchedularRepository, _visibilityManager,
                                                              _templateManager, EmailEventType.TripReport, EmailContentType.Html),
            ReportNameConstants.REPORT_TRIP_TRACING => null,
            _ => throw new ArgumentException(message: "invalid Report Key value", paramName: nameof(reportKey)),
        };

        public async Task<bool> GenerateReport()
        {
            if (!IsAllParameterSet) throw new Exception("Report Creation all Parameters are not set.");
            await Report.SetParameters(ReportSchedulerData);
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10, Right = 10, Left = 10, Bottom = 10 },
                //DocumentTitle = "PDF Report"//,
                //Out = $@"C:\Users\harneet.r (58879009)\Documents\POC\Employee_Report{ReportSchedulerData.Id}.pdf"
            };
            string htmlText = await Report.GenerateTemplate(await GetLogoImage());

            _logger.LogInformation($"Rpt Id: {ReportSchedulerData.Id}: {htmlText}");

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlText,
                //Page = "https://code-maze.com/", //USE THIS PROPERTY TO GENERATE PDF CONTENT FROM AN HTML PAGE
                //WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "style.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer", Spacing = 0 }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            var pdf123 = _generatePdf.Convert(pdf);
            return await _reportSchedularRepository
                            .InsertReportPDF(new ScheduledReport
                            {
                                Report = pdf123,
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

        private async Task<byte[]> GetLogoImage()
        {
            var reportLogo = await _reportSchedularRepository.GetReportLogo(ReportSchedulerData.CreatedBy);
            return reportLogo?.Image;
        }
    }
}
