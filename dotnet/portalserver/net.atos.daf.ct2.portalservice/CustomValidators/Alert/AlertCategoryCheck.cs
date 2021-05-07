using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.CustomValidators.Alert
{
    public class AlertCategoryAttribute: ValidationAttribute
    {
        public AlertCategoryAttribute()
            : base("Invalid alert {0}")
        {

        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string val = (string)value;
            bool valid = false;
            switch (val)
            {
                case "L":
                    valid = true;
                    break;
                case "F":
                    valid = true;
                    break;
                case "R":
                    valid = true;
                    break;
            }

            if (valid)
                return null;

            return new ValidationResult(base.FormatErrorMessage(validationContext.MemberName)
                , new string[] { validationContext.MemberName });
        }
    }

}
