namespace net.atos.daf.ct2.account.entity
{
    public class MenuFeatureDto
    {
        public int FeatureId { get; set; }
        public string FeatureName { get; set; }
        public string FeatureType { get; set; }
        public string FeatureKey { get; set; }
        public int FeatureLevel { get; set; }
        public int? MenuId { get; set; }
        public string MenuName { get; set; }
        public string TranslatedMenuName { get; set; }
        public string ParentMenuName { get; set; }
        public string MenuKey { get; set; }
        public string MenuUrl { get; set; }
        public int? MenuSortId { get; set; }
    }
}
