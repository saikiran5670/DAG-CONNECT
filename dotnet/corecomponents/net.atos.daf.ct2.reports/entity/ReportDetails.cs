
using System.Collections.Generic;

namespace net.atos.daf.ct2.reports.entity
{
	public class UserPreferenceCreateRequest
	{
		public List<Atribute> AtributesShowNoShow { get; set; }
	}
	public class Atribute
	{
		public int AccountId { get; set; }
		public int ReportId { get; set; }
		public int DataAttributeId { get; set; }
		public char IsExclusive { get; set; }
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
