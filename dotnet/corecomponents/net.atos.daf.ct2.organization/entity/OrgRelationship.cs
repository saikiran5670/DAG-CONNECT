using System;

namespace net.atos.daf.ct2.organization.entity
{
    public class OrgRelationship
    {
         public int Id { get; set; }
         public int OrganizationId  { get; set; }
         public int FeaturesetId   { get; set; }
         public string Name   { get; set; }
         public string Description   { get; set; }
    }
}
