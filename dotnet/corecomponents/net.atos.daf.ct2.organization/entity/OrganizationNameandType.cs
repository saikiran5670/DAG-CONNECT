
namespace net.atos.daf.ct2.organization.entity
{
    public class OrganizationNameandID
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class OrganizationByID
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
    }
}
