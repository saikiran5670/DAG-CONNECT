using System;
using System.Globalization;

namespace net.atos.daf.ct2.singlesignonservice.Common
{
    public class Common
    {
        public static bool ValidateFieldLength(int maxlength, string field)
        {
            if (!string.IsNullOrEmpty(field))
            {
                if (field.Trim().Length <= maxlength)
                {
                    return true;
                }

            }
            else if (field == null || field.Trim() == "")
            {
                return true;
            }
            return false;
        }

        public static bool IsValidDate(string dateTime)
        {

            string dateformat = "yyyy-mm-dd";
            bool validDate = (DateTime.TryParseExact(Convert.ToString(dateTime), dateformat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedatetime));
            return validDate;
        }

    }
}
