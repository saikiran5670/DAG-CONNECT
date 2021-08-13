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
            reportEmailFrequency.ReportPrevioudScheduleRunDate = UTCHandling.GetUTCFromDateTime(date);
            var nowDate = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            do
            {
                switch (reportEmailFrequency.FrequencyType)
                {

                    case TimeFrequenyType.Daily:
                        reportEmailFrequency.ReportNextScheduleRunDate = UTCHandling.GetUTCFromDateTime(date.AddDays(1));
                        reportEmailFrequency.StartDate = UTCHandling.GetUTCFromDateTime(deafaultDateTime.AddMilliseconds(reportEmailFrequency.StartDate).ToLocalTime().AddDays(1));
                        reportEmailFrequency.EndDate = UTCHandling.GetUTCFromDateTime(deafaultDateTime.AddMilliseconds(reportEmailFrequency.EndDate).ToLocalTime().AddDays(1));

                        break;
                    case TimeFrequenyType.Weekly:
                        reportEmailFrequency.ReportNextScheduleRunDate = UTCHandling.GetUTCFromDateTime(date.AddDays(7));
                        reportEmailFrequency.StartDate = UTCHandling.GetUTCFromDateTime(deafaultDateTime.AddMilliseconds(reportEmailFrequency.StartDate).ToLocalTime().AddDays(7));
                        reportEmailFrequency.EndDate = UTCHandling.GetUTCFromDateTime(deafaultDateTime.AddMilliseconds(reportEmailFrequency.EndDate).ToLocalTime().AddDays(7));

                        break;
                    case TimeFrequenyType.BiWeekly:
                        reportEmailFrequency.ReportNextScheduleRunDate = UTCHandling.GetUTCFromDateTime(date.AddDays(14));
                        reportEmailFrequency.StartDate = UTCHandling.GetUTCFromDateTime(deafaultDateTime.AddMilliseconds(reportEmailFrequency.StartDate).ToLocalTime().AddDays(14));
                        reportEmailFrequency.EndDate = UTCHandling.GetUTCFromDateTime(deafaultDateTime.AddMilliseconds(reportEmailFrequency.EndDate).ToLocalTime().AddDays(14));

                        break;
                }
                date = deafaultDateTime.AddMilliseconds(reportEmailFrequency.ReportNextScheduleRunDate).ToLocalTime();
            } while (reportEmailFrequency.EndDate < nowDate);
        }

        public ReportEmailFrequency GetNextQuarterTime(long currentdate)
        {

            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(currentdate).ToLocalTime();
            var nowDate = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            DateTime firstDayOfQuarter, lastDayOfQuarter, nextRunDate;
            nextRunDate = date;
            do
            {
                var quarterNumber = ((date.Month - 1) / 3) + 1;
                firstDayOfQuarter = new DateTime(date.Year, ((quarterNumber - 1) * 3) + 1, 1);
                lastDayOfQuarter = firstDayOfQuarter.AddMonths(3).AddDays(-1);
                //var nextQuarter = quarterNumber + 1;
                // firstDayOfnextQuarter = new DateTime(date.Year, ((nextQuarter - 1) * 3) + 1, 1);
                // lastDayOfnextQuarter = firstDayOfnextQuarter.AddMonths(3).AddDays(-1);
                date = start.AddMilliseconds(UTCHandling.GetUTCFromDateTime(lastDayOfQuarter.AddDays(1))).ToLocalTime();
            } while (UTCHandling.GetUTCFromDateTime(lastDayOfQuarter) < nowDate);
            var tempNextDate = lastDayOfQuarter.AddDays(1);
            nextRunDate = new DateTime(tempNextDate.Year, tempNextDate.Month, 1, nextRunDate.Hour, nextRunDate.Minute, nextRunDate.Second);

            var reportEmailFrequency = new ReportEmailFrequency()
            {
                StartDate = UTCHandling.GetUTCFromDateTime(firstDayOfQuarter),
                EndDate = UTCHandling.GetUTCFromDateTime(lastDayOfQuarter.AddHours(23).AddMinutes(59).AddSeconds(59)),
                FrequencyType = TimeFrequenyType.Quartly,
                ReportNextScheduleRunDate = UTCHandling.GetUTCFromDateTime(nextRunDate)
            };
            return reportEmailFrequency;
        }


        public ReportEmailFrequency GetNextMonthlyTime(long currentdate)
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(currentdate).ToLocalTime();
            var nowDate = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            DateTime nextMonth, startDate, endDate;
            do
            {
                nextMonth = date.AddMonths(1);
                startDate = new DateTime(date.Year, date.Month, 1);
                endDate = startDate.AddMonths(1).AddSeconds(-1);
                date = start.AddMilliseconds(UTCHandling.GetUTCFromDateTime(nextMonth)).ToLocalTime();
            } while (UTCHandling.GetUTCFromDateTime(endDate) < nowDate);

            var reportEmailFrequency = new ReportEmailFrequency()
            {
                StartDate = UTCHandling.GetUTCFromDateTime(startDate),
                EndDate = UTCHandling.GetUTCFromDateTime(endDate),
                FrequencyType = TimeFrequenyType.Monthly,
                ReportNextScheduleRunDate = UTCHandling.GetUTCFromDateTime(nextMonth)
            };
            return reportEmailFrequency;
        }

    }
}
