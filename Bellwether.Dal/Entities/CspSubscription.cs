using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.Dal.Entities
{
	public class CspSubscription
	{
		[Key]
		public int Id { get; set; }
		public string SubscriptionId { get; set; }
		public string CustomerId { get; set; }
		public string OfferId { get; set; }
		public string OfferName { get; set; }
		public string FriendlyName { get; set; }
		public decimal Quantity { get; set; }
		public string UnitType { get; set; }
		public DateTime CreationDateUtc { get; set; }
		public DateTime EffectiveStartDateUtc { get; set; }
		public DateTime CommitmentEndDateUtc { get; set; }
		public string Status { get; set; }
		public bool AutoRenewEnabled { get; set; }
		public bool IsTrial { get; set; }
		public string BillingType { get; set; }
		public string BillingCycle { get; set; }
		public string ContractType { get; set; }
		public string OrderId { get; set; }
	}
}
