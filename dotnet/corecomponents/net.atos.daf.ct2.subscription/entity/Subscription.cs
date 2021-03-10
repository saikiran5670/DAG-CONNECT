using System;

namespace net.atos.daf.ct2.subscription.entity
{
	public class Subscription
	{
		public string OrganizationId { get; set; }//M
		public string packageId { get; set; }//M
		public string[] VINs { get; set; }
		
	}
	public class SubscriptionResponse
	{
		public string orderId { get; set; }//M
		public int numberOfVehicles { get; set; }//M

	}
}
