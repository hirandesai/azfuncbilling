using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.Dal.Entities
{
	public class CspCustomer
	{
		[Key]
		public int Id { get; set; }
		public Guid CustomerId { get; set; }

		public Guid TenantId { get; set; }

		public string Domain { get; set; }

		public string CompanyName { get; set; }

		public string Relationship { get; set; }
	}
}
