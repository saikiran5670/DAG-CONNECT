using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.subscription.entity
{
	public class SubscriptionDetails
	{
		public string subscription_id { get; set; }
		public string type { get; set; }
		public string name { get; set; }
		public string package_code { get; set; }
		public long subscription_start_date { get; set; }
		public long subscription_end_date { get; set; }
		public bool is_active { get; set; }
		public int count { get; set; }
	}
	public class SubscriptionDetailsRequest
	{
		public int organization_id { get; set; }
		public string type { get; set; }
		public StatusType is_active { get; set; }
		
	}
	public enum StatusType
	{
		None = 0,
		True = 1,
		False = 2
	}
}
