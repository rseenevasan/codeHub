using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace Time.Core.AzureStorage
{
    #region IAzureStorageContext
    public interface IAzureStorageContext
    {
        CloudStorageAccount GetCloudStorageAccount();
        CloudTableClient GetCloudTableClient();
        CloudQueueClient GetCloudQueueClient();
        CloudBlobClient GetCloudBlobClient();
    }
    #endregion

    #region AzureStorageContext
    public class AzureStorageContext : IAzureStorageContext
    {
        #region Private Variables
        private readonly CloudStorageAccount _cloudStorageAccount;
        private readonly CloudTableClient _cloudTableClient;
        #endregion

        #region Initializing
        public AzureStorageContext(string tableStorageConnectionString)
        {
            if (string.IsNullOrEmpty(tableStorageConnectionString))
            {
                throw new Exception("Table storage connection string is must");
            }
            _cloudStorageAccount = CloudStorageAccount.Parse(tableStorageConnectionString);
            _cloudTableClient = _cloudStorageAccount.CreateCloudTableClient();
        }
        #endregion

        #region GetCloudStorageAccount
        public CloudStorageAccount GetCloudStorageAccount()
        {
            return _cloudStorageAccount;
        }
        #endregion

        #region GetCloudTableClient
        public CloudTableClient GetCloudTableClient()
        {
            return _cloudTableClient;
        }
        #endregion

        #region GetCloudQueueClient
        public CloudQueueClient GetCloudQueueClient()
        {
            return _cloudStorageAccount.CreateCloudQueueClient();
        }
        #endregion

        #region GetCloudBlobClient
        public CloudBlobClient GetCloudBlobClient()
        {
            return _cloudStorageAccount.CreateCloudBlobClient();
        } 
        #endregion
    } 
    #endregion
}