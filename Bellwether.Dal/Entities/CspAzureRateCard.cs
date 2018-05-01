using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.Dal.Entities
{
	public class CspAzureRateCard
	{
		[Key]
		public int Id { get; set; }
		public string MeterId { get; set; }
		public string MeterName { get; set; }
		public string RateKey { get; set; }
		public string RateValue { get; set; }
		public string Tags { get; set; }
		public string Category { get; set; }
		public string SubCategory { get; set; }
		public string Region { get; set; }
		public string Unit { get; set; }
		public decimal IncludedQuantity { get; set; }
		public DateTime EffectiveDate { get; set; }
	}
}
