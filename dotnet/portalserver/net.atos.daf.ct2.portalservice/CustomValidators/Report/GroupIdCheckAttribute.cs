using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace net.atos.daf.ct2.portalservice.CustomValidators.Report
{
    public class GroupIdCheckAttribute : ValidationAttribute
    {
        public GroupIdCheckAttribute()
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //consider all values are valid.
            bool valid = true;
            var lstGroupId = (List<string>)value;
            foreach (var item in lstGroupId)
            {
                valid = (!string.IsNullOrEmpty(item) && item.Contains("all")) || ((!string.IsNullOrEmpty(item) && !item.Contains("all") && int.TryParse(item, out int groupid)) ? true : false);
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

