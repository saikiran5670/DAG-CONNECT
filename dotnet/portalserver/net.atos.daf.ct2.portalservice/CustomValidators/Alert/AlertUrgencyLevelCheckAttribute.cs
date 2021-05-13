using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.CustomValidators.Alert
{
    public class AlertUrgencyLevelCheckAttribute : ValidationAttribute
    {

        public AlertUrgencyLevelCheckAttribute()
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

            if (valid)
                return null;

            return new ValidationResult(base.FormatErrorMessage(validationContext.MemberName)
                , new string[] { validationContext.MemberName });
        }
    }

}

