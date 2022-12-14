using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.CustomValidators.Common
{
    public class StateAttribute : ValidationAttribute
    {
        public StateAttribute()
        : base("Invalid {0}")
        {

        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string val = (string)value;
            bool valid = false;
            switch (val)
            {
                case "A":
                    valid = true;
                    break;
                case "I":
                    valid = true;
                    break;
                case "D":
                    valid = true;
                    break;
            }

            if (valid)
                return null;

            return new ValidationResult(base.FormatErrorMessage(validationContext.MemberName)
                , new string[] { validationContext.MemberName });
        }
    }
    //This Validation is for accepting only state A(Enable) and I(Disable)
    public class StateForEnableDisableAttribute : ValidationAttribute
    {
        public StateForEnableDisableAttribute()
        : base("Invalid {0}") { }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            switch (value)
            {
                case "A":
                case "I":
                    return null;
                default:
                    return new ValidationResult(base.FormatErrorMessage(validationContext.MemberName)
               , new string[] { validationContext.MemberName });
            }
        }
    }
}
