using Bellwether.StorageClient.MessageFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.StorageClient
{	
	public class UtilizationQueueClient : BaseStorageQueueClient<UtilizationMessage>
	{
		public UtilizationQueueClient(string storageConnectionString) : base(QueueTypes.Subscription, storageConnectionString)
		{

		}
	}
}
