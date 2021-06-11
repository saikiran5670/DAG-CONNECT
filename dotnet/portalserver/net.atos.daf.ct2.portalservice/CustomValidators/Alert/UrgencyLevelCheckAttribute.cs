using System.ComponentModel.DataAnnotations;
namespace net.atos.daf.ct2.portalservice.CustomValidators.Alert
{
    public class UrgencyLevelCheckAttribute : ValidationAttribute
    {
        public UrgencyLevelCheckAttribute()
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string val = (string)value;
            bool valid = false;
            switch (val)
            {
                case "C":
                    valid = true;
                    break;
                case "W":
                    valid = true;
                    break;
                case "A":
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

