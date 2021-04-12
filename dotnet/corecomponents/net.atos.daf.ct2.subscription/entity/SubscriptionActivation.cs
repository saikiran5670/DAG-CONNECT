﻿using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.subscription.entity
{
	public class SubscriptionActivation
	{
		public string OrganizationId { get; set; }//M
		public string packageId { get; set; }//M
		public List<string> VINs { get; set; }
		public long StartDateTime { get; set; }

	}

	public class Package
	{
		public int id { get; set; }
		public string type { get; set; }
		public string packagecode { get; set; }
	}

	public class subscriptionIdType
	{
		public int id { get; set; }
		public string type { get; set; }
	}

	public class subscriptionIdStatus
	{
		public string subscription_id { get; set; }
		public string state { get; set; }
	}
	public class UnSubscribeVin
	{
		public int id { get; set; }
		public string state { get; set; }
	}
	
	public class SubscriptionResponse
	{
		public string orderId { get; set; }//M
		public int numberOfVehicles { get; set; }//M

	}
}
