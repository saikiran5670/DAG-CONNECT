﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.email.Entity;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.reportscheduler.repository
{
    public partial class ReportSchedulerRepository : IReportSchedulerRepository
    {
        public async Task<IEnumerable<ReportSchedulerEmailResult>> GetReportEmailDetails()
        {
            MapperRepo repositoryMapper = new MapperRepo();
            try
            {
                string queryAlert = @"SELECT repsch.id as ReportSchedulerId,  
                                            repsch.organization_id as OrganizationId, 
                                            repsch.code as LanguageCode,
                                            repsch.frequency_type as FrequencyType,                                           
                                            repsch.last_schedule_run_date as LastScheduleRunDate, 
                                            repsch.next_schedule_run_date as NextScheduleRunDate,                                            
                                            repsch.mail_subject as MailSubject, 
                                            repsch.mail_description as MailDescription,  
                                            repsch.start_date as StartDate, 
                                            repsch.end_date as EndDate,                                            
                                            repsch.created_by as ReportCreatedBy, 
                                            receipt.email as EmailId, 
                                            schrep.token as ReportToken,
                                            coalesce((select t.value from translation.translation as t where t.name =report.key and t.code=repsch.code), (select t.value from translation.translation as t where t.name =report.key and t.code='EN-GB')) as Key                                           
	                                        FROM master.reportscheduler as repsch	                                 
	                                        Inner JOIN master.scheduledreportrecipient as receipt
	                                        ON repsch.id=receipt.schedule_report_id AND repsch.status='A' AND receipt.state='A'	                                  
	                                        Inner JOIN master.scheduledreport as schrep
	                                        ON repsch.id=schrep.schedule_report_id AND repsch.start_date=schrep.start_date AND repsch.end_date=schrep.end_date AND repsch.status='A'
                                            Inner JOIN master.report as report
										    ON report.id=repsch.report_id";
                queryAlert += " where schrep.is_mail_send=false AND date_trunc('hour', (to_timestamp(repsch.next_schedule_run_date/1000) AT TIME ZONE 'UTC')) <= date_trunc('hour', NOW() AT TIME ZONE 'UTC')";

                IEnumerable<ReportSchedulerEmailResult> reportSchedulerResult = await _dataAccess.QueryAsync<ReportSchedulerEmailResult>(queryAlert);
                return reportSchedulerResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<IEnumerable<ReportEmailFrequency>> GetMissingSchedulerData()
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@now_date", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                //Note: next_schedule_run_date < @now_date and as the email job runs at hourly basis, so there will be always gap of i hour.
                #region Query GetReportCreationScheduler
                var query = @"select rs.id as SchedulerId, rs.organization_id as OrganizationId, rs.report_id as ReportId, 
                            rs.frequency_type as FrequencyTypeValue,
							rs.status as Status, 
                            rs.type as Type, 
							rs.start_date as StartDate,
							rs.end_date as EndDate,
                             rs.last_schedule_run_date as ReportPrevioudScheduleRunDate, 
                            rs.next_schedule_run_date as ReportNextScheduleRunDate,  
                            rs.created_by as CreatedBy
                            from master.reportscheduler rs              
								 left join master.scheduledreport sr 
                                    on sr.schedule_report_id = rs.id AND rs.next_schedule_run_date < @now_date 
                                        and rs.start_date = sr.start_date and  sr.end_date = rs.end_date
                                       and rs.status = 'A'  
                            where rs.next_schedule_run_date < @now_date and rs.status = 'A' and sr.id is null
							order by rs.next_schedule_run_date";
                #endregion
                return _dataAccess.QueryAsync<ReportEmailFrequency>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> UpdateTimeRangeByDate(ReportEmailFrequency reportEmailFrequency)
        {
            try
            {
                _helper.GetNextFrequencyTime(reportEmailFrequency);
                var query = @"UPDATE master.reportscheduler 
                          SET start_date=@start_date ,
                              next_schedule_run_date=@next_schedule_run_date ,
                              last_schedule_run_date=@last_schedule_run_date ,
                              end_date=@end_date 
                          WHERE id=@id RETURNING id";
                var parameter = new DynamicParameters();
                parameter.Add("@id", reportEmailFrequency.ReportId);
                parameter.Add("@start_date", reportEmailFrequency.StartDate);
                parameter.Add("@end_date", reportEmailFrequency.EndDate);
                parameter.Add("@next_schedule_run_date", reportEmailFrequency.ReportNextScheduleRunDate);
                parameter.Add("@last_schedule_run_date", reportEmailFrequency.ReportPrevioudScheduleRunDate);

                int rowEffected = await _dataAccess.ExecuteAsync(query, parameter);
                return rowEffected;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> UpdateTimeRangeByCalenderTime(ReportEmailFrequency reportEmailFrequency)
        {
            try
            {
                var next_schedule_run_date = reportEmailFrequency.FrequencyType == ENUM.TimeFrequenyType.Monthly ?
                                        _helper.GetNextMonthlyTime(reportEmailFrequency.ReportScheduleRunDate) :
                                        _helper.GetNextQuarterTime(reportEmailFrequency.ReportScheduleRunDate);

                var query = @"UPDATE master.reportscheduler 
                          SET start_date=@start_date ,
                              next_schedule_run_date=@next_schedule_run_date ,
                              last_schedule_run_date=@last_schedule_run_date , 
                              end_date=@end_date 
                              WHERE id=@id RETURNING id";
                var parameter = new DynamicParameters();
                parameter.Add("@id", reportEmailFrequency.ReportId);
                parameter.Add("@start_date", next_schedule_run_date.StartDate);
                parameter.Add("@end_date", next_schedule_run_date.EndDate);
                parameter.Add("@next_schedule_run_date", next_schedule_run_date.ReportNextScheduleRunDate);
                parameter.Add("@last_schedule_run_date", reportEmailFrequency.ReportPrevioudScheduleRunDate);

                int rowEffected = await _dataAccess.ExecuteAsync(query, parameter);
                return rowEffected;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<int> UpdateIsMailSend(Guid token, bool isMailSend)
        {
            try
            {
                var query = @"UPDATE master.scheduledreport 
                            SET is_mail_send=@isMailSend
                            WHERE token=@token RETURNING id";
                var parameter = new DynamicParameters();
                parameter.Add("@token", token);
                parameter.Add("@isMailSend", isMailSend);
                int rowEffected = await _dataAccess.ExecuteAsync(query, parameter);
                return rowEffected;
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}
