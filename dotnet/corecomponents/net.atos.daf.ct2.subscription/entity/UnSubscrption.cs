using System;

namespace net.atos.daf.ct2.subscription.entity
{
	public class UnSubscription
	{
		public string serviceSubscriberId { get; set; }//M
		public string orderId { get; set; }//M
		public string[] VINs { get; set; }
	
	}
}
