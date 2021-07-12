using System.Collections.Generic;
using Google.Protobuf;
using net.atos.daf.ct2.reportscheduler.entity;

namespace net.atos.daf.ct2.reportschedulerservice.Entity
{
    public class Mapper
    {
        public ReportTypeRequest MapReportType(ReportType reportType)
        {
            ReportTypeRequest objreporttype = new ReportTypeRequest();
            objreporttype.Id = reportType.Id;
            objreporttype.ReportName = string.IsNullOrEmpty(reportType.ReportName) ? string.Empty : reportType.ReportName;
            objreporttype.Key = string.IsNullOrEmpty(reportType.Key) ? string.Empty : reportType.Key;
            objreporttype.IsDriver = reportType.IsDriver;
            objreporttype.IsVehicle = reportType.Key == ReportNameConstants.REPORT_TRIP ? true : false;
            return objreporttype;
        }

        public DriverDetailRequest MapDriverDetail(DriverDetail driverDetail)
        {
            DriverDetailRequest objdriverdetail = new DriverDetailRequest();
            objdriverdetail.Id = driverDetail.Id;
            objdriverdetail.DriverId = driverDetail.DriverId;
            objdriverdetail.DriverName = string.IsNullOrEmpty(driverDetail.DriverName) ? string.Empty : driverDetail.DriverName;
            return objdriverdetail;
        }

        public ReceiptEmailsRequest MapReceiptEmail(ReceiptEmails receiptEmail)
        {
            ReceiptEmailsRequest objreceiptemail = new ReceiptEmailsRequest();
            objreceiptemail.Email = receiptEmail.Email;
            return objreceiptemail;
        }

