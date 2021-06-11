using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.CustomValidators.Alert
{
    public class NotificationFreqThreshholdValueCheckAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;
        public NotificationFreqThreshholdValueCheckAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int currentValue = (int)value;
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null)
                throw new ArgumentException("Property with this name not found");
            string comparisonValue = (string)property.GetValue(validationContext.ObjectInstance);
            if (comparisonValue.ToUpper() == "E" && currentValue > 0)
            {
                return ValidationResult.Success;
            }
            else if (comparisonValue.ToUpper() == "T" || comparisonValue.ToUpper() == "O")
            {
                return ValidationResult.Success;
            }
            else
                return new ValidationResult(ErrorMessage);
        }
    }
}
