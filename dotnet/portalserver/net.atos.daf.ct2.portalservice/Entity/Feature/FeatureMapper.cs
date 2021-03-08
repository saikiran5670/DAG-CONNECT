using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
