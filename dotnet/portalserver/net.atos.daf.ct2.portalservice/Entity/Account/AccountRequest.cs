using System;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.Account
{
    public class AccountRequest
    {
        public AccountRequest()
        {
            Type = "P";
        }
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string Type { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int OrganizationId { get; set; }
        public string DriverId { get; set; }
        //public string CreatedAt { get; set; }

    }
    public class AccountResponse
    {
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string Type { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int OrganizationId { get; set; }
        public string DriverId { get; set; }
        public int PreferenceId { get; set; }
        public int BlobId { get; set; }
        public long CreatedAt { get; set; }

    }
    public class AccountDetailRequest
    {
        public int AccountId { get; set; }
        public int OrganizationId { get; set; }
        public int AccountGroupId { get; set; }
        public int VehicleGroupId { get; set; }
        public int RoleId { get; set; }
        public string Name { get; set; }
    }
    //public class AccessRelationshipRequest
    //{
    //    public int Id { get; set; }
    //    public string AccessRelationType { get; set; }
    //    public int AccountGroupId { get; set; }
    //    public int VehicleGroupId { get; set; }
    //}
    //public class AccessRelationshipFilter
    //{
    //    public int AccountId { get; set; }
    //    public int AccountGroupId { get; set; }
    //}
    public class ChangePasswordRequest
    {
        public string EmailId { get; set; }
        public string Password { get; set; }
    }

    public class AccountOrganizationRequest
    {
 
        public int AccountId { get; set; }
        public int OrganizationId { get; set; }
    }

    public class AccountPreference
    {
        public AccountResponse Account { get; set; }
        public AccountPreferenceResponse Preference { get; set; }
        
    }
    public class ResetPasswordInitiateRequest
    {
        [Required]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", ErrorMessage = "The field EmailId must be in proper format.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Minimum length of {0} field is {2} and maximum length is {1} characters.")]
        public string EmailId { get; set; }
    }

    public class ResetPasswordRequest
    {
        [Required]
        [StringLength(36, MinimumLength = 36, ErrorMessage = "The field {0} must be a string with a length of {1} characters.")]
        public string ProcessToken { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class ResetPasswordInvalidateRequest
    {
        [Required]
        [StringLength(36, MinimumLength = 36, ErrorMessage = "The field {0} must be a string with a length of {1} characters.")]
        public string ResetToken { get; set; }
    }

    public class GetResetPasswordTokenStatusRequest
    {
        [Required]
        [StringLength(36, MinimumLength = 36, ErrorMessage = "The field {0} must be a string with a length of {1} characters.")]
        public string ProcessToken { get; set; }
    }

    public class MenuFeatureRequest
    {
        [Required]
        public int AccountId { get; set; }
        [Required]
        public int RoleId { get; set; }
        [Required]
        public int OrganizationId { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 2, ErrorMessage = "Minimum length of {0} field is {2} and maximum length is {1} characters.")]
        public string LanguageCode { get; set; }
    }

    public class TokenSSORequest
    {
        [Required]
        public string Email { get; set; }
    }

    public class OrgSwitchRequest
    {
        [Required]
        public int AccountId { get; set; }
        [Required]
        public int ContextOrgId { get; set; }
        [Required]
        public string LanguageCode { get; set; }
    }

    public class AccountInfoRequest
    {
        [Required]
        public int AccountId { get; set; }
        [Required]
        public int OrgId { get; set; }
        [Required]
        public int RoleId { get; set; }
    }
}
