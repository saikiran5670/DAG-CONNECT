using System;
using System.ComponentModel.DataAnnotations;

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

            switch (val)
            {
                case "S": //Square meter: System Unit for Area 
                case "B"://bit: System unit for Data Transfer Rate
                case "Y"://byte: System unit for Digital Storage
                case "J"://Joule: System Unit for Energy
                case "H"://Hertz: System Unit for Frequency
                case "K"://Kilometer per liter: System unit for Fuel Economy 
                case "M"://Meter: System unit for  Speed
                case "N"://Meter: System unit for Length
                case "G"://Kilogram: System unit for Mass
                case "D"://Degree: System unit for Plane Angle
                case "P"://Pascal: System unit for Pressure
                case "C"://Celsius: Systemm unit for Temparature
                case "T"://Second: System unit for Time
                case "L"://Liter: System unit for Volume
                    return null;
                default:
                    return new ValidationResult(base.FormatErrorMessage(validationContext.MemberName)
                , new string[] { validationContext.MemberName });
            }
        }
    }
}
