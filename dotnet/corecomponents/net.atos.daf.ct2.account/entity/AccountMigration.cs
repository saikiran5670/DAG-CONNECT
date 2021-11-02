
namespace net.atos.daf.ct2.account.entity
{
    public class AccountMigration
    {
        public int AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class AccountMigrationResponse
    {
        public string Message { get; set; }
        public int[] FailedAccountIds { get; set; }
    }

    public enum AccountMigrationState
    {
        Pending = 'P',
        Completed = 'C',
        Failed = 'F'
    }
}
