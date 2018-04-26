using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bellwether.StorageClient
{
	public class BaseStorageQueueClient<T>
	{
		private readonly QueueTypes queueType;
		private readonly string storageConnectionString;
		public BaseStorageQueueClient(QueueTypes queueType, string storageConnectionString)
		{
			this.queueType = queueType;
			this.storageConnectionString = storageConnectionString;
		}
		public async Task AddMessageAsync(T message)
		{
			CloudQueueClient queueClient = GetStorageAccount().CreateCloudQueueClient();
			var requestQueue = queueClient.GetQueueReference(queueType.ToString());
			requestQueue.CreateIfNotExists();

			await requestQueue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(message)));
		}
		private CloudStorageAccount GetStorageAccount()
		{
			return CloudStorageAccount.Parse(storageConnectionString);
		}
	}
}
