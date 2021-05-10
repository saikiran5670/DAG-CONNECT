using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.CustomValidators.Geofence
{
    public class NameAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return new RegularExpressionAttribute(@"^[\s\w\p{L}\-\.]{1,100}$").IsValid(Convert.ToString(value).Trim());
        }
    }
}
