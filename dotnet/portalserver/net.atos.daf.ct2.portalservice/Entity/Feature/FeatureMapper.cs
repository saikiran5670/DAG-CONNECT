using FeatureBusinessService = net.atos.daf.ct2.featureservice;


namespace net.atos.daf.ct2.portalservice.Entity.Feature
{
    public class FeatureMapper
    {
        public FeatureBusinessService.FetureSetRequest ToFeature(FeatureSet request)
        {
            var featureset = new FeatureBusinessService.FetureSetRequest();
            featureset.FeatureSetID = request.FeatureSetID;
            featureset.Name = request.Name;
            featureset.CreatedBy = request.created_by;
            //featureset.Features = request.Features;
            return featureset;
        }
    }
}
