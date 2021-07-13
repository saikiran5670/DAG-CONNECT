using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace net.atos.daf.ct2.ecoscoredataservice.CustomAttributes
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
                MailAddress m = new MailAddress((string)value);

                return ValidationResult.Success;
            }
            catch (FormatException)
            {
                return new ValidationResult(ErrorMessageString);
            }
        }
    }
}
