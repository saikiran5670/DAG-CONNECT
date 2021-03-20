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
        public string Password { get; set; }
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
        public string Password { get; set; }
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
        [Required(ErrorMessage = "")]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", ErrorMessage = "")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "")]
        public string EmailId { get; set; }
    }

    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "")]
        [StringLength(36, MinimumLength = 36, ErrorMessage = "")]
        public string ProcessToken { get; set; }
        [Required(ErrorMessage = "")]
        public string Password { get; set; }
    }

    public class ResetPasswordInvalidateRequest
    {
        [Required(ErrorMessage = "")]
        [StringLength(36, MinimumLength = 36, ErrorMessage = "")]
        public string ResetToken { get; set; }
    }

    public class MenuFeatureRequest
    {
        [Required(ErrorMessage = "")]
        public int AccountId { get; set; }
        [Required(ErrorMessage = "")]
        public int RoleId { get; set; }
        [Required(ErrorMessage = "")]
        public int OrganizationId { get; set; }
        [Required(ErrorMessage = "")]
        [StringLength(8, MinimumLength = 5, ErrorMessage = "")]
        public string LanguageCode { get; set; }
    }
}
