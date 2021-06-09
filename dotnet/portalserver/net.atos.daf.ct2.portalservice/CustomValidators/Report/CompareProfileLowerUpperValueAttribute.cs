using System;
using System.ComponentModel.DataAnnotations;
using net.atos.daf.ct2.portalservice.Entity.Report;

namespace net.atos.daf.ct2.portalservice.CustomValidators.Report
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CompareProfileLowerUpperValueAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public CompareProfileLowerUpperValueAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;
            var currentValue = (double)value;

            var comparisonProperty = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (comparisonProperty == null)
                throw new ArgumentException($"Property with given name not found");

            var comparisonValue = (double)comparisonProperty.GetValue(validationContext.ObjectInstance);

            if (_comparisonProperty.Equals(nameof(EcoScoreProfileKPI.UpperValue)))
            {
                if (currentValue > comparisonValue)
                    return new ValidationResult(ErrorMessage);
            }
            else if (_comparisonProperty.Equals(nameof(EcoScoreProfileKPI.LowerValue)))
            {
                if (currentValue < comparisonValue)
                    return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
