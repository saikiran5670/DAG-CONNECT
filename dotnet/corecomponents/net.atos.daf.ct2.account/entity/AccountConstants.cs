namespace net.atos.daf.ct2.account.entity
{
    public static class AccountConstants
    {
        public const string ERROR_INVALID_GRANT = "invalid_grant";
        public const string ERROR_PWD_EXPIRED = "Account is not fully set up";
        public const string ERROR_RESET_TOKEN_NOTFOUND = "Token not generated, Please contact DAF system Admin";
        public const string ERROR_ACCOUNT_BLOCKED = "Account is blocked, please contact DAF System Admin.";
        public const string ERROR_ACCOUNT_LOCKED_INTERVAL = "Login failed. Please try with proper credentials after {0} minutes.";
        public const string ERROR_ACCOUNT_LOGIN_FAILED = "Login failed. Please try with proper credentials.";
    }
}
