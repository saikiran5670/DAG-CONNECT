using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class UserPreferenceCreateRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Account Id is required.")]
        public int AccountId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Report Id is required.")]
        public int ReportId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Organization Id is required.")]
        public int OrganizationId { get; set; }
        public long CreatedAt { get; set; }
        public long ModifiedAt { get; set; }
        [StringLength(1, MinimumLength = 1)]
        public string Type { get; set; }
        [StringLength(1, MinimumLength = 1)]
        public string ChartType { get; set; }
        public List<Atribute> AtributesShowNoShow { get; set; }
    }
    public class Atribute
    {
        [Range(1, int.MaxValue, ErrorMessage = "DataAttribute Id is required.")]
        public int DataAttributeId { get; set; }
        [StringLength(1, MinimumLength = 1)]
        public string State { get; set; }
    }
    public class ReportListedParamaters
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string GroupDate { get; set; }
        public string VehicleGroupName { get; set; }
        public string VinName { get; set; }

    }

    public class VehicleGroupResponse
    {
        public List<string> VehicleGroup { get; set; }
    }
    public class VehicleResponse
    {
        public List<string> Vin { get; set; }
    }

    public enum IsExclusive
    {
        None = 0,
        Yes = 'Y',
        No = 'N'
    }
}