        public ReportSchedulerMap ToReportSchedulerEntity(ReportSchedulerRequest request)
        {
            ReportSchedulerMap reportscheduler = new ReportSchedulerMap();
            reportscheduler.Id = request.Id;
            reportscheduler.OrganizationId = request.OrganizationId;
            reportscheduler.ReportId = request.ReportId;
            reportscheduler.FrequencyType = string.IsNullOrEmpty(request.FrequencyType) ? string.Empty : request.FrequencyType;
            reportscheduler.Status = string.IsNullOrEmpty(request.Status) ? string.Empty : request.Status;
            reportscheduler.Type = string.IsNullOrEmpty(request.Type) ? string.Empty : request.Type;
            reportscheduler.StartDate = request.StartDate;
            reportscheduler.EndDate = request.EndDate;
            reportscheduler.Code = string.IsNullOrEmpty(request.Code) ? string.Empty : request.Code;
            reportscheduler.LastScheduleRunDate = request.LastScheduleRunDate;
            reportscheduler.NextScheduleRunDate = request.NextScheduleRunDate;
            reportscheduler.CreatedAt = request.CreatedAt;
            reportscheduler.CreatedBy = request.CreatedBy;
            reportscheduler.ModifiedAt = request.ModifiedAt;
            reportscheduler.ModifiedBy = request.ModifiedBy;
            reportscheduler.MailDescription = string.IsNullOrEmpty(request.MailDescription) ? string.Empty : request.MailDescription;
            reportscheduler.MailSubject = string.IsNullOrEmpty(request.MailSubject) ? string.Empty : request.MailSubject;
            reportscheduler.ReportDispatchTime = request.ReportDispatchTime;
            reportscheduler.ScheduledReportRecipient = new List<ScheduledReportRecipient>();
            if (request.ScheduledReportRecipient.Count > 0)
            {
                foreach (var item in request.ScheduledReportRecipient)
                {
                    reportscheduler.ScheduledReportRecipient.Add(ToScheduledReportRecipientEntity(item));
                }
            }
            if (request.ScheduledReportDriverRef.Count > 0)
            {
                foreach (var item in request.ScheduledReportDriverRef)
                {
                    reportscheduler.ScheduledReportDriverRef.Add(ToScheduledReportDriverRefEntity(item));
                }
            }
            reportscheduler.ScheduledReportVehicleRef = new List<ScheduledReportVehicleRef>();
            if (request.ScheduledReportVehicleRef.Count > 0)
            {
                foreach (var item in request.ScheduledReportVehicleRef)
                {
                    reportscheduler.ScheduledReportVehicleRef.Add(ToScheduledReportVehicleRefEntity(item));

                }
            }
            return reportscheduler;
        }
        public ReportSchedulerRequest MapReportSchedulerEntity(ReportSchedulerMap request)
        {
            ReportSchedulerRequest reportscheduler = new ReportSchedulerRequest();
            reportscheduler.Id = request.Id;
            reportscheduler.ReportName = string.IsNullOrEmpty(request.ReportName) ? string.Empty : request.ReportName;
            reportscheduler.OrganizationId = request.OrganizationId;
            reportscheduler.ReportId = request.ReportId;
            reportscheduler.FrequencyType = string.IsNullOrEmpty(request.FrequencyType) ? string.Empty : request.FrequencyType;
            reportscheduler.Status = string.IsNullOrEmpty(request.Status) ? string.Empty : request.Status;
            reportscheduler.Type = string.IsNullOrEmpty(request.Type) ? string.Empty : request.Type;
            reportscheduler.StartDate = request.StartDate;
            reportscheduler.EndDate = request.EndDate;
            reportscheduler.Code = string.IsNullOrEmpty(request.Code) ? string.Empty : request.Code;
            reportscheduler.LastScheduleRunDate = request.LastScheduleRunDate;
            reportscheduler.NextScheduleRunDate = request.NextScheduleRunDate;
            reportscheduler.CreatedAt = request.CreatedAt;
            reportscheduler.CreatedBy = request.CreatedBy;
            reportscheduler.ModifiedAt = request.ModifiedAt;
            reportscheduler.ModifiedBy = request.ModifiedBy;
            reportscheduler.MailDescription = string.IsNullOrEmpty(request.MailDescription) ? string.Empty : request.MailDescription;
            reportscheduler.MailSubject = string.IsNullOrEmpty(request.MailSubject) ? string.Empty : request.MailSubject;
            reportscheduler.ReportDispatchTime = request.ReportDispatchTime;
            if (request.ScheduledReportRecipient.Count > 0)
            {
                foreach (var item in request.ScheduledReportRecipient)
                {
                    reportscheduler.ScheduledReportRecipient.Add(MapScheduledReportRecipientEntity(item));
                }
            }
            if (request.ScheduledReportDriverRef.Count > 0)
            {
                foreach (var item in request.ScheduledReportDriverRef)
                {
                    reportscheduler.ScheduledReportDriverRef.Add(MapScheduledReportDriverRefEntity(item));
                }
            }
            if (request.ScheduledReportVehicleRef.Count > 0)
            {
                foreach (var item in request.ScheduledReportVehicleRef)
                {
                    reportscheduler.ScheduledReportVehicleRef.Add(MapScheduledReportVehicleRefEntity(item));

                }
            }
            if (request.ScheduledReport.Count > 0)
            {
                foreach (var item in request.ScheduledReport)
                {
                    reportscheduler.ScheduledReport.Add(MapScheduledReportEntity(item));
                }
            }
            return reportscheduler;
        }
        public ScheduledReportRecipient ToScheduledReportRecipientEntity(ScheduledReportRecipientRequest request)
        {
            ScheduledReportRecipient schedulereportsr = new ScheduledReportRecipient();
            schedulereportsr.Id = request.Id;
            schedulereportsr.ScheduleReportId = request.ScheduleReportId;
            schedulereportsr.Email = string.IsNullOrEmpty(request.Email) ? string.Empty : request.Email;
            schedulereportsr.State = string.IsNullOrEmpty(request.State) ? string.Empty : request.State;
            schedulereportsr.CreatedAt = request.CreatedAt;
            schedulereportsr.ModifiedAt = request.ModifiedAt;
            return schedulereportsr;
        }
        public ScheduledReportRecipientRequest MapScheduledReportRecipientEntity(ScheduledReportRecipient request)
        {
            ScheduledReportRecipientRequest schedulereportsr = new ScheduledReportRecipientRequest();
            schedulereportsr.Id = request.Id;
            schedulereportsr.ScheduleReportId = request.ScheduleReportId;
            schedulereportsr.Email = string.IsNullOrEmpty(request.Email) ? string.Empty : request.Email;
            schedulereportsr.State = string.IsNullOrEmpty(request.State) ? string.Empty : request.State;
            schedulereportsr.CreatedAt = request.CreatedAt;
            schedulereportsr.ModifiedAt = request.ModifiedAt;
            return schedulereportsr;
        }

