using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Organization
{
   
        public class OrganizationRelationShipCreate
        {
            public int Id { get; set; }
            public int RelationShipId { get; set; }
            public int[] VehicleGroupId { get; set; }
            public int OwnerOrgId { get; set; }
            public int CreatedOrgId { get; set; }
            public int[] TargetOrgId { get; set; }
            public bool allow_chain { get; set; }
           public bool IsConfirm { get; set; }

    }
    
}
