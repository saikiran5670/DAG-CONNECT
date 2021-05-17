using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class AlertUpdateRequest
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public string ValidityPeriodType { get; set; }
        public long ValidityStartDate { get; set; }
        public long ValidityEndDate { get; set; }
        public int VehicleGroupId { get; set; }
        public long ModifiedAt { get; set; }
        public int ModifiedBy { get; set; }
    }
}
