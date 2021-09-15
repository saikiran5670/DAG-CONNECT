using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using net.atos.daf.ct2.data;

namespace net.atos.daf.ct2.accountdataservice.CustomAttributes
{
    public class ValueWithinCheckAttribute : ValidationAttribute
    {
        private readonly string[] _values;

        public ValueWithinCheckAttribute(string[] values = null)
        : base("Invalid {0}")
        {
            _values = values;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string[] values;
            var memoryCache = (IMemoryCache)validationContext.GetService(typeof(IMemoryCache));
            var dataAccess = (IDataAccess)validationContext.GetService(typeof(IDataAccess));
            MasterData master = new MasterData(memoryCache, dataAccess);

            switch (validationContext.MemberName)
            {
                case "Language":
                    values = master.GetCachedLanguages().Result;
                    break;
                case "TimeZone":
                    values = master.GetCachedTimeZones().Result;
                    break;
                case "DateFormat":
                    values = master.GetCachedDateFormats().Result;
                    break;
                case "TimeFormat":
                case "UnitDisplay":
                case "VehicleDisplay":
                    values = _values;
                    break;
                default:
                    values = new string[] { };
                    break;
            }

            if (values.Contains(((string)value ?? string.Empty).ToLower()))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(ErrorMessageString);
            }
        }
    }
}
