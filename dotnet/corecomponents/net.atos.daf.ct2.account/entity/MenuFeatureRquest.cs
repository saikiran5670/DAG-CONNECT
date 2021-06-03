namespace net.atos.daf.ct2.account.entity
{
    public class MenuFeatureRquest
    {
        public int AccountId { get; set; }
        public int RoleId { get; set; }
        public int OrganizationId { get; set; }
        public string LanguageCode { get; set; }
        public int ContextOrgId { get; set; }
    }
}
