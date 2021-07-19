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
        [RegularExpression(@"^[\s\w\p{L}\-\.]{1,100}$", ErrorMessage = "Profile name Allowed: a - z, A - Z, 0 - 9, hyphens dash, spaces, periods, international alphabets [e.g.à, è, ì, ò, ù, À, È, Ì, Ò, Ù] and Not allowed: special chars [i.e. !, @, #, $, %, &, *] with maximun length 50.")]
        public string Name { get; set; }

        [StringLength(120, MinimumLength = 0, ErrorMessage = "The field {0} must be a string with a length of {1} characters.")]
        [RegularExpression(@"^[\s\w\p{L}\-\.]{1,100}$", ErrorMessage = "Profile Description Allowed: a-z, A-Z, 0-9, hyphens dash, spaces, periods, international ref. alphabets [e.g. à, è, ì, ò, ù, À, È, Ì, Ò, Ù] and Not allowed: special chars [i.e. !, @, #, $, %, &, *] with maximun length 120.")]
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
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "The field {0} must be a string with a length of {1} characters.")]
        [RegularExpression(@"^[\s\w\p{L}\-\.]{1,100}$", ErrorMessage = "Profile name Allowed: a - z, A - Z, 0 - 9, hyphens dash, spaces, periods, international alphabets [e.g.à, è, ì, ò, ù, À, È, Ì, Ò, Ù] and Not allowed: special chars [i.e. !, @, #, $, %, &, *] with maximun length 50.")]
        public string Name { get; set; }
        [StringLength(120, MinimumLength = 0, ErrorMessage = "The field {0} must be a string with a length of {1} characters.")]
        [RegularExpression(@"^[\s\w\p{L}\-\.]{1,100}$", ErrorMessage = "Profile Description Allowed: a-z, A-Z, 0-9, hyphens dash, spaces, periods, international ref. alphabets [e.g. à, è, ì, ò, ù, À, È, Ì, Ò, Ù] and Not allowed: special chars [i.e. !, @, #, $, %, &, *] with maximun length 120.")]
        public string Description { get; set; }
        public List<EcoScoreProfileKPI> ProfileKPIs { get; set; }
    }

    public class EcoScoreProfileDeleteRequest
    {
        public int ProfileId { get; set; }
    }

    public class EcoScoreReportByAllDriversRequest
    {
        [Required]
        public long StartDateTime { get; set; }
        [Required]
        public long EndDateTime { get; set; }
        [Required]
        public List<string> VINs { get; set; }
        [Range(0, 100, ErrorMessage = "Minimum Trip Distance should be non-negative and less than 100.")]
        public double MinTripDistance { get; set; }
        [Range(0, 100, ErrorMessage = "Minimum Driver Total Distance should be non-negative and less than 100.")]
        public double MinDriverTotalDistance { get; set; }
        public int TargetProfileId { get; set; }
        public int ReportId { get; set; }
    }

    public class EcoScoreReportCompareDriversRequest
    {
        [Required]
        public long StartDateTime { get; set; }
        [Required]
        public long EndDateTime { get; set; }
        [Required]
        public List<string> VINs { get; set; }
        [Required]
        public List<string> DriverIds { get; set; }
        [Range(0, 100, ErrorMessage = "Minimum Trip Distance should be non-negative and less than 100.")]
        public double MinTripDistance { get; set; }
        [Range(0, 100, ErrorMessage = "Minimum Driver Total Distance should be non-negative and less than 100.")]
        public double MinDriverTotalDistance { get; set; }
        [Required]
        public int TargetProfileId { get; set; }
        [Required]
        public int ReportId { get; set; }
    }

    public class EcoScoreReportSingleDriverRequest
    {
        [Required]
        public long StartDateTime { get; set; }
        [Required]
        public long EndDateTime { get; set; }
        [Required]
        public List<string> VINs { get; set; }
        [Required]
        public string DriverId { get; set; }
        [Range(0, 100, ErrorMessage = "Minimum Trip Distance should be non-negative and less than 100.")]
        public double MinTripDistance { get; set; }
        [Range(0, 100, ErrorMessage = "Minimum Driver Total Distance should be non-negative and less than 100.")]
        public double MinDriverTotalDistance { get; set; }
        [Required]
        public int TargetProfileId { get; set; }
        [Required]
        public int ReportId { get; set; }
        public int OrgId { get; set; }
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


