using System;

namespace net.atos.daf.ct2.subscription.entity
{
	public class UnSubscription
	{
		public string OrganizationID { get; set; }//M
		public string OrderID { get; set; }//M
		public string[] VINs { get; set; }
		public DateTime EndDateTime { get; set; }

	}
}
