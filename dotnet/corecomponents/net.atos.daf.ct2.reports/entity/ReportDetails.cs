
using System.Collections.Generic;

namespace net.atos.daf.ct2.reports.entity
{
    public class UserPreferenceCreateRequest
    {
        public int AccountId { get; set; }
        public int ReportId { get; set; }
        public int OrganizationId { get; set; }
        public long CreatedAt { get; set; }
        public long ModifiedAt { get; set; }

        public List<Atribute> AtributesShowNoShow { get; set; }
    }
    public class Atribute
    {
        public int DataAttributeId { get; set; }
        public char State { get; set; }
        public char Type { get; set; }
        public char? ChartType { get; set; }
        public string ThresholdType { get; set; }
        public double ThresholdValue { get; set; }
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
}
