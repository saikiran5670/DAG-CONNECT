using System.ComponentModel.DataAnnotations;
namespace net.atos.daf.ct2.portalservice.CustomValidators.Alert
{
    public class ValidityTypeCheckAttribute : ValidationAttribute
    {
        public ValidityTypeCheckAttribute()
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string val = (string)value;
            bool valid = false;
            switch (val)
            {
                case "A":
                case "C":
                case "E":
                case "W":
                    valid = true;
                    break;
            }
            return valid
                ? null
                : new ValidationResult(base.FormatErrorMessage(validationContext.MemberName)
                , new string[] { validationContext.MemberName });
        }
    }

}

