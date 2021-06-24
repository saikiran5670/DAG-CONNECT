using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.reportscheduler.repository
{
    public partial class ReportSchedulerRepository : IReportSchedulerRepository
    {
        public Task<int> SendReportEmail() => throw new NotImplementedException();

        public async void GetReportEmailDetails()
        {
            string queryAlert = @"SELECT repsch.id as repsch_id, 
                                            repsch.organization_id as repsch_organization_id, 
                                            repsch.report_id as repsch_report_id, 
                                            repsch.frequency_type as repsch_frequency_type,                                           
                                            repsch.last_schedule_run_date as repsch_last_schedule_run_date, 
                                            repsch.next_schedule_run_date as repsch_next_schedule_run_date,                                            
                                            repsch.mail_subject as repsch_mail_subject, 
                                            repsch.mail_description as repsch_mail_description,                                         
                                            receipt.id as receipt_id, 
                                            repsch.created_by as repsch_created_by,                                           
                                            repsch.modified_by as repsch_modified_by, 
                                            receipt.schedule_report_id as receipt_schedule_report_id, 
                                            receipt.email as receipt_email,  
                                            schrep.id as schrep_id, 
                                            schrep.token as schrep_token,
                                            schrep.schedule_report_id as schrep_schedule_report_id, 
                                            schrep.report as schrep_report,
                                            schrep.downloaded_at as schrep_downloaded_at, 
                                            schrep.valid_till as schrep_valid_till, 
                                            schrep.created_at as schrep_created_at, 
                                            schrep.start_date as schrep_start_date, 
                                            schrep.end_date as schrep_end_date
	                                        FROM master.reportscheduler as repsch	                                 
	                                       LEFT JOIN master.scheduledreportrecipient as receipt
	                                       ON repsch.id=receipt.schedule_report_id AND repsch.status='A' AND receipt.state='A'	                                  
	                                       LEFT JOIN master.scheduledreport as schrep
	                                       ON repsch.id=schrep.schedule_report_id AND repsch.status='A' ";
            queryAlert += " where date_trunc('hour', (to_timestamp(repsch.next_schedule_run_date) AT TIME ZONE 'UTC')) = date_trunc('hour', NOW() AT TIME ZONE 'UTC') GROUP BY repsch.created_by";

            IEnumerable<ReportSchedulerResult> reportSchedulerResult = await _dataAccess.QueryAsync<ReportSchedulerResult>(queryAlert);

        }

        public async Task<int> UpdateTimeRangeByDate(ReportEmailFrequency reportEmailFrequency)
        {
            var next_schedule_run_date = _helper.GetNextFrequencyTime(reportEmailFrequency.ReportScheduleRunDate, reportEmailFrequency.FrequencyType);
            var query = @"UPDATE master.reportscheduler 
                          SET start_date=@start_date ,
                              next_schedule_run_date=@next_schedule_run_date ,
                              previous_schedule_run_date=@previous_schedule_run_date 
                              end_date=@end_date 
                          WHERE id=@id RETURNING id";
            var parameter = new DynamicParameters();
            parameter.Add("@id", reportEmailFrequency.ReportId);
            parameter.Add("@start_date", next_schedule_run_date.StartDate);
            parameter.Add("@end_date", next_schedule_run_date.EndDate);
            parameter.Add("@next_schedule_run_date", next_schedule_run_date.ReportNextScheduleRunDate);
            parameter.Add("@previous_schedule_run_date", next_schedule_run_date.ReportPrevioudScheduleRunDate);

            int rowEffected = await _dataAccess.ExecuteAsync(query, parameter);
            return rowEffected;
        }
        public async Task<int> UpdateTimeRangeByCalenderTime(ReportEmailFrequency reportEmailFrequency)
        {

            var next_schedule_run_date = reportEmailFrequency.FrequencyType == ENUM.TimeFrequenyType.Monthly ?
                                        _helper.GetNextMonthlyTime(reportEmailFrequency.ReportScheduleRunDate) :
                                        _helper.GetNextQuarterTime(reportEmailFrequency.ReportScheduleRunDate);

            var query = @"UPDATE master.reportscheduler 
                          SET start_date=@start_date ,
                              next_schedule_run_date=@next_schedule_run_date ,
                              previous_schedule_run_date=@previous_schedule_run_date 
                              end_date=@end_date 
                              WHERE id=@id RETURNING id";
            var parameter = new DynamicParameters();
            parameter.Add("@id", reportEmailFrequency.ReportId);
            parameter.Add("@start_date", next_schedule_run_date.StartDate);
            parameter.Add("@end_date", next_schedule_run_date.EndDate);
            parameter.Add("@next_schedule_run_date", next_schedule_run_date.ReportNextScheduleRunDate);
            parameter.Add("@previous_schedule_run_date", next_schedule_run_date.ReportPrevioudScheduleRunDate);

            int rowEffected = await _dataAccess.ExecuteAsync(query, parameter);
            return rowEffected;
        }
    }
}
