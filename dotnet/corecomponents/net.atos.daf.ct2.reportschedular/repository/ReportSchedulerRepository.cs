﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.visibility;

namespace net.atos.daf.ct2.reportscheduler.repository
{
    public partial class ReportSchedulerRepository : IReportSchedulerRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IVisibilityManager _visibilityManager;
        private readonly Helper _helper;
        public ReportSchedulerRepository(IDataAccess dataAccess, IVisibilityManager visibilityManager)
        {
            _dataAccess = dataAccess;
            _visibilityManager = visibilityManager;
            _helper = new Helper();
        }

        #region Activate Report Scheduler

        #endregion

        #region Parameter Report Schedular 
        public async Task<IEnumerable<ReportType>> GetReportType(int accountid, int organizationid)
        {
            try
            {
                var parameterType = new DynamicParameters();
                var queryStatement =
                    @"SELECT DISTINCT r.id as Id,r.name as ReportName, trim(r.key) as Key, r.support_driver_sch_rep as IsDriver
                    FROM master.report r
                    INNER JOIN
                    (
	                    --Account Route
	                    SELECT f.id
	                    FROM master.Account acc
	                    INNER JOIN master.AccountRole ar ON acc.id = ar.account_id AND acc.id = @account_id AND ar.organization_id = @organization_id AND acc.state = 'A'
	                    INNER JOIN master.Role r ON ar.role_id = r.id AND r.state = 'A'
	                    INNER JOIN master.FeatureSet fset ON r.feature_set_id = fset.id AND fset.state = 'A'
	                    INNER JOIN master.FeatureSetFeature fsf ON fsf.feature_set_id = fset.id
	                    INNER JOIN master.Feature f ON f.id = fsf.feature_id AND f.state = 'A' AND f.type <> 'D' AND f.name not like 'api.%' and r.level <= f.level
	                    INTERSECT 
	                    (
		                    SELECT f.id
		                    FROM
		                    (
			                    SELECT pkg.feature_set_id
			                    FROM master.Package pkg
			                    INNER JOIN master.Subscription s ON s.package_id = pkg.id AND s.organization_id = @organization_id AND s.state = 'A' AND pkg.state = 'A'
			                    UNION
			                    SELECT pkg.feature_set_id FROM master.Package pkg WHERE pkg.type='P' AND pkg.state = 'A'	--Consider platform type packages
		                    ) subs
		                    INNER JOIN master.FeatureSet fset ON subs.feature_set_id = fset.id AND fset.state = 'A'
		                    INNER JOIN master.FeatureSetFeature fsf ON fsf.feature_set_id = fset.id
		                    INNER JOIN master.Feature f ON f.id = fsf.feature_id AND f.state = 'A' AND f.type <> 'D' AND f.name not like 'api.%'
	                    )    
                    ) features ON r.feature_id = features.id";

                parameterType.Add("@organization_id", organizationid);
                parameterType.Add("@account_id", accountid);

                IEnumerable<ReportType> reporttype = await _dataAccess.QueryAsync<ReportType>(queryStatement, parameterType);
                return reporttype;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<ReportType>> GetReportType(int accountId, int organizationId, int contextorgId, int roleId)
        {
            try
            {
                var parameterType = new DynamicParameters();
                var queryStatement = @"SELECT distinct rpt.id as Id,rpt.name as ReportName, trim(rpt.key) as Key, rpt.support_driver_sch_rep as IsDriver
                                        from 
                                        (
	                                        --Account Route
	                                        SELECT f.id
	                                        FROM master.Account acc
	                                        INNER JOIN master.AccountRole ar ON acc.id = ar.account_id AND acc.id = @account_id AND ar.organization_id = @organization_id AND ar.role_id = @role_id AND acc.state = 'A'
	                                        INNER JOIN master.Role r ON ar.role_id = r.id AND r.state = 'A'
	                                        INNER JOIN master.FeatureSet fset ON r.feature_set_id = fset.id AND fset.state = 'A'
 	                                        INNER JOIN master.FeatureSetFeature fsf ON fsf.feature_set_id = fset.id
	                                        INNER JOIN master.Feature f ON f.id = fsf.feature_id AND f.state = 'A' AND f.type <> 'D' and r.level <= f.level
			                                INNER JOIN master.Report rpt ON rpt.feature_id = f.id AND rpt.scheduled_report = 'Y'
	                                        INTERSECT
	                                        --Subscription Route
	                                        SELECT f.id
	                                        FROM
	                                        (
		                                        SELECT pkg.feature_set_id
		                                        FROM master.Package pkg
		                                        INNER JOIN master.Subscription s ON s.package_id = pkg.id AND s.organization_id = @context_org_id AND s.state = 'A' AND pkg.state = 'A'
		                                        UNION
		                                        SELECT pkg.feature_set_id FROM master.Package pkg WHERE pkg.type='P' AND pkg.state = 'A'    --Consider platform type packages
	                                        ) subs
	                                        INNER JOIN master.FeatureSet fset ON subs.feature_set_id = fset.id AND fset.state = 'A'
 	                                        INNER JOIN master.FeatureSetFeature fsf ON fsf.feature_set_id = fset.id
	                                        INNER JOIN master.Feature f ON f.id = fsf.feature_id AND f.state = 'A' AND f.type <> 'D'
			                                INNER JOIN master.Report rpt ON  rpt.feature_id = f.id AND rpt.scheduled_report = 'Y'
                                        ) fsets
                                        INNER JOIN master.Report rpt ON rpt.feature_id = fsets.id ; ";

                parameterType.Add("@organization_id", organizationId);
                parameterType.Add("@account_id", accountId);
                parameterType.Add("@context_org_id", contextorgId);
                parameterType.Add("@role_id", roleId);

                IEnumerable<ReportType> reporttype = await _dataAccess.QueryAsync<ReportType>(queryStatement, parameterType);
                return reporttype;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<ReceiptEmails>> GetRecipientsEmails(int organizationid)
        {
            try
            {
                var parameterType = new DynamicParameters();
                var queryStatement = @"SELECT distinct 
                                        acc.email as Email from master.account acc										
										INNER JOIN master.accountorg accrorg
										ON acc.id = accrorg.account_id
										INNER JOIN master.organization org
										ON accrorg.organization_id = org.id
                                        Where org.id= @organization_id
                                        AND acc.state='A' AND email is not null
										AND acc.type <>'S' AND org.state='A';";
                parameterType.Add("@organization_id", organizationid);
                IEnumerable<ReceiptEmails> reporttype = await _dataAccess.QueryAsync<ReceiptEmails>(queryStatement, null);
                return reporttype;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<DriverDetail>> GetDriverDetails(int organizationid)
        {
            try
            {
                var parameterType = new DynamicParameters();
                var queryStatement = @"SELECT id as Id, driver_id_ext as DriverId, first_name ||' '||last_name as DriverName
	                                    FROM master.driver 
	                                    Where organization_id= @organization_id
	                                    AND opt_in='I'
	                                    AND state='A';";
                parameterType.Add("@organization_id", organizationid);
                IEnumerable<DriverDetail> driverdetails = await _dataAccess.QueryAsync<DriverDetail>(queryStatement, null);
                return driverdetails;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Create CreateReportSchedular
        public async Task<ReportSchedulerMap> CreateReportScheduler(ReportSchedulerMap report)
        {
            _dataAccess.Connection.Open();
            var transactionScope = _dataAccess.Connection.BeginTransaction();
            try
            {
                string queryReportSchedular = @"INSERT INTO master.reportscheduler
                                                (organization_id, 
                                                report_id, 
                                                frequency_type,
                                                status,
                                                type,                                                 
                                                start_date, 
                                                end_date, 
                                                code,
                                                last_schedule_run_date,
                                                next_schedule_run_date,
                                                created_at,
                                                created_by,
                                                modified_at,
                                                modified_by,
                                                mail_subject,
                                                mail_description,
                                                report_dispatch_time)
	                                            VALUES (
                                                @organization_id, 
                                                @report_id, 
                                                @frequency_type, 
                                                @status, 
                                                @type,                                               
                                                @start_date, 
                                                @end_date, 
                                                @code,
                                                @last_schedule_run_date,
                                                @next_schedule_run_date,
                                                @created_at,
                                                @created_by,
                                                @modified_at,
                                                @modified_by,
                                                @mail_subject,
                                                @mail_description,
                                                @report_dispatch_time) RETURNING id";

                var parameterReportSchedular = new DynamicParameters();
                parameterReportSchedular.Add("@organization_id", report.OrganizationId);
                parameterReportSchedular.Add("@report_id", report.ReportId);
                parameterReportSchedular.Add("@frequency_type", report.FrequencyType);
                parameterReportSchedular.Add("@status", report.Status);
                parameterReportSchedular.Add("@type", report.Type);
                parameterReportSchedular.Add("@start_date", report.StartDate);
                parameterReportSchedular.Add("@end_date", report.EndDate);
                parameterReportSchedular.Add("@code", report.Code);
                parameterReportSchedular.Add("@last_schedule_run_date", report.LastScheduleRunDate);
                parameterReportSchedular.Add("@next_schedule_run_date", report.NextScheduleRunDate);
                parameterReportSchedular.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameterReportSchedular.Add("@created_by", report.CreatedBy);
                parameterReportSchedular.Add("@modified_at", report.ModifiedAt);
                parameterReportSchedular.Add("@modified_by", report.ModifiedBy);
                parameterReportSchedular.Add("@mail_subject", report.MailSubject);
                parameterReportSchedular.Add("@mail_description", report.MailDescription);
                parameterReportSchedular.Add("@report_dispatch_time", report.ReportDispatchTime);

                var reportId = await _dataAccess.ExecuteScalarAsync<int>(queryReportSchedular, parameterReportSchedular);
                report.Id = reportId;

                foreach (var recipient in report.ScheduledReportRecipient)
                {
                    recipient.ScheduleReportId = report.Id;
                    recipient.Id = await CreateScheduleRecipient(recipient);
                }

                foreach (var vehicleref in report.ScheduledReportVehicleRef)
                {
                    vehicleref.ScheduleReportId = report.Id;
                    vehicleref.ScheduleReportId = await Createschedulereportvehicleref(vehicleref);
                }

                foreach (var driverref in report.ScheduledReportDriverRef)
                {
                    if (driverref.DriverId == 0)//ALL driver list
                    {
                        IEnumerable<DriverDetail> driverList = await GetDriverDetails(report.OrganizationId);
                        foreach (var item in driverList)
                        {
                            driverref.ScheduleReportId = report.Id;
                            driverref.DriverId = Convert.ToInt32(item.Id);
                            int scheduledrid = await Createscheduledreportdriverref(driverref);
                        }
                    }
                    else //Single driver
                    {
                        driverref.ScheduleReportId = report.Id;
                        int scheduledrid = await Createscheduledreportdriverref(driverref);
                    }

                }
                transactionScope.Commit();
            }
            catch (Exception)
            {
                transactionScope.Rollback();
                throw;
            }
            finally
            {
                _dataAccess.Connection.Close();
            }
            return report;
        }

        private async Task<int> CreateScheduleRecipient(ScheduledReportRecipient srecipient)
        {
            try
            {
                var parameterschedulerecipient = new DynamicParameters();
                parameterschedulerecipient.Add("@schedule_report_id", srecipient.ScheduleReportId);
                parameterschedulerecipient.Add("@emaile", srecipient.Email);
                parameterschedulerecipient.Add("@state", srecipient.State);
                parameterschedulerecipient.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameterschedulerecipient.Add("@modified_at", srecipient.ModifiedAt);

                string querySchedulerecipient = @"INSERT INTO master.scheduledreportrecipient
                                                (schedule_report_id, 
                                                email, 
                                                state, 
                                                created_at,
                                                modified_at)
                                                VALUES 
                                                (@schedule_report_id, 
                                                 @emaile, 
                                                 @state, 
                                                 @created_at,
                                                 @modified_at) RETURNING schedule_report_id";

                var id = await _dataAccess.ExecuteScalarAsync<int>(querySchedulerecipient, parameterschedulerecipient);
                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<int> DeleteScheduleRecipient(int reportscheduleId)
        {
            try
            {
                var parameterschedulerecipient = new DynamicParameters();
                parameterschedulerecipient.Add("@schedule_report_id", reportscheduleId);

                string querySchedulerecipient = @"DELETE from master.scheduledreportrecipient
                                                WHERE schedule_report_id=@schedule_report_id RETURNING schedule_report_id";

                var id = await _dataAccess.ExecuteScalarAsync<int>(querySchedulerecipient, parameterschedulerecipient);
                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<int> Createscheduledreportdriverref(ScheduledReportDriverRef sdriverref)
        {
            try
            {
                var parameterScheduledReportDriverRef = new DynamicParameters();
                parameterScheduledReportDriverRef.Add("@schedule_report_id", sdriverref.ScheduleReportId);
                parameterScheduledReportDriverRef.Add("@driver_id", sdriverref.DriverId);

                parameterScheduledReportDriverRef.Add("@state", sdriverref.State);
                parameterScheduledReportDriverRef.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameterScheduledReportDriverRef.Add("@created_by", sdriverref.CreatedAt);
                parameterScheduledReportDriverRef.Add("@modified_at", sdriverref.ModifiedAt);
                parameterScheduledReportDriverRef.Add("@modified_by", sdriverref.ModifiedBy);

                string queryscheduledreportdriverref = @"INSERT INTO master.scheduledreportdriverref
                                                        (report_schedule_id, 
                                                        driver_id, 
                                                        state, 
                                                        created_at, 
                                                        created_by,
                                                        modified_at,
                                                        modified_by)
                                                        VALUES 
                                                        (@schedule_report_id,
                                                        @driver_id,  
                                                        @state, 
                                                        @created_at,
                                                        @created_by,
                                                        @modified_at,
                                                        @modified_by) RETURNING driver_id";

                var id = await _dataAccess.ExecuteScalarAsync<int>(queryscheduledreportdriverref, parameterScheduledReportDriverRef);
                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<int> Deletescheduledreportdriverref(int reportscheduleId)
        {
            try
            {
                var parameterschedulerecipient = new DynamicParameters();
                parameterschedulerecipient.Add("@report_schedule_id", reportscheduleId);

                string querySchedulerecipient = @"DELETE from master.scheduledreportdriverref
                                                WHERE report_schedule_id=@report_schedule_id RETURNING report_schedule_id";

                var id = await _dataAccess.ExecuteScalarAsync<int>(querySchedulerecipient, parameterschedulerecipient);
                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<int> Createschedulereportvehicleref(ScheduledReportVehicleRef svehicleref)
        {
            try
            {
                var parameterScheduledReportVehicleRef = new DynamicParameters();
                parameterScheduledReportVehicleRef.Add("@report_schedule_id", svehicleref.ScheduleReportId);
                parameterScheduledReportVehicleRef.Add("@vehicle_group_id", svehicleref.VehicleGroupId);
                parameterScheduledReportVehicleRef.Add("@state", svehicleref.State);
                parameterScheduledReportVehicleRef.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameterScheduledReportVehicleRef.Add("@created_by", svehicleref.CreatedBy);
                parameterScheduledReportVehicleRef.Add("@modified_at", svehicleref.ModifiedAt);
                parameterScheduledReportVehicleRef.Add("@modified_by", svehicleref.ModifiedBy);

                string queryscheduledreportvehicleref = @"INSERT INTO master.scheduledreportvehicleref
                                                        (report_schedule_id, 
                                                        vehicle_group_id,  
                                                        state, 
                                                        created_at,
                                                        created_by,
                                                        modified_at,
                                                        modified_by)
                                                        VALUES 
                                                        (@report_schedule_id,
                                                        @vehicle_group_id, 
                                                        @state, 
                                                        @created_at,
                                                        @created_by,
                                                        @modified_at,
                                                        @modified_by) RETURNING vehicle_group_id";

                var id = await _dataAccess.ExecuteScalarAsync<int>(queryscheduledreportvehicleref, parameterScheduledReportVehicleRef);
                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<int> Deleteschedulereportvehicleref(int reportscheduleId)
        {
            try
            {
                var parameterschedulerecipient = new DynamicParameters();
                parameterschedulerecipient.Add("@report_schedule_id", reportscheduleId);

                string querySchedulerecipient = @"DELETE from master.scheduledreportvehicleref
                                                WHERE report_schedule_id=@report_schedule_id RETURNING report_schedule_id";

                var id = await _dataAccess.ExecuteScalarAsync<int>(querySchedulerecipient, parameterschedulerecipient);
                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Update UpdateReportSchedular
        public async Task<ReportSchedulerMap> UpdateReportScheduler(ReportSchedulerMap report)
        {
            _dataAccess.Connection.Open();
            var transactionScope = _dataAccess.Connection.BeginTransaction();
            try
            {
                string queryReportSchedular = @"UPDATE master.reportscheduler
                                                SET 
	                                            frequency_type=@frequency_type, 
	                                            status=@status, 
	                                            type=@type,	                                            
	                                            start_date=@start_date, 
	                                            end_date=@end_date, 
	                                            code=@code, 
	                                            last_schedule_run_date=@last_schedule_run_date, 
	                                            next_schedule_run_date=@next_schedule_run_date, 
	                                            modified_at=@modified_at, 
	                                            modified_by=@modified_by, 
	                                            mail_subject=@mail_subject, 
	                                            mail_description=@mail_description, 
	                                            report_dispatch_time=@report_dispatch_time
                                                WHERE id=@id
                                                RETURNING id";

                var parameterReportSchedular = new DynamicParameters();
                parameterReportSchedular.Add("@id", report.Id);
                parameterReportSchedular.Add("@frequency_type", report.FrequencyType);
                parameterReportSchedular.Add("@status", report.Status);
                parameterReportSchedular.Add("@type", report.Type);
                parameterReportSchedular.Add("@start_date", report.StartDate);
                parameterReportSchedular.Add("@end_date", report.EndDate);
                parameterReportSchedular.Add("@code", report.Code);
                parameterReportSchedular.Add("@last_schedule_run_date", report.LastScheduleRunDate);
                parameterReportSchedular.Add("@next_schedule_run_date", report.NextScheduleRunDate);
                parameterReportSchedular.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameterReportSchedular.Add("@modified_by", report.ModifiedBy);
                parameterReportSchedular.Add("@mail_subject", report.MailSubject);
                parameterReportSchedular.Add("@mail_description", report.MailDescription);
                parameterReportSchedular.Add("@report_dispatch_time", report.ReportDispatchTime);

                var reportId = await _dataAccess.ExecuteScalarAsync<int>(queryReportSchedular, parameterReportSchedular);
                report.Id = reportId;

                int recepiet = await _dataAccess.QuerySingleAsync<int>("SELECT count(*) FROM master.scheduledreportrecipient where schedule_report_id=@schedule_report_id", new { schedule_report_id = report.Id });
                if (recepiet > 0)
                {
                    await DeleteScheduleRecipient(report.Id);
                }
                foreach (var recipient in report.ScheduledReportRecipient)
                {
                    recipient.ScheduleReportId = report.Id;
                    recipient.ModifiedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                    recipient.Id = await CreateScheduleRecipient(recipient);
                }

                int vehiclerefid = await _dataAccess.QuerySingleAsync<int>("SELECT count(*) FROM master.scheduledreportvehicleref where report_schedule_id=@report_schedule_id", new { report_schedule_id = report.Id });
                if (vehiclerefid > 0)
                {
                    await Deleteschedulereportvehicleref(report.Id);
                }
                foreach (var vehicleref in report.ScheduledReportVehicleRef)
                {
                    vehicleref.ScheduleReportId = report.Id;
                    vehicleref.ModifiedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                    vehicleref.ScheduleReportId = await Createschedulereportvehicleref(vehicleref);
                }

                int driverrefid = await _dataAccess.QuerySingleAsync<int>("SELECT count(*) FROM master.scheduledreportdriverref where report_schedule_id=@report_schedule_id", new { report_schedule_id = report.Id });
                if (driverrefid > 0)
                {
                    await Deletescheduledreportdriverref(report.Id);
                }
                foreach (var driverref in report.ScheduledReportDriverRef)
                {
                    if (driverref.DriverId == 0)//ALL driver list
                    {
                        IEnumerable<DriverDetail> driverList = await GetDriverDetails(report.OrganizationId);
                        foreach (var item in driverList)
                        {
                            driverref.ScheduleReportId = report.Id;
                            driverref.DriverId = Convert.ToInt32(item.Id);
                            driverref.ModifiedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                            int scheduledrid = await Createscheduledreportdriverref(driverref);
                        }
                    }
                    else //Single driver
                    {
                        driverref.ScheduleReportId = report.Id;
                        driverref.ModifiedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                        int scheduledrid = await Createscheduledreportdriverref(driverref);
                    }

                }

                transactionScope.Commit();
            }
            catch (Exception)
            {
                transactionScope.Rollback();
                throw;
            }
            finally
            {
                _dataAccess.Connection.Close();
            }
            return report;
        }
        #endregion

        #region Get Report Scheduler
        public async Task<IEnumerable<ReportSchedulerMap>> GetReportSchedulerList(int organizationid, List<int> vehicleIds, List<int> groupIds)
        {
            MapperRepo repositoryMapper = new MapperRepo();
            try
            {
                var parameter = new DynamicParameters();

                string query = @"SELECT repsch.id as repsch_id, 
                                            repsch.organization_id as repsch_organization_id, 
                                            repsch.report_id as repsch_report_id,
											rep.name as rep_reportname,
                                            repsch.frequency_type as repsch_frequency_type, 
                                            repsch.status as repsch_status, 
                                            repsch.type as repsch_type,                                            
                                            repsch.start_date as repsch_start_date, 
                                            repsch.end_date as repsch_end_date, 
                                            repsch.code as repsch_code, 
                                            repsch.last_schedule_run_date as repsch_last_schedule_run_date, 
                                            repsch.next_schedule_run_date as repsch_next_schedule_run_date, 
                                            repsch.created_at as repsch_created_at, 
                                            repsch.created_by as repsch_created_by, 
                                            repsch.modified_at as repsch_modified_at, 
                                            repsch.modified_by as repsch_modified_by, 
                                            repsch.mail_subject as repsch_mail_subject, 
                                            repsch.mail_description as repsch_mail_description,
                                            repsch.report_dispatch_time as repsch_report_dispatch_time,
                                            driveref.report_schedule_id as driveref_report_schedule_id, 
                                            driveref.driver_id as driveref_driver_id,
											dr.first_name ||' '||dr.last_name as dr_driverName,
                                            driveref.state as driveref_state, 
                                            driveref.created_at as driveref_created_at, 
                                            driveref.created_by as driveref_created_by, 
                                            driveref.modified_at as driveref_modified_at, 
                                            driveref.modified_by as driveref_modified_by,
                                            receipt.id as receipt_id, 
                                            receipt.schedule_report_id as receipt_schedule_report_id, 
                                            receipt.email as receipt_email, 
                                            receipt.state as receipt_state, 
                                            receipt.created_at as receipt_created_at, 
                                            receipt.modified_at as receipt_modified_at,
                                            vehref.report_schedule_id as vehref_report_schedule_id, 
                                            (CASE WHEN grp.group_type='S' THEN 0 ELSE vehref.vehicle_group_id END) as vehref_vehicle_group_id, 
                                            vehref.state as vehref_state, 
                                            vehref.created_at vehref_created_at, 
                                            vehref.created_by as vehref_created_by, 
                                            vehref.modified_at as vehref_modified_at, 
                                            vehref.modified_by as vehref_modified_by,
                                            (CASE WHEN grp.group_type='S' THEN vehs.id END) as vehicleid,
                                            (CASE WHEN grp.group_type='S' THEN vehs.vin END) as vin,
                                            (CASE WHEN grp.group_type='S' THEN vehs.license_plate_number END) as regno,
					                        (CASE WHEN grp.group_type='S' THEN vehs.name END) as vehiclename,
					                        (CASE WHEN grp.group_type<>'S' THEN grp.name END) as vehiclegroupname,
                                            grp.group_type as vehiclegrouptype,
                                            grp.function_enum as functionenum                                           
	                                    FROM master.reportscheduler as repsch
	                                    LEFT JOIN master.scheduledreportdriverref as driveref
	                                    ON repsch.id=driveref.report_schedule_id AND driveref.state='A'
	                                    LEFT JOIN master.scheduledreportrecipient as receipt
	                                    ON repsch.id=receipt.schedule_report_id AND repsch.status <>'D' AND receipt.state='A'
	                                    LEFT JOIN master.scheduledreportvehicleref as vehref
	                                    ON repsch.id=vehref.report_schedule_id AND repsch.status <>'D' AND vehref.state='A'	                                   
                                        LEFT JOIN master.group grp 
					                    on vehref.vehicle_group_id=grp.id
					                    LEFT JOIN master.groupref vgrpref
					                    on  grp.id=vgrpref.group_id and grp.object_type='V'	
					                    LEFT JOIN master.vehicle veh
					                    on vgrpref.ref_id=veh.id and veh.id = ANY(@vehicleIds)
                                        LEFT JOIN master.vehicle vehs
					                    on grp.ref_id=vehs.id and grp.group_type='S' and vehs.id = ANY(@vehicleIds)
										LEFT JOIN master.driver dr
										on driveref.driver_id = dr.id and dr.state='A'
										INNER JOIN master.report rep
										on rep.id=repsch.report_id";
                long currentdate = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                query = query + " where repsch.organization_id = @organization_id and repsch.status<>'D'";
                parameter.Add("@organization_id", organizationid);
                parameter.Add("@currentDate", currentdate);
                parameter.Add("@vehicleIds", vehicleIds);
                parameter.Add("@groupIds", groupIds);
                IEnumerable<ReportSchedulerResult> reportSchedulerResult = await _dataAccess.QueryAsync<ReportSchedulerResult>(query, parameter);
                return await repositoryMapper.GetReportSchedulerList(reportSchedulerResult);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScheduledReport>> GetScheduledReport(int reportschedulerId)
        {
            MapperRepo repositoryMapper = new MapperRepo();
            try
            {
                var parameter = new DynamicParameters();

                string query = @"SELECT schrep.id as Id, 
                                            schrep.schedule_report_id as ScheduleReportId,                                            
                                            schrep.downloaded_at as DownloadedAt, 
                                            schrep.valid_till as ValidTill, 
                                            schrep.created_at as CreatedAt, 
                                            schrep.start_date as StartDate, 
                                            schrep.end_date as EndDate
											FROM master.scheduledreport as schrep
											WHERE schrep.schedule_report_id =@reportschedulerId AND schrep.valid_till > @currentDate";
                long currentdate = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                parameter.Add("@reportschedulerId", reportschedulerId);
                parameter.Add("@currentDate", currentdate);
                return await _dataAccess.QueryAsync<ScheduledReport>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region EnableDisableDeleteReport
        public async Task<int> ManipulateReportSchedular(ReportStatusUpdateDeleteModel objReportStatusUpdateDeleteModel)
        {
            try
            {
                string query = string.Empty;

                query = @"UPDATE master.reportscheduler 
                          SET status=@status 
                          WHERE id=@id 
                          AND organization_id=@organization_id
                          RETURNING id";
                var parameter = new DynamicParameters();
                parameter.Add("@id", objReportStatusUpdateDeleteModel.ReportId);
                parameter.Add("@status", objReportStatusUpdateDeleteModel.Status);
                parameter.Add("@organization_id", objReportStatusUpdateDeleteModel.OrganizationId);

                int rowEffected = await _dataAccess.ExecuteAsync(query, parameter);
                if (rowEffected > 0)
                {
                    return objReportStatusUpdateDeleteModel.ReportId;// to show reportid in gRPC message
                }
                else
                {
                    return 0;//to return Failed Message in grpc
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region GetPDFinBinaryFormatByReportId
        public async Task<PDFReportScreenModel> GetPDFBinaryFormatById(ReportPDFByidModel request)
        {
            try
            {
                var param = new DynamicParameters();
                string query = @"SELECT 
                                    id as Id,
                                    schedule_report_id as ScheduleReportId,
                                    report as Report,
                                    token as Token,
                                    downloaded_at as DownloadedAt,
                                    valid_till as ValidTill,
                                    created_at as CreatedAt,
                                    start_date as StartDate,
                                    end_date as EndDate,
                                    is_mail_send as IsMailSend,
                                    file_name as FileName
                                FROM master.scheduledreport
                                WHERE id=@id";
                param.Add("@id", request.Id);
                var data = await _dataAccess.QueryAsync<PDFReportScreenModel>(query, param);
                return data.FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region GetPDFinBinaryFormatByToken
        public async Task<PDFReportScreenModel> GetPDFBinaryFormatByToken(ReportPDFBytokenModel request)
        {
            try
            {
                var param = new DynamicParameters();
                string query = @"SELECT 
                                    id as Id,
                                    schedule_report_id as ScheduleReportId,
                                    report as Report,
                                    token as Token,
                                    downloaded_at as DownloadedAt,
                                    valid_till as ValidTill,
                                    created_at as CreatedAt,
                                    start_date as StartDate,
                                    end_date as EndDate,
                                    is_mail_send as IsMailSend,
                                    file_name as FileName
                                FROM master.scheduledreport                                
                                WHERE token=@token";
                param.Add("@token", Guid.Parse(request.Token));
                var data = await _dataAccess.QueryAsync<PDFReportScreenModel>(query, param);
                return data.FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Update PDFBinary Record once downloaded by Token
        public async Task<string> UpdatePDFBinaryRecordByToken(string token)
        {
            try
            {
                string query = string.Empty;
                query = @"UPDATE master.scheduledreport 
                          SET downloaded_at=@downloaded_at 
                          WHERE token=@token";
                var parameter = new DynamicParameters();
                parameter.Add("@downloaded_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameter.Add("@token", Guid.Parse(token));
                int rowEffected = await _dataAccess.ExecuteAsync(query, parameter);
                if (rowEffected > 0)
                {
                    return token;// to check condition in gRPC message
                }
                else
                {
                    return string.Empty;//to return Failed Message in grpc
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Get Feature id by Account Id and Org ID
        public async Task<IEnumerable<int>> GetfeaturesByAccountAndOrgId(int accountid, int organizationid, string featureName = "alerts.")
        {
            try
            {
                var parameterType = new DynamicParameters();
                var queryStatement =
                    @"SELECT DISTINCT features.id as Id
                    FROM 
                    (
	                    --Account Route
	                    SELECT f.id
	                    FROM master.Account acc
	                    INNER JOIN master.AccountRole ar ON acc.id = ar.account_id AND acc.id = @account_id AND ar.organization_id = @organization_id AND acc.state = 'A'
	                    INNER JOIN master.Role r ON ar.role_id = r.id AND r.state = 'A'
	                    INNER JOIN master.FeatureSet fset ON r.feature_set_id = fset.id AND fset.state = 'A'
	                    INNER JOIN master.FeatureSetFeature fsf ON fsf.feature_set_id = fset.id
	                    INNER JOIN master.Feature f ON f.id = fsf.feature_id AND f.state = 'A' AND f.type <> 'D' AND lower(f.name) like @feature_name and r.level <= f.level
	                    INTERSECT 
	                    (
		                    SELECT f.id
		                    FROM
		                    (
			                    SELECT pkg.feature_set_id
			                    FROM master.Package pkg
			                    INNER JOIN master.Subscription s ON s.package_id = pkg.id AND s.organization_id = @organization_id AND s.state = 'A' AND pkg.state = 'A'
			                    UNION
			                    SELECT pkg.feature_set_id FROM master.Package pkg WHERE pkg.type='P' AND pkg.state = 'A'	--Consider platform type packages
		                    ) subs
		                    INNER JOIN master.FeatureSet fset ON subs.feature_set_id = fset.id AND fset.state = 'A'
		                    INNER JOIN master.FeatureSetFeature fsf ON fsf.feature_set_id = fset.id
		                    INNER JOIN master.Feature f ON f.id = fsf.feature_id AND f.state = 'A' AND f.type <> 'D' AND lower(f.name) like @feature_name
	                    )    
                    ) features";

                parameterType.Add("@organization_id", organizationid);
                parameterType.Add("@account_id", accountid);
                parameterType.Add("@feature_name", $"{featureName}%");

                return await _dataAccess.QueryAsync<int>(queryStatement, parameterType);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
