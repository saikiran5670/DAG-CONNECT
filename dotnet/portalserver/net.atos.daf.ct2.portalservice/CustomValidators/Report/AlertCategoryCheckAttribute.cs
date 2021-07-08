using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.CustomValidators.Report
{
    public class AlertCategoryCheckAttribute : ValidationAttribute
    {
        public AlertCategoryCheckAttribute()
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //consider all values are valid.
            bool valid = true;
            var lstGroupId = (List<string>)value;
            foreach (var item in lstGroupId)
            {
                switch (item.ToUpper())
                {
                    case "A":
                    case "C":
                    case "W":
                    case "ALL":
                        valid = true;
                        break;
                    default:
                        valid = false;
                        break;
                }
                if (!valid)
                {
                    //Invalidate payload if any of item is in invalid format.
                    break;
                }
            }
            return valid
                ? null
                : new ValidationResult(base.FormatErrorMessage(validationContext.MemberName)
                , new string[] { validationContext.MemberName });
        }
    }
}