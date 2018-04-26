using Bellwether.StorageClient.MessageFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.StorageClient
{
	public class CustomersQueueClient: BaseStorageQueueClient<CustomerMessage>
	{
		public CustomersQueueClient(string storageConnectionString) :base(QueueTypes.Customers, storageConnectionString)
		{
			
		}
	}
}
