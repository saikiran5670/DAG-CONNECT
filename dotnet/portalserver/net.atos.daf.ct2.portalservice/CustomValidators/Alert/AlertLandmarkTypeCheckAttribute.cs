using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.CustomValidators.Alert
{
    public class AlertLandmarkTypeCheckAttribute : ValidationAttribute
    {

        public AlertLandmarkTypeCheckAttribute()
        {

        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string val = (string)value;

            bool valid = false;

            switch (val)
            {
                case "N":
                    valid = true;
                    break;
                case "P":
                    valid = true;
                    break;
                case "C":
                    valid = true;
                    break;
                case "O":
                    valid = true;
                    break;
                case "E":
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

