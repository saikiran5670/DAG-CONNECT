﻿using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class EcoScoreProfileDto
    {
        public int Id { get; set; }
        public int? OrganizationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ActionedBy { get; set; }
        public DateTime LastUpdate { get; set; }
        public List<EcoScoreProfileKPI> ProfileKPIs { get; set; }
        public bool IsDeleteAllowed { get; set; }
    }

    public class EcoScoreProfileKPI
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public string SectionDescription { get; set; }
        public int KPIId { get; set; }
        public string KPIName { get; set; }
        public string LimitType { get; set; }
        public double LimitValue { get; set; }
        public double TargetValue { get; set; }
        public double LowerValue { get; set; }
        public double UpperValue { get; set; }
        public string RangeValueType { get; set; }
        public double MaxUpperValue { get; set; }
        public int SequenceNo { get; set; }
    }
}
