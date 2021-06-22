using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.reportscheduler.report
{
    public interface IReportCreator
    {
        void SetParameters(string reportName, string reportKey);
        Task<byte[]> GenerateReport();
    }
}
