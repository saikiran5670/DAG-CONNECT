using net.atos.daf.ct2.features.entity;

namespace net.atos.daf.ct2.role.entity
{
    public class RoleMaster
    {
        public int? Organization_Id { get; set; }
        //public int RoleMasterId { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public int Created_by { get; set; }
        public long Created_at { get; set; }
        public long Modified_at { get; set; }
        public int Updatedby { get; set; }
        public string State { get; set; }

        public int? Feature_set_id { get; set; }
        public int? Featurescount { get; set; }
        public FeatureSet FeatureSet { get; set; }
        public int Level { get; set; }
        public string Code { get; set; }

    }


    public class AssignedRoles
    {
        public int Roleid { get; set; }
        public int Accountid { get; set; }
        public string Salutation { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}