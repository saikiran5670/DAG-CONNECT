using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using net.atos.daf.ct2.account.report;
using net.atos.daf.ct2.reports;

namespace net.atos.daf.ct2.reportscheduler.report
{
    public class ReportCreator : IReportCreator
    {
        private readonly IConverter _generatePdf;
        private readonly IReportManager _reportManager;

        public string ReportName { get; private set; }
        public string ReportKey { get; private set; }
        public IReport Report { get; private set; }
        

        public ReportCreator(IConverter generatePdf, IReportManager reportManager)
        {
            _generatePdf = generatePdf;
            _reportManager = reportManager;
        }

        public void SetParameters(string reportName, string reportKey)
        {
            ReportName = reportName;
            ReportKey = reportKey;
            Report = InitializeReport(reportKey);
        }

        private IReport InitializeReport(string reportKey) =>
        reportKey switch
        {
            "lblTripReport" => new TripReport(_reportManager),
            "lblTripTracing" => null,
            _ => throw new ArgumentException(message: "invalid Report Key value", paramName: nameof(reportKey)),
        };

        public async Task<byte[]> GenerateReport()
        {
            var globalSettings = new GlobalSettings
            {                
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10, Bottom =10},
                //DocumentTitle = "PDF Report"//,
                //Out = @"C:\Harneet\POC\Employee_Report5.pdf"
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = await GenerateTemplate(),
                //Page = "https://code-maze.com/", //USE THIS PROPERTY TO GENERATE PDF CONTENT FROM AN HTML PAGE
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "Chart.min.js") },
                //HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                //FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            return _generatePdf.Convert(pdf);
        }

        private async Task<string> GenerateTemplate()
        {
            Byte[] bytes = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "assets", "DAFLogo.png"));
            String logoPath = Convert.ToBase64String(bytes);
            string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Chart.min.js");
            StringBuilder html = new StringBuilder();
            html.Append("<html><head>");
            //html.AppendFormat("<script src='{0}' ></script>", scriptPath);
            html.Append("<script > 	Function.prototype.bind = Function.prototype.bind || function (thisp) {var fn = this;return function () {return fn.apply(thisp, arguments);};};</script>");
            html.Append("<style>body{font-family: -apple-system,BlinkMacSystemFont,'Segoe UI'}.header img {  float: left;  width: 150px;  height: 100px;  background: #555;}.header h1 {  position: relative;  top: 18px;  left: 10px;}table {font-family: -apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,'Helvetica Neue',Arial,'Noto Sans','Liberation Sans',sans-serif,'Apple Color Emoji','Segoe UI Emoji','Segoe UI Symbol','Noto Color Emoji',Helvetica Neue LT Std-Md;border-collapse: collapse;width: 100%;}td, th { border-bottom-width: 1px solid Red;text-align: left;padding: 8px; }tr:nth-child(odd) {background-color: #ecf7fe; padding: 8px; margin: 15px;}tr:nth-child(even) {background-color: #ffff; padding: 8px; margin: 15px;}tr:{min-height: 45px!important;align-items: center; padding: 8px; margin: 15px;}}</style></head><body>");
            html.Append("</head ><body>");
            html.Append("</br>");
            html.AppendFormat("<span><img src='data:image/gif;base64,{0}' /><div><span style='text-align: center;'><h1>{1}</h1></span>", logoPath, ReportName);
            html.Append("</br>");
            html.Append(await Report.GenerateSummary());
            html.Append("</br>");
            html.Append(await Report.GenerateTable());
            //html.Append("<canvas id='myChart' style='width:100%;max-width:600px'></canvas>");
            //html.Append("<script>var xValues = ['Italy', 'France', 'Spain', 'USA', 'Argentina']; var yValues = [55, 49, 44, 24, 15]; var barColors = ['red', 'green','blue','orange','brown']; new Chart('myChart', {type: 'bar',data: {labels: xValues, datasets: [{backgroundColor: barColors, data: yValues}]  },  options: { legend: {display: false}, title: { display: true, text: 'World Wine Production 2018'} } }); </script>");
            html.Append("</body></html>");
            return html.ToString();
        }
    }
}
