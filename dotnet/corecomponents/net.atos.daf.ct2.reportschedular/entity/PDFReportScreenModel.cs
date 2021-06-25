using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class PDFReportScreenModel
    {
        public int Id { get; set; }
        public int ScheduleReportId { get; set; }
        public byte[] Report { get; set; }
        public Guid Token { get; set; }
        public long DownloadedAt { get; set; }
        public long ValidTill { get; set; }
        public long CreatedAt { get; set; }
        public long StartDate { get; set; }
        public long EndDate { get; set; }
        public int IsMailSend { get; set; }
        public string FileName { get; set; }
    }
    public class ReportPDFByidModel
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
    }
    public class ReportPDFBytokenModel
    {
        public string Token { get; set; }
        public int OrganizationId { get; set; }
    }
}
