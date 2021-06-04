using System.Collections.Generic;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class EcoScoreProfileCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDAFStandard { get; set; }
        public List<EcoScoreProfileKPI> ProfileKPIs { get; set; }
    }

    public class EcoScoreProfileKPI
    {
        public int KPI_Id { get; set; }
        public int LimitValue { get; set; }
        public int TargetValue { get; set; }
        public int LowerValue { get; set; }
        public int UpperValue { get; set; }
    }

    public class EcoScoreProfileUpdateRequest
    {
        public int ProfileId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<EcoScoreProfileKPI> ProfileKPIs { get; set; }
    }

    public class EcoScoreProfileDeleteRequest
    {
        public int ProfileId { get; set; }
    }

    public class EcoScoreGetProfilesResponse
    {
        public int ProfileId { get; set; }
        public string Name { get; set; }
        public bool IsDeleteAllowed { get; set; }
    }
}
