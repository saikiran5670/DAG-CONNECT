
namespace net.atos.daf.ct2.subscription.entity
{
    public class SubscriptionDetailsRequest
    {
        public int Organization_id { get; set; }
        public string Type { get; set; }
        public ActiveState State { get; set; }
        public bool Filter { get; set; }
    }

    public enum ActiveState
    {
        None = 0,
        A = 1,
        D = 2
    }
}
