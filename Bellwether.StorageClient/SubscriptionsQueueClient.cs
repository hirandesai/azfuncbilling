using Bellwether.StorageClient.MessageFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.StorageClient
{
	public class SubscriptionsQueueClient : BaseStorageQueueClient<SubscriptionMessage>
	{
		public SubscriptionsQueueClient(string storageConnectionString) : base(QueueTypes.Subscriptions, storageConnectionString)
		{

		}
	}
}
