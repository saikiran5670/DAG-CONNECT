using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.reportscheduler.entity;

namespace net.atos.daf.ct2.reportscheduler.repository
{
    public class ReportSchedulerRepository : IReportSchedulerRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartdataAccess;
        public ReportSchedulerRepository(IDataAccess dataAccess, IDataMartDataAccess dataMartdataAccess)
        {
            _dataAccess = dataAccess;
            _dataMartdataAccess = dataMartdataAccess;
        }

        #region Activate Report Scheduler

        #endregion

        #region Parameter Report Schedular 
        public async Task<IEnumerable<ReportType>> GetReportType(int accountid, int organizationid)
        {
            try
            {
                var parameterType = new DynamicParameters();
                var queryStatement = @"SELECT distinct r.id as Id,r.name as ReportName
					                      FROM master.report r						                     
						                     INNER JOIN master.Feature f ON f.id = r.feature_id AND f.state = 'A' 
						                     INNER JOIN master.FeatureSetFeature fsf ON fsf.feature_id = f.id
						                     INNER JOIN master.FeatureSet fset ON fsf.feature_set_id = fset.id AND fset.state = 'A'
						                     INNER JOIN master.Role ro ON ro.feature_set_id = fset.id AND ro.state = 'A'
						                     INNER JOIN master.AccountRole ar ON ro.id = ar.role_id and ar.organization_id = @organization_id
						                     INNER JOIN master.account acc ON acc.id = ar.account_id AND acc.state = 'A' AND acc.id = @account_id
											 INNER JOIN master.package pack ON pack.feature_set_id = fset.id AND fset.state = 'A' AND pack.state='A'
											 INNER JOIN master.subscription sub ON sub.package_id = pack.id AND sub.state = 'A' AND pack.state='A'
	 			                          WHERE acc.id = @account_id AND ar.Organization_id = @organization_id ; ";

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
        public async Task<IEnumerable<ReceiptEmails>> GetRecipientsEmails(int organizationid)
        {
            try
            {
                var parameterType = new DynamicParameters();
                var queryStatement = @"SELECT distinct 
                                        acc.email as Email from master.account acc
										INNER JOIN master.accountrole accrole
										ON acc.id = accrole.account_id
                                        Where accrole.organization_id= @organization_id
                                        AND state='A' AND email is not null;";
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
                var queryStatement = @"SELECT distinct
                                       id as Id
                                      ,dr.first_name || ' ' || dr.last_name AS DriverName                                         		  
                                      ,dr.driver_id as DriverId                                        		                                 		                                    		  
                                       FROM 
                                       master.driver dr
                                       Where dr.organization_id= @organization_id;";
                parameterType.Add("@organization_id", organizationid);
                IEnumerable<DriverDetail> driverdetails = await _dataMartdataAccess.QueryAsync<DriverDetail>(queryStatement, null);
                return driverdetails;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Create CreateReportSchedular
        public async Task<ReportScheduler> CreateReportSchedular(ReportScheduler report)
        {
            _dataAccess.Connection.Open();
            var transactionScope = _dataAccess.Connection.BeginTransaction();
            try
            {
                var parameterReportSchedular = new DynamicParameters();
                parameterReportSchedular.Add("@organization_id", report.OrganizationId);
                parameterReportSchedular.Add("@report_id", report.Id);
                parameterReportSchedular.Add("@frequency_type", report.FrequencyType);
                parameterReportSchedular.Add("@status", report.Status);
                parameterReportSchedular.Add("@type", report.Type);
                parameterReportSchedular.Add("@file_name", report.FileName);
                parameterReportSchedular.Add("@start_date", report.OrganizationId);
                parameterReportSchedular.Add("@end_date", report.EndDate);
                parameterReportSchedular.Add("@code", report.Code);
                parameterReportSchedular.Add("@last_schedule_run_date", report.LastScheduleRunDate);
                parameterReportSchedular.Add("@next_schedule_run_date", report.NextScheduleRunDate);
                parameterReportSchedular.Add("@created_at", report.CreatedAt);
                parameterReportSchedular.Add("@created_by", report.CreatedBy);
                parameterReportSchedular.Add("@modified_at", report.ModifiedAt);
                parameterReportSchedular.Add("@modified_by", report.ModifiedBy);
                parameterReportSchedular.Add("@mail_subject", report.MailSubject);
                parameterReportSchedular.Add("@mail_description", report.MailDescription);
                parameterReportSchedular.Add("@report_dispatch_time", report.ReportDispatchTime);

                string queryReportSchedular = @"INSERT INTO master.reportscheduler(organization_id, report_id,  frequency_type,
                                        status,type, file_name, start_date, end_date, code,
                                        last_schedule_run_date,next_schedule_run_date,created_at,created_by,modified_at,modified_by,
                                        mail_subject,mail_description,report_dispatch_time)

	                                    VALUES (@organization_id, @report_id, 
                                        @frequency_type, @status, 
                                        @type, @file_name, @start_date, @end_date, @code,@last_schedule_run_date,
                                        @next_schedule_run_date,@created_at,@created_by,
                                        @modified_at,@modified_by,@mail_subject,@mail_description,@report_dispatch_time) RETURNING id";


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
                    driverref.ScheduleReportId = report.Id;
                    int scheduledrid = await Createscheduledreportdriverref(driverref);
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
                parameterschedulerecipient.Add("@created_at", srecipient.CreatedAt);
                parameterschedulerecipient.Add("@modified_at", srecipient.ModifiedAt);

                string querySchedulerecipient = @"INSERT INTO master.scheduledreportrecipient(schedule_report_id, email, state, 
                                                     created_at, modified_at)
                                                 VALUES (@schedule_report_id, @emaile, @state, @created_at,@modified_at) RETURNING id";

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
                parameterScheduledReportDriverRef.Add("@created_at", sdriverref.CreatedAt);
                parameterScheduledReportDriverRef.Add("@created_by", sdriverref.CreatedBy);
                parameterScheduledReportDriverRef.Add("@modified_at", sdriverref.ModifiedAt);
                parameterScheduledReportDriverRef.Add("@modified_by", sdriverref.ModifiedBy);

                string queryscheduledreportdriverref = @"INSERT INTO master.scheduledreportdriverref(report_schedule_id, driver_id, state, 
                                                     created_at, created_by, modified_at,modified_by)
                                                 VALUES (@schedule_report_id,@driver_id,  @state, @created_at,@created_by,@modified_at,@modified_by) RETURNING id";

                var id = await _dataAccess.ExecuteScalarAsync<int>(queryscheduledreportdriverref, parameterScheduledReportDriverRef);
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
                parameterScheduledReportVehicleRef.Add("@schedule_report_id", svehicleref.ScheduleReportId);
                parameterScheduledReportVehicleRef.Add("@vehicle_group_id", svehicleref.VehicleGroupId);
                parameterScheduledReportVehicleRef.Add("@state", svehicleref.State);
                parameterScheduledReportVehicleRef.Add("@created_at", svehicleref.CreatedAt);
                parameterScheduledReportVehicleRef.Add("@created_by", svehicleref.CreatedBy);
                parameterScheduledReportVehicleRef.Add("@modified_at", svehicleref.ModifiedAt);
                parameterScheduledReportVehicleRef.Add("@modified_by", svehicleref.ModifiedBy);

                string queryscheduledreportvehicleref = @"INSERT INTO master.scheduledreportvehicleref(report_schedule_id, vehicle_group_id,  state, 
                                                     created_at,created_by, modified_at,modified_by)
                                                 VALUES (@schedule_report_id,@vehicle_group_id, @state, @created_at,@created_by,@modified_at,@modified_by) RETURNING id";

                var id = await _dataAccess.ExecuteScalarAsync<int>(queryscheduledreportvehicleref, parameterScheduledReportVehicleRef);
                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Update UpdateReportSchedular
        public Task<ReportScheduler> UpdateReportSchedular(ReportScheduler report) => throw new NotImplementedException();
        #endregion

        #region Get Report Scheduler
        public async Task<IEnumerable<ReportScheduler>> GetReportSchedulerList(int organizationid)
        {
            MapperRepo repositoryMapper = new MapperRepo();
            try
            {
                var parameterAlert = new DynamicParameters();

                string queryAlert = @"SELECT repsch.id as repsch_id, 
                                            repsch.organization_id as repsch_organization_id, 
                                            repsch.report_id as repsch_report_id, 
                                            repsch.frequency_type as repsch_frequency_type, 
                                            repsch.status as repsch_status, 
                                            repsch.type as repsch_type, 
                                            repsch.file_name as repsch_file_name, 
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
                                            vehref.vehicle_group_id as vehref_vehicle_group_id, 
                                            vehref.state as vehref_state, 
                                            vehref.created_at vehref_created_at, 
                                            vehref.created_by as vehref_created_by, 
                                            vehref.modified_at as vehref_modified_at, 
                                            vehref.modified_by as vehref_modified_by,
                                            schrep.id as schrep_id, 
                                            schrep.schedule_report_id as schrep_schedule_report_id, 
                                            schrep.report as schrep_report,
                                            schrep.downloaded_at as schrep_downloaded_at, 
                                            schrep.valid_till as schrep_valid_till, 
                                            schrep.created_at as schrep_created_at, 
                                            schrep.start_date as schrep_start_date, 
                                            schrep.end_date as schrep_end_date
	                                    FROM master.reportscheduler as repsch
	                                    LEFT JOIN master.scheduledreportdriverref as driveref
	                                    ON repsch.id=driveref.report_schedule_id AND driveref.state='A'
	                                    LEFT JOIN master.scheduledreportrecipient as receipt
	                                    ON repsch.id=receipt.schedule_report_id AND repsch.status='A' AND receipt.state='A'
	                                    LEFT JOIN master.scheduledreportvehicleref as vehref
	                                    ON repsch.id=vehref.report_schedule_id AND repsch.status='A' AND vehref.state='A'
	                                    LEFT JOIN master.scheduledreport as schrep
	                                    ON repsch.id=schrep.schedule_report_id AND repsch.status='A' ";

                queryAlert = queryAlert + " where repsch.organization_id = @organization_id and repsch.status<>'D'";
                parameterAlert.Add("@organization_id", organizationid);
                IEnumerable<ReportSchedulerResult> reportSchedulerResult = await _dataAccess.QueryAsync<ReportSchedulerResult>(queryAlert, parameterAlert);
                return repositoryMapper.GetReportSchedulerList(reportSchedulerResult);
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
                          SET state=@state 
                          WHERE id=@id 
                          AND organization_id=@organization_id";
                var parameter = new DynamicParameters();
                parameter.Add("@id", objReportStatusUpdateDeleteModel.ReportId);
                parameter.Add("@state", objReportStatusUpdateDeleteModel.Status);
                parameter.Add("@organization_id", objReportStatusUpdateDeleteModel.OrganizationId);
                return await _dataAccess.ExecuteAsync(query, parameter);
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
