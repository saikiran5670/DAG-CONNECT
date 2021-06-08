using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class EcoScoreProfileDto
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public char DefaultESVersionType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ActionedBy { get; set; }
        public List<EcoScoreProfileKPI> ProfileKPIs { get; set; }
    }

    public class EcoScoreProfileKPI
    {
        public int KPIId { get; set; }
        public double LimitValue { get; set; }
        public double TargetValue { get; set; }
        public double LowerValue { get; set; }
        public double UpperValue { get; set; }
    }
}
