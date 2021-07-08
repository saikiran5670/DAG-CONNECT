using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class MapperRepo
    {
        public IEnumerable<ReportSchedulerMap> GetReportSchedulerList(IEnumerable<ReportSchedulerResult> reportSchedulerResult)
        {
            List<ReportSchedulerMap> reportSchedulerList = new List<ReportSchedulerMap>();

            //Lookups are implemeted to avoid inserting duplicate entry of same id into the list
            Dictionary<int, ReportSchedulerMap> reportSchedulerLookup = new Dictionary<int, ReportSchedulerMap>();
            Dictionary<int, ScheduledReport> scheduledReportLookup = new Dictionary<int, ScheduledReport>();
            Dictionary<int, ScheduledReportRecipient> scheduledReportRecipientLookup = new Dictionary<int, ScheduledReportRecipient>();
            Dictionary<Tuple<int, int, int>, ScheduledReportVehicleRef> scheduledReportVehicleRefLookup = new Dictionary<Tuple<int, int, int>, ScheduledReportVehicleRef>();
            Dictionary<Tuple<int, int>, ScheduledReportDriverRef> scheduledReportDriverRefLookup = new Dictionary<Tuple<int, int>, ScheduledReportDriverRef>();

            foreach (var reportSchedulerItem in reportSchedulerResult)
            {
                if (!reportSchedulerLookup.TryGetValue(Convert.ToInt32(reportSchedulerItem.Repsch_id), out ReportSchedulerMap reportScheduler))
                {
                    reportSchedulerLookup.Add(Convert.ToInt32(reportSchedulerItem.Repsch_id), reportScheduler = ToReportSchedulerModel(reportSchedulerItem));
                }
                if (reportScheduler.ScheduledReportRecipient == null)
                {
                    reportScheduler.ScheduledReportRecipient = new List<ScheduledReportRecipient>();
                }
                if (reportScheduler.ScheduledReportVehicleRef == null)
                {
                    reportScheduler.ScheduledReportVehicleRef = new List<ScheduledReportVehicleRef>();
                }
                if (reportScheduler.ScheduledReport == null)
                {
                    reportScheduler.ScheduledReport = new List<ScheduledReport>();
                }
                if (reportScheduler.ScheduledReportDriverRef == null)
                {
                    reportScheduler.ScheduledReportDriverRef = new List<ScheduledReportDriverRef>();
                }
                if (reportSchedulerItem.Receipt_id > 0 && reportSchedulerItem.Repsch_id == reportSchedulerItem.Receipt_schedule_report_id)
                {
                    if (!scheduledReportRecipientLookup.TryGetValue(Convert.ToInt32(reportSchedulerItem.Receipt_id), out _))
                    {
                        var scheduledReportRecipient = ToScheduledReportRecipientModel(reportSchedulerItem);
                        scheduledReportRecipientLookup.Add(Convert.ToInt32(reportSchedulerItem.Receipt_id), scheduledReportRecipient);
                        reportScheduler.ScheduledReportRecipient.Add(scheduledReportRecipient);
                    }
                }
                if ((reportSchedulerItem.Vehref_vehicle_group_id > 0 || reportSchedulerItem.Vehicleid > 0) && reportSchedulerItem.Repsch_id == reportSchedulerItem.Vehref_report_schedule_id)
                {
                    if (!scheduledReportVehicleRefLookup.TryGetValue(Tuple.Create(Convert.ToInt32(reportSchedulerItem.Vehref_vehicle_group_id), reportSchedulerItem.Vehicleid, reportSchedulerItem.Repsch_id), out _))
                    {
                        var scheduledReportVehicle = ToScheduledReportVehicleRefModel(reportSchedulerItem);
                        scheduledReportVehicleRefLookup.Add(Tuple.Create(Convert.ToInt32(reportSchedulerItem.Vehref_vehicle_group_id), reportSchedulerItem.Vehicleid, reportSchedulerItem.Repsch_id), scheduledReportVehicle);
                        reportScheduler.ScheduledReportVehicleRef.Add(scheduledReportVehicle);
                    }
                }
                if (reportSchedulerItem.Driveref_driver_id > 0 && reportSchedulerItem.Repsch_id == reportSchedulerItem.Driveref_report_schedule_id)
                {
                    if (!scheduledReportDriverRefLookup.TryGetValue(Tuple.Create(Convert.ToInt32(reportSchedulerItem.Driveref_driver_id), reportSchedulerItem.Repsch_id), out _))
                    {
                        var scheduledReportDriver = ToScheduledReportDriverRefModel(reportSchedulerItem);
                        scheduledReportDriverRefLookup.Add(Tuple.Create(Convert.ToInt32(reportSchedulerItem.Driveref_driver_id), reportSchedulerItem.Repsch_id), scheduledReportDriver);
                        reportScheduler.ScheduledReportDriverRef.Add(scheduledReportDriver);
                    }
                }
                if (reportSchedulerItem.Schrep_id > 0 && reportSchedulerItem.Repsch_id == reportSchedulerItem.Schrep_schedule_report_id)
                {
                    if (!scheduledReportLookup.TryGetValue(Convert.ToInt32(reportSchedulerItem.Schrep_id), out _))
                    {
                        var scheduledReport = ToScheduledReportModel(reportSchedulerItem);
                        scheduledReportLookup.Add(Convert.ToInt32(reportSchedulerItem.Schrep_id), scheduledReport);
                        reportScheduler.ScheduledReport.Add(scheduledReport);
                    }
                }
            }
            foreach (var keyValuePair in reportSchedulerLookup)
            {
                //add report scheduler object along with child tables to report scheduler list 
                reportSchedulerList.Add(keyValuePair.Value);
            }
            return reportSchedulerList;
        }

        public ReportSchedulerMap ToReportSchedulerModel(ReportSchedulerResult request)
        {
            ReportSchedulerMap reportScheduler = new ReportSchedulerMap();
            reportScheduler.Id = request.Repsch_id;
            reportScheduler.ReportName = request.Rep_reportname;
            reportScheduler.OrganizationId = request.Repsch_organization_id;
            reportScheduler.ReportId = request.Repsch_report_id;
            reportScheduler.FrequencyType = request.Repsch_frequency_type;
            reportScheduler.Status = request.Repsch_status;
            reportScheduler.Type = request.Repsch_type;
            reportScheduler.StartDate = request.Repsch_start_date;
            reportScheduler.EndDate = request.Repsch_end_date;
            reportScheduler.Code = request.Repsch_code;
            reportScheduler.LastScheduleRunDate = request.Repsch_last_schedule_run_date;
            reportScheduler.NextScheduleRunDate = request.Repsch_next_schedule_run_date;
            reportScheduler.CreatedAt = request.Repsch_created_at;
            reportScheduler.CreatedBy = request.Repsch_created_by;
            reportScheduler.ModifiedAt = request.Repsch_modified_at;
            reportScheduler.ModifiedBy = request.Repsch_modified_by;
            reportScheduler.MailDescription = request.Repsch_mail_description;
            reportScheduler.MailSubject = request.Repsch_mail_subject;
            reportScheduler.ReportDispatchTime = request.Repsch_report_dispatch_time;
            reportScheduler.ScheduledReport = new List<ScheduledReport>();
            reportScheduler.ScheduledReportDriverRef = new List<ScheduledReportDriverRef>();
            reportScheduler.ScheduledReportRecipient = new List<ScheduledReportRecipient>();
            reportScheduler.ScheduledReportVehicleRef = new List<ScheduledReportVehicleRef>();
            return reportScheduler;
        }

        public ScheduledReportDriverRef ToScheduledReportDriverRefModel(ReportSchedulerResult request)
        {
            ScheduledReportDriverRef scheduledReportDriverRef = new ScheduledReportDriverRef();
            scheduledReportDriverRef.ScheduleReportId = request.Driveref_report_schedule_id;
            scheduledReportDriverRef.DriverId = request.Driveref_driver_id;
            scheduledReportDriverRef.DriverName = request.Dr_driverName;
            scheduledReportDriverRef.State = request.Driveref_state;
            scheduledReportDriverRef.CreatedAt = request.Driveref_created_at;
            scheduledReportDriverRef.CreatedBy = request.Driveref_created_by;
            scheduledReportDriverRef.ModifiedAt = request.Driveref_modified_at;
            scheduledReportDriverRef.ModifiedBy = request.Driveref_modified_by;
            return scheduledReportDriverRef;
        }

        public ScheduledReportVehicleRef ToScheduledReportVehicleRefModel(ReportSchedulerResult request)
        {
            ScheduledReportVehicleRef schedulereportvr = new ScheduledReportVehicleRef();
            schedulereportvr.ScheduleReportId = request.Vehref_report_schedule_id;
            schedulereportvr.VehicleGroupId = request.Vehref_vehicle_group_id;
            schedulereportvr.State = request.Vehref_state;
            schedulereportvr.CreatedAt = request.Vehref_created_at;
            schedulereportvr.CreatedBy = request.Vehref_created_by;
            schedulereportvr.ModifiedAt = request.Vehref_modified_at;
            schedulereportvr.ModifiedBy = request.Vehref_modified_by;
            schedulereportvr.VehicleId = request.Vehicleid;
            schedulereportvr.Vin = request.Vin;
            schedulereportvr.Regno = request.Regno;
            schedulereportvr.VehicleName = request.Vehiclename;
            schedulereportvr.VehicleGroupName = request.Vehiclegroupname;
            schedulereportvr.VehicleGroupType = request.Vehiclegrouptype;
            schedulereportvr.FunctionEnum = request.Functionenum;
            return schedulereportvr;
        }
        public ScheduledReportRecipient ToScheduledReportRecipientModel(ReportSchedulerResult request)
        {
            ScheduledReportRecipient schedulereportsr = new ScheduledReportRecipient();
            schedulereportsr.Id = request.Receipt_id;
            schedulereportsr.ScheduleReportId = request.Receipt_schedule_report_id;
            schedulereportsr.Email = request.Receipt_email;
            schedulereportsr.State = request.Receipt_state;
            schedulereportsr.CreatedAt = request.Receipt_created_at;
            schedulereportsr.ModifiedAt = request.Receipt_modified_at;
            return schedulereportsr;
        }
        public ScheduledReport ToScheduledReportModel(ReportSchedulerResult request)
        {
            ScheduledReport scheduledReport = new ScheduledReport();
            scheduledReport.Id = request.Schrep_id;
            scheduledReport.ScheduleReportId = request.Schrep_schedule_report_id;
            scheduledReport.DownloadedAt = request.Schrep_downloaded_at;
            scheduledReport.ValidTill = request.Schrep_valid_till;
            scheduledReport.CreatedAt = request.Schrep_created_at;
            scheduledReport.StartDate = request.Schrep_start_date;
            scheduledReport.EndDate = request.Schrep_end_date;
            return scheduledReport;
        }
    }
}
