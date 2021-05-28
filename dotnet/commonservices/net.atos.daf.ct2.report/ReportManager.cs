﻿using net.atos.daf.ct2.report.repository;
using System;

namespace net.atos.daf.ct2.report
{
    public class ReportManager : IReportManager
    {
        private readonly IReportRepository _reportRepository;

        public ReportManager(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }
    }
}
