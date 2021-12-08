using System.Collections.Generic;

namespace net.atos.daf.ct2.schedularservice.ServiceSchedular
{
    public static class PgSqlErrorCodes
    {
        public static List<string> connectionErrorCodes = new List<string>() { "08000", "08003", "08006", "08001", "08004", "08007", "08P01" };
        public static List<string> pgSQLErrorCodes = new List<string>() { "P0000", "P0001", "P0002", "P0003", "P0004" };
        public static List<string> sessionTimeoutCodes = new List<string>() { "25P03", "57P05" };
        public static List<string> authorizationCodes = new List<string>() { "28000", "28P01" };
        public static List<string> invalidTransactionCodes = new List<string>() { "2D000" };
        public static List<string> transactionRollbackCodes = new List<string>() { "40000", "40001", "40002", "40003", "40P01" };
        public static List<string> insufficientResourcesCodes = new List<string>() { "53000", "53100", "53200", "53300", "53400" };
        public static List<string> programLimitExceededCodes = new List<string>() { "54000", "54001", "54011", "54023" };

    }
}
