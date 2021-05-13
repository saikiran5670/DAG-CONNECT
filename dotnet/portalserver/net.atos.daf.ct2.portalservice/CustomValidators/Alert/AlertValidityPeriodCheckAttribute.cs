using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.CustomValidators.Alert
{
    public class AlertValidityPeriodCheckAttribute : ValidationAttribute
    {

        public AlertValidityPeriodCheckAttribute()
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
                case "C":
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
