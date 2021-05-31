namespace net.atos.daf.ct2.account.entity
{

    public class AccessRelationshipFilter
    {
        public int AccountId { get; set; }
        public int AccountGroupId { get; set; }
        public int VehicleGroupId { get; set; }
    }
    public class AccountVehicleAccessRelationshipFilter
    {
        public int OrganizationId { get; set; }

    }
}
