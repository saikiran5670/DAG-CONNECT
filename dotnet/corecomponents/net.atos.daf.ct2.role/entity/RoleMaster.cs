using System;
using System.Collections.Generic;
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
        public int Createdby { get; set; }
        public int Updatedby { get; set; }
        public bool Is_Active { get; set; }
        public int Feature_set_id { get; set; }
        public int FeaturesCount { get; set; }
        public FeatureSet FeatureSet { get; set; }
        
    }
}