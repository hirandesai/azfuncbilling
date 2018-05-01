using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.Dal.Entities
{
	public class CspUtilization
	{
		[Key]
		public int Id { get; set; }
		public string CustomerId { get; set; }
		public string SubscriptionId { get; set; }
		public string ResourceGuid { get; set; }
		public string ResourceName { get; set; }
		public string Category { get; set; }
		public string SubCategory { get; set; }
		public string Region { get; set; }
		public DateTime UsageDateUtc { get; set; }
		public decimal Quantity { get; set; }
		public string Unit { get; set; }
	}
}
