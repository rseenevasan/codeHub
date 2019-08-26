using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Time.Core.Extensions;
using Time.Core.Logging;

namespace Time.Core.AzureStorage
{
    #region IAzureTableEngine
    public interface IAzureTableEngine<TTableEntity> where TTableEntity : ITableEntity, new()
    {
        Task<TTableEntity> RetrieveEntityAsync(string tablename, string partitionKey, string rowKey);
        Task<CloudTable> GetCloudTableAsync(string tablename);
        Task<IEnumerable<TTableEntity>> RetrieveByPartitionKeyAsync(string tablename, string partitionKey);
        Task<IEnumerable<TTableEntity>> RetrieveByRowKeyAsync(string tablename, string rowKey);
        Task<TTableEntity> InsertOrReplaceAsync(string tablename, TTableEntity entity);
        Task BatchInsertOrReplaceAsync(string tablename, IEnumerable<TTableEntity> entities);
        Task DeleteAsync(string tablename, TTableEntity entity);
        Task DeleteAsync(string tablename, string partitionKey);
        Task DeleteAsync(string tablename, string partitionKey, string rowKey);
        Task BatchDeleteAsync(string tablename, IEnumerable<TTableEntity> entities);
        Task<IEnumerable<TTableEntity>> ExecuteQueryAsync(string tablename, TableQuery<TTableEntity> query);
    }
    #endregion

    #region AzureTableEngine
    public class AzureTableEngine<TTableEntity> : IAzureTableEngine<TTableEntity>
    where TTableEntity : TableEntity, new()
    {

        #region Private Variables
        private const int MaxAzureTableStorageBatch = 100;
        private bool _isFirstTime = true;

        public string TableName { get; protected set; }

        private readonly ILogger _logger;
        private readonly IAzureStorageContext _azureStorageContext;
        #endregion

        #region Initializing
        public AzureTableEngine(IAzureStorageContext azureStorageContext, ILogger logger)
        {
            _logger = logger;
            _azureStorageContext = azureStorageContext;
        }
        #endregion

        #region RetrieveEntityAsync
        public async Task<TTableEntity> RetrieveEntityAsync(string tablename, string partitionKey, string rowKey)
        {
            var table = await GetCloudTableAsync(tablename);
            var operation = TableOperation.Retrieve<TTableEntity>(partitionKey, rowKey);
            var result = await table.ExecuteAsync(operation);
            return result.Result as TTableEntity;
        }
        #endregion

        #region RetrieveByPartitionKeyAsync
        public async Task<IEnumerable<TTableEntity>> RetrieveByPartitionKeyAsync(string tablename, string partitionKey)
        {
            TableQuery<TTableEntity> query;
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                query = new TableQuery<TTableEntity>();
            }
            else
            {
                query = new TableQuery<TTableEntity>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
            }
            return await ExecuteQueryAsync(tablename, query);
        }
        #endregion

