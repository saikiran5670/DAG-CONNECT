using System;
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
                queryAlert += " where date_trunc('hour', (to_timestamp(repsch.next_schedule_run_date/1000) AT TIME ZONE 'UTC')) <= date_trunc('hour', NOW() AT TIME ZONE 'UTC')";

                IEnumerable<ReportSchedulerEmailResult> reportSchedulerResult = await _dataAccess.QueryAsync<ReportSchedulerEmailResult>(queryAlert);
                return reportSchedulerResult;
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
                parameter.Add("@last_schedule_run_date", next_schedule_run_date.ReportPrevioudScheduleRunDate);

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
