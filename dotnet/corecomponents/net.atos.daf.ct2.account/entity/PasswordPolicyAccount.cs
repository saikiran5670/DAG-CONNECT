namespace net.atos.daf.ct2.account.entity
{
    public class PasswordPolicyAccount
    {
        public int AccountId { get; set; }
        public int FailedLoginAttempts { get; set; } = 0;
        public long? LockedUntil { get; set; } = null;
        public int AccountLockAttempts { get; set; } = 0;
        public bool IsBlocked { get; set; } = false;
        public long? LastLogin { get; set; } = null;
        public bool IsReminderSent { get; set; }
    }
}
