using System.Reflection;
namespace net.atos.daf.ct2.customerdataservice.Common
{

    public class Common
    {
        public static bool AllPropertiesValid(object myObject)
        {
            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(myObject);
                    if (string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}



