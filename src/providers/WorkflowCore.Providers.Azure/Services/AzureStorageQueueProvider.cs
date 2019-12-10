﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using WorkflowCore.Interface;

namespace WorkflowCore.Providers.Azure.Services
{
    public class AzureStorageQueueProvider : IQueueProvider
    {
        private readonly Dictionary<QueueType, CloudQueue> _queues = new Dictionary<QueueType, CloudQueue>();

        public bool IsDequeueBlocking => false;

        public AzureStorageQueueProvider(string connectionString)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudQueueClient();

            _queues[QueueType.Workflow] = client.GetQueueReference("workflowcore-workflows");
            _queues[QueueType.Event] = client.GetQueueReference("workflowcore-events");
        }

        public async Task QueueWork(string id, QueueType queue)
        {
            var msg = new CloudQueueMessage(id);
            await _queues[queue].AddMessageAsync(msg);
        }

        public async Task<string> DequeueWork(QueueType queue, CancellationToken cancellationToken)
        {
            CloudQueue cloudQueue = _queues[queue];

            if (cloudQueue == null)
                return null;
            
            var msg = await cloudQueue.GetMessageAsync();

            if (msg == null)
                return null;

            await cloudQueue.DeleteMessageAsync(msg);
            return msg.AsString;
        }

        public async Task Start()
        {
            foreach (var queue in _queues.Values)
            {
                await queue.CreateIfNotExistsAsync();
            }
        }

        public Task Stop() => Task.CompletedTask;

        public void Dispose()
        {
        }
    }
}