        #region RetrieveByRowKeyAsync
        public async Task<IEnumerable<TTableEntity>> RetrieveByRowKeyAsync(string tablename, string rowKey)
        {
            TableQuery<TTableEntity> query;

            if (string.IsNullOrWhiteSpace(rowKey))
            {
                query = new TableQuery<TTableEntity>();
            }
            else
            {
                query = new TableQuery<TTableEntity>()
                    .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey));
            }

            return await ExecuteQueryAsync(tablename, query);
        }
        #endregion

        #region InsertOrReplaceAsync
        public async Task<TTableEntity> InsertOrReplaceAsync(string tablename, TTableEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var table = await GetCloudTableAsync(tablename);

            var operation = TableOperation.InsertOrReplace(entity);

            var result = await table.ExecuteAsync(operation);

            if (result == null)
            {
                _logger.Log(LogLevel.Error, $"AzureTableStore::InsertOrReplaceAsync({entity.PartitionKey} | {entity.RowKey}) Result: {result.HttpStatusCode} | {result.Result.ToString()}");
                return null;
            }
            else
                return (TTableEntity)result.Result;
        }
        #endregion

        #region BatchInsertOrReplaceAsync
        public async Task BatchInsertOrReplaceAsync(string tablename, IEnumerable<TTableEntity> entities)
        {
            if (entities == null)
            {
                _logger.LogInfo($"#BatchInsert error: {tablename}");
                return;
            }

            await InsertMaxLimitAsync(tablename, entities);
        }

        private async Task<IList<TableResult>> InsertMaxLimitAsync<T>(string tableName, IEnumerable<T> entities)
           where T : ITableEntity
        {
            var results = new List<TableResult>();
            var grouped = entities.GroupBy(e => e.PartitionKey);

            var table = await GetCloudTableAsync(tableName);

            foreach (var group in grouped)
            {
                foreach (var chunk in group.Chunk(MaxAzureTableStorageBatch))
                {
                    var insert = new TableBatchOperation();

                    foreach (var entity in chunk)
                    {
                        insert.InsertOrReplace(entity);
                    }

                    var batchResults = await table.ExecuteBatchAsync(insert);
                    results.AddRange(batchResults);
                }
            }

            return results;
        }
        #endregion

        #region DeleteAsync
        public async Task DeleteAsync(string tablename, TTableEntity entity)
        {
            if (entity == null)
            {
                return;
            }

            var table = await GetCloudTableAsync(tablename);

            // There is a known "feature" with Table Storage that has yet to be addressed involving deleting
            // entities.  Essentially, if a Table Entity is converted to a model and then back to an
            // entity, for some reason, it is never found for the delete even though PartitionKey and RowKey
            // values have not changed.  It forces us to retrieve the entity before asking for the delete.
            // This is very inefficient, however, in the case of the dictionary service, deleting rows from
            // table storage will not be frequent and the number of rows will be small.
            var retrievedEntity = await RetrieveEntityAsync(tablename, entity.PartitionKey, entity.RowKey);

            if (retrievedEntity != null)
            {
                var operation = TableOperation.Delete(retrievedEntity);

                await table.ExecuteAsync(operation);
            }
        }         

        public async Task DeleteAsync(string tablename, string partitionKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
                return;

            var results = await RetrieveByPartitionKeyAsync(tablename, partitionKey);
            if (results != null && results.Any())
            {
                results.ToList().ForEach(async x => await DeleteAsync(tablename, x));
            }
        }

        public async Task DeleteAsync(string tablename, string partitionKey, string rowKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey) || string.IsNullOrWhiteSpace(rowKey))
                return;

            var result = await RetrieveEntityAsync(tablename, partitionKey, rowKey);
            if (result != null)
            {
                await DeleteAsync(tablename, result);
            }
        }
        #endregion

        #region BatchDeleteAsync
        public async Task BatchDeleteAsync(string tablename, IEnumerable<TTableEntity> entities)
        {
            if (entities == null)
            {
                return;
            }

            var table = await GetCloudTableAsync(tablename);

            // Azure table storage batch operations are restricted to 100 at a time
            var batchedEntities = entities.Batch(MaxAzureTableStorageBatch);
            foreach (var batch in batchedEntities)
            {
                var batchOperation = new TableBatchOperation();
                foreach (var entity in batch)
                {
                    // There is a known "feature" with Table Storage that has yet to be addressed involving deleting
                    // entities.  Essentially, if a Table Entity is converted to a model and then back to an
                    // entity, for some reason, it is never found for the delete even though PartitionKey and RowKey
                    // values have not changed.  It forces us to retrieve the entity before asking for the delete.
                    // This is very inefficient, however, in the case of the dictionary service, deleting rows from
                    // table storage will not be frequent and the number of rows will be small.
                    var retrievedEntity = await RetrieveEntityAsync(tablename, entity.PartitionKey, entity.RowKey);

                    if (retrievedEntity != null)
                    {
                        batchOperation.Delete(retrievedEntity);
                    }
                }

                if (batchOperation.Count > 0)
                {
                    await table.ExecuteBatchAsync(batchOperation);
                }
            }
        }
        #endregion

        #region ExecuteQueryAsync
        public async Task<IEnumerable<TTableEntity>> ExecuteQueryAsync(string tablename, TableQuery<TTableEntity> query)
        {
            var table = await GetCloudTableAsync(tablename);
            TableQuerySegment<TTableEntity> querySegment = null;
            var returnList = new List<TTableEntity>();

            while (querySegment == null || querySegment.ContinuationToken != null)
            {
                querySegment = await table.ExecuteQuerySegmentedAsync(query, querySegment?.ContinuationToken).ConfigureAwait(false);
                returnList.AddRange(querySegment);
            }

            return returnList;
        }
        #endregion

        #region GetCloudTableAsync
        public async Task<CloudTable> GetCloudTableAsync(string tablename)
        {
            var table = _azureStorageContext.GetCloudTableClient().GetTableReference(tablename);
            if (_isFirstTime)
            {
                var creationResult = await table.CreateIfNotExistsAsync();
                if (creationResult)
                {
                    _logger.Log(LogLevel.Error, $"AzureTableStore::GetCloudTableAsync() TableName: {tablename}, Result: {creationResult}");
                }
                _isFirstTime = false;
            }

            return table;
        } 
        #endregion
    } 
    #endregion
}