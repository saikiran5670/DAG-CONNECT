using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace net.atos.daf.ct2.accountdataservice.CustomAttributes
{
    public class ValueWithinCheckAttribute : ValidationAttribute
    {
        private readonly string[] _values;

        public ValueWithinCheckAttribute(string[] values)
        : base("Invalid {0}")
        {
            _values = values;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (_values.Contains(((string)value).ToLower()))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(ErrorMessageString);
            }
        }
    }
}
