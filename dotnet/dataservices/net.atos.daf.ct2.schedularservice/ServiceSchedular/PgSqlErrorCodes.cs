using System.Collections.Generic;

namespace net.atos.daf.ct2.schedularservice.ServiceSchedular
{
    public static class PgSqlErrorCodes
    {
        public static List<string> connectionErrorCodes = new List<string>() { "08000", "08003", "08006", "08001", "08004", "08007", "08P01" };
        public static List<string> pgSQLErrorCodes = new List<string>() { "P0000", "P0001", "P0002", "P0003", "P0004" };
        public static List<string> sessionTimeoutCodes = new List<string>() { "25P03", "57P05" };
    }
}
