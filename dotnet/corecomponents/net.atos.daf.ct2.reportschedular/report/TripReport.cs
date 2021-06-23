using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.reports;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportscheduler.helper;
using net.atos.daf.ct2.reportscheduler.report;
using net.atos.daf.ct2.reportscheduler.repository;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.account.report
{
    public class TripReport : IReport
    {
        private readonly IReportSchedulerRepository _reportSchedularRepository;
        public string VIN { get; private set; }
        public string TimeZoneName { get; private set; }
        public string VehicleName { get; private set; }
        public string RegistrationNo { get; private set; }
        public long FromDate { get; private set; }
        public long ToDate { get; private set; }
        public bool IsAllParameterSet { get; private set; } = false;
        public ReportCreationScheduler ReportSchedulerData { get; private set; }

        public TripReport(IReportManager reportManager,
                          IReportSchedulerRepository reportSchedularRepository)
        {
            ReportManager = reportManager;
            _reportSchedularRepository = reportSchedularRepository;
        }

        public async Task SetParameters(ReportCreationScheduler reportSchedulerData)
        {
            FromDate = reportSchedulerData.StartDate;
            ToDate = reportSchedulerData.EndDate;
            var vechicleList = await _reportSchedularRepository.GetVehicleListForSingle(reportSchedulerData.Id);
            VIN = vechicleList.VIN;
            VehicleName = vechicleList.VehicleName;
            RegistrationNo = vechicleList.RegistrationNo;
            ReportSchedulerData = reportSchedulerData;
            TimeZoneName = reportSchedulerData.TimeZoneId > 0 ? TimeZoneSingleton.GetInstance(_reportSchedularRepository).GetTimeZoneName(reportSchedulerData.TimeZoneId) : "UTC";
            IsAllParameterSet = true;
        }
        public IReportManager ReportManager { get; }


        public Task<string> GenerateSummary()
        {
            if (!IsAllParameterSet) throw new Exception("Trip Report all Parameters are not set.");
            var fromDate = TimeZoneHelper.GetDateTimeFromUTC(FromDate, TimeZoneName);
            var toDate = TimeZoneHelper.GetDateTimeFromUTC(ToDate, TimeZoneName);
            StringBuilder html = new StringBuilder();
            html.AppendFormat(@"
            <table style='width: 100%; border-collapse: collapse;' border = '0'>                   
                   <tr>
                        <td style = 'width: 25%;' > From:{0}</td>
                        <td style = 'width: 25%;'> To: {1}</td>
                        <td style = 'width: 25%;'> Vehicle: {2} </td>
                        <td style = 'width: 25%;' > Vehicle Name: {3}</td>
                        <td style = 'width: 25%;' > Registration #: {4}</td>                        
                  </tr>   
             </table>",
                       fromDate.ToString("dd/MM/yyyy HH:mm:ss"),
                       toDate.ToString("dd/MM/yyyy HH:mm:ss"),
                       VIN, VehicleName, RegistrationNo
                            );
            return Task.FromResult<string>(html.ToString());
        }

        public async Task<string> GenerateTable()
        {
            var result = await ReportManager.GetFilteredTripDetails(new TripFilterRequest { StartDateTime = FromDate, EndDateTime = ToDate, VIN = VIN });
            string res = JsonConvert.SerializeObject(result);
            var tripReportDetails = JsonConvert.DeserializeObject<List<TripReportDetails>>(res);
            var tripReportPdfDetails = new List<TripReportPdfDetails>();
            foreach (var tripData in tripReportDetails)
            {
                tripReportPdfDetails.Add(
                    new TripReportPdfDetails
                    {
                        StartDate = TimeZoneHelper.GetDateTimeFromUTC(FromDate, TimeZoneName).ToString("yyyy-MM-ddTHH:mm:ss"),
                        EndDate = TimeZoneHelper.GetDateTimeFromUTC(FromDate, TimeZoneName).ToString("yyyy-MM-ddTHH:mm:ss"),
                        VIN = tripData.VIN,
                        Distance = tripData.Distance,
                        IdleDuration = tripData.IdleDuration,
                        AverageSpeed = tripData.AverageSpeed,
                        AverageWeight = tripData.AverageWeight,
                        Odometer = tripData.Odometer,
                        StartPosition = tripData.StartPosition,
                        EndPosition = tripData.EndPosition,
                        FuelConsumed = tripData.FuelConsumed,
                        DrivingTime = tripData.DrivingTime,
                        Alert = tripData.Alert,
                        Events = tripData.Events,
                        FuelConsumed100km = tripData.FuelConsumed100km
                    });
            }
            var html = ReportHelper.ToDataTableAndGenerateHTML<TripReportDetails>(tripReportDetails);
            return await Task.FromResult<string>(html);
        }

        public async Task<byte[]> GetLogoImage()
        {
            var reportLogo = await _reportSchedularRepository.GetReportLogo(ReportSchedulerData.CreatedBy);
            return reportLogo.Image;
        }
    }
}
