using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using net.atos.daf.ct2.portalservice.CustomValidators.Report;

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
        public double LimitValue { get; set; }
        public double TargetValue { get; set; }
        public double LowerValue { get; set; }
        public double UpperValue { get; set; }
        public double MaxUpperValue { get; set; }
        public int SequenceNo { get; set; }
    }

    public class EcoScoreProfileCreateRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "The field {0} must be a string with a length of {1} characters.")]
        public string Name { get; set; }

        [StringLength(120, MinimumLength = 0, ErrorMessage = "The field {0} must be a string with a length of {1} characters.")]
        public string Description { get; set; }

        public bool IsDAFStandard { get; set; }

        public List<EcoScoreProfileKPI> ProfileKPIs { get; set; }
    }

    public class EcoScoreProfileKPI
    {
        [Required]
        public int KPIId { get; set; }
        [Required]
        public char LimitType { get; set; }
        [Required]
        [CompareProfileLimitValue("TargetValue", "LowerValue", "UpperValue", "LimitType", ErrorMessage = "Values are not within threshold limits.")]
        public double LimitValue { get; set; }
        [Required]
        [CompareProfileTargetValue("LimitValue", "LowerValue", "UpperValue", "LimitType", ErrorMessage = "Values are not within threshold limits.")]
        public double TargetValue { get; set; }
        [Required]
        [CompareProfileLowerUpperValue("UpperValue", ErrorMessage = "Lower/Upper values are invalid.")]
        public double LowerValue { get; set; }
        [Required]
        [CompareProfileLowerUpperValue("LowerValue", ErrorMessage = "Lower/Upper values are invalid.")]
        public double UpperValue { get; set; }
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
  
}



    public enum LimitType
    {
        Min = 'N',
        Max = 'X',
        None = 'O'
    }

    public enum RangeValueType
    {
        Decimal = 'D',
        Time = 'T'
    }

  
