using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.vehicledataservice.Common
{
    public class Common
    {
        public static bool ValidateFieldLength(int maxlength,string field)
        {
            if (!string.IsNullOrEmpty(field))
            {
                if (field.Trim().Length <= maxlength)
                {
                    return true;
                }

            }
            else if (field == null || field.Trim()=="")
            {
                return true;
            }
            return false;
        }

        public static bool IsValidDate(string dateTime)
        {
            bool validDate = DateTime.TryParse(dateTime, out DateTime ValidDateTime);
            if (validDate)
                return true;
            else
                return false;
        }

    }
}
