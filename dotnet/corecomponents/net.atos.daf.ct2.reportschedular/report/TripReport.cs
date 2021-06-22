using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.reports;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportscheduler.helper;
using net.atos.daf.ct2.reportscheduler.report;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.account.report
{
    public class TripReport : IReport
    {
        public string VIN { get; private set; }
        public string VehicleName { get; private set; }
        public string RegistrationNo { get; private set; }
        public long FromDate { get; private set; }
        public long ToDate { get; private set; }

        public TripReport(IReportManager reportManager)
        {
            ReportManager = reportManager;
        }

        public IReportManager ReportManager { get; }

        public Task<string> GenerateSummary()
        {
            var fromDate = new DateTime(2021, 06, 21, 0, 0, 0);
            var toDate = new DateTime(2021, 06, 21, 23, 59, 59);
            StringBuilder html = new StringBuilder();
            html.AppendFormat(@"
            <table style='width: 100%; border-collapse: collapse;' border = '0'>                   
                   <tr>
                        <td style = 'width: 25%;'> Vehicle: {0} </td>
                        <td style = 'width: 25%;' > Vehicle Name: {1}</td>
                        <td style = 'width: 25%;' > Registration #: {2}</td>
                        <td style = 'width: 25%;' > From:{3}</td>
                        <td style = 'width: 25%;'> To: {4}</td>
                  </tr>   </table>", VIN, VehicleName, RegistrationNo,
                          fromDate.ToString("dd/MM/yyyy HH:mm:ss"),
                          toDate.ToString("dd/MM/yyyy HH:mm:ss"));

            return Task.FromResult<string>(html.ToString());
        }

        public async Task<string> GenerateTable()
        {
            var result = await ReportManager.GetFilteredTripDetails(new TripFilterRequest { StartDateTime = FromDate, EndDateTime = ToDate, VIN = VIN });
            string res = JsonConvert.SerializeObject(result);
            var tripReportDetails = JsonConvert.DeserializeObject<List<TripReportDetails>>(res);
            var html = ReportHelper.GenerateHTMLString(ReportHelper.ToDataTable<TripReportDetails>(tripReportDetails));
            return await Task.FromResult<string>(html);
        }

        public Task<byte[]> GetLogoImage()
        {
            throw new NotImplementedException();
        }
    }
}
