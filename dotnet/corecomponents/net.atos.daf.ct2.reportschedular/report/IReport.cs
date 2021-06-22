using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.reportscheduler.report
{
    public interface IReport
    {
        Task<byte[]> GetLogoImage();

        Task<string> GenerateSummary();

        Task<string> GenerateTable();
    }
}
