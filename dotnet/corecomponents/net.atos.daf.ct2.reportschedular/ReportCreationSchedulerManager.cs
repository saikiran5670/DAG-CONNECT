using System;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.entity;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.reportscheduler.report;
using net.atos.daf.ct2.reportscheduler.repository;

namespace net.atos.daf.ct2.reportscheduler
{
    public partial class ReportCreationSchedulerManager : IReportCreationSchedulerManager
    {

        private readonly IReportSchedulerRepository _reportSchedulerRepository;
        private readonly IAuditTraillib _auditLog;
        //private readonly IGeneratePdf _generatePdf;
        private readonly IReportCreator _reportCreator;

        public ReportCreationSchedulerManager(IReportSchedulerRepository reportSchedularRepository,
                                      IAuditTraillib auditLog,
                                      //IGeneratePdf generatePdf,
                                      IReportCreator reportCreator)
        {
            _reportSchedulerRepository = reportSchedularRepository;
            _auditLog = auditLog;
            //_generatePdf = generatePdf;
            _reportCreator = reportCreator;
        }
        public async Task<int> GenerateReport()
        {
            try
            {
                //Get the records from reportscheduler for next run date as today
                foreach (var reportSchedulerData in await _reportSchedulerRepository.GetReportCreationSchedulerList())
                //int cnt = 1;
                //while (cnt == 1)
                {
                    try
                    {
                        //Generate Report as per report id / key
                        _reportCreator.SetParameters(reportSchedulerData);
                        //_reportCreator.SetParameters("Trip Report", "lblTripReport");
                        var pdf = await _reportCreator.GenerateReport();
                        //Insert the pdf bytes into scheduledreport , with 
                        // Calculate nect run
                        
                        //cnt += 1;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception)
            {
                await _auditLog.AddLogs(new AuditTrail
                {
                    Created_at = DateTime.Now,
                    Performed_at = DateTime.Now,
                    Performed_by = 2,
                    Component_name = "Report Creation Scheduler",
                    Service_name = "reportscheduler.CoreComponent",
                    Event_type = AuditTrailEnum.Event_type.Mail,
                    Event_status = AuditTrailEnum.Event_status.FAILED,
                    Message = $"Report Created successfully",
                    Sourceobject_id = 0,
                    Targetobject_id = 0,
                    Updated_data = "ReportCreationScheduler"
                });
            }

            return 1;
        }
    }
}
