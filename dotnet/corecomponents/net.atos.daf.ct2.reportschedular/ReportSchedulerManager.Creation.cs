using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportscheduler.repository;

namespace net.atos.daf.ct2.reportscheduler
{
    public partial class ReportSchedulerManager : IReportSchedulerManager
    {
        public async Task<int> GenerateReport()
        {
            //Get the records from reportscheduler for next run date as today
            //Loop
            //Generate Report as per report id / key
            //Insert the pdf bytes into scheduledreport , with 
            // Calculate nect run


            return 1;
        }
    }
}
