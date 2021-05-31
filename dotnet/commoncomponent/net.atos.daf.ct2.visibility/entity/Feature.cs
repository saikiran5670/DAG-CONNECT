namespace net.atos.daf.ct2.visibility.entity
{
    public class Feature : DateTimeStamp
    {
        public int RoleFeatureId { get; set; }
        public int RoleFeatureTypeId { get; set; }
        public string FeatureDescription { get; set; }
        public int ParentFeatureId { get; set; }
        public string ParentFeatureName { get; set; }
        public bool IsMenu { get; set; }
        public int SeqNum { get; set; }
        public bool IsRoleFeatureEnabled { get; set; }
        public int FeatureSetID { get; set; }
    }
}
