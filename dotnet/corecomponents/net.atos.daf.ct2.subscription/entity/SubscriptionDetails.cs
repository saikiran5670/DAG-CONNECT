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
	}

}
