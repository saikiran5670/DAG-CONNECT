using System;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.fmsdataservice.customAttributes
{
    public class ValidateDataType : ValidationAttribute
    {
        private readonly string _typeProperty;

        public ValidateDataType(string typeProperty)
        {
            _typeProperty = typeProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (value != null)
                {
                    if (_typeProperty.Equals("int") && int.TryParse(value.ToString(), out _))
                        return ValidationResult.Success;
                    if (_typeProperty.Equals("long") && long.TryParse(value.ToString(), out _))
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
