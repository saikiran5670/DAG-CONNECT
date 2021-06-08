using System.ComponentModel.DataAnnotations;
namespace net.atos.daf.ct2.portalservice.CustomValidators.Alert
{
    public class AlertTimingDetailTypeCheckAttribute : ValidationAttribute
    {
        public AlertTimingDetailTypeCheckAttribute()
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string val = (string)value;
            bool valid = false;
            switch (val)
            {
                case "F":
                case "N":
                case "U":
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