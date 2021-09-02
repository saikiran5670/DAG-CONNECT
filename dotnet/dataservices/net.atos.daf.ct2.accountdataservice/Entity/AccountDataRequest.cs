using System;
using System.ComponentModel.DataAnnotations;
using net.atos.daf.ct2.accountdataservice.CustomAttributes;

namespace net.atos.daf.ct2.accountdataservice.Entity
{
    public class DriverLookupRequest
    {
        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_PARAMETER")]
        [EmailRegex(ErrorMessage = "INVALID_PARAMETER")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_PARAMETER")]
        [StringLength(19, MinimumLength = 1, ErrorMessage = "INVALID_PARAMETER")]
        public string DriverId { get; set; }
    }

    public class DriverRegisterRequest
    {
        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_FIELD")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "INVALID_FIELD")]
        public string OrganisationId { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_FIELD")]
        [StringLength(19, MinimumLength = 1, ErrorMessage = "INVALID_FIELD")]
        public string DriverId { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_FIELD")]
        public string Authorization { get; set; }
    }

    public class DriverValidateRequest
    {
        [StringLength(100, MinimumLength = 0, ErrorMessage = "INVALID_FIELD")]
        public string OrganisationId { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_FIELD")]
        [StringLength(19, MinimumLength = 1, ErrorMessage = "INVALID_FIELD")]
        public string DriverId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "MISSING_FIELD")]
        public string Authorization { get; set; }
    }

    public class ChangePasswordRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "MISSING_FIELD")]
        public string Authorization { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "MISSING_FIELD")]
        public string NewAuthorization { get; set; }
    }

    public class ResetPasswordRequest
    {
        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_FIELD")]
        [EmailRegex(ErrorMessage = "INVALID_FIELD")]
        public string AccountId { get; set; }
    }

    public class GetPreferencesRequest
    {
        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_FIELD")]
        [EmailRegex(ErrorMessage = "INVALID_FIELD")]
        public string AccountId { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_FIELD")]
        [StringLength(19, MinimumLength = 1, ErrorMessage = "INVALID_FIELD")]
        public string DriverId { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_FIELD")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "INVALID_FIELD")]
        public string OrganisationId { get; set; }
    }

    public class UpdatePreferencesRequest
    {
        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_FIELD")]
        [EmailRegex(ErrorMessage = "INVALID_FIELD")]
        public string AccountId { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "MISSING_FIELD")]
        [StringLength(19, MinimumLength = 1, ErrorMessage = "INVALID_FIELD")]
        public string DriverId { get; set; }

        [Required(ErrorMessage = "MISSING_FIELD")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "INVALID_FIELD")]
        public string Language { get; set; }

        [Required(ErrorMessage = "MISSING_FIELD")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "INVALID_FIELD")]
        public string TimeZone { get; set; }

        [Required(ErrorMessage = "MISSING_FIELD")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "INVALID_FIELD")]
        [ValueWithinCheck(values: new string[] { "dd/mm/yyyy", "mm/dd/yyyy", "dd-mm-yyyy", "mm-dd-yyyy" }, ErrorMessage = "INVALID_FIELD")]
        public string DateFormat { get; set; }

        [Required(ErrorMessage = "MISSING_FIELD")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "INVALID_FIELD")]
        [ValueWithinCheck(values: new string[] { "metric", "imperial" }, ErrorMessage = "INVALID_FIELD")]
        public string UnitDisplay { get; set; }

        [Required(ErrorMessage = "MISSING_FIELD")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "INVALID_FIELD")]
        [ValueWithinCheck(values: new string[] { "vin", "name", "regno" }, ErrorMessage = "INVALID_FIELD")]
        public string VehicleDisplay { get; set; }

        [Required(ErrorMessage = "MISSING_FIELD")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "INVALID_FIELD")]
        [ValueWithinCheck(values: new string[] { "12 hours", "24 hours" }, ErrorMessage = "INVALID_FIELD")]
        public string TimeFormat { get; set; }
    }
}
