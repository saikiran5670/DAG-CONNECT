using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.CustomValidators.Alert
{
    public class AlertUnitTypeCheckAttribute : ValidationAttribute
    {
        public AlertUnitTypeCheckAttribute()
        {

        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string val = Convert.ToString(value);
            
            bool valid = false;
            
            switch (val)
            {
                case "S": //Square meter: System Unit for Area 
                    valid = true;
                    break;
                case "B"://bit: System unit for Data Transfer Rate
                    valid = true;
                    break;
                case "Y"://byte: System unit for Digital Storage
                    valid = true;
                    break;
                case "J"://Joule: System Unit for Energy
                    valid = true;
                    break;
                case "H"://Hertz: System Unit for Frequency
                    valid = true;
                    break;
                case "K"://Kilometer per liter: System unit for Fuel Economy 
                    valid = true;
                    break;
                case "M"://Meter: System unit for  Speed
                    valid = true;
                    break;
                case "N"://Meter: System unit for Length
                    valid = true;
                    break;
                case "G"://Kilogram: System unit for Mass
                    valid = true;
                    break;
                case "D"://Degree: System unit for Plane Angle
                    valid = true;
                    break;
                case "P"://Pascal: System unit for Pressure
                    valid = true;
                    break;
                case "C"://Celsius: Systemm unit for Temparature
                    valid = true;
                    break;
                case "T"://Second: System unit for Time
                    valid = true;
                    break;
                case "L"://Liter: System unit for Volume
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
