using System.ComponentModel.DataAnnotations;
namespace net.atos.daf.ct2.portalservice.CustomValidators.Alert
{
    public class NotificationPeriodTypeCheckAttribute : ValidationAttribute
    {
        public NotificationPeriodTypeCheckAttribute()
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string val = (string)value;
            bool valid = false;
            switch (val)
            {
                case "D":
                case "M":
                case "W":
                case "Y":
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