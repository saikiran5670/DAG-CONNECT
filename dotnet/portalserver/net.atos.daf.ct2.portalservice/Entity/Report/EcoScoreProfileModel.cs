using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class EcoScoreProfileModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public List<EcoScoreProfileSection> ProfileSections { get; set; }
    }

    public class EcoScoreProfileSection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<EcoScoreKPI> KPIs { get; set; }
    }

    public class EcoScoreKPI
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public char LimitType { get; set; }
        public char RangeValueType { get; set; }
        public int LimitValue { get; set; }
        public int TargetValue { get; set; }
        public int LowerValue { get; set; }
        public int UpperValue { get; set; }
        public int MaxUpperValue { get; set; }
        public int SequenceNo { get; set; }
    }

    public class EcoScoreProfileCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDAFStandard { get; set; }
        public List<EcoScoreProfileKPI> ProfileKPIs { get; set; }
    }

    public class EcoScoreProfileKPI
    {
        public int KPIId { get; set; }
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
