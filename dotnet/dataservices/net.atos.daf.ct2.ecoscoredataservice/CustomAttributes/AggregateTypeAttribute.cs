using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using net.atos.daf.ct2.reports.entity;

namespace net.atos.daf.ct2.ecoscoredataservice.CustomAttributes
{
    public class AggregateTypeAttribute : ValidationAttribute
    {
        public AggregateTypeAttribute()
        : base("Invalid {0}")
        {

        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var values = Enum.GetNames(typeof(AggregateType));

            if (values.Contains(value))
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
