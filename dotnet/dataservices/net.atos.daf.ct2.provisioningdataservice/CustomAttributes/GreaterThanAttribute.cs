using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace net.atos.daf.ct2.provisioningdataservice.CustomAttributes
{
    public class GreaterThanAttribute : ValidationAttribute
    {
        private readonly string _startTimestampProperty;

        public GreaterThanAttribute(string startTimestampProperty)
        {
            _startTimestampProperty = startTimestampProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (value != null)
                {
                    var endTimestamp = (long)value;

                    var startTimestampProperty = validationContext.ObjectType.GetProperty(_startTimestampProperty);
                    var startTimestamp = startTimestampProperty.GetValue(validationContext.ObjectInstance);

                    if (startTimestamp == null)
                        return ValidationResult.Success;

                    if ((long)startTimestamp < endTimestamp)
                        return ValidationResult.Success;
                    else
                        return new ValidationResult(ErrorMessageString);
                }
                return ValidationResult.Success;
            }
            catch (FormatException)
            {
                return new ValidationResult(ErrorMessageString);
            }
        }
    }
}
