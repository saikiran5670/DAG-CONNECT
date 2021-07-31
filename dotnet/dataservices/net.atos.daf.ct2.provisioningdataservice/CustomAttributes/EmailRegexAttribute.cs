using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace net.atos.daf.ct2.provisioningdataservice.CustomAttributes
{
    public class EmailRegexAttribute : ValidationAttribute
    {
        public EmailRegexAttribute()
        : base("Invalid {0}")
        {

        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var emailString = value is null ? string.Empty : (string)value;
                if (Regex.IsMatch(emailString, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
                    return ValidationResult.Success;
                else
                    return new ValidationResult(ErrorMessageString);
            }
            catch (FormatException)
            {
                return new ValidationResult(ErrorMessageString);
            }
        }
    }
}
