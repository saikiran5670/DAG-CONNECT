using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class ReportType
    {
        public int Id { get; set; }
        public string ReportName { get; set; }
        public bool IsDriver { get; set; }
    }
    public class DriverDetail
    {
        public int Id { get; set; }
        public string DriverId { get; set; }
        public string DriverName { get; set; }
    }
    public class ReceiptEmails
    {
        public string Email { get; set; }
    }
    public class ReportParameter
    {
        public IEnumerable<ReportType> ReportType { get; set; }
        public IEnumerable<DriverDetail> DriverDetail { get; set; }
        public IEnumerable<ReceiptEmails> ReceiptEmails { get; set; }
    }

    public class ReportStatusUpdateDeleteModel
    {
        public int ReportId { get; set; }
        public int OrganizationId { get; set; }
        public string Status { get; set; }
    }
}
