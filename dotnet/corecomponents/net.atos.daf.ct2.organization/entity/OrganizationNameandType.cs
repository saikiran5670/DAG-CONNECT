
namespace net.atos.daf.ct2.organization.entity
{
    public class OrganizationNameandID
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class OrganizationByID
    {
        public int id { get; set; }
        public int roleId { get; set; }
    }
}
