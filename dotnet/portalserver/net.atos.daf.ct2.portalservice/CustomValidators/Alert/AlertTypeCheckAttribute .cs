using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.CustomValidators.Alert
{
    public class AlertTypeCheckAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public AlertTypeCheckAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string currentValue = (string)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            string comparisonValue = (string)property.GetValue(validationContext.ObjectInstance);

            if ((comparisonValue.ToUpper() == "L" &&
                (currentValue.ToUpper() == "C" || currentValue.ToUpper() == "D" || currentValue.ToUpper() == "G" ||
                 currentValue.ToUpper() == "N" || currentValue.ToUpper() == "S" || currentValue.ToUpper() == "U" ||
                 currentValue.ToUpper() == "X" || currentValue.ToUpper() == "Y")
                 )
                ||
                (comparisonValue.ToUpper() == "F" &&
                (currentValue.ToUpper() == "A" || currentValue.ToUpper() == "F" || currentValue.ToUpper() == "H" ||
                 currentValue.ToUpper() == "I" || currentValue.ToUpper() == "L" || currentValue.ToUpper() == "P" ||
                 currentValue.ToUpper() == "T"))
                ||
                (comparisonValue.ToUpper() == "R" &&
                 (currentValue.ToUpper() == "E" || currentValue.ToUpper() == "O"))
               )
            {
                return ValidationResult.Success;
            }
            else
                return new ValidationResult(ErrorMessage);

        }
    }
}
