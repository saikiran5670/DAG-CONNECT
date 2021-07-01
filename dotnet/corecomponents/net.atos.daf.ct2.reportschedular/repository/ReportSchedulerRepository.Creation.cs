using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.reportscheduler.repository
{
    public partial class ReportSchedulerRepository : IReportSchedulerRepository
    {
        public Task<IEnumerable<ReportCreationScheduler>> GetReportCreationSchedulerList()
        {
            try
            {
                #region Query GetReportCreationScheduler
                var query = @"select rs.id as Id, rs.organization_id as OrganizationId, rs.report_id as ReportId, 
                            r.name as ReportName, trim(r.key) as ReportKey,rs.frequency_type as FrequecyType, rs.status as Status, 
                            rs.type as Type, rs.start_date as StartDate, rs.end_date as EndDate,
                            trim(rs.code) as Code, rs.last_schedule_run_date as LastScheduleRunDate, 
                            rs.next_schedule_run_date as NextScheduleRunDate, rs.created_at as CreatedAt, 
                            rs.created_by as CreatedBy, rs.modified_at as ModifiedAT, rs.modified_by as ModifiedBy, 
                            rs.mail_subject as MailSubject, rs.mail_description as MailDescription, 
                            rs.report_dispatch_time as ReportDispatchTime,
							ap.timezone_id as TimeZoneId,
                            ap.language_id as LanguageId,
                            ap.currency_id as CurrencyId,
                            ap.unit_id as UnitId,
                            ap.vehicle_display_id as VehicleDisplayId,
                            ap.date_format_id as DateFormatId,
                            ap.time_format_id as TimeFormatId
                            from master.reportscheduler rs
                                 inner join master.report r on date_trunc('day', (to_timestamp(rs.next_schedule_run_date/1000) AT TIME ZONE 'UTC'))=
date_trunc('day', NOW() AT TIME ZONE 'UTC') and rs.status = 'A' and r.id = rs.report_id
								 left Join master.account ac on ac.id = rs.created_by and ac.state='A'
	                             left join master.accountpreference ap on ap.id = ac.preference_id								 
								 left join master.scheduledreport sr on sr.schedule_report_id = rs.id and rs.start_date = sr.start_date and  sr.end_date = rs.end_date
                            where sr.id is null order by rs.next_schedule_run_date";
                #endregion
                return _dataAccess.QueryAsync<ReportCreationScheduler>(query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<IEnumerable<VehicleList>> GetVehicleList(int reprotSchedulerId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@report_schedule_id", reprotSchedulerId);
                var query = @"with cte_VehicaleList
                            AS (
                            select distinct ref_id as VehicleId from master.group where id  in (select vehicle_group_id from master.scheduledreportvehicleref where report_schedule_id = @report_schedule_id) and group_type = 'S' and object_type = 'V'
                            union
                            select distinct ref_id as VehicleId 
                            from master.groupref
                            where group_id in (select distinct id from master.group where id in (select vehicle_group_id from master.scheduledreportvehicleref where report_schedule_id = @report_schedule_id) and group_type = 'G' and object_type = 'V')
                            )
                            select distinct v.id as Id ,v.vin as VIN ,v.name as VehicleName,v.license_plate_number as RegistrationNo
                            from cte_VehicaleList vl
                                 inner join master.vehicle v on v.id = vl.VehicleId";
                return _dataAccess.QueryAsync<VehicleList>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<VehicleList> GetVehicleListForSingle(int reprotSchedulerId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@report_schedule_id", reprotSchedulerId);
                var query = @"with cte_VehicaleList
                            AS (
                            select distinct ref_id as VehicleId from master.group where id  in (select vehicle_group_id from master.scheduledreportvehicleref where report_schedule_id = @report_schedule_id) and group_type = 'S' and object_type = 'V'
                            )
                            select distinct v.id as Id ,v.vin as VIN ,v.name as VehicleName,v.license_plate_number as RegistrationNo
                            from cte_VehicaleList vl
                                 inner join master.vehicle v on v.id = vl.VehicleId";
                return _dataAccess.QueryFirstOrDefaultAsync<VehicleList>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<IEnumerable<UserTimeZone>> GetUserTimeZone()
        {
            try
            {
                var query = @"select id as Id, name as Name from master.timezone";
                return _dataAccess.QueryAsync<UserTimeZone>(query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<IEnumerable<UserDateFormat>> GetUserDateFormat()
        {
            try
            {
                var query = @"select id as Id, name as Name from master.dateformat";
                return _dataAccess.QueryAsync<UserDateFormat>(query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<IEnumerable<UserTimeFormat>> GetUserTimeFormat()
        {
            try
            {
                var query = @"select id as Id, key as Key from master.timeformat";
                return _dataAccess.QueryAsync<UserTimeFormat>(query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<IEnumerable<UnitName>> GetUnitName()
        {
            try
            {
                var query = @"select id as Id, key as Key from master.unit";
                return _dataAccess.QueryAsync<UnitName>(query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<ReportLogo> GetReportLogo(int accountId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@account_id", accountId);
                var query = @"select ic.icon as Image
                                from master.account acc
                                     inner join master.accountpreference ap on acc.id = @account_id and ap.id = acc.preference_id
	                                 inner join master.icon ic on ic.id = ap.icon_id";
                return _dataAccess.QueryFirstOrDefaultAsync<ReportLogo>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<int> InsertReportPDF(ScheduledReport scheduledReport)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@schedule_report_id", scheduledReport.ScheduleReportId);
                parameter.Add("@report", scheduledReport.Report);
                parameter.Add("@token", scheduledReport.Token);
                parameter.Add("@valid_till", scheduledReport.ValidTill);
                parameter.Add("@created_at", scheduledReport.CreatedAt);
                parameter.Add("@start_date", scheduledReport.StartDate);
                parameter.Add("@end_date", scheduledReport.EndDate);
                parameter.Add("@is_mail_send", scheduledReport.IsMailSend);
                parameter.Add("@file_name", scheduledReport.FileName);

                var query = @"insert into master.scheduledreport (schedule_report_id, report, token,
									valid_till, created_at, start_date, end_date,
									is_mail_send,file_name)
                               values (@schedule_report_id, @report, @token, @valid_till,
		                               @created_at, @start_date, @end_date,@is_mail_send,@file_name) RETURNING id";
                return _dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<IEnumerable<ReportColumnName>> GetColumnName(int reportId, string languageCode)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@report_id", reportId);
                parameter.Add("@code", languageCode);
                var query = @"select da.key as Key,reverse(split_part(reverse(ts.value), '.',1)) as Value
                              from master.report r 
                                 inner join master.reportattribute ra on r.id = @report_id and  r.id = ra.report_id
	                             inner join master.dataattribute da on ra.data_attribute_id = da.id
	                             left join translation.translation ts on ts.code = @code and ts.name = da.key ";
                return _dataAccess.QueryAsync<ReportColumnName>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
