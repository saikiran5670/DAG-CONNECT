using System;
using System.ComponentModel.DataAnnotations;
using net.atos.daf.ct2.portalservice.Entity.Report;

namespace net.atos.daf.ct2.portalservice.CustomValidators.Report
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CompareProfileTargetValueAttribute : ValidationAttribute
    {
        private readonly string _limitValueProperty;
        private readonly string _lowerValueProperty;
        private readonly string _upperValueProperty;
        private readonly string _limitTypeProperty;

        public CompareProfileTargetValueAttribute(string limitValueProperty, string lowerValueProperty, string upperValueProperty, string limitTypeProperty)
        {
            _limitValueProperty = limitValueProperty;
            _lowerValueProperty = lowerValueProperty;
            _upperValueProperty = upperValueProperty;
            _limitTypeProperty = limitTypeProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;
            var target = (double)value;

            var limitProperty = validationContext.ObjectType.GetProperty(_limitValueProperty);
            var lowerProperty = validationContext.ObjectType.GetProperty(_lowerValueProperty);
            var upperProperty = validationContext.ObjectType.GetProperty(_upperValueProperty);
            var limitTypeProperty = validationContext.ObjectType.GetProperty(_limitTypeProperty);

            if (limitTypeProperty == null || lowerProperty == null || upperProperty == null || limitTypeProperty == null)
                throw new ArgumentException($"Property with given name not found");

            var limit = (double)limitProperty.GetValue(validationContext.ObjectInstance);
            var lower = (double)lowerProperty.GetValue(validationContext.ObjectInstance);
            var upper = (double)upperProperty.GetValue(validationContext.ObjectInstance);
            var limitType = (char)limitTypeProperty.GetValue(validationContext.ObjectInstance);

            if (target < lower || target > upper)
                return new ValidationResult(ErrorMessage);

            if (limitType == (char)LimitType.Min)
            {
                if (target < limit)
                    return new ValidationResult(ErrorMessage);
            }
            else if (limitType == (char)LimitType.Max)
            {
                if (target > limit)
                    return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
