using System;
using net.atos.daf.ct2.account.ENUM;
namespace net.atos.daf.ct2.account.entity
{
    public class ResetPasswordToken
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public Guid ProcessToken { get; set; }
        public ResetTokenStatus Status { get; set; }
        public long? ExpiryAt { get; set; }
        public long? CreatedAt { get; set; }
    }
}
