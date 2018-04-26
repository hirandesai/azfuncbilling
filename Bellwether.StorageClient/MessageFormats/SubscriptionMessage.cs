using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.StorageClient.MessageFormats
{
	public class SubscriptionMessage: CustomerMessage
	{
		public string SubscriptionId { get; set; }
	}
}