        public ScheduledReportDriverRef ToScheduledReportDriverRefEntity(ScheduledReportDriverRefRequest request)
        {
            ScheduledReportDriverRef schedulereportdr = new ScheduledReportDriverRef();
            schedulereportdr.ScheduleReportId = request.ScheduleReportId;
            schedulereportdr.DriverId = request.DriverId;
            schedulereportdr.State = string.IsNullOrEmpty(request.State) ? string.Empty : request.State;
            schedulereportdr.CreatedAt = request.CreatedAt;
            schedulereportdr.CreatedBy = request.CreatedBy;
            schedulereportdr.ModifiedAt = request.ModifiedAt;
            schedulereportdr.ModifiedBy = request.ModifiedBy;
            return schedulereportdr;
        }
        public ScheduledReportDriverRefRequest MapScheduledReportDriverRefEntity(ScheduledReportDriverRef request)
        {
            ScheduledReportDriverRefRequest schedulereportdr = new ScheduledReportDriverRefRequest();
            schedulereportdr.ScheduleReportId = request.ScheduleReportId;
            schedulereportdr.DriverId = request.DriverId;

            schedulereportdr.State = string.IsNullOrEmpty(request.State) ? string.Empty : request.State;
            schedulereportdr.CreatedAt = request.CreatedAt;
            schedulereportdr.CreatedBy = request.CreatedBy;
            schedulereportdr.ModifiedAt = request.ModifiedAt;
            schedulereportdr.ModifiedBy = request.ModifiedBy;
            return schedulereportdr;
        }

        public ScheduledReportVehicleRef ToScheduledReportVehicleRefEntity(ScheduledReportVehicleRefRequest request)
        {
            ScheduledReportVehicleRef schedulereportvr = new ScheduledReportVehicleRef();
            schedulereportvr.ScheduleReportId = request.ScheduleReportId;
            schedulereportvr.VehicleGroupId = request.VehicleGroupId;
            schedulereportvr.State = string.IsNullOrEmpty(request.State) ? string.Empty : request.State;
            schedulereportvr.CreatedAt = request.CreatedAt;
            schedulereportvr.CreatedBy = request.CreatedBy;
            schedulereportvr.ModifiedAt = request.ModifiedAt;
            schedulereportvr.ModifiedBy = request.ModifiedBy;
            return schedulereportvr;
        }

        public ScheduledReportVehicleRefRequest MapScheduledReportVehicleRefEntity(ScheduledReportVehicleRef request)
        {
            ScheduledReportVehicleRefRequest schedulereportvr = new ScheduledReportVehicleRefRequest();
            schedulereportvr.ScheduleReportId = request.ScheduleReportId;
            schedulereportvr.VehicleGroupId = request.VehicleGroupId;
            schedulereportvr.State = string.IsNullOrEmpty(request.State) ? string.Empty : request.State;
            schedulereportvr.CreatedAt = request.CreatedAt;
            schedulereportvr.CreatedBy = request.CreatedBy;
            schedulereportvr.ModifiedAt = request.ModifiedAt;
            schedulereportvr.ModifiedBy = request.ModifiedBy;
            schedulereportvr.VehicleId = request.VehicleId;
            schedulereportvr.Vin = string.IsNullOrEmpty(request.Vin) ? string.Empty : request.Vin;
            schedulereportvr.Regno = string.IsNullOrEmpty(request.Regno) ? string.Empty : request.Regno;
            schedulereportvr.VehicleName = string.IsNullOrEmpty(request.VehicleName) ? string.Empty : request.VehicleName;
            schedulereportvr.VehicleGroupName = string.IsNullOrEmpty(request.VehicleGroupName) ? string.Empty : request.VehicleGroupName;
            schedulereportvr.VehicleGroupType = string.IsNullOrEmpty(request.VehicleGroupType) ? string.Empty : request.VehicleGroupType;
            schedulereportvr.FunctionEnum = string.IsNullOrEmpty(request.FunctionEnum) ? string.Empty : request.FunctionEnum;
            return schedulereportvr;
        }
        public ScheduledReportRequest MapScheduledReportEntity(ScheduledReport request)
        {
            ScheduledReportRequest scheduleReport = new ScheduledReportRequest();
            scheduleReport.Id = request.Id;
            scheduleReport.ScheduleReportId = request.ScheduleReportId;
            scheduleReport.DownloadedAt = request.DownloadedAt;
            scheduleReport.ValidTill = request.ValidTill;
            scheduleReport.CreatedAt = request.CreatedAt;
            scheduleReport.StartDate = request.StartDate;
            scheduleReport.EndDate = request.EndDate;
            return scheduleReport;
        }
        internal ReportPDFResponse MapPDFRepoModel(PDFReportScreenModel request)
        {
            ReportPDFResponse objReportPDFResponse = new ReportPDFResponse();
            objReportPDFResponse.Id = request.Id;
            objReportPDFResponse.ScheduleReportId = request.ScheduleReportId;
            objReportPDFResponse.Report = ByteString.CopyFrom(request.Report) ?? null;
            objReportPDFResponse.FileName = request.FileName;
            return objReportPDFResponse;
        }
    }
}