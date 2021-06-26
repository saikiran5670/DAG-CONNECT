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
        public Task<int> SendReportEmail() => throw new NotImplementedException();

        public async Task<IEnumerable<ReportSchedulerMap>> GetReportEmailDetails()
        {
            MapperRepo repositoryMapper = new MapperRepo();
            try
            {
                string queryAlert = @"SELECT repsch.id as repsch_id,  
                                            repsch.organization_id as repsch_organization_id, 
                                            repsch.report_id as repsch_report_id, 
                                            repsch.frequency_type as repsch_frequency_type,                                           
                                            repsch.last_schedule_run_date as repsch_last_schedule_run_date, 
                                            repsch.next_schedule_run_date as repsch_next_schedule_run_date,                                            
                                            repsch.mail_subject as repsch_mail_subject, 
                                            repsch.mail_description as repsch_mail_description,  
                                            repsch.start_date as repsch_start_date, 
                                            repsch.end_date as repsch_end_date
                                            receipt.id as receipt_id, 
                                            repsch.created_by as repsch_created_by, 
                                            receipt.email as receipt_email, 
                                            schrep.token as schrep_token,
                                            schrep.valid_till as schrep_valid_till, 
                                            schrep.created_at as schrep_created_at, 
                                            schrep.start_date as schrep_start_date, 
                                            schrep.end_date as schrep_end_date
	                                        FROM master.reportscheduler as repsch	                                 
	                                        Inner JOIN master.scheduledreportrecipient as receipt
	                                        ON repsch.id=receipt.schedule_report_id AND repsch.status='A' AND receipt.state='A'	                                  
	                                        inner JOIN master.scheduledreport as schrep
	                                       ON repsch.id=schrep.schedule_report_id AND repsch.start_date=schrep.start_date AND repsch.end_date=schrep.end_date AND repsch.status='A' ";
                queryAlert += " where date_trunc('hour', (to_timestamp(repsch.next_schedule_run_date/1000) AT TIME ZONE 'UTC')) = date_trunc('hour', NOW() AT TIME ZONE 'UTC')";

                IEnumerable<ReportSchedulerResult> reportSchedulerResult = await _dataAccess.QueryAsync<ReportSchedulerResult>(queryAlert);
                return repositoryMapper.GetReportSchedulerList(reportSchedulerResult);
            }
            catch (Exception)
            {
                throw;
            }
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

        public async Task<string> GetLanguageCodePreference(string emailId, int? orgId)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@emailId", emailId.ToLower());

                string accountQuery =
                    @"SELECT preference_id from master.account where lower(email) = @emailId";

                var accountPreferenceId = await _dataAccess.QueryFirstAsync<int?>(accountQuery, parameter);

                if (!accountPreferenceId.HasValue)
                {
                    string orgQuery = string.Empty;
                    int? orgPreferenceId = null;
                    if (orgId.HasValue && orgId > 0)
                    {
                        var orgParameter = new DynamicParameters();
                        orgParameter.Add("@orgId", orgId);

                        orgQuery = @"SELECT preference_id from master.organization WHERE id=@orgId";

                        orgPreferenceId = await _dataAccess.QueryFirstAsync<int?>(orgQuery, orgParameter);
                    }
                    else
                    {
                        orgQuery =
                            @"SELECT o.preference_id from master.account acc
                            INNER JOIN master.accountOrg ao ON acc.id=ao.account_id
                            INNER JOIN master.organization o ON ao.organization_id=o.id
                            where lower(acc.email) = @emailId";

                        orgPreferenceId = await _dataAccess.QueryFirstAsync<int?>(orgQuery, parameter);
                    }

                    if (!orgPreferenceId.HasValue)
                        return "EN-GB";
                    else
                        return await GetCodeByPreferenceId(orgPreferenceId.Value);
                }
                return await GetCodeByPreferenceId(accountPreferenceId.Value);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> GetCodeByPreferenceId(int preferenceId)
        {
            try
            {
                var parameter = new DynamicParameters();

                parameter.Add("@preferenceId", preferenceId);

                string query =
                    @"SELECT l.code from master.accountpreference ap
                    INNER JOIN translation.language l ON ap.id = @preferenceId AND ap.language_id=l.id";

                var languageCode = await _dataAccess.QueryFirstAsync<string>(query, parameter);

                return languageCode;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
