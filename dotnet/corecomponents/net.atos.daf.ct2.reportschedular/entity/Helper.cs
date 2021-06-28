using System;
using net.atos.daf.ct2.reportscheduler.ENUM;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class Helper
    {
        public void GetNextFrequencyTime(ReportEmailFrequency reportEmailFrequency)
        {
            DateTime deafaultDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = deafaultDateTime.AddMilliseconds(reportEmailFrequency.ReportScheduleRunDate).ToLocalTime();


            switch (reportEmailFrequency.FrequencyType)
            {

                case TimeFrequenyType.Daily:
                    reportEmailFrequency.ReportNextScheduleRunDate = UTCHandling.GetUTCFromDateTime(date.AddDays(1));
                    break;
                case TimeFrequenyType.Weekly:
                    reportEmailFrequency.ReportNextScheduleRunDate = UTCHandling.GetUTCFromDateTime(date.AddDays(7));
                    break;
                case TimeFrequenyType.BiWeekly:
                    reportEmailFrequency.ReportNextScheduleRunDate = UTCHandling.GetUTCFromDateTime(date.AddDays(14));
                    break;
                case TimeFrequenyType.Monthly:
                    reportEmailFrequency = GetNextMonthlyTime(reportEmailFrequency.ReportScheduleRunDate);
                    break;
                case TimeFrequenyType.Quartly:
                    reportEmailFrequency = GetNextQuarterTime(reportEmailFrequency.ReportScheduleRunDate);
                    break;
            }
            reportEmailFrequency.ReportPrevioudScheduleRunDate = UTCHandling.GetUTCFromDateTime(date);
            reportEmailFrequency.StartDate = UTCHandling.GetUTCFromDateTime(deafaultDateTime.AddMilliseconds(reportEmailFrequency.StartDate).ToLocalTime().AddDays(1));
            reportEmailFrequency.EndDate = UTCHandling.GetUTCFromDateTime(deafaultDateTime.AddMilliseconds(reportEmailFrequency.EndDate).ToLocalTime().AddDays(1));
        }

        public ReportEmailFrequency GetNextQuarterTime(long currentdate)
        {

            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(currentdate).ToLocalTime();
            var quarterNumber = ((date.Month - 1) / 3) + 1;
            var firstDayOfQuarter = new DateTime(date.Year, ((quarterNumber - 1) * 3) + 1, 1);
            var lastDayOfQuarter = firstDayOfQuarter.AddMonths(3).AddDays(-1);
            var nextQuarter = quarterNumber + 1;
            var firstDayOfnextQuarter = new DateTime(date.Year, ((nextQuarter - 1) * 3) + 1, 1);
            var lastDayOfnextQuarter = firstDayOfnextQuarter.AddMonths(3).AddDays(-1);
            var reportEmailFrequency = new ReportEmailFrequency()
            {
                StartDate = UTCHandling.GetUTCFromDateTime(firstDayOfnextQuarter),
                EndDate = UTCHandling.GetUTCFromDateTime(lastDayOfnextQuarter),
                FrequencyType = TimeFrequenyType.Quartly
            };
            return reportEmailFrequency;
        }


        public ReportEmailFrequency GetNextMonthlyTime(long currentdate)
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(currentdate).ToLocalTime();
            var nextMonth = date.AddMonths(1);
            var startDate = new DateTime(nextMonth.Year, nextMonth.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var reportEmailFrequency = new ReportEmailFrequency()
            {
                StartDate = UTCHandling.GetUTCFromDateTime(startDate),
                EndDate = UTCHandling.GetUTCFromDateTime(endDate),
                FrequencyType = TimeFrequenyType.Monthly
            };
            return reportEmailFrequency;
        }

    }
}
