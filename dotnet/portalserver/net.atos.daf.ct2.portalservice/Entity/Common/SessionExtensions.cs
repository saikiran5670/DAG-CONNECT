using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.portalservice.Entity.Common
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.Remove(key);
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
