using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Time.Core.Logging;

namespace Time.Core.AzureStorage
{
    #region IAzureQueueEngine
    public interface IAzureQueueEngine
    {
        Task<CloudQueue> GetCloudQueue(string queuename);
        Task<IEnumerable<CloudQueueMessage>> DequeueMessages(string queuename, int count, int visibilityTimeoutInSeconds = 30);
        Task DeleteMessages(string queuename, IList<CloudQueueMessage> messages);
        Task DeleteMessage(string queuename, CloudQueueMessage toDelete);
        Task<CloudQueueMessage> DequeueMessage(string queuename);
        Task<CloudQueueMessage> PeekMessage(string queuename);
        Task InsertMessage(string queuename, string content);
    }
    #endregion

    #region AzureQueueEngine
    public class AzureQueueEngine : IAzureQueueEngine
    {
        #region Private Variables
        // this is an Azure Storage Queue max, it is not configurable or arbitrary
        private readonly int MaxBatchQueueMessages = 32;

        private readonly ILogger _logger;
        private readonly IAzureStorageContext _azureStorageContext;
        #endregion

        #region Initializing
        public AzureQueueEngine(IAzureStorageContext azureTableContext, ILogger logger)
        {
            _logger = logger;
            _azureStorageContext = azureTableContext;
        }
        #endregion

        #region QueueLength
        public async Task<int?> QueueLength(string queuename)
        {
            var cloudQueue = await GetCloudQueue(queuename);
            await cloudQueue.FetchAttributesAsync();

            return cloudQueue.ApproximateMessageCount;
        }
        #endregion

        #region DequeueMessages
        public async Task<IEnumerable<CloudQueueMessage>> DequeueMessages(string queuename, int count, int visibilityTimeoutInSeconds = 30)
        {
            var cloudQueue = await GetCloudQueue(queuename);
            return await cloudQueue.GetMessagesAsync(Math.Min(count, MaxBatchQueueMessages));
        }

        public async Task<CloudQueueMessage> DequeueMessage(string queuename)
        {
            // the message must be deleted before the default processing time of 30 seconds,
            // else other queue processors can see the message
            var cloudQueue = await GetCloudQueue(queuename);
            return await cloudQueue.GetMessageAsync();
        }
        #endregion

        #region DeleteMessages
        public async Task DeleteMessages(string queuename, IList<CloudQueueMessage> messages)
        {
            var cloudQueue = await GetCloudQueue(queuename);

            Parallel.ForEach(messages, async message =>
            {
                await cloudQueue.DeleteMessageAsync(message);
            });
        }

        public async Task DeleteMessage(string queuename, CloudQueueMessage toDelete)
        {
            var cloudQueue = await GetCloudQueue(queuename);
            await cloudQueue.DeleteMessageAsync(toDelete);
        }

        #endregion

        #region PeekMessage
        public async Task<CloudQueueMessage> PeekMessage(string queuename)
        {
            var cloudQueue = await GetCloudQueue(queuename);
            return await cloudQueue.PeekMessageAsync();
        }
        #endregion

        #region InsertMessage
        public async Task InsertMessage(string queuename, string content)
        {
            var cloudQueue = await GetCloudQueue(queuename);

            var cloudQueueMessage = new CloudQueueMessage(content);
            await cloudQueue.AddMessageAsync(cloudQueueMessage);
        }
        #endregion

        #region GetCloudQueue
        public async Task<CloudQueue> GetCloudQueue(string queuename)
        {
            var queueClient = _azureStorageContext.GetCloudQueueClient();
            if (queueClient == null)
            {
                _logger.Log(LogLevel.Error, $"GetCloudQueueClient {queuename} not found");
                throw new AccessViolationException("Could not access cloud queue.");
            }

            var queue = queueClient.GetQueueReference(queuename);
            await queue.CreateIfNotExistsAsync();
            return queue;
        } 
        #endregion
    } 
    #endregion
}