
using System.Collections.Generic;

namespace net.atos.daf.ct2.report.entity
{
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
