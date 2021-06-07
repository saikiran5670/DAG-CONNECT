using System;
using System.ComponentModel.DataAnnotations;
using net.atos.daf.ct2.portalservice.Entity.Report;

namespace net.atos.daf.ct2.portalservice.CustomValidators.Report
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CompareProfileLimitValueAttribute : ValidationAttribute
    {
        private readonly string _targetValueProperty;
        private readonly string _lowerValueProperty;
        private readonly string _upperValueProperty;        
        private readonly string _limitTypeProperty;

        public CompareProfileLimitValueAttribute(string targetValueProperty, string lowerValueProperty, string upperValueProperty, string limitTypeProperty)
        {
            _targetValueProperty = targetValueProperty;
            _lowerValueProperty = lowerValueProperty;
            _upperValueProperty = upperValueProperty;
            _limitTypeProperty = limitTypeProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;
            var limit = (double)value;

            var targetProperty = validationContext.ObjectType.GetProperty(_targetValueProperty);
            var lowerProperty = validationContext.ObjectType.GetProperty(_lowerValueProperty);
            var upperProperty = validationContext.ObjectType.GetProperty(_upperValueProperty);
            var limitTypeProperty = validationContext.ObjectType.GetProperty(_limitTypeProperty);

            if (targetProperty == null || lowerProperty == null || upperProperty == null || limitTypeProperty == null)
                throw new ArgumentException($"Property with given name not found");

            var target = (double)targetProperty.GetValue(validationContext.ObjectInstance);
            var lower = (double)lowerProperty.GetValue(validationContext.ObjectInstance);
            var upper = (double)upperProperty.GetValue(validationContext.ObjectInstance);
            var limitType = (char)limitTypeProperty.GetValue(validationContext.ObjectInstance);

            if(limit < lower || limit > upper)
                return new ValidationResult(ErrorMessage);

            if (limitType == (char)LimitType.Min)
            {
                if (limit > target)
                    return new ValidationResult(ErrorMessage);
            }
            else if (limitType == (char)LimitType.Max)
            {
                if (limit < target)
                    return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
