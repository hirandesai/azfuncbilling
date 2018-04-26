using Bellwether.StorageClient.MessageFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.StorageClient
{
	public class SubscriptionQueueClient: BaseStorageQueueClient<SubscriptionMessage>
	{
		public SubscriptionQueueClient(string storageConnectionString) :base(QueueTypes.Subscription, storageConnectionString)
		{

		}
	}
}
