using System;
using System.ComponentModel.DataAnnotations;
using net.atos.daf.ct2.provisioningdataservice.CustomAttributes;

namespace net.atos.daf.ct2.provisioningdataservice.Entity
{
    public class VehicleCurrentRequest
    {
        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_PARAMETER")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "INVALID_PARAMETER")]
        public string OrgId { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_PARAMETER")]
        [EmailRegex(ErrorMessage = "INVALID_PARAMETER")]
        public string Account { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_PARAMETER")]
        [StringLength(19, MinimumLength = 1, ErrorMessage = "INVALID_PARAMETER")]
        public string DriverId { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "INVALID_PARAMETER")]
        public long? StartTimestamp { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "INVALID_PARAMETER")]
        [GreaterThan(nameof(StartTimestamp), ErrorMessage = "INVALID_PARAMETER")]
        public long? EndTimestamp { get; set; }
    }

    public class VehicleListRequest
    {
        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_PARAMETER")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "INVALID_PARAMETER")]
        public string OrgId { get; set; }

        [EmailRegex(ErrorMessage = "INVALID_PARAMETER")]
        public string Account { get; set; }

        [StringLength(19, MinimumLength = 0, ErrorMessage = "INVALID_PARAMETER")]
        public string DriverId { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "INVALID_PARAMETER")]
        public long? StartTimestamp { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "INVALID_PARAMETER")]
        [GreaterThan(nameof(StartTimestamp), ErrorMessage = "INVALID_PARAMETER")]
        public long? EndTimestamp { get; set; }
    }

    public class DriverCurrentRequest
    {
        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_PARAMETER")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "INVALID_PARAMETER")]
        public string OrgId { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_PARAMETER")]
        [StringLength(17, MinimumLength = 1, ErrorMessage = "INVALID_PARAMETER")]
        public string VIN { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "INVALID_PARAMETER")]
        public long? StartTimestamp { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "INVALID_PARAMETER")]
        [GreaterThan(nameof(StartTimestamp), ErrorMessage = "INVALID_PARAMETER")]
        public long? EndTimestamp { get; set; }
    }

    public class DriverListRequest
    {
        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_PARAMETER")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "INVALID_PARAMETER")]
        public string OrgId { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "INVALID_PARAMETER")]
        public long? StartTimestamp { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "INVALID_PARAMETER")]
        [GreaterThan(nameof(StartTimestamp), ErrorMessage = "INVALID_PARAMETER")]
        public long? EndTimestamp { get; set; }
    }

    public class OrganisationRequest
    {
        [EmailRegex(ErrorMessage = "INVALID_PARAMETER")]
        public string Account { get; set; }

        [StringLength(19, MinimumLength = 0, ErrorMessage = "INVALID_PARAMETER")]
        public string DriverId { get; set; }

        [StringLength(17, MinimumLength = 0, ErrorMessage = "INVALID_PARAMETER")]
        public string VIN { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "INVALID_PARAMETER")]
        public long? StartTimestamp { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "INVALID_PARAMETER")]
        [GreaterThan(nameof(StartTimestamp), ErrorMessage = "INVALID_PARAMETER")]
        public long? EndTimestamp { get; set; }
    }
}
