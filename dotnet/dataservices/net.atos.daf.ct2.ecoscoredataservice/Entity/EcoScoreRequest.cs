using System;
using System.ComponentModel.DataAnnotations;
using net.atos.daf.ct2.ecoscoredataservice.CustomAttributes;

namespace net.atos.daf.ct2.ecoscoredataservice.Entity
{
    public class EcoScoreRequest
    {
        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_PARAMETER")]
        [EmailRegex(ErrorMessage = "INVALID_PARAMETER")]
        public string AccountEmail { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_PARAMETER")]
        [StringLength(19, MinimumLength = 1, ErrorMessage = "INVALID_PARAMETER")]
        public string DriverId { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_PARAMETER")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "INVALID_PARAMETER")]
        public string OrganizationId { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_PARAMETER")]
        [StringLength(17, MinimumLength = 1, ErrorMessage = "INVALID_PARAMETER")]
        public string VIN { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_PARAMETER")]
        [StringLength(5, MinimumLength = 3, ErrorMessage = "INVALID_PARAMETER")]
        [AggregateType(ErrorMessage = "INVALID_PARAMETER")]
        public string AggregationType { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "INVALID_PARAMETER")]
        [Required(ErrorMessage = "MISSING_PARAMETER")]
        public long? StartTimestamp { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "INVALID_PARAMETER")]
        [Required(ErrorMessage = "MISSING_PARAMETER")]
        [GreaterThan(nameof(StartTimestamp), ErrorMessage = "INVALID_PARAMETER")]
        public long? EndTimestamp { get; set; }
    }
}
