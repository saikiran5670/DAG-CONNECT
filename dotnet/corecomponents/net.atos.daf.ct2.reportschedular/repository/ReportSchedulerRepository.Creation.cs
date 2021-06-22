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
                            r.name as ReportName, r.key as ReportKey,rs.frequency_type as FrequecyType, rs.status as Status, 
                            rs.type as Type, rs.start_date as StartDate, rs.end_date as EndDate,
                            rs.code as Code, rs.last_schedule_run_date as LastScheduleRunDate, 
                            rs.next_schedule_run_date as NextScheduleRunDate, rs.created_at as CreatedAt, 
                            rs.created_by as CreatedBy, rs.modified_at as ModifiedAT, rs.modified_by as ModifiedBy, 
                            rs.mail_subject as MailSubject, rs.mail_description as MailDescription, 
                            rs.report_dispatch_time as ReportDispatchTime
                            from master.reportscheduler rs
                                 inner join master.report r on to_date(TO_CHAR(TO_TIMESTAMP(rs.next_schedule_run_date / 1000), 'DD/MM/YYYY') , 'DD/MM/YYYY')=
									                            to_date(TO_CHAR(now(), 'DD/MM/YYYY') , 'DD/MM/YYYY') and r.id = rs.report_id
	                             left join master.scheduledreport sr on sr.schedule_report_id = rs.id and rs.start_date = sr.start_date and  sr.end_date = rs.end_date
                            where to_date(TO_CHAR(TO_TIMESTAMP(rs.next_schedule_run_date / 1000), 'DD/MM/YYYY') , 'DD/MM/YYYY')=
									                            to_date(TO_CHAR(now(), 'DD/MM/YYYY') , 'DD/MM/YYYY') and
	                              sr.id is null";
                #endregion
                return _dataAccess.QueryAsync<ReportCreationScheduler>(query);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<IEnumerable<VehicleList>> GetVehicleList(IEnumerable<int> groupId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@group_id", groupId.ToArray());
                var query = @"with cte_VehicaleList
                            AS (
                            select distinct ref_id as VehicleId from master.group where id = Any(@group_id) and group_type = 'S' and object_type = 'V'
                            union
                            select distinct ref_id as VehicleId 
                            from master.groupref
                            where group_id in (select distinct id from master.group where id = Any(@group_id) and group_type = 'G' and object_type = 'V')
                            )
                            select distinct vin as VIN 
                            from cte_VehicaleList vl
                                 inner join master.vehicle v on v.id = vl.VehicleId";
                return _dataMartdataAccess.QueryAsync<VehicleList>(query, parameter);
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
                var query = @"select icon 
                              from master.icon 
                              where id in (select icon_id from master.accountpreference where id = @account_id";
                return _dataMartdataAccess.ExecuteScalarAsync<ReportLogo>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
